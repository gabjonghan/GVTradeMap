/*-------------------------------------------------------------------------

 경과일수카운터
 完全な경과일수とはならない可能性があるが, 
 허용범위の精도は保てる

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
		private const int					DEF_COUNTER_MAX	= 99;	// 카운터がカンストする初期値
	
		private int							m_days;					// 確定분
		private int							m_voyage_days_start;	// チェック開始時の항해일수
		private int							m_voyage_days;			// 현재の항해일수
		private int							m_counter_max;			// 카운터がカンストする일수

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
				// 最初の업데이트
				m_voyage_days_start	= days;
				m_voyage_days		= days;
			}else{
				// 항해일수が前회よりも소さかったら
				// 確定일수を업데이트する
				if(days < m_voyage_days){
					m_days				+= m_voyage_days - m_voyage_days_start;
					m_voyage_days_start	= days;
					m_voyage_days		= days;
				}
				// 今회の항해일수
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
		 now_daysを基準とする
		---------------------------------------------------------------------------*/
		public void Reset(int now_days)
		{
			m_days					= 0;
			m_voyage_days_start		= now_days;
			m_voyage_days			= now_days;
		}

		/*-------------------------------------------------------------------------
		 경과일수を得る
		 m_counter_max日でカンストする
		---------------------------------------------------------------------------*/
		public int GetDays()
		{
			// 確定분+항해일수
			int	days	= get_true_days();
			return (days <= m_counter_max)? days: m_counter_max;
		}

		/*-------------------------------------------------------------------------
		 경과일수を得る
		---------------------------------------------------------------------------*/
		protected int get_true_days()
		{
			return m_days + (m_voyage_days - m_voyage_days_start);
		}
	}
}
