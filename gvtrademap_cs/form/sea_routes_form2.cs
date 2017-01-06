/*-------------------------------------------------------------------------

 항로도목록
 모드レスダイア로그として사용すること

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using directx;
using gvo_base;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Utility;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public partial class sea_routes_form2 : Form {
		private gvt_lib m_lib;				  // よく使う기능をまとめたもの
		private GvoDatabase m_db;				   // DB

		private bool m_disable_update_select;

		private list_view_db m_view1;
		private list_view_db m_view2;
		private list_view_db m_view3;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public sea_routes_form2(gvt_lib lib, GvoDatabase db) {
			m_lib = lib;
			m_db = db;

			InitializeComponent();

			Useful.SetFontMeiryo(this, def.MEIRYO_POINT);

			m_disable_update_select = false;

			// tooltip
			toolTip1.AutoPopDelay = 30 * 1000;	  // 30초표시
			toolTip1.BackColor = Color.LightYellow;

			// ViewとDBを関連付けておく
			m_view1 = new list_view_db(listView1, m_db.SeaRoute.searoutes);
			m_view2 = new list_view_db(listView2, m_db.SeaRoute.favorite_sea_routes);
			m_view3 = new list_view_db(listView3, m_db.SeaRoute.trash_sea_routes);

			// 各ページの初期化
			init_page1();
			init_page2();
			init_page3();
		}

		/*-------------------------------------------------------------------------
		 ページの初期化
		---------------------------------------------------------------------------*/
		private void init_page1() {
			// ヘッダのコラムを추가しておく
			// コラムの사이즈は覚えておく必要がある
			listView1.Columns.Add("표시", 50);
			listView1.Columns.Add("시작위치", 160);
			listView1.Columns.Add("종료위치", 160);
			listView1.Columns.Add("항해일수", 60);
			listView1.Columns.Add("항해시간", 170);
		}

		/*-------------------------------------------------------------------------
		 ページの初期化
		---------------------------------------------------------------------------*/
		private void init_page2() {
			// ヘッダのコラムを추가しておく
			// コラムの사이즈は覚えておく必要がある
			listView2.Columns.Add("표시", 50);
			listView2.Columns.Add("시작위치", 160);
			listView2.Columns.Add("종료위치", 160);
			listView2.Columns.Add("항해일수", 60);
			listView2.Columns.Add("항해시간", 170);
		}

		/*-------------------------------------------------------------------------
		 ページの初期化
		---------------------------------------------------------------------------*/
		private void init_page3() {
			// ヘッダのコラムを추가しておく
			// コラムの사이즈は覚えておく必要がある
			listView3.Columns.Add("표시", 50);
			listView3.Columns.Add("시작위치", 160);
			listView3.Columns.Add("종료위치", 160);
			listView3.Columns.Add("항해일수", 60);
			listView3.Columns.Add("항해시간", 170);
		}

		/*-------------------------------------------------------------------------
		 ESCが押されたときの닫기動作
		---------------------------------------------------------------------------*/
		private void button1_Click(object sender, EventArgs e) {
			this.Hide();		// 표시を消す
		}

		/*-------------------------------------------------------------------------
		 閉じられようとしている
		---------------------------------------------------------------------------*/
		private void sea_routes_form2_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing) {
				// このフォームの x ボタンが押されたときのみ취소する
				// Alt + F4 でもここを通る
				e.Cancel = true;
				this.Hide();	// 비표시にする

				// 선택상태を리셋する
				m_db.SeaRoute.ResetSelectFlag();
			}
		}

		/*-------------------------------------------------------------------------
		 최초に표시されたとき
		---------------------------------------------------------------------------*/
		private void sea_routes_form2_Shown(object sender, EventArgs e) {
			UpdateAllList();
		}

		/*-------------------------------------------------------------------------
		 전체목록내용を업데이트する
		 목록の수が変わった場合
		---------------------------------------------------------------------------*/
		public void UpdateAllList() {
			update_sea_routes_list();
			update_favorite_sea_routes_list();
			update_trash_sea_routes_list();
		}

		/*-------------------------------------------------------------------------
		 최신の항로도の내용を업데이트する
		 변경があったであろう箇所のみ다시그리기
		 전체を다시그리기するとちらつくため
		---------------------------------------------------------------------------*/
		public void RedrawNewestSeaRoutes() {
			// 항로도목록以외を업데이트する必要が今のところない
			switch (tabControl1.SelectedIndex) {
				case 0: {
						// 최신の항로도の내용を업데이트する
						int count = m_view1.view.Items.Count;
						if (count <= 0) return;
						Rectangle rect = m_view1.view.GetItemRect(count - 1);
						m_view1.view.Invalidate(rect);
						m_view1.view.Update();
					}
					break;
				case 1:
				case 2:
					break;
			}
		}

		/*-------------------------------------------------------------------------
		 항로도목록목록내용を업데이트する
		---------------------------------------------------------------------------*/
		private void update_sea_routes_list() {
			tabPage1.Text = String.Format("항로도({0})", m_db.SeaRoute.searoutes.Count);
			update_list_count(m_view1);
		}

		/*-------------------------------------------------------------------------
		 즐겨찾기항로도목록목록내용を업데이트する
		---------------------------------------------------------------------------*/
		private void update_favorite_sea_routes_list() {
			tabPage2.Text = String.Format("항로도즐겨찾기({0})", m_db.SeaRoute.favorite_sea_routes.Count);
			update_list_count(m_view2);
		}

		/*-------------------------------------------------------------------------
		 ごみ箱항로도목록목록내용を업데이트する
		---------------------------------------------------------------------------*/
		private void update_trash_sea_routes_list() {
			tabPage3.Text = String.Format("과거의항로도(비표시)({0})", m_db.SeaRoute.trash_sea_routes.Count);
			update_list_count(m_view3);
		}

		/*-------------------------------------------------------------------------
		 목록の수を업데이트する
		 一番상に표시されていた아이템を그리기범위내にスクロールさせる기능付き
		---------------------------------------------------------------------------*/
		private void update_list_count(list_view_db view) {
			ListViewItem item = view.view.TopItem;
			int index = -1;
			if (item != null) index = item.Index;

			// 一番下までスクロールした상태で항목수を減らすと例외がでる
			// ListViewのバグ?
			view.view.VirtualListSize = 0;			  // 例외対策
			view.view.VirtualListSize = view.db.Count;  // 新しい항목수

			/*			// 見える위치にスクロールさせる
						if(   (view.view.Items.Count > 0)
							&&(Index > 0) ){
							// 一도一番下までスクロールさせる
							view.view.EnsureVisible(view.view.Items.Count - 1);
							// 目的の아이템を一番상にする
							// 一番상が무리なら적당な위치にスクロールする
							view.view.EnsureVisible(Index);
						}
			*/

			/*			int Count	= m_view1.view.Items.Count;
						if(Count > 0){
							Rectangle	rect	= view.view.GetItemRect(Count - 1);	// もっとも新しい아이템のRect
							view.view.Invalidate(view.view.RectangleToScreen(rect));
						}
			*/
			// 一番下が見える위치にスクロールさせる	
			// 一番下の정보が최신のもの
			if (view.view.Items.Count > 0) {
				// 一도一番下までスクロールさせる
				view.view.EnsureVisible(view.view.Items.Count - 1);
			}
		}

		/*-------------------------------------------------------------------------
		 항로목록が선택한
		---------------------------------------------------------------------------*/
		private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
			selected_index_changed(m_view1);
		}
		private void listView2_SelectedIndexChanged(object sender, EventArgs e) {
			selected_index_changed(m_view2);
		}
		private void listView3_SelectedIndexChanged(object sender, EventArgs e) {
			selected_index_changed(m_view3);
		}

		/*-------------------------------------------------------------------------
		 항로목록が선택한
		 sub
		---------------------------------------------------------------------------*/
		private void selected_index_changed(list_view_db view) {
			if (m_disable_update_select) return;

			// 선택상태を리셋する
			m_db.SeaRoute.ResetSelectFlag();

			if (view.view.SelectedIndices.Count < 1) return;

			// 최초の항목をセンタリングする
			SeaRoutes.Voyage i = get_route(view.db, view.view.SelectedIndices[0]);
			if (i != null) {
				if (i.GamePoint1st.X < 0) return;
				if (i.GamePoint1st.Y < 0) return;
				m_lib.setting.centering_gpos = i.GamePoint1st;
				m_lib.setting.req_centering_gpos.Request();
			}

			// 선택상태にする
			foreach (int index in view.view.SelectedIndices) {
				SeaRoutes.Voyage ii = get_route(view.db, index);
				if (ii == null) continue;
				ii.IsSelected = true;
			}
		}

		/*-------------------------------------------------------------------------
		 지정된번호の항로도を得る
		 범위외のときはnullを返す
		---------------------------------------------------------------------------*/
		private SeaRoutes.Voyage get_route(List<SeaRoutes.Voyage> list, int index) {
			if (list == null) return null;
			if (index < 0) return null;
			if (index >= list.Count) return null;
			return list[index];
		}

		/*-------------------------------------------------------------------------
		 표시
		---------------------------------------------------------------------------*/
		private void show_hide_SToolStripMenuItem_Click(object sender, EventArgs e) {
			set_draw_flag(m_view1, true);
			update_sea_routes_list();
		}
		private void toolStripMenuItem7_Click(object sender, EventArgs e) {
			set_draw_flag(m_view2, true);
			update_favorite_sea_routes_list();
		}

		/*-------------------------------------------------------------------------
		 비표시
		---------------------------------------------------------------------------*/
		private void hide_ToolStripMenuItem_Click(object sender, EventArgs e) {
			set_draw_flag(m_view1, false);
			update_sea_routes_list();
		}
		private void toolStripMenuItem2_Click(object sender, EventArgs e) {
			set_draw_flag(m_view2, false);
			update_favorite_sea_routes_list();
		}

		/*-------------------------------------------------------------------------
		 표시, 비표시
		 sub
		---------------------------------------------------------------------------*/
		private void set_draw_flag(list_view_db view, bool is_show) {
			// 선택상태を리셋する
			m_db.SeaRoute.ResetSelectFlag();

			if (view.view.SelectedIndices.Count < 1) return;

			m_disable_update_select = true;
			foreach (int index in view.view.SelectedIndices) {
				SeaRoutes.Voyage ii = get_route(view.db, index);
				if (ii == null) continue;
				ii.IsEnableDraw = is_show;
			}
			m_disable_update_select = false;
		}

		/*-------------------------------------------------------------------------
		 전체선택상태にする
		---------------------------------------------------------------------------*/
		private void all_select_AToolStripMenuItem_Click(object sender, EventArgs e) {
			select_all(m_view1);
		}
		private void toolStripMenuItem5_Click(object sender, EventArgs e) {
			select_all(m_view2);
		}
		private void toolStripMenuItem9_Click(object sender, EventArgs e) {
			select_all(m_view3);
		}

		/*-------------------------------------------------------------------------
		 전체선택상태にする
		 sub
		---------------------------------------------------------------------------*/
		private void select_all(list_view_db view) {
			m_disable_update_select = true;
			for (int i = 0; i < view.db.Count; i++) {
				view.view.SelectedIndices.Add(i);
			}
			m_disable_update_select = false;
			selected_index_changed(view);
		}

		/*-------------------------------------------------------------------------
		 선택상태を해제する
		---------------------------------------------------------------------------*/
		private void button3_Click(object sender, EventArgs e) {
			// 선택상태を리셋する
			m_db.SeaRoute.ResetSelectFlag();

			m_disable_update_select = true;
			listView1.SelectedIndices.Clear();
			listView2.SelectedIndices.Clear();
			listView3.SelectedIndices.Clear();
			m_disable_update_select = false;
		}

		/*-------------------------------------------------------------------------
		 삭제
		 선택されている항로도を삭제する
		---------------------------------------------------------------------------*/
		private void move_trash_RToolStripMenuItem_Click(object sender, EventArgs e) {
			List<SeaRoutes.Voyage> list = get_selected_routes_list(m_view1);
			if (list == null) return;
			if (!ask_remove()) return;

			m_db.SeaRoute.RemoveSeaRoutes(list);
			m_db.SeaRoute.ResetSelectFlag();
			update_sea_routes_list();
		}
		private void toolStripMenuItem4_Click(object sender, EventArgs e) {
			List<SeaRoutes.Voyage> list = get_selected_routes_list(m_view2);
			if (list == null) return;
			if (!ask_remove()) return;

			m_db.SeaRoute.RemoveFavoriteSeaRoutes(list);
			m_db.SeaRoute.ResetSelectFlag();
			update_favorite_sea_routes_list();
		}
		private void toolStripMenuItem8_Click(object sender, EventArgs e) {
			List<SeaRoutes.Voyage> list = get_selected_routes_list(m_view3);
			if (list == null) return;
			if (!ask_remove()) return;

			m_db.SeaRoute.RemoveTrashSeaRoutes(list);
			m_db.SeaRoute.ResetSelectFlag();
			update_trash_sea_routes_list();
		}

		/*-------------------------------------------------------------------------
		 삭제확인
		---------------------------------------------------------------------------*/
		private bool ask_remove() {
			if (MessageBox.Show("선택한항로도를 삭제합니다. \n삭제하면 복구가 불가능합니다. \n괜찮겠습니까?",
								"항로도삭제확인",
								MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.No) return false;
			return true;
		}

		/*-------------------------------------------------------------------------
		 선택されている항로도목록を得る
		 목록がないときはnullを返す
		---------------------------------------------------------------------------*/
		private List<SeaRoutes.Voyage> get_selected_routes_list(list_view_db view) {
			if (view.view.SelectedIndices.Count < 1) return null;

			List<SeaRoutes.Voyage> list = new List<SeaRoutes.Voyage>();
			foreach (int index in view.view.SelectedIndices) {
				SeaRoutes.Voyage ii = get_route(view.db, index);
				if (ii == null) continue;
				list.Add(ii);
			}
			if (list.Count <= 0) return null;
			return list;
		}

		/*-------------------------------------------------------------------------
		 항로도から즐겨찾기に이동
		---------------------------------------------------------------------------*/
		private void add_AToolStripMenuItem_Click(object sender, EventArgs e) {
			List<SeaRoutes.Voyage> list = get_selected_routes_list(m_view1);
			if (list == null) return;
			m_db.SeaRoute.MoveSeaRoutesToFavoriteSeaRoutes(list);
			m_db.SeaRoute.ResetSelectFlag();
			UpdateAllList();
		}

		/*-------------------------------------------------------------------------
		 항로도から과거의항로도に이동
		---------------------------------------------------------------------------*/
		private void delete_DToolStripMenuItem_Click(object sender, EventArgs e) {
			List<SeaRoutes.Voyage> list = get_selected_routes_list(m_view1);
			if (list == null) return;
			m_db.SeaRoute.MoveSeaRoutesToTrashSeaRoutes(list);
			m_db.SeaRoute.ResetSelectFlag();
			UpdateAllList();
		}

		/*-------------------------------------------------------------------------
		 즐겨찾기から과거의항로도に이동
		---------------------------------------------------------------------------*/
		private void toolStripMenuItem3_Click(object sender, EventArgs e) {
			List<SeaRoutes.Voyage> list = get_selected_routes_list(m_view2);
			if (list == null) return;
			m_db.SeaRoute.MoveFavoriteSeaRoutesToTrashSeaRoutes(list);
			m_db.SeaRoute.ResetSelectFlag();
			UpdateAllList();
		}

		/*-------------------------------------------------------------------------
		 과거의항로도から즐겨찾기に이동
		---------------------------------------------------------------------------*/
		private void toolStripMenuItem6_Click(object sender, EventArgs e) {
			List<SeaRoutes.Voyage> list = get_selected_routes_list(m_view3);
			if (list == null) return;
			m_db.SeaRoute.MoveTrashSeaRoutesToFavoriteSeaRoutes(list);
			m_db.SeaRoute.ResetSelectFlag();
			UpdateAllList();
		}

		/*-------------------------------------------------------------------------
		 항목설정
		---------------------------------------------------------------------------*/
		private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
			set_item(m_view1, e);
		}
		private void listView2_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
			set_item(m_view2, e);
		}
		private void listView3_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) {
			set_item(m_view3, e);
		}

		/*-------------------------------------------------------------------------
		 항목설정
		---------------------------------------------------------------------------*/
		private void set_item(list_view_db view, RetrieveVirtualItemEventArgs e) {
			SeaRoutes.Voyage ii = get_route(view.db, e.ItemIndex);
			if (ii == null) {
				ListViewItem item = new ListViewItem("---", 0);
				item.SubItems.Add("---");
				item.SubItems.Add("---");
				item.SubItems.Add("---");
				item.SubItems.Add("---");
				e.Item = item;
			} else {
				e.Item = create_item(ii, view != m_view3);
			}
		}

		/*-------------------------------------------------------------------------
		 아이템を작성함
		---------------------------------------------------------------------------*/
		private ListViewItem create_item(SeaRoutes.Voyage i, bool is_draw_show_flag) {
			GvoWorldInfo.Info info1 = m_db.World.FindInfo_WithoutSea(transform.ToPoint(i.MapPoint1st));
			string _1st_name = (info1 != null) ? info1.Name : "";
			GvoWorldInfo.Info info2 = m_db.World.FindInfo_WithoutSea(transform.ToPoint(i.MapPointLast));
			string last_name = (info2 != null) ? info2.Name : "";

			string show_str = (i.IsEnableDraw) ? "표시" : "비표시";
			if (!is_draw_show_flag) show_str = "---";

			ListViewItem item = new ListViewItem(show_str, 0);
			item.UseItemStyleForSubItems = false;
			item.Tag = i;
			//			item.ToolTipText				= i.TooltipString;
			item.SubItems.Add(_1st_name + "(" + i.GamePoint1stStr + ")");
			item.SubItems.Add(last_name + "(" + i.GamePointLastString + ")");
			item.SubItems.Add(i.MaxDaysString);
			item.SubItems.Add(i.DateTimeString);

			if (is_draw_show_flag) {
				item.SubItems[0].ForeColor = (i.IsEnableDraw) ? Color.Blue : Color.Red;
			}
			return item;
		}

		/*-------------------------------------------------------------------------
		 listViewとdb
		---------------------------------------------------------------------------*/
		private class list_view_db {
			private ListView m_list_view;
			private List<SeaRoutes.Voyage> m_db_list;

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public ListView view { get { return m_list_view; } }
			public List<SeaRoutes.Voyage> db { get { return m_db_list; } }

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public list_view_db(ListView view, List<SeaRoutes.Voyage> db) {
				m_list_view = view;
				m_db_list = db;
			}
		}

		/*-------------------------------------------------------------------------
		 メニューの유효と무효
		---------------------------------------------------------------------------*/
		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e) {
			if (!set_context_menu_state(contextMenuStrip1, listView1, 7)) e.Cancel = true;
		}
		private void contextMenuStrip2_Opening(object sender, CancelEventArgs e) {
			if (!set_context_menu_state(contextMenuStrip2, listView2, 6)) e.Cancel = true;
		}
		private void contextMenuStrip3_Opening(object sender, CancelEventArgs e) {
			if (!set_context_menu_state(contextMenuStrip3, listView3, 3)) e.Cancel = true;
		}

		/*-------------------------------------------------------------------------
		 メニューの유효と무효
		 sub
		---------------------------------------------------------------------------*/
		private bool set_context_menu_state(ContextMenuStrip menu, ListView list_view, int all_select_index) {
			if (list_view.Items.Count <= 0) return false;   // 비표시

			if (list_view.SelectedIndices.Count <= 0) {
				foreach (ToolStripItem i in menu.Items) {
					i.Enabled = false;
				}
			} else {
				foreach (ToolStripItem i in menu.Items) {
					i.Enabled = true;
				}
			}
			// 전체선택
			if (list_view.Items.Count > 0) {
				if (menu.Items.Count > all_select_index) {
					menu.Items[all_select_index].Enabled = true;
				}
			}

			return true;	// 표시
		}

		private void sea_routes_form2_Load(object sender, EventArgs e) {

		}

		private void label1_Click(object sender, EventArgs e) {

		}
	}
}
