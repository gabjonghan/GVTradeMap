/*-------------------------------------------------------------------------

 본인の배정보
 정보取得はLocalPCもしくはナビゲーションクライアントから行う
 일付しか得られないときの위치予測표시付き

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;

using directx;
using gvo_base;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Utility;
using win32;

using net_base;
using gvo_net_base;
using System.Drawing;
using System.Windows.Forms;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class myship_info {
		// ナビゲーションクライアントからの受信タイムアウト
		// タイムアウト時は나침반の각도を初期化する
		// 항로공유時は해상ではない扱いとなる
		private const int FROM_CLIENT_RECEIVE_TIME_OUT = 1000 * 5;  // 5초
																	// 해상ではないと判断する시간
																	// この시간일付が캡처できなければ해상ではないと判断する
																	// 도달예상위치の리셋も絡むため, 少し長めに설정する
		private const int OUT_OF_SEA_TIME_OUT = 1000 * 5;   // 5초
															// 도달예상위치
		private const float STEP_POSITION_SPEED_MIN = 1;		// なにも표시しない속도
		private const float STEP_POSITION_SPEED_MIN1 = 2.8f;		// 5일간격での표시になる속도
		private const int STEP_POSITION_DAYS_MAX = 50;	  // 50일분まで표시
															// 진행방향선の長さ
		private const float ANGLE_LINE_LENGTH = 3000;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private gvt_lib m_lib;				  // 
		private GvoDatabase m_db;				   // 


		private Point m_pos;					// 본인の배위치
												// 항로공유時にこの値を渡す
		private float m_angle;			  // 본인の배の向き
		private bool m_is_in_the_sea;	   // 해상のときtrue
											//		private float				m_show_speed;			// 도달예상표시アニメーション용속도

		private gvo_server_service m_server_service;		// ナビゲーションクライアントからの受信

		private DateTimer m_capture_timer;	  // 캡처간격용
		private int m_capture_interval;	 // 캡처간격
		private DateTimer m_expect_pos_timer;	   // 예상위치계산용
		private DateTimer m_expect_delay_timer; // 예상위치消去용ディレイタイマ

		private Vector2 m_expect_vector;		// 예상위치용각도
		private bool m_is_draw_expect_pos;  // 예상위치を그리기するときtrue

		private bool m_capture_sucess;	  // 측량위치を得たときtrue
											// UpdateDomains()を呼ぶ毎にfalseにされ, 得られたときのみtrueになる

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public Point pos { get { return m_pos; } }
		public float angle { get { return m_angle; } }
		public gvo_server_service server_service { get { return m_server_service; } }
		public bool is_in_the_sea { get { return m_is_in_the_sea; } }
		public bool is_analized_pos {
			get {
				if (m_pos.X < 0) return false;
				if (m_pos.Y < 0) return false;
				return true;
			}
		}
		private bool is_draw_expect_pos {
			get {
				if (!m_is_draw_expect_pos) return false;
				if (m_expect_vector == Vector2.Empty) return false;
				return true;
			}
		}
		public bool capture_sucess { get { return m_capture_sucess; } }

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public myship_info(gvt_lib lib, GvoDatabase db) {
			m_lib = lib;
			m_db = db;

			m_pos = new Point(-1, -1);
			m_angle = -1;
			m_is_in_the_sea = false;
			// 도달예상アニメーション용속도
			//			m_show_speed			= 0;

			m_server_service = new gvo_server_service();		// ナビゲーションクライアントからの受信
			m_capture_timer = new DateTimer();			  // 캡처간격용
			m_expect_pos_timer = new DateTimer();			   // 예상위치계산용
			m_expect_delay_timer = new DateTimer();			 // 예상위치消去용ディレイタイマ

			m_capture_sucess = false;

			// 도달예상위치を리셋
			reset_expect();
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private void reset_expect() {
			m_expect_vector = Vector2.Empty;
			m_is_draw_expect_pos = false;
			m_expect_pos_timer.StartSection();
		}

		/*-------------------------------------------------------------------------
		 업데이트
		---------------------------------------------------------------------------*/
		public void Update() {
			m_capture_sucess = false;

			// 서버업데이트
			update_server();

			// 캡처간격の업데이트
			update_capture_interval();

			// 정보の업데이트
			if (m_lib.setting.is_server_mode) {
				// ナビゲーションクライアントからの受信
				do_receive_client();
			} else {
				// 화면캡처
				do_capture();
				// 로그からの해역변동업데이트
				m_db.GvoChat.UpdateSeaArea_DoRequest();
			}
		}

		/*-------------------------------------------------------------------------
		 서버업데이트
		---------------------------------------------------------------------------*/
		private void update_server() {
			if (m_lib.setting.is_server_mode) {
				// 서버모드
				// 시작していなければ시작する
				if (!m_server_service.is_listening) {
					m_server_service.Listen(m_lib.setting.port_index);
				}
			} else {
				// 통상모드
				// 시작していれば종료する
				m_server_service.Close();
			}
		}

		/*-------------------------------------------------------------------------
		 화면캡처간격を업데이트する
		---------------------------------------------------------------------------*/
		private void update_capture_interval() {
			switch (m_lib.setting.capture_interval) {
				case CaptureIntervalIndex.Per250ms:
					m_capture_interval = 250;
					break;
				case CaptureIntervalIndex.Per500ms:
					m_capture_interval = 500;
					break;
				case CaptureIntervalIndex.Per2000ms:
					m_capture_interval = 2000;
					break;
				case CaptureIntervalIndex.Per1000ms:
				default:
					m_capture_interval = 1000;
					break;
			}
		}

		/*-------------------------------------------------------------------------
		 캡처
		---------------------------------------------------------------------------*/
		private void do_capture() {
			// 前회の캡처からの경과시간を得る
			int sectiontime = m_capture_timer.GetSectionTimeMilliseconds();
			if (sectiontime < m_capture_interval) {
				// 캡처간격ではない
				return;
			}
			// 간격測定리셋
			m_capture_timer.StartSection();

			// 정보업데이트
			update_myship_data(sectiontime, get_myship_data());
		}

		/*-------------------------------------------------------------------------
		 본인の배の정보を得る
		 캡처関係の정보
		 ある時点でのスナップショットとなる
		---------------------------------------------------------------------------*/
		private gvo_analized_data get_myship_data() {
			// 항로기록なしならなにもしない
			if (!m_lib.setting.save_searoutes) return null;

			// 캡처방법の지정
			if (m_lib.setting.windows_vista_aero) m_db.Capture.capture_mode = gvo_capture.mode.vista;
			else m_db.Capture.capture_mode = gvo_capture.mode.xp;

			// 화면캡처
			// 일수, 측량좌표, 나침반の각도
			m_db.Capture.CaptureAll();

			// 정보を구축して返す
			return gvo_analized_data.FromAnalizedData(m_db.Capture, m_db.GvoChat);
		}

		/*-------------------------------------------------------------------------
		 クライアントからの受信チェック
		---------------------------------------------------------------------------*/
		private void do_receive_client() {
			// 캡처정보を受信したか조사
			gvo_analized_data data = get_received_capture_data();

			if (data == null) {
				if (m_capture_timer.GetSectionTimeMilliseconds() >= FROM_CLIENT_RECEIVE_TIME_OUT) {
					// 受信タイムアウト
					// 해상に居ないと判断する
					m_is_in_the_sea = false;
					// 도달예상を리셋
					reset_expect();
				}
				return;
			}

			// 前회からの경과시간を得る
			int sectiontime = m_capture_timer.GetSectionTimeMilliseconds();
			// 간격測定리셋
			m_capture_timer.StartSection();

			// 정보업데이트
			update_myship_data(sectiontime, data);
		}

		/*-------------------------------------------------------------------------
		 受信정보を得る
		 캡처関係の정보
		 해역변동はこのメソッド내で업데이트してしまう
		---------------------------------------------------------------------------*/
		private gvo_analized_data get_received_capture_data() {
			gvo_tcp_client client = m_server_service.GetClient();
			if (client == null) return null;

			// 해역변동
			gvo_map_cs_chat_base.sea_area_type[] sea_info = client.sea_info;		// コピーを得る
			if (sea_info != null) {
				foreach (gvo_map_cs_chat_base.sea_area_type i in sea_info) {
					switch (i.type) {
						case gvo_map_cs_chat_base.sea_type.safty:
							m_db.SeaArea.SetType(i.name, sea_area.sea_area_once.sea_type.safty);
							break;
						case gvo_map_cs_chat_base.sea_type.lawless:
							m_db.SeaArea.SetType(i.name, sea_area.sea_area_once.sea_type.lawless);
							break;
						default:
							m_db.SeaArea.SetType(i.name, sea_area.sea_area_once.sea_type.normal);
							break;
					}
				}
			}

			gvo_analized_data data = client.capture_data;   // コピーを得る
															// 캡처정보を受信していれば정보を返す
			if (data.capture_days_success || data.capture_success) {
				return data;
			}
			// 정보を受信していない
			return null;
		}

		/*-------------------------------------------------------------------------
		 본인の배の정보업데이트
		 クライアントから受信した정보か自ら得た정보かは考慮されない
		---------------------------------------------------------------------------*/
		private void update_myship_data(int sectiontime, gvo_analized_data data) {
			// 
			if (data == null) return;

			// 항로기록なしならなにもしない
			if (!m_lib.setting.save_searoutes) {
				// 위치と각도は初期化する
				m_pos = new Point(-1, -1);
				m_is_in_the_sea = false;
				// 도달예상を리셋
				reset_expect();
				return;
			}

			// 경과시간だけ추가
			m_db.SpeedCalculator.AddIntervalOnly(sectiontime);

			// 조선개시と종료
			// 同時に플래그が立った場合は종료を優先する
			if (data.is_start_build_ship) {
				m_db.BuildShipCounter.StartBuildShip(data.build_ship_name);
			}
			if (data.is_finish_build_ship) {
				m_db.BuildShipCounter.FinishBuildShip();
			}

			// 일付캡처成功なら이자からの일수を업데이트する
			if (data.capture_days_success) {
				// 이자からの일수を업데이트する
				m_db.InterestDays.Update(data.days, data.interest);
				// 조선일수を업데이트する
				m_db.BuildShipCounter.Update(data.days);
			} else {
				if (m_expect_delay_timer.GetSectionTimeMilliseconds() > OUT_OF_SEA_TIME_OUT) {
					// 해상ではない
					m_is_in_the_sea = false;
					// 도달예상を리셋
					reset_expect();
				}
				// 일付캡처실패ならそれ以외の분석を行わない
				return;
			}

			// 캡처が成功したかチェック
			if (!data.capture_success) {
				// 일付のみ캡처成功
				m_is_draw_expect_pos = true;		// 도달예상위치を그리기する必要あり
													// ディレイタイマ리셋
				m_expect_delay_timer.StartSection();
				return;
			}

			// 본인の배の위치
			m_pos = new Point(data.pos_x, data.pos_y);
			m_angle = data.angle;
			m_is_in_the_sea = true;	 // 해상
			m_capture_sucess = true;		// 캡처成功

			// 측량위치を추가する
			// 위치によっては추가されない
			m_db.SeaRoute.AddPoint(m_pos,
									data.days,
									gvo_chat.ToIndex(data.accident));

			// 속도算出
			// 업데이트간격はすでに설정済み
			m_db.SpeedCalculator.Add(m_pos, 0);

			// 캡처時は그리기を스킵しない
			m_lib.device.SetMustDrawFlag();

			// 도달위치예상용각도の업데이트
			m_expect_vector = transform.ToVector2(gvo_capture.AngleToVector(m_angle));
			// タイマ리셋
			m_expect_pos_timer.StartSection();
			m_expect_delay_timer.StartSection();
			m_is_draw_expect_pos = false;   // 분석できてるので예상위치を그리기する必要がない
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		public void Draw() {
			if (!is_analized_pos) return;

			m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_myship_proc), ANGLE_LINE_LENGTH);
		}

		/*-------------------------------------------------------------------------
		 본인 배 그리기
		---------------------------------------------------------------------------*/
		private void draw_myship_proc(Vector2 offset, LoopXImage image) {
			// 지도좌표로 변환
			Vector2 pos = image.GlobalPos2LocalPos(transform.game_pos2_map_pos(transform.ToVector2(m_pos), m_lib.loop_image), offset);

			// 각도
			draw_angle_line_all(pos, image);

			m_lib.device.sprites.BeginDrawSprites(m_lib.icons.texture);
			// 도달예상위치
			int color = -1;
			if (m_lib.setting.draw_setting_myship_expect_pos) {
				if (draw_expect_pos(pos, m_db.SpeedCalculator.speed_map)) {
					color = Color.FromArgb(160, 255, 255, 255).ToArgb();
				}
			}
			// 본인 배의 위치
			m_lib.device.sprites.AddDrawSpritesNC(new Vector3(pos.X, pos.Y, 0.3f + 0.001f), m_lib.icons.GetIcon(icons.icon_index.myship), color);
			m_lib.device.sprites.EndDrawSprites();
		}

		/*-------------------------------------------------------------------------
		 각도선을 그리기
		---------------------------------------------------------------------------*/
		private void draw_angle_line_all(Vector2 pos, LoopXImage image) {
			// 
			if (!m_lib.setting.draw_myship_angle) return;

			DrawSettingMyShipAngle flag = m_lib.setting.draw_setting_myship_angle;

			if (is_in_the_sea && (flag & DrawSettingMyShipAngle.draw_1) != 0) {
				// 측량으로부터
				// 정확도가 높을수록 불투명하게 그리게 함
				// 정확도가 낮으면 거의 보이지 않음
				int alpha = (int)((95f + 150f * (m_db.SpeedCalculator.angle_precision)));
				Color color = Color.FromArgb(alpha, 255, 255, 255);

				draw_angle_line(pos, image, m_db.SpeedCalculator.angle, color, 2.0f);

				// 속도로부터 구해진 도달예상위치
				if (m_lib.setting.draw_setting_myship_angle_with_speed_pos) {
					draw_step_position2(pos,										image,
										m_db.SpeedCalculator.angle,										m_db.SpeedCalculator.speed_map,
										color);
				}
			}
			if ((is_in_the_sea)
				&& ((flag & DrawSettingMyShipAngle.draw_0) != 0)) {
				// 나침반으로부터
				Color color = (is_draw_expect_pos) ? Color.Tomato : Color.Black;
				draw_angle_line(pos, image, m_angle, color, 1.25f);

				// 속도로부터 구해진 도달예상위치
				if (m_lib.setting.draw_setting_myship_angle_with_speed_pos) {
					draw_step_position2(pos,										image,
										m_angle,										m_db.SpeedCalculator.speed_map, 
										color);
				}
			}
		}

		/*-------------------------------------------------------------------------
		 현재위치로부터 현재속도로 진행하였을때 도달하는 위치그리기
		 화살표와 같다고 보면 됨
		---------------------------------------------------------------------------*/
		private void draw_step_position2(Vector2 pos, LoopXImage image, float angle, float speed, Color baseColor) {
			if (angle < 0) return;
			float speed_knot = speed_calculator.MapToKnotSpeed(speed);  // knotに변환
			if (speed_knot < STEP_POSITION_SPEED_MIN) return;	   // 최저속도

			// 표시용の속도を求める
			speed = transform_speed(speed, image);

			m_lib.device.device.RenderState.ZBufferEnable = false;

			m_lib.device.line.Width = 1;
			m_lib.device.line.Antialias = m_lib.setting.enable_line_antialias;
			m_lib.device.line.Pattern = -1;
			m_lib.device.line.PatternScale = 1.0f;
			m_lib.device.line.Begin();

			float length = 0;
			int count = 1;
			int days = 0;
			int days2 = 0;
			Vector2 vec = transform.ToVector2(gvo_capture.AngleToVector(angle));
			Vector2 vec2 = new Vector2(vec.Y, -vec.X);  // 90도회전させたベクトル
			Vector2[] points = new Vector2[3];

			Color red = Color.FromArgb(baseColor.A, Math.Min(Color.Red.R + baseColor.R, 255), Math.Min(Color.Red.G + baseColor.G, 255), Math.Min(Color.Red.B + baseColor.B, 255));
			Color tomato = Color.FromArgb(baseColor.A, Math.Min(Color.Tomato.R + baseColor.R, 255), Math.Min(Color.Tomato.G + baseColor.G, 255), Math.Min(Color.Tomato.B + baseColor.B, 255));

			float max = ANGLE_LINE_LENGTH * image.ImageScale;
			while (length < max) {
				Vector2 step = vec * (speed * count);
				Vector2 step2 = vec * ((speed * count) - 4.5f);
				Color color = baseColor;
				float l = 2.5f;

				if (++days >= 5) {
					// 5일区切りで색を変える
					if (++days2 >= 2) {
						// 10일区切り
						color = red;
						days2 = 0;
						l = 4;
					} else {
						color = tomato;
						l = 3;
					}
					days = 0;

					points[0] = pos + step2 + (vec2 * l);
					points[1] = pos + step;
					points[2] = pos + step2 + (vec2 * -l);
					m_lib.device.line.Draw(points, color);
				} else {
					// 1일간격
					// 속도によっては표시されない
					if (speed_knot > STEP_POSITION_SPEED_MIN1) {
						points[0] = pos + step2 + (vec2 * l);
						points[1] = pos + step;
						points[2] = pos + step2 + (vec2 * -l);
						m_lib.device.line.Draw(points, color);
					}
				}

				length += speed;
				// 일수제한
				if (++count > STEP_POSITION_DAYS_MAX) break;
			}
			m_lib.device.line.End();
			//

			m_lib.device.device.RenderState.ZBufferEnable = true;
		}

		/*-------------------------------------------------------------------------
		 속도を1일に進んだ距離に변환する
		 지도の拡縮が考慮される
		---------------------------------------------------------------------------*/
		private float transform_speed(float speed, LoopXImage image) {
			speed = update_step_position_speed(speed);
			// 맵좌표계に변환
			// 측량좌표の세로가로比と지도좌표の세로가로比が異なる場合おかしくなるので注意
			speed *= transform.get_rate_to_map_x(image);
			// 표시스케일を考慮
			speed *= image.ImageScale;

			return speed;
		}

		/*-------------------------------------------------------------------------
		 표시용속도
		 속도と표시용속도の差が대きいほど대きく속도に近づき
		 差が소さいときはゆっくり속도に近づく
		---------------------------------------------------------------------------*/
		private float update_step_position_speed(float speed_map) {
			speed_map *= 60;										// 1분間に進める距離
																	//
			return speed_map;
			//
			/*
						float	gap	= Math.Abs(speed_map - m_show_speed);

						gap	*= gap;						// ギャップが대きい程速く近づく
						if(gap > 300)	gap	= 300;		// 최대ギャップ
						gap		*= 1f/(30);				// 
						if(gap < 0.4f)	gap	= 0.4f;		// 도달속도が遅すぎる場合の조정する

						if(speed_map > m_show_speed){
							m_show_speed	+= gap;
							if(m_show_speed > speed_map)	m_show_speed = speed_map;
						}else{
							m_show_speed	-= gap;
							if(m_show_speed < speed_map)	m_show_speed = speed_map;
						}
						return m_show_speed;
			*/
		}

		/*-------------------------------------------------------------------------
		 진행방향の그리기
		---------------------------------------------------------------------------*/
		private void draw_angle_line(Vector2 pos, LoopXImage image, float angle, Color color, float width = 1.0f) {
			if (angle < 0) return;

			m_lib.device.device.RenderState.ZBufferEnable = false;

			m_lib.device.line.Width = width;
			m_lib.device.line.Antialias = m_lib.setting.enable_line_antialias;
			m_lib.device.line.Pattern = -1;
			m_lib.device.line.PatternScale = 1.0f;
			m_lib.device.line.Begin();

			Vector2 vec = transform.ToVector2(gvo_capture.AngleToVector(angle));
			Vector2[] points = new Vector2[2];
			points[0] = pos;
			points[1] = pos + (vec * (ANGLE_LINE_LENGTH * image.ImageScale));
			m_lib.device.line.Draw(points, color);

			m_lib.device.line.End();
			m_lib.device.device.RenderState.ZBufferEnable = true;
		}

		/*-------------------------------------------------------------------------
		 도달예상위치
		 도달예상위치를 그리기시작하면 true를 반환
		---------------------------------------------------------------------------*/
		private bool draw_expect_pos(Vector2 pos, float speed) {
			// 표시する必要があるかチェック
			if (!is_draw_expect_pos) return false;
			// 최저속도
			if (speed_calculator.MapToKnotSpeed(speed) < STEP_POSITION_SPEED_MIN) return false;

			// 경과시간
			int sectiontime = m_expect_pos_timer.GetSectionTimeMilliseconds();

			// 표시용の속도を求める
			speed = transform_speed(speed, m_lib.loop_image);
			speed *= 1f / 60;				   // 1초동안의 거리를 변환
			speed *= 1f / 1000;				 // sectiontime에 합침(1sec=1000)
			speed *= sectiontime;			   // 경과시간분 거리를 진행
			pos += m_expect_vector * speed; // 이동벡터를 업데이트

			// 예상위치그리기
			m_lib.device.sprites.AddDrawSpritesNC(new Vector3(pos.X, pos.Y, 0.3f), m_lib.icons.GetIcon(icons.icon_index.myship));
			return true;
		}
	}
}
