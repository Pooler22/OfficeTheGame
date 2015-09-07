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
		string portListener, name;
		string portSender = "3659";
		DatagramSocket sender;
		DatagramSocket listener;
		PlayPage playPage;

		public UDPClient(PlayPage page, string port, string name)
		{
			this.playPage = page;
			this.portListener = port;
			this.name = name;

			bytes = new byte[name.Length * sizeof(char)];
			System.Buffer.BlockCopy(name.ToCharArray(), 0, bytes, 0, bytes.Length);
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

		public async void Start()
		{
			try
			{
				sender = new DatagramSocket();
				listener = new DatagramSocket();
				listener.MessageReceived += MessageReceived;
				listener.BindEndpointAsync(new HostName(LocalIPAddress()), portListener);
				//await listener.BindEndpointAsync(new HostName("192.168.1.102"), portSender);
				//await listener.BindServiceNameAsync(portSender);
				playPage.DisplayMessages("Start UDP server");
			}
			catch (Exception ex)
			{
				playPage.DisplayMessages("Error: Start UDP server" + ex.ToString());
			}
		}

		private void MessageReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs args)
		{
			try
			{
				DataReader reader = args.GetDataReader();
				reader.InputStreamOptions = InputStreamOptions.Partial;
				uint bytesRead = reader.UnconsumedBufferLength;
				string message = reader.ReadString(bytesRead);

				playPage.DisplayMessages("Message received from [" +
					args.RemoteAddress.DisplayName.ToString() + "]:" + args.RemotePort + ": " + message);

				string meaasge2 = "ready " + LocalIPAddress() + " ";
				playPage.DisplayMessages("Message prepare " + meaasge2);
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

		public void Stop()
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
						playPage.DisplayMessages("Send Message host: " + host + " portSender: " + port);
					}
					catch(Exception ex)
					{
						playPage.DisplayMessages("Error Send Message");
					}
				}
			}
		}
	}
}
