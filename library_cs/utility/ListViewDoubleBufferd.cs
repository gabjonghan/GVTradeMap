//-------------------------------------------------------------------------
// 더블 버퍼링된 ListView
// 깜빡임을 줄인다
//-------------------------------------------------------------------------
using System.Windows.Forms;

//-------------------------------------------------------------------------
namespace Utility.Ctrl
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// 더블 버퍼링된 ListView.
	/// 깜빡임을 줄인다.
	/// 덤으로 SubItem 별 ToolTip, 컬럼별로 아이템 문자열 정렬에 대응.
	/// </summary>
	/// <remarks>
	/// <para>SubItem 별 ToolTip 은 Utility.Useful.UpdateListViewSubItemToolTip() 을 사용한다.</para>
	/// <para>ToolTip 을 지정하지 않은 경우에는 SubItem 별 ToolTip 이 되지 않는다.</para>
	/// <para>아이템 정렬은 ListViewItemSorter 를 사용한다.</para>
	/// <para>초기값에서는 정렬하지 않는다.</para>
	/// <para>사용할 경우에는 EnableSort() 로 유효하게 할 것.</para>
	/// </remarks>
	public class ListViewDoubleBufferd : ListView
	{
		private ListViewItemSorter			m_sorter;		// 정렬

		/// <summary>
		/// 대상 ToolTip.
		/// 지정하지 않은 경우에는 SubItem 별로 ToolTip 을 설정하지 않으므로 주의
		/// </summary>
		public ToolTip						ToolTip{	get;	set;	}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축
		/// </summary>
		public ListViewDoubleBufferd()
		{
			try{
				// 더블 버퍼로 만든다
				this.DoubleBuffered	= true;
			}catch{
			}

			// ToolTip
			this.ToolTip		= null;
			this.MouseMove		+= new MouseEventHandler(mouse_move);

			// 정렬
			this.ColumnClick	+= new ColumnClickEventHandler(column_click);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 정렬을 유효하게 한다.
		/// </summary>
		/// <param name="is_normal">초기값을 오름차순 정렬로 할 경우 true</param>
		public void EnableSort(bool is_normal)
		{
			m_sorter	= new ListViewItemSorter(is_normal);
		}

   		//-------------------------------------------------------------------------
		/// <summary>
		/// 정렬을 무효로 한다.
		/// </summary>
		public void DisableSort()
		{
			m_sorter	= null;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 마우스가 움직였다.
		/// SubItem 별로 ToolTip 을 지정한다.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event Args</param>
		private void mouse_move(object sender, MouseEventArgs e)
		{
			// SubItem 별로 ToolTip 을 지정한다
			Useful.UpdateListViewSubItemToolTip(this, this.ToolTip, e.X, e.Y);
		}	

		//-------------------------------------------------------------------------
		/// <summary>
		/// 컬럼이 클릭되었다.
		/// 정렬한다.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void column_click(object sender, ColumnClickEventArgs e)
		{
			if(m_sorter == null)	return;
			m_sorter.Sort(this, e.Column);
		}
	}
}
