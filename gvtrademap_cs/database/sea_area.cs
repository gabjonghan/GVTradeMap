/*-------------------------------------------------------------------------

 해역변동시스템のエリア표시
 マスクからエリアを作成する기능付き

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;

using directx;
using Utility;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class sea_area : IDisposable
	{
		/*-------------------------------------------------------------------------
		 해역군1つ
		---------------------------------------------------------------------------*/
		public class sea_area_once : IDisposable
		{
			/*-------------------------------------------------------------------------
			 해역1つ
			---------------------------------------------------------------------------*/
			class data : TextureUnit
			{
				private string				m_name;
				private Vector2				m_pos;
				private Vector2				m_size;
				private bool				m_is_create_from_mask;

				// 矩形判定用
				private hittest				m_hittest;
	
				/*-------------------------------------------------------------------------
				 
				---------------------------------------------------------------------------*/
				public string name{						get{	return m_name;		}}
				public Vector2 position{				get{	return m_pos;		}}
				public Vector2 size{					get{	return m_size;		}}
				public bool is_create_from_mask{		get{	return m_is_create_from_mask;	}}
	
				/*-------------------------------------------------------------------------
				 
				---------------------------------------------------------------------------*/
				public data(string name, Vector2 pos, Vector2 size, bool is_create_from_mask) : base()
				{
					m_name					= name;
					m_pos					= pos;
					m_size					= size;
					m_is_create_from_mask	= is_create_from_mask;

					m_hittest				= new hittest(new Rectangle(0, 0, (int)size.X, (int)size.Y), transform.ToPoint(pos));
				}

				/*-------------------------------------------------------------------------
				 マスクから作成
				---------------------------------------------------------------------------*/
				public void CreateFromMask(d3d_device device, ref byte[] image, Size size, int stride)
				{
					// マスクから作る必要のない해역は矩形でよい
					if(!m_is_create_from_mask)	return;

					Rectangle rect	= new Rectangle((int)m_pos.X, (int)m_pos.Y, (int)m_size.X & ~1, (int)m_size.Y & ~1);
					base.CreateFromMask(device, ref image, size, stride, rect);
				}

				/*-------------------------------------------------------------------------
				 그리기
				---------------------------------------------------------------------------*/
				public void Draw(Vector2 offset, LoopXImage image, int color)
				{
					if(IsCreate){
						// マスクから作った텍스쳐あり
						Draw(new Vector3(offset.X, offset.Y, 0.79f), image.ImageScale, color);
					}else{
						// 単純な矩形
						// マスクを作らない분텍스쳐容量を削る
						Vector2 pos0	= m_pos;
						Vector2 pos		= image.GlobalPos2LocalPos(pos0, offset);
						Vector2	size	= m_size;
						size.X			-= 1;
						size.Y			-= 1;
						size			*= image.ImageScale;

						// 화면外カリング
						if(pos.X + size.X < 0)	return;
						if(pos.Y + size.Y < 0)	return;
						Vector2 csize	= image.Device.client_size;
						if(pos.X >= csize.X)	return;
						if(pos.Y >= csize.Y)	return;
			
						image.Device.DrawFillRect(new Vector3(pos.X, pos.Y, 0.79f), size, color);
					}
				}

				/*-------------------------------------------------------------------------
				 ヒットテスト
				---------------------------------------------------------------------------*/
				public bool HitTest(Point pos)
				{
					return m_hittest.HitTest(pos);
				}
			}

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public enum sea_type{
				normal,		// 통상
				safty,		// 안전화
				lawless,	// 무법화
			};

			private List<data>				m_list;
			private string					m_name;
			private sea_type				m_type;

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public string name{			get{	return m_name;		}}
			public sea_type type{		get{	return m_type;		}
										set{	m_type	= value;	}}
			public string type_str{		get{	return ToString(m_type);	}}

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public sea_area_once(string name)
			{
				m_name			= name;
				m_type			= sea_type.normal;
				m_list			= new List<data>();
			}

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public void Dispose()
			{
				if(m_list == null)	return;

				foreach(data d in m_list){
					d.Dispose();
				}
				m_list.Clear();
			}
	
			/*-------------------------------------------------------------------------
			 追加
			---------------------------------------------------------------------------*/
			public void Add(string name, Vector2 pos, Vector2 size, bool is_create_from_mask)
			{
				m_list.Add(new data(name, pos, size, is_create_from_mask));
			}

			/*-------------------------------------------------------------------------
			 マスクから作成
			---------------------------------------------------------------------------*/
			public void CreateFromMask(d3d_device device, ref byte[] image, Size size, int stride)
			{
				foreach(data d in m_list){
					d.CreateFromMask(device, ref image, size, stride);
				}
			}

			/*-------------------------------------------------------------------------
			 ツールチップ用の文字列を得る
			---------------------------------------------------------------------------*/
			public string GetToolTipString()
			{
				string	str	= "";
				foreach(data d in m_list){
					if(d.name != ""){
						if(str != "")	str	+= "\n";
						str	+= d.name;
					}
				}
				return str;
			}

			/*-------------------------------------------------------------------------
			 그리기
			---------------------------------------------------------------------------*/
			public void Draw(Vector2 offset, LoopXImage image, int alpha, int alpha2, color_type type)
			{
				if(m_type == sea_type.normal)	return;		// 통상상태

				int color;
				// 지도の종류によって色を若干変える
				if(m_type == sea_type.safty){
					// 안전
					if(type == color_type.type1)	color	= Color.FromArgb(alpha, 0, 128, 220).ToArgb();
					else							color	= Color.FromArgb(alpha, 0,  64, 200).ToArgb();
				}else{
					// 무법
					if(type == color_type.type1)	color	= Color.FromArgb(alpha2, 200, 0, 0).ToArgb();
					else							color	= Color.FromArgb(alpha2, 200, 0, 0).ToArgb();
				}
				foreach(data d in m_list){
					d.Draw(offset, image, color);
				}
			}

			/*-------------------------------------------------------------------------
			 ヒットテスト
			 지도좌표で渡すこと
			---------------------------------------------------------------------------*/
			public bool HitTest(Point pos)
			{
				foreach(data d in m_list){
					if(d.HitTest(pos))	return true;
				}
				return false;
			}

			/*-------------------------------------------------------------------------
			 sea_typeを文字列で得る
			---------------------------------------------------------------------------*/
			public static string ToString(sea_type t)
			{
				switch(t){
				case sea_type.normal:		return "위험해역";
				case sea_type.safty:		return "안전해역";
				case sea_type.lawless:		return "무법해역";
				}
				return "unknown";
			}
		}

		public enum name{
			carib,
			west_africa,
			south_atlantic,
			east_africa,
			red_sea,
			indian,
			east_latin_america,
			southeast_asia,
			south_pacific,
			west_latin_america,
			east_asia,
			max
		};
		public enum color_type{
			type1,
			type2,
		};

		private const int					ALPHA_MIN		= 35;
		private const int					ALPHA_MAX		= 60;
		private const int					ALPHA_CENTER	= (ALPHA_MAX-ALPHA_MIN)/2;
		private const int					ANGLE_STEP		= 2;
		private const int					ANGLE_STEP2		= 2*2;

		private gvt_lib						m_lib;
		private float						m_angle;
		private float						m_angle2;
		private int							m_alpha;
		private int							m_alpha2;
		private List<sea_area_once>			m_groups;
		private color_type					m_color_type;

		private int							m_progress_max;
		private int							m_progress_current;
		private string						m_progress_info_str;
		private bool						m_is_loaded_mask;		// マスクを読み込み済のときtrue
	
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public List<sea_area_once> groups{		get{	return m_groups;			}}
		public color_type color{				get{	return m_color_type;		}
												set{	m_color_type	= value;	}}

		public int progress_max{				get{	return m_progress_max;		}}
		public int progress_current{			get{	return m_progress_current;	}}
		public string progress_info_str{		get{	return m_progress_info_str;		}}

		public bool IsLoadedMask{				get{	return m_is_loaded_mask;	}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public sea_area(gvt_lib lib, string fname)
		{
			m_lib			= lib;

			m_angle			= 0;
			m_angle2		= 0;
			m_color_type	= color_type.type1;
			m_groups		= new List<sea_area_once>();

			m_progress_max		= 0;
			m_progress_current	= 0;
			m_progress_info_str		= "";
			m_is_loaded_mask	= false;

			sea_area_once	once;

			// 카리브해
			once		= new sea_area_once("카리브해");
			once.Add("산후안앞바다", new Vector2(3970,1049), new Vector2(73,149), true);
			once.Add("アンティル諸島앞바다", new Vector2(3819,1049), new Vector2(150,149), true);
			once.Add("중央대서양", new Vector2(4044,	1199), new Vector2(373,148), false);
			once.Add("서카리브해", new Vector2(3670,1049), new Vector2(148,136), true);
			once.Add("", new Vector2(3753,1185), new Vector2(65,124), true);

			once.Add("コッド곶앞바다", new Vector2(3819,750), new Vector2(224,148), true);
			once.Add("バミューダ諸島앞바다", new Vector2(3717,899), new Vector2(326,149), true);
			once.Add("テラ・ノヴァ해", new Vector2(3790-1,600), new Vector2(254,149), true);
			m_groups.Add(once);
	
			// 아프리카서해안
			once		= new sea_area_once("아프리카서해안");
			once.Add("穀物해안앞바다", new Vector2(4418,1199), new Vector2(224,299), true);
			once.Add("黄金해안앞바다", new Vector2(4643,1325), new Vector2(149,173), true);
			once.Add("ギニア만", new Vector2(1,1322), new Vector2(164,176), true);
			m_groups.Add(once);

			// 남대서양
			once		= new sea_area_once("남대서양");
			once.Add("ナミビア앞바다", new Vector2(1,1499), new Vector2(180,149), true);
			once.Add("喜望峰앞바다", new Vector2(1,1649), new Vector2(297,223), true);
			once.Add("ケープ해저분지", new Vector2(1,1873), new Vector2(297,224), false);
			once.Add("남대서양", new Vector2(4494,1499), new Vector2(298,599), false);
			m_groups.Add(once);

			// 아프리카동해안
			once		= new sea_area_once("아프리카동해안");
			once.Add("アガラス곶앞바다", new Vector2(299,1683), new Vector2(149,189), true);
			once.Add("アガラス해저분지", new Vector2(299,1873), new Vector2(299,225), false);
			once.Add("モザンビーク해峡", new Vector2(449,1649), new Vector2(149,223), true);
			once.Add("マダガスカル앞바다", new Vector2(458,1500), new Vector2(440,149), true);
			once.Add("남서인도양", new Vector2(599,1649), new Vector2(299,449), true);
			m_groups.Add(once);

			// 홍해
			once		= new sea_area_once("홍해");
			once.Add("ザンジバル앞바다", new Vector2(513,1348), new Vector2(385,151), true);
			once.Add("アラビア해", new Vector2(600,1199), new Vector2(298,149), true);
			once.Add("홍해", new Vector2(457,1076), new Vector2(142,202), true);
			once.Add("ペルシャ만", new Vector2(638,1086), new Vector2(260,112), true);
			m_groups.Add(once);
	
			// 인도양
			once		= new sea_area_once("인도양");
			once.Add("인도서해안앞바다", new Vector2(899,1125), new Vector2(109,149), true);
			once.Add("인도남해안앞바다", new Vector2(899,1274), new Vector2(224,149), true);
			once.Add("ベンガル만", new Vector2(1067,1134), new Vector2(272,139), true);
			once.Add("중부인도양", new Vector2(899,1424), new Vector2(449,224), true);
			once.Add("남인도양", new Vector2(899,1649), new Vector2(301,449), false);
			once.Add("남동인도양", new Vector2(1201,1649), new Vector2(296,449), false);
			m_groups.Add(once);

			// 중남미 동해안
			once		= new sea_area_once("중남미 동해안");
			once.Add("남카리브해", new Vector2(3819,1199), new Vector2(224,148), true);
			once.Add("メキシコ만", new Vector2(3497,1055), new Vector2(173,132), true);
			once.Add("サンロケ곶앞바다", new Vector2(4194,1348), new Vector2(223,150), true);
			once.Add("アマゾン川流역", new Vector2(3900,1348), new Vector2(293,117), true);
			once.Add("남서대서양", new Vector2(4194,1499), new Vector2(299,373), true);
			once.Add("ブエノスアイレス앞바다", new Vector2(3946,1678), new Vector2(248,195), true);
			once.Add("アルゼンチン해저분지", new Vector2(3894,1873), new Vector2(374,225), true);
			once.Add("ジョージア해저분지", new Vector2(4269,1873), new Vector2(224,225), false);
			m_groups.Add(once);

			// 동남アジア
			once		= new sea_area_once("동남アジア");
			once.Add("アンダマン해", new Vector2(1124,1274), new Vector2(224,149), true);
			once.Add("ジャワ해", new Vector2(1349,1348), new Vector2(224,150), true);
			once.Add("ジャワ島남方앞바다", new Vector2(1349,1500), new Vector2(224,73), false);
			once.Add("", new Vector2(1349,1572), new Vector2(148,76), false);
			once.Add("シャム만", new Vector2(1349,1199), new Vector2(149,148), true);
			once.Add("バンダ해", new Vector2(1574,1423), new Vector2(298,150), true);
			once.Add("セレベス해", new Vector2(1499,1199), new Vector2(223,148), true);
			once.Add("", new Vector2(1574,1347), new Vector2(148,76), true);
			once.Add("서カロリン해저분지", new Vector2(1723,1199), new Vector2(149,223), true);
			m_groups.Add(once);

			// 남태평양
			once		= new sea_area_once("남태평양");
			once.Add("チリ해저분지", new Vector2(3595,1797), new Vector2(299,301), true);
			once.Add("서オーストラリア해저분지", new Vector2(1498,1573), new Vector2(224,224), true);
			once.Add("パース해저분지", new Vector2(1498,1798), new Vector2(224,300), true);
			once.Add("남オーストラリア해저분지", new Vector2(1723,1893), new Vector2(299,205), true);
			once.Add("アラフラ해", new Vector2(1723,1573), new Vector2(373,211), true);
			once.Add("", new Vector2(1873,1497), new Vector2(223,76), true);
			once.Add("동カロリン해저분지", new Vector2(1873,1199), new Vector2(223,297), true);
			once.Add("メラネシア해저분지", new Vector2(2097,1199), new Vector2(223,298), false);
			once.Add("コーラル해", new Vector2(2097,1498), new Vector2(299,299), true);
			once.Add("タスマン해", new Vector2(2023,1798), new Vector2(448,300), true);
			once.Add("중央태평양해저분지서", new Vector2(2321,1199), new Vector2(373,298), false);
			once.Add("サモア해저분지", new Vector2(2397,1498), new Vector2(224,299), true);
			once.Add("남태평양해저분지서", new Vector2(2472,1798), new Vector2(298,300), false);
			once.Add("중央태평양해저분지", new Vector2(2695,1199), new Vector2(449,298), false);
			once.Add("남태평양해저분지북", new Vector2(2622,1498), new Vector2(523,299), false);
			once.Add("남태평양해저분지", new Vector2(2771,1798), new Vector2(374,300), false);
			once.Add("남태평양해저분지동", new Vector2(3146,1498), new Vector2(448,600), false);
            once.Add("ハワイ앞바다", new Vector2(2321,751), new Vector2(449,447), true);
            m_groups.Add(once);

			// 중남미 서해안
			once		= new sea_area_once("중남미 서해안");
			once.Add("ペルー해저분지", new Vector2(3595,1498), new Vector2(260,298), true);
			once.Add("グアヤキル만", new Vector2(3595,1350), new Vector2(126,147), true);
			once.Add("パナマ만", new Vector2(3595,1234), new Vector2(136,114), true);
			once.Add("중央태평양해저분지동", new Vector2(3145,1199), new Vector2(299,298), true);
			once.Add("ガラパゴス諸島앞바다", new Vector2(3445,1350), new Vector2(149,147), true);
			once.Add("テワンテペク만", new Vector2(3445,1234), new Vector2(149,114), true);
			m_groups.Add(once);

			// 동アジア
			once		= new sea_area_once("동アジア");
			once.Add("동アジア서부", new Vector2(1393,912), new Vector2(254,286), true);
			once.Add("동アジア동부", new Vector2(1649,751), new Vector2(373,447), true);
			once.Add("북서태평양해저분지", new Vector2(2024,751), new Vector2(297,447), true);
			m_groups.Add(once);

			// 極북대서양
			once		= new sea_area_once("極북대서양");
			once.Add("フラム해峡",           new Vector2(4344,   0), new Vector2(224,294), true);
			once.Add("デンマーク해저분지",       new Vector2(4344, 295), new Vector2(224,155), false);
			once.Add("ロフォーテン해저분지",     new Vector2(4569,   0), new Vector2(223,294), true);
			once.Add("ノルウェー해저분지",       new Vector2(4569, 295), new Vector2(224,155), false);
            once.Add("ノルウェー해저분지2",      new Vector2(   0, 295), new Vector2( 74,155), false);
            m_groups.Add(once);

			// ヨーロッパ極북
			once		= new sea_area_once("ヨーロッパ極북");
            once.Add("서バレンツ해",         new Vector2(   1,   0), new Vector2(447,294), true);
            once.Add("북ノルウェー해",       new Vector2(  75, 295), new Vector2(300,155), true);
            once.Add("동バレンツ해",         new Vector2( 449,   0), new Vector2(304,294), true);
            once.Add("동バレンツ해2",        new Vector2( 601, 294), new Vector2(152,156), true);
            once.Add("白해",                 new Vector2( 376, 295), new Vector2(223,225), true);
            m_groups.Add(once);

            // ユーラシア북
            once = new sea_area_once("ユーラシア북");
            once.Add("서カラ해",             new Vector2( 754,   0), new Vector2(369,450), true);
			once.Add("동カラ해",             new Vector2(1124,   0), new Vector2(294,450), true);
			once.Add("ラプテフ해",           new Vector2(1419,   0), new Vector2(298,450), true);
			m_groups.Add(once);

			// ユーラシア極동
			once		= new sea_area_once("ユーラシア極동");
            once.Add("コテリヌイ島앞바다",       new Vector2(1718,   0), new Vector2(304,450), true);
            once.Add("동シベリア해",         new Vector2(2023,   0), new Vector2(373,450), true);
            once.Add("チュクチ해",           new Vector2(2397,   0), new Vector2(297,450), true);
			m_groups.Add(once);

			// ベーリング해
			once		= new sea_area_once("ベーリング해");
            once.Add("동ベーリング해",       new Vector2(2472, 451), new Vector2(222,298), true);
            once.Add("서ベーリング해",       new Vector2(2246, 451), new Vector2(225,298), true);
            once.Add("カムチャツカ半島앞바다",   new Vector2(2023, 451), new Vector2(222,298), true);
            once.Add("オホーツク해",         new Vector2(1648, 451), new Vector2(374,298), true);
            m_groups.Add(once);

			// 북미서해안
			once		= new sea_area_once("북미서해안");
			once.Add("アレキサンダー諸島앞바다", new Vector2(2994, 451), new Vector2(223,447), true);
			once.Add("북동태평양",           new Vector2(2770, 750), new Vector2(223,448), false);
			once.Add("アラスカ만",           new Vector2(2695, 451), new Vector2(298,298), true);
			once.Add("カリフォルニア만",     new Vector2(3218, 899), new Vector2(254,299), true);
			once.Add("サンフランシスコ앞바다",   new Vector2(2994, 899), new Vector2(223,299), true);
			m_groups.Add(once);

			// 동カナダ
			once		= new sea_area_once("동カナダ");
			once.Add("ハドソン해峡",         new Vector2(3746, 451), new Vector2(148,148), true);
            once.Add("ハドソン만",           new Vector2(3595, 451), new Vector2(150,298), true);
			once.Add("バフィン만",           new Vector2(3895, 295), new Vector2(147,155), false);
			once.Add("バフィン島앞바다",         new Vector2(3595, 295), new Vector2(299,155), true);
			once.Add("エルズミーア島앞바다",     new Vector2(3595,   0), new Vector2(447,294), true);
			m_groups.Add(once);

			// 서カナダ
			once		= new sea_area_once("서カナダ");
			once.Add("북極諸島앞바다",           new Vector2(3218,   0), new Vector2(376,450), true);
			once.Add("ボーフォート해",       new Vector2(2994,   0), new Vector2(223,450), true);
			once.Add("バロー곶앞바다",           new Vector2(2695,   0), new Vector2(298,450), true);
			m_groups.Add(once);

			// 読み込み
			load(fname);
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public void Dispose()
		{
			if(m_groups == null)	return;

			foreach(sea_area_once i in m_groups){
				i.Dispose();
			}
			m_groups.Clear();
		}

		/*-------------------------------------------------------------------------
		 読み込み
		---------------------------------------------------------------------------*/
		private void load(string fname)
		{
			if(!File.Exists(fname))	return;		// ファイルが見つからない

			string line = "";
			try{
				using (StreamReader	sr	= new StreamReader(
					fname, Encoding.GetEncoding("UTF-8"))) {

					while((line = sr.ReadLine()) != null){
						if(line == "")			continue;

						string[]	split	= line.Split(new char[]{','});
						if(split.Length < 2)	continue;

						SetType(split[0], sea_area_once.sea_type.normal + Useful.ToInt32(split[1], 0));
					}
				}
			}catch{
				// 読み込み실패
			}
		}

		/*-------------------------------------------------------------------------
		 書き出し
		---------------------------------------------------------------------------*/
		public void WriteSetting(string fname)
		{
			try{
				using (StreamWriter	sw	= new StreamWriter(
					fname, false, Encoding.GetEncoding("UTF-8"))) {

					string	str;
					foreach(sea_area_once d in m_groups){
						str	= d.name + ",";
						str	+= (int)d.type;
						sw.WriteLine(str);
					}
				}
			}catch{
				// 書き出し실패
			}
		}
	
		/*-------------------------------------------------------------------------
		 マスク読み込み用정보の初期化
		---------------------------------------------------------------------------*/
		public void InitializeFromMaskInfo()
		{
			m_progress_max			= 0;
			m_progress_current		= 0;
			m_progress_info_str		= "로딩중...";
		}
	
		/*-------------------------------------------------------------------------
		 マスクから作成する
		 マスクが必要ない해역はなにもしない
		---------------------------------------------------------------------------*/
		public bool CreateFromMask(string fname)
		{
			try{
				InitializeFromMaskInfo();

				// イメージの読み込み
				Bitmap	bitmap			= new Bitmap(fname);
				Size	size			= new Size(bitmap.Width, bitmap.Height);

				// ロックしてイメージ取り出し
				// R5G6B5に변환しておく
				BitmapData bmpdata	= bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
													ImageLockMode.ReadOnly,
													PixelFormat.Format16bppRgb565);

				IntPtr		ptr		= bmpdata.Scan0;
				int			length	= bmpdata.Height * bmpdata.Stride;
				int			stride	= bmpdata.Stride;
				byte[]		image	= new byte[length];
				Marshal.Copy(ptr, image, 0, length);
				bitmap.UnlockBits(bmpdata);

				// オリジナルは解放しておく
				bitmap.Dispose();
				bitmap					= null;

				m_progress_max			= m_groups.Count;
				m_progress_current		= 0;
				m_progress_info_str		= "";
				foreach(sea_area_once d in m_groups){
					m_progress_info_str	= "텍스쳐전송중... " + d.name;
					d.CreateFromMask(m_lib.device, ref image, size, stride);
					m_progress_current++;
				}
				m_progress_info_str	= "완료";

				image				= null;
				m_is_loaded_mask	= true;		// マスクを読み込み済

				System.GC.Collect();
			}catch{
				// 何かを실패
				return false;
			}
			return true;
		}
	
		/*-------------------------------------------------------------------------
		 업데이트
		---------------------------------------------------------------------------*/
		public void Update()
		{
			m_alpha		= ALPHA_MIN + ALPHA_CENTER + (int)((float)Math.Sin(Useful.ToRadian(m_angle)) * ALPHA_CENTER);
			m_angle		+= ANGLE_STEP;
			if(m_angle >= 360)	m_angle		-= 360;

			m_alpha2	= ALPHA_MIN + ALPHA_CENTER + (int)((float)Math.Sin(Useful.ToRadian(m_angle2)) * ALPHA_CENTER);
			m_angle2	+= ANGLE_STEP2;
			if(m_angle2 >= 360)	m_angle2	-= 360;

			// アニメーションやめ
			// 可変フレームレートになったため
			m_angle		= 30;
			m_angle2	= 30;
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		public void Draw()
		{
			m_lib.device.device.RenderState.ZBufferEnable	= false;
			m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_proc), 0);
			m_lib.device.device.RenderState.ZBufferEnable	= true;
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		private void draw_proc(Vector2 offset, LoopXImage image)
		{
			foreach(sea_area_once d in m_groups){
// debug
//				d.Type	= sea_area_once.sea_type.lawless;
				d.Draw(offset, image, m_alpha, m_alpha2, m_color_type);
			}
		}

		/*-------------------------------------------------------------------------
		 상태を설정する
		---------------------------------------------------------------------------*/
		public bool SetType(string name, sea_area.sea_area_once.sea_type type)
		{
			sea_area.sea_area_once d	= find(name);
			if(d == null)		return false;
			if(d.name == null)	return false;

			if((int)type < 0)									type	= sea_area_once.sea_type.normal;
			if((int)type > (int)sea_area_once.sea_type.lawless)	type	= sea_area_once.sea_type.normal;
	
			// 以前と違う場合지도の合成をリクエストする
			if(d.type != type){
				d.type	= type;
				m_lib.setting.req_update_map.Request();
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 리셋
		 전체해역군を통상상태にする
		---------------------------------------------------------------------------*/
		public void ResetSeaType()
		{
			foreach(sea_area.sea_area_once d in m_groups){
				d.type	= sea_area_once.sea_type.normal;
			}
		}
	
		/*-------------------------------------------------------------------------
		 지도좌표から해역군명を得る
		 得られた이름は SetType() に사용できる
		 해역군명が得られない場合はnullを返す
		---------------------------------------------------------------------------*/
		public string Find(Vector2 pos)
		{
			return Find(transform.ToPoint(pos));
		}
		public string Find(Point pos)
		{
			foreach(sea_area.sea_area_once d in m_groups){
				if(d.HitTest(pos))	return d.name;		// 見つかった해역군명を返す
			}
			return null;
		}
	
		/*-------------------------------------------------------------------------
		 검색
		---------------------------------------------------------------------------*/
		private sea_area.sea_area_once find(string name)
		{
			if(name == null)	return null;
			foreach(sea_area.sea_area_once d in m_groups){
				if(d.name == name)	return d;
			}
			// 지정된이름の해역군が存在しない
			return null;
		}

		/*-------------------------------------------------------------------------
		 ドラッグ&ドロップからの분석
		---------------------------------------------------------------------------*/
		public static List<sea_area_once_from_dd> AnalizeFromDD(string str)
		{
			List<sea_area_once_from_dd>	list	= new List<sea_area_once_from_dd>();
			string[]	lines	= str.Split(new string[]{"\r\n","\n"}, StringSplitOptions.RemoveEmptyEntries);
			foreach(string l in lines){
				sea_area_once_from_dd	data	= new sea_area_once_from_dd();
				if(data.Analize(l)){
					list.Add(data);
				}
			}
			return list;
		}

		/*-------------------------------------------------------------------------
		 ドラッグ&ドロップからの분석を反映
		 목록전부を反映させる
		---------------------------------------------------------------------------*/
		public void UpdateFromDD(List<sea_area_once_from_dd> list, bool is_clear)
		{
			if(list == null)	return;
			if(is_clear)	ResetSeaType();			// 전부を위험해역にする

			// 逆順で反映させる
			for(int i=list.Count-1; i>=0; i--){
				SetType(list[i].name, list[i]._sea_type);
			}
		}
	}

	/*-------------------------------------------------------------------------
	 해역군1つ
	 ドラッグ&ドロップからの분석내용
	---------------------------------------------------------------------------*/
	public class sea_area_once_from_dd
	{
		private string								m_name;			// 해역군명
		private string								m_server;		// 서버
		private DateTime							m_date;			// 期限
		private sea_area.sea_area_once.sea_type		m_sea_type;		// 状況
	
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public string							name		{	get{	return m_name;		}}
		public string							server_str	{	get{	return m_server;	}}
		public DateTime							date		{	get{	return m_date;		}}
		public string							date_str	{	get{	return Useful.TojbbsDateTimeString(m_date);	}}
		public sea_area.sea_area_once.sea_type	_sea_type	{	get{	return m_sea_type;	}}
		public string							_sea_type_str	{	get{	return sea_area.sea_area_once.ToString(m_sea_type);	}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public sea_area_once_from_dd()
		{
			m_name			= "";						// 해역군명
			m_server		= "";						// 서버
			m_date			= new DateTime();			// 期限
			m_sea_type		= sea_area.sea_area_once.sea_type.normal;		// 状況
		}
	
		/*-------------------------------------------------------------------------
		 분석
		 1データ분
		 フォーマット
		   서버,해역명,상태,終了日時,補足
		 補足はあってもなくてもよい
		---------------------------------------------------------------------------*/
		public bool Analize(string line)
		{
			try{
				string[]	split	= line.Split(new char[]{','});
				if(split.Length < 4)	return false;		// 항목が少なすぎる

				m_server	= split[0];
				m_name		= split[1];
				if(split[2].IndexOf("안전") >= 0){
					m_sea_type	= sea_area.sea_area_once.sea_type.safty;
				}else if(split[2].IndexOf("무법") >= 0){
					m_sea_type	= sea_area.sea_area_once.sea_type.lawless;
				}else{
					m_sea_type	= sea_area.sea_area_once.sea_type.normal;
				}
				m_date		= Useful.ToDateTime(split[3]);
			}catch{
				return false;
			}
			return true;
		}
	}
}
