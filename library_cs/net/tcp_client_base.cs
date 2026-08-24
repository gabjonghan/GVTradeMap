/*-------------------------------------------------------------------------

 TCP 클라이언트 베이스

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
	// 데이터 수신 이벤트 핸들러
	public delegate void ReceivedDataEventHandler(object sender, ReceivedDataEventArgs e);

	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class ReceivedDataEventArgs : EventArgs
	{
		private string					m_received_string;
		private tcp_client_base			m_client;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public string			received_string		{	get{	return m_received_string;	}}
		public tcp_client_base	client				{	get{	return m_client;			}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public ReceivedDataEventArgs(tcp_client_base c, string str)
		{
			m_client			= c;
			m_received_string	= str;
		}
	}

	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class tcp_client_base : IDisposable
	{
		private Socket					m_client;
		private IPEndPoint				m_local_ep;
		private IPEndPoint				m_remote_ep;
		private MemoryStream			m_received_bytes;
		private Encoding				m_encoding;
		private bool					m_disconnected_from_server;

		private readonly object			m_sync_socket = new object();

		// event
		public event EventHandler				Connected;
		public event ReceivedDataEventHandler	ReceivedData;
		public event EventHandler				Disconnected;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public bool is_closed				{	get{
													if(m_client == null)	return true;
													return false;
												}
											}
		public bool is_connected			{	get{
													if(is_closed)	return false;
													return m_client.Connected;
												}
											}
//		public Socket		client			{	get{	return m_client;		}}
		public IPEndPoint	local_ep		{	get{	return m_local_ep;		}}
		public IPEndPoint	remote_ep		{	get{	return m_remote_ep;		}}
		protected Encoding	encoding		{	get{	return m_encoding;		}
												set{	m_encoding	= value;	}}
	
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public tcp_client_base()
		{
			m_client		= new Socket(	AddressFamily.InterNetwork,
											SocketType.Stream, ProtocolType.Tcp);
			m_local_ep		= null;
			m_remote_ep		= null;

			initialize();
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public tcp_client_base(Socket soc)
		{
			m_client		= soc;
			m_local_ep		= (IPEndPoint)soc.LocalEndPoint;
			m_remote_ep		= (IPEndPoint)soc.RemoteEndPoint;

			initialize();
		}

		/*-------------------------------------------------------------------------
		 初期化
		---------------------------------------------------------------------------*/
		private void initialize()
		{
			m_encoding		= Encoding.UTF8;
			m_disconnected_from_server	= false;
		}
	
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public virtual void Dispose()
		{
			Close();
		}

		/*-------------------------------------------------------------------------
		 닫기
		---------------------------------------------------------------------------*/
		public void Close()
		{
			Close(true);
		}
		public void Close(bool is_self)
		{
			if(m_client == null)	return;

			try{
				m_client.Shutdown(SocketShutdown.Both);
				m_client.Close();
			}catch{
			}
			m_client	= null;

			if(m_received_bytes != null){
				m_received_bytes.Close();
				m_received_bytes	= null;
			}
			m_disconnected_from_server	= !is_self;

			// 이벤트 발생
			OnDisconnected(new EventArgs());
		}

		/*-------------------------------------------------------------------------
		 送信
		---------------------------------------------------------------------------*/
		public void Send(string str)
		{
			if(!is_connected){
				throw new ApplicationException("접속되어 있지 않습니다");
			}
			
			// 문자열을 Byte 형 배열로 변환
			// EOT 를 추가한다
			byte[] send_bytes	= m_encoding.GetBytes(str + "\u0004");

			lock(m_sync_socket){
				//데이터를 송신한다
				m_client.Send(send_bytes);
			}
		}

		/*-------------------------------------------------------------------------
		 서버에 접속한다
		---------------------------------------------------------------------------*/
		public void Connect(string host, int port)
		{
			IPAddress[]	list	= net_useful.GetIpAddressIpv4(host);
			if(   (list == null)
				||(list.Length <= 0) ){
				throw new ApplicationException("서버를 찾을 수 없습니다");
			}
			Connect(list[0], port);
		}
		public void Connect(IPAddress host, int port)
		{
			if(is_closed){
				throw new ApplicationException("서버를 찾을 수 없습니다");
			}
			if(is_connected){
				throw new ApplicationException("이미 접속중입니다");
			}

			// 접속한다
			m_client.Connect(new IPEndPoint(host, port));

			m_local_ep		= (IPEndPoint)m_client.LocalEndPoint;
			m_remote_ep		= (IPEndPoint)m_client.RemoteEndPoint;

			// 이벤트 발생
			OnConnected(new EventArgs());

			// 비동기 데이터 수신을 시작한다
			StartReceive();
		}
	
		/*-------------------------------------------------------------------------
		 비동기 데이터 수신을 시작한다
		---------------------------------------------------------------------------*/
		public void StartReceive()
		{
			if(!is_connected){
				throw new ApplicationException("접속되어 있지 않습니다");
			}

			// 初期化
			byte[]	receive_buffer	= new byte[1024];
			m_received_bytes		= new MemoryStream();

			// 비동기 수신 시작
			try{
				m_client.BeginReceive(	receive_buffer,
										0, receive_buffer.Length,
										SocketFlags.None,
										new AsyncCallback(receive_data_callback),
										receive_buffer);
			}catch{
				Close(false);
			}
		}

		/*-------------------------------------------------------------------------
		 BeginReceive 의 콜백
		---------------------------------------------------------------------------*/
		private void receive_data_callback(IAsyncResult ar)
		{
			int		len = -1;
			//읽어들인 길이를 가져온다
			try{
				lock(m_sync_socket){
					len	= m_client.EndReceive(ar);
				}
			}catch{
			}

			//접속이 끊겼는지 확인한다
			if(len <= 0){
				Close(false);
				return;
			}

			//수신한 데이터를 가져온다
			byte[]	receive_buffer	= (byte[])ar.AsyncState;

			//수신한 데이터를 누적한다
			m_received_bytes.Write(receive_buffer, 0, len);

			//최댓값을 넘었을 때는 접속을 닫는다
			if(m_received_bytes.Length > int.MaxValue){
				Close(false);
				return;
			}

			//끝까지 수신했는지 확인한다
			if(m_received_bytes.Length >= 1){
				m_received_bytes.Seek(-1, System.IO.SeekOrigin.End);
				if(m_received_bytes.ReadByte() == (int)'\u0004'){	// 종료 코드가 있으면
					// 끝까지 수신했을 때
					// 수신한 데이터를 문자열로 변환
					string	str = m_encoding.GetString(m_received_bytes.ToArray());
					m_received_bytes.Close();

					// 분해한다
					int startPos = 0, endPos;
					while((endPos = str.IndexOf('\u0004', startPos)) >=0){
						string line = str.Substring(startPos, endPos - startPos);
						startPos = endPos + 1;
						// 이벤트 발생
						OnReceivedData(new ReceivedDataEventArgs(this, line));
					}
					m_received_bytes	= new MemoryStream();
				}else{
					// 맨 뒤로 시크한다
					m_received_bytes.Seek(0, System.IO.SeekOrigin.End);
				}
			}

			try{
				lock(m_sync_socket){
					// 다시 수신 시작
					m_client.BeginReceive(	receive_buffer,
											0, receive_buffer.Length,
											SocketFlags.None,
											new AsyncCallback(receive_data_callback),
											receive_buffer);
				}
			}catch{
				Close(false);
			}
		}
	
		/*-------------------------------------------------------------------------
		 서버에 접속했다
		---------------------------------------------------------------------------*/
		protected virtual void OnConnected(EventArgs e)
		{
			if(Connected != null){
				Connected(this, e);
			}
		}

		/*-------------------------------------------------------------------------
		 서버로부터 접속이 끊겼다
		 서버와의 접속을 끊었다
		---------------------------------------------------------------------------*/
		protected virtual void OnDisconnected(EventArgs e)
		{
			if(Disconnected != null){
				Disconnected(this, e);
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
	}
}
