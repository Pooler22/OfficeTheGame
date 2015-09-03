using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;


namespace testUniveralApp
{
    class ConnectionTCP
    {
		PlayPage playPage;
		StreamSocketListener listener = null;
		StreamSocket sender = null;
		string name;

		public ConnectionTCP(PlayPage playPage, string name, int portListener)
		{
			this.playPage = playPage;
			this.name = name;
			
			Task.Run(
				async () =>
				{
					await StartListener(portListener);
				}).Wait();
		}

		private async Task StartListener(int port)
		{
			try
			{
				if (listener == null)
				{
					listener = new StreamSocketListener();
					listener.ConnectionReceived += OnConnectionReceived;
					await listener.BindServiceNameAsync(port.ToString());
					DisplayMessages("Listening.");
				}
			}
			catch (Exception ex)
			{
				DisplayMessages("Listening error: " + ex.ToString());
			}
		}

		private async void OnConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
		{
			try
			{
				DisplayMessages(args.Socket.Information.RemoteAddress.DisplayName + " connected.");
				while (true)
				{
					string request = await Read(args.Socket.InputStream);
					if (String.IsNullOrEmpty(request))
					{
						return;
					}
					DisplayMessages(request);
					string response = "Yes, I am ñoño. The time is " + DateTime.Now + ".\r\n";
					await Send(args.Socket.OutputStream, response);
				}
			}
			catch (Exception ex)
			{
				DisplayMessages(ex.ToString());
			}
		}

		private void StopListener()
		{
			if (listener != null)
			{
				listener.Dispose();
				listener = null;
				DisplayMessages("Not listening anymore.");
			}
		}

		private async Task<string> Read(IInputStream inputStream)
		{
			DataReader reader = new DataReader(inputStream);
			reader.InputStreamOptions = InputStreamOptions.Partial;

			string message = "";
			while (!message.EndsWith("\r\n"))
			{
				uint bytesRead = await reader.LoadAsync(16);
				if (bytesRead == 0)
				{
					DisplayMessages("The connection was closed by remote host.");
					break;
				}
				// TODO: Why DataReader doesn't have ReadChar()?
				message += reader.ReadString(bytesRead);
			}
			reader.DetachStream();
			return message;
		}

		private async Task Send(IOutputStream outputStream, string message)
		{
			DataWriter writer = new DataWriter(outputStream);
			uint messageLength = writer.MeasureString(message);
			writer.WriteString(message);
			uint bytesWritten = await writer.StoreAsync();
			//Debug.Assert(bytesWritten == messageLength);
			writer.DetachStream();
		}


		public void startSender(int portSender)
		{
			Task.Run(
				async () =>
				{
					await StartSender(portSender);
				}).Wait();
		}

		private async Task StartSender(int portSender)
		{
			try
			{
				sender = new StreamSocket();
				await sender.ConnectAsync(new HostName(GetLocalIPv4()), portSender.ToString());
				DisplayMessages( "Connected.");
			}
			catch (Exception ex)
			{
				DisplayMessages(ex.ToString());
			}
		}

		private async void SendRequest()
		{
			if (sender != null)
			{
				string request = "Are you ñoño? Can you tell me what time is it?\r\n";
				await Send(sender.OutputStream, request);

				string response = await Read(sender.InputStream);
				DisplayMessages( response);
			}
		}

		private void DisconnectSender()
		{
			if (sender != null)
			{
				sender.Dispose();
				sender = null;
				DisplayMessages("Closed.");
			}
		}


		public static string GetLocalIPv4()
		{
			ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
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


		private void DisplayMessages(string message)
		{
			playPage.DisplayMessages(name + ": " + message);
		}

    }
}
