using FP_Growth.Algorithm;
using FP_Growth.DAO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP_Growth
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            double minSupport = 0.005;
            double minConfidence = 0.5;
            string filepath = @"path/to/file";

            List<Transaction> dataset = PrepareData(filepath);

            FPGrowth method = new FPGrowth(dataset, minSupport * dataset.Count);
            FPTree tree = method.GenerateTree(false);
            var patterns = method.MineFrequentPatterns();
            var rules = method.GenerateRules(patterns, minSupport, minConfidence);
            DateTime end = DateTime.Now;
            Console.WriteLine("Program finished in {0} seconds", (end - start).TotalSeconds);
            patterns.ForEach(p => Console.WriteLine(p));
            Console.WriteLine("==========================================================================");
            rules.Sort((r1, r2) => r2.Confidence.CompareTo(r1.Confidence));
            rules.ForEach(p => Console.WriteLine(p));
            Console.ReadKey();
        }
        private static List<Transaction> PrepareData(string filepath)
        {
            var result = new List<Transaction>();
            System.IO.StreamReader file =
                           new System.IO.StreamReader(filepath);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                result.Add(new Transaction(line.Split(',')));
            }
            return result;
        }
    }
}
