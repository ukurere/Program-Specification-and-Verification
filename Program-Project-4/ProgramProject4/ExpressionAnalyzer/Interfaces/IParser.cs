using ExpressionAnalyzer.Models;

namespace ExpressionAnalyzer.Interfaces
{
    public interface IParser
    {
        AstNode Parse(string expression);
    }
}
