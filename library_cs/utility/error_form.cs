/*-------------------------------------------------------------------------

 예기치 못한 오류
 스택 트레이스를 표시한다
 지정된 URL 로 이동 가능

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

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace Utility
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public partial class error_form : Form
	{
		private string				m_message;
		private string				m_url;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public error_form(string window_title, Exception ex, string message_top, string url)
		{
			initialize(window_title, ex, message_top, url, "");
		}	
		public error_form(string window_title, Exception ex, string message_top, string url, string device_info_string)
		{
			initialize(window_title, ex, message_top, url, device_info_string);
		}	

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private void initialize(string window_title, Exception ex, string message_top, string url, string device_info_string)
		{
			InitializeComponent();

			// 오류 내용 생성
			string		message		= "";

			if(!String.IsNullOrEmpty(message_top)){
				message		+= message_top + "\n";
			}

			// 일시
			message			+= "DATE:" + Useful.TojbbsDateTimeString(DateTime.Now) + "\n";

			// OS버전
			OperatingSystem	os	= Environment.OSVersion;
			message			+= "OS:" + os.VersionString + "\n";
			message			+= "OS:" + Useful.GetOsName(os) + "\n";

			// device info string
			if(!String.IsNullOrEmpty(device_info_string)){
				message		+= "DeviceInfo:" + device_info_string + "\n";
			}
	
			if(ex == null){
				//
				message		+= "오류 내용이 불명";
			}else{
				message		+= "Message: " + ex.Message + "\nStackTrace:\n";
				message		+= make_error_message(ex.StackTrace);
			}
			m_message	= message;

			// window 타이틀
			if(!String.IsNullOrEmpty(window_title)){
				this.Text	= window_title;
			}

			// 오류 내용
			textBox1.AcceptsReturn	= true;
			textBox1.Lines			= m_message.Split(new char[]{'\n'});
			textBox1.Select(0, 0);

			// 오류 보고 버튼
			if(!String.IsNullOrEmpty(url)){
				m_url	= url;
			}else{
				m_url	= "";
				button2.Enabled	= false;
				label2.Text		= "--";
			}
		}

		/*-------------------------------------------------------------------------
		 오류 내용을 클립보드에 복사한다
		---------------------------------------------------------------------------*/
		private void button3_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(m_message);
		}

		/*-------------------------------------------------------------------------
		 오류 보고를 하는 페이지를 연다
		---------------------------------------------------------------------------*/
		private void button2_Click(object sender, EventArgs e)
		{
			if(m_url != ""){
				Process.Start(m_url);
			}
		}

		/*-------------------------------------------------------------------------
		 오류 메시지를 생성한다
		 소스 위치를 삭제했었지만, 필요 없어 보여서 거의 아무것도 하지 않고 반환하도록 변경
		---------------------------------------------------------------------------*/
		static private string make_error_message(string str)
		{
			try{
				str		= str.Replace("\r\n", "\n");
				return str;
			}catch{
				// 무언가 실패했을 때는 그대로 반환한다
				return str;
			}
		}
	}
}
