/*-------------------------------------------------------------------------

 チャット분석
 リクエスト付き
 預金の이자は재해とは独立して분석される
 위험해역변동시스템も独立して분석される
 アクシデントは분석時の最後のもののみ

 로그の업데이트チェックに時間が掛かるため, スレッド推奨
 해역변동は専用のメソッドUpdateSeaArea_DoRequest()でメインスレッドから行うこと

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
		 해역변동の업데이트
		 リクエストがあるときのみ분석する
		---------------------------------------------------------------------------*/
		public void UpdateSeaArea_DoRequest()
		{
			if(IsRequest()){
				update_sea_area();
			}
		}
	
		/*-------------------------------------------------------------------------
		 재해정보から保存用の値に변환する
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
			case accident.treasure1:			// 何かいい物
			case accident.treasure2:			// 何か見つかるかも
			case accident.treasure3:			// 高価なもの
				return 111;
			case accident.escape_battle:		// 全배が戦場を離れました
			case accident.win_battle:			// 승리
			case accident.lose_battle:			// 패배
				return 110;
			}
			return -1;		// unknown
		}

		/*-------------------------------------------------------------------------
		 해역변동시스템を업데이트する
		---------------------------------------------------------------------------*/
		private void update_sea_area()
		{
			sea_area_type[]	list	= base.sea_area_type_list;

			// 反映させる
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
