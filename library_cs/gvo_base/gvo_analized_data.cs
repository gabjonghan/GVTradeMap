/*-------------------------------------------------------------------------

 캡처 화면에서 얻은 분석 정보
 로그 분석에서 얻은 정보
 를 관리한다
 TCP 서버 관계로 분석 정보를 한데 모아 관리함으로써
 어디서 얻은 정보인지 신경 쓸 필요가 없어진다

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvo_base
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class gvo_analized_data
	{
		private int										m_days;
		private int										m_pos_x;
		private int										m_pos_y;
		private bool									m_interest;
		private float									m_angle;
		private gvo_map_cs_chat_base.accident			m_accident;
		private bool									m_is_start_build_ship;
		private string									m_build_ship_name;
		private bool									m_is_finish_build_ship;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public bool capture_point_success{
			get{
				if(pos_x < 0)		return false;
				if(pos_y < 0)		return false;
				return true;		// 정상적으로 캡처했다
			}
		}
		public bool capture_days_success{	get{	return (days < 0)? false: true;	}}
		public bool capture_success{
			get{
				if(!capture_days_success)	return false;
				return capture_point_success;
			}
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public int		days
		{
			get{	return m_days;			}
			set{	m_days	= value;		}
		}
		public int		pos_x
		{
			get{	return m_pos_x;			}
			set{	m_pos_x	= value;		}
		}
		public int		pos_y
		{
			get{	return m_pos_y;			}
			set{	m_pos_y	= value;		}
		}
		public float	angle
		{
			get{	return m_angle;			}
			set{	m_angle	= value;		}
		}
		public bool		interest
		{
			get{	return m_interest;		}
			set{	m_interest	= value;	}
		}
		public gvo_map_cs_chat_base.accident accident
		{
			get{	return m_accident;		}
			set{	m_accident	= value;	}
		}
		public bool is_start_build_ship
		{
			get{	return m_is_start_build_ship;		}
			set{	m_is_start_build_ship	= value;	}
		}
		public string build_ship_name
		{
			get{	return m_build_ship_name;			}
			set{	m_build_ship_name	= value;		}
		}
		public bool is_finish_build_ship
		{
			get{	return m_is_finish_build_ship;		}
			set{	m_is_finish_build_ship	= value;	}
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public gvo_analized_data()
		{
			Clear();
		}

		/*-------------------------------------------------------------------------
		 내용 초기화
		---------------------------------------------------------------------------*/
		public void Clear()
		{
			m_days		= -1;
			m_pos_x		= -1;
			m_pos_y		= -1;
			m_angle		= -1;
			m_interest	= false;
			m_accident	= gvo_map_cs_chat_base.accident.none;

			m_is_start_build_ship	= false;
			m_build_ship_name		= "";
			m_is_finish_build_ship	= false;
		}

		/*-------------------------------------------------------------------------
		 복사
		---------------------------------------------------------------------------*/
		public gvo_analized_data Clone()
		{
			gvo_analized_data	data	= new gvo_analized_data();
			
			data.m_days					= m_days;
			data.m_pos_x				= m_pos_x;
			data.m_pos_y				= m_pos_y;
			data.m_angle				= m_angle;
			data.m_interest				= m_interest;
			data.m_accident				= m_accident;
			data.m_is_start_build_ship	= m_is_start_build_ship;
			data.m_build_ship_name		= m_build_ship_name;
			data.m_is_finish_build_ship	= m_is_finish_build_ship;
			return data;
		}

		/*-------------------------------------------------------------------------
		 분석 정보로부터 구축
		---------------------------------------------------------------------------*/
		static public gvo_analized_data FromAnalizedData(gvo_capture_base capture, gvo_map_cs_chat_base chat)
		{
			gvo_analized_data	data	= new gvo_analized_data();
			
			data.m_days					= capture.days;
			data.m_pos_x				= capture.point.X;
			data.m_pos_y				= capture.point.Y;
			data.m_angle				= capture.angle;
			data.m_interest				= chat.is_interest;
			data.m_accident				= chat._accident;
			data.m_is_start_build_ship	= chat.is_start_build_ship;
			data.m_build_ship_name		= chat.build_ship_name;
			data.m_is_finish_build_ship	= chat.is_finish_build_ship;

			// 조선 관계는 무조건 리셋한다
			chat.ResetBuildShip();

			if(data.capture_days_success){
				// 날짜를 캡처했다면 이자를 리셋한다
				chat.ResetInterest();	// 이자 리셋
			}
			if(data.capture_success){
				// 전부 캡처했다면 재해를 리셋한다
				chat.ResetAccident();	// 재해 리셋
			}
			return data;
		}
	}
}
