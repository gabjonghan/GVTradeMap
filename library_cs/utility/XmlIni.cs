//-------------------------------------------------------------------------
// Xmlを使ったini
// iniと同じように사용できる
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
	/// XmlIniを사용した설정관리. 
	/// XmlIniのインスタンス化を自動で行うため, 
	/// IniBaseのインターフェイスでのアクセスを実装するだけで설정の읽기が行える. 
	/// </summary>
	public class XmlIniSetting : IniSettingBase
	{
		private string				m_file_name;
		private string				m_id;

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축
		/// </summary>
		/// <param name="file_name">읽기파일명(フルパスであること)</param>
		/// <param name="id">
		/// <para>설정파일が正しいことを認識するID</para>
		/// <para>アプリケーション毎にユニークなものにすること</para>
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
		/// XmlIniから로드
		/// </summary>
		public override void Load()
		{
			XmlIni		ini			= new XmlIni(m_file_name, m_id);
			// 읽기
			Load(ini);
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// XmlIniに書きだす. 
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
	/// Xmlを使ったini. 
	/// iniと同じように사용できる. 
	/// IniBaseのインターフェイスでアクセスできる. 
	/// </summary>
	public class XmlIni : IniBase
	{
		/// <summary>
		/// ルート명
		/// </summary>
		protected const string				ROOT_NAME		= "XmlIniRoot";
		/// <summary>
		/// ID타입명
		/// IDを格納するアトリビュート명として사용される
		/// </summary>
		protected const string				XMLINI_TYPE		= "XmlIniType";

		/// <summary>
		/// Xmlドキュメント
		/// </summary>
		protected XmlDocument				m_document;
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// ルートを得る
		/// </summary>
		protected XmlElement	root{		get{	return m_document.DocumentElement;	}}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 新しいDocument	を작성
		/// </summary>
		/// <param name="id">
		/// <para>설정파일が正しいことを認識するID</para>
		/// <para>アプリケーション毎にユニークなものにすること</para>
		/// </param>
		public XmlIni(string id)
		{
			create_new_document(id);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 읽기つつ작성
		/// 읽기に실패した場合は新しいDocumentを작성함
		/// </summary>
		/// <param name="file_name">읽기파일명</param>
		/// <param name="id">
		/// <para>설정파일が正しいことを認識するID</para>
		/// <para>アプリケーション毎にユニークなものにすること</para>
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
		/// <para>설정파일が正しいことを認識するID</para>
		/// <para>アプリケーション毎にユニークなものにすること</para>
		/// </param>
		/// <returns>읽기に成功した場合trueを返す</returns>
		public bool Load(string file_name, string id)
		{
			try{
				m_document				= new XmlDocument();
				m_document.Load(file_name);

				// IDの識別
				if(!is_match_attribute(id)){
					// 識別に실패
					create_new_document(id);
					return false;
				}
				// 읽기成功
				return true;
			}catch{
				// 읽기실패
				create_new_document(id);
				return false;
			}
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 書き出し
		/// </summary>
		/// <param name="file_name">書き出し先파일명</param>
		public virtual void Save(string file_name)
		{
			m_document.Save(file_name);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// ルートのアトリビュートを見てIDが一致するかどうか調べる
		/// </summary>
		/// <param name="id"></param>
		/// <returns>一致した場合true</returns>
		private bool is_match_attribute(string id)
		{
			XmlAttribute	attri	= this.root.Attributes[XMLINI_TYPE];
			if(attri == null)		return false;	// アトリビュートがない
			if(attri.Value != id)	return false;	// 一致しない

			// 一致した
			return true;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 新しいDocument	を작성
		/// ROOTを작성함
		/// </summary>
		/// <param name="id"></param>
		protected void create_new_document(string id)
		{
			m_document		= new XmlDocument();
			m_document.AppendChild(
				m_document.CreateXmlDeclaration("1.0", "UTF-8", null));

			// ルートを작성
			m_document.AppendChild(create_element(ROOT_NAME));

			// このXmlIniBaseを識別するID
			XmlAttribute	attri	= m_document.CreateAttribute(XMLINI_TYPE);
			attri.Value				= id;
			this.root.Attributes.Append(attri);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 데이터取得
		/// 데이터が得られない場合はdefault_valueを返す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터が得られない場合の初期値</param>
		/// <returns>得られた데이터</returns>
		public override string GetProfile(string group_name, string name, string default_value)
		{
			XmlNode		node	= get_element(group_name, name);
			if(node == null)	return default_value;		// 要素がないので初期値を返す

			string		text	= get_first_text(node);		// 최초のXmlTextの내용を得る
			if(text == null)	return default_value;		// XmlTextがないので初期値を返す
			return text;									// 得られた데이터
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 데이터取得
		/// 데이터が得られない場合はdefault_valueを返す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터が得られない場合の初期値</param>
		/// <returns>
		/// 得られた데이터
		/// 대きさ0の配열の가능性有り
		/// </returns>
		public override string[] GetProfile(string group_name, string name, string[] default_value)
		{
			XmlNode		node	= get_element(group_name, name);
			if(node == null)	return default_value;		// 要素がないので初期値を返す

			List<string>	list	= new List<string>();
			foreach(XmlNode n in node.ChildNodes){
				string		text	= get_first_text(n);	// 최초のXmlTextの내용を得る
				if(text != null)	list.Add(text);
			}
			return list.ToArray();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// ノードの최초のテキストを得る. 
		/// 子ノードのうち, 최초のXmlTextの내용を返す. 
		/// 子ノードにXmlTextが含まれない場合はnullを返す. 
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private string get_first_text(XmlNode node)
		{
			if(node == null)			return null;
			if(!node.HasChildNodes)		return null;	// ノードが子ノードを持っていない

			foreach(XmlNode i in node.ChildNodes){
				if(i is XmlText){
					return i.Value;						// ""の場合あり
				}
			}
			return null;								// 子ノードがXmlTextを持っていない
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 데이터があるかどうかを得る
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <returns>데이터がある場合true</returns>
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
		/// 配열の내용は전부置きかえられる
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
		/// groupを得る
		/// groupが無ければ작성함
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <returns>그룹</returns>
		protected XmlNode get_group(string group_name)
		{
			XmlNode		group_node	= this.root[group_name];

			if(group_node == null){
				// 無ければ作る
				group_node	= create_element(group_name);
				this.root.AppendChild(group_node);
			}
			return group_node;
		}	

		//-------------------------------------------------------------------------
		/// <summary>
		/// 설정정보を得る
		/// group이름を渡す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <returns>데이터ノード</returns>
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
		/// <returns>작성したElement</returns>
		private XmlElement create_element(string name)
		{
			try{
				return m_document.CreateElement(name);
			}catch{
				throw new Exception(String.Format("Elementの작성に실패하였습니다. \n[ {0} ] に사용できない문자が含まれている가능性があります. ", name));
			}
		}
		
		//-------------------------------------------------------------------------
		/// <summary>
		/// 설정を更新
		/// 설정항목がない場合Elementが作られる
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
		/// 설정を更新
		/// 설정항목がない場合Elementが作られる
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
		protected void update_element(string group_name, string name, string[] value)
		{
			XmlNode		node		= get_edit_node(group_name, name);

			// 要素を更新
			foreach(string s in value){
				XmlElement	array_node	= create_element("array");
				array_node.AppendChild(m_document.CreateTextNode(s));
				node.AppendChild(array_node);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 編集するべきノードを得る. 
		/// 정보更新용. ノードが無ければ작성して返す. 
		/// ノードが既に存在していれば子供を全てを삭제して返す. 
		/// </summary>
		/// <param name="group_name"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private XmlNode get_edit_node(string group_name, string name)
		{
			XmlNode		group_node	= get_group(group_name);
			XmlNode		node		= group_node[name];

			// 要素があるかチェック
			if(node != null){
				// 子供を全部삭제
				node.RemoveAll();
			}else{
				// 新しい要素として추가
				node	= create_element(name);
				group_node.AppendChild(node);
			}

			// 編集するべきNode
			return node;
		}
	}
}
