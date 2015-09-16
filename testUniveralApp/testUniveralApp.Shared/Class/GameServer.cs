using System.Threading.Tasks;

namespace testUniveralApp.Class
{
    internal class GameServer
    {
        public delegate void ChangedEventHandler(string e, string remoteAdress, string remotePort);

        public event ChangedEventHandler Received;

        public event ChangedEventHandler Received2;

        private UDPListener serverUDP;
        private TCPClientRemote firstConnectionClient;
        private TCPClientRemote client;
        private Server server;
        private PlayPage playpage;
        private string portTCP3S, portTCP2S, portTCP1L;
        private bool serverBussyflag;
        private GameData gameData;

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

            firstConnectionClient = TCPClientRemote.Instance;
            firstConnectionClient.initTCPClient(playpage, name);
            firstConnectionClient.initListener(portTCP3L);
            firstConnectionClient.Received += firstConnectionReceived;

            this.portTCP3S = portTCP3S;
            this.portTCP2S = portTCP2S;
            this.portTCP1L = portTCP1L;

            server = new Server(playpage, portTCP2S);
            server.Received1 += onServerRecieved1;
            server.Received2 += onServerRecieved2;


            client = TCPClientRemote.Instance;
            client.initTCPClient(playpage, name);
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
                           //int tmpPlayer2Pos = mathFunction(player2Pos);
                           if (gameData.yPos >= 100 || gameData.yPos <= 0)
                           {
                               gameData.yMove = -gameData.yMove;
                           }
                           gameData.yPos += gameData.yMove;
                           if (gameData.yPos < 3 || gameData.yPos > 97)
                           {
                               gameData.xPos = gameData.yPos = 50;
                           }
                           if (((gameData.yPos < 5))
                           || (gameData.yPos > 95))
                               gameData.yMove = -gameData.yMove;

                           sendToPlayer1(gameData.xPos + " " + gameData.yPos + " " + mathFunction(gameData.player2Pos));
                           sendToPlayer2(mathFunction(gameData.xPos) + " " + mathFunction(gameData.yPos) + " " + mathFunction(gameData.player1Pos));
                           await Task.Delay(50);
                       }
                   });
        }

        private void onServerRecieved1(string message)
        {
            gameData.player1Pos = int.Parse(message.Split(' ', '\r')[0]);
        }

        private void onServerRecieved2(string message)
        {
            gameData.player2Pos = int.Parse(message.Split(' ', '\r')[0]);
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
            if (firstConnectionClient != null)
            {
                firstConnectionClient.Dispose();
            }
        }
    }
}