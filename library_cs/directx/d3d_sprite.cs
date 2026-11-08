/*-------------------------------------------------------------------------

 Direct3D
 스프라이트
 텍스쳐 한 장에서 대량으로 그리기 위한 용도
 쉐이더를 쓸 수 있을 때는 쉐이더를 사용한다

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace directx {
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class d3d_sprite : IDisposable {
		private const int DRAW_SPRITE_ONCE = 128;
		private const int SWAP_BFFER_MAX = 16;	  // 쉐이더 사용 시의 버퍼 수

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private Device m_d3d_device;

		private CustomVertex.TransformedColoredTextured[] m_vertex_list;
		private int m_sprite_count;
		private Texture m_texture;		  // 일시적으로 보관하는 텍스쳐

		private Vector2 m_map_offset;
		private float m_map_scale;
		private Vector2 m_global_scale;

		// 현재 프레임에 그린 스프라이트 수
		private int m_draw_sprites_in_frame;

		// 쉐이더 사용 시
		private Effect m_effect;
		private VertexDeclaration m_decl;
		private d3d_writable_vb_with_index m_vb;
		private sprite_vertex[] m_vbo;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public int draw_sprites_in_frame { get { return m_draw_sprites_in_frame; } }
		// 외부에서 특별히 쉐이더를 사용할 때용
		// 평소에는 얻을 필요가 없다
		public Effect effect { get { return m_effect; } }

		/*-------------------------------------------------------------------------
		 프레임 시작
		---------------------------------------------------------------------------*/
		public void BeginFrame() {
			m_draw_sprites_in_frame = 0;
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public d3d_sprite(Device device, bool is_use_ve1_1_ps1_1) {
			m_d3d_device = device;

			// 버퍼
			m_vertex_list = new CustomVertex.TransformedColoredTextured[DRAW_SPRITE_ONCE * 6];
			m_sprite_count = 0;

			// 쉐이더 사용 시
			m_effect = null;
			m_decl = null;
			m_vb = null;
			m_vbo = null;

			// 쉐이더를 쓸 수 있으면 사용한다
			if (is_use_ve1_1_ps1_1) {
				Assembly ass = Assembly.GetExecutingAssembly();
				initialize_shader(ass.GetManifestResourceStream("directx.fx.sprite.fxo"));
			}
			BeginFrame();
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public void Dispose() {
			DisposeEffect();
		}

		/*-------------------------------------------------------------------------
		 이펙트 해제
		---------------------------------------------------------------------------*/
		public void DisposeEffect() {
			if (m_effect != null) m_effect.Dispose();
			if (m_decl != null) m_decl.Dispose();
			if (m_vb != null) m_vb.Dispose();
			m_effect = null;
			m_decl = null;
			m_vb = null;
		}

		/*-------------------------------------------------------------------------
		 쉐이더 사용 초기화
		 스프라이트용 이펙트를 읽어들여 초기화한다
		 .fx 든 .fxo 든 상관없다
		---------------------------------------------------------------------------*/
		/*
				private void initialize_shader(string fname)
				{
					string	error;
					try{
						m_effect	= Effect.FromFile(	m_d3d_device, fname,
														null, null,
														ShaderFlags.None,
														null, out error);
						initialize_shader();
					}catch{
						m_effect	= null;
					}
				}
		*/
		/*-------------------------------------------------------------------------
		 쉐이더 사용 초기화
		 스프라이트용 이펙트를 읽어들여 초기화한다
		 임베드된 리소스로부터 읽기
		 .fxo 여야 한다
		---------------------------------------------------------------------------*/
		private void initialize_shader(Stream stream) {
			string error;
			try {
				m_effect = Effect.FromStream(m_d3d_device, stream,
												null, null,
												ShaderFlags.None,
												null, out error);
				initialize_shader();
			} catch {
				m_effect = null;
			}
		}

		/*-------------------------------------------------------------------------
		 쉐이더 사용용 초기화
		 Effect 는 이미 설정되어 있어야 한다
		---------------------------------------------------------------------------*/
		private void initialize_shader() {
			try {
				m_decl = new VertexDeclaration(m_d3d_device, sprite_vertex.VertexElements);
				m_vb = new d3d_writable_vb_with_index(
													m_d3d_device,
													typeof(sprite_vertex),
													DRAW_SPRITE_ONCE * 4,
													SWAP_BFFER_MAX);
				m_vbo = new sprite_vertex[DRAW_SPRITE_ONCE * 4];
			} catch {
				// 실패
				m_effect = null;
				m_decl = null;
				m_vb = null;
				m_vbo = null;
			}
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 시작
		 같은 텍스쳐를 사용하는 스프라이트 그리기용
		---------------------------------------------------------------------------*/
		public void BeginDrawSprites(Texture tex) {
			// 쉐이더 사용 시 설정
			begin_draw_sprites_shader(tex);

			if (m_effect == null) {
				// 쉐이더 미사용 시 설정
				m_texture = tex;
				m_map_offset = new Vector2(0, 0);
				m_map_scale = 1;
				m_global_scale = new Vector2(1, 1);
			}
			m_sprite_count = 0;
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 시작
		 같은 텍스쳐를 사용하는 스프라이트 그리기용
		---------------------------------------------------------------------------*/
		private void begin_draw_sprites_shader(Texture tex) {
			if (m_effect == null) return;

			try {
				// 쉐이더 상수를 설정한다.
				float[] vector = { m_d3d_device.Viewport.Width, m_d3d_device.Viewport.Height };
				m_effect.SetValue("ViewportSize", vector);
				m_effect.SetValue("Texture", tex);

				// Technique
				// 통상 스프라이트
				m_effect.Technique = "Sprite";
			} catch {
				// 실패
				// 쉐이더 사용을 중단한다
				DisposeEffect();
			}
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 시작
		 같은 텍스쳐를 사용하는 스프라이트 그리기용
		 글로벌 파라미터 지정
		---------------------------------------------------------------------------*/
		public void BeginDrawSprites(Texture tex, Vector2 map_offset, float map_scale, Vector2 global_scale) {
			// 쉐이더 사용 시 설정
			begin_draw_sprites_shader(tex, map_offset, map_scale, global_scale);

			if (m_effect == null) {
				// 쉐이더 미사용 시 설정
				m_texture = tex;
				m_map_offset = map_offset;
				m_map_scale = map_scale;
				m_global_scale = global_scale;
			}
			m_sprite_count = 0;
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 시작
		 같은 텍스쳐를 사용하는 스프라이트 그리기용
		 글로벌 파라미터 지정
		---------------------------------------------------------------------------*/
		public void begin_draw_sprites_shader(Texture tex, Vector2 map_offset, float map_scale, Vector2 global_scale) {
			if (m_effect == null) return;

			try {
				// 쉐이더 상수를 설정한다.
				float[] vector = { m_d3d_device.Viewport.Width, m_d3d_device.Viewport.Height };
				float[] _offset = { map_offset.X, map_offset.Y };
				float[] gscale = { global_scale.X, global_scale.Y };
				m_effect.SetValue("ViewportSize", vector);
				m_effect.SetValue("Texture", tex);
				m_effect.SetValue("MapOffset", _offset);
				m_effect.SetValue("MapScale", map_scale);
				m_effect.SetValue("GlobalScale", gscale);

				// technique
				m_effect.Technique = "SpriteWithGlobalParams";	  // 글로벌 파라미터 + 확대축소 + 회전 포함
			} catch {
				// 실패
				// 쉐이더 사용을 중단한다
				DisposeEffect();
			}
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private Vector3 goffsetscale(Vector3 p) {
			p.X += m_map_offset.X;
			p.Y += m_map_offset.Y;
			p.X *= m_map_scale;
			p.Y *= m_map_scale;
			return p;
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private Vector2 gscale(Vector2 scale) {
			scale.X *= m_global_scale.X;
			scale.Y *= m_global_scale.Y;
			return scale;
		}

		//#if aaa
		/*-------------------------------------------------------------------------
		 스프라이트 그리기 추가
		 등배, 흰색, 불투명
		---------------------------------------------------------------------------*/
		public bool AddDrawSprites(Vector3 pos, d3d_sprite_rects.rect _rect) {
			return AddDrawSprites(pos, _rect, -1);
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 추가
		 등배, 색지정
		---------------------------------------------------------------------------*/
		public bool AddDrawSprites(Vector3 pos, d3d_sprite_rects.rect _rect, int color) {
			return AddDrawSprites(pos, _rect, color, new Vector2(0, 0));
		}
		public bool AddDrawSprites(Vector3 pos, d3d_sprite_rects.rect _rect, int color, Vector2 offset2) {
			if (m_effect != null) {
				// 쉐이더사용
				add_sprite_to_vertex_buffer_shader(pos, _rect, 0, new Vector2(1, 1), color, offset2);
			} else {
				pos = goffsetscale(pos);
				Vector2 size = gscale(_rect.size);

				// 정수화
				pos.X = (float)(int)pos.X;
				pos.Y = (float)(int)pos.Y;
				pos.X += offset2.X;
				pos.Y += offset2.Y;

				pos.X += _rect.lefttop.X;
				pos.Y += _rect.lefttop.Y;

				// 컬링
				if (pos.X >= m_d3d_device.Viewport.Width + m_d3d_device.Viewport.X) return false;
				if (pos.Y >= m_d3d_device.Viewport.Height + m_d3d_device.Viewport.Y) return false;
				if (pos.X + size.X < 0) return false;
				if (pos.Y + size.Y < 0) return false;

				// 추가
				add_sprite_to_vertex_buffer(pos, size, _rect, color);
			}

			if (m_sprite_count >= DRAW_SPRITE_ONCE) {
				draw_sprites();
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 추가
		 스케일지정
		---------------------------------------------------------------------------*/
		public bool AddDrawSprites(Vector3 pos, d3d_sprite_rects.rect _rect, Vector2 scale) {
			return AddDrawSprites(pos, _rect, scale, -1);
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 추가
		 스케일지정, 색지정
		---------------------------------------------------------------------------*/
		public bool AddDrawSprites(Vector3 pos, d3d_sprite_rects.rect _rect, Vector2 scale, int color) {
			return AddDrawSprites(pos, _rect, scale, color, new Vector2(0, 0));
		}
		public bool AddDrawSprites(Vector3 pos, d3d_sprite_rects.rect _rect, Vector2 scale, int color, Vector2 offset2) {
			if (m_effect != null) {
				// 쉐이더사용
				add_sprite_to_vertex_buffer_shader(pos, _rect, 0, scale, color, offset2);
			} else {
				pos = goffsetscale(pos);

				Vector2 offset = _rect.lefttop;
				Vector2 size = gscale(_rect.size);

				offset.X *= scale.X;
				offset.Y *= scale.Y;
				size.X *= scale.X;
				size.Y *= scale.Y;

				// 정수화
				pos.X = (float)(int)pos.X;
				pos.Y = (float)(int)pos.Y;
				pos.X += offset2.X;
				pos.Y += offset2.Y;

				pos.X += offset.X;
				pos.Y += offset.Y;

				// 컬링
				if (pos.X >= m_d3d_device.Viewport.Width + m_d3d_device.Viewport.X) return false;
				if (pos.Y >= m_d3d_device.Viewport.Height + m_d3d_device.Viewport.Y) return false;
				if (pos.X + size.X < 0) return false;
				if (pos.Y + size.Y < 0) return false;

				// 추가
				add_sprite_to_vertex_buffer(pos, size, _rect, color);
			}
			if (m_sprite_count >= DRAW_SPRITE_ONCE) {
				draw_sprites();
			}
			return true;
		}
		//#endif
		/*-------------------------------------------------------------------------
		 스프라이트 그리기 추가
		 등배, 흰색, 불투명
		 화면외컬링없음
		---------------------------------------------------------------------------*/
		public bool AddDrawSpritesNC(Vector3 pos, d3d_sprite_rects.rect _rect) {
			return AddDrawSpritesNC(pos, _rect, -1);
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 추가
		 등배, 색지정
		 화면외컬링없음
		---------------------------------------------------------------------------*/
		public bool AddDrawSpritesNC(Vector3 pos, d3d_sprite_rects.rect _rect, int color) {
			return AddDrawSpritesNC(pos, _rect, color, new Vector2(0, 0));
		}
		public bool AddDrawSpritesNC(Vector3 pos, d3d_sprite_rects.rect _rect, int color, Vector2 offset2) {
			if (m_effect != null) {
				// 쉐이더사용
				add_sprite_to_vertex_buffer_shader(pos, _rect, 0, new Vector2(1, 1), color, offset2);
			} else {
				pos = goffsetscale(pos);
				Vector2 size = gscale(_rect.size);

				// 정수화
				pos.X = (float)(int)pos.X;
				pos.Y = (float)(int)pos.Y;
				pos.X += offset2.X;
				pos.Y += offset2.Y;

				pos.X += _rect.lefttop.X;
				pos.Y += _rect.lefttop.Y;

				// 추가
				add_sprite_to_vertex_buffer(pos, size, _rect, color);
			}
			if (m_sprite_count >= DRAW_SPRITE_ONCE) {
				draw_sprites();
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 추가
		 스케일지정
		 화면외컬링없음
		---------------------------------------------------------------------------*/
		public bool AddDrawSpritesNC(Vector3 pos, d3d_sprite_rects.rect _rect, Vector2 scale) {
			return AddDrawSpritesNC(pos, _rect, scale, -1);
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 추가
		 스케일지정, 색지정
		 화면외컬링없음
		---------------------------------------------------------------------------*/
		public bool AddDrawSpritesNC(Vector3 pos, d3d_sprite_rects.rect _rect, Vector2 scale, int color) {
			return AddDrawSpritesNC(pos, _rect, scale, color, new Vector2(0, 0));
		}
		public bool AddDrawSpritesNC(Vector3 pos, d3d_sprite_rects.rect _rect, Vector2 scale, int color, Vector2 offset2) {
			if (m_effect != null) {
				// 쉐이더사용
				add_sprite_to_vertex_buffer_shader(pos, _rect, 0, scale, color, offset2);
			} else {
				pos = goffsetscale(pos);

				Vector2 offset = _rect.lefttop;
				Vector2 size = gscale(_rect.size);

				offset.X *= scale.X;
				offset.Y *= scale.Y;
				size.X *= scale.X;
				size.Y *= scale.Y;

				pos.X += offset.X;
				pos.Y += offset.Y;
				// 정수화
				pos.X = (float)(int)pos.X;
				pos.Y = (float)(int)pos.Y;
				pos.X += offset2.X;
				pos.Y += offset2.Y;

				// 추가
				add_sprite_to_vertex_buffer(pos, size, _rect, color);
			}

			if (m_sprite_count >= DRAW_SPRITE_ONCE) {
				draw_sprites();
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 종료
		---------------------------------------------------------------------------*/
		public void EndDrawSprites() {
			if (m_sprite_count > 0) {
				draw_sprites();
			}
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기
		 어느 정도 모인 수만큼 그린다
		 텍스쳐는 이미 설정되어 있어야 한다
		---------------------------------------------------------------------------*/
		private void draw_sprites() {
			if (m_effect != null) {
				// 쉐이더사용
				m_vb.SetData<sprite_vertex>(m_vbo);	 // 정점 데이터 설정
														//왠지 느려서 사용 중단

				m_effect.Begin(0);
				m_effect.BeginPass(0);

				m_d3d_device.VertexDeclaration = m_decl;
				m_d3d_device.SetStreamSource(0, m_vb.vb, 0, sprite_vertex.SizeInBytes);
				m_d3d_device.Indices = m_vb.ib;
				m_d3d_device.DrawIndexedPrimitives(PrimitiveType.TriangleList,
											0, 0, m_sprite_count * 4, 0, m_sprite_count * 2);
				m_effect.EndPass();
				m_effect.End();
				m_vb.Flip();			// 다음 버퍼로
			} else {
				// 통상
				// draw primitives
				m_d3d_device.SetTexture(0, m_texture);
				m_d3d_device.VertexFormat = CustomVertex.TransformedColoredTextured.Format;
				m_d3d_device.DrawUserPrimitives(PrimitiveType.TriangleList, m_sprite_count * 2, m_vertex_list);
			}
			m_sprite_count = 0;
		}

		/*-------------------------------------------------------------------------
		 스프라이트 추가
		 vertex buffer 에 추가
		---------------------------------------------------------------------------*/
		private void add_sprite_to_vertex_buffer(Vector3 pos, Vector2 size, d3d_sprite_rects.rect _rect, int color) {
			int index = m_sprite_count * 6;

			pos.X -= 0.5f;
			pos.Y -= 0.5f;

			for (int i = 0; i < 6; i++) {
				m_vertex_list[index + i].X = pos.X;
				m_vertex_list[index + i].Y = pos.Y;
				m_vertex_list[index + i].Z = pos.Z;
				m_vertex_list[index + i].Rhw = 1;
				m_vertex_list[index + i].Color = color;
			}

			// 좌상단 삼각형
			m_vertex_list[index + 1].X += size.X;
			m_vertex_list[index + 2].Y += size.Y;

			// 
			m_vertex_list[index + 0].Tu = _rect.uv[0].X;
			m_vertex_list[index + 0].Tv = _rect.uv[0].Y;
			m_vertex_list[index + 1].Tu = _rect.uv[1].X;
			m_vertex_list[index + 1].Tv = _rect.uv[1].Y;
			m_vertex_list[index + 2].Tu = _rect.uv[2].X;
			m_vertex_list[index + 2].Tv = _rect.uv[2].Y;

			// 우하단 삼각형
			m_vertex_list[index + 3].X += size.X;
			m_vertex_list[index + 4].X += size.X;
			m_vertex_list[index + 4].Y += size.Y;
			m_vertex_list[index + 5].Y += size.Y;

			// 
			m_vertex_list[index + 3].Tu = _rect.uv[1].X;
			m_vertex_list[index + 3].Tv = _rect.uv[1].Y;
			m_vertex_list[index + 4].Tu = _rect.uv[3].X;
			m_vertex_list[index + 4].Tv = _rect.uv[3].Y;
			m_vertex_list[index + 5].Tu = _rect.uv[2].X;
			m_vertex_list[index + 5].Tv = _rect.uv[2].Y;

			m_sprite_count++;
			m_draw_sprites_in_frame++;
		}

		/*-------------------------------------------------------------------------
		 스프라이트 추가
		 쉐이더사용
		---------------------------------------------------------------------------*/
		private void add_sprite_to_vertex_buffer_shader(Vector3 pos, d3d_sprite_rects.rect _rect, float angle_rad, Vector2 scale, int color, Vector2 offset) {
			int index = m_sprite_count * 4;

			Vector3 param = new Vector3(scale.X, scale.Y, angle_rad);
			for (int i = 0; i < 4; i++) {
				m_vbo[index + i].color = color;
				m_vbo[index + i].Position = pos;
				m_vbo[index + i].offset1 = _rect.offset[i];
				m_vbo[index + i].offset2 = offset;
				m_vbo[index + i].param = param;
				m_vbo[index + i].uv = _rect.uv[i];
			}

			m_sprite_count++;
			m_draw_sprites_in_frame++;
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 추가
		 회전지정
		 각도는 라디안
		 화면외컬링없음
		 회전 지정은 화면 밖 컬링 없음일 때만
		---------------------------------------------------------------------------*/
		public bool AddDrawSpritesNC(Vector3 pos, d3d_sprite_rects.rect _rect, float angle_rad) {
			return AddDrawSpritesNC(pos, _rect, angle_rad, new Vector2(1, 1), -1);
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 추가
		 스케일지정, 회전지정
		 각도는 라디안
		 화면외컬링없음
		 회전 지정은 화면 밖 컬링 없음일 때만
		---------------------------------------------------------------------------*/
		public bool AddDrawSpritesNC(Vector3 pos, d3d_sprite_rects.rect _rect, float angle_rad, Vector2 scale) {
			return AddDrawSpritesNC(pos, _rect, angle_rad, scale, -1);
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 추가
		 색지정, 회전지정
		 각도는 라디안
		 화면외컬링없음
		 회전 지정은 화면 밖 컬링 없음일 때만
		---------------------------------------------------------------------------*/
		public bool AddDrawSpritesNC(Vector3 pos, d3d_sprite_rects.rect _rect, float angle_rad, int color) {
			return AddDrawSpritesNC(pos, _rect, angle_rad, new Vector2(1, 1), color);
		}

		/*-------------------------------------------------------------------------
		 스프라이트 그리기 추가
		 스케일지정, 색지정, 회전지정
		 각도는 라디안
		 화면외컬링없음
		 회전 지정은 화면 밖 컬링 없음일 때만
		---------------------------------------------------------------------------*/
		public bool AddDrawSpritesNC(Vector3 pos, d3d_sprite_rects.rect _rect, float angle_rad, Vector2 scale, int color) {
			return AddDrawSpritesNC(pos, _rect, angle_rad, scale, color, new Vector2(0, 0));
		}
		public bool AddDrawSpritesNC(Vector3 pos, d3d_sprite_rects.rect _rect, float angle_rad, Vector2 scale, int color, Vector2 offset2) {
			if (m_effect != null) {
				// 쉐이더사용
				add_sprite_to_vertex_buffer_shader(pos, _rect, angle_rad, scale, color, offset2);
			} else {
				pos = goffsetscale(pos);
				Vector2 scale2 = gscale(scale);

				Matrix mat = Matrix.RotationZ(angle_rad);		   // Z 축 회전
				mat = Matrix.Scaling(scale2.X, scale2.Y, 1) * mat;  // 확대 축소

				Vector3 p0 = new Vector3(_rect.offset[0].X, _rect.offset[0].Y, 0);
				Vector3 p1 = new Vector3(_rect.offset[1].X, _rect.offset[1].Y, 0);
				Vector3 p2 = new Vector3(_rect.offset[2].X, _rect.offset[2].Y, 0);
				Vector3 p3 = new Vector3(_rect.offset[3].X, _rect.offset[3].Y, 0);

				// 좌표변환
				p0.TransformCoordinate(mat);
				p1.TransformCoordinate(mat);
				p2.TransformCoordinate(mat);
				p3.TransformCoordinate(mat);

				int index = m_sprite_count * 6;

				pos.X = (int)pos.X;
				pos.Y = (int)pos.Y;
				pos.X += offset2.X;
				pos.Y += offset2.Y;

				pos.X -= 0.5f;
				pos.Y -= 0.5f;

				for (int i = 0; i < 6; i++) {
					m_vertex_list[index + i].X = pos.X;
					m_vertex_list[index + i].Y = pos.Y;
					m_vertex_list[index + i].Z = pos.Z;
					m_vertex_list[index + i].Rhw = 1;
					m_vertex_list[index + i].Color = color;
				}

				// 좌상단 삼각형
				m_vertex_list[index + 0].X += p0.X;
				m_vertex_list[index + 0].Y += p0.Y;
				m_vertex_list[index + 1].X += p1.X;
				m_vertex_list[index + 1].Y += p1.Y;
				m_vertex_list[index + 2].X += p2.X;
				m_vertex_list[index + 2].Y += p2.Y;

				// 
				m_vertex_list[index + 0].Tu = _rect.uv[0].X;
				m_vertex_list[index + 0].Tv = _rect.uv[0].Y;
				m_vertex_list[index + 1].Tu = _rect.uv[1].X;
				m_vertex_list[index + 1].Tv = _rect.uv[1].Y;
				m_vertex_list[index + 2].Tu = _rect.uv[2].X;
				m_vertex_list[index + 2].Tv = _rect.uv[2].Y;

				// 우하단 삼각형
				m_vertex_list[index + 3].X += p1.X;
				m_vertex_list[index + 3].Y += p1.Y;
				m_vertex_list[index + 4].X += p3.X;
				m_vertex_list[index + 4].Y += p3.Y;
				m_vertex_list[index + 5].X += p2.X;
				m_vertex_list[index + 5].Y += p2.Y;

				// 
				m_vertex_list[index + 3].Tu = _rect.uv[1].X;
				m_vertex_list[index + 3].Tv = _rect.uv[1].Y;
				m_vertex_list[index + 4].Tu = _rect.uv[3].X;
				m_vertex_list[index + 4].Tv = _rect.uv[3].Y;
				m_vertex_list[index + 5].Tu = _rect.uv[2].X;
				m_vertex_list[index + 5].Tv = _rect.uv[2].Y;

				m_sprite_count++;
				m_draw_sprites_in_frame++;
			}

			if (m_sprite_count >= DRAW_SPRITE_ONCE) {
				draw_sprites();
			}
			return true;
		}

		/*-------------------------------------------------------------------------
		 쉐이더용 정점
		---------------------------------------------------------------------------*/
		[StructLayout(LayoutKind.Sequential)]
		protected struct sprite_vertex {
			public Vector3 Position;   // 좌표					12
			public Vector2 uv;		  // uv					8
			public Vector2 offset1; // x, y		offset1		8
			public Vector2 offset2; // x, y		offset2		8
			public Vector3 param;	   // x, y		scale		12
										// z		angle_rad
			public int color;	   // color				4

			/*-------------------------------------------------------------------------
			 정점 정보
			---------------------------------------------------------------------------*/
			public static VertexElement[] VertexElements = {
				// Position (12바이트)
				new VertexElement(0, 0, DeclarationType.Float3,
										DeclarationMethod.Default,
										DeclarationUsage.Position, 0),
				// uv (8바이트)
				new VertexElement(0, 12, DeclarationType.Float2,
										DeclarationMethod.Default,
										DeclarationUsage.TextureCoordinate, 0),
				// offset1 offset2
				new VertexElement(0, 12+8, DeclarationType.Float4,
										DeclarationMethod.Default,
										DeclarationUsage.TextureCoordinate, 1),
				// scale angle_rad
				new VertexElement(0, 12+8+16, DeclarationType.Float3,
										DeclarationMethod.Default,
										DeclarationUsage.TextureCoordinate, 2),

				// Color  (4바이트)
				new VertexElement(0, 12+8+16+12, DeclarationType.Color,
										DeclarationMethod.Default,
										DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd,
			};

			/*-------------------------------------------------------------------------
			 정점 하나의 크기
			---------------------------------------------------------------------------*/
			public static int SizeInBytes {
				get { return Marshal.SizeOf(typeof(sprite_vertex)); }
			}
		}
	}
}
