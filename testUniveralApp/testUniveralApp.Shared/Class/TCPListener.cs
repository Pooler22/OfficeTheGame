using System;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace testUniveralApp
{
    internal class TCPListener
    {
        public delegate void ChangedEventHandler(string e, string remoteAdress, string remotePort);

        public event ChangedEventHandler Received;

        public string name { get; set; }
        private StreamSocketListener listener = null;
        private StreamSocket sender = null;
        private PlayPage playPage;

        public TCPListener(PlayPage playPage, string name)
        {
            this.playPage = playPage;
            this.name = name;
        }

        //listener
        public void initListener(string portListener)
        {
            Task.Run(
                async () =>
                {
                    await Listener(portListener);
                })
                .Wait();
        }

        private async Task Listener(string portListener)
        {
            try
            {
                if (listener == null)
                {
                    listener = new StreamSocketListener();
                    listener.ConnectionReceived += OnConnectionReceived;
                    await listener.BindServiceNameAsync(portListener.ToString());
                    playPage.DisplayMessages(name + " :LISTENER: TCP Listener [local]:" + portListener + " started");
                }
            }
            catch (Exception ex)
            {
                playPage.DisplayMessages(name + " :ERROR: LISTENER: TCP Listener [local]:" + portListener + " started\n" + ex.ToString());
            }
        }

        private async void OnConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                playPage.DisplayMessages(name + " " + args.Socket.Information.RemoteAddress.DisplayName + " connected.");
                while (true)
                {
                    string request = await Read(args.Socket.InputStream);

                    if (String.IsNullOrEmpty(request))
                    {
                        return;
                    }
                    OnReceived(request, args.Socket.Information.RemoteAddress.DisplayName, args.Socket.Information.RemotePort);
                    playPage.DisplayMessages(name + " :LISTENER: Recived TCP " + request);
                    //string response = "Respone.\r\n";
                    //await Send(args.Socket.OutputStream, response);
                }
            }
            catch (Exception ex)
            {
                playPage.DisplayMessages(name + " :LISTENER: Recived TCP " + ex.ToString());
            }
        }

        protected virtual void OnReceived(string request, string remoteAdress, string remotePort)
        {
            if (Received != null)
                Received(request, remoteAdress, remotePort);
        }

        private void DisposeListener()
        {
            if (listener != null)
            {
                listener.Dispose();
                listener = null;
            }
        }

        private async Task<string> Read(IInputStream inputStream)
        {
            DataReader reader = new DataReader(inputStream);
            reader.InputStreamOptions = InputStreamOptions.Partial;
            string message = "";

            while (!message.EndsWith("\r\n"))
            {
                uint bytesRead = await reader.LoadAsync(16);
                if (bytesRead == 0)
                {
                    playPage.DisplayMessages(name + " :LISTENER:  The connection was closed by remote host.");
                    break;
                }
                message += reader.ReadString(bytesRead);
            }
            reader.DetachStream();
            return message;
        }

        private async Task Send(IOutputStream outputStream, string message)
        {
            DataWriter writer = new DataWriter(outputStream);
            uint messageLength = writer.MeasureString(message);
            writer.WriteString(message);
            uint bytesWritten = await writer.StoreAsync();
            writer.DetachStream();
        }

        //sender
        public void initSender(string portSender, string remoteAdress)
        {
            Task.Run(
                async () =>
                {
                    await Sender(portSender, remoteAdress);
                })
                .Wait();
        }

        private async Task Sender(string portSender, string remoteAdress)
        {
            try
            {
                sender = new StreamSocket();
                await sender.ConnectAsync(new HostName(remoteAdress), portSender);
                playPage.DisplayMessages(name + " :TPC Sender[" + remoteAdress + ":" + portSender + "] started");
            }
            catch (Exception ex)
            {
                playPage.DisplayMessages(name + " :ERROR: TCP Sender[" + remoteAdress + ":" + portSender + "] started\n" + ex.ToString());
            }
        }

        public async void SendRequest(string request)
        {
            if (sender != null)
            {
                request += "\r\n";
                await Send(sender.OutputStream, request);
                //string response = await Read(sender.InputStream);
                //DisplayMessages(response);
            }
        }

        private void DisconnectSender()
        {
            if (sender != null)
            {
                sender.Dispose();
                sender = null;
            }
        }

        internal void Dispose()
        {
            if (listener != null)
                listener.Dispose();
            if (sender != null)
                sender.Dispose();
        }
    }
}