/*-------------------------------------------------------------------------

 QueryPerformanceCounter()を使ったタイマ
 処理시간を計ったりできる

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

		private long				m_last_counter;						// GetElapsedTime()용前회のカウント値
		private long				m_section_start_counter;			// 

		private long				m_qpf_ticks_per_sec;				// 1초間に増えるカウンタ
		private float				m_inv_qpf_ticks_per_sec;			// 1초間に増えるカウンタの逆수

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
		 前회からの経過시간を得る
		 1.f == 1초
		---------------------------------------------------------------------------*/
		public float GetElapsedTime()
		{
			long	t		= 0;

			QueryPerformanceCounter(ref t);
			float	elapsed_time	= m_inv_qpf_ticks_per_sec * (float)(t - m_last_counter);
			m_last_counter	= t;	// 次회용に覚えておく
			return elapsed_time;
		}

		/*-------------------------------------------------------------------------
		 前회からの経過시간を得る
		 ミリセカンドで得る
		 1000 == 1초
		---------------------------------------------------------------------------*/
		public int GetElapsedTimeMilliseconds()
		{
			return (int)(GetElapsedTime() * 1000f);
		}

		/*-------------------------------------------------------------------------
		 区間経過시간
		 計測開始
		---------------------------------------------------------------------------*/
		public void StartSection()
		{
			long	t		= 0;

			QueryPerformanceCounter(ref t);
			m_section_start_counter	= t;
		}

		/*-------------------------------------------------------------------------
		 計測開始からの経過시간
		---------------------------------------------------------------------------*/
		public float GetSectionTime()
		{
			long	t		= 0;

			QueryPerformanceCounter(ref t);
			return m_inv_qpf_ticks_per_sec * (float)(t - m_section_start_counter);
		}

		/*-------------------------------------------------------------------------
		 지정された시간が経過するまで待つ
		 長시간待たせるとCPU시간を多く消費するので注意
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
