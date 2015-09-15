using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using testUniveralApp.Class;

namespace testUniveralApp
{
	class UDPListenerFinder
	{
		string portListener, portSender, name;
		DatagramSocket listener, sender;
		PlayPage playPage;

		public UDPListenerFinder(PlayPage playPage, string name, string portListener, string portSender)
		{
			this.playPage = playPage;
			this.name = name;
			this.portListener = portListener;
			this.portSender = portSender;

            Task.Run(
                async () =>
                {
                    await initFinder();
                })
                .Wait();
        }

		async Task initFinder()
		{
			try
			{
				sender = new DatagramSocket();
				listener = new DatagramSocket();
				listener.MessageReceived += MessageReceived;
				await listener.BindEndpointAsync(new HostName(IPAdress.LocalIPAddress()), portListener);
				playPage.DisplayMessages(name + " :UDP Finder [local]:" + portListener + " started");
			}
			catch (Exception ex)
			{
				playPage.DisplayMessages(name + " :ERROR: UDP Finder [local]:" + portListener + " started\n" + ex.ToString());
			}
		}
		
		private void MessageReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs args)
		{
			playPage.DisplayMessages(name + " :MessageReceived");
			try
			{
				DataReader reader = args.GetDataReader();
				reader.InputStreamOptions = InputStreamOptions.Partial;
				uint bytesRead = reader.UnconsumedBufferLength;
				string message = reader.ReadString(bytesRead);
				string msg = message.Replace("\0", string.Empty);
				playPage.DisplayMessages(name + " :Message received from [" +
					args.RemoteAddress.DisplayName.ToString() + "]:" + args.RemotePort + ": " + message);

				reader.Dispose();
				playPage.AddServer(msg);
			}
			catch (Exception ex)
			{
				playPage.DisplayMessages(name + " :ERROR: Message received from:" + ex.ToString());
			}
		}
		
		public void Dispose()
		{
			if (sender != null) 
				sender.Dispose();
			if (listener != null) 
				listener.Dispose();
		}

		public async void SendDiscovery()
		{
			string str = "discovery";
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            await SendMessage(bytes, "255.255.255.255", portSender);
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
						playPage.DisplayMessages(name + " :Send message");
					}
					catch (Exception ex)
					{
						playPage.DisplayMessages(name + " :Error Send message\n" + ex.ToString());
					}
				}
			}
		}
	}
}
