/*-------------------------------------------------------------------------

 조선으로부터의 경과 일수
 완전한 경과 일수가 되지는 않을 수 있지만,
 허용 범위의 정확도는 유지된다

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
		private const int					COUNTER_MAX	= 999;	// 999 일에서 최대치에 도달한다
	
		private GlobalSettings						m_setting;

		private bool						m_is_now_build;		// 조선 중이면 true
		private string						m_ship_name;		// 선박명
		private int							m_finish_days;		// 선박명에서 분석한 조선 종료 일수

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public bool	IsNowBuild				{	get{	return m_is_now_build;		}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public gvo_build_ship_counter(GlobalSettings _setting)
			: base()
		{
			// 최대치에 도달하는 일수를 설정한다
			base.CounterMax			= COUNTER_MAX;

			// 설정항목
			m_setting				= _setting;

			// 
			base.Days				= m_setting.build_ship_days;
			m_is_now_build			= m_setting.is_now_build_ship;
			m_ship_name				= m_setting.build_ship_name;
			m_finish_days			= get_build_ship_days(m_ship_name);	// 일수를 얻는다
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
			m_finish_days	= get_build_ship_days(m_ship_name);		// 일수를 얻는다

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

			// 일수 카운트 갱신
			base.UpdateBase(days);

			// 설정항목업데이트
			update_settings();
		}

		/*-------------------------------------------------------------------------
		 팝업용 문자열을 얻는다
		---------------------------------------------------------------------------*/
		public string GetPopupString()
		{
			if(m_is_now_build){
				// 조선중
				if(m_finish_days > 0){
					// 선박명으로 종료 일수를 분석할 수 있었다
					if(GetDays() > m_finish_days){
						// 완성되는 일수가 경과했다
						return String.Format("[ {0} ] 을 건조중\n{1}일 경과\n완성으로부터 {2}일 경과",
												m_ship_name,
												base.GetDays(),
												GetDays() - m_finish_days);
					}else if(GetDays() == m_finish_days){
						// 딱 완성됐다
						return String.Format("[ {0} ] 을 건조중\n{1}일 경과\n완성되었습니다",
												m_ship_name,
												base.GetDays());
					}else{
						// 완성되는 일수가 경과하지 않았다
						return String.Format("[ {0} ] 을 건조중\n{1}일 경과\n남은 {2}일",
												m_ship_name,
												base.GetDays(),
												m_finish_days - base.GetDays());
					}
				}else{
					// 선박명으로 종료 일수를 분석하지 못했다
					return String.Format("[ {0} ] 을 건조중\n{1}일 경과\n선박명에 14일 같은 이름을 붙이면\n남은 일수를 계산할 수 있습니다",
											m_ship_name,
											base.GetDays());
				}
			}else{
				// 조선개시대기
				return "건조중이 아닙니다";
			}
		}

		/*-------------------------------------------------------------------------
		 조선 일수를 얻는다
		 선박명에 14일 등이 포함되어 있으면 그것을 사용한다
		 99 일에서 최대치 도달
		 일수가 포함되지 않은 경우에는 -1 을 반환한다
		---------------------------------------------------------------------------*/
		private int get_build_ship_days(string name)
		{
			// 전각 숫자를 반각으로 변환한다
			name	= Useful.AdjustNumber(name);

			Match	m	= Useful.match(@"([0-9]+)일", name);
			if(m == null)	return -1;		// 일수가 포함되지 않았다

			// 일수로 변환
			return Useful.ToInt32(m.Groups[1].Value, -1);
		}
	}
}
