/*-------------------------------------------------------------------------

 왼쪽 윈도우
 아이템 정보를 그린다

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Collections.Generic;
using System;
using System.Windows.Forms;

using directx;
using Utility;
using Utility.KeyAssign;
using System.Diagnostics;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class item_window : d3d_windows.window {
		// 위치와 크기(크기는 자동으로 조정된다)
		private const int WINDOW_POS_X = 3;
		private const int WINDOW_POS_Y = 3;
		private const float WINDOW_POS_Z = 0.2f;
		private const int WINDOW_SIZE_X = 250;  // 초기 크기
		private const int WINDOW_SIZE_Y = 200;  // 초기 크기

		// 배치 간격
		private const int TABS_STEP_X = def.ICON_SIZE_X + 1;
		private const int ICONS_STEP_X = def.ICON_SIZE_X + 4;
		// 기본적인 세로 방향 간격
		private const int STEP_Y0 = def.ICON_SIZE_Y + 2;	// 상부의 아이콘용
		private const int STEP_Y = def.ICON_SIZE_Y + 4;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private gvt_lib m_lib;					  // 
		private GvoDatabase m_db;					   // DB
		private spot m_spot;						// 장소 표시
		private gvtrademap_cs_form m_form;					  // 우클릭 메뉴 전용

		private GvoWorldInfo.Info m_info;					   // 그리기대상
		private const int TAB_STAFF = 11;		// 주점 종업원 탭
		private const int TAB_ADJUTANT = 12;	// 부관 탭
		private const int TAB_MEMO = 13;		// 메모 탭
		private const int TAB_MAX = 14;

		private int m_tab_index;				// 현재 탭 위치

		private hittest_list m_hittest_list;				// 사각형 관리
		private Vector2 m_window_size;			  // 윈도우사이즈

		private TextBox m_memo_text_box;			// 메모용 텍스트 BOX
		private ListView m_list_view;			   // 아이템 표시용 목록 뷰

		private bool m_is_1st_draw;

		private enum item_index {
			item_list,		  // 아이템목록
			country,			// 국아이콘
			icons,			  // 위쪽 아이콘
			tabs,			   // 탭
			web,				// web아이콘
			county_name,		// 국명
			lang1,			  // 언어1
			lang2,			  // 언어2
			max
		};

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public GvoWorldInfo.Info info {
			get { return m_info; }
			set { set_info(value, true); }
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public item_window(gvt_lib lib, GvoDatabase db, spot _spot, TextBox memo_text_box, ListView list_view, gvtrademap_cs_form form)
			: base(lib.device, new Vector2(WINDOW_POS_X, WINDOW_POS_Y), new Vector2(WINDOW_SIZE_X, WINDOW_SIZE_Y), WINDOW_POS_Z) {
			base.title = "아이템 창";

			m_window_size = new Vector2(0, 0);
			m_memo_text_box = memo_text_box;
			m_list_view = list_view;
			m_form = form;

			m_lib = lib;

			m_db = db;
			m_spot = _spot;

			m_tab_index = 0;
			m_info = null;

			m_is_1st_draw = true;

			// 아이템추가
			m_hittest_list = new hittest_list();

			// 아이템목록
			m_hittest_list.Add(new hittest());
			// 국아이콘
			m_hittest_list.Add(new hittest());
			// 아이콘들
			m_hittest_list.Add(new hittest());
			// 탭
			m_hittest_list.Add(new hittest());
			// web아이콘
			m_hittest_list.Add(new hittest());
			// 국명
			m_hittest_list.Add(new hittest());
			// 언어1
			m_hittest_list.Add(new hittest());
			// 언어2
			m_hittest_list.Add(new hittest());
		}

		/*-------------------------------------------------------------------------
		 info설정
		---------------------------------------------------------------------------*/
		private void set_info(GvoWorldInfo.Info _info, bool with_spot_reset) {
			if (_info == null) return;

			if (with_spot_reset) {
				// 무조건 장소 종료
				m_spot.SetSpot(spot.type.none, "");
				m_form.UpdateSpotList();
			}

			// 내용이 같은 경우에는 아무것도 하지 않는다
			if (m_info == _info) return;

			// 메모
			// 텍스트 박스의 내용을 DB 에 대피
			UpdateMemo();	   // 업데이트

			// 업데이트
			m_info = _info;
			// 메모용 텍스트 박스 갱신
			m_memo_text_box.Text = m_info.Memo;
			// 아이템목록업데이트
			update_item_list();
		}

		/*-------------------------------------------------------------------------
		 마우스 누르기 시작
		---------------------------------------------------------------------------*/
		override protected void OnMouseDownClient(Point pos, MouseButtons button) {
			if ((button & MouseButtons.Left) != 0) {
				_window_on_mouse_l_down(pos);
			} else if ((button & MouseButtons.Right) != 0) {
				_window_on_mouse_r_down(pos);
			}
		}

		/*-------------------------------------------------------------------------
		 마우스 좌클릭 누르기 시작
		---------------------------------------------------------------------------*/
		private void _window_on_mouse_l_down(Point pos) {
			switch (m_hittest_list.HitTest_Index(pos)) {
				case (int)item_index.tabs:
					on_mouse_l_click_item_tabs(pos);
					break;
				default:
					break;
			}
		}

		/*-------------------------------------------------------------------------
		 마우스 우클릭 누르기 시작
		---------------------------------------------------------------------------*/
		private void _window_on_mouse_r_down(Point pos) {
			switch (m_hittest_list.HitTest_Index(pos)) {
				case (int)item_index.tabs:
					on_mouse_l_click_item_tabs(pos);
					break;
			}
		}

		/*-------------------------------------------------------------------------
		 마우스클릭
		---------------------------------------------------------------------------*/
		override protected void OnMouseClikClient(Point pos, MouseButtons button) {
			if ((button & MouseButtons.Left) != 0) {
				_window_on_mouse_l_click(pos);
			} else if ((button & MouseButtons.Right) != 0) {
				_window_on_mouse_r_click(pos);
			}
		}

		/*-------------------------------------------------------------------------
		 마우스좌클릭
		---------------------------------------------------------------------------*/
		private void _window_on_mouse_l_click(Point pos) {
			switch (m_hittest_list.HitTest_Index(pos)) {
				case (int)item_index.country:
					on_mouse_l_click_country(pos);
					break;
				case (int)item_index.icons:
					on_mouse_l_click_icons(pos);
					break;
				case (int)item_index.county_name:
					on_mouse_l_click_country_name(pos);
					break;
				case (int)item_index.lang1:
					on_mouse_l_click_lang(pos, 0);
					break;
				case (int)item_index.lang2:
					on_mouse_l_click_lang(pos, 1);
					break;
				case (int)item_index.web:
					on_mouse_l_click_web(pos);
					break;
				default:
					break;
			}
		}

		/*-------------------------------------------------------------------------
		 마우스우클릭
		---------------------------------------------------------------------------*/
		private void _window_on_mouse_r_click(Point pos) {
			switch (m_hittest_list.HitTest_Index(pos)) {
				case (int)item_index.tabs:
					on_mouse_r_click_item_tabs(pos);
					break;
				case (int)item_index.country:
					on_mouse_r_click_country(pos);
					break;
				case (int)item_index.county_name:
					on_mouse_r_click_country_name(pos);
					break;
			}
		}

		/*-------------------------------------------------------------------------
		 마우스 휠
		---------------------------------------------------------------------------*/
		protected override void OnMouseWheelClient(Point pos, int delta) {
			switch (m_hittest_list.HitTest_Index(pos)) {
				case (int)item_index.tabs:
					on_mouse_wheel_item_tabs(pos, delta);
					break;
				default:
					break;
			}
		}

		/*-------------------------------------------------------------------------
		 업데이트
		---------------------------------------------------------------------------*/
		override protected void OnUpdateClient() {
			// 최소화 시에는 메모 윈도우를 비표시로 한다
			if (base.window_mode == d3d_windows.window.mode.small) {
				EnableMemoWindow(false);
				EnableItemWindow(false);
				return;
			}

			// 8dot 만큼의 갭을 가진다
			d3d_windows.window iw = base.FindWindow("설정 창");
			// WinForms 목록은 DPI 만큼 글자가 커지므로 너비도 함께 늘린다
			float width = ((iw != null) ? iw.normal_size.X : WINDOW_SIZE_X) * Useful.GetDpiRatio();
			base.size = new Vector2(width, base.screen_size.Y - 4 * 2);

			// 사각형을 다시 설정한다
			update_rects();

			if (base.size != m_window_size) {
				// 메모
				hittest ht = m_hittest_list[(int)item_index.item_list];
				Rectangle rect = ht.CalcRect();
				m_memo_text_box.Location = new Point(rect.Left, rect.Top);
				m_memo_text_box.Size = new Size(rect.Width, rect.Height + 1);
				m_list_view.Location = m_memo_text_box.Location;
				m_list_view.Size = m_memo_text_box.Size;
				// 컬럼 크기 조정
				ajust_item_columns_width();
			}
			update_memo_window();

			m_window_size = base.size;
		}

		/*-------------------------------------------------------------------------
		 사각형을 다시 설정한다
		---------------------------------------------------------------------------*/
		private void update_rects() {
			Point offset = transform.ToPoint(base.client_pos);

			// 오프셋 갱신
			foreach (hittest h in m_hittest_list) {
				h.position = offset;
			}

			hittest ht;
			int pos_y = 0;

			// 국아이콘
			ht = m_hittest_list[(int)item_index.country];
			ht.rect = new Rectangle(0, pos_y, 24, def.ICON_SIZE_Y);
			pos_y += STEP_Y0;

			// 아이콘들
			ht = m_hittest_list[(int)item_index.icons];
			ht.rect = new Rectangle(2, pos_y, ICONS_STEP_X * 4, def.ICON_SIZE_Y);

			// web아이콘
			ht = m_hittest_list[(int)item_index.web];
			ht.rect = new Rectangle(2 + ICONS_STEP_X * 4 + (10), pos_y, def.ICON_SIZE_X + 1, def.ICON_SIZE_Y + 1);

			// 국명
			ht = m_hittest_list[(int)item_index.county_name];
			ht.rect = new Rectangle(24 + 4, 0 + 2, 130, def.ICON_SIZE_Y);

			// 언어1
			ht = m_hittest_list[(int)item_index.lang1];
			ht.rect = new Rectangle((int)base.client_size.X - 130, 0 + 2, 130, def.ICON_SIZE_Y);

			// 언어2
			ht = m_hittest_list[(int)item_index.lang2];
			ht.rect = new Rectangle((int)base.client_size.X - 130, pos_y + 1, 130, def.ICON_SIZE_Y);

			pos_y += STEP_Y0;

			// 아이템목록
			// 아이템 개수가 잘 들어가도록 크기를 조정한다
			int size_y = (int)base.client_size.Y - (pos_y + STEP_Y) - (int)base.pos.Y;
			d3d_windows.window iw = base.FindWindow("설정 창");
			if (iw != null) size_y -= (int)iw.size.Y;

			ht = m_hittest_list[(int)item_index.item_list];
			ht.rect = new Rectangle(0, pos_y, (int)base.client_size.X, size_y);
			pos_y += size_y + 2;

			// 탭
			ht = m_hittest_list[(int)item_index.tabs];
			ht.rect = new Rectangle(2, pos_y, TABS_STEP_X * TAB_MAX, def.ICON_SIZE_Y);
			pos_y += STEP_Y;

			// 세로 크기를 갱신한다
			base.client_size = new Vector2(base.client_size.X, pos_y);
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		override protected void OnDrawClient() {
			if (m_is_1st_draw) {
				update_item_list();
				m_is_1st_draw = false;
			}

			// 아이템목록
			// 탭의 현재 상태를 그린다
			draw_current_tab();

			// 국명
			draw_country_name();

			// 언어
			draw_lang();

			// 마우스가 있는 위치의 배경
			draw_current_back();

			base.device.sprites.BeginDrawSprites(m_lib.icons.texture);
			{

				// 국아이콘
				draw_country();

				// 아이콘들
				draw_icons();

				// web아이콘
				draw_web();

				// 탭
				draw_tabs();

			}
			base.device.sprites.EndDrawSprites();

			// 선택중아이콘
			//			draw_select_cursor();
		}

		/*-------------------------------------------------------------------------
		 선택중아이콘
		---------------------------------------------------------------------------*/
		/*
				private void draw_select_cursor()
				{
					if(Info == null)				return;

					int			Index	= m_select_index - m_draw_top_item_index;
					if(Index < 0)					return;
					if(Index >= m_item_list_max)	return;

					hittest		ht		= m_hittest_list[(int)item_index.item_list];
					Rectangle	rect	= ht.CalcRect();

					Viewport	view	= base.SetViewport(rect.X, rect.Y,
															rect.Width + 1, rect.Height);

					Vector3		pos		= new Vector3(rect.X, rect.Y, base.z);

					pos.X	+= 2;
					pos.Y	+= Index * STEP_Y;
					pos.Y	+= STEP_Y / 2;
					base.Device.sprites.BeginDrawSprites(m_lib.icons.Texture);
					base.Device.sprites.AddDrawSprites(pos, m_lib.icons.GetIcon(icons.IconIndex.select_0));
					base.Device.sprites.EndDrawSprites();
					base.SetViewport(view);
				}
		*/
		/*-------------------------------------------------------------------------
		 현재 마우스가 있는 위치 설정
		---------------------------------------------------------------------------*/
		private void draw_current_back() {
			Point pos = base.device.GetClientMousePosition();

			switch (m_hittest_list.HitTest_Index(pos)) {
				case (int)item_index.icons:
					draw_current_back_icons(pos);
					break;
				case (int)item_index.web:
					draw_current_back_web(pos);
					break;
				case (int)item_index.tabs:
					draw_current_back_tabs(pos);
					break;
			}
		}

		/*-------------------------------------------------------------------------
		 현재 마우스가 있는 위치 설정
		 아이콘들
		---------------------------------------------------------------------------*/
		private void draw_current_back_icons(Point pos) {
			hittest ht = m_hittest_list[(int)item_index.icons];
			Rectangle rect = ht.CalcRect();

			pos.X -= rect.X;
			pos.X /= ICONS_STEP_X;	  // Index;

			Vector3 dpos = new Vector3(rect.X + ICONS_STEP_X * pos.X, rect.Y, base.z);
			dpos.X -= 1;
			dpos.Y -= 1;
			base.DrawCurrentButtonBack(dpos, new Vector2(def.ICON_SIZE_X + 2, def.ICON_SIZE_Y + 2));
		}

		/*-------------------------------------------------------------------------
		 현재 마우스가 있는 위치 설정
		 Web아이콘
		---------------------------------------------------------------------------*/
		private void draw_current_back_web(Point pos) {
			if (info == null) return;
			if (!info.IsUrl) return;

			hittest ht = m_hittest_list[(int)item_index.web];
			Rectangle rect = ht.CalcRect();

			Vector3 dpos = new Vector3(rect.X, rect.Y, base.z);
			dpos.X -= 1;
			dpos.Y -= 1;
			base.DrawCurrentButtonBack(dpos, new Vector2(def.ICON_SIZE_X + 2, def.ICON_SIZE_Y + 2));
		}

		/*-------------------------------------------------------------------------
		 현재 마우스가 있는 위치 설정
		 탭
		---------------------------------------------------------------------------*/
		private void draw_current_back_tabs(Point pos) {
			hittest ht = m_hittest_list[(int)item_index.tabs];
			Rectangle rect = ht.CalcRect();

			pos.X -= rect.X;
			pos.X /= TABS_STEP_X;

			Vector3 dpos = new Vector3(rect.X + TABS_STEP_X * pos.X, rect.Y, base.z);
			dpos.X -= 2;
			dpos.Y -= 2;
			base.DrawCurrentButtonBack(dpos, new Vector2(def.ICON_SIZE_X + 4, def.ICON_SIZE_Y + 5));
		}

		/*-------------------------------------------------------------------------
		 국명
		---------------------------------------------------------------------------*/
		private void draw_country_name() {
			if (info == null) return;

			hittest ht = m_hittest_list[(int)item_index.county_name];
			Rectangle rect = ht.CalcRect();

			Vector3 pos = new Vector3(rect.X, rect.Y, base.z);

			// 이름
			base.device.DrawText(font_type.normal, info.Name,
									(int)pos.X, (int)pos.Y, Color.Black);
		}

		/*-------------------------------------------------------------------------
		 언어
		---------------------------------------------------------------------------*/
		private void draw_lang() {
			if (info == null) return;

			hittest ht = m_hittest_list[(int)item_index.lang1];
			Rectangle rect = ht.CalcRect();

			Vector3 pos = new Vector3(rect.X, rect.Y, base.z);

			// 언어
			if (info.InfoType == GvoWorldInfo.InfoType.City) {
				if (info.Lang1 != "") {
					base.device.DrawTextR(font_type.normal, info.Lang1,
											(int)pos.X + rect.Width, (int)pos.Y, Color.Black);
				}
				if (info.Lang2 != "") {
					base.device.DrawTextR(font_type.normal, info.Lang2,
											(int)pos.X + rect.Width, (int)pos.Y + (STEP_Y0 - 1), Color.Black);
				}
			} else if (info.InfoType == GvoWorldInfo.InfoType.Sea) {
				if (info.SeaInfo != null) {
					// 최대 속도 상승
					base.device.DrawTextR(font_type.normal, info.SeaInfo.SpeedUpRateString,
											(int)pos.X + rect.Width, (int)pos.Y, Color.Black);
				}
			}
		}

		/*-------------------------------------------------------------------------
		 web아이콘
		---------------------------------------------------------------------------*/
		private void draw_web() {
			if (info == null) return;
			if (!info.IsUrl) return;

			hittest ht = m_hittest_list[(int)item_index.web];
			Rectangle rect = ht.CalcRect();

			Vector3 pos = new Vector3(rect.X, rect.Y, base.z);

			icons.icon_index icon = (info.InfoType == GvoWorldInfo.InfoType.City)
											? icons.icon_index.web
											: icons.icon_index.map_icon;
			base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(icon));
		}

		/*-------------------------------------------------------------------------
		 현재 탭을 그린다
		---------------------------------------------------------------------------*/
		private void draw_current_tab() {
			hittest ht = m_hittest_list[(int)item_index.item_list];
			Rectangle rect = ht.CalcRect();

			Vector3 pos = new Vector3(rect.X, rect.Y, base.z);
			Vector2 size = new Vector2(rect.Width, rect.Height);
			Vector3 pos2 = pos;

			// 탭의 현재 상태
			pos2 = pos;
			pos2.Y += size.Y;
			pos2.X += TABS_STEP_X * m_tab_index;
			base.device.DrawFillRect(pos2, new Vector2(TABS_STEP_X, STEP_Y), Color.FromArgb(128, 255, 255, 255).ToArgb());
			base.device.DrawLineRect(pos2, new Vector2(TABS_STEP_X, STEP_Y + 1), Color.Black.ToArgb());
		}

		/*-------------------------------------------------------------------------
		 아이템 목록 개수를 얻는다
		---------------------------------------------------------------------------*/
		private int get_item_list_count() {
			return get_item_list_count(m_tab_index);
		}
		private int get_item_list_count(int index) {
			if (info == null) return 0;
			if (index == TAB_MEMO) return info.GetMemoLines();		// 메모
			return info.GetCount(GvoWorldInfo.Info.GroupIndex._0 + index);
		}

		/*-------------------------------------------------------------------------
		 국아이콘그리기
		---------------------------------------------------------------------------*/
		private void draw_country() {
			if (info == null) return;

			hittest ht = m_hittest_list[(int)item_index.country];
			Rectangle rect = ht.CalcRect();
			Vector3 pos = new Vector3(rect.X, rect.Y, base.z);

			icons.icon_index index;
			switch (info.InfoType) {
				case GvoWorldInfo.InfoType.PF:
					index = icons.icon_index.country_0;
					break;
				case GvoWorldInfo.InfoType.Sea:
					index = icons.icon_index.country_2;
					break;
				case GvoWorldInfo.InfoType.Shore:
				case GvoWorldInfo.InfoType.OutsideCity:
					index = icons.icon_index.country_3;
					break;
				case GvoWorldInfo.InfoType.Shore2:
					index = icons.icon_index.country_1;
					break;
				case GvoWorldInfo.InfoType.City:
				default:
					index = icons.icon_index.country_4;
					// 동맹 관계에 따라 깃발을 선택한다
					index += (int)info.MyCountry;
					break;
			}

			// 아이콘
			base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(index));
		}

		/*-------------------------------------------------------------------------
		 아이콘들 그리기
		---------------------------------------------------------------------------*/
		private void draw_icons() {
			hittest ht = m_hittest_list[(int)item_index.icons];
			Rectangle rect = ht.CalcRect();

			Vector3 pos = new Vector3(rect.X, rect.Y, base.z);

			for (int i = 0; i < 4; i++) {
				icons.icon_index index = icons.icon_index.tab2_gray_0;
				if (info != null) {
					if ((info.Sakaba & (1 << i)) != 0) {
						index = icons.icon_index.tab2_0;
					}
				}
				base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(index + i));
				pos.X += ICONS_STEP_X;
			}
		}

		/*-------------------------------------------------------------------------
		 탭들 그리기
		---------------------------------------------------------------------------*/
		private void draw_tabs() {
			hittest ht = m_hittest_list[(int)item_index.tabs];
			Rectangle rect = ht.CalcRect();

			Vector3 pos = new Vector3(rect.X, rect.Y, base.z);

			for (int i = 0; i < TAB_MAX; i++) {
				bool enable = (info != null) && (get_item_list_count(i) > 0);
				if ((i == TAB_STAFF) || (i == TAB_ADJUTANT)) {
					// 회색 아이콘이 따로 없으므로 어둡게 그린다
					icons.icon_index added = (i == TAB_STAFF) ? icons.icon_index.tab_staff : icons.icon_index.tab_adjutant;
					base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(added), enable ? -1 : Color.Gray.ToArgb());
				} else {
					// 메모는 아이콘의 원래 칸을 쓴다
					int icon = (i == TAB_MEMO) ? 11 : i;
					icons.icon_index index = enable ? icons.icon_index.tab_0 : icons.icon_index.tab_gray_0;
					base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(index + icon));
				}
				pos.X += TABS_STEP_X;
			}
		}

		/*-------------------------------------------------------------------------
		 마우스좌클릭
		 탭
		---------------------------------------------------------------------------*/
		private void on_mouse_l_click_item_tabs(Point pos) {
			hittest ht = m_hittest_list[(int)item_index.tabs];
			Rectangle rect = ht.CalcRect();

			pos.X -= rect.X;
			ajust_tab_index(pos.X / TABS_STEP_X);
		}

		/*-------------------------------------------------------------------------
		 마우스우클릭
		 탭
		---------------------------------------------------------------------------*/
		private void on_mouse_r_click_item_tabs(Point pos) {
			on_mouse_l_click_item_tabs(pos);

			// 탭을 장소 표시
			m_spot.SetSpot(spot.type.tab_0 + m_tab_index, "");
			m_form.UpdateSpotList();
		}

		/*-------------------------------------------------------------------------
		 마우스 휠
		 탭
		---------------------------------------------------------------------------*/
		private void on_mouse_wheel_item_tabs(Point pos, int delta) {
			if (delta > 0) ajust_tab_index(m_tab_index - 1);
			else ajust_tab_index(m_tab_index + 1);
		}

		/*-------------------------------------------------------------------------
		 탭 위치 설정과 조정
		---------------------------------------------------------------------------*/
		private void ajust_tab_index(int index) {
			// 루프시킨다
			if (index < 0) index = TAB_MAX - 1;
			if (index >= TAB_MAX) index = 0;

			m_tab_index = index;

			// 메모 윈도우 상태 갱신
			// 깜빡이는 문제로 여기서 update_memo_window() 를 호출하는 것은 중단
			//			update_memo_window();
			// 아이템목록업데이트
			update_item_list();
		}

		/*-------------------------------------------------------------------------
		 검색된 아이템을 장소 표시한다
		---------------------------------------------------------------------------*/
		public void SpotItem(GvoDatabase.Find select) {
			if (select == null) {
				// 선택이 없으면 장소를 해제한다
				m_spot.SetSpot(spot.type.none, "");
				m_form.UpdateSpotList();
				return;
			}
			// 아이템 DB 는 장소 지정 불가능
			if (select.Type == GvoDatabase.Find.FindType.Database) return;

			GvoWorldInfo.Info _info = m_db.World.FindInfo(select.InfoName);
			if (_info != null) {
				// 장소 리셋 없이 설정
				set_info(_info, false);
				// 센터링하도록 한다
				req_centering_info();
			}

			switch (select.Type) {
				case GvoDatabase.Find.FindType.Data:
				case GvoDatabase.Find.FindType.DataPrice:
					// 아이템에서 선택한 것을 검색한다
					find_item_for_selected(select.Data.Name);
					// 아이템을 장소 지정
					m_spot.SetSpot(spot.type.has_item, select.Data.Name);
					break;
				case GvoDatabase.Find.FindType.Database:
					// 아이템 DB 는 장소 지정 불가능
					break;
				case GvoDatabase.Find.FindType.InfoName:
					// 도시를 장소 지정
					m_spot.SetSpot(spot.type.city_name, select.InfoName);
					break;
				case GvoDatabase.Find.FindType.Lang:
					// 언어를 장소 지정
					m_spot.SetSpot(spot.type.language, select.Lang);
					break;
				case GvoDatabase.Find.FindType.CulturalSphere:
					// 문화권
					m_spot.SetSpot(spot.type.cultural_sphere, select.InfoName);
					break;
			}
			m_form.UpdateSpotList();
		}

		/*-------------------------------------------------------------------------
		 장소 아이템 변경
		 장소 정보는 변경되지 않는다
		---------------------------------------------------------------------------*/
		public void SpotItemChanged(spot.spot_once select) {
			if (select == null) return;
			if (select.info == null) return;

			GvoWorldInfo.Info _info = select.info;
			if (_info != null) {
				// 장소 리셋 없이 설정
				set_info(_info, false);
				// 센터링하도록 한다
				req_centering_info();
			}

			// 아이템에서 선택한 것을 검색한다
			find_item_for_selected(select.name);
		}

		/*-------------------------------------------------------------------------
		 아이템에서 선택한 것을 검색한다
		---------------------------------------------------------------------------*/
		private void find_item_for_selected(string find_item) {
			if (info == null) return;

			for (GvoWorldInfo.Info.GroupIndex i = GvoWorldInfo.Info.GroupIndex._0; i < GvoWorldInfo.Info.GroupIndex.max; i++) {
				for (int s = 0; s < info.GetCount(i); s++) {
					GvoWorldInfo.Info.Group.Data d = info.GetData(i, s);
					if (d == null) continue;
					if (d.Name != find_item) continue;

					// 선택한 아이템이 보이도록 스크롤 위치를 조정
					ajust_tab_index((int)i);

					// 표시 위치를 조정
					if (m_list_view.Items.Count >= s) {
						m_list_view.Items[s].Selected = true;
						m_list_view.Items[s].EnsureVisible();
						m_list_view.Items[s].Focused = true;
					}
					return;
				}
			}
		}

		/*-------------------------------------------------------------------------
		 센터링되어 있는 도시를 센터링한다
		 리퀘스트
		---------------------------------------------------------------------------*/
		private void req_centering_info() {
			if (info == null) return;

			m_lib.setting.centering_gpos = transform.map_pos2_game_pos(info.position, m_lib.loop_image);
			m_lib.setting.req_centering_gpos.Request();
		}

		/*-------------------------------------------------------------------------
		 국기를 좌클릭
		---------------------------------------------------------------------------*/
		private void on_mouse_l_click_country(Point pos) {
			// 국기를 장소 표시
			// 도시만
			if (info == null) return;
			if (info.InfoType != GvoWorldInfo.InfoType.City) return;

			m_spot.SetSpot(spot.type.country_flags, "");
			m_form.UpdateSpotList();
		}

		/*-------------------------------------------------------------------------
		 국기를 우클릭
		---------------------------------------------------------------------------*/
		private void on_mouse_r_click_country(Point pos) {
			// 도시만
			if (info == null) return;
			if (info.InfoType != GvoWorldInfo.InfoType.City) return;
			// 동맹할 수 있는 도시만
			if (info.AllianceType != GvoWorldInfo.AllianceType.Alliance) return;

			// 우클릭 메뉴를 연다
			m_form.ShowChangeDomainsMenuStrip(pos);
		}

		/*-------------------------------------------------------------------------
		 이름을 우클릭
		---------------------------------------------------------------------------*/
		private void on_mouse_r_click_country_name(Point pos) {
			// 도시만
			if (info == null) return;
			if (info.InfoType != GvoWorldInfo.InfoType.City) return;
			if (info.CulturalSphere == GvoWorldInfo.CulturalSphere.Unknown) return;

			// 장소 표시
			SpotItem(new GvoDatabase.Find(info.CulturalSphere, ""));
		}

		/*-------------------------------------------------------------------------
		 아이콘들을 좌클릭
		---------------------------------------------------------------------------*/
		private void on_mouse_l_click_icons(Point pos) {
			hittest ht = m_hittest_list[(int)item_index.icons];
			Rectangle rect = ht.CalcRect();

			pos.X -= rect.X;
			pos.X /= ICONS_STEP_X;	  // Index;

			// 아이콘을 장소 표시
			m_spot.SetSpot(spot.type.icons_0 + pos.X, "");
			m_form.UpdateSpotList();
		}

		/*-------------------------------------------------------------------------
		 국명을 좌클릭
		---------------------------------------------------------------------------*/
		private void on_mouse_l_click_country_name(Point pos) {
			if (info == null) return;

			// 센터링 리퀘스트
			req_centering_info();

			// 장소
			m_spot.SetSpot(spot.type.city_name, info.Name);
			m_form.UpdateSpotList();
		}

		/*-------------------------------------------------------------------------
		 언어를 좌클릭
		---------------------------------------------------------------------------*/
		private void on_mouse_l_click_lang(Point pos, int index) {
			if (info == null) return;

			// 언어를 장소 표시
			string lang = (index == 0) ? info.Lang1 : info.Lang2;
			if (lang == "") return;

			m_spot.SetSpot(spot.type.language, lang);
			m_form.UpdateSpotList();
		}

		/*-------------------------------------------------------------------------
		 Web 아이콘을 좌클릭
		---------------------------------------------------------------------------*/
		private void on_mouse_l_click_web(Point pos) {
			if (info == null) return;
			if (!info.IsUrl) return;

			if (info.UrlIndex != -1) {
				// 인벤 지리 DB 의 도시·상륙지 상세
				Process.Start(def.INVEN_AREACITY_URL + info.UrlIndex.ToString());
			} else {
				// 번호가 없는 옛 데이터(url_string)는 지리 DB 목록을 연다
				Process.Start(def.INVEN_AREACITY_LIST_URL);
			}
		}

		/*-------------------------------------------------------------------------
		 tab 을 전환한다
		---------------------------------------------------------------------------*/
		public void ChangeTab(bool is_next) {
			if (is_next) {
				ajust_tab_index(m_tab_index + 1);
			} else {
				ajust_tab_index(m_tab_index - 1);
			}
			m_lib.device.SetMustDrawFlag();
		}

		/*-------------------------------------------------------------------------
		 툴팁 표시용 문자열을 얻는다
		 표시할 문자열이 없는 경우에는 null 을 반환한다
		---------------------------------------------------------------------------*/
		override protected string OnToolTipStringClient(Point pos) {
			switch (m_hittest_list.HitTest_Index(pos)) {
				case (int)item_index.icons: return get_tooltip_string_icons(pos);
				case (int)item_index.tabs: return get_tooltip_string_tabs(pos);
			}

			// 아래는 info 를 참조해야 하는 것들
			if (info == null) return null;

			switch (m_hittest_list.HitTest_Index(pos)) {
				case (int)item_index.web:
					return info.GetToolTipString_HP();
				case (int)item_index.county_name: {
						string tmp = info.TooltipString;
						tmp += "\n좌클릭으로 중심 이동";
						if (info.InfoType == GvoWorldInfo.InfoType.City) {
							tmp += "\n우클릭으로 소속 문화권의 장소 표시";
						}
						return tmp;
					}
				case (int)item_index.lang1:
				case (int)item_index.lang2:
					if ((info.Lang1 != "")
						|| (info.Lang2 != "")) {
						return "사용 언어\n좌클릭으로 장소 표시";
					}
					break;
				case (int)item_index.country:
					switch (info.InfoType) {
						case GvoWorldInfo.InfoType.City:
							switch (info.AllianceType) {
								case GvoWorldInfo.AllianceType.Piratical:
								case GvoWorldInfo.AllianceType.Unknown: return info.AllianceTypeStr + "\n좌클릭으로 장소 표시";
								case GvoWorldInfo.AllianceType.Alliance: return info.AllianceTypeStr + " " + info.CountryStr + "\n좌클릭으로 장소 표시\n우클릭으로 동맹국 변경";

								case GvoWorldInfo.AllianceType.Capital:
								case GvoWorldInfo.AllianceType.Territory:
									return info.CountryStr + " " + info.AllianceTypeStr + "\n좌클릭으로 장소 표시";
							}
							break;
						case GvoWorldInfo.InfoType.OutsideCity:
						case GvoWorldInfo.InfoType.PF:
						case GvoWorldInfo.InfoType.Sea:
						case GvoWorldInfo.InfoType.Shore:
						case GvoWorldInfo.InfoType.Shore2:
							//					return Info.InfoTypeStr + "\n좌클릭으로 장소 표시";
							return info.InfoTypeStr;
					}
					break;
			}
			return null;
		}

		/*-------------------------------------------------------------------------
		 툴팁 표시용 문자열을 얻는다
		 표시할 문자열이 없는 경우에는 null 을 반환한다
		 아이템목록
		---------------------------------------------------------------------------*/
		private string get_tooltip_string_item_list(GvoWorldInfo.Info.Group.Data d) {
			if (d == null) return null;

			string str = d.TooltipString;

			// 도움말을 포함한다
			//			if((!db.IsSkill)&&(!db.IsReport)){
			str += "\n(우클릭으로 메뉴 열기)";
			//			}
			return str;
		}

		/*-------------------------------------------------------------------------
		 툴팁 표시용 문자열을 얻는다
		 표시할 문자열이 없는 경우에는 null 을 반환한다
		 아이콘들
		---------------------------------------------------------------------------*/
		private string get_tooltip_string_icons(Point pos) {
			hittest ht = m_hittest_list[(int)item_index.icons];
			Rectangle rect = ht.CalcRect();

			pos.X -= rect.X;
			pos.X /= ICONS_STEP_X;	  // Index;

			switch (pos.X) {
				case 0: return "의뢰 중개인/주점 아가씨/판매원\n좌클릭으로 장소 표시";
				case 1: return "서고\n좌클릭으로 장소 표시";
				case 2: return "번역가\n좌클릭으로 장소 표시";
				case 3: return "무역상인\n좌클릭으로 장소 표시";
			}
			return null;
		}

		/*-------------------------------------------------------------------------
		 툴팁 표시용 문자열을 얻는다
		 표시할 문자열이 없는 경우에는 null 을 반환한다
		 탭
		---------------------------------------------------------------------------*/
		private string get_tooltip_string_tabs(Point pos) {
			hittest ht = m_hittest_list[(int)item_index.tabs];
			Rectangle rect = ht.CalcRect();

			pos.X -= rect.X;
			pos.X /= TABS_STEP_X;

			if (0 <= pos.X && pos.X < TAB_MAX) {
				return GvoWorldInfo.Info.GetGroupName((GvoWorldInfo.Info.GroupIndex)pos.X) + "\n우클릭으로 장소 표시";
			}
			return null;
		}

		/*-------------------------------------------------------------------------
		 메모 윈도우 상태 갱신
		---------------------------------------------------------------------------*/
		private void update_memo_window() {
			if (m_tab_index == TAB_MEMO) {
				EnableMemoWindow(true);
				EnableItemWindow(false);
			} else {
				EnableMemoWindow(false);
				EnableItemWindow(true);
			}
		}

		/*-------------------------------------------------------------------------
		 메모 윈도우 상태 설정
		---------------------------------------------------------------------------*/
		public void EnableMemoWindow(bool is_enable) {
			enable_ctrl(m_memo_text_box, is_enable);
		}

		/*-------------------------------------------------------------------------
		 아이템 윈도우 상태 설정
		---------------------------------------------------------------------------*/
		public void EnableItemWindow(bool is_enable) {
			enable_ctrl(m_list_view, is_enable);
		}

		/*-------------------------------------------------------------------------
		 컨트롤 상태 설정
		---------------------------------------------------------------------------*/
		private void enable_ctrl(Control ctrl, bool is_enable) {
			// 표시설정
			if (is_enable) {
				if (!ctrl.Visible) {
					ctrl.Visible = true;
				}

				// 편집 가능/불가능 설정
				if (m_info == null) {
					if (ctrl.Enabled) ctrl.Enabled = false;
				} else {
					if (!ctrl.Enabled) ctrl.Enabled = true;
				}
			} else {
				if (ctrl.Visible) {
					ctrl.Visible = false;
					base.device.form.Focus();	   // 포커스를 되돌린다
				}
			}
		}

		/*-------------------------------------------------------------------------
		 메모를 갱신한다
		 종료 시 갱신용
		---------------------------------------------------------------------------*/
		public void UpdateMemo() {
			if (info == null) return;
			info.Memo = m_memo_text_box.Text;
		}

		/*-------------------------------------------------------------------------
		 아이템목록업데이트
		---------------------------------------------------------------------------*/
		private void update_item_list() {
			if (info == null) {
				// 내용 초기화
				m_list_view.Clear();
				return;
			}

			// 갱신 시작
			m_list_view.BeginUpdate();

			// 내용 초기화
			m_list_view.Clear();

			float dpi = Useful.GetDpiRatio();
			// 헤더
			switch (m_tab_index) {
				case 0:
					m_list_view.Columns.Add("명칭", (int)(80 * dpi));
					m_list_view.Columns.Add("★", (int)(25 * dpi), HorizontalAlignment.Center);
					m_list_view.Columns.Add("종류", (int)(52 * dpi), HorizontalAlignment.Center);
					if (info.InfoType == GvoWorldInfo.InfoType.City) {
						m_list_view.Columns.Add("가격", (int)(55 * dpi), HorizontalAlignment.Right);
					} else {
						m_list_view.Columns.Add("스킬", (int)(60 * dpi), HorizontalAlignment.Center);
					}
					break;
				case 3: // 스킬, 보고
					m_list_view.Columns.Add("명칭", (int)(140 * dpi));
					m_list_view.Columns.Add("인물", (int)(120 * dpi));
					break;
				case 10:	// 행상인
					m_list_view.Columns.Add("명칭", (int)(160 * dpi));
					m_list_view.Columns.Add("인물", (int)(60 * dpi));
					break;
				case 5:		// 조선공
					m_list_view.Columns.Add("명칭", (int)(120 * dpi));
					m_list_view.Columns.Add("위치", (int)(120 * dpi));
					break;
				case TAB_STAFF:
					m_list_view.Columns.Add("명칭", (int)(160 * dpi));
					m_list_view.Columns.Add("구분", (int)(80 * dpi));
					break;
				case TAB_ADJUTANT:
					m_list_view.Columns.Add("명칭", (int)(75 * dpi));
					m_list_view.Columns.Add("직업, 스킬", (int)(165 * dpi));
					break;
				default:
					m_list_view.Columns.Add("명칭", (int)(160 * dpi));
					m_list_view.Columns.Add("가격", (int)(80 * dpi), HorizontalAlignment.Right);
					break;
			}

			// 추가
			int count = info.GetCount(GvoWorldInfo.Info.GroupIndex._0 + m_tab_index);
			GvoWorldInfo.Info.Group g = info.GetGroup(GvoWorldInfo.Info.GroupIndex._0 + m_tab_index);
			System.Drawing.Font font = m_list_view.Font;
			for (int i = 0; i < count; i++) {
				GvoWorldInfo.Info.Group.Data d = g.GetData(i);
				ListViewItem item = new ListViewItem(d.Name, 0);
				item.UseItemStyleForSubItems = false;
				item.ForeColor = d.Color;
				item.Tag = d;
				string tt = get_tooltip_string_item_list(d);
				if (tt != null) item.ToolTipText = tt;
				if (m_tab_index == 0) {
					item.SubItems.Add((d.IsBonusItem) ? "★" : "", Color.Tomato, item.BackColor, font);
					item.SubItems.Add(d.Type,
										d.CategolyColor, item.BackColor, font);
				}
				item.SubItems.Add(d.Price, d.PriceColor, item.BackColor, font);
				m_list_view.Items.Add(item);
			}

			// 업데이트종료
			m_list_view.EndUpdate();

			// 컬럼 크기 조정
			ajust_item_columns_width();
		}

		/*-------------------------------------------------------------------------
		 아이템 목록의 컬럼 폭 조정
		 첫 컬럼의 크기를 조정해서 가로 스크롤바가 나오지 않도록 한다
		---------------------------------------------------------------------------*/
		private void ajust_item_columns_width() {
			if (m_list_view.Columns.Count < 1) return;
			if (m_tab_index == 3) return;	   // 인물 탭은 특별히 아무것도 하지 않는다

			Size size = m_list_view.ClientSize;
			// 첫 컬럼 이외 컬럼들의 폭 합계를 구한다
			int width = 0;
			for (int i = 1; i < m_list_view.Columns.Count; i++) {
				width += m_list_view.Columns[i].Width;
			}
			// 나머지를 첫 컬럼의 폭으로 삼는다
			width = size.Width - width;
			if (width <= 0) width = 20;
			m_list_view.Columns[0].Width = width;
		}

		/*-------------------------------------------------------------------------
		 아이템 목록에 마우스가 있는지 여부를 반환한다
		 포커스 판정용
		---------------------------------------------------------------------------*/
		public bool HitTest_ItemList(Point pos) {
			if (base.window_mode == mode.small) return false;   // 최소화중
			if (m_hittest_list.HitTest_Index(pos) == (int)item_index.item_list) {
				return true;
			}
			return false;
		}
	}
}
