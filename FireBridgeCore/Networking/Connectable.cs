using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking
{
    public class Connectable
    {
        private ConnectionStatus _status = ConnectionStatus.Disconnected;
        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

        public ConnectionStatus Status
        {
            get => _status;

            protected set
            {
                if (value == _status)
                    return;

                var old = _status;
                _status = value;
                OnConnectionStatusChanged(
                    new ConnectionStatusChangedEventArgs() { Before = old, Now = value });
            }
        }
        protected virtual void OnConnectionStatusChanged(ConnectionStatusChangedEventArgs e)
        {
            if (ConnectionStatusChanged == null)
                return;

            foreach (EventHandler<ConnectionStatusChangedEventArgs> reciever in ConnectionStatusChanged.GetInvocationList())
                Task.Run(() => { reciever.Invoke(this, e); });
        }
    }
}
