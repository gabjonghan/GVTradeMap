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
	/// XmlIniを사용した설정管理. 
	/// XmlIniのインスタンス化を自動で行うため, 
	/// IniBaseのインターフェイスでのアクセスを実装するだけで설정の読み書きが行える. 
	/// </summary>
	public class XmlIniSetting : IniSettingBase
	{
		private string				m_file_name;
		private string				m_id;

		//-------------------------------------------------------------------------
		/// <summary>
		/// 構築
		/// </summary>
		/// <param name="file_name">読み込みファイル명(フルパスであること)</param>
		/// <param name="id">
		/// <para>설정ファイルが正しいことを認識するID</para>
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
			// 読み込み
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
		/// IDタイプ명
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
		/// 新しいDocument	を作成
		/// </summary>
		/// <param name="id">
		/// <para>설정ファイルが正しいことを認識するID</para>
		/// <para>アプリケーション毎にユニークなものにすること</para>
		/// </param>
		public XmlIni(string id)
		{
			create_new_document(id);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 読み込みつつ作成
		/// 読み込みに실패した場合は新しいDocumentを作成する
		/// </summary>
		/// <param name="file_name">読み込みファイル명</param>
		/// <param name="id">
		/// <para>설정ファイルが正しいことを認識するID</para>
		/// <para>アプリケーション毎にユニークなものにすること</para>
		/// </param>
		public XmlIni(string file_name, string id)
		{
			Load(file_name, id);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 読み込み
		/// </summary>
		/// <param name="file_name">読み込みファイル명</param>
		/// <param name="id">
		/// <para>설정ファイルが正しいことを認識するID</para>
		/// <para>アプリケーション毎にユニークなものにすること</para>
		/// </param>
		/// <returns>読み込みに成功した場合trueを返す</returns>
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
				// 読み込み成功
				return true;
			}catch{
				// 読み込み실패
				create_new_document(id);
				return false;
			}
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 書き出し
		/// </summary>
		/// <param name="file_name">書き出し先ファイル명</param>
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
		/// 新しいDocument	を作成
		/// ROOTを作成する
		/// </summary>
		/// <param name="id"></param>
		protected void create_new_document(string id)
		{
			m_document		= new XmlDocument();
			m_document.AppendChild(
				m_document.CreateXmlDeclaration("1.0", "UTF-8", null));

			// ルートを作成
			m_document.AppendChild(create_element(ROOT_NAME));

			// このXmlIniBaseを識別するID
			XmlAttribute	attri	= m_document.CreateAttribute(XMLINI_TYPE);
			attri.Value				= id;
			this.root.Attributes.Append(attri);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// データ取得
		/// データが得られない場合はdefault_valueを返す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="default_value">データが得られない場合の初期値</param>
		/// <returns>得られたデータ</returns>
		public override string GetProfile(string group_name, string name, string default_value)
		{
			XmlNode		node	= get_element(group_name, name);
			if(node == null)	return default_value;		// 要素がないので初期値を返す

			string		text	= get_first_text(node);		// 最初のXmlTextの내용を得る
			if(text == null)	return default_value;		// XmlTextがないので初期値を返す
			return text;									// 得られたデータ
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// データ取得
		/// データが得られない場合はdefault_valueを返す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="default_value">データが得られない場合の初期値</param>
		/// <returns>
		/// 得られたデータ
		/// 대きさ0の配列の可能性有り
		/// </returns>
		public override string[] GetProfile(string group_name, string name, string[] default_value)
		{
			XmlNode		node	= get_element(group_name, name);
			if(node == null)	return default_value;		// 要素がないので初期値を返す

			List<string>	list	= new List<string>();
			foreach(XmlNode n in node.ChildNodes){
				string		text	= get_first_text(n);	// 最初のXmlTextの내용を得る
				if(text != null)	list.Add(text);
			}
			return list.ToArray();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// ノードの最初のテキストを得る. 
		/// 子ノードのうち, 最初のXmlTextの내용を返す. 
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
		/// データがあるかどうかを得る
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <returns>データがある場合true</returns>
		protected override bool HasProfile(string group_name, string name)
		{
			return (get_element(group_name, name) != null)? true: false;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// データ설정
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
		public override void SetProfile(string group_name, string name, string value)
		{
			update_element(group_name, name, value);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// データ설정
		/// 配列の내용は전부置きかえられる
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
		public override void SetProfile(string group_name, string name, string[] value)
		{
			update_element(group_name, name, value);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// groupを得る
		/// groupが無ければ作成する
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
		/// <param name="name">データ명</param>
		/// <returns>データノード</returns>
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
		/// <returns>作成したElement</returns>
		private XmlElement create_element(string name)
		{
			try{
				return m_document.CreateElement(name);
			}catch{
				throw new Exception(String.Format("Elementの作成に실패하였습니다. \n[ {0} ] に사용できない文字が含まれている可能性があります. ", name));
			}
		}
		
		//-------------------------------------------------------------------------
		/// <summary>
		/// 설정を更新
		/// 설정항목がない場合Elementが作られる
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
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
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
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
		/// 정보更新用. ノードが無ければ作成して返す. 
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
				// 新しい要素として追加
				node	= create_element(name);
				group_node.AppendChild(node);
			}

			// 編集するべきNode
			return node;
		}
	}
}
