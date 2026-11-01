/*-------------------------------------------------------------------------

 자주 쓰는 것들을 모아 놓은 것
 DirectX 관계상 Dispose() 를 가장 마지막에 호출할 것
 (적어도 텍스쳐 등을 파기한 뒤 Dispose() 가 호출되어야 한다)

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using directx;
using System.Text;
using System.Reflection;
using Utility.Ini;
using Utility.KeyAssign;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	/// <summary>
	/// 키할당기능
	/// </summary>
	public enum KeyFunction {
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
		setting_window_button_exec_gvoac,	  // 해역 정보 수집 시작
		folder_open_00,
		folder_open_01,
		folder_open_02,
		folder_open_03,
		window_top_most,
		window_maximize,
		window_minimize,
		window_change_border_style,
		window_close,
		game_info_dlg,
		site_open_inven,				  // 대항해시대 인벤을 연다
		site_open_joy,					  // 대항해시대 조이를 연다

		unknown_function = 65535,		  // 정의되지 않은 기능
	};

	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class gvt_lib : IDisposable {
		private d3d_device m_d3d_device;			// 그리기
		private LoopXImage m_loop_x_image;		// 지도관리
		private icons m_icons;			// 아이콘관리
		private nameTexture m_nameTexture;
		private GlobalSettings m_setting;			  // 설정항목
		private IniProfileSetting m_ini_manager;			// 설정 읽기 관리
		private KeyAssignManager m_key_assign_manager;  // 키할당관리

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public LoopXImage loop_image { get { return m_loop_x_image; } }
		public d3d_device device { get { return m_d3d_device; } }
		public icons icons { get { return m_icons; } }
		public nameTexture nameTexture { get { return m_nameTexture; } }
		public GlobalSettings setting { get { return m_setting; } }
		public IniProfileSetting IniManager { get { return m_ini_manager; } }
		public KeyAssignManager KeyAssignManager { get { return m_key_assign_manager; } }

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public gvt_lib(System.Windows.Forms.Form form, string ini_file_name) {
			// 설정 항목의 읽기 관리
			m_ini_manager = new IniProfileSetting(ini_file_name);

			// 설정항목
			m_setting = new GlobalSettings();
			// 키할당관리
			m_key_assign_manager = new KeyAssignManager();

			// 등록
			m_ini_manager.AddIniSaveLoad(m_setting);
			m_ini_manager.AddIniSaveLoad(m_key_assign_manager, "key_assign");

			// 메인 윈도우 그리기용
			m_d3d_device = new d3d_device(form);
			m_d3d_device.skip_max = def.SKIP_DRAW_FRAME_MAX;		// 그리기스킵수

			// 지도관리
			m_loop_x_image = new LoopXImage(m_d3d_device);

			// 아이콘관리
			m_icons = new icons(m_d3d_device, def.ICONSIMAGE_FULLNAME);
			
			// 도시 등의 문자 그림 관리
			m_nameTexture = new nameTexture(m_d3d_device);
			
			// 키 할당 초기화
			init_key_assign();
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public void Dispose() {
			if (m_loop_x_image != null) m_loop_x_image.Dispose();
			if (m_icons != null) m_icons.Dispose();
			
			// 마지막에 D3Ddevice 를 파기한다
			if (m_d3d_device != null) m_d3d_device.Dispose();

			m_loop_x_image = null;
			m_icons = null;
			
			m_d3d_device = null;
		}

		/// <summary>
		/// 키 할당 초기화
		/// </summary>
		private void init_key_assign() {
			m_key_assign_manager.List.AddAssign("지도 변경", "지도", Keys.M, KeyFunction.map_change);
			m_key_assign_manager.List.AddAssign("지도 크기 초기화", "지도", Keys.Home, KeyFunction.map_reset_scale);
			m_key_assign_manager.List.AddAssign("지도 확대", "지도", Keys.Add, KeyFunction.map_zoom_in);
			m_key_assign_manager.List.AddAssign("지도 축소", "지도", Keys.Subtract, KeyFunction.map_zoom_out);
			m_key_assign_manager.List.AddAssign("블루 라인 초기화", "진행 방향선", Keys.B, KeyFunction.blue_line_reset);
			m_key_assign_manager.List.AddAssign("장소 표시 해제", "해제", Keys.Escape, KeyFunction.cancel_spot);
			m_key_assign_manager.List.AddAssign("항로도 선택 해제", "해제", Keys.Escape, KeyFunction.cancel_select_sea_routes);
			m_key_assign_manager.List.AddAssign("아이템 창 표시/최소화", "아이템 창", Keys.None, KeyFunction.item_window_show_min);
			m_key_assign_manager.List.AddAssign("다음 탭으로 이동", "아이템 창", Keys.Tab | Keys.Control, KeyFunction.item_window_next_tab);
			m_key_assign_manager.List.AddAssign("이전 탭으로 이동", "아이템 창", Keys.Tab | Keys.Control | Keys.Shift, KeyFunction.item_window_prev_tab);
			m_key_assign_manager.List.AddAssign("설정 창 표시/최소화", "설정 창", Keys.None, KeyFunction.setting_window_show_min);
			m_key_assign_manager.List.AddAssign("항로 기록 ON/OFF", "설정 창", Keys.None, KeyFunction.setting_window_button_00);
			m_key_assign_manager.List.AddAssign("공유 중인 배 표시 ON/OFF", "설정 창", Keys.None, KeyFunction.setting_window_button_01);
			m_key_assign_manager.List.AddAssign("@Web 아이콘 표시 ON/OFF", "설정 창", Keys.None, KeyFunction.setting_window_button_02);
			m_key_assign_manager.List.AddAssign("메모 아이콘 표시 ON/OFF", "설정 창", Keys.None, KeyFunction.setting_window_button_03);
			m_key_assign_manager.List.AddAssign("항로선 표시 ON/OFF", "설정 창", Keys.None, KeyFunction.setting_window_button_04);
			m_key_assign_manager.List.AddAssign("날짜 말풍선 표시 전환", "설정 창", Keys.None, KeyFunction.setting_window_button_05);
			m_key_assign_manager.List.AddAssign("재해 팝업 표시 ON/OFF", "설정 창", Keys.None, KeyFunction.setting_window_button_06);
			m_key_assign_manager.List.AddAssign("현재 위치를 중심으로 표시 ON/OFF", "설정 창", Keys.None, KeyFunction.setting_window_button_07);
			m_key_assign_manager.List.AddAssign("진행 방향선 표시 ON/OFF", "설정 창", Keys.None, KeyFunction.setting_window_button_08);
			m_key_assign_manager.List.AddAssign("해역 변동 시스템 설정", "설정 창", Keys.None, KeyFunction.setting_window_button_09);
			m_key_assign_manager.List.AddAssign("항로도 스크린샷", "설정 창", Keys.None, KeyFunction.setting_window_button_10);
			m_key_assign_manager.List.AddAssign("항로도 목록 열기", "설정 창", Keys.L | Keys.Control, KeyFunction.setting_window_button_11);
			m_key_assign_manager.List.AddAssign("전체 검색 창 열기", "설정 창", Keys.F | Keys.Control, KeyFunction.setting_window_button_12);
			m_key_assign_manager.List.AddAssign("설정 대화상자 열기", "설정 창", Keys.None, KeyFunction.setting_window_button_13);
			m_key_assign_manager.List.AddAssign("게임 자료 창 열기", "설정 창", Keys.None, KeyFunction.game_info_dlg);
			// 일본판 전용 도구(gvoac.exe)를 실행하므로 실수로 실행되지 않도록 기본 단축키는 없음
			m_key_assign_manager.List.AddAssign("해역 정보 수집 시작 (일본판 전용)", "설정 창", Keys.None, KeyFunction.setting_window_button_exec_gvoac);
			m_key_assign_manager.List.AddAssign("항로도 스크린샷 폴더 열기", "폴더", Keys.None, KeyFunction.folder_open_00);
			m_key_assign_manager.List.AddAssign("대항해시대Online 채팅 로그 폴더 열기", "폴더", Keys.None, KeyFunction.folder_open_01);
			m_key_assign_manager.List.AddAssign("대항해시대Online 메일 폴더 열기", "폴더", Keys.None, KeyFunction.folder_open_02);
			m_key_assign_manager.List.AddAssign("대항해시대Online 스크린샷 폴더 열기", "폴더", Keys.None, KeyFunction.folder_open_03);
			m_key_assign_manager.List.AddAssign("항상 맨 위에 표시 ON/OFF", "창", Keys.None, KeyFunction.window_top_most);
			m_key_assign_manager.List.AddAssign("창 최대화/이전 크기로", "창", Keys.None, KeyFunction.window_maximize);
			m_key_assign_manager.List.AddAssign("최소화", "창", Keys.None, KeyFunction.window_minimize);
			m_key_assign_manager.List.AddAssign("창 테두리 표시/숨김", "창", Keys.None, KeyFunction.window_change_border_style);
			m_key_assign_manager.List.AddAssign("프로그램 종료", "창", Keys.None, KeyFunction.window_close);
			m_key_assign_manager.List.AddAssign("대항해시대 인벤 열기", "사이트", Keys.None, KeyFunction.site_open_inven);
			m_key_assign_manager.List.AddAssign("대항해시대 조이 열기", "사이트", Keys.None, KeyFunction.site_open_joy);
		}
	}
}
