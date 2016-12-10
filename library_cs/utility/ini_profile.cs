//-------------------------------------------------------------------------
//
// GetPrivateProfileString()を사용したiniファイルアクセス
//
//-------------------------------------------------------------------------
using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Utility.Ini
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// IniProfileを使った설정管理
	/// </summary>
	public class IniProfileSetting : IniSettingBase
	{
		private string				m_file_name;

		//-------------------------------------------------------------------------
		/// <summary>
		/// 構築
		/// </summary>
		/// <param name="file_name">설정ファイル명(フルパスであること)</param>
		public IniProfileSetting(string file_name)
		{
			if(String.IsNullOrEmpty(file_name))	throw new ArgumentException();
			m_file_name		= file_name;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 読み込み
		/// </summary>
		public override void  Load()
		{
		 	IniProfile	ini	= new IniProfile(m_file_name);
			Load(ini);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 書き出し
		/// </summary>
		public override void Save()
		{
			IniProfile	ini	= new IniProfile(m_file_name);
			Save(ini);
		}
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// GetPrivateProfileString()を사용したiniファイルアクセス
	/// </summary>
	public class IniProfile : IniBase
	{
		/// <summary>
		/// API宣言
		/// iniファイル読込み
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
		/// iniファイル書込み
		/// </summary>
		[DllImport("kernel32.dll", EntryPoint="WritePrivateProfileString")] 
		private static extern uint WritePrivateProfileString(
			  string lpApplicationName
			, string lpEntryName
			, string lpEntryString
			, string lpFileName ); 
	

		private const int			BUFF_LEN		= 256;	// 文字数
		private string				m_file_name;

		//-------------------------------------------------------------------------
		/// <summary>
		/// 構築
		/// </summary>
		/// <param name="file_name"></param>
		public IniProfile(string file_name)
		{
			m_file_name		= file_name;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// データがあるかどうかを得る. 
		/// とりあえず無条건でtrueを返す. 
		/// </summary>
		/// <param name="group_name"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		protected override bool HasProfile(string group_name, string name)
		{
			return true;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// stringデータ取得
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">이름</param>
		/// <param name="default_value">初期値</param>
		/// <returns>取得されたデータ</returns>
		public override string GetProfile(string group_name, string name, string default_value)
		{
			StringBuilder	sb	= new StringBuilder(BUFF_LEN);
			uint			ret	= GetPrivateProfileString(	group_name,
															name,
															default_value,
															sb, Convert.ToUInt32(sb.Capacity),
															m_file_name);
			return sb.ToString();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// string[]データ取得
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">이름</param>
		/// <param name="default_value">初期値</param>
		/// <returns>取得されたデータ</returns>
		public override string[] GetProfile(string group_name, string name, string[] default_value)
		{
			throw new NotImplementedException();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// stringデータ설정
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">이름</param>
		/// <param name="value">データ</param>
		public override void SetProfile(string group_name, string name, string value)
		{
			uint	ret		= WritePrivateProfileString(group_name,
														name,
														value,
														m_file_name);
			if(ret == 0){
				throw new Exception("データの書き込みに실패");
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// string[]データ설정
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">이름</param>
		/// <param name="value">データ</param>
		public override void SetProfile(string group_name, string name, string[] value)
		{
			throw new NotImplementedException();
		}
	}
}
