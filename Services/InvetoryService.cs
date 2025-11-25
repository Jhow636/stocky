using Stocky.Models;
using System.Collections.Generic;
using System.Linq;

namespace Stocky.Services
{
    public class InvetoryService
    {
        private readonly List<Product> _products = new();
        private int _nextId = 1;

        public List<Product> GetAll() => _products;

        public Product? GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public Product AddProduct(string productName, int currentStock)
        {
            var product = new Product
            {
                Id = _nextId++,
                ProductName = productName,
                CurrentStock = currentStock,
                Description = string.Empty,
                MinStock = 0,
                MaxStock = 0,
                Location = string.Empty,
                Price = 0m,
                CategoryId = 0
            };

            _products.Add(product);
            return product;
        }

        public void IncreaseStock(Product product, int quantity)
        {
            product.CurrentStock += quantity;
        }

        public bool DecreaseStock(Product product, int quantity)
        {
            if (quantity > product.CurrentStock)
                return false;

            product.CurrentStock -= quantity;
            return true;
        }
    }
}
