using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp.Class
{
    class GameClient
    {
		UDPClientFinder finderUDP;
		TCPClient client;
		string name;

		public GameClient(PlayPage playpage, string name, string portUDP1, string portUDP2, string portTCP2L)
		{
			this.name = name;

			finderUDP = new UDPClientFinder(playpage, name, portUDP2, portUDP1);
			finderUDP.Start();
			finderUDP.SendDiscovery();

			client = new TCPClient(playpage, name);
			client.initListener(portTCP2L);
		}

		public void sendToServerName(string port, string adres)
		{
			client.initSender(port, adres);
			client.SendRequest(name);
		}

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
		}
	}
}
