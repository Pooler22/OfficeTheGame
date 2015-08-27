﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

using System.Net;
using System.Windows;
using System.Windows.Input;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Networking;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace testUniveralApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayPage : Page
    {
		private StreamSocket _socket = new StreamSocket();
		private StreamSocketListener _listener = new StreamSocketListener();
		private List<StreamSocket> _connections = new List<StreamSocket>();
  
		public PlayPage()
        {
            this.InitializeComponent();
        }

		private void Button_Click_Back_To_MainPage(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(MainPage));
		}

		async private void WaitForData(StreamSocket socket)
		{
			var dr = new DataReader(socket.InputStream);
			//dr.InputStreamOptions = InputStreamOptions.Partial;
			var stringHeader = await dr.LoadAsync(4);

			if (stringHeader == 0)
			{
				LogMessage(string.Format("Disconnected (from {0})", socket.Information.RemoteHostName.DisplayName));
				return;
			}

			int strLength = dr.ReadInt32();

			uint numStrBytes = await dr.LoadAsync((uint)strLength);
			string msg = dr.ReadString(numStrBytes);

			LogMessage(string.Format("Received (from {0}): {1}", socket.Information.RemoteHostName.DisplayName, msg));

			WaitForData(socket);
		}

		async private void Connect(object sender, RoutedEventArgs e)
		{
			try
			{
				await _socket.ConnectAsync(new HostName("127.0.0.1"), "7");
				LogMessage(string.Format("Connected to {0}", _socket.Information.RemoteHostName.DisplayName));
				WaitForData(_socket);
			}
			catch (Exception ex)
			{
				LogMessage(string.Format("Connected false"));
			}
		}

		async private void Listen(object sender, RoutedEventArgs e)
		{
			_listener.ConnectionReceived += listenerConnectionReceived;
			await _listener.BindServiceNameAsync("7");
			LogMessage(string.Format("listening on {0}...", _listener.Information.LocalPort));
		}

		void listenerConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
		{
			_connections.Add(args.Socket);

			LogMessage(string.Format("Incoming connection from {0}", args.Socket.Information.RemoteHostName.DisplayName));

			WaitForData(args.Socket);
		}

		private void LogMessage(string message)
		{
			outTextBlock.Text = message;
		}

		private void Send(object sender, RoutedEventArgs e)
		{
			SendMessage(_socket, inTextBox.Text);
			inTextBox.Text = "";
		}

		async private void SendMessage(StreamSocket socket, string message)
		{
			var writer = new DataWriter(socket.OutputStream);
			var len = writer.MeasureString(message); // Gets the UTF-8 string length.
			writer.WriteInt32((int)len);
			writer.WriteString(message);
			var ret = await writer.StoreAsync();
			writer.DetachStream();

			//LogMessage(string.Format("Sent (to {0}) {1}", socket.Information.RemoteHostName.DisplayName, message));
		}

		private void Reply(object sender, RoutedEventArgs e)
		{
			foreach (var sock in _connections)
			{
				SendMessage(sock, inTextBox.Text);
			}
		}

		private void inTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key == Windows.System.VirtualKey.Enter)
			{
				SendMessage(_socket, "asd");
				inTextBox.Text = "";
			}
		}
    }
}
