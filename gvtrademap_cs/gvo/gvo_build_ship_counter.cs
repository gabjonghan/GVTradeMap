/*-------------------------------------------------------------------------

 조선からの경과일수
 完全な경과일수とはならない가능性があるが, 
 허용범위の정확도は保てる

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using Utility;
using System.Text.RegularExpressions;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class gvo_build_ship_counter : gvo_day_counter
	{
		private const int					COUNTER_MAX	= 999;	// 999일でカンストする
	
		private GlobalSettings						m_setting;

		private bool						m_is_now_build;		// 조선중ならtrue
		private string						m_ship_name;		// 선박명
		private int							m_finish_days;		// 선박명から분석した조선종료일수

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public bool	IsNowBuild				{	get{	return m_is_now_build;		}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public gvo_build_ship_counter(GlobalSettings _setting)
			: base()
		{
			// カンストする일수を설정する
			base.CounterMax			= COUNTER_MAX;

			// 설정항목
			m_setting				= _setting;

			// 
			base.Days				= m_setting.build_ship_days;
			m_is_now_build			= m_setting.is_now_build_ship;
			m_ship_name				= m_setting.build_ship_name;
			m_finish_days			= get_build_ship_days(m_ship_name);	// 일수を得る
		}

		/*-------------------------------------------------------------------------
		 설정항목업데이트
		---------------------------------------------------------------------------*/
		private void update_settings()
		{
			m_setting.build_ship_days		= base.get_true_days();
			m_setting.is_now_build_ship		= m_is_now_build;
			m_setting.build_ship_name		= m_ship_name;
		}

		/*-------------------------------------------------------------------------
		 조선개시
		---------------------------------------------------------------------------*/
		public void StartBuildShip(string ship_name)
		{
			base.Reset();					// 카운터리셋
			m_is_now_build	= true;			// 조선개시
			m_ship_name		= ship_name;	// 선박명
			m_finish_days	= get_build_ship_days(m_ship_name);		// 일수を得る

			// 설정항목업데이트
			update_settings();
		}
	
		/*-------------------------------------------------------------------------
		 조선종료
		---------------------------------------------------------------------------*/
		public void FinishBuildShip()
		{
			m_is_now_build	= false;		// 조선종료
			base.Reset();					// 카운터리셋
			m_ship_name		= "";
			m_finish_days	= -1;

			// 설정항목업데이트
			update_settings();
		}
	
		/*-------------------------------------------------------------------------
		 업데이트
		---------------------------------------------------------------------------*/
		public void Update(int days)
		{
			if(!m_is_now_build)			return;

			// 일수カウントの업데이트
			base.UpdateBase(days);

			// 설정항목업데이트
			update_settings();
		}

		/*-------------------------------------------------------------------------
		 ポップアップ용の문자열を得る
		---------------------------------------------------------------------------*/
		public string GetPopupString()
		{
			if(m_is_now_build){
				// 조선중
				if(m_finish_days > 0){
					// 선박명から종료일수분석できている
					if(GetDays() > m_finish_days){
						// 完成する일수が경과している
						return String.Format("[ {0} ]を建造중\n{1}일경과\n完成から{2}일경과",
												m_ship_name,
												base.GetDays(),
												GetDays() - m_finish_days);
					}else if(GetDays() == m_finish_days){
						// 丁도完成してる
						return String.Format("[ {0} ]を建造중\n{1}일경과\n完成しました",
												m_ship_name,
												base.GetDays());
					}else{
						// 完成する일수が경과していない
						return String.Format("[ {0} ]を建造중\n{1}일경과\n残り{2}일",
												m_ship_name,
												base.GetDays(),
												m_finish_days - base.GetDays());
					}
				}else{
					// 선박명から종료일수が분석できていない
					return String.Format("[ {0} ]を建造중\n{1}일경과\n선박명に 14일 のような이름を付けると\n残り일수を계산できます",
											m_ship_name,
											base.GetDays());
				}
			}else{
				// 조선개시대기
				return "建造중ではありません";
			}
		}

		/*-------------------------------------------------------------------------
		 조선일수を得る
		 선박명に 14일 등が含まれていればそれを使う
		 99일でカンスト
		 일수が含まれない場合は-1を返す
		---------------------------------------------------------------------------*/
		private int get_build_ship_days(string name)
		{
			// 全角수字を반角に변환する
			name	= Useful.AdjustNumber(name);

			Match	m	= Useful.match(@"([0-9]+)일", name);
			if(m == null)	return -1;		// 일수が含まれない

			// 일수に변환
			return Useful.ToInt32(m.Groups[1].Value, -1);
		}
	}
}
