/*-------------------------------------------------------------------------

 속도계산
 진행방향각도계산
 진행 방향은 측량 위치에서 얻는다

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using Utility;


/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class speed_calculator
	{
		// knot -> km 변환 레이트
		private const float						KM_KNOT_RATE	= 1.852f;
		// map -> knot 변환 레이트
		// 속도계산
		// from
		// 0.99 판의 속도 계산과 맞는지는 확인하지 않았습니다만, 직접 확인한 데까지는…
		// 1. 적도상의 한 바퀴 = 40075.1km
		// 2. 측량 수치로는 한 바퀴 = 16384point (라고 가정)
		// 3. 현실의 1s = 게임의 0.4h
		// 4. 현실 1s 동안 게임 안에서 1kt 로 나아가면 (1.852km/h*0.4h=0.7408km)
		// 5. 0.7408km 는 측량으로 0.3029point
		// 1kt=0.3029point/real_second
		// 예를 들어 현실 1s 동안 6point 움직였다면 (6/0.3029)*(0.3029point/1real_second)=약 20kt
		private const float						MAP_KNOT_RATE	= 0.3029f;
	
		/*-------------------------------------------------------------------------
		 계산용
		---------------------------------------------------------------------------*/
		class data
		{
			private float						m_length;		// 이동 거리
			private Point						m_pos;			// 위치
			private float						m_interval;		// 샘플링 간격

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public float length				{	get{	return m_length;		}}
			public Point position			{	get{	return m_pos;			}}
			public float interval			{	get{	return m_interval;		}}

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public data(float length, float interval)
			{
				m_length		= length;
				m_pos			= new Point(0, 0);
				m_interval		= interval;
			}

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public data(Point pos, float interval)
			{
				m_length		= 0;
				m_pos			= pos;
				m_interval		= interval;
			}
		}
		
		// 보관할 최대 간격
		// 적어도 계산용 간격보다는 커야 한다
		private const int						LIST_INTERVAL_MAX			= 120*1000;
//		// 속도계산용최저간격
//		private const int						CALC_INTERVAL_MIN			= 6*1000;
		// 속도계산용최대간격
		private const int						CALC_INTERVAL_MAX			= 20*1000;
//		// 최저 간격과 최대 간격에서의 속도 차가 이 값 미만이면 최대 간격의 속도를 채용한다
//		private const float						CALC_INTERVAL_GAP_RATE_MAX	= 0.05f;

		// 각도 계산 시의 최저 계산 간격
		// 간격이 너무 좁으면 오차가 너무 커서 쓸 수 없다
		private const int						CALC_ANGLE_MIN_INTERVAL		= 20*1000;
		private const int						CALC_ANGLE_STEP_INTERVAL	= 5*1000;
		// 각도 계산 시의 최고 계산 간격
		// 간격이 넓을수록 오차가 줄어들지만, 실시간성이 떨어진다
		private const int						CALC_ANGLE_MAX_INTERVAL		= 60*1000;
		// 각도 계산 시의 정확도를 구하기 위한 것
		private const int						CALC_ANGLE_MAX_PRECISION_COUNTS	= 
			((CALC_ANGLE_MAX_INTERVAL-CALC_ANGLE_MIN_INTERVAL)/CALC_ANGLE_STEP_INTERVAL) +1;

		// 최대 각도차(degree)
		// 각도차이므로 +- 로 지정한 값의 두 배 범위가 유효
		private const float						ANGLE_GAP_MAX				= 2;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private List<data>						m_length_list;		// 속도계산용위치목록
		private List<data>						m_angle_list;		// 각도계산용목록
		private int								m_map_size_x;		// X 방향의 루프 크기(측량 좌표계)

		private float							m_interval;			// 추가 인터벌
		private Point							m_old_pos;			// 지난번 위치
	
		private float							m_speed;			// 속도(측량좌표계)
		private float							m_angle;			// 각도

		private float							m_angle_gap_cos;	// 각도차용 cos(θ)
		private float							m_angle_precision;	// 각도의 정확도
																	// 0～1

		private bool							m_req_reset_angle;	// 각도 계산 리셋

//		private bool							m_is_speed2;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		// 측량 좌표계에서의 속도
		public float speed_map			{		get{	return m_speed;					}}
		// 노트
		public float speed_knot			{		get{	return MapToKnotSpeed(m_speed);	}}
		// Km/h
		public float speed_km			{		get{	return MapToKmSpeed(m_speed);	}}
		// 각도(degree)
		public float angle				{		get{	return m_angle;					}}
		// 각도의 정확도(0~1)
		public float angle_precision	{		get{	return m_angle_precision;		}}

