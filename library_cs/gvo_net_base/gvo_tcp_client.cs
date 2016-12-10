/*-------------------------------------------------------------------------

 교역MapC#用
 TCPクライアント

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
	public class gvo_tcp_client : tcp_client_protocol_base
	{
		public const string			PROTOCOL_NAME		= "GVO_NAVIGATION_PROTOCOL";
		public const int			PROTOCOL_VERSION	= 1;

		// command
		private const string		COMMAND_CAPALL		= "CAPALL";
		private const string		COMMAND_CAPDAY		= "CAPDAY";
		private const string		COMMAND_SEAINFO		= "SEAINFO";
		private const string		COMMAND_ERROR		= "ERROR";

		// 同期用
		private readonly object		m_sync_object		= new object();

		// 受信データ
		private gvo_analized_data	m_received_data;
		private List<gvo_map_cs_chat_base.sea_area_type>	m_sea_info;

		// 受信フラグ
		private bool				m_enable_receive_data;
	
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public gvo_analized_data capture_data
		{
			get{
				lock(m_sync_object){
					// コピーを返す
					gvo_analized_data	data	= m_received_data.Clone();
					m_received_data.Clear();
					return data;
				}
			}
		}
		public gvo_map_cs_chat_base.sea_area_type[] sea_info
		{
			get{
				lock(m_sync_object){
					// コピーを返す
					// 내용はクリアされる
					if(m_sea_info.Count <= 0)	return null;	// 정보なし
					gvo_map_cs_chat_base.sea_area_type[] list	= m_sea_info.ToArray();
					m_sea_info.Clear();
					return list;
				}
			}
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public gvo_tcp_client()
			: base(PROTOCOL_NAME, PROTOCOL_VERSION)
		{
			init();
			// クライアントはデータ受信しない
			m_enable_receive_data	= false;
		}
		public gvo_tcp_client(Socket sct)
			: base(PROTOCOL_NAME, PROTOCOL_VERSION, sct)
		{
			init();
			// 서버はデータ受信する
			m_enable_receive_data	= true;
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private void init()
		{
			// コマンド受信ハンドラ
			base.ReceivedCommand		+= new ReceivedCommandEventHandler(received_command_handler);

			// 受信データ
			m_received_data		= new gvo_analized_data();
			m_sea_info			= new List<gvo_map_cs_chat_base.sea_area_type>();
		}

		/*-------------------------------------------------------------------------
		 캡처정보の送信
		 全て
		---------------------------------------------------------------------------*/
		public void SendCaptureAll(int days, int x, int y, float angle, bool interest, gvo_map_cs_chat_base.accident accident)
		{
			string[]	datas;
			if(accident != gvo_map_cs_chat_base.accident.none){
				datas	= new string[]{	days.ToString(),
										x.ToString(),
										y.ToString(),
										angle.ToString(),
										(interest)? "1": "0",
										gvo_map_cs_chat_base.ToAccidentString(accident)};
			}else if(interest){
				datas	= new string[]{	days.ToString(),
										x.ToString(),
										y.ToString(),
										angle.ToString(),
										"1"};
			}else{
				datas	= new string[]{	days.ToString(),
										x.ToString(),
										y.ToString(),
										angle.ToString()};
			}
			send_data(COMMAND_CAPALL, datas);
		}

		/*-------------------------------------------------------------------------
		 캡처정보の送信
		 日付のみ
		---------------------------------------------------------------------------*/
		public void SendCaptureDays(int days, bool interest)
		{
			send_data(COMMAND_CAPDAY, new string[]{	days.ToString(),
													(interest)? "1": "0"});
		}

		/*-------------------------------------------------------------------------
		 해역변동정보の送信
		---------------------------------------------------------------------------*/
		public void SendSeaInfo(gvo_map_cs_chat_base.sea_area_type info)
		{
			if(info == null)	return;
			string	type	= gvo_map_cs_chat_base.ToSeaTypeString(info.type);
			send_data(COMMAND_SEAINFO, new string[]{info.name, type});
		}

		/*-------------------------------------------------------------------------
		 通信
		---------------------------------------------------------------------------*/
		private void send_data(string command, string[] datas)
		{
			if(base.state != client_state.ready)	return;	// 通信可能でない

			try{
				SendData(command, datas);
//			}catch(Exception exc){
			}catch{
//				m_client.Dispose();
//				m_client	= null;
//				MessageBox.Show(exc.Message);
			}
		}

		/*-------------------------------------------------------------------------
		 受信
		---------------------------------------------------------------------------*/
		private void received_command_handler(object sender, string[] datas)
		{
			// 受信フラグによっては전부のデータを捨てる
			if(!m_enable_receive_data)	return;

			// 受信時は完全にロックする
			lock(m_sync_object){
				switch(datas[0]){
				case COMMAND_CAPALL:
					m_received_data.Clear();
					try{
						m_received_data.days	= Convert.ToInt32(datas[1]);
						m_received_data.pos_x	= Convert.ToInt32(datas[2]);
						m_received_data.pos_y	= Convert.ToInt32(datas[3]);
						m_received_data.angle	= Convert.ToSingle(datas[4]);
						if(datas.Length >= 6){
							// 이자정보を含む
							m_received_data.interest	= (Convert.ToInt32(datas[5]) == 0)? false: true;
						}
						if(datas.Length >= 7){
							// 재해정보を含む
							m_received_data.accident	= gvo_map_cs_chat_base.ToAccident(datas[6]);
						}
					}catch{
						m_received_data.Clear();
					}
					break;
				case COMMAND_CAPDAY:
					m_received_data.Clear();
					try{
						m_received_data.days		= Convert.ToInt32(datas[1]);
						m_received_data.interest	= (Convert.ToInt32(datas[2]) == 0)? false: true;
					}catch{
						m_received_data.Clear();
					}
					break;
				case COMMAND_SEAINFO:
					try{
						gvo_map_cs_chat_base.sea_area_type	si	= new gvo_map_cs_chat_base.sea_area_type(datas[1],
																	gvo_map_cs_chat_base.ToSeaType(datas[2]));
						m_sea_info.Add(si);
					}catch{
						m_received_data.Clear();
					}
					break;
				case COMMAND_ERROR:
					break;
				}
			}
		}
	}
}
