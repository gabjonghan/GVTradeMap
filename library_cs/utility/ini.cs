//-------------------------------------------------------------------------
//
// Iniのインターフェイスと管理ベース
// IniBaseを実装したクラスならどれでも설정정보の読み書きが可能となる
//
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Utility.Ini
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// iniを사용するインターフェイス. 
	/// クラス毎に그룹명を変えて이름の衝突を防ぐ. 
	/// </summary>
	/// <remarks>
	/// <para>설정を保持するクラスがこのインターフェイスを実装し, </para>
	/// <para>IniSettingBaseに登録すれば설정の読み書きをまとめて行える. </para>
	/// </remarks>
	public interface IIniSaveLoad
	{
		/// <summary>
		/// 그룹명の初期値, 
		/// 그룹명を指定しなかったときに사용される. 
		/// 初期値を持つことで그룹명を用意する手間を省く. 
		/// </summary>
		string DefaultIniGroupName{	get;	}

		/// <summary>
		/// 読み込み
		/// </summary>
		/// <param name="ini">IIni</param>
		/// <param name="group">그룹명</param>
		void IniLoad(IIni ini, string group);

		/// <summary>
		/// 書き出し
		/// </summary>
		/// <param name="ini">IIni</param>
		/// <param name="group">그룹명</param>
		void IniSave(IIni ini, string group);
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// IniBaseを사용した설정管理のベース. 
	/// 複数のIIniSaveLoadを登録できる. 
	/// 初期値では그룹명の重複をチェックする. 
	/// 그룹명が重複してもいい場合はEnableDuplicateGroupNameをtrueにすること. 
	/// 継承したクラスのコンストラクタでファイル명を指定することを期待している. 
	/// </summary>
	public abstract class IniSettingBase
	{
		/// <summary>
		/// ユーザ목록
		/// </summary>
		private List<SaveLoadNode>			m_list;

		/// <summary>
		/// 그룹명の重複チェック, trueのとき重複を許可する. 
		/// 初期値はfalse
		/// </summary>
		protected bool						m_enable_duplicate_group_name;

		/// <summary>
		/// 그룹명の重複チェック, trueのとき重複を許可する. 
		/// 初期値はfalse
		/// </summary>
		public bool EnableDuplicateGroupName	{	get{	return m_enable_duplicate_group_name;		}
													set{	m_enable_duplicate_group_name	= value;	}}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 構築
		/// </summary>
		public IniSettingBase()
		{
			m_list							= new List<SaveLoadNode>();
			m_enable_duplicate_group_name	= false;	// 그룹명の重複チェック유효
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 追加
		/// </summary>
		/// <param name="user">IIniを사용するユーザ</param>
		/// <param name="group">userが사용する그룹명</param>
		public virtual void AddIniSaveLoad(IIniSaveLoad user, string group)
		{
			// 그룹명の重複チェック
			if(!m_enable_duplicate_group_name){
				if(is_duplicate_group_name(group)){
					throw new Exception(String.Format("[ {0} ]\r\n그룹명が重複しています. ", group));
				}
			}
			m_list.Add(new SaveLoadNode(user, group));
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 追加, 그룹명はuser.DefaultGroupNameを사용する
		/// </summary>
		/// <param name="user">IIniを사용するユーザ</param>
		public virtual void AddIniSaveLoad(IIniSaveLoad user)
		{
			AddIniSaveLoad(user, user.DefaultIniGroupName);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 그룹명が重複しているか調べる. 
		/// </summary>
		/// <param name="group">調べる그룹명</param>
		/// <returns>重複していればtrue</returns>
		private bool is_duplicate_group_name(string group)
		{
			foreach(SaveLoadNode i in m_list){
				if(i.Group == group)	return true;
			}
			return false;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 로드. 
		/// 継承先でIIniを作成し, Load(IIni ini)を呼び出すこと. 
		/// コンストラクタでファイル명등を指定すること. 
		/// </summary>
		public abstract void Load();

		//-------------------------------------------------------------------------
		/// <summary>
		/// IIniから로드
		/// </summary>
		/// <param name="ini"></param>
		protected virtual void Load(IIni ini)
		{
			if(ini == null)		throw new ArgumentException();
			foreach(SaveLoadNode i in m_list){
				i.Load(ini);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 書きだす. 
		/// 継承先でIIniを作成し, Save(IIni ini)を呼び出すこと. 
		/// コンストラクタでファイル명등を指定すること. 
		/// </summary>
		public abstract void Save();

		//-------------------------------------------------------------------------
		/// <summary>
		/// IIniに書きだす. 
		/// </summary>
		/// <param name="ini">IIni</param>
		protected virtual void Save(IIni ini)
		{
			if(ini == null)		throw new ArgumentException();
			foreach(SaveLoadNode i in m_list){
				i.Save(ini);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// IniBase管理ノード
		/// </summary>
		private class SaveLoadNode
		{
			private IIniSaveLoad				m_user;
			private string						m_group;

			/// <summary>
			/// 그룹명
			/// </summary>
			public string Group					{	get{	return m_group;		}}
	
			//-------------------------------------------------------------------------
			/// <summary>
			/// 構築
			/// </summary>
			/// <param name="user">ユーザ</param>
			/// <param name="group">ユーザが사용する그룹명</param>
			public SaveLoadNode(IIniSaveLoad user, string group)
			{
				if(user == null)					throw new ArgumentException();
				if(String.IsNullOrEmpty(group))		throw new ArgumentException();

				m_user		= user;
				m_group		= group;
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 読み込み
			/// </summary>
			/// <param name="ini">IniBase</param>
			public void Load(IIni ini)
			{
				if(ini == null)						throw new ArgumentException();
				m_user.IniLoad(ini, m_group);
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 書き出し
			/// </summary>
			/// <param name="ini">IIni</param>
			public void Save(IIni ini)
			{
				if(ini == null)						throw new ArgumentException();
				m_user.IniSave(ini, m_group);
			}
		}
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// iniアクセスベース, 
	/// 文字列설정と取得を実装して사용する. 
	/// </summary>
	/// <remarks>
	/// <para>iniと同じように사용できる. </para>
	/// <para>配列を扱うことができる. </para>
	/// <para>bool,int,long,double,float,stringに対応</para>
	/// <para>データの取得と설정のインターフェイスのみを実装している. </para>
	/// <para>ファイルへの読み書きは継承先が自由に行う. </para>
	/// <para>사용者はファイルへの読み書きがどう行われるか意識する必要はない. </para>
	/// <para>IniBaseに対してGetProfile()とSetProfile()を呼ぶだけでよい. </para>
	/// <para>ファイルへの読み書きを隠蔽するため, IniSettingBaseを継承して사용すること. </para>
	/// <para>IniBaseを継承したクラスのインスタンスを直接作成するべきではない. </para>
	/// <para>xmlを使ったクラスとしてXmlIniとXmlIniSettingが用意されている. </para>
	/// </remarks>
	public abstract class IniBase : IIni
	{
		//-------------------------------------------------------------------------
		/// <summary>
		/// データがあるかどうかを得る. 
		/// 継承先で実装すること
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <returns>データがある場合true</returns>
		protected abstract bool HasProfile(string group_name, string name);

		//-------------------------------------------------------------------------
		/// <summary>
		/// データ取得
		/// データが得られない場合はdefault_valueを返す
		/// 継承先で実装すること
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="default_value">データが得られない場合の初期値</param>
		/// <returns>得られたデータ</returns>
		public abstract string GetProfile(string group_name, string name, string default_value);
		
		//-------------------------------------------------------------------------
		/// <summary>
		/// データ取得
		/// データが得られない場合はdefault_valueを返す
		/// 継承先で実装すること
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="default_value">データが得られない場合の初期値</param>
		/// <returns>
		/// 得られたデータ
		/// 대きさ0の配列の可能性有り
		/// </returns>
		public abstract string[] GetProfile(string group_name, string name, string[] default_value);

		//-------------------------------------------------------------------------
		/// <summary>
		/// データ설정
		/// 継承先で実装すること
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
		public abstract void SetProfile(string group_name, string name, string value);

		//-------------------------------------------------------------------------
		/// <summary>
		/// データ설정
		/// 配列の내용は전부置きかえられる
		/// 継承先で実装すること
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
		public abstract void SetProfile(string group_name, string name, string[] value);
	

		//-------------------------------------------------------------------------
		/// <summary>
		/// boolデータ取得
		/// データが得られない場合はdefault_valueを返す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="default_value">データが得られない場合の初期値</param>
		/// <returns>得られたデータ</returns>
		public bool GetProfile(string group_name, string name, bool default_value)
		{
			if(!HasProfile(group_name, name))	return default_value;
			string	value	= GetProfile(group_name, name, "");
			try{
				return to_bool(value);
			}catch{
				return default_value;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// intデータ取得
		/// データが得られない場合はdefault_valueを返す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="default_value">データが得られない場合の初期値</param>
		/// <returns>得られたデータ</returns>
		public int GetProfile(string group_name, string name, int default_value)
		{
			if(!HasProfile(group_name, name))	return default_value;
			string	value	= GetProfile(group_name, name, "");
			try{
				return to_int(value);
			}catch{
				return default_value;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// longデータ取得
		/// データが得られない場合はdefault_valueを返す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="default_value">データが得られない場合の初期値</param>
		/// <returns>得られたデータ</returns>
		public long GetProfile(string group_name, string name, long default_value)
		{
			if(!HasProfile(group_name, name))	return default_value;
			string	value	= GetProfile(group_name, name, "");
			try{
				return to_long(value);
			}catch{
				return default_value;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// doubleデータ取得
		/// データが得られない場合はdefault_valueを返す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="default_value">データが得られない場合の初期値</param>
		/// <returns>得られたデータ</returns>
		public double GetProfile(string group_name, string name, double default_value)
		{
			if(!HasProfile(group_name, name))	return default_value;
			string	value	= GetProfile(group_name, name, "");
			try{
				return to_double(value);
			}catch{
				return default_value;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// floatデータ取得
		/// データが得られない場合はdefault_valueを返す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="default_value">データが得られない場合の初期値</param>
		/// <returns>得られたデータ</returns>
		public float GetProfile(string group_name, string name, float default_value)
		{
			if(!HasProfile(group_name, name))	return default_value;
			string	value	= GetProfile(group_name, name, "");
			try{
				return to_float(value);
			}catch{
				return default_value;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// bool설정を更新
		/// 설정항목がない場合Elementが作られる
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
		public void SetProfile(string group_name, string name, bool value)
		{
			SetProfile(group_name, name, to_string(value));
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// int설정を更新
		/// 설정항목がない場合Elementが作られる
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
		public void SetProfile(string group_name, string name, int value)
		{
			SetProfile(group_name, name, to_string(value));
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// long설정を更新
		/// 설정항목がない場合Elementが作られる
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
		public void SetProfile(string group_name, string name, long value)
		{
			SetProfile(group_name, name, to_string(value));
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// double설정を更新
		/// 설정항목がない場合Elementが作られる
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
		public void SetProfile(string group_name, string name, double value)
		{
			SetProfile(group_name, name, to_string(value));
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// float설정を更新
		/// 설정항목がない場合Elementが作られる
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
		public void SetProfile(string group_name, string name, float value)
		{
			SetProfile(group_name, name, to_string(value));
		}


		//-------------------------------------------------------------------------
		/// <summary>
		/// bool[]データ取得
		/// データが得られない場合はdefault_valueを返す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="default_value">データが得られない場合の初期値</param>
		/// <returns>
		/// 得られたデータ
		/// 대きさ0の配列の可能性有り
		/// </returns>
		public bool[] GetProfile(string group_name, string name, bool[] default_value)
		{
			if(!HasProfile(group_name, name))	return default_value;

			string[]	value	= GetProfile(group_name, name, new string[]{});
			try{
				List<bool>	list	= new List<bool>();
				foreach(string s in value){
					try{
						bool	b	= to_bool(s);
						list.Add(b);
					}catch{
						// 실패した要素は無視
					}
				}
				return list.ToArray();
			}catch{
				return default_value;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// int[]データ取得
		/// データが得られない場合はdefault_valueを返す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="default_value">データが得られない場合の初期値</param>
		/// <returns>
		/// 得られたデータ
		/// 대きさ0の配列の可能性有り
		/// </returns>
		public int[] GetProfile(string group_name, string name, int[] default_value)
		{
			if(!HasProfile(group_name, name))	return default_value;

			string[]	value	= GetProfile(group_name, name, new string[]{});
			try{
				List<int>	list	= new List<int>();
				foreach(string s in value){
					try{
						int	b	= to_int(s);
						list.Add(b);
					}catch{
						// 실패した要素は無視
					}
				}
				return list.ToArray();
			}catch{
				return default_value;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// long[]データ取得
		/// データが得られない場合はdefault_valueを返す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="default_value">データが得られない場合の初期値</param>
		/// <returns>
		/// 得られたデータ
		/// 대きさ0の配列の可能性有り
		/// </returns>
		public long[] GetProfile(string group_name, string name, long[] default_value)
		{
			if(!HasProfile(group_name, name))	return default_value;

			string[]	value	= GetProfile(group_name, name, new string[]{});
			try{
				List<long>	list	= new List<long>();
				foreach(string s in value){
					try{
						long	b	= to_long(s);
						list.Add(b);
					}catch{
						// 실패した要素は無視
					}
				}
				return list.ToArray();
			}catch{
				return default_value;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// double[]データ取得
		/// データが得られない場合はdefault_valueを返す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="default_value">データが得られない場合の初期値</param>
		/// <returns>
		/// 得られたデータ
		/// 대きさ0の配列の可能性有り
		/// </returns>
		public double[] GetProfile(string group_name, string name, double[] default_value)
		{
			if(!HasProfile(group_name, name))	return default_value;

			string[]	value	= GetProfile(group_name, name, new string[]{});
			try{
				List<double>	list	= new List<double>();
				foreach(string s in value){
					try{
						double	b	= to_double(s);
						list.Add(b);
					}catch{
						// 실패した要素は無視
					}
				}
				return list.ToArray();
			}catch{
				return default_value;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// float[]データ取得
		/// データが得られない場合はdefault_valueを返す
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="default_value">データが得られない場合の初期値</param>
		/// <returns>
		/// 得られたデータ
		/// 대きさ0の配列の可能性有り
		/// </returns>
		public float[] GetProfile(string group_name, string name, float[] default_value)
		{
			if(!HasProfile(group_name, name))	return default_value;

			string[]	value	= GetProfile(group_name, name, new string[]{});
			try{
				List<float>	list	= new List<float>();
				foreach(string s in value){
					try{
						float	b	= to_float(s);
						list.Add(b);
					}catch{
						// 실패した要素は無視
					}
				}
				return list.ToArray();
			}catch{
				return default_value;
			}
		}


		//-------------------------------------------------------------------------
		/// <summary>
		/// bool[]설정を更新
		/// 설정항목がない場合Elementが作られる
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
		public void SetProfile(string group_name, string name, bool[] value)
		{
			List<string>	list	= new List<string>();
			foreach(bool v in value){
				list.Add(to_string(v));
			}
			SetProfile(group_name, name, list.ToArray());
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// int[]설정を更新
		/// 설정항목がない場合Elementが作られる
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
		public void SetProfile(string group_name, string name, int[] value)
		{
			List<string>	list	= new List<string>();
			foreach(int v in value){
				list.Add(to_string(v));
			}
			SetProfile(group_name, name, list.ToArray());
		}
		
		//-------------------------------------------------------------------------
		/// <summary>
		/// long[]설정を更新
		/// 설정항목がない場合Elementが作られる
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
		public void SetProfile(string group_name, string name, long[] value)
		{
			List<string>	list	= new List<string>();
			foreach(long v in value){
				list.Add(to_string(v));
			}
			SetProfile(group_name, name, list.ToArray());
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// double[]설정を更新
		/// 설정항목がない場合Elementが作られる
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
		public void SetProfile(string group_name, string name, double[] value)
		{
			List<string>	list	= new List<string>();
			foreach(double v in value){
				list.Add(to_string(v));
			}
			SetProfile(group_name, name, list.ToArray());
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// float[]설정を更新
		/// 설정항목がない場合Elementが作られる
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">データ명</param>
		/// <param name="value">データ</param>
		public void SetProfile(string group_name, string name, float[] value)
		{
			List<string>	list	= new List<string>();
			foreach(float v in value){
				list.Add(to_string(v));
			}
			SetProfile(group_name, name, list.ToArray());
		}

		//-------------------------------------------------------------------------
		private string to_string(bool value)
		{
			if(value)	return "1";
			return "0";
		}
		private string to_string(int value){	return value.ToString();	}
		private string to_string(long value){	return value.ToString();	}
		private string to_string(double value){	return value.ToString();	}
		private string to_string(float value){	return value.ToString();	}

		//-------------------------------------------------------------------------
		private bool to_bool(string str)
		{
			if(String.IsNullOrEmpty(str))	throw new Exception();
			if(str == "0")					return false;
			return true;
		}
		private int to_int(string str)
		{
			if(String.IsNullOrEmpty(str))	throw new Exception();
			return Convert.ToInt32(str);
		}
		private long to_long(string str)
		{
			if(String.IsNullOrEmpty(str))	throw new Exception();
			return Convert.ToInt64(str);
		}
		private double to_double(string str)
		{
			if(String.IsNullOrEmpty(str))	throw new Exception();
			return Convert.ToDouble(str);
		}
		private float to_float(string str)
		{
			if(String.IsNullOrEmpty(str))	throw new Exception();
			return (float)Convert.ToDouble(str);
		}
	}
}
