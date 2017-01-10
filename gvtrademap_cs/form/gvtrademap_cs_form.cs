/*-------------------------------------------------------------------------

 main form

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 define
---------------------------------------------------------------------------*/
// CPU사용현황を표시する
//#define	DEBUG_DRAW_CPU_BAR
// 나침반분석デバッグ
//#define	DEBUG_COMPASS
// 공유항로デバッグ
//#define	DEBUG_SHARE_ROUTES
// 진행방향선デバッグ
//#define	DEBUG_ANGLE_LINE
// デバッグ문자열の표시
//#define	DEBUG_DRAW_DEBUG_STRING

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using directx;
using gvo_base;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Utility;
using win32;

using net_base;
using gvo_net_base;
using Utility.KeyAssign;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public partial class gvtrademap_cs_form : Form {
		// タイマ간격
		private const int CHAT_LOG_TIMER_INTERVAL = 5100;	   // 5.1초간격
		private const int SHARE_TIMER_INTERVAL = 60 * 1000; // 1분간격

		// ツールチップ표시までの시간
		//		private const int			TOOLTIP_INITIAL			= 7;		// 1/60単位
		private const int TOOLTIP_INITIAL = 15;	 // 1/60単位

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		// 현재パス
		private string m_current_path;

		// 위치정보
		private Point m_old_mouse_pos;  // 前회の마우스위치
		private Point m_mouse_move;	 // 마우스の이동량
		private myship_info m_myship_info;	  // 본인の배の정보

		private Point m_select_pos;	 // マーカーを置いた위치

		// 현재の지도정보
		private MapIndex m_map_index;	   // 지도번호
		private bool m_use_mixed_map;   // 항로도즐겨찾기と合成した지도を사용する

		private bool m_pause;		   // 최소화중はtrue

		// 	
		private gvt_lib m_lib;			  // よく使う기능をまとめたもの
		private GvoDatabase m_db;			   // 정보관리

		// 윈도우관리
		// 그리기関係
		private d3d_windows m_windows;		  // 윈도우관리
		private item_window m_item_window;	  // 좌の윈도우
		private setting_window m_setting_window;	// 설정ボタン윈도우
		private spot m_spot;				// 장소표시
		private info_windows m_info_windows;		// 정보표시윈도우관리

		// 검색ダイア로그
		private find_form2 m_find_form;	 // 검색ダイア로그
		private RequestCtrl m_req_show_find_form;
		// 항로도목록ダイア로그
		private sea_routes_form2 m_sea_routes_form;
		private RequestCtrl m_req_sea_routes_form;

		// 마우스훅
		private globalmouse_hook m_mouse_hook;	  // 마우스훅

		// ツールチップ
		private ToolTip m_tooltip;		  // ツールチップ
		private int m_tooltip_interval; // ツールチップ표시までの카운터
		private bool m_show_tooltip;		// ツールチップ표시중のときtrue
		private Point m_tooltip_old_mouse_pos;

		// 스레드
		private ManualResetEvent m_exit_thread_event;   // 스레드종료イベント관리
		private Thread m_load_map_t;		// 지도읽기스레드
		private Thread m_load_info_t;	   // 정보읽기스레드
		private Thread m_share_t;		   // 항로공유스레드
		private Thread m_chat_log_t;		// チャット분석스레드

		// ***
		private LoadInfosStatus _LoadInfosStatus;

		// タイマ
		private System.Windows.Forms.Timer m_share_timer;	   // 항로공유タイマ

#if DEBUG_DRAW_CPU_BAR
		// for debug
		private qpctimer			m_qpct;				// パフォーマンス測定용
		private cpubar				m_cpubar;
#endif
		//
		private Point m_memo_icon_pos;  // 메모아이콘추가용좌표
		private map_mark.data m_memo_icon_data;

#if DEBUG_ANGLE_LINE
		private float m_debug_angle = 0;
		private int m_debug_angle_i = 0;
#endif
		private string m_device_info_string;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private bool is_load_map {
			get {
				if (m_load_map_t == null) return false;
				else return m_load_map_t.IsAlive;
			}
		}
		private bool is_load_info {
			get {
				if (m_load_info_t == null) return false;
				else return m_load_info_t.IsAlive;
			}
		}
		private bool is_load {
			get {
				if (is_load_map) return true;
				if (is_load_info) return true;
				return false;
			}
		}
		private bool is_chat_log_t {
			get {
				if (m_chat_log_t == null) return false;
				else return m_chat_log_t.IsAlive;
			}
		}
		// 항로공유스레드が動いているかどうかを得る
		private bool is_share {
			get {
				if (m_share_t == null) return false;
				else return m_share_t.IsAlive;
			}
		}
		// 스레드が動いているかどうかを得る
		private bool is_run_thread {
			get {
				if (is_load) return true;
				if (is_share) return true;
				if (is_chat_log_t) return true;
				return false;
			}
		}
		private Rectangle main_window_crect { get { return this.ClientRectangle; } }

		private bool is_show_menu_strip {
			get {
				if (contextMenuStrip1.Visible) return true;
				if (contextMenuStrip2.Visible) return true;
				if (contextMenuStrip3.Visible) return true;
				return false;
			}
		}
		public string device_info_string { get { return m_device_info_string; } }

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public gvtrademap_cs_form() {
			InitializeComponent();

			// タイトル
			this.Text = def.WINDOW_TITLE;
#if DEBUG_SHARE_ROUTES
			this.Text	+= " 항로공유デバッグ";
#endif
			Useful.SetFontMeiryo(this, def.MEIRYO_POINT);
		}

		/*-------------------------------------------------------------------------
		 初期化
		---------------------------------------------------------------------------*/
		public bool Initialize() {
			// アプリケーションの格納パス
			m_current_path = Path.GetDirectoryName(Application.ExecutablePath);
			// 현재ディレクトリを설정する
			System.Environment.CurrentDirectory = m_current_path;

			// 必要なパスの작성
			file_ctrl.CreatePath(def.MAP_PATH);
			file_ctrl.CreatePath(def.SEAROUTE_PATH);
			file_ctrl.CreatePath(def.SS_PATH);
			file_ctrl.CreatePath(def.MEMO_PATH);

			// 마우스による드래그용
			m_old_mouse_pos = new Point(0, 0);
			m_mouse_move = new Point(0, 0);

			// 정보
			m_select_pos = new Point(0, 0);

			// 메모아이콘설정용	
			m_memo_icon_pos = new Point(0, 0);
			m_memo_icon_data = null;

			// 스레드종료관리
			m_exit_thread_event = new ManualResetEvent(false);

			// 그리기関係
			m_lib = new gvt_lib(this, Path.Combine(m_current_path, def.INI_FNAME));
			// デバイス정보を得ておく
			// 予期しない오류표시時용
			m_device_info_string = m_lib.device.deviec_info_string_short;

			// Cmdデリゲート
			m_lib.KeyAssignManager.OnProcessCmdKey += new OnProcessCmdKey(process_cnd_key);
			m_lib.KeyAssignManager.OnUpdateAssignList += new EventHandler(update_menu_shortcut);
			// メニューのタグを설정する
			init_menu_tag();

			// iniの읽기
			m_lib.IniManager.Load();

			// ショートカット키の표시をメニューに反映させる
			update_menu_shortcut(null, EventArgs.Empty);

			// GvoDatabase
			m_db = new GvoDatabase(m_lib);

			// 본인の배の정보
			m_myship_info = new myship_info(m_lib, m_db);

			// ***
			_LoadInfosStatus = new LoadInfosStatus();

			// 장소표시
			m_spot = new spot(m_lib, m_db.World);
			// 윈도우관리
			m_windows = new d3d_windows(m_lib.device);
			m_item_window = new item_window(m_lib, m_db, m_spot, textBox1, listView1, this);
			m_setting_window = new setting_window(m_lib, m_db, this);
			// 아이템윈도우を先に등록する
			m_windows.Add(m_setting_window);
			m_windows.Add(m_item_window);
			// 서브윈도우
			m_info_windows = new info_windows(m_lib, m_db, m_myship_info);

			// 마우스훅
			// 사용するときだけ훅する
			m_mouse_hook = null;

			// ツールチップ
			m_tooltip = new ToolTip();
			m_tooltip.SetToolTip(listView1, "おまじない");
			m_tooltip.AutoPopDelay = 30 * 1000;	 // 30초표시
			m_tooltip.BackColor = Color.LightYellow;
			m_show_tooltip = false;
			m_tooltip_old_mouse_pos = new Point(0, 0);

			// 설정정보初期化
			// 윈도우위치등を反映
			init_setting();

			// 검색ダイア로그
			m_find_form = new find_form2(m_lib, m_db, m_spot, m_item_window);
			m_find_form.Location = m_lib.setting.find_window_location;
			m_find_form.Size = m_lib.setting.find_window_size;
			m_req_show_find_form = new RequestCtrl();
			// 검색で좌표を검색していると시작時にそこがセンタリングされてしまう
			// なのでリクエストを空読みして스킵する
			m_lib.setting.req_centering_gpos.IsRequest();

			// 항로도목록ダイア로그
			m_sea_routes_form = new sea_routes_form2(m_lib, m_db);
			m_sea_routes_form.Location = m_lib.setting.sea_routes_window_location;
			m_sea_routes_form.Size = m_lib.setting.sea_routes_window_size;
			m_req_sea_routes_form = new RequestCtrl();

			// 지도읽기
			m_map_index = MapIndex.Max;			 // 최초は必ず읽기
			m_use_mixed_map = m_lib.setting.use_mixed_map;  // 설정を로드
			load_map();												 // 읽기스레드시작

			// タイマ
			m_share_timer = new System.Windows.Forms.Timer();
			m_share_timer.Interval = SHARE_TIMER_INTERVAL;
			m_share_timer.Tick += new EventHandler(share_timer_Tick);
			m_share_timer.Start();

			// 디테일정보읽기스레드
			m_load_info_t = new Thread(new ThreadStart(load_info_proc));
			m_load_info_t.Name = "load info";
			m_load_info_t.Start();
			// 로그분석스레드
			m_chat_log_t = new Thread(new ThreadStart(chat_log_proc));
			m_chat_log_t.Name = "analize chat log";
			m_chat_log_t.Start();

#if DEBUG_DRAW_CPU_BAR
			// for debug
			m_cpubar					= new cpubar(m_lib.device);
			m_qpct						= new qpctimer();
#endif
			// ポーズ플래그
			m_pause = false;
			return true;
		}

		/*-------------------------------------------------------------------------
		 설정정보설정
		---------------------------------------------------------------------------*/
		private void init_setting() {
			// 윈도우위치, 사이즈の復元
			// 내부でOnMove()が呼ばれるため, ちょっといやな感じに설정する
			Size size = m_lib.setting.window_size;
			this.Location = m_lib.setting.window_location;
			this.Size = size;

			// 윈도우枠없음
			if (m_lib.setting.is_border_style_none) {
				ExecFunction(KeyFunction.window_change_border_style);
			}

			// 선택してる도시
			m_item_window.info = m_db.World.FindInfo(m_lib.setting.select_info);
			m_item_window.EnableItemWindow(false);

			// 위치
			m_lib.loop_image.OffsetPosition = new Vector2(m_lib.setting.map_pos_x, m_lib.setting.map_pos_y);
			// 스케일
			m_lib.loop_image.SetScale(m_lib.setting.map_scale, new Point(0, 0), false);

			// 아이템윈도우상태
			// 최소화と통상化
			if (!m_lib.setting.is_item_window_normal_size) {
				m_item_window.window_mode = d3d_windows.window.mode.small;
			}
			if (!m_lib.setting.is_setting_window_normal_size) {
				m_setting_window.window_mode = d3d_windows.window.mode.small;
			}
		}

		/*-------------------------------------------------------------------------
		 メニューのタグを설정する
		 TagにKeyFunctionを지정することで自動でコマンドが実行されるようにする
		---------------------------------------------------------------------------*/
		private void init_menu_tag() {
			m_lib.KeyAssignManager.BindTagForMenuItem(exexgvoacToolStripMenuItem, KeyFunction.setting_window_button_exec_gvoac);
			m_lib.KeyAssignManager.BindTagForMenuItem(openpathscreenshot2ToolStripMenuItem, KeyFunction.folder_open_00);
			m_lib.KeyAssignManager.BindTagForMenuItem(openpathlogToolStripMenuItem, KeyFunction.folder_open_01);
			m_lib.KeyAssignManager.BindTagForMenuItem(openpathmailToolStripMenuItem, KeyFunction.folder_open_02);
			m_lib.KeyAssignManager.BindTagForMenuItem(openpathscreenshotToolStripMenuItem, KeyFunction.folder_open_03);
			m_lib.KeyAssignManager.BindTagForMenuItem(changeBorderStyleToolStripMenuItem, KeyFunction.window_change_border_style);
			m_lib.KeyAssignManager.BindTagForMenuItem(closeFormToolStripMenuItem, KeyFunction.window_close);
			m_lib.KeyAssignManager.BindTagForMenuItem(clear_spotToolStripMenuItem, KeyFunction.cancel_spot);
		}

		/*-------------------------------------------------------------------------
		 키할당をメニューに反映させる
		 KeyAssignManager.OnUpdateAssignList용
		---------------------------------------------------------------------------*/
		private void update_menu_shortcut(object sender, EventArgs e) {
			m_lib.KeyAssignManager.UpdateMenuShortcutKeys(contextMenuStrip1);
			m_lib.KeyAssignManager.UpdateMenuShortcutKeys(contextMenuStrip2);
			m_lib.KeyAssignManager.UpdateMenuShortcutKeys(contextMenuStrip3);
		}

		/*-------------------------------------------------------------------------
		 Closed
		---------------------------------------------------------------------------*/
		private void gvtrademap_cs_form_FormClosed(object sender, FormClosedEventArgs e) {
			// 스레드を종료させる
			finish_all_threads();

			// 마우스훅を종료する
			dispose_mouse_hook();

			// 메모を업데이트する
			m_item_window.UpdateMemo();

			// 검색ダイア로그の위치と사이즈
			m_lib.setting.find_window_location = m_find_form.Location;
			m_lib.setting.find_window_size = m_find_form.Size;
			m_lib.setting.find_window_visible = m_find_form.Visible;

			// 항로도목록ダイア로그の위치と사이즈
			m_lib.setting.sea_routes_window_location = m_sea_routes_form.Location;
			m_lib.setting.sea_routes_window_size = m_sea_routes_form.Size;
			m_lib.setting.sea_routes_window_visible = m_sea_routes_form.Visible;

			// 선택してる도시
			if (m_item_window.info != null) m_lib.setting.select_info = m_item_window.info.Name;
			else m_lib.setting.select_info = "";

			// 위치
			m_lib.setting.map_pos_x = m_lib.loop_image.OffsetPosition.X;
			m_lib.setting.map_pos_y = m_lib.loop_image.OffsetPosition.Y;

			// 스케일
			m_lib.setting.map_scale = m_lib.loop_image.ImageScale;

			// 아이템윈도우상태
			m_lib.setting.is_item_window_normal_size = m_item_window.window_mode == d3d_windows.window.mode.normal;
			m_lib.setting.is_setting_window_normal_size = m_setting_window.window_mode == d3d_windows.window.mode.normal;

			// iniの書き出し
			m_lib.IniManager.Save();

			// GvoDatabaseで書きだす必要のある정보の書き出し
			m_db.WriteSettings();
		}

		/*-------------------------------------------------------------------------
		 스레드종료シグナルを설정し, 스레드종료を待つ
		---------------------------------------------------------------------------*/
		private void finish_all_threads() {
			// 스레드종료イベントを설정する
			if (m_exit_thread_event != null) {
				m_exit_thread_event.Set();
			}

			wait_finish_thread(m_load_map_t);
			wait_finish_thread(m_load_info_t);
			wait_finish_thread(m_share_t);
			wait_finish_thread(m_chat_log_t);
		}

		/*-------------------------------------------------------------------------
		 스레드종료を待つ
		---------------------------------------------------------------------------*/
		private void wait_finish_thread(Thread t) {
			if (t == null) return;
			if (!t.IsAlive) return;	 // 動いていない
			t.Join();				   // 종료대기
		}

		/*-------------------------------------------------------------------------
		 OnPaint
		---------------------------------------------------------------------------*/
		private void m_main_window_Paint(object sender, PaintEventArgs e) {
			// メイン윈도우の업데이트
			// ダイア로그그리기時のために용意しておく
			// 背景を그리기する関수が불명のため, 一도背景색で전체を塗られる
			m_lib.device.SetMustDrawFlag();
			update_main_window();
		}

		/*-------------------------------------------------------------------------
		 지도읽기
		---------------------------------------------------------------------------*/
		private void load_map() {
			if (is_load) return;		// なにか로딩중
			if ((m_map_index == m_lib.setting.map)
			   && (m_use_mixed_map == m_lib.setting.use_mixed_map)) return;	 // 변경없음

			m_load_map_t = new Thread(new ThreadStart(load_map_proc));
			m_load_map_t.Name = "load map";
			m_load_map_t.Start();
		}

		/*-------------------------------------------------------------------------
		 좌표변환
		 主に도시の추가時용
		 측량좌표を지도좌표に변환してデバッグ出力する
		---------------------------------------------------------------------------*/
		private void debug_transform_pos(int x, int y) {
			Point pos = transform.game_pos2_map_pos(new Point(x, y), m_lib.loop_image);
			Debug.WriteLine(String.Format("{0},{1}", pos.X, pos.Y));
		}

		/*-------------------------------------------------------------------------
		 メイン윈도우の업데이트
		---------------------------------------------------------------------------*/
		public void update_main_window() {
			if (m_lib.device.device == null) return;

			// 最前열표시
			if (this.TopMost != m_lib.setting.window_top_most) {
				this.TopMost = m_lib.setting.window_top_most;
			}

			// 지도변경チェック
			load_map();

			if (is_load) {
				// 何か로딩중

				// 최소화중は그리지않음
				if (m_pause) return;

				update_main_window_load();
			} else {
				// 통상時

				// タスクの実行
				do_tasks();

				// 최소화중は그리지않음
				if (m_pause) return;

				// 그리기
				if (m_lib.device.IsNeedDraw()) {
					do_draw();
				}
			}
		}

		/*-------------------------------------------------------------------------
		 定例処理
		---------------------------------------------------------------------------*/
		private void do_tasks() {
#if DEBUG_DRAW_CPU_BAR
			m_qpct.GetElapsedTime();
#endif

			//
			// リクエストの実行
			//

			// 검색ダイア로그の표시
			// 시작時の1회のみ
			if (m_req_show_find_form.IsRequest()) {
				show_find_dialog();
				this.Activate();
			}
			// 항로도목록ダイア로그の표시
			// 시작時の1회のみ
			if (m_req_sea_routes_form.IsRequest()) {
				show_sea_routes_dialog();
				this.Activate();
			}

			// 본인の배の위치등업데이트
			m_myship_info.Update();

			// 현재の배の위치を중心に이동する
			if (m_lib.setting.center_myship) {
				if (m_myship_info.capture_sucess) {   // 캡처が成功したときのみ
					centering_pos(transform.game_pos2_map_pos(m_myship_info.pos, m_lib.loop_image));
				}
			}

			// 장소
			do_spot_request();

			// 特定の게임좌표をセンタリングする
			if (m_lib.setting.req_centering_gpos.IsRequest()) {
				centering_pos(transform.game_pos2_map_pos(m_lib.setting.centering_gpos, m_lib.loop_image));
				m_select_pos = m_lib.setting.centering_gpos;
			}

			// 
			// 설정の反映
			// 

			// 서버とベース국설정
			m_db.World.SetServerAndCountry(m_lib.setting.server, m_lib.setting.country);
			// 최적화の업데이트
			// 설정ダイア로그で내용が변경された場合그리기목록を作りなおす
			m_db.WebIcons.Update();
			// 지도のオフセット업데이트
			m_lib.loop_image.AddOffset(new Vector2(m_mouse_move.X, m_mouse_move.Y));

			// 아이템윈도우
			m_windows.Update();
			// tooltip
			update_tooltip();
			// 정보표시윈도우
			m_info_windows.Update(m_select_pos, m_old_mouse_pos);

			// 드래그량を0に戻す
			if ((m_mouse_move.X != 0) || (m_mouse_move.Y != 0)) {
				// 드래그중は그리기스킵없음
				m_lib.device.SetMustDrawFlag();
			}

			m_mouse_move = new Point(0, 0);

			// 마우스훅시작と종료
			// 설정により시작と종료を切り替える
			do_mouse_hook();

			// 위험해역변동시스템
			if (m_map_index == MapIndex.Map2) m_db.SeaArea.color = sea_area.color_type.type1;
			else m_db.SeaArea.color = sea_area.color_type.type2;
			m_db.SeaArea.Update();

			// 스크린샷
			if (m_lib.setting.req_screen_shot.IsRequest()) {
				screen_shot();
			}

			// 항로도목록の업데이트
			if (m_db.SeaRoute.req_update_list.IsRequest()) {
				// 다시구축
				m_sea_routes_form.UpdateAllList();
				m_db.SeaRoute.req_redraw_list.IsRequest();	  // 空読み
			}
			if (m_db.SeaRoute.req_redraw_list.IsRequest()) {
				// 다시그리기
				// 최신の항로도のみ다시그리기
				m_sea_routes_form.RedrawNewestSeaRoutes();
			}
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		private void do_draw() {
			// 화면のクリア
			m_lib.device.Clear(Color.Black);
			//			m_lib.Device.Clear(Color.FromArgb(255,152,176,190));

			// 그리기開始
			if (!m_lib.device.Begin()) return;	  // デバイスロスト중

			// 지도그리기
			draw_map();

			// GvoDatabase
			m_db.Draw();

			// 본인배の위치등그리기
			m_myship_info.Draw();

			// 장소
			m_spot.Draw();

			// 위도と경도の그리기
			// 좌표
			if (m_lib.setting.tude_interval != TudeInterval.None) {
				latitude_longitude.DrawPoints(m_lib);
			}

			// 아이템윈도우
			m_windows.Draw();

			if (!m_lib.setting.is_server_mode) {
				// 캡처디테일を그리기
				m_db.Capture.DrawCapturedTexture();
			}

#if DEBUG_DRAW_DEBUG_STRING
			debug_draw_debug_string();
#endif
#if DEBUG_COMPASS
			debug_compass();
#endif
#if DEBUG_ANGLE_LINE
			debug_angle_line();
#endif
			// 정보표시윈도우
			m_info_windows.Draw();

			// 화면枠
			draw_frame();

#if DEBUG_DRAW_CPU_BAR
			float check	= m_qpct.GetElapsedTime();
			m_cpubar.Update(check, 1f/60);
#endif
			m_lib.device.End();

			// 화면を전送
			m_lib.device.Present();
		}

		/*-------------------------------------------------------------------------
		 지도を그리기する
		 해역변동, 위도, 경도선그리기付き
		---------------------------------------------------------------------------*/
		private void draw_map() {
			// 지도の合成업데이트
			m_lib.loop_image.MergeImage(new LoopXImage.DrawHandler(m_db.DrawForMargeInfoNames),
										m_lib.setting.req_update_map.IsRequest());
			// 지도그리기
			m_lib.loop_image.Draw();

			// 위도と경도の그리기
			switch (m_lib.setting.tude_interval) {
				case TudeInterval.Interval1000:
					latitude_longitude.DrawLines(m_lib);
					break;
				case TudeInterval.Interval100:
					latitude_longitude.DrawLines100(m_lib);
					break;
			}
		}

		/*-------------------------------------------------------------------------
		 フレームを描く
		 windows Vista aeroのときは描かない
		---------------------------------------------------------------------------*/
		private void draw_frame() {
			if (m_lib.setting.windows_vista_aero) return;

			Vector2 size = m_lib.device.client_size;
			size.X -= 1;
			size.Y -= 1;
			m_lib.device.DrawLineRect(new Vector3(0, 0, 0.0001f), size, Color.Black.ToArgb());
		}

		/*-------------------------------------------------------------------------
		 メイン윈도우に마우스カーソルがあるかどうかを조사
		---------------------------------------------------------------------------*/
		private bool is_inside_mouse_cursor_main_window() {
			Point pos = this.PointToClient(MousePosition);
			hittest test = new hittest(this.ClientRectangle);

			return test.HitTest(pos);
		}

		#region 로딩시의 그리기
		/*-------------------------------------------------------------------------
		 로딩시의 그리기
		---------------------------------------------------------------------------*/
		public void update_main_window_load() {
			// 메모윈도우를 비표시
			m_item_window.EnableMemoWindow(false);
			m_item_window.EnableItemWindow(false);

			// 마우스 드래그시 무효
			m_mouse_move = new Point(0, 0);

			try {
				m_lib.device.Clear(Color.White);
				if (!m_lib.device.Begin()) return;	  // デバイスロスト중

				// 로딩 진행정도를 표시함
				// 화면 가로사이즈/2 를 최대로 함
				float size_x = (float)(main_window_crect.Width / 2);
				float y = (float)(main_window_crect.Height / 2) - ((((16 + 8) + 8) * 4) / 2);
				float x = size_x - (size_x / 2);

				// 도시디테일
				draw_progress("도시디테일...", _LoadInfosStatus.StatusMessage,
								x, y, size_x,
								_LoadInfosStatus.NowStep,
								_LoadInfosStatus.MaxStep, Color.Tomato.ToArgb());

				// 지도읽기
				y += 16 + 8 + 8;
				draw_progress("지도...", m_lib.loop_image.LoadStr,
								x, y, size_x,
								m_lib.loop_image.LoadCurrent,
								m_lib.loop_image.LoadMax, Color.SkyBlue.ToArgb());

				// マスク
				y += 16 + 8 + 8;
				draw_progress("육지 마스킹...", m_db.SeaArea.progress_info_str,
								x, y, size_x,
								m_db.SeaArea.progress_current,
								m_db.SeaArea.progress_max, Color.SkyBlue.ToArgb());

				// 화면枠
				draw_frame();
				m_lib.device.End();
			} catch (Exception ex) {
				Console.WriteLine("그리는 중 예외를 캐치(로딩중)\n" + ex.StackTrace);
			}

			// 화면を전送
			m_lib.device.Present();
		}

		/*-------------------------------------------------------------------------
		 프로그래스 바 그리기
		---------------------------------------------------------------------------*/
		private void draw_progress(string str, string str2, float x, float y, float size_x, int current, int max, int color) {
			m_lib.device.DrawText(font_type.normal, str, (int)x, (int)y, Color.Black);
			m_lib.device.DrawTextR(font_type.normal, String.Format("{0} {1}/{2}", str2, current, max),
									(int)x + (int)size_x, (int)y, Color.Black);

			// プ로그レスバー
			float percent;
			if (max > 0) percent = (float)current / max;
			else percent = 0;
			m_lib.device.DrawFillRect(new Vector3(x, y + 16, 0.1f),
										new Vector2(size_x * percent, 8),
										color);
			m_lib.device.DrawLineRect(new Vector3(x, y + 16, 0.1f),
										new Vector2(size_x, 8), Color.Black.ToArgb());
		}
		#endregion

		#region 스레드
		/*-------------------------------------------------------------------------
		 지도읽기스레드
		---------------------------------------------------------------------------*/
		private void load_map_proc() {
			string[] map_name_tbl = new string[]{
				def.MAP_FULLFNAME1,
				def.MAP_FULLFNAME2,
			};
			string[] map_name_tbl2 = new string[]{
				def.MIX_MAP_FULLFNAME1,
				def.MIX_MAP_FULLFNAME2,
			};

			// 進捗현황取得용に初期化
			m_lib.loop_image.InitializeCreateImage();
			if (!m_db.SeaArea.IsLoadedMask) {
				m_db.SeaArea.InitializeFromMaskInfo();
			}

			// 지도の읽기
			m_map_index = m_lib.setting.map;
			m_use_mixed_map = m_lib.setting.use_mixed_map;

			if (m_use_mixed_map) {
				// 合成した지도を사용する

				// 合成してなければ合成する
				favoriteroute.MixMap(map_name_tbl[(int)m_map_index],
										def.FAVORITEROUTE_FULLFNAME,
										map_name_tbl2[(int)m_map_index],
										ImageFormat.Png);
				//										ImageFormat.Jpeg);
				//										ImageFormat.Bmp);

				if (File.Exists(map_name_tbl2[(int)m_map_index])) {
					// 항로도즐겨찾기合成후の지도がある
					m_lib.loop_image.CreateImage(map_name_tbl2[(int)m_map_index]);
				} else {
					// 항로도즐겨찾기合成후の지도がない
					m_lib.loop_image.CreateImage(map_name_tbl[(int)m_map_index]);
				}
			} else {
				// 항로도즐겨찾기合成후の지도を사용しない
				m_lib.loop_image.CreateImage(map_name_tbl[(int)m_map_index]);
			}

			// 해역군をマスクから작성
			if (!m_db.SeaArea.IsLoadedMask) {
				m_db.SeaArea.CreateFromMask(def.MAP_MASK_FULLFNAME);
			}

			// 지도合成リクエスト
			m_lib.setting.req_update_map.Request();

			// 종료후少しだけ100%표시
			Thread.Sleep(100);	  // 0.1초
			Debug.WriteLine("finish load map.");

			// 좌표변환
			//			debug_transform_pos(13283, 2173);
			//			debug_transform_pos(13086, 2797);
			//			debug_transform_pos(12714, 3565);
		}

		/*-------------------------------------------------------------------------
		 디테일정보읽기스레드
		---------------------------------------------------------------------------*/
		private void load_info_proc() {
			_LoadInfosStatus.Start(3, "인터넷에서 동맹항 현황 파악");

			if (m_lib.setting.connect_network) {
				m_db.World.DownloadDomains(def.LOCAL_NEW_DOMAINS_INDEX_FULLFNAME);
			}

			// 디테일정보읽기
			_LoadInfosStatus.IncStep("도시정보 가져오기");
			m_db.World.Load(def.INFO_FULLNAME, def.NEW_DOMAINS_INDEX_FULLFNAME, def.LOCAL_NEW_DOMAINS_INDEX_FULLFNAME);

			// 아이템DBとリンクさせる
			m_db.World.LinkItemDatabase(m_db.ItemDatabase);

			// Web아이콘읽기
			// ネットワークから取得するかどうかに関係なく読み込もうとする
			_LoadInfosStatus.IncStep("Web아이콘 가져오기");
			m_db.WebIcons.Load(def.WEB_ICONS_FULLFNAME);

			// 검색ダイア로그を표시する
			if (m_lib.setting.find_window_visible) m_req_show_find_form.Request();
			if (m_lib.setting.sea_routes_window_visible) m_req_sea_routes_form.Request();

			// 종료후少しだけ100%표시
			_LoadInfosStatus.IncStep("완료");
			Thread.Sleep(100);
			Debug.WriteLine("finish load info.");
		}

		/*-------------------------------------------------------------------------
		 로그분석스레드
		---------------------------------------------------------------------------*/
		private void chat_log_proc() {
			int total_sleep = 0;
			bool old_analize = true;
			while (!m_exit_thread_event.WaitOne(0, false)) {
				Thread.Sleep(200);
				total_sleep += 200;
				if (total_sleep > CHAT_LOG_TIMER_INTERVAL) {
					total_sleep -= CHAT_LOG_TIMER_INTERVAL;

					if ((m_lib.setting.is_server_mode)
						|| (!m_lib.setting.enable_analize_log_chat)
						|| (!m_lib.setting.save_searoutes)) {
						// 분석しなかった
						old_analize = false;
						continue;
					}

					// 로그분석유효時のみ
					m_db.GvoChat.AnalyzeNewestChatLog();	// 로그분석을실시
					m_db.GvoChat.Request();				 // 해역변동を反映させてもらうリクエスト

					// 前회분석しなかった場合は분석결과を捨てる
					// 해역변동は反映させる
					// 스레드の関係で捨て損ねることがある
					// それほど問題にはならないので放置してる
					if (!old_analize) {
						m_db.GvoChat.ResetAccident();
						m_db.GvoChat.ResetInterest();
						m_db.GvoChat.ResetBuildShip();
					}

					// 분석した
					old_analize = true;
				}
			}
			Debug.WriteLine("finish analize chat log.");
		}

		/*-------------------------------------------------------------------------
		 항로공유스레드
		---------------------------------------------------------------------------*/
		private void share_proc() {
#if DEBUG_SHARE_ROUTES
			m_db.share_routes.Share();
#else
			if (!m_myship_info.is_analized_pos) {
				// 본인의 위치が분からない
				m_db.ShareRoutes.Share(0, 0, ShareRoutes.State.outof_sea);
			} else {
				// 나침반の각도が得られていれば해상
				ShareRoutes.State _state = (m_myship_info.is_in_the_sea)
													? ShareRoutes.State.in_the_sea
													: ShareRoutes.State.outof_sea;
				m_db.ShareRoutes.Share((int)m_myship_info.pos.X, (int)m_myship_info.pos.Y, _state);
			}
#endif
		}
		#endregion

		#region タイマコールバック
		/*-------------------------------------------------------------------------
		 항로공유タイマコールバック
		 ついでに季節チェック을실시
		---------------------------------------------------------------------------*/
		private void share_timer_Tick(object sender, EventArgs e) {
			// 季節チェック업데이트
			if (m_db.GvoSeason.UpdateSeason()) {
				// 季節が変わった
				// 지도업데이트をリクエストする
				m_lib.setting.req_update_map.Request();
			}

#if !DEBUG_SHARE_ROUTES
			// 공유が유효でなければなにもしない
			if (!m_lib.setting.enable_share_routes) return;

			// 대항해시대Onlineが시작してなければ공유しない
			if (!gvo_capture.IsFoundGvoWindow()) return;

			// 스레드が動いている場合はなにもしない
			// 1분이상스레드が動いているのはなにか問題を抱えている
			if (is_share) return;
#endif
			if (m_exit_thread_event.WaitOne(0, false)) return;		  // 종료しようとしている

			// ネットワークへの연결は시간がかかるので스레드で行う
			m_share_t = new Thread(new ThreadStart(share_proc));
			m_share_t.Name = "share network";
			m_share_t.Start();
		}
		#endregion

		#region 마우스関係
		/*-------------------------------------------------------------------------
		 メイン윈도우내で마우스ボタン押す
		---------------------------------------------------------------------------*/
		private void MainWindowMouseDown(object sender, MouseEventArgs e) {
			Point pos = new Point(e.X, e.Y);

			// 今회の위치を覚えておく
			m_old_mouse_pos = pos;
			m_lib.device.SetMustDrawFlag();

			if (!m_item_window.HitTest_ItemList(pos)) {
				// コントロールのフォーカスを외す
				ActiveControl = null;
			}

			// 윈도우관리チェック
			if (m_windows.OnMouseDown(pos, e.Button)) {
				this.Capture = false;
				return;
			}

			// 정보표시윈도우관리チェック
			if (m_info_windows.HitTest(pos)) {
				this.Capture = false;
				return;
			}

			// 설정により動作が異なる
			if (m_lib.setting.compatible_windows_rclick) {
				// 우클릭でメニューが開く版

				// 선택등は좌클릭か우클릭
				if (((e.Button & MouseButtons.Left) != 0)
					|| ((e.Button & MouseButtons.Right) != 0)) {
					m_select_pos = transform.client_pos2_game_pos(pos, m_lib.loop_image);

					if (!m_spot.is_spot) {
						// 何かあれば선택する
						m_item_window.info = m_db.World.FindInfo(m_lib.loop_image.MousePos2GlobalPos(pos));
					}
				}
			} else {
				// Ctrl+우클릭でメニューが開く版

				// 선택등は좌클릭のみ
				if ((e.Button & MouseButtons.Left) != 0) {
					m_select_pos = transform.client_pos2_game_pos(pos, m_lib.loop_image);

					// 何かあれば선택する
					m_item_window.info = m_db.World.FindInfo(m_lib.loop_image.MousePos2GlobalPos(pos));
				}
			}

			// 마우스캡처ー開始
			this.Capture = true;
		}

		/*-------------------------------------------------------------------------
		 メイン윈도우내で마우스클릭
		---------------------------------------------------------------------------*/
		private void gvtrademap_cs_form_MouseClick(object sender, MouseEventArgs e) {
			Point pos = new Point(e.X, e.Y);

			// 今회の위치を覚えておく
			m_old_mouse_pos = pos;

			// 윈도우관리チェック
			if (m_windows.OnMouseClick(pos, e.Button)) {
				return;
			}

			// 정보표시윈도우관리チェック
			if (m_info_windows.OnMouseClick(pos, e.Button, this)) {
				return;
			}

			// 설정により動作が異なる
			if (m_lib.setting.compatible_windows_rclick) {
				// 우클릭でメニューが開く版
				// 메모아이콘
				if ((e.Button & MouseButtons.Right) != 0) {
					main_window_context_menu(pos);
					return;
				}
			} else {
				// Ctrl+우클릭でメニューが開く版
				// 메모아이콘
				if (((e.Button & MouseButtons.Right) != 0)
					&& ((user32.GetKeyState(user32.VK_CONTROL) & 0x8000) != 0)) {
					main_window_context_menu(pos);
					return;
				}
			}
		}

		/*-------------------------------------------------------------------------
		 メイン윈도우내で마우스ボタン離す
		---------------------------------------------------------------------------*/
		private void MainWindowMouseUp(object sender, MouseEventArgs e) {
			// 마우스캡처ー종료
			this.Capture = false;
		}

		/*-------------------------------------------------------------------------
		 メイン윈도우내で마우스이동
		---------------------------------------------------------------------------*/
		private void MainWindowMouseMove(object sender, MouseEventArgs e) {
			// 마우스캡처ー중なら
			if (this.Capture) {
				// 이동량を加算する
				// 지도이동용
				m_mouse_move.X += e.X - m_old_mouse_pos.X;
				m_mouse_move.Y += e.Y - m_old_mouse_pos.Y;
			}

			// 今회の위치を覚えておく
			m_old_mouse_pos.X = e.X;
			m_old_mouse_pos.Y = e.Y;
		}

		/*-------------------------------------------------------------------------
		 メイン윈도우내で마우스ダブル클릭
		---------------------------------------------------------------------------*/
		private void m_main_window_MouseDoubleClick(object sender, MouseEventArgs e) {
			// 윈도우관리が최초にチェック
			if (m_windows.OnMouseDoubleClick(new Point(e.X, e.Y), e.Button)) return;

			// 윈도우では処理されなかった
		}

		/*-------------------------------------------------------------------------
		 마우스ホイール
		---------------------------------------------------------------------------*/
		private void FormMouseWheel(object sender, MouseEventArgs e) {
			if (!is_inside_mouse_cursor_main_window()) return;

			// メイン윈도우내
			// 윈도우관리が최초にチェック
			Point client_mouse_pos = this.PointToClient(MousePosition);
			if (m_windows.OnMouseWheel(client_mouse_pos, e.Delta)) return;

			// 윈도우では処理されなかった
			// メイン윈도우の스케일변경	
			zoom_map((e.Delta > 0) ? true : false, client_mouse_pos);
		}
		#endregion

		#region 윈도우사이즈변경と이동
		/*-------------------------------------------------------------------------
		 윈도우사이즈변경
		---------------------------------------------------------------------------*/
		private void gvtrademap_cs_form_Resize(object sender, EventArgs e) {
			// InitializeComponent()내で呼ばれる事例が보고されたため, 
			// m_lib が작성されたかどうかをチェックする
			// このPCではInitializeComponent()내で呼ばれないため, デバッグ不가능
			if (m_lib == null) return;
			if (m_lib.setting == null) return;

			// ウィンドウが최소화あるいは표시されているかどうかをチェック
			m_pause = ((this.WindowState == FormWindowState.Minimized) || !this.Visible);

			// 설정정보を업데이트する
			update_windows_position();
		}

		/*-------------------------------------------------------------------------
		 윈도우이동
		---------------------------------------------------------------------------*/
		private void gvtrademap_cs_form_Move(object sender, EventArgs e) {
			// 설정정보を업데이트する
			update_windows_position();
		}

		/*-------------------------------------------------------------------------
		 설정정보용윈도우사이즈업데이트
		 통상時のみ업데이트される
		---------------------------------------------------------------------------*/
		private void update_windows_position() {
			// 최대化及び최소화でないときのみ
			if (this.WindowState != FormWindowState.Normal) return;

			if (m_lib == null) return;
			if (m_lib.setting == null) return;
			m_lib.setting.window_location = this.Location;
			m_lib.setting.window_size = this.Size;
		}
		#endregion

		#region 마우스훅관계
		/*-------------------------------------------------------------------------
		 마우스훅시작と종료
		 설정により시작と종료を切り替える
		 사용하지 않을 경우 후킹하지 않는 편이 좋다.
		---------------------------------------------------------------------------*/
		private void do_mouse_hook() {
			// 버그로 기능을 막아둠
			/*if (m_lib.setting.hook_mouse) {
				// 사용
				if (m_mouse_hook != null) return;	   // すでに시작중

				// 마우스훅開始
				m_mouse_hook = new globalmouse_hook();
			} else {
				// 未사용
				// 使っていれば훅を외す
				dispose_mouse_hook();
			}*/
		}

		/*-------------------------------------------------------------------------
		 마우스훅종료
		---------------------------------------------------------------------------*/
		private void dispose_mouse_hook() {
			if (m_mouse_hook == null) return;

			// 마우스훅を종료する
			m_mouse_hook.Dispose();
			m_mouse_hook = null;
		}
		#endregion

		#region ツールチップ関係
		/*-------------------------------------------------------------------------
		 ツールチップの업데이트
		---------------------------------------------------------------------------*/
		private void update_tooltip() {
			// ツールチップの위치が윈도우위치基準なのでLocationを基準とする
			Point mpos = new Point(MousePosition.X - Location.X,
									MousePosition.Y - Location.Y);

			if (m_show_tooltip) {
				// tooltip표시중
				if (is_show_menu_strip) {
					try {
						m_tooltip.Hide(this);
					} catch {
						//						MessageBox.Show("Hide()오류をキャッチ");
					}
					m_show_tooltip = false;
					m_tooltip_interval = 0;
				} else {
					Vector2 v1 = new Vector2(mpos.X, mpos.Y);
					Vector2 v2 = new Vector2(m_tooltip_old_mouse_pos.X, m_tooltip_old_mouse_pos.Y);

					v1 -= v2;
					if (v1.LengthSq() >= 8f * 8f) {	   // 8dot이상動いたら
														  // 표시종료
						try {
							m_tooltip.Hide(this);
						} catch {
							//							MessageBox.Show("Hide()오류をキャッチ");
						}
						m_show_tooltip = false;
						m_tooltip_old_mouse_pos = mpos;
					}
					m_tooltip_interval = 0;
				}
			} else {
				if (m_tooltip_old_mouse_pos == mpos) {
					if (is_show_menu_strip) {
						m_tooltip_interval = 0;
					} else {
						// 마우스の이동없음
						if (!is_inside_mouse_cursor_main_window()) {
							// 윈도우외
							m_tooltip_old_mouse_pos = mpos;
							m_tooltip_interval = 0;
						} else {
							if (++m_tooltip_interval >= TOOLTIP_INITIAL) {
								// クライアント좌표
								Point pos = this.PointToClient(MousePosition);
								// 아이템윈도우からツールチップを得る
								string str = m_windows.GetToolTipString(pos);
								// メイン윈도우からツールチップを得る
								if (str == null) str = get_tooltip_string(pos);
								if (str != null) {
									// なにかツールチップがあれば표시する
									// 마우스の그리기후の사이즈분ずらしたいが, 取得のしかたが분からない
									try {
										m_tooltip.Show(str, this, mpos.X + 10, mpos.Y, 40000000);
									} catch {
									}
									m_show_tooltip = true;
								} else {
									// ないのでスルー
									m_tooltip_interval = TOOLTIP_INITIAL;
								}
							}
						}
					}
				} else {
					// 마우스の이동あり
					m_tooltip_old_mouse_pos = mpos;
					m_tooltip_interval = 0;
				}
			}
		}

		/*-------------------------------------------------------------------------
		 ツールチップ문자열を得る
		 정보표시윈도우との判定
		 장소時は장소からのツールチップ문자열を得る
		---------------------------------------------------------------------------*/
		private string get_tooltip_string(Point pos) {
			// 정보표시윈도우
			string tip = m_info_windows.OnToolTipString(pos);

			// 장소
			Point gpos = m_lib.loop_image.MousePos2GlobalPos(pos);
			if (tip == null) tip = m_spot.GetToolTipString(gpos);

			// 메모아이콘
			if (tip == null) tip = m_db.MapMark.GetToolTip(gpos);

			// 도시명
			/*			if(m_lib.setting.map_draw_names == map_draw_names.draw){
							// 그리기する설정なのでポップアップしない
							return tip;
						}
						if(tip == null){
							GvoWorldInfo.Info		Info	= m_db.World.FindInfo(gpos);
							if(Info != null){
								if(Info.InfoType != GvoWorldInfo.InfoType.SEA){
									// 해역はポップアップしない
									tip	= Info.Name;
								}
							}
						}
			*/
			if (tip == null) {
				GvoWorldInfo.Info info = m_db.World.FindInfo(gpos);
				if (info != null) {
					tip = info.TooltipString;
					if (m_lib.setting.map_draw_names == MapDrawNames.Draw) {
						// 도시명を그리기する모드
						// 상륙지 등은 팝업하지 않음
						if (info.InfoType == GvoWorldInfo.InfoType.OutsideCity) return null;
						if (info.InfoType == GvoWorldInfo.InfoType.Shore) return null;
						if (info.InfoType == GvoWorldInfo.InfoType.Shore2) return null;
					}
				}
			}

			return tip;
		}
		#endregion

		#region 스크린샷
		/*-------------------------------------------------------------------------
		 스크린샷
		 ビデオ메모リからメイン메모リへの전送속도の関係で
		 フル사이즈だとかなり시간がかかる
		 
		 512*512の렌더링용サーフェイスを작성
		 렌더링
		 取り出し
		 連結
		 を必要な사이즈분行う
		---------------------------------------------------------------------------*/
		private void screen_shot() {
			const int RENDER_TARGET_SIZE_X = 512;
			const int RENDER_TARGET_SIZE_Y = 512;

			// 렌더링시작위치と사이즈
			Point tmp;
			Size size;
			m_db.SeaRoute.CalcScreenShotBoundingBox(out tmp, out size);
			if ((size.Width <= 0) || (size.Height <= 0)) {
				MessageBox.Show("항로정보がないため, SSを작성できません. ",
								"보고", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			Device device = m_lib.device.device;
			Surface depth = device.DepthStencilSurface;
			Surface backbuffer = device.GetBackBuffer(0, 0, BackBufferType.Mono);
			Surface rendertarget = null;
			Surface offscreen = null;
			Surface rendertarget_depth = null;

			try {
				// 렌더링 타겟
				rendertarget = device.CreateRenderTarget(RENDER_TARGET_SIZE_X, RENDER_TARGET_SIZE_Y,
																Format.R5G6B5, MultiSampleType.None, 0, false);
				// メイン메모リ상のサーフェイス
				// 렌더링결과取り出し용
				offscreen = device.CreateOffscreenPlainSurface(RENDER_TARGET_SIZE_X, RENDER_TARGET_SIZE_Y,
																		Format.R5G6B5, Pool.SystemMemory);
				// 深도バッファ
				rendertarget_depth = device.CreateDepthStencilSurface(RENDER_TARGET_SIZE_X, RENDER_TARGET_SIZE_Y,
																		DepthFormat.D16, MultiSampleType.None, 0, false);
			} catch {
				// 렌더링용バッファの작성に실패
				if (rendertarget != null) rendertarget.Dispose();
				if (offscreen != null) offscreen.Dispose();
				if (rendertarget_depth != null) rendertarget_depth.Dispose();

				MessageBox.Show("스크린샷 저장에 실패하였습니다. \n스크린샷 정보 작성에 실패하였습니다. ",
								"보고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			Cursor = Cursors.WaitCursor;

			// 그리기대상を변경する
			device.DepthStencilSurface = rendertarget_depth;
			device.SetRenderTarget(0, rendertarget);

			// 그리기위치と스케일を退避
			m_lib.loop_image.PushDrawParams();

			Vector2 offset = new Vector2(tmp.X, tmp.Y);
			UInt16[] buffer = new UInt16[size.Width * size.Height];

			// 스케일は등배
			m_lib.loop_image.SetScale(1, new Point(0, 0), false);

			Vector2 pos = new Vector2(0, 0);

			// 렌더링후, 取り出し連結する
			for (; pos.Y < size.Height; pos.Y += RENDER_TARGET_SIZE_Y) {
				for (pos.X = 0; pos.X < size.Width; pos.X += RENDER_TARGET_SIZE_X) {
					// オフセット설정
					m_lib.loop_image.OffsetPosition = -(offset + pos);
					// 렌더링
					screen_shot_draw();
					// 결과を取りだす
					// 렌더링종료まで待たされる
					SurfaceLoader.FromSurface(offscreen, rendertarget, Filter.None, 0);
					// 連結する
					screen_shot_chain_image(buffer,
											size.Width, size.Height, size.Width,
											(int)pos.X, (int)pos.Y,
											offscreen);
				}
			}

			// 그리기위치と스케일を元に戻す
			m_lib.loop_image.PopDrawParams();

			// 그리기대상を元に戻す
			device.DepthStencilSurface = depth;
			device.SetRenderTarget(0, backbuffer);
			m_lib.device.UpdateClientSize();

			// 確保したバッファの解放
			rendertarget.Dispose();
			offscreen.Dispose();
			rendertarget_depth.Dispose();
			depth.Dispose();
			backbuffer.Dispose();

			try {
				// UInt16[] を bitmapに변환して jpg で書きだし
				GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
				Bitmap bitmap = new Bitmap(size.Width, size.Height, size.Width * 2,
													PixelFormat.Format16bppRgb565,
													handle.AddrOfPinnedObject());
				string fname = "searoute" + DateTime.Now.ToString("yyyyMMddHHmmss");
				switch (m_lib.setting.ss_format) {
					case SSFormat.Png:
						fname = Path.Combine(m_current_path, def.SS_PATH + fname + ".png");
						bitmap.Save(fname, ImageFormat.Png);
						break;
					case SSFormat.Jpeg:
						fname = Path.Combine(m_current_path, def.SS_PATH + fname + ".jpg");
						bitmap.Save(fname, ImageFormat.Jpeg);
						break;
					case SSFormat.Bmp:
					default:
						fname = Path.Combine(m_current_path, def.SS_PATH + fname + ".bmp");
						bitmap.Save(fname, ImageFormat.Bmp);
						break;
				}
				handle.Free();
				bitmap.Dispose();
				buffer = null;
				System.GC.Collect();

				Cursor = Cursors.Default;
				MessageBox.Show("스크린샷을 저장하였습니다. \n" + fname,
								"보고", MessageBoxButtons.OK, MessageBoxIcon.Information);
				Process.Start(Path.Combine(m_current_path, def.SS_PATH));
			} catch {
				Cursor = Cursors.Default;
				buffer = null;
				System.GC.Collect();
				MessageBox.Show("스크린샷 저장에 실패하였습니다. \n파일 작성에 실패하였습니다. ",
								"보고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/*-------------------------------------------------------------------------
		 스크린샷
		 그리기
		 지도と항로도のみ
		 항로도は표시설정がそのまま反映される
		---------------------------------------------------------------------------*/
		private void screen_shot_draw() {
			// 화면のクリア
			m_lib.device.Clear(Color.Black);

			// 그리기開始
			if (!m_lib.device.Begin()) return;	  // デバイスロスト중

			// 지도그리기
			draw_map();

			// GvoDatabase
			m_db.DrawForScreenShot();
			m_lib.device.End();
		}

		/*-------------------------------------------------------------------------
		 스크린샷
		 得られたイメージを連結する
		 strideは통상size_xと同じ値となる
		---------------------------------------------------------------------------*/
		private void screen_shot_chain_image(UInt16[] image, int size_x, int size_y, int stride, int offset_x, int offset_y, Surface offscreen) {
			// 전送先の矩形외のときはそのまま帰る
			if (offset_x >= size_x) return;
			if (offset_y >= size_y) return;
			if (offset_x + offscreen.Description.Width < 0) return;
			if (offset_y + offscreen.Description.Height < 0) return;

			// ロック
			int pitch;
			UInt16[,] buf = (UInt16[,])offscreen.LockRectangle(typeof(UInt16), LockFlags.ReadOnly, out pitch, offscreen.Description.Height, offscreen.Description.Width);

			// 전送先の矩形に収まるように사이즈を수정する
			int src_size_x = offscreen.Description.Width;
			int src_size_y = offscreen.Description.Height;
			if (offset_x + src_size_x > size_x) src_size_x -= (offset_x + src_size_x) - size_x;
			if (offset_y + src_size_y > size_y) src_size_y -= (offset_y + src_size_y) - size_y;

			// マイナスオフセットに대응
			int start_x = 0;
			int start_y = 0;
			if (offset_x < 0) {
				start_x = -offset_x;
				offset_x = 0;
				src_size_x -= start_x;
			}
			if (offset_y < 0) {
				start_y = -offset_y;
				offset_y = 0;
				src_size_y -= start_y;
			}

			// 전送
			int index = (offset_y * stride) + offset_x;
			for (int y = 0; y < src_size_y; y++) {
				for (int x = 0; x < src_size_x; x++) {
					image[index + x] = buf[start_y + y, start_x + x];
				}
				index += stride;
			}

			// ロック해제
			offscreen.UnlockRectangle();
		}
		#endregion

		#region 동맹국변경용メニュー関係
		/*-------------------------------------------------------------------------
		 동맹국변경용メニューを開く
		---------------------------------------------------------------------------*/
		public void ShowChangeDomainsMenuStrip(Point pos) {
			contextMenuStrip1.Show(this, pos);
		}

		/*-------------------------------------------------------------------------
		 동맹국변경
		---------------------------------------------------------------------------*/
		private void ToolStripMenuItem_country0_Click(object sender, EventArgs e) { set_domain(GvoWorldInfo.Country.England); }
		private void ToolStripMenuItem_country1_Click(object sender, EventArgs e) { set_domain(GvoWorldInfo.Country.Spain); }
		private void ToolStripMenuItem_country2_Click(object sender, EventArgs e) { set_domain(GvoWorldInfo.Country.Portugal); }
		private void ToolStripMenuItem_country3_Click(object sender, EventArgs e) { set_domain(GvoWorldInfo.Country.Netherlands); }
		private void ToolStripMenuItem_country4_Click(object sender, EventArgs e) { set_domain(GvoWorldInfo.Country.France); }
		private void ToolStripMenuItem_country5_Click(object sender, EventArgs e) { set_domain(GvoWorldInfo.Country.Venezia); }
		private void ToolStripMenuItem_country6_Click(object sender, EventArgs e) { set_domain(GvoWorldInfo.Country.Turkey); }
		private void ToolStripMenuItem_country00_Click(object sender, EventArgs e) { set_domain(GvoWorldInfo.Country.Unknown); }

		/*-------------------------------------------------------------------------
		 동맹국변경
		---------------------------------------------------------------------------*/
		private void set_domain(GvoWorldInfo.Country country) {
			if (m_item_window.info == null) return;

			// 업데이트
			if (!m_db.World.SetDomain(m_item_window.info.Name, country)) return;

			// 업데이트された
			string str = m_db.World.GetNetUpdateString(m_item_window.info.Name);
			if (str == null) return;

			// 서버に업데이트정보を送る
			// オフ라인모드時は送らない
			if (!m_lib.setting.connect_network) return;

			// どうも변경できなくなってる?
			// オリジナル교역Mapでも변경できない
			//			str	= Useful.UrlEncodeShiftJis(str);
			Console.WriteLine("동맹국변경:" + HttpDownload.Download(def.URL_HP_ORIGINAL + @"/gvgetdomain.cgi?" + str, Encoding.UTF8));

			//			str				= "/gvgetdomain.cgi?" + str;
			//			string	Data	= tcp_text.Download("gvtrademap.daa.jp", str, Encoding.UTF8);
		}
		#endregion

		#region メイン윈도우 우클릭メニュー
		/*-------------------------------------------------------------------------
		 メイン윈도우
		 우클릭メニュー
		---------------------------------------------------------------------------*/
		private void main_window_context_menu(Point p) {
			m_memo_icon_pos = m_lib.loop_image.MousePos2GlobalPos(p);
			m_memo_icon_data = m_db.MapMark.FindData(m_memo_icon_pos);

			if (m_memo_icon_data == null) {
				edit_memo_icon_ToolStripMenuItem.Enabled = false;
				remove_memo_icon_ToolStripMenuItem.Enabled = false;
			} else {
				edit_memo_icon_ToolStripMenuItem.Enabled = true;
				remove_memo_icon_ToolStripMenuItem.Enabled = true;
			}

			// 해역변동시스템の업데이트
			string name = m_db.SeaArea.Find(m_memo_icon_pos);
			if (name == null) {
				normal_sea_area_ToolStripMenuItem.Text = "--을 위험해역(통상상태)으로 설정";
				normal_sea_area_ToolStripMenuItem.Enabled = false;
				safty_sea_area_ToolStripMenuItem.Text = "--을 안전해역으로 설정";
				safty_sea_area_ToolStripMenuItem.Enabled = false;
				lawless_sea_area_ToolStripMenuItem.Text = "--을 무법해역으로 설정";
				lawless_sea_area_ToolStripMenuItem.Enabled = false;
			} else {
				normal_sea_area_ToolStripMenuItem.Text = name + "을 위험해역(통상상태)으로 설정";
				normal_sea_area_ToolStripMenuItem.Enabled = true;
				safty_sea_area_ToolStripMenuItem.Text = name + "을 안전해역으로 설정";
				safty_sea_area_ToolStripMenuItem.Enabled = true;
				lawless_sea_area_ToolStripMenuItem.Text = name + "을 무법해역으로 설정";
				lawless_sea_area_ToolStripMenuItem.Enabled = true;
			}
			contextMenuStrip2.Show(this, p);
		}

		/*-------------------------------------------------------------------------
		 메모아이콘
		 목적지아이콘を추가
		---------------------------------------------------------------------------*/
		private void set_target_memo_icon_ToolStripMenuItem_Click(object sender, EventArgs e) {
			m_db.MapMark.Add(m_memo_icon_pos,
							map_mark.map_mark_type.icon11,
							"목적지주변입니다");
			m_memo_icon_data = null;

			// 추가されたのに비표시だとあれなので強制표시とする
			m_lib.setting.draw_icons = true;
		}

		/*-------------------------------------------------------------------------
		 메모아이콘
		 메모아이콘を추가
		---------------------------------------------------------------------------*/
		private void add_memo_icon_ToolStripMenuItem_Click(object sender, EventArgs e) {
			using (map_mark_form dlg = new map_mark_form(transform.map_pos2_game_pos(m_memo_icon_pos, m_lib.loop_image))) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					// 추가する
					m_db.MapMark.Add(transform.game_pos2_map_pos(dlg.position, m_lib.loop_image),
									map_mark.map_mark_type.axis0 + dlg.icon_index,
									dlg.memo);

					// 추가されたのに비표시だとあれなので強制표시とする
					m_lib.setting.draw_icons = true;
				}
			}
			m_memo_icon_data = null;
		}

		/*-------------------------------------------------------------------------
		 메모아이콘
		 메모아이콘を編集
		---------------------------------------------------------------------------*/
		private void edit_memo_icon_ToolStripMenuItem_Click(object sender, EventArgs e) {
			if (m_memo_icon_data == null) return;

			using (map_mark_form dlg = new map_mark_form(m_memo_icon_data.gposition, (int)m_memo_icon_data.type, m_memo_icon_data.memo)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					m_memo_icon_data.position = transform.game_pos2_map_pos(dlg.position, m_lib.loop_image);
					m_memo_icon_data.type = map_mark.map_mark_type.axis0 + dlg.icon_index;
					m_memo_icon_data.memo = dlg.memo;

					// 추가されたのに비표시だとあれなので強制표시とする
					m_lib.setting.draw_icons = true;
				}
			}
			m_memo_icon_data = null;		// 참조を切る
		}

		/*-------------------------------------------------------------------------
		 메모아이콘
		 메모아이콘を삭제
		---------------------------------------------------------------------------*/
		private void remove_memo_icon_ToolStripMenuItem_Click(object sender, EventArgs e) {
			if (m_memo_icon_data == null) return;
			m_db.MapMark.RemoveData(m_memo_icon_data);
			m_memo_icon_data = null;		// 참조を切る
		}

		/*-------------------------------------------------------------------------
		 메모아이콘
		 전체목적지메모아이콘を삭제
		---------------------------------------------------------------------------*/
		private void remove_all_target_memo_icon_ToolStripMenuItem_Click(object sender, EventArgs e) {
			m_db.MapMark.RemoveAllTargetData();
			m_memo_icon_data = null;

			// 비표시だとあれなので強制표시とする
			m_lib.setting.draw_icons = true;
		}

		/*-------------------------------------------------------------------------
		 메모아이콘
		 전부삭제
		---------------------------------------------------------------------------*/
		private void remove_all_memo_icon_ToolStripMenuItem_Click(object sender, EventArgs e) {
			m_db.MapMark.RemoveAllData();
			m_memo_icon_data = null;
		}
		/*-------------------------------------------------------------------------
		 마우스위치の해역군を위험해역に설정する
		---------------------------------------------------------------------------*/
		private void normal_sea_area_ToolStripMenuItem_Click(object sender, EventArgs e) {
			set_sea_area_rclick(sea_area.sea_area_once.sea_type.normal);
		}
		/*-------------------------------------------------------------------------
		 마우스위치の해역군を안전해역に설정する
		---------------------------------------------------------------------------*/
		private void safty_sea_area_ToolStripMenuItem_Click(object sender, EventArgs e) {
			set_sea_area_rclick(sea_area.sea_area_once.sea_type.safty);
		}
		/*-------------------------------------------------------------------------
		 마우스위치の해역군を무법해역に설정する
		---------------------------------------------------------------------------*/
		private void lawless_sea_area_ToolStripMenuItem_Click(object sender, EventArgs e) {
			set_sea_area_rclick(sea_area.sea_area_once.sea_type.lawless);
		}
		/*-------------------------------------------------------------------------
		 마우스위치の해역군を설정する
		---------------------------------------------------------------------------*/
		private void set_sea_area_rclick(sea_area.sea_area_once.sea_type type) {
			m_db.SeaArea.SetType(m_db.SeaArea.Find(m_memo_icon_pos),
									type);
		}
		#endregion

		#region 아이템목록 우클릭
		/*-------------------------------------------------------------------------
		 아이템목록を클릭
		 現状우클릭のみ
		---------------------------------------------------------------------------*/
		private void listView1_MouseClick(object sender, MouseEventArgs e) {
			if ((e.Button & MouseButtons.Right) == 0) return;	   // 우클릭のみ

			GvoWorldInfo.Info.Group.Data d = get_selected_item();
			if (d == null) return;

			// 표시위치조정
			Point pos = new Point(e.X, e.Y);

			ItemDatabaseCustom.Data db = d.ItemDb;
			if (db == null) {
				// 아이템DBと一致しない
				open_recipe_wiki0_ToolStripMenuItem.Enabled = false;
				open_recipe_wiki1_ToolStripMenuItem.Enabled = false;
				copy_all_to_clipboardToolStripMenuItem.Enabled = false;
			} else {
				copy_all_to_clipboardToolStripMenuItem.Enabled = true;
				if (db.IsSkill || db.IsReport) {
					// 스킬か보고
					open_recipe_wiki0_ToolStripMenuItem.Enabled = false;
					open_recipe_wiki1_ToolStripMenuItem.Enabled = false;
				} else {
					if (db.IsRecipe) {
						// 레시피
						open_recipe_wiki0_ToolStripMenuItem.Enabled = true;
						open_recipe_wiki1_ToolStripMenuItem.Enabled = false;
					} else {
						// 레시피以외
						open_recipe_wiki0_ToolStripMenuItem.Enabled = false;
						open_recipe_wiki1_ToolStripMenuItem.Enabled = true;
					}
				}
			}
			contextMenuStrip3.Show(listView1, pos);
		}

		/*-------------------------------------------------------------------------
		 레시피정보wikiを開く
		 레시피검색
		---------------------------------------------------------------------------*/
		private void open_recipe_wiki0_toolStripMenuItem_Click(object sender, EventArgs e) {
			GvoWorldInfo.Info.Group.Data item = get_selected_item();
			if (item == null) return;
			ItemDatabaseCustom.Data db = item.ItemDb;
			if (db == null) return;

			db.OpenRecipeWiki0();
		}

		/*-------------------------------------------------------------------------
		 레시피정보wikiを開く
		 작성가능かどうか검색
		---------------------------------------------------------------------------*/
		private void open_recipe_wiki1_ToolStripMenuItem_Click(object sender, EventArgs e) {
			GvoWorldInfo.Info.Group.Data item = get_selected_item();
			if (item == null) return;
			ItemDatabaseCustom.Data db = item.ItemDb;
			if (db == null) return;

			db.OpenRecipeWiki1();
		}

		/*-------------------------------------------------------------------------
		 명칭をクリップボードにコピーする
		---------------------------------------------------------------------------*/
		private void copy_name_to_clipboardToolStripMenuItem_Click(object sender, EventArgs e) {
			GvoWorldInfo.Info.Group.Data item = get_selected_item();
			if (item == null) return;
			ItemDatabaseCustom.Data db = item.ItemDb;
			if (db == null) {
				// 아이템DBにないので아이템명をコピーする
				Clipboard.SetText(item.Name);
			} else {
				// 아이템DBの명칭を使う
				// 아이템DBの명칭が正しい
				Clipboard.SetText(db.Name);
			}
		}

		/*-------------------------------------------------------------------------
		 디테일をクリップボードにコピーする
		---------------------------------------------------------------------------*/
		private void copy_all_to_clipboardToolStripMenuItem_Click(object sender, EventArgs e) {
			GvoWorldInfo.Info.Group.Data item = get_selected_item();
			if (item == null) return;
			Clipboard.SetText(item.TooltipString);
		}

		/*-------------------------------------------------------------------------
		 선택されている아이템を得る
		---------------------------------------------------------------------------*/
		private GvoWorldInfo.Info.Group.Data get_selected_item() {
			if (listView1.SelectedItems.Count <= 0) return null;

			ListViewItem item = listView1.SelectedItems[0];
			if (item.Tag == null) return null;
			return (GvoWorldInfo.Info.Group.Data)item.Tag;
		}

		/*-------------------------------------------------------------------------
		 장소표시
		---------------------------------------------------------------------------*/
		private void spotToolStripMenuItem1_Click(object sender, EventArgs e) {
			GvoWorldInfo.Info.Group.Data d = get_selected_item();
			if (d == null) return;
			m_spot.SetSpot(spot.type.has_item, d.Name);
			UpdateSpotList();
		}

		/*-------------------------------------------------------------------------
		 장소표시해제
		---------------------------------------------------------------------------*/
		private void reset_spot() {
			m_spot.SetSpot(spot.type.none, "");
			UpdateSpotList();
		}
		#endregion

		#region 드래그＆ドロップ
		/*-------------------------------------------------------------------------
		 DragEnter
		---------------------------------------------------------------------------*/
		private void gvtrademap_cs_form_DragEnter(object sender, DragEventArgs e) {
			// Textのドロップを受け入れる
			if (e.Data.GetDataPresent(DataFormats.Text)) {
				e.Effect = DragDropEffects.Copy;
			} else {
				e.Effect = DragDropEffects.None;
			}
		}

		/*-------------------------------------------------------------------------
		 DragDrop
		---------------------------------------------------------------------------*/
		private void gvtrademap_cs_form_DragDrop(object sender, DragEventArgs e) {
			AllowDrop = false;
			try {
				string data = e.Data.GetData(DataFormats.Text) as string;

				List<sea_area_once_from_dd> list = sea_area.AnalizeFromDD(data);
				sea_area_dd_form dlg = new sea_area_dd_form(m_lib, m_db, list);
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					// 反映させる
					// フィルタの내용を反映した목록を사용する
					// 해역정보は전부初期化してから反映される
					m_db.SeaArea.UpdateFromDD(dlg.filterd_list, true);
				}
			} catch {
				MessageBox.Show("해역정보受け取りに실패하였습니다. ");
			}
			AllowDrop = true;
		}
		#endregion

		#region 定例処理関係
		/*-------------------------------------------------------------------------
		 장소リクエストを実行する
		---------------------------------------------------------------------------*/
		private void do_spot_request() {
			if (m_lib.setting.req_spot_item.IsRequest()) {
				// 장소開始
				GvoDatabase.Find item = m_lib.setting.req_spot_item.Arg1 as GvoDatabase.Find;
				m_item_window.SpotItem(item);
			} else if (m_lib.setting.req_spot_item_changed.IsRequest()) {
				// 장소대상の변경
				spot.spot_once item = m_lib.setting.req_spot_item_changed.Arg1 as spot.spot_once;
				m_item_window.SpotItemChanged(item);
			}
		}

		/*-------------------------------------------------------------------------
		 지정된위치をセンタリング
		---------------------------------------------------------------------------*/
		private void centering_pos(Point pos) {
			Point offset = new Point(0, 0);
			if (m_item_window.window_mode == d3d_windows.window.mode.normal) {
				offset = new Point((int)(m_item_window.pos.X + m_item_window.size.X), 0);
			}
			m_lib.loop_image.MoveCenterOffset(pos, offset);
			m_lib.device.SetMustDrawFlag();
		}
		#endregion

		#region 키입력관계
		/*-------------------------------------------------------------------------
		 키が押された
		---------------------------------------------------------------------------*/
		private void gvtrademap_cs_form_KeyDown(object sender, KeyEventArgs e) {
#if DEBUG_COMPASS
			// 나침반분석デバッグ용
			switch(e.KeyData){
			case Keys.Up:
				m_db.capture.m_angle_x	-= 0.5f;
				break;
			case Keys.Down:
				m_db.capture.m_angle_x	+= 0.5f;
				break;
			case Keys.Left:
				m_db.capture.m_factor	-= 0.5f;
				break;
			case Keys.Right:
				m_db.capture.m_factor	+= 0.5f;
				break;
			case Keys.NumPad1:
				m_db.capture.m_l		-= 0.5f;
				break;
			case Keys.NumPad2:
				m_db.capture.m_l		+= 0.5f;
				break;
			}
#endif
		}

		/*-------------------------------------------------------------------------
		 Cmd実行
		---------------------------------------------------------------------------*/
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			// フォーカスがTextBoxのときは優先的にTextBoxに키を渡す
			if (this.ActiveControl != textBox1) {
				// 키할당관리を呼ぶ
				if (m_lib.KeyAssignManager.ProcessCmdKey(keyData)) {
					// 키할당관리で処理された
					return true;
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		/*-------------------------------------------------------------------------
		 Cmd実行コールバック
		---------------------------------------------------------------------------*/
		private void process_cnd_key(object sender, KeyAssignEventArg arg) {
			if (!(arg.Tag is KeyFunction)) return;
			ExecFunction((KeyFunction)arg.Tag);
		}
		#endregion

		#region 기능の実行
		/*-------------------------------------------------------------------------
		 기능の実行
		---------------------------------------------------------------------------*/
		public void ExecFunction(KeyFunction func) {
			switch (func) {
				case KeyFunction.map_change:
					// 지도바꾸기
					if (!is_load) {
						if (++m_lib.setting.map >= MapIndex.Max) {
							m_lib.setting.map = MapIndex.Map1;
						}
					}
					break;
				case KeyFunction.map_reset_scale:
					// 스케일の리셋
					// メイン윈도우の중心を중心に스케일변경
					m_lib.loop_image.SetScale(1, client_center(), true);
					m_lib.device.SetMustDrawFlag();
					break;
				case KeyFunction.blue_line_reset:
					// 각도계산の리셋
					m_db.SpeedCalculator.ResetAngle();
					m_lib.device.SetMustDrawFlag();
					break;
				case KeyFunction.map_zoom_in:
					// 拡대
					zoom_map(true, client_center());
					break;
				case KeyFunction.map_zoom_out:
					// 縮소
					zoom_map(false, client_center());
					break;
				case KeyFunction.window_top_most:
					// 最前面표시切り替え
					m_lib.setting.window_top_most = !m_lib.setting.window_top_most;
					break;
				case KeyFunction.window_maximize:
					// 최대化
					switch (this.WindowState) {
						case FormWindowState.Normal:
							this.WindowState = FormWindowState.Maximized;
							break;
						case FormWindowState.Maximized:
							this.WindowState = FormWindowState.Normal;
							break;
						case FormWindowState.Minimized:
							this.WindowState = FormWindowState.Normal;
							break;
					}
					break;
				case KeyFunction.window_minimize:
					// 최소화
					if (this.WindowState != FormWindowState.Minimized) {
						this.WindowState = FormWindowState.Minimized;
					}
					break;
				case KeyFunction.folder_open_01:
					// 로그폴더を開く
					Process.Start(gvo_def.GetGvoLogPath());
					break;
				case KeyFunction.folder_open_02:
					// メール폴더を開く
					Process.Start(gvo_def.GetGvoMailPath());
					break;
				case KeyFunction.folder_open_03:
					// 스크린샷폴더を開く
					Process.Start(gvo_def.GetGvoScreenShotPath());
					break;
				case KeyFunction.folder_open_00:
					// 항로도の스크린샷폴더を開く
					Process.Start(Path.Combine(m_current_path, def.SS_PATH));
					break;
				case KeyFunction.cancel_spot:
					// 장소の중止
					reset_spot();
					break;
				case KeyFunction.cancel_select_sea_routes:
					// 항로도の선택を리셋する
					m_db.SeaRoute.ResetSelectFlag();
					m_lib.device.SetMustDrawFlag();
					break;
				case KeyFunction.item_window_show_min:
					// 아이템윈도우の표시, 최소화
					m_item_window.ToggleWindowMode();
					m_lib.device.SetMustDrawFlag();
					break;
				case KeyFunction.item_window_next_tab:
					// 아이템윈도우の次のタブ
					m_item_window.ChangeTab(true);
					break;
				case KeyFunction.item_window_prev_tab:
					// 아이템윈도우の前のタブ
					m_item_window.ChangeTab(false);
					break;
				case KeyFunction.setting_window_show_min:
					// 설정윈도우の표시, 최소화
					m_setting_window.ToggleWindowMode();
					m_lib.device.SetMustDrawFlag();
					break;
				case KeyFunction.setting_window_button_00:
					m_lib.setting.save_searoutes = (m_lib.setting.save_searoutes) ? false : true;
					m_lib.device.SetMustDrawFlag();
					break;
				case KeyFunction.setting_window_button_01:
					m_lib.setting.draw_share_routes = (m_lib.setting.draw_share_routes) ? false : true;
					m_lib.device.SetMustDrawFlag();
					break;
				case KeyFunction.setting_window_button_02:
					m_lib.setting.draw_web_icons = (m_lib.setting.draw_web_icons) ? false : true;
					m_lib.device.SetMustDrawFlag();
					break;
				case KeyFunction.setting_window_button_03:
					m_lib.setting.draw_icons = (m_lib.setting.draw_icons) ? false : true;
					m_lib.device.SetMustDrawFlag();
					break;
				case KeyFunction.setting_window_button_04:
					m_lib.setting.draw_sea_routes = (m_lib.setting.draw_sea_routes) ? false : true;
					m_lib.device.SetMustDrawFlag();
					break;
				case KeyFunction.setting_window_button_05:
					if (m_lib.setting.draw_popup_day_interval == 0) m_lib.setting.draw_popup_day_interval = 1;
					else if (m_lib.setting.draw_popup_day_interval == 1) m_lib.setting.draw_popup_day_interval = 5;
					else m_lib.setting.draw_popup_day_interval = 0;
					m_lib.device.SetMustDrawFlag();
					break;
				case KeyFunction.setting_window_button_06:
					m_lib.setting.draw_accident = (m_lib.setting.draw_accident) ? false : true;
					m_lib.device.SetMustDrawFlag();
					break;
				case KeyFunction.setting_window_button_07:
					m_lib.setting.center_myship = (m_lib.setting.center_myship) ? false : true;
					m_lib.device.SetMustDrawFlag();
					break;
				case KeyFunction.setting_window_button_08:
					m_lib.setting.draw_myship_angle = (m_lib.setting.draw_myship_angle) ? false : true;
					m_lib.device.SetMustDrawFlag();
					break;
				case KeyFunction.setting_window_button_09:
					// 위험해역변동시스템설정
					edit_sea_area_dlg();
					break;
				case KeyFunction.setting_window_button_10:
					// 스크린샷リクエスト
					m_lib.setting.req_screen_shot.Request();
					break;
				case KeyFunction.setting_window_button_11:
					// 항로목록표시
					show_sea_routes_dialog();
					break;
				case KeyFunction.setting_window_button_12:
					// 검색ダイア로그を開く
					show_find_dialog();
					break;
				case KeyFunction.setting_window_button_13:
					// 설정ダイア로그を開く
					do_setting_dlg();
					break;
				case KeyFunction.setting_window_button_exec_gvoac:
					// 해역정보수집を시작する
					try {
						Process.Start(def.SEAAREA_APP_FNAME);
					} catch {
						MessageBox.Show("해역정보수집の시작に실패하였습니다. ", "시작오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					break;
				case KeyFunction.window_change_border_style:
					// 윈도우枠の표시/비표시
					{
						Size size = this.Size;
						this.FormBorderStyle = (this.FormBorderStyle == FormBorderStyle.None)
													? FormBorderStyle.Sizable
													: FormBorderStyle.None;
						this.Size = size;
						m_lib.setting.is_border_style_none = this.FormBorderStyle == FormBorderStyle.None;
					}
					break;
				case KeyFunction.window_close:
					// 종료
					this.Close();
					break;
			}
		}

		/*-------------------------------------------------------------------------
		 지도の拡縮
		---------------------------------------------------------------------------*/
		private void zoom_map(bool is_zoom_in, Point center) {
			// メイン윈도우の스케일변경	
			float scale;
			if (is_zoom_in) scale = m_lib.loop_image.ImageScale * 1.189f;
			else scale = m_lib.loop_image.ImageScale * 0.841f;

			// 적당に丸めておく
			scale = (float)Math.Round((double)scale / 5, 2) * 5;
			m_lib.loop_image.SetScale(scale, center, true);

			m_lib.device.SetMustDrawFlag();
		}

		/*-------------------------------------------------------------------------
		 クライアント矩形の중心を得る
		---------------------------------------------------------------------------*/
		private Point client_center() {
			Rectangle rect = this.ClientRectangle;
			return new Point(rect.Width / 2, rect.Height / 2);
		}

		/*-------------------------------------------------------------------------
		 검색ダイア로그を표시する
		---------------------------------------------------------------------------*/
		private void show_find_dialog() {
			show_find_dialog(true);
		}
		private void show_find_dialog(bool is_active_find_mode) {
			if (is_active_find_mode) m_find_form.SetFindMode();

			if (!m_find_form.Visible) {
				// 표시されていないので표시する
				m_find_form.Show(this);
			} else {
				// 표시されているのでアクティブにする
				// 검색時のみで장소時はフォーカスを移さない
				if (is_active_find_mode) m_find_form.Activate();
			}
			m_lib.device.SetMustDrawFlag();
		}

		/*-------------------------------------------------------------------------
		 항로도목록ダイア로그を표시する
		---------------------------------------------------------------------------*/
		private void show_sea_routes_dialog() {
			if (!m_sea_routes_form.Visible) {
				m_sea_routes_form.Show(this);
			} else {
				m_sea_routes_form.Activate();
			}
		}

		/*-------------------------------------------------------------------------
		 위험해역변경시스템설정ダイア로그を開く
		---------------------------------------------------------------------------*/
		private void edit_sea_area_dlg() {
			using (setting_sea_area_form dlg = new setting_sea_area_form(m_db.SeaArea)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					// 업데이트する
					dlg.Update(m_db.SeaArea);
				}
			}
		}

		/*-------------------------------------------------------------------------
		 설정ダイア로그を開く
		---------------------------------------------------------------------------*/
		private void do_setting_dlg() {
			string info = m_lib.device.deviec_info_string;
			using (setting_form2 dlg = new setting_form2(m_lib.setting, m_lib.KeyAssignManager.List, info)) {
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					// 설정항목を反映させる
					UpdateSettings(dlg);
				}
			}
		}

		/*-------------------------------------------------------------------------
		 설정내용を업데이트する
		---------------------------------------------------------------------------*/
		public void UpdateSettings(setting_form2 dlg) {
			m_lib.setting.Clone(dlg.setting);
			m_lib.KeyAssignManager.List = dlg.KeyAssignList;
		}

		/*-------------------------------------------------------------------------
		 장소목록を표시する
		 장소목록の업데이트も含む
		---------------------------------------------------------------------------*/
		public void UpdateSpotList() {
			if (m_spot == null) return;
			if (m_find_form == null) return;

			if (m_spot.is_spot) {
				show_find_dialog(false);
			}
			m_find_form.UpdateSpotList();
			this.Activate();
			m_lib.device.SetMustDrawFlag();
		}
		#endregion

		/*-------------------------------------------------------------------------
		 多중시작時にすでに시작している 교역MAP C# をアクティブにする
		 まだ윈도우を작성していないこと
		---------------------------------------------------------------------------*/
		public static void ActiveGVTradeMap() {
			IntPtr hwnd = user32.FindWindowA(null, def.WINDOW_TITLE);
			if (hwnd == IntPtr.Zero) return;		// 見つからない

			// アクティブにする
			user32.SetForegroundWindow(hwnd);
		}

#if DEBUG_COMPASS
		/*-------------------------------------------------------------------------
		 나침반デバッグ
		---------------------------------------------------------------------------*/
		private void debug_compass()
		{
			m_lib.device.DrawText(font_type.normal,
									String.Format("angle={0}, fov={1}, l={2}", m_db.capture.m_angle_x, m_db.capture.m_factor, m_db.capture.m_l),
									10, 32, Color.Black);
		}
#endif

#if DEBUG_ANGLE_LINE
		/*-------------------------------------------------------------------------
		 진행방향선 디버그
		---------------------------------------------------------------------------*/
		private void debug_angle_line() {
			Vector2 pos = new Vector2(this.ClientRectangle.Width / 2,
													this.ClientRectangle.Height / 2);
			// 속도(측량좌표계)
			float speed_map = speed_calculator.KnotToMapSpeed(10.0f);
			//			float		speed_map	= speed_calculator.KmToMapSpeed(1);

			++m_debug_angle_i;
			if (m_debug_angle_i < 60) {
			} else if (m_debug_angle_i < 60 * 2) {
				speed_map = speed_calculator.KnotToMapSpeed(10.5f);
			} else if (m_debug_angle_i < 60 * 3) {
				speed_map = speed_calculator.KnotToMapSpeed(10.2f);
			} else {
				m_debug_angle_i = 0;
			}

			m_lib.device.DrawText(font_type.normal,
									String.Format("angle={0}, speed={1}, {2}knt, {3}km/h",
													m_debug_angle,
													speed_map,
													speed_calculator.MapToKnotSpeed(speed_map),
													speed_calculator.MapToKmSpeed(speed_map)),
									10, 32, Color.Black);

			//draw_angle_line(pos, m_lib.loop_image, m_debug_angle, Color.Black);
			//draw_step_position2(pos, m_lib.loop_image, m_debug_angle, speed_map);

			m_debug_angle += 1.0f;
			if (m_debug_angle >= 360) m_debug_angle -= 360;
		}
#endif

#if DEBUG_DRAW_DEBUG_STRING
		/*-------------------------------------------------------------------------
		 デバッグ용문자열の그리기
		---------------------------------------------------------------------------*/
		private void debug_draw_debug_string() {
			m_lib.device.systemfont.locate = new Vector3(10, 100, 0.1f);
			m_lib.device.systemfont.Puts(String.Format("Marge map = {0}msec\n", m_lib.loop_image.MargeImageMS), Color.Black);
			/*			m_lib.device.systemfont.Puts(String.Format("sprites = {0}\n", m_lib.device.sprites.draw_sprites_in_frame), Color.Black);
						m_lib.device.systemfont.Puts(String.Format("texturedfont cash = {0}\n", m_lib.device.textured_font.cash_count), Color.Black);
						m_lib.device.systemfont.Puts(String.Format("share BB = {0}\n", m_db.share_routes.share_bb.Count), Color.Black);

						// 마우스カーソルの위치
						// 게임좌표と맵좌표
						Point	map_pos		= m_lib.loop_image.MousePos2GlobalPos(m_old_mouse_pos);
						Point	game_pos	= transform.client_pos2_game_pos(m_old_mouse_pos, m_lib.loop_image);
						m_lib.device.systemfont.Puts(String.Format("game({0},{1}) map({2},{3})\n", game_pos.X, game_pos.Y, map_pos.X, map_pos.Y), Color.Black);
			*/
		}
#endif
	}
}
