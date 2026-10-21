//-------------------------------------------------------------------------
//
// Http 다운로드
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
	/// Http 다운로드
	/// </summary>
	static public class HttpDownload
	{
		//-------------------------------------------------------------------------
		/// <summary>
		/// TLS 1.2 를 유효하게 한다
		/// .NET 2.0 의 SecurityProtocolType 에는 Tls12 가 없으므로 값(3072)으로 지정한다
		/// TLS 1.2 를 지원하지 않는 환경에서는 예외가 나므로 기존 설정 그대로 둔다
		/// </summary>
		static HttpDownload()
		{
			try{
				ServicePointManager.SecurityProtocol	|= (SecurityProtocolType)3072;
			}catch{
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 다운로드해서 파일에 기록한다
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="write_file_name">기록할 파일명</param>
		/// <returns>다운로드에 성공한 경우 true</returns>
		static public bool Download(string url, string write_file_name)
		{
			try{
				//WebRequest 작성
				HttpWebRequest	webreq = (HttpWebRequest)System.Net.WebRequest.Create(url);

				//서버로부터의 응답을 수신하기 위한 WebResponse 취득
				HttpWebResponse	webres = (HttpWebResponse)webreq.GetResponse();

				//응답 데이터를 수신하기 위한 Stream 취득
				using(Stream strm = webres.GetResponseStream()){
					//파일에 쓰기 위한 FileStream 작성
					using(FileStream fs = new FileStream(write_file_name, FileMode.Create, FileAccess.Write)){
						//응답 데이터를 파일에 기록한다
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
		/// 다운로드해서 문자열로 반환한다.
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="encoder">Encoding</param>
		/// <returns>다운로드한 문자열, 실패한 경우 null 을 반환한다</returns>
		static public string Download(string url, Encoding encoder)
		{
			try{
				//WebRequest 작성
				HttpWebRequest	webreq = (HttpWebRequest)System.Net.WebRequest.Create(url);

				//서버로부터의 응답을 수신하기 위한 WebResponse 취득
				HttpWebResponse	webres = (HttpWebResponse)webreq.GetResponse();

				string	str;
				using(StreamReader sr = new StreamReader(webres.GetResponseStream(), encoder)){
					// 전부 로드
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
