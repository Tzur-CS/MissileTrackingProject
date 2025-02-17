// using System;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading.Tasks;
// using System.Linq;
// using System.Collections.Generic;
// using MissileTracking.Commands;
// using MissileTracking.Database;
// using MissileTracking.Models;
// using MissileTracking.Parsers;
// using MissileTracking.Interception;
//
// namespace MissileTracking.Server
// {
//     public class TcpMissileServer
//     {
//         private readonly int _port;
//         private readonly Func<MissileDbContext> _createDbContext;
//         private readonly CommandFactory _commandFactory;
//
//         public TcpMissileServer(int port, Func<MissileDbContext> createDbContext, HashSet<string> policy)
//         {
//             _port = port;
//             _createDbContext = createDbContext;
//             var missileInterceptor = new MissileInterceptor(_createDbContext, policy);
//             _commandFactory = new CommandFactory(missileInterceptor);
//         }
//
//         public async Task StartAsync()
//         {
//             TcpListener listener = new TcpListener(IPAddress.Any, _port);
//             listener.Start();
//             Console.WriteLine($"[Server] Listening on port {_port}...");
//
//             while (true)
//             {
//                 TcpClient client = await listener.AcceptTcpClientAsync();
//                 _ = HandleClientAsync(client);
//             }
//         }
//
//         private async Task HandleClientAsync(TcpClient client)
//         {
//             using (client)
//             {
//                 NetworkStream stream = client.GetStream();
//                 string receivedData = await ReadDataAsync(stream);
//                 Console.WriteLine($"[Server] Received: {receivedData}");
//
//                 try
//                 {
//                     await ProcessCommandAsync(receivedData, stream);
//                 }
//                 catch (Exception ex)
//                 {
//                     Console.WriteLine($"[Server] Error processing request: {ex.Message}");
//                     await SendResponseAsync(stream, "Server error");
//                 }
//             }
//         }
//
//         private async Task<string> ReadDataAsync(NetworkStream stream)
//         {
//             byte[] buffer = new byte[1024];
//             int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
//             return Encoding.UTF8.GetString(buffer, 0, bytesRead);
//         }
//
//         private async Task ProcessCommandAsync(string receivedData, NetworkStream stream)
//         {
//             var parts = receivedData.Split(':');
//             if (parts.Length > 3)
//             {
//                 await SendResponseAsync(stream, "Invalid command format");
//                 return;
//             }
//
//             string commandType = parts[0];
//             string? requestData = parts.Length == 2 ? parts[1] : null;
//
//             await ExecuteCommandAsync(commandType, requestData, stream);
//         }
//
//         private async Task ExecuteCommandAsync(string commandType, string? requestData, NetworkStream stream)
//         {
//             ICommand command = _commandFactory.GetCommand(commandType);
//             if (command != null)
//             {
//                 await command.ExecuteAsync(requestData, stream, _createDbContext);
//             }
//             else
//             {
//                 await SendResponseAsync(stream, "Unknown command");
//             }
//         }
//
//         private async Task SendResponseAsync(NetworkStream stream, string message)
//         {
//             byte[] responseData = Encoding.UTF8.GetBytes(message);
//             await stream.WriteAsync(responseData, 0, responseData.Length);
//         }
//     }
// }
