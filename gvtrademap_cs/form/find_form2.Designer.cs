namespace gvtrademap_cs
{
	partial class find_form2
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param Name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(find_form2));
			this.button3 = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.listView1 = new System.Windows.Forms.ListView();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.listView2 = new System.Windows.Forms.ListView();
			this.listView3 = new System.Windows.Forms.ListView();
			this.comboBox4 = new System.Windows.Forms.ComboBox();
			this.comboBox3 = new System.Windows.Forms.ComboBox();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.open_recipe_wiki0_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.open_recipe_wiki1_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.copy_name_to_clipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copy_all_to_clipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.button5 = new System.Windows.Forms.Button();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.contextMenuStrip1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// button3
			// 
			this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button3.Location = new System.Drawing.Point(203, 204);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(211, 23);
			this.button3.TabIndex = 6;
			this.button3.Text = "장소표시";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 50000;
			this.toolTip1.InitialDelay = 500;
			this.toolTip1.ReshowDelay = 100;
			// 
			// listView1
			// 
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(6, 64);
			this.listView1.Name = "listView1";
			this.listView1.ShowItemToolTips = true;
			this.listView1.Size = new System.Drawing.Size(408, 134);
			this.listView1.TabIndex = 5;
			this.toolTip1.SetToolTip(this.listView1, "おまじない");
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
			this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
			this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
			this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.comboBox1.Location = new System.Drawing.Point(6, 7);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(250, 20);
			this.comboBox1.TabIndex = 0;
			this.toolTip1.SetToolTip(this.comboBox1, "入力された문자열が含まれるものを전체검색します\r\nxxxx,yyyy 形式で入力することで特定の좌표をセンタリングすることができます");
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// listView2
			// 
			this.listView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listView2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listView2.FullRowSelect = true;
			this.listView2.GridLines = true;
			this.listView2.HideSelection = false;
			this.listView2.Location = new System.Drawing.Point(6, 34);
			this.listView2.Name = "listView2";
			this.listView2.ShowItemToolTips = true;
			this.listView2.Size = new System.Drawing.Size(408, 164);
			this.listView2.TabIndex = 29;
			this.toolTip1.SetToolTip(this.listView2, "おまじない");
			this.listView2.UseCompatibleStateImageBehavior = false;
			this.listView2.View = System.Windows.Forms.View.Details;
			this.listView2.SelectedIndexChanged += new System.EventHandler(this.listView2_SelectedIndexChanged);
			this.listView2.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView2_ColumnClick);
			// 
			// listView3
			// 
			this.listView3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listView3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listView3.FullRowSelect = true;
			this.listView3.GridLines = true;
			this.listView3.HideSelection = false;
			this.listView3.Location = new System.Drawing.Point(6, 6);
			this.listView3.Name = "listView3";
			this.listView3.ShowItemToolTips = true;
			this.listView3.Size = new System.Drawing.Size(408, 192);
			this.listView3.TabIndex = 29;
			this.toolTip1.SetToolTip(this.listView3, "おまじない");
			this.listView3.UseCompatibleStateImageBehavior = false;
			this.listView3.View = System.Windows.Forms.View.Details;
			this.listView3.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView3_MouseDoubleClick);
			this.listView3.SelectedIndexChanged += new System.EventHandler(this.listView3_SelectedIndexChanged);
			this.listView3.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView3_ColumnClick);
			// 
			// comboBox4
			// 
			this.comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox4.FormattingEnabled = true;
			this.comboBox4.Items.AddRange(new object[] {
			"부분일치",
			"완전일치",
			"앞부분일치",
			"뒷부분일치"});
			this.comboBox4.Location = new System.Drawing.Point(262, 35);
			this.comboBox4.Name = "comboBox4";
			this.comboBox4.Size = new System.Drawing.Size(101, 20);
			this.comboBox4.TabIndex = 4;
			this.toolTip1.SetToolTip(this.comboBox4, resources.GetString("comboBox4.ToolTip"));
			this.comboBox4.SelectedIndexChanged += new System.EventHandler(this.comboBox4_SelectedIndexChanged);
			// 
			// comboBox3
			// 
			this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox3.FormattingEnabled = true;
			this.comboBox3.Items.AddRange(new object[] {
			"명칭 등",
			"종류"});
			this.comboBox3.Location = new System.Drawing.Point(175, 35);
			this.comboBox3.Name = "comboBox3";
			this.comboBox3.Size = new System.Drawing.Size(81, 20);
			this.comboBox3.TabIndex = 3;
			this.toolTip1.SetToolTip(this.comboBox3, "・명칭 등\r\n　아이템 이름, 도시명, 사용언어검색. \r\n・종류\r\n　아이템종류별로 검색. \r\n　예:식료품");
			this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
			// 
			// comboBox2
			// 
			this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Items.AddRange(new object[] {
			"도시정보",
			"아이템DB",
			"전체정보"});
			this.comboBox2.Location = new System.Drawing.Point(6, 35);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(163, 20);
			this.comboBox2.TabIndex = 2;
			this.toolTip1.SetToolTip(this.comboBox2, "・도시정보\r\n　도시정보 검색. \r\n　검색결과와 장소표시가 가능. \r\n・아이템DB\r\n　독자적인 아이템DB 사용 검색" +
					". \r\n　검색결과와 장소표시가 불가능. \r\n・전체정보\r\n　위의 모두를 검색. ");
			this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.open_recipe_wiki0_ToolStripMenuItem,
			this.open_recipe_wiki1_ToolStripMenuItem,
			this.toolStripSeparator3,
			this.copy_name_to_clipboardToolStripMenuItem,
			this.copy_all_to_clipboardToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip3";
			this.contextMenuStrip1.Size = new System.Drawing.Size(352, 98);
			// 
			// open_recipe_wiki0_ToolStripMenuItem
			// 
			this.open_recipe_wiki0_ToolStripMenuItem.Name = "open_recipe_wiki0_ToolStripMenuItem";
			this.open_recipe_wiki0_ToolStripMenuItem.Size = new System.Drawing.Size(351, 22);
			this.open_recipe_wiki0_ToolStripMenuItem.Text = "레시피の디테일を레시피정보wikiで조사";
			this.open_recipe_wiki0_ToolStripMenuItem.Click += new System.EventHandler(this.open_recipe_wiki0_ToolStripMenuItem_Click);
			// 
			// open_recipe_wiki1_ToolStripMenuItem
			// 
			this.open_recipe_wiki1_ToolStripMenuItem.Name = "open_recipe_wiki1_ToolStripMenuItem";
			this.open_recipe_wiki1_ToolStripMenuItem.Size = new System.Drawing.Size(351, 22);
			this.open_recipe_wiki1_ToolStripMenuItem.Text = "레시피で작성가능かどうか레시피정보wikiで조사";
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
			this.copy_name_to_clipboardToolStripMenuItem.Text = "명칭をクリップボードにコピー";
			this.copy_name_to_clipboardToolStripMenuItem.Click += new System.EventHandler(this.copy_name_to_clipboardToolStripMenuItem_Click);
			// 
			// copy_all_to_clipboardToolStripMenuItem
			// 
			this.copy_all_to_clipboardToolStripMenuItem.Name = "copy_all_to_clipboardToolStripMenuItem";
			this.copy_all_to_clipboardToolStripMenuItem.Size = new System.Drawing.Size(351, 22);
			this.copy_all_to_clipboardToolStripMenuItem.Text = "디테일をクリップボードにコピー";
			this.copy_all_to_clipboardToolStripMenuItem.Click += new System.EventHandler(this.copy_all_to_clipboardToolStripMenuItem_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(428, 259);
			this.tabControl1.TabIndex = 8;
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.Color.Transparent;
			this.tabPage1.Controls.Add(this.comboBox4);
			this.tabPage1.Controls.Add(this.comboBox3);
			this.tabPage1.Controls.Add(this.comboBox2);
			this.tabPage1.Controls.Add(this.button3);
			this.tabPage1.Controls.Add(this.listView1);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.comboBox1);
			this.tabPage1.Controls.Add(this.button1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(420, 233);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "검색";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(373, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 12);
			this.label2.TabIndex = 28;
			this.label2.Text = "0000건";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(262, 6);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(101, 24);
			this.button1.TabIndex = 1;
			this.button1.Text = "검색";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// tabPage3
			// 
			this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.tabPage3.Controls.Add(this.button5);
			this.tabPage3.Controls.Add(this.listView3);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(420, 233);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "문화권";
			// 
			// button5
			// 
			this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button5.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button5.Location = new System.Drawing.Point(203, 204);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(211, 23);
			this.button5.TabIndex = 28;
			this.button5.Text = "장소표시";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.tabPage2.Controls.Add(this.label3);
			this.tabPage2.Controls.Add(this.label1);
			this.tabPage2.Controls.Add(this.button2);
			this.tabPage2.Controls.Add(this.listView2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(420, 233);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "장소표시";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 10);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 12);
			this.label3.TabIndex = 31;
			this.label3.Text = "label3";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(373, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 12);
			this.label1.TabIndex = 30;
			this.label1.Text = "0000건";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button2.Location = new System.Drawing.Point(203, 204);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(211, 23);
			this.button2.TabIndex = 28;
			this.button2.Text = "장소표시해제";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button4
			// 
			this.button4.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button4.Location = new System.Drawing.Point(177, 241);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(37, 15);
			this.button4.TabIndex = 9;
			this.button4.TabStop = false;
			this.button4.Text = "button4";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// find_form2
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button4;
			this.ClientSize = new System.Drawing.Size(452, 283);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.button4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(468, 320);
			this.Name = "find_form2";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "전체검색";
			this.Activated += new System.EventHandler(this.find_form2_Activated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.find_form2_FormClosing);
			this.contextMenuStrip1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem open_recipe_wiki0_ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem open_recipe_wiki1_ToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem copy_name_to_clipboardToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copy_all_to_clipboardToolStripMenuItem;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ListView listView2;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.ListView listView3;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.ComboBox comboBox4;
		private System.Windows.Forms.ComboBox comboBox3;
	}
}