/*-------------------------------------------------------------------------

 ネットワーク関係でよく使うもの

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace net_base
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	static public class net_useful
	{
		/*-------------------------------------------------------------------------
		 ローカルホスト명を得る
		---------------------------------------------------------------------------*/
		static public string GetHostName()
		{
			return Dns.GetHostName();
		}
	
		/*-------------------------------------------------------------------------
		 ローカルPCのIPアドレスを得る
		 IPv4とIPv6が混ざったものが得られる
		 同期형のため, アドレス解決に시간がかかる場合あり
		---------------------------------------------------------------------------*/
		static public IPAddress[] GetLocalIpAddress()
		{
			try{
				return Dns.GetHostAddresses(Dns.GetHostName());
			}catch{
				// 실패
				return null;
			}
		}	

		/*-------------------------------------------------------------------------
		 ローカルPCのIPアドレスを得る
		 Ipv4のみ
		 同期형のため, アドレス解決に시간がかかる場合あり
		---------------------------------------------------------------------------*/
		static public IPAddress[] GetLocalIpAddress_Ipv4()
		{
			return GetAddressListIpv4(GetLocalIpAddress());
		}

		/*-------------------------------------------------------------------------
		 ローカルPCのIPアドレスを得る
		 Ipv6のみ
		 同期형のため, アドレス解決に시간がかかる場合あり
		---------------------------------------------------------------------------*/
		static public IPAddress[] GetLocalIpAddress_Ipv6()
		{
			return GetAddressListIpv6(GetLocalIpAddress());
		}

		/*-------------------------------------------------------------------------
		 지정したhostのIPアドレスを得る
		---------------------------------------------------------------------------*/
		static public IPAddress[] GetIpAddress(string host)
		{
			return Dns.GetHostEntry(host).AddressList;
		}

		/*-------------------------------------------------------------------------
		 지정したhostのIPアドレスを得る
		 Ipv4のみ
		---------------------------------------------------------------------------*/
		static public IPAddress[] GetIpAddressIpv4(string host)
		{
			return GetAddressListIpv4(GetIpAddress(host));
		}

		/*-------------------------------------------------------------------------
		 지정したhostのIPアドレスを得る
		 Ipv6のみ
		---------------------------------------------------------------------------*/
		static public IPAddress[] GetIpAddressIpv6(string host)
		{
			return GetAddressListIpv6(GetIpAddress(host));
		}
	
		/*-------------------------------------------------------------------------
		 지정したAddressFamilyのみの목록を得る
		---------------------------------------------------------------------------*/
		static public IPAddress[] GetAddressListIpv4(IPAddress[] list)
		{
			return GetAddressList(list, AddressFamily.InterNetwork);
		}
		static public IPAddress[] GetAddressListIpv6(IPAddress[] list)
		{
			return GetAddressList(list, AddressFamily.InterNetworkV6);
		}
		static public IPAddress[] GetAddressList(IPAddress[] list, AddressFamily family)
		{
			if(list == null)	return null;

			int	count	= 0;
			foreach(IPAddress ip in list){
				if(ip.AddressFamily == family){
					count++;
				}
			}
			if(count <= 0)	return null;

			IPAddress[]	ret	= new IPAddress[count];
			int	i	= 0;
			foreach(IPAddress ip in list){
				if(ip.AddressFamily == family){
					ret[i++]	= ip;
				}
			}
			return ret;
		}
	}
}
