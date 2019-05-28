using FP_Growth.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP_Growth.Algorithm
{
    public class ARuleCalculator
    {
        public List<ItemSet> Patterns { get; set; }
        public int DataSetCount { get; set; }
        public double GetConfidence(double support, IEnumerable<string> rule_from)
        {

            return (double)support / GetSupport(rule_from);
        }
        public ARuleCalculator(IEnumerable<ItemSet> patterns, int dataSetCount)
        {
            Patterns = patterns.ToList();
            DataSetCount = dataSetCount;
        }

        public double GetLift(double confidence, string rule_to)
        {
            return confidence / GetSupport(new string[] { rule_to });
        }

        public double GetSupport(IEnumerable<string> itemset)
        {
            foreach (var pattern in Patterns)
            {
                if (Enumerable.SequenceEqual(pattern.Items.OrderBy(i => i), itemset.OrderBy(i => i)))
                {
                    return pattern.Support / DataSetCount;
                }
            }

            return -1;
        }
    }
}
