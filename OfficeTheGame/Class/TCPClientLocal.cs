using System;
using System.Threading.Tasks;

namespace OfficeTheGame.Class
{
    internal class TCPClientLocal : TCPClient
    {
        bool listener = false;
        bool sender = false;
        string message;
        int pid;

        public delegate void ChangedEventHandler(string e, string remoteAdress, string remotePort);

        public event ChangedEventHandler Received;

        public event ChangedEventHandler Received2;

        private static TCPClientLocal instance;

        public TCPClientLocal()
        {
        }

        public void initTCPClient(PlayPage playpage, string name)
        {
            this.playPage = playpage;
            this.name = name;
        }

        private string name;
        private PlayPage playPage;

        public static TCPClientLocal Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TCPClientLocal();
                }
                return instance;
            }
        }

        public override void Dispose()
        {
            // throw new NotImplementedException();
        }

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
            listener = true;
            Received += OnConnectionReceived;
            playPage.DisplayMessages(name + " :TCP LOCAL Listener [local]:" + portListener + " started");
        }

        private async void OnConnectionReceived(string message, string remoteAdress, string remotePort)
        {
            playPage.DisplayMessages(name + " :LOCAL Recived TCP: start");
            try
            {
                playPage.DisplayMessages("Local :Recived TCP: " + message);
                OnReceived(message, null, null);
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
            sender = true;
            playPage.DisplayMessages(name + " :TPC LOCAL Sender[" + remoteAdress + ":" + portSender + "] started");
        }

        public override void SendRequest(string request)
        {
            if (sender)
            {
                request += "\r\n";
                Send(request);
            }
        }

        private void Send(string message)
        {
            this.message = message;
            if (Received2 != null)
                Received2(message, null, null);
        }

        private void DisconnectSender()
        {
            sender = false;
        }

        private void DisconnectListener()
        {
            listener = false;
            
        }
    }
}