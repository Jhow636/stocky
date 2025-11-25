namespace Stocky.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public override string ToString() => $"[{Id}] {CategoryName}";
    }
}
