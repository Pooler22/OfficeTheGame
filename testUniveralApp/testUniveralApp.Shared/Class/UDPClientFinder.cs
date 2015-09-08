using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

namespace testUniveralApp
{
	class UDPClientFinder
	{
		string portListener, portSender;
		DatagramSocket listener, sender;
		PlayPage playPage;

		public UDPClientFinder(PlayPage playPage, string portSender)
		{
			this.portSender = portSender;
			this.portListener = "4441";
			this.playPage = playPage;
		}

		public void Start()
		{
			Task.Run(
				async () =>
				{
					await initFinder();
				})
				.Wait();
		}

		public async Task initFinder()
		{
			try
			{
				sender = new DatagramSocket();
				listener = new DatagramSocket();
				listener.MessageReceived += MessageReceived;
				listener.BindEndpointAsync(new HostName(GetLocalIPv4()), portListener);
				playPage.DisplayMessages("UDP Finder [local]:" + portListener + " started");
			}
			catch (Exception ex)
			{
				playPage.DisplayMessages("ERROR: UDP Finder [local]:" + portListener + " started\n" + ex.ToString());
			}
		}
		
		private void MessageReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs args)
		{
			playPage.DisplayMessages("MessageReceived");
			try
			{
				DataReader reader = args.GetDataReader();
				reader.InputStreamOptions = InputStreamOptions.Partial;
				uint bytesRead = reader.UnconsumedBufferLength;
				string message = reader.ReadString(bytesRead);
				string msg = message.Replace("\0", string.Empty);
				playPage.DisplayMessages("Message received from [" +
					args.RemoteAddress.DisplayName.ToString() + "]:" + args.RemotePort + ": " + message);

				reader.Dispose();
				playPage.AddServer(msg);
			}
			catch (Exception ex)
			{
				playPage.DisplayMessages("ERROR: Message received from:" + ex.ToString());
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
			using (var stream = await sender.GetOutputStreamAsync(new HostName(host), port))
			{
				using (var writer = new DataWriter(stream))
				{
					try
					{
						writer.WriteBytes(message);
						await writer.StoreAsync();
						playPage.DisplayMessages("Send Message portSender: " + host + " " + port);
					}
					catch (Exception ex)
					{
						playPage.DisplayMessages("Error Send Message");
					}
				}
			}
		}
		
		public static string LocalIPAddress()
		{
			List<string> ipAddresses = new List<string>();
			var hostnames = NetworkInformation.GetHostNames();
			foreach (var hn in hostnames)
			{
				if (hn.IPInformation != null &&
					(hn.IPInformation.NetworkAdapter.IanaInterfaceType == 71 // Wifi
					|| hn.IPInformation.NetworkAdapter.IanaInterfaceType == 6)) // Ethernet (Emulator) 
				{
					string ipAddress = hn.DisplayName;
					ipAddresses.Add(ipAddress);
				}
				
			}
			
			if (ipAddresses.Count < 1)
			{
				return null;
			}
			else if (ipAddresses.Count == 1)
			{
				return ipAddresses[0];
			}
			else
			{
				//if multiple suitable address were found use the last one
				//(regularly the external interface of an emulated device)
				return ipAddresses[ipAddresses.Count - 1];
			}
		}

		public async void BroadcastIP()
		{
			string str = "discovery";
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			SendMessage(bytes,  "255.255.255.255", portSender);
		}

		byte[] IPMessage()
		{
			//IP Address to byte()
			string[] strIPTemp = LocalIPAddress().Split('.');
			if (strIPTemp.Length != 4) throw new Exception("Invalid IP Address");
			return Enumerable.Range(0, 5).Select(x =>
						(x == 0) ? (byte)1 : Convert.ToByte(strIPTemp[x - 1])).ToArray();
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
	}
}
