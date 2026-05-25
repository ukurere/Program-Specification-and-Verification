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

                if (IsNumberStart(c))
                {
                    tokens.Add(ReadNumber(expression, ref i));
                    continue;
                }

                tokens.Add(new Token(MapCharToTokenType(c)));
                i++;
            }

            tokens.Add(new Token(TokenType.End));
            return tokens;
        }

        private static bool IsNumberStart(char c) => char.IsDigit(c) || c == '.';

        private static Token ReadNumber(string expression, ref int i)
        {
            int start = i;
            while (i < expression.Length && IsNumberStart(expression[i]))
                i++;
            double val = double.Parse(expression[start..i], CultureInfo.InvariantCulture);
            return new Token(TokenType.Number, val);
        }

        private static TokenType MapCharToTokenType(char c) => c switch
        {
            '+' => TokenType.Plus,
            '-' => TokenType.Minus,
            '*' => TokenType.Star,
            '/' => TokenType.Slash,
            '(' => TokenType.LParen,
            ')' => TokenType.RParen,
            _ => throw new InvalidOperationException($"Unknown character '{c}'")
        };
    }
}
