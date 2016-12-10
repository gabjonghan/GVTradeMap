/*-------------------------------------------------------------------------

 Direct3D

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 define
---------------------------------------------------------------------------*/
// 쉐이더を強制的に사용しない
//#define	NOUSE_SHADER

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Windows.Forms;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace directx
{
    public enum font_type{
		normal,
		small,
	};

	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class d3d_device : d3d_base_device
	{
		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private Microsoft.DirectX.Direct3D.Font		m_font;
		private Microsoft.DirectX.Direct3D.Font		m_font_small;
		private Line								m_line;
		private Sprite								m_font_sprite;
		private bool								m_is_use_font_sprite;
	
		// 쉐이더버전
		// VS1.1 PS1.1が使えるかどうか
		private bool								m_is_use_ve1_1_ps1_1;
		private string								m_device_info_string;
		private string								m_device_info_string_short;

		// 그리기支援
		private	d3d_point							m_points;
		private d3d_sprite							m_sprite;

		private d3d_systemfont						m_systemfont;
		private d3d_textured_font					m_textured_font;

		private Vector2								m_client_size;

		// 그리기스킵
		// Present()が呼ばれるとクリアされる
		private int									m_skip_count;		// 스킵フレーム数
		private int									m_skip_max;			// 스킵する최대フレーム数
		private bool								m_is_must_draw;		// 無条건に그리기する場合true
		
		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public Line line							{	get{	return m_line;				}}
		public d3d_systemfont systemfont			{	get{	return m_systemfont;		}}
		public d3d_textured_font textured_font		{	get{	return m_textured_font;		}}

		public Vector2 client_size					{	get{	return m_client_size;		}}

		public d3d_point points						{	get{	return m_points;		}}
		public d3d_sprite sprites					{	get{	return m_sprite;		}}
#if NOUSE_SHADER
		public bool is_use_ve1_1_ps1_1				{	get{	return false;					}}
#else
		public bool is_use_ve1_1_ps1_1				{	get{	return m_is_use_ve1_1_ps1_1;	}}
#endif
		public string deviec_info_string			{	get{	return m_device_info_string;		}}
		public string deviec_info_string_short		{	get{	return m_device_info_string_short;	}}
		public bool now_use_shader					{	get{
																if(!is_use_ve1_1_ps1_1)		return false;
																if(m_sprite.effect == null)	return false;
																return true;
														}
													}

		public int skip_count						{	get{	return m_skip_count;	}}
		public int skip_max							{	get{	return m_skip_max;		}
														set{	m_skip_max	= value;	}}

		/*-------------------------------------------------------------------------
		 初期化
		---------------------------------------------------------------------------*/
		public d3d_device(System.Windows.Forms.Form form)
			: base()
		{
			// 作成パラメータ
			PresentParameters	param		= new PresentParameters();
			param.Windowed					= true;
			param.SwapEffect				= SwapEffect.Discard;
			param.PresentationInterval		= PresentInterval.Immediate;
			param.EnableAutoDepthStencil	= true;
			param.AutoDepthStencilFormat	= DepthFormat.D16;
			param.BackBufferCount			= 1;
			param.BackBufferFormat			= Format.Unknown;

			// デバイスを作成
			try{
				base.Create(form, param);
			}catch{
				MessageBox.Show("DirectX 초기화에 실패하였습니다. ", "초기화오류");
				return;
			}
	
			try{
				// リセット時のデリゲート追加
				base.device.DeviceReset	+= new System.EventHandler(device_reset);
				// 쉐이더の버전を調べる
				check_shader_support();

				// フォントを作成
				m_font			= new Microsoft.DirectX.Direct3D.Font(base.device, new System.Drawing.Font("MS UI Gothic", 9));
				m_font_small	= new Microsoft.DirectX.Direct3D.Font(base.device, new System.Drawing.Font("MS UI Gothic", 8));
// CreaTypeで그리기できないのでメイリオはやめ
//				m_font			= new Microsoft.DirectX.Direct3D.Font(base.device, new System.Drawing.Font("メイリオ", 9));
//				m_font_small	= new Microsoft.DirectX.Direct3D.Font(base.device, new System.Drawing.Font("メイリオ", 7));

				m_font_sprite			= new Sprite(base.device);
				m_is_use_font_sprite	= false;

				// ライン그리기
				m_line	= new Line(base.device);

				// ステートの初期化
				device_reset(this, null);

				// 그리기支援
				m_points			= new d3d_point(device);
				m_sprite			= new d3d_sprite(device, is_use_ve1_1_ps1_1);
	
				// 시스템フォント
				m_systemfont		= new d3d_systemfont(this);
				// 텍스쳐化されたフォント
				// あまり変動しない文字列그리기用
				m_textured_font		= new d3d_textured_font(this, m_font);

				// デバイスの정보を得る
				get_device_information();
				// クライアントサイズ更新
				UpdateClientSize();
			}catch{
				MessageBox.Show("DirectX 초기화후 설정에 실패하였습니다. ", "초기화오류");
				base.Dispose();
			}

			m_skip_count	= 0;
			m_skip_max		= 0;
			m_is_must_draw	= false;
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public override void Dispose()
		{
			if(m_textured_font != null)		m_textured_font.Dispose();
			if(m_systemfont != null)		m_systemfont.Dispose();
			if(m_sprite != null)			m_sprite.Dispose();
			if(m_font_sprite != null)		m_font_sprite.Dispose();
			if(m_line != null)				m_line.Dispose();
			if(m_font_small != null)		m_font_small.Dispose();
			if(m_font != null)				m_font.Dispose();

			m_textured_font		= null;
			m_systemfont		= null;
			m_sprite			= null;
			m_font_sprite		= null;
			m_line				= null;
			m_font_small		= null;
			m_font				= null;

			// device
			base.Dispose();
		}

		/*-------------------------------------------------------------------------
		 쉐이더が사용可能か調べる
		---------------------------------------------------------------------------*/
		private void check_shader_support()
		{
			// 버전が쉐이더 1.1 以降であることを確認する
			Version	v1_1	= new Version(1,1);

			// デバイスの能力
			Caps	caps	= base.caps;

			// サ포트される쉐이더の버전を確認します. 
			if((caps.VertexShaderVersion >= v1_1) && (caps.PixelShaderVersion >= v1_1)){
				m_is_use_ve1_1_ps1_1	= true;
			}else{
				m_is_use_ve1_1_ps1_1	= false;
			}
		}
	
		/*-------------------------------------------------------------------------
		 デバイス정보を得る
		 単純に文字列とする
		---------------------------------------------------------------------------*/
		private void get_device_information()
		{
			Caps			caps		= base.caps;
			AdapterDetails	details		= Manager.Adapters[base.adpter_index].Information;
			
			// 短い版
			m_device_info_string_short	= details.Description;

			// 통상
			m_device_info_string	= details.Description + "\n";
			m_device_info_string	+= "VertexShader: " + caps.VertexShaderVersion.ToString() + "  ";
			m_device_info_string	+= "PixelShader: " + caps.PixelShaderVersion.ToString() + "\n";

			m_device_info_string	+= "정점처리:";
			if((base.create_flags & CreateFlags.HardwareVertexProcessing) != 0){
				// ハードウェアで頂点変換に対応している
				m_device_info_string	+= "HardwareVertexProcessing";
				if((base.create_flags & CreateFlags.PureDevice) != 0){
					// ピュアデバイス
					m_device_info_string	+= "(PureDevice)\n";
				}else{
					m_device_info_string	+= "\n";
				}
			}else{
				// ハードウェアで頂点変換に対応していない
				if((base.create_flags & CreateFlags.SoftwareVertexProcessing) != 0){
					m_device_info_string	+= "SoftwareVertexProcessing\n";
				}
			}
			m_device_info_string	+= "Vertex/Pixel Shader:";
			m_device_info_string	+= (now_use_shader)? "유효": "무효";
		}
	
		/*-------------------------------------------------------------------------
		 デバイスリセット時の初期化
		---------------------------------------------------------------------------*/
		private void device_reset(object sender, System.EventArgs e)
		{
			// 適当にステート설정
			// 통상の半透明설정
			//	頂点カラーでの半透明유효
			//	텍스쳐に含まれる半透明유효
			// 裏面ポリゴンのカリングなし
			// Zバッファ유효
			// ライティング무효
			// バイリニアフィルタ
			// UVクランプ
			base.device.RenderState.CullMode				= Cull.None;
			base.device.RenderState.Lighting				= false;
//			base.device.RenderState.ZBufferEnable			= false;
			base.device.RenderState.ZBufferEnable			= true;
			base.device.RenderState.ZBufferFunction			= Compare.LessEqual;
			base.device.RenderState.AlphaBlendEnable		= true;
			base.device.RenderState.AlphaTestEnable			= true;
			base.device.RenderState.AlphaFunction			= Compare.Greater;
			base.device.RenderState.ReferenceAlpha			= 0;
			base.device.RenderState.SourceBlend				= Blend.SourceAlpha;
			base.device.RenderState.DestinationBlend		= Blend.InvSourceAlpha;
			base.device.SamplerState[0].MagFilter			= TextureFilter.Linear;
			base.device.SamplerState[0].MinFilter			= TextureFilter.Linear;
			base.device.SamplerState[0].AddressU			= TextureAddress.Clamp;
			base.device.SamplerState[0].AddressV			= TextureAddress.Clamp;
			base.device.TextureState[0].AlphaOperation		= TextureOperation.Modulate;
			base.device.TextureState[0].AlphaArgument1		= TextureArgument.TextureColor;
			base.device.TextureState[0].AlphaArgument2		= TextureArgument.Current;

//			UpdateFontPreLoadCharactor();
		}

		/*-------------------------------------------------------------------------
		 無条건に그리기するフラグを설정する
		---------------------------------------------------------------------------*/
		public void SetMustDrawFlag()
		{
			m_is_must_draw	= true;
		}

		/*-------------------------------------------------------------------------
		 그리기する必要があるかどうかを得る
		   無条건に그리기するフラグが설정されている
		   스킵数がskip_max이상
		 のときtrueを返す
		---------------------------------------------------------------------------*/
		public bool IsNeedDraw()
		{
			if(m_is_must_draw)				return true;
			if(m_skip_count >= m_skip_max)	return true;
			m_skip_count++;	// 스킵数を増やす
			return false;	// 그리기する必要がない
		}

		/*-------------------------------------------------------------------------
		 그리기開始
		---------------------------------------------------------------------------*/
		public override bool Begin()
		{
			if(!base.Begin())	return false;

			UpdateClientSize();
			m_sprite.BeginFrame();
			m_systemfont.BeginFrame();
			return true;
		}

		/*-------------------------------------------------------------------------
		 present
		---------------------------------------------------------------------------*/
		public override bool Present()
		{
			m_skip_count	= 0;
			m_is_must_draw	= false;

			return base.Present();
		}

		/*-------------------------------------------------------------------------
		 クライアントサイズの更新
		---------------------------------------------------------------------------*/
		public void UpdateClientSize()
		{
			if(base.device != null){
				m_client_size	= new Vector2(	base.device.Viewport.Width,
												base.device.Viewport.Height);
			}else{
				m_client_size	= new Vector2(100, 100);
			}
		}

		/*-------------------------------------------------------------------------
		 텍스쳐の그리기
		 텍스쳐をsizeの대きさで그리기する
		 UVは指定できない
		 sizeと텍스쳐のサイズが同じでない場合スケーリングされる
		---------------------------------------------------------------------------*/
		public void DrawTexture(Texture tex, Vector3 pos, Vector2 size)
		{
			DrawTexture(tex, pos, size, Color.White.ToArgb());
		}
		public void DrawTexture(Texture tex, Vector3 pos, Vector2 size, int color)
		{
			CustomVertex.TransformedColoredTextured[]	vb	= new CustomVertex.TransformedColoredTextured[4];
										
			pos.X	-= 0.5f;
			pos.Y	-= 0.5f;
	
			for(int i=0; i<4; i++){
				vb[i].Color	= color;
				vb[i].Rhw	= 1f;
				vb[i].Z		= pos.Z;
			}
			vb[0].X		= pos.X;
			vb[0].Y		= pos.Y;
			vb[0].Tu	= 0;
			vb[0].Tv	= 0;

			vb[1].X		= pos.X + size.X;
			vb[1].Y		= pos.Y;
			vb[1].Tu	= 1;
			vb[1].Tv	= 0;

			vb[2].X		= pos.X;
			vb[2].Y		= pos.Y + size.Y;
			vb[2].Tu	= 0;
			vb[2].Tv	= 1;
	
			vb[3].X		= pos.X + size.X;
			vb[3].Y		= pos.Y + size.Y;
			vb[3].Tu	= 1;
			vb[3].Tv	= 1;

			base.device.VertexFormat	= CustomVertex.TransformedColoredTextured.Format;
			base.device.SetTexture(0, tex);
			base.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, vb);
		}

		/*-------------------------------------------------------------------------
		 単色矩形の그리기
		 フィルされた矩形
		---------------------------------------------------------------------------*/
		public void DrawFillRect(Vector3 pos, Vector2 size, int color)
		{
			CustomVertex.TransformedColored[]	vb	= new CustomVertex.TransformedColored[4];
									
			for(int i=0; i<4; i++){
				vb[i].Color	= color;
				vb[i].Rhw	= 1f;
				vb[i].Z		= pos.Z;
			}
			vb[0].X		= pos.X;
			vb[0].Y		= pos.Y;

			vb[1].X		= pos.X + size.X;
			vb[1].Y		= pos.Y;

			vb[2].X		= pos.X;
			vb[2].Y		= pos.Y + size.Y;
	
			vb[3].X		= pos.X + size.X;
			vb[3].Y		= pos.Y + size.Y;

			base.device.VertexFormat	= CustomVertex.TransformedColored.Format;
			base.device.SetTexture(0, null);
			base.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, vb);
		}
	
		/*-------------------------------------------------------------------------
		 矩形の그리기
		 フィルされない
		---------------------------------------------------------------------------*/
		public void DrawLineRect(Vector3 pos, Vector2 size, int color)
		{
			CustomVertex.TransformedColored[]	vb	= new CustomVertex.TransformedColored[5];
									
			for(int i=0; i<5; i++){
				vb[i].Color	= color;
				vb[i].Rhw	= 1f;
				vb[i].Z		= pos.Z;
			}

//			pos.X		-= 0.5f;
//			pos.Y		-= 0.5f;
	
			vb[0].X		= pos.X;
			vb[0].Y		= pos.Y;
/*
			vb[1].X		= pos.X + Size.X - 1;
			vb[1].Y		= pos.Y;

			vb[2].X		= pos.X + Size.X - 1;
			vb[2].Y		= pos.Y + Size.Y - 1;

			vb[3].X		= pos.X;
			vb[3].Y		= pos.Y + Size.Y - 1;
*/
			vb[1].X		= pos.X + size.X;
			vb[1].Y		= pos.Y;

			vb[2].X		= pos.X + size.X;
			vb[2].Y		= pos.Y + size.Y;

			vb[3].X		= pos.X;
			vb[3].Y		= pos.Y + size.Y;

			vb[4].X		= pos.X;
			vb[4].Y		= pos.Y;

			base.device.VertexFormat	= CustomVertex.TransformedColored.Format;
			base.device.SetTexture(0, null);
			base.device.DrawUserPrimitives(PrimitiveType.LineStrip, 4, vb);
		}

		/*-------------------------------------------------------------------------
		 선の그리기
		---------------------------------------------------------------------------*/
		public void DrawLine(Vector3 pos, Vector2 pos2, int color)
		{
			CustomVertex.TransformedColored[]	vb	= new CustomVertex.TransformedColored[2];
									
			vb[0].X		= pos.X;
			vb[0].Y		= pos.Y;
			vb[0].Rhw	= 1f;
			vb[0].Z		= pos.Z;
			vb[0].Color	= color;

			vb[1].X		= pos2.X;
			vb[1].Y		= pos2.Y;
			vb[1].Rhw	= 1f;
			vb[1].Z		= pos.Z;
			vb[1].Color	= color;

			base.device.VertexFormat	= CustomVertex.TransformedColored.Format;
			base.device.SetTexture(0, null);
			base.device.DrawUserPrimitives(PrimitiveType.LineList, 1, vb);
		}

		/*-------------------------------------------------------------------------
		 선の그리기
		---------------------------------------------------------------------------*/
		public void DrawLineStrip(Vector3 pos, Vector2[] vec, int color)
		{
			CustomVertex.TransformedColored[]	vb	= new CustomVertex.TransformedColored[vec.Length];

			for(int i=0; i<vec.Length; i++){
				vb[i].X		= pos.X + vec[i].X;
				vb[i].Y		= pos.Y + vec[i].Y;
				vb[i].Z		= pos.Z;
				vb[i].Rhw	= 1f;
				vb[i].Color	= color;
			}

			base.device.VertexFormat	= CustomVertex.TransformedColored.Format;
			base.device.SetTexture(0, null);
			base.device.DrawUserPrimitives(PrimitiveType.LineStrip, vec.Length -1, vb);
		}

		/*-------------------------------------------------------------------------
		 文字列の그리기開始
		 複数の文字列を그리기するとき用
		---------------------------------------------------------------------------*/
		public void BeginFont()
		{
			m_font_sprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortTexture);
			m_is_use_font_sprite	= true;
		}
	
		/*-------------------------------------------------------------------------
		 文字列の그리기終了
		 複数の文字列を그리기するとき用
		---------------------------------------------------------------------------*/
		public void EndFont()
		{
			m_font_sprite.End();
			m_is_use_font_sprite	= false;
		}
		
		/*-------------------------------------------------------------------------
		 文字列の그리기
		---------------------------------------------------------------------------*/
		public void DrawText(font_type type, string str, int x, int y, Color color)
		{
			Microsoft.DirectX.Direct3D.Font	font;
			if(type == font_type.normal)		font	= m_font;
			else								font	= m_font_small;
			Sprite	sprite	= (m_is_use_font_sprite)? m_font_sprite: null;
			font.DrawText(sprite, str, new Point(x, y+1), color);
		}

		/*-------------------------------------------------------------------------
		 文字列の그리기時のサイズを得る
		---------------------------------------------------------------------------*/
		public Rectangle MeasureText(font_type type, string str, Color color)
		{
			Microsoft.DirectX.Direct3D.Font	font;
			if(type == font_type.normal)		font	= m_font;
			else								font	= m_font_small;
			Sprite	sprite	= (m_is_use_font_sprite)? m_font_sprite: null;
			return font.MeasureString(sprite, str, DrawTextFormat.None, color);
		}

		/*-------------------------------------------------------------------------
		 文字列の그리기
		 右詰め
		 xで終わるように그리기される
		---------------------------------------------------------------------------*/
		public void DrawTextR(font_type type, string str, int x, int y, Color color)
		{
			Microsoft.DirectX.Direct3D.Font	font;
			if(type == font_type.normal)		font	= m_font;
			else								font	= m_font_small;

			Sprite	sprite	= (m_is_use_font_sprite)? m_font_sprite: null;
			Rectangle rect	= font.MeasureString(sprite, str, DrawTextFormat.None, color);
			font.DrawText(sprite, str, new Point(x - rect.Width, y+1), color);
		}

		/*-------------------------------------------------------------------------
		 文字列の그리기
		 センタリング
		 xが真中にくるように그리기される
		---------------------------------------------------------------------------*/
		public void DrawTextC(font_type type, string str, int x, int y, Color color)
		{
			Microsoft.DirectX.Direct3D.Font	font;
			if(type == font_type.normal)		font	= m_font;
			else								font	= m_font_small;

			Sprite	sprite	= (m_is_use_font_sprite)? m_font_sprite: null;
			Rectangle rect	= font.MeasureString(sprite, str, DrawTextFormat.None, color);
			font.DrawText(sprite, str, new Point(x - (rect.Width / 2), y+1), color);
		}

		/*-------------------------------------------------------------------------
		 マウス위치を得る
		 クライアント좌표を返す
		---------------------------------------------------------------------------*/
		public Point GetClientMousePosition()
		{
			return base.form.PointToClient(System.Windows.Forms.Control.MousePosition);
		}
	}
}
