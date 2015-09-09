using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp.Class
{
    class GameServer
    {
		UDPClient serverUDP;
		Server server;
		TCPClient client;
		TCPListener listener;
		PlayPage playpage;
		string portTCP2L;

		public GameServer(PlayPage playpage, string name, string portUDP1, string portUDP2, string portTCP1L, string portTCP1S, string portTCP2L, string portTCP2S, string portListener)
		{
			this.portTCP2L = portTCP2L;
			this.playpage = playpage;
			serverUDP = new UDPClient(playpage, name, portUDP1, portUDP2);
			serverUDP.Start();

			listener = new TCPListener(playpage, name);
			listener.initListener(portListener);
			listener.Received += OnReceived;

			server = new Server(playpage, portTCP2S, name);
			client = new TCPClient(playpage, name);
			
			server.addForPlayer1Listener(portTCP1L);
			client.initListener(portTCP1S);
			server.addForPlayer2Listener(portTCP2L);
			client.initSender(portTCP1L, IPAdress.LocalIPAddress());
			server.addForPlayer1Sender(portTCP1S, IPAdress.LocalIPAddress());
			
			server.sendToPlayer1("Wait for another player.");
		}

		private void OnReceived(string remoteName, string remoteAdress, string remotePort)
		{
			listener.initSender(portTCP2L, remoteAdress);
			if (listener.name.Equals(remoteName.Split('\r')[0]))
			{
				playpage.DisplayMessages("Check name: this same names");
				listener.SendRequest("No acces\r\n");
			}
			else
			{
				playpage.DisplayMessages("Check name: different name");
				playpage.AddClient(remoteName.Split('\r')[0]);
			}

		}

		public void sendToPlayer2(string message)
		{
			listener.SendRequest(message);
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
