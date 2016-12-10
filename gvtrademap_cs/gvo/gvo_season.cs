/*-------------------------------------------------------------------------

 季節管理
 9時間毎に夏と冬が入れ換わる

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using useful;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs
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
	
		private DateTime				m_next_season_start;	// 次回の季節変動開始日時
		private DateTime				m_now_season_start;		// 今回の季節変動開始日時
		private season					m_now_season;			// 現在の季節
		private DateTime				m_base_season_start;	// 基準となる日時

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public DateTime next_season_start			{	get{	return m_next_season_start;				}}
		public string next_season_start_jbbsstr		{	get{	return useful.useful.TojbbsDateTimeString(m_next_season_start);		}}
		public string next_season_start_shortstr	{	get{	return useful.useful.ToShortDateTimeString(m_next_season_start);	}}
		public DateTime now_season_start			{	get{	return m_now_season_start;				}}
		public string now_season_start_jbbsstr		{	get{	return useful.useful.TojbbsDateTimeString(m_now_season_start);		}}
		public string now_season_start_shortstr		{	get{	return useful.useful.ToShortDateTimeString(m_now_season_start);		}}
		public season now_season					{	get{	return m_now_season;					}}
		public string now_season_str				{	get{	return ToSeasonString(m_now_season);	}}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public gvo_season()
		{
			// 夏の基準となる日時
			// 未来でも過去でもよい
			m_base_season_start	= new DateTime(2010, 3, 2, 13, 30, 0);	// 夏
			
			// 更新
			UpdateSeason();
		}
		
		/*-------------------------------------------------------------------------
		 更新
		---------------------------------------------------------------------------*/
		public void UpdateSeason()
		{
			DateTime	now		= DateTime.Now;

			long	ticks		= now.Ticks - m_base_season_start.Ticks;
			long	t			= ticks / TimeSpan.FromHours(9).Ticks;
			if(t < 0)	t--;
			// 偶数なら夏、基数なら冬
			m_now_season		= ((t & 1) == 0)? season.summer: season.winter;

			// 今回の変動開始日時
			m_now_season_start	= m_base_season_start.AddHours((t + 0) * 9);
//			Debug.WriteLine(TojbbsDateTimeString(m_now_season_start));
	
			// 次回の変動開始日時
			m_next_season_start	= m_base_season_start.AddHours((t + 1) * 9);
//			Debug.WriteLine(TojbbsDateTimeString(m_next_season_start));
		}

		/*-------------------------------------------------------------------------
		 季節を文字列で返す
		---------------------------------------------------------------------------*/
		public static string ToSeasonString(season s)
		{
			switch(s){
			case season.summer:		return "夏";
			case season.winter:		return "冬";
			}
			return "不明";
		}
	}
}
