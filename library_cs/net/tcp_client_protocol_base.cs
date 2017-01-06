/*-------------------------------------------------------------------------

 TCPクライアントベース
 通信プロトコル実装용ベース

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
	public class tcp_client_protocol_base : tcp_client_base
	{
		private const string		VERSION_COMMAND		= "VERSION";
	
		public enum client_state{
			init,
			chekc_version,	// 버전確認중
			error_version,	// 버전エラー
			ready,			// 通信OK
			disconected,	// 切断されている
		};
	
		// datas[0]はコマンド
		// datas[1]からは데이터
		// 데이터수はコマンドによって異なる
		public delegate void ReceivedCommandEventHandler(object sender, string[] datas);

		private	string				m_protocol_name;		// プロトコル명
		private int					m_version;				// プロトコル버전
		private client_state		m_state;				// ステータス

		public ReceivedCommandEventHandler	ReceivedCommand;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public client_state state		{	get{	return m_state;		}}
	
		/*-------------------------------------------------------------------------
		 クライアント용
		---------------------------------------------------------------------------*/
		public tcp_client_protocol_base(string protocol_name, int version)
			: base()
		{
			init(protocol_name, version);
		}

		/*-------------------------------------------------------------------------
		 서버용
		---------------------------------------------------------------------------*/
		public tcp_client_protocol_base(string protocol_name, int version, Socket soc)
			: base(soc)
		{
			init(protocol_name, version);

			// 버전정보を送る
			send_version();
		}
	
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private void init(string protocol_name, int version)
		{
			m_protocol_name		= protocol_name;		// プロトコル명
			m_version			= version;				// プロトコル버전
			m_state				= client_state.init;	// 初期化중
	
			// 데이터受信時のハンドラ
			base.ReceivedData	+= new ReceivedDataEventHandler(received_handler);
		}

		/*-------------------------------------------------------------------------
		 데이터を送信する
		 commandには:を含んではいけない
		 VERSIONという이름のcommandは予約されている
		---------------------------------------------------------------------------*/
		public void SendData(string command, string[] datas)
		{
			if(m_state == client_state.error_version){
				throw new Exception("プロトコル버전が異なります\n버전を合わせてください");
			}
			if(m_state != client_state.ready){
				// 準備が완료していない
				throw new Exception("버전チェックが완료していません");
			}
#if DEBUG
			if(command == VERSION_COMMAND){
				throw new Exception(VERSION_COMMAND + " という이름のコマンド명は지정できません");
			}
#endif
			base.Send(CreatePacket(command, datas));
		}

		/*-------------------------------------------------------------------------
		 パケットを작성함
		---------------------------------------------------------------------------*/
		static public string CreatePacket(string command, string[] datas)
		{
#if DEBUG
			if(command.IndexOf(':') >= 0){
				throw new Exception(": を含むプロトコル명は지정できません");
			}
#endif
			string	packet	= command;
			if(   (datas != null)
				&&(datas.Length > 0) ){
				foreach(string d in datas){
					packet	+= ':' + d;
				}
			}else{
				packet	+= ":";
			}
			return packet;
		}

		/*-------------------------------------------------------------------------
		 데이터受信
		 데이터を분解してハンドラに渡す
		---------------------------------------------------------------------------*/
		private void received_handler(object sender, ReceivedDataEventArgs e)
		{
			string[]	datas	= e.received_string.Split(':');
			if(datas.Length <= 0)	return;		// 데이터エラー

			// 버전정보
			if(datas[0] == VERSION_COMMAND){
				if(datas.Length != 3){
					// エラー
					m_state		= client_state.error_version;
					return;
				}
				if(   (datas[1] != m_version.ToString())
					||(datas[2] != m_protocol_name) ){
					// エラー
					m_state		= client_state.error_version;
					return;
				}
				// 通信가능
				m_state		= client_state.ready;
				return;
			}
			
			// ハンドラに渡す
			if(ReceivedCommand != null){
				ReceivedCommand(this, datas);
			}
		}

		/*-------------------------------------------------------------------------
		 서버に接続した
		---------------------------------------------------------------------------*/
		protected override void OnConnected(EventArgs e)
		{
			base.OnConnected(e);

			// 버전정보を送る
			send_version();
		}
	
		/*-------------------------------------------------------------------------
		 버전정보を送る
		---------------------------------------------------------------------------*/
		private void send_version()
		{
			m_state		= client_state.chekc_version;
			base.Send(CreatePacket(VERSION_COMMAND, new string[]{m_version.ToString(), m_protocol_name}));
		}
	
		/*-------------------------------------------------------------------------
		 서버から切断された
		 서버から切断した
		---------------------------------------------------------------------------*/
		protected override void OnDisconnected(EventArgs e)
		{
			base.OnDisconnected(e);
			m_state		= client_state.disconected;		// 切断
		}
	}
}
