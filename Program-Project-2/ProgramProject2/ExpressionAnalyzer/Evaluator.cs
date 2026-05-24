using ExpressionAnalyzer.Interfaces;
using ExpressionAnalyzer.Models;

namespace ExpressionAnalyzer
{
    public class Evaluator : IEvaluator
    {
        public double Evaluate(AstNode node)
        {
            switch (node.Type)
            {
                case NodeType.Number:
                    return node.Value;
                case NodeType.Add:
                    return Evaluate(node.Left!) + Evaluate(node.Right!);
                case NodeType.Subtract:
                    return Evaluate(node.Left!) - Evaluate(node.Right!);
                case NodeType.Multiply:
                    return Evaluate(node.Left!) * Evaluate(node.Right!);
                case NodeType.Divide:
                    double divisor = Evaluate(node.Right!);
                    if (divisor == 0)
                        throw new DivideByZeroException("Division by zero");
                    return Evaluate(node.Left!) / divisor;
                case NodeType.Negate:
                    return -Evaluate(node.Left!);
                default:
                    throw new ArgumentException($"Unknown node type: {node.Type}");
            }
        }
    }
}
