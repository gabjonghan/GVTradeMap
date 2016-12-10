/*-------------------------------------------------------------------------

 main form

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

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public partial class draw_setting_form : Form
	{
		private	setting					m_setting;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public setting _setting{		get{	return m_setting;		}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public draw_setting_form(setting _setting, draw_setting_page type)
		{
			// 設定内容をコピーして持つ
			m_setting				= _setting.Clone();

			InitializeComponent();
			useful.useful.SetFontMeiryo(this, def.MEIRYO_POINT);

			// tooltip
			toolTip1.AutoPopDelay		= 30*1000;		// 30秒表示
			toolTip1.BackColor			= Color.LightYellow;

			// @Web icons
			{
				draw_setting_web_icons	flag	= m_setting.draw_setting_web_icons;
				checkBox1.Checked			= (flag & draw_setting_web_icons.wind) != 0;
				checkBox2.Checked			= (flag & draw_setting_web_icons.accident_0) != 0;
				checkBox3.Checked			= (flag & draw_setting_web_icons.accident_1) != 0;
				checkBox4.Checked			= (flag & draw_setting_web_icons.accident_2) != 0;
				checkBox5.Checked			= (flag & draw_setting_web_icons.accident_3) != 0;
				checkBox6.Checked			= (flag & draw_setting_web_icons.accident_4) != 0;
			}
			// memo icons
			{
				draw_setting_memo_icons	flag	= m_setting.draw_setting_memo_icons;
				checkBox12.Checked			= (flag & draw_setting_memo_icons.wind) != 0;
				checkBox11.Checked			= (flag & draw_setting_memo_icons.memo_0) != 0;
				checkBox10.Checked			= (flag & draw_setting_memo_icons.memo_1) != 0;
				checkBox9.Checked			= (flag & draw_setting_memo_icons.memo_2) != 0;
				checkBox8.Checked			= (flag & draw_setting_memo_icons.memo_3) != 0;
				checkBox7.Checked			= (flag & draw_setting_memo_icons.memo_4) != 0;
				checkBox13.Checked			= (flag & draw_setting_memo_icons.memo_5) != 0;
				checkBox19.Checked			= (flag & draw_setting_memo_icons.memo_6) != 0;
				checkBox18.Checked			= (flag & draw_setting_memo_icons.memo_7) != 0;
				checkBox17.Checked			= (flag & draw_setting_memo_icons.memo_8) != 0;
				checkBox16.Checked			= (flag & draw_setting_memo_icons.memo_9) != 0;
				checkBox15.Checked			= (flag & draw_setting_memo_icons.memo_10) != 0;
				checkBox14.Checked			= (flag & draw_setting_memo_icons.memo_11) != 0;
			}
			// 災害
			{
				draw_setting_accidents	flag	= m_setting.draw_setting_accidents;
				checkBox25.Checked			= (flag & draw_setting_accidents.accident_0) != 0;
				checkBox24.Checked			= (flag & draw_setting_accidents.accident_1) != 0;
				checkBox23.Checked			= (flag & draw_setting_accidents.accident_2) != 0;
				checkBox22.Checked			= (flag & draw_setting_accidents.accident_3) != 0;
				checkBox21.Checked			= (flag & draw_setting_accidents.accident_4) != 0;
				checkBox20.Checked			= (flag & draw_setting_accidents.accident_5) != 0;
				checkBox31.Checked			= (flag & draw_setting_accidents.accident_6) != 0;
				checkBox30.Checked			= (flag & draw_setting_accidents.accident_7) != 0;
				checkBox29.Checked			= (flag & draw_setting_accidents.accident_8) != 0;
				checkBox28.Checked			= (flag & draw_setting_accidents.accident_9) != 0;
				checkBox27.Checked			= (flag & draw_setting_accidents.accident_10) != 0;
			}
			// 予想線
			{
				draw_setting_myship_angle	flag	= m_setting.draw_setting_myship_angle;
				checkBox26.Checked			= (flag & draw_setting_myship_angle.draw_0) != 0;
				checkBox32.Checked			= (flag & draw_setting_myship_angle.draw_1) != 0;
				checkBox33.Checked			= m_setting.draw_setting_myship_angle_with_speed_pos;
			}

			// 表示するページの設定
			if((int)type < 0)							type	= draw_setting_page.web_icons;
			if(type > draw_setting_page.myship_angle)	type	= draw_setting_page.myship_angle;
			tabControl1.SelectTab((int)type);
		}

		/*-------------------------------------------------------------------------
		 閉じられた
		---------------------------------------------------------------------------*/
		private void draw_setting_form_FormClosed(object sender, FormClosedEventArgs e)
		{
			{
				draw_setting_web_icons	flag	= 0;
				flag		|= (checkBox1.Checked)? draw_setting_web_icons.wind: 0;
				flag		|= (checkBox2.Checked)? draw_setting_web_icons.accident_0: 0;
				flag		|= (checkBox3.Checked)? draw_setting_web_icons.accident_1: 0;
				flag		|= (checkBox4.Checked)? draw_setting_web_icons.accident_2: 0;
				flag		|= (checkBox5.Checked)? draw_setting_web_icons.accident_3: 0;
				flag		|= (checkBox6.Checked)? draw_setting_web_icons.accident_4: 0;
				m_setting.draw_setting_web_icons	= flag;
			}
			{
				draw_setting_memo_icons	flag	= 0;
				flag		|= (checkBox12.Checked)? draw_setting_memo_icons.wind: 0;
				flag		|= (checkBox11.Checked)? draw_setting_memo_icons.memo_0: 0;
				flag		|= (checkBox10.Checked)? draw_setting_memo_icons.memo_1: 0;
				flag		|= (checkBox9.Checked)? draw_setting_memo_icons.memo_2: 0;
				flag		|= (checkBox8.Checked)? draw_setting_memo_icons.memo_3: 0;
				flag		|= (checkBox7.Checked)? draw_setting_memo_icons.memo_4: 0;
				flag		|= (checkBox13.Checked)? draw_setting_memo_icons.memo_5: 0;
				flag		|= (checkBox19.Checked)? draw_setting_memo_icons.memo_6: 0;
				flag		|= (checkBox18.Checked)? draw_setting_memo_icons.memo_7: 0;
				flag		|= (checkBox17.Checked)? draw_setting_memo_icons.memo_8: 0;
				flag		|= (checkBox16.Checked)? draw_setting_memo_icons.memo_9: 0;
				flag		|= (checkBox15.Checked)? draw_setting_memo_icons.memo_10: 0;
				flag		|= (checkBox14.Checked)? draw_setting_memo_icons.memo_11: 0;
				m_setting.draw_setting_memo_icons	= flag;
			}
			{
				draw_setting_accidents	flag	= 0;
				flag		|= (checkBox25.Checked)? draw_setting_accidents.accident_0: 0;
				flag		|= (checkBox24.Checked)? draw_setting_accidents.accident_1: 0;
				flag		|= (checkBox23.Checked)? draw_setting_accidents.accident_2: 0;
				flag		|= (checkBox22.Checked)? draw_setting_accidents.accident_3: 0;
				flag		|= (checkBox21.Checked)? draw_setting_accidents.accident_4: 0;
				flag		|= (checkBox20.Checked)? draw_setting_accidents.accident_5: 0;
				flag		|= (checkBox31.Checked)? draw_setting_accidents.accident_6: 0;
				flag		|= (checkBox30.Checked)? draw_setting_accidents.accident_7: 0;
				flag		|= (checkBox29.Checked)? draw_setting_accidents.accident_8: 0;
				flag		|= (checkBox28.Checked)? draw_setting_accidents.accident_9: 0;
				flag		|= (checkBox27.Checked)? draw_setting_accidents.accident_10: 0;
				m_setting.draw_setting_accidents	= flag;
			}
			{
				draw_setting_myship_angle	flag	= 0;
				flag		|= (checkBox26.Checked)? draw_setting_myship_angle.draw_0: 0;
				flag		|= (checkBox32.Checked)? draw_setting_myship_angle.draw_1: 0;
				m_setting.draw_setting_myship_angle	= flag;
				m_setting.draw_setting_myship_angle_with_speed_pos	= checkBox33.Checked;
			}
		}
	}
}
