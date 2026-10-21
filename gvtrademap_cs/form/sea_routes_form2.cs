/*-------------------------------------------------------------------------

 항로도목록
 모드리스 대화상자로 사용할 것

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
		private gvt_lib m_lib;				  // 자주 쓰는 기능을 모아 놓은 것
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

			// View 와 DB 를 연결해 둔다
			m_view1 = new list_view_db(listView1, m_db.SeaRoute.searoutes);
			m_view2 = new list_view_db(listView2, m_db.SeaRoute.favorite_sea_routes);
			m_view3 = new list_view_db(listView3, m_db.SeaRoute.trash_sea_routes);

			// 각 페이지 초기화
			init_page1();
			init_page2();
			init_page3();
		}

		/*-------------------------------------------------------------------------
		 페이지 초기화
		---------------------------------------------------------------------------*/
		private void init_page1() {
			// 헤더 컬럼을 추가해 둔다
			// 컬럼 크기는 기억해 둘 필요가 있다
			listView1.Columns.Add("표시", 50);
			listView1.Columns.Add("시작 위치", 160);
			listView1.Columns.Add("종료 위치", 160);
			listView1.Columns.Add("항해 일수", 60);
			listView1.Columns.Add("항해 시간", 170);
		}

		/*-------------------------------------------------------------------------
		 페이지 초기화
		---------------------------------------------------------------------------*/
		private void init_page2() {
			// 헤더 컬럼을 추가해 둔다
			// 컬럼 크기는 기억해 둘 필요가 있다
			listView2.Columns.Add("표시", 50);
			listView2.Columns.Add("시작 위치", 160);
			listView2.Columns.Add("종료 위치", 160);
			listView2.Columns.Add("항해 일수", 60);
			listView2.Columns.Add("항해 시간", 170);
		}

		/*-------------------------------------------------------------------------
		 페이지 초기화
		---------------------------------------------------------------------------*/
		private void init_page3() {
			// 헤더 컬럼을 추가해 둔다
			// 컬럼 크기는 기억해 둘 필요가 있다
			listView3.Columns.Add("표시", 50);
			listView3.Columns.Add("시작 위치", 160);
			listView3.Columns.Add("종료 위치", 160);
			listView3.Columns.Add("항해 일수", 60);
			listView3.Columns.Add("항해 시간", 170);
		}

		/*-------------------------------------------------------------------------
		 ESC 가 눌렸을 때의 닫기 동작
		---------------------------------------------------------------------------*/
		private void button1_Click(object sender, EventArgs e) {
			this.Hide();		// 표시를 지운다
		}

		/*-------------------------------------------------------------------------
		 닫히려고 하고 있다
		---------------------------------------------------------------------------*/
		private void sea_routes_form2_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing) {
				// 이 폼의 x 버튼이 눌렸을 때만 취소한다
				// Alt + F4 로도 여기를 지난다
				e.Cancel = true;
				this.Hide();	// 비표시로 만든다

				// 선택 상태를 리셋한다
				m_db.SeaRoute.ResetSelectFlag();
			}
		}

		/*-------------------------------------------------------------------------
		 처음 표시되었을 때
		---------------------------------------------------------------------------*/
		private void sea_routes_form2_Shown(object sender, EventArgs e) {
			UpdateAllList();
		}

		/*-------------------------------------------------------------------------
		 전체 목록 내용을 갱신한다
		 목록의 개수가 바뀐 경우
		---------------------------------------------------------------------------*/
		public void UpdateAllList() {
			update_sea_routes_list();
			update_favorite_sea_routes_list();
			update_trash_sea_routes_list();
		}

		/*-------------------------------------------------------------------------
		 최신 항로도의 내용을 갱신한다
		 변경이 있었을 만한 부분만 다시 그리기
		 전체를 다시 그리면 깜빡이기 때문
		---------------------------------------------------------------------------*/
		public void RedrawNewestSeaRoutes() {
			// 항로도 목록 이외를 갱신할 필요가 지금은 없다
			switch (tabControl1.SelectedIndex) {
				case 0: {
						// 최신 항로도의 내용을 갱신한다
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
		 항로도 목록의 내용을 갱신한다
		---------------------------------------------------------------------------*/
		private void update_sea_routes_list() {
			tabPage1.Text = String.Format("항로도({0})", m_db.SeaRoute.searoutes.Count);
			update_list_count(m_view1);
		}

		/*-------------------------------------------------------------------------
		 즐겨찾기 항로도 목록의 내용을 갱신한다
		---------------------------------------------------------------------------*/
		private void update_favorite_sea_routes_list() {
			tabPage2.Text = String.Format("즐겨찾기 항로도({0})", m_db.SeaRoute.favorite_sea_routes.Count);
			update_list_count(m_view2);
		}

		/*-------------------------------------------------------------------------
		 휴지통 항로도 목록의 내용을 갱신한다
		---------------------------------------------------------------------------*/
		private void update_trash_sea_routes_list() {
			tabPage3.Text = String.Format("과거 항로도(숨김)({0})", m_db.SeaRoute.trash_sea_routes.Count);
			update_list_count(m_view3);
		}

		/*-------------------------------------------------------------------------
		 목록의 개수를 갱신한다
		 맨 위에 표시되어 있던 아이템을 그리기 범위 안으로 스크롤시키는 기능 포함
		---------------------------------------------------------------------------*/
		private void update_list_count(list_view_db view) {
			ListViewItem item = view.view.TopItem;
			int index = -1;
			if (item != null) index = item.Index;

			// 맨 아래까지 스크롤한 상태에서 항목 수를 줄이면 예외가 발생한다
			// ListView 의 버그?
			view.view.VirtualListSize = 0;			  // 예외 대책
			view.view.VirtualListSize = view.db.Count;  // 새 항목 수

			/*			// 다시 그리기
						if(   (view.view.Items.Count > 0)
							&&(Index > 0) ){
							// 한 번 맨 아래까지 스크롤시킨다
							view.view.EnsureVisible(view.view.Items.Count - 1);
							// 목적한 아이템을 맨 위로 올린다
							// 맨 위가 무리라면 적당한 위치로 스크롤한다
							view.view.EnsureVisible(Index);
						}
			*/

			/*			int Count	= m_view1.view.Items.Count;
						if(Count > 0){
							Rectangle	rect	= view.view.GetItemRect(Count - 1);	// 가장 새로운 아이템의 Rect
							view.view.Invalidate(view.view.RectangleToScreen(rect));
						}
			*/
			// 맨 아래가 보이는 위치로 스크롤시킨다
			// 맨 아래의 정보가 최신인 것
			if (view.view.Items.Count > 0) {
				// 한 번 맨 아래까지 스크롤시킨다
				view.view.EnsureVisible(view.view.Items.Count - 1);
			}
		}

		/*-------------------------------------------------------------------------
		 항로 목록이 선택되었다
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
		 항로 목록이 선택되었다
		 sub
		---------------------------------------------------------------------------*/
		private void selected_index_changed(list_view_db view) {
			if (m_disable_update_select) return;

			// 선택 상태를 리셋한다
			m_db.SeaRoute.ResetSelectFlag();

			if (view.view.SelectedIndices.Count < 1) return;

			// 최초 항목을 센터링한다
			SeaRoutes.Voyage i = get_route(view.db, view.view.SelectedIndices[0]);
			if (i != null) {
				if (i.GamePoint1st.X < 0) return;
				if (i.GamePoint1st.Y < 0) return;
				m_lib.setting.centering_gpos = i.GamePoint1st;
				m_lib.setting.req_centering_gpos.Request();
			}

			// 선택 상태로 만든다
			foreach (int index in view.view.SelectedIndices) {
				SeaRoutes.Voyage ii = get_route(view.db, index);
				if (ii == null) continue;
				ii.IsSelected = true;
			}
		}

		/*-------------------------------------------------------------------------
		 지정된 번호의 항로도를 얻는다
		 범위 밖일 때는 null 을 반환한다
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
			// 선택 상태를 리셋한다
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
		 전체 선택 상태로 만든다
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
		 전체 선택 상태로 만든다
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
		 선택 상태를 해제한다
		---------------------------------------------------------------------------*/
		private void button3_Click(object sender, EventArgs e) {
			// 선택 상태를 리셋한다
			m_db.SeaRoute.ResetSelectFlag();

			m_disable_update_select = true;
			listView1.SelectedIndices.Clear();
			listView2.SelectedIndices.Clear();
			listView3.SelectedIndices.Clear();
			m_disable_update_select = false;
		}

		/*-------------------------------------------------------------------------
		 삭제
		 선택되어 있는 항로도를 삭제한다
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
			if (MessageBox.Show("선택한 항로도를 삭제합니다. \n삭제하면 복구할 수 없습니다. \n계속할까요?",
								"항로도 삭제 확인",
								MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.No) return false;
			return true;
		}

		/*-------------------------------------------------------------------------
		 선택되어 있는 항로도 목록을 얻는다
		 목록이 없을 때는 null 을 반환한다
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
		 항로도에서 즐겨찾기로 이동
		---------------------------------------------------------------------------*/
		private void add_AToolStripMenuItem_Click(object sender, EventArgs e) {
			List<SeaRoutes.Voyage> list = get_selected_routes_list(m_view1);
			if (list == null) return;
			m_db.SeaRoute.MoveSeaRoutesToFavoriteSeaRoutes(list);
			m_db.SeaRoute.ResetSelectFlag();
			UpdateAllList();
		}

		/*-------------------------------------------------------------------------
		 항로도에서 과거 항로도로 이동
		---------------------------------------------------------------------------*/
		private void delete_DToolStripMenuItem_Click(object sender, EventArgs e) {
			List<SeaRoutes.Voyage> list = get_selected_routes_list(m_view1);
			if (list == null) return;
			m_db.SeaRoute.MoveSeaRoutesToTrashSeaRoutes(list);
			m_db.SeaRoute.ResetSelectFlag();
			UpdateAllList();
		}

		/*-------------------------------------------------------------------------
		 즐겨찾기에서 과거 항로도로 이동
		---------------------------------------------------------------------------*/
		private void toolStripMenuItem3_Click(object sender, EventArgs e) {
			List<SeaRoutes.Voyage> list = get_selected_routes_list(m_view2);
			if (list == null) return;
			m_db.SeaRoute.MoveFavoriteSeaRoutesToTrashSeaRoutes(list);
			m_db.SeaRoute.ResetSelectFlag();
			UpdateAllList();
		}

		/*-------------------------------------------------------------------------
		 과거 항로도에서 즐겨찾기로 이동
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
		 아이템을 작성한다
		---------------------------------------------------------------------------*/
		private ListViewItem create_item(SeaRoutes.Voyage i, bool is_draw_show_flag) {
			GvoWorldInfo.Info info1 = m_db.World.FindInfo_WithoutSea(transform.ToPoint(i.MapPoint1st));
			string _1st_name = (info1 != null) ? info1.Name : "";
			GvoWorldInfo.Info info2 = m_db.World.FindInfo_WithoutSea(transform.ToPoint(i.MapPointLast));
			string last_name = (info2 != null) ? info2.Name : "";

			string show_str = (i.IsEnableDraw) ? "표시" : "숨김";
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
		 listView 와 db
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
		 메뉴의 유효와 무효
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
		 메뉴의 유효와 무효
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
