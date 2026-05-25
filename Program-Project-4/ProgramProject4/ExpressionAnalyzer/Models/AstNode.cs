namespace ExpressionAnalyzer.Models
{
    public enum NodeType { Number, Add, Subtract, Multiply, Divide, Negate }

    public class AstNode
    {
        public NodeType Type { get; }
        public double Value { get; }
        public AstNode? Left { get; }
        public AstNode? Right { get; }

        public AstNode(double value)
        {
            Type = NodeType.Number;
            Value = value;
        }

        public AstNode(NodeType type, AstNode left, AstNode right)
        {
            Type = type;
            Left = left;
            Right = right;
        }

        public AstNode(NodeType type, AstNode operand)
        {
            Type = type;
            Left = operand;
        }
    }
}
