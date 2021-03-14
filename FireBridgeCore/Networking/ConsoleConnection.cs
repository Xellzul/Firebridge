using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking
{
    public class ConsoleConnection : Connection
    {
        public virtual void Start(Stream reader, Stream writer)
        {
            if (_shouldEnd)
                return;

            Status = ConnectionStatus.Connecting;

            _readStream = reader;
            _writeStream = writer;

            this._readerThread = new Thread(ReadLoop);
            this._readerThread.IsBackground = true;
            this._readerThread.Name = "ReaderThread Console";
            this._readerThread.Start();

            Status = ConnectionStatus.Connected;
        }

        public override void Close()
        {
            Status = ConnectionStatus.Disconnected;
            _shouldEnd = true;
            _readStream = null;
            _writeStream = null;
        }

    }
}
