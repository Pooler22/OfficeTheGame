namespace testUniveralApp.Class
{
    internal abstract class TCPClient
    {
        public string name;
        private PlayPage playPage;

        public void initTCPClient(PlayPage playpage, string name)
        {
            this.playPage = playpage;
            this.name = name;
        }

        public abstract void initListener(string portListener);

        public abstract void initSender(string portSender, string remoteAdress);

        public abstract void SendRequest(string request);

        public abstract void Dispose();
    }
}