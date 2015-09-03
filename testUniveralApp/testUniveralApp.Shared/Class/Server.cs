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
		ConnectionUDP brodcastListener;
		GameData data;

		public Server()
		{
			
		}

		public void initUDPLstener(PlayPage page, int portListener)
		{
			brodcastListener = new ConnectionUDP(page, "SERVER");
			brodcastListener.initServer(portListener);
		}

		public void addForPlayer1Listener(PlayPage page, int portListener)
		{
			toClient1 = new ConnectionTCP(page, "Server");
			toClient1.initListener(portListener);
		}

		public void addForPlayer2Listener(PlayPage page, int portListener)
		{
			toClient2 = new ConnectionTCP(page, "Server");
			toClient2.initListener(portListener);
		}

		public void addForPlayer1Sender(int portSender)
		{
			toClient1.initSender(portSender);
		}

		public void addForPlayer2Sender(int portSender)
		{
			toClient2.initSender(portSender);
		}
		
		public void sendToPlayer1(string message)
		{
			toClient1.SendRequest(message);
		}

		public void sendToPlayer2(string message)
		{
			toClient2.SendRequest(message);
		}
		
	}
}
