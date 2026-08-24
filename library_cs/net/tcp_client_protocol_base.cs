/*-------------------------------------------------------------------------

 TCP 클라이언트 베이스
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
	public class tcp_client_protocol_base : tcp_client_base
	{
		private const string		VERSION_COMMAND		= "VERSION";
	
		public enum client_state{
			init,
			chekc_version,	// 버전確認중
			error_version,	// 버전 오류
			ready,			// 通信OK
			disconected,	// 접속이 끊긴 상태
		};
	
		// datas[0] 은 커맨드
		// datas[1] 부터는 데이터
		// 데이터 개수는 커맨드에 따라 다르다
		public delegate void ReceivedCommandEventHandler(object sender, string[] datas);

		private	string				m_protocol_name;		// 프로토콜명
		private int					m_version;				// 프로토콜 버전
		private client_state		m_state;				// 상태

		public ReceivedCommandEventHandler	ReceivedCommand;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public client_state state		{	get{	return m_state;		}}
	
		/*-------------------------------------------------------------------------
		 클라이언트용
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

			// 버전 정보를 보낸다
			send_version();
		}
	
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private void init(string protocol_name, int version)
		{
			m_protocol_name		= protocol_name;		// 프로토콜명
			m_version			= version;				// 프로토콜 버전
			m_state				= client_state.init;	// 初期化중
	
			// 데이터 수신 시 핸들러
			base.ReceivedData	+= new ReceivedDataEventHandler(received_handler);
		}

		/*-------------------------------------------------------------------------
		 데이터를 송신한다
		 command 에는 : 를 포함해서는 안 된다
		 VERSION 이라는 이름의 command 는 예약되어 있다
		---------------------------------------------------------------------------*/
		public void SendData(string command, string[] datas)
		{
			if(m_state == client_state.error_version){
				throw new Exception("프로토콜 버전이 다릅니다\n버전을 맞춰 주십시오");
			}
			if(m_state != client_state.ready){
				// 준비가 완료되지 않았다
				throw new Exception("버전 확인이 완료되지 않았습니다");
			}
#if DEBUG
			if(command == VERSION_COMMAND){
				throw new Exception(VERSION_COMMAND + " 이라는 커맨드명은 지정할 수 없습니다");
			}
#endif
			base.Send(CreatePacket(command, datas));
		}

		/*-------------------------------------------------------------------------
		 패킷을 작성한다
		---------------------------------------------------------------------------*/
		static public string CreatePacket(string command, string[] datas)
		{
#if DEBUG
			if(command.IndexOf(':') >= 0){
				throw new Exception(": 를 포함한 프로토콜명은 지정할 수 없습니다");
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
		 데이터를 분해해서 핸들러에 넘긴다
		---------------------------------------------------------------------------*/
		private void received_handler(object sender, ReceivedDataEventArgs e)
		{
			string[]	datas	= e.received_string.Split(':');
			if(datas.Length <= 0)	return;		// 데이터 오류

			// 버전정보
			if(datas[0] == VERSION_COMMAND){
				if(datas.Length != 3){
					// 오류
					m_state		= client_state.error_version;
					return;
				}
				if(   (datas[1] != m_version.ToString())
					||(datas[2] != m_protocol_name) ){
					// 오류
					m_state		= client_state.error_version;
					return;
				}
				// 通信가능
				m_state		= client_state.ready;
				return;
			}
			
			// 핸들러에 넘긴다
			if(ReceivedCommand != null){
				ReceivedCommand(this, datas);
			}
		}

		/*-------------------------------------------------------------------------
		 서버에 접속했다
		---------------------------------------------------------------------------*/
		protected override void OnConnected(EventArgs e)
		{
			base.OnConnected(e);

			// 버전 정보를 보낸다
			send_version();
		}
	
		/*-------------------------------------------------------------------------
		 버전 정보를 보낸다
		---------------------------------------------------------------------------*/
		private void send_version()
		{
			m_state		= client_state.chekc_version;
			base.Send(CreatePacket(VERSION_COMMAND, new string[]{m_version.ToString(), m_protocol_name}));
		}
	
		/*-------------------------------------------------------------------------
		 서버로부터 접속이 끊겼다
		 서버와의 접속을 끊었다
		---------------------------------------------------------------------------*/
		protected override void OnDisconnected(EventArgs e)
		{
			base.OnDisconnected(e);
			m_state		= client_state.disconected;		// 切断
		}
	}
}
