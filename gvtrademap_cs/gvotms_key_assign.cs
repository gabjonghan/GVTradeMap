/*-------------------------------------------------------------------------
 
 키 할당

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using Utility;
using Utility.KeyAssign;
using System.Windows.Forms;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{

#if aa
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class gvotms_key_assign : key_assign_manager
	{
		// 기능
		public enum KEY_FUNCTION{
			map_change,
			map_reset_scale,
			map_zoom_in,
			map_zoom_out,
			blue_line_reset,
			cancel_spot,
			cancel_select_sea_routes,
			item_window_show_min,
			item_window_next_tab,
			item_window_prev_tab,
			setting_window_show_min,
			setting_window_button_00,
			setting_window_button_01,
			setting_window_button_02,
			setting_window_button_03,
			setting_window_button_04,
			setting_window_button_05,
			setting_window_button_06,
			setting_window_button_07,
			setting_window_button_08,
			setting_window_button_09,
			setting_window_button_10,
			setting_window_button_11,
			setting_window_button_12,
			setting_window_button_13,
			setting_window_button_exec_gvoac,		// 해역 정보 수집 시작
			folder_open_00,
			folder_open_01,
			folder_open_02,
			folder_open_03,
			window_top_most,
			window_maximize,
			window_minimize,
			window_change_border_style,
			window_close,

			unknown_function	= 65535,			// 정의되지 않은 기능
		};

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public gvotms_key_assign()
			: base()
		{
			init();
		}
	
		/*-------------------------------------------------------------------------
		 초기화
		---------------------------------------------------------------------------*/
		private void init()
		{
			add_assign("지도 변경", "지도",Keys.M,													KEY_FUNCTION.map_change);
			add_assign("지도 축척 리셋", "지도", Keys.Home,										KEY_FUNCTION.map_reset_scale);
			add_assign("지도 확대", "지도", Keys.Add,													KEY_FUNCTION.map_zoom_in);
			add_assign("지도 축소", "지도", Keys.Subtract,											KEY_FUNCTION.map_zoom_out);
			add_assign("블루 라인 리셋", "진행방향선", Keys.B,									KEY_FUNCTION.blue_line_reset);
			add_assign("장소해제", "해제", Keys.Escape,											KEY_FUNCTION.cancel_spot);
			add_assign("항로도 선택 해제", "해제", Keys.Escape,										KEY_FUNCTION.cancel_select_sea_routes);
			add_assign("아이템 윈도우 표시/최소화", "아이템윈도우", Keys.None,		KEY_FUNCTION.item_window_show_min);
			add_assign("다음 탭으로 이동", "아이템윈도우", Keys.Tab|Keys.Control,				KEY_FUNCTION.item_window_next_tab);
			add_assign("이전 탭으로 이동", "아이템윈도우", Keys.Tab|Keys.Control|Keys.Shift,	KEY_FUNCTION.item_window_prev_tab);
			add_assign("설정 윈도우 표시/최소화", "설정윈도우", Keys.None,					KEY_FUNCTION.setting_window_show_min);
			add_assign("항로기록ON/OFF", "설정윈도우", Keys.None,									KEY_FUNCTION.setting_window_button_00);
			add_assign("공유 중인 배 표시 ON/OFF", "설정윈도우", Keys.None,						KEY_FUNCTION.setting_window_button_01);
			add_assign("@Web아이콘표시ON/OFF", "설정윈도우", Keys.None,						KEY_FUNCTION.setting_window_button_02);
			add_assign("메모아이콘표시ON/OFF", "설정윈도우", Keys.None,						KEY_FUNCTION.setting_window_button_03);
			add_assign("항로선표시ON/OFF", "설정윈도우", Keys.None,								KEY_FUNCTION.setting_window_button_04);
			add_assign("날짜 말풍선 표시 전환", "설정윈도우", Keys.None,						KEY_FUNCTION.setting_window_button_05);
			add_assign("재해 팝업 표시 ON/OFF", "설정윈도우", Keys.None,					KEY_FUNCTION.setting_window_button_06);
			add_assign("현재 위치 중심 표시 ON/OFF", "설정윈도우", Keys.None,						KEY_FUNCTION.setting_window_button_07);
			add_assign("진행방향선표시ON/OFF", "설정윈도우", Keys.None,							KEY_FUNCTION.setting_window_button_08);
			add_assign("해역변동시스템설정", "설정윈도우", Keys.None,						KEY_FUNCTION.setting_window_button_09);
			add_assign("항로도 스크린샷", "설정윈도우", Keys.None,					KEY_FUNCTION.setting_window_button_10);
			add_assign("항로도 목록 열기", "설정윈도우", Keys.L|Keys.Control,					KEY_FUNCTION.setting_window_button_11);
			add_assign("전체 검색 열기", "설정윈도우", Keys.F|Keys.Control,				KEY_FUNCTION.setting_window_button_12);
			add_assign("설정 대화상자 열기", "설정윈도우", Keys.None,							KEY_FUNCTION.setting_window_button_13);
			add_assign("해역 정보 수집 시작", "설정윈도우", Keys.A|Keys.Control,				KEY_FUNCTION.setting_window_button_exec_gvoac);
			add_assign("항로도 스크린샷 폴더 열기", "폴더", Keys.None,			KEY_FUNCTION.folder_open_00);
			add_assign("대항해시대Online 메일 폴더 열기", "폴더", Keys.None,			KEY_FUNCTION.folder_open_01);
			add_assign("대항해시대Online 채팅 폴더 열기", "폴더", Keys.None,			KEY_FUNCTION.folder_open_02);
			add_assign("대항해시대Online 스크린샷 폴더 열기", "폴더", Keys.None,	KEY_FUNCTION.folder_open_03);
			add_assign("맨 앞 표시 ON/OFF", "윈도우", Keys.None,									KEY_FUNCTION.window_top_most);
			add_assign("최대화/통상화", "윈도우", Keys.None,										KEY_FUNCTION.window_maximize);
			add_assign("최소화", "윈도우", Keys.None,												KEY_FUNCTION.window_minimize);
			add_assign("윈도우 테두리 표시/비표시", "윈도우", Keys.None,						KEY_FUNCTION.window_change_border_style);
			add_assign("교역MAP C# 닫기", "윈도우", Keys.None,									KEY_FUNCTION.window_close);
		}

		/*-------------------------------------------------------------------------
		 기능 추가
		---------------------------------------------------------------------------*/
		private void add_assign(string name, string group, Keys key, KEY_FUNCTION kf)
		{
			AddAssign(name, group, key, kf, kf.ToString());
		}

		/*-------------------------------------------------------------------------
		 키 입력으로 기능을 얻는다
		 같은 키가 할당된 경우 전부의 목록이 반환된다
		 KeyDown 이벤트에서 호출되기를 기대한다
		 할당된 기능이 없는 경우에는 null 을 반환한다
		---------------------------------------------------------------------------*/
		public List<KEY_FUNCTION> KeysToFunction(KeyEventArgs e)
		{
			List<assign>	alist	= GetAssignedList(e);
			if(alist == null)	return null;		// 할당된 기능은 없다

			List<KEY_FUNCTION>	list	= new List<KEY_FUNCTION>();
			foreach(assign i in alist){
				// 기능으로 변환해서 추가
				try{
					KEY_FUNCTION	f	= (KEY_FUNCTION)i.Tag;
					list.Add(f);
				}catch{
				}
			}
			if(list.Count <= 0)	return null;
			return list;
		}
	}
#endif
}
