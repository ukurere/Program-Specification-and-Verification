using ExpressionAnalyzer.Interfaces;
using ExpressionAnalyzer.Models;
using Moq;

namespace ExpressionAnalyzer.Tests
{
    public class MockTests
    {
        private readonly Mock<IExpressionSource> _sourceMock = new();
        private readonly Mock<IParser> _parserMock = new();
        private readonly Mock<IEvaluator> _evalMock = new();

        // Сценарій 1: нормальний пайплайн — зчитати → розібрати → обчислити
        [Fact]
        public void Pipeline_ValidExpression_ReturnsCorrectResult()
        {
            var ast = new AstNode(NodeType.Add, new AstNode(3), new AstNode(2));
            _sourceMock.Setup(s => s.ReadExpression()).Returns("3+2");
            _parserMock.Setup(p => p.Parse("3+2")).Returns(ast);
            _evalMock.Setup(e => e.Evaluate(ast)).Returns(5.0);

            var expr = _sourceMock.Object.ReadExpression();
            var tree = _parserMock.Object.Parse(expr);
            var result = _evalMock.Object.Evaluate(tree);

            Assert.Equal("3+2", expr);
            Assert.Equal(NodeType.Add, tree.Type);
            Assert.Equal(5.0, result);

            _sourceMock.Verify(s => s.ReadExpression(), Times.Once);
            _parserMock.Verify(p => p.Parse("3+2"), Times.Once);
            _evalMock.Verify(e => e.Evaluate(ast), Times.Once);
        }

        // Сценарій 2: співставлення параметрів — It.Is<> для рядків що містять "+"
        [Fact]
        public void Parse_WithPlusInExpression_MatchedByParameterMatcher()
        {
            var ast = new AstNode(NodeType.Add, new AstNode(1), new AstNode(2));
            _parserMock
                .Setup(p => p.Parse(It.Is<string>(s => s.Contains('+'))))
                .Returns(ast);

            var result1 = _parserMock.Object.Parse("1+2");
            var result2 = _parserMock.Object.Parse("10+20+30");

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Equal(NodeType.Add, result1.Type);
            Assert.Equal(NodeType.Add, result2.Type);

            _parserMock.Verify(
                p => p.Parse(It.Is<string>(s => s.Contains('+'))),
                Times.Exactly(2));
        }

        // Сценарій 3: виключення — мок кидає FileNotFoundException
        [Fact]
        public void ReadExpression_SourceThrowsFileNotFoundException_ExceptionPropagated()
        {
            _sourceMock
                .Setup(s => s.ReadExpression())
                .Throws(new FileNotFoundException("File not found", "expr.txt"));

            var ex = Assert.Throws<FileNotFoundException>(
                () => _sourceMock.Object.ReadExpression());

            Assert.Equal("expr.txt", ex.FileName);
            Assert.Contains("File not found", ex.Message);

            _sourceMock.Verify(s => s.ReadExpression(), Times.Once);
        }

        // Сценарій 4: різні відповіді на кожен наступний виклик (SetupSequence)
        [Fact]
        public void Evaluate_CalledThreeTimes_ReturnsDifferentResultEachCall()
        {
            var ast = new AstNode(42.0);
            _evalMock
                .SetupSequence(e => e.Evaluate(ast))
                .Returns(11.0)
                .Returns(0.0)
                .Returns(-5.0);

            var r1 = _evalMock.Object.Evaluate(ast);
            var r2 = _evalMock.Object.Evaluate(ast);
            var r3 = _evalMock.Object.Evaluate(ast);

            Assert.Equal(11.0, r1);
            Assert.Equal(0.0, r2);
            Assert.Equal(-5.0, r3);

            _evalMock.Verify(e => e.Evaluate(ast), Times.Exactly(3));
        }

        // Сценарій 5: перевірка порядку та кількості викликів (MockSequence)
        [Fact]
        public void Pipeline_VerifyCallOrderAndExactCount()
        {
            var strictSource = new Mock<IExpressionSource>(MockBehavior.Strict);
            var strictParser = new Mock<IParser>(MockBehavior.Strict);
            var strictEval = new Mock<IEvaluator>(MockBehavior.Strict);

            var ast = new AstNode(NodeType.Multiply, new AstNode(3), new AstNode(4));
            var sequence = new MockSequence();

            strictSource.InSequence(sequence).Setup(s => s.ReadExpression()).Returns("3*4");
            strictParser.InSequence(sequence).Setup(p => p.Parse("3*4")).Returns(ast);
            strictEval.InSequence(sequence).Setup(e => e.Evaluate(ast)).Returns(12.0);

            var expr = strictSource.Object.ReadExpression();
            var tree = strictParser.Object.Parse(expr);
            var result = strictEval.Object.Evaluate(tree);

            Assert.Equal("3*4", expr);
            Assert.Equal(NodeType.Multiply, tree.Type);
            Assert.Equal(12.0, result);

            strictSource.Verify(s => s.ReadExpression(), Times.Once);
            strictParser.Verify(p => p.Parse("3*4"), Times.Once);
            strictEval.Verify(e => e.Evaluate(ast), Times.Once);
        }
    }
}
