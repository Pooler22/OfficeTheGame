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
		string port;
		string portFinder;
		DatagramSocket sender;
		DatagramSocket listener;
		PlayPage playPage;

		public UDPClientFinder(PlayPage playPage, string port)
		{
			this.port = port;
			this.playPage = playPage;
			this.portFinder = "3659";
		}

		public async void Start()
		{
			try
			{
				listener = new DatagramSocket();
				listener.MessageReceived += MessageReceived;
				await listener.BindEndpointAsync(null, port.ToString());
				//await listener.BindEndpointAsync(new HostName(GetLocalIPv4()), portFinder);
				playPage.DisplayMessages("UDP Finder start");
			}
			catch (Exception ex)
			{
				playPage.DisplayMessages("Error: UDP Finder start " + ex.ToString());
			}
		}
		
		private async void MessageReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs args)
		{
			try
			{
				DataReader reader = args.GetDataReader();
				reader.InputStreamOptions = InputStreamOptions.Partial;
				uint bytesRead = reader.UnconsumedBufferLength;
				string message = reader.ReadString(bytesRead);

				playPage.DisplayMessages("Message received from [" +
					args.RemoteAddress.DisplayName.ToString() + "]:" + args.RemotePort + ": " + message);

				reader.Dispose();

				string meaasge2 = "ready " + " port ip";
				byte[] bytes1 = new byte[meaasge2.Length * sizeof(char)];
				System.Buffer.BlockCopy(meaasge2.ToCharArray(), 0, bytes1, 0, bytes1.Length);

				await SendMessage(bytes1, args.RemoteAddress.DisplayName.ToString(), message);
			}
			catch (Exception ex)
			{
				playPage.DisplayMessages("ERROR: Message received from:");
			}
		}
		
		public void Stop()
		{
			if (sender != null) 
				sender.Dispose();
			if (listener != null) 
				listener.Dispose();
		}

		public async Task SendMessage(byte[] message, string host, string port)
		{
			sender = new DatagramSocket();
			using (var stream = await sender.GetOutputStreamAsync(new HostName(host), port))
			{
				using (var writer = new DataWriter(stream))
				{
					try
					{
						writer.WriteBytes(message);
						await writer.StoreAsync();
						playPage.DisplayMessages("Send Message port: " + host + " " + port);
					}
					catch (Exception ex)
					{
						playPage.DisplayMessages("Error Send Message");
					}
				}
			}
			sender.Dispose();
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
			string str = listener.Information.LocalPort.ToString();
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			await SendMessage(bytes, "255.255.255.255", port);
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
