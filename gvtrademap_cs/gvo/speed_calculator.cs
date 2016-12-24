/*-------------------------------------------------------------------------

 속도계산
 진행방향각도계산
 진행방향は측량の위치から得る

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
		// knot -> km 변환レート
		private const float						KM_KNOT_RATE	= 1.852f;
		// map -> knot 변환レート
		// 속도계산
		// from
		// 0.99版の속도계산とあっているかどうかは확인していませんが, 본인で확인したところまで…
		// 1.赤道上の一周=40075.1km
		// 2.측량の수字では一周=16384point(と仮定)
		// 3.リアルで1s=게임の0.4h
		// 4.リアルで1s間게임の중で1ktで進むと(1.852km/h*0.4h=0.7408km)
		// 5.0.7408kmは측량で0.3029point
		// 1kt=0.3029point/real_second
		// たとえばリアル1s間6point動いたら (6/0.3029)*(0.3029point/1real_second)=約20kt
		private const float						MAP_KNOT_RATE	= 0.3029f;
	
		/*-------------------------------------------------------------------------
		 계산용
		---------------------------------------------------------------------------*/
		class data
		{
			private float						m_length;		// 이동距離
			private Point						m_pos;			// 위치
			private float						m_interval;		// サンプリング간격

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
		
		// 保持する최대간격
		// 少なくとも계산용の간격よりも대きいこと
		private const int						LIST_INTERVAL_MAX			= 120*1000;
//		// 속도계산용최저간격
//		private const int						CALC_INTERVAL_MIN			= 6*1000;
		// 속도계산용최대간격
		private const int						CALC_INTERVAL_MAX			= 20*1000;
//		// 최저간격と최대간격での속도差がこの値未満なら최대간격の속도を採용する
//		private const float						CALC_INTERVAL_GAP_RATE_MAX	= 0.05f;

		// 각도계산時の최저계산간격
		// あまり간격が狭いと誤差が대きすぎて使い物にならない
		private const int						CALC_ANGLE_MIN_INTERVAL		= 20*1000;
		private const int						CALC_ANGLE_STEP_INTERVAL	= 5*1000;
		// 각도계산時の最高계산간격
		// 간격が広いほど誤差が少なくなるが, リアルタイム性が落ちる
		private const int						CALC_ANGLE_MAX_INTERVAL		= 60*1000;
		// 각도계산時の정확도を求めるためのもの
		private const int						CALC_ANGLE_MAX_PRECISION_COUNTS	= 
			((CALC_ANGLE_MAX_INTERVAL-CALC_ANGLE_MIN_INTERVAL)/CALC_ANGLE_STEP_INTERVAL) +1;

		// 최대각도差(degree)
		// 각도差のため+-で지정する배の범위が유효
		private const float						ANGLE_GAP_MAX				= 2;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private List<data>						m_length_list;		// 속도계산용위치목록
		private List<data>						m_angle_list;		// 각도계산용목록
		private int								m_map_size_x;		// X방향のループ사이즈(측량좌표계)

		private float							m_interval;			// 추가インターバル
		private Point							m_old_pos;			// 前회の위치
	
		private float							m_speed;			// 속도(측량좌표계)
		private float							m_angle;			// 각도

		private float							m_angle_gap_cos;	// 각도差용cos(θ)
		private float							m_angle_precision;	// 각도の정확도
																	// 0～1

		private bool							m_req_reset_angle;	// 각도계산の리셋

//		private bool							m_is_speed2;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		// 측량좌표계での속도
		public float speed_map			{		get{	return m_speed;					}}
		// ノット
		public float speed_knot			{		get{	return MapToKnotSpeed(m_speed);	}}
		// Km/h
		public float speed_km			{		get{	return MapToKmSpeed(m_speed);	}}
		// 각도(degree)
		public float angle				{		get{	return m_angle;					}}
		// 각도の정확도(0～1)
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
		 위치を추가
		 intervalはミリ초지정
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

			// 2초未満の업데이트時は목록を업데이트しない
			if(m_interval < 2000){
				return;
			}
			
			// 속도
			SeaRoutes.SeaRoutePoint	p1	= new SeaRoutes.SeaRoutePoint(transform.ToVector2(m_old_pos));
			SeaRoutes.SeaRoutePoint	p2	= new SeaRoutes.SeaRoutePoint(transform.ToVector2(pos));
			float				l	= p1.Length(p2, m_map_size_x);		// 距離
			if(l < 100){
				// 대きすぎる距離は無視する
				m_length_list.Add(new data(l, m_interval));
			}

			// 각도	
			m_angle_list.Add(new data(pos, m_interval));

			// 목록사이즈を조정する
			ajust_list_size(m_length_list);
			ajust_list_size(m_angle_list);
	
			// 
			m_interval	= 0;	// 업데이트간격初期化
			m_old_pos	= pos;	// 위치업데이트

			// 속도を계산する
			clac_speed();
			// 각도を계산する
			calc_angle();
		}

		/*-------------------------------------------------------------------------
		 데이터사이즈を조정する
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
		 간격のみ추가
		 측량の캡처に실패したとき용
		---------------------------------------------------------------------------*/
		public void AddIntervalOnly(int interval)
		{
			m_interval	+= interval;
		}

		/*-------------------------------------------------------------------------
		 속도を계산する
		---------------------------------------------------------------------------*/
		private void clac_speed()
		{
			m_speed			= calc_speed_sub(CALC_INTERVAL_MAX);

/*
			float	speed1	= calc_speed_sub(CALC_INTERVAL_MIN);
			float	speed2	= calc_speed_sub(CALC_INTERVAL_MAX);

			// 속도差によりどちらを使うかを決める
			float	tmp		= Math.Abs(speed1 - speed2);
			float	tmp2	= speed1 * CALC_INTERVAL_GAP_RATE_MAX;
			// 속도差がCALC_INTERVAL_GAP_RATE_MAX未満ならより간격の長いのもを사용する
			if(tmp < tmp2){
				// より安定する속도
				m_speed		= speed2;
				m_is_speed2	= true;
			}else{
				// 방향전換등でリアルタイム性が高いことが望まれる속도
				m_speed		= speed1;
				m_is_speed2	= false;
			}
*/
/*	
			float	l			= 0;
			float	interval	= 0;

			// 距離と計測간격の合計を出す
			// CALC_INTERVAL_MAX분を최대とする
			for(int i=m_length_list.Count-1; i>=0; i--){
				l			+= m_length_list[i].length;
				interval	+= m_length_list[i].interval;
				if(interval >= CALC_INTERVAL_MAX){
					// 安定する속도が得られる시간が得られた
					break;
				}
			}

			// 安定する속도が得られるサンプル수でなくても
			// 속도は求めておく

			// 1초에進んだ距離
			// 측량좌표계
			m_speed		= l / (interval / 1000);
*/
		}

		/*-------------------------------------------------------------------------
		 이동距離を得る
		 지정된간격분の속도を得る
		---------------------------------------------------------------------------*/
		private float calc_speed_sub(int calc_interval)
		{
			float	length		= 0;
			float	interval	= 0;

			// 距離と計測간격の合計を出す
			// calc_interval분を최대とする
			for(int i=m_length_list.Count-1; i>=0; i--){
				length		+= m_length_list[i].length;
				interval	+= m_length_list[i].interval;
				if(interval >= calc_interval){
					break;
				}
			}
			// 1초에進んだ距離
			// 측량좌표계
			return length / (interval / 1000);
		}
	
		/*-------------------------------------------------------------------------
		 측량 -> knot
		---------------------------------------------------------------------------*/
		public static float MapToKnotSpeed(float speed_map)
		{
			float knot	= speed_map * (1f / MAP_KNOT_RATE);

			// 속도が대きすぎる場合は0を返す
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
		 각도を계산する
		 ぶれは최대간격時+-0.1도程도
		---------------------------------------------------------------------------*/
		private void calc_angle()
		{
			if(m_angle_list.Count <= 1)		return;		// 각도を계산できない
	
			// 基準点
			int		count			= m_angle_list.Count;
			Point	pos				= m_angle_list[count -1].position;
			float	interval		= m_angle_list[count -1].interval;

			List<Point>	pos_list	= new List<Point>();

			// 条건の간격を満たす좌표を捜す
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

			if(pos_list.Count <= 0)		return;		// 각도が求められるサンプルが集まっていない

			// 各이동ベクトルを得る
			List<Vector2>	v_list		= new List<Vector2>();
			foreach(Point p in pos_list){
				Vector2		v_tmp	= transform.SubVector_LoopX(pos, p, m_map_size_x);
				v_tmp.Normalize();				// 正規化
				if(v_tmp.LengthSq() >= 0.7f){
					// ベクトルの長さが短すぎた場合추가しない
					v_list.Add(v_tmp);
				}
			}
			
			if(v_list.Count <= 0){
				// 각도が求められない
				m_angle				= -1;
				m_angle_precision	= 0;
				return;
			}

			// 최저でも1つの각도が得られることが確定している
			Vector2		v		= v_list[0];		// 基準とする각도

			// 安定する각도を선택する
			// できるだけ広い간격で得られた각도がよい
			int		precision	= 1;
			if(v_list.Count >= 2){
				Vector2	v2	= v;
				for(int i=1; i<v_list.Count; i++){
					// 각도差を得る
					float	cos	= Vector2.Dot(v, v_list[i]);
					// 각도差が대きすぎなら종료
					if(cos <= m_angle_gap_cos)		break;

					// 安定した각도
					v2		= v_list[i];

					precision++;
				}
				v	= v2;
			}

			// 진행방향정확도
			m_angle_precision	= (float)precision / CALC_ANGLE_MAX_PRECISION_COUNTS;

			// 표시向け각도をデグリーで得る	
			// radian -> degree
			m_angle		= Useful.ToDegree((float)Math.Atan2(v.Y, v.X));
			// 동が0도になってるので, 북を0도にする
			m_angle		+= 90;
			// -180 ～ +180
			//	0 ～  360 に조정する
			while(m_angle < 0)		m_angle		+= 360;
			while(m_angle >= 360)	m_angle		-= 360;
		}

		/*-------------------------------------------------------------------------
		 각도계산を리셋する
		---------------------------------------------------------------------------*/
		public void ResetAngle()
		{
			m_req_reset_angle	= true;
		}
	}
}
