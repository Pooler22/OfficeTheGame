using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp.Class
{
    class GameClient
    {
		UDPClientFinder finderUDP;
		Client client;
		string name;

		public GameClient(PlayPage playpage, string name, string portUDP1, string portUDP2, string portTCP2S)
		{
			this.name = name;

			finderUDP = new UDPClientFinder(playpage, name, portUDP2, portUDP1);
			finderUDP.Start();
			finderUDP.BroadcastIP();

			client = new Client(playpage, name);
			client.initClientListener(portTCP2S);
		}

		public void sendToServerName(string port, string adres)
		{
			client.initClientSender(port, adres);
			client.sendToServer(name);
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
