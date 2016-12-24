namespace gvtrademap_cs
{
	partial class setting_sea_area_form
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
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.listView1 = new System.Windows.Forms.ListView();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.button4 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(384, 340);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(97, 23);
			this.button2.TabIndex = 11;
			this.button2.Text = "취소";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(280, 340);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(97, 23);
			this.button1.TabIndex = 10;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// button3
			// 
			this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button3.Location = new System.Drawing.Point(14, 311);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(247, 23);
			this.button3.TabIndex = 12;
			this.button3.Text = "모든해역군을통상상태로복원";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// listView1
			// 
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(14, 12);
			this.listView1.Name = "listView1";
			this.listView1.ShowItemToolTips = true;
			this.listView1.Size = new System.Drawing.Size(466, 293);
			this.listView1.TabIndex = 13;
			this.toolTip1.SetToolTip(this.listView1, "おまじない");
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.listView1_ColumnWidthChanged);
			this.listView1.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.listView1_ColumnWidthChanging);
			this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
			this.listView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDown);
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
			"위험해역(통상상태)",
			"안전해역",
			"무법해역"});
			this.comboBox1.Location = new System.Drawing.Point(142, 77);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(142, 20);
			this.comboBox1.TabIndex = 14;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 50000;
			this.toolTip1.InitialDelay = 500;
			this.toolTip1.ReshowDelay = 100;
			// 
			// button4
			// 
			this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button4.Location = new System.Drawing.Point(14, 340);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(247, 23);
			this.button4.TabIndex = 15;
			this.button4.Text = "해역정보수집시작";
			this.toolTip1.SetToolTip(this.button4, "해역변동수집시작함. \r\n해역변동수집は\r\n・したらば掲示판の안전해역정보スレ\r\n・대항해시대Onlineまとめwiki(統括)\r\nから해역변동정보を取得します" +
		". ");
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// setting_sea_area_form
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button2;
			this.ClientSize = new System.Drawing.Size(495, 375);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(499, 328);
			this.Name = "setting_sea_area_form";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "위험해역변동시스템설정";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.setting_sea_area_form_FormClosed);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button button4;
	}
}