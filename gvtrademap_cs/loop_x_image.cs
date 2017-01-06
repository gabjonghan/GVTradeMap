/*-------------------------------------------------------------------------

 x방향にループするイメージ
 イメージに合成してしまう모드に대응
 イメージは렌더링 타겟として確保される

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using directx;
using Utility;
using System.Drawing.Drawing2D;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	class TextureUnit : IDisposable
	{
		private d3d_device m_device;				// デバイス
		private Texture m_texture;			  // 그리기する텍스쳐
		private Texture m_texture_sysmem;	   // 시스템메모リに確保した텍스쳐
		private Vector2 m_offset;			   // 切り出しオフセット
		private Size m_size;					// 切り出し사이즈
		private Size m_texture_size;			// 텍스쳐사이즈
												// 

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public bool IsCreate { get { return (m_texture != null); } }

		public Texture Texture { get { return m_texture; } }
		public Vector2 Offset { get { return m_offset; } }

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public TextureUnit()
		{
			m_device = null;
			m_texture = null;
			m_texture_sysmem = null;
		}

		/*-------------------------------------------------------------------------
		 image から rect 사이즈を切り取って텍스쳐を작성
		 렌더링 타겟を작성함ことにより, 사이즈を2のべき乗に조정する
		---------------------------------------------------------------------------*/
		virtual public void Create(d3d_device device, ref byte[] image, Size size, int stride, Rectangle rect)
		{
			m_device = device;
			m_offset.X = rect.X;
			m_offset.Y = rect.Y;

			m_size.Width = rect.Width;
			m_size.Height = rect.Height;
			if ((m_size.Width + rect.X) > size.Width)
			{
				m_size.Width = size.Width - rect.X;
			}
			if ((m_size.Height + rect.Y) > size.Height)
			{
				m_size.Height = size.Height - rect.Y;
			}

			// 텍스쳐사이즈を2のべき乗にする
			m_texture_size = d3d_utility.TextureSizePow2(m_size);
			//			m_texture_size	= m_size;

			// 텍스쳐の작성とロック
			if (m_texture_sysmem != null) m_texture_sysmem.Dispose();
			m_texture_sysmem = new Texture(m_device.device, m_texture_size.Width, m_texture_size.Height,
												1, Usage.Dynamic, Format.R5G6B5, Pool.SystemMemory);
			UInt16[,] buf = (UInt16[,])m_texture_sysmem.LockRectangle(typeof(UInt16), 0, LockFlags.Discard, m_texture_size.Height, m_texture_size.Width);

			// 텍스쳐내용の生成
			int index = (stride * rect.Y) + (rect.X * sizeof(UInt16));
			for (int y = 0; y < m_size.Height; y++)
			{
				UInt16 color = 0;
				for (int x = 0; x < m_size.Width; x++)
				{
					color = (UInt16)((image[index + x * sizeof(UInt16) + 1] << 8)
										| (image[index + x * sizeof(UInt16) + 0] << 0));
					buf[y, x] = color;
				}
				// 사이즈조정をした場合は1ドット余분にコピーする
				// バイリニア時の참조장소対策
				if (m_texture_size.Width > m_size.Width)
				{
					buf[y, m_size.Width] = color;
				}
				index += stride;
			}
			m_texture_sysmem.UnlockRectangle(0);

			if (m_texture != null) m_texture.Dispose();

			// Managedな텍스쳐に전送する
			//			m_texture	= d3d_utility.CreateTextureFromTexture(m_device.Device, Texture);
			// 렌더링 타겟の텍스쳐に전送する
			RefreshTexture();
		}

		/*-------------------------------------------------------------------------
		 image から rect 사이즈を切り取って텍스쳐を작성
		 マスク용
		---------------------------------------------------------------------------*/
		virtual public void CreateFromMask(d3d_device device, ref byte[] image, Size size, int stride, Rectangle rect)
		{
			m_device = device;
			m_offset.X = rect.X;
			m_offset.Y = rect.Y;

			m_size.Width = rect.Width;
			m_size.Height = rect.Height;
			if ((m_size.Width + rect.X) > size.Width)
			{
				m_size.Width = size.Width - rect.X;
			}
			if ((m_size.Height + rect.Y) > size.Height)
			{
				m_size.Height = size.Height - rect.Y;
			}

			m_texture_size = m_size;

			// 텍스쳐の작성とロック
			using (Texture texture = new Texture(m_device.device, m_size.Width, m_size.Height,
												1, Usage.Dynamic, Format.A1R5G5B5, Pool.SystemMemory))
			{
				UInt16[,] buf = (UInt16[,])texture.LockRectangle(typeof(UInt16), 0, LockFlags.Discard, m_size.Height, m_size.Width);

				// 텍스쳐내용の生成
				int index = (stride * rect.Y) + (rect.X * sizeof(UInt16));
				for (int y = 0; y < m_size.Height; y++)
				{
					for (int x = 0; x < m_size.Width; x++)
					{
						UInt16 color = (UInt16)((image[index + x * sizeof(UInt16) + 1] << 8)
												| (image[index + x * sizeof(UInt16) + 0] << 0));

						// jpgなのである程도誤差を認める
						if ((((color >> 0) & 0x1f) < 0x10)
							|| (((color >> 5) & 0x3f) < 0x30)
							|| (((color >> 5 + 6) & 0x1f) < 0x10))
						{
							color = 0x7fff;	 // 抜き
						}

						buf[y, x] = color;
					}
					index += stride;
				}
				texture.UnlockRectangle(0);

				// Managedな텍스쳐に전送する
				if (m_texture != null) m_texture.Dispose();
				m_texture = d3d_utility.CreateTextureFromTexture(m_device.device, texture);
			}
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public void Dispose()
		{
			if (m_texture != null)
			{
				m_texture.Dispose();
				m_texture = null;
			}
			if (m_texture_sysmem != null)
			{
				m_texture_sysmem.Dispose();
				m_texture_sysmem = null;
			}
		}

		/*-------------------------------------------------------------------------
		 リフレッシュ
		 デバイスロスト対策
		---------------------------------------------------------------------------*/
		public void RefreshTexture()
		{
			// 特殊な사용をしない
			if (m_texture_sysmem == null) return;

			// すでにあれば解放する
			if (m_texture != null) m_texture.Dispose();

			// 렌더링 타겟として사용できる텍스쳐を작성し, 전送する
			m_texture = d3d_utility.CreateRenderTargetTextureSameSize(m_device.device, m_texture_sysmem);
			d3d_utility.CopyTexture(m_device.device, m_texture, m_texture_sysmem);
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		public void Draw(Vector3 pos, float scale)
		{
			Draw(pos, scale, Color.White.ToArgb());
		}
		public void Draw(Vector3 pos, float scale, int color)
		{
			if (m_texture == null) return;

			Vector2 offset = m_offset * scale;
			//			Vector2		ImageSize		= new Vector2(ImageScale * m_size.Width, ImageScale * m_size.Height);
			Vector2 size = new Vector2(scale * m_texture_size.Width, scale * m_texture_size.Height);
			Vector2 client_size = m_device.client_size;

			// オフセットを考慮
			pos.X *= scale;
			pos.Y *= scale;
			pos.X += offset.X;
			pos.Y += offset.Y;

			// 그리기범위チェック
			// debug
			// 범위내の端付近で컬링する
			//			if(pos.X + ImageSize.X < 0+32)		return;
			//			if(pos.Y + ImageSize.Y < 0+32)		return;
			//			if(pos.X > client_size.X-32)	return;
			//			if(pos.Y > client_size.Y-32)	return;
			// 통상の컬링
			if (pos.X + size.X < 0) return;
			if (pos.Y + size.Y < 0) return;
			if (pos.X > client_size.X) return;
			if (pos.Y > client_size.Y) return;

			// 그리기
			m_device.DrawTexture(m_texture, pos, size, color);
		}
	}

	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class LoopXImage : IDisposable
	{
		private const int TEXSIZE_STEP = 512;	   // 분할사이즈
		private const int HEIGHT_MARGIN = 200;	  // 상下のスクロールマージン
		private const float SCALE_MIN = 0.15f;	   // 縮소한계
		private const float SCALE_MAX = 2;		// 拡대한계

		private d3d_device m_device;					// デバイス

		private Vector2 m_image_size;			   // 이미지 전체의 크기
		private List<TextureUnit> m_textures;				   // 텍스쳐목록

		// 그리기위치と스케일
		private Vector2 m_offset;				   // オフセット
		private float m_scale;				  // 拡대縮소率

		// 그리기위치と스케일退避용
		private Vector2 m_offset_shelter;		   // オフセット
		private float m_scale_shelter;		  // 拡대縮소率
		private bool m_is_pushed_params;			// 

		// 스레드で로드場合の진행具合
		private int m_load_current;
		private int m_load_max;
		private string m_load_str;

		// 그리기支援デリゲート
		// 그리기時のX방향ループに대응し, オフセットを解決する
		// このデリゲートが何회呼ばれるかはクライアント사이즈と画상사이즈による
		public delegate void DrawHandler(Vector2 draw_offset, LoopXImage image);

		private bool m_device_lost;

		public int MargeImageMS;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		// 설정파일읽기용
		public Vector2 OffsetPosition
		{
			get { return m_offset; }
			set { m_offset = value; }
		}

		public Vector2 ImageSize { get { return m_image_size; } }
		public float ImageScale { get { return m_scale; } }

		public int LoadCurrent { get { return m_load_current; } }
		public int LoadMax { get { return m_load_max; } }
		public string LoadStr { get { return m_load_str; } }


		public d3d_device Device { get { return m_device; } }

		/*-------------------------------------------------------------------------
		 オフセットの加算
		---------------------------------------------------------------------------*/
		public void AddOffset(Vector2 add_offset)
		{
			// 스케일が소さいほど이동량が대きくなる
			m_offset += add_offset * (1 / ImageScale);
		}

		/*-------------------------------------------------------------------------
		 스케일설정
		 지정하는 위치를 중심으로 하는 경우는 true를 전달, 
		 중심으로 하는 위치를 pos에 설정함
		 마우스의 클라이언트 좌표를 전달, 또는 클라이언트 영역의 중심을 통과
		 false시 pos을 참조하지 않음
		---------------------------------------------------------------------------*/
		public void SetScale(float scale, Point center_pos, bool is_center_mouse)
		{
			// 범위チェック
			if (scale < SCALE_MIN) scale = SCALE_MIN;
			if (scale > SCALE_MAX) scale = SCALE_MAX;

			if ((scale > 1 - 1e-6) && (scale < 1 + 1e-6))
			{
				scale = 1;
			}

			if (is_center_mouse)
			{
				// 지정된위치が중心になるようにオフセットを변경する
				Vector2 pos = new Vector2(center_pos.X, center_pos.Y);

				// 중心위치の差분を得る
				Vector2 old_offset = pos * (1 / m_scale);
				Vector2 new_offset = pos * (1 / scale);

				// オフセットを수정する
				m_offset -= old_offset - new_offset;

				m_scale = scale;
			}
			else
			{
				// オフセットを변경しない
				m_scale = scale;
			}
		}

		/*-------------------------------------------------------------------------
		 지정된좌표が중心になるようにオフセットを변경する
		---------------------------------------------------------------------------*/
		public void MoveCenterOffset(Point center)
		{
			MoveCenterOffset(center, new Point(0, 0));
		}
		public void MoveCenterOffset(Point center, Point offset)
		{
			Vector2 pos = new Vector2(center.X, center.Y);
			Vector2 soffset = new Vector2((m_device.client_size.X - offset.X) / 2, (m_device.client_size.Y - offset.Y) / 2);
			Vector2 _offset = new Vector2(offset.X, offset.Y);

			_offset *= (1f / ImageScale);
			soffset *= (1f / ImageScale);
			pos -= soffset;
			pos -= _offset;
			m_offset = -pos;
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public LoopXImage(d3d_device device)
		{
			m_device = device;
			m_offset = new Vector2(0, 0);
			m_textures = new List<TextureUnit>();
			m_is_pushed_params = false;

			m_scale = 1;

			m_offset_shelter = new Vector2(0, 0);
			m_scale_shelter = 1;

			m_device_lost = false;
			if (m_device.device != null)
			{
				m_device.device.DeviceReset += new System.EventHandler(device_reset);
			}
		}

		/*-------------------------------------------------------------------------
		 デバイスロスト
		 플래그だけ立てる
		---------------------------------------------------------------------------*/
		private void device_reset(object sender, System.EventArgs e)
		{
			m_device_lost = true;
		}

		/*-------------------------------------------------------------------------
		 イメージの구축
		 進捗현황の初期化
		---------------------------------------------------------------------------*/
		public void InitializeCreateImage()
		{
			m_load_current = 0;
			m_load_max = 0;
			m_load_str = "";
		}

		/// <summary>
		/// Resize the image to the specified width and height.
		/// </summary>
		/// <param name="image">The image to resize.</param>
		/// <param name="width">The width to resize to.</param>
		/// <param name="height">The height to resize to.</param>
		/// <returns>The resized image.</returns>
		public static Bitmap ResizeImage(Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}

		/*-------------------------------------------------------------------------
		 이미지 구축
		---------------------------------------------------------------------------*/
		public bool CreateImage(string file_name)
		{
			// 텍스쳐목록を解放
			Dispose();

			InitializeCreateImage();

			try
			{
				// イメージの읽기
				m_load_str = file_name;
				Bitmap bitmap = ResizeImage(new Bitmap(file_name), (int)def.GAME_WIDTH/4, (int)def.GAME_HEIGHT/4);
				Vector2 size = new Vector2(bitmap.Width, bitmap.Height);
				m_image_size = size;

				// 로드枚수を수える
				int count_x = bitmap.Width / TEXSIZE_STEP;
				if ((bitmap.Width % TEXSIZE_STEP) != 0) count_x++;
				int count_y = bitmap.Height / TEXSIZE_STEP;
				if ((bitmap.Height % TEXSIZE_STEP) != 0) count_y++;
				m_load_max = count_x * count_y;

				// ロックしてイメージ取り出し
				// R5G6B5に변환しておく
				BitmapData bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
													ImageLockMode.ReadOnly,
													PixelFormat.Format16bppRgb565);
				//													PixelFormat.Format32bppArgb);

				IntPtr ptr = bmpdata.Scan0;
				int length = bmpdata.Height * bmpdata.Stride;
				int stride = bmpdata.Stride;
				byte[] image = new byte[length];
				Marshal.Copy(ptr, image, 0, length);
				bitmap.UnlockBits(bmpdata);

				// オリジナルは解放しておく
				bitmap.Dispose();
				bitmap = null;

				// 분할して텍스쳐を구축する
				m_load_str = "텍스쳐전송중...";
				for (int y = 0; y < m_image_size.Y; y += TEXSIZE_STEP)
				{
					for (int x = 0; x < m_image_size.X; x += TEXSIZE_STEP)
					{
						TextureUnit tex = new TextureUnit();
						tex.Create(m_device,
									ref image,
									new Size((int)m_image_size.X, (int)m_image_size.Y),
									stride,
									new Rectangle(x, y, TEXSIZE_STEP, TEXSIZE_STEP));
						m_textures.Add(tex);
						m_load_current++;
					}
				}
				image = null;
				m_load_str = "완료";

				System.GC.Collect();
			}
			catch
			{
				// 何かを실패
				return false;
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 イメージとの合成
		 Begin()を読んだ直후に呼ぶこと
		 must_margeがtrueのとき必ず合成する
		 デバイスロスト時は必ず合成する
		 合成しないときは handler にnullを지정できる

		 デバイスロスト時の処理を含むため, 必ず呼び出す必要がある
		---------------------------------------------------------------------------*/
		public void MergeImage(DrawHandler handler, bool must_merge)
		{
			if (!m_device_lost && !must_merge) return;

			DateTimer d = new DateTimer();

			// 텍스쳐업데이트
			foreach (TextureUnit tex in m_textures)
			{
				tex.RefreshTexture();
			}
			// 合成대상がなければ返る
			if (handler == null)
			{
				m_device_lost = false;
				// 合成に掛かった시간	
				MargeImageMS = d.GetSectionTimeMilliseconds();
				return;
			}

			// 合成
			// 렌더링 타겟を지정
			Surface depth = m_device.device.DepthStencilSurface;
			Surface backbuffer = m_device.device.GetBackBuffer(0, 0, BackBufferType.Mono);
			m_device.device.DepthStencilSurface = null;	 // zバッファなし

			try
			{
				foreach (TextureUnit tex in m_textures)
				{
					// 렌더링 타겟を설정

					if (tex.Texture == null) continue;	  // 텍스쳐が作れていない

					//					Surface	a	= ((Texture)null).GetSurfaceLevel(0);
					m_device.device.SetRenderTarget(0, tex.Texture.GetSurfaceLevel(0));
					m_device.UpdateClientSize();

					// 그리기위치と스케일を退避
					PushDrawParams();
					m_offset = -tex.Offset;
					SetScale(1, new Point(0, 0), false);

					// 렌더링
					handler(m_offset, this);

					PopDrawParams();
				}
			}
			catch
			{
				// 保険
				PopDrawParams();
			}

			// 렌더링 타겟を元に戻す
			m_device.device.DepthStencilSurface = depth;
			m_device.device.SetRenderTarget(0, backbuffer);
			m_device.UpdateClientSize();

			backbuffer.Dispose();
			depth.Dispose();

			m_device_lost = false;
			// 合成に掛かった시간	
			MargeImageMS = d.GetSectionTimeMilliseconds();
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		public void Draw()
		{
			// 읽기チェック
			if (m_image_size.X <= 0) return;
			if (m_image_size.Y <= 0) return;

			// Y방향クランプなし
			// 自由にスクロールする

			// 그리기
			m_device.device.RenderState.ZBufferEnable = false;
			EnumDrawCallBack(new DrawHandler(draw_proc), 0);
			m_device.device.RenderState.ZBufferEnable = true;
		}

		/*-------------------------------------------------------------------------
		 그리기시작위치を求める
		 화면외にどれだけ余りを持たせるか지정できる
		---------------------------------------------------------------------------*/
		private void ajust_draw_start_offset_x(float outside_length_x)
		{
			while (m_offset.X > -outside_length_x) m_offset.X -= m_image_size.X;
			while (m_offset.X <= -(m_image_size.X + outside_length_x)) m_offset.X += m_image_size.X;
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		private void draw_proc(Vector2 offset, LoopXImage image)
		{
			Vector3 pos = new Vector3(offset.X, offset.Y, 0.9f);
			foreach (TextureUnit tex in m_textures)
			{
				tex.Draw(pos, ImageScale);
			}
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public void Dispose()
		{
			foreach (TextureUnit tex in m_textures)
			{
				tex.Dispose();
			}
			m_textures.Clear();
		}

		/*-------------------------------------------------------------------------
		 마우스좌표と표시オフセットを解決した좌표を得る
		 마우스좌표はクライアントレクトからの相対좌표であること
		---------------------------------------------------------------------------*/
		public Point MousePos2GlobalPos(Point mouse_pos)
		{
			return transform.ToPoint(MousePos2GlobalPos(transform.ToVector2(mouse_pos)));
		}
		public Vector2 MousePos2GlobalPos(Vector2 mouse_pos)
		{
			// 스케일の逆수
			mouse_pos *= 1 / ImageScale;

			Vector2 pos = mouse_pos - m_offset;

			// イメージ사이즈に収まるように조정
			while (pos.X >= m_image_size.X) pos.X -= m_image_size.X;
			return pos;
		}

		/*-------------------------------------------------------------------------
		 그리기オフセットを得る
		 그리기オフセットは整수に丸められている
		---------------------------------------------------------------------------*/
		public Vector2 GetDrawOffset()
		{
			Vector2 _offset = OffsetPosition;
			_offset.X = (float)(int)_offset.X;
			_offset.Y = (float)(int)_offset.Y;
			return _offset;
		}

		/*-------------------------------------------------------------------------
		 グローバルな지도좌표から그리기용の좌표に변환する
		---------------------------------------------------------------------------*/
		public Vector2 GlobalPos2LocalPos(Vector2 global_pos)
		{
			global_pos += GetDrawOffset();
			global_pos *= ImageScale;
			return global_pos;
		}

		/*-------------------------------------------------------------------------
		 オフセット좌표がクライアント내に入るように조정する
		 X방향のみ
		---------------------------------------------------------------------------*/
		public Vector2 AjustLocalPos(Vector2 pos)
		{
			if (pos.X > 0)
			{
				while (pos.X >= m_device.device.Viewport.Width)
				{
					pos.X -= m_image_size.X * ImageScale;
				}
			}
			else
			{
				while (pos.X < 0)
				{
					pos.X += m_image_size.X * ImageScale;
				}
			}
			return pos;
		}

		/*-------------------------------------------------------------------------
		 グローバルな지도좌표から그리기용の좌표に변환する
		 オフセット지정版
		---------------------------------------------------------------------------*/
		public Vector2 GlobalPos2LocalPos(Vector2 global_pos, Vector2 _offset)
		{
			global_pos += _offset;
			global_pos *= ImageScale;
			return global_pos;
		}

		/*-------------------------------------------------------------------------
		 그리기支援

		 最終的な그리기좌표は
		 GlobalPos2LocalPos();
		 で得ること
		---------------------------------------------------------------------------*/
		public void EnumDrawCallBack(DrawHandler handler, float outside_offset_x)
		{
			if (handler == null) return;

			ajust_draw_start_offset_x(outside_offset_x);

			Vector2 offset = GetDrawOffset();
			do
			{
				// 그리기コールバック呼び出し
				handler(offset, this);

				// オフセット조정
				offset.X += ImageSize.X;
			} while (offset.X < (((1f / ImageScale) * m_device.client_size.X) + outside_offset_x));
		}

		/*-------------------------------------------------------------------------
		 그리기위치と스케일をpush
		 1회しかpushできないので注意
		---------------------------------------------------------------------------*/
		public void PushDrawParams()
		{
			m_offset_shelter = m_offset;
			m_scale_shelter = m_scale;
			m_is_pushed_params = true;
		}

		/*-------------------------------------------------------------------------
		 그리기위치と스케일をpop
		 1회しかpopできないので注意
		---------------------------------------------------------------------------*/
		public void PopDrawParams()
		{
			if (!m_is_pushed_params) return;

			m_offset = m_offset_shelter;
			m_scale = m_scale_shelter;
			m_is_pushed_params = false;
		}

	}
}
