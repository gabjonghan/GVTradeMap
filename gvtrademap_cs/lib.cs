/*-------------------------------------------------------------------------

 よく使うものをまとめたもの
 DirectXの関係から最後にDispose()を呼ぶこと
 (少なくとも텍스쳐등を破棄した後Dispose()が呼ばれなくてはならない)

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
namespace gvtrademap_cs
{
	/// <summary>
	/// キー割り当て기능
	/// </summary>
	public enum KeyFunction{
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
	public class gvt_lib : IDisposable
	{
		private d3d_device			m_d3d_device;			// 그리기
		private LoopXImage		m_loop_x_image;			// 지도管理
		private icons				m_icons;				// 아이콘管理
		private infonameimage		m_infonameimage;		// 도시등の文字の絵管理
		private seainfonameimage	m_seainfonameimage;		// 해역の文字の絵管理
		private GlobalSettings				m_setting;				// 설정항목
		private IniProfileSetting	m_ini_manager;			// 설정の読み書き管理
		private KeyAssignManager	m_key_assign_manager;	// キー割り当て管理

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public LoopXImage loop_image{				get{	return m_loop_x_image;			}}
		public d3d_device device{					get{	return m_d3d_device;			}}
		public icons icons{							get{	return m_icons;					}}
		public infonameimage infonameimage{			get{	return m_infonameimage;			}}
		public seainfonameimage seainfonameimage{	get{	return m_seainfonameimage;		}}
		public GlobalSettings setting{						get{	return m_setting;				}}
		public IniProfileSetting IniManager{		get{	return m_ini_manager;			}}
		public KeyAssignManager KeyAssignManager{	get{	return m_key_assign_manager;	}}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public gvt_lib(System.Windows.Forms.Form form, string ini_file_name)
		{
			// 설정항목の読み書き管理
			m_ini_manager				= new IniProfileSetting(ini_file_name);

			// 설정항목
			m_setting					= new GlobalSettings();
			// キー割り当て管理
			m_key_assign_manager		= new KeyAssignManager();

			// 登録
			m_ini_manager.AddIniSaveLoad(m_setting);
			m_ini_manager.AddIniSaveLoad(m_key_assign_manager, "key_assign");
	
			// メイン윈도우그리기用
			m_d3d_device				= new d3d_device(form);
			m_d3d_device.skip_max		= def.SKIP_DRAW_FRAME_MAX;		// 그리기스킵数

			// 지도管理
			m_loop_x_image				= new LoopXImage(m_d3d_device);

			// 아이콘管理
			m_icons						= new icons(m_d3d_device, def.ICONSIMAGE_FULLNAME);
			// 도시등の文字の絵管理
			m_infonameimage				= new infonameimage(m_d3d_device, def.INFONAMEIMAGE_FULLNAME);
			// 해역の文字の絵管理
			m_seainfonameimage			= new seainfonameimage(m_d3d_device, def.SEAINFONAMEIMAGE_FULLNAME);

			// キー割り当て初期化
			init_key_assign();
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public void Dispose()
		{
			if(m_loop_x_image != null)		m_loop_x_image.Dispose();
			if(m_icons != null)				m_icons.Dispose();
			if(m_infonameimage != null)		m_infonameimage.Dispose();
			if(m_seainfonameimage != null)	m_seainfonameimage.Dispose();

			// 最後にD3Ddeviceを破棄する
			if(m_d3d_device != null)		m_d3d_device.Dispose();
	
			m_loop_x_image		= null;
			m_icons				= null;
			m_infonameimage		= null;
			m_seainfonameimage	= null;

			m_d3d_device		= null;
		}

		/// <summary>
		/// キー割り当て初期化
		/// </summary>
		private void init_key_assign()
		{
			m_key_assign_manager.List.AddAssign("지도변경", "지도",Keys.M,													KeyFunction.map_change);
			m_key_assign_manager.List.AddAssign("지도 크기 초기화", "지도", Keys.Home,										KeyFunction.map_reset_scale);
			m_key_assign_manager.List.AddAssign("지도확대", "지도", Keys.Add,												KeyFunction.map_zoom_in);
			m_key_assign_manager.List.AddAssign("지도축소", "지도", Keys.Subtract,											KeyFunction.map_zoom_out);
			m_key_assign_manager.List.AddAssign("블루라인 초기화", "진행방향선", Keys.B,								KeyFunction.blue_line_reset);
			m_key_assign_manager.List.AddAssign("장소해제", "해제", Keys.Escape,											KeyFunction.cancel_spot);
			m_key_assign_manager.List.AddAssign("항로도 선택해제", "해제", Keys.Escape,										KeyFunction.cancel_select_sea_routes);
			m_key_assign_manager.List.AddAssign("아이템윈도우 표시/최소화", "아이템윈도우", Keys.None,		KeyFunction.item_window_show_min);
			m_key_assign_manager.List.AddAssign("다음 탭 이동", "아이템윈도우", Keys.Tab|Keys.Control,				KeyFunction.item_window_next_tab);
			m_key_assign_manager.List.AddAssign("이전 탭 이동", "아이템윈도우", Keys.Tab|Keys.Control|Keys.Shift,	KeyFunction.item_window_prev_tab);
			m_key_assign_manager.List.AddAssign("설정윈도우 표시/최소화", "설정윈도우", Keys.None,				KeyFunction.setting_window_show_min);
			m_key_assign_manager.List.AddAssign("항로기록 ON/OFF", "설정윈도우", Keys.None,								KeyFunction.setting_window_button_00);
			m_key_assign_manager.List.AddAssign("공유중인 배표시 ON/OFF", "설정윈도우", Keys.None,					KeyFunction.setting_window_button_01);
			m_key_assign_manager.List.AddAssign("@Web아이콘 표시 ON/OFF", "설정윈도우", Keys.None,						KeyFunction.setting_window_button_02);
			m_key_assign_manager.List.AddAssign("메모아이콘 표시 ON/OFF", "설정윈도우", Keys.None,						KeyFunction.setting_window_button_03);
			m_key_assign_manager.List.AddAssign("항로선표시 ON/OFF", "설정윈도우", Keys.None,								KeyFunction.setting_window_button_04);
			m_key_assign_manager.List.AddAssign("목표말풍선 표시전환", "설정윈도우", Keys.None,					KeyFunction.setting_window_button_05);
			m_key_assign_manager.List.AddAssign("재해팝업 표시 ON/OFF", "설정윈도우", Keys.None,					KeyFunction.setting_window_button_06);
			m_key_assign_manager.List.AddAssign("현재위치를 중심으로 표시 ON/OFF", "설정윈도우", Keys.None,						KeyFunction.setting_window_button_07);
			m_key_assign_manager.List.AddAssign("진행방향선 표시 ON/OFF", "설정윈도우", Keys.None,							KeyFunction.setting_window_button_08);
			m_key_assign_manager.List.AddAssign("해역변동시스템 설정", "설정윈도우", Keys.None,						KeyFunction.setting_window_button_09);
			m_key_assign_manager.List.AddAssign("항로도 스크린샷", "설정윈도우", Keys.None,					KeyFunction.setting_window_button_10);
			m_key_assign_manager.List.AddAssign("항로도 목록 열기", "설정윈도우", Keys.L|Keys.Control,					KeyFunction.setting_window_button_11);
			m_key_assign_manager.List.AddAssign("전체검색창 열기", "설정윈도우", Keys.F|Keys.Control,				KeyFunction.setting_window_button_12);
			m_key_assign_manager.List.AddAssign("설정 열기", "설정윈도우", Keys.None,						KeyFunction.setting_window_button_13);
			m_key_assign_manager.List.AddAssign("해역정보수집을 시작", "설정윈도우", Keys.A|Keys.Control,			KeyFunction.setting_window_button_exec_gvoac);
			m_key_assign_manager.List.AddAssign("항로도 스크린샷 폴더 열기", "폴더", Keys.None,		KeyFunction.folder_open_00);
			m_key_assign_manager.List.AddAssign("대항해시대Online 메일 폴더 열기", "폴더", Keys.None,			KeyFunction.folder_open_01);
			m_key_assign_manager.List.AddAssign("대항해시대Online 채팅 폴더 열기", "폴더", Keys.None,			KeyFunction.folder_open_02);
			m_key_assign_manager.List.AddAssign("대항해시대Online 스크린샷 폴더 열기", "폴더", Keys.None,	KeyFunction.folder_open_03);
			m_key_assign_manager.List.AddAssign("최신화면 표시 ON/OFF", "윈도우", Keys.None,									KeyFunction.window_top_most);
			m_key_assign_manager.List.AddAssign("창 최대화/이전크기로", "윈도우", Keys.None,										KeyFunction.window_maximize);
			m_key_assign_manager.List.AddAssign("최소화", "윈도우", Keys.None,											KeyFunction.window_minimize);
			m_key_assign_manager.List.AddAssign("테두리 표시/비표시", "윈도우", Keys.None,						KeyFunction.window_change_border_style);
			m_key_assign_manager.List.AddAssign("프로그램 종료", "윈도우", Keys.None,								KeyFunction.window_close);
		}
	}
}