//		public bool is_speed2			{		get{	return m_is_speed2;				}}

	
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public speed_calculator(int map_size_x)
		{
			m_map_size_x		= map_size_x;
			m_length_list		= new List<data>();
			m_angle_list		= new List<data>();

			m_interval			= 0;
			m_old_pos			= new Point(0, 0);

			m_angle				= -1;

			m_angle_gap_cos		= (float)Math.Cos(Useful.ToRadian(ANGLE_GAP_MAX));
			m_angle_precision	= 0;

			m_req_reset_angle	= false;

			m_speed				= 0;
		}
	
		/*-------------------------------------------------------------------------
		 위치를 추가
		 interval 은 밀리초 지정
		 1000 = 1초
		---------------------------------------------------------------------------*/
		public void Add(Point pos, int interval)
		{
			// 각도계산리셋
			if(m_req_reset_angle){
				m_angle_list.Clear();
				m_angle				= -1;
				m_angle_precision	= 0;
				m_req_reset_angle	= false;
			}
	
			// 간격
			m_interval		+= interval;

			// 2 초 미만의 갱신 시에는 목록을 갱신하지 않는다
			if(m_interval < 2000){
				return;
			}
			
			// 속도
			SeaRoutes.SeaRoutePoint	p1	= new SeaRoutes.SeaRoutePoint(transform.ToVector2(m_old_pos));
			SeaRoutes.SeaRoutePoint	p2	= new SeaRoutes.SeaRoutePoint(transform.ToVector2(pos));
			float				l	= p1.Length(p2, m_map_size_x);		// 거리
			if(l < 100){
				// 너무 큰 거리는 무시한다
				m_length_list.Add(new data(l, m_interval));
			}

			// 각도	
			m_angle_list.Add(new data(pos, m_interval));

			// 목록 크기를 조정한다
			ajust_list_size(m_length_list);
			ajust_list_size(m_angle_list);
	
			// 
			m_interval	= 0;	// 갱신 간격 초기화
			m_old_pos	= pos;	// 위치업데이트

			// 속도를 계산한다
			clac_speed();
			// 각도를 계산한다
			calc_angle();
		}

		/*-------------------------------------------------------------------------
		 데이터 크기를 조정한다
		---------------------------------------------------------------------------*/
		private void ajust_list_size(List<data> list)
		{
			while(true){
				float	tmp		= 0;
				foreach(data s in list){
					tmp	+= s.interval;
				}
				if(tmp > LIST_INTERVAL_MAX)		list.RemoveAt(0);
				else							break;
			}
		}

		/*-------------------------------------------------------------------------
		 간격만 추가
		 측량 캡처에 실패했을 때용
		---------------------------------------------------------------------------*/
		public void AddIntervalOnly(int interval)
		{
			m_interval	+= interval;
		}

		/*-------------------------------------------------------------------------
		 속도를 계산한다
		---------------------------------------------------------------------------*/
		private void clac_speed()
		{
			m_speed			= calc_speed_sub(CALC_INTERVAL_MAX);

/*
			float	speed1	= calc_speed_sub(CALC_INTERVAL_MIN);
			float	speed2	= calc_speed_sub(CALC_INTERVAL_MAX);

			// 속도 차이에 따라 어느 쪽을 쓸지 정한다
			float	tmp		= Math.Abs(speed1 - speed2);
			float	tmp2	= speed1 * CALC_INTERVAL_GAP_RATE_MAX;
			// 속도 차가 CALC_INTERVAL_GAP_RATE_MAX 미만이면 간격이 더 긴 쪽을 사용한다
			if(tmp < tmp2){
				// 더 안정적인 속도
				m_speed		= speed2;
				m_is_speed2	= true;
			}else{
				// 방향 전환 등에서 실시간성이 높기를 바라는 속도
				m_speed		= speed1;
				m_is_speed2	= false;
			}
*/
/*	
			float	l			= 0;
			float	interval	= 0;

			// 거리와 계측 간격의 합계를 낸다
			// CALC_INTERVAL_MAX 분을 최대로 삼는다
			for(int i=m_length_list.Count-1; i>=0; i--){
				l			+= m_length_list[i].length;
				interval	+= m_length_list[i].interval;
				if(interval >= CALC_INTERVAL_MAX){
					// 안정적인 속도를 얻을 수 있는 시간이 확보되었다
					break;
				}
			}

			// 안정적인 속도를 얻을 수 있는 샘플 수가 아니더라도
			// 속도는 구해 둔다

			// 1 초에 나아간 거리
			// 측량좌표계
			m_speed		= l / (interval / 1000);
*/
		}

		/*-------------------------------------------------------------------------
		 이동 거리를 얻는다
		 지정된 간격만큼의 속도를 얻는다
		---------------------------------------------------------------------------*/
		private float calc_speed_sub(int calc_interval)
		{
			float	length		= 0;
			float	interval	= 0;

			// 거리와 계측 간격의 합계를 낸다
			// calc_interval 분을 최대로 삼는다
			for(int i=m_length_list.Count-1; i>=0; i--){
				length		+= m_length_list[i].length;
				interval	+= m_length_list[i].interval;
				if(interval >= calc_interval){
					break;
				}
			}
			// 1 초에 나아간 거리
			// 측량좌표계
			return length / (interval / 1000);
		}
	
		/*-------------------------------------------------------------------------
		 측량 -> knot
		---------------------------------------------------------------------------*/
		public static float MapToKnotSpeed(float speed_map)
		{
			float knot	= speed_map * (1f / MAP_KNOT_RATE);

			// 속도가 너무 큰 경우에는 0 을 반환한다
			if(knot < 100f)	return knot;
			return 0;
		}
	
		/*-------------------------------------------------------------------------
		 knot -> 측량
		---------------------------------------------------------------------------*/
		public static float KnotToMapSpeed(float knot)
		{
			return knot * MAP_KNOT_RATE;
		}
	
		/*-------------------------------------------------------------------------
		 knot -> km
		---------------------------------------------------------------------------*/
		public static float KnotToKmSpeed(float knot)
		{
			return knot * KM_KNOT_RATE;
		}
		
		/*-------------------------------------------------------------------------
		 km -> knot
		---------------------------------------------------------------------------*/
		public static float KmToKnotSpeed(float km)
		{
			return km * (1f / KM_KNOT_RATE);
		}
		
		/*-------------------------------------------------------------------------
		 km -> 측량
		---------------------------------------------------------------------------*/
		public static float KmToMapSpeed(float km)
		{
			return KnotToMapSpeed(KmToKnotSpeed(km));
		}

		/*-------------------------------------------------------------------------
		 측량 -> km
		---------------------------------------------------------------------------*/
		public static float MapToKmSpeed(float speed_map)
		{
			return KnotToKmSpeed(MapToKnotSpeed(speed_map));
		}

		/*-------------------------------------------------------------------------
		 각도를 계산한다
		 흔들림은 최대 간격일 때 +-0.1 도 정도
		---------------------------------------------------------------------------*/
		private void calc_angle()
		{
			if(m_angle_list.Count <= 1)		return;		// 각도를 계산할 수 없다
	
			// 기준점
			int		count			= m_angle_list.Count;
			Point	pos				= m_angle_list[count -1].position;
			float	interval		= m_angle_list[count -1].interval;

			List<Point>	pos_list	= new List<Point>();

			// 조건의 간격을 만족하는 좌표를 찾는다
			float	next_interval	= CALC_ANGLE_MIN_INTERVAL;
			for(int i=count-2; i>=0; i--){
				interval	+= m_angle_list[i].interval;
				if(interval >= next_interval){
					pos_list.Add(m_angle_list[i].position);

					next_interval	+= CALC_ANGLE_STEP_INTERVAL;
					if(next_interval > CALC_ANGLE_MAX_INTERVAL){
						break;
					}
				}
			}

			if(pos_list.Count <= 0)		return;		// 각도를 구할 수 있는 샘플이 모이지 않았다

			// 각 이동 벡터를 얻는다
			List<Vector2>	v_list		= new List<Vector2>();
			foreach(Point p in pos_list){
				Vector2		v_tmp	= transform.SubVector_LoopX(pos, p, m_map_size_x);
				v_tmp.Normalize();				// 정규화
				if(v_tmp.LengthSq() >= 0.7f){
					// 벡터의 길이가 너무 짧은 경우에는 추가하지 않는다
					v_list.Add(v_tmp);
				}
			}
			
			if(v_list.Count <= 0){
				// 각도를 구할 수 없다
				m_angle				= -1;
				m_angle_precision	= 0;
				return;
			}

			// 최소한 하나의 각도는 얻을 수 있음이 확정되어 있다
			Vector2		v		= v_list[0];		// 기준으로 삼을 각도

			// 안정적인 각도를 선택한다
			// 되도록 넓은 간격에서 얻은 각도가 좋다
			int		precision	= 1;
			if(v_list.Count >= 2){
				Vector2	v2	= v;
				for(int i=1; i<v_list.Count; i++){
					// 각도차를 얻는다
					float	cos	= Vector2.Dot(v, v_list[i]);
					// 각도차가 너무 크면 종료
					if(cos <= m_angle_gap_cos)		break;

					// 안정적인 각도
					v2		= v_list[i];

					precision++;
				}
				v	= v2;
			}

			// 진행방향정확도
			m_angle_precision	= (float)precision / CALC_ANGLE_MAX_PRECISION_COUNTS;

			// 표시용 각도를 도(degree)로 얻는다
			// radian -> degree
			m_angle		= Useful.ToDegree((float)Math.Atan2(v.Y, v.X));
			// 동쪽이 0 도로 되어 있으므로, 북쪽을 0 도로 만든다
			m_angle		+= 90;
			// -180 ～ +180
			//	0 ~ 360 으로 조정한다
			while(m_angle < 0)		m_angle		+= 360;
			while(m_angle >= 360)	m_angle		-= 360;
		}

		/*-------------------------------------------------------------------------
		 각도 계산을 리셋한다
		---------------------------------------------------------------------------*/
		public void ResetAngle()
		{
			m_req_reset_angle	= true;
		}
	}
}
