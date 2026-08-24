//-------------------------------------------------------------------------
// 키 할당 헬퍼
// KeyAssignForm 의 동작 내용
// 대화상자에 KeyAssignForm 의 항목을 끼워 넣었을 때 사용한다
// ComboBox	그룹선택용
// Button		할당변경용
// Button		할당解除용
// Button		초기 할당으로 되돌리기
// ListView	할당목록
// 이 다섯 가지 컨트롤을 넘긴다
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows.Forms;

//-------------------------------------------------------------------------
namespace Utility.KeyAssign
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// 키 할당 헬퍼.
	/// KeyAssignForm 의 동작 내용.
	/// 대화상자에 KeyAssignForm 의 항목을 끼워 넣었을 때 사용한다.
	/// </summary>
	public sealed class KeyAssiginSettingHelper
	{
		private ComboBox						m_select_group_cbox;
		private Button							m_assign_button;
		private Button							m_remove_assign_button;
		private Button							m_default_all_assign_button;
		private ListView						m_list_view;
		private Form							m_form;

		private KeyAssignList					m_assign_list;

		/// <summary>
		/// OK 버튼이 눌렸을 때의 변경 내용
		/// </summary>
		public KeyAssignList List				{	get{	return m_assign_list;	}}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축
		/// </summary>
		/// <param name="assign_list">필수, 복제해서 보관된다</param>
		/// <param name="form">설정용 폼, 할당용 대화상자 표시 시에 참조된다. 필수</param>
		/// <param name="cbox">그룹 선택용, null 이어도 된다</param>
		/// <param name="list_view">할당 목록 표시용 ListView, 필수</param>
		/// <param name="assign_button">할당 버튼, 필수</param>
		/// <param name="remove_assign_button">할당 삭제 버튼, null 이어도 된다</param>
		/// <param name="default_all_assign_button">전부를 초기값으로 되돌리는 버튼, null 이어도 된다</param>
		public KeyAssiginSettingHelper(
					KeyAssignList assign_list,
					Form form,
					ComboBox cbox,
					ListView list_view,
					Button assign_button,
					Button remove_assign_button,
					Button default_all_assign_button)
		{
			m_assign_list				= assign_list.DeepClone();	// 목록을 복사해서 가진다

			m_form						= form;
			m_select_group_cbox			= cbox;
			m_list_view					= list_view;
			m_assign_button				= assign_button;
			m_remove_assign_button		= remove_assign_button;
			m_default_all_assign_button	= default_all_assign_button;

			// 컨트롤 초기화
			init_ctrl();

			// 初期化
			init();

			// 할당 버튼 갱신
			update_assign_button();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 컨트롤 초기화
		/// </summary>
		private void init_ctrl()
		{
			// 그룹선택
			if(m_select_group_cbox != null){
				m_select_group_cbox.DropDownStyle			= System.Windows.Forms.ComboBoxStyle.DropDownList;
				m_select_group_cbox.FormattingEnabled		= true;
				m_select_group_cbox.SelectedIndexChanged	+= new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			}

			// 할당 변경 버튼
			if(m_assign_button != null){
				m_assign_button.Click += new System.EventHandler(this.button1_Click);
			}

			// 할당 해제 버튼
			if(m_remove_assign_button != null){
				m_remove_assign_button.Click += new System.EventHandler(this.button2_Click);
			}
	
			// 전부 초기화 버튼
			if(m_default_all_assign_button != null){
				m_default_all_assign_button.Click += new System.EventHandler(this.button3_Click);
			}
	
			// 목록
			if(m_list_view != null){
				m_list_view.FullRowSelect = true;
				m_list_view.GridLines = true;
				m_list_view.HideSelection = false;
				m_list_view.MultiSelect = false;
				m_list_view.ShowItemToolTips = true;
				m_list_view.UseCompatibleStateImageBehavior = false;
				m_list_view.View = System.Windows.Forms.View.Details;
				m_list_view.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
				m_list_view.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 初期化
		/// </summary>
		private void init()
		{
			m_list_view.Columns.Add("그룹", 90);
			m_list_view.Columns.Add("기능", 180);
			m_list_view.Columns.Add("할당", 100);

			List<string>	glist	= m_assign_list.GetGroupList();
			if(glist == null)	return;

			if(m_select_group_cbox != null){
				m_select_group_cbox.Items.Clear();
				m_select_group_cbox.Items.Add("전부");
				foreach(string i in glist){
					m_select_group_cbox.Items.Add(i);
				}
				if(m_select_group_cbox.Items.Count > 0){
					m_select_group_cbox.SelectedIndex	= 0;
				}
			}else{
				update_list();
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 목록을 갱신한다
		/// </summary>
		private void update_list()
		{
			m_list_view.BeginUpdate();
			m_list_view.Items.Clear();

			// 그룹 선택 상태에 따라 목록 내용을 정한다
			List<KeyAssignList.Assign>	list	= null;

			if(m_select_group_cbox != null){
				if(m_select_group_cbox.Text != "전부"){
					list	= m_assign_list.GetAssignedListFromGroup(m_select_group_cbox.Text);
				}
			}
			
			if(list != null){
				foreach(KeyAssignList.Assign i in list){
					add_item(m_list_view, i);
				}
			}else{
				foreach(KeyAssignList.Assign i in m_assign_list){
					add_item(m_list_view, i);
				}
			}

			m_list_view.EndUpdate();

			// 할당 버튼 갱신
			update_assign_button();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// ListView 에 추가한다
		/// </summary>
		/// <param name="listview">대상 ListView</param>
		/// <param name="i">키 할당</param>
		private void add_item(ListView listview, KeyAssignList.Assign i)
		{
			ListViewItem	item	= new ListViewItem(i.Group, 0);
			item.Tag				= i;
//			item.ToolTipText		= i.tool_tip;
			item.SubItems.Add(i.Name);
			item.SubItems.Add(i.KeysString);

			listview.Items.Add(item);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 할당 변경 버튼 갱신
		/// </summary>
		private void update_assign_button()
		{
			KeyAssignList.Assign	i	= get_selected_item();
			if(i != null){
				update_button(m_assign_button, true);
				update_button(m_remove_assign_button, (i.Keys == Keys.None)? false: true);
			}else{
				update_button(m_assign_button, false);
				update_button(m_remove_assign_button, false);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 버튼의 유효, 무효 갱신.
		/// </summary>
		/// <param name="button">버튼</param>
		/// <param name="enable">유효하게 할 때 true</param>
		private void update_button(Button button, bool enable)
		{
			if(button == null)				return;
			if(button.Enabled != enable)	button.Enabled	= enable;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 선택이 변경되었다
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			// 할당 버튼 갱신
			update_assign_button();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 더블 클릭되었다
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listView1_DoubleClick(object sender, EventArgs e)
		{
			button1_Click(sender, e);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 그룹 선택이 변경되었다
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			update_list();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 키할당변경
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, EventArgs e)
		{
			KeyAssignList.Assign	i		= get_selected_item();

			if(i == null)		return;

			using(KeyAssignForm dlg = new KeyAssignForm(i)){
				if(dlg.ShowDialog(m_form) == DialogResult.OK){
					i.Keys		= dlg.NewAssignKey;

					// 목록 갱신
					m_list_view.SelectedItems[0].SubItems[2].Text	= i.KeysString;

					// 버튼 상태 갱신
					update_assign_button();
				}
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 키할당解除
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button2_Click(object sender, EventArgs e)
		{
			KeyAssignList.Assign	i		= get_selected_item();

			if(i == null)		return;

			// 할당없음
			i.Keys		= Keys.None;

			// 목록 갱신
			m_list_view.SelectedItems[0].SubItems[2].Text	= i.KeysString;

			// 버튼 상태 갱신
			update_assign_button();
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 선택한 아이템을 얻는다
		/// </summary>
		/// <returns></returns>
		private KeyAssignList.Assign get_selected_item()
		{
			if(m_list_view.SelectedItems.Count <= 0)	return null;

			ListViewItem				item	= m_list_view.SelectedItems[0];
			return item.Tag as KeyAssignList.Assign;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 전부 초기값으로 되돌린다
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button3_Click(object sender, EventArgs e)
		{
			// 초기값으로 되돌린다
			m_assign_list.DefaultAll();

			foreach(ListViewItem i in m_list_view.Items){
				KeyAssignList.Assign	item	= i.Tag as KeyAssignList.Assign;
				if(item == null)	continue;

				i.SubItems[2].Text	= item.KeysString;
			}
		}
	}
}
