namespace gvtrademap_cs
{
	partial class sea_routes_form2
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
			this.button1 = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.listView1 = new System.Windows.Forms.ListView();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.show_SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.hide_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.add_AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.delete_DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.move_trash_RToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.all_select_AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.listView2 = new System.Windows.Forms.ListView();
			this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			this.listView3 = new System.Windows.Forms.ListView();
			this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.label1 = new System.Windows.Forms.Label();
			this.button3 = new System.Windows.Forms.Button();
			this.contextMenuStrip1.SuspendLayout();
			this.contextMenuStrip2.SuspendLayout();
			this.contextMenuStrip3.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(14, 12);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(21, 8);
			this.button1.TabIndex = 0;
			this.button1.TabStop = false;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// listView1
			// 
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listView1.ContextMenuStrip = this.contextMenuStrip1;
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(7, 6);
			this.listView1.Name = "listView1";
			this.listView1.ShowItemToolTips = true;
			this.listView1.Size = new System.Drawing.Size(735, 192);
			this.listView1.TabIndex = 6;
			this.toolTip1.SetToolTip(this.listView1, "おまじない");
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.VirtualMode = true;
			this.listView1.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listView1_RetrieveVirtualItem);
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.show_SToolStripMenuItem,
			this.hide_ToolStripMenuItem,
			this.toolStripSeparator1,
			this.add_AToolStripMenuItem,
			this.delete_DToolStripMenuItem,
			this.move_trash_RToolStripMenuItem,
			this.toolStripSeparator2,
			this.all_select_AToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(238, 148);
			this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
			// 
			// show_SToolStripMenuItem
			// 
			this.show_SToolStripMenuItem.Name = "show_SToolStripMenuItem";
			this.show_SToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
			this.show_SToolStripMenuItem.Text = "표시(&S)";
			this.show_SToolStripMenuItem.Click += new System.EventHandler(this.show_hide_SToolStripMenuItem_Click);
			// 
			// hide_ToolStripMenuItem
			// 
			this.hide_ToolStripMenuItem.Name = "hide_ToolStripMenuItem";
			this.hide_ToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
			this.hide_ToolStripMenuItem.Text = "비표시(&H)";
			this.hide_ToolStripMenuItem.Click += new System.EventHandler(this.hide_ToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(234, 6);
			// 
			// add_AToolStripMenuItem
			// 
			this.add_AToolStripMenuItem.Name = "add_AToolStripMenuItem";
			this.add_AToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
			this.add_AToolStripMenuItem.Text = "즐겨찾기 항로도 목록으로 이동(&M)";
			this.add_AToolStripMenuItem.Click += new System.EventHandler(this.add_AToolStripMenuItem_Click);
			// 
			// delete_DToolStripMenuItem
			// 
			this.delete_DToolStripMenuItem.Name = "delete_DToolStripMenuItem";
			this.delete_DToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
			this.delete_DToolStripMenuItem.Text = "과거의 항로도 목록으로 이동(&D)";
			this.delete_DToolStripMenuItem.Click += new System.EventHandler(this.delete_DToolStripMenuItem_Click);
			// 
			// move_trash_RToolStripMenuItem
			// 
			this.move_trash_RToolStripMenuItem.Name = "move_trash_RToolStripMenuItem";
			this.move_trash_RToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
			this.move_trash_RToolStripMenuItem.Text = "삭제(&R)";
			this.move_trash_RToolStripMenuItem.Click += new System.EventHandler(this.move_trash_RToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(234, 6);
			// 
			// all_select_AToolStripMenuItem
			// 
			this.all_select_AToolStripMenuItem.Name = "all_select_AToolStripMenuItem";
			this.all_select_AToolStripMenuItem.Size = new System.Drawing.Size(237, 22);
			this.all_select_AToolStripMenuItem.Text = "전부선택(&A)";
			this.all_select_AToolStripMenuItem.Click += new System.EventHandler(this.all_select_AToolStripMenuItem_Click);
			// 
			// listView2
			// 
			this.listView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.listView2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listView2.ContextMenuStrip = this.contextMenuStrip2;
			this.listView2.FullRowSelect = true;
			this.listView2.GridLines = true;
			this.listView2.HideSelection = false;
			this.listView2.Location = new System.Drawing.Point(7, 6);
			this.listView2.Name = "listView2";
			this.listView2.ShowItemToolTips = true;
			this.listView2.Size = new System.Drawing.Size(735, 192);
			this.listView2.TabIndex = 7;
			this.toolTip1.SetToolTip(this.listView2, "주술");
			this.listView2.UseCompatibleStateImageBehavior = false;
			this.listView2.View = System.Windows.Forms.View.Details;
			this.listView2.VirtualMode = true;
			this.listView2.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listView2_RetrieveVirtualItem);
			this.listView2.SelectedIndexChanged += new System.EventHandler(this.listView2_SelectedIndexChanged);
			// 
			// contextMenuStrip2
			// 
			this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripMenuItem7,
			this.toolStripMenuItem2,
			this.toolStripSeparator3,
			this.toolStripMenuItem3,
			this.toolStripMenuItem4,
			this.toolStripSeparator4,
			this.toolStripMenuItem5});
			this.contextMenuStrip2.Name = "contextMenuStrip1";
			this.contextMenuStrip2.Size = new System.Drawing.Size(224, 126);
			this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
			// 
			// toolStripMenuItem7
			// 
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Size = new System.Drawing.Size(223, 22);
			this.toolStripMenuItem7.Text = "표시(&S)";
			this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(223, 22);
			this.toolStripMenuItem2.Text = "비표시(&H)";
			this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(220, 6);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(223, 22);
			this.toolStripMenuItem3.Text = "과거의 항로도 목록으로 이동(&D)";
			this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(223, 22);
			this.toolStripMenuItem4.Text = "삭제(&R)";
			this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(220, 6);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(223, 22);
			this.toolStripMenuItem5.Text = "전부선택(&A)";
			this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
			// 
			// listView3
			// 
			this.listView3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.listView3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listView3.ContextMenuStrip = this.contextMenuStrip3;
			this.listView3.FullRowSelect = true;
			this.listView3.GridLines = true;
			this.listView3.HideSelection = false;
			this.listView3.Location = new System.Drawing.Point(7, 6);
			this.listView3.Name = "listView3";
			this.listView3.ShowItemToolTips = true;
			this.listView3.Size = new System.Drawing.Size(735, 192);
			this.listView3.TabIndex = 8;
			this.toolTip1.SetToolTip(this.listView3, "주술");
			this.listView3.UseCompatibleStateImageBehavior = false;
			this.listView3.View = System.Windows.Forms.View.Details;
			this.listView3.VirtualMode = true;
			this.listView3.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listView3_RetrieveVirtualItem);
			this.listView3.SelectedIndexChanged += new System.EventHandler(this.listView3_SelectedIndexChanged);
			// 
			// contextMenuStrip3
			// 
			this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripMenuItem6,
			this.toolStripMenuItem8,
			this.toolStripSeparator6,
			this.toolStripMenuItem9});
			this.contextMenuStrip3.Name = "contextMenuStrip1";
			this.contextMenuStrip3.Size = new System.Drawing.Size(238, 76);
			this.contextMenuStrip3.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip3_Opening);
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(237, 22);
			this.toolStripMenuItem6.Text = "즐겨찾기항로도목록に이동(&M)";
			this.toolStripMenuItem6.Click += new System.EventHandler(this.toolStripMenuItem6_Click);
			// 
			// toolStripMenuItem8
			// 
			this.toolStripMenuItem8.Name = "toolStripMenuItem8";
			this.toolStripMenuItem8.Size = new System.Drawing.Size(237, 22);
			this.toolStripMenuItem8.Text = "삭제(&R)";
			this.toolStripMenuItem8.Click += new System.EventHandler(this.toolStripMenuItem8_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(234, 6);
			// 
			// toolStripMenuItem9
			// 
			this.toolStripMenuItem9.Name = "toolStripMenuItem9";
			this.toolStripMenuItem9.Size = new System.Drawing.Size(237, 22);
			this.toolStripMenuItem9.Text = "전부선택(&A)";
			this.toolStripMenuItem9.Click += new System.EventHandler(this.toolStripMenuItem9_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Location = new System.Drawing.Point(14, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.ShowToolTips = true;
			this.tabControl1.Size = new System.Drawing.Size(758, 230);
			this.tabControl1.TabIndex = 1;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.listView1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(750, 204);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "항로도";
			this.tabPage1.ToolTipText = "통상의 항로도목록. 설정항목의 유지수 설정을 초과하는 오래된 항로도는 자동으로 과거의 항로도에 이동됩니다.";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.tabPage2.Controls.Add(this.listView2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(750, 204);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "항로도즐겨찾기";
			this.tabPage2.ToolTipText = "즐겨찾기 항로도를 등록. 자동으로 삭제되지 않습니다.";
			// 
			// tabPage3
			// 
			this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.tabPage3.Controls.Add(this.listView3);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(750, 204);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "휴지통";
			this.tabPage3.ToolTipText = "과거의 항로도 목록으로 그리지 않습니다. 이 때문에, CPU부하가 가볍고 많은 항로도를 유지할 수 있습니다.";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 253);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(173, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "우클릭 메뉴로 삭제 가능합니다";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// button3
			// 
			this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button3.Location = new System.Drawing.Point(573, 248);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(188, 23);
			this.button3.TabIndex = 7;
			this.button3.Text = "선택상태 해제";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// sea_routes_form2
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button1;
			this.ClientSize = new System.Drawing.Size(786, 283);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "sea_routes_form2";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "항로도 목록";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.sea_routes_form2_FormClosing);
			this.Load += new System.EventHandler(this.sea_routes_form2_Load);
			this.Shown += new System.EventHandler(this.sea_routes_form2_Shown);
			this.contextMenuStrip1.ResumeLayout(false);
			this.contextMenuStrip2.ResumeLayout(false);
			this.contextMenuStrip3.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem add_AToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem delete_DToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem show_SToolStripMenuItem;
		private System.Windows.Forms.ListView listView2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.ListView listView3;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem all_select_AToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem move_trash_RToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
		private System.Windows.Forms.ToolStripMenuItem hide_ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
	}
}