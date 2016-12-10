//-------------------------------------------------------------------------
// キーアサインヘルパ
// KeyAssignFormの動作내용
// ダイア로그にKeyAssignFormの항목を埋め込んだときに사용する
// ComboBox	그룹선택用
// Button		割り当て변경用
// Button		割り当て解除用
// Button		初期割り当てに戻す
// ListView	割り当て목록
// の5つのコントロールを渡す
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows.Forms;

//-------------------------------------------------------------------------
namespace Utility.KeyAssign
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// キーアサインヘルパ. 
	/// KeyAssignFormの動作내용. 
	/// ダイア로그にKeyAssignFormの항목を埋め込んだときに사용する. 
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
		/// OKボタンが押されたときの변경내용
		/// </summary>
		public KeyAssignList List				{	get{	return m_assign_list;	}}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 構築
		/// </summary>
		/// <param name="assign_list">必須, 複製して保持される</param>
		/// <param name="form">설정用フォーム, アサイン用ダイア로그표시時に参照される. 必須</param>
		/// <param name="cbox">그룹선택用, nullでもよい</param>
		/// <param name="list_view">アサイン목록표시用ListView, 必須</param>
		/// <param name="assign_button">アサインボタン, 必須</param>
		/// <param name="remove_assign_button">アサイン삭제ボタン, nullでもよい</param>
		/// <param name="default_all_assign_button">전부を初期値に戻すボタン, nullでもよい</param>
		public KeyAssiginSettingHelper(
					KeyAssignList assign_list,
					Form form,
					ComboBox cbox,
					ListView list_view,
					Button assign_button,
					Button remove_assign_button,
					Button default_all_assign_button)
		{
			m_assign_list				= assign_list.DeepClone();	// 목록をコピーして持つ

			m_form						= form;
			m_select_group_cbox			= cbox;
			m_list_view					= list_view;
			m_assign_button				= assign_button;
			m_remove_assign_button		= remove_assign_button;
			m_default_all_assign_button	= default_all_assign_button;

			// コントロールの初期化
			init_ctrl();

			// 初期化
			init();

			// 割り当てボタンの更新
			update_assign_button();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// コントロールの初期化
		/// </summary>
		private void init_ctrl()
		{
			// 그룹선택
			if(m_select_group_cbox != null){
				m_select_group_cbox.DropDownStyle			= System.Windows.Forms.ComboBoxStyle.DropDownList;
				m_select_group_cbox.FormattingEnabled		= true;
				m_select_group_cbox.SelectedIndexChanged	+= new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			}

			// 割り当て변경ボタン
			if(m_assign_button != null){
				m_assign_button.Click += new System.EventHandler(this.button1_Click);
			}

			// 割り当て解除ボタン
			if(m_remove_assign_button != null){
				m_remove_assign_button.Click += new System.EventHandler(this.button2_Click);
			}
	
			// 全て初期化ボタン
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
			m_list_view.Columns.Add("割り当て", 100);

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
		/// 목록を更新する
		/// </summary>
		private void update_list()
		{
			m_list_view.BeginUpdate();
			m_list_view.Items.Clear();

			// 그룹선택状況により목록내용を決める
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

			// 割り当てボタンの更新
			update_assign_button();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// ListViewに追加する
		/// </summary>
		/// <param name="listview">対象のListView</param>
		/// <param name="i">キーアサイン</param>
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
		/// 割り当て변경ボタンの更新
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
		/// ボタンの유효, 무효の更新. 
		/// </summary>
		/// <param name="button">ボタン</param>
		/// <param name="enable">유효にするときtrue</param>
		private void update_button(Button button, bool enable)
		{
			if(button == null)				return;
			if(button.Enabled != enable)	button.Enabled	= enable;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 선택が변경された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			// 割り当てボタンの更新
			update_assign_button();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// ダブルクリックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listView1_DoubleClick(object sender, EventArgs e)
		{
			button1_Click(sender, e);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 그룹선택が변경された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			update_list();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// キー割り当て변경
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

					// 목록の更新
					m_list_view.SelectedItems[0].SubItems[2].Text	= i.KeysString;

					// ボタン상태の更新
					update_assign_button();
				}
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// キー割り当て解除
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button2_Click(object sender, EventArgs e)
		{
			KeyAssignList.Assign	i		= get_selected_item();

			if(i == null)		return;

			// 割り当てなし
			i.Keys		= Keys.None;

			// 목록の更新
			m_list_view.SelectedItems[0].SubItems[2].Text	= i.KeysString;

			// ボタン상태の更新
			update_assign_button();
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 선택한아이템を得る
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
		/// 전부初期値に戻す
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button3_Click(object sender, EventArgs e)
		{
			// 初期値に戻す
			m_assign_list.DefaultAll();

			foreach(ListViewItem i in m_list_view.Items){
				KeyAssignList.Assign	item	= i.Tag as KeyAssignList.Assign;
				if(item == null)	continue;

				i.SubItems[2].Text	= item.KeysString;
			}
		}
	}
}
