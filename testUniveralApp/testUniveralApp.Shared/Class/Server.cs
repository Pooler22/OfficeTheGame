using testUniveralApp.Class;

namespace testUniveralApp
{
    public partial class Server
    {
        public delegate void ChangedEventHandler(string messag);

        public event ChangedEventHandler Received1;

        public event ChangedEventHandler Received2;

        private TCPClientLocal Client1;
         private TCPClientRemote Client2;
        private PlayPage playPage;
        private string portSender;

        public Server(PlayPage playPage, string portSender, string name = "Server")
        {
            this.Client1 = TCPClientLocal.Instance;
            Client1.initTCPClient(playPage, name);
            this.Client2 = TCPClientRemote.Instance;
            Client2.initTCPClient(playPage, name);
            this.playPage = playPage;
            this.portSender = portSender;
            Client1.Received += OnReceived1;
            Client2.Received += OnReceived2;
        }

        public void addForPlayer1Listener(string portListener)
        {
            Client1.initListener(portListener);
        }

        public void addForPlayer1Sender(string portSender, string remoteAdress)
        {
            Client1.initSender(portSender, remoteAdress);
        }

        public void addForPlayer2Listener(string portListener)
        {
            Client2.initListener(portListener);
        }

        public void addForPlayer2Sender(string portSender, string remoteAdress)
        {
            Client2.initSender(portSender, remoteAdress);
        }

        public void Dispose()
        {
            if (Client1 != null)
                Client1.Dispose();
            if (Client2 != null)
                Client2.Dispose();
        }

        public void sendToPlayer1(string message)
        {
            Client1.SendRequest(message);
        }

        public void sendToPlayer2(string message)
        {
            Client2.SendRequest(message);
        }

        private void OnReceived1(string message, string remoteAdress, string remotePort)
        {
            if (Received1 != null)
                Received1(message);
        }

        private void OnReceived2(string message, string remoteAdress, string remotePort)
        {
            if (Received2 != null)
                Received2(message);
        }
    }
}