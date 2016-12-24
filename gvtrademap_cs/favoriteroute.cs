/*-------------------------------------------------------------------------

 항로도즐겨찾기の合成
 合成후の画상を書き出す
 合成후の画상が최신の場合なにもしない

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
		 合成
		 A + B -> C
		---------------------------------------------------------------------------*/
		static public bool MixMap(string fname_a, string fname_b, string fname_c, ImageFormat format)
		{
			bool	is_create	= false;
	
			// 파일の存在확인
			if(!File.Exists(fname_a))	return false;
			if(!File.Exists(fname_b)){
				// Bがない場合はCを삭제する
				file_ctrl.RemoveFile(fname_c);
				return false;
			}

			// 작성함かどうかのチェック
			if(!File.Exists(fname_c)){
				// Cがないときは無条건で작성
				is_create	= true;
			}else{
				FileInfo	info_a	= new FileInfo(fname_a);
				FileInfo	info_b	= new FileInfo(fname_b);
				FileInfo	info_c	= new FileInfo(fname_c);

				// 合成후の정보が古い場合は작성함
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
					MessageBox.Show("항로도즐겨찾기の画상사이즈が지도と異なります. ", "항로도즐겨찾기の合成중", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}

				// 合成
				// 単純に50%で合成される
				// 真白は無視される
				for(int y=0; y<size_a.Height; y++){
					int	index	= stride_a * y;
					for(int x=0; x<size_a.Width; x++, index+=3){
						byte c20	= image_b[index + 0];
						byte c21	= image_b[index + 1];
						byte c22	= image_b[index + 2];

						// 抜きチェック
						// 白
						if(   (c20 >= 250)
							&&(c21 >= 250)
							&&(c22 >= 250) ) continue;

						byte c10	= image_a[index + 0];
						byte c11	= image_a[index + 1];
						byte c12	= image_a[index + 2];

						// 合成
						c10		= (byte)((c10 / 2) + (c20 / 2));
						c11		= (byte)((c11 / 2) + (c21 / 2));
						c12		= (byte)((c12 / 2) + (c22 / 2));
						image_a[index + 0]	= c10;
						image_a[index + 1]	= c11;
						image_a[index + 2]	= c12;
					}
				}

				// 書き出し
				GCHandle	handle	= GCHandle.Alloc(image_a, GCHandleType.Pinned);
				Bitmap		bitmap	= new Bitmap(	size_a.Width, size_a.Height, stride_a,
													PixelFormat.Format24bppRgb,
													handle.AddrOfPinnedObject());
				// 書き出しフォーマットを선택する
				bitmap.Save(fname_c, format);
				handle.Free();
				bitmap.Dispose();
			}catch{
			}

			System.GC.Collect();
			return true;
		}

		/*-------------------------------------------------------------------------
		 イメージを得る
		---------------------------------------------------------------------------*/
		static private byte[] load_image(string fname, out Size size, out int stride)
		{
			Bitmap	bitmap		= new Bitmap(fname);
			size				= new Size(bitmap.Width, bitmap.Height);
			
			// ロックしてイメージ取り出し
			BitmapData bmpdata	= bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
												ImageLockMode.ReadOnly,
												PixelFormat.Format24bppRgb);

			IntPtr		ptr		= bmpdata.Scan0;
			stride				= bmpdata.Stride;
			int			length	= bmpdata.Height * stride;
			byte[]		image	= new byte[length];
			Marshal.Copy(ptr, image, 0, length);
			bitmap.UnlockBits(bmpdata);

			// 解放しておく
			bitmap.Dispose();
			return image;
		}
	}
}
