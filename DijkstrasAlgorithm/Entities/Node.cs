using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class Node
    {
        public string Label { get; set; }
        public int? CostFromSource { get; set; }
        public Dictionary<string, int> Neighbors { get; set; }
        public Node Parent { get; set; }

        public override int GetHashCode()
        {
            return Label.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Node other = obj as Node;

            if (other != null)
            {
                return this.Label == other.Label;
            }

            return false;
        }
    }
}