/*-------------------------------------------------------------------------

 ドラッグ&ドロップされた해역정보

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

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public partial class sea_area_dd_form : Form
	{
		private gvt_lib							m_lib;
		private List<sea_area_once_from_dd>		m_list;
		private GvoDatabase						m_db;

		// フィルタ後の목록
		private List<sea_area_once_from_dd>		m_filterd_list;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public List<sea_area_once_from_dd>	filterd_list	{	get{	return m_filterd_list;	}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public sea_area_dd_form(gvt_lib lib, GvoDatabase db, List<sea_area_once_from_dd> list)
		{
			m_lib				= lib;
			m_list				= list;
			m_db				= db;

			m_filterd_list		= new List<sea_area_once_from_dd>();

			InitializeComponent();
			Useful.SetFontMeiryo(this, def.MEIRYO_POINT);

			listView1.Columns.Add("서버",		80);
			listView1.Columns.Add("해역명",		100);
			listView1.Columns.Add("상태",		100);
			listView1.Columns.Add("終了日時",	180);
			listView1.Columns.Add("補足",		100);
	
			checkBox1.Checked	= true;
			checkBox2.Checked	= true;

			// 목록업데이트
			update_list();
		}

		/*-------------------------------------------------------------------------
		 목록업데이트
		---------------------------------------------------------------------------*/
		private void update_list()
		{
			listView1.BeginUpdate();
			listView1.Items.Clear();

			foreach(sea_area_once_from_dd o in m_list){
				ListViewItem	item	= new ListViewItem(o.server_str, 0);
				item.UseItemStyleForSubItems	= false;
				item.Tag				= o;
				item.SubItems.Add(o.name);
				item.SubItems.Add(o._sea_type_str);
				item.SubItems.Add(Useful.TojbbsDateTimeString(o.date));

				bool	check_date	= (o.date < DateTime.Now)? false: true;
				item.SubItems.Add((check_date)? "継続중": "期限切れ");

				switch(o._sea_type){
				case sea_area.sea_area_once.sea_type.safty:
					item.SubItems[2].ForeColor	= Color.Blue;
					break;
				case sea_area.sea_area_once.sea_type.lawless:
					item.SubItems[2].ForeColor	= Color.Red;
					break;
				}
				item.SubItems[4].ForeColor		= (check_date)? Color.Green: Color.Red;

				// フィルタ
				if(checkBox1.Checked){
					// 서버によるフィルタ
					if(m_db.World.MyServer != GvoWorldInfo.GetServerFromString(o.server_str)){
						continue;
					}
				}
				if(checkBox2.Checked){
					// 期限によるフィルタ
					if(o.date < DateTime.Now){
						continue;
					}
				}
				
				listView1.Items.Add(item);
			}
			listView1.EndUpdate();
		}

		/*-------------------------------------------------------------------------
		 チェックボックスの내용が변경された
		---------------------------------------------------------------------------*/
		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			// 목록업데이트
			update_list();
		}
		/*-------------------------------------------------------------------------
		 チェックボックスの내용が변경された
		---------------------------------------------------------------------------*/
		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			// 목록업데이트
			update_list();
		}

		/*-------------------------------------------------------------------------
		 閉じられた
		---------------------------------------------------------------------------*/
		private void sea_area_dd_form_FormClosed(object sender, FormClosedEventArgs e)
		{
			m_filterd_list.Clear();
			foreach(ListViewItem i in listView1.Items){
				sea_area_once_from_dd	data	= i.Tag as sea_area_once_from_dd;
				if(data == null)	continue;
				m_filterd_list.Add(data);
			}
		}

        private void sea_area_dd_form_Load(object sender, EventArgs e)
        {

        }
    }
}
