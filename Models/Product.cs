using System;

namespace Stocky.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MinStock { get; set; }
        public int MaxStock { get; set; }
        public int CurrentStock { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {ProductName} - Qty: {CurrentStock} (Min:{MinStock} Max:{MaxStock}) CatId:{CategoryId}";
        }
    }
}
