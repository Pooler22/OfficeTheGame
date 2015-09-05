using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp
{
    class Client
    {
		string name;
		ConnectionTCP toServerTCP;
		ConnectionUDP brodcastDiscovery;
		GameData data;

		public Client(string name)
		{
			this.name = name;
		}

		public void initUDPFinder(PlayPage page, int portListener)
		{
			brodcastDiscovery = new ConnectionUDP(page, name);
			brodcastDiscovery.initUDPFinder(portListener);
		}

		public void initClientListener(PlayPage page, int portListener)
		{
			toServerTCP = new ConnectionTCP(page, "Client");
			toServerTCP.initListener(portListener);
		}

		public void initClientSender(int portSender)
		{
			toServerTCP.initSender(portSender);
		}
    }
}
