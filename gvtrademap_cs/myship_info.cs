/*-------------------------------------------------------------------------

 본인 배 정보
 정보 취득은 LocalPC 또는 내비게이션 클라이언트로부터 수행한다
 날짜만 얻을 수 있을 때의 위치 예측 표시 포함

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
		// 내비게이션 클라이언트로부터의 수신 타임아웃
		// 타임아웃 시에는 나침반 각도를 초기화한다
		// 항로 공유 시에는 해상이 아닌 것으로 취급한다
		private const int FROM_CLIENT_RECEIVE_TIME_OUT = 1000 * 5;  // 5초
																	// 해상이 아니라고 판단하는 시간
																	// 이 시간 동안 날짜를 캡처하지 못하면 해상이 아니라고 판단한다
																	// 도달 예상 위치 리셋도 얽혀 있으므로 조금 길게 설정한다
		private const int OUT_OF_SEA_TIME_OUT = 1000 * 5;   // 5초
															// 도달예상위치
		private const float STEP_POSITION_SPEED_MIN = 1;		// 아무것도 표시하지 않는 속도
		private const float STEP_POSITION_SPEED_MIN1 = 2.8f;		// 5 일 간격으로 표시하게 되는 속도
		private const int STEP_POSITION_DAYS_MAX = 50;	  // 50 일분까지 표시
															// 진행 방향선의 길이
		private const float ANGLE_LINE_LENGTH = 3000;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private gvt_lib m_lib;				  // 
		private GvoDatabase m_db;				   // 


		private Point m_pos;					// 본인 배 위치
												// 항로 공유 시에 이 값을 넘긴다
		private float m_angle;			  // 본인 배의 방향
		private bool m_is_in_the_sea;	   // 해상일 때 true
											//		도달 예상 표시 애니메이션용 속도

		private gvo_server_service m_server_service;		// 내비게이션 클라이언트로부터의 수신

		private DateTimer m_capture_timer;	  // 캡처간격용
		private int m_capture_interval;	 // 캡처간격
		private DateTimer m_expect_pos_timer;	   // 예상위치계산용
		private DateTimer m_expect_delay_timer; // 예상 위치 소거용 딜레이 타이머

		private Vector2 m_expect_vector;		// 예상위치용각도
		private bool m_is_draw_expect_pos;  // 예상 위치를 그릴 때 true

		private bool m_capture_sucess;	  // 측량 위치를 얻었을 때 true
											// UpdateDomains() 를 호출할 때마다 false 가 되고, 얻었을 때만 true 가 된다

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
			// 도달 예상 애니메이션용 속도
			//			m_show_speed			= 0;

			m_server_service = new gvo_server_service();		// 내비게이션 클라이언트로부터의 수신
			m_capture_timer = new DateTimer();			  // 캡처간격용
			m_expect_pos_timer = new DateTimer();			   // 예상위치계산용
			m_expect_delay_timer = new DateTimer();			 // 예상 위치 소거용 딜레이 타이머

			m_capture_sucess = false;

			// 도달 예상 위치를 리셋
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

			// 캡처 간격 갱신
			update_capture_interval();

			// 정보 갱신
			if (m_lib.setting.is_server_mode) {
				// 내비게이션 클라이언트로부터의 수신
				do_receive_client();
			} else {
				// 화면캡처
				do_capture();
				// 로그로부터의 해역 변동 갱신
				m_db.GvoChat.UpdateSeaArea_DoRequest();
			}
		}

		/*-------------------------------------------------------------------------
		 서버업데이트
		---------------------------------------------------------------------------*/
		private void update_server() {
			if (m_lib.setting.is_server_mode) {
				// 서버모드
				// 시작하지 않았으면 시작한다
				if (!m_server_service.is_listening) {
					m_server_service.Listen(m_lib.setting.port_index);
				}
			} else {
				// 통상모드
				// 시작했으면 종료한다
				m_server_service.Close();
			}
		}

		/*-------------------------------------------------------------------------
		 화면 캡처 간격을 갱신한다
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
			// 지난번 캡처로부터의 경과 시간을 얻는다
			int sectiontime = m_capture_timer.GetSectionTimeMilliseconds();
			if (sectiontime < m_capture_interval) {
				// 캡처 간격이 아니다
				return;
			}
			// 간격 측정 리셋
			m_capture_timer.StartSection();

			// 정보업데이트
			update_myship_data(sectiontime, get_myship_data());
		}

		/*-------------------------------------------------------------------------
		 본인 배의 정보를 얻는다
		 캡처 관련 정보
		 어느 시점의 스냅샷이 된다
		---------------------------------------------------------------------------*/
		private gvo_analized_data get_myship_data() {
			// 항로 기록이 없으면 아무것도 하지 않는다
			if (!m_lib.setting.save_searoutes) return null;

			// 캡처 방법 지정
			if (m_lib.setting.windows_vista_aero) m_db.Capture.capture_mode = gvo_capture.mode.vista;
			else m_db.Capture.capture_mode = gvo_capture.mode.xp;

			// 화면캡처
			// 일수, 측량 좌표, 나침반 각도
			m_db.Capture.CaptureAll();

			// 정보를 구축해서 반환한다
			return gvo_analized_data.FromAnalizedData(m_db.Capture, m_db.GvoChat);
		}

		/*-------------------------------------------------------------------------
		 클라이언트로부터의 수신 확인
		---------------------------------------------------------------------------*/
		private void do_receive_client() {
			// 캡처 정보를 수신했는지 조사
			gvo_analized_data data = get_received_capture_data();

			if (data == null) {
				if (m_capture_timer.GetSectionTimeMilliseconds() >= FROM_CLIENT_RECEIVE_TIME_OUT) {
					// 수신 타임아웃
					// 해상에 있지 않다고 판단한다
					m_is_in_the_sea = false;
					// 도달 예상을 리셋
					reset_expect();
				}
				return;
			}

			// 지난번부터의 경과 시간을 얻는다
			int sectiontime = m_capture_timer.GetSectionTimeMilliseconds();
			// 간격 측정 리셋
			m_capture_timer.StartSection();

			// 정보업데이트
			update_myship_data(sectiontime, data);
		}

		/*-------------------------------------------------------------------------
		 수신 정보를 얻는다
		 캡처 관련 정보
		 해역 변동은 이 메서드 안에서 갱신해 버린다
		---------------------------------------------------------------------------*/
		private gvo_analized_data get_received_capture_data() {
			gvo_tcp_client client = m_server_service.GetClient();
			if (client == null) return null;

			// 해역변동
			gvo_map_cs_chat_base.sea_area_type[] sea_info = client.sea_info;		// 복사본을 얻는다
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

			gvo_analized_data data = client.capture_data;   // 복사본을 얻는다
															// 캡처 정보를 수신했으면 정보를 반환한다
			if (data.capture_days_success || data.capture_success) {
				return data;
			}
			// 정보를 수신하지 않았다
			return null;
		}

		/*-------------------------------------------------------------------------
		 본인 배의 정보 갱신
		 클라이언트에서 수신한 정보인지 직접 얻은 정보인지는 고려하지 않는다
		---------------------------------------------------------------------------*/
		private void update_myship_data(int sectiontime, gvo_analized_data data) {
			// 
			if (data == null) return;

			// 항로 기록이 없으면 아무것도 하지 않는다
			if (!m_lib.setting.save_searoutes) {
				// 위치와 각도는 초기화한다
				m_pos = new Point(-1, -1);
				m_is_in_the_sea = false;
				// 도달 예상을 리셋
				reset_expect();
				return;
			}

			// 경과 시간만 추가
			m_db.SpeedCalculator.AddIntervalOnly(sectiontime);

			// 조선 시작과 종료
			// 동시에 플래그가 선 경우에는 종료를 우선한다
			if (data.is_start_build_ship) {
				m_db.BuildShipCounter.StartBuildShip(data.build_ship_name);
			}
			if (data.is_finish_build_ship) {
				m_db.BuildShipCounter.FinishBuildShip();
			}

			// 날짜 캡처에 성공했으면 이자로부터의 일수를 갱신한다
			if (data.capture_days_success) {
				// 이자로부터의 일수를 갱신한다
				m_db.InterestDays.Update(data.days, data.interest);
				// 조선 일수를 갱신한다
				m_db.BuildShipCounter.Update(data.days);
			} else {
				if (m_expect_delay_timer.GetSectionTimeMilliseconds() > OUT_OF_SEA_TIME_OUT) {
					// 해상이 아니다
					m_is_in_the_sea = false;
					// 도달 예상을 리셋
					reset_expect();
				}
				// 날짜 캡처에 실패하면 그 외의 분석은 하지 않는다
				return;
			}

			// 캡처가 성공했는지 확인
			if (!data.capture_success) {
				// 날짜만 캡처 성공
				m_is_draw_expect_pos = true;		// 도달 예상 위치를 그릴 필요 있음
													// 딜레이 타이머 리셋
				m_expect_delay_timer.StartSection();
				return;
			}

			// 본인 배의 위치
			m_pos = new Point(data.pos_x, data.pos_y);
			m_angle = data.angle;
			m_is_in_the_sea = true;	 // 해상
			m_capture_sucess = true;		// 캡처 성공

			// 측량 위치를 추가한다
			// 위치에 따라서는 추가되지 않는다
			m_db.SeaRoute.AddPoint(m_pos,
									data.days,
									gvo_chat.ToIndex(data.accident));

			// 속도 산출
			// 갱신 간격은 이미 설정되어 있다
			m_db.SpeedCalculator.Add(m_pos, 0);

			// 캡처 시에는 그리기를 스킵하지 않는다
			m_lib.device.SetMustDrawFlag();

			// 도달 위치 예상용 각도 갱신
			m_expect_vector = transform.ToVector2(gvo_capture.AngleToVector(m_angle));
			// 타이머 리셋
			m_expect_pos_timer.StartSection();
			m_expect_delay_timer.StartSection();
			m_is_draw_expect_pos = false;   // 분석이 되고 있으므로 예상 위치를 그릴 필요가 없다
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
			float speed_knot = speed_calculator.MapToKnotSpeed(speed);  // knot 로 변환
			if (speed_knot < STEP_POSITION_SPEED_MIN) return;	   // 최저속도

			// 표시용 속도를 구한다
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
			Vector2 vec2 = new Vector2(vec.Y, -vec.X);  // 90 도 회전시킨 벡터
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
					// 5 일 단위로 색을 바꾼다
					if (++days2 >= 2) {
						// 10 일 단위
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
					// 속도에 따라서는 표시되지 않는다
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
		 속도를 하루에 나아간 거리로 변환한다
		 지도의 확대축소가 고려된다
		---------------------------------------------------------------------------*/
		private float transform_speed(float speed, LoopXImage image) {
			speed = update_step_position_speed(speed);
			// 맵 좌표계로 변환
			// 측량 좌표의 세로가로비와 지도 좌표의 세로가로비가 다르면 이상해지므로 주의
			speed *= transform.get_rate_to_map_x(image);
			// 표시 스케일을 고려
			speed *= image.ImageScale;

			return speed;
		}

		/*-------------------------------------------------------------------------
		 표시용속도
		 속도와 표시용 속도의 차가 클수록 크게 속도에 가까워지고
		 차가 작을 때는 천천히 속도에 가까워진다
		---------------------------------------------------------------------------*/
		private float update_step_position_speed(float speed_map) {
			speed_map *= 60;										// 1 분간 나아가는 거리
																	//
			return speed_map;
			//
			/*
						float	gap	= Math.Abs(speed_map - m_show_speed);

						gap	*= gap;						// 갭이 클수록 빠르게 가까워진다
						if(gap > 300)	gap	= 300;		// 최대 갭
						gap		*= 1f/(30);				// 
						if(gap < 0.4f)	gap	= 0.4f;		// 도달 속도가 너무 느릴 경우에 조정한다

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
		 진행 방향 그리기
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
			// 표시할 필요가 있는지 확인
			if (!is_draw_expect_pos) return false;
			// 최저속도
			if (speed_calculator.MapToKnotSpeed(speed) < STEP_POSITION_SPEED_MIN) return false;

			// 경과시간
			int sectiontime = m_expect_pos_timer.GetSectionTimeMilliseconds();

			// 표시용 속도를 구한다
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
