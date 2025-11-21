using Stocky.Models;

namespace Stocky.Services
{
    public class InventoryService
    {
        private readonly List<Product> _products = new();
        private int _nextId = 1;

        public List<Product> GetAll() => _products;

        public Product? GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public Product AddProduct(string name, int initialQty)
        {
            var product = new Product
            {
                Id = _nextId++,
                Name = name,
                Quantity = initialQty
            };

            _products.Add(product);
            return product;
        }

        public void IncreaseStock(Product product, int quantity)
        {
            product.Quantity += quantity;
        }

        public bool DecreaseStock(Product product, int quantity)
        {
            if (quantity > product.Quantity)
                return false;

            product.Quantity -= quantity;
            return true;
        }
    }
}
