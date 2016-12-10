/*-------------------------------------------------------------------------

 좌の윈도우
 아이템정보を그리기する

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Collections.Generic;
using System;
using System.Windows.Forms;

using directx;
using Utility;
using Utility.KeyAssign;
using System.Diagnostics;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class item_window : d3d_windows.window
	{
		// 위치とサイズ(サイズは自動で調整される)
		private const int				WINDOW_POS_X			= 3;
		private const int				WINDOW_POS_Y			= 3;
		private const float				WINDOW_POS_Z			= 0.2f;
		private const int				WINDOW_SIZE_X			= 250;	// 初期サイズ
		private const int				WINDOW_SIZE_Y			= 200;	// 初期サイズ

		// 配置간격
		private const int				TABS_STEP_X				= def.ICON_SIZE_X+4;
		private const int				ICONS_STEP_X			= def.ICON_SIZE_X+4;
		// 기본的な세로방향の간격
		private const int				STEP_Y0					= def.ICON_SIZE_Y+2;	// 上부の아이콘用
		private const int				STEP_Y					= def.ICON_SIZE_Y+4;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		private gvt_lib					m_lib;						// 
		private GvoDatabase				m_db;						// DB
		private spot					m_spot;						// 장소표시
		private gvtrademap_cs_form		m_form;						// 우클릭メニュー専用

		private GvoWorldInfo.Info			m_info;						// 그리기対象
		private int						m_tab_index;				// 현재のタブ위치

		private hittest_list			m_hittest_list;				// 矩形管理
		private Vector2					m_window_size;				// 윈도우サイズ

		private TextBox					m_memo_text_box;			// 메모用テキストBOX
		private ListView				m_list_view;				// 아이템표시用목록ビュー

		private bool					m_is_1st_draw;
	
		private enum item_index{
			item_list,			// 아이템목록
			country,			// 国아이콘
			icons,				// 上の아이콘
			tabs,				// タブ
			web,				// web아이콘
			county_name,		// 国명
			lang1,				// 言語1
			lang2,				// 言語2
			max
		};

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public GvoWorldInfo.Info　info{	get{	return m_info;		}
										set{	set_info(value, true);	}}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public item_window(gvt_lib lib, GvoDatabase db, spot _spot, TextBox memo_text_box, ListView list_view, gvtrademap_cs_form form)
			: base(lib.device, new Vector2(WINDOW_POS_X, WINDOW_POS_Y), new Vector2(WINDOW_SIZE_X, WINDOW_SIZE_Y), WINDOW_POS_Z)
		{
			base.title				= "아이템윈도우";

			m_window_size			= new Vector2(0, 0);
			m_memo_text_box			= memo_text_box;
			m_list_view				= list_view;
			m_form					= form;
	
			m_lib					= lib;

			m_db					= db;
			m_spot					= _spot;

			m_tab_index				= 0;
			m_info					= null;

			m_is_1st_draw			= true;

			// 아이템追加
			m_hittest_list			= new hittest_list();

			// 아이템목록
			m_hittest_list.Add(new hittest());
			// 国아이콘
			m_hittest_list.Add(new hittest());
			// 아이콘たち
			m_hittest_list.Add(new hittest());
			// タブ
			m_hittest_list.Add(new hittest());
			// web아이콘
			m_hittest_list.Add(new hittest());
			// 国명
			m_hittest_list.Add(new hittest());
			// 言語1
			m_hittest_list.Add(new hittest());
			// 言語2
			m_hittest_list.Add(new hittest());
		}

		/*-------------------------------------------------------------------------
		 info설정
		---------------------------------------------------------------------------*/
		private void set_info(GvoWorldInfo.Info _info, bool with_spot_reset)
		{
			if(_info == null)		return;

			if(with_spot_reset){
				// 無条건で장소終了
				m_spot.SetSpot(spot.type.none, "");
				m_form.UpdateSpotList();
			}

			// 내용が同じ場合はなにもしない
			if(m_info == _info)		return;
			
			// 메모
			// テキストボックスの내용をDBに退避
			UpdateMemo();		// 업데이트

			// 업데이트
			m_info					= _info;
			// 메모用テキストボックス업데이트
			m_memo_text_box.Text	= m_info.Memo;
			// 아이템목록업데이트
			update_item_list();
		}
	
		/*-------------------------------------------------------------------------
		 마우스押し始め
		---------------------------------------------------------------------------*/
		override protected void OnMouseDownClient(Point pos, MouseButtons button)
		{
			if((button & MouseButtons.Left) != 0){
				_window_on_mouse_l_down(pos);
			}else if((button & MouseButtons.Right) != 0){
				_window_on_mouse_r_down(pos);
			}
		}

		/*-------------------------------------------------------------------------
		 마우스좌押し始め
		---------------------------------------------------------------------------*/
		private void _window_on_mouse_l_down(Point pos)
		{
			switch(m_hittest_list.HitTest_Index(pos)){
			case (int)item_index.tabs:
				on_mouse_l_click_item_tabs(pos);
				break;
			default:
				break;
			}
		}
	
		/*-------------------------------------------------------------------------
		 마우스우押し始め
		---------------------------------------------------------------------------*/
		private void _window_on_mouse_r_down(Point pos)
		{
			switch(m_hittest_list.HitTest_Index(pos)){
			case (int)item_index.tabs:
				on_mouse_l_click_item_tabs(pos);
				break;
			}
		}

		/*-------------------------------------------------------------------------
		 마우스클릭
		---------------------------------------------------------------------------*/
		override protected void OnMouseClikClient(Point pos, MouseButtons button)
		{
			if((button & MouseButtons.Left) != 0){
				_window_on_mouse_l_click(pos);
			}else if((button & MouseButtons.Right) != 0){
				_window_on_mouse_r_click(pos);
			}
		}

		/*-------------------------------------------------------------------------
		 마우스좌클릭
		---------------------------------------------------------------------------*/
		private void _window_on_mouse_l_click(Point pos)
		{
			switch(m_hittest_list.HitTest_Index(pos)){
			case (int)item_index.country:
				on_mouse_l_click_country(pos);
				break;
			case (int)item_index.icons:
				on_mouse_l_click_icons(pos);
				break;
			case (int)item_index.county_name:
				on_mouse_l_click_country_name(pos);
				break;
			case (int)item_index.lang1:
				on_mouse_l_click_lang(pos, 0);
				break;
			case (int)item_index.lang2:
				on_mouse_l_click_lang(pos, 1);
				break;
			case (int)item_index.web:
				on_mouse_l_click_web(pos);
				break;
			default:
				break;
			}
		}

		/*-------------------------------------------------------------------------
		 마우스우클릭
		---------------------------------------------------------------------------*/
		private void _window_on_mouse_r_click(Point pos)
		{
			switch(m_hittest_list.HitTest_Index(pos)){
			case (int)item_index.tabs:
				on_mouse_r_click_item_tabs(pos);
				break;
			case (int)item_index.country:
				on_mouse_r_click_country(pos);
				break;
			case (int)item_index.county_name:
				on_mouse_r_click_country_name(pos);
				break;
			}
		}

		/*-------------------------------------------------------------------------
		 마우스ホイール
		---------------------------------------------------------------------------*/
		protected override void OnMouseWheelClient(Point pos, int delta)
		{
			switch(m_hittest_list.HitTest_Index(pos)){
			case (int)item_index.tabs:
				on_mouse_wheel_item_tabs(pos, delta);
				break;
			default:
				break;
			}
		}

		/*-------------------------------------------------------------------------
		 업데이트
		---------------------------------------------------------------------------*/
		override protected void OnUpdateClient()
		{
			// 최소화時は메모윈도우を비표시にする
			if(base.window_mode == d3d_windows.window.mode.small){
				EnableMemoWindow(false);
				EnableItemWindow(false);
				return;
			}

			// 8dot분のギャップを持つ
			d3d_windows.window	iw	= base.FindWindow("설정윈도우");
			if(iw != null)		base.size	= new Vector2(iw.normal_size.X, base.screen_size.Y - 4*2);
			else				base.size	= new Vector2(WINDOW_SIZE_X, base.screen_size.Y - 4*2);

			// 矩形を설정し直す
			update_rects();

			if(base.size != m_window_size){
				// 메모
				hittest		ht		= m_hittest_list[(int)item_index.item_list];
				Rectangle	rect	= ht.CalcRect();
				m_memo_text_box.Location	= new Point(rect.Left, rect.Top);
				m_memo_text_box.Size		= new Size(rect.Width, rect.Height + 1);
				m_list_view.Location		= m_memo_text_box.Location;
				m_list_view.Size			= m_memo_text_box.Size;
				// コラムサイズ調整
				ajust_item_columns_width();
			}
			update_memo_window();

			m_window_size	= base.size;
		}

		/*-------------------------------------------------------------------------
		 矩形を설정し直す
		---------------------------------------------------------------------------*/
		private void update_rects()
		{
			Point		offset	= transform.ToPoint(base.client_pos);

			// オフセットの업데이트
			foreach(hittest h in m_hittest_list){
				h.position	= offset;
			}

			hittest		ht;
			int			pos_y	= 0;

			// 国아이콘
			ht		= m_hittest_list[(int)item_index.country];
			ht.rect	= new Rectangle(0, pos_y, 24, def.ICON_SIZE_Y);
			pos_y	+= STEP_Y0;

			// 아이콘たち
			ht		= m_hittest_list[(int)item_index.icons];
			ht.rect	= new Rectangle(2, pos_y, ICONS_STEP_X * 4, def.ICON_SIZE_Y);

			// web아이콘
			ht		= m_hittest_list[(int)item_index.web];
			ht.rect	= new Rectangle(2+ ICONS_STEP_X * 4 + (10), pos_y, def.ICON_SIZE_X+1, def.ICON_SIZE_Y+1);

			// 国명
			ht		= m_hittest_list[(int)item_index.county_name];
			ht.rect	= new Rectangle(24+4, 0+2, 130, def.ICON_SIZE_Y);

			// 言語1
			ht		= m_hittest_list[(int)item_index.lang1];
			ht.rect	= new Rectangle((int)base.client_size.X - 130, 0+2, 130, def.ICON_SIZE_Y);

			// 言語2
			ht		= m_hittest_list[(int)item_index.lang2];
			ht.rect	= new Rectangle((int)base.client_size.X - 130, pos_y+1, 130, def.ICON_SIZE_Y);

			pos_y	+= STEP_Y0;

			// 아이템목록
			// 아이템数がうまく収まるようにサイズを調整する
			int		size_y	= (int)base.client_size.Y - (pos_y + STEP_Y) - (int)base.pos.Y;
			d3d_windows.window	iw	= base.FindWindow("설정윈도우");
			if(iw != null)	size_y	-= (int)iw.size.Y;

			ht		= m_hittest_list[(int)item_index.item_list];
			ht.rect	= new Rectangle(0, pos_y, (int)base.client_size.X, size_y);
			pos_y	+= size_y + 2;

			// タブ
			ht		= m_hittest_list[(int)item_index.tabs];
			ht.rect	= new Rectangle(2, pos_y, TABS_STEP_X * 12, def.ICON_SIZE_Y);
			pos_y	+= STEP_Y;

			// 세로サイズを업데이트する
			base.client_size	= new Vector2(base.client_size.X, pos_y);
		}
	
		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		override protected void OnDrawClient()
		{
			if(m_is_1st_draw){
				update_item_list();
				m_is_1st_draw	= false;
			}
	
			// 아이템목록
			// タブの현재を그리기
			draw_current_tab();

			// 国명
			draw_country_name();

			// 言語
			draw_lang();

			// 마우스がある위치の背景
			draw_current_back();

			base.device.sprites.BeginDrawSprites(m_lib.icons.texture);{

				// 国아이콘
				draw_country();
		
				// 아이콘たち
				draw_icons();

				// web아이콘
				draw_web();
	
				// タブ
				draw_tabs();

			}base.device.sprites.EndDrawSprites();

			// 선택중아이콘
//			draw_select_cursor();
		}

		/*-------------------------------------------------------------------------
		 선택중아이콘
		---------------------------------------------------------------------------*/
