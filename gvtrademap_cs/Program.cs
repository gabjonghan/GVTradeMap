using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using Utility;

namespace gvtrademap_cs
{
	static class Program
	{
		/// <summary>
		/// 애플리케이션의 메인 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Mutex	m	= new Mutex(false, "mutex_gvtrademap_cs_cookie_Zephyros");
			string	device_info_string	= "";
	
			if(m.WaitOne(0, false)){
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				// MDX1.1 확인
				AssemblyName	error_ass	= null;
				if(!Useful.LoadReferencedAssembly(Assembly.GetExecutingAssembly(), out error_ass)){
					// Assembly 읽기에 실패
					// 주로 MDX1.1 이 설치되어 있지 않은 경우
					using(assembly_load_error_form dlg = new assembly_load_error_form(error_ass)){
						dlg.ShowDialog();
					}
					return;
				}
#if !DEBUG
				try{
#endif
					using(gvtrademap_cs_form frm = new gvtrademap_cs_form())
					{
						if(frm.Initialize()){		// 初期化
							device_info_string	= frm.device_info_string;
							frm.Show();				// 표시開始

							int		old_tick_count	= System.Environment.TickCount;
							while(frm.Created){
								// 초당 60 프레임을 한계로 삼는다
								// 그다지 정확도가 높지는 않을 것이다
								if(System.Environment.TickCount - old_tick_count >= 1000/60){
									old_tick_count	= System.Environment.TickCount;
									frm.update_main_window();
								}
								Application.DoEvents();
								Thread.Sleep(1);		// 가능한 한 CPU 를 쓰지 않는다
														// 처리가 제때 끝나지 않아도 반드시 쉰다
							}
						}
					}
#if !DEBUG
				}catch(Exception ex){
					// 예기치 못한 오류
					using(
						error_form	dlg = new error_form(def.WINDOW_TITLE,
														ex,
														def.WINDOW_TITLE,
														def.URL4,
														(device_info_string != "")? device_info_string: "未取得") ){
						dlg.ShowDialog();
					}
				}
#endif
			}else{
				// 이미 실행 중이다
				// 활성화시키고 종료한다
				gvtrademap_cs_form.ActiveGVTradeMap();
			}
			// m 이 GC 에 의해 해제되지 않도록 한다
			GC.KeepAlive(m);
		}
	}
}
