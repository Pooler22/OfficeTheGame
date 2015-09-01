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
		string name { get; set; }
		string type{ get; set; }
		string message;
		DatagramSocket receiveSocket;
		StreamSocketListener listener = null;
		private StreamSocket socket = null;
		String ipAddress, port;
		ConnectionProfile connectionProfile;
				
		public PlayPage()
        {
            this.InitializeComponent();
			message = null;
			speedPlayer = 10;
			receiveSocket = null;
			ipAddress = "193.168.1.3";
			port = "2704";
			connectionProfile = NetworkInformation.GetInternetConnectionProfile();
		}
	
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.name = e.Parameter as string;
			this.type = name.Substring(0,1);
			this.name = name.Substring(1);
			playerButton.Content= name;
			enemyButton.Content = type;
			loadingBar.IsEnabled = false;
			
			if(type.Equals("s"))
			{
				serverUDP();
				Task.Delay(100);
				UdpSend(name);
			}
			else if (type.Equals("c"))
			{
				UdpSend(name);
			}
		}

		private void Button_Click_Back_To_MainPage(object sender, RoutedEventArgs e)
		{
			DisconnectUdp();
			this.Frame.GoBack();
		}

		private async void DisplayoutTextBlock(string message)
		{
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => loadingStackPanel.Children.Add(new TextBlock(){Text = message}));
			//await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => textBlock.Text += "\n" + message);
		}

		private async void serverUDP()
		{
			try
			{
				if (receiveSocket == null)
				{
					receiveSocket = new DatagramSocket();
					receiveSocket.MessageReceived += OnMessageReceived;
					await receiveSocket.BindServiceNameAsync(port);
					DisplayoutTextBlock("Server start\nWait for another player.");
					loadingBar.IsEnabled = true;
				}
			}
			catch (Exception ex)
			{
				loadingBar.IsEnabled = false;
				DisplayoutTextBlock("Error: server start, " + ex.ToString());
			}
		}

		private void DisconnectUdp()
		{
			if (receiveSocket != null)
			{
				receiveSocket.Dispose();
				receiveSocket = null;
				DisplayoutTextBlock("Disconnected.");
			}
		}

		private async void UdpSend(string message)
		{
			DatagramSocket sendSocket = new DatagramSocket();
			sendSocket.MessageReceived += OnMessageReceived;
			try
			{
				await sendSocket.ConnectAsync(new HostName(ipAddress), port);
				DataWriter writer = new DataWriter(sendSocket.OutputStream);
				uint messageLength = writer.MeasureString(message);
				writer.WriteString(message);
				uint bytesWritten = await writer.StoreAsync();
				Debug.Assert(bytesWritten == messageLength);
				DisplayoutTextBlock("Message sent: " + message);
			}
			catch (Exception ex)
			{
				DisplayoutTextBlock(ex.ToString());
			}
		}

		private void OnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		{
			try
			{
				DataReader reader = args.GetDataReader();
				reader.InputStreamOptions = InputStreamOptions.Partial;
				uint bytesRead = reader.UnconsumedBufferLength;
				string message = reader.ReadString(bytesRead);
				DisplayoutTextBlock("Message received from [" + args.RemoteAddress.DisplayName + "]:" + args.RemotePort + ": " + message);
				this.message = message;
			}
			catch (Exception ex)
			{
				DisplayoutTextBlock("Peer didn't receive message.");
			}
		}

		bool correctName(string nameOtherPlayer)
		{
			return this.name.Equals(nameOtherPlayer);
		}

		private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key == Windows.System.VirtualKey.Left)
			{
				playerButton.Margin = new Thickness(playerButton.Margin.Left - speedPlayer, playerButton.Margin.Top, playerButton.Margin.Right, playerButton.Margin.Bottom);
			}
			else if (e.Key == Windows.System.VirtualKey.Right)
			{
				playerButton.Margin = new Thickness(playerButton.Margin.Left + speedPlayer, playerButton.Margin.Top, playerButton.Margin.Right, playerButton.Margin.Bottom);
			}
		}

		private void playerButton_PointerMoved(object sender, PointerRoutedEventArgs e)
		{
			PointerPoint point = e.GetCurrentPoint(this);
			playerButton.Margin = new Thickness(point.Position.X - (playerButton.Width / 2.0), playerButton.Margin.Top, playerButton.Margin.Right, playerButton.Margin.Bottom);
			DisplayoutTextBlock(point.Position.X.ToString());
		}

		private void playPanel_Loaded(object sender, RoutedEventArgs e)
		{
			playerButton.Margin = new Thickness(0, playPanel.ActualHeight - (playerButton.Height), playerButton.Margin.Right, playerButton.Margin.Bottom);
			enemyButton.Margin = new Thickness(0, 0, enemyButton.Margin.Right, enemyButton.Margin.Bottom);
		
		}
	}
}
