using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using System.IO;
using Windows.Storage.Streams;
using Windows.Networking.Connectivity;

namespace testUniveralApp
{
	class UDPClient
	{
		byte[] bytes;
		string name, portListener, portSender;
		DatagramSocket sender, listener;
		PlayPage playPage;

		public UDPClient(PlayPage page, string portListener, string name)
		{
			portSender = "4441";
			this.portListener = portListener;
			this.playPage = page;
			this.name = name;

			bytes = new byte[name.Length * sizeof(char)];
			System.Buffer.BlockCopy(name.ToCharArray(), 0, bytes, 0, bytes.Length);
		}
		
		public static string LocalIPAddress()
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

		public void Start()
		{
			Task.Run(
				async () =>
				{
					await initClient();
				});
		}

		public async Task initClient()
		{
			try
			{
				listener = new DatagramSocket();
				listener.MessageReceived += MessageReceived;
				listener.BindEndpointAsync(new HostName(LocalIPAddress()), portListener);
				playPage.DisplayMessages("UDP Listener [local]:" + portListener + " started");
			}
			catch (Exception ex)
			{
				playPage.DisplayMessages("ERROR: UDP Listener [local]:" + portListener + " started\n" + ex.ToString());
			}
		}

		private void MessageReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs args)
		{
			try
			{
				DataReader reader = args.GetDataReader();
				reader.InputStreamOptions = InputStreamOptions.Partial;
				uint bytesRead = reader.UnconsumedBufferLength;
				String message = reader.ReadString(bytesRead);

				playPage.DisplayMessages("Message received from [" +
					args.RemoteAddress.DisplayName.ToString() + "]:" + args.RemotePort + ": " + message);

				string meaasge2 = LocalIPAddress() + " " + name;
				byte[] bytes1 = new byte[meaasge2.Length * sizeof(char)];
				System.Buffer.BlockCopy(meaasge2.ToCharArray(), 0, bytes1, 0, bytes1.Length);
				SendMessage(bytes1, "255.255.255.255", portSender);
				reader.Dispose();
			}
			catch (Exception ex)
			{
				playPage.DisplayMessages("ERROR: Message received from:");
			}			
		}

		public void Dispose()
		{
			if (sender != null) 
				sender.Dispose();
			if (listener != null) 
				listener.Dispose();
		}
		
		public async Task SendMessage(byte[] message, string host, string port)
		{
			sender = new DatagramSocket();
			try
			{
				using (var stream = await sender.GetOutputStreamAsync(new HostName(host), port))
				{
					var writer = new DataWriter(stream);
					writer.WriteBytes(message);
					await writer.StoreAsync();
					playPage.DisplayMessages("Send Message host: " + host + " portSender: " + port);
				}
			}
			catch
			{
				playPage.DisplayMessages("Error: Send Message");
			}
			sender.Dispose();
		}
	}
}
