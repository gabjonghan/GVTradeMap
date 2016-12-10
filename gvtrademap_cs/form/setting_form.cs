/*-------------------------------------------------------------------------

 設定ダイアログ

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

using useful;
using System.IO;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public partial class setting_form : Form
	{
		private	setting					m_setting;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public setting _setting{		get{	return m_setting;		}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public setting_form(setting _setting, string device_info)
		{
			// 設定内容をコピーして持つ
			m_setting				= _setting.Clone();

			InitializeComponent();
			useful.useful.SetFontMeiryo(this, def.MEIRYO_POINT);

			// ツールチップを登録する
			toolTip1.AutoPopDelay	= 30*1000;		// 30秒表示
			toolTip1.BackColor		= Color.LightYellow;
			toolTip1.SetToolTip(comboBox2, "プレイしているサーバを選択します");
			toolTip1.SetToolTip(comboBox3, "属している国を選択します");
			toolTip1.SetToolTip(comboBox1, "地図を選択します");
			toolTip1.SetToolTip(comboBox4, "画面キャプチャ間隔を選択します\n短い間隔でキャプチャするほどコンパスの角度のレスポンスがよくなりますがCPU時間を多く消費します\nCPUに余裕がある場合は0.5秒に1回を選択してください\nさらにCPUに余裕がある場合は0.25秒に1回を選択してください\n初期値は1秒に1回です");
			toolTip1.SetToolTip(comboBox5, "緯度、経度線の描画方法を選択します\n単位は測量で得られる値です\n初期値は座標のみ描画です");
			toolTip1.SetToolTip(textBox1, "航路共有用のグループ名を指定します\n空白にすると航路共有されません");
			toolTip1.SetToolTip(textBox2, "航路共有描画時に表示される名前を指定します\n指定した名前を航路共有する他のメンバーに伝えます\n空白にすると航路共有されません");
			toolTip1.SetToolTip(textBox3, "航路図を保持する数を指定します\n仕様変更により単位が変更されました\n航路図の色が変わるタイミングで1消費します\n初期値は20です");
			toolTip1.SetToolTip(checkBox1, "インターネットに接続するかどうかを指定します\nチェックを入れると起動時のデータ更新、航路共有が有効になります");
			toolTip1.SetToolTip(checkBox2, "マウスの戻る・進むボタンでスキル・道具窓を開きます");
			toolTip1.SetToolTip(checkBox3, "画面キャプチャ方法を指定します\nWindows Vistaを使用していて航路図がうまく書かれない場合チェックを入れてください");
			toolTip1.SetToolTip(checkBox4, "航路図の色が変わるタイミングですべての航路図(日付アイコン含む)を削除します\n海に出る度に航路図を削除してる人向け");
			toolTip1.SetToolTip(checkBox5, "災害ポップアップ、利息からの経過日数、海域変動システム用にログ解析を行います");
			toolTip1.SetToolTip(checkBox6, "航路共有を有効にする場合はチェックを入れてください\nインターネットから更新情報等を受け取るにチェックを入れている必要があります");
			toolTip1.SetToolTip(checkBox7, "航路図の色が変わるタイミングですべての航路図を削除する際、\nついでに災害ポップアップも削除します");
			toolTip1.SetToolTip(checkBox8, "ウインドウ最小化時に画面キャプチャ、ログ解析、航路共有を行うかどうかを設定します\nチェックをはずすとウインドウ最小化時のCPU使用率を下げることができます");
			toolTip1.SetToolTip(checkBox9, "起動時インターネットから@Webアイコンを取得します\n取得した@Webアイコンはローカルに保存されます\n起動時に毎回取得する必要がない場合はチェックをはずしてください");
			toolTip1.SetToolTip(comboBox6, "街アイコンのサイズを選択します。\n海岸線がアイコンで隠れるのがいやな方は小さいアイコンを選択してください。");
			toolTip1.SetToolTip(comboBox7, "街名等を描画するかどうかを選択します。\n描画しない場合はマウスを乗せると街名がポップアップします。");
			toolTip1.SetToolTip(comboBox8, "スクリーンショットのフォーマットを選択します。\n初期値はbmpです。");

			string	str		= "右クリック時の動作を選択します\n";
			str				+= "チェック有\n";
			str				+= "  右クリックでコンテキストメニューが開く\n";
			str				+= "  右クリックでも街を選択できる\n";
			str				+= "  スポット解除はESCキーのみ\n";
			str				+= "チェックなし\n";
			str				+= "  Ctrl+右クリックでコンテキストメニューが開く\n";
			str				+= "  右クリックでは街を選択できない\n";
			str				+= "  スポット解除はESCキーかどこかの街を選択\n";
			toolTip1.SetToolTip(checkBox10, str);
			toolTip1.SetToolTip(checkBox11, "お気に入り航路と合成した地図を使用します\nこの項目はお気に入り航路の使用／不使用切り替えのために用意されています");
			toolTip1.SetToolTip(checkBox12, "ウインドウを常に最前面に表示します");
			toolTip1.SetToolTip(checkBox13, "航路図、コンパスの角度線、進路予想線の描画方法を指定します\nチェックを入れた場合アンチエイリアスで描画されます");
			toolTip1.SetToolTip(checkBox14, "一番新しい航路図以外を半透明で描画します\n日付、災害ポップアップも半透明になります");
			toolTip1.SetToolTip(checkBox15, "航路図が途切れないようにできるだけ繋ぎます\n繋いだ結果、大陸を横断する可能性がありますがほとんどの場合問題ないと思われます");
			toolTip1.SetToolTip(checkBox16, "@Webアイコン描画時に同じ種類で距離が近い場合、1つにまとめます。\n@Webアイコン表示時のごちゃごちゃした感じを軽減します。");

			// 設定項目を反映させる
			comboBox2.SelectedIndex	= (int)m_setting.server;
			comboBox3.SelectedIndex	= m_setting.country - domains.country.England;
			comboBox1.SelectedIndex	= (int)m_setting.map;
			if(m_setting.capture_interval == capture_interval_index.per_250ms){
				comboBox4.SelectedIndex	= 0;
			}else{
				comboBox4.SelectedIndex	= (int)m_setting.capture_interval + 1;
			}
			comboBox5.SelectedIndex	= (int)m_setting.tude_interval;
			comboBox6.SelectedIndex	= (int)m_setting.map_icon;
			comboBox7.SelectedIndex	= (int)m_setting.map_draw_names;
			comboBox8.SelectedIndex	= (int)m_setting.ss_format;
			textBox1.Text			= m_setting.share_group;
			textBox2.Text			= m_setting.share_group_myname;
			textBox3.Text			= m_setting.searoutes_group_max.ToString();
			checkBox1.Checked		= m_setting.connect_network;
			checkBox2.Checked		= m_setting.hook_mouse;
			checkBox3.Checked		= m_setting.windows_vista_aero;
			checkBox4.Checked		= m_setting.only_active_searoutes;
			checkBox5.Checked		= m_setting.enable_analize_log_chat;
			checkBox6.Checked		= m_setting.is_share_routes;
			checkBox7.Checked		= m_setting.with_remove_accident;
			checkBox8.Checked		= m_setting.do_tasks_minimize_window;
			checkBox9.Checked		= m_setting.connect_web_icon;
			checkBox10.Checked		= m_setting.compatible_windows_rclick;
			checkBox11.Checked		= m_setting.use_mixed_map;
			checkBox12.Checked		= m_setting.window_top_most;
			checkBox13.Checked		= m_setting.enable_line_antialias;
			checkBox14.Checked		= m_setting.enable_sea_routes_aplha;
			checkBox15.Checked		= m_setting.searoutes_chain_points;
			checkBox16.Checked		= m_setting.remove_near_web_icons;

			// コントロールの有効、無効を設定する
			update_gray_ctrl();

			// デバイス情報
			textBox4.Lines			= device_info.Split(new string[]{"\n", "\r\n"}, StringSplitOptions.None);
		}

		/*-------------------------------------------------------------------------
		 ダイアログが閉じられた
		---------------------------------------------------------------------------*/
		private void setting_form_FormClosed(object sender, FormClosedEventArgs e)
		{
			m_setting.server					= domains.server.Euros + comboBox2.SelectedIndex;
			m_setting.country					= domains.country.England + comboBox3.SelectedIndex;
			m_setting.map						= MapIndex.Map1 + comboBox1.SelectedIndex;
			m_setting.map_icon					= map_icon.big + comboBox6.SelectedIndex;
			m_setting.map_draw_names			= map_draw_names.draw + comboBox7.SelectedIndex;
			m_setting.ss_format					= ss_format.bmp + comboBox8.SelectedIndex;

			if(comboBox4.SelectedIndex == 0){
				m_setting.capture_interval		= capture_interval_index.per_250ms;
			}else{
				m_setting.capture_interval		= capture_interval_index.per_500ms + (comboBox4.SelectedIndex - 1);
			}

			m_setting.tude_interval				= TudeInterval.None + comboBox5.SelectedIndex;
			m_setting.share_group				= textBox1.Text;
			m_setting.share_group_myname		= textBox2.Text;
			m_setting.connect_network			= checkBox1.Checked;
			m_setting.hook_mouse				= checkBox2.Checked;
			m_setting.windows_vista_aero		= checkBox3.Checked;
			m_setting.only_active_searoutes		= checkBox4.Checked;
			m_setting.enable_analize_log_chat	= checkBox5.Checked;
			m_setting.is_share_routes			= checkBox6.Checked;
			m_setting.with_remove_accident		= checkBox7.Checked;
			m_setting.do_tasks_minimize_window	= checkBox8.Checked;
			m_setting.connect_web_icon			= checkBox9.Checked;
			m_setting.compatible_windows_rclick	= checkBox10.Checked;
			m_setting.use_mixed_map				= checkBox11.Checked;
			m_setting.window_top_most			= checkBox12.Checked;
			m_setting.enable_line_antialias		= checkBox13.Checked;
			m_setting.enable_sea_routes_aplha	= checkBox14.Checked;
			m_setting.searoutes_chain_points	= checkBox15.Checked;
			m_setting.remove_near_web_icons		= checkBox16.Checked;

			try{
				m_setting.searoutes_group_max	= Convert.ToInt32(textBox3.Text);
			}catch{
				m_setting.searoutes_group_max	= -1;	// 初期値に戻す
			}
		}

		/*-------------------------------------------------------------------------
		 チェックボックスが変更された
		---------------------------------------------------------------------------*/
		private void all_ctrl_CheckStateChanged(object sender, EventArgs e)
		{
			update_gray_ctrl();
		}

		/*-------------------------------------------------------------------------
		 グレイ表示を更新する
		---------------------------------------------------------------------------*/
		private void update_gray_ctrl()
		{
			// 航路共有
			if(checkBox1.Checked){
				checkBox6.Enabled	= true;
				checkBox9.Enabled	= true;
			}else{
				checkBox6.Enabled	= false;
				checkBox9.Enabled	= false;
			}

			if(checkBox1.Checked && checkBox6.Checked){
				textBox1.Enabled	= true;
				textBox2.Enabled	= true;
			}else{
				textBox1.Enabled	= false;
				textBox2.Enabled	= false;
			}

			// 海に出る度に航路図を削除する
			if(checkBox4.Checked)	checkBox7.Enabled	= true;
			else					checkBox7.Enabled	= false;
		}

		/*-------------------------------------------------------------------------
		 バージョン情報
		---------------------------------------------------------------------------*/
		private void button3_Click(object sender, EventArgs e)
		{
			version_form	dlg		= new version_form(def.WINDOW_TITLE);

			dlg.ShowDialog(this);
			dlg.Dispose();
		}

		/*-------------------------------------------------------------------------
		 更新確認
		---------------------------------------------------------------------------*/
		private void button4_Click(object sender, EventArgs e)
		{
			Cursor	= Cursors.WaitCursor;
			// バージョンを確認する
			downloadfile	dlf		= new downloadfile();
			bool			result	= dlf.Download(def.VERSION_URL, def.VERSION_FNAME);
			Cursor	= Cursors.Default;

			if(!result){
				// 更新しない
				MessageBox.Show(this, "更新情報が取得できませんでした。\nインターネットの接続を確認してください。", "更新確認エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			dlf		= null;
	
			// 更新されたデータがあるかどうか確認する
			string	line	= "";
			List<string>	data	= new List<string>();
			int		version	= 0;
			try{
				using (StreamReader	sr	= new StreamReader(
					def.VERSION_FNAME, Encoding.GetEncoding("Shift_JIS"))){

					// バージョン
					line	= sr.ReadLine();
					version	= Convert.ToInt32(line);

					// 残り
					while((line = sr.ReadLine()) != null){
						data.Add(line);
					}
				}
			}catch{
				MessageBox.Show(this, "バージョン情報が確認できません。\n更新確認に失敗しました。", "更新確認エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if(version > def.VERSION){
				// 更新されている
				check_update_result		dlg		= new check_update_result(data.ToArray());
				dlg.ShowDialog(this);
				dlg.Dispose();
			}else{
				// 更新されていない
				MessageBox.Show(this, "更新されたソフトウェアは見つかりませんでした。\nお使いのバージョンが最新です。", "更新確認結果", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
	}
}
