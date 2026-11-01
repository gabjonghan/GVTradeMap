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
		private bool				m_is_1st;		// 최초의 위치를 설정하면 true
		private Vector2				m_offset_lt;	// 좌상단의 오프셋
		private Vector2				m_offset_rb;	// 우하단의 오프셋
	
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
		 갱신
		---------------------------------------------------------------------------*/
		public void Update(Vector2 pos)
		{
			if(!m_is_1st){
				// 처음이므로 단순히 대입
				m_min		= pos;
				m_max		= pos;
				m_is_1st	= true;
			}else{
				// 갱신
				if(pos.X < m_min.X)		m_min.X	= pos.X;
				if(pos.X > m_max.X)		m_max.X	= pos.X;
				if(pos.Y < m_min.Y)		m_min.Y	= pos.Y;
				if(pos.Y > m_max.Y)		m_max.Y	= pos.Y;
			}
		}

		/*-------------------------------------------------------------------------
		 갱신한 결과를 새로운 D3dBB2d 로 반환한다
		---------------------------------------------------------------------------*/
		public D3dBB2d IfUpdate(Vector2 pos)
		{
			D3dBB2d	bb	= new D3dBB2d();
			if(m_is_1st){
				// 초기화되어 있으면 최대와 최소를 설정한다
				bb.Update(Min);
				bb.Update(Max);
			}
			bb.Update(pos);		// 갱신해 본다
			bb.OffsetLT	= m_offset_lt;
			bb.OffsetRB	= m_offset_rb;
			return bb;
		}
	
		/*-------------------------------------------------------------------------
		 컬링 확인
		---------------------------------------------------------------------------*/
		public bool IsCulling(CullingRect rect)
		{
			return IsCulling(new Vector2(0,0), 1, rect);
		}
		public bool IsCulling(Vector2 offset, float scale, CullingRect rect)
		{
			// 설정하지 않았을 때는 컬링한다
			if(!m_is_1st)		return true;

			Vector2		pos2	= offsetscale(m_max, offset, scale);
			pos2				+=  m_offset_rb;
			if(pos2.X < rect.left_top.X)		return true;
			if(pos2.Y < rect.left_top.Y)		return true;

			Vector2		pos		= offsetscale(m_min + m_offset_lt, offset, scale);
			pos					+=  m_offset_lt;
			if(pos.X >= rect.right_bottom.X)	return true;
			if(pos.Y >= rect.right_bottom.Y)	return true;
			// 컬링하지 않는다
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
