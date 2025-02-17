using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MissileTracking.Services
{
    public class TcpConnectionService
    {
        private const int Port = 5000;
        private readonly MissileTrackingService _trackingService;

        public TcpConnectionService(MissileTrackingService trackingService)
        {
            _trackingService = trackingService;
        }

        public async Task StartAsync()
        {
            var listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            Console.WriteLine($"[Server] Listening on port {Port}...");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                var stream = client.GetStream();
                try
                {
                    await _trackingService.ProcessCommandAsync(stream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Server] Error processing request: {ex.Message}");
                    await SendResponseAsync(stream, "Server error");
                }
            }
        }
        
        public static async Task SendResponseAsync(NetworkStream stream, string message)
        {
            var responseData = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(responseData, 0, responseData.Length);
        }
    }
}
