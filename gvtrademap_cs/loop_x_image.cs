/*-------------------------------------------------------------------------

 x 방향으로 루프하는 이미지
 이미지에 합성해 버리는 모드에 대응
 이미지는 렌더링 타겟으로 확보된다

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
		private d3d_device m_device;				// 디바이스
		private Texture m_texture;			  // 그릴 텍스쳐
		private Texture m_texture_sysmem;	   // 시스템 메모리에 확보한 텍스쳐
		private Vector2 m_offset;			   // 잘라내기 오프셋
		private Size m_size;					// 잘라내기 크기
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
		 image 에서 rect 크기만큼 잘라내 텍스쳐를 작성
		 렌더링 타겟으로 작성하기 때문에, 크기를 2 의 거듭제곱으로 조정한다
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

			// 텍스쳐 크기를 2 의 거듭제곱으로 만든다
			m_texture_size = d3d_utility.TextureSizePow2(m_size);
			//			m_texture_size	= m_size;

			// 텍스쳐 작성과 잠금
			if (m_texture_sysmem != null) m_texture_sysmem.Dispose();
			m_texture_sysmem = new Texture(m_device.device, m_texture_size.Width, m_texture_size.Height,
												1, Usage.Dynamic, Format.R5G6B5, Pool.SystemMemory);
			UInt16[,] buf = (UInt16[,])m_texture_sysmem.LockRectangle(typeof(UInt16), 0, LockFlags.Discard, m_texture_size.Height, m_texture_size.Width);

			// 텍스쳐 내용 생성
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
				// 크기 조정을 한 경우에는 1 도트 여분으로 복사한다
				// 바이리니어 필터일 때의 참조 위치 대책
				if (m_texture_size.Width > m_size.Width)
				{
					buf[y, m_size.Width] = color;
				}
				index += stride;
			}
			m_texture_sysmem.UnlockRectangle(0);

			if (m_texture != null) m_texture.Dispose();

			// Managed 텍스쳐로 전송한다
			//			m_texture	= d3d_utility.CreateTextureFromTexture(m_device.Device, Texture);
			// 렌더링 타겟 텍스쳐로 전송한다
			RefreshTexture();
		}

		/*-------------------------------------------------------------------------
		 image 에서 rect 크기만큼 잘라내 텍스쳐를 작성
		 마스크용
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

			// 텍스쳐 작성과 잠금
			using (Texture texture = new Texture(m_device.device, m_size.Width, m_size.Height,
												1, Usage.Dynamic, Format.A1R5G5B5, Pool.SystemMemory))
			{
				UInt16[,] buf = (UInt16[,])texture.LockRectangle(typeof(UInt16), 0, LockFlags.Discard, m_size.Height, m_size.Width);

				// 텍스쳐 내용 생성
				int index = (stride * rect.Y) + (rect.X * sizeof(UInt16));
				for (int y = 0; y < m_size.Height; y++)
				{
					for (int x = 0; x < m_size.Width; x++)
					{
						UInt16 color = (UInt16)((image[index + x * sizeof(UInt16) + 1] << 8)
												| (image[index + x * sizeof(UInt16) + 0] << 0));

						// jpg 이므로 어느 정도 오차를 허용한다
						if ((((color >> 0) & 0x1f) < 0x10)
							|| (((color >> 5) & 0x3f) < 0x30)
							|| (((color >> 5 + 6) & 0x1f) < 0x10))
						{
							color = 0x7fff;	 // 투명 처리
						}

						buf[y, x] = color;
					}
					index += stride;
				}
				texture.UnlockRectangle(0);

				// Managed 텍스쳐로 전송한다
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
		 리프레시
		 디바이스 로스트 대책
		---------------------------------------------------------------------------*/
		public void RefreshTexture()
		{
			// 특수한 사용은 하지 않는다
			if (m_texture_sysmem == null) return;

			// 이미 있으면 해제한다
			if (m_texture != null) m_texture.Dispose();

			// 렌더링 타겟으로 사용할 수 있는 텍스쳐를 작성해 전송한다
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

			// 오프셋을 고려
			pos.X *= scale;
			pos.Y *= scale;
			pos.X += offset.X;
			pos.Y += offset.Y;

			// 그리기 범위 확인
			// debug
			// 범위 안의 끝부분에서 컬링한다
			//			if(pos.X + ImageSize.X < 0+32)		return;
			//			if(pos.Y + ImageSize.Y < 0+32)		return;
			//			if(pos.X > client_size.X-32)	return;
			//			if(pos.Y > client_size.Y-32)	return;
			// 통상 컬링
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
		private const int HEIGHT_MARGIN = 200;	  // 상하 스크롤 마진
		private const float SCALE_MIN = 0.25f;	   // 축소한계
		private const float SCALE_MAX = 2;		// 확대한계

		private d3d_device m_device;					// 디바이스

		private Vector2 m_image_size;			   // 이미지 전체의 크기
		private List<TextureUnit> m_textures;				   // 텍스쳐목록

		// 그리기 위치와 스케일
		private Vector2 m_offset;				   // 오프셋
		private float m_scale;				  // 확대 축소율

		// 그리기 위치와 스케일 대피용
		private Vector2 m_offset_shelter;		   // 오프셋
		private float m_scale_shelter;		  // 확대 축소율
		private bool m_is_pushed_params;			// 

		// 스레드로 로드할 경우의 진행 상태
		private int m_load_current;
		private int m_load_max;
		private string m_load_str;

		// 그리기 지원 델리게이트
		// 그릴 때의 X 방향 루프에 대응해 오프셋을 해결한다
		// 이 델리게이트가 몇 번 호출되는지는 클라이언트 크기와 이미지 크기에 따라 다르다
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
		 오프셋 가산
		---------------------------------------------------------------------------*/
		public void AddOffset(Vector2 add_offset)
		{
			// 스케일이 작을수록 이동량이 커진다
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
			// 범위 확인
			if (scale < SCALE_MIN) scale = SCALE_MIN;
			if (scale > SCALE_MAX) scale = SCALE_MAX;

			if ((scale > 1 - 1e-6) && (scale < 1 + 1e-6))
			{
				scale = 1;
			}

			if (is_center_mouse)
			{
				// 지정된 위치가 중심이 되도록 오프셋을 변경한다
				Vector2 pos = new Vector2(center_pos.X, center_pos.Y);

				// 중심 위치의 차분을 얻는다
				Vector2 old_offset = pos * (1 / m_scale);
				Vector2 new_offset = pos * (1 / scale);

				// 오프셋을 수정한다
				m_offset -= old_offset - new_offset;

				m_scale = scale;
			}
			else
			{
				// 오프셋을 변경하지 않는다
				m_scale = scale;
			}
		}

		/*-------------------------------------------------------------------------
		 지정된 좌표가 중심이 되도록 오프셋을 변경한다
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
		 디바이스 로스트
		 플래그만 세운다
		---------------------------------------------------------------------------*/
		private void device_reset(object sender, System.EventArgs e)
		{
			m_device_lost = true;
		}

		/*-------------------------------------------------------------------------
		 이미지 구축
		 진척 상태 초기화
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
			// 텍스쳐 목록 해제
			Dispose();

			InitializeCreateImage();

			try
			{
				// 이미지 읽기
				m_load_str = file_name;
				Bitmap bitmap = ResizeImage(new Bitmap(file_name), (int)def.GAME_WIDTH/4, (int)def.GAME_HEIGHT/4);
				Vector2 size = new Vector2(bitmap.Width, bitmap.Height);
				m_image_size = size;

				// 로드한 장수를 센다
				int count_x = bitmap.Width / TEXSIZE_STEP;
				if ((bitmap.Width % TEXSIZE_STEP) != 0) count_x++;
				int count_y = bitmap.Height / TEXSIZE_STEP;
				if ((bitmap.Height % TEXSIZE_STEP) != 0) count_y++;
				m_load_max = count_x * count_y;

				// 잠그고 이미지를 꺼낸다
				// R5G6B5 로 변환해 둔다
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

				// 원본은 해제해 둔다
				bitmap.Dispose();
				bitmap = null;

				// 분할해서 텍스쳐를 구축한다
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
				// 무언가 실패
				return false;
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 이미지와 합성
		 Begin() 을 호출한 직후에 호출할 것
		 must_marge 가 true 일 때는 반드시 합성한다
		 디바이스 로스트 시에는 반드시 합성한다
		 합성하지 않을 때는 handler 에 null 을 지정할 수 있다

		 디바이스 로스트 시의 처리를 포함하므로, 반드시 호출해야 한다
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
			// 합성 대상이 없으면 돌아간다
			if (handler == null)
			{
				m_device_lost = false;
				// 합성에 걸린 시간
				MargeImageMS = d.GetSectionTimeMilliseconds();
				return;
			}

			// 합성
			// 렌더링 타겟을 지정
			Surface depth = m_device.device.DepthStencilSurface;
			Surface backbuffer = m_device.device.GetBackBuffer(0, 0, BackBufferType.Mono);
			m_device.device.DepthStencilSurface = null;	 // z 버퍼 없음

			try
			{
				foreach (TextureUnit tex in m_textures)
				{
					// 렌더링 타겟을 설정

					if (tex.Texture == null) continue;	  // 텍스쳐를 만들지 못했다

					//					Surface	a	= ((Texture)null).GetSurfaceLevel(0);
					m_device.device.SetRenderTarget(0, tex.Texture.GetSurfaceLevel(0));
					m_device.UpdateClientSize();

					// 그리기 위치와 스케일을 대피
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
				// 보험
				PopDrawParams();
			}

			// 렌더링 타겟을 원래대로 되돌린다
			m_device.device.DepthStencilSurface = depth;
			m_device.device.SetRenderTarget(0, backbuffer);
			m_device.UpdateClientSize();

			backbuffer.Dispose();
			depth.Dispose();

			m_device_lost = false;
			// 합성에 걸린 시간
			MargeImageMS = d.GetSectionTimeMilliseconds();
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		public void Draw()
		{
			// 읽기 확인
			if (m_image_size.X <= 0) return;
			if (m_image_size.Y <= 0) return;

			// Y 방향 클램프 없음
			// 자유롭게 스크롤한다

			// 그리기
			m_device.device.RenderState.ZBufferEnable = false;
			EnumDrawCallBack(new DrawHandler(draw_proc), 0);
			m_device.device.RenderState.ZBufferEnable = true;
		}

		/*-------------------------------------------------------------------------
		 그리기 시작 위치를 구한다
		 화면 밖에 얼마나 여유를 둘지 지정할 수 있다
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
		 마우스 좌표와 표시 오프셋을 해결한 좌표를 얻는다
		 마우스 좌표는 클라이언트 사각형 기준의 상대 좌표여야 한다
		---------------------------------------------------------------------------*/
		public Point MousePos2GlobalPos(Point mouse_pos)
		{
			return transform.ToPoint(MousePos2GlobalPos(transform.ToVector2(mouse_pos)));
		}
		public Vector2 MousePos2GlobalPos(Vector2 mouse_pos)
		{
			// 스케일의 역수
			mouse_pos *= 1 / ImageScale;

			Vector2 pos = mouse_pos - m_offset;

			// 이미지 크기에 들어가도록 조정
			while (pos.X >= m_image_size.X) pos.X -= m_image_size.X;
			return pos;
		}

		/*-------------------------------------------------------------------------
		 그리기 오프셋을 얻는다
		 그리기 오프셋은 정수로 반올림되어 있다
		---------------------------------------------------------------------------*/
		public Vector2 GetDrawOffset()
		{
			Vector2 _offset = OffsetPosition;
			_offset.X = (float)(int)_offset.X;
			_offset.Y = (float)(int)_offset.Y;
			return _offset;
		}

		/*-------------------------------------------------------------------------
		 전역 지도 좌표에서 그리기용 좌표로 변환한다
		---------------------------------------------------------------------------*/
		public Vector2 GlobalPos2LocalPos(Vector2 global_pos)
		{
			global_pos += GetDrawOffset();
			global_pos *= ImageScale;
			return global_pos;
		}

		/*-------------------------------------------------------------------------
		 오프셋 좌표가 클라이언트 안에 들어가도록 조정한다
		 X 방향만
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
		 전역 지도 좌표에서 그리기용 좌표로 변환한다
		 오프셋 지정 버전
		---------------------------------------------------------------------------*/
		public Vector2 GlobalPos2LocalPos(Vector2 global_pos, Vector2 _offset)
		{
			global_pos += _offset;
			global_pos *= ImageScale;
			return global_pos;
		}

		/*-------------------------------------------------------------------------
		 그리기 지원

		 최종적인 그리기 좌표는
		 GlobalPos2LocalPos();
		 으로 얻을 것
		---------------------------------------------------------------------------*/
		public void EnumDrawCallBack(DrawHandler handler, float outside_offset_x)
		{
			if (handler == null) return;

			ajust_draw_start_offset_x(outside_offset_x);

			Vector2 offset = GetDrawOffset();
			do
			{
				// 그리기 콜백 호출
				handler(offset, this);

				// 오프셋 조정
				offset.X += ImageSize.X;
			} while (offset.X < (((1f / ImageScale) * m_device.client_size.X) + outside_offset_x));
		}

		/*-------------------------------------------------------------------------
		 그리기 위치와 스케일을 push
		 한 번만 push 할 수 있으므로 주의
		---------------------------------------------------------------------------*/
		public void PushDrawParams()
		{
			m_offset_shelter = m_offset;
			m_scale_shelter = m_scale;
			m_is_pushed_params = true;
		}

		/*-------------------------------------------------------------------------
		 그리기 위치와 스케일을 pop
		 한 번만 pop 할 수 있으므로 주의
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
