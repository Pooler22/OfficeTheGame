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
    public partial class PlayPage : Page, IDisposable
    {
		int speedPlayer; 
		string portTCP1L, portTCP1S, portTCP2L, portTCP2S;
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
			this.portUDP1 = "4444";
			this.portUDP2 = "4001";
			portTCP1L = "8021";
			portTCP1S = "8022";
			portTCP2L = "8023";
			portTCP2S = "8024";
		}

		public static string LocalIPAddress()
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

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.name = e.Parameter as string;
			this.type = name.Substring(0, 1);
			this.name = name.Substring(1);

			if (type.Equals("s"))
			{
				serverUDP = new UDPClient(this, portUDP1, name);
				serverUDP.Start();
				server = new Server(this,name);
				client = new Client(this, name);

				server.addForPlayer1Listener(portTCP1L);
				client.initClientListener(portTCP1S);
				server.addForPlayer2Listener(portTCP2L);

				client.initClientSender(portTCP1L, LocalIPAddress());
				server.addForPlayer1Sender(portTCP1S, LocalIPAddress());
				client.sendToServer("Client 1 to server");
				server.sendToPlayer1("Wait for another player.");
			}
			else if (type.Equals("c"))
			{
				finderUDP = new UDPClientFinder(this, portUDP1);
				finderUDP.Start();
				finderUDP.BroadcastIP();
				
				client = new Client(this,name);
				client.initClientListener(portTCP2S);
			}
		}

		//view
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
					viewServers.SelectionChanged += ServerListView_SelectionChanged;
				});
		}
		void ServerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			client.initClientSender(portTCP2L, e.AddedItems[0].ToString().Split(' ')[0]);
			client.sendToServer(name);
		}

		public async void AddClient(string message)
		{
			await Dispatcher.RunIdleAsync(
				(unused) =>
				{
					viewClient.IsEnabled = true;
					viewClient.Items.Add("Accept " + message + "\r\n");
					viewClient.Items.Add("Cancel " + message + "\r\n");
					viewClient.ScrollIntoView("Cancel " + message);
					viewClient.SelectionChanged += ServerListView_SelectionChanged1;
					
				});
		}
		void ServerListView_SelectionChanged1(object sender, SelectionChangedEventArgs e)
		{
			string s = e.AddedItems[0].ToString();
			server.sendToPlayer2(s + "\r\n");
		}

		//click event

		public void Dispose()
		{
			if (finderUDP != null)
			{
				finderUDP.Dispose();
			}
			if (serverUDP != null)
			{
				serverUDP.Dispose();
			}
			if (client != null)
			{
				client.Dispose();
			}
			if (clienttest != null)
			{
				clienttest.Dispose();
			}
			if (server != null)
			{
				server.Dispose();
			}
		}

		void Button_Click_Back_To_MainPage(object sender, RoutedEventArgs e)
		{
			this.Dispose();
			this.Frame.GoBack();
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