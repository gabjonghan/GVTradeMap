/*-------------------------------------------------------------------------

 ヒットテスト

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Drawing;
using System.Collections;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace useful
{
	/*-------------------------------------------------------------------------
	 1つの矩形とのテスト
	 矩形1つの場合はそのまま使ってよい
	 複数の場合は hittest_list を使うと扱いが楽
	---------------------------------------------------------------------------*/
	public class hittest
	{
		private Rectangle				m_rect;				// 判定矩形
		private Point					m_position;			// オフセット
		private float					m_scale;			// スケール
		private bool					m_enable;			// 有効時true

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public Rectangle rect		{	get{	return m_rect;			}
										set{	m_rect	= value;		}}
		public Point position		{	get{	return m_position;		}
										set{	m_position	= value;	}}
		public float scale			{	get{	return m_scale;			}
										set{	m_scale		= value;	}}
		public bool enable			{	get{	return m_enable;		}
										set{	m_enable	= value;	}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public hittest()
		{
			m_rect			= new Rectangle(0, 0, 0, 0);
			m_position		= new Point(0, 0);
			m_scale			= 1;
			m_enable		= true;
		}
		public hittest(Rectangle rect)
		{
			m_rect			= rect;
			m_position		= new Point(0, 0);
			m_scale			= 1;
			m_enable		= true;
		}
		public hittest(Rectangle rect, Point position)
		{
			m_rect			= rect;
			m_position		= position;
			m_scale			= 1;
			m_enable		= true;
		}
		public hittest(Rectangle rect, Point position, float scale)
		{
			m_rect			= rect;
			m_position		= position;
			m_scale			= scale;
			m_enable		= true;
		}

		/*-------------------------------------------------------------------------
		 hittestとの比較
		---------------------------------------------------------------------------*/
		public bool HitTest(hittest hit)
		{
			Rectangle	rect		= hit.CalcRect();
			return HitTest(rect);
		}

		/*-------------------------------------------------------------------------
		 矩形との比較
		---------------------------------------------------------------------------*/
		public bool HitTest(Rectangle rect)
		{
			if(!enable)									return false;

			Rectangle	my_rect		= CalcRect();
			if(my_rect.X >= rect.X + rect.Width)		return false;
			if(my_rect.Y >= rect.Y + rect.Height)		return false;
			if(my_rect.X + my_rect.Width < rect.X)		return false;
			if(my_rect.Y + my_rect.Height < rect.Y)		return false;
			return true;
		}

		/*-------------------------------------------------------------------------
		 点との比較
		---------------------------------------------------------------------------*/
		public bool HitTest(Point pos)
		{
			if(!enable)									return false;

			Rectangle	my_rect		= CalcRect();
			if(pos.X < my_rect.X)						return false;
			if(pos.Y < my_rect.Y)						return false;
			if(pos.X >= my_rect.X + my_rect.Width)		return false;
			if(pos.Y >= my_rect.Y + my_rect.Height)		return false;
			return true;
		}

		/*-------------------------------------------------------------------------
		 hittestとの比較
		 合格するとコールバックが呼ばれる
		---------------------------------------------------------------------------*/
		public bool HitTest(hittest hit, int type)
		{
			if(HitTest(hit)){
				OnHit(hit, type);
				return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 矩形との比較
		 合格するとコールバックが呼ばれる
		---------------------------------------------------------------------------*/
		public bool HitTest(Rectangle rect, int type)
		{
			if(HitTest(rect)){
				OnHit(rect, type);
				return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 点との比較
		 合格するとコールバックが呼ばれる
		---------------------------------------------------------------------------*/
		public bool HitTest(Point pos, int type)
		{
			if(HitTest(pos)){
				OnHit(pos, type);
				return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 スケール等を考慮した矩形を得る
		 サイズ等は整数に丸められる
		---------------------------------------------------------------------------*/
		public Rectangle CalcRect()
		{
			float		x		= m_rect.X + (m_scale * m_position.X);
			float		y		= m_rect.Y + (m_scale * m_position.Y);
			float		size_x	= m_scale * m_rect.Width;
			float		size_y	= m_scale * m_rect.Height;
			return new Rectangle((int)x, (int)y, (int)size_x, (int)size_y);
		}

		/*-------------------------------------------------------------------------

		 継承先でのオーバーライド用
		 
		---------------------------------------------------------------------------*/

		/*-------------------------------------------------------------------------
		 ヒットテストに合格したときのコールバック
		 type はHitTest()に渡された値がそのまま渡される
		---------------------------------------------------------------------------*/
		virtual protected void OnHit(hittest hit, int type)
		{
		}
		virtual protected void OnHit(Rectangle rect, int type)
		{
		}
		virtual protected void OnHit(Point pos, int type)
		{
		}
	}

	/*-------------------------------------------------------------------------
	 複数の矩形とのテスト
	---------------------------------------------------------------------------*/
	public class hittest_list : IEnumerable<hittest>
	{
		private List<hittest>			m_list;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public int Count					{	get{	return m_list.Count;	}}
		public hittest this[int index]		{	get{	return m_list[index];	}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public hittest_list()
		{
			m_list		= new List<hittest>();
		}

		/*-------------------------------------------------------------------------
		 追加
		---------------------------------------------------------------------------*/
		public hittest Add(hittest _hittest)
		{
			m_list.Add(_hittest);
			return _hittest;
		}

		/*-------------------------------------------------------------------------
		 削除
		---------------------------------------------------------------------------*/
		public void Remove(hittest _hittest)
		{
			m_list.Remove(_hittest);
		}

		/*-------------------------------------------------------------------------
		 点との比較
		---------------------------------------------------------------------------*/
		public int HitTest_Index(Point pos)
		{
			int	index	= 0;
			foreach(hittest h in m_list){
				if(h.HitTest(pos))		return index;
				index++;
			}
			return -1;
		}
		public hittest HitTest(Point pos)
		{
			foreach(hittest h in m_list){
				if(h.HitTest(pos))		return h;
			}
			return null;
		}

		/*-------------------------------------------------------------------------
		 点との比較
		 合格するとコールバックが呼ばれる
		---------------------------------------------------------------------------*/
		public int HitTest_Index(Point pos, int type)
		{
			int	index	= 0;
			foreach(hittest h in m_list){
				if(h.HitTest(pos, type))	return index;
				index++;
			}
			return -1;
		}
		public hittest HitTest(Point pos, int type)
		{
			foreach(hittest h in m_list){
				if(h.HitTest(pos, type))	return h;
			}
			return null;
		}

		/*-------------------------------------------------------------------------
		 列挙
		---------------------------------------------------------------------------*/
		public IEnumerator<hittest> GetEnumerator()
		{
			foreach(hittest h in m_list){
				yield return h;
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/*-------------------------------------------------------------------------
		 全て削除
		---------------------------------------------------------------------------*/
		public void Clear()
		{
			m_list.Clear();
		}
	}
}
