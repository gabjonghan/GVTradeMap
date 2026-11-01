/*-------------------------------------------------------------------------

 채팅 분석의 베이스
 리퀘스트 포함
 지정한 규칙으로 분석이 가능
 분석은 한 줄 단위만
 여러 줄 분석은 실제 게임에서 실시간으로 로그가 기록되기 때문에,
 아직 필요한 로그가 기록되지 않았을 가능성이 있다

 스레드 대응 버전

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
			index0,		// 첫 문자부터 일치하는 것
			any_index,	// 어딘가에 포함되기만 하면 되는 것
			regex,		// 정규 표현식
		};
	
		/*-------------------------------------------------------------------------
		 분석대상
		---------------------------------------------------------------------------*/
		public class analize_data
		{
			private string				m_analize;
			private type				m_type;
			private Regex				m_regex;		// 정규 표현식일 때의 분석 대상
			private object				m_tag;			// 분석 결과 참조용 태그

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

				// 정규 표현식일 때는 컴파일해 둔다
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
					// index 0 부터 발견된다
					if(line.IndexOf(m_analize) == 0){
						list.Add(new analized_data(this, line));
						return true;
					}
					break;
				case type.any_index:
					// 어딘가에 포함된다
					if(line.IndexOf(m_analize) >= 0){
						list.Add(new analized_data(this, line));
						return true;
					}
					break;
				case type.regex:
					// 정규 표현식
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
			private Match					m_match;	// 정규 표현식일 때

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
	
		private string						m_path;							// 로그 경로
		private FileInfo					m_newest_chat_file_info;		// 최신 로그 파일 정보
		private FileInfo					m_chat_file_info;				// 분석 대상 로그 파일 정보
		private int							m_analyze_lines;				// 분석한 줄 수

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
													// 새 분석 정보는 교체하기 위한 것이므로,
													// 참조만 얻을 수 있으면 된다
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

			m_newest_chat_file_info		= null;		// 갱신이 가장 최근인 로그
			m_chat_file_info			= null;		// 분석 대상 로그
			m_analyze_lines				= 0;		// 분석현황

			m_analize_list				= new List<analize_data>();
			m_analized_list				= null;
		}
	
		/*-------------------------------------------------------------------------
		 분석 정보를 추가한다
		 tag 로 판정할 것
		---------------------------------------------------------------------------*/
		protected void AddAnalizeList(string analize, type _type, object tag)
		{
			m_analize_list.Add(new analize_data(analize, _type, tag));
		}

		/*-------------------------------------------------------------------------
		 분석 정보를 추가한다
		 tag 로 판정할 것
		 예금 이자용
		---------------------------------------------------------------------------*/
		protected void AddAnalizeList_Interest(object tag)
		{
			m_analize_list.Add(new analize_data("예금 이자를", type.index0, tag));
		}

		/*-------------------------------------------------------------------------
		 분석 정보를 초기화한다
		---------------------------------------------------------------------------*/
		protected void ClearAnalizeList()
		{
			m_analize_list.Clear();
		}

		/*-------------------------------------------------------------------------
		 가장 새로운 로그 파일명을 얻는다
		---------------------------------------------------------------------------*/
		private FileInfo get_newest_log_file()
		{
			// 로그 파일 목록을 얻는다
			// 마지막 기록 시간으로 정렬된다
			FileInfo[]	log_list	= GetLogFiles();

			if(log_list == null)		return null;	// 실패
			if(log_list.Length <= 0)	return null;	// 로그가 하나도 없다

			// 마지막 파일이 최신 로그
			return log_list[log_list.Length - 1];
		}
	
		/*-------------------------------------------------------------------------
		 로그 파일명 목록을 얻는다
		---------------------------------------------------------------------------*/
		public FileInfo[] GetLogFiles()
		{
			return GetLogFiles(m_path);
		}
		public static FileInfo[] GetLogFiles(string path)
		{
			try{
				DirectoryInfo	info	= new DirectoryInfo(path);
				if(!info.Exists)		return null;		// 로그 폴더가 존재하지 않는다

				FileInfo[]	file_info	= info.GetFiles("*.html");
				FileInfo[]	file_info2	= info.GetFiles("*.txt");

				// 연결
				int	size	= file_info.Length;
				Array.Resize<FileInfo>(ref file_info, size + file_info2.Length);
				Array.Copy(file_info2, 0, file_info, size, file_info2.Length);

				// 로그가 하나도 없는 경우에는 null 을 반환한다
				if(file_info.Length <= 0)	return null;

				// 마지막 기록 시간으로 정렬한다
				SortFileInfo_LastWriteTime(file_info);
				return file_info;
			}catch{
				return null;
			}
		}

		/*-------------------------------------------------------------------------
		 정렬
		 마지막 기록 시간으로 비교한다
		---------------------------------------------------------------------------*/
		static public void SortFileInfo_LastWriteTime(FileInfo[] list)
		{
			if(list == null)	return;

			// 마지막 기록 시간으로 정렬한다
			Array.Sort<FileInfo>(list, new file_info_compare());
		}
	
		/*-------------------------------------------------------------------------
		 비교
		 마지막 기록 시간으로 비교한다
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
		 최신 로그 분석
		---------------------------------------------------------------------------*/
		public virtual bool AnalyzeNewestChatLog()
		{
			// 분석 목록 초기화
			ResetAnalizedList();

			// 갱신 확인
			if(!check_update_log())		return false;

			// 갱신된 부분을 분석
			List<analized_data>	list	= new List<analized_data>();
			if(do_analize(m_chat_file_info, list, ref m_analyze_lines)){
				analized_list		= list;
				return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 지정된 로그 분석
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
		 최신 로그 정보를 리셋한다
		---------------------------------------------------------------------------*/
		public void ResetNewestLogInfo()
		{
			m_chat_file_info	= null;
		}

		/*-------------------------------------------------------------------------
		 갱신을 확인한다
		---------------------------------------------------------------------------*/
		private bool check_update_log()
		{
			// 현재 로그가 잠겨 있는지 조사한다
			// 잠겨 있다면 대항해시대Online 이 로그를 쥐고 있는 것으로 본다
			// 그 경우 새 로그를 검색하지 않는다
			if(!is_locked_log_file()){
				// 잠겨 있지 않다
				// 가장 새로운 로그를 얻는다
				m_newest_chat_file_info	= get_newest_log_file();
			}

			// 로그 파일 정보 취득에 실패
			if(m_newest_chat_file_info == null)		return false;

			if(m_chat_file_info == null){
				// 첫 분석
				m_chat_file_info	= m_newest_chat_file_info;
				m_analyze_lines		= 0;

				Debug.WriteLine("1st " + m_chat_file_info.FullName);
			}else{
				// 갱신되었는지 조사한다
				if(m_newest_chat_file_info.FullName != m_chat_file_info.FullName){
					// 최신 로그가 다르다
					// 무조건 분석한다
					m_chat_file_info	= m_newest_chat_file_info;
					m_analyze_lines		= 0;

					Debug.WriteLine("new log " + m_chat_file_info.FullName);
				}else{
	//				if(m_newest_chat_file_info.LastWriteTime <= m_chat_file_info.LastWriteTime){
					if(m_newest_chat_file_info.Length <= m_chat_file_info.Length){
						// 갱신되지 않았다
						Debug.WriteLine("skip log " + m_chat_file_info.FullName);
						return false;
					}
					Debug.WriteLine("update log " + m_chat_file_info.FullName);
					// 갱신되었다
					m_chat_file_info	= m_newest_chat_file_info;
				}
			}
			return true;		// 갱신되었다
		}

		/*-------------------------------------------------------------------------
		 현재 로그가 잠겨 있는지 조사한다
		 잠겨 있다면 대항해시대Online 이 로그를 쥐고 있는 것으로 본다
		 그 경우 새 로그를 검색하지 않는다
		---------------------------------------------------------------------------*/
		private bool is_locked_log_file()
		{
			// 현재 로그를 아직 취득하지 않음
			if(m_chat_file_info == null)	return false;
			// 현재 로그가 존재하지 않는다
			if(!m_chat_file_info.Exists)	return false;

			// 최신 정보를 새로 작성
			m_newest_chat_file_info		= new FileInfo(m_chat_file_info.FullName);
	
			try{
				// 정보 갱신
				m_newest_chat_file_info.Refresh();
			}catch{
				// 정보 갱신에 실패
				return false;
			}
			try{
				// 열어 본다
				using(FileStream stream = new FileStream(m_newest_chat_file_info.FullName, FileMode.Open, FileAccess.Read, FileShare.None)){
					// 열렸다면 대항해시대Online 이 로그를 잠그고 있지 않다
				}
			}catch{
				// 대항해시대Online 에 의해 잠겨 있다고 판단한다
				Debug.WriteLine("locked log file.[" + m_newest_chat_file_info.FullName + " ]");
				return true;
			}
			// 잠겨 있지 않으므로 최신 로그를 취득할 필요 있음
			m_newest_chat_file_info	= null;
			Debug.WriteLine("unlocked log file.");
			return false;
		}

		/*-------------------------------------------------------------------------
		 분석한다
		 분석 결과는 list 내용에 추가된다
		---------------------------------------------------------------------------*/
		private bool do_analize(FileInfo file_info, List<analized_data> list, ref int analyze_lines)
		{
			// 스레드 사용 시 null 이 될 가능성이 있다
			// path 의 set 시점
			if(file_info == null)	return false;

			try{
				using(FileStream stream = new FileStream(file_info.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)){
					string	line		= "";
					int		line_count	= 0;

					// TXT 와 HTML 에 대응
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
									if(line_count++ < analyze_lines)	continue;	// 분석이 끝난 줄
								}else{
									if(line.Length < 22)				continue;	// 로그가 아니라고 판단한다
									if(line_count++ < analyze_lines)	continue;	// 분석이 끝난 줄
									line	= line.Substring(22);					// 
								}
								// 이 줄을 분석한다
								AnalyzeLine(line, list);
							}
							// 분석한 위치를 기억해 둔다
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
			// 등록된 규칙으로 분석
			foreach(analize_data d in m_analize_list){
				if(d.Analize(line, list)){
					// 대상을 찾았다
					return;
				}
			}
		}

		/*-------------------------------------------------------------------------
		 분석 내용을 리셋한다
		---------------------------------------------------------------------------*/
		public void ResetAnalizedList()
		{
			analized_list			= null;
		}
	}
}
