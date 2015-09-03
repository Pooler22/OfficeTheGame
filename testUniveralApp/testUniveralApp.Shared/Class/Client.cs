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

		public void initPlayerConnection(PlayPage page, int portListener)
		{
			toServer = new ConnectionTCP(page, "Client", portListener);
		}

		public void addServerConnection(int portSender)
		{
			toServer.startSender(portSender);
		}
    }
}
