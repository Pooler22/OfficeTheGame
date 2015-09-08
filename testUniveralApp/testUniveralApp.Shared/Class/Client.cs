using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp
{
    class Client
    {
		ConnectionTCP toServerTCP;

		public Client(PlayPage page, string name)
		{
			this.toServerTCP = new ConnectionTCP(page, name);
		}

		public void initClientListener(string portListener)
		{
			toServerTCP.initListener(portListener);
		}

		public void initClientSender(string portSender, string remoteAdress)
		{
			toServerTCP.initSender(portSender, remoteAdress);
		}

		public void sendToServer(string message)
		{
			toServerTCP.SendRequest(message);
		}

		internal void Dispose()
		{
			if (toServerTCP != null)
			{
				toServerTCP.Dispose();
			}
		}
	}
}
