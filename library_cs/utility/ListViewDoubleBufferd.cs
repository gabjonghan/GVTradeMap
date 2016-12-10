//-------------------------------------------------------------------------
// ダブルバッファ化されたListView
// ちらつきを軽減する
//-------------------------------------------------------------------------
using System.Windows.Forms;

//-------------------------------------------------------------------------
namespace Utility.Ctrl
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// ダブルバッファ化されたListView. 
	/// ちらつきを軽減する. 
	/// ついでにSubItem毎のToolTip, カラム毎に아이템の文字列でのソートに対応. 
	/// </summary>
	/// <remarks>
	/// <para>SubItem毎のToolTipはUtility.Useful.UpdateListViewSubItemToolTip()を사용する. </para>
	/// <para>ToolTipを指定していない場合はSubItem毎のToolTipにならない. </para>
	/// <para>아이템のソートはListViewItemSorterを사용する. </para>
	/// <para>初期値ではソートしない. </para>
	/// <para>사용する場合はEnableSort()で유효にすること. </para>
	/// </remarks>
	public class ListViewDoubleBufferd : ListView
	{
		private ListViewItemSorter			m_sorter;		// ソート

		/// <summary>
		/// 対象のToolTip. 
		/// 指定していない場合はSubItem毎にToolTipを설정しないので注意
		/// </summary>
		public ToolTip						ToolTip{	get;	set;	}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 構築
		/// </summary>
		public ListViewDoubleBufferd()
		{
			try{
				// ダブルバッファにする
				this.DoubleBuffered	= true;
			}catch{
			}

			// ToolTip
			this.ToolTip		= null;
			this.MouseMove		+= new MouseEventHandler(mouse_move);

			// ソート
			this.ColumnClick	+= new ColumnClickEventHandler(column_click);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// ソートを유효にする. 
		/// </summary>
		/// <param name="is_normal">初期値を昇順ソートにする場合true</param>
		public void EnableSort(bool is_normal)
		{
			m_sorter	= new ListViewItemSorter(is_normal);
		}

   		//-------------------------------------------------------------------------
		/// <summary>
		/// ソートを무효にする. 
		/// </summary>
		public void DisableSort()
		{
			m_sorter	= null;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// マウスが動かされた. 
		/// SubItem毎にToolTipを指定する. 
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event Args</param>
		private void mouse_move(object sender, MouseEventArgs e)
		{
			// SubItem毎にToolTipを指定する
			Useful.UpdateListViewSubItemToolTip(this, this.ToolTip, e.X, e.Y);
		}	

		//-------------------------------------------------------------------------
		/// <summary>
		/// コラムがクリックされた. 
		/// ソートする. 
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
