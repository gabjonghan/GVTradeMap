/*-------------------------------------------------------------------------

 아이콘관리

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
namespace gvtrademap_cs {
	public class icons : d3d_sprite_rects {
		public enum icon_index {
			// 일付変わり時の말풍선아이콘
			days_big_shadow,	// 影

			// 일付표시용수字
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

			// 국아이콘
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

			// 스크린샷と항로도삭제はグレイ표시없음
			setting_10,
			setting_11,
			setting_11_1,

			// 5일아이콘
			setting_12,

			// 장소
			spot_0,
			spot_1,
			spot_2,

			// 상륙지점の지도
			map_icon,

			// 선택용三角
			select_0,
			select_1,

			// 선택용クロスカーソル
			select_cross,

			// 설정ボタン
			setting_button,

			// 속도표시용背景
			speed_background,

			// 장소용국아이콘
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

			// 메모용아이콘
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

			// 메모용아이콘
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

			// 장소용 탭 아이콘
			spot_tab_0,	 // 150번째
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

			// 스크립트로부터 잘라낸 것
			// 일付変わり時の아이콘
			days_mini_6,
			// 일付変わり時の말풍선아이콘
			days_big_6,
			days_big_100,	   // 100일以降の좌우が대きいもの

			string00,		   // 일
			string01,		   // 항로공유
			string02,		   // 이자まで
			string03,		   // 季節
			string04,		   // 지도
			string05,		   // 冬
			string06,		   // 夏
			string07,		   // 조선から

			// 지도용 본거지 항구 상륙지 2차필드 아이콘
			map_capital,	// 185번째 인덱스
			map_capital_islam,	// 186번째 인덱스
			map_city_territory,	// 187번째 인덱스
			map_city,	// 188번째 인덱스
			map_city_islam_territory,	// 189번째 인덱스
			map_city_islam,	// 190번째 인덱스
			map_red_rect,	// 191번째 인덱스
			map_cyan_rect,	// 192번째 인덱스
			map_yellow_rect,	// 193번째 인덱스
			map_red_circle,	// 194번째 인덱스
			map_cyan_circle,	// 195번째 인덱스
			map_yellow_circle,	// 196번째 인덱스
			map_magenta_rect,	// 197번째 인덱스
			map_blue_rect,	// 198번째 인덱스
			map_orange_rect,	// 199번째 인덱스
			map_magenta_circle,	// 200번째 인덱스
			map_blue_circle,	// 201번째 인덱스
			map_orange_circle,	// 202번째 인덱스
			map_white_rect,	// 203번째 인덱스
			map_green_rect,	// 204번째 인덱스
			map_lime_rect,	// 205번째 인덱스
			map_white_circle,	// 206번째 인덱스
			map_green_circle,	// 207번째 인덱스
			map_lime_circle,	// 208번째 인덱스
			
			wind_arrow,			// 풍향

			max
		};

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public icons(d3d_device device, string fname)
			: base(device, fname) {
			if (device.device == null) return;

			// 矩形등록
			add_rects();
		}

		/*-------------------------------------------------------------------------
		 矩形등록
		 icon_indexの順で할당る
		---------------------------------------------------------------------------*/
		private void add_rects() {
			// 影
			AddRect(new Vector2(-5, -6 - 8), new Rectangle(24, 160, 23, 15));

			// 일수 표시용 숫자
			AddRect(new Vector2(-3, -16), new Rectangle(0, 232, 6, 9));	 // 0
			AddRect(new Vector2(-3, -16), new Rectangle(8, 232, 6, 9));
			AddRect(new Vector2(-3, -16), new Rectangle(16, 232, 6, 9));
			AddRect(new Vector2(-3, -16), new Rectangle(24, 232, 6, 9));
			AddRect(new Vector2(-3, -16), new Rectangle(32, 232, 6, 9));
			AddRect(new Vector2(-3, -16), new Rectangle(40, 232, 6, 9));
			AddRect(new Vector2(-3, -16), new Rectangle(48, 232, 6, 9));
			AddRect(new Vector2(-3, -16), new Rectangle(56, 232, 6, 9));
			AddRect(new Vector2(-3, -16), new Rectangle(64, 232, 6, 9));
			AddRect(new Vector2(-3, -16), new Rectangle(72, 232, 6, 9));	// 9

			// 배
			AddRect(new Vector2(-6, -9), new Rectangle(216, 32, 12, 11));

			// 국가아이콘
			AddRect(new Vector2(0, 0), new Rectangle(0, 0, 24, 16));		// 개인농장
			AddRect(new Vector2(0, 0), new Rectangle(0, 16, 24, 16));	   // 2차필드
			AddRect(new Vector2(0, 0), new Rectangle(0, 32, 24, 16));	   // 해역
			AddRect(new Vector2(0, 0), new Rectangle(0, 48, 24, 16));	   // 상륙지
			AddRect(new Vector2(0, 0), new Rectangle(0, 64, 24, 16));	   // 빈 아이콘
			AddRect(new Vector2(0, 0), new Rectangle(0, 80, 24, 16));	   // 영국
			AddRect(new Vector2(0, 0), new Rectangle(0, 96, 24, 16));	   // 스페인
			AddRect(new Vector2(0, 0), new Rectangle(0, 112, 24, 16));	  // 포르투갈
			AddRect(new Vector2(0, 0), new Rectangle(0, 128, 24, 16));	  // 네덜란드
			AddRect(new Vector2(0, 0), new Rectangle(0, 144, 24, 16));	  // 프랑스
			AddRect(new Vector2(0, 0), new Rectangle(0, 160, 24, 16));	  // 베네치아
			AddRect(new Vector2(0, 0), new Rectangle(0, 176, 24, 16));	  // 오스만투르크

			// 탭아이콘
			AddRect(new Vector2(1, 0), new Rectangle(25, 16, 15, 16));	  // 두번째줄 아이콘
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

			AddRect(new Vector2(1, 0), new Rectangle(25, 0, 15, 16));	   // 첫번째줄 아이콘
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
			AddRect(new Vector2(0, 0), new Rectangle(24, 48, 16, 16));  // 100번째 사각형
			AddRect(new Vector2(0, 0), new Rectangle(40, 48, 16, 16));  // 101번쨰 사각형

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

			// 5일아이콘
			AddRect(new Vector2(0, 0), new Rectangle(200, 48, 16, 16));

			// 장소
			AddRect(new Vector2(-20, -20), new Rectangle(25, 97, 40, 40));
			AddRect(new Vector2(-20, -20), new Rectangle(66, 97, 40, 40));
			AddRect(new Vector2(-20, -26), new Rectangle(107, 97, 40, 40));

			// web
			AddRect(new Vector2(0, 0), new Rectangle(232, 0, 17, 17));

			// 선택용三角
			AddRect(new Vector2(0, -3), new Rectangle(40, 144, 8, 8));
			AddRect(new Vector2(0, -3), new Rectangle(48, 144, 8, 8));

			// 선택용クロスカーソル
			AddRect(new Vector2(-6, -6), new Rectangle(24, 144, 14, 14));

			// 설정ボタン
			AddRect(new Vector2(0, 0), new Rectangle(128, 144, 48, 17));

			// 속도표시용背景
			AddRect(new Vector2(0, 0), new Rectangle(80, 144, 47, 16));

			// 장소용 국가아이콘
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

			// 장소용 인물이나の아이콘
			AddRect(new Vector2(-8, -8), new Rectangle(24, 48, 16, 16));
			AddRect(new Vector2(-8, -8), new Rectangle(40, 48, 16, 16));

			AddRect(new Vector2(-8, -8), new Rectangle(200, 16, 16, 16));
			AddRect(new Vector2(-8, -8), new Rectangle(216, 16, 16, 16));

			// 메모용아이콘

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

			// 장소용 탭 아이콘
			AddRect(new Vector2(-8, -8), new Rectangle(24, 16, 16, 16));		// 150번째 사각형
			AddRect(new Vector2(-8, -8), new Rectangle(40, 16, 16, 16));		// 151번째 사각형
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

			// Web
			AddRect(new Vector2(0, 0), new Rectangle(168, 168, 16, 16));
			// 도
			AddRect(new Vector2(0, 0), new Rectangle(208, 144, 12, 12));

			// 스크립트로부터 잘라낸 것
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

			// 지도용 본거지 항구 상륙지 2차필드 아이콘
			AddRect(new Vector2(-12, -22), new Rectangle(232, 160, 254 - 232, 185 - 160)); // 1 본거지, 184번째 사각형
			AddRect(new Vector2(-11, -14), new Rectangle(232, 216, 254 - 232, 235 - 216)); // 2 본거지(이슬람)
			AddRect(new Vector2(-7, -11), new Rectangle(232, 192, 245 - 232, 207 - 192)); // 3 도시 1
			AddRect(new Vector2(-5, -9), new Rectangle(203, 242, 212 - 203, 253 - 242)); // 4 도시 2
			AddRect(new Vector2(-7, -11), new Rectangle(232, 240, 245 - 232, 255 - 240)); // 5 도시 1 (이슬람)
			AddRect(new Vector2(-5, -9), new Rectangle(215, 242, 224 - 215, 253 - 242)); // 6 도시 2 (이슬람)
						
			// 도시 / 상륙지 등 아이콘
			AddRect(new Vector2(-2, -2), new Rectangle(179, 208, 6, 6));  // 7,  빨간색 사각형 
			AddRect(new Vector2(-2, -2), new Rectangle(179, 216, 6, 6));  // 8,  하늘색 사각형
			AddRect(new Vector2(-2, -2), new Rectangle(179, 224, 6, 6));  // 9,  노란색 사각형
			AddRect(new Vector2(-2, -2), new Rectangle(179, 232, 6, 6));  // 10, 빨간색 원
			AddRect(new Vector2(-2, -2), new Rectangle(179, 240, 6, 6));  // 11, 하늘색 원
			AddRect(new Vector2(-2, -2), new Rectangle(179, 248, 6, 6));  // 12, 노란색 원, 195번째
			AddRect(new Vector2(-2, -2), new Rectangle(187, 208, 6, 6));  // 13, 마젠타 사각형
			AddRect(new Vector2(-2, -2), new Rectangle(187, 216, 6, 6));  // 14, 파란색 사각형
			AddRect(new Vector2(-2, -2), new Rectangle(187, 224, 6, 6));  // 15, 주황색 사각형
			AddRect(new Vector2(-2, -2), new Rectangle(187, 232, 6, 6));  // 16, 마젠타 원
			AddRect(new Vector2(-2, -2), new Rectangle(187, 240, 6, 6));  // 17, 파란색 원
			AddRect(new Vector2(-2, -2), new Rectangle(187, 248, 6, 6));  // 18, 주황색 원
			AddRect(new Vector2(-2, -2), new Rectangle(195, 208, 6, 6));  // 19, 흰색 사각형
			AddRect(new Vector2(-2, -2), new Rectangle(195, 216, 6, 6));  // 20, 녹색 사각형
			AddRect(new Vector2(-2, -2), new Rectangle(195, 224, 6, 6));  // 21, 연두색 사각형
			AddRect(new Vector2(-2, -2), new Rectangle(195, 232, 6, 6));  // 22, 흰색 원
			AddRect(new Vector2(-2, -2), new Rectangle(195, 240, 6, 6));  // 23, 녹색 원
			AddRect(new Vector2(-2, -2), new Rectangle(195, 248, 6, 6));  // 24, 연두색 원

			AddRect(new Vector2(-4, -8), new Rectangle(210, 223, 10, 15)); // WindArrowIcon
		}

		/*-------------------------------------------------------------------------
		 矩形を得る
		---------------------------------------------------------------------------*/
		public d3d_sprite_rects.rect GetIcon(icon_index index) {
			return GetRect((int)index);
		}
	}
}
