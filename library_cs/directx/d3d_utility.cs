/*-------------------------------------------------------------------------

 DirectX用ユーティリティ
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
		 텍스쳐のサイズを得る
		 LevelDescription 0 のサイズを返す 
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
		 텍스쳐を作成する
		 텍스쳐はManagedで作成される
		 시스템메모リに作成した텍스쳐を사용できる텍스쳐にするのに사용する
		 bmp등から切り出した텍스쳐を作成する場合は, 
		 시스템메모リに텍스쳐を作成し, この関数でManagedに転送する
		 Managedで作成した텍스쳐をロックしてはいけない
		 
		 텍스쳐はできるだけDDSで作成し, TextureLoaderを使って로드こと
		 TextureLoaderはManagedな텍스쳐を作成してくれる
		---------------------------------------------------------------------------*/
		static public Texture CreateTextureFromTexture(Device device, Texture src_texture)
		{
			SurfaceDescription	d	= src_texture.GetLevelDescription(0);
			return CreateTextureFromTexture(device, src_texture, d.Format);
		}

		/*-------------------------------------------------------------------------
		 フォーマット指定版
		 フォーマット変換は기본的には時間が掛かるので注意
		 フォーマット変換に실패するかもしれない
		---------------------------------------------------------------------------*/
		static public Texture CreateTextureFromTexture(Device device, Texture src_texture, Format format)
		{
			if(device	== null)		return null;
			if(src_texture == null)		return null;

			try{
				// 同じサイズの텍스쳐を作成する
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
		 渡された텍스쳐と同じサイズの텍스쳐を作成する
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
			// Managedで作成する
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
				// 作成실패
				return null;
			}
		}
		/*-------------------------------------------------------------------------
		 渡された텍스쳐と同じサイズの텍스쳐を作成する
		 レンダーターゲットとして作成する
		---------------------------------------------------------------------------*/
		static public Texture CreateRenderTargetTextureSameSize(Device device, Texture src_texture)
		{
			SurfaceDescription	d	= src_texture.GetLevelDescription(0);
			return CreateTextureSameSize(device, src_texture, Usage.RenderTarget, d.Format, Pool.Default);
		}
	
		/*-------------------------------------------------------------------------
		 텍스쳐コピー
		 例えば시스템메모リ上の텍스쳐をVRAMに転送する등
		 サイズ등はチェックしないので注意
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
		 텍스쳐サイズを2のべき乗に調整する
		---------------------------------------------------------------------------*/
		static public Size TextureSizePow2(Size size)
		{
			Size ret	= new Size();
			ret.Width	= size_pow2(size.Width);
			ret.Height	= size_pow2(size.Height);
			return ret;
		}

		/*-------------------------------------------------------------------------
		 サイズを2のべき乗に調整する
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
