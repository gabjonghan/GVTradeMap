/*-------------------------------------------------------------------------

 도시とか해역とか상륙지점정보

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using directx;
using Utility;
using gvo_base;
using System.Diagnostics;
using Microsoft.DirectX;
using System.Xml;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class GvoWorldInfo : IDisposable {
		public enum InfoType {
			City,           // 도시
			Sea,            // 해역
			Shore,          // 상륙지점
			Shore2,         // 2차 필드
			OutsideCity,    // 교외
			PF,             // 개인농장
			InlandCity,     // 내륙도시
			GuildCity,      // 길드개척도시
		};

		// 서버
		public enum Server {
			Euros,
			Zephyros,
			Notos,
			Boreas,
			Helene,
			Polaris,
			Eirene,
			Unknown,
		};
		// 국
		public enum Country {
			Unknown,            // 무소속(보급항등)
			England,
			Spain,
			Portugal,
			Netherlands,
			France,
			Venezia,
			Turkey,
		};

		// 도시の종류
		public enum CityType {
			Capital,            // 수도
			City,               // 도시
			CapitalIslam,       // 수도(이슬람)
			CityIslam,          // 도시(이슬람)
		};
		// 동맹の종류
		public enum AllianceType {
			Unknown,            // 없음(보급항등)
			Alliance,           // 동맹
			Capital,            // 수도
			Territory,          // 영지
			Piratical,          // 해적섬
		};

		// 문화권
		public enum CulturalSphere {
			Unknown,            // 불명
			NorthEurope,        // 북유럽
			Germany,            // 독일
			Netherlands,        // 네덜란드
			Britain,            // 브리튼 섬
			NorthFrance,        // 프랑스 북부
			Iberian,            // 이베리아
			Atlantic,         // 대서양
			ItalySouthFrance,   // 이탈리아/남프랑스
			Balkan,       // 발칸
			Turkey,       // 터키
			NearEast,         // 서아시아
			NorthAfrica,        // 북아프리카
			WestAfrica,   // 서아프리카
			EastAfrica,   // 동아프리카
			Arab,             // 아랍
			Persia,       // 페르시아
			India,          // 인도
			Indochina,      // 인도차이나
			SoutheastAsia,  // 동남아시아
			Oceania,            // 오세아니아
			Caribbean,      // 카리브
			EastLatinAmerica,   // 중남미 동해안
			WestLatinAmerica,   // 중남미 서해안
			China,          // 화남
			Japan,          // 일본
			Taiwan,       // 대만
			Korea,          // 조선
			EastNorthAmerica,   // 북미 동해안
			WestNorthAmerica    // 북미 서해안
		};


		private static EnumParser<GvoWorldInfo.InfoType>[] m_infotype_enum_param = new EnumParser<GvoWorldInfo.InfoType>[8]
	{
	  new EnumParser<GvoWorldInfo.InfoType>(GvoWorldInfo.InfoType.City, "도시"),
	  new EnumParser<GvoWorldInfo.InfoType>(GvoWorldInfo.InfoType.Sea, "해역"),
	  new EnumParser<GvoWorldInfo.InfoType>(GvoWorldInfo.InfoType.Shore, "상륙지"),
	  new EnumParser<GvoWorldInfo.InfoType>(GvoWorldInfo.InfoType.Shore2, "2차 필드"),
	  new EnumParser<GvoWorldInfo.InfoType>(GvoWorldInfo.InfoType.OutsideCity, "교외"),
	  new EnumParser<GvoWorldInfo.InfoType>(GvoWorldInfo.InfoType.PF, "개인농장"),
	  new EnumParser<GvoWorldInfo.InfoType>(GvoWorldInfo.InfoType.InlandCity, "내륙도시"),
	  new EnumParser<GvoWorldInfo.InfoType>(GvoWorldInfo.InfoType.GuildCity, "길드개척도시")
	};
		private static EnumParser<GvoWorldInfo.Country>[] m_country_enum_param = new EnumParser<GvoWorldInfo.Country>[8]
	{
	  new EnumParser<GvoWorldInfo.Country>(GvoWorldInfo.Country.Unknown, "무소속"),
	  new EnumParser<GvoWorldInfo.Country>(GvoWorldInfo.Country.England, "잉글랜드"),
	  new EnumParser<GvoWorldInfo.Country>(GvoWorldInfo.Country.Spain, "스페인"),
	  new EnumParser<GvoWorldInfo.Country>(GvoWorldInfo.Country.Portugal, "포르투갈"),
	  new EnumParser<GvoWorldInfo.Country>(GvoWorldInfo.Country.Netherlands, "네덜란드"),
	  new EnumParser<GvoWorldInfo.Country>(GvoWorldInfo.Country.France, "프랑스"),
	  new EnumParser<GvoWorldInfo.Country>(GvoWorldInfo.Country.Venezia, "베네치아"),
	  new EnumParser<GvoWorldInfo.Country>(GvoWorldInfo.Country.Turkey, "오스만투르크")
	};
		private static EnumParser<GvoWorldInfo.CityType>[] m_citytype_enum_param = new EnumParser<GvoWorldInfo.CityType>[4]
	{
	  new EnumParser<GvoWorldInfo.CityType>(GvoWorldInfo.CityType.Capital, "수도"),
	  new EnumParser<GvoWorldInfo.CityType>(GvoWorldInfo.CityType.City, "도시"),
	  new EnumParser<GvoWorldInfo.CityType>(GvoWorldInfo.CityType.CapitalIslam, "수도(이슬람)"),
	  new EnumParser<GvoWorldInfo.CityType>(GvoWorldInfo.CityType.CityIslam, "도시(이슬람)")
	};
		private static EnumParser<GvoWorldInfo.AllianceType>[] m_alliancetype_enum_param = new EnumParser<GvoWorldInfo.AllianceType>[5]
	{
	  new EnumParser<GvoWorldInfo.AllianceType>(GvoWorldInfo.AllianceType.Unknown, "동맹없음"),
	  new EnumParser<GvoWorldInfo.AllianceType>(GvoWorldInfo.AllianceType.Alliance, "동맹국"),
	  new EnumParser<GvoWorldInfo.AllianceType>(GvoWorldInfo.AllianceType.Capital, "본거지"),
	  new EnumParser<GvoWorldInfo.AllianceType>(GvoWorldInfo.AllianceType.Territory, "영지"),
	  new EnumParser<GvoWorldInfo.AllianceType>(GvoWorldInfo.AllianceType.Piratical, "해적섬")
	};
		private static EnumParser<GvoWorldInfo.CulturalSphere>[] m_culturalsphere_enum_param = new EnumParser<GvoWorldInfo.CulturalSphere>[30]
	{
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Unknown, "불명"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.NorthEurope, "북유럽"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Germany, "독일"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Netherlands, "네덜란드"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Britain, "브리튼 섬"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.NorthFrance, "프랑스 북부"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Iberian, "이베리아"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Atlantic, "대서양"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.ItalySouthFrance, "이탈리아/남프랑스"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Balkan, "발칸"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Turkey, "터키"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.NearEast, "서아시아"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.NorthAfrica, "북아프리카"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.WestAfrica, "서아프리카"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.EastAfrica, "동아프리카"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Arab, "아랍"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Persia, "페르시아"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.India, "인도"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Indochina, "인도차이나"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.SoutheastAsia, "동남아시아"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Oceania, "오세아니아"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Caribbean, "카리브"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.EastLatinAmerica, "중남미 동해안"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.WestLatinAmerica, "중남미 서해안"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.China, "화남"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Japan, "일본"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Taiwan, "대만"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.Korea, "조선"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.EastNorthAmerica, "북미 동해안"),
	  new EnumParser<GvoWorldInfo.CulturalSphere>(GvoWorldInfo.CulturalSphere.WestNorthAmerica, "북미 서해안")
	};

		/*-------------------------------------------------------------------------
		 GvoWorldInfo
		---------------------------------------------------------------------------*/
		private gvt_lib m_lib;
		private gvo_season m_season;

		private draw_infonames m_draw_infonames;        // 도시명などの그리기

		private hittest_list m_world;            // 세계の정보
		private MultiDictionary<string, GvoWorldInfo.Info> m_world_hash_table;
		private GvoItemTypeDatabase m_item_type_db;   // 아이템の종류정보

		private hittest_list m_seas;        // 해역목록
		private hittest_list m_cities;      // 도시목록
		private hittest_list m_shores;      // 1차상륙지, 개인농장
		private hittest_list m_nonseas;     // 위 3가지에 해당하지 않는 모두 

		private GvoWorldInfo.Server m_server;            // 현재서버
		private GvoWorldInfo.Country m_my_country;    // 현재국가

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public hittest_list World { get { return m_world; } }

		public GvoWorldInfo.Server MyServer { get { return m_server; } }
		public GvoWorldInfo.Country MyCountry { get { return m_my_country; } }

		public hittest_list Seas { get { return m_seas; } }
		public hittest_list Cities { get { return m_cities; } }
		public hittest_list Shores { get { return m_shores; } }
		public hittest_list NoSeas { get { return m_nonseas; } }

		public gvo_season Season { get { return m_season; } }

		/*-------------------------------------------------------------------------
		 세계の정보관리
		---------------------------------------------------------------------------*/
		public GvoWorldInfo(gvt_lib lib, gvo_season season, string world_info_fname, string memo_path) {
			m_lib = lib;
			m_season = season;
			m_world = new hittest_list();
			m_world_hash_table = new MultiDictionary<string, GvoWorldInfo.Info>();
			m_item_type_db = new GvoItemTypeDatabase();
			m_seas = new hittest_list();
			m_cities = new hittest_list();
			m_shores = new hittest_list();
			m_nonseas = new hittest_list();

			m_draw_infonames = new draw_infonames(lib, this);

			// 서버と국を初期化
			// 오류번호としておく
			m_server = GvoWorldInfo.Server.Unknown;
			m_my_country = GvoWorldInfo.Country.Unknown;

			// XML정보を로드
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			load_info_xml(world_info_fname);
			Console.WriteLine("load_info_xml()=" + stopwatch.ElapsedMilliseconds.ToString());

			// 메모を로드
			load_memo(memo_path);

			// GvoDomains, GvoItemTypeDatabase등のDBとリンクする
			link_database();

			// 해역목록とその他목록を구축する
			create_seas_list();
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public void Dispose() {
			if (m_draw_infonames != null) m_draw_infonames.Dispose();
			m_draw_infonames = null;
		}

		/*-------------------------------------------------------------------------
		 GvoDomains, GvoItemTypeDatabase등のDBと링크
		---------------------------------------------------------------------------*/
		private void link_database() {
			int index = 0;
			foreach (Info i in m_world) {
				if (i.InfoType != InfoType.Sea) {
					// 해역以외
					bool has_name_image = true;

					if (i.CityInfo != null) {
						has_name_image = i.CityInfo.HasNameImage;
					}

					// 그리기용の矩形を업데이트する
					i.UpdateDrawRects(m_lib, i.Name);
					if (has_name_image) index++;
				}
			}
		}

		/*-------------------------------------------------------------------------
		 해역목록とその他목록を구축する
		---------------------------------------------------------------------------*/
		private void create_seas_list() {
			m_seas.Clear();
			m_cities.Clear();
			m_shores.Clear();
			m_nonseas.Clear();

			foreach (Info i in m_world) {
				if (i.InfoType == InfoType.Sea) {
					// 해역
					if (i.SeaInfo != null) m_seas.Add(i);
				} else if (i.InfoType == InfoType.City) {
					m_cities.Add(i);
				} else if (i.InfoType == InfoType.Shore || i.InfoType == InfoType.PF || i.InfoType == InfoType.GuildCity) {
					m_shores.Add(i);
				} else {
					// 해역이외 도시제외
					m_nonseas.Add(i);
				}
			}
		}

		/*-------------------------------------------------------------------------
		 메모を로드
		---------------------------------------------------------------------------*/
		private void load_memo(string path) {
			foreach (Info i in m_world) {
				i.LoadMemo(path);
			}
		}

		/*-------------------------------------------------------------------------
		 메모を書き出す
		---------------------------------------------------------------------------*/
		public void WriteMemo(string path) {
			foreach (Info i in m_world) {
				i.WriteMemo(path);
			}
		}

		/*-------------------------------------------------------------------------
		 맵좌표から검색
		---------------------------------------------------------------------------*/
		public Info FindInfo(Point map_pos) {
			return (Info)m_world.HitTest(map_pos);
		}

		/*-------------------------------------------------------------------------
		 맵좌표から검색
		 해역を含まない
		---------------------------------------------------------------------------*/
		public Info FindInfo_WithoutSea(Point map_pos) {
			Info i = (Info)m_world.HitTest(map_pos);
			if (i == null) return null;
			if (i.InfoType == InfoType.Sea) return null;
			return (Info)m_world.HitTest(map_pos);
		}

		/*-------------------------------------------------------------------------
		 이름から검색
		---------------------------------------------------------------------------*/
		public Info FindInfo(string name) {
			if (string.IsNullOrEmpty(name)) return null;
			return m_world_hash_table.GetValue(name);
		}

		/*-------------------------------------------------------------------------
		 전체검색
		 검색する문자を含むものを전체검색する
		---------------------------------------------------------------------------*/
		public void FindAll(string find_string, List<GvoDatabase.Find> list, GvoDatabase.Find.FindHandler handler) {
			foreach (Info _info in m_world) {
				_info.FindAll(find_string, list, handler);
			}
		}

		/*-------------------------------------------------------------------------
		 종류で검색
		 검색する문자を含むものを전체검색する
		 例えば 食料품 등での검색용
		---------------------------------------------------------------------------*/
		public void FindAll_FromType(string find_string, List<GvoDatabase.Find> list, GvoDatabase.Find.FindHandler handler) {
			foreach (Info _info in m_world) {
				_info.FindAll_FromType(find_string, list, handler);
			}
		}

		/*-------------------------------------------------------------------------
		 문화권목록を작성함
		---------------------------------------------------------------------------*/
		public GvoDatabase.Find[] CulturalSphereList() {
			List<GvoDatabase.Find> list = new List<GvoDatabase.Find>();
			foreach (GvoWorldInfo.CulturalSphere cs in Enum.GetValues(typeof(GvoWorldInfo.CulturalSphere))) {
				if (cs != GvoWorldInfo.CulturalSphere.Unknown) {
					string culturalSphereTooltip = get_cultural_sphere_tooltip(cs);
					list.Add(new GvoDatabase.Find(cs, culturalSphereTooltip));
				}
			}
			return list.ToArray();
		}

		private string get_cultural_sphere_tooltip(GvoWorldInfo.CulturalSphere cs) {
			string str = "";
			foreach (GvoWorldInfo.Info info in m_world) {
				if (info.CulturalSphere == cs)
					str = str + info.Name + "\n";
			}
			return str;
		}

		public bool DownloadDomains(string domaininfo_file_name) {
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			if (load_domains_from_old_data(download_domains())) {
				WriteDomains(domaininfo_file_name);
				Console.WriteLine("동맹국다운로드" + stopwatch.ElapsedMilliseconds.ToString());
				return true;
			} else {
				Console.WriteLine("동맹국다운로드(실패)" + stopwatch.ElapsedMilliseconds.ToString());
				return false;
			}
		}

		/*-------------------------------------------------------------------------
		 디테일정보の읽기
		 스레드読みに대응
		---------------------------------------------------------------------------*/
		public bool Load(string info_file_name, string domaininfo_file_name, string local_domaininfo_file_name) {
			// 동맹현황읽기
			load_domains_xml(domaininfo_file_name, local_domaininfo_file_name);

			// 아이템분類정보읽기
			m_item_type_db.Load();
			load_items_xml(info_file_name);

			// 서버와 국가를 반영함
			SetServerAndCountry(GvoWorldInfo.Server.Polaris, GvoWorldInfo.Country.France);
			return true;
		}

		/*-------------------------------------------------------------------------
		 아이템DBとリンクさせる
		---------------------------------------------------------------------------*/
		public void LinkItemDatabase(ItemDatabaseCustom item_db) {
			foreach (Info _info in m_world) {
				_info.LinkItemDatabase(item_db);
			}
		}

		/*-------------------------------------------------------------------------
		 서버와 국가를 설정
		---------------------------------------------------------------------------*/
		public void SetServerAndCountry(GvoWorldInfo.Server server, GvoWorldInfo.Country country) {
			if ((m_server == server)
				&& (m_my_country == country)) {
				// 이전과 같을 경우 아무것도 하지 않음
				return;
			}

			// 설정 업데이트
			m_server = server;
			m_my_country = country;

			// 설정을 반영해둠
			update_domains();
		}

		/*-------------------------------------------------------------------------
		 서버와 국가를 설정
		---------------------------------------------------------------------------*/
		private void update_domains() {
			foreach (Info _info in m_world) {
				// DBを업데이트する
				_info.UpdateDomains(m_item_type_db, m_server, m_my_country);
			}
		}

		/*-------------------------------------------------------------------------
		 동맹국を변경する
		---------------------------------------------------------------------------*/
		public bool SetDomain(string city_name, GvoWorldInfo.Country country) {
			GvoWorldInfo.Info info = FindInfo(city_name);
			if (info == null || info.CityInfo == null) {
				return false;
			}

			// 변경
			info.CityInfo.SetDomain(m_server, country);

			// 설정を反映させておく
			update_domains();
			return true;
		}

		/*-------------------------------------------------------------------------
		 도시명그리기
		 상륙지점も含む
		---------------------------------------------------------------------------*/
		public void DrawCityName() {
			m_draw_infonames.DrawCityName();
		}

		/*-------------------------------------------------------------------------
		 해역명그리기
		---------------------------------------------------------------------------*/
		public void DrawSeaName() {
			m_draw_infonames.DrawSeaName();
		}

		/*-------------------------------------------------------------------------
		 동맹국업데이트용の문자열を得る
		 "서버번호"+"도시번호"+"동맹국번호"
		---------------------------------------------------------------------------*/
		public string GetNetUpdateString(string city_name) {
			GvoWorldInfo.Info info = FindInfo(city_name);
			if (info == null)
				return (string)null;
			if (info.CityInfo == null)
				return (string)null;
			else
				return info.CityInfo.GetNetUpdateString(m_server);
		}

		/*-------------------------------------------------------------------------
		 세율を考慮した価格を得る
		---------------------------------------------------------------------------*/
		static public int GetTaxPrice(int price) {
			return price + (int)(def.TAX * price);
		}

		private void load_info_xml(string file_name) {
			m_world.Clear();
			m_world_hash_table.Clear();
			try {
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(file_name);
				if (xmlDocument.DocumentElement == null || xmlDocument.DocumentElement.ChildNodes.Count <= 0)
					return;
				foreach (XmlNode n in xmlDocument.DocumentElement.ChildNodes) {
					GvoWorldInfo.Info t = GvoWorldInfo.Info.FromXml(n);
					if (t != null) {
						m_world.Add((hittest)t);
						m_world_hash_table.Add(t);
					}
				}
			} catch (Exception ex) {
				Console.WriteLine("load_info_xml()");
				Console.WriteLine(ex.ToString());
			}
		}

		private void write_info_xml(string file_name) {
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild((XmlNode)xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", (string)null));
			xmlDocument.AppendChild((XmlNode)xmlDocument.CreateElement("info_root"));
			foreach (GvoWorldInfo.Info info in m_world)
				info.WriteInfoXml(xmlDocument.DocumentElement);
			xmlDocument.Save(file_name);
		}

		private void load_items_xml(string file_name) {
			try {
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(file_name);
				if (xmlDocument.DocumentElement == null || xmlDocument.DocumentElement.ChildNodes.Count <= 0)
					return;
				foreach (XmlNode n in xmlDocument.DocumentElement.ChildNodes) {
					if (n.Attributes != null && n.Attributes["name"] != null) {
						GvoWorldInfo.Info info = FindInfo(n.Attributes["name"].Value);
						if (info != null)
							info.LoadItemsXml(n);
					}
				}
			} catch (Exception ex) {
				Console.WriteLine("load_items_xml()");
				Console.WriteLine(ex.ToString());
			}
		}

		private void write_items_xml(string file_name) {
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild((XmlNode)xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", (string)null));
			xmlDocument.AppendChild((XmlNode)xmlDocument.CreateElement("cityinfo_root"));
			foreach (GvoWorldInfo.Info info in m_world)
				info.WriteItemsXml(xmlDocument.DocumentElement);
			xmlDocument.Save(file_name);
		}

		private void load_domains_xml(string init_file_name, string local_domaininfo_file_name) {
			string filename = !File.Exists(local_domaininfo_file_name) ? init_file_name : local_domaininfo_file_name;
			try {
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(filename);
				if (xmlDocument.DocumentElement == null || xmlDocument.DocumentElement.ChildNodes.Count <= 0)
					return;
				foreach (XmlNode node in xmlDocument.DocumentElement.ChildNodes) {
					if (node.Attributes["name"] != null) {
						GvoWorldInfo.Info info = FindInfo(node.Attributes["name"].Value);
						if (info != null && info.CityInfo != null)
							info.CityInfo.LoadDomainXml(node);
					}
				}
			} catch (Exception ex) {
				Console.WriteLine("load_domains_xml()");
				Console.WriteLine(ex.ToString());
			}
		}

		public bool WriteDomains(string file_name) {
			try {
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.AppendChild((XmlNode)xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", (string)null));
				xmlDocument.AppendChild((XmlNode)xmlDocument.CreateElement("domaininfo_root"));
				foreach (GvoWorldInfo.Info info in m_world) {
					if (info.CityInfo != null)
						info.CityInfo.WriteDomainXml((XmlNode)xmlDocument.DocumentElement);
				}
				xmlDocument.Save(file_name);
			} catch (Exception ex) {
				Console.WriteLine("WriteDomains()");
				Console.WriteLine(ex.ToString());
				return false;
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 InfoType
		---------------------------------------------------------------------------*/
		public static string GetInfoTypeString(GvoWorldInfo.InfoType __type) {
			return EnumParserUtility<GvoWorldInfo.InfoType>.ToString(GvoWorldInfo.m_infotype_enum_param, __type, "불명");
		}

		public static GvoWorldInfo.InfoType GetInfoTypeFromString(string str) {
			return EnumParserUtility<GvoWorldInfo.InfoType>.ToEnum(GvoWorldInfo.m_infotype_enum_param, str, GvoWorldInfo.InfoType.City);
		}

		public static string GetServerStringForShare(GvoWorldInfo.Server _server) {
			switch (_server) {
				case GvoWorldInfo.Server.Zephyros:
					return "zephyros";
				case GvoWorldInfo.Server.Notos:
					return "notos";
				case GvoWorldInfo.Server.Boreas:
					return "boreas";
				case GvoWorldInfo.Server.Helene:
					return "helene";
				case GvoWorldInfo.Server.Polaris:
					return "polaris";
				case GvoWorldInfo.Server.Eirene:
					return "eirene";
				default:
					return "euros";
			}
		}

		/*-------------------------------------------------------------------------
		 서버명から Server を得る
		---------------------------------------------------------------------------*/
		public static Server GetServerFromString(string str) {
			return EnumParserUtility<GvoWorldInfo.Server>.ToEnum(str, GvoWorldInfo.Server.Unknown);
		}

		/*-------------------------------------------------------------------------
		 국명を得る
		---------------------------------------------------------------------------*/
		public static string GetCountryString(Country _country) {
			return EnumParserUtility<GvoWorldInfo.Country>.ToString(GvoWorldInfo.m_country_enum_param, _country, "무소속");
		}

		/*-------------------------------------------------------------------------
		 국명から Country を得る
		---------------------------------------------------------------------------*/
		public static GvoWorldInfo.Country GetCountryFromString(string str) {
			return EnumParserUtility<GvoWorldInfo.Country>.ToEnum(GvoWorldInfo.m_country_enum_param, str, GvoWorldInfo.Country.Unknown);
		}

		/*-------------------------------------------------------------------------
		 도시の종류を得る
		---------------------------------------------------------------------------*/
		public static string GetCityTypeString(CityType _city_type) {
			return EnumParserUtility<GvoWorldInfo.CityType>.ToString(GvoWorldInfo.m_citytype_enum_param, _city_type, "불명");
		}

		public static GvoWorldInfo.CityType GetCityTypeFromString(string str) {
			return EnumParserUtility<GvoWorldInfo.CityType>.ToEnum(GvoWorldInfo.m_citytype_enum_param, str, GvoWorldInfo.CityType.City);
		}

		/*-------------------------------------------------------------------------
		 동맹현황を得る
		---------------------------------------------------------------------------*/
		public static string GetAllianceTypeString(AllianceType _alliance) {
			return EnumParserUtility<GvoWorldInfo.AllianceType>.ToString(GvoWorldInfo.m_alliancetype_enum_param, _alliance, "동맹없음");
		}

		public static GvoWorldInfo.AllianceType GetAllianceTypeFromString(string str) {
			return EnumParserUtility<GvoWorldInfo.AllianceType>.ToEnum(GvoWorldInfo.m_alliancetype_enum_param, str, GvoWorldInfo.AllianceType.Unknown);
		}

		/*-------------------------------------------------------------------------
		 문화권を得る
		---------------------------------------------------------------------------*/
		public static string GetCulturalSphereString(CulturalSphere cs) {
			return EnumParserUtility<GvoWorldInfo.CulturalSphere>.ToString(GvoWorldInfo.m_culturalsphere_enum_param, cs, "불명");
		}

		public static GvoWorldInfo.CulturalSphere GetCulturalSphereFromString(string str) {
			return EnumParserUtility<GvoWorldInfo.CulturalSphere>.ToEnum(GvoWorldInfo.m_culturalsphere_enum_param, str, GvoWorldInfo.CulturalSphere.Unknown);
		}

		private string[] download_domains() {
			string str1 = HttpDownload.Download("http://gvtrademap.daa.jp/domain.php", Encoding.UTF8);
			if (str1 == null)
				return (string[])null;
			string[] strArray = str1.Split(new string[2]
			{
		"\r\n",
		"\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			List<string> list = new List<string>();
			foreach (string str2 in strArray) {
				if (!(str2 == "start")) {
					if (!(str2 == "end"))
						list.Add(str2);
					else
						break;
				}
			}
			return list.ToArray();
		}

		private bool load_domains_from_old_data(string[] domains) {
			if (domains == null || domains.Length != 4)
				return false;
			foreach (GvoWorldInfo.Info info in m_world) {
				if (info.CityInfo != null)
					info.CityInfo.LoadDomainFromNeworkData(domains);
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 도시, 상륙지, 2차필드, 교외 또는 해역 하나에 대한 정보
		---------------------------------------------------------------------------*/
		public class Info : hittest, IDictionaryNode<string> {
			private string[] m_group_name_tbl = new string[]{
												"[교역]", "[도구]", "[공방]", "[인물]",
												"[배]", "[조선공]", "[대포]", "[추가장갑]",
												"[돛]", "[선수상]", "[행상인]",
												"",
											};

			// 판정용 여백
			const int CHECK_MARGIN = 10;

			private string m_name;        // 이름
			private InfoType m_info_type;    // 종류(도시등)
			private int m_url_index;        // URL번호
			private string m_url;            // URLそのもの
											 // 直接リンク
			private GvoWorldInfo.Server m_server;         // プレイしている서버
			private GvoWorldInfo.CityInfo m_cityinfo;        // 도시정보
			private GvoWorldInfo.SeaInfo m_seainfo;   // 해역정보

			private string m_memo;        // 메모
			private string[] m_memo_div_lines;  // 行単位に분할された메모

			private Point m_string_offset1; // 도시명표시용 오프셋(아이콘 대)
			private Point m_string_offset2; // 도시명표시용 오프셋(아이콘 소)
			private d3d_sprite_rects.rect m_icon_rect;  // 아이콘 대 사각형
			private d3d_sprite_rects.rect m_small_icon_rect;    // 아이콘 소 사각형
			private d3d_sprite_rects.rect m_string_rect;        // 문자표시용 사각형

			private List<Group> m_groups;    // group_index분の데이터

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public string Key { get { return m_name; } }
			public string Name { get { return m_name; } }
			public InfoType InfoType { get { return m_info_type; } }
			public string InfoTypeStr { get { return GvoWorldInfo.GetInfoTypeString(m_info_type); } }
			public int UrlIndex { get { return m_url_index; } }
			public string Url { get { return m_url; } }

			public GvoWorldInfo.Server MyServer { get { return m_server; } }
			// 동맹국
			public GvoWorldInfo.Country MyCountry {
				get {
					if (m_cityinfo == null) return GvoWorldInfo.Country.Unknown;
					return m_cityinfo.GetDomain(m_server);
				}
			}
			public string CountryStr { get { return GvoWorldInfo.GetCountryString(this.MyCountry); } }
			public string Lang1 {
				get {
					if (this.m_cityinfo != null)
						return this.m_cityinfo.Lang1;
					else
						return "";
				}
			}
			public string Lang2 {
				get {
					if (this.m_cityinfo != null)
						return this.m_cityinfo.Lang2;
					else
						return "";
				}
			}
			public int Sakaba {
				get {
					if (this.m_cityinfo != null)
						return this.m_cityinfo.Sakaba;
					else
						return 0;
				}
			}

			public GvoWorldInfo.CityType CityType {
				get {
					if (m_cityinfo == null) return GvoWorldInfo.CityType.City;
					return m_cityinfo.CityType;
				}
			}
			public string CityTypeStr { get { return GvoWorldInfo.GetCityTypeString(CityType); } }
			public GvoWorldInfo.AllianceType AllianceType {
				get {
					if (m_cityinfo == null) return GvoWorldInfo.AllianceType.Unknown;
					return m_cityinfo.AllianceType;
				}
			}
			public string AllianceTypeStr { get { return GvoWorldInfo.GetAllianceTypeString(this.AllianceType); } }
			public GvoWorldInfo.CulturalSphere CulturalSphere {
				get {
					if (m_cityinfo == null) return GvoWorldInfo.CulturalSphere.Unknown;
					return m_cityinfo.CulturalSphere;
				}
			}
			public string CulturalSphereStr { get { return GvoWorldInfo.GetCulturalSphereString(this.CulturalSphere); } }

			public GvoWorldInfo.SeaInfo SeaInfo {
				get { return m_seainfo; }
				internal set { m_seainfo = value; }
			}
			public GvoWorldInfo.CityInfo CityInfo {
				get { return m_cityinfo; }
				internal set { m_cityinfo = value; }
			}

			// 메모전체
			// 行単位は전용の関수を使うこと
			public string Memo {
				get { return m_memo; }
				set {
					m_memo = value;
					div_memo_lines();
				}
			}
			public bool IsUrl {
				get {
					if (UrlIndex >= 0) return true;
					if (Url != "") return true;
					return false;
				}
			}

			public Point StringOffset1 { get { return m_string_offset1; } }
			public Point StringOffset2 { get { return m_string_offset2; } }
			public d3d_sprite_rects.rect IconRect { get { return m_icon_rect; } }
			public d3d_sprite_rects.rect SmallIconRect { get { return m_small_icon_rect; } }
			public d3d_sprite_rects.rect NameRect { get { return m_string_rect; } set { m_string_rect = value; } }

			// ツールチップ
			public string TooltipString { get { return __get_tool_tip_string(); } }

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			private Info() {
				int i = 0;
				m_groups = new List<Group>();
				foreach (string s in m_group_name_tbl) {
					m_groups.Add(new Group(s, this, i++));
				}

				m_memo = "";
				div_memo_lines();

				// 判定용矩形を지정しておく
				base.rect = new Rectangle(-CHECK_MARGIN,
													-CHECK_MARGIN,
													CHECK_MARGIN * 2,
													CHECK_MARGIN * 2);

				m_server = GvoWorldInfo.Server.Euros;   // 初期値はユーロス
				m_cityinfo = null;
				m_seainfo = null;

				m_icon_rect = null;
				m_small_icon_rect = null;
				m_string_rect = null;
			}

			/*-------------------------------------------------------------------------
			 지정된아이템があるかを得る
			 완전일치のみで조사
			---------------------------------------------------------------------------*/
			public Group.Data HasItem(string find_item) {
				foreach (Group g in m_groups) {
					Group.Data d = g.HasItem(find_item);
					if (d != null) return d;
				}
				return null;
			}

			/*-------------------------------------------------------------------------
			 메모の읽기
			---------------------------------------------------------------------------*/
			public void LoadMemo(string path) {
				// 파일명작성
				path += m_name + "_memo.txt";

				if (!File.Exists(path)) return;  // 파일을 찾을 수 없습니다

				try {
					using (StreamReader sr = new StreamReader(
						path, Encoding.GetEncoding("UTF-8"))) {

						// 全부로드
						m_memo = sr.ReadToEnd();
					}
					div_memo_lines();
				} catch {
					// 읽기실패
					m_memo = "";
				}
			}

			/*-------------------------------------------------------------------------
			 메모の書き出し
			---------------------------------------------------------------------------*/
			public void WriteMemo(string path) {
				// 파일명작성
				path += m_name + "_memo.txt";

				// 메모정보がなければ書き出さない
				if (m_memo == "") {
					// 파일があれば삭제する
					file_ctrl.RemoveFile(path);
					return;
				}

				try {
					using (StreamWriter sw = new StreamWriter(
						path, false, Encoding.GetEncoding("UTF-8"))) {
						sw.Write(Memo);
					}
				} catch {
					// 書き出し실패
				}
			}

			/*-------------------------------------------------------------------------
			 教えてくれる인を조사
			 教えてくれる인が居なければnullを返す
			---------------------------------------------------------------------------*/
			public string LearnPerson(string find_str) {
				Group.Data d = m_groups[(int)GroupIndex._3].HasItem(find_str);
				if (d == null) return null;
				return d.Price;
			}

			/*-------------------------------------------------------------------------
			 수を得る
			---------------------------------------------------------------------------*/
			public int GetCount(GroupIndex index) {
				if (index < 0) return 0;
				if (index >= GroupIndex.max) return 0;
				return m_groups[(int)index].GetCount();
			}

			/*-------------------------------------------------------------------------
			 그룹を得る
			---------------------------------------------------------------------------*/
			public Group GetGroup(GroupIndex index) {
				if (index < 0) return null;
				if (index >= GroupIndex.max) return null;
				return m_groups[(int)index];
			}

			/*-------------------------------------------------------------------------
			 그룹명から데이터を得る
			---------------------------------------------------------------------------*/
			private Info.Group get_group(string name) {
				foreach (GvoWorldInfo.Info.Group group in m_groups) {
					if (group.Name == name) {
						return group;
					}
				}
				return null;
			}

			/*-------------------------------------------------------------------------
			 데이터を得る
			---------------------------------------------------------------------------*/
			public Group.Data GetData(GroupIndex index, int data_index) {
				Group g = GetGroup(index);
				if (g == null) return null;
				return g.GetData(data_index);
			}

			/*-------------------------------------------------------------------------
			 참조용に데이터업데이트
			---------------------------------------------------------------------------*/
			public void UpdateDomains(GvoItemTypeDatabase type, GvoWorldInfo.Server server, GvoWorldInfo.Country my_country) {
				m_server = server;  // プレイしている서버の업데이트
				foreach (Group g in m_groups) {
					g.UpdateDomains(type, this.MyCountry != my_country);
				}
			}

			/*-------------------------------------------------------------------------
			 그리기용의 사각형을 갱신
			---------------------------------------------------------------------------*/
			public void UpdateDrawRects(gvt_lib lib, String name) {
				if (m_info_type == InfoType.Sea) return;

				if (m_info_type == InfoType.City) {
					switch (this.CityType) {
						case GvoWorldInfo.CityType.Capital:
							m_icon_rect = lib.icons.GetIcon(icons.icon_index.map_capital);
							break;
						case GvoWorldInfo.CityType.City:
							m_icon_rect = lib.icons.GetIcon((this.AllianceType == GvoWorldInfo.AllianceType.Territory) ? icons.icon_index.map_city_territory : icons.icon_index.map_city);
							break;
						case GvoWorldInfo.CityType.CapitalIslam:
							m_icon_rect = lib.icons.GetIcon(icons.icon_index.map_capital_islam);
							break;
						case GvoWorldInfo.CityType.CityIslam:
							m_icon_rect = lib.icons.GetIcon((this.AllianceType == GvoWorldInfo.AllianceType.Territory) ? icons.icon_index.map_city_islam_territory : icons.icon_index.map_city_islam);
							break;
					}
					m_small_icon_rect = lib.icons.GetIcon(icons.icon_index.map_red_rect);
					//m_string_rect = lib.nameTexture.getRect(name);
				} else {
					switch (InfoType) {
						case InfoType.Shore:
							m_icon_rect = lib.icons.GetIcon(icons.icon_index.map_cyan_circle);
							break;
						case InfoType.Shore2:
							m_icon_rect = lib.icons.GetIcon(icons.icon_index.map_blue_circle);
							break;
						case InfoType.OutsideCity:
							m_icon_rect = lib.icons.GetIcon(icons.icon_index.map_orange_circle);
							break;
						case InfoType.PF:
							m_icon_rect = lib.icons.GetIcon(icons.icon_index.map_yellow_circle);
							break;
						case InfoType.InlandCity:
							m_icon_rect = lib.icons.GetIcon(icons.icon_index.map_magenta_circle);
							break;
						case InfoType.GuildCity:
							m_icon_rect = lib.icons.GetIcon(icons.icon_index.map_red_circle);
							break;
					}
					m_small_icon_rect = m_icon_rect;        // 同じ아이콘を사용する
					//m_string_rect = lib.nameTexture.getRect(name);
				}
			}

			/*-------------------------------------------------------------------------
			 아이템の그룹명を得る
			---------------------------------------------------------------------------*/
			static public string GetGroupName(GroupIndex index) {
				switch (index) {
					case Info.GroupIndex._0: return "교역소주인";
					case Info.GroupIndex._1: return "도구점주인";
					case Info.GroupIndex._2: return "대장장이";
					case Info.GroupIndex._3: return "인물";
					case Info.GroupIndex._4: return "조선소주인";
					case Info.GroupIndex._4_1: return "조선공";
					case Info.GroupIndex._5: return "무기장인";
					case Info.GroupIndex._6: return "제재소장인";
					case Info.GroupIndex._7: return "돛제작자";
					case Info.GroupIndex._8: return "조각가";
					case Info.GroupIndex._9: return "행상인";
					case Info.GroupIndex.max: return "메모";
				}
				return "기타";
			}

			/*-------------------------------------------------------------------------
			 메모の行수を得る
			---------------------------------------------------------------------------*/
			public int GetMemoLines() {
				if (m_memo_div_lines == null) return 0;
				return m_memo_div_lines.Length;
			}

			/*-------------------------------------------------------------------------
			 메모を得る
			 行지정
			---------------------------------------------------------------------------*/
			public string GetMemo(int line_index) {
				if (line_index < 0) return "";
				if (line_index >= GetMemoLines()) return "";
				return m_memo_div_lines[line_index];
			}

			/*-------------------------------------------------------------------------
			 메모を行毎に분할する
			---------------------------------------------------------------------------*/
			private void div_memo_lines() {
				if (m_memo == "") {
					m_memo_div_lines = null;
				} else {
					m_memo_div_lines = m_memo.Split(new char[] { '\n' });
				}
			}

			/*-------------------------------------------------------------------------
			 아이템DBとリンクさせる
			---------------------------------------------------------------------------*/
			internal void LinkItemDatabase(ItemDatabaseCustom item_db) {
				foreach (Group g in m_groups) {
					g.LinkItemDatabase(item_db, Name);
				}
			}

			/*-------------------------------------------------------------------------
			 HPを開くtooltipを得る
			---------------------------------------------------------------------------*/
			public string GetToolTipString_HP() {
				if (!IsUrl) return null;        // URLが등록されていない

				if (UrlIndex != -1) {
					if (InfoType == GvoWorldInfo.InfoType.City) {
						// 대투자전
						return "대투자전\n클릭으로 브라우저 열기";
					} else {
						// DKKmap
						return "D.K.K맵\n클릭으로 브라우저 열기";
					}
				} else {
					// クリスタル商会
					//					return "直接リンク\n좌클릭으로 ブラウザを開く";
					return "대항해시대Online 상륙지 지도\n클릭으로 브라우저 열기";
				}
			}

			/*-------------------------------------------------------------------------
			 전체검색
			---------------------------------------------------------------------------*/
			public void FindAll(string find_string, List<GvoDatabase.Find> list, GvoDatabase.Find.FindHandler handler) {
				// 이름
				if (handler(Name, find_string)) {
					list.Add(new GvoDatabase.Find(Name));
				}

				// 언어
				if (handler(Lang1, find_string)) {
					list.Add(new GvoDatabase.Find(Name, Lang1));
				}
				if (handler(Lang2, find_string)) {
					list.Add(new GvoDatabase.Find(Name, Lang2));
				}

				// 아이템から검색
				foreach (Group g in m_groups) {
					g.FindAll(find_string, list, Name, handler);
				}
			}

			/*-------------------------------------------------------------------------
			 전체검색
			 종류별로の검색용
			---------------------------------------------------------------------------*/
			public void FindAll_FromType(string find_string, List<GvoDatabase.Find> list, GvoDatabase.Find.FindHandler handler) {
				// 아이템から검색
				foreach (Group g in m_groups) {
					g.FindAll_FromType(find_string, list, Name, handler);
				}
			}

			/*-------------------------------------------------------------------------
			 ツールチップ용문자열の取得
			---------------------------------------------------------------------------*/
			private string __get_tool_tip_string() {
				string tip = Name + "\n";
				if (SeaInfo != null) {
					// 해역
					return tip + SeaInfo.TooltipString;
				} else {
					// その他
					switch (InfoType) {
						case GvoWorldInfo.InfoType.City:
							switch (AllianceType) {
								case GvoWorldInfo.AllianceType.Unknown:
								case GvoWorldInfo.AllianceType.Piratical:
									tip += AllianceTypeStr;
									break;
								case GvoWorldInfo.AllianceType.Alliance:
									tip += AllianceTypeStr + " " + CountryStr;
									break;
								case GvoWorldInfo.AllianceType.Capital:
								case GvoWorldInfo.AllianceType.Territory:
									tip += CountryStr + " " + AllianceTypeStr;
									break;
							}
							tip += "\n종류:" + CityTypeStr;
							tip += "\n문화권:" + CulturalSphereStr;

							// 사용언어
							if (Lang1 != "") tip += "\n사용언어:" + Lang1;
							if (Lang2 != "") tip += "\n사용언어:" + Lang2;
							break;
						case GvoWorldInfo.InfoType.Sea:
						case GvoWorldInfo.InfoType.Shore:
						case GvoWorldInfo.InfoType.Shore2:
						case GvoWorldInfo.InfoType.OutsideCity:
						case GvoWorldInfo.InfoType.PF:
							tip += InfoTypeStr;
							break;
					}
					return tip;
				}
			}


			internal static GvoWorldInfo.Info FromXml(XmlNode n) {
				if (n == null)
					return (GvoWorldInfo.Info)null;
				if (n.ChildNodes == null)
					return (GvoWorldInfo.Info)null;
				if (n.Name != "info")
					return (GvoWorldInfo.Info)null;
				if (n.Attributes["name"] == null)
					return (GvoWorldInfo.Info)null;
				GvoWorldInfo.Info info = new GvoWorldInfo.Info();
				info.m_name = Useful.XmlGetAttribute(n, "name", info.m_name);
				//info.position = Useful.XmlGetPoint(n, "position", info.position);
				// 측량 데이터는 현재 맵크기의 4배에 해당하므로, 위치를 1/4로 줄여준다.
				Point xmlPosition = Useful.XmlGetPoint(n, "position", info.position);
				xmlPosition.X = (int)(xmlPosition.X / 4);
				xmlPosition.Y = (int)(xmlPosition.Y / 4);
				info.position = xmlPosition;

				String angleStr = Useful.XmlGetAttribute(n, "angle", "0");
				// 360 도 단위 이므로 (반대방향이므로 360 - 각도를 해준 값을) 360으로 나눈 나머지에 PI / 180을 곱해준다. 
				int angle = (360 - Int32.Parse(angleStr)) % 360;
				float rad_angle = (float)(angle * Math.PI / 180.0f);
				info.angle = rad_angle;

				info.m_url_index = Useful.ToInt32(Useful.XmlGetAttribute(n, "url_index", ""), -1);
				info.m_url = "";
				if (info.m_url_index == -1)
					info.m_url = Useful.XmlGetAttribute(n, "url_string", info.m_url);
				info.m_info_type = GvoWorldInfo.GetInfoTypeFromString(Useful.XmlGetAttribute(n, "info_type", ((object)info.m_info_type).ToString()));
				info.m_string_offset1 = Useful.XmlGetPoint(n, "name_offset1", info.m_string_offset1);
				info.m_string_offset2 = Useful.XmlGetPoint(n, "name_offset2", info.m_string_offset2);
				foreach (XmlNode node in n.ChildNodes) {
					GvoWorldInfo.SeaInfo seaInfo = GvoWorldInfo.SeaInfo.FromXml(node, info.m_name);
					if (seaInfo != null)
						info.m_seainfo = seaInfo;
					GvoWorldInfo.CityInfo cityInfo = GvoWorldInfo.CityInfo.FromXml(node, info.m_name);
					if (cityInfo != null)
						info.m_cityinfo = cityInfo;
				}
				return info;
			}

			internal void WriteInfoXml(XmlElement p_node) {
				XmlNode node = Useful.XmlAddNode((XmlNode)p_node, "info", Name);
				write_info_sub(node);
				if (m_cityinfo != null)
					m_cityinfo.WriteXml(node);
				if (m_seainfo == null)
					return;
				m_seainfo.WriteXml(node);
			}

			internal void LoadItemsXml(XmlNode n) {
				if (n.ChildNodes == null || n.ChildNodes.Count <= 0)
					return;
				foreach (XmlNode n1 in n.ChildNodes) {
					if (n1.Attributes["name"] != null) {
						Group group = get_group(n1.Attributes["name"].Value);
						if (group != null)
							group.LoadXml(n1);
					}
				}
			}

			private void write_info_sub(XmlNode node) {
				Useful.XmlAddPoint(node, "position", position);
				if (m_url_index == -1)
					Useful.XmlAddAttribute(node, "url_string", m_url);
				else
					Useful.XmlAddAttribute(node, "url_index", m_url_index.ToString());
				Useful.XmlAddAttribute(node, "info_type", ((object)m_info_type).ToString());
				Useful.XmlAddPoint(node, "name_offset1", m_string_offset1);
				Useful.XmlAddPoint(node, "name_offset2", m_string_offset2);
			}

			internal void WriteItemsXml(XmlElement xmlElement) {
				XmlNode xmlNode = Useful.XmlAddNode((XmlNode)xmlElement, "cityinfo", Name);
				foreach (Group group in m_groups)
					group.WriteInfoXml(xmlNode);
				if (xmlNode.ChildNodes.Count > 0)
					return;
				xmlElement.RemoveChild(xmlNode);
			}

			public enum GroupIndex {
				_0,  // 교역
				_1,  // 도구
				_2,  // 공방
				_3,  // 인물
				_4,  // 조선소주인
				_4_1,   // 조선공
				_5,  // 대포
				_6,  // 추가장갑
				_7,  // 돛
				_8,  // 선수상
				_9,  // 행상인
				max
			};

			/*-------------------------------------------------------------------------
			 데이터그룹
			---------------------------------------------------------------------------*/
			public class Group {
				/*-------------------------------------------------------------------------
				 
				---------------------------------------------------------------------------*/
				private string m_name;        // 이름
				private List<Data> m_datas;   // 데이터
				private GvoWorldInfo.Info m_info;
				private int m_index;

				/*-------------------------------------------------------------------------
				 
				---------------------------------------------------------------------------*/
				public string Name { get { return m_name; } }
				public GvoWorldInfo.Info Info { get { return m_info; } }
				public int Index { get { return m_index; } }

				/*-------------------------------------------------------------------------
				 
				---------------------------------------------------------------------------*/
				public Group(string name, GvoWorldInfo.Info info, int index) {
					m_name = name;
					m_info = info;
					m_index = index;
					m_datas = new List<Data>();
				}

				/*-------------------------------------------------------------------------
				 지정된아이템があるかを得る
				 완전일치のみで조사
				---------------------------------------------------------------------------*/
				public Data HasItem(string find_item) {
					foreach (Data d in m_datas) {
						if (d.Name == find_item) return d;
					}
					return null;
				}

				/*-------------------------------------------------------------------------
				 무역상인と번역가플래그を得る
				 ついでに행상인플래그を설정する
				---------------------------------------------------------------------------*/
				public int GetSakabaFlag() {
					int flag = 0;
					foreach (Data d in m_datas) {
						if (d.Tag == "@") {  // 번역가
							flag |= 4;
						} else if (d.Tag == "#") {   // 무역상인
							flag |= 8;
						} else if (d.Tag == "") {
							// 행상인扱い
							d.Tag = "-";
						}
					}
					return flag;
				}

				/*-------------------------------------------------------------------------
				 수を得る
				---------------------------------------------------------------------------*/
				public int GetCount() {
					return m_datas.Count;
				}

				/*-------------------------------------------------------------------------
				 데이터を得る
				---------------------------------------------------------------------------*/
				public Data GetData(int index) {
					if (index < 0) return null;
					if (index >= GetCount()) return null;
					return m_datas[index];
				}

				/*-------------------------------------------------------------------------
				 데이터を업데이트する
				---------------------------------------------------------------------------*/
				internal void UpdateDomains(GvoItemTypeDatabase item_type_db, bool is_tax) {
					foreach (Data d in m_datas) {
						d.UpdateDomains(item_type_db, is_tax);
					}
				}

				/*-------------------------------------------------------------------------
				 아이템DBとリンクさせる
				---------------------------------------------------------------------------*/
				internal void LinkItemDatabase(ItemDatabaseCustom item_db, string name) {
					foreach (Data d in m_datas) {
						d.LinkItemDatabase(item_db, name);
					}
				}

				/*-------------------------------------------------------------------------
				 전체검색
				---------------------------------------------------------------------------*/
				public void FindAll(string find_string, List<GvoDatabase.Find> list, string info_name, GvoDatabase.Find.FindHandler handler) {
					foreach (Data d in m_datas) {
						if (handler(d.Name, find_string)) {
							list.Add(new GvoDatabase.Find(GvoDatabase.Find.FindType.Data, info_name, d));
						}
						if (handler(d.Price, find_string)) {
							list.Add(new GvoDatabase.Find(GvoDatabase.Find.FindType.DataPrice, info_name, d));
						}
					}
				}

				/*-------------------------------------------------------------------------
				 전체검색
				 종류별로の검색용
				---------------------------------------------------------------------------*/
				public void FindAll_FromType(string find_string, List<GvoDatabase.Find> list, string info_name, GvoDatabase.Find.FindHandler handler) {
					foreach (Data d in m_datas) {
						if (handler(d.Type, find_string)) {
							list.Add(new GvoDatabase.Find(GvoDatabase.Find.FindType.Data, info_name, d));
						}
					}
				}

				internal void LoadXml(XmlNode n) {
					m_datas.Clear();
					if (n.ChildNodes == null || n.ChildNodes.Count <= 0)
						return;
					foreach (XmlNode nn in n.ChildNodes) {
						Data data = Data.FromXml(nn, m_info, m_index);
						if (data != null)
							m_datas.Add(data);
					}
				}

				internal void WriteInfoXml(XmlNode node) {
					if (m_datas.Count <= 0)
						return;
					XmlNode node1 = Useful.XmlAddNode(node, "group", Name);
					if (node1 == null)
						return;
					foreach (Data data in m_datas)
						data.WriteInfoXml(node1);
				}

				/*-------------------------------------------------------------------------
				 데이터
				---------------------------------------------------------------------------*/
				public class Data {
					private Info m_info;                    // 属している도시など
					private string m_name;            // 이름
					private string m_tag;                // タグ
					private string m_tag2;            // タグ2
					private int m_group_index;    // 属している그룹번호

					// 외からの참조용
					private bool m_is_bonus_item;    // 명産품
					private string m_price;    // 가격, 인물명など
					private Color m_color;        // 표시색
					private Color m_price_color;            // 가격용색
					private string m_investment;            // 必要な가격額
					private ItemDatabaseCustom.Data m_item_db;        // 아이템DBの디테일

					/*-------------------------------------------------------------------------
					 
					---------------------------------------------------------------------------*/
					public string Name { get { return m_name; } }
					public Info Info { get { return m_info; } }
					public bool IsBonusItem { get { return m_is_bonus_item; } }
					public string Price { get { return m_price; } }
					public string GroupIndexString { get { return GetGroupName(GroupIndex._0 + m_group_index); } }
					public Color Color { get { return m_color; } }
					public Color PriceColor { get { return m_price_color; } }
					public string Investment { get { return m_investment; } }
					public ItemDatabaseCustom.Data ItemDb { get { return m_item_db; } }
					public bool HasTooltip { get { return (ItemDb == null) ? false : true; } }
					public string TooltipString {
						get {
							if (ItemDb == null) {
								string str;
								str = "명칭:" + Name;
								str += "\n아이템DB에서 내용을 찾을 수 없습니다. \n";
								str += "명칭이 미묘하게 잘못되었거나 미지의 데이터입니다. \n";
								return str;
							} else {
								return getMixedToolTipString();
							}
						}
					}
					public string Type {
						get {
							if (ItemDb == null) return "불명";
							return ItemDb.Type;
						}
					}
					public ItemDatabaseCustom.Categoly Categoly {
						get {
							if (ItemDb == null) return ItemDatabaseCustom.Categoly.Unknown;
							return ItemDb.Categoly;
						}
					}
					public Color CategolyColor {
						get {
							if (ItemDb == null) return Color.Black;
							return ItemDb.CategolyColor;
						}
					}

					internal string Tag {
						get { return m_tag; }
						set { m_tag = value; }
					}
					internal string Tag2 { get { return m_tag2; } }

					/*-------------------------------------------------------------------------
					 
					---------------------------------------------------------------------------*/
					private Data(Info info, int index) {
						m_name = "unknown";
						m_tag = "";
						m_tag2 = "0";
						m_investment = "";

						m_info = info;
						m_group_index = index;

						// 외からの참조용
						m_is_bonus_item = false;
						m_price = "";
						m_color = Color.Black;
						m_price_color = Color.Black;
						m_item_db = null;
					}

					/*-------------------------------------------------------------------------
					 그리기용の색を得る
					---------------------------------------------------------------------------*/
					private Color _get_color() {
						switch (m_tag) {
							//스킬
							case "s": return Color.Black;
							//보고
							case "h": return Color.DarkRed;
							//레시피
							//						case "%":		return Color.DarkGreen;

							// 가격후목록入り
							case "*": return Color.Gray;
							// 가격で得られる
							case "$": return Color.Blue;
							// 가격비교
							//레시피
							case "%": return Color.DarkGreen;
							// 번역가
							case "@": return Color.DarkRed;
							// 판매원
							case "+": return Color.MediumPurple;
							// 무역상인
							case "#": return Color.Green;
							// 행상인
							case "-": return Color.Gray;
							// その他
							default: return Color.Black;
						}
					}

					/*-------------------------------------------------------------------------
					 그리기용の가격を得る
					 종류によっては가격ではないものが返る
					---------------------------------------------------------------------------*/
					private string _get_price() {
						switch (m_tag) {
							// 가격で得られる
							case "$": return "가격";
							// 가격비교
							case "%": return "가격비교";
							// 번역가
							case "@": return "번역가";
							// 판매원
							case "+": return "판매원";
							// 무역상인
							case "#": return "무역상인";
							// 행상인
							case "-": return "행상인";

							// その他
							default:
								return calc_price(false);
						}
					}

					/*-------------------------------------------------------------------------
					 가격を得る
					 세율を考慮する
					---------------------------------------------------------------------------*/
					private string calc_price(bool is_tax) {
						int p;
						if (!Int32.TryParse(m_tag2, out p)) p = 0;
						if (is_tax) p = GetTaxPrice(p);
						return String.Format("{0:#,0}", p);
					}

					/*-------------------------------------------------------------------------
					 업데이트
					---------------------------------------------------------------------------*/
					internal void UpdateDomains(GvoItemTypeDatabase item_type_db, bool is_tax) {
						// 색
						m_color = _get_color();
						m_price_color = m_color;

						// 교역품전용の정보
						if (m_group_index == 0) {
							// 교역품
							// 명産품かどうかを조사
							m_is_bonus_item = item_type_db.IsBonusItem(Name);

							// 가격
							if (m_info.InfoType == InfoType.City) {
								if (item_type_db.IsNanbanTradeItem(Name)) m_price = "남만";
								else m_price = calc_price(is_tax);
							} else {
								update_rank(item_type_db);
							}
						} else {
							// その他
							m_is_bonus_item = false;

							// 가격
							if (m_group_index != 3) m_price = _get_price();
							else m_price = Tag2;                // 인물
						}
					}

					/*-------------------------------------------------------------------------
					 조달などのランクを업데이트する
					---------------------------------------------------------------------------*/
					private void update_rank(GvoItemTypeDatabase type) {
						int index = Useful.ToInt32(m_tag2, 0);

						switch (index) {
							case -1:
								m_price = "채집R" + rank_to_str(type.GetSaisyuRank(Name));
								m_price_color = Color.DarkCyan;
								break;
							case -2:
								m_price = "조달R" + rank_to_str(type.GetChotatuRank(Name));
								m_price_color = Color.Olive;
								break;
							case -3:
								// -3は予約されているが, 데이터には含まれない
								m_price = "탐색";
								m_price_color = Color.DarkViolet;
								break;
							case 0:
							default:
								m_price = "낚시R" + rank_to_str(type.GetFishingRank(Name));
								m_price_color = Color.Gray;
								break;
						}
					}

					/*-------------------------------------------------------------------------
					 ランクを문자열に변환する
					 R1～R19以외は??に변환される
					---------------------------------------------------------------------------*/
					private string rank_to_str(int rank) {
						if (rank < 1) return "??";
						if (rank >= 20) return "??";
						return rank.ToString();
					}

					/*-------------------------------------------------------------------------
					 아이템DBとリンクさせる
					---------------------------------------------------------------------------*/
					internal void LinkItemDatabase(ItemDatabaseCustom db, string info_name) {
						if (String.IsNullOrEmpty(Name)) return;

						ItemDatabaseCustom.Data d = db.Find(Name);
#if DEBUG
						//						if(d == null){
						//							Debug.WriteLine(String.Format("{0} {1}", info_name, name));
						//						}
#endif
						m_item_db = d;
					}

					/*-------------------------------------------------------------------------
					 아이템명변경に대응する
					---------------------------------------------------------------------------*/
					static private void update_rename(Data d) {
						switch (d.m_name) {
							case "仕立て도구": {
									d.m_name = "裁縫도구";
									d.m_tag2 = "10000";
								}
								break;
							case "회避指남書제 1권":
								d.m_name = "連撃指남書제 1권";
								break;
							case "공격指남書제 1권":
								d.m_name = "猛攻指남書제 1권";
								break;
							case "회復指남書제 1권":
								d.m_name = "活용指남書제 1권";
								break;
							case "방어指남書제 1권":
								d.m_name = "奇手指남書제 1권";
								break;
							case "パデットロール":
								d.m_name = "パデッドロール";
								break;
							case "診察실の製법":
								d.m_name = "조선소재・診察실";
								break;
							case "天子の선수상":
								d.m_name = "天使の선수상";
								break;
							case "해역":
								d.m_name = "지리";
								break;
							case "高급상納품の梱包":
								d.m_name = "高급상納품の梱包(NO.1)";
								break;
						}
					}

					/*-------------------------------------------------------------------------
					 디테일とミックスしたツールチップ용の문자열を得る
					---------------------------------------------------------------------------*/
					private string getMixedToolTipString() {
						if (ItemDb == null) return "";

						string str = "명칭:" + this.ItemDb.Name + "\n";
						if (Categoly != ItemDatabaseCustom.Categoly.Unknown) {
							str += "종류:" + Type + "(カテゴリ" + ((int)Categoly + 1).ToString() + ")\n";
						} else {
							str += "종류:" + Type + "\n";
						}
						if (this.Investment != "") {
							str += "가격:" + this.Investment + "\n";
						}
						str += "설명:\n" + ItemDb.Document;
						return str;
					}
					internal static Group.Data FromXml(XmlNode nn, Info info, int index) {
						Group.Data data = new Group.Data(info, index);
						data.m_name = Useful.XmlGetAttribute(nn, "name", data.m_name);
						data.m_tag = Useful.XmlGetAttribute(nn, "option", data.m_tag);
						data.m_tag2 = Useful.XmlGetAttribute(nn, "price", data.m_tag2);
						data.m_investment = Useful.XmlGetAttribute(nn, "investment", data.m_investment);
						return data;
					}

					internal void WriteInfoXml(XmlNode node) {
						XmlNode node1 = Useful.XmlAddNode(node, "item", Name);
						Useful.XmlAddAttribute(node1, "option", m_tag);
						if (m_tag2 != "0")
							Useful.XmlAddAttribute(node1, "price", m_tag2);
						Useful.XmlAddAttribute(node1, "investment", m_investment);
					}
				}
			}
		}

		/*-------------------------------------------------------------------------
		 해역정보
		 主に풍향
		---------------------------------------------------------------------------*/
		public class SeaInfo {
			private string m_name;
			private Point m_wind_pos;        // 풍향を그리기する위치の중心
			private float m_summer_angle;    // 夏の풍향
			private float m_winter_angle;    // 冬の풍향
			private int m_speedup_rate;  // 최대속도상昇
			private int m_summer_angle_deg; // 夏の풍향
			private int m_winter_angle_deg; // 夏の풍향
			private string m_summer_angle_string;   // 16段階の풍향
			private string m_winter_angle_string;   // 16段階の풍향

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public string Key { get { return m_name; } }
			public string Name { get { return m_name; } }
			public Point WindPos { get { return m_wind_pos; } }
			public float SummerAngle { get { return m_summer_angle; } }
			public string SummerAngleString { get { return "여름:" + m_summer_angle_string; } }
			public float WinterAngle { get { return m_winter_angle; } }
			public string WinterAngleString { get { return "겨울:" + m_winter_angle_string; } }
			public int SpeedUpRate { get { return m_speedup_rate; } }
			public string SpeedUpRateString { get { return "속도상승:" + ((SpeedUpRate == 0) ? "미조사" : SpeedUpRate.ToString() + "%"); } }
			public string TooltipString {
				get {
					string tip = "";
					tip += SummerAngleString + "\n";
					tip += WinterAngleString + "\n";
					tip += SpeedUpRateString;
					return tip;
				}
			}

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			private SeaInfo(string name) {
				m_name = name;
				m_wind_pos = Point.Empty;
				m_speedup_rate = 0;
				m_summer_angle_deg = 0;
				m_winter_angle_deg = 0;
				update_angle();
			}

			private void update_angle() {
				m_summer_angle = Useful.ToRadian(m_summer_angle_deg);
				m_winter_angle = Useful.ToRadian(m_winter_angle_deg);
				m_summer_angle_string = angle_to_string(m_summer_angle_deg);
				m_winter_angle_string = angle_to_string(m_winter_angle_deg);
			}

			/*-------------------------------------------------------------------------
			 각도から문자열に변환する
			 与える각도が逆のため, 
			   0도=남からの풍
			   90도=서からの풍
			   180도=북からの풍
			   270도=동からの풍
			---------------------------------------------------------------------------*/
			private string angle_to_string(int angle) {
				float tmp = (float)angle + ((360f / 16f) / 2);
				if (tmp < 0) tmp = 0;
				angle = (int)(tmp / (360f / 16f));
				switch (angle) {
					case 0: return "남풍";
					case 1: return "남남서풍";
					case 2: return "남서풍";
					case 3: return "서남서풍";
					case 4: return "서풍";
					case 5: return "서북서풍";
					case 6: return "북서풍";
					case 7: return "북북서풍";
					case 8: return "북풍";
					case 9: return "북북동풍";
					case 10: return "북동풍";
					case 11: return "동북동풍";
					case 12: return "동풍";
					case 13: return "동남동풍";
					case 14: return "남동풍";
					default: return "남남동풍";
				}
			}

			internal static SeaInfo FromXml(XmlNode node, string name) {
				if (node == null)
					return (SeaInfo)null;
				if (node.Name != "sea_detail")
					return (SeaInfo)null;
				if (node.ChildNodes == null)
					return (SeaInfo)null;
				SeaInfo seaInfo = new SeaInfo(name);
				seaInfo.m_speedup_rate = Useful.ToInt32(Useful.XmlGetAttribute(node, "speedup_rate", ""), seaInfo.m_speedup_rate);
				seaInfo.m_summer_angle_deg = Useful.ToInt32(Useful.XmlGetAttribute(node, "summer_angle_deg", ""), seaInfo.m_summer_angle_deg);
				seaInfo.m_winter_angle_deg = Useful.ToInt32(Useful.XmlGetAttribute(node, "winter_angle_deg", ""), seaInfo.m_winter_angle_deg);
				//seaInfo.m_wind_pos = Useful.XmlGetPoint(node, "name_position", seaInfo.m_wind_pos);
				// 측량 데이터는 현재 맵크기의 4배에 해당하므로, 위치를 1/4로 줄여준다.
				Point xmlPosition = Useful.XmlGetPoint(node, "name_position", seaInfo.m_wind_pos);
				xmlPosition.X = (int)(xmlPosition.X / 4);
				xmlPosition.Y = (int)(xmlPosition.Y / 4);
				seaInfo.m_wind_pos = xmlPosition;

				seaInfo.update_angle();
				return seaInfo;
			}

			public void WriteXml(XmlNode node) {
				XmlNode xmlNode = Useful.XmlAddNode(node, "sea_detail");
				Useful.XmlAddAttribute(xmlNode, "speedup_rate", m_speedup_rate.ToString());
				Useful.XmlAddAttribute(xmlNode, "summer_angle_deg", m_summer_angle_deg.ToString());
				Useful.XmlAddAttribute(xmlNode, "winter_angle_deg", m_winter_angle_deg.ToString());
				Useful.XmlAddPoint(xmlNode, "name_position", m_wind_pos);
			}
		}

		/*-------------------------------------------------------------------------
		 도시정보
		---------------------------------------------------------------------------*/
		public class CityInfo {
			private string m_name;        // 이름
			private GvoWorldInfo.Country[] m_domains;        // 동맹현황
			private int m_index;            // 도시번호
			private GvoWorldInfo.CityType m_city_type;  // 도시の종류
			private GvoWorldInfo.AllianceType m_alliance_type;  // 동맹の종류
			private GvoWorldInfo.CulturalSphere m_cultural_sphere;  // 문화권
			private bool m_has_name_image;  // 이름の絵정보を持つときtrue
			private string m_lang1;
			private string m_lang2;
			private int m_sakaba_flag;

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public string Key { get { return m_name; } }
			public string Name { get { return m_name; } }
			public int Index { get { return m_index; } }
			public GvoWorldInfo.CityType CityType { get { return m_city_type; } }
			public GvoWorldInfo.AllianceType AllianceType { get { return m_alliance_type; } }
			public GvoWorldInfo.CulturalSphere CulturalSphere { get { return m_cultural_sphere; } }
			public bool HasNameImage { get { return m_has_name_image; } }
			public string Lang1 { get { return m_lang1; } }
			public string Lang2 { get { return m_lang2; } }
			public int Sakaba { get { return m_sakaba_flag; } }

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			private CityInfo(string name) {
				m_name = name;
				m_index = 0;
				m_city_type = GvoWorldInfo.CityType.City;
				m_alliance_type = GvoWorldInfo.AllianceType.Alliance;
				m_cultural_sphere = GvoWorldInfo.CulturalSphere.Unknown;
				m_has_name_image = false;
				m_lang1 = "";
				m_lang2 = "";
				m_sakaba_flag = 0;
				m_domains = new GvoWorldInfo.Country[Enum.GetValues(typeof(GvoWorldInfo.Server)).Length];

				// 전부불명
				for (int i = 0; i < m_domains.Length; i++) {
					m_domains[i] = GvoWorldInfo.Country.Unknown;
				}
			}

			/*-------------------------------------------------------------------------
			 동맹현황を설정
			---------------------------------------------------------------------------*/
			public void SetDomain(GvoWorldInfo.Server server_index, GvoWorldInfo.Country country_index) {
				// 동맹국のときのみ설정する
				if (this.AllianceType != GvoWorldInfo.AllianceType.Alliance) return;

				m_domains[(int)server_index] = country_index;
			}

			/*-------------------------------------------------------------------------
			 동맹현황を설정
			 全서버に同じ値を설정する
			 동맹국を지정できない도시용
			---------------------------------------------------------------------------*/
			public void SetDomain(GvoWorldInfo.Country country_index) {
				// 동맹국のときは一括설정무효
				if (this.AllianceType == GvoWorldInfo.AllianceType.Alliance) return;

				for (int i = 0; i < this.m_domains.Length; i++) {
					m_domains[i] = country_index;
				}
			}

			public string GetNetUpdateString(GvoWorldInfo.Server server_index) {
				return ((int)server_index).ToString() + "+" + Index.ToString() + "+" + ((int)GetDomain(server_index)).ToString();
			}

			/*-------------------------------------------------------------------------
			 동맹현황を得る
			---------------------------------------------------------------------------*/
			public GvoWorldInfo.Country GetDomain(GvoWorldInfo.Server server_index) {
				return m_domains[(int)server_index];
			}

			internal static GvoWorldInfo.CityInfo FromXml(XmlNode node, string name) {
				if (node == null)
					return (GvoWorldInfo.CityInfo)null;
				if (node.Name != "city_detail")
					return (GvoWorldInfo.CityInfo)null;
				GvoWorldInfo.CityInfo cityInfo = new GvoWorldInfo.CityInfo(name);
				cityInfo.m_index = Useful.ToInt32(Useful.XmlGetAttribute(node, "index", ""), cityInfo.m_index);
				cityInfo.m_city_type = GvoWorldInfo.GetCityTypeFromString(Useful.XmlGetAttribute(node, "city_type", ((object)cityInfo.m_city_type).ToString()));
				cityInfo.m_alliance_type = GvoWorldInfo.GetAllianceTypeFromString(Useful.XmlGetAttribute(node, "alliance_type", ((object)cityInfo.m_alliance_type).ToString()));
				cityInfo.m_cultural_sphere = GvoWorldInfo.GetCulturalSphereFromString(Useful.XmlGetAttribute(node, "cultural_sphere", ((object)cityInfo.m_cultural_sphere).ToString()));
				cityInfo.m_has_name_image = Useful.ToBool(Useful.XmlGetAttribute(node, "has_name_image", ""), cityInfo.m_has_name_image);
				cityInfo.m_lang1 = Useful.XmlGetAttribute(node, "lang1", cityInfo.m_lang1);
				cityInfo.m_lang2 = Useful.XmlGetAttribute(node, "lang2", cityInfo.m_lang2);
				cityInfo.m_sakaba_flag = Useful.ToInt32(Useful.XmlGetAttribute(node, "bar_flags", ""), cityInfo.m_sakaba_flag);
				cityInfo.SetDomain(GvoWorldInfo.GetCountryFromString(Useful.XmlGetAttribute(node, "default_country", ((object)GvoWorldInfo.Country.Unknown).ToString())));
				return cityInfo;
			}

			public void WriteXml(XmlNode node) {
				XmlNode node1 = Useful.XmlAddNode(node, "city_detail");
				Useful.XmlAddAttribute(node1, "index", m_index.ToString());
				Useful.XmlAddAttribute(node1, "city_type", ((object)m_city_type).ToString());
				Useful.XmlAddAttribute(node1, "alliance_type", ((object)m_alliance_type).ToString());
				Useful.XmlAddAttribute(node1, "cultural_sphere", ((object)m_cultural_sphere).ToString());
				Useful.XmlAddAttribute(node1, "has_name_image", m_has_name_image.ToString());
				if (m_alliance_type == GvoWorldInfo.AllianceType.Capital || m_alliance_type == GvoWorldInfo.AllianceType.Territory)
					Useful.XmlAddAttribute(node1, "default_country", ((object)GetDomain(GvoWorldInfo.Server.Euros)).ToString());
				Useful.XmlAddAttribute(node1, "lang1", m_lang1);
				Useful.XmlAddAttribute(node1, "lang2", m_lang2);
				Useful.XmlAddAttribute(node1, "bar_flags", m_sakaba_flag.ToString());
			}

			public void LoadDomainXml(XmlNode node) {
				if (node == null || node.Name != "domain_info" || (node.Attributes["name"] == null || node.Attributes["name"].Value != m_name) || (node.ChildNodes == null || m_alliance_type != GvoWorldInfo.AllianceType.Alliance))
					return;
				foreach (XmlNode node1 in node) {
					load_domain_sub(node1, GvoWorldInfo.Server.Euros);
					load_domain_sub(node1, GvoWorldInfo.Server.Zephyros);
					load_domain_sub(node1, GvoWorldInfo.Server.Notos);
					load_domain_sub(node1, GvoWorldInfo.Server.Boreas);
				}
			}

			private void load_domain_sub(XmlNode node, GvoWorldInfo.Server server) {
				if (node.Name != "server" || node.Attributes["name"] == null || node.Attributes["name"].Value != ((object)server).ToString())
					return;
				m_domains[(int)server] = GvoWorldInfo.GetCountryFromString(Useful.XmlGetAttribute(node, "country", ((object)GvoWorldInfo.Country.Unknown).ToString()));
			}

			public void WriteDomainXml(XmlNode node) {
				if (node == null || m_alliance_type != GvoWorldInfo.AllianceType.Alliance)
					return;
				XmlNode p_node = Useful.XmlAddNode(node, "domain_info", m_name);
				Useful.XmlAddAttribute(Useful.XmlAddNode(p_node, "server", ((object)GvoWorldInfo.Server.Euros).ToString()), "country", ((object)m_domains[0]).ToString());
				Useful.XmlAddAttribute(Useful.XmlAddNode(p_node, "server", ((object)GvoWorldInfo.Server.Zephyros).ToString()), "country", ((object)m_domains[1]).ToString());
				Useful.XmlAddAttribute(Useful.XmlAddNode(p_node, "server", ((object)GvoWorldInfo.Server.Notos).ToString()), "country", ((object)m_domains[2]).ToString());
				Useful.XmlAddAttribute(Useful.XmlAddNode(p_node, "server", ((object)GvoWorldInfo.Server.Boreas).ToString()), "country", ((object)m_domains[3]).ToString());
			}

			public bool LoadDomainFromNeworkData(string[] domains) {
				if (m_alliance_type != GvoWorldInfo.AllianceType.Alliance || domains == null || (domains.Length != 4 || Index < 0) || (domains[0].Length < Index || domains[1].Length < Index || (domains[2].Length < Index || domains[3].Length < Index)))
					return false;
				m_domains[0] = GvoWorldInfo.GetCountryFromString(domains[0][Index].ToString());
				m_domains[1] = GvoWorldInfo.GetCountryFromString(domains[1][Index].ToString());
				m_domains[2] = GvoWorldInfo.GetCountryFromString(domains[2][Index].ToString());
				m_domains[3] = GvoWorldInfo.GetCountryFromString(domains[3][Index].ToString());
				return true;
			}
		}
	}

	/*-------------------------------------------------------------------------
	 아이템の종류
	 명産, 채집, 탐색ランク
	 カテゴリは아이템DBに이동した
	---------------------------------------------------------------------------*/
	public class GvoItemTypeDatabase {
		private MultiDictionary<string, ItemRank> m_bonus_items;            // 명産품扱いの아이템
		private MultiDictionary<string, ItemRank> m_nanban_trade_items; // 남만품扱いの아이템
		private MultiDictionary<string, ItemRank> m_fishting_ranks;  // 낚시ランク
		private MultiDictionary<string, ItemRank> m_collect_ranks;  // 채집ランク
		private MultiDictionary<string, ItemRank> m_supply_ranks;        // 조달ランク

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public GvoItemTypeDatabase() {
			m_bonus_items = new MultiDictionary<string, ItemRank>();
			m_nanban_trade_items = new MultiDictionary<string, ItemRank>();

			m_fishting_ranks = new MultiDictionary<string, ItemRank>();
			m_collect_ranks = new MultiDictionary<string, ItemRank>();
			m_supply_ranks = new MultiDictionary<string, ItemRank>();
		}

		public void Load() {
			try {
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(def.ITEM_TYPE_DB);
				if (xmlDocument.DocumentElement.ChildNodes == null)
					return;
				foreach (XmlNode node in xmlDocument.DocumentElement.ChildNodes) {
					if (node.Attributes["name"] != null) {
						switch (node.Attributes["name"].Value) {
							case "fish_rank":
								load_sub(node, m_fishting_ranks);
								continue;
							case "collect_ranks":
								load_sub(node, m_collect_ranks);
								continue;
							case "supply_ranks":
								load_sub(node, m_supply_ranks);
								continue;
							case "bonus_items":
								load_sub(node, m_bonus_items);
								continue;
							case "nanban_trade_items":
								load_sub(node, m_nanban_trade_items);
								continue;
							default:
								continue;
						}
					}
				}
			} catch (Exception ex) {
				Console.WriteLine("採取ランク등の읽기で問題発生");
				Console.Write(ex.Message);
			}
		}

		private void load_sub(XmlNode node, MultiDictionary<string, ItemRank> list) {
			list.Clear();
			if (node == null || node.ChildNodes == null)
				return;
			foreach (XmlNode n in node.ChildNodes) {
				ItemRank t = ItemRank.FromXml(n);
				if (t != null)
					list.Add(t);
			}
		}

		private void write_xml(string file_name) {
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild((XmlNode)xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", (string)null));
			xmlDocument.AppendChild((XmlNode)xmlDocument.CreateElement("itemtype_db_root"));
			write_item_ranks(xmlDocument.DocumentElement, "fish_rank", m_fishting_ranks);
			write_item_ranks(xmlDocument.DocumentElement, "collect_ranks", m_collect_ranks);
			write_item_ranks(xmlDocument.DocumentElement, "supply_ranks", m_supply_ranks);
			write_item_ranks(xmlDocument.DocumentElement, "bonus_items", m_bonus_items);
			write_item_ranks(xmlDocument.DocumentElement, "nanban_trade_items", m_nanban_trade_items);
			xmlDocument.Save(file_name);
		}

		private void write_item_ranks(XmlElement p_node, string name, MultiDictionary<string, ItemRank> list) {
			XmlNode node = Useful.XmlAddNode((XmlNode)p_node, "group", name);
			foreach (ItemRank itemRank in list)
				itemRank.WriteXml(node);
		}


		/*-------------------------------------------------------------------------
		 명産품かどうか조사
		---------------------------------------------------------------------------*/
		public bool IsBonusItem(string name) {
			if (m_bonus_items.GetValue(name) != null)
				return true;
			else
				return IsNanbanTradeItem(name);
		}

		/*-------------------------------------------------------------------------
		 남만품かどうか조사
		---------------------------------------------------------------------------*/
		public bool IsNanbanTradeItem(string name) {
			return m_nanban_trade_items.GetValue(name) != null;
		}

		private int get_rank(MultiDictionary<string, ItemRank> list, string name) {
			ItemRank itemRank = list.GetValue(name);
			if (itemRank == null)
				return 0;
			else
				return itemRank.Rank;
		}

		/*-------------------------------------------------------------------------
		 낚시ランクを得る
		---------------------------------------------------------------------------*/
		public int GetFishingRank(string name) {
			return get_rank(m_fishting_ranks, name);
		}

		/*-------------------------------------------------------------------------
		 채집ランクを得る
		---------------------------------------------------------------------------*/
		public int GetSaisyuRank(string name) {
			return get_rank(m_collect_ranks, name);
		}

		/*-------------------------------------------------------------------------
		 조달ランクを得る
		---------------------------------------------------------------------------*/
		public int GetChotatuRank(string name) {
			return get_rank(m_supply_ranks, name);
		}

		/*-------------------------------------------------------------------------
		 낚시, 채집, 조달용ランク取得
		---------------------------------------------------------------------------*/
		public class ItemRank : IDictionaryNode<string> {
			private string m_name;        // 아이템명
			private int m_rank;    // 必要ランク

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public string Key { get { return m_name; } }
			public string Name { get { return m_name; } }
			public int Rank { get { return m_rank; } }

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public ItemRank() {
				m_name = "";
				m_rank = 0;
			}

			internal static ItemRank FromXml(XmlNode n) {
				if (n == null)
					return (ItemRank)null;
				ItemRank itemRank = new ItemRank();
				itemRank.m_name = Useful.XmlGetAttribute(n, "name", itemRank.m_name);
				itemRank.m_rank = Useful.ToInt32(Useful.XmlGetAttribute(n, "rank", "0"), 0);
				if (string.IsNullOrEmpty(itemRank.m_name))
					return null;
				else
					return itemRank;
			}

			internal void WriteXml(XmlNode node) {
				Useful.XmlAddAttribute(Useful.XmlAddNode(node, "item", m_name), "rank", m_rank.ToString());
			}
		}
	}
}
