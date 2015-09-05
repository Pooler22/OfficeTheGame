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



namespace testUniveralApp
{
	class UDPClientFinder
	{
		public event ClientFoundEvent OnClientFound;
		public delegate void ClientFoundEvent(byte[] clientIP);

		string port;
		string portFinder;
		DatagramSocket socket;
		DatagramSocket listener;
		PlayPage playPage;

		public UDPClientFinder(PlayPage playPage, string port)
		{
			this.port = port;
			this.playPage = playPage;
			portFinder = "4001";
		}

		public async void Start()
		{
			try
			{
				listener = new DatagramSocket();
				listener.MessageReceived += MessageReceived;
				await listener.BindServiceNameAsync(portFinder);
				playPage.DisplayMessages("UDP Finder start");
			}
			catch (Exception ex)
			{
				playPage.DisplayMessages("Error: UDP Finder start " + ex.ToString());
			}
		}
		
		async void MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		{
			var result = args.GetDataStream();
			var resultStream = result.AsStreamForRead(1024);

			try
			{
				byte[] buffer = new byte[5];
				byte[] message = IPMessage();
				await resultStream.ReadAsync(buffer, 0, 5);
				for (int i = 0; i <= 5; i++) //compare arrays
				{
					if (buffer[i] != message[i])
					{
						if (buffer[0] == 1 && OnClientFound != null)
						{
							await SendMessage(message,
								args.RemoteAddress.ToString(), args.RemotePort);
							OnClientFound(buffer.Skip(1).ToArray());
							break;
						}
					}
				}
			}
			catch
			{ //TODO: Error handler
			}
		}

		public void Stop()
		{
			if (socket != null) 
				socket.Dispose();
			if (listener != null) 
				listener.Dispose();
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
			string str = listener.Information.LocalPort;
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			await SendMessage(bytes, "255.255.255.255", port);
		}

		async Task SendMessage(byte[] message, string host, string port)
		{
			socket = new DatagramSocket();

			using (var stream = await socket.GetOutputStreamAsync(new HostName(host), port))
			{
				using (var writer = new DataWriter(stream))
				{
					writer.WriteBytes(message);
					await writer.StoreAsync();
					playPage.DisplayMessages("Finder send message");
				}
			}
			socket.Dispose();
		}

		byte[] IPMessage()
		{
			//IP Address to byte()
			string[] strIPTemp = LocalIPAddress().Split('.');
			if (strIPTemp.Length != 4) throw new Exception("Invalid IP Address");
			return Enumerable.Range(0, 5).Select(x =>
						(x == 0) ? (byte)1 : Convert.ToByte(strIPTemp[x - 1])).ToArray();
		}
	}
}
