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
    public partial class PlayPage : Page
    {
		int speedPlayer;
		string portUDP;
		string name { get; set; }
		string type { get; set; }

		Server server;
		Client client, clientTest;
		UDPClient serverUDP;
		UDPClientFinder finderServerUDP;
	
		public PlayPage()
        {
            this.InitializeComponent();
			this.speedPlayer = 10;
			this.portUDP = "4000";
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.name = e.Parameter as string;
			this.type = name.Substring(0, 1);
			this.name = name.Substring(1);

			loadingBar.IsEnabled = false;

			if (type.Equals("s"))
			{
				serverUDP = new UDPClient(this, portUDP);
				serverUDP.OnDataReceived += OnDataReceived;
				serverUDP.Start();
				
				//server = new Server(name);
				//server.initUDPListener(this, portUDP);
				//client = new Client(name);
				//server.addForPlayer1Listener(this, 80);
				//client.initClientListener(this, 81);	
				//server.addForPlayer1Sender(81);
				//client.initClientSender(80);
				//server.sendToPlayer1("Wait for another player.");
				
			}
			else if (type.Equals("c"))

			{
				finderServerUDP = new UDPClientFinder(this, portUDP);
				finderServerUDP.OnClientFound += OnClientFound;
				finderServerUDP.Start();
				finderServerUDP.BroadcastIP();
				
				//client = new Client(name);
				//client.initUDPFinder(this, portUDP);
				//server.addForPlayer2Listener(this, 82);
				//client.initClientListener(this, 83);
				//server.addForPlayer2Sender(83);
				//client.initClientSender(82);
			}
		}

		void OnClientFound(byte[] clientIP)
		{
			DisplayMessages(clientIP.ToString());
			SendUserData(clientIP);
		}

		private async void SendUserData(byte[] dest)
		{
			{
				string str = "name";
				byte[] bytes = new byte[str.Length * sizeof(char)];
				System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
				DisplayMessages(name.ToString());
				await serverUDP.SendMessage(0, bytes, dest);
				await serverUDP.SendMessage(1, bytes, dest);
			}
		}

		private void OnDataReceived(byte[] dest, byte msgType, byte[] data)
		{
			DisplayMessages(data.ToString());
			SendUserData(dest);
		}

		private bool _discovery = false;

		public async void DisplayMessages(string message)
		{
			await Dispatcher.RunIdleAsync(
				(unused) =>
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
			//disconnectServer();
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

		public static string GetLocalIPv4()
		{
			ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
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

		private void send_Click(object sender, RoutedEventArgs e)
		{
			//sendFromClient("connect " + name.ToString(), ip.Text.ToString(), portTB.Text);
			clientTest = new Client(name);

			server.addForPlayer2Listener(this, 82);
			clientTest.initClientListener(this, 83);

			server.addForPlayer2Sender(83);
			clientTest.initClientSender(82);

			server.sendToPlayer1("play");
			server.sendToPlayer2("play");
		}
	}

}
