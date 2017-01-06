/*-------------------------------------------------------------------------

 설정ダイア로그2
 カテゴリ毎に분けた
 표시항목を合体させた
 버전정보を合体させた
 해역변동は合体させてない

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Utility;
using Utility.KeyAssign;
using net_base;
using System.Net;
using System.Diagnostics;
using System.IO;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public partial class setting_form2 : Form {
		public enum tab_index {
			general,
			sea_routes,
			capture_log,
			access_network,
			draw_flags,
			other,
			version,
		};

		private GlobalSettings m_setting;
		private KeyAssignList m_key_assign_list;
		private KeyAssiginSettingHelper m_key_assign_helper;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public GlobalSettings setting { get { return m_setting; } }
		public KeyAssignList KeyAssignList { get { return m_key_assign_list; } }

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public setting_form2(GlobalSettings _setting, KeyAssignList assign_list, string device_info) {
			init(_setting, assign_list, device_info, tab_index.general, DrawSettingPage.WebIcons);
		}
		// 그리기항목설정時용
		public setting_form2(GlobalSettings _setting, KeyAssignList assign_list, string device_info, DrawSettingPage _draw_setting_page) {
			init(_setting, assign_list, device_info, tab_index.draw_flags, _draw_setting_page);
		}
		// ページ지정
		public setting_form2(GlobalSettings _setting, KeyAssignList assign_list, string device_info, tab_index _tab_index) {
			init(_setting, assign_list, device_info, _tab_index, DrawSettingPage.WebIcons);
		}

		/*-------------------------------------------------------------------------
		 初期化
		---------------------------------------------------------------------------*/
		private void init(GlobalSettings _setting, KeyAssignList assign_list, string device_info, tab_index index, DrawSettingPage page) {
			// 설정내용をコピーして持つ
			m_setting = _setting.Clone();
			m_key_assign_list = assign_list.DeepClone();

			InitializeComponent();
			Useful.SetFontMeiryo(this, 8f);
			Useful.SetFontMeiryo(listBox1, 9f);

			// ツールチップを등록する
			toolTip1.AutoPopDelay = 30 * 1000;	  // 30초표시
			toolTip1.BackColor = Color.LightYellow;
			toolTip1.SetToolTip(comboBox2, "플레이하는 서버를 선택");
			toolTip1.SetToolTip(comboBox3, "소속되어 있는 국가를 선택");
			toolTip1.SetToolTip(comboBox1, "지도를 선택");
			toolTip1.SetToolTip(comboBox5, "위도, 경도선의 그리기방법을 선택\n단위는 측량에서 얻어지는 값과 같습니다 (0~16384)\n초기값은 좌표만 그리기");
			toolTip1.SetToolTip(textBox1, "항로공유용 그룹명을 지정\n비워두면 항로가 공유되지 않습니다");
			toolTip1.SetToolTip(textBox2, "항로공유 그리기시 표시되는 이름을 지정\n지정된 이름을 항로공유하는 다른 멤버에게 전합니다.\n비워두면 항로가 공유되지 않습니다");
			toolTip1.SetToolTip(checkBox8, "체크하면 도시명, 도시아이콘 등 최대한 같은 크기로 표시함\n체크하지 않으면 지도의 축척에 맞춰서 크기를 표시함\n체크하지 않으면 그리기가 빨라집니다");
			toolTip1.SetToolTip(checkBox1, "인터넷 연결 여부를 지정\n체크하면 시작할 때 데이터업데이트, 항로공유가 가능하게 됩니다\n한국어판에서는 동작여부를 보장하지 않습니다");
			toolTip1.SetToolTip(checkBox2, "마우스의 앞/뒤 버튼으로 스킬・도구창을 열기");
			toolTip1.SetToolTip(checkBox6, "항로공유를 사용하는 경우 체크하세요.\n인터넷으로 업데이트 정보 등을 받으려면 체크해야 합니다.");
			toolTip1.SetToolTip(checkBox9, "시작시 인터넷으로부터 @Web아이콘을 얻어옵니다.\n얻어진 @Web아이콘은 내 컴퓨터에 보존됩니다.\n시작시 매번 받을 필요가 없는 경우 체크를 해제하십시오.");
			toolTip1.SetToolTip(comboBox6, "도시아이콘의 사이즈를 선택\n해안선이 아이콘으로 가려지는것이 싫은 경우 작은 아이콘 또는 아이콘 표시 안함을 선택");
			toolTip1.SetToolTip(comboBox7, "도시명 그리기 여부를 선택\n그리지 않더라도 도시 주변에 마우스를 가져가면 도시명 팝업이 뜨게 됩니다.");
			toolTip1.SetToolTip(comboBox8, "스크린샷 포맷을 선택\n초기값은 bmp");
			toolTip1.SetToolTip(checkBox4, "조선중이 아니더라도 조선카운터를 표시할지를 설정");

			string str = "우클릭시의 동작을 선택\n";
			str += "체크함\n";
			str += "  우클릭으로 컨텍스트 메뉴를 열기\n";
			str += "  우클릭으로 도시선택이 가능\n";
			str += "  장소해제는 ESC키로만\n";
			str += "체크안함\n";
			str += "  Ctrl+우클릭으로 컨텍스트 메뉴를 열기\n";
			str += "  우클릭으로 도시선택 불가능\n";
			str += "  장소해제는 ESC키나 다른도시를 선택\n";
			toolTip1.SetToolTip(checkBox10, str);
			toolTip1.SetToolTip(checkBox11, "항로도즐겨찾기를 합성한 지도를 사용\n이항목은 항로도 즐겨찾기 사용/미사용 전환을 위해 사용됨");
			toolTip1.SetToolTip(checkBox12, "윈도우를 항상 맨 앞으로 표시");
			toolTip1.SetToolTip(checkBox13, "항로도, 나침반の각도선, 항로예상선의 그리기방법을 지정\n체크시 안티앨리어싱 사용");
			toolTip1.SetToolTip(checkBox14, "가장 최근의 항로도를 제외하고 반투명으로 그리기\n일자, 재해팝업도 반투명이 됩니다");
			toolTip1.SetToolTip(checkBox16, "@Web아이콘 그리기시 같은 종류로 거리가 가까운 경우 하나로 합침\n@Web아이콘 표시시 좀더 깔끔해집니다");

			toolTip1.SetToolTip(checkBox3, "화면캡처 방법을 지정\nWindows Vista 사용시 측량인식이 잘 되지 않을 경우 체크하십시오\nWindows7은 체크가 필요없습니다.");
			toolTip1.SetToolTip(checkBox5, "재해팝업, 이자 경과일수, 해역변동시스템용 로그분석을 사용");
			toolTip1.SetToolTip(checkBox17, "캡처한 화면을 선수상의 오른쪽에 표시\n나침반 각도 분석 및 확인용\n일반적으로 체크할 필요가 없습니다.");
			toolTip1.SetToolTip(comboBox4, "화면캡처 간격을 선택\n짧은간격으로 캡처할수록 나침반의 각도변경에 대한 반응이 빨라지지만 CPU를 많이 사용합니다\nCPU가 감당할 수 있는 경우 0.5초에 1회 또는 그 이하를 선택해 주세요\n초기값은 1초에 1회");
			toolTip1.SetToolTip(textBox6, "TCP서버가 사용하는 포트번호를 지정\n특별히 변경할 필요는 없습니다");
			toolTip1.SetToolTip(checkBox7, "정보윈도우의 좌표계나 측량계가 아닌 지도계にします\n開発時の위치取得용です");

			str = "표시할 최저항해일수를 지정\n";
			str += "이 설정은 최신의 항로도에는 영향을 주지 않습니다\n";
			str += "좁은 범위의 항해시 항로도가 뒤죽박죽이 되는 현상을 줄여줍니다\n";
			str += "만약 3개로 설정시 항해일수 2일 이하의 항로도는 그리지 않습니다\n";
			str += "0으로 설정하면 전체항로도를 그리게 됩니다\n";
			str += "초기값은 0";
			toolTip1.SetToolTip(textBox8, str);

			toolTip1.SetToolTip(checkBox18, "즐겨찾기 항로도를 반투명으로 그리기");
			toolTip1.SetToolTip(checkBox19, "즐겨찾기 항로도의 재해 팝업도 그리기");
			str = "과거의 항로도를 몇개까지 목록에 저장할지 설정\n";
			str += "그리기와 다르게, 이 경우에는 CPU부하가 낮아 많은 항로도를 보존해도 문제가 없습니다.\n";
			str += "초기값은 200";
			toolTip1.SetToolTip(textBox9, str);

			str = "항로도를 보존を保持する수を지정します\n";
			str += "保持수を多くすると그리기負荷が増えます\n";
			str += "해に出る도に항로도を전체삭제している方は1を지정してください\n";
			str += "初期値は20です";
			toolTip1.SetToolTip(textBox3, str);

			// 설정항목を反映させる
			comboBox2.SelectedIndex = (int)m_setting.server;
			comboBox3.SelectedItem = GvoWorldInfo.GetCountryString(this.m_setting.country);
			comboBox1.SelectedIndex = (int)m_setting.map;
			comboBox5.SelectedIndex = (int)m_setting.tude_interval;
			comboBox6.SelectedIndex = (int)m_setting.map_icon;
			comboBox7.SelectedIndex = (int)m_setting.map_draw_names;
			comboBox8.SelectedIndex = (int)m_setting.ss_format;
			textBox1.Text = m_setting.share_group;
			textBox2.Text = m_setting.share_group_myname;
			textBox3.Text = m_setting.searoutes_group_max.ToString();
			textBox9.Text = m_setting.trash_searoutes_group_max.ToString();
			textBox8.Text = m_setting.minimum_draw_days.ToString();
			checkBox1.Checked = m_setting.connect_network;
			checkBox2.Checked = m_setting.hook_mouse;
			checkBox6.Checked = m_setting.is_share_routes;
			checkBox9.Checked = m_setting.connect_web_icon;
			checkBox10.Checked = m_setting.compatible_windows_rclick;
			checkBox11.Checked = m_setting.use_mixed_map;
			checkBox12.Checked = m_setting.window_top_most;
			checkBox13.Checked = m_setting.enable_line_antialias;
			checkBox14.Checked = m_setting.enable_sea_routes_aplha;
			checkBox16.Checked = m_setting.remove_near_web_icons;
			checkBox8.Checked = !m_setting.is_mixed_info_names;
			checkBox18.Checked = m_setting.enable_favorite_sea_routes_alpha;
			checkBox19.Checked = m_setting.draw_favorite_sea_routes_alpha_popup;
			checkBox4.Checked = m_setting.force_show_build_ship;
			checkBox7.Checked = m_setting.debug_flag_show_potision;
			checkBox15.Checked = m_setting.enable_dpi_scaling;

			if (m_setting.capture_interval == CaptureIntervalIndex.Per250ms) {
				comboBox4.SelectedIndex = 0;
			} else {
				comboBox4.SelectedIndex = (int)m_setting.capture_interval + 1;
			}
			checkBox3.Checked = m_setting.windows_vista_aero;
			checkBox5.Checked = m_setting.enable_analize_log_chat;
			checkBox17.Checked = m_setting.draw_capture_info;

			textBox6.Text = m_setting.port_index.ToString();
			try {
				string host_name = net_useful.GetHostName();
				textBox5.AppendText(host_name + "\n");
				IPAddress[] list = net_useful.GetLocalIpAddress_Ipv4();
				if ((list != null)
					&& (list.Length > 0)) {
					textBox5.AppendText(list[0].ToString());
				}
			} catch {
				textBox5.AppendText("PC명\n");
				textBox5.AppendText("IPアドレスの取得に실패");
			}

			// 모드설정
			if (m_setting.is_server_mode) {
				radioButton2.Checked = true;
			} else {
				radioButton1.Checked = true;
			}

			// デバイス정보
			textBox4.Lines = device_info.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);

			// 버전정보
			label5.Text = def.WINDOW_TITLE;

			// 표시항목の初期化
			init_draw_setting(page);

			// 유효, 무효の업데이트
			update_gray_ctrl();

			// 初期ページ
			listBox1.SelectedIndex = (int)index;

			// HP
			linkLabel1.Text = def.URL_HP;

			// 키할당
			m_key_assign_helper = new KeyAssiginSettingHelper(m_key_assign_list,
																		this, comboBox9, listView1, button3, button5, button6);
		}

		/*-------------------------------------------------------------------------
		 표시항목の初期化
		---------------------------------------------------------------------------*/
		private void init_draw_setting(DrawSettingPage page) {
			// @Web icons
			{
				DrawSettingWebIcons flag = m_setting.draw_setting_web_icons;
				checkBox100.Checked = (flag & DrawSettingWebIcons.wind) != 0;
				checkBox101.Checked = (flag & DrawSettingWebIcons.accident_0) != 0;
				checkBox102.Checked = (flag & DrawSettingWebIcons.accident_1) != 0;
				checkBox103.Checked = (flag & DrawSettingWebIcons.accident_2) != 0;
				checkBox104.Checked = (flag & DrawSettingWebIcons.accident_3) != 0;
				checkBox105.Checked = (flag & DrawSettingWebIcons.accident_4) != 0;
			}
			// Memo icons
			{
				DrawSettingMemoIcons flag = m_setting.draw_setting_memo_icons;
				checkBox200.Checked = (flag & DrawSettingMemoIcons.wind) != 0;
				checkBox201.Checked = (flag & DrawSettingMemoIcons.memo_0) != 0;
				checkBox202.Checked = (flag & DrawSettingMemoIcons.memo_1) != 0;
				checkBox203.Checked = (flag & DrawSettingMemoIcons.memo_2) != 0;
				checkBox204.Checked = (flag & DrawSettingMemoIcons.memo_3) != 0;
				checkBox205.Checked = (flag & DrawSettingMemoIcons.memo_4) != 0;
				checkBox206.Checked = (flag & DrawSettingMemoIcons.memo_5) != 0;
				checkBox207.Checked = (flag & DrawSettingMemoIcons.memo_6) != 0;
				checkBox208.Checked = (flag & DrawSettingMemoIcons.memo_7) != 0;
				checkBox209.Checked = (flag & DrawSettingMemoIcons.memo_8) != 0;
				checkBox210.Checked = (flag & DrawSettingMemoIcons.memo_9) != 0;
				checkBox211.Checked = (flag & DrawSettingMemoIcons.memo_10) != 0;
				checkBox212.Checked = (flag & DrawSettingMemoIcons.memo_11) != 0;
			}
			// 재해
			{
				DrawSettingAccidents flag = m_setting.draw_setting_accidents;
				checkBox300.Checked = (flag & DrawSettingAccidents.accident_0) != 0;
				checkBox301.Checked = (flag & DrawSettingAccidents.accident_1) != 0;
				checkBox302.Checked = (flag & DrawSettingAccidents.accident_2) != 0;
				checkBox303.Checked = (flag & DrawSettingAccidents.accident_3) != 0;
				checkBox304.Checked = (flag & DrawSettingAccidents.accident_4) != 0;
				checkBox305.Checked = (flag & DrawSettingAccidents.accident_5) != 0;
				checkBox306.Checked = (flag & DrawSettingAccidents.accident_6) != 0;
				checkBox307.Checked = (flag & DrawSettingAccidents.accident_7) != 0;
				checkBox308.Checked = (flag & DrawSettingAccidents.accident_8) != 0;
				checkBox309.Checked = (flag & DrawSettingAccidents.accident_9) != 0;
				checkBox310.Checked = (flag & DrawSettingAccidents.accident_10) != 0;
			}
			// 예상선
			{
				DrawSettingMyShipAngle flag = m_setting.draw_setting_myship_angle;
				checkBox400.Checked = (flag & DrawSettingMyShipAngle.draw_0) != 0;
				checkBox401.Checked = m_setting.draw_setting_myship_angle_with_speed_pos;
				checkBox402.Checked = (flag & DrawSettingMyShipAngle.draw_1) != 0;
				checkBox403.Checked = m_setting.draw_setting_myship_expect_pos;
			}

			// 표시するページ설정
			if ((int)page < 0) page = DrawSettingPage.WebIcons;
			if (page > DrawSettingPage.MyShipAngle) page = DrawSettingPage.MyShipAngle;
			tabControl1.SelectTab((int)page);
		}

		/*-------------------------------------------------------------------------
		 목록ボックスの선택내용が변경された
		---------------------------------------------------------------------------*/
		private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
			// どうもパネルの順番が一定ではない感じなので
			// 直接지정する
			switch (listBox1.SelectedIndex) {
				case 0:	 // 기본설정
					panelManager1.SelectedPanel = managedPanel1;
					break;
				case 1:	 // 항로도설정
					panelManager1.SelectedPanel = managedPanel7;
					break;
				case 2:	 // 캡처, 로그분석
					panelManager1.SelectedPanel = managedPanel2;
					break;
				case 3:	 // 인터넷연결, 항로공유
					panelManager1.SelectedPanel = managedPanel3;
					break;
				case 4:	 // 표시항목
					panelManager1.SelectedPanel = managedPanel4;
					break;
				case 5:	 // その他
					panelManager1.SelectedPanel = managedPanel5;
					break;
				case 6:	 // 키보드할당
					panelManager1.SelectedPanel = managedPanel8;
					break;
				case 7:	 // 버전정보
					panelManager1.SelectedPanel = managedPanel6;
					break;
			}
		}

		/*-------------------------------------------------------------------------
		 閉じられた
		---------------------------------------------------------------------------*/
		private void setting_form2_FormClosed(object sender, FormClosedEventArgs e) {
			m_setting.server = GvoWorldInfo.GetServerFromString(comboBox2.Text);
			m_setting.country = GvoWorldInfo.GetCountryFromString(comboBox3.Text);
			m_setting.map = MapIndex.Map1 + comboBox1.SelectedIndex;
			m_setting.map_icon = MapIcon.Big + comboBox6.SelectedIndex;
			m_setting.map_draw_names = MapDrawNames.Draw + comboBox7.SelectedIndex;
			m_setting.ss_format = SSFormat.Bmp + comboBox8.SelectedIndex;

			m_setting.tude_interval = TudeInterval.None + comboBox5.SelectedIndex;
			m_setting.share_group = textBox1.Text;
			m_setting.share_group_myname = textBox2.Text;
			m_setting.connect_network = checkBox1.Checked;
			m_setting.hook_mouse = checkBox2.Checked;
			m_setting.is_share_routes = checkBox6.Checked;
			m_setting.connect_web_icon = checkBox9.Checked;
			m_setting.compatible_windows_rclick = checkBox10.Checked;
			m_setting.use_mixed_map = checkBox11.Checked;
			m_setting.window_top_most = checkBox12.Checked;
			m_setting.enable_line_antialias = checkBox13.Checked;
			m_setting.enable_sea_routes_aplha = checkBox14.Checked;
			m_setting.remove_near_web_icons = checkBox16.Checked;
			m_setting.is_mixed_info_names = !checkBox8.Checked;
			m_setting.enable_favorite_sea_routes_alpha = checkBox18.Checked;
			m_setting.draw_favorite_sea_routes_alpha_popup = checkBox19.Checked;
			m_setting.force_show_build_ship = checkBox4.Checked;
			m_setting.debug_flag_show_potision = checkBox7.Checked;
			m_setting.enable_dpi_scaling = checkBox15.Checked;

			m_setting.searoutes_group_max = Useful.ToInt32(textBox3.Text, -1);
			m_setting.trash_searoutes_group_max = Useful.ToInt32(textBox9.Text, -1);
			m_setting.minimum_draw_days = Useful.ToInt32(textBox8.Text, -1);

			m_setting.is_server_mode = radioButton2.Checked;
			m_setting.port_index = Useful.ToInt32(textBox6.Text, def.DEFALUT_PORT_INDEX);

			if (comboBox4.SelectedIndex == 0) {
				m_setting.capture_interval = CaptureIntervalIndex.Per250ms;
			} else {
				m_setting.capture_interval = CaptureIntervalIndex.Per500ms + (comboBox4.SelectedIndex - 1);
			}
			m_setting.windows_vista_aero = checkBox3.Checked;
			m_setting.enable_analize_log_chat = checkBox5.Checked;
			m_setting.draw_capture_info = checkBox17.Checked;

			// 표시항목の保存
			save_draw_setting();

			// 키할당
			m_key_assign_list = m_key_assign_helper.List;
		}

		/*-------------------------------------------------------------------------
		 표시항목の保存
		---------------------------------------------------------------------------*/
		private void save_draw_setting() {
			{
				DrawSettingWebIcons flag = 0;
				flag |= (checkBox100.Checked) ? DrawSettingWebIcons.wind : 0;
				flag |= (checkBox101.Checked) ? DrawSettingWebIcons.accident_0 : 0;
				flag |= (checkBox102.Checked) ? DrawSettingWebIcons.accident_1 : 0;
				flag |= (checkBox103.Checked) ? DrawSettingWebIcons.accident_2 : 0;
				flag |= (checkBox104.Checked) ? DrawSettingWebIcons.accident_3 : 0;
				flag |= (checkBox105.Checked) ? DrawSettingWebIcons.accident_4 : 0;
				m_setting.draw_setting_web_icons = flag;
			}
			{
				DrawSettingMemoIcons flag = 0;
				flag |= (checkBox200.Checked) ? DrawSettingMemoIcons.wind : 0;
				flag |= (checkBox201.Checked) ? DrawSettingMemoIcons.memo_0 : 0;
				flag |= (checkBox202.Checked) ? DrawSettingMemoIcons.memo_1 : 0;
				flag |= (checkBox203.Checked) ? DrawSettingMemoIcons.memo_2 : 0;
				flag |= (checkBox204.Checked) ? DrawSettingMemoIcons.memo_3 : 0;
				flag |= (checkBox205.Checked) ? DrawSettingMemoIcons.memo_4 : 0;
				flag |= (checkBox206.Checked) ? DrawSettingMemoIcons.memo_5 : 0;
				flag |= (checkBox207.Checked) ? DrawSettingMemoIcons.memo_6 : 0;
				flag |= (checkBox208.Checked) ? DrawSettingMemoIcons.memo_7 : 0;
				flag |= (checkBox209.Checked) ? DrawSettingMemoIcons.memo_8 : 0;
				flag |= (checkBox210.Checked) ? DrawSettingMemoIcons.memo_9 : 0;
				flag |= (checkBox211.Checked) ? DrawSettingMemoIcons.memo_10 : 0;
				flag |= (checkBox212.Checked) ? DrawSettingMemoIcons.memo_11 : 0;
				m_setting.draw_setting_memo_icons = flag;
			}
			{
				DrawSettingAccidents flag = 0;
				flag |= (checkBox300.Checked) ? DrawSettingAccidents.accident_0 : 0;
				flag |= (checkBox301.Checked) ? DrawSettingAccidents.accident_1 : 0;
				flag |= (checkBox302.Checked) ? DrawSettingAccidents.accident_2 : 0;
				flag |= (checkBox303.Checked) ? DrawSettingAccidents.accident_3 : 0;
				flag |= (checkBox304.Checked) ? DrawSettingAccidents.accident_4 : 0;
				flag |= (checkBox305.Checked) ? DrawSettingAccidents.accident_5 : 0;
				flag |= (checkBox306.Checked) ? DrawSettingAccidents.accident_6 : 0;
				flag |= (checkBox307.Checked) ? DrawSettingAccidents.accident_7 : 0;
				flag |= (checkBox308.Checked) ? DrawSettingAccidents.accident_8 : 0;
				flag |= (checkBox309.Checked) ? DrawSettingAccidents.accident_9 : 0;
				flag |= (checkBox310.Checked) ? DrawSettingAccidents.accident_10 : 0;
				m_setting.draw_setting_accidents = flag;
			}
			{
				DrawSettingMyShipAngle flag = 0;
				flag |= (checkBox400.Checked) ? DrawSettingMyShipAngle.draw_0 : 0;
				flag |= (checkBox402.Checked) ? DrawSettingMyShipAngle.draw_1 : 0;
				m_setting.draw_setting_myship_angle = flag;
				m_setting.draw_setting_myship_angle_with_speed_pos = checkBox401.Checked;
				m_setting.draw_setting_myship_expect_pos = checkBox403.Checked;
			}
		}

		/*-------------------------------------------------------------------------
		 유효, 무효の업데이트
		---------------------------------------------------------------------------*/
		private void radioButton1_CheckedChanged(object sender, EventArgs e) {
			update_gray_ctrl();
		}
		private void radioButton2_CheckedChanged(object sender, EventArgs e) {
			update_gray_ctrl();
		}
		private void checkBox1_CheckedChanged(object sender, EventArgs e) {
			update_gray_ctrl();
		}
		private void checkBox6_CheckedChanged(object sender, EventArgs e) {
			update_gray_ctrl();
		}
		private void checkBox4_CheckedChanged(object sender, EventArgs e) {
			update_gray_ctrl();
		}
		private void checkBox15_CheckedChanged(object sender, EventArgs e) {
			update_gray_ctrl();
		}
		private void checkBox400_CheckedChanged(object sender, EventArgs e) {
			update_gray_ctrl();
		}

		/*-------------------------------------------------------------------------
		 グレイ표시を업데이트する
		---------------------------------------------------------------------------*/
		private void update_gray_ctrl() {
			// 항로공유
			if (checkBox1.Checked) {
				checkBox6.Enabled = true;
				checkBox9.Enabled = true;
			} else {
				checkBox6.Enabled = false;
				checkBox9.Enabled = false;
			}

			if (checkBox1.Checked && checkBox6.Checked) {
				textBox1.Enabled = true;
				textBox2.Enabled = true;
			} else {
				textBox1.Enabled = false;
				textBox2.Enabled = false;
			}

			// 모드
			if (radioButton1.Checked) {
				// 통상모드
				comboBox4.Enabled = true;
				checkBox3.Enabled = true;
				checkBox5.Enabled = true;
				checkBox17.Enabled = true;
				textBox6.Enabled = false;
			} else {
				// TCP서버모드
				comboBox4.Enabled = false;
				checkBox3.Enabled = false;
				checkBox5.Enabled = false;
				checkBox17.Enabled = false;
				textBox6.Enabled = true;
			}

			// 표시항목
			if (checkBox400.Checked) {
				checkBox401.Enabled = true;
				checkBox403.Enabled = true;
			} else {
				checkBox401.Enabled = false;
				checkBox403.Enabled = false;
			}
		}

		/*-------------------------------------------------------------------------
		 HPを開く
		---------------------------------------------------------------------------*/
		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			Process.Start(def.URL_HP);
			linkLabel1.LinkVisited = true;
		}

		/*-------------------------------------------------------------------------
		 HPを開く
		 대항해시대Online ツール配布所
		---------------------------------------------------------------------------*/
		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			Process.Start(def.URL_HP_ORIGINAL);
			linkLabel2.LinkVisited = true;
		}

		/*-------------------------------------------------------------------------
		 최신버전チェック
		---------------------------------------------------------------------------*/
		private void button4_Click(object sender, EventArgs e) {
			Cursor = Cursors.WaitCursor;
			// 버전を확인する
			bool result = HttpDownload.Download(def.VERSION_URL, def.VERSION_FNAME);
			Cursor = Cursors.Default;

			if (!result) {
				// 업데이트しない
				MessageBox.Show(this, "업데이트정보が取得できませんでした. \nインターネットの연결を확인してください. ", "업데이트확인오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// 업데이트された데이터があるかどうか확인する
			string line = "";
			List<string> data = new List<string>();
			int version = 0;
			try {
				using (StreamReader sr = new StreamReader(
					def.VERSION_FNAME, Encoding.GetEncoding("UTF-8"))) {

					// 버전
					line = sr.ReadLine();
					version = Convert.ToInt32(line);

					// 残り
					while ((line = sr.ReadLine()) != null) {
						data.Add(line);
					}
				}
			} catch {
				MessageBox.Show(this, "버전정보が확인できません. \n업데이트확인に실패하였습니다. ", "업데이트확인오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (version > def.VERSION) {
				// 업데이트されている
				check_update_result dlg = new check_update_result(data.ToArray());
				dlg.ShowDialog(this);
				dlg.Dispose();
			} else {
				// 업데이트されていない
				MessageBox.Show(this, "업데이트されたソフトウェアは見つかりませんでした. \nお使いの버전が최신です. ", "업데이트확인결과", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void setting_form2_Load(object sender, EventArgs e) {

		}

		private void checkBox16_CheckedChanged(object sender, EventArgs e) {

		}

		private void toolTip1_Popup(object sender, PopupEventArgs e) {

		}

		private void groupBox11_Enter(object sender, EventArgs e) {

		}

		private void checkBox207_CheckedChanged(object sender, EventArgs e) {

		}
	}
}
