/*-------------------------------------------------------------------------

 航路削除ダイアログ

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
using utility;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public partial class sea_routes_form : Form
	{
		private	setting					m_setting;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public setting _setting{		get{	return m_setting;		}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public sea_routes_form(setting _setting)
		{
			// 設定内容をコピーして持つ
			m_setting				= _setting.Clone();

			InitializeComponent();
			useful.SetFontMeiryo(this, def.MEIRYO_POINT);

			checkBox1.Checked		= m_setting.remove_sea_routes_routes;
			checkBox2.Checked		= m_setting.remove_sea_routes_popup;
			checkBox3.Checked		= m_setting.remove_sea_routes_accident;
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private void sea_routes_form_FormClosed(object sender, FormClosedEventArgs e)
		{
			m_setting.remove_sea_routes_routes		= checkBox1.Checked;
			m_setting.remove_sea_routes_popup		= checkBox2.Checked;
			m_setting.remove_sea_routes_accident	= checkBox3.Checked;
		}
	}
}
