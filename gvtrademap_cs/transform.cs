/*-------------------------------------------------------------------------

 지도と게임좌표間の좌표변환

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Drawing;
using Microsoft.DirectX;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	public static class transform
	{
		/*-------------------------------------------------------------------------
		 Vector2 から Point へ
		---------------------------------------------------------------------------*/
		public static Point ToPoint(Vector2 p)
		{
			return new Point((int)p.X, (int)p.Y);
		}

		/*-------------------------------------------------------------------------
		 Point から Vector2 へ
		---------------------------------------------------------------------------*/
		public static Vector2 ToVector2(Point p)
		{
			return new Vector2(p.X, p.Y);
		}

		/*-------------------------------------------------------------------------
		 Size から Vector2 へ
		---------------------------------------------------------------------------*/
		public static Vector2 ToVector2(Size p)
		{
			return new Vector2(p.Width, p.Height);
		}

		/*-------------------------------------------------------------------------
		 Vector2 から Vector3 へ
		---------------------------------------------------------------------------*/
		public static Vector3 ToVector3(Vector2 p, float z)
		{
			return new Vector3(p.X, p.Y, z);
		}

		/*-------------------------------------------------------------------------
		 Vector2 から PointF へ
		---------------------------------------------------------------------------*/
		public static PointF ToPointF(Vector2 p)
		{
			return new PointF(p.X, p.Y);
		}

		/*-------------------------------------------------------------------------
		 PointF から Vector2 へ
		---------------------------------------------------------------------------*/
		public static Vector2 ToVector2(PointF p)
		{
			return new Vector2(p.X, p.Y);
		}

		/*-------------------------------------------------------------------------
		 クライアント좌표からのオフセットから게임좌표へ
		---------------------------------------------------------------------------*/
		public static Vector2 client_pos2_game_pos(Vector2 pos, LoopXImage loop_image)
		{
			return map_pos2_game_pos(loop_image.MousePos2GlobalPos(pos), loop_image);
		}
		public static Point client_pos2_game_pos(Point pos, LoopXImage loop_image)
		{
			return ToPoint(map_pos2_game_pos(loop_image.MousePos2GlobalPos(ToVector2(pos)), loop_image));
		}
	
		/*-------------------------------------------------------------------------
		 map좌표から게임좌표へ
		---------------------------------------------------------------------------*/
		public static Vector2 map_pos2_game_pos(Vector2 pos, LoopXImage loop_image)
		{
			if(pos.X < 0)					pos.X	= 0;
			if(pos.Y < 0)					pos.Y	= 0;
			if(pos.Y >= loop_image.ImageSize.Y)	pos.Y	= loop_image.ImageSize.Y-1;
	
			pos.X	= pos.X - (((int)(pos.X / loop_image.ImageSize.X)) * loop_image.ImageSize.X);

			pos.X	= pos.X * get_rate_to_game_x(loop_image);
			pos.Y	= pos.Y * get_rate_to_game_y(loop_image);
			return pos;
		}
		public static Point map_pos2_game_pos(Point pos, LoopXImage loop_image)
		{
			return ToPoint(map_pos2_game_pos(ToVector2(pos), loop_image));
		}

		/*-------------------------------------------------------------------------
		 게임좌표からmap좌표へ
		---------------------------------------------------------------------------*/
		public static Vector2 game_pos2_map_pos(Vector2 pos, LoopXImage loop_image)
		{
			if(pos.X < 0)					pos.X	= 0;
			if(pos.Y < 0)					pos.Y	= 0;
			if(pos.Y >= def.GAME_HEIGHT)	pos.Y	= def.GAME_HEIGHT-1;

			pos.X	= pos.X - (((int)(pos.X / def.GAME_WIDTH)) * def.GAME_WIDTH);

			pos.X	= pos.X * get_rate_to_map_x(loop_image);
			pos.Y	= pos.Y * get_rate_to_map_y(loop_image);
			return pos;
		}
		public static Point game_pos2_map_pos(Point pos, LoopXImage loop_image)
		{
			return ToPoint(game_pos2_map_pos(ToVector2(pos), loop_image));
		}

		/*-------------------------------------------------------------------------
		 게임좌표からmap좌표への比を得る
		 게임同様, 가로2:세로1の지도を사용しているとどちらも同じ値を返す
		---------------------------------------------------------------------------*/
		public static float get_rate_to_map_x(LoopXImage loop_image)
		{
			return loop_image.ImageSize.X / def.GAME_WIDTH;
		}
		public static float get_rate_to_map_y(LoopXImage loop_image)
		{
			return loop_image.ImageSize.Y / def.GAME_HEIGHT;
		}
		/*-------------------------------------------------------------------------
		 map좌표から게임좌표への比を得る
		 게임同様, 가로2:세로1の지도を사용しているとどちらも同じ値を返す
		---------------------------------------------------------------------------*/
		public static float get_rate_to_game_x(LoopXImage loop_image)
		{
			return (float)def.GAME_WIDTH / loop_image.ImageSize.X;
		}
		public static float get_rate_to_game_y(LoopXImage loop_image)
		{
			return (float)def.GAME_HEIGHT / loop_image.ImageSize.Y;
		}
	
		/*-------------------------------------------------------------------------
		 差を得る
		 X방향のループが考慮される
		 v0 - v1
		---------------------------------------------------------------------------*/
		static public Vector2 SubVector_LoopX(Point v0, Point v1, int size_x)
		{
			float	y	= v0.Y - v1.Y;		// Yはそのまま

			float	x0	= v0.X - v1.X;
			float	x1	= (v0.X - size_x) - v1.X;
			float	x2	= (v0.X + size_x) - v1.X;

			// 絶対値で대きさを比べる
			float	x00	= Math.Abs(x0);
			float	x01	= Math.Abs(x1);
			float	x02	= Math.Abs(x2);
	
			if(x00 < x01){
				if(x00 > x02)	x0	= x2;
			}else{
				x00		= x01;
				x0		= x1;
				if(x00 > x02)	x0	= x2;
			}
			return new Vector2(x0, y);
		}
	}
}
