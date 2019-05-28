using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP_Growth.DAO
{
    public class AssociationRule
    {
        public IEnumerable<string> Rule_From { get; set; }
        public string Rule_To { get; set; }
        public double Confidence { get; set; }
        public double Support { get; set; }
        public double Lift { get; set; }
        public AssociationRule(IEnumerable<string> rule_from, string rule_to)
        {
            Rule_From = rule_from;
            Rule_To = rule_to;
        }

        public override String ToString()
        {
            return "{(" + string.Join(",", Rule_From) + ") => (" + Rule_To + ") Support: " + Support + "\tConfidence: " + Confidence + "\tLift:" + Lift + "}";
        }
    }
}
