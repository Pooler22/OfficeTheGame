using System;
using System.Threading.Tasks;
using OfficeTheGame.Class;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace OfficeTheGame.Class
{
    internal class UDPListener
    {
        private string name, portListener, portSender;
        private DatagramSocket sender, listener;
        private PlayPage playPage;

        public UDPListener(PlayPage playPage, string name, string portListener, string portSender)
        {
            this.playPage = playPage;
            this.name = name;
            this.portListener = portListener;
            this.portSender = portSender;

            Task.Run(
                async () =>
                {
                    await startListener();
                });
        }

        public async Task startListener()
        {
            try
            {
                listener = new DatagramSocket();
                listener.MessageReceived += MessageReceived;
                await listener.BindEndpointAsync(new HostName(IPAdress.LocalIPAddress()), portListener);
                playPage.DisplayMessages(name + " :UDP Listener [local]:" + portListener + " started");
            }
            catch (Exception ex)
            {
                playPage.DisplayMessages(name + " :ERROR: UDP Listener [local]:" + portListener + " started\n" + ex.ToString());
            }
        }

        private void MessageReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs args)
        {
            try
            {
                DataReader reader = args.GetDataReader();
                reader.InputStreamOptions = InputStreamOptions.Partial;
                uint bytesRead = reader.UnconsumedBufferLength;
                String message = reader.ReadString(bytesRead);

                playPage.DisplayMessages(name + " :Message received [" +
                    args.RemoteAddress.DisplayName.ToString() + "]:" + args.RemotePort + ": " + message);

                SendMessage(IPAdress.LocalIPAddress() + " " + name, args.RemoteAddress.DisplayName.ToString(), portSender);
                reader.Dispose();
            }
            catch (Exception ex)
            {
                playPage.DisplayMessages(name + " :ERROR: Message received from:\n" + ex.ToString());
            }
        }

        public void SendMessage(string message, string host, string port)
        {
            byte[] bytes1 = new byte[message.Length * sizeof(char)];
            System.Buffer.BlockCopy(message.ToCharArray(), 0, bytes1, 0, bytes1.Length);

            Task.Run(
                async () =>
                {
                    await SendMessage(bytes1, host, port);
                })
                .Wait();
        }

        public async Task SendMessage(byte[] message, string host, string port)
        {
            sender = new DatagramSocket();
            try
            {
                using (var stream = await sender.GetOutputStreamAsync(new HostName(host), port))
                {
                    var writer = new DataWriter(stream);
                    writer.WriteBytes(message);
                    await writer.StoreAsync();
                    playPage.DisplayMessages(name + " :Send message");
                }
            }
            catch (Exception ex)
            {
                playPage.DisplayMessages(name + " :Error: Send Message\n" + ex.ToString());
            }
            sender.Dispose();
        }

        public void Dispose()
        {
            if (sender != null)
                sender.Dispose();
            if (listener != null)
                listener.Dispose();
        }
    }
}