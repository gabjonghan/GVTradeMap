/*-------------------------------------------------------------------------

 TCP 서버 베이스
 통신 프로토콜 구현용 베이스

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.IO;
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
	public class tcp_server_protocol_base : tcp_server_base
	{
		private	string				m_protocol_name;		// 프로토콜명
		private int					m_version;				// 프로토콜 버전

		private readonly object		m_sync_object	= new object();

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		new public tcp_client_protocol_base[]	client_list
		{
			get{
				lock(m_sync_socket){
					tcp_client_protocol_base[]	list	= new tcp_client_protocol_base[m_client_list.Count];
					for(int i=0; i<m_client_list.Count; i++){
						list[i]		= m_client_list[i] as tcp_client_protocol_base;
					}
					return list;
				}
			}
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public tcp_server_protocol_base(string protocol_name, int version)
			: base()
		{
			m_protocol_name		= protocol_name;		// 프로토콜명
			m_version			= version;				// 프로토콜 버전

			base.ReceivedData	+= new ReceivedDataEventHandler(received_handler);
		}

		/*-------------------------------------------------------------------------
		 데이터受信
		---------------------------------------------------------------------------*/
		private void received_handler(object sender, ReceivedDataEventArgs e)
		{
		}

		/*-------------------------------------------------------------------------
		 tcp_client_base 의 작성
		 상속할 때는 이 메서드를 오버라이드할 것
		---------------------------------------------------------------------------*/
		protected override tcp_client_base CreateClient(Socket sct)
		{
			return new tcp_client_protocol_base(m_protocol_name, m_version, sct);
		}

		/*-------------------------------------------------------------------------
		 모든 클라이언트에 송신
		---------------------------------------------------------------------------*/
		public void SendDataToAllClients(string command, string[] datas)
		{
			if(m_client_list == null)	return;

			string	packet	= tcp_client_protocol_base.CreatePacket(command, datas);
			lock(m_sync_socket){
				foreach(tcp_client_base ii in m_client_list){
					tcp_client_protocol_base	i	= ii as tcp_client_protocol_base;
					if(i == null)												continue;
					if(i.state != tcp_client_protocol_base.client_state.ready)	continue;

					i.Send(packet);
				}
			}
		}
	}
}
