using System.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Linq;
using System.IO;
using System.Collections.Concurrent;
using System.Diagnostics;
using Windows.Networking.Connectivity;
using System.Globalization;

using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Runtime.InteropServices.WindowsRuntime;
namespace testUniveralApp
{
	class ConnectionUDP
	{
		DatagramSocket listener;
		PlayPage playPage;
		string name;
		int port;
		
		public ConnectionUDP(PlayPage playPage, string name)
		{
			this.playPage = playPage;
			this.name = name;
		}

		//Server

		public delegate void DataReceivedEvent(byte[] dest, byte msgType, byte[] data);
		public event DataReceivedEvent OnDataReceived;
		DatagramSocket socket;
		DatagramSocket socket2;

		public void initUDPListener(int port)
		{
			this.port = port;
			Task.Run(async
				() =>
			{
				await this.startServer(port);
			})
				.Wait();
		}

		private async Task startServer(int port)
		{
			try
			{
				if (listener == null)
				{
					listener = new DatagramSocket();
					listener.MessageReceived += SocketOnMessageReceived;
					await listener.BindEndpointAsync(null, port.ToString());
					DisplayMessages("Server UDP runing...");
				}
			}
			catch (Exception ex)
			{
				DisplayMessages("Server UDP error: " + ex.ToString());
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
				DisplayMessages("Server UDP received: " + dataReader);
				dataReader.Dispose();

				if (OnDataReceived != null) OnDataReceived(destination, msgType, bData);
			}
			catch
			{ //TODO: Error handler
			}
		}

		public void Stop()
		{
			if (socket != null) socket.Dispose();
			if (socket2 != null) socket2.Dispose();
		}

		public async Task SendMessage(byte msgType, byte[] message, byte[] host)
		{
			socket = new DatagramSocket();
			string _host = host[0].ToString() + "." + host[1].ToString() + "." + host[2].ToString() + "." + host[3].ToString();
			using (var stream = await socket.GetOutputStreamAsync(new HostName(_host), port.ToString()))
			{
				using (var writer = new DataWriter(stream))
				{
					writer.WriteByte(msgType);
					writer.WriteInt32(message.Length);
					writer.WriteBytes(message);
					await writer.StoreAsync();
				}
			}
			socket.Dispose();
		}



		//private void OnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		//{
		//	try
		//	{
		//		DataReader reader = args.GetDataReader();
		//		reader.InputStreamOptions = InputStreamOptions.Partial;
		//		uint bytesRead = reader.UnconsumedBufferLength;
		//		string message = reader.ReadString(bytesRead);
		//		DisplayMessages("Server UDP received: " + message);
		//		sendFromClient(name, args.RemoteAddress.DisplayName, args.RemotePort);
		//	}
		//	catch (Exception ex)
		//	{
		//		DisplayMessages("Server UDP didn't receive message. " + ex.ToString());
		//	}
		//}

		//private async void sendFromClient(string message, string adress, string portListener)
		//{
		//	listener.MessageReceived += OnMessageReceived;
		//	try
		//	{
		//		await listener.ConnectAsync(new HostName(adress), portListener);
		//		DataWriter writer = new DataWriter(listener.OutputStream);
		//		uint messageLength = writer.MeasureString(message);
		//		writer.WriteString(message);
		//		uint bytesWritten = await writer.StoreAsync();
		//		DisplayMessages("Server UDP sent: " + message);
		//	}
		//	catch (Exception ex)
		//	{
		//		DisplayMessages("Server UDP sent error: " + ex.ToString());
		//	}
		//}

		//Client

		public void initUDPFinder(int port)
		{
			Task.Run(async
				() =>
			{
				await SendMessage(port);
			})
				.Wait();
		}

		public async Task SendMessage(int port)
		{
			var socket = new DatagramSocket();
			socket.Control.DontFragment = true;
			socket.MessageReceived += SocketOnMessageReceived1;

			using (var stream = await socket.GetOutputStreamAsync(new HostName("255.255.255.255"), port.ToString()))
			{
				using (var writer = new DataWriter(stream))
				{
					var data = Encoding.UTF8.GetBytes(name);
					DisplayMessages("Client UDP send name " + name);
					writer.WriteBytes(data);
					writer.StoreAsync();
				}
			}
		}

		private async void SocketOnMessageReceived1(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		{
			var result = args.GetDataStream();
			var resultStream = result.AsStreamForRead(1024);

			using (var reader = new StreamReader(resultStream))
			{
				var text = await reader.ReadToEndAsync();
				DisplayMessages("Client UDP get " + text);
			}
		}
		


		private void disconnectServer()
		{
			if (listener != null)
			{
				listener.Dispose();
				listener = null;
				DisplayMessages("Disconnected UDP.");
			}
		}

		private void DisplayMessages(string message)
		{
			playPage.DisplayMessages(name + ": " + message);
		}

	}
}
