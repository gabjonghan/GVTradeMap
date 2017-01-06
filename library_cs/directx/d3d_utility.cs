/*-------------------------------------------------------------------------

 DirectX용ユーティリティ
 全てstaticメソッド

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
		 텍스쳐の사이즈を得る
		 LevelDescription 0 の사이즈を返す 
		 tex==null のとき Vector2(0,0) を返す
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
		 텍스쳐を작성함
		 텍스쳐はManagedで작성される
		 시스템메모リに작성した텍스쳐を사용できる텍스쳐にするのに사용する
		 bmp등から切り出した텍스쳐を작성함場合は, 
		 시스템메모リに텍스쳐を작성し, この関수でManagedに전送する
		 Managedで작성した텍스쳐をロックしてはいけない
		 
		 텍스쳐はできるだけDDSで작성し, TextureLoaderを使って로드こと
		 TextureLoaderはManagedな텍스쳐を작성してくれる
		---------------------------------------------------------------------------*/
		static public Texture CreateTextureFromTexture(Device device, Texture src_texture)
		{
			SurfaceDescription	d	= src_texture.GetLevelDescription(0);
			return CreateTextureFromTexture(device, src_texture, d.Format);
		}

		/*-------------------------------------------------------------------------
		 フォーマット지정版
		 フォーマット변환は기본的には시간が掛かるので注意
		 フォーマット변환に실패するかもしれない
		---------------------------------------------------------------------------*/
		static public Texture CreateTextureFromTexture(Device device, Texture src_texture, Format format)
		{
			if(device	== null)		return null;
			if(src_texture == null)		return null;

			try{
				// 同じ사이즈の텍스쳐を작성함
				Texture dst_texture		= CreateTextureSameSize(device, src_texture, format);
				if(dst_texture == null){
					// 실패
					return null;
				}

				// コピー
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
		 渡された텍스쳐と同じ사이즈の텍스쳐を작성함
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
			// Managedで작성함
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
		 渡された텍스쳐と同じ사이즈の텍스쳐を작성함
		 렌더링 타겟として작성함
		---------------------------------------------------------------------------*/
		static public Texture CreateRenderTargetTextureSameSize(Device device, Texture src_texture)
		{
			SurfaceDescription	d	= src_texture.GetLevelDescription(0);
			return CreateTextureSameSize(device, src_texture, Usage.RenderTarget, d.Format, Pool.Default);
		}
	
		/*-------------------------------------------------------------------------
		 텍스쳐コピー
		 例えば시스템메모リ상の텍스쳐をVRAMに전送する등
		 사이즈등はチェックしないので注意
		---------------------------------------------------------------------------*/
		static public bool CopyTexture(Device device, Texture dst_texture, Texture src_texture)
		{
			if(device	== null)		return false;
			if(src_texture == null)		return false;
			if(dst_texture == null)		return false;

			try{
				// 単純にコピー
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
		 텍스쳐사이즈を2のべき乗に조정する
		---------------------------------------------------------------------------*/
		static public Size TextureSizePow2(Size size)
		{
			Size ret	= new Size();
			ret.Width	= size_pow2(size.Width);
			ret.Height	= size_pow2(size.Height);
			return ret;
		}

		/*-------------------------------------------------------------------------
		 사이즈を2のべき乗に조정する
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
