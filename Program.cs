using Stocky.Services;
using Stocky.Utils;

var inventoryService = new InventoryService();
var movementService = new MovementService();

bool running = true;

while (running)
{
    Console.WriteLine("\n===== STOREROOM INVENTORY =====");
    Console.WriteLine("1 - Register new product");
    Console.WriteLine("2 - List products");
    Console.WriteLine("3 - Register stock ENTRY");
    Console.WriteLine("4 - Register stock EXIT");
    Console.WriteLine("5 - Show movement history");
    Console.WriteLine("0 - Exit");
    Console.Write("Choose an option: ");

    switch (Console.ReadLine())
    {
        case "1":
            RegisterProduct();
            break;

        case "2":
            ListProducts();
            break;

        case "3":
            RegisterEntry();
            break;

        case "4":
            RegisterExit();
            break;

        case "5":
            ShowMovements();
            break;

        case "0":
            running = false;
            break;

        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}

void RegisterProduct()
{
    string name = InputReader.ReadString("Product name: ");
    int qty = InputReader.ReadInt("Initial quantity: ");

    var p = inventoryService.AddProduct(name, qty);

    if (qty > 0)
        movementService.AddMovement(p.Id, p.Name, qty, "IN", "Initial stock");

    Console.WriteLine($"Product '{p.Name}' registered with ID {p.Id}");
}

void ListProducts()
{
    var products = inventoryService.GetAll();

    if (products.Count == 0)
    {
        Console.WriteLine("No products registered.");
        return;
    }

    Console.WriteLine("ID\tName\tQuantity");
    foreach (var p in products)
        Console.WriteLine($"{p.Id}\t{p.Name}\t{p.Quantity}");
}

void RegisterEntry()
{
    ListProducts();
    int id = InputReader.ReadInt("Product ID: ");

    var p = inventoryService.GetById(id);
    if (p == null)
    {
        Console.WriteLine("Product not found.");
        return;
    }

    int qty = InputReader.ReadInt("Quantity to add: ");
    inventoryService.IncreaseStock(p, qty);

    movementService.AddMovement(p.Id, p.Name, qty, "IN", "Entry");
    Console.WriteLine("Entry registered.");
}

void RegisterExit()
{
    ListProducts();
    int id = InputReader.ReadInt("Product ID: ");

    var p = inventoryService.GetById(id);
    if (p == null)
    {
        Console.WriteLine("Product not found.");
        return;
    }

    Console.WriteLine($"Current stock: {p.Quantity}");
    int qty = InputReader.ReadInt("Quantity to remove: ");

    if (!inventoryService.DecreaseStock(p, qty))
    {
        Console.WriteLine("Not enough stock.");
        return;
    }

    movementService.AddMovement(p.Id, p.Name, qty, "OUT", "Exit");
    Console.WriteLine("Exit registered.");
}

void ShowMovements()
{
    var list = movementService.GetAll();

    if (list.Count == 0)
    {
        Console.WriteLine("No movements yet.");
        return;
    }

    Console.WriteLine("DATE\t\tTYPE\tPRODUCT\tQTY\tDESC");

    foreach (var m in list)
    {
        Console.WriteLine($"{m.Date:yyyy-MM-dd HH:mm}\t{m.Type}\t{m.ProductName}\t{m.Quantity}\t{m.Description}");
    }
}
