/*-------------------------------------------------------------------------

 ListViewソート
 単純に文字列でソートされる

---------------------------------------------------------------------------*/
using System;
using System.Windows.Forms;
using System.Collections;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace useful
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class listviewitem_sorter
	{
		private int					m_sort_order;			// ソート方向

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public bool	is_sort_order_normal		{	get{	return (m_sort_order > 0)? true: false;	}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public listviewitem_sorter()
		{
			ResetSortOrder();
		}
	
		/*-------------------------------------------------------------------------
		 ソートオーダーを設定する
		 is_normal=trueのとき昇順でソートされる
		---------------------------------------------------------------------------*/
		public void SetSortOrder(bool is_normal)
		{
			m_sort_order	= 1;
			if(!is_normal)	m_sort_order	*= -1;
		}

		/*-------------------------------------------------------------------------
		 ソートオーダーを初期化する
		---------------------------------------------------------------------------*/
		public void ResetSortOrder()
		{
			SetSortOrder(true);
		}

		/*-------------------------------------------------------------------------
		 ソートオーダーを反転させる
		---------------------------------------------------------------------------*/
		public void FlipSortOrder()
		{
			m_sort_order	*= -1;
		}

		/*-------------------------------------------------------------------------
		 ソートする
		 ソートオーダーは反転される
		---------------------------------------------------------------------------*/
		public bool Sort(ListView listview, int colum_index)
		{
			if(colum_index >= listview.Columns.Count){
				// ソート対象のコラムがlistviewに存在しない
				return false;
			}
	
			listview.ListViewItemSorter	= new ListViewItemComparer(colum_index, m_sort_order);
			listview.Sort();
			FlipSortOrder();
			return true;
		}

	
		/*-------------------------------------------------------------------------
		 ソート
		---------------------------------------------------------------------------*/
		private class ListViewItemComparer : IComparer
		{
			private int		col;
			private int		sortOrder;

			// コンストラクタ
			public ListViewItemComparer(int col, int sortOrder)
			{
				this.col		= col;
				this.sortOrder	= sortOrder;
			}
			// 比較メソッド
			public int Compare(object x, object y)
			{
				ListViewItem	item1	= x as ListViewItem;
				ListViewItem	item2	= y as ListViewItem;

				if(item1 == null)					return 0;
				if(item2 == null)					return 0;
				if(col >= item1.SubItems.Count)		return 0;
				if(col >= item2.SubItems.Count)		return 0;
				return String.Compare(	item1.SubItems[col].Text,
										item2.SubItems[col].Text) * sortOrder;
			}
		}
	}
}
