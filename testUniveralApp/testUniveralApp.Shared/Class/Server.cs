using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace testUniveralApp
{
	public partial class Server
    {
		ConnectionTCP toClient1;
		ConnectionTCP toClient2;
		GameData data;

		public Server(PlayPage page, string name = "Server")
		{
			toClient1 = new ConnectionTCP(page, name);
			toClient2 = new ConnectionTCP(page, name);
		}

		public void addForPlayer1Listener(int portListener)
		{
			toClient1.initListener(portListener);
		}

		public void addForPlayer2Listener(PlayPage page, int portListener)
		{
			toClient2.initListener(portListener);
		}

		public void addForPlayer1Sender(int portSender, string remoteAdress)
		{
			toClient1.initSender(portSender, remoteAdress);
		}

		public void addForPlayer2Sender(int portSender, string remoteAdress)
		{
			toClient2.initSender(portSender, remoteAdress);
		}
		
		public void sendToPlayer1(string message)
		{
			toClient1.SendRequest(message);
		}

		public void sendToPlayer2(string message)
		{
			toClient2.SendRequest(message);
		}


		internal void Dispose()
		{
			if (toClient1 != null)
				toClient1.Dispose();
			if (toClient2 != null)
				toClient2.Dispose();
		}
	}
}
