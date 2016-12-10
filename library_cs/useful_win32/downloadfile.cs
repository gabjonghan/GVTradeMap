/*-------------------------------------------------------------------------

 Webからのファイルダウンロード

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace useful
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class downloadfile
	{
		private long					m_file_size;		// ファイルサイズ
		private long					m_download_size;	// ダウンロードしたサイズ
		private bool					m_is_finish;		// ダウンロード完了時true
	
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public long file_size{			get{	return m_file_size;			}}
		public long download_size{		get{	return m_download_size;		}}
		public bool is_finish{			get{	return m_is_finish;			}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public downloadfile()
		{
			m_file_size		= 0;
			m_download_size	= 0;
			m_is_finish		= false;
		}

		/*-------------------------------------------------------------------------
		 ダウンロード
		---------------------------------------------------------------------------*/
		public bool Download(string url, string fname)
		{
			m_file_size		= 0;
			m_download_size	= 0;
			m_is_finish		= false;

			try{
				//WebRequestの作成
				System.Net.HttpWebRequest webreq =
				(System.Net.HttpWebRequest) System.Net.WebRequest.Create(url);

				//サーバーからの応答を受信するためのWebResponseを取得
				System.Net.HttpWebResponse webres =
				(System.Net.HttpWebResponse) webreq.GetResponse();

				m_file_size		= webres.ContentLength;
				m_download_size	= 0;

				//応答データを受信するためのStreamを取得
				using(System.IO.Stream strm = webres.GetResponseStream()){
					//ファイルに書き込むためのFileStreamを作成
					using(System.IO.FileStream fs =
						new System.IO.FileStream(fname,
							System.IO.FileMode.Create, 
							System.IO.FileAccess.Write)){

						//応答データをファイルに書き込む
						byte[] readData = new byte[1024*8];
						int readSize = 0;
						while((readSize = strm.Read(readData, 0, readData.Length)) != 0){
							fs.Write(readData, 0, readSize);
							m_download_size		+= readSize;
						}
					}
				}
			}catch{
//				string		str	= String.Format("[{0}]\nのダウンロードに失敗しました。\n", url);
//				str		+= "インターネットとの接続を確認し、再試行してください。\n";
//				str		+= "それでも問題が解決しない場合は作者に連絡してください。";
//				MessageBox.Show(str, "ファイルのダウンロード");
				return false;
			}

			m_is_finish	= true;
			return true;	
		}
	}
}
