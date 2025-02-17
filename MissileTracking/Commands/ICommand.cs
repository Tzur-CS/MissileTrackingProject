using System.Net.Sockets;
using MissileTracking.Database;

namespace MissileTracking.Commands
{
    public interface ICommand
    {
        Task ExecuteAsync(string request, NetworkStream stream, Func<MissileDbContext> dbContextProvider);
    }
}