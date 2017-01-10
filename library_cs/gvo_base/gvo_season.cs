/*-------------------------------------------------------------------------

 季節관리
 9시간毎に夏と冬が入れ換わる

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
	
		private DateTime				m_next_season_start;	// 次회の季節変動開始일時
		private DateTime				m_now_season_start;		// 今회の季節変動開始일時
		private season					m_now_season;			// 現在の季節
		private DateTime				m_base_season_start;	// 基準となる일時

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
			// 여름의 基準となる일時
			// 未来でも過去でもよい
			m_base_season_start	= new DateTime(2010, 3, 2, 13, 30, 0);	// 夏
			m_now_season		= season.MAX;

			// 更新
			UpdateSeason();
		}
		
		/*-------------------------------------------------------------------------
		 更新
		 季節が変わったときtrueを返す
		---------------------------------------------------------------------------*/
		public bool UpdateSeason()
		{
			bool		ret		= false;
			DateTime	now		= DateTime.Now;

			long	ticks		= now.Ticks - m_base_season_start.Ticks;
			long	t			= ticks / TimeSpan.FromHours(9).Ticks;
			if(t < 0)	t--;
			// 偶수なら夏, 奇수なら冬
			season	now_s		= ((t & 1) == 0)? season.summer: season.winter;
			if(now_s != m_now_season)	ret	= true;	// 季節が変わった
			m_now_season		= now_s;	// 今회得たの季節

			// 今회の変動開始일時
			m_now_season_start	= m_base_season_start.AddHours((t + 0) * 9);
//			Debug.WriteLine(TojbbsDateTimeString(m_now_season_start));
	
			// 次회の変動開始일時
			m_next_season_start	= m_base_season_start.AddHours((t + 1) * 9);
//			Debug.WriteLine(TojbbsDateTimeString(m_next_season_start));

			// 季節が変わったかどうかを返す
			return ret;
		}

		/*-------------------------------------------------------------------------
		 季節を문자열で返す
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
