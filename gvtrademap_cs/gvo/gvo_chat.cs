/*-------------------------------------------------------------------------

 채팅 분석
 리퀘스트 포함
 예금 이자는 재해와는 독립적으로 분석된다
 위험해역 변동 시스템도 독립적으로 분석된다
 사고는 분석 시점의 마지막 것만

 로그 갱신 확인에 시간이 걸리므로 스레드 사용을 권장
 해역 변동은 전용 메서드 UpdateSeaArea_DoRequest() 로 메인 스레드에서 수행할 것

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using Utility;
using gvo_base;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class gvo_chat : gvo_map_cs_chat_base
	{
		private sea_area					m_sea_area;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public gvo_chat(sea_area _sea_area)
			: base()
		{
			m_sea_area		= _sea_area;
		}
		public gvo_chat(string path, sea_area _sea_area)
			: base(path)
		{
			m_sea_area		= _sea_area;
		}
		
		/*-------------------------------------------------------------------------
		 해역 변동 갱신
		 리퀘스트가 있을 때만 분석한다
		---------------------------------------------------------------------------*/
		public void UpdateSeaArea_DoRequest()
		{
			if(IsRequest()){
				update_sea_area();
			}
		}
	
		/*-------------------------------------------------------------------------
		 재해 정보를 저장용 값으로 변환한다
		---------------------------------------------------------------------------*/
		static public int ToIndex(accident a)
		{
			switch(a){
			case accident.shark1:				// 상어1
			case accident.shark2:				// 상어2
				return 101;
			case accident.fire:					// 화재
				return 102;
			case accident.seaweed:				// 수초
				return 103;
			case accident.seiren:				// 사이렌
				return 104;
			case accident.compass:				// 나침반
				return 105;
			case accident.storm:				// 폭풍
				return 106;
			case accident.blizzard:				// 눈보라
				return 107;
			case accident.mouse:				// 쥐
				return 108;
			case accident.UMA:					// 정체모를괴물
				return 109;
			case accident.treasure1:			// 뭔가 좋은 물건
			case accident.treasure2:			// 뭔가 발견될지도
			case accident.treasure3:			// 고가의 물건
				return 111;
			case accident.escape_battle:		// 모든 배가 전장을 떠났습니다
			case accident.win_battle:			// 승리
			case accident.lose_battle:			// 패배
				return 110;
			}
			return -1;		// unknown
		}

		/*-------------------------------------------------------------------------
		 해역 변동 시스템을 갱신한다
		---------------------------------------------------------------------------*/
		private void update_sea_area()
		{
			sea_area_type[]	list	= base.sea_area_type_list;

			// 반영시킨다
			if(list != null){
				foreach(sea_area_type d in list){
					switch(d.type){
					case sea_type.normal:
						m_sea_area.SetType(d.name, sea_area.sea_area_once.sea_type.normal);
						break;
					case sea_type.safty:
						m_sea_area.SetType(d.name, sea_area.sea_area_once.sea_type.safty);
						break;
					case sea_type.lawless:
						m_sea_area.SetType(d.name, sea_area.sea_area_once.sea_type.lawless);
						break;
					}
				}
			}

			// 리셋
			base.ResetSeaArea();
		}
	}
}
