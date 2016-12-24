//-------------------------------------------------------------------------
// 키アサイン관리
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Utility.Ini;
using System.Collections;

//-------------------------------------------------------------------------
namespace Utility.KeyAssign
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// ショートカット実行용デリゲート
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="arg"></param>
	public delegate void OnProcessCmdKey(object sender, KeyAssignEventArg arg);

	//-------------------------------------------------------------------------
	/// <summary>
	/// ショートカット実行時の引수
	/// </summary>
	public sealed class KeyAssignEventArg : EventArgs
	{
		private object							m_tag;

		/// <summary>
		/// ショートカットに関連付けられたobject, 
		/// 통상Enumを사용する
		/// </summary>
		public object Tag						{	get{	return m_tag;		}}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축
		/// </summary>
		/// <param name="tag">tag</param>
		public KeyAssignEventArg(object tag)
		{
			m_tag		= tag;
		}
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// アサインルール, 
	/// System.Windows.Forms.Shortcutのみを할당가능にする版
	/// </summary>
	public class KeyAssignRuleOnlyShortcut : KeyAssignRule
	{
		//-------------------------------------------------------------------------
		/// <summary>
		/// System.Windows.Forms.Shortcutの場合のみtrueを返す
		/// </summary>
		/// <param name="key">Keys</param>
		/// <returns>アサイン가능ならtrue</returns>
		public override bool CanAssignKeys(Keys key)
		{
			// System.Windows.Forms.Shortcutかどうかチェック
			if(!KeyAssignRule.IsShortcut(key))	return false;

			// base
			return base.CanAssignKeys(key);
		}
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// アサインルール, 
	/// 할당가능かどうかと키の문자열への변환을실시. 
	/// 継承してルールを변경することができる. 
	/// 動的にルールを변경すると유저を混乱させる原因となるので注意. 
	/// </summary>
	public class KeyAssignRule
	{
		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축
		/// </summary>
		public KeyAssignRule()
		{
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// アサイン가능かどうかを得る. 
		/// </summary>
		/// <param name="e">KeyEventArgs</param>
		/// <returns>アサイン가능ならtrue</returns>
		public bool CanAssignKeys(KeyEventArgs e)
		{
			return CanAssignKeys(e.KeyData);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// アサイン가능かどうかを得る. 
		/// ESC, TAB, 英字単体등が가능. 
		/// このメソッドをオーバーライドしてアサイン가능な키の組み合わせを
		/// カスタマイズできる. 
		/// このメソッドは最低限할당られないもののみfalseを返す. 
		/// </summary>
		/// <param name="key">Keys</param>
		/// <returns>アサイン가능ならtrue</returns>
		public virtual bool CanAssignKeys(Keys key)
		{
			key	&= ~(Keys.Alt | Keys.Control | Keys.Shift);

			// なし
			if(key == Keys.None)				return false;

			// Ctrl등
			if(key == Keys.ShiftKey)			return false;
			if(key == Keys.Menu)				return false;
			if(key == Keys.ControlKey)			return false;

			// CapsLock
			if(key == (Keys.Oemtilde | Keys.D0))				return false;
			// ひらがな
			if(key == (Keys.OemBackslash | Keys.ShiftKey))		return false;
			if(key == Keys.KanaMode)							return false;
			// IME
			if(key == Keys.IMEAceept)							return false;
			if(key == Keys.IMEConvert)							return false;
			if(key == Keys.IMENonconvert)						return false;
			if(key == Keys.IMEAceept)							return false;
			if(key == Keys.IMEAccept)							return false;
			if(key == Keys.IMEModeChange)						return false;
			// Win
			if(key == Keys.LWin)								return false;
			if(key == Keys.RWin)								return false;

			if(key == Keys.Apps)								return false;
			if(key == Keys.NumLock)								return false;

			// CapsLock
			if(key == Keys.Capital)								return false;

			// 반角, 全角
			if(key == (Keys.Oemtilde | Keys.D4))				return false;
			if(key == (Keys.OemBackslash | Keys.ControlKey))	return false;
	
			// アサイン가능
			return true;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// System.Windows.Forms.Shortcutかどうかを得る. 
		/// System.Windows.Forms.Shortcutのみをアサイン가능にするヘルパ. 
		/// CanAssignKeys()をオーバーライドしたときに使えるかもしれない. 
		/// </summary>
		/// <param name="key">key</param>
		/// <returns>アサイン가능ならtrue</returns>
		static public bool IsShortcut(Keys key)
		{
			// Noneはfalseを返す
			if(key == Keys.None)	return false;

			// 변환できればアサイン가능
			object	a = Useful.ToEnum(typeof(Shortcut), (object)(int)key);
			if(a == null)	return false;
			return true;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 키の문자열を得る. 
		/// </summary>
		/// <param name="e">KeyEventArgs</param>
		/// <returns>키내용の문자열</returns>
		/// <remarks>
		/// <para>키の문자열を得る. </para>
		/// <para>何も押されていないときは"なし"を返す. </para>
		/// <para>내部でCanAssignKeys()を呼び出し, アサイン가능な키かどうかを判断する. </para>
		/// <para>アサイン가능でなければ"なし"を返す. </para>
		/// <para>Ctrl, Shift, Altは特別でこれらの키のみが押された場合は</para>
		/// <para>"Ctrl+"등を返す. </para>
		/// </remarks>
		public string GetKeysString(KeyEventArgs e)
		{
			return GetKeysString(e.KeyData);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 키の문자열を得る
		/// </summary>
		/// <param name="key">Keys</param>
		/// <returns>키내용の문자열</returns>
		/// <remarks>
		/// <para>키の문자열を得る. </para>
		/// <para>KeyEventArgs.KeyDataを渡すこと</para>
		/// <para>何も押されていないときは"なし"を返す. </para>
		/// <para>내部でCanAssignKeys()を呼び出し, アサイン가능な키かどうかを判断する. </para>
		/// <para>アサイン가능でなければ"なし"を返す. </para>
		/// <para>Ctrl, Shift, Altは特別でこれらの키のみが押された場合は</para>
		/// <para>"Ctrl+"등を返す. </para>
		/// </remarks>
		public string GetKeysString(Keys key)
		{
			string	str	= "";
			if((key & Keys.Alt) != 0)		str	+= "Alt+";
			if((key & Keys.Control) != 0)	str	+= "Ctrl+";
			if((key & Keys.Shift) != 0)		str += "Shift+";

			// 할당られる키かチェック
			if(CanAssignKeys(key)){
				str	+= keys_to_string(key);
			}else{
				if(str == "")	return "なし";	// 무효키
			}
			return str;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 키を문자열で得る
		/// Oem1とかを少しマシにする
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		static protected string keys_to_string(Keys key)
		{
			key	&= ~(Keys.Alt | Keys.Control | Keys.Shift);

			switch(key){
			case Keys.Back:			return "BackSpace";
			case Keys.D0:			return "0";
			case Keys.D1:			return "1";
			case Keys.D2:			return "2";
			case Keys.D3:			return "3";
			case Keys.D4:			return "4";
			case Keys.D5:			return "5";
			case Keys.D6:			return "6";
			case Keys.D7:			return "7";
			case Keys.D8:			return "8";
			case Keys.D9:			return "9";
			case Keys.OemMinus:		return "=";
			case Keys.Oem7:			return "^";
			case Keys.Oem5:			return "|";
			case Keys.Oem3:			return "@";
			case Keys.Oem4:			return "[";
			case Keys.Oemplus:		return ";";
			case Keys.Oem1:			return ":";
			case Keys.Oem6:			return "]";
			case Keys.Oemcomma:		return ",";
			case Keys.OemPeriod:	return ".";
			case Keys.Oem2:			return "/";
			case Keys.Oem102:		return "\\";
			case Keys.Enter:		return "Enter";
			case Keys.PageUp:		return "PageUp";
			case Keys.PageDown:		return "PageDown";
			case Keys.Escape:		return "ESC";
			case Keys.Add:			return "+";
			case Keys.Subtract:		return "-";
			case Keys.Divide:		return "/";
			case Keys.Multiply:		return "*";
//			case Keys.None:			return "なし";
			}
			// その他
			return key.ToString();
		}
	}
	
	//-------------------------------------------------------------------------
	/// <summary>
	/// 키アサイン관리
	/// </summary>
	/// <remarks>
	/// <para>키アサインを관리する. </para>
	/// <para>1つの키アサインに할り振れる키は1つ. (Ctrl+C등)</para>
	/// <para>List.AddAssign()を呼び, 키アサイン테이블을初期化する. </para>
	/// <para>このクラスはIIniSaveLoadを継承しているため, IniBaseを사용した설정파일の읽기が가능. </para>
	/// <para>ShowSettingDialog()でKeyAssignFormによる설정が가능</para>
	/// <para>アサインを実行するにはProcessCmdKey()を呼ぶ. </para>
	/// <para>ProcessCmdKey()は통상FormのProcessCmdKey()내で呼ぶ. </para>
	/// <para>할당가능かどうかの判定にはAssignRuleが사용される. </para>
	/// <para>独自の할당ルールを지정する場合は, AssignRuleを継承しコンストラクタに渡す. </para>
	/// <para>引수なしのコンストラクタを사용すると, AssignRuleを사용する. </para>
	/// <para>AssignRuleを継承したKeyAssignRuleOnlyShortcutはSystem.Windows.Forms.Shortcutのみを할당가능にする. </para>
	/// <para>メニューやコンテキストメニューと同期させるには, </para>
	/// <para>BindTagForMenuItem()でタグを関連付け, UpdateMenuShortcutKeys()でショートカット키の표시を更新する. </para>
	/// <para>同期させるとメニュークリックのアサイン実行が自動で行われる. </para>
	/// </remarks>
	public class KeyAssignManager : IIniSaveLoad
	{
		private KeyAssignList				m_list;						// assign list
		private OnProcessCmdKey				m_on_processcmdkey;			// ショートカットの実行
		private EventHandler				m_on_update_assign_list;	// アサイン목록が更新されたときに呼び出されるデリゲート

		//-------------------------------------------------------------------------
		/// <summary>
		/// アサイン목록を得る. 
		/// 설정時はOnUpdateAssignList()が呼ばれる. 
		/// </summary>
		public KeyAssignList List			{	get{	return m_list;		}
												set{	
													m_list	= value;
													if(m_on_update_assign_list != null){
														m_on_update_assign_list(this, EventArgs.Empty);
													}
												}
											}

		/// <summary>
		/// 키が押されたときのデリゲート
		/// </summary>
		public event OnProcessCmdKey OnProcessCmdKey{
			add{	m_on_processcmdkey	+= value;	}
			remove{	m_on_processcmdkey	-= value;	}
		}

		/// <summary>
		/// アサイン목록が更新されたときに呼び出されるデリゲート
		/// </summary>
		public event EventHandler OnUpdateAssignList{
			add{	m_on_update_assign_list	+= value;	}
			remove{	m_on_update_assign_list	-= value;	}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// IniBase읽기용그룹명の初期値
		/// </summary>
		public string DefaultIniGroupName		{	get{	return "KeyAssignManagerSettings";	}}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축, 할당ルールにはAssignRuleが사용される
		/// </summary>
		public KeyAssignManager()
			: this(new KeyAssignRule())
		{
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축, 할당ルール지정版
		/// </summary>
		/// <param name="rule"></param>
		public KeyAssignManager(KeyAssignRule rule)
		{
			m_list			= new KeyAssignList(rule);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// ショートカットの実行. 
		/// 통상メインformのProcessCmdKey내で呼び出す. 
		/// Assignされた키が押された場合, OnProcessCmdKeyに등록された
		/// デリゲートが呼び出される. 
		/// 意図しない呼び出しをチェックするため, OnProcessCmdKeyがnullの場合例외を投げる
		/// </summary>
		/// <param name="keyData">keyData</param>
		/// <returns>ショートカットを実行した場合true</returns>
		public bool ProcessCmdKey(Keys keyData)
		{
			if(m_on_processcmdkey == null)	throw new Exception("OnProcessCmdKeyにデリゲートを등록してから사용してください. ");

			List<KeyAssignList.Assign>	alist	= m_list.GetAssignedList(keyData);
			if(alist == null)	return false;		// 할당られた기능はない

			// 대응するアサイン분イベントを発生させる
			foreach(KeyAssignList.Assign i in alist){
				m_on_processcmdkey(this, new KeyAssignEventArg(i.Tag));
			}
			return true;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// メニューをクリックされたときのショートカットの実行. 
		/// メニューのClickイベント내で呼ぶ. 
		/// BindTagForMenuItem()でタグを설정しておけば自動で呼ばれる. 
		/// </summary>
		/// <param name="sender">Clickイベントの引수そのまま</param>
		/// <param name="e">Clickイベントの引수そのまま</param>
		public void OnClickToolStripMenu(object sender, EventArgs e)
		{
			if(m_on_processcmdkey == null)	throw new Exception("OnProcessCmdKeyにデリゲートを등록してから사용してください. ");

			// ToolStripMenuItem以외は無視
			if(!(sender is ToolStripMenuItem))	return;
			ToolStripMenuItem	ts	= (ToolStripMenuItem)sender;

			// Tagがnullかどうかだけチェック
			if(ts.Tag == null)					return;

			// ショートカット実行
			m_on_processcmdkey(this, new KeyAssignEventArg(ts.Tag));
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// メニュー아이템のタグを설정する. 
		/// クリック時のデリゲートの등록も行う. 
		/// デリゲートにはOnClickToolStripMenu()を할당る. 
		/// このメソッドでメニュー아이템にTagを할당, 
		/// UpdateMenuShortcutKeys()でショートカットの표시を更新する. 
		/// クリックされた場合は自動でOnClickToolStripMenu()が呼ばれる. 
		/// </summary>
		/// <param name="item">メニュー아이템</param>
		/// <param name="tag">タグ</param>
		public void BindTagForMenuItem(ToolStripMenuItem item, object tag)
		{
			if(item == null)	throw new ArgumentNullException();
			if(tag == null)		throw new ArgumentNullException();

			item.Tag	= tag;
			item.Click	+= new EventHandler(OnClickToolStripMenu);
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// メニューのショートカット키を更新する. 
		/// メニューに표시されるショートカットと관리されているアサインを一致させる. 
		/// ToolStripItem.TagがAssign.Tagと一致するものを검색し, ショートカットを更新する. 
		/// 표시のみ설정するため, メニューのショートカットの기능は使わない. 
		/// ContextMenuStrip版
		/// </summary>
		/// <param name="menu">ContextMenuStrip</param>
		public void UpdateMenuShortcutKeys(ContextMenuStrip menu)
		{
			foreach(ToolStripItem i in menu.Items){
				update_shortcut_keys(i);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// メニューのショートカット키を更新する. 
		/// メニューに표시されるショートカットと관리されているアサインを一致させる. 
		/// ToolStripItem.TagがAssign.Tagと一致するものを검색し, ショートカットを更新する. 
		/// 표시のみ설정するため, メニューのショートカットの기능は使わない. 
		/// MenuStrip版
		/// </summary>
		/// <param name="menu">MenuStrip</param>
		public void UpdateMenuShortcutKeys(MenuStrip menu)
		{
			foreach(ToolStripItem i in menu.Items){
				update_shortcut_keys(i);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// ショートカット키を更新する. 
		/// Tagが一致する場合, 現在のショートカット키を할당る. 
		/// 표시のみ설정するため, メニューのショートカットの기능は使わない. 
		/// </summary>
		/// <param name="item">대상</param>
		private void update_shortcut_keys(ToolStripItem item)
		{
			// ToolStripMenuItem以외は無視
			if(item is ToolStripMenuItem){
				ToolStripMenuItem	smi = (ToolStripMenuItem)item;
				foreach(ToolStripItem i in smi.DropDownItems){
					// 子がいれば다시帰で潜る
					update_shortcut_keys(i);
				}

				// Tagが一致するAssignに更新
				if(smi.Tag != null){
					KeyAssignList.Assign	a	= m_list.GetAssign(smi.Tag);
					if(a != null){
						// smi.ShortcutKeysにはShortcutのみしか지정できないため, 
						// 표시설정のみ行う
						smi.ShortcutKeys				= Keys.None;	// ショートカット키としてはなし
						smi.ShowShortcutKeys			= true;
						smi.ShortcutKeyDisplayString	= (a.Keys == Keys.None)? "": a.KeysString;
					}
				}
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 설정파일からの읽기
		/// </summary>
		/// <param name="p">IniBase</param>
		/// <param name="group">그룹명</param>
		public void IniLoad(IIni p, string group)
		{
			if(String.IsNullOrEmpty(group))		return;
			if(p == null)						return;

			m_list.IniLoad(p, group);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 설정파일に書き出し
		/// </summary>
		/// <param name="p">IniBase</param>
		/// <param name="group">그룹명</param>
		public void IniSave(IIni p, string group)
		{
			if(String.IsNullOrEmpty(group))		return;
			if(p == null)						return;

			m_list.IniSave(p, group);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 키アサイン설정
		/// 설정ダイア로그を開きアサインを설정する. 
		/// 独自のダイア로그を사용するときは
		/// KeyAssiginSettingHelperを利용する. 
		/// </summary>
		/// <param name="form">親となるフォーム</param>
		/// <returns>변경が行われたときtrue</returns>
		public bool ShowSettingDialog(Form form)
		{
			using(KeyAssignListForm dlg = new KeyAssignListForm(m_list)){
				if(dlg.ShowDialog(form) == DialogResult.OK){
					// 설정변경を反映させる
					// OnUpdateAssignList()を呼ぶ
					this.List	= dlg.List;
					return true;
				}
			}
			return false;
		}
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// アサイン목록. 
	/// 설정변경용にDeepClone()できる. 
	/// </summary>
	public sealed class KeyAssignList : IEnumerable<KeyAssignList.Assign>
	{
		private List<Assign>				m_list;
		private KeyAssignRule				m_assign_rule;			// 할당ルール

		/// <summary>
		/// 목록수
		/// </summary>
		public int Count					{	get{	return m_list.Count;	}}

		/// <summary>
		/// index참조
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Assign this[int index]		{	get{	return m_list[index];	}}
		

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축, 
		/// KeyAssignRuleを사용する
		/// </summary>
		public KeyAssignList()
			: this(new KeyAssignRule())
		{
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축, 
		/// KeyAssignRuleを지정する
		/// </summary>
		/// <param name="rule"></param>
		public KeyAssignList(KeyAssignRule rule)
		{
			m_list			= new List<Assign>();
			m_assign_rule	= rule;

			if(m_assign_rule == null){
				throw new ArgumentNullException("AssignRuleの지정がnullです. ");
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축, 
		/// コピーコンストラクタ
		/// </summary>
		/// <param name="from"></param>
		public KeyAssignList(KeyAssignList from)
		{
			m_list			= new List<Assign>();
			m_assign_rule	= from.m_assign_rule;	// ルールは참조をコピー
			foreach(Assign i in from.m_list){
				m_list.Add(new Assign(i));
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// コピー
		/// KeyAssignRuleは참조のコピーなので注意
		/// </summary>
		/// <returns></returns>
		public KeyAssignList DeepClone()
		{
			return new KeyAssignList(this);
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 추가, 
		/// 初期値なしの場合はKeys.Noneを渡すこと
		/// </summary>
		/// <param name="name">アサイン명</param>
		/// <param name="group">그룹명</param>
		/// <param name="default_key">初期アサイン</param>
		/// <param name="tag">タグ</param>
		/// <param name="ini_name">설정파일書き出し時の이름(英수が望ましい)</param>
		public void AddAssign(string name, string group, Keys default_key, object tag, string ini_name)
		{
			m_list.Add(new Assign(m_assign_rule, name, group, default_key, tag, ini_name));
		}
		
		//-------------------------------------------------------------------------
		/// <summary>
		/// 추가, 
		/// public void AddAssign(string name, string group, Keys default_key, object tag, string ini_name)
		/// の少し引수が少ない版
		/// </summary>
		/// <param name="name">アサイン명</param>
		/// <param name="group">그룹명</param>
		/// <param name="default_key">初期アサイン</param>
		/// <param name="tag">タグ</param>
		/// <remarks>
		/// <para>AddAssign(name, group, default_key, tag, tag.ToString());</para>
		/// <para>と呼んだのと同じ. </para>
		/// <para>ini_nameを個別に설정しないならこのメソッドを사용する. </para>
		/// </remarks>
		public void AddAssign(string name, string group, Keys default_key, object tag)
		{
			AddAssign(name, group, default_key, tag, tag.ToString());
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 그룹の목록を得る
		/// ない場合はnullを返す
		/// </summary>
		/// <returns>그룹목록</returns>
		public List<string> GetGroupList()
		{
			List<string>	list	= new List<string>();

			// 一致するものを검색
			foreach(Assign a in m_list){
				if(!is_found(a.Group, list)){
					list.Add(a.Group);
				}
			}
			if(list.Count <= 0)		return null;
			return list;
		}

		//-------------------------------------------------------------------------
		private bool is_found(string str, List<string> list)
		{
			foreach(string i in list){
				if(i == str)	return true;
			}
			return false;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 지정された그룹に属する목록を得る
		/// ない場合はnullを返す
		/// </summary>
		/// <param name="group">그룹명</param>
		/// <returns>그룹に属する키アサイン목록</returns>
		public List<Assign> GetAssignedListFromGroup(string group)
		{
			List<Assign>	list	= new List<Assign>();
			// 一致するものを검색
			foreach(Assign a in m_list){
				if(a.Group == group){
					list.Add(a);
				}
			}
			if(list.Count <= 0)		return null;
			return list;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 渡された키がアサインされた목록を返す
		/// アサインされていない場合はnullを返す
		/// </summary>
		/// <param name="e">KeyEventArgs</param>
		/// <returns>アサインされた키の목록</returns>
		public List<Assign> GetAssignedList(KeyEventArgs e)
		{
			return GetAssignedList(e.KeyData);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 渡された키がアサインされた목록を返す
		/// アサインされていない場合はnullを返す
		/// KeyEventArgs.KeyCodeを渡すこと
		/// </summary>
		/// <param name="key">키</param>
		/// <returns>アサインされた키の목록</returns>
		public List<Assign> GetAssignedList(Keys key)
		{
			List<Assign>	list	= new List<Assign>();

			// 一致するものを검색
			foreach(Assign a in m_list){
				if(a.IsAssignedKey(key)){
					list.Add(a);
				}
			}

			if(list.Count <= 0)	return null;	// アサインされていない
			return list;			
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 키アサインを전부初期値に戻す
		/// </summary>
		public void DefaultAll()
		{
			foreach(Assign a in m_list){
				a.Default();
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Tagに関連付けられた키アサインを得る
		/// </summary>
		/// <param name="tag">Tag</param>
		/// <returns>키アサイン</returns>
		public Assign GetAssign(object tag)
		{
			// 一致するものを검색
			foreach(Assign a in m_list){
				if(a.Tag.Equals(tag))	return a;
			}
			return null;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Tagに関連付けられた키アサインのショートカット키を문자열で得る. 
		/// 할당なし, 関連付けられた키アサインがない場合は""を返す. 
		/// </summary>
		/// <param name="tag">Tag</param>
		/// <returns>ショートカット키の문자열</returns>
		public string GetAssignShortcutText(object tag)
		{
			Assign	a	= GetAssign(tag);
			if(a == null)				return "";
			if(a.Keys == Keys.None)		return "";
			return a.KeysString;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 설정파일からの읽기
		/// IniBaseを대상としている
		/// </summary>
		/// <param name="p">IniBase</param>
		/// <param name="group">그룹명</param>
		public void IniLoad(IIni p, string group)
		{
			if(String.IsNullOrEmpty(group))		return;
			if(p == null)						return;

			foreach(Assign a in m_list){
				a.IniLoad(p, group);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 설정파일に書き出し
		/// IniBaseを대상としている
		/// </summary>
		/// <param name="p">IniBase</param>
		/// <param name="group">그룹명</param>
		public void IniSave(IIni p, string group)
		{
			if(String.IsNullOrEmpty(group))		return;
			if(p == null)						return;

			foreach(Assign a in m_list){
				a.IniSave(p, group);
			}
		}

		//-------------------------------------------------------------------------
		/// 열挙
		public IEnumerator<Assign> GetEnumerator()
		{
			for(int i=0; i<m_list.Count; i++){
				yield return m_list[i];
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 키アサイン
		/// </summary>
		public sealed class Assign
		{
			private KeyAssignRule			m_assign_rule;	// 할당ルール

			private Keys					m_keys;			// KeyEventArgs.KeyData
			private Keys					m_default_keys;	// KeyEventArgs.KeyData
			private string					m_name;			// function name
			private string					m_group;		// group name
			private object					m_tag;			// tag
			private string					m_ini_name;		// 설정保存용명

			//-------------------------------------------------------------------------
			/// <summary>
			/// 키アサインを문자열で得る
			/// </summary>
			public string KeysString		{	get{	return GetKeysString(m_keys);	}}
			/// <summary>
			/// 키アサイン설정と取得
			/// </summary>
			public Keys Keys				{	get{	return m_keys;					}
									internal	set{	m_keys	= (CanAssignKeys(value))? value: Keys.None;	}}

			/// <summary>
			/// 初期値の키アサインを得る
			/// </summary>
			public Keys DefaultKeys			{	get{	return m_default_keys;			}}
			/// <summary>
			/// 初期値の키アサインを문자열で得る
			/// </summary>
			public string DefaultKeysString	{	get{	return GetKeysString(m_default_keys);	}}
			/// <summary>
			/// アサイン명を得る
			/// </summary>
			public string Name				{	get{	return m_name;					}}
			/// <summary>
			/// 그룹명を得る
			/// </summary>
			public string Group				{	get{	return m_group;					}}
			/// <summary>
			/// Tagを得る
			/// </summary>
			public object Tag				{	get{	return m_tag;					}}
			/// <summary>
			/// 설정파일書き出し時の이름を得る
			/// </summary>
			public string IniName			{	get{	return m_ini_name;				}}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 구축
			/// 初期アサインを최초のアサインとする
			/// </summary>
			/// <param name="rule">할당ルール</param>
			/// <param name="name">アサイン명</param>
			/// <param name="group">그룹명</param>
			/// <param name="default_key">初期アサイン</param>
			/// <param name="tag">タグ</param>
			/// <param name="ini_name">설정파일書き出し時の이름(英수が望ましい)</param>
			internal Assign(KeyAssignRule rule, string name, string group, Keys default_key, object tag, string ini_name)
				: this(rule, name, group, default_key, default_key, tag, ini_name)
			{
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 구축
			/// 初期アサインとアサインを別々に설정する版
			/// </summary>
			/// <param name="rule">할당ルール</param>
			/// <param name="name">アサイン명</param>
			/// <param name="group">그룹명</param>
			/// <param name="default_key">初期アサイン</param>
			/// <param name="key">アサイン</param>
			/// <param name="tag">タグ</param>
			/// <param name="ini_name">설정파일書き出し時の이름(英수が望ましい)</param>
			internal Assign(KeyAssignRule rule, string name, string group, Keys default_key, Keys key, object tag, string ini_name)
			{
				if(rule == null){
					throw new ArgumentNullException("ショートカットに할당られるかを決定するためにAssignRuleが必要です. ");
				}
				m_assign_rule	= rule;

				m_keys			= key;
				m_default_keys	= default_key;
				m_name			= name;
				m_group			= group;
				m_tag			= tag;
				m_ini_name		= ini_name;

				// アサイン不가능ならNoneにする
				if(!CanAssignKeys(m_keys)){
					m_keys			= Keys.None;
				}
				// 初期値で할당不可なら例외を投げる
				if(   (m_default_keys != Keys.None)
					&&(!CanAssignKeys(m_default_keys)) ){
					throw new Exception("할당できない키の組み合わせが初期値に지정されました. ");
//					m_default_keys	= Keys.None;
				}
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 구축, コピーコンストラクタ, 
			/// </summary>
			/// <param name="from">コピー元のAssign</param>
			internal Assign(Assign from)
				: this(	from.m_assign_rule,
						from.Name,
						from.Group,
						from.DefaultKeys,
						from.Keys,
						from.Tag,
						from.IniName)
			{
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// アサインされた키かどうかを得る
			/// Keys.Noneが渡された場合はfalseを返す
			/// </summary>
			/// <param name="key">Keys</param>
			/// <returns>アサインされていればtrue</returns>
			public bool IsAssignedKey(Keys key)
			{
				if(key == Keys.None)	return false;
				return m_keys == key;
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 初期値に설정する
			/// </summary>
			internal void Default()
			{
				this.Keys	= this.DefaultKeys;
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 설정파일からの읽기
			/// </summary>
			/// <param name="p">IniBase</param>
			/// <param name="group">그룹명</param>
			internal void IniLoad(IIni p, string group)
			{
				if(String.IsNullOrEmpty(group))			return;
				if(String.IsNullOrEmpty(m_ini_name))	return;
				if(p == null)							return;

				// 할당不可ならNone
				this.Keys		= (Keys)p.GetProfile(group, m_ini_name, (int)m_keys);
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 설정파일に書き出し
			/// </summary>
			/// <param name="p">IniBase</param>
			/// <param name="group"></param>
			internal void IniSave(IIni p, string group)
			{
				if(String.IsNullOrEmpty(group))			return;
				if(String.IsNullOrEmpty(m_ini_name))	return;
				if(p == null)							return;

				p.SetProfile(group, m_ini_name, (int)m_keys);
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 키の문자열を得る. 
			/// 何も押されていないときは"なし"を返す. 
			/// 할당가능でないものは"なし"を返す. 
			/// 할당가능かどうかはKeyAssignRule.CanAssignKeys()の実装による. 
			/// </summary>
			/// <param name="key"></param>
			/// <returns></returns>
			public string GetKeysString(Keys key)
			{
				return m_assign_rule.GetKeysString(key);
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 할당가능かどうかを得る. 
			/// 할당가능かどうかはKeyAssignRule.CanAssignKeys()の実装による. 
			/// </summary>
			/// <param name="key">key</param>
			/// <returns>할당가능ならtrue</returns>
			public bool CanAssignKeys(Keys key)
			{
				return m_assign_rule.CanAssignKeys(key);
			}
		}
	}
}
