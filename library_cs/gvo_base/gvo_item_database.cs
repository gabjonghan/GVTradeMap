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
		private Dictionary<string, string>		m_ajust_name_list;		// 微妙な이름の間違い조정용

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		// カテゴリ
		public enum Categoly{
			Categoly1,
			Categoly2,
			Categoly3,
			Categoly4,
			Unknown
		};
		// 종류の그룹
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
			Technic,	// 陸戦テクニック
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
			// 이름수정목록の작성
			create_ajust_name_list();
		}
		public ItemDatabase(string fname)
			: this()
		{
			// DBの구축
			Load(fname);
		}

		/*-------------------------------------------------------------------------
		 DBの구축
		---------------------------------------------------------------------------*/
		public void Load(string fname)
		{
			base.Clear();
			try{
				using (StreamReader	sr	= new StreamReader(
					fname, Encoding.GetEncoding("UTF-8"))) {

					string line = "";
					while((line = sr.ReadLine()) != null){
						Data	_data	= new Data();

						if(_data.CreateFromString(line)){
							base.Add(_data);
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
		 이름조정용목록を작성함
		---------------------------------------------------------------------------*/
		private void create_ajust_name_list()
		{
			m_ajust_name_list	= new Dictionary<string,string>();

			m_ajust_name_list.Add("ロット(NO.1)", "ロット（NO.1）");
			m_ajust_name_list.Add("ロット(NO.2)", "ロット（NO.2）");
			m_ajust_name_list.Add("ロット(NO.3)", "ロット（NO.3）");
			m_ajust_name_list.Add("ロット(NO.4)", "ロット（NO.4）");
			m_ajust_name_list.Add("ロット(NO.5)", "ロット（NO.5）");
			m_ajust_name_list.Add("ロット(NO.6)", "ロット（NO.6）");
			m_ajust_name_list.Add("ロット(NO.7)", "ロット（NO.7）");
			m_ajust_name_list.Add("ロット(NO.8)", "ロット（NO.8）");
			m_ajust_name_list.Add("ロット(NO.9)", "ロット（NO.9）");
			m_ajust_name_list.Add("ロット(NO.10)", "ロット（NO.10）");
			m_ajust_name_list.Add("ロット(NO.11)", "ロット（NO.11）");
			m_ajust_name_list.Add("ロット(NO.12)", "ロット（NO.12）");
			m_ajust_name_list.Add("ロット(NO.13)", "ロット（NO.13）");
			m_ajust_name_list.Add("ロット(NO.14)", "ロット（NO.14）");
			m_ajust_name_list.Add("ロット(No.14)", "ロット（NO.14）");
			m_ajust_name_list.Add("鉱石精錬の書", "鉱石製錬の書");
			m_ajust_name_list.Add("合金製錬の書", "合金精錬の書");
			m_ajust_name_list.Add("貴金属の精錬법", "貴金属の製錬법");
			m_ajust_name_list.Add("アラブ神獣の선수상彫刻術", "アラブの神獣の선수상彫刻術");
			m_ajust_name_list.Add("ボンバルタ", "ボンバルダ");
			m_ajust_name_list.Add("牛革製ベスト", "牛皮製ベスト");
			m_ajust_name_list.Add("花嫁衣装の縫製書", "花嫁衣裳の縫製법");
			m_ajust_name_list.Add("소형배용高級上納품の梱包", "소형高級上納품の梱包");
			m_ajust_name_list.Add("중형배용高級上納품の梱包", "중형高級上納품の梱包");
			m_ajust_name_list.Add("대형배용高級上納품の梱包", "대형高級上納품の梱包");
			m_ajust_name_list.Add("高級上納품(소형배용)", "高級上納품（소형배용）");
			m_ajust_name_list.Add("高級上納품(중형배용)", "高級上納품（중형배용）");
			m_ajust_name_list.Add("高級上納품(대형배용)", "高級上納품（대형배용）");
			m_ajust_name_list.Add("全艤装補助돛縫製법", "全艤装補助돛組立법");
			m_ajust_name_list.Add("ペットの育て方初級編", "ペットの育て方　初級編");
			m_ajust_name_list.Add("セット料理集第1集", "セット料理集第1巻");
			m_ajust_name_list.Add("フォルダンミルクレープ", "フォンダン・ミルクレープ");
			m_ajust_name_list.Add("果実を使ったお菓子", "果物を使ったお菓子");
			m_ajust_name_list.Add("防御職인の工芸技법", "防具職인の工芸技법");
			m_ajust_name_list.Add("きのこバターソテー", "きのこのバターソテー");
			m_ajust_name_list.Add("フルーツ盛り合わせ", "フルーツの盛り合わせ");
			m_ajust_name_list.Add("ブッシュドノエル", "ブッシュ・ド・ノエル");
			m_ajust_name_list.Add("実용衣装裁縫術・第1巻", "実용衣装縫製術・第1巻");
			m_ajust_name_list.Add("ローマ神話の선수상彫刻術", "ローマ神話の彫刻術");
			m_ajust_name_list.Add("一味違う！手作り소物", "一味違う！　手作り소物");
			m_ajust_name_list.Add("ゲルマン諸語", "ゲルマン諸語翻訳메모");
			m_ajust_name_list.Add("동欧諸語", "동欧諸語翻訳메모");
			m_ajust_name_list.Add("ロマンス諸語", "ロマンス諸語翻訳메모");
			m_ajust_name_list.Add("アルタイ諸語", "アルタイ諸語翻訳메모");
			m_ajust_name_list.Add("セム・ハム諸語", "セム・ハム諸語翻訳메모");
			m_ajust_name_list.Add("アメリカ諸語", "アメリカ諸語翻訳메모");
			m_ajust_name_list.Add("아프리카諸語", "아프리카諸語翻訳메모");
			m_ajust_name_list.Add("인도洋諸語", "인도洋諸語翻訳메모");
			m_ajust_name_list.Add("デミ・カルヴァリン포10문", "デミ・カルヴァリン10문");
			m_ajust_name_list.Add("デミ・カルヴァリン포12문", "デミ・カルヴァリン12문");
			m_ajust_name_list.Add("デミ・カルヴァリン포14문", "デミ・カルヴァリン14문");
			m_ajust_name_list.Add("デミ・カルヴァリン포16문", "デミ・カルヴァリン16문");
			m_ajust_name_list.Add("漁師の心得・鮮魚保存법", "漁師の心得　鮮魚保存법");
			m_ajust_name_list.Add("ファルコン2포", "ファルコン포2문");
			m_ajust_name_list.Add("ファルコン4포", "ファルコン포4문");
			m_ajust_name_list.Add("ファルコン6포", "ファルコン포6문");
			m_ajust_name_list.Add("ファルコン8포", "ファルコン포8문");
			m_ajust_name_list.Add("マクラジャボトル", "マラクジャボトル");
			m_ajust_name_list.Add("マクラジャジュース", "マラクジャジュース");
			m_ajust_name_list.Add("경량シーダ추가장갑", "경량シーダー추가장갑");
			m_ajust_name_list.Add("소袖♂", "소袖");
			m_ajust_name_list.Add("소袖♀", "소袖");
			m_ajust_name_list.Add("折鳥帽子♂", "折烏帽子");
			m_ajust_name_list.Add("かんざし♀", "かんざし");
			m_ajust_name_list.Add("通天冠♂", "通天冠");
			m_ajust_name_list.Add("歩揺♀","歩揺");
			m_ajust_name_list.Add("四方平定巾♂","四方平定巾");
			m_ajust_name_list.Add("窄袖衫襦♀","窄袖衫襦");
			m_ajust_name_list.Add("直裾深衣♂","直裾深衣");
		}
	
		/*-------------------------------------------------------------------------
		 검색
		 完全一致のみ
		 아이템とのリンク용
		---------------------------------------------------------------------------*/
		public Data Find(string name)
		{
			// 이름の間違いを수정する
			name	= adjust_name(name);

			return base.GetValue(name);
		}

		/*-------------------------------------------------------------------------
		 검색
		 IDで검색
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
		 검색아이템명の이름수정
		---------------------------------------------------------------------------*/
		private string adjust_name(string name)
		{
			// ★ を省く
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
		 타입명からカテゴリを得る
		 食料품 などの이름からカテゴリを得る
		---------------------------------------------------------------------------*/
		static public Categoly GetCategolyFromType(string name)
		{
			switch(name){
			case "食料품":
			case "調味料":
			case "雑貨":
			case "医薬품":
			case "家畜":
				return Categoly.Categoly1;
			case "酒類":
			case "鉱石":
			case "染料":
			case "工業품":
			case "嗜好품":
				return Categoly.Categoly2;
			case "繊維":
			case "織物":
			case "武具":
			case "火器":
			case "工芸품":
			case "미술품":
				return Categoly.Categoly3;
			case "香辛料":
			case "貴金属":
			case "香料":
			case "宝石":
				return Categoly.Categoly4;
			}
			return Categoly.Unknown;
		}

		/*-------------------------------------------------------------------------
		 カテゴリ그리기용の색を得る
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
		 타입명から타입の그룹を得る
		 食料품 などの이름から 교역품 등の그룹を得る
		---------------------------------------------------------------------------*/
		static public TypeGroup GetTypeGroupFromType(string name)
		{
			switch(name){
			case "食料품":
			case "家畜":
			case "酒類":
			case "調味料":
			case "嗜好품":
			case "香辛料":
			case "香料":
			case "医薬품":
			case "繊維":
			case "染料":
			case "織物":
			case "貴金属":
			case "鉱石":
			case "宝石":
			case "工芸품":
			case "미술품":
			case "雑貨":
			case "武具":
			case "火器":
			case "工業품":
				return TypeGroup.Trade;
			case "消耗품":
			case "推薦状":
			case "레시피帳":
			case "宝箱":
			case "ロット":
			case "素材":
			case "ペット権利書":
			case "不動産権利書":
			case "배権利書":
			case "家具":
			case "物資":
				return TypeGroup.Item;
			case "소형돛배":
			case "中소형돛배":
			case "중형돛배":
			case "中대형돛배":
			case "대형돛배":
			case "中소형갤리":
			case "중형갤리":
			case "中대형갤리":
			case "대형갤리":
				return TypeGroup.Ship;
			case "배首선수상":
			case "추가장갑":
			case "特殊병기":
			case "補助돛":
			case "舷側포":
			case "배首포":
			case "배尾포":
			case "紋章":
				return TypeGroup.Rigging;
			case "頭장비품":
			case "体장비품":
			case "足장비품":
			case "手장비품":
			case "武器・도구":
			case "装身具":
				return TypeGroup.Equip;
			case "모험스킬":
			case "교역스킬":
			case "海事스킬":
			case "언어스킬":
			case "아이템効果스킬":
			case "副官스킬":
			case "배스킬스킬":
				return TypeGroup.Skill;
			case "보고":
				return TypeGroup.Report;
			case "陸戦テクニック":
				return TypeGroup.Technic;	// 陸戦テクニック
			default:
				return TypeGroup.Unknown;
			}
		}

		public static TypeGroup2 GetTypeGroupFromType2(string name)
		{
			switch (name)
			{
				case "食料품":
				case "家畜":
				case "酒類":
				case "調味料":
				case "嗜好품":
				case "香辛料":
				case "香料":
				case "医薬품":
				case "繊維":
				case "染料":
				case "織物":
				case "貴金属":
				case "鉱石":
				case "宝石":
				case "工芸품":
				case "미술품":
				case "雑貨":
				case "武具":
				case "火器":
				case "工業품":
					return TypeGroup2.Trade;
				case "消耗품":
				case "推薦状":
				case "레시피帳":
				case "宝箱":
				case "ロット":
				case "素材":
				case "ペット権利書":
				case "家具":
					return TypeGroup2.Item;
				case "소형돛배":
				case "中소형돛배":
				case "중형돛배":
				case "中대형돛배":
				case "대형돛배":
				case "中소형갤리":
				case "중형갤리":
				case "中대형갤리":
				case "대형갤리":
					return TypeGroup2.Ship;
				case "배首선수상":
				case "추가장갑":
				case "特殊병기":
				case "補助돛":
				case "舷側포":
				case "배首포":
				case "배尾포":
				case "紋章":
					return TypeGroup2.Item;
				case "頭장비품":
				case "体장비품":
				case "足장비품":
				case "手장비품":
				case "武器・도구":
				case "装身具":
					return TypeGroup2.Item;
				default:
					return TypeGroup2.Unknown;
			}
		}

		/*-------------------------------------------------------------------------
		 타입の그룹を문자열に변환する
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
			case TypeGroup.Rigging:		return "艤装";
			case TypeGroup.Skill:		return "스킬";
			case TypeGroup.Report:		return "보고";
			case TypeGroup.Technic:		return "陸戦テクニック";
			default:
				return "불명";
			}
		}

		/// <summary>
		/// テクニック정보のコンバート
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
								Debug.WriteLine("종류:陸戦テクニック");
							}else if(split.Length == 11){
								// 전투계
								Debug.WriteLine("ID:1");
								Debug.WriteLine("명칭:" + split[1]);
								Debug.WriteLine("설명:" + type + " " + split[7]);
								Debug.WriteLine(split[3]);
								foreach(string i in create_document0(split)){
									Debug.WriteLine(i);
								}
								Debug.WriteLine("종류:陸戦テクニック");
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
				ItemDatabase.Data data = GetValue(i.Name);
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
			private Categoly				m_categoly;				// 교역품時のカテゴリ
			private TypeGroup				m_type_group;			// 종류の그룹
			private TypeGroup2				m_type_group2;			// 所持품カテゴリ
			private bool					m_is_combat_item;		// 陸戦아이템のときtreu

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public string Key{				get{	return m_name;			}}
			public int Id{					get{	return m_id;			}}
			public string Name{				get{	return m_name;			}}
			public string Type{				get{	return m_type;			}}
			public string Document{			get{	return m_document;		}}
			public bool IsRecipe{			get{	return (Type == "레시피帳")? true: false;	}}
			public bool IsSkill{			get{
													if(Type == "모험스킬")	return true;
													if(Type == "교역스킬")	return true;
													if(Type == "海事스킬")	return true;
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
			 ItemDb.txt から구축
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

					if(m_document.IndexOf("다시사용시간：") >= 0){
						// 다시사용시간：が含まれれば陸戦아이템とする
						m_is_combat_item	= true;
					}

					// 교역품時のカテゴリ
					m_categoly		= ItemDatabase.GetCategolyFromType(m_type);
					// 종류の그룹
					m_type_group	= ItemDatabase.GetTypeGroupFromType(m_type);
					m_type_group2	= ItemDatabase.GetTypeGroupFromType2(m_type);
				}catch{
					return false;
				}
				return true;
			}

			/*-------------------------------------------------------------------------
			 ツールチップ용の문자열を得る
			---------------------------------------------------------------------------*/
			public virtual string GetToolTipString()
			{
				string	str		= "명칭:" + Name + "\n";
				str				+= "종류:" + Type + "\n";
				str				+= "설명:\n" + Document;
				return str;
			}

			/*-------------------------------------------------------------------------
			 레시피정보wikiを開く
			 레시피검색
			---------------------------------------------------------------------------*/
			public void OpenRecipeWiki0()
			{
				// EUCでURLエンコード
				string	urlenc	= Useful.UrlEncodeEUCJP(this.Name);

				// 검색결과を開く
				Process.Start(gvo_def.URL2 + urlenc);	// 레시피검색
			}

			/*-------------------------------------------------------------------------
			 레시피정보wikiを開く
			 작성가능かどうか검색
			---------------------------------------------------------------------------*/
			public void OpenRecipeWiki1()
			{
				// EUCでURLエンコード
				string	urlenc	= Useful.UrlEncodeEUCJP(this.Name);

				// 검색결과を開く
				Process.Start(gvo_def.URL3 + urlenc);	// 레시피で작성가능か검색
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
