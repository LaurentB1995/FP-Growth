using FP_Growth.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP_Growth.Algorithm
{
    public class FPGrowth
    {
        protected List<Transaction> dataSet;
        protected FPTree tree;
        protected double minSupport = 0;
        protected List<ItemSet> patterns;
        public Dictionary<string, int> Supports;

        public FPGrowth(List<Transaction> dataset, double support)
        {
            dataSet = dataset;
            tree = new FPTree();
            patterns = new List<ItemSet>();
            minSupport = support;
        }

        public FPTree GenerateTree(bool isSubTree)
        {
            DateTime start = DateTime.Now;
            List<string> domain = Transaction.ExtractDomain(dataSet);
            Supports = GetInitialSupports(domain, minSupport);



            foreach (Transaction transaction in dataSet)
            {

                int count = 0;
                var orderedTransaction = transaction.items.OrderBy(i => Supports.Keys.ToList().IndexOf(i)).ToList();
                if (orderedTransaction.Count > 0)
                {
                    string item = orderedTransaction.ElementAt(0);
                    while (!Supports.ContainsKey(item))
                    {
                        orderedTransaction.RemoveAt(0);
                        if (orderedTransaction.Count > 0)
                        {
                            item = orderedTransaction.ElementAt(0);
                        }
                        else
                        {
                            break;
                        }

                    }
                    FPNode node = null;
                    while (count < orderedTransaction.Count)
                    {
                        item = orderedTransaction.ElementAt(count);
                        if (count == 0)
                        {
                            if (tree.ContainsNode(item))
                            {
                                node = tree.GetNode(item);
                                node.Count++;
                                //Console.WriteLine("Incremented node {0} to count {1}", node.Name,node.Count);
                            }
                            else
                            {
                                node = tree.AddNode(item);
                                //Console.WriteLine("Attached node {0} to root", node.Name);
                            }
                        }
                        else
                        {
                            if (node.ContainsNode(item))
                            {
                                node = node.GetNode(item);
                                node.Count++;
                                //Console.WriteLine("Incremented node {0} to count {1}", node.Name, node.Count);
                            }
                            else
                            {
                                var old = node;
                                node = node.AddNode(item);
                                //Console.WriteLine("Attached node {0} to {1}", node.Name,old.Name);
                            }

                        }

                        count++;
                    }
                }
            }
            DateTime end = DateTime.Now;
            if (!isSubTree)
            {
                Console.WriteLine("Generated main tree in {0} seconds", (end - start).TotalSeconds);
            }
            return tree;
        }

        protected Dictionary<string, int> GetInitialSupports(List<string> domain, double minSup)
        {
            Supports = new Dictionary<string, int>();
            foreach (string item in domain)
            {
                Supports.Add(item, 0);
            }
            foreach (Transaction transaction in dataSet)
            {
                foreach (string item in transaction.items)
                {
                    Supports[item]++;
                }
            }
            var list = Supports.ToList();
            list.Sort((p1, p2) => p1.Value.CompareTo(p2.Value) * -1);
            Supports = list.ToDictionary((key) => key.Key, (value) => value.Value);
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (var key in Supports.Keys)
            {
                if (Supports[key] >= minSup)
                {
                    result.Add(key, Supports[key]);
                }
                else
                {
                    break;
                }
            }

            Supports = result;
            return result;
        }

        public List<ItemSet> MineFrequentPatterns()
        {
            DateTime start = DateTime.Now;
            var result = tree.MinePatterns(Supports, minSupport);
            DateTime end = DateTime.Now;
            Console.WriteLine("Generated patterns in {0} seconds", (end - start).TotalSeconds);
            return result;
        }

        public List<AssociationRule> GenerateRules(List<ItemSet> patterns, double minSupport, double minConfidence)
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("Generating rules for {0} patterns...", patterns.Count);

            ARuleCalculator calc = new ARuleCalculator(patterns, dataSet.Count);
            List<AssociationRule> rules = new List<AssociationRule>();


            foreach (ItemSet pattern in patterns)
            {

                if (pattern.Items.Count() >= 2)
                {

                    foreach (var item in pattern.Items)
                    {
                        IEnumerable<string> rule_from = pattern.Items.Where(p => !p.Equals(item)).ToList();
                        string rule_to = item;
                        AssociationRule rule = new AssociationRule(rule_from, rule_to);
                        rule.Support = pattern.Support / dataSet.Count;
                        rule.Confidence = calc.GetConfidence(rule.Support, rule.Rule_From);

                        if (rule.Confidence >= minConfidence)
                        {
                            rule.Lift = calc.GetLift(rule.Confidence, rule.Rule_To);
                            if (rule.Lift > 1)
                            {
                                rules.Add(rule);
                            }
                        }

                    }
                }


            }
            DateTime end = DateTime.Now;
            Console.WriteLine("Generated {0} rules in {1} seconds", rules.Count, (end - start).TotalSeconds);
            return rules;
        }
    }
}
