using System;
using System.Collections.Generic;
using System.Text;

namespace testUniveralApp
{
	public class Player
	{
		public Player()
		{
			name = System.String.Empty;
			ipAdress = System.String.Empty;
			port = System.String.Empty;
		}
		public string name { get; set; }
		public string ipAdress { get; set; }
		public string port { get; set; }
		public double positionPlayer { get; set; }
		public double positionPlayer2 { get; set; }
	}
}
