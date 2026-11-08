//-------------------------------------------------------------------------
//
// Ini 의 인터페이스와 관리 베이스
// IniBase 를 구현한 클래스라면 어느 것이든 설정 정보 읽기가 가능해진다
//
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Utility.Ini
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// ini 를 사용하는 인터페이스.
	/// 클래스마다 그룹명을 바꿔 이름 충돌을 막는다.
	/// </summary>
	/// <remarks>
	/// <para>설정을 보관하는 클래스가 이 인터페이스를 구현하고,</para>
	/// <para>IniSettingBase 에 등록하면 설정 읽기를 한꺼번에 처리할 수 있다.</para>
	/// </remarks>
	public interface IIniSaveLoad
	{
		/// <summary>
		/// 그룹명의 초기값,
		/// 그룹명을 지정하지 않았을 때 사용된다.
		/// 초기값을 가짐으로써 그룹명을 준비하는 수고를 던다.
		/// </summary>
		string DefaultIniGroupName{	get;	}

		/// <summary>
		/// 읽기
		/// </summary>
		/// <param name="ini">IIni</param>
		/// <param name="group">그룹명</param>
		void IniLoad(IIni ini, string group);

		/// <summary>
		/// 기록
		/// </summary>
		/// <param name="ini">IIni</param>
		/// <param name="group">그룹명</param>
		void IniSave(IIni ini, string group);
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// IniBase 를 사용한 설정 관리의 베이스.
	/// 여러 개의 IIniSaveLoad 를 등록할 수 있다.
	/// 초기값에서는 그룹명의 중복을 확인한다.
	/// 그룹명이 중복돼도 되는 경우에는 EnableDuplicateGroupName 을 true 로 할 것.
	/// 상속한 클래스의 생성자에서 파일명을 지정하기를 기대한다.
	/// </summary>
	public abstract class IniSettingBase
	{
		/// <summary>
		/// 유저목록
		/// </summary>
		private List<SaveLoadNode>			m_list;

		/// <summary>
		/// 그룹명 중복 확인, true 일 때 중복을 허용한다.
		/// 초기값은 false
		/// </summary>
		protected bool						m_enable_duplicate_group_name;

		/// <summary>
		/// 그룹명 중복 확인, true 일 때 중복을 허용한다.
		/// 초기값은 false
		/// </summary>
		public bool EnableDuplicateGroupName	{	get{	return m_enable_duplicate_group_name;		}
													set{	m_enable_duplicate_group_name	= value;	}}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축
		/// </summary>
		public IniSettingBase()
		{
			m_list							= new List<SaveLoadNode>();
			m_enable_duplicate_group_name	= false;	// 그룹명 중복 확인 유효
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 추가
		/// </summary>
		/// <param name="user">IIni 를 사용하는 유저</param>
		/// <param name="group">user 가 사용하는 그룹명</param>
		public virtual void AddIniSaveLoad(IIniSaveLoad user, string group)
		{
			// 그룹명 중복 확인
			if(!m_enable_duplicate_group_name){
				if(is_duplicate_group_name(group)){
					throw new Exception(String.Format("[ {0} ]\r\n그룹명이 중복되었습니다. ", group));
				}
			}
			m_list.Add(new SaveLoadNode(user, group));
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 추가, 그룹명은 user.DefaultGroupName 을 사용한다
		/// </summary>
		/// <param name="user">IIni 를 사용하는 유저</param>
		public virtual void AddIniSaveLoad(IIniSaveLoad user)
		{
			AddIniSaveLoad(user, user.DefaultIniGroupName);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 그룹명이 중복되는지 조사한다.
		/// </summary>
		/// <param name="group">조사할 그룹명</param>
		/// <returns>중복되면 true</returns>
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
		/// 상속받은 쪽에서 IIni 를 작성하고, Load(IIni ini) 를 호출할 것.
		/// 생성자에서 파일명 등을 지정할 것.
		/// </summary>
		public abstract void Load();

		//-------------------------------------------------------------------------
		/// <summary>
		/// IIni 로부터 로드
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
		/// 기록한다.
		/// 상속받은 쪽에서 IIni 를 작성하고, Save(IIni ini) 를 호출할 것.
		/// 생성자에서 파일명 등을 지정할 것.
		/// </summary>
		public abstract void Save();

		//-------------------------------------------------------------------------
		/// <summary>
		/// IIni 에 기록한다.
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
		/// IniBase 관리 노드
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
			/// 구축
			/// </summary>
			/// <param name="user">유저</param>
			/// <param name="group">유저가 사용하는 그룹명</param>
			public SaveLoadNode(IIniSaveLoad user, string group)
			{
				if(user == null)					throw new ArgumentException();
				if(String.IsNullOrEmpty(group))		throw new ArgumentException();

				m_user		= user;
				m_group		= group;
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 읽기
			/// </summary>
			/// <param name="ini">IniBase</param>
			public void Load(IIni ini)
			{
				if(ini == null)						throw new ArgumentException();
				m_user.IniLoad(ini, m_group);
			}

			//-------------------------------------------------------------------------
			/// <summary>
			/// 저장
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
	/// ini 접근 베이스,
	/// 문자열 설정과 취득을 구현해서 사용한다.
	/// </summary>
	/// <remarks>
	/// <para>ini 와 같은 방식으로 사용할 수 있다.</para>
	/// <para>배열을 다룰 수 있다.</para>
	/// <para>bool, int, long, double, float, string 에 대응</para>
	/// <para>데이터 취득과 설정의 인터페이스만 구현되어 있다.</para>
	/// <para>파일 입출력은 상속받은 쪽이 자유롭게 수행한다.</para>
	/// <para>사용자는 파일 입출력이 어떻게 이루어지는지 의식할 필요가 없다.</para>
	/// <para>IniBase 에 대해 GetProfile() 과 SetProfile() 을 호출하기만 하면 된다.</para>
	/// <para>파일 입출력을 은폐하기 위해, IniSettingBase 를 상속해서 사용할 것.</para>
	/// <para>IniBase 를 상속한 클래스의 인스턴스를 직접 작성해서는 안 된다.</para>
	/// <para>xml 을 사용한 클래스로 XmlIni 와 XmlIniSetting 이 준비되어 있다.</para>
	/// </remarks>
	public abstract class IniBase : IIni
	{
		//-------------------------------------------------------------------------
		/// <summary>
		/// 데이터가 있는지 여부를 얻는다.
		/// 상속받은 쪽에서 구현할 것
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <returns>데이터가 있는 경우 true</returns>
		protected abstract bool HasProfile(string group_name, string name);

		//-------------------------------------------------------------------------
		/// <summary>
		/// 데이터 취득
		/// 데이터를 얻지 못한 경우에는 default_value 를 반환한다
		/// 상속받은 쪽에서 구현할 것
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터를 얻지 못한 경우의 초기값</param>
		/// <returns>얻어진 데이터</returns>
		public abstract string GetProfile(string group_name, string name, string default_value);
		
		//-------------------------------------------------------------------------
		/// <summary>
		/// 데이터 취득
		/// 데이터를 얻지 못한 경우에는 default_value 를 반환한다
		/// 상속받은 쪽에서 구현할 것
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터를 얻지 못한 경우의 초기값</param>
		/// <returns>
		/// 얻어진 데이터
		/// 크기 0 인 배열일 가능성 있음
		/// </returns>
		public abstract string[] GetProfile(string group_name, string name, string[] default_value);

		//-------------------------------------------------------------------------
		/// <summary>
		/// 데이터설정
		/// 상속받은 쪽에서 구현할 것
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
		public abstract void SetProfile(string group_name, string name, string value);

		//-------------------------------------------------------------------------
		/// <summary>
		/// 데이터설정
		/// 배열의 내용은 전부 대체된다
		/// 상속받은 쪽에서 구현할 것
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
		public abstract void SetProfile(string group_name, string name, string[] value);
	

		//-------------------------------------------------------------------------
		/// <summary>
		/// bool 데이터 취득
		/// 데이터를 얻지 못한 경우에는 default_value 를 반환한다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터를 얻지 못한 경우의 초기값</param>
		/// <returns>얻어진 데이터</returns>
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
		/// int 데이터 취득
		/// 데이터를 얻지 못한 경우에는 default_value 를 반환한다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터를 얻지 못한 경우의 초기값</param>
		/// <returns>얻어진 데이터</returns>
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
		/// long 데이터 취득
		/// 데이터를 얻지 못한 경우에는 default_value 를 반환한다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터를 얻지 못한 경우의 초기값</param>
		/// <returns>얻어진 데이터</returns>
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
		/// double 데이터 취득
		/// 데이터를 얻지 못한 경우에는 default_value 를 반환한다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터를 얻지 못한 경우의 초기값</param>
		/// <returns>얻어진 데이터</returns>
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
		/// float 데이터 취득
		/// 데이터를 얻지 못한 경우에는 default_value 를 반환한다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터를 얻지 못한 경우의 초기값</param>
		/// <returns>얻어진 데이터</returns>
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
		/// bool 설정을 갱신
		/// 설정 항목이 없는 경우 Element 가 만들어진다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
		public void SetProfile(string group_name, string name, bool value)
		{
			SetProfile(group_name, name, to_string(value));
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// int 설정을 갱신
		/// 설정 항목이 없는 경우 Element 가 만들어진다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
		public void SetProfile(string group_name, string name, int value)
		{
			SetProfile(group_name, name, to_string(value));
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// long 설정을 갱신
		/// 설정 항목이 없는 경우 Element 가 만들어진다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
		public void SetProfile(string group_name, string name, long value)
		{
			SetProfile(group_name, name, to_string(value));
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// double 설정을 갱신
		/// 설정 항목이 없는 경우 Element 가 만들어진다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
		public void SetProfile(string group_name, string name, double value)
		{
			SetProfile(group_name, name, to_string(value));
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// float 설정을 갱신
		/// 설정 항목이 없는 경우 Element 가 만들어진다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
		public void SetProfile(string group_name, string name, float value)
		{
			SetProfile(group_name, name, to_string(value));
		}


		//-------------------------------------------------------------------------
		/// <summary>
		/// bool[] 데이터 취득
		/// 데이터를 얻지 못한 경우에는 default_value 를 반환한다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터를 얻지 못한 경우의 초기값</param>
		/// <returns>
		/// 얻어진 데이터
		/// 크기 0 인 배열일 가능성 있음
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
						// 실패한 요소는 무시
					}
				}
				return list.ToArray();
			}catch{
				return default_value;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// int[] 데이터 취득
		/// 데이터를 얻지 못한 경우에는 default_value 를 반환한다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터를 얻지 못한 경우의 초기값</param>
		/// <returns>
		/// 얻어진 데이터
		/// 크기 0 인 배열일 가능성 있음
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
						// 실패한 요소는 무시
					}
				}
				return list.ToArray();
			}catch{
				return default_value;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// long[] 데이터 취득
		/// 데이터를 얻지 못한 경우에는 default_value 를 반환한다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터를 얻지 못한 경우의 초기값</param>
		/// <returns>
		/// 얻어진 데이터
		/// 크기 0 인 배열일 가능성 있음
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
						// 실패한 요소는 무시
					}
				}
				return list.ToArray();
			}catch{
				return default_value;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// double[] 데이터 취득
		/// 데이터를 얻지 못한 경우에는 default_value 를 반환한다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터를 얻지 못한 경우의 초기값</param>
		/// <returns>
		/// 얻어진 데이터
		/// 크기 0 인 배열일 가능성 있음
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
						// 실패한 요소는 무시
					}
				}
				return list.ToArray();
			}catch{
				return default_value;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// float[] 데이터 취득
		/// 데이터를 얻지 못한 경우에는 default_value 를 반환한다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="default_value">데이터를 얻지 못한 경우의 초기값</param>
		/// <returns>
		/// 얻어진 데이터
		/// 크기 0 인 배열일 가능성 있음
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
						// 실패한 요소는 무시
					}
				}
				return list.ToArray();
			}catch{
				return default_value;
			}
		}


		//-------------------------------------------------------------------------
		/// <summary>
		/// bool[] 설정을 갱신
		/// 설정 항목이 없는 경우 Element 가 만들어진다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
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
		/// int[] 설정을 갱신
		/// 설정 항목이 없는 경우 Element 가 만들어진다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
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
		/// long[] 설정을 갱신
		/// 설정 항목이 없는 경우 Element 가 만들어진다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
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
		/// double[] 설정을 갱신
		/// 설정 항목이 없는 경우 Element 가 만들어진다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
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
		/// float[] 설정을 갱신
		/// 설정 항목이 없는 경우 Element 가 만들어진다
		/// </summary>
		/// <param name="group_name">그룹명</param>
		/// <param name="name">데이터명</param>
		/// <param name="value">데이터</param>
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
