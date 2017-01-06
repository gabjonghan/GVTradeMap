/*-------------------------------------------------------------------------

 チャット분석のベース
 リクエスト付き
 지정したルールでの분석が가능
 분석は1行単位のみ
 複수行での분석は実機からリアルタイムに로그が書き出されるため, 
 まだ必要な로그が書き出されていない가능性がある

 스레드대응版

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using Utility;
using System.Text.RegularExpressions;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvo_base
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public abstract class gvo_chat_base : RequestCtrl
	{
		public enum type{
			index0,		// 1문자目から一致するもの
			any_index,	// どこかに含まれればいいもの
			regex,		// 正規表現
		};
	
		/*-------------------------------------------------------------------------
		 분석대상
		---------------------------------------------------------------------------*/
		public class analize_data
		{
			private string				m_analize;
			private type				m_type;
			private Regex				m_regex;		// 正規表現時の분석대상
			private object				m_tag;			// 분석결과참조용タグ

			/*-------------------------------------------------------------------------
			
			---------------------------------------------------------------------------*/
			public string analize		{	get{	return m_analize;	}}
			public type type			{	get{	return m_type;		}}
			public object tag			{	get{	return m_tag;		}}

			/*-------------------------------------------------------------------------
			
			---------------------------------------------------------------------------*/
			public analize_data(string analize, type type)
			{
				init(analize, type);
			}
			public analize_data(string analize, type type, object tag)
			{
				init(analize, type);
				m_tag		= tag;
			}

			/*-------------------------------------------------------------------------
			 init
			---------------------------------------------------------------------------*/
			private void init(string analize, type type)
			{
				m_analize	= analize;
				m_type		= type;
				m_regex		= null;
				m_tag		= null;

				// 正規表現時はコンパイルしておく
				if(type == type.regex){
					m_regex	= new Regex(analize);
				}
			}

			/*-------------------------------------------------------------------------
			 분석
			---------------------------------------------------------------------------*/
			public bool Analize(string line, List<analized_data> list)
			{
				switch(m_type){
				case type.index0:
					// index 0 から見つかる
					if(line.IndexOf(m_analize) == 0){
						list.Add(new analized_data(this, line));
						return true;
					}
					break;
				case type.any_index:
					// どこかに含まれる
					if(line.IndexOf(m_analize) >= 0){
						list.Add(new analized_data(this, line));
						return true;
					}
					break;
				case type.regex:
					// 正規表現
					Match	match	= m_regex.Match(line);
					if(match.Success){
						list.Add(new analized_data(this, line, match));
						return true;
					}
					break;
				}
				return false;
			}
		};

		/*-------------------------------------------------------------------------
		 분석결과
		---------------------------------------------------------------------------*/
		public class analized_data
		{
			private analize_data			m_analize_data;
			private string					m_line;
			private Match					m_match;	// 正規表現時

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public string line			{	get{	return m_line;					}}
			public Match match			{	get{	return m_match;					}}
			public object tag			{	get{	return m_analize_data.tag;		}}

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public analized_data(analize_data analize, string line)
			{
				m_analize_data		= analize;
				m_line				= line;
				m_match				= null;
			}

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public analized_data(analize_data analize, string line, Match match)
			{
				m_analize_data		= analize;
				m_line				= line;
				m_match				= match;
			}
		};
	
		private string						m_path;							// 로그パス
		private FileInfo					m_newest_chat_file_info;		// 最新の로그파일정보
		private FileInfo					m_chat_file_info;				// 분석대상の로그파일정보
		private int							m_analyze_lines;				// 분석した行수

		private List<analize_data>			m_analize_list;					// 분석대상
		private List<analized_data>			m_analized_list;				// 분석결과

		// 스레드대응
		private readonly object				m_syncobject	= new object();

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public List<analized_data> analized_list	{
											get{
												List<analized_data>	list;
												lock(m_syncobject){
													// 新しい분석정보は差し替えのため, 
													// 참조が得られればよい
													list	= m_analized_list;
												}
												return list;
											}
									private set{
												lock(m_syncobject){
													m_analized_list	= value;
												}
											}
									}

		public string current_log_fullname	{	get{
													if(m_chat_file_info == null)	return "";
													return m_chat_file_info.FullName;
												}
											}
		public string current_log_name		{	get{
													if(m_chat_file_info == null)	return "";
													return m_chat_file_info.Name;
												}
											}
		public bool is_update				{	get{
													if(analized_list == null)		return false;
													if(analized_list.Count <= 0)	return false;
													return true;
												}
											}
		public string path					{	get{	return m_path;			}
												set{
													m_path				= value;
													ResetNewestLogInfo();
												}
											}
	
		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public gvo_chat_base()
		{
			init(gvo_def.GetGvoLogPath());
		}
		public gvo_chat_base(string path)
		{
			init(path);
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private void init(string path)
		{
			m_path						= path;

			m_newest_chat_file_info		= null;		// 更新が最も新しい로그
			m_chat_file_info			= null;		// 분석대상の로그
			m_analyze_lines				= 0;		// 분석현황

			m_analize_list				= new List<analize_data>();
			m_analized_list				= null;
		}
	
		/*-------------------------------------------------------------------------
		 분석정보を추가する
		 tagで判定すること
		---------------------------------------------------------------------------*/
		protected void AddAnalizeList(string analize, type _type, object tag)
		{
			m_analize_list.Add(new analize_data(analize, _type, tag));
		}

		/*-------------------------------------------------------------------------
		 분석정보を추가する
		 tagで判定すること
		 예금 이자용
		---------------------------------------------------------------------------*/
		protected void AddAnalizeList_Interest(object tag)
		{
			m_analize_list.Add(new analize_data("예금 이자로", type.index0, tag));
		}

		/*-------------------------------------------------------------------------
		 분석정보をクリアする
		---------------------------------------------------------------------------*/
		protected void ClearAnalizeList()
		{
			m_analize_list.Clear();
		}

		/*-------------------------------------------------------------------------
		 最も新しい로그파일명を得る
		---------------------------------------------------------------------------*/
		private FileInfo get_newest_log_file()
		{
			// 로그파일목록を得る
			// 最終쓰기시간でソートされる
			FileInfo[]	log_list	= GetLogFiles();

			if(log_list == null)		return null;	// 실패
			if(log_list.Length <= 0)	return null;	// 로그が1つもない

			// 最후の파일が最新로그
			return log_list[log_list.Length - 1];
		}
	
		/*-------------------------------------------------------------------------
		 로그파일명목록を得る
		---------------------------------------------------------------------------*/
		public FileInfo[] GetLogFiles()
		{
			return GetLogFiles(m_path);
		}
		public static FileInfo[] GetLogFiles(string path)
		{
			try{
				DirectoryInfo	info	= new DirectoryInfo(path);
				if(!info.Exists)		return null;		// 로그フォルダが存在しない

				FileInfo[]	file_info	= info.GetFiles("*.html");
				FileInfo[]	file_info2	= info.GetFiles("*.txt");

				// 連結
				int	size	= file_info.Length;
				Array.Resize<FileInfo>(ref file_info, size + file_info2.Length);
				Array.Copy(file_info2, 0, file_info, size, file_info2.Length);

				// 로그が1つもない場合はnullを返す
				if(file_info.Length <= 0)	return null;

				// 最終쓰기시간でソートする
				SortFileInfo_LastWriteTime(file_info);
				return file_info;
			}catch{
				return null;
			}
		}

		/*-------------------------------------------------------------------------
		 ソート
		 最終쓰기시간で比較する
		---------------------------------------------------------------------------*/
		static public void SortFileInfo_LastWriteTime(FileInfo[] list)
		{
			if(list == null)	return;

			// 最終쓰기시간でソートする
			Array.Sort<FileInfo>(list, new file_info_compare());
		}
	
		/*-------------------------------------------------------------------------
		 比較
		 最終쓰기시간で比較する
		---------------------------------------------------------------------------*/
		public class file_info_compare : IComparer<FileInfo>
		{
			public int Compare(FileInfo x, FileInfo y)
			{
				if(x.LastWriteTime < y.LastWriteTime)	return -1;
				if(x.LastWriteTime > y.LastWriteTime)	return 1;
				return 0;
			}
		}

		/*-------------------------------------------------------------------------
		 最新の로그분석
		---------------------------------------------------------------------------*/
		public virtual bool AnalyzeNewestChatLog()
		{
			// 분석목록をクリア
			ResetAnalizedList();

			// 更新をチェック
			if(!check_update_log())		return false;

			// 更新された部분を분석
			List<analized_data>	list	= new List<analized_data>();
			if(do_analize(m_chat_file_info, list, ref m_analyze_lines)){
				analized_list		= list;
				return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 지정された로그분석
		---------------------------------------------------------------------------*/
		public List<analized_data> AnalyzeChatLog(string fname)
		{
			try{
				FileInfo	info		= new FileInfo(fname);
				return AnalyzeChatLog(info);
			}catch{
				return null;
			}
		}
		public List<analized_data> AnalyzeChatLog(FileInfo info)
		{
			List<analized_data>	list	= new List<analized_data>();

			int	lines			= 0;
			if(!do_analize(info, list, ref lines)){
				// 실패
				list	= null;
			}
			return list;
		}

		/*-------------------------------------------------------------------------
		 最新の로그정보をリセットする
		---------------------------------------------------------------------------*/
		public void ResetNewestLogInfo()
		{
			m_chat_file_info	= null;
		}

		/*-------------------------------------------------------------------------
		 更新をチェックする
		---------------------------------------------------------------------------*/
		private bool check_update_log()
		{
			// 現在の현재로그がロックされているかどうかを調べる
			// ロックされているなら대항해시대Onlineが로그を握ってるとする
			// その場合新しい로그を검색しない
			if(!is_locked_log_file()){
				// ロックされていない
				// 最も新しい로그を得る
				m_newest_chat_file_info	= get_newest_log_file();
			}

			// 로그파일정보の取得に실패
			if(m_newest_chat_file_info == null)		return false;

			if(m_chat_file_info == null){
				// 初めての분석
				m_chat_file_info	= m_newest_chat_file_info;
				m_analyze_lines		= 0;

				Debug.WriteLine("1st " + m_chat_file_info.FullName);
			}else{
				// 更新されたか調べる
				if(m_newest_chat_file_info.FullName != m_chat_file_info.FullName){
					// 最新の로그が違う
					// 無条건で분석する
					m_chat_file_info	= m_newest_chat_file_info;
					m_analyze_lines		= 0;

					Debug.WriteLine("new log " + m_chat_file_info.FullName);
				}else{
	//				if(m_newest_chat_file_info.LastWriteTime <= m_chat_file_info.LastWriteTime){
					if(m_newest_chat_file_info.Length <= m_chat_file_info.Length){
						// 更新されていない
						Debug.WriteLine("skip log " + m_chat_file_info.FullName);
						return false;
					}
					Debug.WriteLine("update log " + m_chat_file_info.FullName);
					// 更新された
					m_chat_file_info	= m_newest_chat_file_info;
				}
			}
			return true;		// 更新された
		}

		/*-------------------------------------------------------------------------
		 現在の현재로그がロックされているかどうかを調べる
		 ロックされているなら대항해시대Onlineが로그を握ってるとする
		 その場合新しい로그を검색しない
		---------------------------------------------------------------------------*/
		private bool is_locked_log_file()
		{
			// 현재로그を未取得
			if(m_chat_file_info == null)	return false;
			// 현재로그が存在しない
			if(!m_chat_file_info.Exists)	return false;

			// 最新정보を新しく작성
			m_newest_chat_file_info		= new FileInfo(m_chat_file_info.FullName);
	
			try{
				// 정보更新
				m_newest_chat_file_info.Refresh();
			}catch{
				// 정보更新に실패
				return false;
			}
			try{
				// 開いてみる
				using(FileStream stream = new FileStream(m_newest_chat_file_info.FullName, FileMode.Open, FileAccess.Read, FileShare.None)){
					// 開けた場合は대항해시대Onlineが로그をロックしていない
				}
			}catch{
				// 대항해시대Onlineによってロックされていると判断する
				Debug.WriteLine("locked log file.[" + m_newest_chat_file_info.FullName + " ]");
				return true;
			}
			// ロックされていないので最新로그を取得する必要あり
			m_newest_chat_file_info	= null;
			Debug.WriteLine("unlocked log file.");
			return false;
		}

		/*-------------------------------------------------------------------------
		 분석する
		 분석결과はlist내용に추가される
		---------------------------------------------------------------------------*/
		private bool do_analize(FileInfo file_info, List<analized_data> list, ref int analyze_lines)
		{
			// 스레드時にnullになる가능性がある
			// pathのset時
			if(file_info == null)	return false;

			try{
				using(FileStream stream = new FileStream(file_info.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)){
					string	line		= "";
					int		line_count	= 0;

					// TXTとHTMLに대응
					string		ex		= Path.GetExtension(file_info.FullName);
					bool		is_text;
					Encoding	encoder	= null;
					if(ex == ".txt"){
						is_text	= true;
						encoder	= Encoding.GetEncoding("UTF-8");
					}else{
						is_text	= false;
						encoder	= Encoding.UTF8;
					}

					try{
						using(StreamReader	sr	= new StreamReader(stream, encoder)){
							while((line = sr.ReadLine()) != null){
								if(is_text){
									if(line_count++ < analyze_lines)	continue;	// 분석済みの行
								}else{
									if(line.Length < 22)				continue;	// 로그ではないと判断する
									if(line_count++ < analyze_lines)	continue;	// 분석済みの行
									line	= line.Substring(22);					// 
								}
								// この行を분석する
								AnalyzeLine(line, list);
							}
							// 분석した장소を覚えておく
							analyze_lines	= line_count;
						}
					}catch{
						// 실패
						return false;
					}
				}						
			}catch{
				// 실패
				Debug.WriteLine("can't open " + file_info.FullName);
				return false;
			}
			return true;
		}
	
		/*-------------------------------------------------------------------------
		 분석
		---------------------------------------------------------------------------*/
		protected virtual void AnalyzeLine(string line, List<analized_data> list)
		{
			// 등록されているルールでの분석
			foreach(analize_data d in m_analize_list){
				if(d.Analize(line, list)){
					// 대상が見つかった
					return;
				}
			}
		}

		/*-------------------------------------------------------------------------
		 분석내용をリセットする
		---------------------------------------------------------------------------*/
		public void ResetAnalizedList()
		{
			analized_list			= null;
		}
	}
}
