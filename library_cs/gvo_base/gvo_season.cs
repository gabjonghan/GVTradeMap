/*-------------------------------------------------------------------------

 季節관리
 9 시간마다 여름과 겨울이 뒤바뀐다

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Utility;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvo_base
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class gvo_season
	{
		public enum season{
			summer,
			winter,
			MAX
		};
	
		private DateTime				m_next_season_start;	// 다음 회 계절 변동 시작 일시
		private DateTime				m_now_season_start;		// 이번 회 계절 변동 시작 일시
		private season					m_now_season;			// 현재 계절
		private DateTime				m_base_season_start;	// 기준이 되는 일시

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public DateTime next_season_start			{	get{	return m_next_season_start;				}}
		public string next_season_start_jbbsstr		{	get{	return Useful.TojbbsDateTimeString(m_next_season_start);		}}
		public string next_season_start_shortstr	{	get{	return Useful.ToShortDateTimeString(m_next_season_start);	}}
		public DateTime now_season_start			{	get{	return m_now_season_start;				}}
		public string now_season_start_jbbsstr		{	get{	return Useful.TojbbsDateTimeString(m_now_season_start);		}}
		public string now_season_start_shortstr		{	get{	return Useful.ToShortDateTimeString(m_now_season_start);		}}
		public season now_season					{	get{	return m_now_season;					}}
		public string now_season_str				{	get{	return ToSeasonString(m_now_season);	}}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public gvo_season()
		{
			// 여름의 기준이 되는 일시
			// 미래든 과거든 상관없다
			m_base_season_start	= new DateTime(2010, 3, 2, 13, 30, 0);	// 夏
			m_now_season		= season.MAX;

			// 更新
			UpdateSeason();
		}
		
		/*-------------------------------------------------------------------------
		 更新
		 계절이 바뀌었을 때 true 를 반환한다
		---------------------------------------------------------------------------*/
		public bool UpdateSeason()
		{
			bool		ret		= false;
			DateTime	now		= DateTime.Now;

			long	ticks		= now.Ticks - m_base_season_start.Ticks;
			long	t			= ticks / TimeSpan.FromHours(9).Ticks;
			if(t < 0)	t--;
			// 짝수면 여름, 홀수면 겨울
			season	now_s		= ((t & 1) == 0)? season.summer: season.winter;
			if(now_s != m_now_season)	ret	= true;	// 계절이 바뀌었다
			m_now_season		= now_s;	// 이번에 얻은 계절

			// 이번 회 변동 시작 일시
			m_now_season_start	= m_base_season_start.AddHours((t + 0) * 9);
//			Debug.WriteLine(TojbbsDateTimeString(m_now_season_start));
	
			// 다음 회 변동 시작 일시
			m_next_season_start	= m_base_season_start.AddHours((t + 1) * 9);
//			Debug.WriteLine(TojbbsDateTimeString(m_next_season_start));

			// 계절이 바뀌었는지 여부를 반환한다
			return ret;
		}

		/*-------------------------------------------------------------------------
		 계절을 문자열로 반환한다
		---------------------------------------------------------------------------*/
		public static string ToSeasonString(season s)
		{
			switch(s){
			case season.summer:		return "夏";
			case season.winter:		return "冬";
			}
			return "불명";
		}
	}
}
