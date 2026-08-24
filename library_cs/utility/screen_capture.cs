/*-------------------------------------------------------------------------

 화면캡처
 캡처 대상은 GetDC() 로 얻을 것
 Vista Aero 에서는 캡처가 매우 느리므로 주의

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using win32;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace Utility
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class ScreenCapture : IDisposable
	{
		private Bitmap			m_bitmap;			// 캡처용
		private	byte[]			m_image;			// 캡처 이미지
		private int				m_stride;			// 캡처 이미지 스트라이드
		private Size			m_size;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public byte[] Image{	get{	return m_image;		}}
		public int Stride{		get{	return m_stride;	}}
		public Size Size{		get{	return m_size;		}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public ScreenCapture(int size_x, int size_y)
		{
			m_bitmap		= new Bitmap(size_x, size_y);
			m_size.Width	= size_x;
			m_size.Height	= size_y;
			m_image			= null;
			m_stride		= 0;
		}

		/*-------------------------------------------------------------------------
		 캡처
		---------------------------------------------------------------------------*/
		public void DoCapture(IntPtr src_hdc, Point dst_pos, Point src_pos, Size size)
		{
			Graphics	graphics	= Graphics.FromImage(m_bitmap);
			IntPtr		hdc			= graphics.GetHdc();
			gdi32.BitBlt(
				hdc, dst_pos.X, dst_pos.Y, size.Width, size.Height,
				src_hdc, src_pos.X, src_pos.Y,
				gdi32.SRCCOPY);
			graphics.ReleaseHdc(hdc);
			graphics.Dispose();
		}

		/*-------------------------------------------------------------------------
		 이미지 구축
		---------------------------------------------------------------------------*/
		public void CreateImage()
		{
			BitmapData	bmpdata	= m_bitmap.LockBits(new Rectangle(0, 0, m_size.Width, m_size.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

			IntPtr		ptr		= bmpdata.Scan0;
			int			length	= bmpdata.Height * bmpdata.Stride;
			m_stride			= bmpdata.Stride;
			// 이미지 버퍼에 복사한다
			update_image_buffer(length);
			Marshal.Copy(ptr, m_image, 0, length);
			m_bitmap.UnlockBits(bmpdata);
		}

		/*-------------------------------------------------------------------------
		 이미지용 버퍼를 작성한다
		 이미 작성된 경우에는 아무것도 하지 않는다
		 크기가 다른 경우에는 다시 작성한다
		---------------------------------------------------------------------------*/
		private void update_image_buffer(int length)
		{
			if(m_image == null){
				// 작성하지 않았으므로 작성한다
				m_image			= new byte[length];
			}else{
				if(m_image.Length != length){
					// 크기가 다르므로 다시 작성한다
					m_image		= new byte[length];
				}
			}
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public void Dispose()
		{
			if(m_bitmap != null)	m_bitmap.Dispose();
			m_bitmap	= null;
		}
	}
}
