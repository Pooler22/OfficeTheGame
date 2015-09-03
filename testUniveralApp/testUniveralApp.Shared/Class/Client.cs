using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp
{
    class Client
    {
		ConnectionTCP toServer;
		ConnectionUDP brodcastDiscovery;

		public Client()
		{
			
		}

		public void addServerConnection(PlayPage page)
		{
			toServer = new ConnectionTCP(page, "Client", 80);
			toServer.startSender(82);
		}
    }
}
