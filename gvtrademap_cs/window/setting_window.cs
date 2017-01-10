/*-------------------------------------------------------------------------

 좌の윈도우
 설정아이콘を그리기する

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
using System.Diagnostics;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	class setting_window : d3d_windows.window {
		// 위치と사이즈(사이즈は自動で조정される)
		private const int WINDOW_POS_X = 3;
		private const int WINDOW_POS_Y = 3;
		private const float WINDOW_POS_Z = 0.2f;
		private const int WINDOW_SIZE_X = 250;  // 初期사이즈
		private const int WINDOW_SIZE_Y = 200;  // 初期사이즈

		// クライアント사이즈
		// 세로は윈도우사이즈から계산される
		private const int CLIENT_SIZE_X = (16 + 4) * (int)setting_icons_index.max + 6;

		// 配置간격
		private const int SETTING_ICONS_STEP = def.ICON_SIZE_X + 0;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private gvtrademap_cs_form m_form;
		private gvt_lib m_lib;					  // 
		private GvoDatabase m_db;					   // DB

		private hittest_list m_hittest_list;				// 矩形관리

		private enum item_index {
			setting,			// 설정
			setting_button,	 // 설정ボタン
			max
		};

		private enum setting_icons_index {
			save_searoutes,
			share_routes,
			web_icons,
			memo_icons,
			searoutes,
			popup_day_interval,
			accident,
			center_my_ship,
			myship_angle,
			sea_area,
			screen_shot,
			show_searoutes_list,
			max
		};

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public setting_window(gvt_lib lib, GvoDatabase db, gvtrademap_cs_form form)
			: base(lib.device, new Vector2(WINDOW_POS_X, WINDOW_POS_Y), new Vector2(WINDOW_SIZE_X, WINDOW_SIZE_Y), WINDOW_POS_Z) {
			base.title = "설정윈도우";

			m_form = form;
			m_lib = lib;
			m_db = db;

			// 아이템추가
			m_hittest_list = new hittest_list();

			// 설정
			m_hittest_list.Add(new hittest());
			// 설정ボタン
			m_hittest_list.Add(new hittest());
		}

		/*-------------------------------------------------------------------------
		 업데이트
		---------------------------------------------------------------------------*/
		override protected void OnUpdateClient() {
			base.client_size = new Vector2(CLIENT_SIZE_X, def.ICON_SIZE_Y);

			// 좌下固定
			base.pos = new Vector2(WINDOW_POS_X, screen_size.Y - base.size.Y - 4);

			Point offset = transform.ToPoint(base.client_pos);

			// オフセットの업데이트
			foreach (hittest h in m_hittest_list) {
				h.position = offset;
			}

			hittest ht;

			// 설정
			ht = m_hittest_list[(int)item_index.setting];
			ht.rect = new Rectangle(1, 0, SETTING_ICONS_STEP * (int)setting_icons_index.max, def.ICON_SIZE_Y);

			// 설정ボタン
			ht = m_hittest_list[(int)item_index.setting_button];
			ht.rect = new Rectangle((int)base.client_size.X - 48, 0, 48, def.ICON_SIZE_Y);
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		override protected void OnDrawClient() {
			// 설정목록の背景
			draw_seting_back();

			// 현재の마우스がある장소설정
			draw_current_setting_back();

			base.device.sprites.BeginDrawSprites(m_lib.icons.texture);
			{
				// 설정
				draw_setting();

				// 설정ボタン
				draw_setting_button();

			}
			base.device.sprites.EndDrawSprites();
		}

		/*-------------------------------------------------------------------------
		 설정목록の背景
		---------------------------------------------------------------------------*/
		private void draw_seting_back() {
			hittest ht = m_hittest_list[(int)item_index.setting];
			Rectangle rect = ht.CalcRect();

			Vector3 pos = new Vector3(base.client_pos.X, rect.Y, base.z);
			base.device.DrawFillRect(pos, new Vector2(base.client_size.X, rect.Height + 1), Color.FromArgb(255, 96, 96, 96).ToArgb());
		}

		/*-------------------------------------------------------------------------
		 현재の마우스がある장소설정
		---------------------------------------------------------------------------*/
		private void draw_current_setting_back() {
			Point pos = base.device.GetClientMousePosition();

			hittest ht = m_hittest_list[(int)item_index.setting];
			if (!ht.HitTest(pos)) return;
			Rectangle rect = ht.CalcRect();

			pos.X -= rect.X;
			pos.X /= SETTING_ICONS_STEP;

			base.DrawCurrentButtonBack(new Vector3(rect.X + SETTING_ICONS_STEP * pos.X, rect.Y, base.z),
										new Vector2(def.ICON_SIZE_X, def.ICON_SIZE_Y));
		}

		/*-------------------------------------------------------------------------
		 설정ボタン
		---------------------------------------------------------------------------*/
		private void draw_setting_button() {
			hittest ht = m_hittest_list[(int)item_index.setting_button];
			Rectangle rect = ht.CalcRect();

			Vector3 pos = new Vector3(rect.X, rect.Y, base.z);
			base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(icons.icon_index.setting_button));
		}

		/*-------------------------------------------------------------------------
		 설정
		---------------------------------------------------------------------------*/
		private void draw_setting() {
			hittest ht = m_hittest_list[(int)item_index.setting];
			Rectangle rect = ht.CalcRect();

			Vector3 pos = new Vector3(rect.X, rect.Y, base.z);

			// 
			base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon((m_lib.setting.save_searoutes)
											? icons.icon_index.setting_0
											: icons.icon_index.setting_gray_0));
			pos.X += SETTING_ICONS_STEP;
			base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon((m_lib.setting.draw_share_routes)
											? icons.icon_index.setting_1
											: icons.icon_index.setting_gray_1));
			pos.X += SETTING_ICONS_STEP;
			base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon((m_lib.setting.draw_web_icons)
											? icons.icon_index.setting_8
											: icons.icon_index.setting_gray_8));
			pos.X += SETTING_ICONS_STEP;
			base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon((m_lib.setting.draw_icons)
											? icons.icon_index.setting_2
											: icons.icon_index.setting_gray_2));

			pos.X += SETTING_ICONS_STEP;
			base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon((m_lib.setting.draw_sea_routes)
											? icons.icon_index.setting_3
											: icons.icon_index.setting_gray_3));
			pos.X += SETTING_ICONS_STEP;
			base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon((m_lib.setting.draw_popup_day_interval == 1)
											? icons.icon_index.setting_4
											: (m_lib.setting.draw_popup_day_interval == 5)
											? icons.icon_index.setting_12
											: icons.icon_index.setting_gray_4));
			pos.X += SETTING_ICONS_STEP;
			base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon((m_lib.setting.draw_accident)
											? icons.icon_index.setting_5
											: icons.icon_index.setting_gray_5));
			pos.X += SETTING_ICONS_STEP;
			base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon((m_lib.setting.center_myship)
											? icons.icon_index.setting_6
											: icons.icon_index.setting_gray_6));
			pos.X += SETTING_ICONS_STEP;
			base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon((m_lib.setting.draw_myship_angle)
											? icons.icon_index.setting_7
											: icons.icon_index.setting_gray_7));
			pos.X += SETTING_ICONS_STEP;

			// 残り3つはグレー표시없음
			for (int i = 0; i < 3; i++) {
				base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(icons.icon_index.setting_10 + i));
				pos.X += SETTING_ICONS_STEP;
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
		 마우스ダブル클릭
		---------------------------------------------------------------------------*/
		override protected void OnMouseDClikClient(Point pos, MouseButtons button) {
			if ((button & MouseButtons.Left) != 0) {
				_window_on_mouse_l_click(pos);
			}
		}

		/*-------------------------------------------------------------------------
		 마우스좌클릭
		---------------------------------------------------------------------------*/
		private void _window_on_mouse_l_click(Point pos) {
			switch (m_hittest_list.HitTest_Index(pos)) {
				case (int)item_index.setting:
					on_mouse_l_click_setting(pos);
					break;
				case (int)item_index.setting_button:
					on_mouse_l_click_setting_button(pos);
					break;
				default:
					break;
			}
		}

		/*-------------------------------------------------------------------------
		 마우스좌클릭
		 설정항목
		---------------------------------------------------------------------------*/
		private void on_mouse_l_click_setting(Point pos) {
			hittest ht = m_hittest_list[(int)item_index.setting];
			Rectangle rect = ht.CalcRect();

			pos.X -= rect.X;
			pos.X /= SETTING_ICONS_STEP;

			setting_icons_index index = setting_icons_index.save_searoutes + pos.X;
			m_form.ExecFunction(get_extc_function(index));
		}

		/*-------------------------------------------------------------------------
		 実行する기능を得る
		---------------------------------------------------------------------------*/
		private KeyFunction get_extc_function(setting_icons_index index) {
			switch (index) {
				case setting_icons_index.save_searoutes: return KeyFunction.setting_window_button_00;
				case setting_icons_index.share_routes: return KeyFunction.setting_window_button_01;
				case setting_icons_index.web_icons: return KeyFunction.setting_window_button_02;
				case setting_icons_index.memo_icons: return KeyFunction.setting_window_button_03;
				case setting_icons_index.searoutes: return KeyFunction.setting_window_button_04;
				case setting_icons_index.popup_day_interval: return KeyFunction.setting_window_button_05;
				case setting_icons_index.accident: return KeyFunction.setting_window_button_06;
				case setting_icons_index.center_my_ship: return KeyFunction.setting_window_button_07;
				case setting_icons_index.myship_angle: return KeyFunction.setting_window_button_08;
				case setting_icons_index.sea_area: return KeyFunction.setting_window_button_09;
				case setting_icons_index.screen_shot: return KeyFunction.setting_window_button_10;
				case setting_icons_index.show_searoutes_list: return KeyFunction.setting_window_button_11;
			}

			// 未定義の기능を返す
			return KeyFunction.unknown_function;
		}

		/*-------------------------------------------------------------------------
		 설정ボタン
		---------------------------------------------------------------------------*/
		private void on_mouse_l_click_setting_button(Point pos) {
			hittest ht = m_hittest_list[(int)item_index.setting_button];
			Rectangle rect = ht.CalcRect();

			pos.X -= rect.X;

			if (pos.X < 16) {
				// 검색
				m_form.ExecFunction(KeyFunction.setting_window_button_12);
			} else {
				// 설정
				m_form.ExecFunction(KeyFunction.setting_window_button_13);
			}
		}

		/*-------------------------------------------------------------------------
		 ツールチップ표시용の문자열を得る
		 표시すべき문자열がない場合nullを返す
		---------------------------------------------------------------------------*/
		override protected string OnToolTipStringClient(Point pos) {
			switch (m_hittest_list.HitTest_Index(pos)) {
				case (int)item_index.setting: return get_tooltip_string_setting(pos);
				case (int)item_index.setting_button: return get_tooltip_string_setting_button(pos);
			}
			return null;
		}

		/*-------------------------------------------------------------------------
		 ツールチップ표시용の문자열を得る
		 표시すべき문자열がない場合nullを返す
		 설정아이콘
		---------------------------------------------------------------------------*/
		private string get_tooltip_string_setting(Point pos) {
			hittest ht = m_hittest_list[(int)item_index.setting];
			Rectangle rect = ht.CalcRect();

			pos.X -= rect.X;
			pos.X /= SETTING_ICONS_STEP;

			setting_icons_index index = setting_icons_index.save_searoutes + pos.X;
			string tip;
			switch (index) {
				case setting_icons_index.save_searoutes: tip = "항로기록（예측량）"; break;
				case setting_icons_index.share_routes: tip = "공유하고 있는 선박표시（설정 필요）"; break;
				case setting_icons_index.web_icons: tip = "@Web아이콘 표시\n우클릭으로 표시항목 설정"; break;
				case setting_icons_index.memo_icons: tip = "메모아이콘 표시\n우클릭으로 표시항목 설정"; break;
				case setting_icons_index.searoutes: tip = "항로선표시"; break;
				case setting_icons_index.popup_day_interval: tip = "말풍선표시"; break;
				case setting_icons_index.accident: tip = "재해표시\n우클릭으로 표시항목설정"; break;
				case setting_icons_index.center_my_ship: tip = "현재위치 중심으로 표시"; break;
				case setting_icons_index.myship_angle: tip = "나침반의 각도선, 진로예상선표시\n우클릭으로 표시항목 설정"; break;
				case setting_icons_index.sea_area: tip = "위험해역변동시스템설정"; break;
				case setting_icons_index.screen_shot: tip = "항로의 스크린샷보존"; break;
				case setting_icons_index.show_searoutes_list: tip = "항로도목록\n우클릭으로 항로도 설정"; break;
				default:
					return null;
			}
			return tip + get_assign_shortcut_text(get_extc_function(index));
		}

		/*-------------------------------------------------------------------------
		 アサインされた키の문자열を得る
		 アサインされていなければ""を返す
		---------------------------------------------------------------------------*/
		private string get_assign_shortcut_text(KeyFunction function) {
			string shortcut = m_lib.KeyAssignManager.List.GetAssignShortcutText(function);
			if (String.IsNullOrEmpty(shortcut)) return "";

			return "(" + shortcut + ")";
		}

		/*-------------------------------------------------------------------------
		 ツールチップ표시용の문자열を得る
		 표시すべき문자열がない場合nullを返す
		 설정ボタン
		---------------------------------------------------------------------------*/
		private string get_tooltip_string_setting_button(Point pos) {
			hittest ht = m_hittest_list[(int)item_index.setting_button];
			Rectangle rect = ht.CalcRect();

			pos.X -= rect.X;

			if (pos.X < 16) {
				return "전체검색ダイア로그を開く" + get_assign_shortcut_text(KeyFunction.setting_window_button_12);
			} else {
				return "설정ダイア로그を開く" + get_assign_shortcut_text(KeyFunction.setting_window_button_13);
			}
		}

		/*-------------------------------------------------------------------------
		 마우스우클릭
		---------------------------------------------------------------------------*/
		private void _window_on_mouse_r_click(Point pos) {
			switch (m_hittest_list.HitTest_Index(pos)) {
				case (int)item_index.setting:
					on_mouse_r_click_setting(pos);
					break;
				default:
					break;
			}
		}

		/*-------------------------------------------------------------------------
		 마우스우클릭
		 설정항목
		---------------------------------------------------------------------------*/
		private void on_mouse_r_click_setting(Point pos) {
			hittest ht = m_hittest_list[(int)item_index.setting];
			Rectangle rect = ht.CalcRect();

			pos.X -= rect.X;
			pos.X /= SETTING_ICONS_STEP;

			setting_icons_index index = setting_icons_index.save_searoutes + pos.X;
			switch (index) {
				case setting_icons_index.save_searoutes:
					break;
				case setting_icons_index.share_routes:
					break;
				case setting_icons_index.searoutes:
					break;
				case setting_icons_index.popup_day_interval:
					break;
				case setting_icons_index.center_my_ship:
					break;
				case setting_icons_index.sea_area:
					break;
				case setting_icons_index.screen_shot:
					break;

				case setting_icons_index.web_icons:
					set_draw_setting(DrawSettingPage.WebIcons);
					break;
				case setting_icons_index.memo_icons:
					set_draw_setting(DrawSettingPage.MemoIcons);
					break;
				case setting_icons_index.accident:
					set_draw_setting(DrawSettingPage.Accidents);
					break;
				case setting_icons_index.myship_angle:
					set_draw_setting(DrawSettingPage.MyShipAngle);
					break;
				case setting_icons_index.show_searoutes_list: {
						string info = m_lib.device.deviec_info_string;
						using (setting_form2 dlg = new setting_form2(m_lib.setting, m_lib.KeyAssignManager.List, info, setting_form2.tab_index.sea_routes)) {
							if (dlg.ShowDialog(base.device.form) == DialogResult.OK) {
								// 설정항목を反映させる
								m_form.UpdateSettings(dlg);
							}
						}
					}
					break;
			}
		}

		/*-------------------------------------------------------------------------
		 표시항목설정
		---------------------------------------------------------------------------*/
		private void set_draw_setting(DrawSettingPage type) {
			string info = m_lib.device.deviec_info_string;
			using (setting_form2 dlg = new setting_form2(m_lib.setting, m_lib.KeyAssignManager.List, info, type)) {
				if (dlg.ShowDialog(base.device.form) == DialogResult.OK) {
					// 설정항목を反映させる
					m_form.UpdateSettings(dlg);
				}
			}
		}
	}
}
