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

namespace testUniveralApp
{
	class UDPClient
	{
		byte[] bytes;
		string port, name;
		DatagramSocket sender;
		DatagramSocket listener;
		PlayPage playPage;

		public UDPClient(PlayPage page, string port, string name)
		{
			this.playPage = page;
			this.port = port;
			this.name = name;

			bytes = new byte[name.Length * sizeof(char)];
			System.Buffer.BlockCopy(name.ToCharArray(), 0, bytes, 0, bytes.Length);
		}

		public async void Start()
		{
			try
			{
				listener = new DatagramSocket();
				listener.MessageReceived += MessageReceived;
				await listener.BindEndpointAsync(null, port.ToString());
				//await listener.BindServiceNameAsync(port);
				playPage.DisplayMessages("Start UDP server");
			}
			catch (Exception ex)
			{
				playPage.DisplayMessages("Error: Start UDP server" + ex.ToString());
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
					catch(Exception ex)
					{
						playPage.DisplayMessages("Error Send Message");
					}
				}
			}
			sender.Dispose();
		}
	}
}
