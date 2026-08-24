//-------------------------------------------------------------------------
// 유니크한 문자열
// 빈 문자열은 추가되지 않는다
// 검색 이력 등에 사용한다
// 추가된 문자열은 선두에 추가된다
//-------------------------------------------------------------------------
using System.Collections.Generic;
using System.Collections;

//-------------------------------------------------------------------------
namespace Utility
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// 유니크한 문자열
	/// 빈 문자열은 추가되지 않는다
	/// 검색 이력 등에 사용한다
	/// 추가된 문자열은 선두에 추가된다
	/// </summary>
	public sealed class UniqueString : IEnumerable<string>
	{
		#region Fields
		private const int				MAX		= 20;
		private const int				MIN		= 1;

		private List<string>			m_strings;			// 중복되지 않는 문자열 목록
		private int						m_max;				// 보관할 최대 개수
		#endregion

		#region Properties
		//-------------------------------------------------------------------------
		/// <summary>
		/// 내용 취득
		/// </summary>
		/// <param name="i">인덱스</param>
		/// <returns>지정된 내용</returns>
		public string this[int i]		{		get{	return m_strings[i];	}}
		/// <summary>
		/// 보관 개수 취득
		/// </summary>
		public int Count				{		get{	return m_strings.Count;	}}
		/// <summary>
		/// 선두 내용 취득
		/// </summary>
		public string Top				{		get{	if(m_strings.Count <= 0)	return "";
														return m_strings[0];	}}
		/// <summary>
		/// 최대 보관 개수의 취득과 설정
		/// </summary>
		public int Max					{		get{	return m_max;			}
												set{	m_max	= value;
														if(m_max < MIN)	m_max	= MIN;
														ajust_count();
												}
										}
		#endregion

		#region Constructors
		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축
		/// </summary>
		public UniqueString()
		{
			m_strings		= new List<string>();
			m_max			= MAX;
		}
		#endregion

		//-------------------------------------------------------------------------
		/// <summary>
		/// 배열로 변환
		/// </summary>
		/// <returns>변환된 배열</returns>
		public string[] ToArray()
		{
			return m_strings.ToArray();
		}

		//-------------------------------------------------------------------------
		/// 열挙
		public IEnumerator<string> GetEnumerator()
		{
			for(int i=0; i<m_strings.Count; i++){
				yield return m_strings[i];
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 추가
		/// 추가되면 true 를 반환한다
		/// 선두에 추가된다
		/// </summary>
		/// <param name="str">추가할 문자열</param>
		/// <returns>추가되면 true</returns>
		public bool Add(string str)
		{
			if(string.IsNullOrEmpty(str))	return false;

			// 삭제한다
			Remove(str);
			// 선두에 추가
			m_strings.Insert(0, str);

			// 최대 개수에 맞도록 조정한다
			ajust_count();
			return true;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 배열로 설정
		/// 설정 파일에서 읽어들일 때 사용
		/// </summary>
		/// <param name="list">목록</param>
		public void SetRange(string[] list)
		{
			m_strings.Clear();
			foreach(string s in list){
				AddLast(s);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 추가
		/// 마지막에 추가된다
		/// 추가되면 true 를 반환한다
		/// 설정 파일에서 읽어들일 때 사용
		/// </summary>
		/// <param name="str">추가할 문자열</param>
		/// <returns>추가되면 true</returns>
		public bool AddLast(string str)
		{
			if(string.IsNullOrEmpty(str))	return false;

			// 삭제한다
			Remove(str);
			// 마지막에 추가
			m_strings.Add(str);

			// 최대 개수에 맞도록 조정한다
			ajust_count();
			return true;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 전부 삭제
		/// </summary>
		public void Clear()
		{
			m_strings.Clear();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 일치하는 문자열을 삭제한다
		/// 삭제한 경우에는 true 를 반환한다
		/// </summary>
		/// <param name="str">삭제할 문자열</param>
		/// <returns>삭제하면 true</returns>
		public bool Remove(string str)
		{
			if(has_string(str)){
				// 삭제한다
				m_strings.Remove(str);
				return true;
			}
			return false;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 複製
		/// </summary>
		/// <param name="list">複製元</param>
		public void Clone(UniqueString list)
		{
			Clear();
			Max		= list.Max;
			foreach(string str in list){
				m_strings.Add(str);
			}
		}

		#region Private Methods
		//-------------------------------------------------------------------------
		/// 포함되는지 여부를 얻는다
		private bool has_string(string str)
		{
			foreach(string s in m_strings){
				if(s == str)	return true;	// 포함되어 있다
			}
			return false;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 保持수조정
		/// </summary>
		private void ajust_count()
		{
			// 최대 개수에 맞도록 조정한다
			while(m_strings.Count > m_max){
				m_strings.RemoveAt(m_strings.Count -1);
			}
		}
		#endregion
	}
}
