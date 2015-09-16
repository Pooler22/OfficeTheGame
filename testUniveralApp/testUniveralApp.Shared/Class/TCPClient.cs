namespace testUniveralApp.Class
{
    internal abstract class TCPClient
    {
        public abstract void initListener(string portListener);

        public abstract void initSender(string portSender, string remoteAdress);

        public abstract void SendRequest(string request);

        public abstract void Dispose();
    }
}