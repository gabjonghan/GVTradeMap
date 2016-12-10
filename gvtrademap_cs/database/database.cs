/*-------------------------------------------------------------------------

 정보管理

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using gvo_base;
using Utility;
using System.IO;
using Microsoft.DirectX;
using System.Diagnostics;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class GvoDatabase : IDisposable
	{
		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private GvoWorldInfo			m_world_info;		// 세계の정보
		private gvo_capture			m_capture;			// 캡처
		private SeaRoutes			m_searoute;			// 항로도
		private speed_calculator	m_speed;			// 속도
		private ShareRoutes		m_share_routes;		// 항로공유
		private WebIcons			m_web_icons;		// @web icon
		private gvo_chat			m_gvochat;			// 로그분석
		private interest_days		m_interest_days;	// 이자からの경과일수
		private gvo_build_ship_counter	m_build_ship_counter;	// 조선일수管理
		private map_mark			m_map_mark;			// 메모아이콘
		private ItemDatabaseCustom	m_item_database;	// 아이템DB
		private ShipPartsDataBase	m_ship_parts_database;	// 배부品DB
		private sea_area			m_sea_area;			// 위험해역변동시스템
		private gvo_season			m_season;			// 季節チェック
	
		private gvt_lib				m_lib;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public GvoWorldInfo World				{	get{	return m_world_info;		}}
		public gvo_capture Capture			{	get{	return m_capture;			}}
		public SeaRoutes SeaRoute			{	get{	return m_searoute;			}}
		public speed_calculator SpeedCalculator		{	get{	return m_speed;				}}
		public ShareRoutes ShareRoutes	{	get{	return m_share_routes;		}}
		public WebIcons WebIcons			{	get{	return m_web_icons;			}}
		public gvo_chat GvoChat				{	get{	return m_gvochat;			}}
		public interest_days InterestDays	{	get{	return m_interest_days;		}}
		public gvo_build_ship_counter BuildShipCounter	{	get{	return m_build_ship_counter;	}}
		public map_mark MapMark			{	get{	return m_map_mark;			}}
		public ItemDatabaseCustom ItemDatabase	{	get{	return m_item_database;		}}
		public sea_area SeaArea			{	get{	return m_sea_area;			}}
		public gvo_season GvoSeason			{	get{	return m_season;			}}
	
		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public GvoDatabase(gvt_lib lib)
		{
			m_lib						= lib;

			// 季節チェック
			m_season					= new gvo_season();

			// 세계の정보
			m_world_info				= new GvoWorldInfo(	lib,
															m_season,
															def.WORLDINFOS_FULLNAME,
															def.MEMO_PATH);

			// 아이템DB
			m_item_database				= new ItemDatabaseCustom(def.ITEMDB_FULLNAME);

			// 배パーツ
			m_ship_parts_database		= new ShipPartsDataBase(def.SHIP_PARTS_FULLNAME);
            m_item_database.MergeShipPartsDatabase(m_ship_parts_database);

			// 속도
			m_speed						= new speed_calculator(def.GAME_WIDTH);

			// 항로도
			m_searoute					= new SeaRoutes(	lib,
															def.SEAROUTE_FULLFNAME,
															def.FAVORITE_SEAROUTE_FULLFNAME,
															def.TRASH_SEAROUTE_FULLFNAME);

			// @web icons
			m_web_icons					= new WebIcons(lib);
			// 메모아이콘
			m_map_mark					= new map_mark(lib, def.MEMO_ICONS_FULLFNAME);
			// 항로공유
			m_share_routes				= new ShareRoutes(lib);
			// 화면캡처
			m_capture					= new gvo_capture(lib);
	
			// 이자からの경과일수
			m_interest_days				= new interest_days(lib.setting);
			// 조선일수管理
			m_build_ship_counter		= new gvo_build_ship_counter(lib.setting);

			// 위험해역변동시스템
			m_sea_area					= new sea_area(lib, def.SEAAREA_FULLFNAME);

			// 로그분석
			m_gvochat					= new gvo_chat(m_sea_area);
			// 1도로그분석をしておく
			// 분석내용は捨てる
			m_gvochat.AnalyzeNewestChatLog();
			m_gvochat.ResetAll();
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public void Dispose()
		{
			if(m_world_info != null)	m_world_info.Dispose();
			if(m_sea_area != null)		m_sea_area.Dispose();
			if(m_capture != null)		m_capture.Dispose();

			m_world_info	= null;
			m_sea_area		= null;
			m_capture		= null;
		}
	
		/*-------------------------------------------------------------------------
		 설정항목の書き出し
		---------------------------------------------------------------------------*/
		public void WriteSettings()
		{
			// 항로도の書き出し
			m_searoute.Write(def.SEAROUTE_FULLFNAME);
			// 즐겨찾기항로도の書き出し
			m_searoute.WriteFavorite(def.FAVORITE_SEAROUTE_FULLFNAME);
			// ごみ箱항로도の書き出し
			m_searoute.WriteTrash(def.TRASH_SEAROUTE_FULLFNAME);

			// 詳細정보書き出し
			m_world_info.WriteDomains(def.LOCAL_NEW_DOMAINS_INDEX_FULLFNAME);

			// 메모書き出し
			m_world_info.WriteMemo(def.MEMO_PATH);

			// 메모아이콘書き出し
			m_map_mark.Write(def.MEMO_ICONS_FULLFNAME);

			// 위험해역변동시스템정보書き出し
			m_sea_area.WriteSetting(def.SEAAREA_FULLFNAME);
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		public void Draw() 
		{
			DrawForScreenShot();
			// 공유배그리기
			m_share_routes.Draw();
		}

		/*-------------------------------------------------------------------------
		 스크린샷用그리기
		---------------------------------------------------------------------------*/
		public void DrawForScreenShot()
		{
			// @web icons
			m_web_icons.Draw();
			// 항로도그리기
			m_searoute.DrawRoutesLines();

			if(!m_lib.setting.is_mixed_info_names){
				// 해역명と풍향그리기
				m_world_info.DrawSeaName();
				// 도시명と상륙지점명그리기
				m_world_info.DrawCityName();
			}

			// 吹き出し그리기
			m_searoute.DrawPopups();

			// 메모아이콘
			m_map_mark.Draw();
		}

		/*-------------------------------------------------------------------------
		 지도と도시명の合成用
		 도시명合成しない場合は해역변동시스템のみ合成する
		---------------------------------------------------------------------------*/
		public void DrawForMargeInfoNames(Vector2 draw_offset, LoopXImage image)
		{
			// 해역변동시스템
			m_sea_area.Draw();

			if(m_lib.setting.is_mixed_info_names){
				// 해역명と풍향그리기
				m_world_info.DrawSeaName();
				// 도시명と상륙지점명그리기
				m_world_info.DrawCityName();
			}
		}

		/*-------------------------------------------------------------------------
		 전체검색
		---------------------------------------------------------------------------*/
		public List<Find> FindAll(string find_string)
		{
			List<Find>	list	= new List<Find>();

			Find.FindHandler	handler;

			// 검색방법
			// 부분일치도
			switch(m_lib.setting.find_filter3){
			case _find_filter3.full_match:
				handler		= new Find.FindHandler(Find.FindHander2);
				break;
			case _find_filter3.prefix_search:
				handler		= new Find.FindHandler(Find.FindHander3);
				break;
			case _find_filter3.suffix_search:
				handler		= new Find.FindHandler(Find.FindHander4);
				break;
			case _find_filter3.full_text_search:
			default:
				handler		= new Find.FindHandler(Find.FindHander1);
				break;
			}
			
			// 검색
			if(m_lib.setting.find_filter2 == _find_filter2.name){
				// 명칭 등から검색
				// 세계の정보から검색
				if(m_lib.setting.find_filter == _find_filter.both
					|| m_lib.setting.find_filter == _find_filter.world_info){
					World.FindAll(find_string, list, handler);
				}
				// 아이템DBから검색
				if(m_lib.setting.find_filter == _find_filter.both
					|| m_lib.setting.find_filter == _find_filter.item_database){
					m_item_database.FindAll(find_string, list, handler);
				}
			}else{
				// 종류별로검색
				// 세계の정보から검색
				if(m_lib.setting.find_filter == _find_filter.both
					|| m_lib.setting.find_filter == _find_filter.world_info){
					World.FindAll_FromType(find_string, list, handler);
				}
				// 아이템DBから검색
				if(m_lib.setting.find_filter == _find_filter.both
					|| m_lib.setting.find_filter == _find_filter.item_database){
					m_item_database.FindAll_FromType(find_string, list, handler);
				}
			}
			return list;
		}

		/*-------------------------------------------------------------------------
		 문화권목록を得る
		---------------------------------------------------------------------------*/
		public List<Find> GetCulturalSphereList()
		{
			// 문화권검색
			// 必ず特定の목록が得られる
			return new List<Find>(World.CulturalSphereList());
		}

		/*-------------------------------------------------------------------------
		 전체검색
		---------------------------------------------------------------------------*/
		public class Find
		{
			public enum FindType{
				Data,				// 아이템
				DataPrice,			// 아이템の가격内
				Database,			// 아이템DB
				InfoName,			// 도시명
				Lang,				// 사용언어
                CulturalSphere,		// 문화권
			};

			private FindType						m_type;				// 見つかった종류

			private GvoWorldInfo.Info.Group.Data	m_data;				// 아이템
			private ItemDatabase.Data				m_database;			// 아이템DB
			private string							m_info_name;		// 도시명
																		// 장소用に Type が database 以外は내용が入る
			private string							m_lang;				// 사용언어
			private GvoWorldInfo.CulturalSphere		m_cultural_sphere;	// 문화권검색時
			private string							m_cultural_sphere_tool_tip;

			// str1 に str2 が含まれるかどうかを返す
			public delegate bool FindHandler(string str1, string str2);

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public FindType Type						{	get{	return m_type;		}}
			public GvoWorldInfo.Info.Group.Data Data	{	get{	return m_data;		}}
			public ItemDatabaseCustom.Data Database			{	get{	return m_database;	}}
			public string InfoName						{	get{	return m_info_name;	}}
			public string Lang							{	get{	return m_lang;		}}
			public GvoWorldInfo.CulturalSphere CulturalSphere	{	get{	return m_cultural_sphere;	}}

			// 목록への표시用
			public string NameString{
				get{
					switch(Type){
					case FindType.Data:
						if(Data == null)		break;
						return Data.Name + "[" + Data.Price + "]";
					case FindType.DataPrice:
						if(Data == null)		break;
						return Data.Price + "[" + Data.Name + "]";
					case FindType.Database:
						if(Database == null)	break;
						return Database.Name;
					case FindType.InfoName:
						if(InfoName == null)	break;
						return InfoName;
					case FindType.Lang:
						if(Lang == null)		break;
						return Lang;
					case FindType.CulturalSphere:
						return GvoWorldInfo.GetCulturalSphereString(CulturalSphere);
					}
					return "불명";
				}
			}
			public string TypeString{
				get{
					switch(Type){
					case FindType.Data:
						if(Data == null)		break;
						if(Data.ItemDb != null)	return Data.ItemDb.Type;
						return Data.GroupIndexString;
					case FindType.DataPrice:
						if(Data == null)		break;
						if(Data.ItemDb != null)	return Data.ItemDb.Type + "[TAG]";
						return Data.GroupIndexString + "[TAG]";
					case FindType.Database:
						if(Database == null)	break;
						return Database.Type;
					case FindType.InfoName:
						return "도시명";
					case FindType.Lang:
						return "사용언어";
					case FindType.CulturalSphere:
						return "문화권";
					}
					return "불명";
				}
			}
			public string SpotString{
				get{
					switch(Type){
					case FindType.Data:
					case FindType.DataPrice:
					case FindType.Lang:
					case FindType.InfoName:
						if(InfoName == null)	break;
						return InfoName;
					case FindType.Database:
						return "아이템DB";
					case FindType.CulturalSphere:
						return "";
					}
					return "불명";
				}
			}
			public string TooltipString{
				get{
					switch(Type){
					case FindType.Database:
						if(Database == null)	break;
						return Database.GetToolTipString();
					case FindType.Data:
					case FindType.DataPrice:
						if(Data == null)		break;
						return Data.TooltipString;
					case FindType.InfoName:
					case FindType.Lang:
						break;
					case FindType.CulturalSphere:
						return m_cultural_sphere_tool_tip;
					}
					return "";
				}
			}
			
			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
      private Find()
      {
        this.m_type = FindType.InfoName;
        this.m_data = (GvoWorldInfo.Info.Group.Data) null;
        this.m_database = (ItemDatabase.Data) null;
        this.m_info_name = "";
        this.m_lang = (string) null;
        this.m_cultural_sphere = GvoWorldInfo.CulturalSphere.Unknown;
        this.m_cultural_sphere_tool_tip = "";
      }

			public Find(string _info_name) : this()
			{
				m_type				= FindType.InfoName;
				m_info_name			= _info_name;
			}
			public Find(FindType _type, string _info_name, GvoWorldInfo.Info.Group.Data _data) : this()
			{
				m_type				= _type;

				m_data				= _data;
				if(m_data != null){
					m_database	= m_data.ItemDb;
				}
				m_info_name			= _info_name;
			}
			public Find(string _info_name, string _lang) : this()
			{
				m_type				= FindType.Lang;
				m_info_name			= _info_name;
				m_lang				= _lang;
			}
			public Find(ItemDatabaseCustom.Data _database)
			{
				m_type				= FindType.Database;
				m_database			= _database;
			}
			public Find(GvoWorldInfo.CulturalSphere cs, string tooltip_str) : this()
			{
				m_type				= FindType.CulturalSphere;
				m_info_name			= GvoWorldInfo.GetCulturalSphereString(cs);
				m_cultural_sphere	= cs;
				m_cultural_sphere_tool_tip	= tooltip_str;
			}

			/*-------------------------------------------------------------------------
			 一致判定
			 부분일치
			---------------------------------------------------------------------------*/
			static public bool FindHander1(string str1, string str2)
			{
				if(str1.IndexOf(str2) >= 0){
					return true;
				}
				return false;
			}

			/*-------------------------------------------------------------------------
			 一致判定
			 완전일치
			---------------------------------------------------------------------------*/
			static public bool FindHander2(string str1, string str2)
			{
				return str1 == str2;
			}

			/*-------------------------------------------------------------------------
			 一致判定
			 앞부분일치
			---------------------------------------------------------------------------*/
			static public bool FindHander3(string str1, string str2)
			{
				return str1.StartsWith(str2);
			}

			/*-------------------------------------------------------------------------
			 一致判定
			 뒷부분일치
			---------------------------------------------------------------------------*/
			static public bool FindHander4(string str1, string str2)
			{
				return str1.EndsWith(str2);
			}
		}
	}
}
