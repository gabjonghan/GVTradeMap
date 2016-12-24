/*-------------------------------------------------------------------------
 
 키アサイン

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
			setting_window_button_exec_gvoac,		// 해역정보수집の시작
			folder_open_00,
			folder_open_01,
			folder_open_02,
			folder_open_03,
			window_top_most,
			window_maximize,
			window_minimize,
			window_change_border_style,
			window_close,

			unknown_function	= 65535,			// 定義されていない기능
		};

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public gvotms_key_assign()
			: base()
		{
			init();
		}
	
		/*-------------------------------------------------------------------------
		 初期化
		---------------------------------------------------------------------------*/
		private void init()
		{
			add_assign("지도の변경", "지도",Keys.M,													KEY_FUNCTION.map_change);
			add_assign("지도の축척리셋", "지도", Keys.Home,										KEY_FUNCTION.map_reset_scale);
			add_assign("지도を拡대", "지도", Keys.Add,													KEY_FUNCTION.map_zoom_in);
			add_assign("지도を縮소", "지도", Keys.Subtract,											KEY_FUNCTION.map_zoom_out);
			add_assign("ブルー라인리셋", "진행방향선", Keys.B,									KEY_FUNCTION.blue_line_reset);
			add_assign("장소해제", "해제", Keys.Escape,											KEY_FUNCTION.cancel_spot);
			add_assign("항로도の선택해제", "해제", Keys.Escape,										KEY_FUNCTION.cancel_select_sea_routes);
			add_assign("아이템윈도우の표시/최소화", "아이템윈도우", Keys.None,		KEY_FUNCTION.item_window_show_min);
			add_assign("次のタブへ이동", "아이템윈도우", Keys.Tab|Keys.Control,				KEY_FUNCTION.item_window_next_tab);
			add_assign("前のタブへ이동", "아이템윈도우", Keys.Tab|Keys.Control|Keys.Shift,	KEY_FUNCTION.item_window_prev_tab);
			add_assign("설정윈도우の표시/최소화", "설정윈도우", Keys.None,					KEY_FUNCTION.setting_window_show_min);
			add_assign("항로기록ON/OFF", "설정윈도우", Keys.None,									KEY_FUNCTION.setting_window_button_00);
			add_assign("공유している배표시ON/OFF", "설정윈도우", Keys.None,						KEY_FUNCTION.setting_window_button_01);
			add_assign("@Web아이콘표시ON/OFF", "설정윈도우", Keys.None,						KEY_FUNCTION.setting_window_button_02);
			add_assign("메모아이콘표시ON/OFF", "설정윈도우", Keys.None,						KEY_FUNCTION.setting_window_button_03);
			add_assign("항로선표시ON/OFF", "설정윈도우", Keys.None,								KEY_FUNCTION.setting_window_button_04);
			add_assign("일付ふきだし표시切り替え", "설정윈도우", Keys.None,						KEY_FUNCTION.setting_window_button_05);
			add_assign("재해ポップアップ표시ON/OFF", "설정윈도우", Keys.None,					KEY_FUNCTION.setting_window_button_06);
			add_assign("현재위치중心표시ON/OFF", "설정윈도우", Keys.None,						KEY_FUNCTION.setting_window_button_07);
			add_assign("진행방향선표시ON/OFF", "설정윈도우", Keys.None,							KEY_FUNCTION.setting_window_button_08);
			add_assign("해역변동시스템설정", "설정윈도우", Keys.None,						KEY_FUNCTION.setting_window_button_09);
			add_assign("항로도の스크린샷", "설정윈도우", Keys.None,					KEY_FUNCTION.setting_window_button_10);
			add_assign("항로도목록を開く", "설정윈도우", Keys.L|Keys.Control,					KEY_FUNCTION.setting_window_button_11);
			add_assign("전체검색を開く", "설정윈도우", Keys.F|Keys.Control,				KEY_FUNCTION.setting_window_button_12);
			add_assign("설정ダイア로그を開く", "설정윈도우", Keys.None,							KEY_FUNCTION.setting_window_button_13);
			add_assign("해역정보수집を시작する", "설정윈도우", Keys.A|Keys.Control,				KEY_FUNCTION.setting_window_button_exec_gvoac);
			add_assign("항로도の스크린샷폴더を開く", "폴더", Keys.None,			KEY_FUNCTION.folder_open_00);
			add_assign("대항해시대Onlineのメール폴더を開く", "폴더", Keys.None,			KEY_FUNCTION.folder_open_01);
			add_assign("대항해시대Onlineのチャット폴더を開く", "폴더", Keys.None,			KEY_FUNCTION.folder_open_02);
			add_assign("대항해시대Onlineの스크린샷폴더を開く", "폴더", Keys.None,	KEY_FUNCTION.folder_open_03);
			add_assign("最前面표시ON/OFF", "윈도우", Keys.None,									KEY_FUNCTION.window_top_most);
			add_assign("최대化/통상化", "윈도우", Keys.None,										KEY_FUNCTION.window_maximize);
			add_assign("최소화", "윈도우", Keys.None,												KEY_FUNCTION.window_minimize);
			add_assign("윈도우枠の표시/비표시", "윈도우", Keys.None,						KEY_FUNCTION.window_change_border_style);
			add_assign("교역MAP C#を닫기", "윈도우", Keys.None,									KEY_FUNCTION.window_close);
		}

		/*-------------------------------------------------------------------------
		 기능の추가
		---------------------------------------------------------------------------*/
		private void add_assign(string name, string group, Keys key, KEY_FUNCTION kf)
		{
			AddAssign(name, group, key, kf, kf.ToString());
		}

		/*-------------------------------------------------------------------------
		 키入力から기능を得る
		 同一の키が할당られている場合전부の목록が返る
		 KeyDownイベントから呼ばれることを期待している
		 할당られた기능がない場合nullを返す
		---------------------------------------------------------------------------*/
		public List<KEY_FUNCTION> KeysToFunction(KeyEventArgs e)
		{
			List<assign>	alist	= GetAssignedList(e);
			if(alist == null)	return null;		// 할당られた기능はない

			List<KEY_FUNCTION>	list	= new List<KEY_FUNCTION>();
			foreach(assign i in alist){
				// 기능に변환して추가
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
