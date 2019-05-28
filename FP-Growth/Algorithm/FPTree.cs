using FP_Growth.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP_Growth.Algorithm
{
    public class FPTree
    {
        public FPNode rootNode;

        public List<ItemSet> patterns;
        public FPTree()
        {
            rootNode = new FPNode("");
        }

        public Boolean ContainsNode(string name)
        {
            return rootNode.leafs.Where(rn => rn.Name.Equals(name)).Count() == 1;
        }

        public FPNode GetNode(string name)
        {
            return rootNode.leafs.Where(rn => rn.Name.Equals(name)).First();
        }

        public FPNode AddNode(string name)
        {
            var node = new FPNode(name);
            rootNode.leafs.Add(node);
            return node;
        }

        public List<ItemSet> MinePatterns(Dictionary<string, int> supports, double support)
        {
            patterns = new List<ItemSet>();
            foreach (var supp in supports) //add all frequent 1 item sets
            {
                patterns.Add(new ItemSet(new string[] { supp.Key }, supp.Value));
            }

            var _supp = supports.Keys.Reverse();
            List<string> usedKeys = new List<string>();
            foreach (string key in _supp)
            {
                usedKeys.Add(key);
                List<ItemSet> freqByKey = new List<ItemSet>();
                MinePatterns(rootNode, key, support, freqByKey);
                Dictionary<string, double> FList = GenerateFList(freqByKey, key, support);
                List<ItemSet> twoItemSets = new List<ItemSet>();
                FList = FList.OrderByDescending(kv => kv.Value).Reverse().ToDictionary(kv => kv.Key, kv => kv.Value);
                foreach (var kvpair in FList)
                {
                    ItemSet its = new ItemSet(new string[] { key, kvpair.Key }, kvpair.Value); //add frequent 2 item sets containing string key
                    twoItemSets.Add(its);
                    patterns.Add(its);
                }

                FPTree subTree = BuildSubTree(freqByKey, key, support);
                foreach (var twoItemSet in twoItemSets) //if more then 1 element in FLISt --> candidate 3+ ItemSet
                {
                    string itskey = twoItemSet.Items.ToList()[1];

                    var itsval = twoItemSet.Support;

                    var subFreqByKey = new List<ItemSet>();
                    MinePatterns(subTree.rootNode, itskey, support, subFreqByKey);
                    var itsFList = GenerateFList(subFreqByKey, itskey, support);

                    var _FList = new Dictionary<string, double>(FList);
                    foreach (string k in usedKeys) //remove combinations where keys are already used so there's no duplicates
                    {
                        if (itsFList.ContainsKey(k))
                        {
                            itsFList.Remove(k);
                        }
                    }
                    MineLonger(twoItemSet.Items, support, itsFList, subFreqByKey); // Mine longer sets containing this 2 item set (recursive)
                }

            }

            return patterns.Where(p => p.Items.Count() > 0).ToList();
        }

        void MineLonger(IEnumerable<string> previous, double minSupport, Dictionary<string, double> subFList, List<ItemSet> SubFreqKey)
        {
            foreach (var kvpair in subFList.Reverse())
            {
                var items = previous.ToList();
                items.Add(kvpair.Key); // add all (n+1)-itemsets containing previous and where n = previous.Count
                ItemSet its = new ItemSet(items, kvpair.Value);
                if (its.Support >= minSupport)
                {
                    patterns.Add(its);

                }
            }
            foreach (var kvpair in subFList.Reverse())
            {
                var items = previous.ToList();
                items.Add(kvpair.Key);
                List<ItemSet> subFreqByKey = new List<ItemSet>();
                FPTree subsubTree = BuildSubTree(SubFreqKey, items[items.Count - 2], minSupport);
                MinePatterns(subsubTree.rootNode, kvpair.Key, minSupport, subFreqByKey);
                var _subFList = GenerateFList(subFreqByKey, kvpair.Key, minSupport); //create new FList for newly added pattern
                MineLonger(items, minSupport, _subFList, subFreqByKey);
            }
        }


        private FPTree BuildSubTree(List<ItemSet> freqByKey, string key, double support)
        {
            List<Transaction> subtreeData = new List<Transaction>();
            foreach (var transaction in freqByKey)
            {
                for (int i = 0; i < transaction.Support; i++)
                {
                    subtreeData.Add(new Transaction(transaction.Items.Where(ti => !ti.Equals(key))));
                }

            }
            FPGrowth method = new FPGrowth(subtreeData, support);
            return method.GenerateTree(true);
        }

        public Dictionary<string, double> GenerateFList(List<ItemSet> freqByKey, string target, double support)
        {
            Dictionary<string, double> FList = new Dictionary<string, double>();
            foreach (var its in freqByKey)
            {
                foreach (string item in its.Items)
                {
                    if (FList.ContainsKey(item))
                    {
                        FList[item] += its.Support;
                    }
                    else
                    {
                        FList.Add(item, its.Support);
                    }
                }
            }
            return FList.Where(kv => kv.Value >= support && !kv.Key.Equals(target)).OrderByDescending(kv => kv.Value).Reverse().ToDictionary(kv => kv.Key, kv => kv.Value);
        }
        public void MinePatterns(FPNode node, string target, double minSupport, List<ItemSet> freqByKey)
        {
            if (node.Name.Equals(target))
            {
                var fis = new ItemSet((node.BuildPath() + target).Split(','), node.Count);
                freqByKey.Add(fis);
                string key = string.Join(",", fis.Items);

            }
            for (int i = 0; i < node.leafs.Count; ++i)
            {
                MinePatterns(node.leafs[i], target, minSupport, freqByKey);
            }
        }
    }
}
