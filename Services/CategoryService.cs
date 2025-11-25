using Stocky.Models;
using System.Collections.Generic;
using System.Linq;

namespace Stocky.Services
{
    public class CategoryService
    {
        private readonly List<Category> _categories = new();
        private int _nextId = 1;

        public List<Category> GetAll() => _categories;

        public Category? GetById(int id) => _categories.FirstOrDefault(c => c.Id == id);

        public Category Register(string name)
        {
            var cat = new Category { Id = _nextId++, CategoryName = name };
            _categories.Add(cat);
            return cat;
        }

        public bool Update(int id, string name)
        {
            var c = GetById(id);
            if (c == null) return false;
            c.CategoryName = name;
            return true;
        }

        public bool Delete(int id)
        {
            var c = GetById(id);
            if (c == null) return false;
            return _categories.Remove(c);
        }
    }
}
