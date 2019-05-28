using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP_Growth.Algorithm
{
    public class FPNode
    {
        public List<FPNode> leafs;
        public FPNode Parent { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }

        public FPNode(string name) : this(name, null)
        {
        }
        public FPNode()
        {
            leafs = new List<FPNode>();
            this.Parent = null;

        }
        public FPNode(string name, FPNode parent)
        {
            leafs = new List<FPNode>();
            this.Parent = parent;
            Name = name;
            Count = 1;
        }

        public Boolean ContainsNode(string name)
        {
            return leafs.Where(l => l.Name.Equals(name)).Count() == 1;
        }
        public FPNode GetNode(string name)
        {
            return leafs.Where(l => l.Name.Equals(name)).First();
        }
        public FPNode AddNode(string name)
        {
            var node = new FPNode(name, this);
            leafs.Add(node);
            return node;
        }

        public string BuildPath()
        {
            StringBuilder sb = new StringBuilder("");
            FPNode x = this;
            while (x.Parent != null)
            {
                sb.Append(x.Parent.Name).Append(",");
                x = x.Parent;
            }
            if (sb.Length > 0)
            {
                sb = sb.Remove(sb.Length - 1, 1);
                var arr = sb.ToString().Split(',');
                Array.Reverse(arr);
                return string.Join(",", arr) + ",";
            }
            return sb.ToString();

        }
    }
}
