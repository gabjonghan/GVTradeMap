/*-------------------------------------------------------------------------

 항로공유詳細

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
using System.Collections;

using Utility;
using Utility.Ctrl;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public partial class share_routes_form : Form
	{
		private Point						m_selected_position;
		private ListViewItemSorter			m_sorter;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public bool is_selected{
			get{
				if(m_selected_position.X < 0)	return false;
				if(m_selected_position.Y < 0)	return false;
				return true;
			}
		}
		public Point selected_position{		get{	return m_selected_position;		}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public share_routes_form(List<ShareRoutes.ShareShip> list)
		{
			m_sorter					= new ListViewItemSorter();
			m_selected_position			= new Point(-1, -1);
	
			InitializeComponent();
			Useful.SetFontMeiryo(this, def.MEIRYO_POINT);

			label2.Text					= String.Format("{0}인", list.Count);

			listView1.FullRowSelect		= true;
			listView1.GridLines			= true;

			listView1.Columns.Add("이름",	150);
			listView1.Columns.Add("장소",	100);
			listView1.Columns.Add("상태",	100);

			foreach(ShareRoutes.ShareShip s in list){
				ListViewItem	item	= new ListViewItem(s.Name, 0);
				item.SubItems.Add(String.Format("{0},{1}", s.Position.X, s.Position.Y));
				if(s.State == ShareRoutes.State.in_the_sea){
					item.SubItems.Add("항해중");
				}else{
					item.SubItems.Add("정박중");
				}
				listView1.Items.Add(item);
			}
		}

		/*-------------------------------------------------------------------------
		 閉じられた
		---------------------------------------------------------------------------*/
		private void share_routes_form_FormClosed(object sender, FormClosedEventArgs e)
		{
			if(listView1.SelectedItems.Count <= 0)		return;

			ListViewItem	item	= listView1.SelectedItems[0];
			string		positon		= item.SubItems[1].Text;
			string[]	split		= positon.Split(new char[]{','});
			if(split.Length != 2)						return;

			m_selected_position		= Useful.ToPoint(split[0], split[1], new Point(-1, -1));
		}
	
		/*-------------------------------------------------------------------------
		 ヘッダを클릭
		---------------------------------------------------------------------------*/
		private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// ソートする
			m_sorter.Sort(listView1, e.Column);
		}

		/*-------------------------------------------------------------------------
		 ダブル클릭
		---------------------------------------------------------------------------*/
		private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			button1.PerformClick();
		}
	}
}
