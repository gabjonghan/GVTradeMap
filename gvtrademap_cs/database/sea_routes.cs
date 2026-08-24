/*-------------------------------------------------------------------------

 항로도

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 define
---------------------------------------------------------------------------*/
//#define	DRAW_SEA_ROUTES_BOUNDINGBOX			// 항로도의 바운딩 박스 그리기
//#define	DRAW_POPUPS_BOUNDINGBOX				// 팝업의 바운딩 박스 그리기
//#define	DISABLE_SEA_ROUTES_CHAIN_POINTS		// 항로도를 되도록 이어 붙이는 처리 무효

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using Utility;
using directx;
using System.Collections;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class SeaRoutes {
		// 선으로 볼 거리의 최대
		private const float LINE_ROUTE_MAX = 15 * 15;
		// 선으로 추가할 최저 거리
		private const float ADD_POINT_MIN = 7 * 7;
		// 너무 떨어진 경우에 추가할 각도(degree)
		private const float ADD_POINT_ANGLE_GAP_MAX = 16;
		//		private const float			ADD_POINT_ANGLE_GAP_MAX			= 25;
		private const float ADD_POINT_ANGLE_LENGTH_SQ_MAX = 2000 * 2000;

		// 스크린샷의 좌우 여유
		private const int SCREEN_SHOT_BOUNDING_BOX_GAP_X = 64;
		// 스크린샷의 상하 여유
		private const int SCREEN_SHOT_BOUNDING_BOX_GAP_Y = 64;

		// 최신 항로 이외의 반투명 정도
		private const float SEAROUTES_ALPHA = 0.4f;

		// 하나의 선으로 볼 바운딩 박스의 크기
		// 크기가 너무 크면 그리기에서 손해이므로, 적당한 크기가 되면 선을 분할한다
		// 단위는 지도 좌표계
		private const float BB_SIZE_MAX = 250;
		private const float BB_POPUP_SIZE_MAX = 350f;
		// 화면 밖 판정 시의 여백
		private const float BB_OUTSIDESCREEEN_OFFSET = 32f;

		// 스크린샷의 분포 확인 단위(dot)
		private const int SS_DISTRIBUTION_X = 64;

		/*-------------------------------------------------------------------------
		 항로좌표
		---------------------------------------------------------------------------*/
		public class SeaRoutePoint {
			private Vector2 m_pos;			  // 좌표
			private int m_color_index;	  // 색번호
			private int m_color;			// 그리기색
											// 반투명값이 0 이므로 주의

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public Vector2 Position {
				get { return m_pos; }
				set { m_pos = value; }
			}
			public int ColorIndex { get { return m_color_index; } }
			public int Color { get { return m_color; } }

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public SeaRoutePoint(float x, float y, int color_index) {
				m_pos.X = x;
				m_pos.Y = y;
				m_color_index = color_index;
				m_color = DrawColor.GetColor(color_index);
			}
			public SeaRoutePoint(float x, float y) {
				m_pos.X = x;
				m_pos.Y = y;
				m_color_index = 0;
				m_color = DrawColor.GetColor(ColorIndex);
			}
			public SeaRoutePoint(Vector2 pos, int color_index) {
				m_pos = pos;
				m_color_index = color_index;
				m_color = DrawColor.GetColor(color_index);
			}
			public SeaRoutePoint(Vector2 pos) {
				m_pos = pos;
				m_color_index = 0;
				m_color = DrawColor.GetColor(ColorIndex);
			}
			public SeaRoutePoint(SeaRoutePoint p) {
				m_pos = p.Position;
				m_color_index = p.ColorIndex;
				m_color = p.Color;
			}
			/*-------------------------------------------------------------------------
			 거리의 제곱을 반환한다
			---------------------------------------------------------------------------*/
			public float LengthSq(SeaRoutePoint p) {
				Vector2 l = new Vector2(p.Position.X - m_pos.X,
											p.Position.Y - m_pos.Y);
				return l.LengthSq();
			}
			/*-------------------------------------------------------------------------
			 거리를 반환한다
			---------------------------------------------------------------------------*/
			public float Length(SeaRoutePoint p) {
				Vector2 l = new Vector2(p.Position.X - m_pos.X,
											p.Position.Y - m_pos.Y);
				return l.Length();
			}

			/*-------------------------------------------------------------------------
			 거리의 제곱을 반환한다
			 루프를 고려해 가까운 쪽의 거리를 반환한다
			 가까운 위치를 near_p 에 반환한다
			 near_p == p 이면 루프를 고려하지 않은 위치가 최단
			---------------------------------------------------------------------------*/
			public float LengthSq(SeaRoutePoint p, int size_x) {
				SeaRoutePoint near = null;
				return LengthSq(p, size_x, ref near);
			}
			public float LengthSq(SeaRoutePoint p, int size_x, ref SeaRoutePoint near_p) {
				float l1 = LengthSq(p);
				SeaRoutePoint p2 = new SeaRoutePoint(p);
				p2.build_loop_x(size_x);
				SeaRoutePoint p3 = new SeaRoutePoint(p);
				p3.build_loop_x(-size_x);
				float l2 = LengthSq(p2);
				float l3 = LengthSq(p3);

				if (l1 < l2) {
					if (l1 < l3) {
						near_p = p;
						return l1;
					} else {
						near_p = p3;
						return l3;
					}
				} else {
					if (l2 < l3) {
						near_p = p2;
						return l2;
					} else {
						near_p = p3;
						return l3;
					}
				}
			}

			/*-------------------------------------------------------------------------
			 거리를 반환한다
			 루프를 고려해 가까운 쪽의 거리를 반환한다
			---------------------------------------------------------------------------*/
			public float Length(SeaRoutePoint p, int size_x) {
				return (float)Math.Sqrt(LengthSq(p, size_x));
			}
			public float Length(SeaRoutePoint p, int size_x, ref SeaRoutePoint near_p) {
				SeaRoutePoint near = null;
				return (float)Math.Sqrt(LengthSq(p, size_x, ref near));
			}

			/*-------------------------------------------------------------------------
			 루프를 고려한 위치를 작성한다
			 단순히 position.x + size_x
			---------------------------------------------------------------------------*/
			private void build_loop_x(int size_x) {
				m_pos.X += size_x;
			}

			/*-------------------------------------------------------------------------
			 넘겨받은 point 로부터의 이동 벡터를 얻는다
			 루프는 고려되지 않는다
			 벡터는 정규화되어 있지 않다
			 정규화된 벡터를 얻으려면 GetVectorNormalized() 를 사용한다
			---------------------------------------------------------------------------*/
			public Vector2 GetVector(SeaRoutePoint from) {
				return new Vector2(this.Position.X - from.Position.X,
									this.Position.Y - from.Position.Y);
			}

			/*-------------------------------------------------------------------------
			 넘겨받은 point 로부터의 이동 벡터를 얻는다
			 루프는 고려되지 않는다
			 정규화된 벡터를 반환한다
			---------------------------------------------------------------------------*/
			public Vector2 GetVectorNormalized(SeaRoutePoint from) {
				Vector2 vec = GetVector(from);
				vec.Normalize();
				return vec;
			}

			/*-------------------------------------------------------------------------
			 넘겨받은 point 로부터의 이동 벡터를 얻는다
			 루프가 고려된다
			 벡터는 정규화되어 있지 않다
			 정규화된 벡터를 얻으려면 GetVectorNormalized() 를 사용한다
			---------------------------------------------------------------------------*/
			public Vector2 GetVector(SeaRoutePoint from, int loop_x_size) {
				SeaRoutePoint near_p = null;
				from.LengthSq(this, loop_x_size, ref near_p);
				return near_p.GetVector(from);
			}

			/*-------------------------------------------------------------------------
			 넘겨받은 point 로부터의 이동 벡터를 얻는다
			 루프가 고려된다
			 정규화된 벡터를 반환한다
			---------------------------------------------------------------------------*/
			public Vector2 GetVectorNormalized(SeaRoutePoint from, int loop_x_size) {
				Vector2 vec = GetVector(from, loop_x_size);
				vec.Normalize();
				return vec;
			}
		}

		/*-------------------------------------------------------------------------
		 말풍선좌표
		---------------------------------------------------------------------------*/
		public class SeaRoutePopupPoint : SeaRoutePoint {
			private int m_days;			 // 일수

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public int Days { get { return m_days; } }

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public SeaRoutePopupPoint(float x, float y, int color_index, int days)
				: base(x, y, color_index) {
				m_days = days;
			}
			public SeaRoutePopupPoint(Vector2 pos, int color_index, int days)
				: this(pos.X, pos.Y, color_index, days) {
			}
		}

		/*-------------------------------------------------------------------------
		 말풍선좌표
		 바운딩 박스 구분
		---------------------------------------------------------------------------*/
		public class SeaRoutePopupPointsBB : D3dBB2d {
			private List<SeaRoutePopupPoint> m_points;
			private int m_max_days;

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public List<SeaRoutePopupPoint> Points { get { return m_points; } }
			public int MaxDays { get { return m_max_days; } }

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public SeaRoutePopupPointsBB()
				: base() {
				m_points = new List<SeaRoutePopupPoint>();
				base.OffsetLT = new Vector2(-BB_OUTSIDESCREEEN_OFFSET, -BB_OUTSIDESCREEEN_OFFSET);
				base.OffsetRB = new Vector2(BB_OUTSIDESCREEEN_OFFSET, BB_OUTSIDESCREEEN_OFFSET);
				m_max_days = 0;
			}

			/*-------------------------------------------------------------------------
			 추가
			 바운딩 박스의 크기가 일정 이상일 때는 추가하지 않고 false 를 반환한다
			---------------------------------------------------------------------------*/
			public bool Add(SeaRoutePopupPoint p) {
				Vector2 size = base.IfUpdate(p.Position).Size;
				if (size.X > BB_POPUP_SIZE_MAX) return false;
				if (size.Y > BB_POPUP_SIZE_MAX) return false;

				// 추가
				m_points.Add(p);
				base.Update(p.Position);

				// 최대 항해 일수 갱신
				foreach (SeaRoutePopupPoint i in m_points) {
					if (i.Days > m_max_days) {
						m_max_days = i.Days;
					}
				}

				return true;		// 추가했다
			}
		}

		/*-------------------------------------------------------------------------
		 항로도용라인
		---------------------------------------------------------------------------*/
		class RouteLineBB : D3dBB2d {
			private List<Vector2> m_points;
			private int m_color;			// alpha 값은 0 이므로 그릴 때 주의

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public int Color { get { return m_color; } }
			public int Count { get { return m_points.Count; } }

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public RouteLineBB(int color) {
				m_points = new List<Vector2>();
				m_color = color;
				base.OffsetLT = new Vector2(-2, -2);
				base.OffsetRB = new Vector2(2, 2);
			}

			/*-------------------------------------------------------------------------
			 추가
			---------------------------------------------------------------------------*/
			public void AddPoint(Vector2 point) {
				m_points.Add(point);
				base.Update(point);
			}

			/*-------------------------------------------------------------------------
			 컬링 정보를 얻는다
			 true 를 반환했을 때 컬링한다
			---------------------------------------------------------------------------*/
			public bool IsCulling(Vector2 offset, LoopXImage image) {
				return base.IsCulling(offset, image.ImageScale, new D3dBB2d.CullingRect(image.Device.client_size));
			}

			/*-------------------------------------------------------------------------
			 바운딩 박스를 그린다
			 디버그용
			---------------------------------------------------------------------------*/
			public void DrawBB(Vector2 offset, LoopXImage image) {
				// 그리기
				base.Draw(image.Device, 0.5f, offset, image.ImageScale, Color | (255 << 24));
			}

			/*-------------------------------------------------------------------------
			 그리기용으로 좌표 변환된 배열을 얻는다
			 IsCulling() 으로 컬링할지 여부를 조사한 뒤 이 함수를 호출할 것
			---------------------------------------------------------------------------*/
			public Vector2[] BuildLinePoints(Vector2 offset, LoopXImage image) {
				Vector2[] list = new Vector2[m_points.Count];
				int i = 0;
				foreach (Vector2 p in m_points) {
					list[i++] = image.GlobalPos2LocalPos(p, offset);
				}
				return list;
			}
		}

		/*-------------------------------------------------------------------------
		 1 항해분의 항로
		---------------------------------------------------------------------------*/
		public class Voyage {
			private gvt_lib m_lib;				  //

			private List<SeaRoutePoint> m_routes;			   // 항로
			private List<SeaRoutePopupPointsBB> m_popups;			   // 말풍선
			private List<SeaRoutePopupPointsBB> m_accidents;			// 재해
			private List<RouteLineBB> m_line_routes;			// 그리기용항로라인
			private bool m_is_build_line_routes;	// 항로 라인을 만들면 true

			private float m_alpha;			  // 전체에 걸리는 반투명 정도
			private float m_gap_cos;				// 되도록 라인을 이어 붙일 각도(cos)

			private bool m_is_draw;			 // 그릴 때 true
			private int m_max_days;			 // 최대항해일수
			private int m_minimum_draw_days;	// 그릴 최저 항해 일수
			private DateTime m_date;					// 항해시간
			private bool m_is_selected;		 // 항로도 목록에서 선택되어 있을 때 true

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			// 그릴 때의 반투명도
			public float Alpha {
				get { return m_alpha; }
				set {
					m_alpha = value;
					if (m_alpha < 0) m_alpha = 0;
					if (m_alpha > 1) m_alpha = 1;
				}
			}
			// 비었는지 여부를 얻는다
			public bool IsEmpty {
				get {
					if (m_routes.Count > 1) return false;
					if (m_popups.Count > 1) return false;
					if (m_accidents.Count > 1) return false;
					return true;
				}
			}
			// 유저 지정에 의한 그리기 억제
			public bool IsEnableDraw {
				get { return m_is_draw; }
				set { m_is_draw = value; }
			}

			// 최대항해일수
			public int MaxDays { get { return m_max_days; } }
			public string MaxDaysString { get { return String.Format("{0}일", MaxDays); } }

			// 그릴 최저 항해 일수
			public int MinimumDrawDays {
				get { return m_minimum_draw_days; }
				set { m_minimum_draw_days = value; }
			}

			// 출발 지점
			public Vector2 MapPoint1st {
				get {
					if (m_routes.Count > 0) return m_routes[0].Position;
					return new Vector2(-1, -1);
				}
			}
			public string MapPoint1stString { get { return get_pos_str(MapPoint1st); } }
			public Point GamePoint1st {
				get {
					if (m_routes.Count > 0) {
						return transform.ToPoint(
									transform.map_pos2_game_pos(m_routes[0].Position, m_lib.loop_image));
					}
					return new Point(-1, -1);
				}
			}
			public string GamePoint1stStr { get { return get_pos_str(GamePoint1st); } }

			// 종료 지점
			public Vector2 MapPointLast {
				get {
					if (m_routes.Count > 0) {
						return m_routes[m_routes.Count - 1].Position;
					}
					return new Vector2(-1, -1);
				}
			}
			public string MapPointLastString { get { return get_pos_str(MapPointLast); } }
			public Point GamePointLast {
				get {
					if (m_routes.Count > 0) {
						return transform.ToPoint(
									transform.map_pos2_game_pos(m_routes[m_routes.Count - 1].Position, m_lib.loop_image));
					}
					return new Point(-1, -1);
				}
			}
			public string GamePointLastString { get { return get_pos_str(GamePointLast); } }

			public DateTime DateTime { get { return m_date; } }
			public string DateTimeString {
				get {
					if (m_date.Ticks == 0) {
						//					if(m_date == null){
						return "불명(구 데이터)";
					}
					return Useful.TojbbsDateTimeString(m_date);
				}
			}
			public bool IsSelected {
				get { return m_is_selected; }
				set { m_is_selected = value; }
			}

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public Voyage(gvt_lib lib) {
				m_lib = lib;

				m_popups = new List<SeaRoutePopupPointsBB>();
				m_accidents = new List<SeaRoutePopupPointsBB>();
				m_routes = new List<SeaRoutePoint>();
				m_line_routes = new List<RouteLineBB>();

				// 선의 구축 미완성
				m_is_build_line_routes = false;

				Alpha = 1;
				m_gap_cos = (float)Math.Cos(Useful.ToRadian(ADD_POINT_ANGLE_GAP_MAX));
				m_is_draw = true;
				m_max_days = 0;
				m_minimum_draw_days = 0;
				m_is_selected = false;

				m_date = DateTime.Now;
			}

			/*-------------------------------------------------------------------------
			 추가
			---------------------------------------------------------------------------*/
			public void AddPopup(SeaRoutePopupPoint p) {
				if (m_popups.Count <= 0) {
					m_popups.Add(new SeaRoutePopupPointsBB());
				}
				if (!m_popups[m_popups.Count - 1].Add(p)) {
					m_popups.Add(new SeaRoutePopupPointsBB());
					m_popups[m_popups.Count - 1].Add(p);
				}

				// 최대 항해 일수 갱신
				foreach (SeaRoutePopupPointsBB i in m_popups) {
					if (i.MaxDays > m_max_days) m_max_days = i.MaxDays;
				}
			}
			public void AddAccident(SeaRoutePopupPoint p) {
				if (m_accidents.Count <= 0) {
					m_accidents.Add(new SeaRoutePopupPointsBB());
				}
				if (!m_accidents[m_accidents.Count - 1].Add(p)) {
					m_accidents.Add(new SeaRoutePopupPointsBB());
					m_accidents[m_accidents.Count - 1].Add(p);
				}
			}
			public void AddPoint(SeaRoutePoint p) {
				m_routes.Add(p);
				m_is_build_line_routes = false; // 항로선 구축 리퀘스트
			}

			/*-------------------------------------------------------------------------
			 항로도의 라인을 구축한다
			 어느 정도 모인 라인을 작성한다
			 좌표 간의 거리에 따라 라인으로 삼을지 판단한다
			 바운딩 박스의 크기가 커진 경우 선을 나눈다
			 좌표 간의 거리가 너무 큰 경우에는 각도차에 따라 라인으로 삼을지 결정한다
			---------------------------------------------------------------------------*/
			private void build_line_routes() {
				if (m_is_build_line_routes) return;	 // 만들어져 있다

				m_line_routes.Clear();				  // 파기
				if (m_routes.Count < 2) return;	 // 선을 만들 수 없다

				SeaRoutePoint old_pos = m_routes[0];	// 최초의 좌표
				SeaRoutePoint old_pos2 = null;		  // 두 개 전의 좌표
				RouteLineBB route = null;

				for (int i = 1; i < m_routes.Count; i++) {
					if (route == null) {
						// 시작
						route = new RouteLineBB(old_pos.Color);
						route.AddPoint(old_pos.Position);
					}

					// 루프를 고려한 거리의 제곱을 얻는다
					// near_p 는 가까운 위치를 반환한다
					// 루프하지 않는 경우가 최단이면 near_p == m_routes[i]
					SeaRoutePoint near_p = null;
					float length = old_pos.LengthSq(m_routes[i],
														(int)m_lib.loop_image.ImageSize.X,
														ref near_p);

					// 거리 판정
					if (length > LINE_ROUTE_MAX) {
#if DISABLE_SEA_ROUTES_CHAIN_POINTS
						// 되도록 이어 붙이는 처리 무효
						near_p	= null;
#else
						if (length > ADD_POINT_ANGLE_LENGTH_SQ_MAX) {
							// 거리가 너무 멀다
							near_p = null;
						} else {
							// 각도차 판정
							// 각도차가 너무 크면 null 이 반환된다
							near_p = check_point_sub(old_pos, old_pos2, near_p);
						}
#endif
					}

					// 추가
					if (near_p != null) route.AddPoint(near_p.Position);

					// 바운딩 박스의 크기 확인
					// 분할 플래그이거나 크기가 너무 크면 분할한다
					if ((near_p != m_routes[i])			 // 루프를 고려한 위치를 추가했는지, 추가되지 않았는지
						|| (route.Size.X >= BB_SIZE_MAX)			// 바운딩 박스의 크기를 넘었다
						|| (route.Size.Y >= BB_SIZE_MAX)) {	 // 바운딩 박스의 크기를 넘었다
																// 선을 구분한다
						add_route(route);
						route = null;
					}

					// 이전 좌표를 갱신한다
					old_pos2 = old_pos;
					old_pos = m_routes[i];
				}

				// 추가할 나머지가 있으면 추가한다
				if (route != null) add_route(route);

				// 작성완료
				m_is_build_line_routes = true;
			}

			/*-------------------------------------------------------------------------
			 거리가 먼 경우의 확인
			 각도차로 추가할지 여부를 정한다
			 각도차가 너무 크면 null 을 반환한다
			---------------------------------------------------------------------------*/
			private SeaRoutePoint check_point_sub(SeaRoutePoint old_pos1, SeaRoutePoint old_pos2, SeaRoutePoint new_pos) {
				if (old_pos1 == null) return null;
				if (old_pos2 == null) return null;

				// 이동 벡터를 얻는다
				// 루프가 고려된다
				Vector2 old_vec = old_pos1.GetVectorNormalized(old_pos2, (int)m_lib.loop_image.ImageSize.X);
				Vector2 new_vec = new_pos.GetVectorNormalized(old_pos1, (int)m_lib.loop_image.ImageSize.X);

				// 각도차(cos)를 얻는다
				float gap = Vector2.Dot(old_vec, new_vec);
				if (gap >= m_gap_cos) {
					return new_pos;
				}
				return null;
			}

			/*-------------------------------------------------------------------------
			 선을 목록에 추가
			---------------------------------------------------------------------------*/
			private void add_route(RouteLineBB route) {
				if (route.Count < 2) return;
				m_line_routes.Add(route);
			}

			/*-------------------------------------------------------------------------
			 항로도그리기
			---------------------------------------------------------------------------*/
			public void DrawRoutes(Vector2 offset, LoopXImage image, bool is_select_mode) {
				// 그릴 필요가 있는지 확인
				if (!can_draw(is_select_mode)) return;

				// 라인을 구축
				// 이미 작성되어 있으면 아무것도 하지 않는다
				// 그릴 필요가 있는 경우에만 구축을 시도한다
				build_line_routes();

				// 반투명 값을 얻는다
				int alpha = (int)(m_alpha * 255);
				if (m_is_selected) alpha = 255;
				alpha <<= 24;

				// 그리기
				foreach (RouteLineBB line in m_line_routes) {
					// 바운딩 박스로 컬링
					if (line.IsCulling(offset, image)) continue;

					// 좌표변환후그리기
					m_lib.device.line.Draw(line.BuildLinePoints(offset, image), line.Color | alpha);
				}
			}

			/*-------------------------------------------------------------------------
			 그릴지 확인
			 선택 모드 중이고 선택 없음
			 그리기플래그
			 항해 일수가 그릴 최저 항해 일수에 미치지 못한다
			---------------------------------------------------------------------------*/
			private bool can_draw(bool is_select_mode) {
				// 선택 시에는 무조건 그린다
				if (m_is_selected) return true;
				// 선택 모드 중이고 선택 중이 아니다
				if (is_select_mode && (!m_is_selected)) return false;

				// 비표시 시에는 아무것도 하지 않는다
				if (!m_is_draw) return false;
				// 항해 일수가 그릴 최저 항해 일수에 미치지 못하면 그리지 않음
				if (MaxDays < MinimumDrawDays) return false;

				// 그린다
				return true;
			}

			/*-------------------------------------------------------------------------
			 바운딩 박스그리기
			 디버그용
			---------------------------------------------------------------------------*/
			public void DrawRoutesBB(Vector2 offset, LoopXImage image, bool is_select_mode) {
				// 그릴 필요가 있는지 확인
				if (!can_draw(is_select_mode)) return;

				foreach (RouteLineBB line in m_line_routes) {
					line.DrawBB(offset, image);
				}
			}

			/*-------------------------------------------------------------------------
			 말풍선그리기
			---------------------------------------------------------------------------*/
			public void DrawPopups(Vector2 offset, LoopXImage image, bool is_select_mode) {
				// 그릴 필요가 있는지 확인
				if (!can_draw(is_select_mode)) return;

				float size = image.ImageScale;
				if (size < 0.5) size = 0.5f;
				else if (size > 1) size = 1;

				// 반투명 정도를 반영
				float alpha = (m_is_selected) ? 1 : m_alpha;
				int alpha1 = ((int)(alpha * 255)) << 24;
				int alpha2 = ((int)(alpha * 64)) << 24;
				int color1 = 0x00ffffff | alpha1;
				int color2 = 0x00ffffff | alpha2;

				D3dBB2d.CullingRect rect = new D3dBB2d.CullingRect(image.Device.client_size);
				foreach (SeaRoutePopupPointsBB b in m_popups) {
					// 바운딩 박스로 화면 밖인지 조사
					if (b.IsCulling(offset, image.ImageScale, rect)) {
						continue;
					}
#if DRAW_POPUPS_BOUNDINGBOX
					d3d_bb2d.Draw(b.bb, image.device, 0.5f, offset, image.scale, Color.Blue.ToArgb());
#endif
					foreach (SeaRoutePopupPoint p in b.Points) {
						// 날짜 말풍선
						if (p.ColorIndex < 0) continue;
						if (p.ColorIndex >= 8) continue;

						Vector3 pos = new Vector3(p.Position.X, p.Position.Y, 0.5f);

						if ((p.Days % m_lib.setting.draw_popup_day_interval) != 0) {
							// 통상의 작은 아이콘
							m_lib.device.sprites.AddDrawSpritesNC(pos,
													m_lib.icons.GetIcon(icons.icon_index.days_mini_6),
													new Vector2(size, size),
													alpha1 | p.Color);
						} else {
							// 날짜 말풍선

							// 그림자
							pos.Z = 0.8f;
							m_lib.device.sprites.AddDrawSpritesNC(pos,
													m_lib.icons.GetIcon(icons.icon_index.days_big_shadow),
													new Vector2(1, 1),
													color2);
							pos.Z = 0.5f;
							// 3 자리인 경우에는 가로로 넓은 그림을 사용한다
							icons.icon_index icon = (p.Days >= 100) ? icons.icon_index.days_big_100 : icons.icon_index.days_big_6;
							// 말풍선
							if (m_lib.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(icon), alpha1 | p.Color)) {
								// 말풍선이 컬링되지 않았으면 일수를 그린다
								// 4 자리 이상은 캡처할 수 없을 것이다, 아마도
								int days = p.Days;
								if (days > 999) days = 999;
								if (days < 0) days = 0;

								int max = 1;
								Vector2 offset2 = new Vector2(0, 0);
								if (days >= 100) {	  // 3 자리
									offset2.X += 7;
									max = 3;
								} else if (days >= 10) {	// 2 자리
									offset2.X += 4;
									max = 2;
								}
								// 일수
								for (int i = 0; i < max; i++) {
									m_lib.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(icons.icon_index.number_0 + (days % 10)), color1, offset2);
									offset2.X -= 7;
									days /= 10;
								}
							}
						}
					}
				}
			}

			/*-------------------------------------------------------------------------
			 재해그리기
			---------------------------------------------------------------------------*/
			public void DrawAccidents(Vector2 offset, LoopXImage image, bool is_select_mode) {
				// 그릴 필요가 있는지 확인
				if (!can_draw(is_select_mode)) return;

				// 반투명 정도를 반영
				float alpha = (m_is_selected) ? 1 : m_alpha;
				int alpha1 = ((int)(alpha * 255)) << 24;
				int alpha2 = ((int)(alpha * 64)) << 24;
				int color1 = 0x00ffffff | alpha1;
				int color2 = 0x00ffffff | alpha2;

				D3dBB2d.CullingRect rect = new D3dBB2d.CullingRect(image.Device.client_size);
				foreach (SeaRoutePopupPointsBB b in m_accidents) {
					// 바운딩 박스로 화면 밖인지 조사
					if (b.IsCulling(offset, image.ImageScale, rect)) {
						continue;
					}
#if DRAW_POPUPS_BOUNDINGBOX
					d3d_bb2d.Draw(b.bb, image.device, 0.5f, offset, image.scale, Color.Red.ToArgb());
#endif

					foreach (SeaRoutePopupPoint p in b.Points) {
						if (p.ColorIndex < 101) continue;
						if (p.ColorIndex > 111) continue;
						if (!is_draw_popups(p.ColorIndex)) continue;

						Vector3 pos = new Vector3(p.Position.X, p.Position.Y, 0.5f);

						// 그림자
						pos.Z = 0.8f;
						m_lib.device.sprites.AddDrawSpritesNC(pos,
												m_lib.icons.GetIcon(icons.icon_index.accident_popup_shadow),
												new Vector2(1, 1),
												color2);

						// 말풍선
						pos.Z = 0.5f;
						m_lib.device.sprites.AddDrawSpritesNC(pos,
									m_lib.icons.GetIcon(icons.icon_index.accident_popup), color1);
						// 내용을 그린다
						m_lib.device.sprites.AddDrawSpritesNC(pos,
									m_lib.icons.GetIcon(icons.icon_index.accident_0 + (p.ColorIndex - 101)), color1);
					}
				}
			}

			/*-------------------------------------------------------------------------
			 표시 항목 확인
			---------------------------------------------------------------------------*/
			private bool is_draw_popups(int index) {
				// 그리기플래그
				DrawSettingAccidents flag = m_lib.setting.draw_setting_accidents;

				switch (index) {
					case 101:
						if ((flag & DrawSettingAccidents.accident_0) == 0) return false;
						break;
					case 102:
						if ((flag & DrawSettingAccidents.accident_1) == 0) return false;
						break;
					case 103:
						if ((flag & DrawSettingAccidents.accident_2) == 0) return false;
						break;
					case 104:
						if ((flag & DrawSettingAccidents.accident_3) == 0) return false;
						break;
					case 105:
						if ((flag & DrawSettingAccidents.accident_4) == 0) return false;
						break;
					case 106:
						if ((flag & DrawSettingAccidents.accident_5) == 0) return false;
						break;
					case 107:
						if ((flag & DrawSettingAccidents.accident_6) == 0) return false;
						break;
					case 108:
						if ((flag & DrawSettingAccidents.accident_7) == 0) return false;
						break;
					case 109:
						if ((flag & DrawSettingAccidents.accident_8) == 0) return false;
						break;
					case 110:
						if ((flag & DrawSettingAccidents.accident_9) == 0) return false;
						break;
					case 111:
						if ((flag & DrawSettingAccidents.accident_10) == 0) return false;
						break;
				}
				return true;
			}

			/*-------------------------------------------------------------------------
			 읽기 시작
			 항해 시간을 초기화한다
			---------------------------------------------------------------------------*/
			public void StartLoad() {
				m_date = new DateTime();
			}

			/*-------------------------------------------------------------------------
			 읽기
			---------------------------------------------------------------------------*/
			public void LoadFromLine(string line) {
				try {
					string[] split = line.Split(new char[] { ',' });

					switch (split[0]) {
						case "popup":
						case "accidents": {
								int x = Convert.ToInt32(split[1]);
								int y = Convert.ToInt32(split[2]);
								int days = Convert.ToInt32(split[3]);
								int color_index = Convert.ToInt32(split[4]);

								// 추가
								if ((color_index >= 101) && (color_index <= 111)) {
									// 재해
									AddAccident(new SeaRoutePopupPoint(x, y, color_index, days));
								} else {
									// 날짜
									AddPopup(new SeaRoutePopupPoint(x, y, color_index, days));
								}
							}
							break;
						case "routes": {
								float x = (float)Convert.ToDouble(split[1]);
								float y = (float)Convert.ToDouble(split[2]);
								int color_index = Convert.ToInt32(split[3]);

								// 추가
								m_routes.Add(new SeaRoutePoint(x, y, color_index));
							}
							break;
						case "draw": {
								int flag = Convert.ToInt32(split[1]);
								m_is_draw = (flag != 0) ? true : false;
							}
							break;
						case "date": {
								m_date = Useful.ToDateTime(split[1]);
							}
							break;
						default:
							break;
					}
				} catch {
					// 읽기실패
					// 이 행은 무시된다
				}
			}

			/*-------------------------------------------------------------------------
			 기록
			---------------------------------------------------------------------------*/
			public void Write(StreamWriter sw) {
				if (IsEnableDraw) sw.WriteLine("draw,1");
				else sw.WriteLine("draw,0");
				sw.WriteLine("date," + Useful.TojbbsDateTimeString(m_date));

				foreach (SeaRoutePopupPointsBB b in m_popups) {
					foreach (SeaRoutePopupPoint p in b.Points) {
						string str = "";
						str += "popup,";
						str += ((int)p.Position.X).ToString() + ",";
						str += ((int)p.Position.Y).ToString() + ",";
						str += p.Days + ",";
						str += p.ColorIndex;
						sw.WriteLine(str);
					}
				}
				foreach (SeaRoutePopupPointsBB b in m_accidents) {
					foreach (SeaRoutePopupPoint p in b.Points) {
						string str = "";
						str += "accidents,";
						str += ((int)p.Position.X).ToString() + ",";
						str += ((int)p.Position.Y).ToString() + ",";
						str += p.Days + ",";
						str += p.ColorIndex;
						sw.WriteLine(str);
					}
				}

				foreach (SeaRoutePoint p in m_routes) {
					string str = "";
					str += "routes,";
					str += p.Position.X + ",";
					str += p.Position.Y + ",";
					str += p.ColorIndex;
					sw.WriteLine(str);
				}
			}

			/*-------------------------------------------------------------------------
			 그리기용 색 번호를 얻는다
			---------------------------------------------------------------------------*/
			public int GetColorIndex() {
				if (m_routes.Count > 0) {
					return m_routes[0].ColorIndex;
				}
				if (m_popups.Count > 0) {
					if (m_popups[0].Points.Count > 0) {
						return m_popups[0].Points[0].ColorIndex;
					}
				}
				return 0;
			}

			/*-------------------------------------------------------------------------
			 삭제
			---------------------------------------------------------------------------*/
			public void Remove(bool popups, bool accident, bool routes) {
				if (popups) m_popups.Clear();
				if (accident) m_accidents.Clear();
				if (routes) {
					m_routes.Clear();
					m_line_routes.Clear();
				}
			}

			/*-------------------------------------------------------------------------
			 스크린샷용
			 포함되는 범위를 등록한다
			---------------------------------------------------------------------------*/
			public void SS_AddMinMaxList(List<Point>[] map, ref int min_y, ref int max_y, bool is_select_mode) {
				// 그릴 필요가 있는지 확인
				if (!can_draw(is_select_mode)) return;

				foreach (SeaRoutePoint p in m_routes) {
					add_minmax_list(map,
									transform.ToPoint(p.Position),
									ref min_y, ref max_y);
				}
				foreach (SeaRoutePopupPointsBB b in m_popups) {
					foreach (SeaRoutePopupPoint p in b.Points) {
						add_minmax_list(map,
										transform.ToPoint(p.Position),
										ref min_y, ref max_y);
					}
				}
				foreach (SeaRoutePopupPointsBB b in m_accidents) {
					foreach (SeaRoutePopupPoint p in b.Points) {
						add_minmax_list(map,
										transform.ToPoint(p.Position),
										ref min_y, ref max_y);
					}
				}
			}

			/*-------------------------------------------------------------------------
			 X 방향은 대략적인 구분으로 등록한다
			 Y 방향은 그대로 최대 최소를 얻는다
			---------------------------------------------------------------------------*/
			private void add_minmax_list(List<Point>[] map, Point pos, ref int min_y, ref int max_y) {
				calc_bounding_box_y(pos.Y, ref min_y, ref max_y);
				int index = pos.X / SS_DISTRIBUTION_X;
				if (index < 0) return;
				if (index >= map.Length) return;
				map[index].Add(pos);
			}

			/*-------------------------------------------------------------------------
			 스크린샷용으로 바운딩 박스를 구한다
			 세로방향
			---------------------------------------------------------------------------*/
			private void calc_bounding_box_y(int y, ref int min, ref int max) {
				if (y <= 64) return;
				if (y > max) max = y;
				if (y < min) min = y;
			}

			/*-------------------------------------------------------------------------
			 위치를 문자열로 얻는다
			---------------------------------------------------------------------------*/
			private string get_pos_str(Point pos) {
				if ((pos.X < 0)
					|| (pos.Y < 0)) {
					return "불명의위치";
				}
				return String.Format("{0},{1}", pos.X, pos.Y);
			}
			private string get_pos_str(Vector2 pos) {
				if ((pos.X < 0)
					|| (pos.Y < 0)) {
					return "불명의위치";
				}
				return String.Format("{0},{1}", (int)pos.X, (int)pos.Y);
			}
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private gvt_lib m_lib;				  //

		private List<Voyage> m_sea_routes;		  // 항로목록
													// 색이 바뀌는 타이밍에 구분된다
		private List<Voyage> m_favorite_sea_routes; // 항로도즐겨찾기목록
		private List<Voyage> m_trash_sea_routes;		// 휴지통 항로 목록

		// 추가 시용
		private int m_color_index;
		private int m_old_days;
		private Point m_old_day_pos;
		private Point m_old_pos;
		private bool m_is_1st;

		// 선택 중에는 선택된 항로도만 그린다
		private bool m_is_select_mode;

		private RequestCtrl m_req_update_list;	  // 항로도 목록 갱신 리퀘스트
		private RequestCtrl m_req_redraw_list;	  // 항로도 목록 다시 그리기 리퀘스트

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public List<Voyage> searoutes { get { return m_sea_routes; } }
		public List<Voyage> favorite_sea_routes { get { return m_favorite_sea_routes; } }
		public List<Voyage> trash_sea_routes { get { return m_trash_sea_routes; } }
		public RequestCtrl req_update_list { get { return m_req_update_list; } }
		public RequestCtrl req_redraw_list { get { return m_req_redraw_list; } }

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public SeaRoutes(gvt_lib lib, string file_name, string favorite_file_name, string trash_file_name) {
			m_lib = lib;

			m_sea_routes = new List<Voyage>();
			m_favorite_sea_routes = new List<Voyage>();
			m_trash_sea_routes = new List<Voyage>();
			m_req_update_list = new RequestCtrl();
			m_req_redraw_list = new RequestCtrl();

			// 선택모드
			m_is_select_mode = false;

			// 추가 초기화
			init_add_points();

			// 이전 버전 읽기
			load_old_routes(def.SEAROUTE_FULLFNAME1, def.SEAROUTE_FULLFNAME2);

			// 항로도읽기
			load_routes(file_name);
			// 즐겨찾기항로도읽기
			load_routes_sub(m_favorite_sea_routes, favorite_file_name);
			// 휴지통 항로도 읽기
			load_routes_sub(m_trash_sea_routes, trash_file_name);
		}

		/*-------------------------------------------------------------------------
		 이전 버전 읽기
		---------------------------------------------------------------------------*/
		private bool load_old_routes(string file_name1, string file_name2) {
			// 둘 다 없으면 아무것도 하지 않는다
			// 최초 실행 시에 읽어들인 뒤에는 삭제된다
			if ((!File.Exists(file_name1))
				&& (!File.Exists(file_name2))) {
				return true;
			}

			string line = "";
			int old_color_index = -1;

			// 항로도
			try {
				using (StreamReader sr = new StreamReader(
					file_name2, Encoding.GetEncoding("UTF-8"))) {

					while ((line = sr.ReadLine()) != null) {
						try {
							string[] split = line.Split(new char[] { ',' });

							int x = Convert.ToInt32(split[0]);
							int y = Convert.ToInt32(split[1]);
							int color_index = Convert.ToInt32(split[2]);

							// 추가
							if (old_color_index != color_index) {
								// 색이 바뀌면 그룹을 추가
								add_sea_routes();
								old_color_index = color_index;
							}
							add_point(new SeaRoutePoint(x, y, color_index));
						} catch {
						}
					}
				}
			} catch {
				// 읽기실패
				m_sea_routes.Clear();
			}

			// 팝업
			// 문제 회피를 위해, 최신 위치에 전부 로드
			try {
				using (StreamReader sr = new StreamReader(
					file_name1, Encoding.GetEncoding("UTF-8"))) {

					while ((line = sr.ReadLine()) != null) {
						try {
							string[] split = line.Split(new char[] { ',' });

							int x = Convert.ToInt32(split[0]);
							int y = Convert.ToInt32(split[1]);
							int days = Convert.ToInt32(split[2]);
							int color_index = Convert.ToInt32(split[3]);

							// 추가
							if ((color_index >= 101) && (color_index <= 111)) {
								// 재해
								add_accident(new SeaRoutePopupPoint(x, y, color_index, days));
							} else {
								// 날짜
								add_popup(new SeaRoutePopupPoint(x, y, color_index, days));
							}
						} catch {
						}
					}
				}
			} catch {
				// 읽기실패
				m_sea_routes.Clear();
			}

			// 오래된 버전의 파일은 삭제한다
			file_ctrl.RemoveFile(file_name1);
			file_ctrl.RemoveFile(file_name2);
			return true;
		}

		/*-------------------------------------------------------------------------
		 읽기
		---------------------------------------------------------------------------*/
		private bool load_routes(string file_name) {
			// 읽기
			if (!load_routes_sub(m_sea_routes, file_name)) return false;

			// 새로 추가할 때의 색을 결정한다
			if (m_sea_routes.Count > 1) {
				m_color_index = get_newest_sea_routes().GetColorIndex();
				if (m_color_index < 0) m_color_index = 0;
				if (++m_color_index >= 8) m_color_index = 0;
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 읽기
		 sub
		---------------------------------------------------------------------------*/
		private bool load_routes_sub(List<Voyage> list, string file_name) {
			string line = "";
			try {
				using (StreamReader sr = new StreamReader(
					file_name, Encoding.GetEncoding("UTF-8"))) {

					while ((line = sr.ReadLine()) != null) {
						if (line == "start routes") {
							add_sea_routes(list);
							get_newest_sea_routes(list).StartLoad();
						} else {
							get_newest_sea_routes(list).LoadFromLine(line);
						}
					}
				}
			} catch {
				// 읽기실패
				return false;
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 최신 항로 정보를 얻는다
		---------------------------------------------------------------------------*/
		private Voyage get_newest_sea_routes() {
			return get_newest_sea_routes(m_sea_routes);
		}
		private Voyage get_newest_sea_routes(List<Voyage> list) {
			if (list.Count < 1) {
				// 없으므로 만든다
				add_sea_routes(list);
			}
			return list[list.Count - 1];
		}

		/*-------------------------------------------------------------------------
		 항로 정보를 추가한다
		---------------------------------------------------------------------------*/
		private void add_sea_routes() {
			add_sea_routes(m_sea_routes);
		}
		private void add_sea_routes(List<Voyage> list) {
			list.Add(new Voyage(m_lib));

			// 목록 갱신 리퀘스트
			m_req_update_list.Request();
		}

		/*-------------------------------------------------------------------------
		 보관 개수 조정
		---------------------------------------------------------------------------*/
		private void ajust_size() {
			// 항로도목록조정
			if (m_sea_routes.Count > 0) {
				while (m_sea_routes.Count > m_lib.setting.searoutes_group_max) {
					Voyage once = m_sea_routes[0];
					m_sea_routes.RemoveAt(0);
					// 과거 항로도 목록으로 이동시킨다
					m_trash_sea_routes.Add(once);

					// 목록 갱신 리퀘스트
					m_req_update_list.Request();
				}
			}
			// 과거의항로도목록조정
			while (m_trash_sea_routes.Count > m_lib.setting.trash_searoutes_group_max) {
				m_trash_sea_routes.RemoveAt(0);

				// 목록 갱신 리퀘스트
				m_req_update_list.Request();
			}
		}

		/*-------------------------------------------------------------------------
		 그리기
		 항로도
		---------------------------------------------------------------------------*/
		public void DrawRoutesLines() {
			// 보관 개수 조정
			ajust_size();

			// 선택 모드인지 판단한다
			check_select_mode();

			// 반투명 정도를 반영시킨다
			set_alpha();
			// 그릴 최저 항해 일수를 반영시킨다
			set_minimum_draw_days();

			// 항로도그리기
			draw_routes();
		}

		/*-------------------------------------------------------------------------
		 항로도를 전부 열거한다
		---------------------------------------------------------------------------*/
		private IEnumerable<Voyage> enum_all_sea_routes() {
			foreach (Voyage p in m_trash_sea_routes) yield return p;
			foreach (Voyage p in m_favorite_sea_routes) yield return p;
			foreach (Voyage p in m_sea_routes) yield return p;
		}

		/*-------------------------------------------------------------------------
		 항로도를 열거한다
		 휴지통은 열거되지 않는다
		---------------------------------------------------------------------------*/
		private IEnumerable<Voyage> enum_sea_routes_without_trash() {
			foreach (Voyage p in m_favorite_sea_routes) yield return p;
			foreach (Voyage p in m_sea_routes) yield return p;
		}

		/*-------------------------------------------------------------------------
		 선택 모드 중인지 판단한다
		---------------------------------------------------------------------------*/
		private void check_select_mode() {
			m_is_select_mode = false;

			IEnumerable<Voyage> list = enum_all_sea_routes();
			foreach (Voyage p in list) {
				if (p.IsSelected) {
					m_is_select_mode = true;
					break;
				}
			}
		}

		/*-------------------------------------------------------------------------
		 그리기
		 말풍선
		 DrawRoutesLines() 를 호출한 뒤여야 한다
		---------------------------------------------------------------------------*/
		public void DrawPopups() {
			// 재해그리기
			draw_accident();
			// 말풍선그리기
			draw_popups();
		}

		/*-------------------------------------------------------------------------
		 반투명 정도를 지정한다
		 설정에 따라 최신 항로 이외를 흐리게 한다
		---------------------------------------------------------------------------*/
		private void set_alpha() {
			if (m_lib.setting.enable_sea_routes_aplha) {
				// 최신 이외를 흐리게 한다
				foreach (Voyage p in m_sea_routes) {
					p.Alpha = SEAROUTES_ALPHA;
				}
				// 최신은 불투명
				if (m_sea_routes.Count > 0) {
					get_newest_sea_routes().Alpha = 1;
				}
			} else {
				// 전부 불투명
				foreach (Voyage p in m_sea_routes) {
					p.Alpha = 1;
				}
			}

			if (m_lib.setting.enable_favorite_sea_routes_alpha) {
				foreach (Voyage p in m_favorite_sea_routes) {
					p.Alpha = SEAROUTES_ALPHA;
				}
			} else {
				foreach (Voyage p in m_favorite_sea_routes) {
					p.Alpha = 1;
				}
			}
		}

		/*-------------------------------------------------------------------------
		 그릴 최저 항해 일수를 반영시킨다
		---------------------------------------------------------------------------*/
		private void set_minimum_draw_days() {
			foreach (Voyage p in m_sea_routes) {
				p.MinimumDrawDays = m_lib.setting.minimum_draw_days;
			}
			// 최신은 반드시 그린다
			if (m_sea_routes.Count > 0) {
				get_newest_sea_routes().MinimumDrawDays = 0;
			}
		}

		/*-------------------------------------------------------------------------
		 항로도그리기
		---------------------------------------------------------------------------*/
		private void draw_routes() {
			if (!m_lib.setting.draw_sea_routes) return;

			float size = 1 * m_lib.loop_image.ImageScale;
			if (size < 1) size = 1;
			else if (size > 2) size = 2;

			// 선택 모드 중에는 굵게 그린다
			if (m_is_select_mode) size *= 3;

			m_lib.device.line.Width = size;
			m_lib.device.line.Antialias = m_lib.setting.enable_line_antialias;
			m_lib.device.line.Pattern = -1;
			m_lib.device.line.PatternScale = 1.0f;

			m_lib.device.device.RenderState.ZBufferEnable = false;
			m_lib.device.line.Begin();
			m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_routes_proc), 8f);
			m_lib.device.line.End();

#if DRAW_SEA_ROUTES_BOUNDINGBOX
			// debug
			// 바운딩 박스그리기
			m_lib.loop_image.EnumDrawCallBack(new loop_x_image.DrawHandler(draw_routes_bb_proc), 8f); 
#endif
			m_lib.device.device.RenderState.ZBufferEnable = true;
		}

		/*-------------------------------------------------------------------------
		 항로도그리기
		---------------------------------------------------------------------------*/
		private void draw_routes_proc(Vector2 offset, LoopXImage image) {
			IEnumerable<Voyage> list = (m_is_select_mode)
														? enum_all_sea_routes()
														: enum_sea_routes_without_trash();
			foreach (Voyage p in list) {
				p.DrawRoutes(offset, image, m_is_select_mode);
			}
		}

		/*-------------------------------------------------------------------------
		 바운딩 박스그리기
		 디버그용
		---------------------------------------------------------------------------*/
		private void draw_routes_bb_proc(Vector2 offset, LoopXImage image) {
			IEnumerable<Voyage> list = (m_is_select_mode)
														? enum_all_sea_routes()
														: enum_sea_routes_without_trash();
			foreach (Voyage p in list) {
				p.DrawRoutesBB(offset, image, m_is_select_mode);
			}
		}

		/*-------------------------------------------------------------------------
		 말풍선그리기
		---------------------------------------------------------------------------*/
		private void draw_popups() {
			if (m_lib.setting.draw_popup_day_interval == 0) return;
			m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_popups_proc), 32f);
		}

		/*-------------------------------------------------------------------------
		 말풍선그리기
		---------------------------------------------------------------------------*/
		private void draw_popups_proc(Vector2 offset, LoopXImage image) {
			m_lib.device.sprites.BeginDrawSprites(m_lib.icons.texture, offset, image.ImageScale, new Vector2(1, 1));
			IEnumerable<Voyage> list = (m_is_select_mode)
														? enum_all_sea_routes()
														: enum_sea_routes_without_trash();
			foreach (Voyage p in list) {
				p.DrawPopups(offset, image, m_is_select_mode);
			}
			m_lib.device.sprites.EndDrawSprites();
		}

		/*-------------------------------------------------------------------------
		 재해그리기
		---------------------------------------------------------------------------*/
		private void draw_accident() {
			if (!m_lib.setting.draw_accident) return;

			m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_accident_proc), 32f);
		}

		/*-------------------------------------------------------------------------
		 재해그리기
		---------------------------------------------------------------------------*/
		private void draw_accident_proc(Vector2 offset, LoopXImage image) {
			m_lib.device.sprites.BeginDrawSprites(m_lib.icons.texture, offset, image.ImageScale, new Vector2(1, 1));

			if (m_is_select_mode) {
				IEnumerable<Voyage> list = enum_all_sea_routes();
				foreach (Voyage p in list) {
					p.DrawAccidents(offset, image, m_is_select_mode);
				}
			} else {
				if (m_lib.setting.draw_favorite_sea_routes_alpha_popup) {
					// 즐겨찾기 항로도의 재해 팝업을 그린다
					IEnumerable<Voyage> list = enum_sea_routes_without_trash();
					foreach (Voyage p in list) {
						p.DrawAccidents(offset, image, m_is_select_mode);
					}
				} else {
					// 즐겨찾기 항로도의 재해 팝업을 그리지 않음
					foreach (Voyage p in m_sea_routes) {
						p.DrawAccidents(offset, image, m_is_select_mode);
					}
				}
			}
			m_lib.device.sprites.EndDrawSprites();
		}

		/*-------------------------------------------------------------------------
		 좌표 추가 관계
		---------------------------------------------------------------------------*/
		private void init_add_points() {
			m_color_index = 0;
			m_old_days = 0;
			m_old_day_pos = new Point(0, 0);
			m_old_pos = new Point(0, 0);
			m_is_1st = true;
		}

		/*-------------------------------------------------------------------------
		 항로도 추가
		 거리가 가까운 경우에는 추가하지 않는다
		 날짜가 지난번보다 작으면 새 색으로 바뀐다
		 pos 에는 측량 좌표를 넘길 것
		---------------------------------------------------------------------------*/
		public void AddPoint(Point pos, int days, int accident) {
			if (days < 0) return;
			if (pos.X < 0) return;
			if (pos.Y < 0) return;

			// 지도 좌표로 변환
			Vector2 fpos = transform.game_pos2_map_pos(transform.ToVector2(pos), m_lib.loop_image);

			if (m_is_1st) {
				// 처음
				add_sea_routes();			   // 추가
				m_old_days = days;
				m_old_pos = pos;
				if (days > 0) {
					add_popup(new SeaRoutePopupPoint(fpos, m_color_index, days));
				}
				// 추가만 하고 라인은 구축하지 않는다
				add_point(new SeaRoutePoint(fpos, m_color_index));
				m_is_1st = false;
			} else {
				// 두 번째 이후
				if (m_old_days != days) {
					// 지난번과 같은 날짜가 아니다
					if (m_old_days > days) {
						// 한 번 기항했다
						if (++m_color_index >= 8) m_color_index = 0;
						add_sea_routes();			   // 추가
					}
					// 0 일은 추가하지 않는다
					if (days > 0) add_popup(new SeaRoutePopupPoint(fpos, m_color_index, days));
					m_old_days = days;
				}
				// 항로도
				if (m_old_pos != pos) {
					// 지난번과 위치가 다르다
					// 거리가 너무 가까운 경우에는 추가하지 않는다
					SeaRoutePoint p1 = new SeaRoutePoint(transform.ToVector2(m_old_pos));
					SeaRoutePoint p2 = new SeaRoutePoint(transform.ToVector2(pos));
					float min = ADD_POINT_MIN * transform.get_rate_to_game_x(m_lib.loop_image);
					if (p1.LengthSq(p2, (int)m_lib.loop_image.ImageSize.X) >= min) {
						add_point(new SeaRoutePoint(fpos, m_color_index));
						m_old_pos = pos;
					}
				}
			}

			// 재해
			if ((accident >= 101) && (accident <= 111)) {
				add_accident(new SeaRoutePopupPoint(fpos, accident, days));
			}
		}

		/*-------------------------------------------------------------------------
		 popup 을 추가한다
		---------------------------------------------------------------------------*/
		private void add_popup(SeaRoutePopupPoint p) {
			Voyage once = get_newest_sea_routes();
			once.AddPopup(p);

			// 목록 다시 그리기 리퀘스트
			m_req_redraw_list.Request();
		}

		/*-------------------------------------------------------------------------
		 accident 를 추가한다
		---------------------------------------------------------------------------*/
		private void add_accident(SeaRoutePopupPoint p) {
			Voyage once = get_newest_sea_routes();
			once.AddAccident(p);

			// 목록 다시 그리기 리퀘스트
			m_req_redraw_list.Request();
		}

		/*-------------------------------------------------------------------------
		 SeaRoutePoint 를 추가한다
		---------------------------------------------------------------------------*/
		private void add_point(SeaRoutePoint p) {
			Voyage once = get_newest_sea_routes();
			once.AddPoint(p);

			// 목록 다시 그리기 리퀘스트
			m_req_redraw_list.Request();
		}

		/*-------------------------------------------------------------------------
		 말풍선과 항로도 저장
		---------------------------------------------------------------------------*/
		public bool Write(string file_name) {
			return write(m_sea_routes, file_name);
		}

		/*-------------------------------------------------------------------------
		 즐겨찾기 항로도 저장
		---------------------------------------------------------------------------*/
		public bool WriteFavorite(string file_name) {
			return write(m_favorite_sea_routes, file_name);
		}

		/*-------------------------------------------------------------------------
		 휴지통 항로도 저장
		---------------------------------------------------------------------------*/
		public bool WriteTrash(string file_name) {
			return write(m_trash_sea_routes, file_name);
		}

		/*-------------------------------------------------------------------------
		 항로도 기록
		 sub
		---------------------------------------------------------------------------*/
		static private bool write(List<Voyage> list, string file_name) {
			try {
				using (StreamWriter sw = new StreamWriter(
					file_name, false, Encoding.GetEncoding("UTF-8"))) {

					foreach (Voyage p in list) {
						// 비어 있으면 기록하지 않는다
						if (p.IsEmpty) continue;

						sw.WriteLine("start routes");
						p.Write(sw);
					}
				}
			} catch {
				return false;
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 빈 항로를 삭제한다
		---------------------------------------------------------------------------*/
		private void remove_empty_routes() {
			while (true) {
				Voyage p = find_empty_routes();
				if (p == null) break;
				m_sea_routes.Remove(p);
			}
		}

		/*-------------------------------------------------------------------------
		 빈 항로를 찾는다
		---------------------------------------------------------------------------*/
		private Voyage find_empty_routes() {
			foreach (Voyage p in m_sea_routes) {
				if (p.IsEmpty) return p;
			}
			return null;
		}

		/*-------------------------------------------------------------------------
		 스크린샷용으로 바운딩 박스를 구한다
		 X 방향으로 전체 크기가 필요할 때는
		 태평양에서 구분된다는 점에 주의
		 그 외에는 최소 크기를 얻을 수 있다
		---------------------------------------------------------------------------*/
		public void CalcScreenShotBoundingBox(out Point offset, out Size size) {
			offset = new Point(0, 0);
			size = new Size((int)m_lib.loop_image.ImageSize.X, (int)m_lib.loop_image.ImageSize.Y);

			// 64 도트 단위 분포를 조사
			int map_count = (int)m_lib.loop_image.ImageSize.X / SS_DISTRIBUTION_X;
			if (((int)m_lib.loop_image.ImageSize.X % SS_DISTRIBUTION_X) != 0) map_count++;
			List<Point>[] map = new List<Point>[map_count];
			for (int i = 0; i < map_count; i++) {
				map[i] = new List<Point>();
			}

			int min_y = (int)m_lib.loop_image.ImageSize.Y;
			int max_y = 0;

			// X 방향은 대략적인 구분으로 등록한다
			// Y 방향은 그대로 최대 최소를 얻는다
			check_select_mode();
			IEnumerable<Voyage> list = (m_is_select_mode)
														? enum_all_sea_routes()
														: enum_sea_routes_without_trash();
			foreach (Voyage once in list) {
				once.SS_AddMinMaxList(map, ref min_y, ref max_y, m_is_select_mode);
			}

			// 항로도가 하나도 없는 경우에는 아무것도 하지 않고 돌아간다
			if (is_empty_list(map)) {
				offset = new Point(0, 0);
				size = new Size(0, 0);
				return;
			}

			// X 방향의 최대와 최소를 구한다
			// 빈 곳의 최대를 얻는다
			int start_index_x, free_count_x;
			calc_bounding_box_x(map, out start_index_x, out free_count_x);
			// 사용 범위를 구한다
			int size_x, offset_x;
			calc_screenshot_range(map, start_index_x, free_count_x, out size_x, out offset_x);

			// 오프셋과 크기 설정
			// 지도의 가로 크기보다 커질 수 있다
			size.Width = size_x;
			size.Width = (size.Width + 1) & ~1; // 2 의 배수로 만든다
			offset.X = offset_x;

			size.Height = max_y - min_y;
			size.Height = (size.Height + 1) & ~1;   // 2 의 배수로 만든다
			offset.Y = min_y;

			// 갭을 추가한다
			offset.X -= SCREEN_SHOT_BOUNDING_BOX_GAP_X;
			offset.Y -= SCREEN_SHOT_BOUNDING_BOX_GAP_Y;
			size.Width += SCREEN_SHOT_BOUNDING_BOX_GAP_X * 2;
			size.Height += SCREEN_SHOT_BOUNDING_BOX_GAP_Y * 2;
		}

		/*-------------------------------------------------------------------------
		 스크린샷용 범위를 구한다
		---------------------------------------------------------------------------*/
		private void calc_screenshot_range(List<Point>[] map, int start_index_x, int free_count_x, out int size_x, out int offset_x) {
			if (free_count_x <= 0) {
				// 빈 곳이 없으므로 전부 기록한다
				// 날짜 변경선 부근에서 구분한다
				offset_x = (int)-(m_lib.loop_image.ImageSize.X / 2);
				size_x = (int)m_lib.loop_image.ImageSize.X;
				return;
			}

			// 빈 곳에서 사용 범위를 구한다
			int use_count = map.Length - free_count_x;
			int min = start_index_x + free_count_x;
			if (min >= map.Length) min -= map.Length;
			if (min < 0) min += map.Length;
			int max = min + use_count - 1;
			if (max >= map.Length) max -= map.Length;
			if (max < 0) max += map.Length;

			// 최대와 최소를 얻는다
			Point min_point = map[min][0];
			foreach (Point p in map[min]) {
				if (p.X < min_point.X) min_point = p;
			}
			Point max_point = map[max][0];
			foreach (Point p in map[max]) {
				if (p.X > max_point.X) max_point = p;
			}

			// 잘라내기 시작 위치
			offset_x = min_point.X;
			// 사이즈
			if (max < min) {
				// 최대 쪽이 작은 경우에는 지도 크기를 더한 뒤 뺀다
				size_x = (max_point.X + (int)m_lib.loop_image.ImageSize.X) - min_point.X;
			} else {
				// 보통은 최대에서 최소를 뺀다
				size_x = max_point.X - min_point.X;
			}
		}

		/*-------------------------------------------------------------------------
		 항로도가 하나도 없는지 조사
		---------------------------------------------------------------------------*/
		private bool is_empty_list(List<Point>[] map) {
			int map_count = map.Length;
			for (int i = 0; i < map_count; i++) {
				if (map[i].Count > 0) return false; // 항로도 있음
			}
			// 항로도없음
			return true;
		}

		/*-------------------------------------------------------------------------
		 가장 넓은 빈 곳을 찾는다
		---------------------------------------------------------------------------*/
		private void calc_bounding_box_x(List<Point>[] map, out int start_index, out int free_count) {
			int map_count = map.Length;

			int max_start = -1;
			int max_count = 0;
			for (int i = 0; i < map_count; i++) {
				if (map[i].Count <= 0) {
					// 비어 있다
					int count = calc_bounding_box_x_sub(map, i);
					if (count > max_count) {
						max_start = i;
						max_count = count;
					}
				}
			}
			start_index = max_start;	// 빈 곳 시작 인덱스
			free_count = max_count; // 빈 곳 개수
		}

		/*-------------------------------------------------------------------------
		 연속된 빈 곳을 조사
		---------------------------------------------------------------------------*/
		private int calc_bounding_box_x_sub(List<Point>[] map, int start) {
			int map_count = map.Length;
			int index = 0;
			int empty_count = 0;
			while (index < map_count) {
				int i = start + index;
				if (i >= map_count) i -= map_count;

				if (map[i].Count > 0) break;		// 비어 있지 않다
				empty_count++;
				index++;
			}
			return empty_count;	 // 연속된 빈 곳을 반환한다
		}

		/*-------------------------------------------------------------------------
		 선택 상태를 리셋한다
		---------------------------------------------------------------------------*/
		public void ResetSelectFlag() {
			IEnumerable<Voyage> list = enum_all_sea_routes();
			foreach (Voyage p in list) p.IsSelected = false;
		}

		/*-------------------------------------------------------------------------
		 항로 목록에서 삭제한다
		 최신항로도가 삭제된 상황에 대응
		---------------------------------------------------------------------------*/
		public void RemoveSeaRoutes(List<Voyage> remove_list) {
			// 최신 항로도를 삭제할지 여부를 얻는다
			if (is_newest_sea_routes(remove_list)) {
				// 최신이 사라지므로
				// 시작 직후 상태로 만든다
				init_add_points();
			}

			// 삭제
			remove_sea_routes(m_sea_routes, remove_list);
		}

		/*-------------------------------------------------------------------------
		 항로 목록에서 삭제한다
		---------------------------------------------------------------------------*/
		public void remove_sea_routes(List<Voyage> list, List<Voyage> remove_list) {
			foreach (Voyage i in remove_list) {
				try {
					list.Remove(i);
				} catch {
				}
			}
		}

		/*-------------------------------------------------------------------------
		 항로도 즐겨찾기 목록에서 삭제한다
		 무조건 삭제해도 문제없다
		---------------------------------------------------------------------------*/
		public void RemoveFavoriteSeaRoutes(List<Voyage> remove_list) {
			remove_sea_routes(m_favorite_sea_routes, remove_list);
		}

		/*-------------------------------------------------------------------------
		 휴지통에 들어간 항로 목록에서 삭제한다
		 무조건 삭제해도 문제없다
		---------------------------------------------------------------------------*/
		public void RemoveTrashSeaRoutes(List<Voyage> remove_list) {
			remove_sea_routes(m_trash_sea_routes, remove_list);
		}

		/*-------------------------------------------------------------------------
		 최신 항로도를 삭제할지 여부를 얻는다
		---------------------------------------------------------------------------*/
		private bool is_newest_sea_routes(List<Voyage> list) {
			Voyage once = get_newest_sea_routes();
			foreach (Voyage i in list) {
				if (i == once) return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 항로도 이동
		 항로도에서 즐겨찾기 항로도로
		---------------------------------------------------------------------------*/
		public void MoveSeaRoutesToFavoriteSeaRoutes(List<Voyage> move_list) {
			// 추가
			add_sea_routes(m_sea_routes, m_favorite_sea_routes, move_list);
			// 원본을 삭제
			RemoveSeaRoutes(move_list);
		}

		/*-------------------------------------------------------------------------
		 항로도 이동
		 항로도에서 과거 항로도로
		---------------------------------------------------------------------------*/
		public void MoveSeaRoutesToTrashSeaRoutes(List<Voyage> move_list) {
			// 추가
			add_sea_routes(m_sea_routes, m_trash_sea_routes, move_list);
			// 원본을 삭제
			RemoveSeaRoutes(move_list);
		}

		/*-------------------------------------------------------------------------
		 항로도 이동
		 즐겨찾기 항로도에서 과거 항로도로
		---------------------------------------------------------------------------*/
		public void MoveFavoriteSeaRoutesToTrashSeaRoutes(List<Voyage> move_list) {
			// 추가
			add_sea_routes(m_favorite_sea_routes, m_trash_sea_routes, move_list);
			// 원본을 삭제
			RemoveFavoriteSeaRoutes(move_list);
		}

		/*-------------------------------------------------------------------------
		 항로도 이동
		 과거 항로도에서 즐겨찾기 항로도로
		---------------------------------------------------------------------------*/
		public void MoveTrashSeaRoutesToFavoriteSeaRoutes(List<Voyage> move_list) {
			// 추가
			add_sea_routes(m_trash_sea_routes, m_favorite_sea_routes, move_list);
			// 원본을 삭제
			RemoveTrashSeaRoutes(move_list);
		}

		/*-------------------------------------------------------------------------
		 항로도 이동
		 이동 원본에 있는 항로도만 이동한다
		 이동 원본에 없으면 이동하지 않는다
		---------------------------------------------------------------------------*/
		private void add_sea_routes(List<Voyage> from_list, List<Voyage> to_list, List<Voyage> move_list) {
			foreach (Voyage i in move_list) {
				if (has_sea_routes(from_list, i)) {
					// 추가
					to_list.Add(i);
				}
			}
		}

		/*-------------------------------------------------------------------------
		 지정된 항로도가 목록에 포함되는지 조사
		---------------------------------------------------------------------------*/
		private bool has_sea_routes(List<Voyage> list, Voyage routes) {
			foreach (Voyage i in list) {
				if (i == routes) return true;
			}
			return false;
		}
	}

	/*-------------------------------------------------------------------------
	 그리기색
	 반투명 부분은 0 을 반환하므로, 사용 시에 반투명값을 or 할 것
	---------------------------------------------------------------------------*/
	static internal class DrawColor {
		// 그리기색
		static private int[] m_color_tbl = new int[]{
										Color.FromArgb(0, 224,  64,  64).ToArgb(),	// 적
										Color.FromArgb(0, 224, 160,   0).ToArgb(),	// 오렌지
										Color.FromArgb(0, 224, 224,   0).ToArgb(),	// 황
										Color.FromArgb(0,  64, 160,  64).ToArgb(),	// 녹
										Color.FromArgb(0,  30, 160, 160).ToArgb(),	// 푸른
										Color.FromArgb(0, 120, 120, 230).ToArgb(),	// 연보라
										Color.FromArgb(0, 255, 255, 255).ToArgb(),	// 백
										Color.FromArgb(0, 255, 150, 150).ToArgb(),	// 핑크
									};

		/*-------------------------------------------------------------------------
		 그리기 색을 얻는다
		---------------------------------------------------------------------------*/
		static public int GetColor(int color_index) {
			if (color_index < 0) return m_color_tbl[0];
			if (color_index >= 8) return m_color_tbl[0];
			return m_color_tbl[color_index];
		}
	}
}
