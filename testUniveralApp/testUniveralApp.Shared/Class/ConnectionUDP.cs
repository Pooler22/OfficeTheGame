using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp
{
	class ConnectionUDP
	{

	}
	/*
    class ConnectionUDP
    {
		private async void initServer()
		{
			Task task = Task.Run(async () => { await this.startServer(); });
			task.Wait();
		}

		private async Task startServer()
		{
			try
			{
				if (socketServer == null)
				{
					socketServer = new DatagramSocket();
					socketServer.MessageReceived += OnMessageReceivedFromClient;
					await socketServer.BindServiceNameAsync(portServer);
					DisplayMessages("Server: Running, ip:" + GetLocalIPv4() + " portListener:" + portServer + ". Wait for players.");
				}
			}
			catch (Exception ex)
			{
				DisplayMessages("Server: Error: server start, " + ex.ToString());
			}
		}

		private void OnMessageReceivedFromClient(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		{
			try
			{
				DataReader reader = args.GetDataReader();
				reader.InputStreamOptions = InputStreamOptions.Partial;
				uint bytesRead = reader.UnconsumedBufferLength;
				string message = reader.ReadString(bytesRead);
				DisplayMessages("Server: Message received from [" + args.RemoteAddress.DisplayName + "]:" + args.RemotePort + ": " + message);
				var array = message.Split(' ').Select(s => s.Trim()).ToArray();
				if (array[0].Equals("connect"))
				{
					DisplayMessages("Server: Player " + array[1] + " connected " + ++numberOfPlayer);
					if (numberOfPlayer == 1)
					{
						sendFromServer("You are connected!", args.RemoteAddress.DisplayName, args.RemotePort);
						player1.name = array[1];
						player1.ipAdress = args.RemoteAddress.DisplayName;
						player1.portListener = args.RemotePort;
						sendFromServer("play", player1.ipAdress, player1.portListener);
					}
					else if (numberOfPlayer == 2)
					{
						sendFromServer("You are connected!", args.RemoteAddress.DisplayName, args.RemotePort);
						player2.name = array[1];
						player2.ipAdress = args.RemoteAddress.DisplayName;
						player2.portListener = args.RemotePort;
						sendFromServer("play", player1.ipAdress, player1.portListener);
						sendFromServer("play", player2.ipAdress, player2.portListener);
					}

				}
			}
			catch (Exception ex)
			{
				DisplayMessages("Server: Peer didn't receive message." + ex.ToString());
			}
		}

		private async void sendFromServer(string message, string adress, string portListener)
		{

			socketServer.MessageReceived += OnMessageReceivedFromClient;
			try
			{
				await socketServer.ConnectAsync(new HostName(adress), portListener);
				DataWriter writer = new DataWriter(socketServer.OutputStream);
				uint messageLength = writer.MeasureString(message);
				writer.WriteString(message);
				uint bytesWritten = await writer.StoreAsync();
				Debug.Assert(bytesWritten == messageLength);
				DisplayMessages("Server: Message sent: " + message + " to: " + adress + ", portListener: " + portListener);
			}
			catch (Exception ex)
			{
				DisplayMessages("sERVER: sendFromServer " + ex.ToString());
			}
		}

		//client
		private async void startClient()
		{
			try
			{
				if (playerSocket == null)
				{
					playerSocket = new DatagramSocket();
					playerSocket.MessageReceived += OnMessageReceivedFromServer;
					DisplayMessages("Client: Running, ip:" + GetLocalIPv4() + " portListener:" + portClient);
					await playerSocket.BindServiceNameAsync(portClient);
				}
			}
			catch (Exception ex)
			{
				DisplayMessages("Client: Error: client start, " + ex.ToString());
			}
		}

		private async void sendFromClient(string message, string adress, string portListener)
		{
			playerSocket.MessageReceived += OnMessageReceivedFromServer;
			try
			{
				await playerSocket.ConnectAsync(new HostName(adress), portListener);
				DataWriter writer = new DataWriter(playerSocket.OutputStream);
				uint messageLength = writer.MeasureString(message);
				writer.WriteString(message);
				uint bytesWritten = await writer.StoreAsync();
				Debug.Assert(bytesWritten == messageLength);
				DisplayMessages("Player Message sent: " + message + " to: " + adress + ", portListener: " + portListener);
			}
			catch (Exception ex)
			{
				DisplayMessages("Client: sendFromClient " + ex.ToString());
			}
		}

		private void OnMessageReceivedFromServer(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		{
			try
			{
				DataReader reader = args.GetDataReader();
				reader.InputStreamOptions = InputStreamOptions.Partial;
				uint bytesRead = reader.UnconsumedBufferLength;
				string message = reader.ReadString(bytesRead);
				DisplayMessages("Player: Message received from [" + args.RemoteAddress.DisplayName + "]:" + args.RemotePort + ": " + message);
				//actuallPosition();
				if (play)
				{
					sendFromClient("x " + ball.Margin.Left, GetLocalIPv4(), portServer);
				}
				else if (message.Equals("play"))
				{
					play = true;
				}
			}
			catch (Exception ex)
			{
				DisplayMessages("Player: Peer didn't receive message. " + ex.ToString());
			}
		}

		//connection
		public static string GetLocalIPv4()
		{
			var icp = NetworkInformation.GetInternetConnectionProfile();

			if (icp != null && icp.NetworkAdapter != null)
			{
				var hostname =
					NetworkInformation.GetHostNames()
						.SingleOrDefault(
							hn =>
							hn.IPInformation != null && hn.IPInformation.NetworkAdapter != null
							&& hn.IPInformation.NetworkAdapter.NetworkAdapterId
							== icp.NetworkAdapter.NetworkAdapterId);

				if (hostname != null)
				{
					// the ip address
					return hostname.CanonicalName;
				}
			}

			return null;
		}

		private void disconnectServer()
		{
			if (socketServer != null)
			{
				socketServer.Dispose();
				socketServer = null;
				DisplayMessages("Disconnected.");
			}
		}
    }*/
}
