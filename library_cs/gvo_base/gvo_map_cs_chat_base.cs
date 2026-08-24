/*-------------------------------------------------------------------------

 채팅 분석
 교역Map C# 용
 리퀘스트 포함
 예금 이자는 재해와는 독립적으로 분석된다
 위험해역 변동 시스템도 독립적으로 분석된다
 사고는 분석 시점의 마지막 것만

 스레드대응

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
namespace gvo_base {
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class gvo_map_cs_chat_base : gvo_chat_base {
		// 해역변동현황
		public enum sea_type {
			normal,	 // 통상
			safty,	  // 안전화
			lawless,	// 무법화
		};

		/*-------------------------------------------------------------------------
		 해역변동용
		---------------------------------------------------------------------------*/
		public class sea_area_type {
			private string m_name;
			private sea_type m_type;

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public string name { get { return m_name; } }
			public sea_type type { get { return m_type; } }

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public sea_area_type(string name, sea_type type) {
				m_name = name;
				m_type = type;
			}
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public enum accident {
			none,			   // 없음

			shark1,			 // 상어1
			shark2,			 // 상어2
			fire,			   // 화재
			seaweed,			// 수초
			seiren,			 // 사이렌
			compass,			// 나침반
			storm,			  // 폭풍
			blizzard,		   // 눈보라
			mouse,			  // 쥐
			plague,			 // 전염병
			scurvy,			 // 괴혈병
			UMA,				// 정체모를괴물
			treasure1,		  // 뭔가 좋은 물건
			treasure2,		  // 뭔가 발견될지도
			treasure3,		  // 고가의 물건
			escape_battle,	  // 모든 배가 전장을 떠났습니다
			win_battle,		 // 승리
			lose_battle,		// 패배

			// 아래는 사고가 아니지만 tag 로 사용된다
			interest,		   // 이자
			sea_type_normal,	// 해역변동 통상
			sea_type_safty,	 // 해역변동 안전화
			sea_type_lawless,   // 해역변동 무법화

			buildship_start,	// 조선개시
			buildship_finish,   // 조선완료
		};

		private accident m_accident;					// 이번에 분석한 재해
		private bool m_is_interest;			 // 이자가 오면 true
		private List<sea_area_type> m_sea_area_type_list;	   // 해역변동시스템용

		// 造배
		private bool m_is_start_build_ship;
		private string m_build_ship_name;
		private bool m_is_finish_build_ship;

		// 스레드대응
		private readonly object m_syncobject = new object();

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public accident _accident {
			get {
				accident acc;
				lock (m_syncobject) {
					acc = m_accident;
				}
				return acc;
			}
			private set {
				lock (m_syncobject) {
					m_accident = value;
				}
			}
		}
		public bool is_interest {
			get {
				bool is_interest;
				lock (m_syncobject) {
					is_interest = m_is_interest;
				}
				return is_interest;
			}
			private set {
				lock (m_syncobject) {
					m_is_interest = value;
				}
			}
		}
		public sea_area_type[] sea_area_type_list {
			get {
				lock (m_syncobject) {
					return m_sea_area_type_list.ToArray();
				}
			}
		}
		public bool is_start_build_ship {
			get {
				lock (m_syncobject) {
					return m_is_start_build_ship;
				}
			}
			set {
				lock (m_syncobject) {
					m_is_start_build_ship = value;
				}
			}
		}
		public string build_ship_name {
			get {
				lock (m_syncobject) {
					return m_build_ship_name;
				}
			}
			set {
				lock (m_syncobject) {
					m_build_ship_name = value;
				}
			}
		}
		public bool is_finish_build_ship {
			get {
				lock (m_syncobject) {
					return m_is_finish_build_ship;
				}
			}
			set {
				lock (m_syncobject) {
					m_is_finish_build_ship = value;
				}
			}
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public gvo_map_cs_chat_base()
			: base() {
			init();
		}
		public gvo_map_cs_chat_base(string path)
			: base(path) {
			init();
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private void init() {
			m_accident = accident.none;
			m_is_interest = false;
			m_sea_area_type_list = new List<sea_area_type>();

			// 분석 대상을 설정한다
			init_analyze_list();

			ResetAll();
		}

		/*-------------------------------------------------------------------------
		 분석 대상을 설정한다
		---------------------------------------------------------------------------*/
		private void init_analyze_list() {
			// 분석 규칙을 초기화한다
			base.ResetAnalizedList();

			// index0 의 항목
			base.AddAnalizeList(@"배가 식인상어에 습격당하고 있습니다.", type.index0, accident.shark1);
			base.AddAnalizeList(@"인喰いザメが現れました！", type.index0, accident.shark2);
			base.AddAnalizeList(@"화재가 발생했습니다!", type.index0, accident.fire);
			base.AddAnalizeList(@"수초が舵に絡まっています！", type.index0, accident.seaweed);
			base.AddAnalizeList(@"気味の悪い歌声が聞こえてきました", type.index0, accident.seiren);
			base.AddAnalizeList(@"자기장이 이상합니다. 나침반이 제대로 움직이지 않습니다.", type.index0, accident.compass);
			base.AddAnalizeList(@"폭풍が来ました！　돛を広げていると전覆してしまいます！", type.index0, accident.storm);
			base.AddAnalizeList(@"눈보라になりました！　돛を広げていると凍りついてしまいます", type.index0, accident.blizzard);
			base.AddAnalizeList(@"쥐가 대량 발생했습니다!", type.index0, accident.mouse);
			base.AddAnalizeList(@"전염병이 발생했습니다!", type.index0, accident.plague);
			base.AddAnalizeList(@"괴혈병이 발생했습니다!", type.index0, accident.scurvy);
			base.AddAnalizeList(@"정체모를괴물が現れました", type.index0, accident.UMA);
			base.AddAnalizeList(@"このあたりで何かいい物が見つかるかもしれません", type.index0, accident.treasure1);
			base.AddAnalizeList(@"このあたりで何か見つかるかもしれません", type.index0, accident.treasure2);
			base.AddAnalizeList(@"このあたりで高価なものが見つかるかもしれません", type.index0, accident.treasure3);
			base.AddAnalizeList(@"全배が戦場を離れました", type.index0, accident.escape_battle);

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
			base.AddAnalizeList(@"^(.+)의 건조를 주문했습니다.", type.regex, accident.buildship_start);
			base.AddAnalizeList(@"^(.+)의 강화を依頼しました", type.regex, accident.buildship_start);
			base.AddAnalizeList(@"`의 진수가 무사히 끝났습니다", type.any_index, accident.buildship_finish);
		}

		/*-------------------------------------------------------------------------
		 로그분석
		 여러 개의 재해 로그가 있어도, 마지막 것만 유효로 한다
		 (많은 팝업이 한 번에 생길 필요가 없기 때문)
		---------------------------------------------------------------------------*/
		public override bool AnalyzeNewestChatLog() {
			// 재해없음
			_accident = accident.none;

			// 이자가 왔는지 여부는 여기서 초기화되지 않는다
			// 해역이 변동했는지 여부는 여기서 초기화되지 않는다

			// 최신 로그를 분석
			if (!base.AnalyzeNewestChatLog()) return false;
			// 갱신되었는지 확인
			if (!base.is_update) return true;

			// 분석 내용을 확인한다
			update_analyze();
			return true;
		}

		/*-------------------------------------------------------------------------
		 분석 내용을 확인한다
		---------------------------------------------------------------------------*/
		private void update_analyze() {
			foreach (gvo_chat_base.analized_data d in base.analized_list) {
				switch ((accident)d.tag) {
					case accident.shark1:		   // 상어1
					case accident.shark2:		   // 상어2
					case accident.fire:			 // 화재
					case accident.seaweed:		  // 수초
					case accident.seiren:		   // 사이렌
					case accident.compass:		  // 나침반
					case accident.storm:			// 폭풍
					case accident.blizzard:		 // 눈보라
					case accident.mouse:			// 쥐
					case accident.plague:		   // 전염병
					case accident.scurvy:		   // 괴혈병
					case accident.UMA:			  // 정체모를괴물
					case accident.treasure1:		// 뭔가 좋은 물건
					case accident.treasure2:		// 뭔가 발견될지도
					case accident.treasure3:		// 고가의 물건
					case accident.escape_battle:	// 모든 배가 전장을 떠났습니다
					case accident.win_battle:	   // 승리
					case accident.lose_battle:	  // 패배
						Debug.WriteLine(d.line);
						_accident = (accident)d.tag;	// 단순히 덮어쓰기
						break;
					case accident.interest:		 // 이자
						Debug.WriteLine(d.line);
						m_is_interest = true;
						break;
					case accident.sea_type_normal:  // 해역변동 통상
						lock (m_syncobject) {
							m_sea_area_type_list.Add(new sea_area_type(d.match.Groups[1].Value, sea_type.normal));
						}
						Debug.WriteLine(d.line);
						break;
					case accident.sea_type_safty:   // 해역변동 안전화
						lock (m_syncobject) {
							m_sea_area_type_list.Add(new sea_area_type(d.match.Groups[1].Value, sea_type.safty));
						}
						Debug.WriteLine(d.line);
						break;
					case accident.sea_type_lawless: // 해역변동 무법화
						lock (m_syncobject) {
							m_sea_area_type_list.Add(new sea_area_type(d.match.Groups[1].Value, sea_type.lawless));
						}
						Debug.WriteLine(d.line);
						break;
					case accident.buildship_start:  // 조선개시
						is_start_build_ship = true;
						build_ship_name = d.match.Groups[1].Value;
						break;
					case accident.buildship_finish: // 조선완료
						is_finish_build_ship = true;
						break;
					default:
						break;
				}
			}
		}

		/*-------------------------------------------------------------------------
		 전부 리셋한다
		---------------------------------------------------------------------------*/
		public void ResetAll() {
			ResetAccident();
			ResetInterest();
			ResetSeaArea();
			ResetBuildShip();
		}

		/*-------------------------------------------------------------------------
		 재해 정보를 리셋한다
		---------------------------------------------------------------------------*/
		public void ResetAccident() {
			_accident = accident.none;
		}

		/*-------------------------------------------------------------------------
		 이자가 왔는지 여부를 리셋한다
		---------------------------------------------------------------------------*/
		public void ResetInterest() {
			is_interest = false;
		}

		/*-------------------------------------------------------------------------
		 해역 변동을 리셋한다
		---------------------------------------------------------------------------*/
		public void ResetSeaArea() {
			lock (m_syncobject) {
				m_sea_area_type_list.Clear();
			}
		}

		/*-------------------------------------------------------------------------
		 조선 정보를 리셋한다
		---------------------------------------------------------------------------*/
		public void ResetBuildShip() {
			m_is_start_build_ship = false;
			m_build_ship_name = "";
			m_is_finish_build_ship = false;
		}

		/*-------------------------------------------------------------------------
		 해역변동현황변환
		---------------------------------------------------------------------------*/
		static public string ToSeaTypeString(sea_type type) {
			switch (type) {
				case sea_type.safty: return "안전";
				case sea_type.lawless: return "무법";
				default: return "통상";
			}
		}
		static public sea_type ToSeaType(string type) {
			switch (type) {
				case "안전": return sea_type.safty;
				case "무법": return sea_type.lawless;
				default: return sea_type.normal;
			}
		}

		/*-------------------------------------------------------------------------
		 사고의 변환
		---------------------------------------------------------------------------*/
		static public string ToAccidentString(accident __accident) {
			switch (__accident) {
				case accident.shark1: return "상어1";
				case accident.shark2: return "상어2";
				case accident.fire: return "화재";
				case accident.seaweed: return "수초";
				case accident.seiren: return "사이렌";
				case accident.compass: return "나침반";
				case accident.storm: return "폭풍";
				case accident.blizzard: return "눈보라";
				case accident.mouse: return "쥐";
				case accident.plague: return "전염병";
				case accident.scurvy: return "괴혈병";
				case accident.UMA: return "괴물";
				case accident.treasure1: return "何かいい物";
				case accident.treasure2: return "何か見つかるかも";
				case accident.treasure3: return "高価なもの";
				case accident.escape_battle: return "해전이탈";
				case accident.win_battle: return "해전승리";
				case accident.lose_battle: return "해전패배";
				default: return "불명";
			}
		}
		static public accident ToAccident(string str) {
			switch (str) {
				case "상어1": return accident.shark1;
				case "상어2": return accident.shark2;
				case "화재": return accident.fire;
				case "수초": return accident.seaweed;
				case "사이렌": return accident.seiren;
				case "나침반": return accident.compass;
				case "폭풍": return accident.storm;
				case "눈보라": return accident.blizzard;
				case "쥐": return accident.mouse;
				case "전염병": return accident.mouse;
				case "괴혈병": return accident.mouse;
				case "괴물": return accident.UMA;
				case "何かいい物": return accident.treasure1;
				case "何か見つかるかも": return accident.treasure2;
				case "高価なもの": return accident.treasure3;
				case "해전이탈": return accident.escape_battle;
				case "해전승리": return accident.win_battle;
				case "해전패배": return accident.lose_battle;
				default: return accident.none;
			}
		}
	}
}
