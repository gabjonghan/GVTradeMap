/*-------------------------------------------------------------------------

 도시명などの絵정보

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
    public class infonameimage : d3d_sprite_rects
    {
        private const int ICON_START_INDEX = 0;
        private const int CITY_START_INDEX = 12;

        /*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/

        /*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/

        /*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
        public infonameimage(d3d_device device, string infoimage_fname)
            : base(device, infoimage_fname)
        {
            if (device.device == null) return;
            add_rects();
        }

        /*-------------------------------------------------------------------------
		 아이콘의 구형을 얻음
		---------------------------------------------------------------------------*/
        public d3d_sprite_rects.rect GetIcon(int index)
        {
            if (index < 0) return null;
            if ((ICON_START_INDEX + index) >= rect_count) return null;
            return GetRect(ICON_START_INDEX + index);
        }

        /*-------------------------------------------------------------------------
		 도시명의 구형을 얻음
		---------------------------------------------------------------------------*/
        public d3d_sprite_rects.rect GetCityName(int index)
        {
            if (index < 0) return null;
            if ((CITY_START_INDEX + index) >= rect_count) return null;
            return GetRect(CITY_START_INDEX + index);
        }

        /*-------------------------------------------------------------------------
		 切り取り矩形の追加
		---------------------------------------------------------------------------*/
        private void add_rects()
        {
            // 스크립트에서 생성
            // infonames.dds 와 매핑됨
            // 본거지 항구 상륙지 2차필드 아이콘
            AddRect(new Vector2(-12, -22), new Rectangle(168, 12, 192 - 168, 38 - 12)); // 0
            AddRect(new Vector2(-11, -14), new Rectangle(168, 40, 192 - 168, 60 - 40)); // 1
            AddRect(new Vector2(-7, -11), new Rectangle(72, 60, 86 - 72, 76 - 60)); // 2
            AddRect(new Vector2(-5, -9), new Rectangle(156, 0, 166 - 156, 12 - 0)); // 3
            AddRect(new Vector2(-7, -11), new Rectangle(72, 36, 86 - 72, 52 - 36)); // 4
            AddRect(new Vector2(-5, -9), new Rectangle(168, 0, 178 - 168, 12 - 0)); // 5
            AddRect(new Vector2(-2, -2), new Rectangle(176, 64, 182 - 176, 70 - 64));   // 6
            AddRect(new Vector2(-2, -2), new Rectangle(176, 72, 182 - 176, 78 - 72));   // 7
            AddRect(new Vector2(-2, -2), new Rectangle(176, 80, 182 - 176, 86 - 80));   // 8
            AddRect(new Vector2(-2, -2), new Rectangle(176, 88, 182 - 176, 94 - 88));   // 9
            AddRect(new Vector2(-2, -2), new Rectangle(176, 96, 182 - 176, 102 - 96));  // 10
            AddRect(new Vector2(-2, -2), new Rectangle(176, 104, 182 - 176, 110 - 104));    // 11

            // 도시 정보 (한자당 12.5픽셀, 13, 25, 38, 50, 63, 75, 88, 100)
            // 1열 0
            AddRect(new Vector2(0, 0), new Rectangle(0, 0, 63, 15));            // 0 암스테르담
            AddRect(new Vector2(-12, 0), new Rectangle(0, 16, 50, 15));         // 1 앤트워프
            AddRect(new Vector2(-28, -16), new Rectangle(0, 32, 38, 15));       // 2 뷔스비
            AddRect(new Vector2(0, -16), new Rectangle(0, 48, 38, 15));         // 3 오슬로 
            AddRect(new Vector2(-34, -16), new Rectangle(0, 64, 50, 15));       // 4 코펜하겐
            AddRect(new Vector2(-60, -18), new Rectangle(0, 80, 50, 15));        // 5 스톡홀름
            AddRect(new Vector2(0, 0), new Rectangle(0, 96, 38, 15));           // 6 단치히
            AddRect(new Vector2(-57, -16), new Rectangle(0, 112, 50, 15));      // 7 함부르크
            AddRect(new Vector2(0, 0), new Rectangle(0, 128, 38, 15));          // 8 브레멘
            AddRect(new Vector2(0, 0), new Rectangle(0, 144, 84, 15));      	// 9 그로닝겐
            AddRect(new Vector2(0, -8), new Rectangle(0, 160, 84, 15));     	// 10 베르겐
            AddRect(new Vector2(-8, -15), new Rectangle(0, 176, 84, 15));   	// 11 헤르데르
            AddRect(new Vector2(-14, 0), new Rectangle(0, 192, 84, 15));    	// 12 리가
            AddRect(new Vector2(-24, 0), new Rectangle(0, 208, 84, 15));    	// 13 뤼베크
            AddRect(new Vector2(0, -8), new Rectangle(0, 224, 84, 15));     	// 14 에딘버러
            AddRect(new Vector2(-41, -15), new Rectangle(0, 240, 84, 15));  	// 15 더블린
            AddRect(new Vector2(-20f, 4f), new Rectangle(0, 256, 25, 15));		// 16 도버
            AddRect(new Vector2(-50, -10), new Rectangle(0, 272, 50, 15));  	// 17 플리머스
            AddRect(new Vector2(-28, -15), new Rectangle(0, 288, 25, 15));  	// 18 런던
            AddRect(new Vector2(0, 0), new Rectangle(0, 304, 84, 15));       	// 19 칼레
            AddRect(new Vector2(0, -16), new Rectangle(0, 320, 84, 15));    	// 20 낭트
            AddRect(new Vector2(0, 0), new Rectangle(0, 336, 84, 15));          // 21 보르도
            AddRect(new Vector2(0, -8), new Rectangle(0, 352, 84, 15));         // 22 포르투
            AddRect(new Vector2(-11, 0), new Rectangle(0, 368, 84, 15));    	// 23 카사블랑카
            AddRect(new Vector2(-20, 0), new Rectangle(0, 384, 84, 15));    	// 24 세우타
            AddRect(new Vector2(0, -16), new Rectangle(0, 400, 84, 15));    	// 25 세비야
            AddRect(new Vector2(-28, -16), new Rectangle(0, 416, 84, 15));  	// 26 바르셀로나
            AddRect(new Vector2(-18, -16), new Rectangle(0, 432, 84, 15));  	// 27 팔마
            AddRect(new Vector2(-28, -16), new Rectangle(0, 448, 84, 15));  	// 28 발렌시아
            AddRect(new Vector2(-18, -1), new Rectangle(0, 464, 84, 15));   	// 29 히혼
            AddRect(new Vector2(-2, -14), new Rectangle(0, 480, 84, 15));   	// 30 파루
            AddRect(new Vector2(-22, 0), new Rectangle(0, 496, 84, 15));    	// 31 마데이라
            AddRect(new Vector2(-1, -15), new Rectangle(0, 512, 84, 15));   	// 32 말라가
            AddRect(new Vector2(-28, -16), new Rectangle(0, 528, 84, 15));  	// 33 몽펠리에
            AddRect(new Vector2(-30, -1), new Rectangle(0, 544, 84, 15));   	// 34 라스팔마스
            AddRect(new Vector2(-1, -15), new Rectangle(0, 560, 84, 15));   	// 35 리스본
            AddRect(new Vector2(-44, -1), new Rectangle(0, 576, 84, 15));   	// 36 안코나
            AddRect(new Vector2(-34, -15), new Rectangle(0, 592, 84, 15));  	// 37 베네치아
            AddRect(new Vector2(-1, -8), new Rectangle(0, 608, 84, 15));    	// 38 칼리아리
            AddRect(new Vector2(0, 0), new Rectangle(0, 624, 84, 15));      	// 39 칼비
            AddRect(new Vector2(0, -8), new Rectangle(0, 640, 84, 15));     	// 40 자다르
            AddRect(new Vector2(-45, -8), new Rectangle(0, 656, 84, 15));   	// 41 사사리
            AddRect(new Vector2(-26, -16), new Rectangle(0, 672, 84, 15));  	// 42 제노바
            AddRect(new Vector2(0, 0), new Rectangle(0, 688, 84, 15));      	// 43 시라쿠사
            AddRect(new Vector2(0, -8), new Rectangle(0, 704, 84, 15));     	// 44 트리에스테
            AddRect(new Vector2(-30, 0), new Rectangle(0, 720, 84, 15));    	// 45 나폴리
            AddRect(new Vector2(-14, -15), new Rectangle(0, 736, 84, 15));  	// 46 피사
            AddRect(new Vector2(-28, -16), new Rectangle(0, 752, 84, 15));  	// 47 마르세이유
            AddRect(new Vector2(-1, -8), new Rectangle(0, 768, 84, 15));    	// 48 라구사
            AddRect(new Vector2(-40, -18), new Rectangle(0, 784, 84, 15));    	// 49 아테네
            AddRect(new Vector2(-26, -1), new Rectangle(0, 800, 84, 15));   	// 50 간디아
            AddRect(new Vector2(-41, -14), new Rectangle(0, 816, 84, 15));  	// 51 살로니카
            AddRect(new Vector2(-50, -1), new Rectangle(0, 832, 84, 15));   	// 52 파마구스타
            AddRect(new Vector2(-23, 0), new Rectangle(0, 848, 84, 15));    	// 53 알제
            AddRect(new Vector2(-83, -1), new Rectangle(0, 864, 75, 15));   	// 54 알렉산드리아
            AddRect(new Vector2(-67, -8), new Rectangle(0, 880, 84, 15));   	// 55 이스탄불
            AddRect(new Vector2(-49, -8), new Rectangle(0, 896, 84, 15));   	// 56 오데사
            AddRect(new Vector2(0, -8), new Rectangle(0, 912, 84, 15));     	// 57 카이로
            AddRect(new Vector2(0, -8), new Rectangle(0, 928, 84, 15));     	// 58 카파
            AddRect(new Vector2(-34, -15), new Rectangle(0, 944, 84, 15));  	// 59 세바스토폴
            AddRect(new Vector2(-44, -2), new Rectangle(0, 960, 84, 15));   	// 60 튀니스
            AddRect(new Vector2(-22, 0), new Rectangle(0, 976, 84, 15));    	// 61 트리폴리
            AddRect(new Vector2(-36, 0), new Rectangle(0, 992, 84, 15));    	// 62 트레비존드
            AddRect(new Vector2(-1, -8), new Rectangle(0, 1008, 84, 15));   	// 63 베이루트

            // 2열 96
            AddRect(new Vector2(-1, -3), new Rectangle(96, 0, 38, 15));     	// 64 벵가지
            AddRect(new Vector2(-1, -8), new Rectangle(96, 16, 25, 15));    	// 65 야파
            AddRect(new Vector2(-24, -15), new Rectangle(96, 32, 50, 15));  	// 66 아조레스
            AddRect(new Vector2(-32, -22), new Rectangle(96, 48, 50, 15));   	// 67 산티아고
            AddRect(new Vector2(-20, 0), new Rectangle(96, 64, 63, 15));  	    // 68 산토도밍고
            AddRect(new Vector2(5, 7), new Rectangle(96, 80, 38, 15));        	// 69 산후안
            AddRect(new Vector2(0, -10), new Rectangle(96, 96, 50, 15));      	// 70 자메이카
            AddRect(new Vector2(-18, -15), new Rectangle(96, 112, 38, 15)); 	// 71 하바나
            AddRect(new Vector2(-60, -8), new Rectangle(96, 128, 63, 15));  	// 72 포르토벨로
            AddRect(new Vector2(-7, -13), new Rectangle(96, 144, 63, 15));  	// 73 윌렘스타트
            AddRect(new Vector2(-26, 0), new Rectangle(96, 160, 84, 15));   	// 74 카라카스
            AddRect(new Vector2(-30, 0), new Rectangle(96, 176, 84, 15));   	// 75 트루히요
            AddRect(new Vector2(-2, -8), new Rectangle(96, 192, 84, 15));   	// 76 나소
            AddRect(new Vector2(-56, -14), new Rectangle(96, 208, 84, 15)); 	// 77 마라카이보
            AddRect(new Vector2(-34, -14), new Rectangle(96, 224, 84, 15)); 	// 78 베라크루스
            AddRect(new Vector2(-18, -15), new Rectangle(96, 240, 84, 15)); 	// 79 메리다
            AddRect(new Vector2(-38, -2), new Rectangle(96, 256, 84, 15));  	// 80 그랜드케이맨
            AddRect(new Vector2(-57, -1), new Rectangle(96, 272, 25, 15));  	// 81 카옌
            AddRect(new Vector2(-51, -8), new Rectangle(96, 288, 84, 15));  	// 82 바이아
            AddRect(new Vector2(-72, -8), new Rectangle(96, 304, 84, 15));  	// 83 페르남부쿠
            AddRect(new Vector2(-86, -13), new Rectangle(96, 320, 84, 15)); 	// 84 리우데자네이루
            AddRect(new Vector2(-48, -15), new Rectangle(96, 336, 84, 15)); 	// 85 부에노스아이레스
            AddRect(new Vector2(-83, -8), new Rectangle(96, 352, 84, 15));  	// 86 산안토니오
            AddRect(new Vector2(-30, -15), new Rectangle(96, 368, 84, 15)); 	// 87 아비장
            AddRect(new Vector2(-1, -8), new Rectangle(96, 384, 84, 15));   	// 88 아르긴
            AddRect(new Vector2(-36, -1), new Rectangle(96, 400, 84, 15));  	// 89 카보베르데
            AddRect(new Vector2(-1, -8), new Rectangle(96, 416, 84, 15));   	// 90 카리비브
            AddRect(new Vector2(-1, -8), new Rectangle(96, 432, 84, 15));   	// 91 케이프타운
            AddRect(new Vector2(-44, -15), new Rectangle(96, 448, 84, 15)); 	// 92 세인트조지스
            AddRect(new Vector2(-26, -1), new Rectangle(96, 464, 84, 15));  	// 93 상투메
            AddRect(new Vector2(-1, -8), new Rectangle(96, 480, 84, 15));   	// 94 시에라리온
            AddRect(new Vector2(-2, -8), new Rectangle(96, 496, 84, 15));   	// 95 두알라
            AddRect(new Vector2(-1, -8), new Rectangle(96, 512, 84, 15));   	// 96 베냉
            AddRect(new Vector2(-1, -8), new Rectangle(96, 528, 84, 15));   	// 97 벵겔라
            AddRect(new Vector2(-1, -8), new Rectangle(96, 544, 84, 15));   	// 98 루안다
            AddRect(new Vector2(-39, -8), new Rectangle(96, 560, 84, 15));  	// 99 킬와
            AddRect(new Vector2(-1, -8), new Rectangle(96, 576, 84, 15));   	// 100 잔지바르
            AddRect(new Vector2(-50, -8), new Rectangle(96, 592, 84, 15));  	// 101 소팔라
            AddRect(new Vector2(-49, -8), new Rectangle(96, 608, 84, 15));  	// 102 타마타브
            AddRect(new Vector2(-49, -15), new Rectangle(96, 624, 84, 15)); 	// 103 나탈
            AddRect(new Vector2(-44, -15), new Rectangle(96, 640, 84, 15)); 	// 104 말린디
            AddRect(new Vector2(-70, -8), new Rectangle(96, 656, 84, 15));  	// 105 모가디슈
            AddRect(new Vector2(-2, -8), new Rectangle(96, 672, 84, 15));   	// 106 모잠비크
            AddRect(new Vector2(-49, -8), new Rectangle(96, 688, 84, 15));  	// 107 몸바사
            AddRect(new Vector2(-20, -15), new Rectangle(96, 704, 84, 15)); 	// 108 아덴
            AddRect(new Vector2(-1, -8), new Rectangle(96, 720, 84, 15));   	// 109 제다
            AddRect(new Vector2(-62, -14), new Rectangle(96, 736, 84, 15)); 	// 110 도파르
            AddRect(new Vector2(0, -8), new Rectangle(96, 752, 84, 15));    	// 111 수에즈
            AddRect(new Vector2(-37, -8), new Rectangle(96, 768, 84, 15));  	// 112 세이라
            AddRect(new Vector2(0, -15), new Rectangle(96, 784, 84, 15));   	// 113 소코트라
            AddRect(new Vector2(-57, -3), new Rectangle(96, 800, 84, 15));  	// 114 무스카트
            AddRect(new Vector2(-48, -8), new Rectangle(96, 816, 84, 15));  	// 115 마사와
            AddRect(new Vector2(-39, -8), new Rectangle(96, 832, 84, 15));  	// 116 바스라
            AddRect(new Vector2(-2, -15), new Rectangle(96, 848, 84, 15));  	// 117 호르무즈
            AddRect(new Vector2(-2, -14), new Rectangle(96, 864, 84, 15));  	// 118 캘리컷
            AddRect(new Vector2(-58, -12), new Rectangle(96, 880, 84, 15)); 	// 119 캘커타
            AddRect(new Vector2(0, -8), new Rectangle(96, 896, 84, 15));    	// 120 고아
            AddRect(new Vector2(-2, -14), new Rectangle(96, 912, 84, 15));  	// 121 코친
            AddRect(new Vector2(-50, -4), new Rectangle(96, 928, 84, 15));  	// 122 실론
            AddRect(new Vector2(-14, -15), new Rectangle(96, 944, 84, 15)); 	// 123 디우
            AddRect(new Vector2(-1, -8), new Rectangle(96, 960, 84, 15));   	// 124 퐁디셰리
            AddRect(new Vector2(-53, -14), new Rectangle(96, 976, 84, 15)); 	// 125 마술리파탐
            AddRect(new Vector2(-1, -1), new Rectangle(96, 992, 84, 15));   	// 126 아체
            AddRect(new Vector2(-25, -14), new Rectangle(96, 1008, 84, 15));	// 127 자카르타

            // 3열 200
            AddRect(new Vector2(-21, -14), new Rectangle(200, 0, 84, 15));  	// 128 수라바야
            AddRect(new Vector2(-20, -2), new Rectangle(200, 16, 84, 15));  	// 129 파타니
            AddRect(new Vector2(-51, -8), new Rectangle(200, 32, 84, 15));  	// 130 팔렘방
            AddRect(new Vector2(-45, -14), new Rectangle(200, 48, 84, 15)); 	// 131 반자르마신
            AddRect(new Vector2(-2, -12), new Rectangle(200, 64, 84, 15));  	// 132 페구
            AddRect(new Vector2(-30, -1), new Rectangle(200, 80, 84, 15));  	// 133 마카사르
            AddRect(new Vector2(-16, -14), new Rectangle(200, 96, 84, 15)); 	// 134 말라카
            AddRect(new Vector2(-30, -15), new Rectangle(200, 112, 84, 15));    // 135 지아딘
            AddRect(new Vector2(-2, -8), new Rectangle(200, 128, 84, 15));  	// 136 룬
            AddRect(new Vector2(-35, -9), new Rectangle(200, 144, 84, 15)); 	// 137 마닐라
            AddRect(new Vector2(-1, -2), new Rectangle(200, 160, 84, 15));  	// 138 브루나이
            AddRect(new Vector2(-2, -1), new Rectangle(200, 176, 84, 15));  	// 139 디바오
            AddRect(new Vector2(-30, -16), new Rectangle(200, 192, 84, 15));    // 140 로프부리
            AddRect(new Vector2(-22, -1), new Rectangle(200, 208, 84, 15)); 	// 141 쿠칭
            AddRect(new Vector2(-3, -15), new Rectangle(200, 224, 84, 15)); 	// 142 딜리
            AddRect(new Vector2(-39, -2), new Rectangle(200, 240, 84, 15)); 	// 143 잠비
            AddRect(new Vector2(-30, -1), new Rectangle(200, 256, 84, 15)); 	// 144 암보이나
            AddRect(new Vector2(-47, -3), new Rectangle(200, 272, 84, 15)); 	// 145 테르나테
            AddRect(new Vector2(-16, -14), new Rectangle(200, 288, 84, 15));    // 146 홀로
            AddRect(new Vector2(-26, 0), new Rectangle(200, 304, 84, 15));  	// 147 카카두
            AddRect(new Vector2(0, -8), new Rectangle(200, 320, 84, 15));   	// 148 호바트
            AddRect(new Vector2(-13, -15), new Rectangle(200, 336, 84, 15));    // 149 왕가누이
            AddRect(new Vector2(-37, -8), new Rectangle(200, 352, 84, 15)); 	// 150 쿠가리
            AddRect(new Vector2(-1, -8), new Rectangle(200, 368, 84, 15));  	// 151 핀자라
            AddRect(new Vector2(-1, -8), new Rectangle(200, 384, 84, 15));  	// 152 우수아이아
            AddRect(new Vector2(-32, -15), new Rectangle(200, 400, 84, 15));    // 153 아카풀코
            AddRect(new Vector2(-52, -14), new Rectangle(200, 416, 84, 15));    // 154 과테말라
            AddRect(new Vector2(-33, -12), new Rectangle(200, 432, 84, 15));    // 155 파나마
            AddRect(new Vector2(-2, -8), new Rectangle(200, 448, 84, 15));  	// 156 툼베스
            AddRect(new Vector2(-2, -8), new Rectangle(200, 464, 84, 15));  	// 157 람바예케
            AddRect(new Vector2(-2, -1), new Rectangle(200, 480, 84, 15));  	// 158 리마
            AddRect(new Vector2(-2, -8), new Rectangle(200, 496, 84, 15));  	// 159 코피아포
            AddRect(new Vector2(-2, -8), new Rectangle(200, 512, 84, 15));  	// 160 발파라이소
            AddRect(new Vector2(-20, -1), new Rectangle(200, 528, 84, 15)); 	// 161 괌
            AddRect(new Vector2(-26, -1), new Rectangle(200, 544, 84, 15)); 	// 162 사마라이
            AddRect(new Vector2(-28, -1), new Rectangle(200, 560, 84, 15)); 	// 163 히바오아
            AddRect(new Vector2(-32, 10), new Rectangle(200, 576, 84, 15)); 	// 164 나가사키
            AddRect(new Vector2(12, -10), new Rectangle(200, 592, 84, 15));     // 165 에도
            AddRect(new Vector2(-12, 17), new Rectangle(200, 608, 84, 15)); 	// 166 사카이
            AddRect(new Vector2(0, -8), new Rectangle(200, 624, 84, 15));   	// 167 포항
            AddRect(new Vector2(-25, -8), new Rectangle(200, 640, 84, 15)); 	// 168 부산
            AddRect(new Vector2(-13, -14), new Rectangle(200, 656, 84, 15));    // 169 한양
            AddRect(new Vector2(-38, -12), new Rectangle(200, 672, 84, 15));    // 170 운대산
            AddRect(new Vector2(-27, -8), new Rectangle(200, 688, 84, 15)); 	// 171 항주
            AddRect(new Vector2(-27, -14), new Rectangle(200, 704, 84, 15));    // 172 충칭
            AddRect(new Vector2(-26, -8), new Rectangle(200, 720, 84, 15)); 	// 173 천주
            AddRect(new Vector2(-14, -16), new Rectangle(200, 736, 84, 15));    // 174 마카오
            AddRect(new Vector2(-16, -16), new Rectangle(200, 752, 84, 15));    // 175 단수이
            AddRect(new Vector2(-16, -2), new Rectangle(200, 768, 84, 15)); 	// 176 안평

            AddRect(new Vector2(-70, 14), new Rectangle(200, 784, 63, 15)); 	// 177 누벨프랑스
            AddRect(new Vector2(-70, 0), new Rectangle(200, 800, 63, 15));      // 178 뉴잉글랜드
            AddRect(new Vector2(-80, -10), new Rectangle(200, 816, 63, 15));    // 179 뉴네덜란드
            AddRect(new Vector2(-70, -6), new Rectangle(200, 832, 50, 15)); 	// 180 버지니아
            AddRect(new Vector2(-68, -20), new Rectangle(200, 848, 63, 15));    // 181 캐롤라이나
            AddRect(new Vector2(-62, -18), new Rectangle(200, 864, 38, 15));	// 182 조지아
            AddRect(new Vector2(-58f, -26f), new Rectangle(200, 880, 50, 15)); 	// 183 플로리다
            AddRect(new Vector2(-50f, -23f), new Rectangle(200, 896, 63, 15));  // 184 루이지애나

            AddRect(new Vector2(3f, 3f), new Rectangle(200, 912, 25, 15)); 	    // 185 파리
            AddRect(new Vector2(-10f, -20f), new Rectangle(200, 928, 38, 15)); 	// 186 피렌체
            AddRect(new Vector2(-30f, -8f), new Rectangle(200, 944, 25, 15)); 	// 187 로마
            AddRect(new Vector2(5f, 0f), new Rectangle(200, 960, 50, 15)); 	    // 188 옥스포드

            AddRect(new Vector2(1, 1), new Rectangle(200, 976, 100, 15));   	// 189 상트페테르부르크
            AddRect(new Vector2(1, 1), new Rectangle(200, 992, 38, 15));    	// 190 코콜라
            AddRect(new Vector2(4, 5), new Rectangle(200, 1008, 38, 15));   	// 191 하와이

            // 4열 500
            AddRect(new Vector2(0, 0), new Rectangle(500, 0, 50, 15));      	// 192 나르비크
            AddRect(new Vector2(0, 0), new Rectangle(500, 16, 50, 15)); 	    // 193 만가제야
            AddRect(new Vector2(0, 0), new Rectangle(500, 32, 25, 15)); 	    // 194 틱시
            AddRect(new Vector2(0, 0), new Rectangle(500, 48, 75, 15)); 	    // 195 프랑크푸르트
            AddRect(new Vector2(0, 0), new Rectangle(500, 64, 100, 15)); 	    // 196 페트로파블롭스크
            AddRect(new Vector2(-50, 4), new Rectangle(500, 80, 50, 15)); 	    // 197 포트로얄

            AddRect(new Vector2(0, 0), new Rectangle(500, 96, 75, 15));      	// 198 샌프란시스코
            AddRect(new Vector2(0, 0), new Rectangle(500, 112, 38, 15)); 	    // 199 타코마
            AddRect(new Vector2(0, 0), new Rectangle(500, 128, 38, 15)); 	    // 200 시트카
            AddRect(new Vector2(0, 0), new Rectangle(500, 144, 38, 15)); 	    // 201 배로우
            AddRect(new Vector2(0, 0), new Rectangle(500, 160, 25, 15)); 	    // 202 처칠

            AddRect(new Vector2(-30f, -20f), new Rectangle(500, 176, 50, 15)); 	// 203 맨체스터
            AddRect(new Vector2(-55f, -5f), new Rectangle(500, 192, 50, 15)); 	// 204 포츠머스
            AddRect(new Vector2(-45f, -18f), new Rectangle(500, 208, 50, 15)); 	// 205 로테르담
            AddRect(new Vector2(-55f, -1f), new Rectangle(500, 224, 50, 15));   // 206 르아브르
            AddRect(new Vector2(3f, -15f), new Rectangle(500, 240, 38, 15)); 	// 207 빌바오
            AddRect(new Vector2(3f, -15f), new Rectangle(500, 256, 100, 15)); 	// 208 비아니두카스텔루
            AddRect(new Vector2(3f, -6f), new Rectangle(500, 272, 38, 15)); 	// 209 코토르

            // 개인농장
            AddRect(new Vector2(-52, 0), new Rectangle(500, 288, 100, 15));  	// 1 디에고가르시아섬
            AddRect(new Vector2(-31, -16), new Rectangle(500, 304, 50, 15));	// 2 사바이섬
            AddRect(new Vector2(0, -8), new Rectangle(500, 320, 88, 15));       // 3 세인트루시아섬
            AddRect(new Vector2(-38, -16), new Rectangle(500, 336, 50, 15));	// 4 어센션섬
            AddRect(new Vector2(3, 3), new Rectangle(500, 352, 63, 15));        // 5 어널래스카
            
            // 교외, 상륙지, 2차필드 (한자당 10픽셀, 14, 28, 42, 56, 70, 84, 98, 112) 
            AddRect(new Vector2(-19, 0), new Rectangle(300, 0, 344 - 288, 10 - 0)); // 0
            AddRect(new Vector2(0, -5), new Rectangle(300, 10, 344 - 288, 20 - 10));    // 1
            AddRect(new Vector2(-44, -10), new Rectangle(300, 20, 352 - 288, 30 - 20)); // 2
            AddRect(new Vector2(0, -5), new Rectangle(300, 30, 370 - 288, 40 - 30));    // 3
            AddRect(new Vector2(-5, -10), new Rectangle(300, 40, 334 - 288, 50 - 40));  // 4
            AddRect(new Vector2(-28, -10), new Rectangle(300, 50, 344 - 288, 60 - 50)); // 5
            AddRect(new Vector2(0, -5), new Rectangle(300, 60, 344 - 288, 70 - 60));    // 6
            AddRect(new Vector2(0, -5), new Rectangle(300, 70, 344 - 288, 80 - 70));    // 7
            AddRect(new Vector2(-31, 0), new Rectangle(300, 80, 350 - 288, 90 - 80));   // 8
            AddRect(new Vector2(0, -5), new Rectangle(300, 90, 342 - 288, 100 - 90));   // 9
            AddRect(new Vector2(0, -5), new Rectangle(300, 100, 352 - 288, 110 - 100)); // 10
            AddRect(new Vector2(-27, 0), new Rectangle(300, 110, 342 - 288, 120 - 110));    // 11
            AddRect(new Vector2(-9, 0), new Rectangle(300, 120, 350 - 288, 130 - 120)); // 12
            AddRect(new Vector2(-59, 0), new Rectangle(300, 130, 378 - 288, 140 - 130));    // 13
            AddRect(new Vector2(-15, -1), new Rectangle(300, 140, 336 - 288, 150 - 140));   // 14
            AddRect(new Vector2(0, -5), new Rectangle(300, 150, 330 - 288, 160 - 150)); // 15
            AddRect(new Vector2(-39, -2), new Rectangle(300, 160, 328 - 288, 170 - 160));   // 16
            AddRect(new Vector2(-24, -10), new Rectangle(300, 170, 336 - 288, 180 - 170));  // 17
            AddRect(new Vector2(-54, -5), new Rectangle(300, 180, 342 - 288, 190 - 180));   // 18
            AddRect(new Vector2(-45, -5), new Rectangle(300, 190, 334 - 288, 200 - 190));   // 19
            AddRect(new Vector2(0, -5), new Rectangle(300, 200, 330 - 288, 210 - 200)); // 20
            AddRect(new Vector2(0, -5), new Rectangle(300, 210, 324 - 288, 220 - 210)); // 21
            AddRect(new Vector2(-72, -5), new Rectangle(300, 220, 360 - 288, 230 - 220));   // 22
            AddRect(new Vector2(-33, 0), new Rectangle(300, 230, 344 - 288, 240 - 230));    // 23
            AddRect(new Vector2(-64, -10), new Rectangle(300, 240, 352 - 288, 250 - 240));  // 24
            AddRect(new Vector2(-23, 0), new Rectangle(300, 250, 334 - 288, 260 - 250));    // 25
            AddRect(new Vector2(-46, -5), new Rectangle(300, 260, 334 - 288, 270 - 260));   // 26
            AddRect(new Vector2(0, 0), new Rectangle(300, 270, 342 - 288, 280 - 270));  // 27
            AddRect(new Vector2(-32, -10), new Rectangle(300, 280, 352 - 288, 290 - 280));  // 28
            AddRect(new Vector2(0, -5), new Rectangle(300, 290, 370 - 288, 300 - 290)); // 29
            AddRect(new Vector2(0, -5), new Rectangle(300, 300, 352 - 288, 310 - 300)); // 30
            AddRect(new Vector2(0, -5), new Rectangle(300, 310, 360 - 288, 320 - 310)); // 31
            AddRect(new Vector2(-32, -10), new Rectangle(300, 320, 342 - 288, 330 - 320));  // 32
            AddRect(new Vector2(-72, -5), new Rectangle(300, 330, 360 - 288, 340 - 330));   // 33
            AddRect(new Vector2(-64, -5), new Rectangle(300, 340, 352 - 288, 350 - 340));   // 34
            AddRect(new Vector2(0, -4), new Rectangle(300, 350, 342 - 288, 370 - 350)); // 35
            AddRect(new Vector2(-90, -5), new Rectangle(300, 370, 378 - 288, 380 - 370));   // 36
            AddRect(new Vector2(-36, -5), new Rectangle(300, 380, 324 - 288, 390 - 380));   // 37
            AddRect(new Vector2(0, -5), new Rectangle(300, 390, 324 - 288, 400 - 390)); // 38
            AddRect(new Vector2(-72, -10), new Rectangle(300, 400, 360 - 288, 410 - 400));  // 39
            AddRect(new Vector2(-29, -10), new Rectangle(300, 410, 346 - 288, 420 - 410));  // 40
            AddRect(new Vector2(0, -5), new Rectangle(300, 420, 360 - 288, 430 - 420)); // 41
            AddRect(new Vector2(0, -5), new Rectangle(300, 430, 334 - 288, 440 - 430)); // 42
            AddRect(new Vector2(0, -10), new Rectangle(300, 440, 360 - 288, 450 - 440));    // 43
            AddRect(new Vector2(0, -10), new Rectangle(300, 450, 334 - 288, 470 - 450));    // 44
            AddRect(new Vector2(-25, -10), new Rectangle(300, 470, 338 - 288, 480 - 470));  // 45
            AddRect(new Vector2(0, 0), new Rectangle(300, 480, 346 - 288, 490 - 480));  // 46
            AddRect(new Vector2(0, -5), new Rectangle(300, 490, 378 - 288, 500 - 490)); // 47
            AddRect(new Vector2(-32, 0), new Rectangle(300, 500, 352 - 288, 510 - 500));    // 48
            AddRect(new Vector2(0, -5), new Rectangle(300, 510, 334 - 288, 520 - 510)); // 49
            AddRect(new Vector2(-41, -10), new Rectangle(300, 520, 370 - 288, 530 - 520));  // 50
            AddRect(new Vector2(0, -5), new Rectangle(300, 530, 334 - 288, 540 - 530)); // 51
            AddRect(new Vector2(0, -5), new Rectangle(300, 540, 326 - 288, 550 - 540)); // 52
            AddRect(new Vector2(-26, 0), new Rectangle(300, 550, 360 - 288, 560 - 550));    // 53
            AddRect(new Vector2(-46, -5), new Rectangle(300, 560, 334 - 288, 570 - 560));   // 54
            AddRect(new Vector2(0, -5), new Rectangle(300, 570, 324 - 288, 580 - 570)); // 55
            AddRect(new Vector2(0, -5), new Rectangle(300, 580, 326 - 288, 590 - 580));	// 56
            AddRect(new Vector2(-24f, -16f), new Rectangle(300, 590, 64, 10));  // 57
            AddRect(new Vector2(0, -5), new Rectangle(300, 600, 330 - 288, 610 - 600)); // 58
            AddRect(new Vector2(0, -5), new Rectangle(300, 610, 344 - 288, 620 - 610)); // 59
            AddRect(new Vector2(0, -5), new Rectangle(300, 620, 352 - 288, 630 - 620)); // 60
            AddRect(new Vector2(-42, -5), new Rectangle(300, 630, 330 - 288, 640 - 630));   // 61
            AddRect(new Vector2(-34, -5), new Rectangle(300, 640, 322 - 288, 650 - 640));   // 62
            AddRect(new Vector2(0, -5), new Rectangle(300, 650, 338 - 288, 660 - 650)); // 63
            AddRect(new Vector2(0, -5), new Rectangle(300, 660, 344 - 288, 670 - 660)); // 64
            AddRect(new Vector2(0, -5), new Rectangle(300, 670, 344 - 288, 680 - 670)); // 65
            AddRect(new Vector2(0, 0), new Rectangle(300, 680, 330 - 288, 690 - 680));  // 66
            AddRect(new Vector2(0, -5), new Rectangle(300, 690, 332 - 288, 700 - 690)); // 67
            AddRect(new Vector2(0, -5), new Rectangle(300, 700, 352 - 288, 710 - 700)); // 68
            AddRect(new Vector2(-64, -5), new Rectangle(300, 710, 352 - 288, 720 - 710));   // 69
            AddRect(new Vector2(-37, -10), new Rectangle(300, 720, 362 - 288, 730 - 720));  // 70
            AddRect(new Vector2(0, -10), new Rectangle(300, 730, 326 - 288, 740 - 730));    // 71
            AddRect(new Vector2(0, -5), new Rectangle(300, 740, 334 - 288, 750 - 740)); // 72
            AddRect(new Vector2(-46, -5), new Rectangle(300, 750, 334 - 288, 760 - 750));   // 73
            AddRect(new Vector2(-56, -5), new Rectangle(300, 760, 344 - 288, 770 - 760));   // 74
            AddRect(new Vector2(0, -5), new Rectangle(300, 770, 334 - 288, 780 - 770)); // 75
            AddRect(new Vector2(-8, -10), new Rectangle(300, 780, 338 - 288, 790 - 780));   // 76
            AddRect(new Vector2(0, -5), new Rectangle(300, 790, 336 - 288, 800 - 790)); // 77
            AddRect(new Vector2(0, -5), new Rectangle(300, 800, 362 - 288, 810 - 800)); // 78
            AddRect(new Vector2(-64, -5), new Rectangle(300, 810, 352 - 288, 820 - 810));   // 79
            AddRect(new Vector2(-1, -15), new Rectangle(300, 820, 318 - 288, 840 - 820));   // 80
            AddRect(new Vector2(0, -5), new Rectangle(300, 840, 326 - 288, 850 - 840)); // 81
            AddRect(new Vector2(0, -5), new Rectangle(300, 850, 346 - 288, 860 - 850)); // 82
            AddRect(new Vector2(-28, -1), new Rectangle(300, 860, 344 - 288, 870 - 860));   // 83
            AddRect(new Vector2(-23, -10), new Rectangle(300, 870, 344 - 288, 880 - 870));  // 84
            AddRect(new Vector2(0, -10), new Rectangle(300, 880, 342 - 288, 890 - 880));    // 85
            AddRect(new Vector2(-34, -10), new Rectangle(300, 890, 334 - 288, 900 - 890));  // 86
            AddRect(new Vector2(0, -5), new Rectangle(300, 900, 344 - 288, 910 - 900)); // 87
            AddRect(new Vector2(-28, -5), new Rectangle(300, 910, 316 - 288, 920 - 910));   // 88
            AddRect(new Vector2(0, -5), new Rectangle(300, 920, 324 - 288, 930 - 920)); // 89
            AddRect(new Vector2(-58, -5), new Rectangle(300, 930, 346 - 288, 940 - 930));   // 90
            AddRect(new Vector2(-15, -2), new Rectangle(300, 940, 318 - 288, 960 - 940));   // 91 파나마 북동 パナマ北東

            AddRect(new Vector2(-72, -5), new Rectangle(400, 0, 452 - 380, 10 - 0));    // 92
            AddRect(new Vector2(-20, 0), new Rectangle(400, 10, 420 - 380, 20 - 10));   // 93
            AddRect(new Vector2(0, -5), new Rectangle(400, 20, 452 - 380, 30 - 20));    // 94
            AddRect(new Vector2(-42, -16), new Rectangle(400, 30, 428 - 380, 50 - 30)); // 95
            AddRect(new Vector2(0, -5), new Rectangle(400, 50, 436 - 380, 60 - 50));    // 96
            AddRect(new Vector2(-56, -2), new Rectangle(400, 60, 436 - 380, 70 - 60));  // 97
            AddRect(new Vector2(-31, -10), new Rectangle(400, 70, 428 - 380, 80 - 70)); // 98
            AddRect(new Vector2(0, -5), new Rectangle(400, 80, 434 - 380, 90 - 80));    // 99
            AddRect(new Vector2(0, -5), new Rectangle(400, 90, 436 - 380, 100 - 90));   // 100
            AddRect(new Vector2(-27, -1), new Rectangle(400, 100, 434 - 380, 110 - 100));   // 101
            AddRect(new Vector2(0, -1), new Rectangle(400, 110, 424 - 380, 120 - 110)); // 102
            AddRect(new Vector2(0, -5), new Rectangle(400, 120, 442 - 380, 130 - 120)); // 103
            AddRect(new Vector2(0, -5), new Rectangle(400, 130, 434 - 380, 140 - 130)); // 104
            AddRect(new Vector2(-18, 0), new Rectangle(400, 140, 416 - 380, 150 - 140));    // 105
            AddRect(new Vector2(-56, -5), new Rectangle(400, 150, 436 - 380, 160 - 150));   // 106
            AddRect(new Vector2(-56, -5), new Rectangle(400, 160, 436 - 380, 170 - 160));   // 107
            AddRect(new Vector2(-6, -2), new Rectangle(400, 170, 444 - 380, 180 - 170));    // 108
            AddRect(new Vector2(-64, -5), new Rectangle(400, 180, 444 - 380, 190 - 180));   // 109
            AddRect(new Vector2(-1, -2), new Rectangle(400, 190, 446 - 380, 200 - 190));    // 110
            AddRect(new Vector2(-36, -5), new Rectangle(400, 200, 416 - 380, 210 - 200));   // 111
            AddRect(new Vector2(0, -5), new Rectangle(400, 210, 444 - 380, 220 - 210)); // 112
            AddRect(new Vector2(-64, -5), new Rectangle(400, 220, 444 - 380, 230 - 220));   // 113
            AddRect(new Vector2(-64, -5), new Rectangle(400, 230, 444 - 380, 240 - 230));   // 114
            AddRect(new Vector2(-36, -10), new Rectangle(400, 240, 452 - 380, 250 - 240));  // 115
            AddRect(new Vector2(0, -5), new Rectangle(400, 250, 426 - 380, 260 - 250)); // 116
            AddRect(new Vector2(0, -5), new Rectangle(400, 260, 416 - 380, 270 - 260)); // 117
            AddRect(new Vector2(0, -5), new Rectangle(400, 270, 470 - 380, 280 - 270)); // 118
            AddRect(new Vector2(-25, 0), new Rectangle(400, 280, 428 - 380, 290 - 280));    // 119
            AddRect(new Vector2(0, -5), new Rectangle(400, 290, 460 - 380, 300 - 290)); // 120
            AddRect(new Vector2(0, -5), new Rectangle(400, 300, 426 - 380, 310 - 300)); // 121
            AddRect(new Vector2(-31, -10), new Rectangle(400, 310, 442 - 380, 320 - 310));  // 122
            AddRect(new Vector2(-26, -5), new Rectangle(400, 320, 406 - 380, 330 - 320));   // 123
            AddRect(new Vector2(0, -5), new Rectangle(400, 330, 426 - 380, 340 - 330)); // 124
            AddRect(new Vector2(0, -5), new Rectangle(400, 340, 434 - 380, 350 - 340)); // 125
            AddRect(new Vector2(-21, -1), new Rectangle(400, 350, 422 - 380, 360 - 350));   // 126
            AddRect(new Vector2(0, -5), new Rectangle(400, 360, 426 - 380, 370 - 360)); // 127
            AddRect(new Vector2(-37, 0), new Rectangle(400, 370, 454 - 380, 380 - 370));    // 128
            AddRect(new Vector2(-37, -10), new Rectangle(400, 380, 454 - 380, 390 - 380));  // 129
            AddRect(new Vector2(0, 0), new Rectangle(400, 390, 444 - 380, 400 - 390));  // 130
            AddRect(new Vector2(0, -10), new Rectangle(400, 400, 444 - 380, 410 - 400));    // 131
            AddRect(new Vector2(0, -5), new Rectangle(400, 410, 438 - 380, 420 - 410)); // 132
            AddRect(new Vector2(0, -5), new Rectangle(400, 420, 428 - 380, 430 - 420)); // 133
            AddRect(new Vector2(0, 0), new Rectangle(400, 430, 442 - 380, 440 - 430));  // 134
            AddRect(new Vector2(-27, -10), new Rectangle(400, 440, 434 - 380, 450 - 440));  // 135
            AddRect(new Vector2(0, -10), new Rectangle(400, 450, 396 - 380, 460 - 450));    // 136
            AddRect(new Vector2(-27, -10), new Rectangle(400, 460, 434 - 380, 470 - 460));  // 137
            AddRect(new Vector2(-54, -1), new Rectangle(400, 470, 434 - 380, 480 - 470));   // 138
            AddRect(new Vector2(-18, -10), new Rectangle(400, 480, 416 - 380, 490 - 480));  // 139
            AddRect(new Vector2(-46, -5), new Rectangle(400, 490, 426 - 380, 500 - 490));   // 140
            AddRect(new Vector2(0, -5), new Rectangle(400, 500, 434 - 380, 510 - 500)); // 141
            AddRect(new Vector2(-23, -10), new Rectangle(400, 510, 408 - 380, 520 - 510));  // 142
            AddRect(new Vector2(-12, -10), new Rectangle(400, 520, 404 - 380, 530 - 520));  // 143
            AddRect(new Vector2(-41, -5), new Rectangle(400, 530, 418 - 380, 540 - 530));   // 144
            AddRect(new Vector2(-41, -5), new Rectangle(400, 540, 418 - 380, 550 - 540));   // 145
            AddRect(new Vector2(0, -10), new Rectangle(400, 550, 408 - 380, 560 - 550));    // 146
            AddRect(new Vector2(-27, -5), new Rectangle(400, 560, 408 - 380, 570 - 560));   // 147
            AddRect(new Vector2(-36, -5), new Rectangle(400, 570, 418 - 380, 580 - 570));   // 148
            AddRect(new Vector2(-36, -5), new Rectangle(400, 580, 418 - 380, 590 - 580));   // 149
            AddRect(new Vector2(-36, -5), new Rectangle(400, 590, 418 - 380, 600 - 590));   // 150
            AddRect(new Vector2(-19, -10), new Rectangle(400, 600, 418 - 380, 610 - 600));  // 151
            AddRect(new Vector2(5, -16), new Rectangle(400, 610, 454 - 380, 620 - 610));    // 152
            AddRect(new Vector2(-61, -5), new Rectangle(400, 620, 436 - 380, 630 - 620));   // 153
            AddRect(new Vector2(-70, -5), new Rectangle(400, 630, 444 - 380, 640 - 630));   // 154
                                                                                            // 訳あって手打ち
            AddRect(new Vector2(-5f, -5f), new Rectangle(376, 640, 80, 10));
            AddRect(new Vector2(-28f, -16f), new Rectangle(400, 650, 60, 10));
            AddRect(new Vector2(-42f, -16f), new Rectangle(400, 660, 90, 10));

            AddRect(new Vector2(0, 0), new Rectangle(400, 672, 56, 10)); // 북미대陸서해안
        }
    }
}
