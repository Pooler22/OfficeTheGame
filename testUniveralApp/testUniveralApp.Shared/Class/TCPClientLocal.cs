using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp.Class
{
    class TCPClientLocal : TCPClient
    {
        private string name;
        private PlayPage playpage;

        public TCPClientLocal(PlayPage playpage, string name)
        {
            this.playpage = playpage;
            this.name = name;
        }

        public Action<string, string, string> Received { get; internal set; }

        public override void Dispose()
        {
           // throw new NotImplementedException();
        }

        public override void initListener(string portListener)
        {
           // throw new NotImplementedException();
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
