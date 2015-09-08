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
		string name;
		StreamSocketListener listener = null;
		StreamSocket sender = null;
		PlayPage playPage;

		public ConnectionTCP(PlayPage playPage, string name)
		{
			this.playPage = playPage;
			this.name = name;	
		}

		//listener

		public void initListener(string portListener)
		{
			Task.Run(
				async () =>
				{
					await Listener(portListener);
				})
				.Wait();
		}

		private async Task Listener(string portListener)
		{
			try
			{
				if (listener == null)
				{
					listener = new StreamSocketListener();
					listener.ConnectionReceived += OnConnectionReceived;
					await listener.BindServiceNameAsync(portListener.ToString());
					DisplayMessages("TCP Listener[" + portListener + "] started");
				}
			}
			catch (Exception ex)
			{
				DisplayMessages("ERROR: TCP Listener[" + portListener + "] started");
			}
		}

		private async void OnConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
		{
			try
			{
				DisplayMessages(name + " " + args.Socket.Information.RemoteAddress.DisplayName + " connected.");
				while (true)
				{
					string request = await Read(args.Socket.InputStream);
					if (String.IsNullOrEmpty(request))
					{
						return;
					}
					DisplayMessages(request);
					string response = "Respone.\r\n";
					await Send(args.Socket.OutputStream, response);
				}
			}
			catch (Exception ex)
			{
				DisplayMessages(ex.ToString());
			}
		}

		private void DisposeListener()
		{
			if (listener != null)
			{
				listener.Dispose();
				listener = null;
				DisplayMessages(name + "TCP Dispose listening");
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
					DisplayMessages(name + " The connection was closed by remote host.");
					break;
				}
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
			writer.DetachStream();
		}

		//sender

		public void initSender(string portSender, string remoteAdress)
		{
			Task.Run(
				async () =>
				{
					await Sender(portSender, remoteAdress);
				})
				.Wait();
		}

		private async Task Sender(string portSender, string remoteAdress)
		{
			try
			{
				sender = new StreamSocket();
				await sender.ConnectAsync(new HostName(remoteAdress), portSender);
				DisplayMessages("TPC Sender[" + remoteAdress + ":" + portSender + "] started");
			}
			catch (Exception ex)
			{
				DisplayMessages("ERROR: TCP Sender[" + remoteAdress + ":" + portSender + "] started" + ex.ToString());
			}
		}

		public async void SendRequest(string request)
		{
			if (sender != null)
			{
				request += "\r\n";
				await Send(sender.OutputStream, request);
				string response = await Read(sender.InputStream);
				DisplayMessages(response);
			}
		}

		private void DisconnectSender()
		{
			if (sender != null)
			{
				sender.Dispose();
				sender = null;
				DisplayMessages("TCP Disconnect sender");
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
					return hostname.CanonicalName;
				}
			}

			return null;
		}

		private void DisplayMessages(string message)
		{
			playPage.DisplayMessages(message);
		}

		internal void Dispose()
		{
			if (listener != null)
				listener.Dispose();
			if (sender != null)
				sender.Dispose();
		}
	}
}
