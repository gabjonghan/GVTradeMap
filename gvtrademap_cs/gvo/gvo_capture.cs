/*-------------------------------------------------------------------------

 대항해시대Online화면캡처
 배の각도は2도간격で確定
 360段階の분해능から180段階の値を得る
 0도は実機の仕様により向けない

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 define
---------------------------------------------------------------------------*/
// 나침반분석デバッグ
//#define	DEBUG_COMPASS

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using directx;

using win32;
using Utility;
using gvo_base;
using System.Diagnostics;


/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class gvo_capture : gvo_capture_base {
		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		class capture_image : ScreenCapture {
			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public capture_image(int size_x, int size_y)
				: base(size_x, size_y) {
			}

			/*-------------------------------------------------------------------------
			 캡처した내용を텍스쳐にする
			 デバッグ용
			---------------------------------------------------------------------------*/
			public Texture CreateTexture(Device device) {
				if (base.Image == null) return null;

				try {
					using (Texture tex = new Texture(device, base.Size.Width, base.Size.Height,
														1, Usage.Dynamic, Format.X8R8G8B8, Pool.SystemMemory)) {
						UInt32[,] buf = (UInt32[,])tex.LockRectangle(typeof(UInt32), 0, LockFlags.Discard, base.Size.Height, base.Size.Width);

						// 텍스쳐내용の生成
						int index = 0;
						for (int y = 0; y < base.Size.Height; y++) {
							for (int x = 0; x < base.Size.Width; x++) {
								buf[y, x] = (UInt32)((Image[index + x * 3 + 2] << 16)
													| (Image[index + x * 3 + 1] << 8)
													| (Image[index + x * 3 + 0] << 0)
													| (255 << 24));
							}
							index += Stride;
						}
						tex.UnlockRectangle(0);

						// 사용できる메모リに전送
						return d3d_utility.CreateTextureFromTexture(device, tex);
					}
				} catch {
					// 실패
					return null;
				}
			}
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private gvt_lib m_lib;

		// 캡처画상チェック용
		private Texture m_debug_texture;
		private Texture m_debug_texture2;

#if DEBUG_COMPASS
		public float				m_factor;
		public float				m_angle_x;
		public float				m_l;
#else
		private float m_factor;
		private float m_angle_x;
		private float m_l;
#endif

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public gvo_capture(gvt_lib lib)
			: base() {
			m_lib = lib;

			m_debug_texture = null;
			m_debug_texture2 = null;

			// 테이블生成용조정値
			m_factor = 36f;
			m_angle_x = 31.5f;
			m_l = 54f;

			// 나침반からの각도산출용테이블을작성함
			//			create_compass_tbl();
			// 조정테이블の작성
			//			create_ajust_compass_tbl();
		}

		/*-------------------------------------------------------------------------
		 screen_captureを작성함
		 screen_captureを継承したものを사용する場合オーバーライドすること
		 コンストラクタ내で呼ばれるので注意
		---------------------------------------------------------------------------*/
		protected override ScreenCapture CreateScreenCapture(int size_x, int size_y) {
			return new capture_image(size_x, size_y);
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public override void Dispose() {
			release_debug_textures();
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private void release_debug_textures() {
			if (m_debug_texture != null) m_debug_texture.Dispose();
			if (m_debug_texture2 != null) m_debug_texture2.Dispose();
			m_debug_texture = null;
			m_debug_texture2 = null;
		}

		/*-------------------------------------------------------------------------
		 화면 캡쳐시 일수나 좌표, 진행방향을 얻음
		---------------------------------------------------------------------------*/
		public override bool CaptureAll() {
			if (!base.CaptureAll()) {
				// 캡처실패
				return false;
			}

			if (m_lib.setting.draw_capture_info) {
				// 분석하는 장소를 저장 (디버그용)
				//debug_compass();
				// 캡처한 이미지를 텍스쳐로 만듬
				// 나침반 이미지를 텍스쳐로 만듬
				release_debug_textures();
				m_debug_texture = ((capture_image)capture2).CreateTexture(m_lib.device.device);
				// 일수와 측량위치 이미지를 텍스쳐로 만듬
				m_debug_texture2 = ((capture_image)capture1).CreateTexture(m_lib.device.device);
			}
			return true;
		}

#if DEBUG_COMPASS
		/*-------------------------------------------------------------------------
		 debug
		 검색する장소を赤にする
		 텍스쳐표시時のテスト용
		---------------------------------------------------------------------------*/
		private void debug_compass()
		{
			byte[]	image	= m_capture2.image;
			int		stride	= m_capture2.stride;

			for(int i=0; i<COMPASS_DIV; i++){
				int		index;

				index	= (stride * (m_compass_pos[i].Y)) + (m_compass_pos[i].X * 3);
				image[index + 0]	= 0;
				image[index + 1]	= 0;
				image[index + 2]	= 255;
			}
		}
#endif

		/*-------------------------------------------------------------------------
		 나침반からの각도산출용테이블을작성함
		 조정용
		---------------------------------------------------------------------------*/
		private void create_compass_tbl() {
			m_compass_pos = new Point[COMPASS_DIV];

			Matrix mat = Matrix.PerspectiveFovRH(Useful.ToRadian(60), 1f, 0.1f, 1000f);
			Matrix mat2 = Matrix.RotationX(Useful.ToRadian(m_angle_x));
			Matrix mat3 = Matrix.Translation(0, 0, -100f);
			mat = mat2 * mat3 * mat;
			float tmp = m_factor;
			float factor;
			{
				Vector3 vec = new Vector3(tmp, 0, 0);
				Vector3 vec2 = Vector3.TransformCoordinate(vec, mat);
				factor = 1f / vec2.X;
			}

			for (int i = 0; i < COMPASS_DIV; i++) {
				float angle = Useful.ToRadian(-90 + ((360f / COMPASS_DIV) * i));
				float x = (float)Math.Cos(angle);
				float y = (float)Math.Sin(angle);

				float l = factor * m_l;
				Vector3 vec = new Vector3(x * tmp, y * tmp, 0);
				Vector3 vec2 = Vector3.TransformCoordinate(vec, mat);

				vec2.X *= l;
				vec2.Y *= l;

				int offset_x = 52 + 4;
				int offset_y = 35 + 4;
				int xx = offset_x + (int)vec2.X;
				int yy = offset_y + (int)vec2.Y;

				if (xx < 0) xx = 0;
				if (xx >= 128) xx = 127;
				if (yy < 0) yy = 0;
				if (yy >= 128) yy = 127;
				m_compass_pos[i] = new Point(xx, yy);
				Debug.WriteLine(String.Format("{0}, {1}		// {2}",
												m_compass_pos[i].X, m_compass_pos[i].Y, i));
			}
		}

		/*-------------------------------------------------------------------------
		 캡처디테일の표시
		---------------------------------------------------------------------------*/
		public void DrawCapturedTexture() {
			if (!m_lib.setting.draw_capture_info) return;

			Vector3 spos = new Vector3(m_lib.device.client_size.X - 128 - 4, 4, 0.002f);
			unchecked {
				m_lib.device.DrawFillRect(spos, new Vector2(64 + 16, (12 * 4) + 2 + 2), (int)0xC0000000);
				//				m_lib.Device.DrawFillRect(spos, new Vector2(64+16, (12*(3+3))+2+2), (int)0xC0000000);
			}
			spos.X += 2;
			spos.Y += 2;
			m_lib.device.systemfont.locate = spos;
			m_lib.device.systemfont.Puts(String.Format("index1={0}\nindex2={1}\n",
											m_1st_com_index, m_com_index), Color.White);
			m_lib.device.systemfont.Puts(String.Format("index3={0}\nquadrant={1}\n",
											m_com_index2, m_an_index), Color.White);
			//			m_lib.Device.systemfont.Puts(String.Format("index2={0}\nquadrant={1}\nangle2={2}\n",
			//											m_com_index2, m_an_index2, m_angle2), Color.White);


			Vector3 pos = new Vector3(m_lib.device.client_size.X - 4, 64, 0.001f);

			// 측량と항해일수
			draw_debug_texture(m_debug_texture2, pos, 1f);
			// 나침반
			pos.Y += 8f + d3d_utility.GetTextureSize(m_debug_texture2).Y;
			draw_debug_texture(m_debug_texture, pos, 1f);
		}
		private void draw_debug_texture(Texture tex, Vector3 pos, float scale) {
			if (tex == null) return;

			Vector2 size = d3d_utility.GetTextureSize(tex);
			pos.X -= size.X;
			m_lib.device.DrawTexture(tex, pos, size * scale);
		}
	}
}
