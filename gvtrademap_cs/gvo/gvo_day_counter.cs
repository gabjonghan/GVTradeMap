/*-------------------------------------------------------------------------

 경과일수카운터
 완전한 경과 일수가 되지는 않을 수 있지만,
 허용 범위의 정확도는 유지된다

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class gvo_day_counter
	{
		private const int					DEF_COUNTER_MAX	= 99;	// 카운터가 최대치에 도달하는 초기값
	
		private int							m_days;					// 확정분
		private int							m_voyage_days_start;	// 확인 시작 시점의 항해 일수
		private int							m_voyage_days;			// 현재 항해 일수
		private int							m_counter_max;			// 카운터가 최대치에 도달하는 일수

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		protected int Days{
			get{
				return m_days;
			}
			set{
				m_days	= value;
			}
		}
		protected int VoyageDaysStart{
			get{
				return m_voyage_days_start;
			}
		}
		protected int VoyageDays{
			get{
				return m_voyage_days;
			}
		}
		protected int CounterMax{
			get{
				return m_counter_max;
			}
			set{
				m_counter_max	= value;
				if(m_counter_max < 0)	m_counter_max	= DEF_COUNTER_MAX;
			}
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public gvo_day_counter()
			: this(0)
		{
		}
		public gvo_day_counter(int days)
		{
			Reset();
			m_days					= days;
			m_counter_max			= DEF_COUNTER_MAX;
		}

		/*-------------------------------------------------------------------------
		 업데이트
		---------------------------------------------------------------------------*/
		protected virtual void UpdateBase(int days)
		{
			if(m_voyage_days_start < 0){
				// 최초 갱신
				m_voyage_days_start	= days;
				m_voyage_days		= days;
			}else{
				// 항해 일수가 지난번보다 작으면
				// 확정 일수를 갱신한다
				if(days < m_voyage_days){
					m_days				+= m_voyage_days - m_voyage_days_start;
					m_voyage_days_start	= days;
					m_voyage_days		= days;
				}
				// 이번 항해 일수
				m_voyage_days	= days;
			}
		}
	
		/*-------------------------------------------------------------------------
		 리셋
		---------------------------------------------------------------------------*/
		public void Reset()
		{
			Reset(-1);
		}

		/*-------------------------------------------------------------------------
		 리셋
		 now_days 를 기준으로 삼는다
		---------------------------------------------------------------------------*/
		public void Reset(int now_days)
		{
			m_days					= 0;
			m_voyage_days_start		= now_days;
			m_voyage_days			= now_days;
		}

		/*-------------------------------------------------------------------------
		 경과 일수를 얻는다
		 m_counter_max 일에서 최대치에 도달한다
		---------------------------------------------------------------------------*/
		public int GetDays()
		{
			// 확정분 + 항해 일수
			int	days	= get_true_days();
			return (days <= m_counter_max)? days: m_counter_max;
		}

		/*-------------------------------------------------------------------------
		 경과 일수를 얻는다
		---------------------------------------------------------------------------*/
		protected int get_true_days()
		{
			return m_days + (m_voyage_days - m_voyage_days_start);
		}
	}
}
