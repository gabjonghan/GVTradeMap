/*-------------------------------------------------------------------------

 QueryPerformanceCounter()を使ったタイマ
 処理時間を計ったりできる

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System.Runtime.InteropServices;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace Utility
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class qpctimer
	{
		[DllImport("kernel32.dll")]
		extern static short QueryPerformanceCounter(ref long x);

		[DllImport("kernel32.dll")]
		extern static short QueryPerformanceFrequency(ref long x);

		private long				m_last_counter;						// GetElapsedTime()用前회のカウント値
		private long				m_section_start_counter;			// 

		private long				m_qpf_ticks_per_sec;				// 1초間に増えるカウンタ
		private float				m_inv_qpf_ticks_per_sec;			// 1초間に増えるカウンタの逆数

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public qpctimer()
		{
			m_last_counter				= 0;
			m_inv_qpf_ticks_per_sec		= 0;
			m_section_start_counter		= 0;

			long	ticks_per_sec		= 0;

			QueryPerformanceFrequency(ref ticks_per_sec);
			m_qpf_ticks_per_sec		= ticks_per_sec;
			m_inv_qpf_ticks_per_sec	= (float)(1d / (double)ticks_per_sec);

			// 1度目は捨てる
			GetElapsedTime();
		}

		/*-------------------------------------------------------------------------
		 前회からの経過時間を得る
		 1.f == 1초
		---------------------------------------------------------------------------*/
		public float GetElapsedTime()
		{
			long	t		= 0;

			QueryPerformanceCounter(ref t);
			float	elapsed_time	= m_inv_qpf_ticks_per_sec * (float)(t - m_last_counter);
			m_last_counter	= t;	// 次회用に覚えておく
			return elapsed_time;
		}

		/*-------------------------------------------------------------------------
		 前회からの経過時間を得る
		 ミリセカンドで得る
		 1000 == 1초
		---------------------------------------------------------------------------*/
		public int GetElapsedTimeMilliseconds()
		{
			return (int)(GetElapsedTime() * 1000f);
		}

		/*-------------------------------------------------------------------------
		 区間経過時間
		 計測開始
		---------------------------------------------------------------------------*/
		public void StartSection()
		{
			long	t		= 0;

			QueryPerformanceCounter(ref t);
			m_section_start_counter	= t;
		}

		/*-------------------------------------------------------------------------
		 計測開始からの経過時間
		---------------------------------------------------------------------------*/
		public float GetSectionTime()
		{
			long	t		= 0;

			QueryPerformanceCounter(ref t);
			return m_inv_qpf_ticks_per_sec * (float)(t - m_section_start_counter);
		}

		/*-------------------------------------------------------------------------
		 指定された時間が経過するまで待つ
		 長時間待たせるとCPU時間を多く消費するので注意
		---------------------------------------------------------------------------*/
		public void WaitSectionTime(float section)
		{
			long	s	= (long)(section * m_qpf_ticks_per_sec);

			long	t	= 0;
			while(true){
				QueryPerformanceCounter(ref t);
				if((t - m_section_start_counter) >= s)		break;
			}
		}
	}
}
