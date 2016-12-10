/*-------------------------------------------------------------------------

 화면캡처
 캡처対象はGetDC()で得ること
 Vista Aero時は캡처が非常に重いので注意

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
		private Bitmap			m_bitmap;			// 캡처用
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
		 イメージの構築
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
		 イメージ用バッファを作成する
		 すでに作成済みのときはなにもしない
		 サイズが異なる場合は作成し直す
		---------------------------------------------------------------------------*/
		private void update_image_buffer(int length)
		{
			if(m_image == null){
				// 作成していないので作成する
				m_image			= new byte[length];
			}else{
				if(m_image.Length != length){
					// サイズが異なるので作成し直す
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
