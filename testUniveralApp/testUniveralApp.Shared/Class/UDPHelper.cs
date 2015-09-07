using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace testUniveralApp
{
	public class UDPHelper
	{
		UDPClient _client;
		UDPClientFinder _finder;
		PlayPage page;
		public UDPHelper(PlayPage page, string port)
		{
			this.page = page;
			//serverUDP = new UDPClient(this);
			_finder = new UDPClientFinder(page, port);
			//_finder.OnClientFound += _finder_OnClientFound;
			//_client.OnDataReceived += _client_OnDataReceived;
		}

		void _finder_OnClientFound(byte[] clientIP)
		{
			SendUserData(clientIP);
		}
		
		private async void SendUserData(byte[] dest)
		{
			{
				string str = "name";
				byte[] bytes = new byte[str.Length * sizeof(char)];
				System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);

			//	await _client.SendMessage(0, bytes, dest);
				//await _client.SendMessage(1, bytes, dest);
			}
		}

		private void _client_OnDataReceived(byte[] dest, byte msgType, byte[] data)
		{
			SendUserData(dest);
		}

		private bool _discovery = false;
		
		public bool IsDiscoveryEnabled
		{
			get
			{
				return _discovery;
			}
			set
			{
				_discovery = value;
				if (_discovery)
				{
					_finder.Start();
					_finder.BroadcastIP();
					_client.Start();
				}
				else
				{
					_finder.Stop();
					_client.Stop();
				}
			}
		}

	}
}
