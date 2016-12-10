//-------------------------------------------------------------------------
// ハッシュで管理されたDB
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections;

//-------------------------------------------------------------------------
namespace Utility
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// キーを返す要素
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public interface IDictionaryNode<TKey>
	{
		/// <summary>
		/// キー
		/// </summary>
		TKey Key{	get;	}
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// TKeyが重複可能なハッシュテーブル
	/// </summary>
	/// <typeparam name="TKey">キー</typeparam>
	/// <typeparam name="TValue">要素</typeparam>
	public class MultiDictionary<TKey, TValue> : IEnumerable<TValue>
		where TValue : IDictionaryNode<TKey>
	{
		/// <summary>
		/// ハッシュテーブル
		/// </summary>
		protected Dictionary<TKey, List<TValue>>		m_database;

		//-------------------------------------------------------------------------
		/// <summary>
		/// 構築
		/// </summary>
		public MultiDictionary()
		{
			m_database	= new Dictionary<TKey,List<TValue>>();
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 追加
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
		/// 最初に見つかったtを삭제する. 
		/// t.Keyに重複する要素がある場合は, 重複数に応じて時間がかかる. 
		/// O(重複数)
		/// </summary>
		/// <param name="t">要素</param>
		public void Remove(TValue t)
		{
			List<TValue>	list	= null;
			if(m_database.TryGetValue(t.Key, out list)){
				list.Remove(t);
				if(list.Count <= 0){
					// 要素が全てなくなったらハッシュテーブルからも삭제する
					Remove(t.Key);
				}
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 삭제. 
		/// 指定されたキーの要素を全て삭제する. 
		/// O(1)に近い速度で実行できる. 
		/// </summary>
		/// <param name="key">キー</param>
		public void Remove(TKey key)
		{
			m_database.Remove(key);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 要素の取得. 
		/// 最初に見つかった要素を返す. 
		/// 要素が無ければnullを返す. 
		/// </summary>
		/// <param name="key">キー</param>
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
		/// 指定されたキーを持つ要素を全て返す. 
		/// 要素が無ければnullを返す. 
		/// </summary>
		/// <param name="key">キー</param>
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
		/// 列挙
		/// 列挙される順序は不定
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
		/// 列挙
		/// 列挙される順序は不定
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
	}

	//-------------------------------------------------------------------------
	/// <summary>
	/// シーケンシャルにアクセスできるハッシュテーブル. 
	/// 添字でのアクセスが可能. 
	/// TKeyは重複できない. 
	/// </summary>
	/// <typeparam name="TKey">キー</typeparam>
	/// <typeparam name="TValue">要素</typeparam>
	public class SequentialDictionary<TKey, TValue> : IEnumerable<TValue>
		where TValue : IDictionaryNode<TKey>
	{
		/// <summary>
		/// 목록
		/// </summary>
		protected List<TValue>					m_sequential_database;
		/// <summary>
		/// ハッシュ管理
		/// </summary>
		protected Dictionary<TKey, TValue>		m_database;

		//-------------------------------------------------------------------------
		/// <summary>
		/// 構築
		/// </summary>
		public SequentialDictionary()
		{
			m_sequential_database		= new List<TValue>();
			m_database					= new Dictionary<TKey, TValue>();
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 追加. 
		/// 追加された順番でアクセスできる
		/// t.Keyが存在する場合例外を投げる
		/// </summary>
		/// <param name="t">要素</param>
		public void Add(TValue t)
		{
			try{
				m_database.Add(t.Key, t);
			}catch(Exception e){
				// すでにキーが存在する
				// 例外をそのまま投げる
				throw e;
			}
			// ハッシュテーブルに登録できたら追加
			m_sequential_database.Add(t);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 삭제. 
		/// 最初に見つかったtを삭제する
		/// </summary>
		/// <param name="t">キー</param>
		public void Remove(TValue t)
		{
			m_sequential_database.Remove(t);
			m_database.Remove(t.Key);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 삭제. 
		/// 指定されたキーの要素を삭제する
		/// </summary>
		/// <param name="key">キー</param>
		public void Remove(TKey key)
		{
			TValue	d	= GetValue(key);
			if(d == null)	return;			// キーの要素がない

			Remove(d);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 要素の取得. 
		/// 最初に見つかった要素を返す. 
		/// 要素が無ければnullを返す. 
		/// </summary>
		/// <param name="key">キー</param>
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
		/// 列挙
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
		/// 列挙
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
	}
}
