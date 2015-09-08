namespace testUniveralApp
{
	public partial class Server
    {
		string portSender;
		ConnectionTCP Client1, Client2;
		PlayPage playPage;

		public Server(PlayPage playPage, string portSender, string name = "Server")
		{
			this.Client1 = new ConnectionTCP(playPage, name);
			this.Client2 = new ConnectionTCP(playPage, name);
			this.playPage = playPage;
			this.portSender = portSender;
			Client2.Received += OnReceived;
		}

		private void OnReceived(string remoteName, string remoteAdress, string remotePort)
		{
			Client2.initSender(portSender, remoteAdress);
			if (Client1.name.Equals(remoteName.Split('\r')[0]))
			{
				playPage.DisplayMessages("Check name: this same names");
				Client2.SendRequest("No acces\r\n");
			}
			else
			{
				playPage.DisplayMessages("Check name: different name");
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
