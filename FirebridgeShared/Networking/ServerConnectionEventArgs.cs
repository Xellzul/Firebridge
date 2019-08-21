using System;

namespace FirebridgeShared.Networking
{
    public class ServerConnectionEventArgs : EventArgs
    {
        public Connection Connection { get; set; }
        public ServerConnectionEventArgs(Connection connection)
        {
            Connection = connection;
        }
    }
}
