namespace Stocky.Utils
{
    public static class InputReader
    {
        public static int ReadInt(string message)
        {
            while (true)
            {
                Console.Write(message);

                if (int.TryParse(Console.ReadLine(), out int number))
                    return number;

                Console.WriteLine("Invalid number. Try again.");
            }
        }

        public static string ReadString(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                    return input.Trim();

                Console.WriteLine("Input cannot be empty.");
            }
        }

        public static decimal ReadDecimal(string message)
        {
            while (true)
            {
                Console.Write(message);
                if (decimal.TryParse(Console.ReadLine(), out var d)) return d;
                Console.WriteLine("Invalid decimal. Try again.");
            }
        }
    }
}
