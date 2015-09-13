using System.Collections.Generic;
using System.Linq;
using Windows.Networking.Connectivity;

namespace testUniveralApp.Class
{
    class IPAdress
    {
		public static string LocalIPAddress()
		{
			ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
			var icp = NetworkInformation.GetInternetConnectionProfile();

			if (icp != null && icp.NetworkAdapter != null)
			{
				var hostname =
					NetworkInformation.GetHostNames()
						.SingleOrDefault(
							hn =>
							hn.IPInformation != null 
							&& hn.IPInformation.NetworkAdapter != null
							&& hn.IPInformation.NetworkAdapter.NetworkAdapterId
							== icp.NetworkAdapter.NetworkAdapterId);
				if (hostname != null)
				{
					return hostname.CanonicalName;
				}
			}
			return null;
		}

		public static string LocalIPAddress2()
		{
			List<string> ipAddresses = new List<string>();
			var hostnames = NetworkInformation.GetHostNames();
			foreach (var hn in hostnames)
			{
				if (hn.IPInformation != null &&
					(hn.IPInformation.NetworkAdapter.IanaInterfaceType == 71 // Wifi
					|| hn.IPInformation.NetworkAdapter.IanaInterfaceType == 6)) // Ethernet (Emulator) 
				{
					string ipAddress = hn.DisplayName;
					ipAddresses.Add(ipAddress);
				}
			}

			if (ipAddresses.Count < 1)
			{
				return null;
			}
			else if (ipAddresses.Count == 1)
			{
				return ipAddresses[0];
			}
			else
			{
				//if multiple suitable address were found use the last one
				//(regularly the external interface of an emulated device)
				return ipAddresses[ipAddresses.Count - 1];
			}
		}
    }
}
