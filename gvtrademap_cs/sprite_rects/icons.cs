/*-------------------------------------------------------------------------

 아이콘管理

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
	public class icons : d3d_sprite_rects
	{
		public enum icon_index{
			// 日付変わり時のふきだし아이콘
			days_big_shadow,	// 影

			// 日付표시用数字
			number_0,
			number_1,
			number_2,
			number_3,
			number_4,
			number_5,
			number_6,
			number_7,
			number_8,
			number_9,

			// 본인の배
			myship,

			// 国아이콘
			country_0,
			country_1,
			country_2,
			country_3,
			country_4,
			country_5,
			country_6,
			country_7,
			country_8,
			country_9,
			country_10,
			country_11,

			// タブ아이콘
			tab_0,
			tab_1,
			tab_2,
			tab_3,
			tab_4,
			tab_5,
			tab_6,
			tab_7,
			tab_8,
			tab_9,
            tab_10,
            tab_11,

			tab_gray_0,
			tab_gray_1,
			tab_gray_2,
			tab_gray_3,
			tab_gray_4,
			tab_gray_5,
			tab_gray_6,
			tab_gray_7,
			tab_gray_8,
			tab_gray_9,
            tab_gray_10,
            tab_gray_11,

			// 인물とかの아이콘
			tab2_0,
			tab2_1,
			tab2_2,
			tab2_3,

			tab2_gray_0,
			tab2_gray_1,
			tab2_gray_2,
			tab2_gray_3,

			// 설정아이콘
			setting_0,
			setting_1,
			setting_2,
			setting_3,
			setting_4,
			setting_5,
			setting_6,
			setting_7,
			setting_8,

			setting_gray_0,
			setting_gray_1,
			setting_gray_2,
			setting_gray_3,
			setting_gray_4,
			setting_gray_5,
			setting_gray_6,
			setting_gray_7,
			setting_gray_8,

			// 스크린샷と항로도삭제はグレイ표시なし
			setting_10,
			setting_11,
			setting_11_1,

			// 5日아이콘
			setting_12,

			// 장소
			spot_0,
			spot_1,
			spot_2,

			// 상륙지점の지도
			map_icon,

			// 선택用三角
			select_0,
			select_1,

			// 선택用クロスカーソル
			select_cross,

			// 설정ボタン
			setting_button,

			// 속도표시用背景
			speed_background,

			// 장소用国아이콘
			spot_country_0,
			spot_country_1,
			spot_country_2,
			spot_country_3,
			spot_country_4,
			spot_country_5,
			spot_country_6,
			spot_country_7,
			spot_country_8,
			spot_country_9,
			spot_country_10,
			spot_country_11,

			// 인물とかの아이콘
			spot_tab2_0,
			spot_tab2_1,
			spot_tab2_2,
			spot_tab2_3,

			// 메모用아이콘
			// 풍향
			memo_icon_0,
			memo_icon_1,
			memo_icon_2,
			memo_icon_3,
			memo_icon_4,
			memo_icon_5,
			memo_icon_6,
			memo_icon_7,
			// 재해아이콘
			memo_icon_8,
			memo_icon_9,
			memo_icon_10,
			memo_icon_11,
			memo_icon_12,
			memo_icon_13,
			memo_icon_14,
			memo_icon_15,
			memo_icon_16,
			memo_icon_17,
			memo_icon_18,
			memo_icon_19,

			// 메모用아이콘
			// 풍향
			web_icon_0,
			web_icon_1,
			web_icon_2,
			web_icon_3,
			web_icon_4,
			web_icon_5,
			web_icon_6,
			web_icon_7,
			// web재해아이콘
			web_icon_8,
			web_icon_9,
			web_icon_10,
			web_icon_11,
			web_icon_12,

			// 재해아이콘
			accident_0,
			accident_1,
			accident_2,
			accident_3,
			accident_4,
			accident_5,
			accident_6,
			accident_7,
			accident_8,
			accident_9,
			accident_10,

			accident_popup,
			accident_popup_shadow,
			
			// 장소用タブ아이콘
			spot_tab_0,
			spot_tab_1,
			spot_tab_2,
			spot_tab_3,
			spot_tab_4,
			spot_tab_5,
			spot_tab_6,
			spot_tab_7,
			spot_tab_8,
			spot_tab_9,
            spot_tab_10,
            spot_tab_11,

			// 항로공유 도시に居る
			share_city,

			// 도시, 교외
			city_icon_0,	// 수도
			city_icon_1,	// 도시
			city_icon_2,	// 수도(イスラム)
			city_icon_3,	// 도시(イスラム)
			city_icon_4,	
			city_icon_5,	
			city_icon_6,	
			city_icon_7,	

			// web
			web,

			// 도
			degree,

			// 日付変わり時の아이콘
			days_mini_6,
			// 日付変わり時のふきだし아이콘
			days_big_6,
			days_big_100,		// 100日以降の좌우が대きいもの

			string00,			// 日
			string01,			// 항로공유
			string02,			// 이자まで
			string03,			// 季節
			string04,			// 지도
			string05,			// 冬
			string06,			// 夏
			string07,			// 조선から

			max
		};
		
		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
	
		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public icons(d3d_device device, string fname)
			: base(device, fname)
		{
			if(device.device == null)	return;

			// 矩形登録
			add_rects();
		}
		
		/*-------------------------------------------------------------------------
		 矩形登録
		 icon_indexの順で割り当てる
		---------------------------------------------------------------------------*/
		private void add_rects()
		{
			// 影
			AddRect(new Vector2(-5, -6-8), new Rectangle(24, 160, 23, 15));

			// 日付표시用数値
            AddRect(new Vector2(-3, -16), new Rectangle(0, 232, 6, 9));
            AddRect(new Vector2(-3, -16), new Rectangle(8, 232, 6, 9));
            AddRect(new Vector2(-3, -16), new Rectangle(16, 232, 6, 9));
            AddRect(new Vector2(-3, -16), new Rectangle(24, 232, 6, 9));
            AddRect(new Vector2(-3, -16), new Rectangle(32, 232, 6, 9));
            AddRect(new Vector2(-3, -16), new Rectangle(40, 232, 6, 9));
            AddRect(new Vector2(-3, -16), new Rectangle(48, 232, 6, 9));
            AddRect(new Vector2(-3, -16), new Rectangle(56, 232, 6, 9));
            AddRect(new Vector2(-3, -16), new Rectangle(64, 232, 6, 9));
            AddRect(new Vector2(-3, -16), new Rectangle(72, 232, 6, 9));
            
            // 배
            AddRect(new Vector2(-6, -9), new Rectangle(216, 32, 12, 11));

			// 国아이콘
            AddRect(new Vector2(0, 0), new Rectangle(0, 0, 24, 16));
            AddRect(new Vector2(0, 0), new Rectangle(0, 16, 24, 16));
            AddRect(new Vector2(0, 0), new Rectangle(0, 32, 24, 16));
            AddRect(new Vector2(0, 0), new Rectangle(0, 48, 24, 16));
            AddRect(new Vector2(0, 0), new Rectangle(0, 64, 24, 16));
            AddRect(new Vector2(0, 0), new Rectangle(0, 80, 24, 16));
            AddRect(new Vector2(0, 0), new Rectangle(0, 96, 24, 16));
            AddRect(new Vector2(0, 0), new Rectangle(0, 112, 24, 16));
            AddRect(new Vector2(0, 0), new Rectangle(0, 128, 24, 16));
            AddRect(new Vector2(0, 0), new Rectangle(0, 144, 24, 16));
            AddRect(new Vector2(0, 0), new Rectangle(0, 160, 24, 16));
            AddRect(new Vector2(0, 0), new Rectangle(0, 176, 24, 16));

			// タブ아이콘
            AddRect(new Vector2(1, 0), new Rectangle(25, 16, 15, 16));
            AddRect(new Vector2(0, 0), new Rectangle(40, 16, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(56, 16, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(72, 16, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(88, 16, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(144, 208, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(104, 16, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(120, 16, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(136, 16, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(152, 16, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(168, 16, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(184, 16, 16, 16));

            AddRect(new Vector2(1, 0), new Rectangle(25, 0, 15, 16));
            AddRect(new Vector2(0, 0), new Rectangle(40, 0, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(56, 0, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(72, 0, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(88, 0, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(160, 208, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(104, 0, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(120, 0, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(136, 0, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(152, 0, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(168, 0, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(184, 0, 16, 16));

			// 인물とかの아이콘
            AddRect(new Vector2(0, 0), new Rectangle(24, 48, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(40, 48, 16, 16));

            AddRect(new Vector2(0, 0), new Rectangle(200, 16, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(216, 16, 16, 16));

            AddRect(new Vector2(0, 0), new Rectangle(24, 32, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(40, 32, 16, 16));

            AddRect(new Vector2(0, 0), new Rectangle(200, 0, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(216, 0, 16, 16));
            
			// 설정아이콘
            AddRect(new Vector2(0, 0), new Rectangle(56, 48, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(72, 48, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(88, 48, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(104, 48, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(120, 48, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(136, 48, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(152, 48, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(168, 48, 16, 16));
            
        	// @Web icon
            AddRect(new Vector2(0, 0), new Rectangle(216, 80, 16, 16));

            AddRect(new Vector2(0, 0), new Rectangle(56, 32, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(72, 32, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(88, 32, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(104, 32, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(120, 32, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(136, 32, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(152, 32, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(168, 32, 16, 16));

            // @Web icon
            AddRect(new Vector2(0, 0), new Rectangle(216, 64, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(232, 64, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(184, 32, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(184, 48, 16, 16));
            
            // 5日아이콘
            AddRect(new Vector2(0, 0), new Rectangle(200, 48, 16, 16));

			// 장소
            AddRect(new Vector2(-20, -20), new Rectangle(25, 97, 40, 40));
            AddRect(new Vector2(-20, -20), new Rectangle(66, 97, 40, 40));
            AddRect(new Vector2(-20, -26), new Rectangle(107, 97, 40, 40));

			// web
            AddRect(new Vector2(0, 0), new Rectangle(232, 0, 17, 17));

			// 선택用三角
            AddRect(new Vector2(0, -3), new Rectangle(40, 144, 8, 8));
            AddRect(new Vector2(0, -3), new Rectangle(48, 144, 8, 8));

			// 선택用クロスカーソル
            AddRect(new Vector2(-6, -6), new Rectangle(24, 144, 14, 14));

			// 설정ボタン
            AddRect(new Vector2(0, 0), new Rectangle(128, 144, 48, 17));

			// 속도표시用背景
            AddRect(new Vector2(0, 0), new Rectangle(80, 144, 47, 16));

			// 장소用国아이콘
            AddRect(new Vector2(-12, -8), new Rectangle(0, 0, 24, 16));
            AddRect(new Vector2(-12, -8), new Rectangle(0, 16, 24, 16));
            AddRect(new Vector2(-12, -8), new Rectangle(0, 32, 24, 16));
            AddRect(new Vector2(-12, -8), new Rectangle(0, 48, 24, 16));
            AddRect(new Vector2(-12, -8), new Rectangle(0, 64, 24, 16));
            AddRect(new Vector2(-12, -8), new Rectangle(0, 80, 24, 16));
            AddRect(new Vector2(-12, -8), new Rectangle(0, 96, 24, 16));
            AddRect(new Vector2(-12, -8), new Rectangle(0, 112, 24, 16));
            AddRect(new Vector2(-12, -8), new Rectangle(0, 128, 24, 16));
            AddRect(new Vector2(-12, -8), new Rectangle(0, 144, 24, 16));
            AddRect(new Vector2(-12, -8), new Rectangle(0, 160, 24, 16));
            AddRect(new Vector2(-12, -8), new Rectangle(0, 176, 24, 16));

			// 장소用인물とかの아이콘
            AddRect(new Vector2(-8, -8), new Rectangle(24, 48, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(40, 48, 16, 16));

            AddRect(new Vector2(-8, -8), new Rectangle(200, 16, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(216, 16, 16, 16));
	
			// 메모用아이콘
			
		    //   풍향
            AddRect(new Vector2(-6, -6), new Rectangle(64, 192, 12, 12));
            AddRect(new Vector2(-6, -6), new Rectangle(80, 192, 12, 12));
            AddRect(new Vector2(-6, -6), new Rectangle(96, 192, 12, 12));
            AddRect(new Vector2(-6, -6), new Rectangle(112, 192, 12, 12));
            AddRect(new Vector2(-6, -6), new Rectangle(128, 192, 12, 12));
            AddRect(new Vector2(-6, -6), new Rectangle(144, 192, 12, 12));
            AddRect(new Vector2(-6, -6), new Rectangle(160, 192, 12, 12));
            AddRect(new Vector2(-6, -6), new Rectangle(176, 192, 12, 12));
            
            //   재해아이콘
            AddRect(new Vector2(-8, -8), new Rectangle(24, 64, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(40, 64, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(56, 64, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(72, 64, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(88, 64, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(104, 64, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(120, 64, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(136, 64, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(152, 64, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(168, 64, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(184, 64, 16, 16));
            AddRect(new Vector2(-8, -13), new Rectangle(201, 65, 15, 15));// 목적지のみ特殊

			// web icon
			//   풍향
            AddRect(new Vector2(-6, -6), new Rectangle(64, 192, 12, 12));
            AddRect(new Vector2(-6, -6), new Rectangle(80, 192, 12, 12));
            AddRect(new Vector2(-6, -6), new Rectangle(96, 192, 12, 12));
            AddRect(new Vector2(-6, -6), new Rectangle(112, 192, 12, 12));
            AddRect(new Vector2(-6, -6), new Rectangle(128, 192, 12, 12));
            AddRect(new Vector2(-6, -6), new Rectangle(144, 192, 12, 12));
            AddRect(new Vector2(-6, -6), new Rectangle(160, 192, 12, 12));
            AddRect(new Vector2(-6, -6), new Rectangle(176, 192, 12, 12));
            
            //   재해아이콘
            AddRect(new Vector2(-7, -7), new Rectangle(88, 168, 14, 14));
            AddRect(new Vector2(-7, -7), new Rectangle(104, 168, 14, 14));
            AddRect(new Vector2(-7, -7), new Rectangle(120, 168, 14, 14));
            AddRect(new Vector2(-7, -7), new Rectangle(136, 168, 14, 14));
            AddRect(new Vector2(-7, -7), new Rectangle(152, 168, 14, 14));

			// 재해아이콘
            AddRect(new Vector2(-8, -32), new Rectangle(24, 80, 16, 15));
            AddRect(new Vector2(-8, -32), new Rectangle(40, 80, 16, 15));
            AddRect(new Vector2(-8, -32), new Rectangle(56, 80, 16, 15));
            AddRect(new Vector2(-8, -32), new Rectangle(72, 80, 16, 15));
            AddRect(new Vector2(-8, -32), new Rectangle(88, 80, 16, 15));
            AddRect(new Vector2(-8, -32), new Rectangle(104, 80, 16, 15));
            AddRect(new Vector2(-8, -32), new Rectangle(120, 80, 16, 15));
            AddRect(new Vector2(-8, -32), new Rectangle(136, 80, 16, 15));
            AddRect(new Vector2(-8, -32), new Rectangle(152, 80, 16, 15));
            AddRect(new Vector2(-8, -32), new Rectangle(168, 80, 16, 15));
            AddRect(new Vector2(-8, -32), new Rectangle(184, 80, 16, 15));

            AddRect(new Vector2(-14, -33), new Rectangle(152, 96, 30, 34));

            AddRect(new Vector2(-3, -17), new Rectangle(48, 168, 30, 18));

            // 장소用タブ아이콘
            AddRect(new Vector2(-8, -8), new Rectangle(24, 16, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(40, 16, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(56, 16, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(72, 16, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(88, 16, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(144, 208, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(104, 16, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(120, 16, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(136, 16, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(152, 16, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(168, 16, 16, 16));
            AddRect(new Vector2(-8, -8), new Rectangle(184, 16, 16, 16));

			// 항로공유 도시に居る
            AddRect(new Vector2(-4, -4), new Rectangle(80, 160, 10, 10));

			// 도시, 교외
            AddRect(new Vector2(-11, -18), new Rectangle(232, 160, 24, 26));
            AddRect(new Vector2(-6, -11), new Rectangle(232, 192, 14, 16));
            AddRect(new Vector2(-10, -12), new Rectangle(232, 216, 22, 20));
            AddRect(new Vector2(-6, -11), new Rectangle(232, 240, 14, 16));

            AddRect(new Vector2(-5, -5), new Rectangle(240, 96, 10, 10));
            AddRect(new Vector2(-5, -5), new Rectangle(240, 112, 10, 10));
            AddRect(new Vector2(-4, -4), new Rectangle(240, 128, 8, 8));
            AddRect(new Vector2(-4, -4), new Rectangle(240, 144, 8, 8));
            
            AddRect(new Vector2(0, 0), new Rectangle(168, 168, 16, 16));
            AddRect(new Vector2(0, 0), new Rectangle(208, 144, 12, 12));

			// スクリプトにより切りだされたもの
            AddRect(new Vector2(-3, -5), new Rectangle(32, 184, 6, 6));
            AddRect(new Vector2(-8, -19), new Rectangle(0, 192, 18, 20));
            AddRect(new Vector2(-12, -19), new Rectangle(24, 192, 26, 20));
            AddRect(new Vector2(0, 0), new Rectangle(224, 144, 10, 12));
            AddRect(new Vector2(0, 0), new Rectangle(0, 216, 48, 12));
            AddRect(new Vector2(0, 0), new Rectangle(48, 216, 42, 12));
            AddRect(new Vector2(0, 0), new Rectangle(200, 192, 24, 12));
            AddRect(new Vector2(0, 0), new Rectangle(200, 208, 24, 12));
            AddRect(new Vector2(0, 0), new Rectangle(208, 160, 12, 12));
            AddRect(new Vector2(0, 0), new Rectangle(208, 176, 12, 12));
            AddRect(new Vector2(0, 0), new Rectangle(96, 216, 44, 12));
		}
	
		/*-------------------------------------------------------------------------
		 矩形を得る
		---------------------------------------------------------------------------*/
		public d3d_sprite_rects.rect GetIcon(icon_index index)
		{
			return GetRect((int)index);
		}
	}
}
