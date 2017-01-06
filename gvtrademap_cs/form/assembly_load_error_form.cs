/*-------------------------------------------------------------------------

 Assemblyロード오류

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
using System.Reflection;
using System.Diagnostics;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public partial class assembly_load_error_form : Form
	{
		private AssemblyName			m_assembly_name;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public assembly_load_error_form(AssemblyName assembly_name)
		{
			InitializeComponent();

			m_assembly_name		= assembly_name;

			string	str	= def.WINDOW_TITLE + "\n";
			str			+= assembly_name.FullName + "\n";
			str			+= " 읽기에 실패하였습니다.\n\n";

			str			+= "교역Map C#의 시작애는 Micrsoft DirectX 9.0C 이상, Managed DirectX(MDX1.1)가 필요합니다.\n";
			str			+= "MDX1.1를 설치하려면 DirectX End-User Runtime Web Installer를 실행하십시오.\n";
			str			+= "DirectX End-User Runtime Web Installer와 MDX1.1를 인스톨하게 됩니다.\n";

			str			+= "\n";
			str			+= "MDX1.1를 설치했음에도 시작되지 않을 경우 오류내용을 보고해주면 대응할 수 있을지도 모릅니다.\n(일본어판은 더이상 업데이트되지 않습니다.)";
	
			textBox1.AcceptsReturn	= true;
			textBox1.Lines			= str.Split(new char[]{'\n'});
			textBox1.Select(0, 0);
		}

		/*-------------------------------------------------------------------------
		 DirectX End-User Runtime Web Installer ダウンロードページを開く
		---------------------------------------------------------------------------*/
		private void button3_Click(object sender, EventArgs e)
		{
			Process.Start(def.URL6);
		}

		/*-------------------------------------------------------------------------
		 오류보고을실시ページを開く
		---------------------------------------------------------------------------*/
		private void button2_Click(object sender, EventArgs e)
		{
			Process.Start(def.URL4);
		}
	}
}
