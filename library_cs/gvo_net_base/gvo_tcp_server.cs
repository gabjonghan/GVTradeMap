/*-------------------------------------------------------------------------

 교역MapC#용
 TCP서버

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

using gvo_base;
using net_base;
using Utility;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvo_net_base
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class gvo_tcp_server : tcp_server_protocol_base
	{
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		new public gvo_tcp_client[]	client_list
		{
			get{
				lock(m_sync_socket){
					gvo_tcp_client[]	list	= new gvo_tcp_client[m_client_list.Count];
					for(int i=0; i<m_client_list.Count; i++){
						list[i]		= m_client_list[i] as gvo_tcp_client;
					}
					return list;
				}
			}
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public gvo_tcp_server()
			: base(gvo_tcp_client.PROTOCOL_NAME, gvo_tcp_client.PROTOCOL_VERSION)
		{
			// マルチ유저は未대응
			base.max_client		= 1;
		}
	
		/*-------------------------------------------------------------------------
		 tcp_client_baseの작성
		 継承時はこのメソッドをオーバーライドすること
		---------------------------------------------------------------------------*/
		protected override tcp_client_base CreateClient(Socket sct)
		{
			return new gvo_tcp_client(sct);
		}
	}
}
