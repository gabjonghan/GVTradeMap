/*-------------------------------------------------------------------------

 지도와 게임 좌표 간의 좌표 변환

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
		 Vector2 에서 Point 로
		---------------------------------------------------------------------------*/
		public static Point ToPoint(Vector2 p)
		{
			return new Point((int)p.X, (int)p.Y);
		}

		/*-------------------------------------------------------------------------
		 Point 에서 Vector2 로
		---------------------------------------------------------------------------*/
		public static Vector2 ToVector2(Point p)
		{
			return new Vector2(p.X, p.Y);
		}

		/*-------------------------------------------------------------------------
		 Size 에서 Vector2 로
		---------------------------------------------------------------------------*/
		public static Vector2 ToVector2(Size p)
		{
			return new Vector2(p.Width, p.Height);
		}

		/*-------------------------------------------------------------------------
		 Vector2 에서 Vector3 로
		---------------------------------------------------------------------------*/
		public static Vector3 ToVector3(Vector2 p, float z)
		{
			return new Vector3(p.X, p.Y, z);
		}

		/*-------------------------------------------------------------------------
		 Vector2 에서 PointF 로
		---------------------------------------------------------------------------*/
		public static PointF ToPointF(Vector2 p)
		{
			return new PointF(p.X, p.Y);
		}

		/*-------------------------------------------------------------------------
		 PointF 에서 Vector2 로
		---------------------------------------------------------------------------*/
		public static Vector2 ToVector2(PointF p)
		{
			return new Vector2(p.X, p.Y);
		}

		/*-------------------------------------------------------------------------
		 클라이언트 좌표로부터의 오프셋에서 게임 좌표로
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
		 map 좌표에서 게임 좌표로
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
		 게임 좌표에서 map 좌표로
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
		 게임 좌표에서 map 좌표로의 비를 얻는다
		 게임과 마찬가지로 가로 2 : 세로 1 의 지도를 쓰면 양쪽 다 같은 값을 반환한다
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
		 map 좌표에서 게임 좌표로의 비를 얻는다
		 게임과 마찬가지로 가로 2 : 세로 1 의 지도를 쓰면 양쪽 다 같은 값을 반환한다
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
		 차이를 얻는다
		 X 방향의 루프가 고려된다
		 v0 - v1
		---------------------------------------------------------------------------*/
		static public Vector2 SubVector_LoopX(Point v0, Point v1, int size_x)
		{
			float	y	= v0.Y - v1.Y;		// Y 는 그대로

			float	x0	= v0.X - v1.X;
			float	x1	= (v0.X - size_x) - v1.X;
			float	x2	= (v0.X + size_x) - v1.X;

			// 절댓값으로 크기를 비교한다
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
