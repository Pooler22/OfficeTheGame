using System;
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
using OfficeTheGame.Class;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OfficeTheGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayPage : Page
    {
        private int speedPlayer;
        private string portTCP1L, portTCP1S;
        private string portTCP2L, portTCP2S;
        private string portTCP3L, portTCP3S;
        private string portUDP1, portUDP2;
        private string output;
        private GameServer gameServer;
        private GameClient gameClient;

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

            if (name.Substring(0, 1).Equals("s"))
            {
                gameServer = new GameServer(
                    this, name.Substring(1),
                    portUDP1, portUDP2,
                    portTCP1L, portTCP1S,
                    portTCP2L, portTCP2S,
                    portTCP3L, portTCP3S);
            }
            else if (name.Substring(0, 1).Equals("c"))
            {
                gameClient = new GameClient(
                    this, name.Substring(1),
                    portUDP2, portUDP1,
                    portTCP1S, portTCP1L,
                    portTCP2S, portTCP2L,
                    portTCP3S, portTCP3L);
            }
        }

        //view
        public async void DisplayMessages(string message)
        {
            await Dispatcher.RunIdleAsync(
                (unused) =>
                {
                    view.Items.Add(message);
                });
        }

        public async void AddServer(string message)
        {
            await Dispatcher.RunIdleAsync(
                (unused) =>
                {
                    viewServers.Visibility = Visibility.Visible;
                    viewServers.Items.Add(message);
                    viewServers.SelectionChanged += ServerListView_SelectionChanged;
                });
        }

        private void ServerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gameClient.sendToServerName(portTCP3L, e.AddedItems[0].ToString().Split(' ')[0]);
            viewServers.ClearValue(ItemsControl.ItemsSourceProperty); //need test!!
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

        private void ClientListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gameServer.sendToSelectedClient(e.AddedItems[0].ToString());
            viewClient.ClearValue(ItemsControl.ItemsSourceProperty); //need test!!
        }

        //click event
        private void Button_Click_Back_To_MainPage(object sender, RoutedEventArgs e)
        {
            this.Dispose();
            this.Frame.GoBack();
        }

        //movement
        private void playerButtonMovePointer(object sender, PointerRoutedEventArgs e)
        {
            playerButton.Margin = new Thickness(
                e.GetCurrentPoint(this).Position.X - (playerButton.Width / 2.0),
                playerButton.Margin.Top,
                playerButton.Margin.Right,
                playerButton.Margin.Bottom);
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

        //set positions
        public async void setBallPosition(int x, int y, int player)
        {
            await Dispatcher.RunIdleAsync(
                (unused) =>
                {
                    ball.Margin = new Thickness(
                        (int)((playPanel.ActualWidth * x) / 100),
                        (int)((playPanel.ActualHeight * y) / 100),
                        ball.Margin.Right,
                        ball.Margin.Bottom);
                    enemyButton.Margin = new Thickness(
                        (int)((playPanel.ActualWidth * player) / 100),
                        enemyButton.Margin.Top,
                        enemyButton.Margin.Right,
                        enemyButton.Margin.Bottom);
                });
        }

        public async void OnReceived()
        {
            await Dispatcher.RunIdleAsync(
                (unused) =>
                {
                    output = (Convert.ToInt32(playerButton.Margin.Left / playPanel.ActualWidth * 100)).ToString();
                });
        }

        public string getPlayerPosition()
        {
            return output;
        }

        // init canvans game
        private async void intCanvansGame(object sender, RoutedEventArgs e)
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
                    output = (Convert.ToInt32(playerButton.Margin.Left / playPanel.ActualWidth * 100)).ToString();
                });
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
