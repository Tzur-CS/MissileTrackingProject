using System.Net.Sockets;
using System.Text;

namespace MissileTracking.Client
{
    public class TcpMissileClient
    {
        private readonly string _serverIp;
        private readonly int _port;

        public TcpMissileClient(string serverIp, int port)
        {
            _serverIp = serverIp;
            _port = port;
            
            
        }

        public async Task SendCommandAsync(string message)
        {
            await SendMessageAsync(message);
        }

        private async Task SendMessageAsync(string message)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(_serverIp, _port);
                    NetworkStream stream = client.GetStream();
                    
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    await stream.WriteAsync(data, 0, data.Length);
                    Console.WriteLine($"[Client] Sent: {message}");
                    
                    byte[] responseBuffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                    string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
                    Console.WriteLine($"[Client] Received:\n{response}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Client] Error: {ex.Message}");
            }
        }
    }
}