/*-------------------------------------------------------------------------

 해역명などの絵정보

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
	
using directx;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	public class seainfonameimage : d3d_sprite_rects
	{
		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
	
		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public seainfonameimage(d3d_device device, string infoimage_fname)
			: base(device, infoimage_fname)
		{
			if(device.device == null)	return;
			add_rects();
		}

		/*-------------------------------------------------------------------------
		 풍향용矩形を得る
		---------------------------------------------------------------------------*/
		public d3d_sprite_rects.rect GetWindArrowIcon()
		{
			return GetRect(rect_count - 2);
		}
		/*-------------------------------------------------------------------------
		 도시の위치조정용
		---------------------------------------------------------------------------*/
		public d3d_sprite_rects.rect GetCityIcon()
		{
			return GetRect(rect_count - 1);
		}

		/*-------------------------------------------------------------------------
		 切り取り矩形の추가
		---------------------------------------------------------------------------*/
		private void add_rects()
		{
			//seainfonameimage.cs 
			//seainfonames.dds와 매핑
			AddRect(new Vector2(-5, -6), new Rectangle(0, 0, 60, 12 - 0));	  //   0 ノルウェー해
			AddRect(new Vector2(-5, -6), new Rectangle(0, 12, 74, 24 - 12));	  //   1 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 24, 22, 36 - 24));	  //   2 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 36, 38, 48 - 36));	  //   3 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 48, 70, 60 - 48));	  //   4 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 60, 70, 72 - 60));	  //   5 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 72, 50, 84 - 72));	  //   6 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 84, 52, 96 - 84));	  //   7 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 96, 42, 108 - 96));	  //   8 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 108, 50, 120 - 108));	  //   9 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 120, 52, 132 - 120));	  //  10 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 132, 50, 156 - 132));	  //  11 
			AddRect(new Vector2(-5, -5), new Rectangle(0, 156, 42, 180 - 156));	  //  12 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 180, 46, 192 - 180));	  //  13 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 192, 56, 204 - 192));	  //  14 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 204, 48, 216 - 204));	  //  15 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 216, 46, 228 - 216));	  //  16 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 228, 42, 240 - 228));	  //  17 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 240, 22, 252 - 240));	  //  18 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 252, 50, 264 - 252));	  //  19 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 264, 52, 276 - 264));	  //  20 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 276, 52, 288 - 276));	  //  21 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 288, 42, 300 - 288));	  //  22 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 300, 42, 312 - 300));	  //  23 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 312, 52, 324 - 312));	  //  24 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 324, 40, 336 - 324));	  //  25 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 336, 62, 348 - 336));	  //  26 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 348, 52, 360 - 348));	  //  27 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 360, 62, 372 - 360));	  //  28 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 372, 82, 384 - 372));	  //  29 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 384, 62, 396 - 384));	  //  30 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 396, 70, 408 - 396));	  //  31 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 408, 62, 420 - 408));	  //  32 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 420, 52, 432 - 420));	  //  33 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 432, 22, 444 - 432));	  //  34 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 444, 52, 456 - 444));	  //  35 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 456, 62, 468 - 456));	  //  36 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 468, 60, 480 - 468));	  //  37 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 480, 62, 492 - 480));	  //  38 
			AddRect(new Vector2(-5, -6), new Rectangle(0, 492, 62, 504 - 492));	  //  39 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 0, 54, 12 - 0));		//  40 ベンガル만
			AddRect(new Vector2(-5, -6), new Rectangle(84, 12, 62, 24 - 12));	  //  41 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 24, 80, 36 - 24));	  //  42 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 36, 52, 48 - 36));	  //  43 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 48, 82, 60 - 48));	  //  44 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 60, 52, 72 - 60));	  //  45 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 72, 52, 84 - 72));	  //  46 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 84, 52, 96 - 84));	 //  47 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 96, 52, 108 - 96));	 //  48 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 108, 72, 120 - 108));   //  49 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 120, 62, 132 - 120));   //  50 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 132, 52, 144 - 132));   //  51 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 144, 90, 156 - 144));   //  52 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 156, 82, 168 - 156));   //  53 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 168, 72, 180 - 168));   //  54 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 180, 52, 192 - 180));   //  55 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 192, 62, 204 - 192));   //  56 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 204, 72, 216 - 204));   //  57 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 216, 40, 228 - 216));   //  58 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 228, 42, 240 - 228));   //  59 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 240, 52, 252 - 240));   //  60 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 252, 42, 264 - 252));   //  61 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 264, 72, 276 - 264));   //  62 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 276, 72, 288 - 276));   //  63 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 288, 102, 300 - 288));   //  64 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 300, 52, 312 - 300));   //  65 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 312, 52, 324 - 312));   //  66 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 324, 102, 336 - 324));   //  67 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 336, 52, 348 - 336));   //  68 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 348, 52, 360 - 348));   //  69 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 360, 52, 372 - 360));   //  70 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 372, 72, 384 - 372));   //  71 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 384, 72, 396 - 384));   //  72 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 396, 62, 408 - 396));   //  73 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 408, 72, 420 - 408));   //  74 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 420, 52, 432 - 420));   //  75 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 432, 42, 444 - 432));   //  76 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 444, 72, 456 - 444));   //  77 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 456, 82, 468 - 456));   //  78 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 468, 72, 480 - 468));   //  79 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 480, 82, 492 - 480));   //  80 
			AddRect(new Vector2(-5, -6), new Rectangle(84, 492, 72, 504 - 492));   //  81 
			AddRect(new Vector2(-5, -6), new Rectangle(180,   0,  42, 12)); //  82 パナマ만
			AddRect(new Vector2(-5, -6), new Rectangle(168,  12,  82, 12)); //  83 ガラパゴス諸島앞바다
			AddRect(new Vector2(-5, -6), new Rectangle(180,  24,  62, 12)); //  84 グアヤキル만
			AddRect(new Vector2(-5, -6), new Rectangle(180,  36,  62, 12)); //  85 동アジア서부
			AddRect(new Vector2(-5, -6), new Rectangle(180,  48,  62, 12)); //  86 동アジア동부
			AddRect(new Vector2(-5, -6), new Rectangle(180,  60,  72, 12)); //  87 북서태평양해저분지
			AddRect(new Vector2(-6, -6), new Rectangle(180,  72,  72, 12)); //  88 テラ・ノヴァ앞바다
			AddRect(new Vector2(-6, -6), new Rectangle(168,  84,  72, 12)); //  89 アイスランド해저분지
			AddRect(new Vector2(-6, -6), new Rectangle(180,  96,  60, 12)); //  90 ラブラドル해
			AddRect(new Vector2(-6, -6), new Rectangle(180, 108,  42, 12)); //  91 ハワイ앞바다
			AddRect(new Vector2(-6, -6), new Rectangle(180, 120,  70, 12)); //  92 デンマーク해저분지
			AddRect(new Vector2(-6, -6), new Rectangle(180, 132,  69, 12)); //  93 ノルウェー해저분지
			AddRect(new Vector2(-6, -6), new Rectangle(180, 144,  70, 12)); //  94 북ノルウェー해
			AddRect(new Vector2(-6, -6), new Rectangle(180, 156,  52, 12)); //  95 フラム해峡
			AddRect(new Vector2(-6, -6), new Rectangle(180, 168,  81, 12)); //  96 ロフォーテン해저분지
			AddRect(new Vector2(-6, -6), new Rectangle(180, 180,  63, 12)); //  97 서バレンツ해
			AddRect(new Vector2(-6, -6), new Rectangle(180, 192,  63, 12)); //  98 동バレンツ해
			AddRect(new Vector2(-6, -6), new Rectangle(180, 204,  21, 12)); //  99 白해
			AddRect(new Vector2(-6, -6), new Rectangle(180, 216,  41, 12)); // 100 서カラ해
			AddRect(new Vector2(-6, -6), new Rectangle(180, 228,  41, 12)); // 101 동カラ해
			AddRect(new Vector2(-6, -6), new Rectangle(180, 240,  49, 12)); // 102 ラプテフ해
			AddRect(new Vector2(-6, -6), new Rectangle(180, 252,  70, 12)); // 103 コテリヌイ島앞바다
			AddRect(new Vector2(-6, -6), new Rectangle(180, 264,  60, 12)); // 104 동シベリア해
			AddRect(new Vector2(-6, -6), new Rectangle(180, 276,  50, 12)); // 105 チュクシ해
			AddRect(new Vector2(-6, -6), new Rectangle(180, 300,  71, 12)); // 106 동ベーリング해
			AddRect(new Vector2(-6, -6), new Rectangle(180, 312,  71, 12)); // 107 서ベーリング해
			AddRect(new Vector2(-6, -6), new Rectangle(180, 336,  93, 12)); // 108 カムチャツカ반島앞바다
			AddRect(new Vector2(-6, -6), new Rectangle(180, 348,  61, 12)); // 109 オホーツク해
			AddRect(new Vector2(-6, -6), new Rectangle(180, 360, 101, 12)); // 110 북グリーンランド島앞바다
			AddRect(new Vector2(-6, -6), new Rectangle(180, 372, 101, 12)); // 111 남グリーンランド島앞바다

			AddRect(new Vector2(-6, -6), new Rectangle(285,   0,  52, 12)); // 112 ハドソン만
			AddRect(new Vector2(-6, -6), new Rectangle(285,  12,  62, 12)); // 113 ハドソン해峡
			AddRect(new Vector2(-6, -6), new Rectangle(285,  24,  82, 12)); // 114 カリフォルニア만
			AddRect(new Vector2(-6, -6), new Rectangle(285,  36,  92, 12)); // 115 サンフランシスコ앞바다
			AddRect(new Vector2(-6, -6), new Rectangle(285,  48, 104, 12)); // 116 アレキサンダー諸島앞바다
			AddRect(new Vector2(-6, -6), new Rectangle(285,  60,  54, 12)); // 117 북동태평양
			AddRect(new Vector2(-6, -6), new Rectangle(285,  72,  52, 12)); // 118 アラスカ만
			AddRect(new Vector2(-6, -6), new Rectangle(285,  84,  54, 12)); // 119 バロー곶앞바다
			AddRect(new Vector2(-6, -6), new Rectangle(285,  96,  72, 12)); // 120 ボーフォート해
			AddRect(new Vector2(-6, -6), new Rectangle(285, 108,  54, 12)); // 121 북極諸島앞바다
			AddRect(new Vector2(-6, -6), new Rectangle(285, 120,  82, 12)); // 122 エルズミーア島앞바다
			AddRect(new Vector2(-6, -6), new Rectangle(285, 132,  62, 12)); // 123 バフィン島앞바다
			AddRect(new Vector2(-6, -6), new Rectangle(285, 144,  52, 12)); // 124 バフィン만

			AddRect(new Vector2(-4, -7), new Rectangle(240, 492, 250 - 240, 504 - 492)); // WindArrowIcon
			AddRect(new Vector2(-2, -2), new Rectangle(240, 480, 246 - 240, 486 - 480)); // CityIcon
		}
	}
}
