/*-------------------------------------------------------------------------

 profile

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Text;
using System.Runtime.InteropServices;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace useful
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class profile
	{
		private const int			BUFF_LEN		= 256;	// 文字

		private string				m_file_name;			// ファイル名
		
		/// <summary>
		/// API宣言
		/// * iniﾌｧｲﾙ読込み関数宣言
		/// </summary>
		[DllImport("kernel32.dll", EntryPoint="GetPrivateProfileString")]
		private static extern uint GetPrivateProfileString(
			  string lpApplicationName
			, string lpKeyName
			, string lpDefault
			, System.Text.StringBuilder StringBuilder
			, uint nSize
			, string lpFileName ); 

		/// <summary>
		/// API宣言
		/// * iniﾌｧｲﾙ書込み関数宣言
		/// </summary>
		[DllImport("kernel32.dll", EntryPoint="WritePrivateProfileString")] 
		private static extern uint WritePrivateProfileString(
			  string lpApplicationName
			, string lpEntryName
			, string lpEntryString
			, string lpFileName ); 

   
		/*-------------------------------------------------------------------------
		 fnameはフルパスであること
		 フルパスでない場合windowsディレクトリ内にiniが作られるので注意
		---------------------------------------------------------------------------*/
		public profile(string fname)
		{
			m_file_name		= fname;
		}

		/*-------------------------------------------------------------------------
		 取得
		 文字列
		---------------------------------------------------------------------------*/
		public string GetProfile(string section, string entry, string default_val)
		{
			StringBuilder	sb	= new StringBuilder(BUFF_LEN);
			uint			ret	= GetPrivateProfileString(	section,
															entry,
															default_val,
															sb, Convert.ToUInt32(sb.Capacity),
															m_file_name);
			return sb.ToString();
		}

		/*-------------------------------------------------------------------------
		 取得
		 int
		---------------------------------------------------------------------------*/
		public int GetProfile(string section, string entry, int default_val)
		{
			string	def		= default_val.ToString();
			string	ret		= GetProfile(section, entry, def);
			try{
				return Convert.ToInt32(ret);
			}catch{
				return 0;
			}
		}

		/*-------------------------------------------------------------------------
		 取得
		 float
		---------------------------------------------------------------------------*/
		public float GetProfile(string section, string entry, float default_val)
		{
			string	def		= default_val.ToString();
			string	ret		= GetProfile(section, entry, def);
			try{
				return (float)Convert.ToDouble(ret);
			}catch{
				return 0;
			}
		}

		/*-------------------------------------------------------------------------
		 取得
		 bool
		---------------------------------------------------------------------------*/
		public bool GetProfile(string section, string entry, bool default_val)
		{
			string	def		= (default_val)? "1": "0";
			string	ret		= GetProfile(section, entry, def);
			try{
				if(Convert.ToInt32(ret) == 0)	return false;
				else							return true;
			}catch{
				return false;
			}
		}

		/*-------------------------------------------------------------------------
		 設定
		 文字列
		---------------------------------------------------------------------------*/
		public bool WriteProfile(string section, string entry, string val)
		{
			uint	ret		= WritePrivateProfileString(section,
														entry,
														val,
														m_file_name);
			return (ret > 0)? true: false;
		}

		/*-------------------------------------------------------------------------
		 設定
		 int
		---------------------------------------------------------------------------*/
		public bool WriteProfile(string section, string entry, int val)
		{
			return WriteProfile(section, entry, val.ToString());
		}

		/*-------------------------------------------------------------------------
		 設定
		 float
		---------------------------------------------------------------------------*/
		public bool WriteProfile(string section, string entry, float val)
		{
			return WriteProfile(section, entry, val.ToString());
		}

		/*-------------------------------------------------------------------------
		 設定
		 bool
		---------------------------------------------------------------------------*/
		public bool WriteProfile(string section, string entry, bool val)
		{
			string	str		= (val)? "1": "0";
			return WriteProfile(section, entry, str);
		}
	}
}
