using Stocky.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Stocky.Services
{
    public class EntryService
    {
        private readonly List<Entry> _entries = new();
        private int _nextId = 1;
        private readonly ProductService _productService;

        public EntryService(ProductService productService)
        {
            _productService = productService;
        }

        public Entry Register(User user, int productId, int quantity, string description = "")
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var product = _productService.GetById(productId);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            _productService.IncreaseStock(product, quantity);

            var e = new Entry
            {
                Id = _nextId++,
                ProductId = productId,
                Quantity = quantity,
                Date = DateTime.Now,
                Description = description,
                RegisteredByUserId = user.Id,
                RegisteredByUserEmail = user.Email
            };

            _entries.Add(e);
            return e;
        }

        public List<Entry> GetAll(User user)
        {
            if (!user.IsAdmin())
                throw new UnauthorizedAccessException("Only admin can view all entries.");
            return _entries.ToList();
        }

        public Entry? GetById(int id, User user)
        {
            if (!user.IsAdmin())
                throw new UnauthorizedAccessException("Only admin can view entries.");
            return _entries.FirstOrDefault(x => x.Id == id);
        }
    }
}
