//-------------------------------------------------------------------------
//
// Httpダウンロード
//
//-------------------------------------------------------------------------
using System.Net;
using System.IO;
using System.Text;

//-------------------------------------------------------------------------
namespace Utility
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// Httpダウンロード
	/// </summary>
	static public class HttpDownload
	{
		//-------------------------------------------------------------------------
		/// <summary>
		/// ダウンロード, ファイルに書き出す
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="write_file_name">書き出し先ファイル명</param>
		/// <returns>ダウンロードに成功した場合true</returns>
		static public bool Download(string url, string write_file_name)
		{
			try{
				//WebRequestの作成
				HttpWebRequest	webreq = (HttpWebRequest)System.Net.WebRequest.Create(url);

				//서버ーからの応答を受信するためのWebResponseを取得
				HttpWebResponse	webres = (HttpWebResponse)webreq.GetResponse();

				//応答データを受信するためのStreamを取得
				using(Stream strm = webres.GetResponseStream()){
					//ファイルに書き込むためのFileStreamを作成
					using(FileStream fs = new FileStream(write_file_name, FileMode.Create, FileAccess.Write)){
						//応答データをファイルに書き込む
						byte[] readData = new byte[1024*8];
						int readSize = 0;
						while((readSize = strm.Read(readData, 0, readData.Length)) != 0){
							fs.Write(readData, 0, readSize);
						}
					}
				}
				webres.Close();
			}catch{
				return false;
			}
			return true;	
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// ダウンロード, 文字列で返す. 
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="encoder">Encoding</param>
		/// <returns>ダウンロードした文字列, 실패した場合nullを返す</returns>
		static public string Download(string url, Encoding encoder)
		{
			try{
				//WebRequestの作成
				HttpWebRequest	webreq = (HttpWebRequest)System.Net.WebRequest.Create(url);

				//서버ーからの応答を受信するためのWebResponseを取得
				HttpWebResponse	webres = (HttpWebResponse)webreq.GetResponse();

				string	str;
				using(StreamReader sr = new StreamReader(webres.GetResponseStream(), encoder)){
					// 全て로드
					str		= sr.ReadToEnd();
				}
				webres.Close();
				return str;
			}catch{
				// 실패
				return null;
			}
		}
	}
}
