using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class Rule
    {
        private char characterToBeDecomposed;
        private char? componentA;
        private char? componentB;

        public Rule(char characterToBeDecomposed, string compositionType, char? componentA, char? componentB)
        {
            this.characterToBeDecomposed = characterToBeDecomposed;
            CompositionType = compositionType;
            this.componentA = componentA;
            this.componentB = componentB;
        }

        public char ToBeDecomposed { get; set; }
        public string CompositionType { get; set; }
        public char? ComponentA { get; set; }
        public char? ComponentB { get; set; }
    }
}
