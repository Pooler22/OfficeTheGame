using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp
{
    class Client
    {
		string name;
		ConnectionTCP toServerTCP;
		GameData data;

		public Client(string name)
		{
			this.name = name;
		}

		public void initClientListener(PlayPage page, int portListener)
		{
			toServerTCP = new ConnectionTCP(page, "Client");
			toServerTCP.initListener(portListener);
		}

		public void initClientSender(int portSender, string remoteAdress)
		{
			toServerTCP.initSender(portSender, remoteAdress);
		}

		public void sendToServer(string message)
		{
			toServerTCP.SendRequest(message);
		}
    }
}
