using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MissileTracking.Services
{
    public class TcpConnectionService
    {
        private readonly int _port;
        private readonly MissileTrackingService _trackingService;

        public TcpConnectionService(int port, MissileTrackingService trackingService)
        {
            _port = port;
            _trackingService = trackingService;
        }

        public async Task StartAsync()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, _port);
            listener.Start();
            Console.WriteLine($"[Server] Listening on port {_port}...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                NetworkStream stream = client.GetStream();
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

        private async Task<string> ReadDataAsync(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        private async Task SendResponseAsync(NetworkStream stream, string message)
        {
            byte[] responseData = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(responseData, 0, responseData.Length);
        }
    }
}
