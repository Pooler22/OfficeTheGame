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
		PlayPage playPage;

		public Server(PlayPage playPage, string name = "Server")
		{
			this.playPage = playPage;
			toClient1 = new ConnectionTCP(playPage, name);
			toClient2 = new ConnectionTCP(playPage, name);
			toClient2.Changed += OnConnectionReceived;
		}

		public void addForPlayer1Listener(string portListener)
		{
			toClient1.initListener(portListener);
		}

		public void addForPlayer2Listener(string portListener)
		{
			toClient2.initListener(portListener);
		}

		private void OnConnectionReceived(object sender, string remoteName, string remoteAdress, string remotePort)
		{
			playPage.DisplayMessages("Check Name C:" + remoteName + " S:" + toClient1.name);
			toClient2.initSender("8024", remoteAdress);
			if (toClient1.name.Equals(remoteName.Split('\r')[0]))
			{
				playPage.DisplayMessages("this same");
				toClient2.SendRequest("Fuck you!\r\n");
			}
			else
			{
				playPage.DisplayMessages("diff name");
				playPage.AddClient(remoteName.Split('\r')[0]);
			}

		}

		public void addForPlayer1Sender(string portSender, string remoteAdress)
		{
			toClient1.initSender(portSender, remoteAdress);
		}

		public void addForPlayer2Sender(string portSender, string remoteAdress)
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
