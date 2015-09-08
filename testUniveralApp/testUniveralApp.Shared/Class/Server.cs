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
		ConnectionTCP Client1, Client2;
		PlayPage playPage;

		public Server(PlayPage playPage, string name = "Server")
		{
			this.Client1 = new ConnectionTCP(playPage, name);
			this.Client2 = new ConnectionTCP(playPage, name);
			this.playPage = playPage;
			Client2.Received += OnReceived;
		}

		private void OnReceived(string remoteName, string remoteAdress, string remotePort)
		{
			Client2.initSender("8024", remoteAdress);
			if (Client1.name.Equals(remoteName.Split('\r')[0]))
			{
				playPage.DisplayMessages("Check Name: this same");
				Client2.SendRequest("No acces\r\n");
			}
			else
			{
				playPage.DisplayMessages("Check Name: diff name");
				playPage.AddClient(remoteName.Split('\r')[0]);
			}

		}

		public void addForPlayer1Listener(string portListener)
		{
			Client1.initListener(portListener);
		}

		public void addForPlayer2Listener(string portListener)
		{
			Client2.initListener(portListener);
		}

		public void addForPlayer1Sender(string portSender, string remoteAdress)
		{
			Client1.initSender(portSender, remoteAdress);
		}

		public void addForPlayer2Sender(string portSender, string remoteAdress)
		{
			Client2.initSender(portSender, remoteAdress);
		}
		
		public void sendToPlayer1(string message)
		{
			Client1.SendRequest(message);
		}

		public void sendToPlayer2(string message)
		{
			Client2.SendRequest(message);
		}

		internal void Dispose()
		{
			if (Client1 != null)
				Client1.Dispose();
			if (Client2 != null)
				Client2.Dispose();
		}
	}
}
