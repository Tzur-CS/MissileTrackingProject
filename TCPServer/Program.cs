using MissileTracking.Client;

class Program
{
    static async Task Main(string[] args)
    {
        var client = new TcpMissileClient("127.0.0.1", 5000);
        
        while (true)
        {
            Console.Write("Enter command: ");
            string command = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(command))
            {
                Console.WriteLine("Invalid input. Please enter a valid command.");
                continue;
            }
            
            if (command.ToLower() == "exit")
            {
                break;
            }
            
            await client.SendCommandAsync(command);
        }
    }
}