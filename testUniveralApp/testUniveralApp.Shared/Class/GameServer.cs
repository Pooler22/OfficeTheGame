using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp.Class
{
    class GameServer
    {
		UDPClient serverUDP;
		Server server;
		ConnectionTCP client;

		public GameServer(PlayPage playpage, string name, string portUDP1, string portUDP2, string portTCP1L, string portTCP1S, string portTCP2L, string portTCP2S)
		{
			serverUDP = new UDPClient(playpage, name, portUDP1, portUDP2);
			serverUDP.Start();

			server = new Server(playpage, portTCP2S, name);
			client = new ConnectionTCP(playpage, name);

			server.addForPlayer1Listener(portTCP1L);
			client.initListener(portTCP1S);
			server.addForPlayer2Listener(portTCP2L);
			client.initSender(portTCP1L, IPAdress.LocalIPAddress());
			server.addForPlayer1Sender(portTCP1S, IPAdress.LocalIPAddress());
			
			server.sendToPlayer1("Wait for another player.");
		}

		public void sendToPlayer2(string message)
		{
			server.sendToPlayer2(message + "\r\n");
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
