using System;
using System.Threading.Tasks;
using OfficeTheGame.Class;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace OfficeTheGame.Class
{
    internal class TCPClientRemote : TCPClient
    {
        public delegate void ChangedEventHandler(string e, string remoteAdress, string remotePort);

        public event ChangedEventHandler Received;

        public string name;
        private PlayPage playPage;

        private StreamSocketListener listener = null;
        private StreamSocket sender = null;

        public TCPClientRemote()
        {
        }

        public static TCPClientRemote Instance
        {
            get
            {
                return new TCPClientRemote();
            }
        }

        public void initTCPClient(PlayPage playpage, string name)
        {
            this.playPage = playpage;
            this.name = name;
        }

        //listener

        public override void initListener(string portListener)
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
                    playPage.DisplayMessages(name + " :TCP Listener [local]:" + portListener + " started");
                }
            }
            catch (Exception ex)
            {
                playPage.DisplayMessages(name + " :ERROR: TCP Listener [local]:" + portListener + " started\n" + ex.ToString());
            }
        }

        private async void OnConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            playPage.DisplayMessages(name + " :Recived TCP: start");
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
                    playPage.DisplayMessages(name + " :Recived TCP: " + request);
                    OnReceived(request, args.Socket.Information.RemoteAddress.DisplayName, args.Socket.Information.RemotePort);
                    //string response = "Respone.\r\n";
                    //await Send(args.Socket.OutputStream, response);
                }
            }
            catch (Exception ex)
            {
                playPage.DisplayMessages(name + " :Recived TCP\n" + ex.ToString());
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
                    playPage.DisplayMessages(name + " :The connection was closed by remote host.");
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
        public override void initSender(string portSender, string remoteAdress)
        {
            Task.Run(
                async () =>
                {
                    await Sender(portSender, remoteAdress);
                })
                .Wait();
        }

        public async Task Sender(string portSender, string remoteAdress)
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

        public override async void SendRequest(string request)
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

        public override void Dispose()
        {
            if (listener != null)
                listener.Dispose();
            if (sender != null)
                sender.Dispose();
        }
    }
}