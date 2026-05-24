using ExpressionAnalyzer.Interfaces;

namespace ExpressionAnalyzer
{
    public class FileExpressionSource : IExpressionSource
    {
        private readonly string _path;

        public FileExpressionSource(string path)
        {
            _path = path;
        }

        public string ReadExpression()
        {
            if (!File.Exists(_path))
                throw new FileNotFoundException($"Expression file not found: {_path}", _path);
            return File.ReadAllText(_path).Trim();
        }
    }
}
