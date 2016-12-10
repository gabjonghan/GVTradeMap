/*-------------------------------------------------------------------------

 업데이트チェック결과ダイア로그

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
using System.Diagnostics;
using Utility;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public partial class check_update_result : Form
	{
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public check_update_result(string[] data)
		{
			InitializeComponent();

			textBox1.AcceptsReturn	= true;
			textBox1.Lines			= data;
			textBox1.Select(0, 0);

			Useful.SetFontMeiryo(this, def.MEIRYO_POINT);
		}

		/*-------------------------------------------------------------------------
		 ダウンロードページを開く
		---------------------------------------------------------------------------*/
		private void button4_Click(object sender, EventArgs e)
		{
			Process.Start(def.DOWNLOAD_URL);
		}

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
