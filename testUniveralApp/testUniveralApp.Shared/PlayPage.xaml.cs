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
using System.Threading.Tasks;

using System.Net;
using System.Windows;
using System.Windows.Input;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Networking;
using Windows.Networking.Connectivity;
using System.Text;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Input;



namespace testUniveralApp
{
    public sealed partial class PlayPage : Page
    {
		int speedPlayer;
		string name { get; set; }
		string type{ get; set; }
		string message;
		DatagramSocket receiveSocket;
		String ipAddress, port;
		ConnectionProfile connectionProfile;
				
		public PlayPage()
        {
            this.InitializeComponent();
			message = null;
			speedPlayer = 10;
			receiveSocket = null;
			ipAddress = "224.0.0.3";
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
			}
			else if (type.Equals("c"))
			{
				UdpSend();
				//InitializeSockets();
			}
		}

		private void Button_Click_Back_To_MainPage(object sender, RoutedEventArgs e)
		{
			DisconnectUdp();
			this.Frame.GoBack();
		}

		private async void DisplayOutput(TextBlock textBlock, string message)
		{
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => textBlock.Text += message);
		}

		private async void serverUDP()
		{
			try
			{
				if (receiveSocket == null)
				{
					receiveSocket = new DatagramSocket();

					// MessageReceived handler must be set before BindServiceAsync is called, if not
					// "A method was called at an unexpected time. (Exception from HRESULT: 
					// 0x8000000E)" exception is thrown.
					receiveSocket.MessageReceived += OnMessageReceived;

					// If port is already in used by another socket, "Only one usage of each socket
					// address (protocol/network address/port) is normally permitted. (Exception from
					// HRESULT: 0x80072740)" exception is thrown.
					await receiveSocket.BindServiceNameAsync(port);
					loadingBar.IsEnabled = true;
					DisplayOutput(output, "Wait for another player");
				}
			}
			catch (Exception ex)
			{
				DisplayOutput(output, "Error: server start, " + ex.ToString());
			}
		}

		private void DisconnectUdp()
		{
			if (receiveSocket != null)
			{
				receiveSocket.Dispose();
				receiveSocket = null;
				DisplayOutput(output, "Disconnected.");
			}
		}

		private void OnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		{
			try
			{
				DataReader reader = args.GetDataReader();
				reader.InputStreamOptions = InputStreamOptions.Partial;

				// LoadAsync not needed. The reader comes already loaded.

				// If called by a 'Udp send socket', next line throws an exception because message was not received.

				// If remote peer didn't received message, "An existing connection was forcibly
				// closed by the remote host. (Exception from HRESULT: 0x80072746)" exception is
				// thrown. Maybe only when using ConenctAsync(), not GetOutputStreamAsync().
				uint bytesRead = reader.UnconsumedBufferLength;
				string message = reader.ReadString(bytesRead);

				DisplayOutput(output, "Message received from [" +
					args.RemoteAddress.DisplayName + "]:" + args.RemotePort + ": " + message);
				this.message = message;
			}
			catch (Exception ex)
			{
				DisplayOutput(output, "Peer didn't receive message.");
			}
		}

		bool correctName(string nameOtherPlayer)
		{
			if(this.name ==nameOtherPlayer)
				return false;
			else
				return true;

		}
		//
		// UDP send.
		//

		private async void UdpSend()
		{
			DatagramSocket sendSocket = new DatagramSocket();

			// Even when we do not except any response, this handler is called if any error occurrs.
			sendSocket.MessageReceived += OnMessageReceived;

			try
			{
				await sendSocket.ConnectAsync(new HostName("192.168.0.6"), "2704");
				// DatagramSocket.ConnectAsync() vs DatagramSocket.GetOutputStreamAsync()?
				// Use DatagramSocket.GetOutputStreamAsync() if datagrams are sent to multiple
				// GetOutputStreamAsync() does DNS resolution first.
				// If remote host does not exist, "No such host is known. (Exception from HRESULT: 0x80072AF9)"
				// exception is thrown.
				// If remote host is not listening on the specified host, "An existing connection was forcibly
				// closed by the remote host. (Exception from HRESULT: 0x80072746)" exception is thrown.

				string message = name;
				DataWriter writer = new DataWriter(sendSocket.OutputStream);

				// This is useless in this sample. Just a friendly remainder.
				uint messageLength = writer.MeasureString(message);

				writer.WriteString(message);

				uint bytesWritten = await writer.StoreAsync();

				Debug.Assert(bytesWritten == messageLength);

				DisplayOutput(output, "Message sent: " + message);
			}
			catch (Exception ex)
			{
				DisplayOutput(output, ex.ToString());
			}
		}

		//private void inTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
		//{
		//	if (e.Key == Windows.System.VirtualKey.Enter)
		//	{
		//		SendMessage(_socket, inTextBox.Text);
		//		inTextBox.Text = "";
		//	}
		//}

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
			output.Text = point.Position.X.ToString();
		}

		private void playPanel_Loaded(object sender, RoutedEventArgs e)
		{
			playerButton.Margin = new Thickness(0, playPanel.ActualHeight - (playerButton.Height), playerButton.Margin.Right, playerButton.Margin.Bottom);
		}
	}
}
