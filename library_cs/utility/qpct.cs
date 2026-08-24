/*-------------------------------------------------------------------------

 QueryPerformanceCounter() 를 사용한 타이머
 처리 시간을 잴 수 있다

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

		private long				m_last_counter;						// GetElapsedTime() 용 지난번 카운트 값
		private long				m_section_start_counter;			// 

		private long				m_qpf_ticks_per_sec;				// 1 초 동안 증가하는 카운터
		private float				m_inv_qpf_ticks_per_sec;			// 1 초 동안 증가하는 카운터의 역수

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

			// 첫 번째는 버린다
			GetElapsedTime();
		}

		/*-------------------------------------------------------------------------
		 지난번부터의 경과 시간을 얻는다
		 1.f == 1초
		---------------------------------------------------------------------------*/
		public float GetElapsedTime()
		{
			long	t		= 0;

			QueryPerformanceCounter(ref t);
			float	elapsed_time	= m_inv_qpf_ticks_per_sec * (float)(t - m_last_counter);
			m_last_counter	= t;	// 다음번을 위해 기억해 둔다
			return elapsed_time;
		}

		/*-------------------------------------------------------------------------
		 지난번부터의 경과 시간을 얻는다
		 밀리세컨드로 얻는다
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
		 측정 시작부터의 경과 시간
		---------------------------------------------------------------------------*/
		public float GetSectionTime()
		{
			long	t		= 0;

			QueryPerformanceCounter(ref t);
			return m_inv_qpf_ticks_per_sec * (float)(t - m_section_start_counter);
		}

		/*-------------------------------------------------------------------------
		 지정된 시간이 경과할 때까지 기다린다
		 오래 기다리게 하면 CPU 시간을 많이 소비하므로 주의
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
