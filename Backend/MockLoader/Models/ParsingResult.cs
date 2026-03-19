
namespace MockLoader.Models
{
    public record ParseResult<T>
    {
        public List<T> Items { get; init; } = [];

        public List<string> Issues { get; init; } = [];
    }
}
