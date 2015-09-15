using System.Threading.Tasks;

namespace testUniveralApp.Class
{
    internal class GameServer
    {
        public delegate void ChangedEventHandler(string e, string remoteAdress, string remotePort);

        public event ChangedEventHandler Received;

        public event ChangedEventHandler Received2;

        private UDPListener serverUDP;
        private TCPClient firstConnectionClient;
        private TCPClient client;
        private Server server;
        private PlayPage playpage;
        private string portTCP3S, portTCP2S, portTCP1L;
        private bool serverBussyflag;
        int xMove = 1;
        int yMove = 0;
        int xPos = 50;
        int yPos = 50;
        int player1Pos = 50;
        int player2Pos = 50;

        public GameServer(PlayPage playpage, string name,
            string portUDP1, string portUDP2,
            string portTCP1L, string portTCP1S,
            string portTCP2L, string portTCP2S,
            string portTCP3L, string portTCP3S)
        {
            serverBussyflag = false;
            this.playpage = playpage;

            serverUDP = new UDPListener(playpage, name, portUDP1, portUDP2);

            firstConnectionClient = new TCPClient(playpage, name);
            firstConnectionClient.initListener(portTCP3L);
            firstConnectionClient.Received += firstConnectionReceived;

            this.portTCP3S = portTCP3S;
            this.portTCP2S = portTCP2S;
            this.portTCP1L = portTCP1L;

            server = new Server(playpage, portTCP2S);
            client = new TCPClient(playpage, name);
            client.Received += OnReceived1;
            server.addForPlayer1Listener(portTCP1L);
            client.initListener(portTCP1S);
            server.addForPlayer2Listener(portTCP2L);
            client.initSender(portTCP1L, IPAdress.LocalIPAddress());
            server.addForPlayer1Sender(portTCP1S, IPAdress.LocalIPAddress());
            server.addForPlayer2Listener(portTCP2L);
        }

        // TCP first connection
        private void firstConnectionReceived(string remoteMessage, string remoteAdress, string remotePort)
        {
            if (serverBussyflag)
            {
                firstConnectionClient.initSender(portTCP3S, remoteAdress);
                firstConnectionClient.SendRequest("Cancel server bussy");
            }
            else
            {
                if (firstConnectionClient.name.Equals(remoteMessage.Split('\r')[0]))
                {
                    playpage.DisplayMessages("Check name: this same names");
                    firstConnectionClient.initSender(portTCP3S, remoteAdress);
                    firstConnectionClient.SendRequest("Cancel this same name");
                }
                else
                {
                    playpage.DisplayMessages("Check name: different name");
                    playpage.AddClient(remoteMessage.Split('\r')[0] + " " + remoteAdress + " " + remotePort);
                }
            }
        }

        public void sendToSelectedClient(string message)
        {
            if (message.Split(' ')[0].Equals("Accept"))
            {
                server.addForPlayer2Sender(portTCP2S, message.Split(' ')[2]);
                serverBussyflag = true;
                play();
            }
            firstConnectionClient.initSender(portTCP3S, message.Split(' ')[2]);
            firstConnectionClient.SendRequest(message.Split(' ')[0]);
        }

        //game TCP
        public void play()
        {
            Task.Run(
                   async () =>
                   {
                       while (true)
                       {
                           if (xPos >= 100 || xPos <= 0)
                           {
                               xMove = -xMove;
                           }
                           xPos += xMove;

                           sendToPlayer1(xPos + " " + yPos + " " + player2Pos);
                           sendToPlayer2(xPos + " " + yPos + " " + player1Pos);
                           await Task.Delay(10);
                       }
                   });
        }

        private void OnReceived1(string remoteMessage, string remoteAdress, string remotePort)
        {
            //playpage.OnReceived();
            playpage.setBallPosition(float.Parse(remoteMessage.Split(' ')[0]), float.Parse(remoteMessage.Split(' ','\r')[1]));
        }

        public void sendToPlayer1(string message)
        {
            server.sendToPlayer1(message);
        }

        public void sendToPlayer2(string message)
        {
            server.sendToPlayer2(message);
        }

        public void sendToServer(string message)
        {
            client.SendRequest(message);
        }

        //other
        public void Dispose()
        {
            if (serverUDP != null)
            {
                serverUDP.Dispose();
            }
            if (client != null)
            {
                client.Dispose();
            }
            if (server != null)
            {
                server.Dispose();
            }
        }
    }
}