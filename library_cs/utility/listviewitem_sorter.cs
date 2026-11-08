//-------------------------------------------------------------------------
// ListView 정렬
// 단순히 문자열로 정렬된다
// 숫자로 변환할 수 있는 경우에는 숫자로 정렬한다
//-------------------------------------------------------------------------
using System;
using System.Windows.Forms;
using System.Collections;

//-------------------------------------------------------------------------
namespace Utility.Ctrl
{
	///-------------------------------------------------------------------------
	/// <summary>
	/// ListView 정렬
	/// 단순히 문자열로 정렬된다
	/// </summary>
	public class ListViewItemSorter
	{
		private int					m_sort_order;			// 정렬 방향
		private int					m_default_sort_oder;	// 정렬 방향의 초기값
		private int					m_current_colum_index;	// 마지막으로 정렬한 대상 컬럼

		///-------------------------------------------------------------------------
		/// <summary>
		/// 정렬 순서가 오름차순일 때 true
		/// </summary>
		public bool	IsSortOrderNormal		{	get{	return (m_sort_order > 0)? true: false;	}}

		///-------------------------------------------------------------------------
		/// <summary>
		/// 초기값의 정렬 순서가 오름차순일 때 true
		/// </summary>
		public bool IsDefaultSortOderNormal	{	get{	return (m_default_sort_oder > 0)? true: false;	}}

		///-------------------------------------------------------------------------
		/// <summary>
		/// 구축
		/// </summary>
		public ListViewItemSorter()
			: this(true)
		{
		}

		///-------------------------------------------------------------------------
		/// <summary>
		/// 초기값을 지정해서 구축
		/// </summary>
		/// <param name="is_default_sort_oder_normal">
		/// true 일 때 오름차순으로 정렬된다
		/// SetDefaultSortOrder(true) 와 같다
		/// </param>
		public ListViewItemSorter(bool is_default_sort_oder_normal)
		{
			m_current_colum_index	= -1;
			SetDefaultSortOrder(is_default_sort_oder_normal);
			ResetSortOrder();
		}
	
		///-------------------------------------------------------------------------
		/// <summary>
		/// 정렬 순서를 설정한다
		/// </summary>
		/// <param name="is_normal">true 일 때 오름차순으로 정렬된다</param>
		public void SetSortOrder(bool is_normal)
		{
			m_sort_order	= 1;
			if(!is_normal)	m_sort_order	*= -1;
		}

		///-------------------------------------------------------------------------
		/// <summary>
		/// 초기값의 정렬 순서를 설정한다
		/// </summary>
		/// <param name="is_normal">true 일 때 오름차순으로 정렬된다</param>
		public void SetDefaultSortOrder(bool is_normal)
		{
			m_default_sort_oder	= 1;
			if(!is_normal)	m_default_sort_oder	*= -1;
		}

		///-------------------------------------------------------------------------
		/// <summary>
		/// 정렬 순서를 초기화한다
		/// </summary>
		public void ResetSortOrder()
		{
			// 초기값으로 설정한다
			SetSortOrder(IsDefaultSortOderNormal);
		}

		///-------------------------------------------------------------------------
		/// <summary>
		/// 정렬 순서를 반전시킨다
		/// </summary>
		public void FlipSortOrder()
		{
			m_sort_order	*= -1;
		}

		///-------------------------------------------------------------------------
		/// <summary>
		/// 정렬한다
		/// 정렬 후 정렬 순서는 반전된다
		/// </summary>
		/// <param name="listview">정렬 대상 ListView</param>
		/// <param name="colum_index">정렬 대상 컬럼 번호</param>
		/// <returns></returns>
		public bool Sort(ListView listview, int colum_index)
		{
			if(colum_index >= listview.Columns.Count){
				// 정렬 대상 컬럼이 listview 에 존재하지 않는다
				return false;
			}

			// 지난번 정렬한 컬럼과 다르면 초기값으로 되돌린다
			if(m_current_colum_index != colum_index)	ResetSortOrder();
			m_current_colum_index = colum_index;

			listview.ListViewItemSorter	= new ListViewItemComparer(colum_index, m_sort_order);
			listview.Sort();
			FlipSortOrder();	// 정렬 방향을 반전
			return true;
		}

	
		///-------------------------------------------------------------------------
		/// <summary>
		/// 정렬 시의 비교
		/// </summary>
		private class ListViewItemComparer : IComparer
		{
			private int		col;
			private int		sortOrder;

			///-------------------------------------------------------------------------
			/// <summary>
			/// 구축
			/// </summary>
			/// <param name="col">정렬 대상 컬럼 번호</param>
			/// <param name="sortOrder">정렬 순서</param>
			public ListViewItemComparer(int col, int sortOrder)
			{
				this.col		= col;
				this.sortOrder	= sortOrder;
			}

			///-------------------------------------------------------------------------
			/// <summary>
			/// 비교 메서드
			/// </summary>
			/// <param name="x">비교 대상 1</param>
			/// <param name="y">비교 대상 2</param>
			/// <returns>비교 결과</returns>
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

				// 숫자로 변환할 수 있는지 조사한다
				double val1, val2;
				if(!Double.TryParse(cmp1, out val1))	return cmp_string(cmp1, cmp2) * sortOrder;
				if(!Double.TryParse(cmp2, out val2))	return cmp_string(cmp1, cmp2) * sortOrder;

				if(val1 == val2)	return 0;	// double 을 == 로 비교하는 건 좀 그렇지만 일단 이대로
				if(val1 < val2)		return -1 * sortOrder;
				else				return 1 * sortOrder;
			}

			///-------------------------------------------------------------------------
			/// <summary>
			/// 문자열로 비교
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
