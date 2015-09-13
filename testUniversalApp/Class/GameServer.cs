using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp.Class
{
    class GameServer
    {
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

            server = new Server(playpage, portTCP2S, name);
			client = new TCPClient(playpage, name);
			
			server.addForPlayer1Listener(portTCP1L);
			client.initListener(portTCP1S);
			server.addForPlayer2Listener(portTCP2L);
			client.initSender(portTCP1L, IPAdress.LocalIPAddress());
			server.addForPlayer1Sender(portTCP1S, IPAdress.LocalIPAddress());
			
			//server.sendToPlayer1("Wait for another player.");

            server.addForPlayer2Listener(portTCP2L);
        }

		private void OnReceived(string remoteName, string remoteAdress, string remotePort)
		{
            if (serverBussyflag)
            {
                firstConnectionClient.initSender(portTCP3S, remoteAdress);
                firstConnectionClient.SendRequest("Cancel server bussy");
            }
            else
            {
                if (firstConnectionClient.name.Equals(remoteName.Split('\r')[0]))
                {
                    playpage.DisplayMessages("Check name: this same names");
                    firstConnectionClient.initSender(portTCP3S, remoteAdress); // remote port, remote ip 
                    firstConnectionClient.SendRequest("Cancel this same name"); // message 
                    //listener.SendRequest("No acces\r\n");
                    serverBussyflag = true;
                }
                else
                {
                    playpage.DisplayMessages("Check name: different name");
                    playpage.AddClient(remoteName.Split('\r')[0] + " " + remoteAdress + " " + remotePort);
                    server.addForPlayer2Sender(portTCP2S, remoteAdress);
                }
            }
        }

		public void sendToPlayer2(string message)
		{
            firstConnectionClient.initSender(portTCP3S, message.Split(' ')[2]); // remote port, remote ip 
            firstConnectionClient.SendRequest(message.Split(' ')[0]); // message 
		}

        public void sendToServer(string message)
        {
            client.initSender(portTCP1L, IPAdress.LocalIPAddress()); // remote port, remote ip 
            playpage.DisplayMessages(message);
            client.SendRequest(message); // message 
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
