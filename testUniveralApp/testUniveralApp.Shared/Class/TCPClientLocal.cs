using System.Threading.Tasks;

namespace testUniveralApp.Class
{
    internal class TCPClientLocal : TCPClient
    {
        public delegate void ChangedEventHandler(string e, string remoteAdress, string remotePort);

        public event ChangedEventHandler Received;

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
            playPage.DisplayMessages(name + " :TCP LOCAL Listener [local]:" + portListener + " started");
        }

        public override void initSender(string portSender, string remoteAdress)
        {
            //throw new NotImplementedException();
        }

        public override void SendRequest(string request)
        {
            //throw new NotImplementedException();
        }
    }
}