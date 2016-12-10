//-------------------------------------------------------------------------
// ListViewソート
// 単純に文字列でソートされる
// 数値に変換できる場合は数値でソートする
//-------------------------------------------------------------------------
using System;
using System.Windows.Forms;
using System.Collections;

//-------------------------------------------------------------------------
namespace Utility.Ctrl
{
	///-------------------------------------------------------------------------
	/// <summary>
	/// ListViewソート
	/// 単純に文字列でソートされる
	/// </summary>
	public class ListViewItemSorter
	{
		private int					m_sort_order;			// ソート方向
		private int					m_default_sort_oder;	// ソート方向の初期値
		private int					m_current_colum_index;	// 最後にソートした対象のコラム

		///-------------------------------------------------------------------------
		/// <summary>
		/// ソート順が昇順のときtrue
		/// </summary>
		public bool	IsSortOrderNormal		{	get{	return (m_sort_order > 0)? true: false;	}}

		///-------------------------------------------------------------------------
		/// <summary>
		/// 初期値のソート順が昇順のときtrue
		/// </summary>
		public bool IsDefaultSortOderNormal	{	get{	return (m_default_sort_oder > 0)? true: false;	}}

		///-------------------------------------------------------------------------
		/// <summary>
		/// 構築
		/// </summary>
		public ListViewItemSorter()
			: this(true)
		{
		}

		///-------------------------------------------------------------------------
		/// <summary>
		/// 初期値指定で構築
		/// </summary>
		/// <param name="is_default_sort_oder_normal">
		/// trueのとき昇順でソートされる
		/// SetDefaultSortOrder(true)と同じ
		/// </param>
		public ListViewItemSorter(bool is_default_sort_oder_normal)
		{
			m_current_colum_index	= -1;
			SetDefaultSortOrder(is_default_sort_oder_normal);
			ResetSortOrder();
		}
	
		///-------------------------------------------------------------------------
		/// <summary>
		/// ソートオーダーを설정する
		/// </summary>
		/// <param name="is_normal">trueのとき昇順でソートされる</param>
		public void SetSortOrder(bool is_normal)
		{
			m_sort_order	= 1;
			if(!is_normal)	m_sort_order	*= -1;
		}

		///-------------------------------------------------------------------------
		/// <summary>
		/// 初期値のソートオーダーを설정する
		/// </summary>
		/// <param name="is_normal">trueのとき昇順でソートされる</param>
		public void SetDefaultSortOrder(bool is_normal)
		{
			m_default_sort_oder	= 1;
			if(!is_normal)	m_default_sort_oder	*= -1;
		}

		///-------------------------------------------------------------------------
		/// <summary>
		/// ソートオーダーを初期化する
		/// </summary>
		public void ResetSortOrder()
		{
			// 初期値に설정する
			SetSortOrder(IsDefaultSortOderNormal);
		}

		///-------------------------------------------------------------------------
		/// <summary>
		/// ソートオーダーを反転させる
		/// </summary>
		public void FlipSortOrder()
		{
			m_sort_order	*= -1;
		}

		///-------------------------------------------------------------------------
		/// <summary>
		/// ソートする
		/// ソート後, ソートオーダーは反転される
		/// </summary>
		/// <param name="listview">ソート対象のListView</param>
		/// <param name="colum_index">ソート対象のカラム번호</param>
		/// <returns></returns>
		public bool Sort(ListView listview, int colum_index)
		{
			if(colum_index >= listview.Columns.Count){
				// ソート対象のコラムがlistviewに存在しない
				return false;
			}

			// 前회ソートしたコラムと違えば初期値に戻す
			if(m_current_colum_index != colum_index)	ResetSortOrder();
			m_current_colum_index = colum_index;

			listview.ListViewItemSorter	= new ListViewItemComparer(colum_index, m_sort_order);
			listview.Sort();
			FlipSortOrder();	// ソート方向を反転
			return true;
		}

	
		///-------------------------------------------------------------------------
		/// <summary>
		/// ソート時の比較
		/// </summary>
		private class ListViewItemComparer : IComparer
		{
			private int		col;
			private int		sortOrder;

			///-------------------------------------------------------------------------
			/// <summary>
			/// 構築
			/// </summary>
			/// <param name="col">ソート対象のカラム번호</param>
			/// <param name="sortOrder">ソート順</param>
			public ListViewItemComparer(int col, int sortOrder)
			{
				this.col		= col;
				this.sortOrder	= sortOrder;
			}

			///-------------------------------------------------------------------------
			/// <summary>
			/// 比較メソッド
			/// </summary>
			/// <param name="x">比較対象1</param>
			/// <param name="y">比較対象2</param>
			/// <returns>比較결과</returns>
			public int Compare(object x, object y)
			{
				ListViewItem	item1	= x as ListViewItem;
				ListViewItem	item2	= y as ListViewItem;

				if(item1 == null)					return 0;
				if(item2 == null)					return 0;
				if(col >= item1.SubItems.Count)		return 0;
				if(col >= item2.SubItems.Count)		return 0;

				string	cmp1	= item1.SubItems[col].Text;
				string	cmp2	= item2.SubItems[col].Text;

				// 数値に変換できるか調べる
				double val1, val2;
				if(!Double.TryParse(cmp1, out val1))	return cmp_string(cmp1, cmp2) * sortOrder;
				if(!Double.TryParse(cmp2, out val2))	return cmp_string(cmp1, cmp2) * sortOrder;

				if(val1 == val2)	return 0;	// doubleを==で比べるのはあれだがとりあえずこのまま
				if(val1 < val2)		return -1 * sortOrder;
				else				return 1 * sortOrder;
			}

			///-------------------------------------------------------------------------
			/// <summary>
			/// 文字列での比較
			/// </summary>
			/// <param name="cmp1"></param>
			/// <param name="cmp2"></param>
			/// <returns></returns>
			private int	cmp_string(string cmp1, string cmp2)
			{
				return String.Compare(cmp1, cmp2);
			}
		}
	}
}
