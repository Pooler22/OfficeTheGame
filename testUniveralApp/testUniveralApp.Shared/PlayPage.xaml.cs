﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace testUniveralApp
{
    public sealed partial class PlayPage : Page
    {
		int speedPlayer;
		int numberOfPlayer = 0;
		string name { get; set; }
		string type{ get; set; }
		string ipAddress, port, message;
		DatagramSocket receiveSocketServer;
		DatagramSocket sendSocket;
		DatagramSocket player1, player2;
		ConnectionProfile connectionProfile;
				
		public PlayPage()
        {
            this.InitializeComponent();
			
			speedPlayer = 10;
			message = null;
			receiveSocketServer = null;
			ipAddress = "192.168.1.101";
			port = "2704";
			connectionProfile = NetworkInformation.GetInternetConnectionProfile();
		}

		private async void DisplayMessages(string message)
		{
			await Dispatcher.RunIdleAsync((unused) =>
			{
				view.Items.Add(message);
				view.ScrollIntoView(message);
			});
		}

		private void playerButtonMovePointer(object sender, PointerRoutedEventArgs e)
		{
			playerButton.Margin = new Thickness(
				e.GetCurrentPoint(this).Position.X - (playerButton.Width / 2.0),
				playerButton.Margin.Top,
				playerButton.Margin.Right,
				playerButton.Margin.Bottom);
		}

		private void Button_Click_Back_To_MainPage(object sender, RoutedEventArgs e)
		{
			disconnectServer();
			this.Frame.GoBack();
		}

		private void playerButtonMoveKeys(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key == Windows.System.VirtualKey.Left)
			{
				playerButton.Margin = new Thickness(playerButton.Margin.Left - speedPlayer,
					playerButton.Margin.Top,
					playerButton.Margin.Right,
					playerButton.Margin.Bottom);
			}
			else if (e.Key == Windows.System.VirtualKey.Right)
			{
				playerButton.Margin = new Thickness(playerButton.Margin.Left + speedPlayer,
					playerButton.Margin.Top,
					playerButton.Margin.Right,
					playerButton.Margin.Bottom);
			}
		}

		private void setStartPlayersPositions(object sender, RoutedEventArgs e)
		{
			playerButton.Margin = new Thickness(
				0,
				playPanel.ActualHeight - (playerButton.Height),
				playerButton.Margin.Right,
				playerButton.Margin.Bottom);
			enemyButton.Margin = new Thickness(
				0,
				0,
				enemyButton.Margin.Right,
				enemyButton.Margin.Bottom);
		}

		private void send_Click(object sender, RoutedEventArgs e)
		{
			sendToClient("connect " + name.ToString(), ip.Text.ToString());
		}




	
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.name = e.Parameter as string;
			this.type = name.Substring(0,1);
			this.name = name.Substring(1);
			playerButton.Content = name;
			//enemyButton.Content = type;
			loadingBar.IsEnabled = false;
			
			if(type.Equals("s"))
			{
				startServer();
				sendToClient("connect " + name, GetLocalIPv4());
			}
			else if (type.Equals("c"))
			{
				//sendToClient("connect " + name);
			}
		}

		private async void startServer()
		{
			try
			{
				if (receiveSocketServer == null)
				{
					receiveSocketServer = new DatagramSocket();
					receiveSocketServer.MessageReceived += OnMessageReceivedToServer;
					DisplayMessages("Server: Running. Wait for players.");
					await receiveSocketServer.BindServiceNameAsync(port);
				}
			}
			catch (Exception ex)
			{
				DisplayMessages("Server: Error: server start, " + ex.ToString());
			}
		}
		
		private void OnMessageReceivedToServer(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		{
			try
			{
				DataReader reader = args.GetDataReader();
				reader.InputStreamOptions = InputStreamOptions.Partial;
				uint bytesRead = reader.UnconsumedBufferLength;
				string message = reader.ReadString(bytesRead);
				DisplayMessages("Server: Message received from [" + args.RemoteAddress.DisplayName + "]:" + args.RemotePort + ": " + message);
				var array = message.Split(' ').Select(s => s.Trim()).ToArray();
				if (array[0].Equals("connect"))
				{
					DisplayMessages("Player " + array[1] + " connected " + ++numberOfPlayer);
					if (numberOfPlayer == 1)
					{
						sendFromServer("You are connected!", args.RemoteAddress.DisplayName, args.RemotePort);
					}
					else if (numberOfPlayer == 2)
					{
						sendFromServer("You are connected!", args.RemoteAddress.DisplayName, args.RemotePort);
					}
					else
					{
						//TODO server busy 
					}
				}
				this.message = message;
			}
			catch (Exception ex)
			{
				DisplayMessages("Peer didn't receive message.");
			}
		}
		
		private async void sendFromServer(string message, string adress, string port)
		{
			DatagramSocket sendSocket = new DatagramSocket();
			sendSocket.MessageReceived += OnMessageReceivedToClient;
			try
			{
				await sendSocket.ConnectAsync(new HostName(adress), port);
				DataWriter writer = new DataWriter(sendSocket.OutputStream);
				uint messageLength = writer.MeasureString(message);
				writer.WriteString(message);
				uint bytesWritten = await writer.StoreAsync();
				Debug.Assert(bytesWritten == messageLength);
				DisplayMessages("Message sent: " + message);
			}
			catch (Exception ex)
			{
				DisplayMessages(ex.ToString());
			}
		}


		private void OnMessageReceivedToClient(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		{
			try
			{
				DataReader reader = args.GetDataReader();
				reader.InputStreamOptions = InputStreamOptions.Partial;
				uint bytesRead = reader.UnconsumedBufferLength;
				string message = reader.ReadString(bytesRead);
				DisplayMessages("Message received from [" + args.RemoteAddress.DisplayName + "]:" + args.RemotePort + ": " + message);
				this.message = message;
			}
			catch (Exception ex)
			{
				DisplayMessages("Peer didn't receive message.");
			}
		}

		private async void sendToClient(string message, string ip)
		{
			DatagramSocket sendSocket = new DatagramSocket();
			sendSocket.MessageReceived += OnMessageReceivedToClient;
			try
			{
				await sendSocket.ConnectAsync(new HostName(ip), port);
				DataWriter writer = new DataWriter(sendSocket.OutputStream);
				uint messageLength = writer.MeasureString(message);
				writer.WriteString(message);
				uint bytesWritten = await writer.StoreAsync();
				Debug.Assert(bytesWritten == messageLength);
				DisplayMessages("Message sent: " + message);
			}
			catch (Exception ex)
			{
				DisplayMessages(ex.ToString());
			}
		}


		public static string GetLocalIPv4()
		{
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

		private void disconnectServer()
		{
			if (receiveSocketServer != null)
			{
				receiveSocketServer.Dispose();
				receiveSocketServer = null;
				DisplayMessages("Disconnected.");
			}
		}
	}
}
