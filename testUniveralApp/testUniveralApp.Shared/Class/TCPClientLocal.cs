using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp.Class
{
    class TCPClientLocal : TCPConection
    {
        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void initListener(string portListener)
        {
            throw new NotImplementedException();
        }

        public override void initSender(string portSender, string remoteAdress)
        {
            throw new NotImplementedException();
        }

        public override void SendRequest(string request)
        {
            throw new NotImplementedException();
        }

    }
}
