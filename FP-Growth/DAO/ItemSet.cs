using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP_Growth.DAO
{
    public class ItemSet
    {
        public IEnumerable<string> Items { get; set; }
        public double Support { get; set; }

        public ItemSet(string item, double support) : this(new List<string>(new string[] { item }), support)
        {
        }

        public ItemSet(IEnumerable<string> items, double support)
        {
            this.Items = items;
            this.Support = support;
        }

        public override string ToString()
        {
            // return "{ " + string.Join(", ", Items) + " } with support "+this.Support;
            return string.Join("||", Items) + ":" + this.Support;
        }
    }
}
