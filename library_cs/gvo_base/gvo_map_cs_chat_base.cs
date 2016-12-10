/*-------------------------------------------------------------------------

 チャット분석
 교역MapC#向け
 リクエスト付き
 預金の이자は재해とは独立して분석される
 위험해역변동시스템も独立して분석される
 アクシデントは분석時の最後のもののみ

 スレッド対応

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

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvo_base
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class gvo_map_cs_chat_base : gvo_chat_base
	{
		// 해역변동状況
		public enum sea_type{
			normal,		// 통상
			safty,		// 안전화
			lawless,	// 무법화
		};

		/*-------------------------------------------------------------------------
		 해역변동用
		---------------------------------------------------------------------------*/
		public class sea_area_type
		{
			private string						m_name;
			private sea_type					m_type;

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public string		name			{	get{	return m_name;		}}
			public sea_type		type			{	get{	return m_type;		}}

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public sea_area_type(string name, sea_type type)
			{
				m_name		= name;
				m_type		= type;
			}
		}
	
		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public enum accident{
			none,				// なし

			shark1,				// 상어1
			shark2,				// 상어2
			fire,				// 화재
			seaweed,			// 수초
			seiren,				// 사이렌
			compass,			// 나침반
			storm,				// 폭풍
			blizzard,			// 눈보라
			mouse,				// 쥐
			UMA,				// 정체모를괴물
			treasure1,			// 何かいい物
			treasure2,			// 何か見つかるかも
			treasure3,			// 高価なもの
			escape_battle,		// 全배が戦場を離れました
			win_battle,			// 승리
			lose_battle,		// 패배

			// 以下はアクシデントではないがtagとして사용される
			interest,			// 이자
			sea_type_normal,	// 해역변동 통상
			sea_type_safty,		// 해역변동 안전화
			sea_type_lawless,	// 해역변동 무법화

			buildship_start,	// 조선개시
			buildship_finish,	// 조선완료
		};
	
		private accident					m_accident;					// 今회분석した재해
		private bool						m_is_interest;				// 이자が来たらtrue
		private List<sea_area_type>			m_sea_area_type_list;		// 해역변동시스템用

		// 造배
		private bool						m_is_start_build_ship;
		private string						m_build_ship_name;
		private bool						m_is_finish_build_ship;
	
		// スレッド対応
		private readonly object				m_syncobject	= new object();

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public accident _accident
		{
			get{
					accident	acc;
					lock(m_syncobject){
						acc		= m_accident;
					}
					return acc;
				}
			private set{
					lock(m_syncobject){
						m_accident	= value;
					}
				}
		}
		public bool is_interest
		{
			get{
					bool	is_interest;
					lock(m_syncobject){
						is_interest	= m_is_interest;
					}
					return is_interest;
				}
			private set{
					lock(m_syncobject){
						m_is_interest	= value;
					}
				}
		}
		public sea_area_type[] sea_area_type_list
		{
			get{
				lock(m_syncobject){
					return m_sea_area_type_list.ToArray();
				}
			}
		}
		public bool is_start_build_ship
		{
			get{
				lock(m_syncobject){
					return m_is_start_build_ship;
				}
			}
			set{
				lock(m_syncobject){
					m_is_start_build_ship	= value;
				}
			}
		}
		public string build_ship_name
		{
			get{
				lock(m_syncobject){
					return m_build_ship_name;
				}
			}
			set{
				lock(m_syncobject){
					m_build_ship_name	= value;
				}
			}
		}
		public bool is_finish_build_ship
		{
			get{
				lock(m_syncobject){
					return m_is_finish_build_ship;
				}
			}
			set{
				lock(m_syncobject){
					m_is_finish_build_ship	= value;
				}
			}
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public gvo_map_cs_chat_base()
			: base()
		{
			init();
		}
		public gvo_map_cs_chat_base(string path)
			: base(path)
		{
			init();
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private void init()
		{
			m_accident					= accident.none;
			m_is_interest				= false;
			m_sea_area_type_list		= new List<sea_area_type>();

			// 분석対象を설정する
			init_analyze_list();

			ResetAll();
		}

		/*-------------------------------------------------------------------------
		 분석対象を설정する
		---------------------------------------------------------------------------*/
		private void init_analyze_list()
		{
			// 분석ルールを初期化する
			base.ResetAnalizedList();

			// index0の항목
			base.AddAnalizeList(@"배員が상어に襲われています", type.index0, accident.shark1);
			base.AddAnalizeList(@"인喰いザメが現れました！", type.index0, accident.shark2);
			base.AddAnalizeList(@"화재が発生しました！", type.index0, accident.fire);
			base.AddAnalizeList(@"수초が舵に絡まっています！", type.index0, accident.seaweed);
			base.AddAnalizeList(@"気味の悪い歌声が聞こえてきました", type.index0, accident.seiren);
			base.AddAnalizeList(@"자기장が狂っています. 나침반が使いものになりません", type.index0, accident.compass);
			base.AddAnalizeList(@"폭풍が来ました！　돛を広げていると転覆してしまいます！", type.index0, accident.storm);
			base.AddAnalizeList(@"눈보라になりました！　돛を広げていると凍りついてしまいます", type.index0, accident.blizzard);
			base.AddAnalizeList(@"쥐가 대량 발생했습니다!", type.index0, accident.mouse);
			base.AddAnalizeList(@"정체모를괴물が現れました", type.index0, accident.UMA);
			base.AddAnalizeList(@"このあたりで何かいい物が見つかるかもしれません", type.index0, accident.treasure1);
			base.AddAnalizeList(@"このあたりで何か見つかるかもしれません", type.index0, accident.treasure2);
			base.AddAnalizeList(@"このあたりで高価なものが見つかるかもしれません", type.index0, accident.treasure3);
			base.AddAnalizeList(@"全배が戦場を離れました",	 type.index0, accident.escape_battle);

			// 
			base.AddAnalizeList(@"に승리しました！", type.any_index, accident.win_battle);
			base.AddAnalizeList(@"に패배しました…", type.any_index, accident.lose_battle);
	
			// 이자
			base.AddAnalizeList_Interest(accident.interest);

			// 해역변동
			base.AddAnalizeList(@"^(.+)が위험해역に戻りました！", type.regex, accident.sea_type_normal);
			base.AddAnalizeList(@"^(.+)が안전해역となりました！", type.regex, accident.sea_type_safty);
			base.AddAnalizeList(@"^(.+)が무법해역となりました！", type.regex, accident.sea_type_lawless);

			// 造배
			base.AddAnalizeList(@"^(.+)の建造を注文しました", type.regex, accident.buildship_start);
			base.AddAnalizeList(@"^(.+)の強化を依頼しました", type.regex, accident.buildship_start);
			base.AddAnalizeList(@"`의 진수가 무사히 끝났습니다", type.any_index, accident.buildship_finish);
		}
		
		/*-------------------------------------------------------------------------
		 로그분석
		 複数の재해로그があっても, 最後のものだけが유효とする
		 (たくさんのポップアップが1度に生まれる必要がないため)
		---------------------------------------------------------------------------*/
		public override bool AnalyzeNewestChatLog()
		{
			// 재해なし
			_accident							= accident.none;

			// 이자が来たかどうかはここではクリアされない
			// 해역が変動したかどうかはここではクリアされない

			// 最新로그を분석
			if(!base.AnalyzeNewestChatLog())	return false;
			// 更新されたかチェック
			if(!base.is_update)					return true;

			// 분석내용をチェックする
			update_analyze();
			return true;
		}

		/*-------------------------------------------------------------------------
		 분석내용をチェックする
		---------------------------------------------------------------------------*/
		private void update_analyze()
		{
			foreach(gvo_chat_base.analized_data d in base.analized_list){
				switch((accident)d.tag){
				case accident.shark1:			// 상어1
				case accident.shark2:			// 상어2
				case accident.fire:				// 화재
				case accident.seaweed:			// 수초
				case accident.seiren:			// 사이렌
				case accident.compass:			// 나침반
				case accident.storm:			// 폭풍
				case accident.blizzard:			// 눈보라
				case accident.mouse:			// 쥐
				case accident.UMA:				// 정체모를괴물
				case accident.treasure1:		// 何かいい物
				case accident.treasure2:		// 何か見つかるかも
				case accident.treasure3:		// 高価なもの
				case accident.escape_battle:	// 全배が戦場を離れました
				case accident.win_battle:		// 승리
				case accident.lose_battle:		// 패배
					Debug.WriteLine(d.line);
					_accident		= (accident)d.tag;	// 単純に上書き
					break;
				case accident.interest:			// 이자
					Debug.WriteLine(d.line);
					m_is_interest	= true;
					break;
				case accident.sea_type_normal:	// 해역변동 통상
					lock(m_syncobject){
						m_sea_area_type_list.Add(new sea_area_type(d.match.Groups[1].Value, sea_type.normal));
					}
					Debug.WriteLine(d.line);
					break;
				case accident.sea_type_safty:	// 해역변동 안전화
					lock(m_syncobject){
						m_sea_area_type_list.Add(new sea_area_type(d.match.Groups[1].Value, sea_type.safty));
					}
					Debug.WriteLine(d.line);
					break;
				case accident.sea_type_lawless:	// 해역변동 무법화
					lock(m_syncobject){
						m_sea_area_type_list.Add(new sea_area_type(d.match.Groups[1].Value, sea_type.lawless));
					}
					Debug.WriteLine(d.line);
					break;
				case accident.buildship_start:	// 조선개시
					is_start_build_ship		= true;
					build_ship_name			= d.match.Groups[1].Value;
					break;
				case accident.buildship_finish:	// 조선완료
					is_finish_build_ship	= true;
					break;
				default:
					break;
				}
			}
		}

		/*-------------------------------------------------------------------------
		 全てリセットする
		---------------------------------------------------------------------------*/
		public void ResetAll()
		{
			ResetAccident();
			ResetInterest();
			ResetSeaArea();
			ResetBuildShip();
		}
	
		/*-------------------------------------------------------------------------
		 재해정보をリセットする
		---------------------------------------------------------------------------*/
		public void ResetAccident()
		{
			_accident				= accident.none;
		}

		/*-------------------------------------------------------------------------
		 이자が来たかどうかをリセットする
		---------------------------------------------------------------------------*/
		public void ResetInterest()
		{
			is_interest				= false;
		}

		/*-------------------------------------------------------------------------
		 해역변동をリセットする
		---------------------------------------------------------------------------*/
		public void ResetSeaArea()
		{
			lock(m_syncobject){
				m_sea_area_type_list.Clear();
			}
		}

		/*-------------------------------------------------------------------------
		 造배정보をリセットする
		---------------------------------------------------------------------------*/
		public void ResetBuildShip()
		{
			m_is_start_build_ship	= false;
			m_build_ship_name		= "";
			m_is_finish_build_ship	= false;
		}

		/*-------------------------------------------------------------------------
		 해역변동状況変換
		---------------------------------------------------------------------------*/
		static public string ToSeaTypeString(sea_type type)
		{
			switch(type){
			case sea_type.safty:		return "안전";
			case sea_type.lawless:		return "무법";
			default:					return "통상";
			}
		}
		static public sea_type ToSeaType(string type)
		{
			switch(type){
			case "안전":				return sea_type.safty;
			case "무법":				return sea_type.lawless;
			default:					return sea_type.normal;
			}
		}

		/*-------------------------------------------------------------------------
		 アクシデントの変換
		---------------------------------------------------------------------------*/
		static public string ToAccidentString(accident __accident)
		{
			switch(__accident){
			case accident.shark1:			return "상어1";
			case accident.shark2:			return "상어2";
			case accident.fire:				return "화재";
			case accident.seaweed:			return "수초";
			case accident.seiren:			return "사이렌";
			case accident.compass:			return "나침반";
			case accident.storm:			return "폭풍";
			case accident.blizzard:			return "눈보라";
			case accident.mouse:			return "쥐";
			case accident.UMA:				return "괴물";
			case accident.treasure1:		return "何かいい物";
			case accident.treasure2:		return "何か見つかるかも";
			case accident.treasure3:		return "高価なもの";
			case accident.escape_battle:	return "해전이탈";
			case accident.win_battle:		return "해전승리";
			case accident.lose_battle:		return "해전패배";
			default:						return "なし";
			}
		}
		static public accident ToAccident(string str)
		{
			switch(str){
			case "상어1":				return accident.shark1;
			case "상어2":				return accident.shark2;
			case "화재":				return accident.fire;
			case "수초":					return accident.seaweed;
			case "사이렌":			return accident.seiren;
			case "나침반":				return accident.compass;
			case "폭풍":					return accident.storm;
			case "눈보라":				return accident.blizzard;
			case "쥐":				return accident.mouse;
			case "괴물":				return accident.UMA;
			case "何かいい物":			return accident.treasure1;
			case "何か見つかるかも":	return accident.treasure2;
			case "高価なもの":			return accident.treasure3;
			case "해전이탈":			return accident.escape_battle;
			case "해전승리":			return accident.win_battle;
			case "해전패배":			return accident.lose_battle;
			default:					return accident.none;
			}
		}
	}
}
