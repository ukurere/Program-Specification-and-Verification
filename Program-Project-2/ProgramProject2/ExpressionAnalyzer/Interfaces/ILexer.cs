using ExpressionAnalyzer.Models;

namespace ExpressionAnalyzer.Interfaces
{
    public interface ILexer
    {
        IReadOnlyList<Token> Tokenize(string expression);
    }
}
