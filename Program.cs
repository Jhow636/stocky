using Stocky.Models;
using Stocky.Services;
using Stocky.Utils;

class Program
{
    static CategoryService categoryService = new CategoryService();
    static ProductService productService = new ProductService(categoryService);
    static EntryService entryService = new EntryService(productService);
    static ExitService exitService = new ExitService(productService);
    static List<User> users = new List<User>();
    static User? currentUser;

    static void Main()
    {
        SeedDemoData();

        bool running = true;
        while (running)
        {
            Console.WriteLine("\n===== STOCKY - SIMPLE CONSOLE =====");
            Console.WriteLine($"Logged in as: {(currentUser != null ? $"{currentUser.Email} ({currentUser.Role})" : "No user")}");
            Console.WriteLine("1 - Login / Switch user");
            Console.WriteLine("2 - Register new user (normal)");
            Console.WriteLine("3 - Register product (admin only)");
            Console.WriteLine("4 - Update product (admin only)");
            Console.WriteLine("5 - Delete product (admin only)");
            Console.WriteLine("6 - List products");
            Console.WriteLine("7 - Register stock ENTRY (any user)");
            Console.WriteLine("8 - Register stock EXIT (any user)");
            Console.WriteLine("9 - View movements (admin only)");
            Console.WriteLine("10 - Manage categories (admin only)");
            Console.WriteLine("0 - Exit");
            Console.Write("Choose an option: ");

            var opt = Console.ReadLine();
            try
            {
                switch (opt)
                {
                    case "1": Login(); break;
                    case "2": CreateUser(); break;
                    case "3": RegisterProduct(); break;
                    case "4": UpdateProduct(); break;
                    case "5": DeleteProduct(); break;
                    case "6": ListProducts(); break;
                    case "7": RegisterEntry(); break;
                    case "8": RegisterExit(); break;
                    case "9": ViewMovements(); break;
                    case "10": ManageCategories(); break;
                    case "0": running = false; break;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
            catch (UnauthorizedAccessException uex)
            {
                Console.WriteLine($"Permission denied: {uex.Message}");
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine($"Invalid data: {aex.Message}");
            }
            catch (KeyNotFoundException knf)
            {
                Console.WriteLine($"Not found: {knf.Message}");
            }
            catch (InvalidOperationException ioex)
            {
                Console.WriteLine($"Operation failed: {ioex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        Console.WriteLine("Bye!");
    }

    static void SeedDemoData()
    {
        var defaultCat = categoryService.Register("General");
        var toolsCat = categoryService.Register("Tools");

        var admin = new User { Id = 1, Email = "admin@company", Password = "admin", Role = Role.Admin };
        var user = new User { Id = 2, Email = "user@company", Password = "user", Role = Role.User };
        users.Add(admin);
        users.Add(user);
        currentUser = admin;

        productService.Register(admin, "Screwdriver", "Flat head screwdriver", 1, 100, 20, "A1", 7.50m, defaultCat.Id);
        productService.Register(admin, "Hammer", "Claw hammer", 1, 50, 10, "A2", 12.00m, toolsCat.Id);
    }

    static void Login()
    {
        Console.WriteLine("\n--- Login / Switch user ---");
        Console.WriteLine("Available users:");
        foreach (var u in users) Console.WriteLine($"{u.Id} - {u.Email} ({u.Role})");

        int id = InputReader.ReadInt("Enter user Id to login as (or 0 to cancel): ");
        if (id == 0) return;

        var ufound = users.FirstOrDefault(u => u.Id == id);
        if (ufound == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        currentUser = ufound;
        Console.WriteLine($"Logged in as: {currentUser.Email} ({currentUser.Role})");
    }

    static void CreateUser()
    {
        Console.WriteLine("\n--- Create new user (normal) ---");
        string email = InputReader.ReadString("Email: ");
        string password = InputReader.ReadString("Password: ");
        int nextId = users.Any() ? users.Max(u => u.Id) + 1 : 1;

        var u = new User { Id = nextId, Email = email, Password = password, Role = Role.User };
        users.Add(u);
        Console.WriteLine($"User created with Id {u.Id}. You can now Login as this user.");
    }

    static void RegisterProduct()
    {
        EnsureLoggedIn();
        EnsureAdmin();

        Console.WriteLine("\n--- Register product (admin) ---");
        string name = InputReader.ReadString("Product name: ");
        string description = InputReader.ReadString("Description: ");
        int minStock = InputReader.ReadInt("Minimum stock: ");
        int maxStock = InputReader.ReadInt("Maximum stock: ");
        int initial = InputReader.ReadInt("Initial stock: ");
        string location = InputReader.ReadString("Location: ");
        decimal price = InputReader.ReadDecimal("Unit price: ");

        Console.WriteLine("Available categories:");
        foreach (var c in categoryService.GetAll()) Console.WriteLine($"{c.Id} - {c.CategoryName}");
        int catId = InputReader.ReadInt("Category Id (or 0 to create new): ");
        if (catId == 0)
        {
            string newName = InputReader.ReadString("New category name: ");
            var newCat = categoryService.Register(newName);
            catId = newCat.Id;
            Console.WriteLine($"Created category {newCat.Id} - {newCat.CategoryName}");
        }

        var p = productService.Register(currentUser!, name, description, minStock, maxStock, initial, location, price, catId);
        Console.WriteLine($"Product registered: {p}");
    }

    static void UpdateProduct()
    {
        EnsureLoggedIn();
        EnsureAdmin();

        Console.WriteLine("\n--- Update product (admin) ---");
        ListProducts();
        int id = InputReader.ReadInt("Product Id to update: ");
        var p = productService.GetById(id);
        if (p == null) { Console.WriteLine("Product not found."); return; }

        Console.WriteLine("Press Enter to keep current value.");
        string productName = InputReader.ReadString($"Name ({p.ProductName}): ");
        string description = InputReader.ReadString($"Description ({p.Description}): ");
        int minStock = InputReader.ReadInt($"Min stock ({p.MinStock}): ");
        int maxStock = InputReader.ReadInt($"Max stock ({p.MaxStock}): ");
        int currentStock = InputReader.ReadInt($"Current stock ({p.CurrentStock}): ");
        string location = InputReader.ReadString($"Location ({p.Location}): ");
        decimal price = InputReader.ReadDecimal($"Price ({p.Price}): ");

        Console.WriteLine("Available categories:");
        foreach (var c in categoryService.GetAll()) Console.WriteLine($"{c.Id} - {c.CategoryName}");
        int catId = InputReader.ReadInt($"Category Id ({p.CategoryId}): ");

        productService.Update(currentUser!, id,
            productName == "" ? null : productName,
            description == "" ? null : description,
            minStock, maxStock, currentStock,
            location == "" ? null : location,
            price, catId);
        Console.WriteLine("Product updated.");
    }

    static void DeleteProduct()
    {
        EnsureLoggedIn();
        EnsureAdmin();

        Console.WriteLine("\n--- Delete product (admin) ---");
        ListProducts();
        int id = InputReader.ReadInt("Product Id to delete: ");
        bool ok = productService.Delete(currentUser!, id);
        Console.WriteLine(ok ? "Product deleted." : "Product not found or could not be deleted.");
    }

    static void ListProducts()
    {
        var products = productService.GetAll();
        if (!products.Any())
        {
            Console.WriteLine("No products available.");
            return;
        }

        Console.WriteLine("\nID\tName\tStock\tLocation\tPrice\tCatId");
        foreach (var p in products)
            Console.WriteLine($"{p.Id}\t{p.ProductName}\t{p.CurrentStock}\t{p.Location}\t{p.Price:C}\t{p.CategoryId}");
    }

    static void RegisterEntry()
    {
        EnsureLoggedIn();

        Console.WriteLine("\n--- Register ENTRY ---");
        ListProducts();
        int pid = InputReader.ReadInt("Product ID: ");
        var p = productService.GetById(pid);
        if (p == null) { Console.WriteLine("Product not found."); return; }

        int qty = InputReader.ReadInt("Quantity to add: ");
        string desc = InputReader.ReadString("Entry description: ");

        var entry = entryService.Register(currentUser!, pid, qty, desc);
        Console.WriteLine($"Entry registered: {entry}");
    }

    static void RegisterExit()
    {
        EnsureLoggedIn();

        Console.WriteLine("\n--- Register EXIT ---");
        ListProducts();
        int pid = InputReader.ReadInt("Product ID: ");
        var p = productService.GetById(pid);
        if (p == null) { Console.WriteLine("Product not found."); return; }

        Console.WriteLine($"Current stock: {p.CurrentStock}");
        int qty = InputReader.ReadInt("Quantity to remove: ");
        string desc = InputReader.ReadString("Exit description: ");

        try
        {
            var exit = exitService.Register(currentUser!, pid, qty, desc);
            Console.WriteLine($"Exit registered: {exit}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Cannot register exit: {ex.Message}");
        }
    }

    static void ViewMovements()
    {
        EnsureLoggedIn();
        EnsureAdmin();

        Console.WriteLine("\n--- All Entries ---");
        var entries = entryService.GetAll(currentUser!);
        if (!entries.Any()) Console.WriteLine("No entries.");
        else entries.ForEach(e => Console.WriteLine(e));

        Console.WriteLine("\n--- All Exits ---");
        var exits = exitService.GetAll(currentUser!);
        if (!exits.Any()) Console.WriteLine("No exits.");
        else exits.ForEach(s => Console.WriteLine(s));
    }

    static void ManageCategories()
    {
        EnsureLoggedIn();
        EnsureAdmin();

        Console.WriteLine("\n--- Manage Categories ---");
        Console.WriteLine("1 - List categories");
        Console.WriteLine("2 - Register category");
        Console.WriteLine("3 - Update category");
        Console.WriteLine("4 - Delete category");
        Console.WriteLine("0 - Back");
        var opt = Console.ReadLine();
        switch (opt)
        {
            case "1":
                var cats = categoryService.GetAll();
                if (!cats.Any()) Console.WriteLine("No categories.");
                else cats.ForEach(c => Console.WriteLine($"{c.Id} - {c.CategoryName}"));
                break;
            case "2":
                string name = InputReader.ReadString("Category name: ");
                var cnew = categoryService.Register(name);
                Console.WriteLine($"Category created: {cnew.Id} - {cnew.CategoryName}");
                break;
            case "3":
                foreach (var c in categoryService.GetAll()) Console.WriteLine($"{c.Id} - {c.CategoryName}");
                int upId = InputReader.ReadInt("Category Id to update: ");
                string newName = InputReader.ReadString("New name: ");
                if (categoryService.Update(upId, newName)) Console.WriteLine("Category updated.");
                else Console.WriteLine("Category not found.");
                break;
            case "4":
                foreach (var c in categoryService.GetAll()) Console.WriteLine($"{c.Id} - {c.CategoryName}");
                int delId = InputReader.ReadInt("Category Id to delete: ");
                if (categoryService.Delete(delId)) Console.WriteLine("Category deleted.");
                else Console.WriteLine("Category not found.");
                break;
            default:
                break;
        }
    }

    static void EnsureLoggedIn()
    {
        if (currentUser == null)
            throw new UnauthorizedAccessException("You must be logged in to perform this action.");
    }

    static void EnsureAdmin()
    {
        if (currentUser == null || !currentUser.IsAdmin())
            throw new UnauthorizedAccessException("Admin privileges required.");
    }
}
