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

namespace testUniveralApp
{
	class ConnectionUDP
	{
		DatagramSocket listener;
		PlayPage playPage;
		string name;
		
		public ConnectionUDP(PlayPage playPage, string name)
		{
			this.playPage = playPage;
			this.name = name;
		}

		//Server

		public void initServer(int port)
		{
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
					listener.MessageReceived += OnMessageReceived;
					await listener.BindEndpointAsync(new HostName("255.255.255.255"), 4000.ToString());
					DisplayMessages("Server UDP runing...");
				}
			}
			catch (Exception ex)
			{
				DisplayMessages("Server UDP error: " + ex.ToString());
			}
		}

		private void OnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		{
			try
			{
				DataReader reader = args.GetDataReader();
				reader.InputStreamOptions = InputStreamOptions.Partial;
				uint bytesRead = reader.UnconsumedBufferLength;
				string message = reader.ReadString(bytesRead);
				DisplayMessages("Server UDP received: " + message);
				sendFromClient(name, args.RemoteAddress.DisplayName, args.RemotePort);
			}
			catch (Exception ex)
			{
				DisplayMessages("Server UDP didn't receive message. " + ex.ToString());
			}
		}

		private async void sendFromClient(string message, string adress, string portListener)
		{
			listener.MessageReceived += OnMessageReceived;
			try
			{
				await listener.ConnectAsync(new HostName(adress), portListener);
				DataWriter writer = new DataWriter(listener.OutputStream);
				uint messageLength = writer.MeasureString(message);
				writer.WriteString(message);
				uint bytesWritten = await writer.StoreAsync();
				DisplayMessages("Server UDP sent: " + message);
			}
			catch (Exception ex)
			{
				DisplayMessages("Server UDP sent error: " + ex.ToString());
			}
		}

		//Client

		public void initClient(int port)
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
			socket.MessageReceived += SocketOnMessageReceived;

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

		private async void SocketOnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
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
