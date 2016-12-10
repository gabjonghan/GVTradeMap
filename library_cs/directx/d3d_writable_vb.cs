/*-------------------------------------------------------------------------

 複数のvertex bufferを切り替えながらGPUの待ちを회避する構造

 d3d_writable_vb_with_indexは
 スプライト用のインデックスバッファ付き
 インデックスバッファは
 0-1
 | | 
 2-3
 の順でインデックスが振られている
 TriangleListで그리기すること
 インデックスバッファは16bitで作成されるため, 65536を越えるインデックスは無理
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
		private int						m_index;		// 현재バッファ

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public VertexBuffer vb		{		get{	return m_vb[m_index];	}}
	
		/*-------------------------------------------------------------------------
		 element_count数분のバーテクスバッファをbuffer_count個持つ
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
		 データ설정
		 全体をロックし, _objectを書き込みます
		---------------------------------------------------------------------------*/
		public void SetData<T>(T[] _object) where T : struct
		{
			m_vb[m_index].SetData(_object, 0, LockFlags.None);
		}

		/*-------------------------------------------------------------------------
		 データ설정
		 先頭から指定した数분書き込みます
		 少しだけ更新する場合用
		 なぜか非常に遅いので사용禁止
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
		 バッファフリップ
		 DrawPrimitive()を呼んだ直後にフリップすること
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
	 スプライト그리기用インデックスバッファ付き
	 インデックスバッファは (element_count / 4) * 6 確保される
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
		 インデックスバッファを構築する
		---------------------------------------------------------------------------*/
		public static IndexBuffer CreateSpriteIndexBuffer(Device device, int element_count)
		{
			int		count	= (element_count / 4) * 6;		// 4頂点で6つの頂点になる
			if(count >= UInt16.MaxValue){
				// 65536を越えてしまうときはエラー
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
