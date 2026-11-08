/*-------------------------------------------------------------------------

 게임 자료
 database\gameinfo\*.txt 를 읽어 분류별로 관리한다
 파일 하나가 분류 하나이며, 파일을 추가하면 분류가 늘어난다

 파일명	정렬 순서 "숫자_" 뒤가 분류명이 된다
 이름=x,y	좌표. 지도에 표시할 수 있다. 이름은 생략 가능
 // 제목	이후 좌표의 구역명
 그 밖의 줄	설명문

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.DirectX;

using directx;
using Utility;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class game_info
	{
		// 구분이 쉽도록 분류마다 차례로 쓰는 아이콘
		static readonly icons.icon_index[] POINT_ICONS = new icons.icon_index[]{
			icons.icon_index.map_red_circle,
			icons.icon_index.map_cyan_circle,
			icons.icon_index.map_yellow_circle,
			icons.icon_index.map_magenta_circle,
			icons.icon_index.map_blue_circle,
			icons.icon_index.map_orange_circle,
			icons.icon_index.map_white_circle,
			icons.icon_index.map_green_circle,
			icons.icon_index.map_lime_circle,
		};
		private const float HIT_SIZE = 6f;		// 툴팁 판정 크기(화면 픽셀)

		/*-------------------------------------------------------------------------
		 좌표
		---------------------------------------------------------------------------*/
		public class point
		{
			public string	name;
			public string	group;
			public Point	gpos;		// 게임 좌표
		}

		/*-------------------------------------------------------------------------
		 분류
		---------------------------------------------------------------------------*/
		public class category
		{
			public string		name;
			public string		text;
			public List<point>	points	= new List<point>();
			public bool			visible;

			/*-------------------------------------------------------------------------
			 읽기
			---------------------------------------------------------------------------*/
			static public category Load(string fname)
			{
				category	c	= new category();
				c.name			= Regex.Replace(Path.GetFileNameWithoutExtension(fname), @"^[0-9]+_", "");

				string			group	= "";
				StringBuilder	text	= new StringBuilder();
				foreach(string line in File.ReadAllLines(fname, Encoding.UTF8)){
					// 원본 자료에 x.y 로 적힌 오기가 있어 . 도 구분자로 받는다
					Match	m	= Regex.Match(line, @"^(.*?)=\s*([0-9]+)\s*[,.]\s*([0-9]+)\s*$");
					if(m.Success){
						point	p	= new point();
						p.name		= m.Groups[1].Value.Trim();
						p.group		= group;
						p.gpos		= new Point(Useful.ToInt32(m.Groups[2].Value, 0), Useful.ToInt32(m.Groups[3].Value, 0));
						c.points.Add(p);
					}else if(line.StartsWith("//")){
						string	title	= line.Substring(2).Trim();
						if(title.StartsWith("출처"))	text.AppendLine(title);
						else						group	= title;
					}else{
						text.AppendLine(line);
					}
				}
				c.text	= text.ToString().Trim();
				return c;
			}
		}

		private gvt_lib			m_lib;
		private string			m_path;
		private string			m_setting_fname;
		private List<category>	m_categories	= new List<category>();

		public List<category> categories{	get{	return m_categories;	}}
		public string path{					get{	return m_path;			}}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public game_info(gvt_lib lib, string path, string setting_fname)
		{
			m_lib			= lib;
			m_path			= path;
			m_setting_fname	= setting_fname;
			Reload();
		}

		/*-------------------------------------------------------------------------
		 파일을 다시 읽는다
		---------------------------------------------------------------------------*/
		public void Reload()
		{
			List<string>	visible	= new List<string>();
			if(m_categories.Count > 0){
				foreach(category c in m_categories){
					if(c.visible)	visible.Add(c.name);
				}
			}else if(File.Exists(m_setting_fname)){
				visible.AddRange(File.ReadAllLines(m_setting_fname, Encoding.UTF8));
			}

			m_categories.Clear();
			if(!Directory.Exists(m_path))	return;
			string[]	files	= Directory.GetFiles(m_path, "*.txt");
			Array.Sort(files, StringComparer.Ordinal);
			foreach(string f in files){
				try{
					category	c	= category.Load(f);
					c.visible		= visible.Contains(c.name);
					m_categories.Add(c);
				}catch(Exception ex){
					// 사용자가 고친 파일에 문제가 있어도 나머지는 읽는다
					Console.WriteLine(ex.ToString());
				}
			}
		}

		/*-------------------------------------------------------------------------
		 지도 표시 상태를 기록한다
		---------------------------------------------------------------------------*/
		public void WriteSetting()
		{
			List<string>	visible	= new List<string>();
			foreach(category c in m_categories){
				if(c.visible)	visible.Add(c.name);
			}
			try{
				File.WriteAllLines(m_setting_fname, visible.ToArray(), Encoding.UTF8);
			}catch(Exception ex){
				// 표시 상태를 못 남겨도 다른 설정 저장을 막지 않는다
				Console.WriteLine(ex.ToString());
			}
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		public void Draw()
		{
			bool	is_draw	= false;
			foreach(category c in m_categories){
				if(c.visible && (c.points.Count > 0))	is_draw	= true;
			}
			if(!is_draw)	return;

			m_lib.device.sprites.BeginDrawSprites(m_lib.icons.texture);
			m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_proc), 32f);
			m_lib.device.sprites.EndDrawSprites();
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		private void draw_proc(Vector2 offset, LoopXImage image)
		{
			for(int i=0; i<m_categories.Count; i++){
				category	c	= m_categories[i];
				if(!c.visible)	continue;

				d3d_sprite_rects.rect	icon	= m_lib.icons.GetIcon(POINT_ICONS[i % POINT_ICONS.Length]);
				foreach(point p in c.points){
					Point	mpos	= transform.game_pos2_map_pos(p.gpos, image);
					Vector2	v		= image.GlobalPos2LocalPos(new Vector2(mpos.X, mpos.Y), offset);
					m_lib.device.sprites.AddDrawSprites(new Vector3(v.X, v.Y, 0.5f), icon);
				}
			}
		}

		/*-------------------------------------------------------------------------
		 툴팁을 얻는다
		 좌표는 지도 좌표여야 한다
		---------------------------------------------------------------------------*/
		public string GetToolTip(Point pos)
		{
			float	size	= HIT_SIZE / m_lib.loop_image.ImageScale;
			foreach(category c in m_categories){
				if(!c.visible)	continue;
				foreach(point p in c.points){
					Point	mpos	= transform.game_pos2_map_pos(p.gpos, m_lib.loop_image);
					if((Math.Abs(mpos.X - pos.X) > size) || (Math.Abs(mpos.Y - pos.Y) > size))	continue;

					string	str	= c.name;
					if(p.group != "")	str	+= "\n" + p.group;
					if(p.name != "")	str	+= "\n" + p.name;
					return str + String.Format("\n{0}, {1}", p.gpos.X, p.gpos.Y);
				}
			}
			return null;
		}
	}
}
