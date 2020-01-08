namespace WpfApp2.Models
{
    public class Rule
    {
        private readonly char characterToBeDecomposed;
        private readonly char? componentA;
        private readonly char? componentB;

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
