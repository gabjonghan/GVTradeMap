//-------------------------------------------------------------------------
// 키 할당 관리
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
	/// 단축키 실행용 델리게이트
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="arg"></param>
	public delegate void OnProcessCmdKey(object sender, KeyAssignEventArg arg);

	//-------------------------------------------------------------------------
	/// <summary>
	/// 단축키 실행 시의 인수
	/// </summary>
	public sealed class KeyAssignEventArg : EventArgs
	{
		private object							m_tag;

		/// <summary>
		/// 단축키에 연결된 object,
		/// 통상 Enum 을 사용한다
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
	/// 할당 규칙,
	/// System.Windows.Forms.Shortcut 만 할당 가능하게 하는 버전
	/// </summary>
	public class KeyAssignRuleOnlyShortcut : KeyAssignRule
	{
		//-------------------------------------------------------------------------
		/// <summary>
		/// System.Windows.Forms.Shortcut 인 경우에만 true 를 반환한다
		/// </summary>
		/// <param name="key">Keys</param>
		/// <returns>할당 가능하면 true</returns>
		public override bool CanAssignKeys(Keys key)
		{
			// System.Windows.Forms.Shortcut 인지 확인
			if(!KeyAssignRule.IsShortcut(key))	return false;

			// base
			return base.CanAssignKeys(key);
		}
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// 할당 규칙,
	/// 할당 가능한지 여부와 키의 문자열 변환을 담당한다.
	/// 상속해서 규칙을 변경할 수 있다.
	/// 동적으로 규칙을 변경하면 유저를 혼란시키는 원인이 되므로 주의.
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
		/// 할당 가능한지 여부를 얻는다.
		/// </summary>
		/// <param name="e">KeyEventArgs</param>
		/// <returns>할당 가능하면 true</returns>
		public bool CanAssignKeys(KeyEventArgs e)
		{
			return CanAssignKeys(e.KeyData);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 할당 가능한지 여부를 얻는다.
		/// ESC, TAB, 영문자 단독 등이 가능.
		/// 이 메서드를 오버라이드해서 할당 가능한 키 조합을
		/// 커스터마이즈할 수 있다.
		/// 이 메서드는 최소한 할당할 수 없는 것만 false 를 반환한다.
		/// </summary>
		/// <param name="key">Keys</param>
		/// <returns>할당 가능하면 true</returns>
		public virtual bool CanAssignKeys(Keys key)
		{
			key	&= ~(Keys.Alt | Keys.Control | Keys.Shift);

			// 없음
			if(key == Keys.None)				return false;

			// Ctrl등
			if(key == Keys.ShiftKey)			return false;
			if(key == Keys.Menu)				return false;
			if(key == Keys.ControlKey)			return false;

			// CapsLock
			if(key == (Keys.Oemtilde | Keys.D0))				return false;
			// 히라가나
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

			// 반각, 전각
			if(key == (Keys.Oemtilde | Keys.D4))				return false;
			if(key == (Keys.OemBackslash | Keys.ControlKey))	return false;
	
			// 할당 가능
			return true;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// System.Windows.Forms.Shortcut 인지 여부를 얻는다.
		/// System.Windows.Forms.Shortcut 만 할당 가능하게 하는 헬퍼.
		/// CanAssignKeys() 를 오버라이드했을 때 쓸 수 있을지도 모른다.
		/// </summary>
		/// <param name="key">key</param>
		/// <returns>할당 가능하면 true</returns>
		static public bool IsShortcut(Keys key)
		{
			// None 은 false 를 반환한다
			if(key == Keys.None)	return false;

			// 변환할 수 있으면 할당 가능
			object	a = Useful.ToEnum(typeof(Shortcut), (object)(int)key);
			if(a == null)	return false;
			return true;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 키의 문자열을 얻는다.
		/// </summary>
		/// <param name="e">KeyEventArgs</param>
		/// <returns>키 내용의 문자열</returns>
		/// <remarks>
		/// <para>키의 문자열을 얻는다.</para>
		/// <para>아무것도 누르지 않았을 때는 "없음"을 반환한다.</para>
		/// <para>내부에서 CanAssignKeys() 를 호출해, 할당 가능한 키인지 판단한다.</para>
		/// <para>할당 가능하지 않으면 "없음"을 반환한다.</para>
		/// <para>Ctrl, Shift, Alt 는 특별해서 이 키들만 눌린 경우에는</para>
		/// <para>"Ctrl+" 등을 반환한다.</para>
		/// </remarks>
		public string GetKeysString(KeyEventArgs e)
		{
			return GetKeysString(e.KeyData);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 키의 문자열을 얻는다
		/// </summary>
		/// <param name="key">Keys</param>
		/// <returns>키 내용의 문자열</returns>
		/// <remarks>
		/// <para>키의 문자열을 얻는다.</para>
		/// <para>KeyEventArgs.KeyData 를 넘길 것</para>
		/// <para>아무것도 누르지 않았을 때는 "없음"을 반환한다.</para>
		/// <para>내부에서 CanAssignKeys() 를 호출해, 할당 가능한 키인지 판단한다.</para>
		/// <para>할당 가능하지 않으면 "없음"을 반환한다.</para>
		/// <para>Ctrl, Shift, Alt 는 특별해서 이 키들만 눌린 경우에는</para>
		/// <para>"Ctrl+" 등을 반환한다.</para>
		/// </remarks>
		public string GetKeysString(Keys key)
		{
			string	str	= "";
			if((key & Keys.Alt) != 0)		str	+= "Alt+";
			if((key & Keys.Control) != 0)	str	+= "Ctrl+";
			if((key & Keys.Shift) != 0)		str += "Shift+";

			// 할당할 수 있는 키인지 확인
			if(CanAssignKeys(key)){
				str	+= keys_to_string(key);
			}else{
				if(str == "")	return "없음";	// 무효키
			}
			return str;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 키를 문자열로 얻는다
		/// Oem1 같은 것을 조금 낫게 만든다
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
//			case Keys.None:			return "없음";
			}
			// 그 밖의 것
			return key.ToString();
		}
	}
	
	//-------------------------------------------------------------------------
	/// <summary>
	/// 키 할당 관리
	/// </summary>
	/// <remarks>
	/// <para>키 할당을 관리한다.</para>
	/// <para>하나의 키 할당에 배정할 수 있는 키는 하나. (Ctrl+C 등)</para>
	/// <para>List.AddAssign() 을 호출해 키 할당 테이블을 초기화한다.</para>
	/// <para>이 클래스는 IIniSaveLoad 를 상속하므로, IniBase 를 사용한 설정 파일 읽기가 가능.</para>
	/// <para>ShowSettingDialog() 로 KeyAssignForm 을 통한 설정이 가능</para>
	/// <para>할당을 실행하려면 ProcessCmdKey() 를 호출한다.</para>
	/// <para>ProcessCmdKey() 는 통상 Form 의 ProcessCmdKey() 안에서 호출한다.</para>
	/// <para>할당 가능한지 여부의 판정에는 AssignRule 이 사용된다.</para>
	/// <para>독자적인 할당 규칙을 지정할 경우에는, AssignRule 을 상속해 생성자에 넘긴다.</para>
	/// <para>인수 없는 생성자를 사용하면 AssignRule 을 사용한다.</para>
	/// <para>AssignRule 을 상속한 KeyAssignRuleOnlyShortcut 은 System.Windows.Forms.Shortcut 만 할당 가능하게 한다.</para>
	/// <para>메뉴나 컨텍스트 메뉴와 동기화하려면,</para>
	/// <para>BindTagForMenuItem() 으로 태그를 연결하고, UpdateMenuShortcutKeys() 로 단축키 표시를 갱신한다.</para>
	/// <para>동기화하면 메뉴 클릭으로도 할당 실행이 자동으로 이루어진다.</para>
	/// </remarks>
	public class KeyAssignManager : IIniSaveLoad
	{
		private KeyAssignList				m_list;						// assign list
		private OnProcessCmdKey				m_on_processcmdkey;			// 단축키 실행
		private EventHandler				m_on_update_assign_list;	// 할당 목록이 갱신되었을 때 호출되는 델리게이트

		//-------------------------------------------------------------------------
		/// <summary>
		/// 할당 목록을 얻는다.
		/// 설정 시에는 OnUpdateAssignList() 가 호출된다.
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
		/// 키가 눌렸을 때의 델리게이트
		/// </summary>
		public event OnProcessCmdKey OnProcessCmdKey{
			add{	m_on_processcmdkey	+= value;	}
			remove{	m_on_processcmdkey	-= value;	}
		}

		/// <summary>
		/// 할당 목록이 갱신되었을 때 호출되는 델리게이트
		/// </summary>
		public event EventHandler OnUpdateAssignList{
			add{	m_on_update_assign_list	+= value;	}
			remove{	m_on_update_assign_list	-= value;	}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// IniBase 읽기용 그룹명의 초기값
		/// </summary>
		public string DefaultIniGroupName		{	get{	return "KeyAssignManagerSettings";	}}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축, 할당 규칙에는 AssignRule 이 사용된다
		/// </summary>
		public KeyAssignManager()
			: this(new KeyAssignRule())
		{
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축, 할당 규칙 지정 버전
		/// </summary>
		/// <param name="rule"></param>
		public KeyAssignManager(KeyAssignRule rule)
		{
			m_list			= new KeyAssignList(rule);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 단축키 실행.
		/// 통상 메인 form 의 ProcessCmdKey 안에서 호출한다.
		/// Assign 된 키가 눌린 경우, OnProcessCmdKey 에 등록된
		/// 델리게이트가 호출된다.
		/// 의도하지 않은 호출을 확인하기 위해, OnProcessCmdKey 가 null 인 경우 예외를 던진다
		/// </summary>
		/// <param name="keyData">keyData</param>
		/// <returns>단축키를 실행한 경우 true</returns>
		public bool ProcessCmdKey(Keys keyData)
		{
			if(m_on_processcmdkey == null)	throw new Exception("OnProcessCmdKey 에 델리게이트를 등록한 뒤 사용해 주십시오. ");

			List<KeyAssignList.Assign>	alist	= m_list.GetAssignedList(keyData);
			if(alist == null)	return false;		// 할당된 기능은 없다

			// 대응하는 할당만큼 이벤트를 발생시킨다
			foreach(KeyAssignList.Assign i in alist){
				m_on_processcmdkey(this, new KeyAssignEventArg(i.Tag));
			}
			return true;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 메뉴가 클릭되었을 때의 단축키 실행.
		/// 메뉴의 Click 이벤트 안에서 호출한다.
		/// BindTagForMenuItem() 으로 태그를 설정해 두면 자동으로 호출된다.
		/// </summary>
		/// <param name="sender">Click 이벤트의 인수 그대로</param>
		/// <param name="e">Click 이벤트의 인수 그대로</param>
		public void OnClickToolStripMenu(object sender, EventArgs e)
		{
			if(m_on_processcmdkey == null)	throw new Exception("OnProcessCmdKey 에 델리게이트를 등록한 뒤 사용해 주십시오. ");

			// ToolStripMenuItem 이외는 무시
			if(!(sender is ToolStripMenuItem))	return;
			ToolStripMenuItem	ts	= (ToolStripMenuItem)sender;

			// Tag 가 null 인지만 확인
			if(ts.Tag == null)					return;

			// 단축키 실행
			m_on_processcmdkey(this, new KeyAssignEventArg(ts.Tag));
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 메뉴 아이템의 태그를 설정한다.
		/// 클릭 시의 델리게이트 등록도 함께 수행한다.
		/// 델리게이트에는 OnClickToolStripMenu() 를 할당한다.
		/// 이 메서드로 메뉴 아이템에 Tag 를 할당하고,
		/// UpdateMenuShortcutKeys() 로 단축키 표시를 갱신한다.
		/// 클릭된 경우에는 자동으로 OnClickToolStripMenu() 가 호출된다.
		/// </summary>
		/// <param name="item">메뉴 아이템</param>
		/// <param name="tag">태그</param>
		public void BindTagForMenuItem(ToolStripMenuItem item, object tag)
		{
			if(item == null)	throw new ArgumentNullException();
			if(tag == null)		throw new ArgumentNullException();

			item.Tag	= tag;
			item.Click	+= new EventHandler(OnClickToolStripMenu);
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 메뉴의 단축키를 갱신한다.
		/// 메뉴에 표시되는 단축키와 관리되고 있는 할당을 일치시킨다.
		/// ToolStripItem.Tag 가 Assign.Tag 와 일치하는 것을 검색해, 단축키를 갱신한다.
		/// 표시만 설정하므로, 메뉴의 단축키 기능은 사용하지 않는다.
		/// ContextMenuStrip 버전
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
		/// 메뉴의 단축키를 갱신한다.
		/// 메뉴에 표시되는 단축키와 관리되고 있는 할당을 일치시킨다.
		/// ToolStripItem.Tag 가 Assign.Tag 와 일치하는 것을 검색해, 단축키를 갱신한다.
		/// 표시만 설정하므로, 메뉴의 단축키 기능은 사용하지 않는다.
		/// MenuStrip 버전
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
		/// 단축키를 갱신한다.
		/// Tag 가 일치하는 경우, 현재의 단축키를 할당한다.
		/// 표시만 설정하므로, 메뉴의 단축키 기능은 사용하지 않는다.
		/// </summary>
		/// <param name="item">대상</param>
		private void update_shortcut_keys(ToolStripItem item)
		{
			// ToolStripMenuItem 이외는 무시
			if(item is ToolStripMenuItem){
				ToolStripMenuItem	smi = (ToolStripMenuItem)item;
				foreach(ToolStripItem i in smi.DropDownItems){
					// 자식이 있으면 재귀로 파고든다
					update_shortcut_keys(i);
				}

				// Tag 가 일치하는 Assign 으로 갱신
				if(smi.Tag != null){
					KeyAssignList.Assign	a	= m_list.GetAssign(smi.Tag);
					if(a != null){
						// smi.ShortcutKeys 에는 Shortcut 만 지정할 수 있으므로,
						// 표시 설정만 수행한다
						smi.ShortcutKeys				= Keys.None;	// 단축키로는 없음
						smi.ShowShortcutKeys			= true;
						smi.ShortcutKeyDisplayString	= (a.Keys == Keys.None)? "": a.KeysString;
					}
				}
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 설정 파일에서 읽기
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
		/// 설정 파일에 기록
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
		/// 키 할당 설정
		/// 설정 대화상자를 열어 할당을 설정한다.
		/// 독자적인 대화상자를 사용할 때는
		/// KeyAssiginSettingHelper 를 이용한다.
		/// </summary>
		/// <param name="form">부모가 되는 폼</param>
		/// <returns>변경이 이루어졌을 때 true</returns>
		public bool ShowSettingDialog(Form form)
		{
			using(KeyAssignListForm dlg = new KeyAssignListForm(m_list)){
				if(dlg.ShowDialog(form) == DialogResult.OK){
					// 설정 변경을 반영시킨다
					// OnUpdateAssignList() 를 호출한다
					this.List	= dlg.List;
					return true;
				}
			}
			return false;
		}
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// 할당 목록.
	/// 설정 변경용으로 DeepClone() 할 수 있다.
	/// </summary>
	public sealed class KeyAssignList : IEnumerable<KeyAssignList.Assign>
	{
		private List<Assign>				m_list;
		private KeyAssignRule				m_assign_rule;			// 할당 규칙

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
		/// KeyAssignRule 을 사용한다
		/// </summary>
		public KeyAssignList()
			: this(new KeyAssignRule())
		{
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축, 
		/// KeyAssignRule 을 지정한다
		/// </summary>
		/// <param name="rule"></param>
		public KeyAssignList(KeyAssignRule rule)
		{
			m_list			= new List<Assign>();
			m_assign_rule	= rule;

			if(m_assign_rule == null){
				throw new ArgumentNullException("AssignRule 지정이 null 입니다. ");
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축, 
		/// 복사 생성자
		/// </summary>
		/// <param name="from"></param>
		public KeyAssignList(KeyAssignList from)
		{
			m_list			= new List<Assign>();
			m_assign_rule	= from.m_assign_rule;	// 규칙은 참조를 복사
			foreach(Assign i in from.m_list){
				m_list.Add(new Assign(i));
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 복사
		/// KeyAssignRule 은 참조 복사이므로 주의
		/// </summary>
		/// <returns></returns>
		public KeyAssignList DeepClone()
		{
			return new KeyAssignList(this);
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 추가, 
		/// 초기값이 없는 경우에는 Keys.None 을 넘길 것
		/// </summary>
		/// <param name="name">할당명</param>
		/// <param name="group">그룹명</param>
		/// <param name="default_key">초기 할당</param>
		/// <param name="tag">태그</param>
		/// <param name="ini_name">설정 파일 기록 시의 이름(영숫자가 바람직함)</param>
		public void AddAssign(string name, string group, Keys default_key, object tag, string ini_name)
		{
			m_list.Add(new Assign(m_assign_rule, name, group, default_key, tag, ini_name));
		}
		
		//-------------------------------------------------------------------------
		/// <summary>
		/// 추가, 
		/// public void AddAssign(string name, string group, Keys default_key, object tag, string ini_name)
		/// 의 인수가 조금 적은 버전
		/// </summary>
		/// <param name="name">할당명</param>
		/// <param name="group">그룹명</param>
		/// <param name="default_key">초기 할당</param>
		/// <param name="tag">태그</param>
		/// <remarks>
		/// <para>AddAssign(name, group, default_key, tag, tag.ToString());</para>
		/// <para>라고 호출한 것과 같다.</para>
		/// <para>ini_name 을 개별로 설정하지 않는다면 이 메서드를 사용한다.</para>
		/// </remarks>
		public void AddAssign(string name, string group, Keys default_key, object tag)
		{
			AddAssign(name, group, default_key, tag, tag.ToString());
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 그룹 목록을 얻는다
		/// 없는 경우에는 null 을 반환한다
		/// </summary>
		/// <returns>그룹목록</returns>
		public List<string> GetGroupList()
		{
			List<string>	list	= new List<string>();

			// 일치하는 것을 검색
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
		/// 지정된 그룹에 속하는 목록을 얻는다
		/// 없는 경우에는 null 을 반환한다
		/// </summary>
		/// <param name="group">그룹명</param>
		/// <returns>그룹에 속하는 키 할당 목록</returns>
		public List<Assign> GetAssignedListFromGroup(string group)
		{
			List<Assign>	list	= new List<Assign>();
			// 일치하는 것을 검색
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
		/// 넘겨받은 키가 할당된 목록을 반환한다
		/// 할당되어 있지 않은 경우에는 null 을 반환한다
		/// </summary>
		/// <param name="e">KeyEventArgs</param>
		/// <returns>할당된 키의 목록</returns>
		public List<Assign> GetAssignedList(KeyEventArgs e)
		{
			return GetAssignedList(e.KeyData);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 넘겨받은 키가 할당된 목록을 반환한다
		/// 할당되어 있지 않은 경우에는 null 을 반환한다
		/// KeyEventArgs.KeyCode 를 넘길 것
		/// </summary>
		/// <param name="key">키</param>
		/// <returns>할당된 키의 목록</returns>
		public List<Assign> GetAssignedList(Keys key)
		{
			List<Assign>	list	= new List<Assign>();

			// 일치하는 것을 검색
			foreach(Assign a in m_list){
				if(a.IsAssignedKey(key)){
					list.Add(a);
				}
			}

			if(list.Count <= 0)	return null;	// 할당되어 있지 않다
			return list;			
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 키 할당을 전부 초기값으로 되돌린다
		/// </summary>
		public void DefaultAll()
		{
			foreach(Assign a in m_list){
				a.Default();
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Tag 에 연결된 키 할당을 얻는다
		/// </summary>
		/// <param name="tag">Tag</param>
		/// <returns>키 할당</returns>
		public Assign GetAssign(object tag)
		{
			// 일치하는 것을 검색
			foreach(Assign a in m_list){
				if(a.Tag.Equals(tag))	return a;
			}
			return null;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Tag 에 연결된 키 할당의 단축키를 문자열로 얻는다.
		/// 할당 없음이거나 연결된 키 할당이 없는 경우에는 "" 를 반환한다.
		/// </summary>
		/// <param name="tag">Tag</param>
		/// <returns>단축키의 문자열</returns>
		public string GetAssignShortcutText(object tag)
		{
			Assign	a	= GetAssign(tag);
			if(a == null)				return "";
			if(a.Keys == Keys.None)		return "";
			return a.KeysString;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 설정 파일에서 읽기
		/// IniBase 를 대상으로 한다
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
		/// 설정 파일에 기록
		/// IniBase 를 대상으로 한다
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
		/// 열거
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
		/// 키 할당
		/// </summary>
		public sealed class Assign
		{
			private KeyAssignRule			m_assign_rule;	// 할당 규칙

			private Keys					m_keys;			// KeyEventArgs.KeyData
			private Keys					m_default_keys;	// KeyEventArgs.KeyData
			private string					m_name;			// function name
			private string					m_group;		// group name
			private object					m_tag;			// tag
			private string					m_ini_name;		// 설정 저장용 이름

			//-------------------------------------------------------------------------
			/// <summary>
			/// 키 할당을 문자열로 얻는다
			/// </summary>
			public string KeysString		{	get{	return GetKeysString(m_keys);	}}
			/// <summary>
			/// 키 할당 설정과 취득
			/// </summary>
			public Keys Keys				{	get{	return m_keys;					}
									internal	set{	m_keys	= (CanAssignKeys(value))? value: Keys.None;	}}

			/// <summary>
			/// 초기값의 키 할당을 얻는다
			/// </summary>
			public Keys DefaultKeys			{	get{	return m_default_keys;			}}
			/// <summary>
			/// 초기값의 키 할당을 문자열로 얻는다
			/// </summary>
			public string DefaultKeysString	{	get{	return GetKeysString(m_default_keys);	}}
			/// <summary>
			/// 할당명을 얻는다
			/// </summary>
			public string Name				{	get{	return m_name;					}}
			/// <summary>
			/// 그룹명을 얻는다
			/// </summary>
			public string Group				{	get{	return m_group;					}}
			/// <summary>
			/// Tag 를 얻는다
			/// </summary>
			public object Tag				{	get{	return m_tag;					}}
			/// <summary>
			/// 설정 파일 기록 시의 이름을 얻는다
			/// </summary>
			public string IniName			{	get{	return m_ini_name;				}}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 구축
			/// 초기 할당을 최초의 할당으로 삼는다
			/// </summary>
			/// <param name="rule">할당 규칙</param>
			/// <param name="name">할당명</param>
			/// <param name="group">그룹명</param>
			/// <param name="default_key">초기 할당</param>
			/// <param name="tag">태그</param>
			/// <param name="ini_name">설정 파일 기록 시의 이름(영숫자가 바람직함)</param>
			internal Assign(KeyAssignRule rule, string name, string group, Keys default_key, object tag, string ini_name)
				: this(rule, name, group, default_key, default_key, tag, ini_name)
			{
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 구축
			/// 초기 할당과 할당을 따로따로 설정하는 버전
			/// </summary>
			/// <param name="rule">할당 규칙</param>
			/// <param name="name">할당명</param>
			/// <param name="group">그룹명</param>
			/// <param name="default_key">초기 할당</param>
			/// <param name="key">할당</param>
			/// <param name="tag">태그</param>
			/// <param name="ini_name">설정 파일 기록 시의 이름(영숫자가 바람직함)</param>
			internal Assign(KeyAssignRule rule, string name, string group, Keys default_key, Keys key, object tag, string ini_name)
			{
				if(rule == null){
					throw new ArgumentNullException("단축키에 할당할 수 있는지 결정하기 위해 AssignRule 이 필요합니다. ");
				}
				m_assign_rule	= rule;

				m_keys			= key;
				m_default_keys	= default_key;
				m_name			= name;
				m_group			= group;
				m_tag			= tag;
				m_ini_name		= ini_name;

				// 할당 불가능하면 None 으로 만든다
				if(!CanAssignKeys(m_keys)){
					m_keys			= Keys.None;
				}
				// 초기값으로 할당 불가라면 예외를 던진다
				if(   (m_default_keys != Keys.None)
					&&(!CanAssignKeys(m_default_keys)) ){
					throw new Exception("할당할 수 없는 키 조합이 초기값으로 지정되었습니다. ");
//					m_default_keys	= Keys.None;
				}
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 구축, 복사 생성자,
			/// </summary>
			/// <param name="from">복사할 원본 Assign</param>
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
			/// 할당된 키인지 여부를 얻는다
			/// Keys.None 이 넘어온 경우에는 false 를 반환한다
			/// </summary>
			/// <param name="key">Keys</param>
			/// <returns>할당되어 있으면 true</returns>
			public bool IsAssignedKey(Keys key)
			{
				if(key == Keys.None)	return false;
				return m_keys == key;
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 초기값으로 설정한다
			/// </summary>
			internal void Default()
			{
				this.Keys	= this.DefaultKeys;
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 설정 파일에서 읽기
			/// </summary>
			/// <param name="p">IniBase</param>
			/// <param name="group">그룹명</param>
			internal void IniLoad(IIni p, string group)
			{
				if(String.IsNullOrEmpty(group))			return;
				if(String.IsNullOrEmpty(m_ini_name))	return;
				if(p == null)							return;

				// 할당 불가라면 None
				this.Keys		= (Keys)p.GetProfile(group, m_ini_name, (int)m_keys);
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 설정 파일에 기록
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
			/// 키의 문자열을 얻는다.
			/// 아무것도 누르지 않았을 때는 "없음"을 반환한다.
			/// 할당 가능하지 않은 것은 "없음"을 반환한다.
			/// 할당 가능한지 여부는 KeyAssignRule.CanAssignKeys() 의 구현에 따른다.
			/// </summary>
			/// <param name="key"></param>
			/// <returns></returns>
			public string GetKeysString(Keys key)
			{
				return m_assign_rule.GetKeysString(key);
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 할당 가능한지 여부를 얻는다.
			/// 할당 가능한지 여부는 KeyAssignRule.CanAssignKeys() 의 구현에 따른다.
			/// </summary>
			/// <param name="key">key</param>
			/// <returns>할당 가능하면 true</returns>
			public bool CanAssignKeys(Keys key)
			{
				return m_assign_rule.CanAssignKeys(key);
			}
		}
	}
}
