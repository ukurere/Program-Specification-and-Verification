using System.Globalization;
using ExpressionAnalyzer.Interfaces;
using ExpressionAnalyzer.Models;

namespace ExpressionAnalyzer
{
    public class Lexer : ILexer
    {
        public IReadOnlyList<Token> Tokenize(string expression)
        {
            var tokens = new List<Token>();
            int i = 0;

            while (i < expression.Length)
            {
                char c = expression[i];

                if (char.IsWhiteSpace(c)) { i++; continue; }

                if (char.IsDigit(c) || c == '.')
                {
                    int start = i;
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                        i++;
                    double val = double.Parse(expression[start..i], CultureInfo.InvariantCulture);
                    tokens.Add(new Token(TokenType.Number, val));
                    continue;
                }

                var type = c switch
                {
                    '+' => TokenType.Plus,
                    '-' => TokenType.Minus,
                    '*' => TokenType.Star,
                    '/' => TokenType.Slash,
                    '(' => TokenType.LParen,
                    ')' => TokenType.RParen,
                    _ => throw new InvalidOperationException($"Unknown character '{c}'")
                };
                tokens.Add(new Token(type));
                i++;
            }

            tokens.Add(new Token(TokenType.End));
            return tokens;
        }
    }
}
