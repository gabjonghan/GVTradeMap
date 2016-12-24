/*-------------------------------------------------------------------------

 메모아이콘編集

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
	public partial class map_mark_form : Form
	{
		private int				m_icon_index;
		private Point			m_position;
		private string			m_memo;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public int icon_index{		get{	return m_icon_index;	}}
		public string memo{			get{	return m_memo;			}}
		public Point position{		get{	return m_position;		}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public map_mark_form(Point position)
		{
			init(position, 0, "");
		}
		public map_mark_form(Point position, int index, string memo)
		{
			init(position, index, memo);
		}
		private void init(Point position, int index, string memo)
		{

			InitializeComponent();
			Useful.SetFontMeiryo(this, def.MEIRYO_POINT);

			m_position	= position;

			// 메모
			textBox1.Text	= memo;

			// 좌표
			textBox2.Text	= position.X.ToString();
			textBox3.Text	= position.Y.ToString();
	
			toolTip1.AutoPopDelay	= 30*1000;		// 30초표시
			toolTip1.BackColor		= Color.LightYellow;
			toolTip1.SetToolTip(button1, "풍향");
			toolTip1.SetToolTip(button2, "풍향");
			toolTip1.SetToolTip(button3, "풍향");
			toolTip1.SetToolTip(button4, "풍향");
			toolTip1.SetToolTip(button5, "풍향");
			toolTip1.SetToolTip(button6, "풍향");
			toolTip1.SetToolTip(button7, "풍향");
			toolTip1.SetToolTip(button8, "풍향");

			toolTip1.SetToolTip(button9, "상어");
			toolTip1.SetToolTip(button10, "화재");
			toolTip1.SetToolTip(button11, "수초");
			toolTip1.SetToolTip(button12, "사이렌");
			toolTip1.SetToolTip(button13, "지자기이상");
			toolTip1.SetToolTip(button14, "어장");

			toolTip1.SetToolTip(button15, "マンボウ");
			toolTip1.SetToolTip(button16, "free");
			toolTip1.SetToolTip(button17, "free");
			toolTip1.SetToolTip(button18, "free");
			toolTip1.SetToolTip(button19, "free");
			toolTip1.SetToolTip(button20, "목적지");

			toolTip1.SetToolTip(textBox1, "메모を入力してください");
			toolTip1.SetToolTip(textBox2, "위치Xを지정してください");
			toolTip1.SetToolTip(textBox3, "위치Yを지정してください");

			button1.Click	+= new System.EventHandler(button1_Click);
			button2.Click	+= new System.EventHandler(button2_Click);
			button3.Click	+= new System.EventHandler(button3_Click);
			button4.Click	+= new System.EventHandler(button4_Click);
			button5.Click	+= new System.EventHandler(button5_Click);
			button6.Click	+= new System.EventHandler(button6_Click);
			button7.Click	+= new System.EventHandler(button7_Click);
			button8.Click	+= new System.EventHandler(button8_Click);
			button9.Click	+= new System.EventHandler(button9_Click);
			button10.Click	+= new System.EventHandler(button10_Click);
			button11.Click	+= new System.EventHandler(button11_Click);
			button12.Click	+= new System.EventHandler(button12_Click);
			button13.Click	+= new System.EventHandler(button13_Click);
			button14.Click	+= new System.EventHandler(button14_Click);
			button15.Click	+= new System.EventHandler(button15_Click);
			button16.Click	+= new System.EventHandler(button16_Click);
			button17.Click	+= new System.EventHandler(button17_Click);
			button18.Click	+= new System.EventHandler(button18_Click);
			button19.Click	+= new System.EventHandler(button19_Click);
			button20.Click	+= new System.EventHandler(button20_Click);
	
			change_icon(index);
		}

		/*-------------------------------------------------------------------------
		 선택されてる아이콘を변경する
		---------------------------------------------------------------------------*/
		private void change_icon(int index)
		{
			Button[]	tbl	= new Button[]{
				button1, button2, button3, button4,
				button5, button6, button7, button8,
				button9, button10, button11, button12,
				button13, button14, button15, button16,
				button17, button18, button19, button20,
			};
	
			if(index < 0)	index	= 0;
			if(index >= 20)	index	= 19;

			m_icon_index		= index;

			string		str		= toolTip1.GetToolTip(tbl[m_icon_index]);
			toolTip1.SetToolTip(button23, str);
			label1.Text			= str;
			button23.BackgroundImage	= tbl[m_icon_index].BackgroundImage;
		}

		/*-------------------------------------------------------------------------
		 아이콘선택
		---------------------------------------------------------------------------*/
		private void button1_Click(object sender, EventArgs e){			change_icon(0);		}
		private void button2_Click(object sender, EventArgs e){			change_icon(1);		}
		private void button3_Click(object sender, EventArgs e){			change_icon(2);		}
		private void button4_Click(object sender, EventArgs e){			change_icon(3);		}
		private void button5_Click(object sender, EventArgs e){			change_icon(4);		}
		private void button6_Click(object sender, EventArgs e){			change_icon(5);		}
		private void button7_Click(object sender, EventArgs e){			change_icon(6);		}
		private void button8_Click(object sender, EventArgs e){			change_icon(7);		}
		private void button9_Click(object sender, EventArgs e){			change_icon(8);		}
		private void button10_Click(object sender, EventArgs e){		change_icon(9);		}

		private void button11_Click(object sender, EventArgs e){		change_icon(10);	}
		private void button12_Click(object sender, EventArgs e){		change_icon(11);	}
		private void button13_Click(object sender, EventArgs e){		change_icon(12);	}
		private void button14_Click(object sender, EventArgs e){		change_icon(13);	}
		private void button15_Click(object sender, EventArgs e){		change_icon(14);	}
		private void button16_Click(object sender, EventArgs e){		change_icon(15);	}
		private void button17_Click(object sender, EventArgs e){		change_icon(16);	}
		private void button18_Click(object sender, EventArgs e){		change_icon(17);	}
		private void button19_Click(object sender, EventArgs e){		change_icon(18);	}

		private void button20_Click(object sender, EventArgs e){		change_icon(19);	}

		/*-------------------------------------------------------------------------
		 閉じられた
		---------------------------------------------------------------------------*/
		private void map_mark_form_FormClosed(object sender, FormClosedEventArgs e)
		{
			m_memo			= textBox1.Text;
			try{
				int		x		= Convert.ToInt32(textBox2.Text);
				int		y		= Convert.ToInt32(textBox3.Text);
				m_position.X	= x;
				m_position.Y	= y;
			}catch{
			}
		}
	}
}
