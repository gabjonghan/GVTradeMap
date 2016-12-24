/*-------------------------------------------------------------------------

 TCPクライアントベース

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
	// 데이터受信イベントハンドラ
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

			// イベントを発生
			OnDisconnected(new EventArgs());
		}

		/*-------------------------------------------------------------------------
		 送信
		---------------------------------------------------------------------------*/
		public void Send(string str)
		{
			if(!is_connected){
				throw new ApplicationException("接続していません");
			}
			
			// 문자열をByte형配열に변환
			// EOTを추가する
			byte[] send_bytes	= m_encoding.GetBytes(str + "\u0004");

			lock(m_sync_socket){
				//데이터を送信する
				m_client.Send(send_bytes);
			}
		}

		/*-------------------------------------------------------------------------
		 서버ーに接続する
		---------------------------------------------------------------------------*/
		public void Connect(string host, int port)
		{
			IPAddress[]	list	= net_useful.GetIpAddressIpv4(host);
			if(   (list == null)
				||(list.Length <= 0) ){
				throw new ApplicationException("서버ーが見つかりません");
			}
			Connect(list[0], port);
		}
		public void Connect(IPAddress host, int port)
		{
			if(is_closed){
				throw new ApplicationException("接続していません");
			}
			if(is_connected){
				throw new ApplicationException("接続済です");
			}

			// 接続する
			m_client.Connect(new IPEndPoint(host, port));

			m_local_ep		= (IPEndPoint)m_client.LocalEndPoint;
			m_remote_ep		= (IPEndPoint)m_client.RemoteEndPoint;

			// イベントを発生
			OnConnected(new EventArgs());

			// 非同期데이터受信を開始する
			StartReceive();
		}
	
		/*-------------------------------------------------------------------------
		 非同期데이터受信を開始する
		---------------------------------------------------------------------------*/
		public void StartReceive()
		{
			if(!is_connected){
				throw new ApplicationException("接続していません");
			}

			// 初期化
			byte[]	receive_buffer	= new byte[1024];
			m_received_bytes		= new MemoryStream();

			// 非同期受信を開始
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
		 BeginReceiveのコールバック
		---------------------------------------------------------------------------*/
		private void receive_data_callback(IAsyncResult ar)
		{
			int		len = -1;
			//読み込んだ長さを取得
			try{
				lock(m_sync_socket){
					len	= m_client.EndReceive(ar);
				}
			}catch{
			}

			//切断されたか調べる
			if(len <= 0){
				Close(false);
				return;
			}

			//受信した데이터を取得する
			byte[]	receive_buffer	= (byte[])ar.AsyncState;

			//受信した데이터を蓄積する
			m_received_bytes.Write(receive_buffer, 0, len);

			//최대値を超えた時は, 接続を닫기
			if(m_received_bytes.Length > int.MaxValue){
				Close(false);
				return;
			}

			//最후まで受信したか調べる
			if(m_received_bytes.Length >= 1){
				m_received_bytes.Seek(-1, System.IO.SeekOrigin.End);
				if(m_received_bytes.ReadByte() == (int)'\u0004'){	// 종료コードがあれば
					// 最후まで受信した時
					// 受信した데이터を문자열に변환
					string	str = m_encoding.GetString(m_received_bytes.ToArray());
					m_received_bytes.Close();

					// 분解する
					int startPos = 0, endPos;
					while((endPos = str.IndexOf('\u0004', startPos)) >=0){
						string line = str.Substring(startPos, endPos - startPos);
						startPos = endPos + 1;
						// イベントを発生
						OnReceivedData(new ReceivedDataEventArgs(this, line));
					}
					m_received_bytes	= new MemoryStream();
				}else{
					// 一番후ろにシークする
					m_received_bytes.Seek(0, System.IO.SeekOrigin.End);
				}
			}

			try{
				lock(m_sync_socket){
					// 다시び受信開始
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
		 서버に接続した
		---------------------------------------------------------------------------*/
		protected virtual void OnConnected(EventArgs e)
		{
			if(Connected != null){
				Connected(this, e);
			}
		}

		/*-------------------------------------------------------------------------
		 서버から切断された
		 서버から切断した
		---------------------------------------------------------------------------*/
		protected virtual void OnDisconnected(EventArgs e)
		{
			if(Disconnected != null){
				Disconnected(this, e);
			}
		}

		/*-------------------------------------------------------------------------
		 데이터を受信した
		---------------------------------------------------------------------------*/
		protected virtual void OnReceivedData(ReceivedDataEventArgs e)
		{
			if(ReceivedData != null){
				ReceivedData(this, e);
			}
		}
	}
}