/*
		private void draw_select_cursor()
		{
			if(Info == null)				return;

			int			Index	= m_select_index - m_draw_top_item_index;
			if(Index < 0)					return;
			if(Index >= m_item_list_max)	return;

			hittest		ht		= m_hittest_list[(int)item_index.item_list];
			Rectangle	rect	= ht.CalcRect();

			Viewport	view	= base.SetViewport(rect.X, rect.Y,
													rect.Width + 1, rect.Height);

			Vector3		pos		= new Vector3(rect.X, rect.Y, base.z);

			pos.X	+= 2;
			pos.Y	+= Index * STEP_Y;
			pos.Y	+= STEP_Y / 2;
			base.Device.sprites.BeginDrawSprites(m_lib.icons.Texture);
			base.Device.sprites.AddDrawSprites(pos, m_lib.icons.GetIcon(icons.IconIndex.select_0));
			base.Device.sprites.EndDrawSprites();
			base.SetViewport(view);
		}
*/
		/*-------------------------------------------------------------------------
		 현재の마우스がある장소설정
		---------------------------------------------------------------------------*/
		private void draw_current_back()
		{
			Point pos	= base.device.GetClientMousePosition();
	
			switch(m_hittest_list.HitTest_Index(pos)){
			case (int)item_index.icons:
				draw_current_back_icons(pos);
				break;
			case (int)item_index.web:
				draw_current_back_web(pos);
				break;
			case (int)item_index.tabs:
				draw_current_back_tabs(pos);
				break;
			}
		}	

		/*-------------------------------------------------------------------------
		 현재の마우스がある장소설정
		 아이콘達
		---------------------------------------------------------------------------*/
		private void draw_current_back_icons(Point pos)
		{
			hittest		ht		= m_hittest_list[(int)item_index.icons];
			Rectangle	rect	= ht.CalcRect();

			pos.X	-= rect.X;
			pos.X	/= ICONS_STEP_X;		// Index;

			Vector3		dpos		= new Vector3(rect.X + ICONS_STEP_X * pos.X, rect.Y, base.z);
			dpos.X	-= 1;
			dpos.Y	-= 1;
			base.DrawCurrentButtonBack(dpos, new Vector2(def.ICON_SIZE_X+2, def.ICON_SIZE_Y+2));
		}

		/*-------------------------------------------------------------------------
		 현재の마우스がある장소설정
		 Web아이콘
		---------------------------------------------------------------------------*/
		private void draw_current_back_web(Point pos)
		{
			if(info == null)		return;
			if(!info.IsUrl)		return;
	
			hittest		ht		= m_hittest_list[(int)item_index.web];
			Rectangle	rect	= ht.CalcRect();

			Vector3		dpos		= new Vector3(rect.X, rect.Y, base.z);
			dpos.X	-= 1;
			dpos.Y	-= 1;
			base.DrawCurrentButtonBack(dpos, new Vector2(def.ICON_SIZE_X+2, def.ICON_SIZE_Y+2));
		}

		/*-------------------------------------------------------------------------
		 현재の마우스がある장소설정
		 タブ
		---------------------------------------------------------------------------*/
		private void draw_current_back_tabs(Point pos)
		{
			hittest		ht		= m_hittest_list[(int)item_index.tabs];
			Rectangle	rect	= ht.CalcRect();

			pos.X	-= rect.X;
			pos.X	/= TABS_STEP_X;

			Vector3		dpos		= new Vector3(rect.X + TABS_STEP_X * pos.X, rect.Y, base.z);
			dpos.X	-= 2;
			dpos.Y	-= 2;
			base.DrawCurrentButtonBack(dpos, new Vector2(def.ICON_SIZE_X+4, def.ICON_SIZE_Y+5));
		}

		/*-------------------------------------------------------------------------
		 国명
		---------------------------------------------------------------------------*/
		private void draw_country_name()
		{
			if(info == null)	return;
	
			hittest		ht		= m_hittest_list[(int)item_index.county_name];
			Rectangle	rect	= ht.CalcRect();

			Vector3		pos		= new Vector3(rect.X, rect.Y, base.z);

			// 이름
			base.device.DrawText(font_type.normal, info.Name,
									(int)pos.X, (int)pos.Y, Color.Black);
		}

		/*-------------------------------------------------------------------------
		 言語
		---------------------------------------------------------------------------*/
		private void draw_lang()
		{
			if(info == null)	return;
	
			hittest		ht		= m_hittest_list[(int)item_index.lang1];
			Rectangle	rect	= ht.CalcRect();

			Vector3		pos		= new Vector3(rect.X, rect.Y, base.z);

			// 言語
			if(info.InfoType == GvoWorldInfo.InfoType.City){
				if(info.Lang1 != ""){
					base.device.DrawTextR(font_type.normal, info.Lang1,
											(int)pos.X + rect.Width, (int)pos.Y, Color.Black);
				}
				if(info.Lang2 != ""){
					base.device.DrawTextR(font_type.normal, info.Lang2,
											(int)pos.X + rect.Width, (int)pos.Y + (STEP_Y0-1), Color.Black);
				}
			}else if(info.InfoType == GvoWorldInfo.InfoType.Sea){
				if(info.SeaInfo != null){
					// 최대속도上昇
					base.device.DrawTextR(font_type.normal, info.SeaInfo.SpeedUpRateString,
											(int)pos.X + rect.Width, (int)pos.Y, Color.Black);
				}
			}
		}

		/*-------------------------------------------------------------------------
		 web아이콘
		---------------------------------------------------------------------------*/
		private void draw_web()
		{
			if(info == null)	return;
			if(!info.IsUrl)	return;

			hittest		ht		= m_hittest_list[(int)item_index.web];
			Rectangle	rect	= ht.CalcRect();

			Vector3		pos		= new Vector3(rect.X, rect.Y, base.z);

			icons.icon_index	icon	= (info.InfoType == GvoWorldInfo.InfoType.City)
											? icons.icon_index.web
											: icons.icon_index.map_icon;
			base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(icon));
		}

		/*-------------------------------------------------------------------------
		 현재タブを그리기する
		---------------------------------------------------------------------------*/
		private void draw_current_tab()
		{
			hittest		ht		= m_hittest_list[(int)item_index.item_list];
			Rectangle	rect	= ht.CalcRect();

			Vector3		pos		= new Vector3(rect.X, rect.Y, base.z);
			Vector2		size	= new Vector2(rect.Width, rect.Height);
			Vector3		pos2	= pos;

			// タブの현재
			pos2		= pos;
			pos2.Y		+= size.Y;
			pos2.X		+= TABS_STEP_X * m_tab_index;
			base.device.DrawFillRect(pos2, new Vector2(TABS_STEP_X, STEP_Y), Color.FromArgb(128, 255, 255, 255).ToArgb());
			base.device.DrawLineRect(pos2, new Vector2(TABS_STEP_X, STEP_Y+1), Color.Black.ToArgb());
		}

		/*-------------------------------------------------------------------------
		 아이템목록数を得る
		---------------------------------------------------------------------------*/
		private int get_item_list_count()
		{
			return get_item_list_count(m_tab_index);
		}
		private int get_item_list_count(int index)
		{
			if(info == null)	return 0;
			if(index == 11)		return info.GetMemoLines();		// 메모
			return info.GetCount(GvoWorldInfo.Info.GroupIndex._0 + index);
		}

		/*-------------------------------------------------------------------------
		 国아이콘그리기
		---------------------------------------------------------------------------*/
		private void draw_country()
		{
			if(info == null)		return;

			hittest		ht		= m_hittest_list[(int)item_index.country];
			Rectangle	rect	= ht.CalcRect();
			Vector3		pos		= new Vector3(rect.X, rect.Y, base.z);

			icons.icon_index	index;
			switch(info.InfoType){
			case GvoWorldInfo.InfoType.PF:
				index	= icons.icon_index.country_0;
				break;
			case GvoWorldInfo.InfoType.Sea:
				index	= icons.icon_index.country_2;
				break;
			case GvoWorldInfo.InfoType.Shore:
			case GvoWorldInfo.InfoType.OutsideCity:
				index	= icons.icon_index.country_3;
				break;
			case GvoWorldInfo.InfoType.Shore2:
				index	= icons.icon_index.country_1;
				break;
			case GvoWorldInfo.InfoType.City:
			default:
				index	= icons.icon_index.country_4;
				// 동맹関係により旗を선택する
				index	+= (int)info.MyCountry;
				break;
			}
	
			// 아이콘
			base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(index));
		}

		/*-------------------------------------------------------------------------
		 아이콘たち그리기
		---------------------------------------------------------------------------*/
		private void draw_icons()
		{
			hittest		ht		= m_hittest_list[(int)item_index.icons];
			Rectangle	rect	= ht.CalcRect();

			Vector3		pos		= new Vector3(rect.X, rect.Y, base.z);

			for(int i=0; i<4; i++){
				icons.icon_index	index	= icons.icon_index.tab2_gray_0;
				if(info != null){
					if((info.Sakaba & (1<<i)) != 0){
						index	= icons.icon_index.tab2_0;
					}
				}
				base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(index + i));
				pos.X	+= ICONS_STEP_X;
			}
		}

		/*-------------------------------------------------------------------------
		 タブたち그리기
		---------------------------------------------------------------------------*/
		private void draw_tabs()
		{
			hittest		ht		= m_hittest_list[(int)item_index.tabs];
			Rectangle	rect	= ht.CalcRect();

			Vector3		pos		= new Vector3(rect.X, rect.Y, base.z);

			for(int i=0; i<12; i++){
				icons.icon_index	index	= icons.icon_index.tab_gray_0;
				if(info != null){
					if(get_item_list_count(i) > 0)	index	= icons.icon_index.tab_0;
				}
				base.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(index + i));
				pos.X	+= TABS_STEP_X;
			}
		}

		/*-------------------------------------------------------------------------
		 마우스좌클릭
		 タブ
		---------------------------------------------------------------------------*/
		private void on_mouse_l_click_item_tabs(Point pos)
		{
			hittest		ht		= m_hittest_list[(int)item_index.tabs];
			Rectangle	rect	= ht.CalcRect();

			pos.X	-= rect.X;
			ajust_tab_index(pos.X / TABS_STEP_X);
		}

		/*-------------------------------------------------------------------------
		 마우스우클릭
		 タブ
		---------------------------------------------------------------------------*/
		private void on_mouse_r_click_item_tabs(Point pos)
		{
			on_mouse_l_click_item_tabs(pos);

			// タブを장소표시
			m_spot.SetSpot(spot.type.tab_0 + m_tab_index, "");
			m_form.UpdateSpotList();
		}

		/*-------------------------------------------------------------------------
		 마우스ホイール
		 タブ
		---------------------------------------------------------------------------*/
		private void on_mouse_wheel_item_tabs(Point pos, int delta)
		{
			if(delta > 0)	ajust_tab_index(m_tab_index - 1);
			else			ajust_tab_index(m_tab_index + 1);
		}

		/*-------------------------------------------------------------------------
		 タブ위치설정と調整
		---------------------------------------------------------------------------*/
		private void ajust_tab_index(int index)
		{
			// ループさせる
			if(index < 0)		index	= 11;
			if(index >= 12)		index	= 0;

			m_tab_index				= index;

			// 메모の윈도우상태업데이트
// ちらつく問題でここで update_memo_window() を呼び出すのをやめ
//			update_memo_window();
			// 아이템목록업데이트
			update_item_list();
		}
	
		/*-------------------------------------------------------------------------
		 검색された아이템を장소표시する
		---------------------------------------------------------------------------*/
		public void SpotItem(GvoDatabase.Find select)
		{
			if(select == null){
				// 선택が無ければ장소を해제する
				m_spot.SetSpot(spot.type.none, "");
				m_form.UpdateSpotList();
				return;
			}
			// 아이템DBは장소不可能
			if(select.Type == GvoDatabase.Find.FindType.Database)		return;

			GvoWorldInfo.Info	_info	= m_db.World.FindInfo(select.InfoName);
			if(_info != null){
				// 장소の리셋なしで설정
				set_info(_info, false);
				// センタリングしてもらう
				req_centering_info();
			}

			switch(select.Type){
			case GvoDatabase.Find.FindType.Data:
			case GvoDatabase.Find.FindType.DataPrice:
				// 아이템から선택한ものを검색する
				find_item_for_selected(select.Data.Name);
				// 아이템を장소
				m_spot.SetSpot(spot.type.has_item, select.Data.Name);
				break;
			case GvoDatabase.Find.FindType.Database:
				// 아이템DBは장소不可能
				break;
			case GvoDatabase.Find.FindType.InfoName:
				// 도시を장소
				m_spot.SetSpot(spot.type.city_name, select.InfoName);
				break;
			case GvoDatabase.Find.FindType.Lang:
				// 言語を장소
				m_spot.SetSpot(spot.type.language, select.Lang);
				break;
			case GvoDatabase.Find.FindType.CulturalSphere:
				// 문화권
				m_spot.SetSpot(spot.type.cultural_sphere, select.InfoName);
				break;
			}
			m_form.UpdateSpotList();
		}

		/*-------------------------------------------------------------------------
		 장소아이템の변경
		 장소정보は변경されない
		---------------------------------------------------------------------------*/
		public void SpotItemChanged(spot.spot_once select)
		{
			if(select == null)		return;
			if(select.info == null)	return;

			GvoWorldInfo.Info	_info	= select.info;
			if(_info != null){
				// 장소の리셋なしで설정
				set_info(_info, false);
				// センタリングしてもらう
				req_centering_info();
			}

			// 아이템から선택한ものを검색する
			find_item_for_selected(select.name);
		}

		/*-------------------------------------------------------------------------
		 아이템から선택한ものを검색する
		---------------------------------------------------------------------------*/
		private void find_item_for_selected(string find_item)
		{
			if(info == null)	return;

			for(GvoWorldInfo.Info.GroupIndex i=GvoWorldInfo.Info.GroupIndex._0; i<GvoWorldInfo.Info.GroupIndex.max; i++){
				for(int s=0; s<info.GetCount(i); s++){
					GvoWorldInfo.Info.Group.Data	d	= info.GetData(i, s);
					if(d == null)				continue;
					if(d.Name != find_item)		continue;

					// 선택した아이템が見えるようにスクロール위치を調整
					ajust_tab_index((int)i);

					// 표시위치を調整
					if(m_list_view.Items.Count >= s){
						m_list_view.Items[s].Selected	= true;
						m_list_view.Items[s].EnsureVisible();
						m_list_view.Items[s].Focused	= true;
					}
					return;
				}
			}
		}
	
		/*-------------------------------------------------------------------------
		 センタリングされている도시をセンタリングする
		 リクエスト
		---------------------------------------------------------------------------*/
		private void req_centering_info()
		{
			if(info == null)	return;

			m_lib.setting.centering_gpos	= transform.map_pos2_game_pos(info.position, m_lib.loop_image);
			m_lib.setting.req_centering_gpos.Request();
		}

		/*-------------------------------------------------------------------------
		 国旗を좌클릭
		---------------------------------------------------------------------------*/
		private void on_mouse_l_click_country(Point pos)
		{
			// 国旗を장소표시
			// 도시のみ
			if(info == null)											return;
			if(info.InfoType != GvoWorldInfo.InfoType.City)				return;

			m_spot.SetSpot(spot.type.country_flags, "");
			m_form.UpdateSpotList();
		}

		/*-------------------------------------------------------------------------
		 国旗を우클릭
		---------------------------------------------------------------------------*/
		private void on_mouse_r_click_country(Point pos)
		{
			// 도시のみ
			if(info == null)											return;
			if(info.InfoType != GvoWorldInfo.InfoType.City)				return;
			// 동맹できる도시のみ
			if(info.AllianceType != GvoWorldInfo.AllianceType.Alliance)	return;

			// 우클릭メニューを開く
			m_form.ShowChangeDomainsMenuStrip(pos);
		}

		/*-------------------------------------------------------------------------
		 이름を우클릭
		---------------------------------------------------------------------------*/
		private void on_mouse_r_click_country_name(Point pos)
		{
			// 도시のみ
			if(info == null)											return;
			if(info.InfoType != GvoWorldInfo.InfoType.City)				return;
			if(info.CulturalSphere == GvoWorldInfo.CulturalSphere.Unknown)	return;

			// 장소표시
			SpotItem(new GvoDatabase.Find(info.CulturalSphere, ""));
		}
	
		/*-------------------------------------------------------------------------
		 아이콘たちを좌클릭
		---------------------------------------------------------------------------*/
		private void on_mouse_l_click_icons(Point pos)
		{
			hittest		ht		= m_hittest_list[(int)item_index.icons];
			Rectangle	rect	= ht.CalcRect();

			pos.X	-= rect.X;
			pos.X	/= ICONS_STEP_X;		// Index;

			// 아이콘を장소표시
			m_spot.SetSpot(spot.type.icons_0 + pos.X, "");
			m_form.UpdateSpotList();
		}

		/*-------------------------------------------------------------------------
		 国명を좌클릭
		---------------------------------------------------------------------------*/
		private void on_mouse_l_click_country_name(Point pos)
		{
			if(info == null)		return;

			// センタリングリクエスト
			req_centering_info();

			// 장소
			m_spot.SetSpot(spot.type.city_name, info.Name);
			m_form.UpdateSpotList();
		}

		/*-------------------------------------------------------------------------
		 言語を좌클릭
		---------------------------------------------------------------------------*/
		private void on_mouse_l_click_lang(Point pos, int index)
		{
			if(info == null)		return;

			// 言語を장소표시
			string	lang	= (index == 0)? info.Lang1: info.Lang2;
			if(lang == "")			return;

			m_spot.SetSpot(spot.type.language, lang);
			m_form.UpdateSpotList();
		}

		/*-------------------------------------------------------------------------
		 Web아이콘を좌클릭
		---------------------------------------------------------------------------*/
		private void on_mouse_l_click_web(Point pos)
		{
			if(info == null)			return;
			if(!info.IsUrl)			return;

			if(info.UrlIndex != -1){
				string	url;
				if(info.InfoType == GvoWorldInfo.InfoType.City){
					// 대商戦
					url		= def.URL0 + info.UrlIndex.ToString();
				}else{
					// DKKmap
					url		= def.URL1 + info.UrlIndex.ToString() + ".html";
				}
				Process.Start("http://" + url);
			}else{
				// クリスタル商会
				Process.Start("http://" + def.URL5 + Useful.UrlEncodeEUCJP(info.Url));
			}
		}

		/*-------------------------------------------------------------------------
		 tabを切り替える
		---------------------------------------------------------------------------*/
		public void ChangeTab(bool is_next)
		{
			if(is_next){
				ajust_tab_index(m_tab_index + 1);
			}else{
				ajust_tab_index(m_tab_index - 1);
			}
			m_lib.device.SetMustDrawFlag();
		}
	
		/*-------------------------------------------------------------------------
		 ツールチップ표시用の文字列を得る
		 표시すべき文字列がない場合nullを返す
		---------------------------------------------------------------------------*/
		override protected string OnToolTipStringClient(Point pos)
		{
			switch(m_hittest_list.HitTest_Index(pos)){
			case (int)item_index.icons:				return get_tooltip_string_icons(pos);
			case (int)item_index.tabs:				return get_tooltip_string_tabs(pos);
			}
	
			// 以下はinfoを参照する必要があるもの
			if(info == null)		return null;

			switch(m_hittest_list.HitTest_Index(pos)){
			case (int)item_index.web:
				return info.GetToolTipString_HP();
			case (int)item_index.county_name:
				{
					string	tmp		= info.TooltipString;
					tmp	+= "\n좌클릭で중心に이동";
					if(info.InfoType == GvoWorldInfo.InfoType.City){
						tmp	+= "\n우클릭で属する문화권を장소표시";
					}
					return tmp;
				}
			case (int)item_index.lang1:
			case (int)item_index.lang2:
				if(   (info.Lang1 != "")
					||(info.Lang2 != "") ){
					return "사용언어\n좌클릭で장소표시";
				}
				break;
			case (int)item_index.country:
				switch(info.InfoType){
				case GvoWorldInfo.InfoType.City:
					switch(info.AllianceType){
					case GvoWorldInfo.AllianceType.Piratical:
					case GvoWorldInfo.AllianceType.Unknown:		return info.AllianceTypeStr + "\n좌클릭で장소표시";
					case GvoWorldInfo.AllianceType.Alliance:	return info.AllianceTypeStr + " " + info.CountryStr + "\n좌클릭で장소표시\n우클릭で동맹국변경";

					case GvoWorldInfo.AllianceType.Capital:
					case GvoWorldInfo.AllianceType.Territory:
							return info.CountryStr + " " + info.AllianceTypeStr + "\n좌클릭で장소표시";
					}
					break;
				case GvoWorldInfo.InfoType.OutsideCity:
				case GvoWorldInfo.InfoType.PF:
				case GvoWorldInfo.InfoType.Sea:
				case GvoWorldInfo.InfoType.Shore:
				case GvoWorldInfo.InfoType.Shore2:
//					return Info.InfoTypeStr + "\n좌클릭で장소표시";
					return info.InfoTypeStr;
				}
				break;
			}
			return null;
		}

		/*-------------------------------------------------------------------------
		 ツールチップ표시用の文字列を得る
		 표시すべき文字列がない場合nullを返す
		 아이템목록
		---------------------------------------------------------------------------*/
		private string get_tooltip_string_item_list(GvoWorldInfo.Info.Group.Data d)
		{
			if(d == null)		return null;

			string	str			= d.TooltipString;

			// ヘルプを含める
//			if((!db.IsSkill)&&(!db.IsReport)){
				str			+= "\n(우클릭でメニューを開きます)";
//			}
			return str;
		}
	
		/*-------------------------------------------------------------------------
		 ツールチップ표시用の文字列を得る
		 표시すべき文字列がない場合nullを返す
		 아이콘たち
		---------------------------------------------------------------------------*/
		private string get_tooltip_string_icons(Point pos)
		{
			hittest		ht		= m_hittest_list[(int)item_index.icons];
			Rectangle	rect	= ht.CalcRect();

			pos.X	-= rect.X;
			pos.X	/= ICONS_STEP_X;		// Index;

			switch(pos.X){
			case 0:		return "請負인/酒場娘/판매원\n좌클릭で장소표시";
			case 1:		return "書庫\n좌클릭で장소표시";
			case 2:		return "번역가\n좌클릭で장소표시";
			case 3:		return "豪商\n좌클릭で장소표시";
			}
			return null;
		}

		/*-------------------------------------------------------------------------
		 ツールチップ표시用の文字列を得る
		 표시すべき文字列がない場合nullを返す
		 タブ
		---------------------------------------------------------------------------*/
		private string get_tooltip_string_tabs(Point pos)
		{
			hittest		ht		= m_hittest_list[(int)item_index.tabs];
			Rectangle	rect	= ht.CalcRect();

			pos.X	-= rect.X;
			pos.X	/= TABS_STEP_X;

            if (0 < pos.X && pos.X < 12)
            {
                return GvoWorldInfo.Info.GetGroupName((GvoWorldInfo.Info.GroupIndex)pos.X) + "\n우클릭で장소표시";
            }
			return null;
		}	

		/*-------------------------------------------------------------------------
		 메모の윈도우상태업데이트
		---------------------------------------------------------------------------*/
		private void update_memo_window()
		{
			if(m_tab_index == 11){
				EnableMemoWindow(true);
				EnableItemWindow(false);
			}else{
				EnableMemoWindow(false);
				EnableItemWindow(true);
			}
		}

		/*-------------------------------------------------------------------------
		 메모の윈도우상태설정
		---------------------------------------------------------------------------*/
		public void EnableMemoWindow(bool is_enable)
		{
			enable_ctrl(m_memo_text_box, is_enable);
		}

		/*-------------------------------------------------------------------------
		 아이템の윈도우상태설정
		---------------------------------------------------------------------------*/
		public void EnableItemWindow(bool is_enable)
		{
			enable_ctrl(m_list_view, is_enable);
		}

		/*-------------------------------------------------------------------------
		 コントロールの상태설정
		---------------------------------------------------------------------------*/
		private void enable_ctrl(Control ctrl, bool is_enable)
		{
			// 표시설정
			if(is_enable){
				if(!ctrl.Visible){
					ctrl.Visible		= true;
				}

				// 編集可能不可能설정
				if(m_info == null){
					if(ctrl.Enabled)	ctrl.Enabled	= false;
				}else{
					if(!ctrl.Enabled)	ctrl.Enabled	= true;
				}
			}else{
				if(ctrl.Visible){
					ctrl.Visible		= false;
					base.device.form.Focus();		// フォーカスを返す
				}
			}
		}

		/*-------------------------------------------------------------------------
		 메모を업데이트する
		 終了時の업데이트用
		---------------------------------------------------------------------------*/
		public void UpdateMemo()
		{
			if(info == null)		return;
			info.Memo	= m_memo_text_box.Text;
		}

		/*-------------------------------------------------------------------------
		 아이템목록업데이트
		---------------------------------------------------------------------------*/
		private void update_item_list()
		{
			if(info == null){
				// 내용クリア
				m_list_view.Clear();
				return;
			}

			// 업데이트開始
			m_list_view.BeginUpdate();

			// 내용クリア
			m_list_view.Clear();

			// ヘッダ
			switch(m_tab_index){
			case 0:
				m_list_view.Columns.Add("명칭",	80);
				m_list_view.Columns.Add("★",	25, HorizontalAlignment.Center);
				m_list_view.Columns.Add("종류",	52, HorizontalAlignment.Center);
				if(info.InfoType == GvoWorldInfo.InfoType.City){
					m_list_view.Columns.Add("가격",	55, HorizontalAlignment.Right);
				}else{
					m_list_view.Columns.Add("スキル", 60, HorizontalAlignment.Center);
				}
				break;
			case 3:	// スキル, 보고
				m_list_view.Columns.Add("명칭",	140);
				m_list_view.Columns.Add("인물",	120);
				break;
			case 10:	// 행상인
				m_list_view.Columns.Add("명칭",	160);
				m_list_view.Columns.Add("인물",	60);
				break;
			default:
				m_list_view.Columns.Add("명칭",	160);
				m_list_view.Columns.Add("가격",	80, HorizontalAlignment.Right);
				break;
			}

			// 追加
			int						count	= info.GetCount(GvoWorldInfo.Info.GroupIndex._0 + m_tab_index);
			GvoWorldInfo.Info.Group	g		= info.GetGroup(GvoWorldInfo.Info.GroupIndex._0 + m_tab_index);
			System.Drawing.Font		font	= m_list_view.Font;
			for(int i=0; i<count; i++){
				GvoWorldInfo.Info.Group.Data	d		= g.GetData(i);
				ListViewItem				item	= new ListViewItem(d.Name, 0);
				item.UseItemStyleForSubItems		= false;
				item.ForeColor						= d.Color;
				item.Tag							= d;
				string		tt						= get_tooltip_string_item_list(d);
				if(tt != null)	item.ToolTipText	= tt;
				if(m_tab_index == 0){
					item.SubItems.Add((d.IsBonusItem)? "★": "", Color.Tomato, item.BackColor, font);
					item.SubItems.Add(	d.Type,
										d.CategolyColor, item.BackColor, font);
				}
				item.SubItems.Add(d.Price, d.PriceColor, item.BackColor, font);
				m_list_view.Items.Add(item);
			}

			// 업데이트終了
			m_list_view.EndUpdate();

			// コラムサイズ調整
			ajust_item_columns_width();
		}

		/*-------------------------------------------------------------------------
		 아이템목록のコラム幅調整
		 最初のコラムのサイズを調整して가로スクロールバーが出ないようにする
		---------------------------------------------------------------------------*/
		private void ajust_item_columns_width()
		{
			if(m_list_view.Columns.Count < 1)	return;
			if(m_tab_index == 3)				return;		// 인물タブは特別になにもしない

			Size	size	= m_list_view.ClientSize;
			// 最初のコラム以外のコラムの幅の合計を求める
			int		width	= 0;
			for(int i=1; i<m_list_view.Columns.Count; i++){
				width	+= m_list_view.Columns[i].Width;
			}
			// 余りを最初のコラムの幅とする
			width	= size.Width - width;
			if(width <= 0)	width	= 20;
			m_list_view.Columns[0].Width	= width;
		}

		/*-------------------------------------------------------------------------
		 아이템목록に마우스があるかどうかを返す
		 フォーカス判定用
		---------------------------------------------------------------------------*/
		public bool HitTest_ItemList(Point pos)
		{
			if(base.window_mode == mode.small)		return false;	// 최소화중
			if(m_hittest_list.HitTest_Index(pos) == (int)item_index.item_list){
				return true;
			}
			return false;
		}
	}
}
