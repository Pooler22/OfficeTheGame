using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace testUniveralApp.Class
{
    class GameServer
    {
        public delegate void ChangedEventHandler(string e, string remoteAdress, string remotePort);
        public event ChangedEventHandler Received;
        public event ChangedEventHandler Received2;

        UDPListener serverUDP;
        TCPClient firstConnectionClient;
		TCPClient client;
		Server server;
		PlayPage playpage;
		string portTCP3S;
        string portTCP2S, portTCP1L;
        bool serverBussyflag;

        public GameServer(PlayPage playpage, string name, 
            string portUDP1, string portUDP2, 
            string portTCP1L, string portTCP1S, 
            string portTCP2L, string portTCP2S, 
            string portTCP3L, string portTCP3S)
		{
            serverBussyflag = false;
            this.playpage = playpage;

            serverUDP = new UDPListener(playpage, name, portUDP1, portUDP2);
			serverUDP.Start();

            firstConnectionClient = new TCPClient(playpage, name);
            firstConnectionClient.initListener(portTCP3L);
            firstConnectionClient.Received += OnReceived;

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
			
			//server.sendToPlayer1("50 50");
            
            server.addForPlayer2Listener(portTCP2L);
        }

        private void OnReceived1(string remoteMessage, string remoteAdress, string remotePort)
        {

            if (Received2 != null)
                Received2(remoteMessage, remoteAdress, remotePort);
        }

        private void OnReceived(string remoteMessage, string remoteAdress, string remotePort)
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
                    firstConnectionClient.initSender(portTCP3S, remoteAdress); // remote port, remote ip 
                    firstConnectionClient.SendRequest("Cancel this same name"); // message 
                    serverBussyflag = true;
                    server.sendToPlayer1("play");
                    server.sendToPlayer2("play");
                    // play();
                }
                else
                {
                    playpage.DisplayMessages("Check name: different name");
                    playpage.AddClient(remoteMessage.Split('\r')[0] + " " + remoteAdress + " " + remotePort);
                    server.addForPlayer2Sender(portTCP2S, remoteAdress);
                }
            }
        }

        void play()
        {
            Task.Run(
                   async () =>
                   {
                       await Task.Delay(1000);

                       while (true)
                       {
                           {
                               server.sendToPlayer1("play");
                               server.sendToPlayer2("play");
                               //DisplayMessages(portUDP1);
                               //gameServer.sendToServer(playerButton.Margin.ToString());
                           }
                           await Task.Delay(1000);
                       }
                   });
        }

        public void sendIniToPlayer2(string message)
		{
            firstConnectionClient.initSender(portTCP3S, message.Split(' ')[2]); // remote port, remote ip 
            firstConnectionClient.SendRequest(message.Split(' ')[0]); // message 
		}

        public void sendToPlayer1(string message)
        {
            server.sendToPlayer1(message);
        }

        public void sendToPlayer2(string message)
        {
            server.sendToPlayer2(message);
        }

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
