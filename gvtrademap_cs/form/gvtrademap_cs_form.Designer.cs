namespace gvtrademap_cs
{
	partial class gvtrademap_cs_form
	{
		/// <summary>
		/// 必要なデザイナ変수です. 
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 사용중のリソースを전부クリーンアップします. 
		/// </summary>
		/// <param Name="disposing">マネージ リソースが破棄される場合 true, 破棄されない場合は false です. </param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				// ここに書かないと例외時스레드を종료させられない?
				// 스레드を종료させる
				finish_all_threads();

				// アンマネージドリソースの破棄
				if(m_db != null)	m_db.Dispose();
				// libは必ず最후(Direct3D Deviceが最후に破棄される)
				if(m_lib != null)	m_lib.Dispose();

				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナで生成されたコード

		/// <summary>
		/// デザイナ サ포트に必要なメソッドです. このメソッドの내용を
		/// コード エディタで변경しないでください. 
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripMenuItem openpathToolStripMenuItem;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(gvtrademap_cs_form));
			this.openpathlogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openpathmailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openpathscreenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.ToolStripMenuItem_country0 = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem_country1 = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem_country2 = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem_country3 = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem_country4 = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem_country5 = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem_country6 = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem_country00 = new System.Windows.Forms.ToolStripMenuItem();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.set_target_memo_icon_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.add_memo_icon_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.edit_memo_icon_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.remove_memo_icon_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.remove_all_target_memo_icon_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.remove_all_memo_icon_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.openpathscreenshot2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setseaareastateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exexgvoacToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.normal_sea_area_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.safty_sea_area_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.lawless_sea_area_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.spotToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.clear_spotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.open_recipe_wiki0_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.open_recipe_wiki1_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.copy_name_to_clipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copy_all_to_clipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.listView1 = new System.Windows.Forms.ListView();
			this.changeBorderStyleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			openpathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			this.contextMenuStrip2.SuspendLayout();
			this.contextMenuStrip3.SuspendLayout();
			this.SuspendLayout();
			// 
			// openpathToolStripMenuItem
			// 
			openpathToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.openpathlogToolStripMenuItem,
			this.openpathmailToolStripMenuItem,
			this.openpathscreenshotToolStripMenuItem});
			openpathToolStripMenuItem.Name = "openpathToolStripMenuItem";
			openpathToolStripMenuItem.Size = new System.Drawing.Size(328, 22);
			openpathToolStripMenuItem.Text = "대항해시대 온라인의 폴더 열기";
			// 
			// openpathlogToolStripMenuItem
			// 
			this.openpathlogToolStripMenuItem.Name = "openpathlogToolStripMenuItem";
			this.openpathlogToolStripMenuItem.Size = new System.Drawing.Size(280, 22);
			this.openpathlogToolStripMenuItem.Text = "로그 폴더 열기...";
			// 
			// openpathmailToolStripMenuItem
			// 
			this.openpathmailToolStripMenuItem.Name = "openpathmailToolStripMenuItem";
			this.openpathmailToolStripMenuItem.Size = new System.Drawing.Size(280, 22);
			this.openpathmailToolStripMenuItem.Text = "메일 폴더 열기...";
			// 
			// openpathscreenshotToolStripMenuItem
			// 
			this.openpathscreenshotToolStripMenuItem.Name = "openpathscreenshotToolStripMenuItem";
			this.openpathscreenshotToolStripMenuItem.Size = new System.Drawing.Size(280, 22);
			this.openpathscreenshotToolStripMenuItem.Text = "스크린샷 폴더 열기...";
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.ToolStripMenuItem_country0,
			this.ToolStripMenuItem_country1,
			this.ToolStripMenuItem_country2,
			this.ToolStripMenuItem_country3,
			this.ToolStripMenuItem_country4,
			this.ToolStripMenuItem_country5,
			this.ToolStripMenuItem_country6,
			this.ToolStripMenuItem_country00});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(169, 180);
			// 
			// ToolStripMenuItem_country0
			// 
			this.ToolStripMenuItem_country0.Image = global::gvtrademap_cs.Properties.Resources.country01;
			this.ToolStripMenuItem_country0.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.ToolStripMenuItem_country0.Name = "ToolStripMenuItem_country0";
			this.ToolStripMenuItem_country0.Size = new System.Drawing.Size(168, 22);
			this.ToolStripMenuItem_country0.Text = "잉글랜드";
			this.ToolStripMenuItem_country0.Click += new System.EventHandler(this.ToolStripMenuItem_country0_Click);
			// 
			// ToolStripMenuItem_country1
			// 
			this.ToolStripMenuItem_country1.Image = global::gvtrademap_cs.Properties.Resources.country1;
			this.ToolStripMenuItem_country1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.ToolStripMenuItem_country1.Name = "ToolStripMenuItem_country1";
			this.ToolStripMenuItem_country1.Size = new System.Drawing.Size(168, 22);
			this.ToolStripMenuItem_country1.Text = "스페인";
			this.ToolStripMenuItem_country1.Click += new System.EventHandler(this.ToolStripMenuItem_country1_Click);
			// 
			// ToolStripMenuItem_country2
			// 
			this.ToolStripMenuItem_country2.Image = global::gvtrademap_cs.Properties.Resources.country2;
			this.ToolStripMenuItem_country2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.ToolStripMenuItem_country2.Name = "ToolStripMenuItem_country2";
			this.ToolStripMenuItem_country2.Size = new System.Drawing.Size(168, 22);
			this.ToolStripMenuItem_country2.Text = "포르투갈";
			this.ToolStripMenuItem_country2.Click += new System.EventHandler(this.ToolStripMenuItem_country2_Click);
			// 
			// ToolStripMenuItem_country3
			// 
			this.ToolStripMenuItem_country3.Image = global::gvtrademap_cs.Properties.Resources.country3;
			this.ToolStripMenuItem_country3.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.ToolStripMenuItem_country3.Name = "ToolStripMenuItem_country3";
			this.ToolStripMenuItem_country3.Size = new System.Drawing.Size(168, 22);
			this.ToolStripMenuItem_country3.Text = "네덜란드";
			this.ToolStripMenuItem_country3.Click += new System.EventHandler(this.ToolStripMenuItem_country3_Click);
			// 
			// ToolStripMenuItem_country4
			// 
			this.ToolStripMenuItem_country4.Image = global::gvtrademap_cs.Properties.Resources.country4;
			this.ToolStripMenuItem_country4.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.ToolStripMenuItem_country4.Name = "ToolStripMenuItem_country4";
			this.ToolStripMenuItem_country4.Size = new System.Drawing.Size(168, 22);
			this.ToolStripMenuItem_country4.Text = "프랑스";
			this.ToolStripMenuItem_country4.Click += new System.EventHandler(this.ToolStripMenuItem_country4_Click);
			// 
			// ToolStripMenuItem_country5
			// 
			this.ToolStripMenuItem_country5.Image = global::gvtrademap_cs.Properties.Resources.country5;
			this.ToolStripMenuItem_country5.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.ToolStripMenuItem_country5.Name = "ToolStripMenuItem_country5";
			this.ToolStripMenuItem_country5.Size = new System.Drawing.Size(168, 22);
			this.ToolStripMenuItem_country5.Text = "베네치아";
			this.ToolStripMenuItem_country5.Click += new System.EventHandler(this.ToolStripMenuItem_country5_Click);
			// 
			// ToolStripMenuItem_country6
			// 
			this.ToolStripMenuItem_country6.Image = global::gvtrademap_cs.Properties.Resources.country6;
			this.ToolStripMenuItem_country6.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.ToolStripMenuItem_country6.Name = "ToolStripMenuItem_country6";
			this.ToolStripMenuItem_country6.Size = new System.Drawing.Size(168, 22);
			this.ToolStripMenuItem_country6.Text = "오스만투르크";
			this.ToolStripMenuItem_country6.Click += new System.EventHandler(this.ToolStripMenuItem_country6_Click);
			// 
			// ToolStripMenuItem_country00
			// 
			this.ToolStripMenuItem_country00.Image = global::gvtrademap_cs.Properties.Resources.country00;
			this.ToolStripMenuItem_country00.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.ToolStripMenuItem_country00.Name = "ToolStripMenuItem_country00";
			this.ToolStripMenuItem_country00.Size = new System.Drawing.Size(168, 22);
			this.ToolStripMenuItem_country00.Text = "무소속";
			this.ToolStripMenuItem_country00.Click += new System.EventHandler(this.ToolStripMenuItem_country00_Click);
			// 
			// textBox1
			// 
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox1.Location = new System.Drawing.Point(12, 12);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size(317, 191);
			this.textBox1.TabIndex = 1;
			this.textBox1.TabStop = false;
			this.textBox1.Visible = false;
			// 
			// contextMenuStrip2
			// 
			this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.set_target_memo_icon_ToolStripMenuItem,
			this.toolStripSeparator2,
			this.add_memo_icon_ToolStripMenuItem,
			this.edit_memo_icon_ToolStripMenuItem,
			this.remove_memo_icon_ToolStripMenuItem,
			this.toolStripSeparator1,
			this.remove_all_target_memo_icon_ToolStripMenuItem,
			this.remove_all_memo_icon_ToolStripMenuItem,
			this.toolStripSeparator5,
			this.openpathscreenshot2ToolStripMenuItem,
			this.setseaareastateToolStripMenuItem,
			openpathToolStripMenuItem,
			this.toolStripSeparator7,
			this.changeBorderStyleToolStripMenuItem,
			this.closeFormToolStripMenuItem});
			this.contextMenuStrip2.Name = "contextMenuStrip2";
			this.contextMenuStrip2.Size = new System.Drawing.Size(329, 292);
			// 
			// set_target_memo_icon_ToolStripMenuItem
			// 
			this.set_target_memo_icon_ToolStripMenuItem.Image = global::gvtrademap_cs.Properties.Resources.memo_icon21;
			this.set_target_memo_icon_ToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(111)))), ((int)(((byte)(161)))));
			this.set_target_memo_icon_ToolStripMenuItem.Name = "set_target_memo_icon_ToolStripMenuItem";
			this.set_target_memo_icon_ToolStripMenuItem.Size = new System.Drawing.Size(328, 22);
			this.set_target_memo_icon_ToolStripMenuItem.Text = "목적지 메모아이콘을 여기에 추가";
			this.set_target_memo_icon_ToolStripMenuItem.Click += new System.EventHandler(this.set_target_memo_icon_ToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(325, 6);
			// 
			// add_memo_icon_ToolStripMenuItem
			// 
			this.add_memo_icon_ToolStripMenuItem.Image = global::gvtrademap_cs.Properties.Resources.memo_icon15;
			this.add_memo_icon_ToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(111)))), ((int)(((byte)(161)))));
			this.add_memo_icon_ToolStripMenuItem.Name = "add_memo_icon_ToolStripMenuItem";
			this.add_memo_icon_ToolStripMenuItem.Size = new System.Drawing.Size(328, 22);
			this.add_memo_icon_ToolStripMenuItem.Text = "메모아이콘을 추가...";
			this.add_memo_icon_ToolStripMenuItem.Click += new System.EventHandler(this.add_memo_icon_ToolStripMenuItem_Click);
			// 
			// edit_memo_icon_ToolStripMenuItem
			// 
			this.edit_memo_icon_ToolStripMenuItem.Name = "edit_memo_icon_ToolStripMenuItem";
			this.edit_memo_icon_ToolStripMenuItem.Size = new System.Drawing.Size(328, 22);
			this.edit_memo_icon_ToolStripMenuItem.Text = "메모아이콘을 편집...";
			this.edit_memo_icon_ToolStripMenuItem.Click += new System.EventHandler(this.edit_memo_icon_ToolStripMenuItem_Click);
			// 
			// remove_memo_icon_ToolStripMenuItem
			// 
			this.remove_memo_icon_ToolStripMenuItem.Name = "remove_memo_icon_ToolStripMenuItem";
			this.remove_memo_icon_ToolStripMenuItem.Size = new System.Drawing.Size(328, 22);
			this.remove_memo_icon_ToolStripMenuItem.Text = "메모아이콘을 삭제";
			this.remove_memo_icon_ToolStripMenuItem.Click += new System.EventHandler(this.remove_memo_icon_ToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(325, 6);
			// 
			// remove_all_target_memo_icon_ToolStripMenuItem
			// 
			this.remove_all_target_memo_icon_ToolStripMenuItem.Name = "remove_all_target_memo_icon_ToolStripMenuItem";
			this.remove_all_target_memo_icon_ToolStripMenuItem.Size = new System.Drawing.Size(328, 22);
			this.remove_all_target_memo_icon_ToolStripMenuItem.Text = "전체 목적지 메모아이콘을 삭제";
			this.remove_all_target_memo_icon_ToolStripMenuItem.Click += new System.EventHandler(this.remove_all_target_memo_icon_ToolStripMenuItem_Click);
			// 
			// remove_all_memo_icon_ToolStripMenuItem
			// 
			this.remove_all_memo_icon_ToolStripMenuItem.Name = "remove_all_memo_icon_ToolStripMenuItem";
			this.remove_all_memo_icon_ToolStripMenuItem.Size = new System.Drawing.Size(328, 22);
			this.remove_all_memo_icon_ToolStripMenuItem.Text = "전체 메모아이콘을 삭제";
			this.remove_all_memo_icon_ToolStripMenuItem.Click += new System.EventHandler(this.remove_all_memo_icon_ToolStripMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(325, 6);
			// 
			// openpathscreenshot2ToolStripMenuItem
			// 
			this.openpathscreenshot2ToolStripMenuItem.Name = "openpathscreenshot2ToolStripMenuItem";
			this.openpathscreenshot2ToolStripMenuItem.Size = new System.Drawing.Size(328, 22);
			this.openpathscreenshot2ToolStripMenuItem.Text = "항로도의 스크린샷 폴더 열기...";
			// 
			// setseaareastateToolStripMenuItem
			// 
			this.setseaareastateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.exexgvoacToolStripMenuItem,
			this.toolStripSeparator6,
			this.normal_sea_area_ToolStripMenuItem,
			this.safty_sea_area_ToolStripMenuItem,
			this.lawless_sea_area_ToolStripMenuItem});
			this.setseaareastateToolStripMenuItem.Name = "setseaareastateToolStripMenuItem";
			this.setseaareastateToolStripMenuItem.Size = new System.Drawing.Size(328, 22);
			this.setseaareastateToolStripMenuItem.Text = "해역변동 시스템 설정";
			// 
			// exexgvoacToolStripMenuItem
			// 
			this.exexgvoacToolStripMenuItem.Name = "exexgvoacToolStripMenuItem";
			this.exexgvoacToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.exexgvoacToolStripMenuItem.Text = "해역변동 수집을 시작...";
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(217, 6);
			// 
			// normal_sea_area_ToolStripMenuItem
			// 
			this.normal_sea_area_ToolStripMenuItem.Name = "normal_sea_area_ToolStripMenuItem";
			this.normal_sea_area_ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.normal_sea_area_ToolStripMenuItem.Text = "을 위험해역으로 설정";
			this.normal_sea_area_ToolStripMenuItem.Click += new System.EventHandler(this.normal_sea_area_ToolStripMenuItem_Click);
			// 
			// safty_sea_area_ToolStripMenuItem
			// 
			this.safty_sea_area_ToolStripMenuItem.Name = "safty_sea_area_ToolStripMenuItem";
			this.safty_sea_area_ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.safty_sea_area_ToolStripMenuItem.Text = "을 안전해역으로 설정";
			this.safty_sea_area_ToolStripMenuItem.Click += new System.EventHandler(this.safty_sea_area_ToolStripMenuItem_Click);
			// 
			// lawless_sea_area_ToolStripMenuItem
			// 
			this.lawless_sea_area_ToolStripMenuItem.Name = "lawless_sea_area_ToolStripMenuItem";
			this.lawless_sea_area_ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.lawless_sea_area_ToolStripMenuItem.Text = "을 무법해역으로 설정";
			this.lawless_sea_area_ToolStripMenuItem.Click += new System.EventHandler(this.lawless_sea_area_ToolStripMenuItem_Click);
			// 
			// contextMenuStrip3
			// 
			this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.spotToolStripMenuItem1,
			this.clear_spotToolStripMenuItem,
			this.toolStripSeparator4,
			this.open_recipe_wiki0_ToolStripMenuItem,
			this.open_recipe_wiki1_ToolStripMenuItem,
			this.toolStripSeparator3,
			this.copy_name_to_clipboardToolStripMenuItem,
			this.copy_all_to_clipboardToolStripMenuItem});
			this.contextMenuStrip3.Name = "contextMenuStrip3";
			this.contextMenuStrip3.Size = new System.Drawing.Size(352, 148);
			// 
			// spotToolStripMenuItem1
			// 
			this.spotToolStripMenuItem1.Name = "spotToolStripMenuItem1";
			this.spotToolStripMenuItem1.Size = new System.Drawing.Size(351, 22);
			this.spotToolStripMenuItem1.Text = "장소표시";
			this.spotToolStripMenuItem1.Click += new System.EventHandler(this.spotToolStripMenuItem1_Click);
			// 
			// clear_spotToolStripMenuItem
			// 
			this.clear_spotToolStripMenuItem.Name = "clear_spotToolStripMenuItem";
			this.clear_spotToolStripMenuItem.Size = new System.Drawing.Size(351, 22);
			this.clear_spotToolStripMenuItem.Text = "장소표시해제";
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(348, 6);
			// 
			// open_recipe_wiki0_ToolStripMenuItem
			// 
			this.open_recipe_wiki0_ToolStripMenuItem.Name = "open_recipe_wiki0_ToolStripMenuItem";
			this.open_recipe_wiki0_ToolStripMenuItem.Size = new System.Drawing.Size(351, 22);
			this.open_recipe_wiki0_ToolStripMenuItem.Text = "레시피의 내용을 레시피정보wiki에서 조사";
			this.open_recipe_wiki0_ToolStripMenuItem.Click += new System.EventHandler(this.open_recipe_wiki0_toolStripMenuItem_Click);
			// 
			// open_recipe_wiki1_ToolStripMenuItem
			// 
			this.open_recipe_wiki1_ToolStripMenuItem.Name = "open_recipe_wiki1_ToolStripMenuItem";
			this.open_recipe_wiki1_ToolStripMenuItem.Size = new System.Drawing.Size(351, 22);
			this.open_recipe_wiki1_ToolStripMenuItem.Text = "레시피에서 작성가능 여부를 레시피정보wiki에서 조사";
			this.open_recipe_wiki1_ToolStripMenuItem.Click += new System.EventHandler(this.open_recipe_wiki1_ToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(348, 6);
			// 
			// copy_name_to_clipboardToolStripMenuItem
			// 
			this.copy_name_to_clipboardToolStripMenuItem.Name = "copy_name_to_clipboardToolStripMenuItem";
			this.copy_name_to_clipboardToolStripMenuItem.Size = new System.Drawing.Size(351, 22);
			this.copy_name_to_clipboardToolStripMenuItem.Text = "명칭을 클립보드에 복사";
			this.copy_name_to_clipboardToolStripMenuItem.Click += new System.EventHandler(this.copy_name_to_clipboardToolStripMenuItem_Click);
			// 
			// copy_all_to_clipboardToolStripMenuItem
			// 
			this.copy_all_to_clipboardToolStripMenuItem.Name = "copy_all_to_clipboardToolStripMenuItem";
			this.copy_all_to_clipboardToolStripMenuItem.Size = new System.Drawing.Size(351, 22);
			this.copy_all_to_clipboardToolStripMenuItem.Text = "내용을 클립보드에 복사";
			this.copy_all_to_clipboardToolStripMenuItem.Click += new System.EventHandler(this.copy_all_to_clipboardToolStripMenuItem_Click);
			// 
			// listView1
			// 
			this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(12, 209);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.ShowItemToolTips = true;
			this.listView1.Size = new System.Drawing.Size(317, 102);
			this.listView1.TabIndex = 3;
			this.listView1.TabStop = false;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.Visible = false;
			this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
			// 
			// changeBorderStyleToolStripMenuItem
			// 
			this.changeBorderStyleToolStripMenuItem.Name = "changeBorderStyleToolStripMenuItem";
			this.changeBorderStyleToolStripMenuItem.Size = new System.Drawing.Size(328, 22);
			this.changeBorderStyleToolStripMenuItem.Text = "윈도우 테두리 표시/비표시";
			// 
			// closeFormToolStripMenuItem
			// 
			this.closeFormToolStripMenuItem.Name = "closeFormToolStripMenuItem";
			this.closeFormToolStripMenuItem.Size = new System.Drawing.Size(328, 22);
			this.closeFormToolStripMenuItem.Text = "교역MAP 프로그램 닫기";
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(325, 6);
			// 
			// gvtrademap_cs_form
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Silver;
			this.ClientSize = new System.Drawing.Size(592, 323);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.textBox1);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(300, 256);
			this.Name = "gvtrademap_cs_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Form1";
			this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.FormMouseWheel);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainWindowMouseUp);
			this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.m_main_window_MouseDoubleClick);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.m_main_window_Paint);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gvtrademap_cs_form_MouseClick);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.gvtrademap_cs_form_DragDrop);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.gvtrademap_cs_form_FormClosed);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainWindowMouseDown);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.gvtrademap_cs_form_DragEnter);
			this.Move += new System.EventHandler(this.gvtrademap_cs_form_Move);
			this.Resize += new System.EventHandler(this.gvtrademap_cs_form_Resize);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainWindowMouseMove);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gvtrademap_cs_form_KeyDown);
			this.contextMenuStrip1.ResumeLayout(false);
			this.contextMenuStrip2.ResumeLayout(false);
			this.contextMenuStrip3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_country0;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_country1;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_country2;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_country3;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_country4;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_country5;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_country6;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_country00;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
		private System.Windows.Forms.ToolStripMenuItem set_target_memo_icon_ToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem add_memo_icon_ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem edit_memo_icon_ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem remove_memo_icon_ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem remove_all_memo_icon_ToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem remove_all_target_memo_icon_ToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
		private System.Windows.Forms.ToolStripMenuItem open_recipe_wiki1_ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem open_recipe_wiki0_ToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem copy_name_to_clipboardToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copy_all_to_clipboardToolStripMenuItem;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ToolStripMenuItem clear_spotToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem spotToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem openpathlogToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openpathmailToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openpathscreenshotToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setseaareastateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem normal_sea_area_ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem safty_sea_area_ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem lawless_sea_area_ToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem exexgvoacToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openpathscreenshot2ToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.ToolStripMenuItem changeBorderStyleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem closeFormToolStripMenuItem;
	}
}

