/*-------------------------------------------------------------------------

 바운딩 박스
 2D용

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using Microsoft.DirectX;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace directx
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class D3dBB2d
	{
		private Vector2				m_min;
		private Vector2				m_max;
		private bool				m_is_1st;		// 최초の위치を설정したらtrue
		private Vector2				m_offset_lt;	// 左上のオフセット
		private Vector2				m_offset_rb;	// 右下のオフセット
	
		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public Vector2 Min			{	get{	return m_min;			}}
		public Vector2 Max			{	get{	return m_max;			}}
		public Vector2 Size			{	get{	return m_max - m_min;	}}
		public Vector2 OffsetLT		{	get{	return m_offset_lt;		}
										set{	m_offset_lt	= value;	}}
		public Vector2 OffsetRB		{	get{	return m_offset_rb;		}
										set{	m_offset_rb	= value;	}}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public D3dBB2d()
		{
			m_min			= new Vector2(0, 0);
			m_max			= new Vector2(0, 0);
			m_offset_lt		= new Vector2(0, 0);
			m_offset_rb		= new Vector2(0, 0);
			m_is_1st		= false;
		}

		/*-------------------------------------------------------------------------
		 更新
		---------------------------------------------------------------------------*/
		public void Update(Vector2 pos)
		{
			if(!m_is_1st){
				// 初めてなので単純に代入
				m_min		= pos;
				m_max		= pos;
				m_is_1st	= true;
			}else{
				// 更新
				if(pos.X < m_min.X)		m_min.X	= pos.X;
				if(pos.X > m_max.X)		m_max.X	= pos.X;
				if(pos.Y < m_min.Y)		m_min.Y	= pos.Y;
				if(pos.Y > m_max.Y)		m_max.Y	= pos.Y;
			}
		}

		/*-------------------------------------------------------------------------
		 更新した결과を新しい D3dBB2d で返す
		---------------------------------------------------------------------------*/
		public D3dBB2d IfUpdate(Vector2 pos)
		{
			D3dBB2d	bb	= new D3dBB2d();
			if(m_is_1st){
				// 初期化されていれば최대と最소を설정する
				bb.Update(Min);
				bb.Update(Max);
			}
			bb.Update(pos);		// 更新してみる
			bb.OffsetLT	= m_offset_lt;
			bb.OffsetRB	= m_offset_rb;
			return bb;
		}
	
		/*-------------------------------------------------------------------------
		 컬링チェック
		---------------------------------------------------------------------------*/
		public bool IsCulling(CullingRect rect)
		{
			return IsCulling(new Vector2(0,0), 1, rect);
		}
		public bool IsCulling(Vector2 offset, float scale, CullingRect rect)
		{
			// 설정してないときは컬링する
			if(!m_is_1st)		return true;

			Vector2		pos2	= offsetscale(m_max, offset, scale);
			pos2				+=  m_offset_rb;
			if(pos2.X < rect.left_top.X)		return true;
			if(pos2.Y < rect.left_top.Y)		return true;

			Vector2		pos		= offsetscale(m_min + m_offset_lt, offset, scale);
			pos					+=  m_offset_lt;
			if(pos.X >= rect.right_bottom.X)	return true;
			if(pos.Y >= rect.right_bottom.Y)	return true;
			// 컬링しない
			return false;
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		static private Vector2 offsetscale(Vector2 p, Vector2 offset, float scale)
		{
			return (p + offset) * scale;
		}
	
		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		public void Draw(d3d_device device, float z, int color)
		{
			D3dBB2d.Draw(this, device, z, color);
		}
		static public void Draw(D3dBB2d bb, d3d_device device, float z, int color)
		{
			Draw(bb, device, z, new Vector2(0, 0), 1, color);
		}
		/*-------------------------------------------------------------------------
		 그리기
		 (offset + pos) * scale
		---------------------------------------------------------------------------*/
		public void Draw(d3d_device device, float z, Vector2 offset, float scale, int color)
		{
			D3dBB2d.Draw(this, device, z, offset, scale, color);
		}
		static public void Draw(D3dBB2d bb, d3d_device device, float z, Vector2 offset, float scale, int color)
		{
			Vector2		pos		= offsetscale(bb.Min, offset, scale);
			Vector2		pos2	= offsetscale(bb.Max, offset, scale);
			pos					+= bb.OffsetLT;
			pos2				+= bb.OffsetRB;
			Vector2		size	= pos2 - pos;

			device.DrawLineRect(new Vector3(pos.X, pos.Y, z), size, color);
		}

		/*-------------------------------------------------------------------------
		 컬링용
		---------------------------------------------------------------------------*/
		public struct CullingRect
		{
			public Vector2			left_top;
			public Vector2			right_bottom;

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public Vector2	Size			{	get{	return right_bottom - left_top;		}
												set{	right_bottom	= value	+ left_top;	}}

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public CullingRect(Vector2 _left_top, Vector2 _right_bottom)
			{
				left_top		= _left_top;
				right_bottom	= _right_bottom;
			}
			public CullingRect(float left, float top, float right, float bottom)
			{
				left_top		= new Vector2(left, top);
				right_bottom	= new Vector2(right, bottom);
			}
			public CullingRect(Vector2 size)
			{
				left_top		= new Vector2(0,0);
				right_bottom	= size;
			}
		}
	}
}
