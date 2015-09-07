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
		string portUDP1, portUDP2;
		string name { get; set; }
		string type { get; set; }
		UDPClient serverUDP;
		UDPClientFinder finderUDP;
		Server server;
		Client client;
	
		public PlayPage()
        {
            this.InitializeComponent();
			
			this.speedPlayer = 10;
			this.portUDP1 = "4000";
			this.portUDP2 = "4001";
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.name = e.Parameter as string;
			this.type = name.Substring(0, 1);
			this.name = name.Substring(1);

			if (type.Equals("s"))
			{
				serverUDP = new UDPClient(this, portUDP1, name);
				serverUDP.Start();

				{
					server = new Server(name);
					client = new Client(name);
					server.addForPlayer1Listener(this, 80);
					client.initClientListener(this, 81);
					server.addForPlayer1Sender(81);
					client.initClientSender(80);
					server.sendToPlayer1("Wait for another player.");
				}
			}
			else if (type.Equals("c"))
			{
				finderUDP = new UDPClientFinder(this, portUDP1);
				finderUDP.Start();
				finderUDP.BroadcastIP();
				
				//client = new Client(name);
				//client.initClientListener(this, 83);
				//client.initClientSender(82);
			}
		}

		public void addTCPsecondPlayer()
		{
			server.addForPlayer2Listener(this, 82);
			server.addForPlayer2Sender(83);
		}

		public async void DisplayMessages(string message)
		{
			await Dispatcher.RunIdleAsync(
				(unused) =>
				{
					view.Items.Add(message);
					view.ScrollIntoView(message);
				});
		}

		public async void AddServer(string message)
		{
			await Dispatcher.RunIdleAsync(
				(unused) =>
				{
					viewServers.Items.Add(message);
					viewServers.ScrollIntoView(message);
					viewServers.SelectionChanged += ListView_SelectionChanged;
				});
		}

		private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			addTCPsecondPlayer();
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
			if(finderUDP != null)
			{
				finderUDP.Stop();
			}
			if(serverUDP != null)
			{
				serverUDP.Stop();
			}
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

		private void find_Click(object sender, RoutedEventArgs e)
		{
			finderUDP.BroadcastIP();
		}

	}

}
