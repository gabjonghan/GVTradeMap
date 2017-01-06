/*-------------------------------------------------------------------------

 위도, 경도그리기
 게임の仕様상
 GAME_WIDTH		= 16384;
 GAME_HEIGHT	= 8192;
 を사용する
 100毎に선が引かれる

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using System.Drawing;

using directx;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	static class latitude_longitude
	{
		/*-------------------------------------------------------------------------
		 선그리기
		 1000단위の선となる
		---------------------------------------------------------------------------*/
		static public void DrawLines(gvt_lib lib)
		{
			lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_lines_proc), 0);

			// 세로방향は1회の그리기でよい
			LoopXImage	image	= lib.loop_image;
			Vector2		size	= image.Device.client_size;
			Vector2	offset = image.GetDrawOffset();

			int index	= 0;
			for(float y=0; y<def.GAME_HEIGHT; y+=1000, index++){
				// 지도좌표に변환
				Vector2	pos0	= transform.game_pos2_map_pos(new Vector2(0, y), image);
				Vector2 pos		= image.GlobalPos2LocalPos(pos0, offset);

				if(pos.Y < 0)		continue;
				if(pos.Y >= size.Y)	continue;

				image.Device.DrawLine(new Vector3(0, pos.Y, 0.79f), new Vector2(size.X, pos.Y), Color.FromArgb(128, 0, 0, 0).ToArgb());
			}
		}

		/*-------------------------------------------------------------------------
		 선그리기
		---------------------------------------------------------------------------*/
		static private void draw_lines_proc(Vector2 offset, LoopXImage image)
		{
			Vector2		size	= image.Device.client_size;
	
			int			index	= 0;
			for(float x=0; x<def.GAME_WIDTH; x+=1000, index++){
				// 지도좌표に변환
				Vector2	pos0	= transform.game_pos2_map_pos(new Vector2(x, 0), image);
				Vector2 pos		= image.GlobalPos2LocalPos(pos0, offset);

				if(pos.X < 0)		continue;
				if(pos.X >= size.X)	continue;

				image.Device.DrawLine(new Vector3(pos.X, 0, 0.79f), new Vector2(pos.X, size.Y), Color.FromArgb(128, 0, 0, 0).ToArgb());
			}
		}
	
		/*-------------------------------------------------------------------------
		 선그리기
		 100단위の선となる
		---------------------------------------------------------------------------*/
		static public void DrawLines100(gvt_lib lib)
		{
			lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_lines100_proc), 0);

			// 세로방향は1회の그리기でよい
			LoopXImage	image	= lib.loop_image;
			Vector2		size	= image.Device.client_size;
			Vector2	offset = image.GetDrawOffset();

			int index	= 0;
			for(float y=0; y<def.GAME_HEIGHT; y+=100, index++){
				// 지도좌표に변환
				Vector2	pos0	= transform.game_pos2_map_pos(new Vector2(0, y), image);
				Vector2 pos		= image.GlobalPos2LocalPos(pos0, offset);

				if(index >= 10)	index	= 0;

				if(pos.Y < 0)		continue;
				if(pos.Y >= size.Y)	continue;

				int		color	= (index == 0)? Color.FromArgb(128, 0, 0, 0).ToArgb(): Color.FromArgb(128, 128, 128, 128).ToArgb();
				image.Device.DrawLine(new Vector3(0, pos.Y, 0.79f), new Vector2(size.X, pos.Y), color);
			}
		}

		/*-------------------------------------------------------------------------
		 선그리기
		---------------------------------------------------------------------------*/
		static private void draw_lines100_proc(Vector2 offset, LoopXImage image)
		{
			Vector2		size	= image.Device.client_size;
	
			int			index	= 0;
			for(float x=0; x<def.GAME_WIDTH; x+=100, index++){
				// 지도좌표に변환
				Vector2	pos0	= transform.game_pos2_map_pos(new Vector2(x, 0), image);
				Vector2 pos		= image.GlobalPos2LocalPos(pos0, offset);

				if(index >= 10)	index	= 0;

				if(pos.X < 0)		continue;
				if(pos.X >= size.X)	continue;

				int		color	= (index == 0)? Color.FromArgb(128, 0, 0, 0).ToArgb(): Color.FromArgb(128, 128, 128, 128).ToArgb();
				image.Device.DrawLine(new Vector3(pos.X, 0, 0.79f), new Vector2(pos.X, size.Y), color);
			}
		}

		/*-------------------------------------------------------------------------
		 위도, 경도그리기
		 화면の상と우に그리기される
		---------------------------------------------------------------------------*/
		static public void DrawPoints(gvt_lib lib)
		{
			d3d_systemfont	font	= lib.device.systemfont;

			font.Begin();
			lib.device.device.RenderState.ZBufferEnable		= false;
			lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_points_proc), 0);

			// 세로방향は1회の그리기でよい	
			Vector2		size	= lib.loop_image.Device.client_size;
			Vector2	offset = lib.loop_image.GetDrawOffset();
			for(float y=0; y<def.GAME_HEIGHT; y+=1000){
				Vector2	pos0	= transform.game_pos2_map_pos(new Vector2(0, y), lib.loop_image);
				Vector2 pos		= lib.loop_image.GlobalPos2LocalPos(pos0, offset);

				Rectangle		rect	= font.MeasureText(y.ToString());

				pos.Y	-= 5;
				if(pos.Y + (rect.Height+4) < 0)	continue;
				if(pos.Y >= size.Y)				continue;

				lib.device.DrawFillRect(new Vector3(size.X - (rect.Width) - 2, pos.Y-1, 0.1f), new Vector2(rect.Width + 2*2, rect.Height), Color.FromArgb(220, 100, 100, 100).ToArgb());
				font.DrawTextR(y.ToString(), (int)size.X, (int)pos.Y, Color.White);
			}
			font.End();
			lib.device.device.RenderState.ZBufferEnable		= true;
		}

		/*-------------------------------------------------------------------------
		 위도, 경도그리기
		---------------------------------------------------------------------------*/
		static private void draw_points_proc(Vector2 offset, LoopXImage image)
		{
			d3d_systemfont	font	= image.Device.systemfont;
			Vector2			size	= image.Device.client_size;

			for(float x=0; x<def.GAME_WIDTH; x+=1000){
				Vector2	pos0	= transform.game_pos2_map_pos(new Vector2(x, 0), image);
				Vector2 pos		= image.GlobalPos2LocalPos(pos0, offset);

				Rectangle		rect	= font.MeasureText(x.ToString());

				if(pos.X + (rect.Width+4) < 0)	continue;
				if(pos.X >= size.X)				continue;
				
				image.Device.DrawFillRect(new Vector3(pos.X - (rect.Width/2) - 3, 0, 0.1f), new Vector2(rect.Width + 2*2, rect.Height), Color.FromArgb(220, 100, 100, 100).ToArgb());
				font.DrawTextC(x.ToString(), (int)pos.X, 0, Color.White);
			}
		}
	}
}
