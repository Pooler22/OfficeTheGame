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
		string port;

		public UDPClient(PlayPage page, string port)
		{
			this.page = page;
			this.port = port;
		}

		public async void Start()
		{
			try
			{
				listener = new DatagramSocket();
				listener.MessageReceived += SocketOnMessageReceived;
				await listener.BindServiceNameAsync(port);
				page.DisplayMessages("Start UDP server");
			}
			catch (Exception ex)
			{
				page.DisplayMessages("Error: Start UDP server" + ex.ToString());
			}
		}

		private async void SocketOnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		{
			var result = args.GetDataStream();
			//var resultStream = result.AsStreamForRead();
			var dataReader = new DataReader(result);

			try
			{
				byte[] destination = new byte[4];
				await dataReader.LoadAsync(4);
				dataReader.ReadBytes(destination);

				await dataReader.LoadAsync(sizeof(byte));
				byte msgType = dataReader.ReadByte();

				await dataReader.LoadAsync(sizeof(Int32));
				int length = dataReader.ReadInt32();

				byte[] bData = new byte[Math.Max(length - 1, 0)];
				await dataReader.LoadAsync((uint)length);
				dataReader.ReadBytes(bData);

				dataReader.Dispose();
				page.DisplayMessages("Start received UDP Client");
				if (OnDataReceived != null) OnDataReceived(destination, msgType, bData);
			}
			catch(Exception ex)
			{
				page.DisplayMessages("Error: Start received UDP Client" + ex.ToString());
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
			using (var stream = await sender.GetOutputStreamAsync(new HostName(_host), port))
			{
				using (var writer = new DataWriter(stream))
				{
					writer.WriteByte(msgType);
					writer.WriteInt32(message.Length);
					writer.WriteBytes(message);
					await writer.StoreAsync();
				}
			}
			sender.Dispose();
		}
	}
}
