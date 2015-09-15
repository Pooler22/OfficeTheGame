namespace testUniveralApp.Class
{
    internal class GameClient
    {
        public delegate void ChangedEventHandler();

        public event ChangedEventHandler Received;

        private UDPListenerFinder finderUDP;

        private TCPClient firstConnectionClient;

        private TCPClient client;
        private PlayPage playpage;
        private string portTCP2S, name;

        public GameClient(PlayPage playpage, string name,
            string portUDP1, string portUDP2,
            string portTCP1L, string portTCP1S,
            string portTCP2L, string portTCP2S,
            string portTCP3L, string portTCP3S)
        {
            this.playpage = playpage;
            this.name = name;
            this.portTCP2S = portTCP2S;

            finderUDP = new UDPListenerFinder(playpage, name, portUDP1, portUDP2);
            finderUDP.SendDiscovery();

            firstConnectionClient = new TCPClient(playpage, name);
            firstConnectionClient.initListener(portTCP3L);
            firstConnectionClient.Received += firstConnectionReceived;

            client = new TCPClient(playpage, name);
            client.initListener(portTCP2L);
            client.Received += OnClientReceived;
        }

        // TCP first connection
        private void firstConnectionReceived(string remoteMessage, string remoteAdress, string remotePort)
        {
            // playpage.DisplayMessages("First connection Recieived ["+ remoteAdress + "]:" + remotePort + " " + remoteMessage.Split('\r')[0]);
            if (remoteMessage.Split('\r')[0].Equals("Accept"))
            {
                //`client.initSender(portTCP2S, remoteAdress);
                playpage.DisplayMessages("Connecting");
            }
            else if (remoteMessage.Split('\r')[0].Equals("Cancel"))
            {
                playpage.DisplayMessages("Disconnecting");
            }
        }

        public void sendToServerName(string port, string adres)
        {
            firstConnectionClient.initSender(port, adres);
            firstConnectionClient.SendRequest(name);
        }

        //game TCP
        private void OnClientReceived(string remoteMessage, string remoteAdress, string remotePort)
        {
            playpage.OnReceived();
            playpage.setBallPosition(int.Parse(remoteMessage.Split(' ')[0]), int.Parse(remoteMessage.Split(' ')[1]), int.Parse(remoteMessage.Split(' ', '\r')[2]));
            client.SendRequest(playpage.getPlayerPosition());
        }

        //other
        public void Dispose()
        {
            if (finderUDP != null)
            {
                finderUDP.Dispose();
            }
            if (client != null)
            {
                client.Dispose();
            }
            if (firstConnectionClient != null)
            {
                firstConnectionClient.Dispose();
            }
        }
    }
}