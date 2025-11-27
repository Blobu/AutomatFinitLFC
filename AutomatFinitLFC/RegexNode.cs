using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatFinitLFC
{
    public class RegexNode
    {
        public char Value { get; set; }
        public RegexNode Left { get; set; }
        public RegexNode Right { get; set; }

        public RegexNode(char value, RegexNode left = null, RegexNode right = null)
        {
            Value = value;
            Left = left;
            Right = right;
        }

    }
}
