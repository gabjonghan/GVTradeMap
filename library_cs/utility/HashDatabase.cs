//-------------------------------------------------------------------------
// ハッシュで관리されたDB
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections;

//-------------------------------------------------------------------------
namespace Utility
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// 키を返す要素
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
	/// TKeyが중複가능なハッシュ테이블
	/// </summary>
	/// <typeparam name="TKey">키</typeparam>
	/// <typeparam name="TValue">要素</typeparam>
	public class MultiDictionary<TKey, TValue> : IEnumerable<TValue>
		where TValue : IDictionaryNode<TKey>
	{
		/// <summary>
		/// ハッシュ테이블
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
		/// 최초に見つかったtを삭제する. 
		/// t.Keyに중複する要素がある場合は, 중複수に応じて시간がかかる. 
		/// O(중複수)
		/// </summary>
		/// <param name="t">要素</param>
		public void Remove(TValue t)
		{
			List<TValue>	list	= null;
			if(m_database.TryGetValue(t.Key, out list)){
				list.Remove(t);
				if(list.Count <= 0){
					// 要素が全てなくなったらハッシュ테이블からも삭제する
					Remove(t.Key);
				}
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 삭제. 
		/// 지정された키の要素を全て삭제する. 
		/// O(1)に近い速度で実行できる. 
		/// </summary>
		/// <param name="key">키</param>
		public void Remove(TKey key)
		{
			m_database.Remove(key);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 要素の取得. 
		/// 최초に見つかった要素を返す. 
		/// 要素が無ければnullを返す. 
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
		/// 要素の取得. 
		/// 지정された키を持つ要素を全て返す. 
		/// 要素が無ければnullを返す. 
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
		/// 全ての要素を삭제
		/// </summary>
		public void Clear()
		{
			m_database.Clear();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 열挙
		/// 열挙される順序は不定
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
		/// 열挙される順序は不定
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// シーケンシャルにアクセスできるハッシュ테이블. 
	/// 添字でのアクセスが가능. 
	/// TKeyは중複できない. 
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
		/// ハッシュ관리
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
		/// 추가された順番でアクセスできる
		/// t.Keyが存在する場合例외を投げる
		/// </summary>
		/// <param name="t">要素</param>
		public void Add(TValue t)
		{
			try{
				m_database.Add(t.Key, t);
			}catch(Exception e){
				// すでに키が存在する
				// 例외をそのまま投げる
				throw e;
			}
			// ハッシュ테이블に등록できたら추가
			m_sequential_database.Add(t);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 삭제. 
		/// 최초に見つかったtを삭제する
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
		/// 지정された키の要素を삭제する
		/// </summary>
		/// <param name="key">키</param>
		public void Remove(TKey key)
		{
			TValue	d	= GetValue(key);
			if(d == null)	return;			// 키の要素がない

			Remove(d);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 要素の取得. 
		/// 최초に見つかった要素を返す. 
		/// 要素が無ければnullを返す. 
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
		/// 全ての要素を삭제
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
