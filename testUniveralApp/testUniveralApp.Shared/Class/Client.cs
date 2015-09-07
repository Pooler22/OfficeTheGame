using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp
{
    class Client
    {
		ConnectionTCP toServerTCP;
		GameData data;

		public Client(PlayPage page, string name)
		{
			this.toServerTCP = new ConnectionTCP(page, name);
		}

		public void initClientListener(int portListener)
		{
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
