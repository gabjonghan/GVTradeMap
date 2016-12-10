/*-------------------------------------------------------------------------

 TCP通信
 テキスト

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Text;
using System.Net.Sockets;
using System.IO;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace useful
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	static public class tcp_text
	{
		private const int				RECEIVETIMEOUT		= 5000;

		/*-------------------------------------------------------------------------
		 テキストデータをダウンロードする
		 失敗した場合はnullを返す
		---------------------------------------------------------------------------*/
		public static string Download(string hostname, string htmlpage, Encoding encoder)
		{
			try{
				string		str;
				using(TcpClient client = new TcpClient(hostname, 80)){
					client.ReceiveTimeout			= RECEIVETIMEOUT;

					NetworkStream	stream			= client.GetStream();

					// 送信
					string			message			= "GET " + htmlpage;
									message			+= " HTTP/1.0\nAccept: */*\nAccept-Language: ja\nUser-Agent: Mozilla/4.0\nHost: ";
									message			+= hostname + "\nConnection: keep-alive\n\n";
					Byte[]			send_data		= encoder.GetBytes(message);
					stream.Write(send_data, 0, send_data.Length);

					// 受信
					using(StreamReader sr = new StreamReader(stream, encoder)){
						// 全て読み込む
						str		= sr.ReadToEnd();
					}
				}
				return str;
			}catch{
				// エラー
				return null;
			}
		}
	}
}
