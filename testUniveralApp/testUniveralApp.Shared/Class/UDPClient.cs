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
		public delegate void DataReceivedEvent(byte[] dest, byte msgType, byte[] data);
		public event DataReceivedEvent OnDataReceived;

		DatagramSocket sender;
		DatagramSocket listener;
		PlayPage page;
		string port, name;
		byte[] bytes;

		public UDPClient(PlayPage page, string port, string name)
		{
			this.page = page;
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
				await listener.BindServiceNameAsync(port);
				page.DisplayMessages("Start UDP server");
			}
			catch (Exception ex)
			{
				page.DisplayMessages("Error: Start UDP server" + ex.ToString());
			}
		}

		private async void MessageReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs args)
		{
			 try
            {
				/*
				 DataReader reader = args.GetDataReader();
				reader.InputStreamOptions = InputStreamOptions.Partial;
				page.DisplayMessages("start reveived");
				// LoadAsync not needed. The reader comes already loaded.

				// If called by a 'Udp send socket', next line throws an exception because message was not received.

				// If remote peer didn't received message, "An existing connection was forcibly
				// closed by the remote host. (Exception from HRESULT: 0x80072746)" exception is
				// thrown. Maybe only when using ConenctAsync(), not GetOutputStreamAsync().
				uint bytesRead = reader.UnconsumedBufferLength;
				string message = reader.ReadString(bytesRead);

				page.DisplayMessages("Message received from [" +
					args.RemoteAddress.DisplayName + "]:" + args.RemotePort + ": " + message);
				 */
				try
				{
					DataReader reader = args.GetDataReader();
					reader.InputStreamOptions = InputStreamOptions.Partial;
					uint bytesRead = reader.UnconsumedBufferLength;
					string message = reader.ReadString(bytesRead);

					page.DisplayMessages("Message received from [" +
						args.RemoteAddress.DisplayName.ToString() + "]:" + args.RemotePort + ": " + message);

					reader.Dispose();

					string meaasge2 = "ready " + name;
					byte[] bytes1 = new byte[meaasge2.Length * sizeof(char)];
					System.Buffer.BlockCopy(meaasge2.ToCharArray(), 0, bytes1, 0, bytes1.Length);

					await SendMessage(bytes1, args.RemoteAddress.ToString(), message);
					await SendMessage(bytes1, args.RemoteAddress.ToString(), args.RemotePort);
				}
				 catch(Exception ex)
				{
					 page.DisplayMessages("ERROR: Message received from:");
				}
            }
            catch (Exception ex)
            {
                page.DisplayMessages("Server didn't receive message." + ex.ToString());
            }
			 

		}

		public void Stop()
		{
			if (sender != null) sender.Dispose();
			if (listener != null) listener.Dispose();
		}

		public async Task SendMessage(byte msgType, byte[] message, byte[] host)
		{
			sender = new DatagramSocket();
			string _host = host[0].ToString() + "." + host[1].ToString() + "." + host[2].ToString() + "." + host[3].ToString();
			using (var stream = await sender.GetOutputStreamAsync(new HostName(_host), port.ToString()))
			{
				using (var writer = new DataWriter(stream))
				{
					//writer.WriteByte(msgType);
					writer.WriteInt32(message.Length);
					writer.WriteBytes(message);
					await writer.StoreAsync();
				}
			}
			sender.Dispose();
		}

		private async Task SendMessage(byte[] message, string host, string port)
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
						page.DisplayMessages("Send Message port: " + port);
					}
					catch(Exception ex)
					{
						page.DisplayMessages("Error Send Message");
					}
				}
			}

		}
	}
}
