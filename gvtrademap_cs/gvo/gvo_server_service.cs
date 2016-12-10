/*-------------------------------------------------------------------------

 TCP서버コントロール

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;

using Utility;
using net_base;
using gvo_base;
using gvo_net_base;
using System.Windows.Forms;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class gvo_server_service : IDisposable
	{
		private gvo_tcp_server				m_server;
		private bool						m_is_error;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		// 서버を시작しているかどうかを得る
		public bool is_listening
		{
			get{
				if(m_server == null)											return false;
				if(m_server.state != tcp_server_base.server_state.listening)	return false;
				return true;
			}
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public gvo_server_service()
		{
			m_server	= null;
			m_is_error	= false;
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public void Dispose()
		{
			Close();
		}

		/*-------------------------------------------------------------------------
		 닫기
		---------------------------------------------------------------------------*/
		public void Close()
		{
			if(m_server != null){
				m_server.Close();
				m_server	= null;
				m_is_error	= false;
			}
		}

		/*-------------------------------------------------------------------------
		 開始
		---------------------------------------------------------------------------*/
		public void Listen(int port_index)
		{
			if(m_is_error)	return;

			Close();
			try{
				m_server	= new gvo_tcp_server();
				m_server.Listen(port_index);
			}catch{
				Close();
				m_is_error	= true;		// 오류
				MessageBox.Show("TCP서버の시작に실패하였습니다. ", "TCP서버시작오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/*-------------------------------------------------------------------------
		 クライアントを得る
		 通信중でない場合はnullを返す
		---------------------------------------------------------------------------*/
		public gvo_tcp_client GetClient()
		{
			if(!is_listening)		return null;

			gvo_tcp_client[]	list	= m_server.client_list;
			if(list == null)		return null;
			if(list.Length <= 0)	return null;

			// 最初に연결されたクライアント
			return list[0];
		}
	}
}
