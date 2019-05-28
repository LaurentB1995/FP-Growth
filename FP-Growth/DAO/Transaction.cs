using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP_Growth.DAO
{
    public class Transaction
    {
        public List<string> items;

        public Transaction(IEnumerable<string> items)
        {
            this.items = new List<string>(items);
        }
        public static List<string> ExtractDomain(IEnumerable<Transaction> transactions)
        {
            HashSet<string> uniqueItems = new HashSet<string>();
            foreach (Transaction transaction in transactions)
            {
                for (int i = 0; i < transaction.items.Count; ++i)
                {
                    uniqueItems.Add(transaction.items[i]);
                }
            }

            return uniqueItems.ToList();
        }

        public Boolean Contains(string item)
        {
            return items.Contains(item);
        }

        public override string ToString()
        {
            return string.Join(",", items);
        }
    }
}
