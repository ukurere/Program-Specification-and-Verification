namespace ExpressionAnalyzer.Models
{
    public enum TokenType { Number, Plus, Minus, Star, Slash, LParen, RParen, End }

    public class Token
    {
        public TokenType Type { get; }
        public double Value { get; }

        public Token(TokenType type, double value = 0)
        {
            Type = type;
            Value = value;
        }

        public override string ToString() =>
            Type == TokenType.Number ? $"NUM({Value})" : Type.ToString();
    }
}
