using Stocky.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Stocky.Services
{
    public class ProductService
    {
        private readonly List<Product> _products = new();
        private int _nextId = 1;
        private readonly CategoryService _categoryService;

        public ProductService(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public List<Product> GetAll() => _products;

        public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);
        
        public Product Register(User user, string productName, string description, int minStock, int maxStock, int currentStock, string location, decimal price, int categoryId)
        {
            if (!user.IsAdmin())
                throw new UnauthorizedAccessException("Only admin can register products.");

            var cat = _categoryService.GetById(categoryId);
            if (cat == null)
                throw new ArgumentException($"Category {categoryId} not found.");

            var p = new Product
            {
                Id = _nextId++,
                ProductName = productName,
                Description = description,
                MinStock = minStock,
                MaxStock = maxStock,
                CurrentStock = currentStock,
                Location = location,
                Price = price,
                CategoryId = categoryId
            };

            _products.Add(p);
            return p;
        }

        public bool Update(User user, int id, string? productName = null, string? description = null, int? minStock = null, int? maxStock = null, int? currentStock = null, string? location = null, decimal? price = null, int? categoryId = null)
        {
            if (!user.IsAdmin())
                throw new UnauthorizedAccessException("Only admin can update products.");

            var p = GetById(id);
            if (p == null) return false;

            if (productName != null) p.ProductName = productName;
            if (description != null) p.Description = description;
            if (minStock.HasValue) p.MinStock = minStock.Value;
            if (maxStock.HasValue) p.MaxStock = maxStock.Value;
            if (currentStock.HasValue) p.CurrentStock = currentStock.Value;
            if (location != null) p.Location = location;
            if (price.HasValue) p.Price = price.Value;
            if (categoryId.HasValue)
            {
                var cat = _categoryService.GetById(categoryId.Value);
                if (cat == null) throw new ArgumentException("Category not found.");
                p.CategoryId = categoryId.Value;
            }

            return true;
        }

        public bool Delete(User user, int id)
        {
            if (!user.IsAdmin())
                throw new UnauthorizedAccessException("Only admin can delete products.");

            var p = GetById(id);
            if (p == null) return false;
            return _products.Remove(p);
        }

        public void IncreaseStock(Product product, int qty) => product.CurrentStock += qty;

        public bool DecreaseStock(Product product, int qty)
        {
            if (qty > product.CurrentStock) return false;
            product.CurrentStock -= qty;
            return true;
        }
    }
}
