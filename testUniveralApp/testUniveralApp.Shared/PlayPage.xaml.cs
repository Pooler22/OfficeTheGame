using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using testUniveralApp.Class;

namespace testUniveralApp
{
    public partial class PlayPage : Page, IDisposable
    {
		int speedPlayer; 
		string portTCP1L, portTCP1S, portTCP2L, portTCP2S;
		string portUDP1, portUDP2;

		GameServer gameServer;
		GameClient gameClient;
	
		public PlayPage()
        {
            this.InitializeComponent();
			
			this.speedPlayer = 10;
			this.portUDP1 = "4444";
			this.portUDP2 = "4001";
			this.portTCP1L = "8021";
			this.portTCP1S = "8022";
			this.portTCP2L = "8023";
			this.portTCP2S = "8024";
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			string name = e.Parameter as string;
			string type = name.Substring(0, 1);
			name = name.Substring(1);

			if (type.Equals("s"))
			{
				gameServer = new GameServer(this, name, portUDP1, portUDP2, portTCP1L, portTCP1S, portTCP2L, portTCP2S);
			}
			else if (type.Equals("c"))
			{
				gameClient = new GameClient(this, name, portUDP1, portUDP2, portTCP2S);
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
					viewServers.Visibility = Visibility.Visible;
					viewServers.Items.Add(message);
					viewServers.ScrollIntoView(message);
					viewServers.SelectionChanged += ServerListView_SelectionChanged;
				});
		}
		
		void ServerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			gameClient.sendToServerName(portTCP2L, e.AddedItems[0].ToString().Split(' ')[0]);
		}

		public async void AddClient(string message)
		{
			await Dispatcher.RunIdleAsync(
				(unused) =>
				{
					viewClient.Visibility = Visibility.Visible;
					viewClient.IsEnabled = true;
					viewClient.Items.Add("Accept " + message + "\r\n");
					viewClient.Items.Add("Cancel " + message + "\r\n");
					viewClient.ScrollIntoView("Cancel " + message);
					viewClient.SelectionChanged += ClientListView_SelectionChanged;
					
				});
		}
		
		void ClientListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			gameServer.sendToPlayer2(e.AddedItems[0].ToString());
		}

		//click event

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
		
		//other

		public void Dispose()
		{
			if (gameServer != null)
			{
				gameServer.Dispose();
			}
			if (gameClient != null)
			{
				gameClient.Dispose();
			}
		}
	}
}