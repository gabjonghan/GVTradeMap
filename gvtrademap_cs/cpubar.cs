/*-------------------------------------------------------------------------

 時間をバーで표시する

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using directx;
using System.Drawing;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class cpubar
	{
		const int	PEAK_WAIT	= 15;
		const int	PEAK_STEP	= 2;

		private d3d_device					m_device;

		private int							m_peak;
		private int							m_peak_wait;

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public cpubar(d3d_device device)
		{
			m_device	= device;

			m_peak		= 0;
			m_peak_wait	= 0;
		}

		/*-------------------------------------------------------------------------
		 업데이트と그리기
		---------------------------------------------------------------------------*/
		public void Update(float val, float max)
		{
			int		size	= (int)((val / max) * 200f);
			int		color;
	
			if(size > 200){
				// 処理落ち
				m_peak		= 200;
				m_peak_wait	= PEAK_WAIT;
				size		= 200;
				color		= Color.Red.ToArgb();
			}else{
				if(size > m_peak){
					m_peak		= size;
					m_peak_wait	= PEAK_WAIT;
				}else{
					if(--m_peak_wait <= 0){
						m_peak		-= PEAK_STEP;
						if(m_peak < 0)	m_peak	= 0;
						m_peak_wait	= 0;
					}
				}
				color		= Color.LightGreen.ToArgb();
			}

			m_device.DrawFillRect(	new Vector3(0, 0, 0.1f),
										new Vector2(200, 4),
										Color.Gray.ToArgb());
			m_device.DrawFillRect(	new Vector3(0, 0, 0.1f),
										new Vector2(size, 4),
										color);
			m_device.DrawLine(	new Vector3(m_peak, 0, 0.1f),
										new Vector2(m_peak, 4),
										Color.Red.ToArgb());
			m_device.DrawLineRect(	new Vector3(0, 0, 0.1f),
										new Vector2(200, 4), Color.Black.ToArgb());

		}
	}
}
