namespace Stocky.Models
{
    public class Movement
    {
        public DateTime Date { get; set; }
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }

        public string Type { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty;
    }
}
