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
using static directx.d3d_textured_font;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	class draw_infonames : IDisposable {
		private gvt_lib m_lib;
		private GvoWorldInfo m_world;

		private Device m_device;

		private bool m_is_create_buffers;   // 쉐이더용 버퍼를 만들면 true
		private bool m_is_error;
		private VertexDeclaration m_decl;

		private d3d_writable_vb_with_index m_icons1_vb;   // 대 아이콘용
		private d3d_writable_vb_with_index m_icons2_vb;   // 소 아이콘용
		private d3d_writable_vb_with_index m_icons3_vb;   // 조합도시 아이콘용

		private d3d_writable_vb_with_index m_city_names1_vb;        // 도시명
		private d3d_writable_vb_with_index m_city_names2_vb;        // 도시명
		private d3d_writable_vb_with_index m_city_names3_vb;        // 조합도시명

		private d3d_writable_vb_with_index m_shore_names_vb;    // 1차 상륙지, 개인농장
		private d3d_writable_vb_with_index m_land_names_vb;     // 2차 상륙지, 내륙도시, 교외명 등 기타

		private d3d_writable_vb_with_index m_sea_icons1_vb;  // 여름용 풍향아이콘
		private d3d_writable_vb_with_index m_sea_icons2_vb;  // 겨울용 풍향아이콘
		private d3d_writable_vb_with_index m_sea_names_vb;  // 여름용 해역명

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public draw_infonames(gvt_lib lib, GvoWorldInfo world) {
			m_lib = lib;
			m_world = world;

			m_device = lib.device.device;

			// 쉐이더사용時
			m_is_create_buffers = false;
			m_is_error = false;

			m_icons1_vb = null;
			m_icons2_vb = null;
			m_icons3_vb = null;

			m_city_names1_vb = null;
			m_city_names2_vb = null;
			m_city_names3_vb = null;

			m_shore_names_vb = null;
			m_land_names_vb = null;

			m_sea_icons1_vb = null;
			m_sea_icons2_vb = null;
			m_sea_names_vb = null;
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public void Dispose() {
			if (m_decl != null) m_decl.Dispose();

			if (m_icons1_vb != null) m_icons1_vb.Dispose();
			if (m_icons2_vb != null) m_icons2_vb.Dispose();
			if (m_icons3_vb != null) m_icons3_vb.Dispose();

			if (m_city_names1_vb != null) m_city_names1_vb.Dispose();
			if (m_city_names2_vb != null) m_city_names2_vb.Dispose();
			if (m_city_names3_vb != null) m_city_names3_vb.Dispose();

			if (m_shore_names_vb != null) m_shore_names_vb.Dispose();
			if (m_land_names_vb != null) m_land_names_vb.Dispose();

			if (m_sea_icons1_vb != null) m_sea_icons1_vb.Dispose();
			if (m_sea_icons2_vb != null) m_sea_icons2_vb.Dispose();
			if (m_sea_names_vb != null) m_sea_names_vb.Dispose();

			m_decl = null;
			m_icons1_vb = null;
			m_icons2_vb = null;
			m_icons3_vb = null;

			m_city_names1_vb = null;
			m_city_names2_vb = null;
			m_city_names3_vb = null;

			m_shore_names_vb = null;
			m_land_names_vb = null;

			m_sea_icons1_vb = null;
			m_sea_icons2_vb = null;
			m_sea_names_vb = null;
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

				m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_cityicon_shader_proc), 64);
				m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_landname_shader_proc), 64);
				m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_cityname_shader_proc), 64);
				
				m_lib.device.device.RenderState.ZBufferEnable = true;
			} else {
				// 통상
				if (m_lib.loop_image.ImageScale < 0.8f || m_lib.setting.map_draw_names == MapDrawNames.OnlyBigCity) {
					m_lib.device.device.RenderState.ZBufferEnable = false;

					m_lib.device.sprites.BeginDrawSprites(m_lib.icons.texture);
					m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_cityicon2_proc), 64);
					m_lib.device.sprites.EndDrawSprites();

					m_lib.device.sprites.BeginDrawSprites(m_lib.nameTexture.cityTexture);
					m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_cityname2_proc), 64);
					m_lib.device.sprites.EndDrawSprites();

					m_lib.device.device.RenderState.ZBufferEnable = true;

					return;
				}

				m_lib.device.device.RenderState.ZBufferEnable = false;

				m_lib.device.sprites.BeginDrawSprites(m_lib.icons.texture);
				m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_cityicon_proc), 64);
				m_lib.device.sprites.EndDrawSprites();

				m_lib.device.sprites.BeginDrawSprites(m_lib.nameTexture.elseTexture);
				m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_landname_proc), 64);
				m_lib.device.sprites.EndDrawSprites();

				m_lib.device.sprites.BeginDrawSprites(m_lib.nameTexture.cityTexture);
				m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_cityname_proc), 64);
				m_lib.device.sprites.EndDrawSprites();

				m_lib.device.device.RenderState.ZBufferEnable = true;
			}
		}

		/*-------------------------------------------------------------------------
		 상륙지그리기
		---------------------------------------------------------------------------*/
		private void draw_landname_proc(Vector2 offset, LoopXImage image) {
			if (m_lib.setting.map_draw_names == MapDrawNames.Hide || m_lib.setting.map_draw_names == MapDrawNames.OnlyCity) {
				return;
			}

			// 1차 상륙지
			draw_namerects(offset, image, m_world.Shores);

			if (m_lib.setting.map_draw_names == MapDrawNames.CityAndShore) {
				return;
			}

			// 2차 상륙지
			draw_namerects(offset, image, m_world.NoSeas);
		}

		/*-------------------------------------------------------------------------
		 도시명그리기
		 상륙지점も含む
		---------------------------------------------------------------------------*/
		private void draw_cityname_proc(Vector2 offset, LoopXImage image) {
			if (m_lib.setting.map_draw_names == MapDrawNames.Hide) {
				return;
			}

			draw_namerects(offset, image, m_world.Cities);
		}

		private void draw_cityname2_proc(Vector2 offset, LoopXImage image) {
			if (m_lib.setting.map_draw_names == MapDrawNames.Hide) {
				return;
			}

			foreach (GvoWorldInfo.Info i in m_world.Cities) {
				if (i.NameRect == null) continue;

				// 조합(2)이나 주점아가씨(1)가 존재하지 않을 경우 스킵
				if ((i.Sakaba & 2) != 2 && (i.Sakaba & 1) != 1) continue;

				// 카리브 영지인 경우 스킵
				if (i.AllianceType == GvoWorldInfo.AllianceType.Territory 
					&& (i.CulturalSphere == GvoWorldInfo.CulturalSphere.Caribbean || i.CulturalSphere == GvoWorldInfo.CulturalSphere.EastLatinAmerica)) continue;

				Vector2 p = image.GlobalPos2LocalPos(transform.ToVector2(i.position), offset);

				// 소 아이콘
				p.X += i.StringOffset2.X;
				p.Y += i.StringOffset2.Y;

				m_lib.device.sprites.AddDrawSpritesNC(new Vector3(p.X, p.Y, 0.3f), i.NameRect, i.angle, -1);
			}
		}

		private void draw_namerects(Vector2 offset, LoopXImage image, hittest_list list) {
			foreach (GvoWorldInfo.Info i in list) {
				if (i.NameRect == null) continue;

				Vector2 p = image.GlobalPos2LocalPos(transform.ToVector2(i.position), offset);

				if (m_lib.setting.map_icon == MapIcon.Big) {
					// 대 아이콘
					p.X += i.StringOffset1.X;
					p.Y += i.StringOffset1.Y;
				} else {
					// 소 아이콘
					p.X += i.StringOffset2.X;
					p.Y += i.StringOffset2.Y;
				}

				m_lib.device.sprites.AddDrawSpritesNC(new Vector3(p.X, p.Y, 0.3f), i.NameRect, i.angle, -1);
			}
		}

		/*-------------------------------------------------------------------------
		 아이콘그리기
		 상륙지점も含む
		---------------------------------------------------------------------------*/
		private void draw_cityicon_proc(Vector2 offset, LoopXImage image) {
			if (m_lib.setting.map_icon == MapIcon.Hide) {
				return;
			}

			draw_iconrects(offset, image, m_world.Cities);

			draw_iconrects(offset, image, m_world.Shores);

			if (m_lib.setting.map_icon == MapIcon.CityAndShore) {
				return;
			}

			draw_iconrects(offset, image, m_world.NoSeas);
		}

		private void draw_cityicon2_proc(Vector2 offset, LoopXImage image) {
			if (m_lib.setting.map_icon == MapIcon.Hide) {
				return;
			}

			foreach (GvoWorldInfo.Info i in m_world.Cities) {
				Vector2 p = image.GlobalPos2LocalPos(transform.ToVector2(i.position), offset);

				// 조합(2)이나 주점아가씨(1)가 존재하지 않을 경우 스킵
				if ((i.Sakaba & 2) != 2 && (i.Sakaba & 1) != 1) continue;

				// 카리브 영지인 경우 스킵
				if (i.AllianceType == GvoWorldInfo.AllianceType.Territory
					&& (i.CulturalSphere == GvoWorldInfo.CulturalSphere.Caribbean || i.CulturalSphere == GvoWorldInfo.CulturalSphere.EastLatinAmerica)) continue;

				m_lib.device.sprites.AddDrawSprites(new Vector3(p.X, p.Y, 0.3f), i.SmallIconRect);
			}
		}

		private void draw_iconrects(Vector2 offset, LoopXImage image, hittest_list list) {
			foreach (GvoWorldInfo.Info i in list) {
				Vector2 p = image.GlobalPos2LocalPos(transform.ToVector2(i.position), offset);

				d3d_sprite_rects.rect refRect;
				if (m_lib.setting.map_icon == MapIcon.Big) {
					// 대 아이콘
					refRect = i.IconRect;
				} else {
					refRect = i.SmallIconRect;
				}

				if (refRect != null) {
					m_lib.device.sprites.AddDrawSprites(new Vector3(p.X, p.Y, 0.3f), refRect);
				}
			}
		}

		/*-------------------------------------------------------------------------
		 상륙지그리기
		 쉐이더사용
		---------------------------------------------------------------------------*/
		private void draw_landname_shader_proc(Vector2 offset, LoopXImage image) {
			if (m_lib.setting.map_draw_names == MapDrawNames.Hide || m_lib.setting.map_draw_names == MapDrawNames.OnlyCity) {
				return;
			}

			if (m_lib.loop_image.ImageScale < 0.8f || m_lib.setting.map_draw_names == MapDrawNames.OnlyBigCity) {
				return;
			}

			set_shader_params(m_lib.nameTexture.elseTexture, offset, image.ImageScale);

			Effect effect = m_lib.device.sprites.effect;
			if (effect == null) return;

			effect.Begin(0);
			effect.BeginPass(0);

			m_lib.device.device.VertexDeclaration = m_decl;

			draw_buffer(m_shore_names_vb, m_world.Shores.Count);

			if (m_lib.setting.map_draw_names == MapDrawNames.CityAndShore) {
				return;
			}

			draw_buffer(m_land_names_vb, m_world.NoSeas.Count);

			effect.EndPass();
			effect.End();
		}

		/*-------------------------------------------------------------------------
		도시명그리기
		상륙지점も含む
		쉐이더사용
	   ---------------------------------------------------------------------------*/
		private void draw_cityname_shader_proc(Vector2 offset, LoopXImage image) {
			if (m_lib.setting.map_draw_names == MapDrawNames.Hide) {
				return;
			}

			set_shader_params(m_lib.nameTexture.cityTexture, offset, image.ImageScale);

			Effect effect = m_lib.device.sprites.effect;
			if (effect == null) return;

			effect.Begin(0);
			effect.BeginPass(0);

			m_lib.device.device.VertexDeclaration = m_decl;
			if (m_lib.loop_image.ImageScale < 0.8f || m_lib.setting.map_draw_names == MapDrawNames.OnlyBigCity) {
				draw_buffer(m_city_names3_vb, m_world.Cities.Count);
			} else if (m_lib.setting.map_icon == MapIcon.Big) {
				// 대きい아이콘		
				draw_buffer(m_city_names1_vb, m_world.Cities.Count);
			} else {
				// 소さい아이콘	
				draw_buffer(m_city_names2_vb, m_world.Cities.Count);
			}
			effect.EndPass();
			effect.End();
		}

		/*-------------------------------------------------------------------------
		 아이콘그리기
		 상륙지점も含む
		 쉐이더사용
		---------------------------------------------------------------------------*/
		private void draw_cityicon_shader_proc(Vector2 offset, LoopXImage image) {
			if (m_lib.setting.map_icon == MapIcon.Hide) {
				return;
			}

			set_shader_params(m_lib.icons.texture, offset, image.ImageScale);

			Effect effect = m_lib.device.sprites.effect;
			if (effect == null) return;

			effect.Begin(0);
			effect.BeginPass(0);

			m_lib.device.device.VertexDeclaration = m_decl;

			if (m_lib.loop_image.ImageScale < 0.8f || m_lib.setting.map_icon == MapIcon.OnlyBigCity) {
				draw_buffer(m_icons3_vb, m_world.Cities.Count);
			} else if (m_lib.setting.map_icon == MapIcon.Big) {
				draw_buffer(m_icons1_vb, m_world.Cities.Count + m_world.Shores.Count + m_world.NoSeas.Count);
			} else if (m_lib.setting.map_icon == MapIcon.Small) {
				draw_buffer(m_icons2_vb, m_world.Cities.Count + m_world.Shores.Count + m_world.NoSeas.Count);
			} else {
				draw_buffer(m_icons2_vb, m_world.Cities.Count + m_world.Shores.Count);
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
				m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_seaicon_shader_proc), 64);
				m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_seaname_shader_proc), 64);
				m_lib.device.device.RenderState.ZBufferEnable = true;
			} else {
				// 통상
				if (m_lib.loop_image.ImageScale <= 0.8f || m_lib.setting.map_draw_names == MapDrawNames.OnlyBigCity) return;

				m_lib.device.device.RenderState.ZBufferEnable = false;

				m_lib.device.sprites.BeginDrawSprites(m_lib.icons.texture);
				m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_seaicon_proc), 64);
				m_lib.device.sprites.EndDrawSprites();

				m_lib.device.sprites.BeginDrawSprites(m_lib.nameTexture.elseTexture);
				m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_seaname_proc), 64);
				m_lib.device.sprites.EndDrawSprites();

				m_lib.device.device.RenderState.ZBufferEnable = true;
			}
		}

		/*-------------------------------------------------------------------------
		 해역명그리기
		---------------------------------------------------------------------------*/
		private void draw_seaname_proc(Vector2 offset, LoopXImage image) {
			if (m_lib.setting.map_draw_names == MapDrawNames.Hide) {
				return;
			}

			int index = 0;
			foreach (GvoWorldInfo.Info i in m_world.Seas) {
				Vector2 p = image.GlobalPos2LocalPos(transform.ToVector2(i.position), offset);
				// 해역명
				m_lib.device.sprites.AddDrawSpritesNC(new Vector3(p.X + 3, p.Y, 0.3f), m_lib.nameTexture.getRect(i.Name), i.angle, -1);
				index++;
			}
		}

		private void draw_seaicon_proc(Vector2 offset, LoopXImage image) {
			if (m_lib.setting.map_draw_names == MapDrawNames.Hide) {
				return;
			}
			
			d3d_sprite_rects.rect _rect = m_lib.icons.GetIcon(icons.icon_index.wind_arrow);

			int index = 0;
			foreach (GvoWorldInfo.Info i in m_world.Seas) {
				// 풍향그리기
				if (i.SeaInfo == null) continue;

				Vector2 p = image.GlobalPos2LocalPos(transform.ToVector2(i.position), offset);

				if (m_world.Season.now_season == gvo_season.season.summer) {
					m_lib.device.sprites.AddDrawSpritesNC(new Vector3(p.X - 3 - 1, p.Y + 3 + 1, 0.3f), _rect, i.SeaInfo.SummerAngle, -1);
				} else {
					m_lib.device.sprites.AddDrawSpritesNC(new Vector3(p.X - 3 - 1, p.Y + 3 + 1, 0.3f), _rect, i.SeaInfo.WinterAngle, -1);
				}

				index++;
			}
		}

		/*-------------------------------------------------------------------------
		 해역명그리기
		 쉐이더사용
		---------------------------------------------------------------------------*/
		private void draw_seaname_shader_proc(Vector2 offset, LoopXImage image) {
			if (m_lib.setting.map_draw_names == MapDrawNames.Hide) {
				return;
			}

			if (m_lib.loop_image.ImageScale <= 0.8f || m_lib.setting.map_draw_names == MapDrawNames.OnlyBigCity) return;

			set_shader_params(m_lib.nameTexture.elseTexture, offset, image.ImageScale);

			Effect effect = m_lib.device.sprites.effect;
			if (effect == null) return;

			effect.Begin(0);
			effect.BeginPass(0);

			draw_buffer(m_sea_names_vb, m_world.Seas.Count * 3);

			effect.EndPass();
			effect.End();
		}

		private void draw_seaicon_shader_proc(Vector2 offset, LoopXImage image) {
			if (m_lib.setting.map_draw_names == MapDrawNames.Hide) {
				return;
			}
			
			if (m_lib.loop_image.ImageScale <= 0.8f || m_lib.setting.map_draw_names == MapDrawNames.OnlyBigCity) return;

			set_shader_params(m_lib.icons.texture, offset, image.ImageScale);

			Effect effect = m_lib.device.sprites.effect;
			if (effect == null) return;

			effect.Begin(0);
			effect.BeginPass(0);

			m_lib.device.device.VertexDeclaration = m_decl;  // 頂点の디테일
			if (m_world.Season.now_season == gvo_season.season.summer) {
				// 夏
				draw_buffer(m_sea_icons1_vb, m_world.Seas.Count * 3);
			} else {
				// 冬
				draw_buffer(m_sea_icons2_vb, m_world.Seas.Count * 3);
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
				float scale = 1.01f;
				//float scale = mapscale;
				//if (scale > 0.8f) scale = 1f;
				//if (scale < 0.5f) scale = 0.5f;
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
				effect.Technique = "SpriteWithGlobalParams";        // グローバルパラメータと拡縮と회전付き
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
			m_lib.nameTexture.init(m_world);

			if (!m_lib.device.is_use_ve1_1_ps1_1) return;     // 쉐이더を使わない
			if (m_is_create_buffers) return;        // 작성されている
			if (m_is_error) return;  // 오류

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

				m_sea_icons1_vb = null;
				m_sea_icons2_vb = null;
				m_sea_names_vb = null;
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
			m_icons1_vb = create_buffer(m_world.Cities.Count + m_world.Shores.Count + m_world.NoSeas.Count);
			m_icons2_vb = create_buffer(m_world.Cities.Count + m_world.Shores.Count + m_world.NoSeas.Count);
			m_icons3_vb = create_buffer(m_world.Cities.Count);

			m_city_names1_vb = create_buffer(m_world.Cities.Count);
			m_city_names2_vb = create_buffer(m_world.Cities.Count);
			m_city_names3_vb = create_buffer(m_world.Cities.Count);

			m_shore_names_vb = create_buffer(m_world.Shores.Count);
			m_land_names_vb = create_buffer(m_world.NoSeas.Count);

			sprite_vertex[] icons1_vbo = new sprite_vertex[(m_world.Cities.Count + m_world.Shores.Count + m_world.NoSeas.Count) * 4];
			sprite_vertex[] icons2_vbo = new sprite_vertex[(m_world.Cities.Count + m_world.Shores.Count + m_world.NoSeas.Count) * 4];
			sprite_vertex[] icons3_vbo = new sprite_vertex[(m_world.Cities.Count) * 4];

			sprite_vertex[] city_names1_vbo = new sprite_vertex[m_world.Cities.Count * 4];
			sprite_vertex[] city_names2_vbo = new sprite_vertex[m_world.Cities.Count * 4];
			sprite_vertex[] city_names3_vbo = new sprite_vertex[m_world.Cities.Count * 4];

			sprite_vertex[] shore_names_vbo = new sprite_vertex[m_world.Shores.Count * 4];
			sprite_vertex[] land_names_vbo = new sprite_vertex[m_world.NoSeas.Count * 4];

			int index = 0;
			foreach (GvoWorldInfo.Info i in m_world.Cities) {
				// 아이콘
				set_vbo(ref icons1_vbo, index, i.position, new Point(0, 0), i.IconRect, 0, -1);
				set_vbo(ref icons2_vbo, index, i.position, new Point(0, 0), i.SmallIconRect, 0, -1);

				// 문자 (도시이름)
				set_vbo(ref city_names1_vbo, index, i.position, i.StringOffset1, i.NameRect, i.angle, -1);
				set_vbo(ref city_names2_vbo, index, i.position, i.StringOffset2, i.NameRect, i.angle, -1);

				// 조합(2)이나 주점아가씨(1)가 존재할 경우, 카리브 영지가 아닌 경우
				if (((i.Sakaba & 2) == 2 || (i.Sakaba & 1) == 1) && (i.AllianceType != GvoWorldInfo.AllianceType.Territory 
					|| (i.CulturalSphere != GvoWorldInfo.CulturalSphere.Caribbean && i.CulturalSphere != GvoWorldInfo.CulturalSphere.EastLatinAmerica))) {

					set_vbo(ref icons3_vbo, index, i.position, new Point(0, 0), i.SmallIconRect, 0, -1);
					set_vbo(ref city_names3_vbo, index, i.position, i.StringOffset2, i.NameRect, i.angle, -1);
				}

				index++;
			}
			int index2 = 0;
			foreach (GvoWorldInfo.Info i in m_world.Shores) {
				// 아이콘
				set_vbo(ref icons1_vbo, index, i.position, new Point(0, 0), i.IconRect, 0, -1);
				set_vbo(ref icons2_vbo, index, i.position, new Point(0, 0), i.SmallIconRect, 0, -1);

				// 문자 (도시이름)
				set_vbo(ref shore_names_vbo, index2, i.position, i.StringOffset2, i.NameRect, i.angle, -1);

				index++;
				index2++;
			}
			int index3 = 0;
			foreach (GvoWorldInfo.Info i in m_world.NoSeas) {
				// 아이콘
				set_vbo(ref icons1_vbo, index, i.position, new Point(0, 0), i.IconRect, 0, -1);
				set_vbo(ref icons2_vbo, index, i.position, new Point(0, 0), i.SmallIconRect, 0, -1);

				// 문자 (도시이름)
				set_vbo(ref land_names_vbo, index3, i.position, i.StringOffset2, i.NameRect, i.angle, -1);

				index++;
				index3++;
			}
			// 정점들을 설정
			m_icons1_vb.SetData<sprite_vertex>(icons1_vbo);
			m_icons2_vb.SetData<sprite_vertex>(icons2_vbo);
			m_icons3_vb.SetData<sprite_vertex>(icons3_vbo);

			m_city_names1_vb.SetData<sprite_vertex>(city_names1_vbo);
			m_city_names2_vb.SetData<sprite_vertex>(city_names2_vbo);
			m_city_names3_vb.SetData<sprite_vertex>(city_names3_vbo);

			m_shore_names_vb.SetData<sprite_vertex>(shore_names_vbo);
			m_land_names_vb.SetData<sprite_vertex>(land_names_vbo);
		}

		/*-------------------------------------------------------------------------
		 해역명と풍향のバッファ작성 
		---------------------------------------------------------------------------*/
		private void create_sea_buffer() {
			m_sea_icons1_vb = create_buffer(m_world.Seas.Count * 3);
			m_sea_icons2_vb = create_buffer(m_world.Seas.Count * 3);
			m_sea_names_vb = create_buffer(m_world.Seas.Count * 3);

			sprite_vertex[] vbo1 = new sprite_vertex[(m_world.Seas.Count * 3) * 4];
			sprite_vertex[] vbo2 = new sprite_vertex[(m_world.Seas.Count * 3) * 4];
			sprite_vertex[] vbo3 = new sprite_vertex[(m_world.Seas.Count * 3) * 4];

			int index = 0;
			float scale = this.GetDpiScaleRatio();
			int diff = (int)(3 * scale);
			foreach (GvoWorldInfo.Info i in m_world.Seas) {
				// 문자 위치
				var pos = new Point(i.position.X, i.position.Y);
				set_vbo(ref vbo3, index, pos, new Point(diff, -diff), m_lib.nameTexture.getRect(i.Name), i.angle, -1);
				index++;
			}

			d3d_sprite_rects.rect windRect = m_lib.icons.GetIcon(icons.icon_index.wind_arrow);

			foreach (GvoWorldInfo.Info i in m_world.Seas) {
				// 풍향
				if (i.SeaInfo != null) {
					Point pos = new Point(i.position.X - 1, i.position.Y + 1);

					// 夏
					set_vbo(ref vbo1, index, pos, new Point(-diff, diff), windRect, i.SeaInfo.SummerAngle, -1);
					index++;
					// 冬
					set_vbo(ref vbo2, index, pos, new Point(-diff, diff), windRect, i.SeaInfo.WinterAngle, -1);
					index++;
				}
			}

			// 頂点を전送
			m_sea_icons1_vb.SetData<sprite_vertex>(vbo1);
			m_sea_icons2_vb.SetData<sprite_vertex>(vbo2);
			m_sea_names_vb.SetData<sprite_vertex>(vbo3);
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
			public Vector2 uv;      // uv					8
			public Vector2 offset1; // x, y		offset1		8
			public Vector2 offset2; // x, y		offset2		8
			public Vector3 param;     // x, y		ImageScale		12
									  // z		angle_rad
			public int color;     // Color				4

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
