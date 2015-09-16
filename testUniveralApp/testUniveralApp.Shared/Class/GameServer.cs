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
        TCPClientLocal clientLocal;
        private Server server;
        private PlayPage playpage;
        private string portTCP3S, portTCP2S, portTCP1L;
        private bool serverBussyflag;
        private GameData gameData;
        private int xMove = 1;
        private int yMove = 1;
        private int xPos = 50;
        private int yPos = 50;
        private int player1Pos = 50;
        private int player2Pos = 50;

        public GameServer(PlayPage playpage, string name,
            string portUDP1, string portUDP2,
            string portTCP1L, string portTCP1S,
            string portTCP2L, string portTCP2S,
            string portTCP3L, string portTCP3S)
        {
            serverBussyflag = false;
            this.playpage = playpage;
            gameData = new GameData();

            serverUDP = new UDPListener(playpage, name, portUDP1, portUDP2);

            firstConnectionClient = new TCPClient(playpage, name);
            firstConnectionClient.initListener(portTCP3L);
            firstConnectionClient.Received += firstConnectionReceived;

            this.portTCP3S = portTCP3S;
            this.portTCP2S = portTCP2S;
            this.portTCP1L = portTCP1L;

            server = new Server(playpage, portTCP2S);
            server.Received1 += onServerRecieved1;
            server.Received2 += onServerRecieved2;

            clientLocal = new TCPClientLocal(playpage, name);
            clientLocal.Received += OnReceived1;

            client = new TCPClient(playpage, name);
            client.Received += OnReceived1;
            server.addForPlayer1Listener(portTCP1L);
            clientLocal.initListener(portTCP1S);
            client.initListener(portTCP1S);
            server.addForPlayer2Listener(portTCP2L);
            clientLocal.initSender(portTCP1L, IPAdress.LocalIPAddress());
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
                           //int tmpPlayer2Pos = mathFunction(player2Pos);
                           if (yPos >= 100 || yPos <= 0)
                           {
                               yMove = -yMove;
                           }
                           yPos += yMove;
                           if (yPos < 3 || yPos > 97)
                           {
                               xPos = yPos = 50;
                           }
                           if (((yPos < 5) && ((player1Pos - xPos) < 5)
                           || (yPos > 95) && ((player1Pos) < 5)))
                               yMove = -yMove;

                           sendToPlayer1(xPos + " " + yPos + " " + mathFunction(player2Pos));
                           sendToPlayer2(mathFunction(xPos) + " " + mathFunction(yPos) + " " + mathFunction(player1Pos));
                           await Task.Delay(100);
                       }
                   });
        }

        private void onServerRecieved1(string message)
        {
            player1Pos = int.Parse(message.Split(' ', '\r')[0]);
        }

        private void onServerRecieved2(string message)
        {
            player2Pos = int.Parse(message.Split(' ', '\r')[0]);
        }

        private int mathFunction(int number)
        {
            if (number > 50)
            {
                return 50 - (number - 50);
            }
            else
            {
                return (50 - number) + 50;
            }
        }

        private void OnReceived1(string remoteMessage, string remoteAdress, string remotePort)
        {
            playpage.OnReceived();
            playpage.setBallPosition(int.Parse(remoteMessage.Split(' ')[0]), int.Parse(remoteMessage.Split(' ')[1]), int.Parse(remoteMessage.Split(' ', '\r')[2]));
            client.SendRequest(playpage.getPlayerPosition());
            clientLocal.SendRequest(playpage.getPlayerPosition());
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
            clientLocal.SendRequest(message);
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
            if (clientLocal != null)
            {
                clientLocal.Dispose();
            }
            if (server != null)
            {
                server.Dispose();
            }
            if (firstConnectionClient != null)
            {
                firstConnectionClient.Dispose();
            }
        }
    }
}