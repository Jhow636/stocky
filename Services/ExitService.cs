using Stocky.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Stocky.Services
{
    public class ExitService
    {
        private readonly List<Exit> _exits = new();
        private int _nextId = 1;
        private readonly ProductService _productService;

        public ExitService(ProductService productService)
        {
            _productService = productService;
        }

        public Exit Register(User user, int productId, int quantity, string description = "")
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var product = _productService.GetById(productId);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            if (!_productService.DecreaseStock(product, quantity))
                throw new InvalidOperationException("Insufficient stock.");

            var s = new Exit
            {
                Id = _nextId++,
                ProductId = productId,
                Quantity = quantity,
                Date = DateTime.Now,
                Description = description,
                RegisteredByUserId = user.Id,
                RegisteredByUserEmail = user.Email
            };

            _exits.Add(s);
            return s;
        }

        public List<Exit> GetAll(User user)
        {
            if (!user.IsAdmin())
                throw new UnauthorizedAccessException("Only admin can view all exits.");
            return _exits.ToList();
        }

        public Exit? GetById(int id, User user)
        {
            if (!user.IsAdmin())
                throw new UnauthorizedAccessException("Only admin can view exits.");
            return _exits.FirstOrDefault(x => x.Id == id);
        }
    }
}
