using ExpressionAnalyzer.Interfaces;
using ExpressionAnalyzer.Models;

namespace ExpressionAnalyzer
{
    public class Parser : IParser
    {
        private readonly ILexer _lexer;
        private IReadOnlyList<Token> _tokens = [];
        private int _pos;

        public Parser(ILexer lexer)
        {
            _lexer = lexer;
        }

        public AstNode Parse(string expression)
        {
            _tokens = _lexer.Tokenize(expression);
            _pos = 0;
            var result = ParseExpr();
            if (Current.Type != TokenType.End)
                throw new FormatException($"Unexpected token: {Current}");
            return result;
        }

        private Token Current => _tokens[_pos];

        private Token Consume() => _tokens[_pos++];

        private AstNode ParseExpr()
        {
            var left = ParseTerm();
            while (Current.Type is TokenType.Plus or TokenType.Minus)
            {
                var op = Consume();
                var right = ParseTerm();
                left = new AstNode(op.Type == TokenType.Plus ? NodeType.Add : NodeType.Subtract, left, right);
            }
            return left;
        }

        private AstNode ParseTerm()
        {
            var left = ParseUnary();
            while (Current.Type is TokenType.Star or TokenType.Slash)
            {
                var op = Consume();
                var right = ParseUnary();
                left = new AstNode(op.Type == TokenType.Star ? NodeType.Multiply : NodeType.Divide, left, right);
            }
            return left;
        }

        private AstNode ParseUnary()
        {
            if (Current.Type == TokenType.Minus)
            {
                Consume();
                return new AstNode(NodeType.Negate, ParseUnary());
            }
            return ParsePrimary();
        }

        private AstNode ParsePrimary()
        {
            if (Current.Type == TokenType.Number)
            {
                var val = Current.Value;
                Consume();
                return new AstNode(val);
            }
            if (Current.Type == TokenType.LParen)
            {
                Consume();
                var node = ParseExpr();
                if (Current.Type != TokenType.RParen)
                    throw new FormatException("Expected ')'");
                Consume();
                return node;
            }
            throw new FormatException($"Unexpected token: {Current}");
        }
    }
}
