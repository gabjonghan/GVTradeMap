/*-------------------------------------------------------------------------

 여러 개의 vertex buffer 를 번갈아 쓰면서 GPU 대기를 회피하는 구조

 d3d_writable_vb_with_index 는
 스프라이트용 인덱스 버퍼 포함
 인덱스 버퍼는
 0-1
 | | 
 2-3
 순서로 인덱스가 매겨져 있다
 TriangleList 로 그릴 것
 인덱스 버퍼는 16bit 로 작성되므로 65536 을 넘는 인덱스는 불가능
---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Runtime.InteropServices;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace directx
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class d3d_writable_vb : IDisposable
	{
		private VertexBuffer[]			m_vb;			// 
		private int						m_index;		// 현재 버퍼

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public VertexBuffer vb		{		get{	return m_vb[m_index];	}}
	
		/*-------------------------------------------------------------------------
		 element_count 개수만큼의 버텍스 버퍼를 buffer_count 개 가진다
		---------------------------------------------------------------------------*/
		public d3d_writable_vb(Device device, Type type, int element_count, int buffer_count)
		{
			m_vb		= new VertexBuffer[buffer_count];
			m_index		= 0;
			for(int i=0; i<buffer_count; i++){
				m_vb[i]			= new VertexBuffer(	type, element_count, 
													device,
													Usage.WriteOnly,
													VertexFormats.None, Pool.Managed);
			}
		}
	
		/*-------------------------------------------------------------------------
		 데이터설정
		 전체를 잠그고 _object 를 쓴다
		---------------------------------------------------------------------------*/
		public void SetData<T>(T[] _object) where T : struct
		{
			m_vb[m_index].SetData(_object, 0, LockFlags.None);
		}

		/*-------------------------------------------------------------------------
		 데이터설정
		 선두부터 지정한 개수만큼 쓴다
		 조금만 갱신할 때용
		 왠지 매우 느리므로 사용 금지
		---------------------------------------------------------------------------*/
/*		public void SetData<T>(T[] _object, int count) where T : struct
		{
			GraphicsStream	st	= m_vb[m_index].Lock(0, Marshal.SizeOf(typeof(T)) * count, LockFlags.None);
			for(int i=0; i<count; i++){
				st.Write(_object[i]);
			}
			m_vb[m_index].Unlock();
		}
*/	
		/*-------------------------------------------------------------------------
		 버퍼 플립
		 DrawPrimitive() 를 호출한 직후에 플립할 것
		---------------------------------------------------------------------------*/
		public void Flip()
		{
			if(++m_index >= m_vb.Length){
				m_index		= 0;
			}
		}

		/*-------------------------------------------------------------------------
		 Dispose
		---------------------------------------------------------------------------*/
		public virtual void Dispose()
		{
			if(m_vb == null)		return;
			if(m_vb.Length <= 0)	return;
	
			for(int i=0; i<m_vb.Length; i++){
				m_vb[i].Dispose();
			}
			m_vb		= null;
			m_index		= 0;
		}
	}

	/*-------------------------------------------------------------------------
	 스프라이트 그리기용 인덱스 버퍼
	 인덱스 버퍼는 (element_count / 4) * 6 보장함
	---------------------------------------------------------------------------*/
	public class d3d_writable_vb_with_index : d3d_writable_vb
	{
		private IndexBuffer				m_ib;			// index buffer

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public IndexBuffer ib		{	get{	return m_ib;	}}
	
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public d3d_writable_vb_with_index(Device device, Type type, int element_count, int buffer_count)
			: base(device, type, element_count, buffer_count)
		{
			m_ib	= CreateSpriteIndexBuffer(device, element_count);
		}

		/*-------------------------------------------------------------------------
		 인덱스 버퍼를 구축한다
		---------------------------------------------------------------------------*/
		public static IndexBuffer CreateSpriteIndexBuffer(Device device, int element_count)
		{
			int		count	= (element_count / 4) * 6;		// 정점 4 개가 정점 6 개가 된다
			if(count >= UInt16.MaxValue){
				// 65536 을 넘어버릴 때는 오류
				throw new Exception();
			}
			IndexBuffer	ib	= new IndexBuffer(device, count * sizeof(short), Usage.WriteOnly, Pool.Managed, true);

			// 인덱스를 할당함
			UInt16[] indices = new UInt16[count];
			UInt16 vertexIndex = 0;
			for(int i=0; i<count; i+= 6){
				indices[i + 0] = (UInt16)( vertexIndex + 0 );
				indices[i + 1] = (UInt16)( vertexIndex + 1 );
				indices[i + 2] = (UInt16)( vertexIndex + 2 );

				indices[i + 3] = (UInt16)( vertexIndex + 1 );
				indices[i + 4] = (UInt16)( vertexIndex + 3 );
				indices[i + 5] = (UInt16)( vertexIndex + 2 );

				vertexIndex += 4;
			}
			ib.SetData(indices, 0, LockFlags.None);
			return ib;
		}

		/*-------------------------------------------------------------------------
		 Dispose
		---------------------------------------------------------------------------*/
		public override void Dispose()
		{
			if(m_ib != null){
				m_ib.Dispose();
				m_ib	= null;
			}
			base.Dispose();
		}
	}
}
