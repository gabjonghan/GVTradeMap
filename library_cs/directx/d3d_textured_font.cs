/*-------------------------------------------------------------------------

 Direct3D
 텍스쳐화시킨 폰트
 Fontは非常に遅いので텍스쳐に렌더링して캐시する
 텍스쳐はFontが렌더링することで作られる

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing.Imaging;
using System;
using System.Runtime.InteropServices;

using Utility;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace directx {
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class d3d_textured_font : IDisposable {
		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public class textured_font : IDisposable {
			private string m_str;		   // 텍스쳐화된 문자열
			private Vector2 m_size;		 // 문자열 크기
											// 그리기時に사용する
											// 텍스쳐사이즈와 관계없음
			private Vector2 m_texture_size; // 텍스쳐사이즈
			private Texture m_texture;	  // 그리기용텍스쳐
			private int m_ref_count;	// 참조회수

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public string str { get { return str; } }
			public Vector2 size { get { return m_size; } }
			public Texture texture { get { return m_texture; } }
			public Vector2 texture_size { get { return m_texture_size; } }
			public int ref_count {
				get { return m_ref_count; }
				internal set { m_ref_count = value; }
			}

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public textured_font(d3d_device device, string str, Microsoft.DirectX.Direct3D.Font font)
				: this(device, str, font, Format.A8R8G8B8) {
			}
			public textured_font(d3d_device device, string str, Microsoft.DirectX.Direct3D.Font font, Format format) {
				m_ref_count = 0;
				m_str = str;
				try {
					Rectangle rect = font.MeasureString(null, str, DrawTextFormat.None, Color.White);
					m_size = new Vector2(rect.Right, rect.Bottom);

					// 텍스쳐を작성
					// 사이즈は4の배수とする
					int width = (((int)m_size.X) + 3) & ~3;
					int height = (((int)m_size.Y) + 3) & ~3;
					m_texture_size = new Vector2(width, height);
					m_texture = new Texture(device.device, width, height, 1,
														Usage.RenderTarget, format, Pool.Default);
				} catch {
					// 텍스쳐작성실패
					m_size = new Vector2(0, 0);
					m_texture_size = new Vector2(0, 0);
					m_texture = null;
					return;
				}

				// 렌더링 타겟を지정
				Surface depth = device.device.DepthStencilSurface;
				Surface backbuffer = device.device.GetBackBuffer(0, 0, BackBufferType.Mono);

				device.device.DepthStencilSurface = null;	   // zバッファなし
				device.device.SetRenderTarget(0, m_texture.GetSurfaceLevel(0));

				// 화면のクリア
				device.Clear(ClearFlags.Target, Color.FromArgb(0, 0, 0, 0));
				// 렌더링
				device.device.RenderState.ZBufferEnable = false;
				font.DrawText(null, str, new Point(0, 0), Color.White);
				device.device.RenderState.ZBufferEnable = true;

				// 렌더링 타겟を元に戻す
				device.device.DepthStencilSurface = depth;
				device.device.SetRenderTarget(0, backbuffer);

				backbuffer.Dispose();
				depth.Dispose();
			}

			/*-------------------------------------------------------------------------

			---------------------------------------------------------------------------*/
			public void Dispose() {
				if (m_texture != null) m_texture.Dispose();
				m_texture = null;
			}
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private d3d_device m_device;
		private Microsoft.DirectX.Direct3D.Font m_font;
		private Dictionary<string, textured_font> m_map;
		private Format m_texture_format;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public int cash_count { get { return m_map.Count; } }

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public d3d_textured_font(d3d_device device, Microsoft.DirectX.Direct3D.Font font) {
			m_device = device;
			m_font = font;
			m_map = new Dictionary<string, textured_font>();
			m_device.device.DeviceReset += new System.EventHandler(device_reset);

			// A1R5G5B5の렌더링 타겟が사용가능か調べる
			// 사용できない場合はA8R8G8B8を사용する
			// Radeon계は사용가능
			// NVIDIA계は사용不可
			// Intel계はたぶん사용가능
			m_texture_format = Format.A1R5G5B5;
			if (!m_device.CheckDeviceFormat(Usage.RenderTarget, ResourceType.Textures, Format.A1R5G5B5)) {
				// A1R5G5B5の렌더링 타겟が作れない
				m_texture_format = Format.A8R8G8B8;
			}
		}

		/*-------------------------------------------------------------------------
		 デバイスリセット時の初期化
		---------------------------------------------------------------------------*/
		private void device_reset(object sender, System.EventArgs e) {
			Clear();
		}

		/*-------------------------------------------------------------------------
		 캐시を全て삭제
		---------------------------------------------------------------------------*/
		public void Clear() {
			Dictionary<string, textured_font>.ValueCollection list = m_map.Values;

			// 텍스쳐を全て破棄
			foreach (textured_font i in list) {
				i.Dispose();
			}
			m_map.Clear();
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public void Dispose() {
			if (m_map != null) {
				Clear();
			}
			m_map = null;
		}

		/*-------------------------------------------------------------------------
		 textured_fontを得る
		 なければ작성される
		 그리기내で呼ぶこと
		---------------------------------------------------------------------------*/
		private textured_font get_textured_font(string str) {
			textured_font tf = null;
			if (m_map.TryGetValue(str, out tf)) {
				tf.ref_count++;
				return tf;
			}

			// ないので작성함
			tf = new textured_font(m_device, str, m_font, m_texture_format);
			m_map.Add(str, tf);	 // 추가
			return tf;
		}

		/*-------------------------------------------------------------------------
		 textured_fontがあるか調べる
		---------------------------------------------------------------------------*/
		private bool is_created_textured_font(string str) {
			textured_font tf = null;
			if (m_map.TryGetValue(str, out tf)) {
				return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 문자열の그리기時の사이즈を得る
		---------------------------------------------------------------------------*/
		public Rectangle MeasureText(string str, Color color) {
			if (is_created_textured_font(str)) {
				// 캐시されていればそのまま사이즈を返す
				textured_font font = get_textured_font(str);
				Rectangle rect = new Rectangle(0, 0, (int)font.size.X, (int)font.size.Y);
				return rect;
			}
			// 캐시されてなければ調べる
			return m_font.MeasureString(null, str, DrawTextFormat.None, color);
		}

		/*-------------------------------------------------------------------------
		 문자열の그리기
		---------------------------------------------------------------------------*/
		public void DrawText(string str, Vector3 pos, Color color) {
			textured_font font = get_textured_font(str);
			if (font.texture == null) return;

			pos.X = (float)(int)pos.X;
			pos.Y = (float)(int)pos.Y;
			m_device.DrawTexture(font.texture, pos, font.texture_size, color.ToArgb());
		}

		/*-------------------------------------------------------------------------
		 문자열の그리기
		 오른쪽 정렬
		 xで終わるように그리기される
		---------------------------------------------------------------------------*/
		public void DrawTextR(string str, Vector3 pos, Color color) {
			textured_font font = get_textured_font(str);
			if (font.texture == null) return;

			pos.X -= font.size.X;
			pos.X = (float)(int)pos.X;
			pos.Y = (float)(int)pos.Y;
			m_device.DrawTexture(font.texture, pos, font.texture_size, color.ToArgb());
		}

		/*-------------------------------------------------------------------------
		 문자열の그리기
		 가운데 정렬
		 xが真중にくるように그리기される
		---------------------------------------------------------------------------*/
		public void DrawTextC(string str, Vector3 pos, Color color) {
			textured_font font = get_textured_font(str);
			if (font.texture == null) return;

			pos.X -= font.size.X / 2;
			pos.X = (float)(int)pos.X;
			pos.Y = (float)(int)pos.Y;
			m_device.DrawTexture(font.texture, pos, font.texture_size, color.ToArgb());
		}
	}
}
