using ExpressionAnalyzer.Models;

namespace ExpressionAnalyzer.Interfaces
{
    public interface IEvaluator
    {
        double Evaluate(AstNode node);
    }
}
