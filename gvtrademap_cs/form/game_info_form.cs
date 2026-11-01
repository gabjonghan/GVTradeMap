/*-------------------------------------------------------------------------

 게임 자료 창
 침몰선, 낚시 발견물, 해역조사, 부관 등 database\gameinfo 의 자료를 보여 준다

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Utility;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class game_info_form : Form
	{
		private gvt_lib		m_lib;
		private game_info	m_info;

		private ListBox		m_list;
		private CheckBox	m_visible;
		private TextBox		m_find;
		private ListView	m_points;
		private RichTextBox	m_text;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public game_info_form(gvt_lib lib, game_info info)
		{
			m_lib			= lib;
			m_info			= info;

			Text			= "게임 자료";
			ShowInTaskbar	= false;
			StartPosition	= FormStartPosition.CenterParent;
			AutoScaleMode	= AutoScaleMode.Font;
			ClientSize		= new Size(640, 480);
			MinimumSize		= new Size(480, 320);

			m_list			= new ListBox();
			m_list.Dock		= DockStyle.Left;
			m_list.Width	= 130;
			m_list.IntegralHeight	= false;
			m_list.SelectedIndexChanged	+= new EventHandler(list_SelectedIndexChanged);

			Panel	top		= new Panel();
			top.Dock		= DockStyle.Top;
			top.Height		= 30;

			m_visible			= new CheckBox();
			m_visible.Text		= "지도에 표시";
			m_visible.Bounds	= new Rectangle(6, 5, 100, 20);
			m_visible.CheckedChanged	+= new EventHandler(visible_CheckedChanged);

			m_find				= new TextBox();
			m_find.Bounds		= new Rectangle(110, 4, 140, 22);
			m_find.TextChanged	+= new EventHandler(find_TextChanged);

			Button	reload		= new Button();
			reload.Text			= "다시 읽기";
			reload.Bounds		= new Rectangle(256, 3, 76, 24);
			reload.Click		+= new EventHandler(reload_Click);

			Button	folder		= new Button();
			folder.Text			= "자료 폴더";
			folder.Bounds		= new Rectangle(336, 3, 76, 24);
			folder.Click		+= new EventHandler(folder_Click);

			top.Controls.AddRange(new Control[]{ m_visible, m_find, reload, folder });

			m_text				= new RichTextBox();
			m_text.Dock			= DockStyle.Top;
			m_text.ReadOnly		= true;
			m_text.BackColor	= SystemColors.Window;
			m_text.DetectUrls	= true;
			m_text.LinkClicked	+= new LinkClickedEventHandler(text_LinkClicked);

			m_points				= new ListView();
			m_points.Dock			= DockStyle.Fill;
			m_points.View			= View.Details;
			m_points.FullRowSelect	= true;
			m_points.HideSelection	= false;
			m_points.Columns.Add("이름", 240);
			m_points.Columns.Add("구역", 130);
			m_points.Columns.Add("좌표", 80);
			m_points.DoubleClick	+= new EventHandler(points_DoubleClick);

			Panel	right	= new Panel();
			right.Dock		= DockStyle.Fill;
			// Dock 은 나중에 넣은 것부터 자리를 잡는다
			right.Controls.Add(m_points);
			right.Controls.Add(m_text);
			right.Controls.Add(top);

			Controls.Add(right);
			Controls.Add(m_list);

			Useful.SetFontMeiryo(this, def.MEIRYO_POINT);
			update_categories();
		}

		/*-------------------------------------------------------------------------
		 닫아도 표시 상태를 유지하도록 숨기기만 한다
		---------------------------------------------------------------------------*/
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if(e.CloseReason == CloseReason.UserClosing){
				e.Cancel	= true;
				Hide();
			}
			base.OnFormClosing(e);
		}

		/*-------------------------------------------------------------------------
		 분류 목록을 갱신한다
		---------------------------------------------------------------------------*/
		private void update_categories()
		{
			m_list.Items.Clear();
			foreach(game_info.category c in m_info.categories){
				m_list.Items.Add(c.name);
			}
			if(m_list.Items.Count > 0)	m_list.SelectedIndex	= 0;
		}

		/*-------------------------------------------------------------------------
		 선택 중인 분류를 얻는다
		---------------------------------------------------------------------------*/
		private game_info.category selected
		{
			get{
				if(m_list.SelectedIndex < 0)	return null;
				return m_info.categories[m_list.SelectedIndex];
			}
		}

		/*-------------------------------------------------------------------------
		 선택 중인 분류의 내용을 보여 준다
		---------------------------------------------------------------------------*/
		private void update_contents()
		{
			game_info.category	c	= selected;
			if(c == null)	return;

			bool	has_points	= c.points.Count > 0;
			m_visible.Enabled	= has_points;
			m_visible.Checked	= c.visible;
			m_find.Enabled		= has_points;
			m_points.Visible	= has_points;

			m_text.Text			= c.text;
			m_text.Visible		= c.text != "";
			// 좌표가 없는 분류는 설명문이 전체를 쓴다
			m_text.Dock			= has_points? DockStyle.Top: DockStyle.Fill;
			m_text.Height		= 90;

			update_points();
		}

		/*-------------------------------------------------------------------------
		 좌표 목록을 갱신한다
		---------------------------------------------------------------------------*/
		private void update_points()
		{
			game_info.category	c	= selected;
			if(c == null)	return;

			string	find	= m_find.Text.Trim();
			m_points.BeginUpdate();
			m_points.Items.Clear();
			foreach(game_info.point p in c.points){
				if((find != "")
					&& (p.name.IndexOf(find, StringComparison.OrdinalIgnoreCase) < 0)
					&& (p.group.IndexOf(find, StringComparison.OrdinalIgnoreCase) < 0))	continue;

				ListViewItem	item	= new ListViewItem(p.name);
				item.SubItems.Add(p.group);
				item.SubItems.Add(String.Format("{0},{1}", p.gpos.X, p.gpos.Y));
				item.Tag				= p;
				m_points.Items.Add(item);
			}
			m_points.EndUpdate();
		}

		private void list_SelectedIndexChanged(object sender, EventArgs e)
		{
			update_contents();
		}

		private void find_TextChanged(object sender, EventArgs e)
		{
			update_points();
		}

		private void visible_CheckedChanged(object sender, EventArgs e)
		{
			game_info.category	c	= selected;
			if(c == null)	return;
			c.visible	= m_visible.Checked;
			m_lib.device.SetMustDrawFlag();
		}

		/*-------------------------------------------------------------------------
		 더블클릭한 좌표를 지도 중심으로
		---------------------------------------------------------------------------*/
		private void points_DoubleClick(object sender, EventArgs e)
		{
			if(m_points.SelectedItems.Count <= 0)	return;
			game_info.point	p	= m_points.SelectedItems[0].Tag as game_info.point;
			if(p == null)		return;

			// 어디인지 보이도록 함께 표시한다
			m_visible.Checked	= true;
			m_lib.setting.centering_gpos	= p.gpos;
			m_lib.setting.req_centering_gpos.Request();
		}

		private void reload_Click(object sender, EventArgs e)
		{
			m_info.Reload();
			update_categories();
			m_lib.device.SetMustDrawFlag();
		}

		private void folder_Click(object sender, EventArgs e)
		{
			if(Directory.Exists(m_info.path))	Process.Start(m_info.path);
		}

		private void text_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			try{
				Process.Start(e.LinkText);
			}catch(Exception){
				MessageBox.Show(this, "링크를 열지 못했습니다.\n" + e.LinkText);
			}
		}
	}
}
