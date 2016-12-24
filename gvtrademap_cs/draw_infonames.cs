/*-------------------------------------------------------------------------

 도시とか해역とか상륙지점の명前그리기
 쉐이더が使えれば쉐이더を使う
 통상の그리기は중いのでできれば쉐이더を使いたい
 쉐이더はこの그리기용に최적화されている

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Drawing;

using directx;
using gvo_base;
using Utility;
using Microsoft.DirectX;
using System.Runtime.InteropServices;
using Microsoft.DirectX.Direct3D;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	class draw_infonames : IDisposable {
		// 季節じゃないほうの풍向の색
		// 少し薄くなるような색
		private const int WIND_ANGLE_COLOR2 = (96 << (8 * 3)) | 0x00ffffff;

		private gvt_lib m_lib;
		private GvoWorldInfo m_world;

		private bool m_is_create_buffers;   // 쉐이더용 버퍼를 만들면 true
		private bool m_is_error;
		private VertexDeclaration m_decl;

		private d3d_writable_vb_with_index m_icons1_vb;		 // 대 아이콘용
		private d3d_writable_vb_with_index m_city_names1_vb;		// 도시명과 상륙지점명
		private d3d_writable_vb_with_index m_icons2_vb;		 // 소 아이콘용
		private d3d_writable_vb_with_index m_city_names2_vb;		// 도시명과 상륙지점명

		private d3d_writable_vb_with_index m_sea_names1_vb;	 // 여름용 해역명
		private d3d_writable_vb_with_index m_sea_names2_vb;	 // 겨울용 해역명

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public draw_infonames(gvt_lib lib, GvoWorldInfo world) {
			m_lib = lib;
			m_world = world;

			// 쉐이더사용時
			m_is_create_buffers = false;
			m_is_error = false;
			m_icons1_vb = null;
			m_city_names1_vb = null;
			m_icons2_vb = null;
			m_city_names2_vb = null;

			m_sea_names1_vb = null;
			m_sea_names2_vb = null;
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public void Dispose() {
			if (m_decl != null) m_decl.Dispose();
			if (m_icons1_vb != null) m_icons1_vb.Dispose();
			if (m_city_names1_vb != null) m_city_names1_vb.Dispose();
			if (m_icons2_vb != null) m_icons2_vb.Dispose();
			if (m_city_names2_vb != null) m_city_names2_vb.Dispose();
			if (m_sea_names1_vb != null) m_sea_names1_vb.Dispose();
			if (m_sea_names2_vb != null) m_sea_names2_vb.Dispose();

			m_decl = null;
			m_icons1_vb = null;
			m_city_names1_vb = null;
			m_icons2_vb = null;
			m_city_names2_vb = null;
			m_sea_names1_vb = null;
			m_sea_names2_vb = null;
		}

		/*-------------------------------------------------------------------------
		 도시명그리기
		 도시아이콘含む
		 쉐이더사용時は전용の構造を구축する
		---------------------------------------------------------------------------*/
		public void DrawCityName() {
			create_buffers();

			if (m_lib.device.sprites.effect != null) {
				// 쉐이더사용
				m_lib.device.device.RenderState.ZBufferEnable = false;
				m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_cityname_shader_proc), 64);
				m_lib.device.device.RenderState.ZBufferEnable = true;
			} else {
				// 통상
				if (m_lib.loop_image.ImageScale <= 0.6f) return;

				m_lib.device.device.RenderState.ZBufferEnable = false;
				m_lib.device.sprites.BeginDrawSprites(m_lib.infonameimage.texture);
				m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_cityname_proc), 64);
				m_lib.device.sprites.EndDrawSprites();
				m_lib.device.device.RenderState.ZBufferEnable = true;
			}
		}

		/*-------------------------------------------------------------------------
		 도시명그리기
		 상륙지점も含む
		---------------------------------------------------------------------------*/
		private void draw_cityname_proc(Vector2 offset, LoopXImage image) {
			if (m_lib.setting.map_icon == MapIcon.Big) {
				// 대 아이콘
				foreach (GvoWorldInfo.Info i in m_world.NoSeas) {
					Vector2 p = image.GlobalPos2LocalPos(transform.ToVector2(i.position), offset);
					if (i.IconRect != null) {
						m_lib.device.sprites.AddDrawSprites(new Vector3(p.X, p.Y, 0.3f), i.IconRect);
					}

					if (m_lib.setting.map_draw_names == MapDrawNames.Hide) continue;
					if (i.NameRect == null) continue;
					p.X += i.StringOffset1.X;
					p.Y += i.StringOffset1.Y;
					m_lib.device.sprites.AddDrawSprites(new Vector3(p.X, p.Y, 0.3f), i.NameRect);
				}
			} else {
				// 소 아이콘
				foreach (GvoWorldInfo.Info i in m_world.NoSeas) {
					Vector2 p = image.GlobalPos2LocalPos(transform.ToVector2(i.position), offset);
					if (i.SmallIconRect != null) {
						m_lib.device.sprites.AddDrawSprites(new Vector3(p.X, p.Y, 0.3f), i.SmallIconRect);
					}

					if (m_lib.setting.map_draw_names == MapDrawNames.Hide) continue;
					if (i.NameRect == null) continue;
					p.X += i.StringOffset2.X;
					p.Y += i.StringOffset2.Y;
					m_lib.device.sprites.AddDrawSprites(new Vector3(p.X, p.Y, 0.3f), i.NameRect);
				}
			}
		}

		/*-------------------------------------------------------------------------
		 도시명그리기
		 상륙지점も含む
		 쉐이더사용
		---------------------------------------------------------------------------*/
		private void draw_cityname_shader_proc(Vector2 offset, LoopXImage image) {
			set_shader_params(m_lib.infonameimage.texture, offset, image.ImageScale);

			Effect effect = m_lib.device.sprites.effect;
			if (effect == null) return;

			effect.Begin(0);
			effect.BeginPass(0);

			m_lib.device.device.VertexDeclaration = m_decl;
			if (m_lib.setting.map_icon == MapIcon.Big) {
				// 대きい아이콘
				draw_buffer(m_icons1_vb, m_world.NoSeas.Count);
				if (m_lib.setting.map_draw_names == MapDrawNames.Draw) {
					// 도시명
					draw_buffer(m_city_names1_vb, m_world.NoSeas.Count);
				}
			} else {
				// 소さい아이콘
				draw_buffer(m_icons2_vb, m_world.NoSeas.Count);
				if (m_lib.setting.map_draw_names == MapDrawNames.Draw) {
					// 도시명
					draw_buffer(m_city_names2_vb, m_world.NoSeas.Count);
				}
			}
			effect.EndPass();
			effect.End();
		}

		/*-------------------------------------------------------------------------
		 해역명그리기
		 풍향含む
		 쉐이더사용時は전용の構造を구축する
		---------------------------------------------------------------------------*/
		public void DrawSeaName() {
			create_buffers();

			if (m_lib.device.sprites.effect != null) {
				// 쉐이더사용
				m_lib.device.device.RenderState.ZBufferEnable = false;
				m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_seaname_shader_proc), 64);
				m_lib.device.device.RenderState.ZBufferEnable = true;
			} else {
				// 통상
				if (m_lib.loop_image.ImageScale <= 0.6f) return;

				m_lib.device.device.RenderState.ZBufferEnable = false;
				m_lib.device.sprites.BeginDrawSprites(m_lib.seainfonameimage.texture);
				m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_seaname_proc), 64);
				m_lib.device.sprites.EndDrawSprites();
				m_lib.device.device.RenderState.ZBufferEnable = true;
			}
		}

		/*-------------------------------------------------------------------------
		 해역명그리기
		---------------------------------------------------------------------------*/
		private void draw_seaname_proc(Vector2 offset, LoopXImage image) {
			d3d_sprite_rects.rect _rect = m_lib.seainfonameimage.GetWindArrowIcon();
			int color1 = (m_world.Season.now_season == gvo_season.season.summer) ? -1 : (96 << (8 * 3)) | 0x00ffffff;
			int color2 = (m_world.Season.now_season == gvo_season.season.winter) ? -1 : (96 << (8 * 3)) | 0x00ffffff;

			int index = 0;
			foreach (GvoWorldInfo.Info i in m_world.Seas) {
				Vector2 p = image.GlobalPos2LocalPos(transform.ToVector2(i.position), offset);
				// 해역명
				m_lib.device.sprites.AddDrawSprites(new Vector3(p.X - 6, p.Y, 0.3f), m_lib.seainfonameimage.GetRect(index));
				index++;

				// 풍향그리기
				if (i.SeaInfo == null) continue;
				p.X += i.SeaInfo.WindPos.X - i.position.X;
				p.Y += i.SeaInfo.WindPos.Y - i.position.Y;
				m_lib.device.sprites.AddDrawSpritesNC(new Vector3(p.X - 6, p.Y, 0.3f), _rect, i.SeaInfo.SummerAngle, color1);
				m_lib.device.sprites.AddDrawSpritesNC(new Vector3(p.X + 6, p.Y, 0.3f), _rect, i.SeaInfo.WinterAngle, color2);
			}
		}

		/*-------------------------------------------------------------------------
		 해역명그리기
		 쉐이더사용
		---------------------------------------------------------------------------*/
		private void draw_seaname_shader_proc(Vector2 offset, LoopXImage image) {
			set_shader_params(m_lib.seainfonameimage.texture, offset, image.ImageScale);

			Effect effect = m_lib.device.sprites.effect;
			if (effect == null) return;

			effect.Begin(0);
			effect.BeginPass(0);

			m_lib.device.device.VertexDeclaration = m_decl;	 // 頂点の디테일
			if (m_world.Season.now_season == gvo_season.season.summer) {
				// 夏
				draw_buffer(m_sea_names1_vb, m_world.Seas.Count * 3);
			} else {
				// 冬
				draw_buffer(m_sea_names2_vb, m_world.Seas.Count * 3);
			}

			effect.EndPass();
			effect.End();
		}

		/*-------------------------------------------------------------------------
		 쉐이더에 대한 파라미터 설정
		---------------------------------------------------------------------------*/
		private void set_shader_params(Texture tex, Vector2 offset, float mapscale) {
			Effect effect = m_lib.device.sprites.effect;
			if (effect == null) return;

			try {
				float scale = mapscale;
				if (scale >= 0.7f) scale = 1f;
				if (scale < 0.5f) scale = 0.5f;
				scale *= this.GetDpiScaleRatio();

				float[] vector = { m_lib.device.client_size.X, m_lib.device.client_size.Y };
				float[] _offset = { offset.X, offset.Y };
				float[] gscale = { scale, scale };
				effect.SetValue("ViewportSize", vector);
				effect.SetValue("Texture", tex);
				effect.SetValue("MapOffset", _offset);
				effect.SetValue("MapScale", mapscale);
				effect.SetValue("GlobalScale", gscale);

				// technique
				effect.Technique = "SpriteWithGlobalParams";		// グローバルパラメータと拡縮と회전付き
			} catch {
				// 쉐이더ーの사용をやめる
				m_lib.device.sprites.DisposeEffect();
			}
		}

		/*-------------------------------------------------------------------------
		 バッファを구축する
		 구축されていればなにもしない
		---------------------------------------------------------------------------*/
		private void create_buffers() {
			if (!m_lib.device.is_use_ve1_1_ps1_1) return;	   // 쉐이더を使わない
			if (m_is_create_buffers) return;		// 작성されている
			if (m_is_error) return;	 // 오류

			try {
				m_decl = new VertexDeclaration(m_lib.device.device, sprite_vertex.VertexElements);

				// 도시명と상륙지점명のバッファ작성
				create_city_buffer();
				create_sea_buffer();

				m_is_create_buffers = true;
				m_is_error = false;
			} catch {
				// 실패
				m_is_create_buffers = false;
				m_icons1_vb = null;
				m_city_names1_vb = null;
				m_icons2_vb = null;
				m_city_names2_vb = null;

				m_sea_names1_vb = null;
				m_sea_names2_vb = null;
				m_is_error = true;
			}
		}

		/*-------------------------------------------------------------------------
		 バーテクスバッファを작성
		 1도작성したら변경しないため, スワップバッファを持たない
		---------------------------------------------------------------------------*/
		private d3d_writable_vb_with_index create_buffer(int sprite_count) {
			return new d3d_writable_vb_with_index(m_lib.device.device,
													typeof(sprite_vertex),
													sprite_count * 4,
													1);
		}

		/*-------------------------------------------------------------------------
		 도시명と상륙지점명のバッファ작성 
		---------------------------------------------------------------------------*/
		private void create_city_buffer() {
			m_icons1_vb = create_buffer(m_world.NoSeas.Count);
			m_icons2_vb = create_buffer(m_world.NoSeas.Count);
			m_city_names1_vb = create_buffer(m_world.NoSeas.Count);
			m_city_names2_vb = create_buffer(m_world.NoSeas.Count);

			sprite_vertex[] icons1_vbo = new sprite_vertex[m_world.NoSeas.Count * 4];
			sprite_vertex[] icons2_vbo = new sprite_vertex[m_world.NoSeas.Count * 4];
			sprite_vertex[] city_names1_vbo = new sprite_vertex[m_world.NoSeas.Count * 4];
			sprite_vertex[] city_names2_vbo = new sprite_vertex[m_world.NoSeas.Count * 4];

			int index = 0;
			foreach (GvoWorldInfo.Info i in m_world.NoSeas) {
				// 아이콘
				set_vbo(ref icons1_vbo, index, i.position, new Point(0, 0), i.IconRect, 0, -1);
				set_vbo(ref icons2_vbo, index, i.position, new Point(0, 0), i.SmallIconRect, 0, -1);
				// 문자 (도시이름)
				set_vbo(ref city_names1_vbo, index, i.position, i.StringOffset1, i.NameRect, i.angle, -1);
				set_vbo(ref city_names2_vbo, index, i.position, i.StringOffset2, i.NameRect, i.angle, -1);

				index++;
			}
			// 정점들을 설정
			m_icons1_vb.SetData<sprite_vertex>(icons1_vbo);
			m_icons2_vb.SetData<sprite_vertex>(icons2_vbo);
			m_city_names1_vb.SetData<sprite_vertex>(city_names1_vbo);
			m_city_names2_vb.SetData<sprite_vertex>(city_names2_vbo);
		}

		/*-------------------------------------------------------------------------
		 해역명と풍향のバッファ작성 
		---------------------------------------------------------------------------*/
		private void create_sea_buffer() {
			m_sea_names1_vb = create_buffer(m_world.Seas.Count * 3);
			m_sea_names2_vb = create_buffer(m_world.Seas.Count * 3);
			sprite_vertex[] vbo1 = new sprite_vertex[(m_world.Seas.Count * 3) * 4];
			sprite_vertex[] vbo2 = new sprite_vertex[(m_world.Seas.Count * 3) * 4];

			int index = 0;
			float scale = this.GetDpiScaleRatio();
			int offset = (int)(6 * scale);
			foreach (GvoWorldInfo.Info i in m_world.Seas) {
				// 문자 위치
				var pos = new Point(i.position.X, i.position.Y);
				pos.Y += (int)((pos.Y - i.SeaInfo.WindPos.Y) * scale * 0.2f); // 문자표시위치를 조금 조정한다.
				set_vbo(ref vbo1, index, pos, new Point(-offset, 0), m_lib.seainfonameimage.GetRect(index), i.angle, -1);
				set_vbo(ref vbo2, index, pos, new Point(-offset, 0), m_lib.seainfonameimage.GetRect(index), i.angle, -1);
				index++;
			}

			foreach (GvoWorldInfo.Info i in m_world.Seas) {
				// 풍향
				if (i.SeaInfo != null) {
					Point pos = new Point(i.SeaInfo.WindPos.X - i.position.X,
											i.SeaInfo.WindPos.Y - i.position.Y);

					// 夏
					set_vbo(ref vbo1, index, i.position, new Point(pos.X - offset, pos.Y), m_lib.seainfonameimage.GetWindArrowIcon(), i.SeaInfo.SummerAngle, -1);
					set_vbo(ref vbo2, index, i.position, new Point(pos.X - offset, pos.Y), m_lib.seainfonameimage.GetWindArrowIcon(), i.SeaInfo.SummerAngle, WIND_ANGLE_COLOR2);
					index++;
					// 冬
					set_vbo(ref vbo1, index, i.position, new Point(pos.X + offset, pos.Y), m_lib.seainfonameimage.GetWindArrowIcon(), i.SeaInfo.WinterAngle, WIND_ANGLE_COLOR2);
					set_vbo(ref vbo2, index, i.position, new Point(pos.X + offset, pos.Y), m_lib.seainfonameimage.GetWindArrowIcon(), i.SeaInfo.WinterAngle, -1);
					index++;
				}
			}

			// 頂点を전送
			m_sea_names1_vb.SetData<sprite_vertex>(vbo1);
			m_sea_names2_vb.SetData<sprite_vertex>(vbo2);
		}

		/*-------------------------------------------------------------------------
		 頂点情報を설정する
		 1つの스프라이트분
		 회전지정
		---------------------------------------------------------------------------*/
		private void set_vbo(ref sprite_vertex[] tbl, int index, Point position, Point offset, d3d_sprite_rects.rect _rect, float angle_rad, int color) {
			if (_rect == null) return;

			Vector3 pos = new Vector3(position.X, position.Y, 0.3f);
			Vector3 param = new Vector3(1, 1, angle_rad);
			Vector2 offset2 = new Vector2(offset.X, offset.Y);

			index *= 4;
			for (int i = 0; i < 4; i++) {
				tbl[index + i].color = color;
				tbl[index + i].Position = pos;
				tbl[index + i].offset1 = _rect.offset[i];
				tbl[index + i].offset2 = offset2;
				tbl[index + i].param = param;
				tbl[index + i].uv = _rect.uv[i];
			}
		}

		/*-------------------------------------------------------------------------
		 하나의 버퍼 그리기
		 파라미터 설정을 해놓을 것
		---------------------------------------------------------------------------*/
		private void draw_buffer(d3d_writable_vb_with_index vb, int sprite_count) {
			m_lib.device.device.SetStreamSource(0, vb.vb, 0, sprite_vertex.SizeInBytes);
			m_lib.device.device.Indices = vb.ib;
			m_lib.device.device.DrawIndexedPrimitives(PrimitiveType.TriangleList,
										0, 0, sprite_count * 4, 0, sprite_count * 2);
		}

		/*-------------------------------------------------------------------------
		 쉐이더용頂点
		---------------------------------------------------------------------------*/
		[StructLayout(LayoutKind.Sequential)]
		protected struct sprite_vertex {
			public Vector3 Position;   // 좌표					12
			public Vector2 uv;		  // uv					8
			public Vector2 offset1; // x, y		offset1		8
			public Vector2 offset2; // x, y		offset2		8
			public Vector3 param;	   // x, y		ImageScale		12
										// z		angle_rad
			public int color;	   // Color				4

			/*-------------------------------------------------------------------------
			 頂点情報
			---------------------------------------------------------------------------*/
			public static VertexElement[] VertexElements = {
				// Position (12バイト)
				new VertexElement(0, 0, DeclarationType.Float3,
										DeclarationMethod.Default,
										DeclarationUsage.Position, 0),
				// uv (8バイト)
				new VertexElement(0, 12, DeclarationType.Float2,
										DeclarationMethod.Default,
										DeclarationUsage.TextureCoordinate, 0),
				// offset1 offset2
				new VertexElement(0, 12+8, DeclarationType.Float4,
										DeclarationMethod.Default,
										DeclarationUsage.TextureCoordinate, 1),
				// ImageScale angle_rad
				new VertexElement(0, 12+8+16, DeclarationType.Float3,
										DeclarationMethod.Default,
										DeclarationUsage.TextureCoordinate, 2),

				// Color  (4バイト)
				new VertexElement(0, 12+8+16+12, DeclarationType.Color,
										DeclarationMethod.Default,
										DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd,
			};

			/*-------------------------------------------------------------------------
			 頂点1つの사이즈
			---------------------------------------------------------------------------*/
			public static int SizeInBytes {
				get { return Marshal.SizeOf(typeof(sprite_vertex)); }
			}
		}

		/*-------------------------------------------------------------------------
		 DPIスケーリング배率取得
		---------------------------------------------------------------------------*/
		private float GetDpiScaleRatio() {
			if (this.m_lib.setting.enable_dpi_scaling) {
				return Useful.GetDpiRatio();
			} else {
				return 1f;
			}
		}
	}
}
