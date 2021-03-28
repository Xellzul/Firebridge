using System;

namespace FireBridgeCore.Networking
{
    public interface IServer
    {
        public bool Start(int port);

        public void Stop();

        public Connection GetConnection(Guid id);

    }
}
