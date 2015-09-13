using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp.Class
{
    class GameClient
    {
		UDPListenerFinder finderUDP;
        TCPClient firstConnectionClient;
        TCPClient client;
		string name;
        PlayPage playpage;
        string portTCP2S;

        public GameClient(PlayPage playpage, string name, 
            string portUDP1, string portUDP2,
            string portTCP1L, string portTCP1S,
            string portTCP2L, string portTCP2S,
            string portTCP3L, string portTCP3S)
		{
            this.playpage = playpage;
            this.name = name;

			finderUDP = new UDPListenerFinder(playpage, name, portUDP1, portUDP2);
			finderUDP.Start();
			finderUDP.SendDiscovery();

            firstConnectionClient = new TCPClient(playpage, name);
            firstConnectionClient.initListener(portTCP3L);
            firstConnectionClient.Received += OnReceived;

            client = new TCPClient(playpage, name);
            client.initListener(portTCP2L);
            this.portTCP2S = portTCP2S;
        }

        private void OnReceived(string remoteMessage, string remoteAdress, string remotePort)
        {
            playpage.DisplayMessages("Recieived ["+ remoteAdress + "]:" + remotePort + " " + remoteMessage.Split('\r')[0]);
            if(remoteMessage.Split('\r')[0].Equals("Accept"))
            {
                playpage.DisplayMessages("Connecting");
                client.initSender(portTCP2S, remoteAdress);
            }
           else  if (remoteMessage.Split('\r')[0].Equals("Cancel"))
            {
                playpage.DisplayMessages("Disconnecting");
            }
        }

        public void sendToServerName(string port, string adres)
		{
            firstConnectionClient.initSender(port, adres);
            firstConnectionClient.SendRequest(name);
		}

		public void Dispose()
		{
			if (finderUDP != null)
			{
				finderUDP.Dispose();
			}
			//if (client != null)
			//{
			//	client.Dispose();
			//}
		}
	}
}
