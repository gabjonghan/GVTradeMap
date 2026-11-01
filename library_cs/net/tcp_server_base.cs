/*-------------------------------------------------------------------------

 TCP 서버 베이스
 여러 클라이언트와 접속할 수 있다

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
	// 서버용 이벤트 핸들러
	public delegate void ServerEventHandler(object sender, ServerEventArgs e);

	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class ServerEventArgs
	{
		private tcp_client_base				m_client;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public tcp_client_base client		{	get{	return m_client;		}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public ServerEventArgs(tcp_client_base c)
		{
			m_client	= c;
		}
	}	

	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class tcp_server_base : IDisposable
	{
		public enum server_state{
			init,
			listening,
			stoped,
		};
		
		private int						MAX_CLIENT		= 16;		// 최대 접속 수
		private int						DEF_BACKLOG		= 100;
	
		private Socket					m_server;
		private IPEndPoint				m_socket_ep;
		private server_state			m_state;
		private int						m_max_client;

		protected List<tcp_client_base>	m_client_list;
	
		// 동기용
		protected readonly object		m_sync_socket	= new object();

		// 이벤트
		public event ServerEventHandler			AcceptedClient;
		public event ReceivedDataEventHandler	ReceivedData;
		public event ServerEventHandler			DisconnectedClient;
	
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
//		public Socket		server			{	get{	return m_server;		}}
		public IPEndPoint	socket_ep		{	get{	return m_socket_ep;		}}
		public int			max_client		{	get{	return m_max_client;		}
												set{	m_max_client	= value;	}}
		public server_state	state			{	get{	return m_state;			}}

		public tcp_client_base[]	client_list	{	get{
														lock(m_sync_socket){
															return m_client_list.ToArray();
														}
													}
												}
	
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public tcp_server_base()
		{
			m_max_client		= MAX_CLIENT;
			m_server			= new Socket(	AddressFamily.InterNetwork,
												SocketType.Stream, ProtocolType.Tcp);
			m_state				= server_state.init;
			m_client_list		= new List<tcp_client_base>();
		}
	
		/*-------------------------------------------------------------------------
		 Dispose
		---------------------------------------------------------------------------*/
		public virtual void Dispose()
		{
			Close();
		}

		/*-------------------------------------------------------------------------
		 Listen 을 정지한다
		 client 를 모두 정지시킨다
		---------------------------------------------------------------------------*/
		public void Close()
		{
			lock(m_sync_socket){
				if(m_state == server_state.listening){
					// listen 을 정지한다
					try{
						m_server.Close();
					}catch{
					}
					m_server	= null;
					m_state		= server_state.stoped;
				}

				// client 를 모두 정지한다
				if(m_client_list != null){
					// Dispose() 안에서 목록으로부터 삭제되기 때문에
					// foreach 를 쓸 수 없다
					while(m_client_list.Count > 0){
						m_client_list[0].Dispose();
					}
					m_client_list	= null;
				}
			}
		}
	
		/*-------------------------------------------------------------------------
		 클라이언트에 송신
		---------------------------------------------------------------------------*/
		public void Send(tcp_client_base client, string str)
		{
			if(client == null)	return;
			lock(m_sync_socket){
				client.Send(str);
			}
		}

		/*-------------------------------------------------------------------------
		 모든 클라이언트에 송신
		---------------------------------------------------------------------------*/
		public void SendToAllClients(string str)
		{
			if(m_client_list == null)	return;

			lock(m_sync_socket){
				foreach(tcp_client_base i in m_client_list){
					i.Send(str);
				}
			}
		}

		/*-------------------------------------------------------------------------
		 Listen 을 시작한다
		---------------------------------------------------------------------------*/
		public void Listen(int portNum)
		{
			// IPv4 의 첫 번째 주소를 얻는다
			IPAddress[]	address_list	= net_useful.GetLocalIpAddress_Ipv4();
			if(   (address_list == null)
				||(address_list.Length <= 0) ){
				throw new ApplicationException("PC 의 IP 주소 취득에 실패");
			}
			Listen(address_list[0], portNum);
		}
		public void Listen(IPAddress host, int portNum)
		{
			Listen(host, portNum, DEF_BACKLOG);
		}
		public void Listen(IPAddress host, int portNum, int backlog)
		{
			if(m_server == null){
				throw new ApplicationException("서버가 닫혀 있습니다");
			}
			if(m_state != server_state.init){
				throw new ApplicationException("초기화 상태가 아닙니다");
			}

			m_socket_ep		= new IPEndPoint(host, portNum);
			m_server.Bind(m_socket_ep);

			m_server.Listen(backlog);
			m_state			= server_state.listening;

			//접속 요청 접수를 시작한다
			m_server.BeginAccept(new AsyncCallback(accept_callback), null);
		}

		/*-------------------------------------------------------------------------
		 tcp_client_base 의 작성
		 상속할 때는 이 메서드를 오버라이드할 것
		---------------------------------------------------------------------------*/
		protected virtual tcp_client_base CreateClient(Socket sct)
		{
			return new tcp_client_base(sct);
		}

		/*-------------------------------------------------------------------------
		 BeginAccept 의 콜백
		---------------------------------------------------------------------------*/
		private void accept_callback(IAsyncResult ar)
		{
			//접속 요청을 받아들인다
			Socket	soc	= null;
			try{
				lock(m_sync_socket){
					soc = m_server.EndAccept(ar);
				}
			}catch{
				this.Close();
				return;
			}

			// tcp_client_base 의 작성
			tcp_client_base	client	= this.CreateClient(soc);

			// 최대 개수를 넘지 않았는지
			if(m_client_list.Count >= m_max_client){
				client.Close();
			}else{
				// 컬렉션에 추가
				lock(m_sync_socket){
					m_client_list.Add(client);
				}

				// 이벤트 핸들러 추가
				client.Disconnected		+= new EventHandler(client_disconnected);
				client.ReceivedData		+= new ReceivedDataEventHandler(client_received_data);
				// 이벤트 발생
				OnAcceptedClient(new ServerEventArgs(client));

				// 데이터 수신 시작
				if(!client.is_closed){
					client.StartReceive();
				}
			}

			// 접속 요청 접수를 재개한다
			m_server.BeginAccept(new AsyncCallback(accept_callback), null);
		}

		/*-------------------------------------------------------------------------
		 클라이언트가 접속을 끊었을 때
		---------------------------------------------------------------------------*/
		private void client_disconnected(object sender, EventArgs e)
		{
			// 목록에서 삭제한다
			lock(m_sync_socket){
				m_client_list.Remove((tcp_client_base)sender);
			}
			// 이벤트 발생
			OnDisconnectedClient(new ServerEventArgs((tcp_client_base)sender));
		}

		/*-------------------------------------------------------------------------
		 데이터를 수신했다
		---------------------------------------------------------------------------*/
		private void client_received_data(object sender, ReceivedDataEventArgs e)
		{
			//이벤트 발생
			OnReceivedData(new ReceivedDataEventArgs((tcp_client_base)sender, e.received_string));
		}

		/*-------------------------------------------------------------------------
		 클라이언트와 접속했다
		---------------------------------------------------------------------------*/
		protected virtual void OnAcceptedClient(ServerEventArgs e)
		{
			if(AcceptedClient != null){
				AcceptedClient(this, e);
			}
		}

		/*-------------------------------------------------------------------------
		 데이터를 수신했다
		---------------------------------------------------------------------------*/
		protected virtual void OnReceivedData(ReceivedDataEventArgs e)
		{
			if(ReceivedData != null){
				ReceivedData(this, e);
			}
		}

		/*-------------------------------------------------------------------------
		 클라이언트가 접속을 끊었다
		---------------------------------------------------------------------------*/
		protected virtual void OnDisconnectedClient(ServerEventArgs e)
		{
			if(DisconnectedClient != null){
				DisconnectedClient(this, e);
			}
		}
	}
}
