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

		public Server()
		{

		}

		public void addPlayer1Connection(PlayPage page)
		{
			toClient1 = new ConnectionTCP(page, "Server", 80);
			toClient1.startSender(82);
		}

		public void addPlayer2Connection(PlayPage page)
		{
			toClient2 = new ConnectionTCP(page, "Server", 81);
			toClient2.startSender(83);
		}
    }
}
