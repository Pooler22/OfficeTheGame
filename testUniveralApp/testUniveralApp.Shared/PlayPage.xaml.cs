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
		string name { get; set; }
		string type{ get; set; }
		string portServer, portClient, message;
		Player player1, player2;
		ConnectionProfile connectionProfile;
		bool play = false;
		Server server;
		Client client;
	
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

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.name = e.Parameter as string;
			this.type = name.Substring(0, 1);
			this.name = name.Substring(1);
			//playerButton.Content = name;
			//enemyButton.Content = type;
			loadingBar.IsEnabled = false;
			if (type.Equals("s"))
			{
				server = new Server();
				client = new Client();
			}
			else if (type.Equals("c"))

			{
				client = new Client();
			}
		}

		public async void DisplayMessages(string message)
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
		}
	}

}
