/*-------------------------------------------------------------------------

 정보표시윈도우

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 define
---------------------------------------------------------------------------*/
// 공유항로デバッグ
//#define	DEBUG_SHARE_ROUTES

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;

using directx;
using Utility;
using gvo_base;
using gvo_net_base;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class info_windows {
		// 서브윈도우
		enum window_index {
			speed,	// 속도윈도우
			position,   // 위치윈도우
			share,	// 항로공유윈도우
			interest,   // 이자からの경과일수윈도우
			build_ship, // 조선からの경과일수윈도우
			tcp_server, // TCP서버
		};

		private const int OFFSET_X = -2;

		private gvt_lib m_lib;
		private GvoDatabase m_db;
		private myship_info m_myship_info;

		private hittest_list m_windows;	  // 윈도우矩形관리
		private Point m_select_pos;  // 십자표시 (이전에 선택한) 위치
		private Point m_mouse_pos;	// 마우스위치

		// 背景색
		static private Color m_back_color = Color.FromArgb(220, 80, 80, 80);

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public info_windows(gvt_lib lib, GvoDatabase db, myship_info myship) {
			m_lib = lib;
			m_db = db;
			m_myship_info = myship;

			m_select_pos = new Point(0, 0);
			m_mouse_pos = new Point(0, 0);

			m_windows = new hittest_list();

			// 속도윈도우
			m_windows.Add(new hittest());
			// 위치윈도우
			m_windows.Add(new hittest());
			// 항로공유윈도우
			m_windows.Add(new hittest());
			// 이자からの경과일수윈도우
			m_windows.Add(new hittest());
			// 조선からの경과일수윈도우
			m_windows.Add(new hittest());
			// TCP서버
			m_windows.Add(new hittest());
		}

		/*-------------------------------------------------------------------------
		 업데이트
		 主に윈도우위치
		---------------------------------------------------------------------------*/
		public void Update(Point select_pos, Point mouse_pos) {
			// 표시용정보
			m_select_pos = select_pos;
			m_mouse_pos = mouse_pos;

			Size rect = new Size((int)m_lib.device.client_size.X, (int)m_lib.device.client_size.Y);

			// 속도윈도우
			{
				hittest ht = m_windows[(int)window_index.speed];
				Size size = new Size(43 + 4, (16 + 1) * 3);
				ht.rect = new Rectangle(rect.Width - size.Width - 3, 3, size.Width, size.Height);
			}

			// 위치윈도우
			{
				hittest ht = m_windows[(int)window_index.position];
				Size size = new Size(78, 14 * (4 + 1) + 2);
				ht.rect = new Rectangle(rect.Width - size.Width, rect.Height - size.Height, size.Width, size.Height);
			}

			// 항로공유윈도우
			{
				hittest ht = m_windows[(int)window_index.share];

				// 상태によっては비표시になる
				ht.enable = true;
#if !DEBUG_SHARE_ROUTES
				if (!m_lib.setting.enable_share_routes) ht.enable = false;
#endif
				Size size = new Size(78, 14 + 2);
				Rectangle prect = m_windows[(int)window_index.position].rect;
				ht.rect = new Rectangle(rect.Width - size.Width, prect.Y - size.Height - 3, size.Width, size.Height);
			}

			// 이자からの경과일수윈도우
			{
				hittest ht = m_windows[(int)window_index.interest];

				// 상태によっては비표시になる
				ht.enable = true;
				if (!m_lib.setting.enable_analize_log_chat) ht.enable = false;
				if (!m_lib.setting.save_searoutes) ht.enable = false;
				// 서버모드時は유효
				if (m_lib.setting.is_server_mode) ht.enable = true;

				Size size = new Size(78, 14 * 1 + 2);
				Rectangle prect = get_parent_window_rect((int)window_index.interest);
				ht.rect = new Rectangle(rect.Width - size.Width, prect.Y - size.Height - 3, size.Width, size.Height);
			}

			// 조선からの경과일수
			{
				hittest ht = m_windows[(int)window_index.build_ship];

				// 상태によっては비표시になる
				ht.enable = true;
				if (!m_lib.setting.enable_analize_log_chat) ht.enable = false;
				if (!m_lib.setting.save_searoutes) ht.enable = false;
				// 서버모드時は유효
				if (m_lib.setting.is_server_mode) ht.enable = true;
				if (!m_lib.setting.force_show_build_ship) {
					// 조선중でないときは비표시
					ht.enable = m_db.BuildShipCounter.IsNowBuild;
				}

				Size size = new Size(78, 14 * 1 + 2);
				Rectangle prect = get_parent_window_rect((int)window_index.build_ship);
				ht.rect = new Rectangle(rect.Width - size.Width, prect.Y - size.Height - 3, size.Width, size.Height);
			}

			// TCP서버
			{
				hittest ht = m_windows[(int)window_index.tcp_server];

				// 상태によっては비표시になる
				ht.enable = true;
				if (!m_lib.setting.is_server_mode) ht.enable = false;

				Size size = new Size(78, 14 + 2);
				Rectangle prect = get_parent_window_rect((int)window_index.tcp_server);
				ht.rect = new Rectangle(rect.Width - size.Width, prect.Y - size.Height - 3, size.Width, size.Height);
			}
		}

		/*-------------------------------------------------------------------------
		 基準となる윈도우矩形を得る
		 그리기대상の윈도우矩形を返す
		---------------------------------------------------------------------------*/
		private Rectangle get_parent_window_rect(int start_index) {
			start_index--;  // 1つ前の윈도우
			if (start_index <= (int)window_index.position) {
				return m_windows[(int)window_index.position].rect;
			}
			for (; start_index > (int)window_index.position; start_index--) {
				if (m_windows[start_index].enable) {
					return m_windows[start_index].rect;
				}
			}
			return m_windows[(int)window_index.position].rect;
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		public void Draw() {
			m_lib.device.systemfont.Begin();
			m_lib.device.sprites.BeginDrawSprites(m_lib.icons.texture);

			// 현재위치그리기
			draw_gpos();
			// 속도그리기
			draw_speed();
			// 항로공유
			draw_share();
			// 이자からの경과일수
			draw_interest();
			// 조선からの경과일수윈도우
			draw_build_ship();
			// TCP서버
			draw_tcp_server();

			m_lib.device.sprites.EndDrawSprites();
			m_lib.device.systemfont.End();
		}

		/*-------------------------------------------------------------------------
		 현재위치の그리기
		 ついでに拡縮率も描く
		---------------------------------------------------------------------------*/
		private void draw_gpos() {
			hittest ht = m_windows[(int)window_index.position];
			Vector3 pos = new Vector3(ht.rect.X, ht.rect.Y, 0.1f);
			Vector2 size = new Vector2(ht.rect.Width, ht.rect.Height);

			m_lib.device.DrawFillRect(pos, size, m_back_color.ToArgb());
			m_lib.device.DrawLineRect(pos, size, Color.Black.ToArgb());

			int pos_y = (int)pos.Y + 2 + 1;

			// 拡縮率 배율
			int scale = (int)Math.Round((double)m_lib.loop_image.ImageScale * 100);
			m_lib.device.systemfont.DrawTextR(String.Format("{0}%", scale),
									(int)pos.X + 75 + OFFSET_X, pos_y, Color.White);

			// 季節
			m_lib.device.sprites.AddDrawSpritesNC(new Vector3(pos.X + 6, pos_y, 0.1f), m_lib.icons.GetIcon(icons.icon_index.string04)); // 지도
			pos_y += 14 - 1;
			m_lib.device.sprites.AddDrawSpritesNC(new Vector3(pos.X + 6, pos_y, 0.1f), m_lib.icons.GetIcon(icons.icon_index.string03)); // 季節
			d3d_sprite_rects.rect _rect = m_lib.icons.GetIcon((m_db.GvoSeason.now_season == gvo_season.season.summer) ? icons.icon_index.string06 : icons.icon_index.string05);
			m_lib.device.sprites.AddDrawSpritesNC(new Vector3(pos.X + 75 + OFFSET_X - 12, pos_y, 0.1f), _rect);

			// 본인の배の위치
			pos_y += 14 - 1;
			string my_pos = "--- , ---";
			Point tmp_p = game_pos_2_map_pos_for_debug(m_myship_info.pos);
			if (m_myship_info.is_analized_pos) {
				my_pos = String.Format("{0} , {1}", tmp_p.X, tmp_p.Y);
			}
			m_lib.device.systemfont.DrawTextR(my_pos,
									(int)pos.X + 75 + OFFSET_X, pos_y, Color.White);

			// 좌클릭した위치
			pos_y += 14 - 1;
			tmp_p = game_pos_2_map_pos_for_debug(m_select_pos);
			m_lib.device.systemfont.DrawTextR(String.Format("{0} , {1}", tmp_p.X, tmp_p.Y),
									(int)pos.X + 75 + OFFSET_X, pos_y, Color.White);
			// 현재の마우스위치
			pos_y += 14 - 1;
			Point gpos = transform.client_pos2_game_pos(m_mouse_pos, m_lib.loop_image);
			tmp_p = game_pos_2_map_pos_for_debug(gpos);
			m_lib.device.systemfont.DrawTextR(String.Format("{0} , {1}", tmp_p.X, tmp_p.Y),
									(int)pos.X + 75 + OFFSET_X, pos_y, Color.White);
			{
				// 좌클릭した위치にクロスカーソルの표시
				// 지도좌표に변환
				Vector2 p = m_lib.loop_image.GlobalPos2LocalPos(transform.game_pos2_map_pos(transform.ToVector2(m_select_pos), m_lib.loop_image));
				// ループを考慮する
				p = m_lib.loop_image.AjustLocalPos(p);

				m_lib.device.sprites.AddDrawSpritesNC(new Vector3(p.X, p.Y, 0.3f),
										m_lib.icons.GetIcon(icons.icon_index.select_cross));
			}
		}

		/*-------------------------------------------------------------------------
		 デバッグ용に측량좌표계から지도좌표계に변환する
		 デバッグ플래그た立ってないときはそのまま返す
		---------------------------------------------------------------------------*/
		private Point game_pos_2_map_pos_for_debug(Point p) {
			// デバッグ플래그がない場合はそのまま返す
			if (!m_lib.setting.debug_flag_show_potision) return p;
			return transform.game_pos2_map_pos(p, m_lib.loop_image);
		}

		/*-------------------------------------------------------------------------
		 속도그리기
		 ついでに진행방향も그리기する
		---------------------------------------------------------------------------*/
		private void draw_speed() {
			hittest ht = m_windows[(int)window_index.speed];
			Vector3 pos = new Vector3(ht.rect.X, ht.rect.Y, 0.3f);

			// 枠
			// 속도용
			m_lib.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(icons.icon_index.speed_background));
			// 각도용
			Vector3 pos2 = pos;
			Vector3 pos3 = pos;
			pos3.Z += -0.1f;
			pos2.Y += 16 + 1;
			pos3.Y += 16 + 3;
			pos3.X += 33;
			m_lib.device.sprites.AddDrawSpritesNC(pos2, m_lib.icons.GetIcon(icons.icon_index.speed_background));
			m_lib.device.sprites.AddDrawSpritesNC(pos3, m_lib.icons.GetIcon(icons.icon_index.degree));
			pos2.Y += 16 + 1;
			pos3.Y += 16 + 1;
			m_lib.device.sprites.AddDrawSpritesNC(pos2, m_lib.icons.GetIcon(icons.icon_index.speed_background));
			m_lib.device.sprites.AddDrawSpritesNC(pos3, m_lib.icons.GetIcon(icons.icon_index.degree));

			// 각도정확도
			pos2.X += 1;
			pos2.Y += 1;
			pos2.Z -= 0.05f;
			m_lib.device.DrawFillRect(pos2, new Vector2(m_db.SpeedCalculator.angle_precision * (ht.rect.Width - 1 - 1), 13), Color.FromArgb(255, 160, 210, 255).ToArgb());
			//			m_lib.Device.DrawFillRect(pos2, new Vector2(0.5f * (ht.rect.Width-1-1), 13), Color.FromArgb(255, 160, 210, 255).ToArgb());

			int pos_x = (int)pos.X + ht.rect.Width - 2;

			// 속도
			m_lib.device.systemfont.DrawTextR(m_db.SpeedCalculator.speed_knot.ToString("0.00") + "Kt",
									pos_x, (int)pos.Y + 1 + 1, Color.Black);
			//			m_lib.Device.systemfont.DrawTextR(m_db.speed.speed_knot.ToString("0.00") + "Kt(" + m_db.speed.speed_map.ToString("0.00") + ")",
			//									pos_x, (int)pos.Y + 1+1, Color.Black);
			// 각도
			// 측량からの각도
			if (m_myship_info.angle >= 0) {
				m_lib.device.systemfont.DrawTextR(m_myship_info.angle.ToString("0.0"),
										pos_x - 12, (int)pos.Y + 1 + 1 + 16 + 1, Color.Black);
			}
			// 나침반からの각도
			if (m_db.SpeedCalculator.angle >= 0) {
				m_lib.device.systemfont.DrawTextR(m_db.SpeedCalculator.angle.ToString("0.0"),
										pos_x - 12, (int)pos.Y + 1 + 1 + 16 + 1 + 16 + 1, Color.Black);
			}
		}

		/*-------------------------------------------------------------------------
		 항로공유
		 윈도우
		---------------------------------------------------------------------------*/
		private void draw_share() {
			hittest ht = m_windows[(int)window_index.share];

			if (!ht.enable) return;  // 유효ではない

			Vector3 pos = new Vector3(ht.rect.X, ht.rect.Y, 0.1f);
			Vector2 size = new Vector2(ht.rect.Width, ht.rect.Height);

			m_lib.device.DrawFillRect(pos, size, m_back_color.ToArgb());
			m_lib.device.DrawLineRect(pos, size, Color.Black.ToArgb());

			m_lib.device.sprites.AddDrawSpritesNC(new Vector3(pos.X + 6, pos.Y + 3, 0.1f), m_lib.icons.GetIcon(icons.icon_index.string01));
			m_lib.device.systemfont.DrawTextR(String.Format("{0}", m_db.ShareRoutes.ShareList.Count),
									(int)pos.X + 75 + OFFSET_X, (int)pos.Y + 3, Color.White);
		}

		/*-------------------------------------------------------------------------
		 이자からの경과일수
		 윈도우
		---------------------------------------------------------------------------*/
		private void draw_interest() {
			hittest ht = m_windows[(int)window_index.interest];

			if (!ht.enable) return;  // 유효ではない

			Vector3 pos = new Vector3(ht.rect.X, ht.rect.Y, 0.1f);
			Vector2 size = new Vector2(ht.rect.Width, ht.rect.Height);

			m_lib.device.DrawFillRect(pos, size, m_back_color.ToArgb());
			m_lib.device.DrawLineRect(pos, size, Color.Black.ToArgb());

			m_lib.device.sprites.AddDrawSpritesNC(new Vector3(pos.X + 6, pos.Y + 3, 0.1f), m_lib.icons.GetIcon(icons.icon_index.string02));
			m_lib.device.sprites.AddDrawSpritesNC(new Vector3(pos.X + 6 + 75 + OFFSET_X - 13, pos.Y + 3, 0.1f), m_lib.icons.GetIcon(icons.icon_index.string00));
			m_lib.device.systemfont.DrawTextR(String.Format("{0}", m_db.InterestDays.GetDays()),
									(int)pos.X + 75 + OFFSET_X - 8, (int)pos.Y + 3, Color.White);
		}

		/*-------------------------------------------------------------------------
		 조선からの경과일수
		 윈도우
		---------------------------------------------------------------------------*/
		private void draw_build_ship() {
			hittest ht = m_windows[(int)window_index.build_ship];

			if (!ht.enable) return;  // 유효ではない

			Vector3 pos = new Vector3(ht.rect.X, ht.rect.Y, 0.1f);
			Vector2 size = new Vector2(ht.rect.Width, ht.rect.Height);

			m_lib.device.DrawFillRect(pos, size, m_back_color.ToArgb());
			m_lib.device.DrawLineRect(pos, size, Color.Black.ToArgb());

			m_lib.device.sprites.AddDrawSpritesNC(new Vector3(pos.X + 6, pos.Y + 3, 0.1f), m_lib.icons.GetIcon(icons.icon_index.string07));
			m_lib.device.sprites.AddDrawSpritesNC(new Vector3(pos.X + 6 + 75 + OFFSET_X - 13, pos.Y + 3, 0.1f), m_lib.icons.GetIcon(icons.icon_index.string00));
			m_lib.device.systemfont.DrawTextR(String.Format("{0}", m_db.BuildShipCounter.GetDays()),
									(int)pos.X + 75 + OFFSET_X - 8, (int)pos.Y + 3, Color.White);
		}

		/*-------------------------------------------------------------------------
		 TCP서버
		 윈도우
		---------------------------------------------------------------------------*/
		private void draw_tcp_server() {
			hittest ht = m_windows[(int)window_index.tcp_server];

			if (!ht.enable) return;  // 유효ではない

			Vector3 pos = new Vector3(ht.rect.X, ht.rect.Y, 0.1f);
			Vector2 size = new Vector2(ht.rect.Width, ht.rect.Height);

			m_lib.device.DrawFillRect(pos, size, m_back_color.ToArgb());
			m_lib.device.DrawLineRect(pos, size, Color.Black.ToArgb());

			m_lib.device.systemfont.DrawText("TCP SERVER",
									(int)pos.X + 3, (int)pos.Y + 3, Color.White);
		}

		/*-------------------------------------------------------------------------
		 ヒットテスト
		---------------------------------------------------------------------------*/
		public bool HitTest(Point point) {
			switch (m_windows.HitTest_Index(point)) {
				case (int)window_index.position: return true;
				case (int)window_index.speed: return true;
				case (int)window_index.interest: return true;
				case (int)window_index.share: return true;
				case (int)window_index.tcp_server: return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 마우스클릭
		 ダイア로그표시용に form を必要とする
		---------------------------------------------------------------------------*/
		public bool OnMouseClick(Point point, MouseButtons mouseButtons, Form form) {
			switch (m_windows.HitTest_Index(point)) {
				case (int)window_index.position: return true;
				case (int)window_index.speed: return true;
				case (int)window_index.tcp_server: return true;

				case (int)window_index.share:
					if ((mouseButtons & MouseButtons.Left) == 0) return true;
					// 좌클릭
					using (share_routes_form dlg = new share_routes_form(m_db.ShareRoutes.ShareList)) {
						if (dlg.ShowDialog(form) == DialogResult.OK) {
							if (dlg.is_selected) {
								// 선택されてる장소をセンタリングする
								m_lib.setting.centering_gpos = dlg.selected_position;
								m_lib.setting.req_centering_gpos.Request();
								// 선택한のに비표시だとあれなので強制표시
								m_lib.setting.draw_share_routes = true;
							}
						}
					}
					return true;
				case (int)window_index.build_ship:
					if ((mouseButtons & MouseButtons.Right) == 0) return true;
					// 우클릭
					if (MessageBox.Show("조선카운터를 리셋합니까?", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
						m_db.BuildShipCounter.FinishBuildShip();
					}
					return true;
			}

			return false;
		}

		/*-------------------------------------------------------------------------
		 ツールチップ용문자열を得る
		---------------------------------------------------------------------------*/
		public string OnToolTipString(Point pos) {
			switch (m_windows.HitTest_Index(pos)) {
				case (int)window_index.position: {
						string str = "지도拡縮率\n季節(";
						str += m_db.GvoSeason.next_season_start_shortstr;
						str += "まで)\n본인 배의 위치\n십자표시の위치\n마우스の위치";
						if (m_lib.setting.debug_flag_show_potision) {
							str += "\n(デバッグ플래그유효)";
						}
						return str;
					}
				case (int)window_index.speed: return "속도\n나침반から분석した각도\n측량から분석した각도";
				case (int)window_index.share: {
						string str = "";
						foreach (ShareRoutes.ShareShip s in m_db.ShareRoutes.ShareList) {
							if (str != "") str += "\n";
							str += s.Name;
						}
						if (str == "") str = "항로공유メンバーが居ません";
						return str;
					}
				case (int)window_index.interest: return m_db.InterestDays.GetPopupString();
				case (int)window_index.build_ship: {
						return m_db.BuildShipCounter.GetPopupString() + "\n(우클릭で리셋)";
					}
				case (int)window_index.tcp_server: {
						gvo_server_service server = m_myship_info.server_service;
						gvo_tcp_client client = server.GetClient();
						if (client == null) {
							if (server.is_listening) {
								return "TCP서버모드\n연결대기";
							} else {
								return "TCP서버모드\n서버の시작に실패";
							}
						} else {
							return "TCP서버모드\n연결済\n通信대상:" + client.remote_ep.ToString();
						}
					}
			}
			return null;
		}
	}
}
