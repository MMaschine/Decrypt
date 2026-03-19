using Esprima.Ast;

namespace MockLoader.Models
{
    public class TopLevelArraysResult
    {
        public Dictionary<string, ArrayExpression> Arrays { get; } = new(StringComparer.Ordinal);

        public List<string> Issues { get; } = [];

        public bool IsSuccess => Issues.Count == 0;
    }
}
