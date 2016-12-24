/*-------------------------------------------------------------------------

 화면캡처
 캡처대상はGetDC()で得ること
 Vista Aero時は캡처が非常に중いので注意

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
		private	byte[]			m_image;			// 캡처イメージ
		private int				m_stride;			// 캡처イメージストライド
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
		 イメージの구축
		---------------------------------------------------------------------------*/
		public void CreateImage()
		{
			BitmapData	bmpdata	= m_bitmap.LockBits(new Rectangle(0, 0, m_size.Width, m_size.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

			IntPtr		ptr		= bmpdata.Scan0;
			int			length	= bmpdata.Height * bmpdata.Stride;
			m_stride			= bmpdata.Stride;
			// イメージバッファにコピーする
			update_image_buffer(length);
			Marshal.Copy(ptr, m_image, 0, length);
			m_bitmap.UnlockBits(bmpdata);
		}

		/*-------------------------------------------------------------------------
		 イメージ용バッファを작성함
		 すでに작성済みのときはなにもしない
		 사이즈が異なる場合は작성し直す
		---------------------------------------------------------------------------*/
		private void update_image_buffer(int length)
		{
			if(m_image == null){
				// 작성していないので작성함
				m_image			= new byte[length];
			}else{
				if(m_image.Length != length){
					// 사이즈が異なるので작성し直す
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
