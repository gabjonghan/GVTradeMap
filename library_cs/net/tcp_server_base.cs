/*-------------------------------------------------------------------------

 TCP서버ベース
 複数のクライアントと接続できる

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
	// 서버用イベントハンドラ
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
		
		private int						MAX_CLIENT		= 16;		// 최대接続数
		private int						DEF_BACKLOG		= 100;
	
		private Socket					m_server;
		private IPEndPoint				m_socket_ep;
		private server_state			m_state;
		private int						m_max_client;

		protected List<tcp_client_base>	m_client_list;
	
		// 同期用
		protected readonly object		m_sync_socket	= new object();

		// イベント
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
		 Listenを停止する
		 clientを全て停止させる
		---------------------------------------------------------------------------*/
		public void Close()
		{
			lock(m_sync_socket){
				if(m_state == server_state.listening){
					// listenを停止する
					try{
						m_server.Close();
					}catch{
					}
					m_server	= null;
					m_state		= server_state.stoped;
				}

				// clientを全て停止する
				if(m_client_list != null){
					// Dispose()内で목록から삭제されるため
					// foreachが使えない
					while(m_client_list.Count > 0){
						m_client_list[0].Dispose();
					}
					m_client_list	= null;
				}
			}
		}
	
		/*-------------------------------------------------------------------------
		 クライアントに送信
		---------------------------------------------------------------------------*/
		public void Send(tcp_client_base client, string str)
		{
			if(client == null)	return;
			lock(m_sync_socket){
				client.Send(str);
			}
		}

		/*-------------------------------------------------------------------------
		 全てのクライアントに送信
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
		 Listenを開始する
		---------------------------------------------------------------------------*/
		public void Listen(int portNum)
		{
			// IPv4の最初のアドレスを得る
			IPAddress[]	address_list	= net_useful.GetLocalIpAddress_Ipv4();
			if(   (address_list == null)
				||(address_list.Length <= 0) ){
				throw new ApplicationException("PCのIPアドレス取得に실패");
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
				throw new ApplicationException("서버が閉じています");
			}
			if(m_state != server_state.init){
				throw new ApplicationException("初期化상태ではありません");
			}

			m_socket_ep		= new IPEndPoint(host, portNum);
			m_server.Bind(m_socket_ep);

			m_server.Listen(backlog);
			m_state			= server_state.listening;

			//接続要求施行を開始する
			m_server.BeginAccept(new AsyncCallback(accept_callback), null);
		}

		/*-------------------------------------------------------------------------
		 tcp_client_baseの作成
		 継承時はこのメソッドをオーバーライドすること
		---------------------------------------------------------------------------*/
		protected virtual tcp_client_base CreateClient(Socket sct)
		{
			return new tcp_client_base(sct);
		}

		/*-------------------------------------------------------------------------
		 BeginAcceptのコールバック
		---------------------------------------------------------------------------*/
		private void accept_callback(IAsyncResult ar)
		{
			//接続要求を受け入れる
			Socket	soc	= null;
			try{
				lock(m_sync_socket){
					soc = m_server.EndAccept(ar);
				}
			}catch{
				this.Close();
				return;
			}

			// tcp_client_baseの作成
			tcp_client_base	client	= this.CreateClient(soc);

			// 최대数を超えていないか
			if(m_client_list.Count >= m_max_client){
				client.Close();
			}else{
				// コレクションに追加
				lock(m_sync_socket){
					m_client_list.Add(client);
				}

				// イベントハンドラの追加
				client.Disconnected		+= new EventHandler(client_disconnected);
				client.ReceivedData		+= new ReceivedDataEventHandler(client_received_data);
				// イベントを発生
				OnAcceptedClient(new ServerEventArgs(client));

				// データ受信開始
				if(!client.is_closed){
					client.StartReceive();
				}
			}

			// 接続要求施行を再開する
			m_server.BeginAccept(new AsyncCallback(accept_callback), null);
		}

		/*-------------------------------------------------------------------------
		 クライアントが切断した時
		---------------------------------------------------------------------------*/
		private void client_disconnected(object sender, EventArgs e)
		{
			// 목록から삭제する
			lock(m_sync_socket){
				m_client_list.Remove((tcp_client_base)sender);
			}
			// イベントを発生
			OnDisconnectedClient(new ServerEventArgs((tcp_client_base)sender));
		}

		/*-------------------------------------------------------------------------
		 データを受信した
		---------------------------------------------------------------------------*/
		private void client_received_data(object sender, ReceivedDataEventArgs e)
		{
			//イベントを発生
			OnReceivedData(new ReceivedDataEventArgs((tcp_client_base)sender, e.received_string));
		}

		/*-------------------------------------------------------------------------
		 クライアントと接続した
		---------------------------------------------------------------------------*/
		protected virtual void OnAcceptedClient(ServerEventArgs e)
		{
			if(AcceptedClient != null){
				AcceptedClient(this, e);
			}
		}

		/*-------------------------------------------------------------------------
		 データを受信した
		---------------------------------------------------------------------------*/
		protected virtual void OnReceivedData(ReceivedDataEventArgs e)
		{
			if(ReceivedData != null){
				ReceivedData(this, e);
			}
		}

		/*-------------------------------------------------------------------------
		 クライアントが切断した
		---------------------------------------------------------------------------*/
		protected virtual void OnDisconnectedClient(ServerEventArgs e)
		{
			if(DisconnectedClient != null){
				DisconnectedClient(this, e);
			}
		}
	}
}
