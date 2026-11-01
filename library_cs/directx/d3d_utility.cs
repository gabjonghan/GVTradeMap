/*-------------------------------------------------------------------------

 DirectX 용 유틸리티
 전부 static 메서드

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace directx
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	static public class d3d_utility
	{
		/*-------------------------------------------------------------------------
		 텍스쳐 크기를 얻는다
		 LevelDescription 0 의 크기를 반환한다
		 tex==null 일 때 Vector2(0,0) 을 반환한다
		---------------------------------------------------------------------------*/
		static public Vector2 GetTextureSize(Texture tex)
		{
			if(tex == null)	return new Vector2(0, 0);

			try{
				SurfaceDescription	dsc	= tex.GetLevelDescription(0);
				return new Vector2(dsc.Width, dsc.Height);
			}catch{
				return new Vector2(0, 0);
			}
		}

		/*-------------------------------------------------------------------------
		 텍스쳐를 작성한다
		 텍스쳐는 Managed 로 작성된다
		 시스템 메모리에 작성한 텍스쳐를 사용 가능한 텍스쳐로 만들 때 사용한다
		 bmp 등에서 잘라낸 텍스쳐를 작성할 경우에는,
		 시스템 메모리에 텍스쳐를 작성하고, 이 함수로 Managed 에 전송한다
		 Managed 로 작성한 텍스쳐를 잠가서는 안 된다
		 
		 텍스쳐는 되도록 DDS 로 작성하고, TextureLoader 를 사용해 로드할 것
		 TextureLoader 는 Managed 인 텍스쳐를 작성해 준다
		---------------------------------------------------------------------------*/
		static public Texture CreateTextureFromTexture(Device device, Texture src_texture)
		{
			SurfaceDescription	d	= src_texture.GetLevelDescription(0);
			return CreateTextureFromTexture(device, src_texture, d.Format);
		}

		/*-------------------------------------------------------------------------
		 포맷 지정 버전
		 포맷 변환은 기본적으로 시간이 걸리므로 주의
		 포맷 변환에 실패할 수도 있다
		---------------------------------------------------------------------------*/
		static public Texture CreateTextureFromTexture(Device device, Texture src_texture, Format format)
		{
			if(device	== null)		return null;
			if(src_texture == null)		return null;

			try{
				// 같은 크기의 텍스쳐를 작성한다
				Texture dst_texture		= CreateTextureSameSize(device, src_texture, format);
				if(dst_texture == null){
					// 실패
					return null;
				}

				// 복사
				if(CopyTexture(device, dst_texture, src_texture)){
					return dst_texture;
				}else{
					dst_texture.Dispose();
					return null;
				}
			}catch{
				return null;
			}
		}

		/*-------------------------------------------------------------------------
		 넘겨받은 텍스쳐와 같은 크기의 텍스쳐를 작성한다
		---------------------------------------------------------------------------*/
		static public Texture CreateTextureSameSize(Device device, Texture src_texture)
		{
			SurfaceDescription	d	= src_texture.GetLevelDescription(0);
			return CreateTextureSameSize(device, src_texture, d.Format);
		}
		static public Texture CreateTextureSameSize(Device device, Texture src_texture, Pool pool)
		{
			SurfaceDescription	d	= src_texture.GetLevelDescription(0);
			return CreateTextureSameSize(device, src_texture, Usage.None, d.Format, pool);
		}
		static public Texture CreateTextureSameSize(Device device, Texture src_texture, Format format)
		{
			// Managed 로 작성한다
			return CreateTextureSameSize(device, src_texture, Usage.None, format, Pool.Managed);
		}
		static public Texture CreateTextureSameSize(Device device, Texture src_texture, Usage usage, Format format, Pool pool)
		{
			Vector2	size	= d3d_utility.GetTextureSize(src_texture);
			try{
				Texture tex	= new Texture(device, (int)size.X, (int)size.Y,
											1, usage, format, pool);
				return tex;
			}catch{
				// 작성실패
				return null;
			}
		}
		/*-------------------------------------------------------------------------
		 넘겨받은 텍스쳐와 같은 크기의 텍스쳐를 작성한다
		 렌더링 타겟으로 작성한다
		---------------------------------------------------------------------------*/
		static public Texture CreateRenderTargetTextureSameSize(Device device, Texture src_texture)
		{
			SurfaceDescription	d	= src_texture.GetLevelDescription(0);
			return CreateTextureSameSize(device, src_texture, Usage.RenderTarget, d.Format, Pool.Default);
		}
	
		/*-------------------------------------------------------------------------
		 텍스쳐 복사
		 예를 들어 시스템 메모리상의 텍스쳐를 VRAM 에 전송하는 등
		 크기 등은 확인하지 않으므로 주의
		---------------------------------------------------------------------------*/
		static public bool CopyTexture(Device device, Texture dst_texture, Texture src_texture)
		{
			if(device	== null)		return false;
			if(src_texture == null)		return false;
			if(dst_texture == null)		return false;

			try{
				// 단순히 복사
				Surface		dst	= dst_texture.GetSurfaceLevel(0);
				Surface		src	= src_texture.GetSurfaceLevel(0);
				SurfaceLoader.FromSurface(dst, src, Filter.None, 0);
				dst.Dispose();
				src.Dispose();
				return true;
			}catch{
				return false;
			}
		}

		/*-------------------------------------------------------------------------
		 텍스쳐 크기를 2 의 거듭제곱으로 조정한다
		---------------------------------------------------------------------------*/
		static public Size TextureSizePow2(Size size)
		{
			Size ret	= new Size();
			ret.Width	= size_pow2(size.Width);
			ret.Height	= size_pow2(size.Height);
			return ret;
		}

		/*-------------------------------------------------------------------------
		 크기를 2 의 거듭제곱으로 조정한다
		---------------------------------------------------------------------------*/
		static private int size_pow2(int size)
		{
			int	pow2	= 2;

			for(int i=0; i<32-2; i++){
				if(size <= pow2)	return pow2;
				pow2	<<= 1;
			}
			return size;
		}
	}
}
