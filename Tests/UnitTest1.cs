using FireBridgeCore.Networking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading;

namespace Tests
{
    [TestClass]
    public class TCPTests
    {
        const int port = 6969;
        IPAddress Localhost = IPAddress.Parse("127.0.0.1");

        [TestMethod]
        public void ServerStartsStops()
        {
            TCPServer server = new TCPServer();
            Assert.IsTrue(server.Start(port));
            server.Stop();
        }

        [TestMethod]
        public void ServerAcceptsConections()
        {
            TCPServer server = new TCPServer();
            Assert.IsTrue(server.Start(port));

            TCPConnection connection = new TCPConnection();
            connection.Start(Localhost, port);
            connection.Close();

            server.Stop();
        }


        bool recieved = false;
        [TestMethod]
        public void CanSendMSG()
        {
            recieved = false;
            TCPServer server = new TCPServer();
            server.ClientConnected += Server_ClientConnected;
            Assert.IsTrue(server.Start(port));

            TCPConnection connection = new TCPConnection();
            connection.Start(Localhost, port);
            connection.Send(new Packet() { Payload = "asdf" });

            Thread.Sleep(1000);
            Assert.IsTrue(recieved);

            connection.Close();

            server.Stop();
        }

        private void Server_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            e.Connection.MessageRecieved += Connection_MessageRecieved;
        }

        private void Connection_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            recieved = true;
            Assert.AreEqual(e.Message.Payload.ToString(), "asdf");
        }
    }
}
