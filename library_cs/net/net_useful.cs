/*-------------------------------------------------------------------------

 네트워크 관련해서 자주 쓰는 것들

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
		 로컬 호스트명을 얻는다
		---------------------------------------------------------------------------*/
		static public string GetHostName()
		{
			return Dns.GetHostName();
		}
	
		/*-------------------------------------------------------------------------
		 로컬 PC 의 IP 주소를 얻는다
		 IPv4 와 IPv6 가 섞인 목록이 반환된다
		 동기 방식이라 주소 해석에 시간이 걸릴 수 있다
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
		 로컬 PC 의 IP 주소를 얻는다
		 IPv4 만
		 동기 방식이라 주소 해석에 시간이 걸릴 수 있다
		---------------------------------------------------------------------------*/
		static public IPAddress[] GetLocalIpAddress_Ipv4()
		{
			return GetAddressListIpv4(GetLocalIpAddress());
		}

		/*-------------------------------------------------------------------------
		 로컬 PC 의 IP 주소를 얻는다
		 IPv6 만
		 동기 방식이라 주소 해석에 시간이 걸릴 수 있다
		---------------------------------------------------------------------------*/
		static public IPAddress[] GetLocalIpAddress_Ipv6()
		{
			return GetAddressListIpv6(GetLocalIpAddress());
		}

		/*-------------------------------------------------------------------------
		 지정한 host 의 IP 주소를 얻는다
		---------------------------------------------------------------------------*/
		static public IPAddress[] GetIpAddress(string host)
		{
			return Dns.GetHostEntry(host).AddressList;
		}

		/*-------------------------------------------------------------------------
		 지정한 host 의 IP 주소를 얻는다
		 IPv4 만
		---------------------------------------------------------------------------*/
		static public IPAddress[] GetIpAddressIpv4(string host)
		{
			return GetAddressListIpv4(GetIpAddress(host));
		}

		/*-------------------------------------------------------------------------
		 지정한 host 의 IP 주소를 얻는다
		 IPv6 만
		---------------------------------------------------------------------------*/
		static public IPAddress[] GetIpAddressIpv6(string host)
		{
			return GetAddressListIpv6(GetIpAddress(host));
		}
	
		/*-------------------------------------------------------------------------
		 지정한 AddressFamily 만 담은 목록을 얻는다
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
