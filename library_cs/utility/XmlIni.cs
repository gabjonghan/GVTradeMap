//-------------------------------------------------------------------------
// Xml 을 사용한 ini
// ini 와 같은 방식으로 사용할 수 있다
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Xml;
using Utility.Ini;

//-------------------------------------------------------------------------
namespace Utility.Xml
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// XmlIni 를 사용한 설정 관리.
	/// XmlIni 의 인스턴스화를 자동으로 처리하므로,
	/// IniBase 의 인터페이스로 접근하는 부분만 구현하면 설정 읽기가 가능하다.
	/// </summary>
	public class XmlIniSetting : IniSettingBase
	{
		private string				m_file_name;
		private string				m_id;

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축
		/// </summary>
		/// <param name="file_name">읽어들일 파일명(전체 경로여야 한다)</param>
		/// <param name="id">
		/// <para>설정 파일이 올바른지 확인하는 ID</para>
		/// <para>애플리케이션마다 유니크한 값으로 할 것</para>
		/// </param>
		public XmlIniSetting(string file_name, string id)
			: base()
		{
			if(String.IsNullOrEmpty(file_name))	throw new ArgumentException();
			if(String.IsNullOrEmpty(id))		throw new ArgumentException();

			m_file_name		= file_name;
			m_id			= id;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// XmlIni 로부터 로드
		/// </summary>
		public override void Load()
		{
			XmlIni		ini			= new XmlIni(m_file_name, m_id);
			// 읽기
			Load(ini);
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// XmlIni 에 기록한다.
		/// </summary>
		public override void Save()
		{
			XmlIni		ini		= new XmlIni(m_id);
			Save(ini);
			ini.Save(m_file_name);
		}
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// Xml 을 사용한 ini.
	/// ini 와 같은 방식으로 사용할 수 있다.
	/// IniBase 의 인터페이스로 접근할 수 있다.
	/// </summary>
	public class XmlIni : IniBase
	{
		/// <summary>
		/// 루트명
		/// </summary>
		protected const string				ROOT_NAME		= "XmlIniRoot";
		/// <summary>
		/// ID타입명
		/// ID 를 담는 어트리뷰트명으로 사용된다
		/// </summary>
		protected const string				XMLINI_TYPE		= "XmlIniType";

		/// <summary>
		/// Xml 도큐먼트
		/// </summary>
		protected XmlDocument				m_document;
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 루트를 얻는다
		/// </summary>
		protected XmlElement	root{		get{	return m_document.DocumentElement;	}}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 새 Document 를 작성
		/// </summary>
		/// <param name="id">
		/// <para>설정 파일이 올바른지 확인하는 ID</para>
		/// <para>애플리케이션마다 유니크한 값으로 할 것</para>
		/// </param>
		public XmlIni(string id)
		{
			create_new_document(id);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 읽으면서 작성
		/// 읽기에 실패한 경우에는 새 Document 를 작성한다
		/// </summary>
		/// <param name="file_name">읽기파일명</param>
		/// <param name="id">
		/// <para>설정 파일이 올바른지 확인하는 ID</para>
		/// <para>애플리케이션마다 유니크한 값으로 할 것</para>
		/// </param>
		public XmlIni(string file_name, string id)
		{
			Load(file_name, id);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 읽기
		/// </summary>
		/// <param name="file_name">읽기파일명</param>
		/// <param name="id">
		/// <para>설정 파일이 올바른지 확인하는 ID</para>
		/// <para>애플리케이션마다 유니크한 값으로 할 것</para>
		/// </param>
		/// <returns>읽기에 성공한 경우 true 를 반환한다</returns>
		public bool Load(string file_name, string id)
		{
			try{
				m_document				= new XmlDocument();
				m_document.Load(file_name);

				// ID 식별
				if(!is_match_attribute(id)){
					// 식별 실패
					create_new_document(id);
					return false;
				}
				// 읽기 성공
				return true;
			}catch{
				// 읽기실패
				create_new_document(id);
				return false;
			}
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 기록
		/// </summary>
		/// <param name="file_name">기록할 파일명</param>
		public virtual void Save(string file_name)
		{
			m_document.Save(file_name);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 루트의 어트리뷰트를 보고 ID 가 일치하는지 조사한다
		/// </summary>
		/// <param name="id"></param>
		/// <returns>일치한 경우 true</returns>
		private bool is_match_attribute(string id)
		{
			XmlAttribute	attri	= this.root.Attributes[XMLINI_TYPE];
			if(attri == null)		return false;	// 어트리뷰트가 없다
			if(attri.Value != id)	return false;	// 일치하지 않는다

			// 일치했다
			return true;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 새 Document 를 작성
		/// ROOT 를 작성한다
		/// </summary>
		/// <param name="id"></param>
		protected void create_new_document(string id)
		{
			m_document		= new XmlDocument();
			m_document.AppendChild(
				m_document.CreateXmlDeclaration("1.0", "UTF-8", null));

			// 루트를 작성
			m_document.AppendChild(create_element(ROOT_NAME));

			// 이 XmlIniBase 를 식별하는 ID
			XmlAttribute	attri	= m_document.CreateAttribute(XMLINI_TYPE);
			attri.Value				= id;
			this.root.Attributes.Append(attri);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 데이터 취득
		/// 데이터를 얻지 못한 경우에는 default_value 를 반환한다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터를 얻지 못한 경우의 초기값</param>
		/// <returns>얻어진 데이터</returns>
		public override string GetProfile(string group_name, string name, string default_value)
		{
			XmlNode		node	= get_element(group_name, name);
			if(node == null)	return default_value;		// 요소가 없으므로 초기값을 반환한다

			string		text	= get_first_text(node);		// 최초의 XmlText 내용을 얻는다
			if(text == null)	return default_value;		// XmlText 가 없으므로 초기값을 반환한다
			return text;									// 얻어진 데이터
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 데이터 취득
		/// 데이터를 얻지 못한 경우에는 default_value 를 반환한다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터를 얻지 못한 경우의 초기값</param>
		/// <returns>
		/// 얻어진 데이터
		/// 크기 0 인 배열일 가능성 있음
		/// </returns>
		public override string[] GetProfile(string group_name, string name, string[] default_value)
		{
			XmlNode		node	= get_element(group_name, name);
			if(node == null)	return default_value;		// 요소가 없으므로 초기값을 반환한다

			List<string>	list	= new List<string>();
			foreach(XmlNode n in node.ChildNodes){
				string		text	= get_first_text(n);	// 최초의 XmlText 내용을 얻는다
				if(text != null)	list.Add(text);
			}
			return list.ToArray();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 노드의 최초 텍스트를 얻는다.
		/// 자식 노드 중 최초 XmlText 의 내용을 반환한다.
		/// 자식 노드에 XmlText 가 없는 경우에는 null 을 반환한다.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private string get_first_text(XmlNode node)
		{
			if(node == null)			return null;
			if(!node.HasChildNodes)		return null;	// 노드가 자식 노드를 갖고 있지 않다

			foreach(XmlNode i in node.ChildNodes){
				if(i is XmlText){
					return i.Value;						// "" 인 경우 있음
				}
			}
			return null;								// 자식 노드가 XmlText 를 갖고 있지 않다
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 데이터가 있는지 여부를 얻는다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <returns>데이터가 있는 경우 true</returns>
		protected override bool HasProfile(string group_name, string name)
		{
			return (get_element(group_name, name) != null)? true: false;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 데이터설정
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
		public override void SetProfile(string group_name, string name, string value)
		{
			update_element(group_name, name, value);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 데이터설정
		/// 배열의 내용은 전부 대체된다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
		public override void SetProfile(string group_name, string name, string[] value)
		{
			update_element(group_name, name, value);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// group 을 얻는다
		/// group 이 없으면 작성한다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <returns>그룹</returns>
		protected XmlNode get_group(string group_name)
		{
			XmlNode		group_node	= this.root[group_name];

			if(group_node == null){
				// 없으면 만든다
				group_node	= create_element(group_name);
				this.root.AppendChild(group_node);
			}
			return group_node;
		}	

		//-------------------------------------------------------------------------
		/// <summary>
		/// 설정 정보를 얻는다
		/// group 이름을 넘긴다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <returns>데이터 노드</returns>
		protected XmlNode get_element(string group_name, string name)
		{
			XmlNode		group_node	= get_group(group_name);
			if(group_node == null)		return null;
			return group_node[name];
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns>작성한 Element</returns>
		private XmlElement create_element(string name)
		{
			try{
				return m_document.CreateElement(name);
			}catch{
				throw new Exception(String.Format("Element 작성에 실패하였습니다. \n[ {0} ] 에 사용할 수 없는 문자가 포함되어 있을 가능성이 있습니다. ", name));
			}
		}
		
		//-------------------------------------------------------------------------
		/// <summary>
		/// 설정을 갱신
		/// 설정 항목이 없는 경우 Element 가 만들어진다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
		protected void update_element(string group_name, string name, string value)
		{
			XmlNode		node		= get_edit_node(group_name, name);
			node.AppendChild(m_document.CreateTextNode(value));
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 설정을 갱신
		/// 설정 항목이 없는 경우 Element 가 만들어진다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
		protected void update_element(string group_name, string name, string[] value)
		{
			XmlNode		node		= get_edit_node(group_name, name);

			// 요소를 갱신
			foreach(string s in value){
				XmlElement	array_node	= create_element("array");
				array_node.AppendChild(m_document.CreateTextNode(s));
				node.AppendChild(array_node);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 편집해야 할 노드를 얻는다.
		/// 정보 갱신용. 노드가 없으면 작성해서 반환한다.
		/// 노드가 이미 존재하면 자식을 전부 삭제하고 반환한다.
		/// </summary>
		/// <param name="group_name"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private XmlNode get_edit_node(string group_name, string name)
		{
			XmlNode		group_node	= get_group(group_name);
			XmlNode		node		= group_node[name];

			// 요소가 있는지 확인
			if(node != null){
				// 자식을 전부 삭제
				node.RemoveAll();
			}else{
				// 새 요소로 추가
				node	= create_element(name);
				group_node.AppendChild(node);
			}

			// 편집해야 할 Node
			return node;
		}
	}
}
