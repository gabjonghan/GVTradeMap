/*-------------------------------------------------------------------------

 장소표시

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Collections.Generic;
using System;

using directx;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class spot
	{
		public enum type{
			none,						// 장소なし
			country_flags,				// 국기표시
			icons_0,					// 看판娘
			icons_1,					// 서고
			icons_2,					// 번역가
			icons_3,					// 무역상인
			has_item,					// 지정した아이템がある
			language,					// 언어

			tab_0,						// 교역
			tab_1,						// 도구
			tab_2,						// 공방
			tab_3,						// 인물
			tab_4,						// 조선소주인
			tab_4_1,					// 조선공
			tab_5,						// 무기장인
			tab_6,						// 제재소장인
			tab_7,						// 돛제작자
			tab_8,						// 조각가
			tab_9,						// 행상인
			tab_10,						// 메모

			city_name,					// 도시명
			cultural_sphere,			// 문화권
		};

		/*-------------------------------------------------------------------------
		 외부참조용장소내용
		---------------------------------------------------------------------------*/
		public class spot_once
		{
			private GvoWorldInfo.Info			m_info;				// 장소대상
			private string					m_name;				// 이름
			private string					m_ex;				// 추가する문자열
			
			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public GvoWorldInfo.Info	info	{	get{	return m_info;		}}
			public string			name	{	get{	return m_name;		}}
			public string			ex		{	get{	return m_ex;		}}
	
			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public spot_once(GvoWorldInfo.Info info, string name, string ex)
			{
				m_info		= info;
				m_name		= name;
				m_ex		= ex;
			}
		}
		
		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private d3d_device					m_device;
		private GvoWorldInfo					m_world;
		private icons						m_icons;
		private LoopXImage				m_loop_image;

		private List<GvoWorldInfo.Info>		m_spots;			// 장소대상
		private type						m_spot_type;		// 장소타입
		private string						m_find_string;		// 검색문자열

		private List<spot_once>				m_spot_list;		// 외부참조向け목록

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public bool is_spot{			get{	return m_spot_type != type.none;	}}
		public type sopt_type{			get{	return m_spot_type;					}}
		public string spot_type_str{	get{	return GetTypeString(m_spot_type);	}}
		public string spot_type_column_str{	get{	return GetExColumnString(m_spot_type);	}}
		public List<spot_once> list{	get{	return m_spot_list;					}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public spot(gvt_lib lib, GvoWorldInfo world)
		{
			m_device		= lib.device;
			m_loop_image	= lib.loop_image;
			m_icons			= lib.icons;
			m_world			= world;

			m_spots			= new List<GvoWorldInfo.Info>();
			m_spot_list		= new List<spot_once>();
			m_spot_type		= type.none;
		}

		/*-------------------------------------------------------------------------
		 장소の종류を설정する
		 장소をやめるには Type.none を渡す
		---------------------------------------------------------------------------*/
		public void SetSpot(type _type, string find_str)
		{
			// 前회の내용をクリア
			m_spots.Clear();
			m_spot_list.Clear();
			m_find_string	= find_str;

			m_spot_type		= _type;
			switch(_type){
			case type.country_flags:		// 국기
				// 국목록を작성함
				foreach(GvoWorldInfo.Info i in m_world.World){
					if(i.InfoType == GvoWorldInfo.InfoType.City){
						m_spots.Add(i);
						m_spot_list.Add(new spot_once(i, i.CountryStr, i.AllianceTypeStr));
					}
				}
				break;
			case type.icons_0:				// 看판娘
			case type.icons_1:				// 서고
			case type.icons_2:				// 번역가
			case type.icons_3:				// 무역상인
				foreach(GvoWorldInfo.Info i in m_world.World){
					if((i.Sakaba & (1<<(_type-type.icons_0))) != 0){
						m_spots.Add(i);
						m_spot_list.Add(new spot_once(i, "", ""));
					}
				}
				break;
			case type.has_item:				// 지정した아이템がある
				foreach(GvoWorldInfo.Info i in m_world.World){
					GvoWorldInfo.Info.Group.Data	d	= i.HasItem(find_str);
					if(d != null){
						m_spots.Add(i);
						m_spot_list.Add(new spot_once(i, find_str, d.Price));
					}
				}
				break;
			case type.language:				// 언어
				foreach(GvoWorldInfo.Info i in m_world.World){
					if(   (i.Lang1 == find_str)
						||(i.Lang2 == find_str) ){
						// 사용언어
						m_spots.Add(i);
						m_spot_list.Add(new spot_once(i, find_str, ""));
					}else{
						if(i.LearnPerson(find_str) != null){
							// 언어取得
							m_spots.Add(i);
							m_spot_list.Add(new spot_once(i, find_str, i.LearnPerson(find_str)));
						}
					}
				}
				break;
			case type.tab_0:				// 교역
			case type.tab_1:				// 도구
			case type.tab_2:				// 공방
			case type.tab_3:				// 인물
			case type.tab_4:				// 조선소주인
			case type.tab_4_1:				// 조선공
			case type.tab_5:				// 무기장인
			case type.tab_6:				// 제재소장인
			case type.tab_7:				// 돛제작자
			case type.tab_8:				// 조각가
			case type.tab_9:				// 행상인
				int	index	= (int)(_type - type.tab_0);
				foreach(GvoWorldInfo.Info i in m_world.World){
					if(i.GetCount(GvoWorldInfo.Info.GroupIndex._0 + index) > 0){
						m_spots.Add(i);
						m_spot_list.Add(new spot_once(i, "", ""));
					}
				}
				break;
			case type.tab_10:				// 메모
				foreach(GvoWorldInfo.Info i in m_world.World){
					if(i.GetMemoLines() > 0){
						m_spots.Add(i);
						m_spot_list.Add(new spot_once(i, "", ""));
					}
				}
				break;
			case type.city_name:			// 도시명
				foreach(GvoWorldInfo.Info i in m_world.World){
					if(i.Name == find_str){
						m_spots.Add(i);
						m_spot_list.Add(new spot_once(i, "", ""));
					}
				}
				break;
			case type.cultural_sphere:		// 문화권
				foreach(GvoWorldInfo.Info i in m_world.World){
					if(i.CulturalSphereStr == find_str){
						m_spots.Add(i);
						m_spot_list.Add(new spot_once(i, "문화권", find_str));
					}
				}
				break;
			default:
				break;
			}
		}

		/*-------------------------------------------------------------------------
		 장소표시
		---------------------------------------------------------------------------*/
		public void Draw()
		{
			if(m_spot_type == type.none)	return;
			if(m_spots.Count == 0)			return;

			// 暗くする
			m_device.DrawFillRect(new Vector3(0, 0, 0.3f+0.01f), new Vector2(m_device.client_size.X, m_device.client_size.Y), Color.FromArgb(160, 0, 0, 0).ToArgb());

			m_loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_proc), 64f);
		}

		/*-------------------------------------------------------------------------
		 장소표시
		---------------------------------------------------------------------------*/
		private void draw_proc(Vector2 offset, LoopXImage image)
		{
			switch(m_spot_type){
			case type.country_flags:		// 국기
				draw_spot_mycountry(offset);
				spot_flags(offset);
				break;
			case type.icons_0:				// 看판娘
			case type.icons_1:				// 서고
			case type.icons_2:				// 번역가
			case type.icons_3:				// 무역상인
				draw_spot(offset);
				spot_cities(offset);
				spot_icons(offset);
				break;
			case type.has_item:				// 지정した아이템がある
				draw_spot(offset);
				spot_cities(offset);
				break;
			case type.language:				// 언어
				draw_spot_for_lang(offset);
				spot_cities(offset);
				spot_learn_lang(offset);
				break;
			case type.tab_0:				// 교역
			case type.tab_1:				// 도구
			case type.tab_2:				// 공방
			case type.tab_3:				// 인물
			case type.tab_4:				// 조선소주인
			case type.tab_4_1:				// 조선공
			case type.tab_5:				// 무기장인
			case type.tab_6:				// 제재소장인
			case type.tab_7:				// 돛제작자
			case type.tab_8:				// 조각가
			case type.tab_9:				// 행상인
			case type.tab_10:				// 메모
				draw_spot(offset);
//				spot_cities(OffsetPosition);
				spot_tab(offset);
				break;
			case type.city_name:			// 도시명
			case type.cultural_sphere:		// 문화권
				draw_spot(offset);
				spot_cities(offset);
				break;
			}
		}

		/*-------------------------------------------------------------------------
		 장소표시
		 국기
		---------------------------------------------------------------------------*/
		private void spot_flags(Vector2 offset)
		{
			float	size	= m_loop_image.ImageScale;
			if(size < 0.5)		size	= 0.5f;
			else if(size > 1)	size	= 1;

			m_device.sprites.BeginDrawSprites(m_icons.texture, offset, m_loop_image.ImageScale, new Vector2(size, size));
			foreach(GvoWorldInfo.Info i in m_spots){
				m_device.sprites.AddDrawSprites(new Vector3(i.position.X, i.position.Y, 0.3f),
										m_icons.GetIcon(icons.icon_index.spot_country_4 + (int)i.MyCountry));
			}
			m_device.sprites.EndDrawSprites();
		}

		/*-------------------------------------------------------------------------
		 장소표시
		---------------------------------------------------------------------------*/
		private void draw_spot(Vector2 offset)
		{
//			return;

			m_device.device.RenderState.ZBufferFunction		= Compare.Less;
			m_device.device.RenderState.SourceBlend			= Blend.SourceAlpha;
			m_device.device.RenderState.DestinationBlend	= Blend.One;

			float	size	= m_loop_image.ImageScale;
			if(size < 0.5)		size	= 0.5f;
//			else if(ImageSize > 2)	ImageSize	= 2;
			else if(size > 1)	size	= 1;

			int	color		= Color.FromArgb(100, 255, 255, 255).ToArgb();
			m_device.sprites.BeginDrawSprites(m_icons.texture, offset, m_loop_image.ImageScale, new Vector2(size, size));
			foreach(GvoWorldInfo.Info i in m_spots){
				m_device.sprites.AddDrawSprites(new Vector3(i.position.X, i.position.Y, 0.3f),
										m_icons.GetIcon(icons.icon_index.spot_0), color);
			}
			m_device.sprites.EndDrawSprites();

			m_device.device.RenderState.ZBufferFunction		= Compare.LessEqual;
			m_device.device.RenderState.SourceBlend			= Blend.SourceAlpha;
			m_device.device.RenderState.DestinationBlend	= Blend.InvSourceAlpha;
		}

		/*-------------------------------------------------------------------------
		 장소표시
		 본인の국のみ
		---------------------------------------------------------------------------*/
		private void draw_spot_mycountry(Vector2 offset)
		{
			m_device.device.RenderState.ZBufferFunction		= Compare.Less;
			m_device.device.RenderState.SourceBlend			= Blend.SourceAlpha;
			m_device.device.RenderState.DestinationBlend	= Blend.One;

			float	size	= m_loop_image.ImageScale;
			if(size < 0.5)		size	= 0.5f;
//			else if(ImageSize > 2)	ImageSize	= 2;
			else if(size > 1)	size	= 1;

			int	color		= Color.FromArgb(100, 255, 255, 255).ToArgb();
			m_device.sprites.BeginDrawSprites(m_icons.texture, offset, m_loop_image.ImageScale, new Vector2(size, size));
			foreach(GvoWorldInfo.Info i in m_spots){
				if(i.MyCountry != m_world.MyCountry)	continue;

				m_device.sprites.AddDrawSprites(new Vector3(i.position.X, i.position.Y, 0.3f),
										m_icons.GetIcon(icons.icon_index.spot_0), color);
			}
			m_device.sprites.EndDrawSprites();

			m_device.device.RenderState.ZBufferFunction		= Compare.LessEqual;
			m_device.device.RenderState.SourceBlend			= Blend.SourceAlpha;
			m_device.device.RenderState.DestinationBlend	= Blend.InvSourceAlpha;
		}

		/*-------------------------------------------------------------------------
		 언어용장소표시
		---------------------------------------------------------------------------*/
		private void draw_spot_for_lang(Vector2 offset)
		{
			m_device.device.RenderState.ZBufferFunction		= Compare.Less;
			m_device.device.RenderState.SourceBlend			= Blend.SourceAlpha;
			m_device.device.RenderState.DestinationBlend	= Blend.One;

			float	size	= m_loop_image.ImageScale;
			if(size < 0.5)		size	= 0.5f;
//			else if(ImageSize > 2)	ImageSize	= 2;
			else if(size > 1)	size	= 1;

			int	color		= Color.FromArgb(100, 255, 255, 255).ToArgb();
			m_device.sprites.BeginDrawSprites(m_icons.texture, offset, m_loop_image.ImageScale, new Vector2(size, size));
			foreach(GvoWorldInfo.Info i in m_spots){
				icons.icon_index	index	= icons.icon_index.spot_2;
				if(i.LearnPerson(m_find_string) != null){
					// 覚えられるところ
					index	= icons.icon_index.spot_0;
				}

				m_device.sprites.AddDrawSprites(new Vector3(i.position.X, i.position.Y, 0.3f),
										m_icons.GetIcon(index), color);
			}
			m_device.sprites.EndDrawSprites();

			m_device.device.RenderState.ZBufferFunction		= Compare.LessEqual;
			m_device.device.RenderState.SourceBlend			= Blend.SourceAlpha;
			m_device.device.RenderState.DestinationBlend	= Blend.InvSourceAlpha;
		}

		/*-------------------------------------------------------------------------
		 언어용장소표시
		 覚えられる도시に인아이콘표시
		---------------------------------------------------------------------------*/
		private void spot_learn_lang(Vector2 offset)
		{
			float	size	= m_loop_image.ImageScale * 1.5f;
			if(size < 0.5)		size	= 0.5f;
			else if(size > 1 * 1.5f)	size	= 1 * 1.5f;

			m_device.sprites.BeginDrawSprites(m_icons.texture, offset, m_loop_image.ImageScale, new Vector2(size, size));
			foreach(GvoWorldInfo.Info i in m_spots){
				if(i.LearnPerson(m_find_string) == null)	continue;

				m_device.sprites.AddDrawSprites(new Vector3(i.position.X, i.position.Y, 0.3f),
										m_icons.GetIcon(icons.icon_index.spot_tab_3));
			}
			m_device.sprites.EndDrawSprites();
		}

		/*-------------------------------------------------------------------------
		 장소표시
		 아이콘たち
		---------------------------------------------------------------------------*/
		private void spot_icons(Vector2 offset)
		{
			float	size	= m_loop_image.ImageScale * 1.2f;
			if(size < 0.5)				size	= 0.5f;
			else if(size > 1 * 1.2f)	size	= 1 * 1.2f;
	
			m_device.sprites.BeginDrawSprites(m_icons.texture, offset, m_loop_image.ImageScale, new Vector2(size, size));
			foreach(GvoWorldInfo.Info i in m_spots){
				m_device.sprites.AddDrawSprites(new Vector3(i.position.X, i.position.Y, 0.3f),
										m_icons.GetIcon(icons.icon_index.spot_tab2_0 + (int)(m_spot_type - type.icons_0)) );
			}
			m_device.sprites.EndDrawSprites();
		}

		/*-------------------------------------------------------------------------
		 장소표시
		 도시
		---------------------------------------------------------------------------*/
		private void spot_cities(Vector2 offset)
		{
			float	size	= m_loop_image.ImageScale * 1.2f;
			if(size < 0.5*1.2f)			size	= 0.5f*1.2f;
			else if(size > 1 * 1.2f)	size	= 1 * 1.2f;
	
			m_device.sprites.BeginDrawSprites(m_icons.texture, offset, m_loop_image.ImageScale, new Vector2(size, size));
			foreach(GvoWorldInfo.Info i in m_spots){
				icons.icon_index	index;
				switch(i.InfoType){
				case GvoWorldInfo.InfoType.City:			// 도시
					// 도시の종류により아이콘を変える
					index	= icons.icon_index.city_icon_0;
					index	+= (int)i.CityType;
					break;
				case GvoWorldInfo.InfoType.Shore:		// 상륙지점
					index	= icons.icon_index.city_icon_4;
					break;

				case GvoWorldInfo.InfoType.Shore2:		// 2차 필드
				case GvoWorldInfo.InfoType.PF:			// 개인농장
					index	= icons.icon_index.city_icon_5;
					break;
				case GvoWorldInfo.InfoType.OutsideCity:	// 교외
					index	= icons.icon_index.city_icon_6;
					break;

				case GvoWorldInfo.InfoType.Sea:			// 해역
				default:
					continue;
				}

				m_device.sprites.AddDrawSprites(new Vector3(i.position.X, i.position.Y, 0.3f),
										m_icons.GetIcon(index));
			}
			m_device.sprites.EndDrawSprites();
		}

		/*-------------------------------------------------------------------------
		 장소표시
		 タブ
		---------------------------------------------------------------------------*/
		private void spot_tab(Vector2 offset)
		{
			float	size	= m_loop_image.ImageScale;
			if(size < 0.5)				size = 0.5f;
			else if (size > 1.0f)	   size = 1.0f;
	
			m_device.sprites.BeginDrawSprites(m_icons.texture, offset, m_loop_image.ImageScale, new Vector2(size, size));
			foreach(GvoWorldInfo.Info i in m_spots){
				m_device.sprites.AddDrawSprites(new Vector3(i.position.X, i.position.Y, 0.3f),
										m_icons.GetIcon(icons.icon_index.spot_tab_0 + (int)(int)(m_spot_type - type.tab_0)) );
			}
			m_device.sprites.EndDrawSprites();
		}

		/*-------------------------------------------------------------------------
		 ツールチップ용문자열を得る
		---------------------------------------------------------------------------*/
		public string GetToolTipString(Point pos)
		{
			switch(m_spot_type){
			case type.country_flags:		// 국기
			case type.icons_0:				// 看판娘
			case type.icons_1:				// 서고
			case type.icons_2:				// 번역가
			case type.icons_3:				// 무역상인
			case type.tab_0:				// 교역
			case type.tab_1:				// 도구
			case type.tab_2:				// 공방
			case type.tab_3:				// 인물
			case type.tab_4:				// 조선공
			case type.tab_4_1:				// 조선소주인
			case type.tab_5:				// 무기장인
			case type.tab_6:				// 제재소장인
			case type.tab_7:				// 돛제작자
			case type.tab_8:				// 조각가
			case type.tab_9:				// 행상인
			case type.city_name:			// 도시명
				break;

			case type.has_item:				// 지정した아이템がある
				foreach(GvoWorldInfo.Info i in m_spots){
					if(i.HitTest(pos)){
						GvoWorldInfo.Info.Group.Data	d	= i.HasItem(m_find_string);
						if(d != null)	return String.Format("{0}[{1}]", d.Name, d.Price);
					}
				}
				break;
			case type.language:				// 언어
				foreach(GvoWorldInfo.Info i in m_spots){
					if(i.HitTest(pos)){
						return i.LearnPerson(m_find_string);
					}
				}
				break;
			case type.cultural_sphere:		// 문화권
				foreach(GvoWorldInfo.Info i in m_spots){
					if(i.HitTest(pos)){
						return i.CulturalSphereStr;
					}
				}
				break;
			}
			return null;
		}

		/*-------------------------------------------------------------------------
		 장소타입を문자열で得る
		---------------------------------------------------------------------------*/
		static public string GetTypeString(type _type)
		{
			switch(_type){
			case type.none:						return "장소なし";
			case type.country_flags:			return "국기";
			case type.icons_0:					return "看판娘";
			case type.icons_1:					return "서고";
			case type.icons_2:					return "번역가";
			case type.icons_3:					return "무역상인";
			case type.has_item:					return "아이템등";
			case type.language:					return "사용언어";
			case type.tab_0:					return "교역所";
			case type.tab_1:					return "도구屋";
			case type.tab_2:					return "공방";
			case type.tab_3:					return "인물";
			case type.tab_4:					return "조선소주인";
			case type.tab_4_1:				  return "조선공";
			case type.tab_5:					return "무기장인";
			case type.tab_6:					return "제재소장인";
			case type.tab_7:					return "돛제작자";
			case type.tab_8:					return "조각가";
			case type.tab_9:					return "행상인";
			case type.tab_10:					return "메모";
			case type.city_name:				return "도시명";
			case type.cultural_sphere:			return "문화권";
			}
			return "불명な타입";
		}

		/*-------------------------------------------------------------------------
		 장소타입からEx문자열を得る
		 장소목록용
		---------------------------------------------------------------------------*/
		static public string GetExColumnString(type _type)
		{
			switch(_type){
			case type.none:						return "";
			case type.country_flags:			return "상태";
			case type.icons_0:					return "";
			case type.icons_1:					return "";
			case type.icons_2:					return "";
			case type.icons_3:					return "";
			case type.has_item:					return "価格등";
			case type.language:					return "언어習得";
			case type.tab_0:					return "";
			case type.tab_1:					return "";
			case type.tab_2:					return "";
			case type.tab_3:					return "";
			case type.tab_4:					return "";
			case type.tab_4_1:				  return "";
			case type.tab_5:					return "";
			case type.tab_6:					return "";
			case type.tab_7:					return "";
			case type.tab_8:					return "";
			case type.tab_9:					return "";
			case type.tab_10:					return "";
			case type.city_name:				return "";
			case type.cultural_sphere:			return "";
			}
			return "";
		}

	}
}
