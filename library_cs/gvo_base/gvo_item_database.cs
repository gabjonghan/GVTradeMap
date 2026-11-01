/*-------------------------------------------------------------------------

 아이템DB

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Diagnostics;
using System.Drawing;
using System;

using Utility;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvo_base
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class ItemDatabase : MultiDictionary<string, ItemDatabase.Data>
	{
		private Dictionary<string, string>		m_ajust_name_list;		// 미묘한 이름 오류 보정용
		private Dictionary<string, Data>		m_normalized_list;		// 표기 차이를 무시한 검색용, 겹치는 키는 null

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		// 카테고리
		public enum Categoly{
			Categoly1,
			Categoly2,
			Categoly3,
			Categoly4,
			Unknown
		};
		// 종류 그룹
		public enum TypeGroup{
			All,		// 전종류
			CityName,	// 도시명
			UseLang,	// 사용언어
			Trade,		// 교역품
			Item,		// 아이템
			Equip,		// 장비
			Ship,		// 배
			Rigging,	// 艤装
			Skill,		// 스킬
			Report,		// 보고
			Technic,	// 테크닉
			Unknown,	// 불명
		};
		// 
		public enum TypeGroup2
		{
			Trade,
			Item,
			Ship,
			Unknown,
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public ItemDatabase()
			: base()
		{
			// 이름 수정 목록 작성
			create_ajust_name_list();
		}
		public ItemDatabase(string fname)
			: this()
		{
			// DB 구축
			Load(fname);
		}

		/*-------------------------------------------------------------------------
		 DB 구축
		---------------------------------------------------------------------------*/
		public void Load(string fname)
		{
			base.Clear();
			m_normalized_list	= new Dictionary<string, Data>();
			try{
				using (StreamReader	sr	= new StreamReader(
					fname, Encoding.GetEncoding("UTF-8"))) {

					string line = "";
					while((line = sr.ReadLine()) != null){
						Data	_data	= new Data();

						if(_data.CreateFromString(line)){
							base.Add(_data);
							add_normalized(_data);
						}else{
							// 분석실패
						}
					}
				}
			}catch{
				// 읽기실패
				base.Clear();
			}

//			technic_cnv();
		}

		/*-------------------------------------------------------------------------
		 이름 보정용 목록을 작성한다
		---------------------------------------------------------------------------*/
		private void create_ajust_name_list()
		{
			m_ajust_name_list	= new Dictionary<string,string>();

			m_ajust_name_list.Add("행운권(NO.1)", "행운권（NO.1）");
			m_ajust_name_list.Add("행운권(NO.2)", "행운권（NO.2）");
			m_ajust_name_list.Add("행운권(NO.3)", "행운권（NO.3）");
			m_ajust_name_list.Add("행운권(NO.4)", "행운권（NO.4）");
			m_ajust_name_list.Add("행운권(NO.5)", "행운권（NO.5）");
			m_ajust_name_list.Add("행운권(NO.6)", "행운권（NO.6）");
			m_ajust_name_list.Add("행운권(NO.7)", "행운권（NO.7）");
			m_ajust_name_list.Add("행운권(NO.8)", "행운권（NO.8）");
			m_ajust_name_list.Add("행운권(NO.9)", "행운권（NO.9）");
			m_ajust_name_list.Add("행운권(NO.10)", "행운권（NO.10）");
			m_ajust_name_list.Add("행운권(NO.11)", "행운권（NO.11）");
			m_ajust_name_list.Add("행운권(NO.12)", "행운권（NO.12）");
			m_ajust_name_list.Add("행운권(NO.13)", "행운권（NO.13）");
			m_ajust_name_list.Add("행운권(NO.14)", "행운권（NO.14）");
			m_ajust_name_list.Add("행운권(No.14)", "행운권（NO.14）");
			m_ajust_name_list.Add("소형배용고급 상납품 포장", "소형고급 상납품 포장");
			m_ajust_name_list.Add("중형배용고급 상납품 포장", "중형고급 상납품 포장");
			m_ajust_name_list.Add("대형배용고급 상납품 포장", "대형고급 상납품 포장");
			m_ajust_name_list.Add("고급 상납품(소형배용)", "고급 상납품（소형배용）");
			m_ajust_name_list.Add("고급 상납품(중형배용)", "고급 상납품（중형배용）");
			m_ajust_name_list.Add("고급 상납품(대형배용)", "고급 상납품（대형배용）");
			m_ajust_name_list.Add("팰콘2포", "팰콘포 2문");
			m_ajust_name_list.Add("팰콘4포", "팰콘포 4문");
			m_ajust_name_list.Add("팰콘6포", "팰콘포 6문");
			m_ajust_name_list.Add("팰콘8포", "팰콘포 8문");
		}
	
		/*-------------------------------------------------------------------------
		 검색
		 완전 일치만
		 아이템과의 링크용
		---------------------------------------------------------------------------*/
		public Data Find(string name, Predicate<string> is_type)
		{
			// 같은 이름의 행이 여럿이면 분류가 맞는 행을 우선한다
			Data[]	list	= base.GetValueList(name);
			if((list != null) && (list.Length > 1)){
				foreach(Data d in list){
					if(is_type(d.Type))	return d;
				}
			}
			return Find(name);
		}

		public Data Find(string name)
		{
			// item_db.txt 에 그대로 있으면 보정표보다 우선한다
			Data	d	= base.GetValue(name);
			if(d != null)	return d;

			// 이름의 오류를 수정한다
			name	= adjust_name(name);

			d	= base.GetValue(name);
			if(d != null)	return d;

			// 띄어쓰기, 전각/반각, ♂♀ 등의 표기 차이만 있는 경우
			return find_normalized(name);
		}

		/*-------------------------------------------------------------------------
		 표기 차이를 무시한 비교용 이름을 얻는다
		 cityinfos.xml 등과 item_db.txt 사이의 띄어쓰기·괄호·성별 기호 차이를 흡수한다
		---------------------------------------------------------------------------*/
		static private string normalize_name(string name)
		{
			StringBuilder	sb	= new StringBuilder(name.Length);
			foreach(char c in name){
				switch(c){
				case '（':	sb.Append('(');	break;
				case '）':	sb.Append(')');	break;
				case '［':	sb.Append('[');	break;
				case '］':	sb.Append(']');	break;
				case '：':	sb.Append(':');	break;
				case '！':	sb.Append('!');	break;
				case '♂':
				case '♀':
				case '・':
				case '·':
					break;
				default:
					if(!char.IsWhiteSpace(c))	sb.Append(c);
					break;
				}
			}
			return sb.ToString();
		}

		/*-------------------------------------------------------------------------
		 표기 차이를 무시한 검색용 목록에 추가한다
		 다른 아이템과 같은 키가 되는 경우는 어느 쪽인지 알 수 없으므로 null 로 해 둔다
		---------------------------------------------------------------------------*/
		private void add_normalized(Data d)
		{
			string	key	= normalize_name(d.Name);
			Data	old;
			if(m_normalized_list.TryGetValue(key, out old)){
				if((old != null) && (old.Name != d.Name))	m_normalized_list[key]	= null;
				return;
			}
			m_normalized_list.Add(key, d);
		}

		/*-------------------------------------------------------------------------
		 표기 차이를 무시해서 검색
		---------------------------------------------------------------------------*/
		private Data find_normalized(string name)
		{
			if(m_normalized_list == null)	return null;
			Data	d;
			if(m_normalized_list.TryGetValue(normalize_name(name), out d))	return d;
			return null;
		}

		/*-------------------------------------------------------------------------
		 검색
		 ID 로 검색
		---------------------------------------------------------------------------*/
		public Data Find(int id)
		{
			IEnumerator<Data>	e	= base.GetEnumerator();
			while(e.MoveNext()){
				if(e.Current.Id == id)	return e.Current;
			}
			return null;
		}
		
		/*-------------------------------------------------------------------------
		 검색 아이템명의 이름 보정
		---------------------------------------------------------------------------*/
		private string adjust_name(string name)
		{
			// ★ 를 생략한다
			int	start_index	= name.IndexOf("★");
			if(start_index >= 0){
				name	= name.Substring(0, start_index);
			}

			// 수정
			string	ajust;
			if(m_ajust_name_list.TryGetValue(name, out ajust)){
				name	= ajust;
			}
			return name;
		}

		/*-------------------------------------------------------------------------
		 타입명으로 카테고리를 얻는다
		 식료품 등의 이름으로 카테고리를 얻는다
		---------------------------------------------------------------------------*/
		static public Categoly GetCategolyFromType(string name)
		{
			switch(name){
			case "식료품":
			case "조미료":
			case "잡화":
			case "의약품":
			case "가축":
				return Categoly.Categoly1;
			case "주류":
			case "광석":
			case "염료":
			case "공업품":
			case "기호품":
				return Categoly.Categoly2;
			case "섬유":
			case "직물":
			case "무기류":
			case "총포류":
			case "공예품":
			case "미술품":
				return Categoly.Categoly3;
			case "향신료":
			case "귀금속":
			case "향료":
			case "보석":
				return Categoly.Categoly4;
			}
			return Categoly.Unknown;
		}

		/*-------------------------------------------------------------------------
		 카테고리 그리기용 색을 얻는다
		---------------------------------------------------------------------------*/
		static public Color GetCategolyColor(Categoly cate)
		{
			switch(cate){
			case Categoly.Categoly1:	return Color.Gray;
			case Categoly.Categoly2:	return Color.OrangeRed;
			case Categoly.Categoly3:	return Color.Green;
			case Categoly.Categoly4:	return Color.Blue;
			}
			return Color.Black;
		}

		/*-------------------------------------------------------------------------
		 타입명으로 타입 그룹을 얻는다
		 식료품 등의 이름으로 교역품 등의 그룹을 얻는다
		---------------------------------------------------------------------------*/
		static public TypeGroup GetTypeGroupFromType(string name)
		{
			switch(name){
			case "식료품":
			case "가축":
			case "주류":
			case "조미료":
			case "기호품":
			case "향신료":
			case "향료":
			case "의약품":
			case "섬유":
			case "염료":
			case "직물":
			case "귀금속":
			case "광석":
			case "보석":
			case "공예품":
			case "미술품":
			case "잡화":
			case "무기류":
			case "총포류":
			case "공업품":
				return TypeGroup.Trade;
			case "소비품":
			case "추천서":
			case "레시피":
			case "보물상자":
			case "행운권":
			case "소재":
			case "선박 소재":
			case "애완동물":
			case "부동산 권리 증서":
			case "선박 교환권":
			case "가구":
			case "물자":
				return TypeGroup.Item;
			case "소형범선":
			case "중소형범선":
			case "중소형 범선":
			case "중형범선":
			case "중형 범선":
			case "중대형범선":
			case "중대형 범선":
			case "대형범선":
			case "대형 범선":
			case "중소형갤리":
			case "중소형 갤리":
			case "중형갤리":
			case "중대형갤리":
			case "대형갤리":
			case "대형 갤리":
				return TypeGroup.Ship;
			case "선수상":
			case "추가장갑":
			case "특수장비":
			case "보조돛":
			case "선측포":
			case "선수포":
			case "선미포":
			case "문장":
				return TypeGroup.Rigging;
			case "머리 장비품":
			case "몸 장비품":
			case "다리 장비품":
			case "팔 장비품":
			case "무기/도구":
			case "장신구":
				return TypeGroup.Equip;
			case "모험스킬":
			case "교역스킬":
			case "전투스킬":
			case "언어스킬":
			case "아이템 효과":
			case "부관스킬":
			case "옵션스킬(선박)":
			case "전용함스킬(선박)":
				return TypeGroup.Skill;
			case "보고":
				return TypeGroup.Report;
			case "테크닉":
				return TypeGroup.Technic;	// 테크닉
			default:
				return TypeGroup.Unknown;
			}
		}

		public static TypeGroup2 GetTypeGroupFromType2(string name)
		{
			switch (name)
			{
				case "식료품":
				case "가축":
				case "주류":
				case "조미료":
				case "기호품":
				case "향신료":
				case "향료":
				case "의약품":
				case "섬유":
				case "염료":
				case "직물":
				case "귀금속":
				case "광석":
				case "보석":
				case "공예품":
				case "미술품":
				case "잡화":
				case "무기류":
				case "총포류":
				case "공업품":
					return TypeGroup2.Trade;
				case "소비품":
				case "추천서":
				case "레시피":
				case "보물상자":
				case "행운권":
				case "소재":
				case "선박 소재":
				case "애완동물":
				case "가구":
					return TypeGroup2.Item;
				case "소형범선":
				case "중소형범선":
				case "중소형 범선":
				case "중형범선":
				case "중형 범선":
				case "중대형범선":
				case "중대형 범선":
				case "대형범선":
				case "대형 범선":
				case "중소형갤리":
				case "중소형 갤리":
				case "중형갤리":
				case "중대형갤리":
				case "대형갤리":
				case "대형 갤리":
					return TypeGroup2.Ship;
				case "선수상":
				case "추가장갑":
				case "특수장비":
				case "보조돛":
				case "선측포":
				case "선수포":
				case "선미포":
				case "문장":
					return TypeGroup2.Item;
				case "머리 장비품":
				case "몸 장비품":
				case "다리 장비품":
				case "팔 장비품":
				case "무기/도구":
				case "장신구":
					return TypeGroup2.Item;
				default:
					return TypeGroup2.Unknown;
			}
		}

		/*-------------------------------------------------------------------------
		 타입 그룹을 문자열로 변환한다
		---------------------------------------------------------------------------*/
		static public string ToString(TypeGroup tg)
		{
			switch(tg){
			case TypeGroup.All:			return "전종류";
			case TypeGroup.CityName:	return "도시명";
			case TypeGroup.UseLang:	return "사용언어";
			case TypeGroup.Trade:		return "교역품";
			case TypeGroup.Item:		return "아이템";
			case TypeGroup.Equip:		return "장비";
			case TypeGroup.Ship:		return "배";
			case TypeGroup.Rigging:		return "의장";
			case TypeGroup.Skill:		return "스킬";
			case TypeGroup.Report:		return "보고";
			case TypeGroup.Technic:		return "테크닉";
			default:
				return "불명";
			}
		}

		/// <summary>
		/// 테크닉 정보 변환
		/// </summary>
		private void technic_cnv()
		{
			string	fname	= @"database\tec_wiki.txt";
	
			string line = "";
			try{
				using (StreamReader	sr	= new StreamReader(
					fname, Encoding.GetEncoding("UTF-8"))) {

					string	type	= "";
					while((line = sr.ReadLine()) != null){
						if(line == "")		continue;

						if(line.IndexOf("**") == 0){
							type = line.Substring(2);
						}else{
							line	= line.Replace(@"&br;", "");

							string[]	split	= line.Split(new char[]{'|'}, StringSplitOptions.None);

							if(split.Length < 1)					continue;
							if(split[1] == @"CENTER:")				continue;
							if(split[1] == @"스킬명")			continue;
							if(split[1] == @"BGCOLOR(#FFE9DD):")	continue;
							if(split[1] == "")						continue;
		
							if(split.Length == 10){
								// 모험, 商인
								Debug.WriteLine("ID:1");
								Debug.WriteLine("명칭:" + split[1]);
								Debug.WriteLine("설명:" + type + "계");
								Debug.WriteLine(split[3]);
								foreach(string i in create_document0(split)){
									Debug.WriteLine(i);
								}
								Debug.WriteLine("종류:테크닉");
							}else if(split.Length == 11){
								// 전투계
								Debug.WriteLine("ID:1");
								Debug.WriteLine("명칭:" + split[1]);
								Debug.WriteLine("설명:" + type + " " + split[7]);
								Debug.WriteLine(split[3]);
								foreach(string i in create_document0(split)){
									Debug.WriteLine(i);
								}
								Debug.WriteLine("종류:테크닉");
							}
							Debug.WriteLine("");
						}
					}
				}
			}catch{
				// 읽기실패
				base.Clear();
			}
		}

		private string[] create_document0(string[] split)
		{
			List<string>	list	= new List<string>();
			string	tmp;
			tmp		= "Rank:" + split[2] + " ";
			tmp		+= "消費:" + split[4] + " ";
			tmp		+= "射程:" + split[5] + " ";
			tmp		+= "범위:" + split[6] + " ";
			list.Add(tmp);
			return list.ToArray();
		}

		public static string ToString(TypeGroup2 tg)
		{
			switch (tg)
			{
				case TypeGroup2.Trade:
					return "교역품";
				case TypeGroup2.Item:
					return "아이템";
				case TypeGroup2.Ship:
					return "배";
				default:
					return "불명";
			}
		}

		public bool MergeShipPartsDatabase(ShipPartsDataBase db)
		{
			if (db == null)
				return false;
			foreach (ShipPartsDataBase.ShipPart i in db.PartsList)
			{
				// ShipParts.xml 은 띄어쓰기 없이 적힌 이름이 많으므로(평갑판형소형선체 등) 표기 차이를 무시하는 Find 로 찾는다
				ItemDatabase.Data data = Find(i.Name);
				if (data != null)
					data.MergeShipPartsDatabase(i);
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 아이템
		---------------------------------------------------------------------------*/
		public class Data : IDictionaryNode<string>
		{
			private int						m_id;
			private string					m_name;
			private string					m_type;
			private string					m_document;
			private Categoly				m_categoly;				// 교역품일 때의 카테고리
			private TypeGroup				m_type_group;			// 종류 그룹
			private TypeGroup2				m_type_group2;			// 所持품카테고리
			private bool					m_is_combat_item;		// 육상전 아이템일 때 true

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public string Key{				get{	return m_name;			}}
			public int Id{					get{	return m_id;			}}
			public string Name{				get{	return m_name;			}}
			public string Type{				get{	return m_type;			}}
			public string Document{			get{	return m_document;		}}
			public bool IsRecipe{			get{	return (Type == "레시피")? true: false;	}}
			public bool IsSkill{			get{
													if(Type == "모험스킬")	return true;
													if(Type == "교역스킬")	return true;
													if(Type == "전투스킬")	return true;
													if(Type == "언어스킬")	return true;
													return false;
											}
								}
			public bool IsReport{			get{	return (Type == "보고")? true: false;		}}
			public Categoly Categoly{		get{	return m_categoly;		}}
			public Color CategolyColor{		get{	return ItemDatabase.GetCategolyColor(m_categoly);	}}
			public TypeGroup TypeGroup{		get{	return m_type_group;	}}
			public bool IsCombatItem{		get{	return m_is_combat_item;	}}
			public TypeGroup2 TypeGroup2{	get{	return m_type_group2;	}}
			public string TypeGroup2Str{	get{	return ItemDatabase.ToString(m_type_group2);	}}
	
			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public Data()
			{
			}

			/*-------------------------------------------------------------------------
			 ItemDb.txt 로부터 구축
			---------------------------------------------------------------------------*/
			public bool CreateFromString(string line)
			{
				string[]	tmp		= line.Split(new char[]{','});
				if(tmp.Length < 4)	return false;

				try{
					m_id			= Useful.ToInt32(tmp[0].Trim(), 0);
					m_type			= tmp[1].Trim();
					m_name			= tmp[2].Trim();
					m_document		= "";
					for(int i=3; i<tmp.Length; i++){
						m_document	+= tmp[i].Trim() + "\n";
					}

					if(m_document.IndexOf("재사용시간") >= 0){
						// 재사용시간 이 포함되면 육상전 아이템으로 본다 (item_db.txt 의 표기는 "재사용시간:225")
						m_is_combat_item	= true;
					}

					// 교역품일 때의 카테고리
					m_categoly		= ItemDatabase.GetCategolyFromType(m_type);
					// 종류 그룹
					m_type_group	= ItemDatabase.GetTypeGroupFromType(m_type);
					m_type_group2	= ItemDatabase.GetTypeGroupFromType2(m_type);
				}catch{
					return false;
				}
				return true;
			}

			/*-------------------------------------------------------------------------
			 툴팁용 문자열을 얻는다
			---------------------------------------------------------------------------*/
			public virtual string GetToolTipString()
			{
				string	str		= "명칭:" + Name + "\n";
				str				+= "종류:" + Type + "\n";
				str				+= "설명:\n" + Document;
				return str;
			}

			/*-------------------------------------------------------------------------
			 레시피 정보 사이트를 연다
			 레시피검색
			---------------------------------------------------------------------------*/
			public void OpenRecipeWiki0()
			{
				// UTF-8 로 URL 인코드
				string	urlenc	= Useful.UrlEncodeUTF8(this.Name);

				// 검색 결과를 연다
				Process.Start(gvo_def.URL2 + urlenc);	// 레시피검색
			}

			/*-------------------------------------------------------------------------
			 레시피 정보 사이트를 연다
			 작성 가능한지 검색
			---------------------------------------------------------------------------*/
			public void OpenRecipeWiki1()
			{
				// UTF-8 로 URL 인코드
				string	urlenc	= Useful.UrlEncodeUTF8(this.Name);

				// 검색 결과를 연다
				Process.Start(gvo_def.URL3 + urlenc);	// 레시피로 작성 가능한지 검색
			}

			internal void MergeShipPartsDatabase(ShipPartsDataBase.ShipPart i)
			{
				if (i == null)
					return;
				ItemDatabase.Data data = this;
				string str = data.m_document + i.ToStringParamsOnly();
				data.m_document = str;
			}
		}
	}
}
