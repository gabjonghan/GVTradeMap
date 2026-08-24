/*-------------------------------------------------------------------------

 mouse hook

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using win32;
using System.Windows.Forms;
using System.Runtime.InteropServices;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------
 
	---------------------------------------------------------------------------*/
	class globalmouse_hook : IDisposable
	{
		public enum SendKeyType{
			TOGGLE_SKILL	= 1,	// 스킬 패널 열고 닫기			Ctrl+Q
			TOGGLE_CUSTOM_SLOT,		// 커스텀 슬롯 열기		Ctrl+Z
			OPEN_ITEM_WINDOW,		// 아이템 윈도우 열기	Ctrl+W
		};
	
		private IntPtr					m_handle;		// DLL 의 핸들

		// DLL 안의 함수
		private delegate int SetDolMouseHookEx(int xbutton1_keytype, int xbutton2_keytype);
		private delegate int UnhookDolMouseHook();
	
		/*-------------------------------------------------------------------------
		 초기화
		 마우스 훅을 시작한다
		---------------------------------------------------------------------------*/
		public globalmouse_hook()
			: this(SendKeyType.TOGGLE_SKILL, SendKeyType.OPEN_ITEM_WINDOW)
		{
		}
		public globalmouse_hook(SendKeyType xbutton1, SendKeyType xbutton2)
		{
			m_handle		= kernel32.LoadLibrary("mousehook.dll");
			if(m_handle == IntPtr.Zero){
				MessageBox.Show("mousehook.dll 의 읽기에 실패");
				return;
			}

			IntPtr	func	= kernel32.GetProcAddress(m_handle, "SetDolMouseHookEx");
			if(func == IntPtr.Zero){
				MessageBox.Show("SetDolMouseHookEx() 의 주소 획득 실패");
				return;
			}

			SetDolMouseHookEx	setDolMouseHook = (SetDolMouseHookEx)Marshal.GetDelegateForFunctionPointer(func, typeof(SetDolMouseHookEx));
			setDolMouseHook((int)xbutton1, (int)xbutton2);
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		~globalmouse_hook()
		{
			Dispose();
		}

		/*-------------------------------------------------------------------------
		 훅되어 있으면 종료시킨다
		---------------------------------------------------------------------------*/
		public void Dispose()
		{
			if(m_handle == IntPtr.Zero){
				return;
			}

			IntPtr	func	= kernel32.GetProcAddress(m_handle, "UnhookDolMouseHook");
			if(func == IntPtr.Zero){
				MessageBox.Show("UnhookDolMouseHook() 의 주소 획득에 실패");
				return;
			}

			UnhookDolMouseHook	unhookDolMouseHook = (UnhookDolMouseHook)Marshal.GetDelegateForFunctionPointer(func, typeof(UnhookDolMouseHook));
			unhookDolMouseHook();

			kernel32.FreeLibrary(m_handle);
			m_handle		= IntPtr.Zero;
		}
	}
}
