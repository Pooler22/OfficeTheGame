using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using testUniveralApp.Class;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;

namespace testUniveralApp
{
    public partial class PlayPage : Page
    {
		int speedPlayer;
        string portTCP1L, portTCP1S;
        string portTCP2L, portTCP2S;
        string portTCP3L, portTCP3S;
		string portUDP1, portUDP2;

		GameServer gameServer;
		GameClient gameClient;
        bool loaded = false;

        public PlayPage()
        {
            this.InitializeComponent();

			this.speedPlayer = 10;
			this.portUDP1 = "4444";
			this.portUDP2 = "4445";
			this.portTCP1L = "8001";
			this.portTCP1S = "8002";
			this.portTCP2L = "8003";
			this.portTCP2S = "8004";
			this.portTCP3L = "8005";
            this.portTCP3S = "8006";
        }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			string name = e.Parameter as string;
			string type = name.Substring(0, 1);
			name = name.Substring(1);

			if (type.Equals("s"))
			{
				gameServer = new GameServer(this, name, portUDP1, portUDP2, portTCP1L, portTCP1S, portTCP2L, portTCP2S, portTCP3L, portTCP3S);
                play();
            }
			else if (type.Equals("c"))
			{
				gameClient = new GameClient(this, name, portUDP2, portUDP1, portTCP1S, portTCP1L, portTCP2S, portTCP2L, portTCP3S, portTCP3L);
                gameClient.Received += OnReceived;
            }
        }

        void play()
        {
            int xMove = 4;
            int yMove = -1;
            int xPos = 50;
            int yPos = 50;
            Task.Run(
                   async () =>
                    {    
                        while (true)
                        {
                            await Dispatcher.RunIdleAsync(
                                (unused) =>
                                {
                                    if(yPos >= 100 || yPos <= 0)
                                    {
                                        yMove = -yMove;
                                    }
                                    yPos += yMove;
                                    
                                    gameServer.sendToPlayer1(xPos + " " + yPos);
                                    gameServer.sendToPlayer2(xPos + " " + yPos);
                                });
                            await Task.Delay(10);
                        }
                    });
        }

        private async void OnReceived(string remoteMessage, string remoteAdress, string remotePort)
        {
            await Dispatcher.RunIdleAsync(
                (unused) =>
                {
                    setButtonPosition(
                        ball,
                        (float)(playPanel.ActualWidth * int.Parse(remoteMessage.Split(' ')[0]) / 100),
                        (float)(playPanel.ActualHeight * int.Parse(remoteMessage.Split(' ')[1]) / 100)
                        );
                });
        }

        //view
        public async void DisplayMessages(string message)
		{
			await Dispatcher.RunIdleAsync(
				(unused) =>
				{
					view.Items.Add(message);
                    //view.ScrollIntoView(message);
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
			gameClient.sendToServerName(portTCP3L, e.AddedItems[0].ToString().Split(' ')[0]);
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
			gameServer.sendIniToPlayer2(e.AddedItems[0].ToString());
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

        async void setButtonPosition(Shape button, float x, float y)
        {
            await Dispatcher.RunIdleAsync(
                (unused) =>
                {
                    button.Margin = new Thickness(
                        x,
                        y,
                        button.Margin.Right,
                        button.Margin.Bottom);
                });
        }

        public async void setBallPosition(float x, float y)
        {
            await Dispatcher.RunIdleAsync(
                (unused) =>
                {
                    
                    ball.Margin = new Thickness(
                        (float)(playPanel.ActualWidth * x / 100),
                        (float)(playPanel.ActualHeight * y / 100),
                        ball.Margin.Right,
                        ball.Margin.Bottom);
                });
        }

        // init canvans game
        async void intCanvansGame(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunIdleAsync(
                (unused) =>
                {
                    playerButton.Width = enemyButton.Width = (int)(ActualWidth / 10);
                    playerButton.Margin = new Thickness(
                        (ActualWidth / 2) - (playerButton.Width / 2),
                        playPanel.ActualHeight - (playerButton.Height),
                        playerButton.Margin.Right,
                        playerButton.Margin.Bottom);
                    enemyButton.Margin = new Thickness(
                        0,
                        0,
                        enemyButton.Margin.Right,
                        enemyButton.Margin.Bottom);
                });
            loaded = true;
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