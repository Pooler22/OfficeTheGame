using System;
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
		string portServer, portClient, message;
		
		DatagramSocket playerSocket;
		Player player1, player2;
		ConnectionProfile connectionProfile;
		bool play = false;
	
		public PlayPage()
        {
            this.InitializeComponent();

			speedPlayer = 10;
			message = null;
			player1 = null;
			player2 = null;
			socketServer = null;
			portServer = "2704";
			portClient = "2705";
			player1 = new Player();
			player2 = new Player();
			connectionProfile = NetworkInformation.GetInternetConnectionProfile();
		}

		//page
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.name = e.Parameter as string;
			this.type = name.Substring(0, 1);
			this.name = name.Substring(1);
			playerButton.Content = name;
			//enemyButton.Content = type;
			loadingBar.IsEnabled = false;
			if (type.Equals("s"))
			{
				startServer();
				Task.WaitAll();
				startClient();
				Task.WaitAll();
				sendFromClient("connect " + name, GetLocalIPv4(), portServer);
			}
			else if (type.Equals("c"))
			{
				startClient();
				//sendFromClient("connect " + name, GetLocalIPv4(), portClient);
			}
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
			sendFromClient("connect " + name.ToString(), ip.Text.ToString(), portTB.Text);
		}

		//server
		private async void initServer()
		{
			Task task = Task.Run(async () => { await this.startServer(); });
			task.Wait();
		}

		private async Task startServer()
		{
			try
			{
				if (socketServer == null)
				{
					socketServer = new DatagramSocket();
					socketServer.MessageReceived += OnMessageReceivedFromClient;
					await socketServer.BindServiceNameAsync(portServer);
					DisplayMessages("Server: Running, ip:"+ GetLocalIPv4() + " port:" + portServer + ". Wait for players.");
				}
			}
			catch (Exception ex)
			{
				DisplayMessages("Server: Error: server start, " + ex.ToString());
			}
		}

		private void OnMessageReceivedFromClient(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
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
					DisplayMessages("Server: Player " + array[1] + " connected " + ++numberOfPlayer);
					if (numberOfPlayer == 1)
					{
						sendFromServer("You are connected!", args.RemoteAddress.DisplayName, args.RemotePort);
						player1.name = array[1];
						player1.ipAdress = args.RemoteAddress.DisplayName;
						player1.port = args.RemotePort;
						sendFromServer("play", player1.ipAdress, player1.port);
					}
					else if (numberOfPlayer == 2)
					{
						sendFromServer("You are connected!", args.RemoteAddress.DisplayName, args.RemotePort);
						player2.name = array[1];
						player2.ipAdress = args.RemoteAddress.DisplayName;
						player2.port = args.RemotePort;
						sendFromServer("play", player1.ipAdress, player1.port);
						sendFromServer("play", player2.ipAdress, player2.port);
					}

				}
			}
			catch (Exception ex)
			{
				DisplayMessages("Server: Peer didn't receive message." + ex.ToString());
			}
		}
		
		private async void sendFromServer(string message, string adress, string port)
		{

			socketServer.MessageReceived += OnMessageReceivedFromClient;
			try
			{
				await socketServer.ConnectAsync(new HostName(adress), port);
				DataWriter writer = new DataWriter(socketServer.OutputStream);
				uint messageLength = writer.MeasureString(message);
				writer.WriteString(message);
				uint bytesWritten = await writer.StoreAsync();
				Debug.Assert(bytesWritten == messageLength);
				DisplayMessages("Server: Message sent: " + message + " to: " + adress + ", port: " + port);
			}
			catch (Exception ex)
			{
				DisplayMessages("sERVER: sendFromServer " + ex.ToString());
			}
		}

		//client
		private async void startClient()
		{
			try
			{
				if (playerSocket == null)
				{
					playerSocket = new DatagramSocket();
					playerSocket.MessageReceived += OnMessageReceivedFromServer;
					DisplayMessages("Client: Running, ip:" + GetLocalIPv4() + " port:" + portClient);
					await playerSocket.BindServiceNameAsync(portClient);
				}
			}
			catch (Exception ex)
			{
				DisplayMessages("Client: Error: client start, " + ex.ToString());
			}
		}


		private async void sendFromClient(string message, string adress, string port)
		{
			playerSocket.MessageReceived += OnMessageReceivedFromServer;
			try
			{
				await playerSocket.ConnectAsync(new HostName(adress), port);
				DataWriter writer = new DataWriter(playerSocket.OutputStream);
				uint messageLength = writer.MeasureString(message);
				writer.WriteString(message);
				uint bytesWritten = await writer.StoreAsync();
				Debug.Assert(bytesWritten == messageLength);
				DisplayMessages("Player Message sent: " + message + " to: " + adress + ", port: " + port);
			}
			catch (Exception ex)
			{
				DisplayMessages("Client: sendFromClient " + ex.ToString());
			}
		}

		private void OnMessageReceivedFromServer(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		{
			try
			{
				DataReader reader = args.GetDataReader();
				reader.InputStreamOptions = InputStreamOptions.Partial;
				uint bytesRead = reader.UnconsumedBufferLength;
				string message = reader.ReadString(bytesRead);
				DisplayMessages("Player: Message received from [" + args.RemoteAddress.DisplayName + "]:" + args.RemotePort + ": " + message);
				//actuallPosition();
				if(play)
				{ 
					sendFromClient("x " + ball.Margin.Left, GetLocalIPv4(), portServer);
				}
				else if(message.Equals("play"))
				{
					play = true;
				}
			}
			catch (Exception ex)
			{
				DisplayMessages("Player: Peer didn't receive message. " + ex.ToString());
			}
		}

		//connection
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
			if (socketServer != null)
			{
				socketServer.Dispose();
				socketServer = null;
				DisplayMessages("Disconnected.");
			}
		}
	}
	
	public class Player
	{
		public Player()
		{
			name = System.String.Empty;
			ipAdress = System.String.Empty;
			port = System.String.Empty;
		}
		public string name { get; set; }
		public string ipAdress { get; set; }
		public string port { get; set; }
		public double positionPlayer { get; set; }
		public double positionPlayer2 { get; set; }
	}
}
