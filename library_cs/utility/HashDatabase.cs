//-------------------------------------------------------------------------
// 해시로 관리되는 DB
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections;

//-------------------------------------------------------------------------
namespace Utility
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// 키를 반환하는 요소
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public interface IDictionaryNode<TKey>
	{
		/// <summary>
		/// 키
		/// </summary>
		TKey Key{	get;	}
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// TKey 가 중복 가능한 해시 테이블
	/// </summary>
	/// <typeparam name="TKey">키</typeparam>
	/// <typeparam name="TValue">要素</typeparam>
	public class MultiDictionary<TKey, TValue> : IEnumerable<TValue>
		where TValue : IDictionaryNode<TKey>
	{
		/// <summary>
		/// 해시 테이블
		/// </summary>
		protected Dictionary<TKey, List<TValue>>		m_database;

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축
		/// </summary>
		public MultiDictionary()
		{
			m_database	= new Dictionary<TKey,List<TValue>>();
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 추가
		/// </summary>
		/// <param name="t">要素</param>
		public void Add(TValue t)
		{
			List<TValue>	list	= null;
			if(m_database.TryGetValue(t.Key, out list)){
				list.Add(t);
			}else{
				list	= new List<TValue>();
				list.Add(t);
				m_database.Add(t.Key, list);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 삭제. 
		/// 처음 발견된 t 를 삭제한다.
		/// t.Key 에 중복되는 요소가 있는 경우에는, 중복 개수에 따라 시간이 걸린다.
		/// O(중複수)
		/// </summary>
		/// <param name="t">要素</param>
		public void Remove(TValue t)
		{
			List<TValue>	list	= null;
			if(m_database.TryGetValue(t.Key, out list)){
				list.Remove(t);
				if(list.Count <= 0){
					// 요소가 전부 없어지면 해시 테이블에서도 삭제한다
					Remove(t.Key);
				}
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 삭제. 
		/// 지정된 키의 요소를 전부 삭제한다.
		/// O(1) 에 가까운 속도로 실행할 수 있다.
		/// </summary>
		/// <param name="key">키</param>
		public void Remove(TKey key)
		{
			m_database.Remove(key);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 요소 취득.
		/// 처음 발견된 요소를 반환한다.
		/// 요소가 없으면 null 을 반환한다.
		/// </summary>
		/// <param name="key">키</param>
		/// <returns>要素</returns>
		public TValue GetValue(TKey key)
		{
			List<TValue>	list	= null;
			if(m_database.TryGetValue(key, out list)){
				if(list.Count > 0)	return list[0];
			}
			return default(TValue);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 요소 취득.
		/// 지정된 키를 가진 요소를 전부 반환한다.
		/// 요소가 없으면 null 을 반환한다.
		/// </summary>
		/// <param name="key">키</param>
		/// <returns>要素목록</returns>
		public TValue[] GetValueList(TKey key)
		{
			List<TValue>	list	= null;
			if(m_database.TryGetValue(key, out list)){
				if(list.Count > 0)	return list.ToArray();
			}
			return null;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 모든 요소를 삭제
		/// </summary>
		public void Clear()
		{
			m_database.Clear();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 열挙
		/// 열거되는 순서는 정해져 있지 않다
		/// </summary>
		/// <returns></returns>
		public IEnumerator<TValue> GetEnumerator()
		{
			foreach(KeyValuePair<TKey, List<TValue>> e in m_database){
				foreach(TValue i in e.Value){
					yield return i;
				}
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 열挙
		/// 열거되는 순서는 정해져 있지 않다
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// 순차적으로 접근할 수 있는 해시 테이블.
	/// 첨자로 접근 가능.
	/// TKey 는 중복될 수 없다.
	/// </summary>
	/// <typeparam name="TKey">키</typeparam>
	/// <typeparam name="TValue">要素</typeparam>
	public class SequentialDictionary<TKey, TValue> : IEnumerable<TValue>
		where TValue : IDictionaryNode<TKey>
	{
		/// <summary>
		/// 목록
		/// </summary>
		protected List<TValue>					m_sequential_database;
		/// <summary>
		/// 해시 관리
		/// </summary>
		protected Dictionary<TKey, TValue>		m_database;

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축
		/// </summary>
		public SequentialDictionary()
		{
			m_sequential_database		= new List<TValue>();
			m_database					= new Dictionary<TKey, TValue>();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 추가. 
		/// 추가된 순서로 접근할 수 있다
		/// t.Key 가 이미 존재하면 예외를 던진다
		/// </summary>
		/// <param name="t">要素</param>
		public void Add(TValue t)
		{
			try{
				m_database.Add(t.Key, t);
			}catch(Exception e){
				// 이미 키가 존재한다
				// 예외를 그대로 던진다
				throw e;
			}
			// 해시 테이블에 등록되면 추가
			m_sequential_database.Add(t);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 삭제. 
		/// 처음 발견된 t 를 삭제한다
		/// </summary>
		/// <param name="t">키</param>
		public void Remove(TValue t)
		{
			m_sequential_database.Remove(t);
			m_database.Remove(t.Key);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 삭제. 
		/// 지정된 키의 요소를 삭제한다
		/// </summary>
		/// <param name="key">키</param>
		public void Remove(TKey key)
		{
			TValue	d	= GetValue(key);
			if(d == null)	return;			// 키의 요소가 없다

			Remove(d);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 요소 취득.
		/// 처음 발견된 요소를 반환한다.
		/// 요소가 없으면 null 을 반환한다.
		/// </summary>
		/// <param name="key">키</param>
		/// <returns>要素</returns>
		public TValue GetValue(TKey key)
		{
			TValue	d;
			if(m_database.TryGetValue(key, out d)){
				return d;
			}
			return default(TValue);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 모든 요소를 삭제
		/// </summary>
		public void Clear()
		{
			m_sequential_database.Clear();
			m_database.Clear();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 열挙
		/// </summary>
		/// <returns></returns>
		public IEnumerator<TValue> GetEnumerator()
		{
			foreach(TValue i in m_sequential_database){
				yield return i;
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 열挙
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
