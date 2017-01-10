/*-------------------------------------------------------------------------

 이름 관련 텍스쳐 작업

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;

using directx;
using System;
using System.Reflection;
using System.IO;
using System.Drawing.Text;
using System.Collections.Generic;
using Utility;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	public class nameTexture : IDisposable {
		private const int HEIGHT = 12;
		private const int DEF_WIDTH = 8;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private d3d_device m_d3d_device;        // 

		private FontDescription m_font_desc;

		private Microsoft.DirectX.Direct3D.Font m_font;

		private Dictionary<string, d3d_sprite_rects.rect> m_nameRect;

		private GvoWorldInfo m_world;

		private Sprite m_sprite;

		private Format m_texture_format;

		private Texture m_cityTexture;
		private Texture m_elseTexture;

		private Vector2 m_textureSize;
		private Vector2 m_offset;

		private bool m_isInitialized;

		private const int MAX_NAME_WIDTH = 170;
		private const int MAX_NAME_HEIGHT = 16;
		private const int MAX_NAME_LINE = 64;
		
		public Texture cityTexture { get { return m_cityTexture; } }
		public Texture elseTexture { get { return m_elseTexture; } }
		
		/*-------------------------------------------------------------------------
		 初期化
		---------------------------------------------------------------------------*/
		public nameTexture(d3d_device device) {
			if (device == null) {
				return;
			}

			PrivateFontCollection privateFonts = new PrivateFontCollection();
			//privateFonts.AddFontFile("font/NanumBarunpenB.ttf");
			privateFonts.AddFontFile("font/NanumGothicExtraBold.ttf");
						
			m_font_desc = new FontDescription();
			m_font_desc.FaceName = privateFonts.Families[0].Name;
			//m_font_desc.FaceName = "Dotum";
			m_font_desc.CharSet = CharacterSet.Hangul;
			m_font_desc.Height = 14;
			m_font_desc.PitchAndFamily = PitchAndFamily.DefaultPitch;
			m_font_desc.Quality = FontQuality.Proof;
			m_font_desc.OutputPrecision = Precision.Default;
			m_font_desc.Weight = FontWeight.ExtraBold;
			
			m_nameRect = new Dictionary<string, d3d_sprite_rects.rect>(512);
			
			m_d3d_device = device;

			m_d3d_device.device.DeviceReset += new System.EventHandler(device_reset);
			
			m_font = new Microsoft.DirectX.Direct3D.Font(m_d3d_device.device, m_font_desc);
			
			m_sprite = new Sprite(m_d3d_device.device);

			m_textureSize = new Vector2(1024, 1024);
			m_offset = new Vector2(0, 0);

			m_isInitialized = false;
		}

		public void init(GvoWorldInfo world) {
			if (m_isInitialized) {
				return;
			} else {
				m_isInitialized = true;
			}

			int index = 0;
			int xIndex = 0;
			int yIndex = 0;
			int xPos = 0;
			int yPos = 0;

			m_world = world;
			
			m_texture_format = Format.A1R5G5B5;
			if (!m_d3d_device.CheckDeviceFormat(Usage.RenderTarget, ResourceType.Textures, Format.A1R5G5B5)) {
				// A1R5G5B5の렌더링 타겟が作れない
				m_texture_format = Format.A8R8G8B8;
			}

			m_cityTexture = new Texture(m_d3d_device.device, 1024, 1024, 1, Usage.RenderTarget, m_texture_format, Pool.Default);
			m_elseTexture = new Texture(m_d3d_device.device, 1024, 1024, 1, Usage.RenderTarget, m_texture_format, Pool.Default);
			
			// 렌더링 타겟을 지정
			Surface depth = m_d3d_device.device.DepthStencilSurface;
			Surface backBuffer = m_d3d_device.device.GetBackBuffer(0, 0, BackBufferType.Mono);

			if (depth != null) {
				m_d3d_device.device.DepthStencilSurface = null;      // zバッファ없음
			}
			m_d3d_device.device.SetRenderTarget(0, m_cityTexture.GetSurfaceLevel(0));

			// 화면のクリア
			m_d3d_device.Clear(ClearFlags.Target, Color.FromArgb(0, 0, 0, 0));
			// 렌더링
			m_d3d_device.device.RenderState.ZBufferEnable = false;
			
			drawWorlds(ref index, ref xIndex, ref yIndex, ref xPos, ref yPos, m_world.Cities);

			m_d3d_device.device.SetRenderTarget(0, m_elseTexture.GetSurfaceLevel(0));

			// 화면のクリア
			m_d3d_device.Clear(ClearFlags.Target, Color.FromArgb(0, 0, 0, 0));
			// 렌더링
			
			xIndex = 0;
			yIndex = 0;
			xPos = 0;
			yPos = 0;

			drawWorlds(ref index, ref xIndex, ref yIndex, ref xPos, ref yPos, m_world.Shores);

			drawWorlds(ref index, ref xIndex, ref yIndex, ref xPos, ref yPos, m_world.NoSeas);

			drawWorlds(ref index, ref xIndex, ref yIndex, ref xPos, ref yPos, m_world.Seas);

			m_d3d_device.device.RenderState.ZBufferEnable = true;

			// 렌더링 타겟を元に戻す
			if (depth != null) {
				m_d3d_device.device.DepthStencilSurface = depth;
			}
			m_d3d_device.device.SetRenderTarget(0, backBuffer);

			backBuffer.Dispose();
			depth.Dispose();

			/*
			GraphicsStream stream = TextureLoader.SaveToStream(ImageFileFormat.Bmp, m_cityTexture);
			Bitmap tex_image = new Bitmap(stream);
			tex_image.Save("map/cityTexture.bmp");

			tex_image.Dispose();

			stream = TextureLoader.SaveToStream(ImageFileFormat.Bmp, m_elseTexture);
			tex_image = new Bitmap(stream);
			tex_image.Save("map/elseTexture.bmp");

			tex_image.Dispose();
			*/
		}

		private void drawWorlds(ref int index, ref int xIndex, ref int yIndex, ref int xPos, ref int yPos, hittest_list list) {
			foreach (GvoWorldInfo.Info i in list) {
				String name = i.Name;				
				if (i.InfoType == GvoWorldInfo.InfoType.GuildCity && i.CityInfo != null && i.CityInfo.HasNameImage == false) {
					continue;
				} else if (i.InfoType == GvoWorldInfo.InfoType.GuildCity && i.CityInfo != null) {
					name = i.Name.Substring(0, i.Name.Length - 1);
				}

				Rectangle rect = m_font.MeasureString(m_sprite, name, DrawTextFormat.Left, Color.White);

				xPos = (xIndex) * MAX_NAME_WIDTH + 1;
				yPos = (yIndex) * 16 + 1;

				Color color = Color.Black;
				if (i.InfoType == GvoWorldInfo.InfoType.Sea) {
					color = Color.RoyalBlue;
				} else if (i.InfoType == GvoWorldInfo.InfoType.Shore) {
					color = Color.SeaGreen;
				} else if (i.InfoType == GvoWorldInfo.InfoType.Shore2) {
					color = Color.Navy;
				} else if (i.InfoType == GvoWorldInfo.InfoType.OutsideCity) {
					color = Color.Chocolate;
				} else if (i.InfoType == GvoWorldInfo.InfoType.PF) {
					color = Color.SaddleBrown;
				}

				m_font.DrawText(null, name, xPos - 1, yPos - 1, Color.FromArgb(190, Color.White));
				m_font.DrawText(null, name, xPos - 1, yPos + 1, Color.FromArgb(190, Color.White));
				m_font.DrawText(null, name, xPos + 1, yPos - 1, Color.FromArgb(190, Color.White));
				m_font.DrawText(null, name, xPos + 1, yPos + 1, Color.FromArgb(190, Color.White));
				m_font.DrawText(null, name, xPos - 1, yPos, Color.FromArgb(230, Color.White));
				m_font.DrawText(null, name, xPos + 1, yPos, Color.FromArgb(230, Color.White));
				m_font.DrawText(null, name, xPos, yPos - 1, Color.FromArgb(230, Color.White));
				m_font.DrawText(null, name, xPos, yPos + 1, Color.FromArgb(230, Color.White));
				m_font.DrawText(null, name, xPos + 0, yPos + 0, color);
				
				rect.X = xPos - 1;
				rect.Y = yPos - 1;

				rect.Width += 3;
				rect.Height += 2;

				Vector2 leftTop = new Vector2(rect.X, rect.Y);
				Vector2 size = new Vector2(rect.Width, rect.Height);
				
				if (i.NameRect != null) {
					if (m_nameRect.ContainsKey(i.Name)) {
						//i.NameRect = new d3d_sprite_rects.rect(m_textureSize, m_offset, rect);
						//m_nameRect.Remove(i.Name);
						//m_nameRect.Add(i.Name, i.NameRect);
					} else {
						i.NameRect = new d3d_sprite_rects.rect(m_textureSize, m_offset, rect);
						m_nameRect.Add(i.Name, i.NameRect);
					}
				} else {
					i.NameRect = new d3d_sprite_rects.rect(m_textureSize, m_offset, rect);
					m_nameRect.Add(i.Name, i.NameRect);
				}

				yIndex++;
				if (yIndex >= MAX_NAME_LINE) {
					xIndex++;
					yIndex = 0;
				}
				index++;
			}
		}

		public d3d_sprite_rects.rect getRect(string name) {
			d3d_sprite_rects.rect rectValue;

			if (m_nameRect.TryGetValue(name, out rectValue)) {
				return rectValue;
			} else {
				return null;
			}
		}

		public void device_reset(object sender, System.EventArgs e) {
			if (m_nameRect != null) {
				m_nameRect.Clear();
			}
			if (m_cityTexture != null) m_cityTexture.Dispose();
			if (m_elseTexture != null) m_elseTexture.Dispose();
			m_isInitialized = false;
		}

		public void Dispose() {
			if (m_font != null) m_font.Dispose();
			if (m_sprite != null) m_sprite.Dispose();
			if (m_nameRect != null) {
				m_nameRect.Clear();
			}
			if (m_cityTexture != null) m_cityTexture.Dispose();
			if (m_elseTexture != null) m_elseTexture.Dispose();
			m_font = null;
			m_sprite = null;
		}
	}
}
