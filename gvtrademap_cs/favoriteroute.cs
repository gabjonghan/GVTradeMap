/*-------------------------------------------------------------------------

 항로도 즐겨찾기 합성
 합성 후의 이미지를 기록한다
 합성 후의 이미지가 최신인 경우에는 아무것도 하지 않는다

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Utility;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	static class favoriteroute
	{
		/*-------------------------------------------------------------------------
		 합성
		 A + B -> C
		---------------------------------------------------------------------------*/
		static public bool MixMap(string fname_a, string fname_b, string fname_c, ImageFormat format)
		{
			bool	is_create	= false;
	
			// 파일 존재 확인
			if(!File.Exists(fname_a))	return false;
			if(!File.Exists(fname_b)){
				// B 가 없는 경우에는 C 를 삭제한다
				file_ctrl.RemoveFile(fname_c);
				return false;
			}

			// 작성할지 여부 확인
			if(!File.Exists(fname_c)){
				// C 가 없을 때는 무조건 작성
				is_create	= true;
			}else{
				FileInfo	info_a	= new FileInfo(fname_a);
				FileInfo	info_b	= new FileInfo(fname_b);
				FileInfo	info_c	= new FileInfo(fname_c);

				// 합성 후의 정보가 오래된 경우에는 작성한다
				if(info_a.LastWriteTime > info_c.LastWriteTime)		is_create	= true;
				if(info_b.LastWriteTime > info_c.LastWriteTime)		is_create	= true;
			}

			if(!is_create)				return true;	// 최신

			try{
				Size		size_a, size_b;
				int			stride_a, stride_b;
				byte[]		image_a		= load_image(fname_a, out size_a, out stride_a);
				byte[]		image_b		= load_image(fname_b, out size_b, out stride_b);

				if(image_a.Length != image_b.Length){
					MessageBox.Show("항로도 즐겨찾기의 이미지 크기가 지도와 다릅니다. ", "항로도 즐겨찾기 합성 중", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}

				// 합성
				// 단순히 50% 로 합성된다
				// 순백색은 무시된다
				for(int y=0; y<size_a.Height; y++){
					int	index	= stride_a * y;
					for(int x=0; x<size_a.Width; x++, index+=3){
						byte c20	= image_b[index + 0];
						byte c21	= image_b[index + 1];
						byte c22	= image_b[index + 2];

						// 투명 처리 확인
						// 흰색
						if(   (c20 >= 250)
							&&(c21 >= 250)
							&&(c22 >= 250) ) continue;

						byte c10	= image_a[index + 0];
						byte c11	= image_a[index + 1];
						byte c12	= image_a[index + 2];

						// 합성
						c10		= (byte)((c10 / 2) + (c20 / 2));
						c11		= (byte)((c11 / 2) + (c21 / 2));
						c12		= (byte)((c12 / 2) + (c22 / 2));
						image_a[index + 0]	= c10;
						image_a[index + 1]	= c11;
						image_a[index + 2]	= c12;
					}
				}

				// 기록
				GCHandle	handle	= GCHandle.Alloc(image_a, GCHandleType.Pinned);
				Bitmap		bitmap	= new Bitmap(	size_a.Width, size_a.Height, stride_a,
													PixelFormat.Format24bppRgb,
													handle.AddrOfPinnedObject());
				// 기록 포맷을 선택한다
				bitmap.Save(fname_c, format);
				handle.Free();
				bitmap.Dispose();
			}catch{
			}

			System.GC.Collect();
			return true;
		}

		/*-------------------------------------------------------------------------
		 이미지를 얻는다
		---------------------------------------------------------------------------*/
		static private byte[] load_image(string fname, out Size size, out int stride)
		{
			Bitmap	bitmap		= new Bitmap(fname);
			size				= new Size(bitmap.Width, bitmap.Height);
			
			// 잠그고 이미지를 꺼낸다
			BitmapData bmpdata	= bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
												ImageLockMode.ReadOnly,
												PixelFormat.Format24bppRgb);

			IntPtr		ptr		= bmpdata.Scan0;
			stride				= bmpdata.Stride;
			int			length	= bmpdata.Height * stride;
			byte[]		image	= new byte[length];
			Marshal.Copy(ptr, image, 0, length);
			bitmap.UnlockBits(bmpdata);

			// 해제해 둔다
			bitmap.Dispose();
			return image;
		}
	}
}
