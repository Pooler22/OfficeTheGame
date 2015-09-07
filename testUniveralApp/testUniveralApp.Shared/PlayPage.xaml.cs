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
		Client client, clienttest;
	
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
					server = new Server(this, name);
					client = new Client(this, name);
					server.addForPlayer1Listener(80);
					client.initClientListener(81);
					server.addForPlayer1Sender(81, "192.168.1.103");
					client.initClientSender(80, "192.168.1.103");
					server.sendToPlayer1("Wait for another player.");
					
				}
			}
			else if (type.Equals("c"))
			{
				finderUDP = new UDPClientFinder(this, portUDP1);
				finderUDP.Start();
				finderUDP.BroadcastIP();
				
				//client = new Client(this,name);
				//server.addForPlayer2Sender(82,"192.168.1.103") ;
			}
		}

		public void addTCPsecondPlayer()
		{
			//client.initClientListener(this, 83);
			//client.initClientSender(82, "192.168.1.102");
		}

		//view
		public async void DisplayMessages(string message)
		{
			if(message.Contains("start"))
			{
				//server.addForPlayer2Sender(83, "192.168.1.103");
				DisplayMessages("wow");
			}
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
					viewServers.SelectionChanged += ServerListView_SelectionChanged;
				});
		}

		void ServerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			addTCPsecondPlayer();
		}

		//click event
		void Button_Click_Back_To_MainPage(object sender, RoutedEventArgs e)
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

		void find_Click(object sender, RoutedEventArgs e)
		{
			server.addForPlayer2Listener(this, 82);
			//clienttest = new Client("ww");
			//clienttest.initClientListener(this, 83);
			clienttest.initClientSender(82, "192.168.1.103");
			clienttest.sendToServer("start");
		}

		//movement
		void playerButtonMovePointer(object sender, PointerRoutedEventArgs e)
		{
			playerButton.Margin = new Thickness(
				e.GetCurrentPoint(this).Position.X - (playerButton.Width / 2.0),
				playerButton.Margin.Top,
				playerButton.Margin.Right,
				playerButton.Margin.Bottom);
		}

		void playerButtonMoveKeys(object sender, KeyRoutedEventArgs e)
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

		void setStartPlayersPositions(object sender, RoutedEventArgs e)
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

	}

}
