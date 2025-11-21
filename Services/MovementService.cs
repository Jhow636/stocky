using Stocky.Models;

namespace Stocky.Services
{
    public class MovementService
    {
        private readonly List<Movement> _movements = new();

        public void AddMovement(int productId, string name, int qty, string type, string? description)
        {
            _movements.Add(new Movement
            {
                Date = DateTime.Now,
                ProductId = productId,
                ProductName = name ?? string.Empty,
                Quantity = qty,
                Type = type,
                Description = description ?? ""
            });
        }

        public List<Movement> GetAll() => _movements;
    }
}
