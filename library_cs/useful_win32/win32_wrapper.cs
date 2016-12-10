/*-------------------------------------------------------------------------

 WIN32ラッパー

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace win32
{
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	static public class user32
	{
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpStr, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindowA(string pClassName, string pWindowName);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd,ref Rectangle rect);

		[DllImport("user32.dll")]
		public static extern bool ClientToScreen(IntPtr hWnd,ref Point p);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern void ReleaseDC(IntPtr hWnd, IntPtr hDC);

        //キーボードイベント処理
        [DllImport("user32.dll")]
        public static extern int keybd_event(int VK, int scan, int flags,int extinfo);

		[DllImport("user32.dll")]
		public static extern short GetKeyState(int nVirtKey);

		// Message
		private const int WM_KEYDOWN	= 0x0100;
		private const int WM_KEYUP		= 0x0101;
		private const int WM_CHAR		= 0x0102;

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd , int msg , IntPtr wp, IntPtr lp);
		[DllImport("user32.dll")]
		public static extern int PostMessage(IntPtr hWnd , int msg , IntPtr wp, IntPtr lp);

		// VK
		public const int		VK_BACK			= 0x08;
		public const int		VK_TAB			= 0x09;
		public const int		VK_SHIFT		= 0x10;
		public const int		VK_CONTROL		= 0x11;
		public const int		VK_MENU			= 0x12;
		public const int		VK_RETURN		= 0x0d;
		public const int		VK_ESCAPE		= 0x1b;
		public const int		VK_SPACE		= 0x20;
		public const int		VK_INSERT		= 0x2d;
		public const int		VK_DELETE		= 0x2e;
		public const int		VK_0			= 0x30;
		public const int		VK_1			= 0x31;
		public const int		VK_2			= 0x32;
		public const int		VK_3			= 0x33;
		public const int		VK_4			= 0x34;
		public const int		VK_5			= 0x35;
		public const int		VK_6			= 0x36;
		public const int		VK_7			= 0x37;
		public const int		VK_8			= 0x38;
		public const int		VK_9			= 0x39;
		public const int		VK_A			= 0x41;
		public const int		VK_B			= 0x42;
		public const int		VK_C			= 0x43;
		public const int		VK_D			= 0x44;
		public const int		VK_E			= 0x45;
		public const int		VK_F			= 0x46;
		public const int		VK_G			= 0x47;
		public const int		VK_H			= 0x48;
		public const int		VK_I			= 0x49;
		public const int		VK_J			= 0x4a;
		public const int		VK_K			= 0x4b;
		public const int		VK_L			= 0x4c;
		public const int		VK_M			= 0x4d;
		public const int		VK_N			= 0x4e;
		public const int		VK_O			= 0x4f;
		public const int		VK_P			= 0x50;
		public const int		VK_Q			= 0x51;
		public const int		VK_R			= 0x52;
		public const int		VK_S			= 0x53;
		public const int		VK_T			= 0x54;
		public const int		VK_U			= 0x55;
		public const int		VK_V			= 0x56;
		public const int		VK_W			= 0x57;
		public const int		VK_X			= 0x58;
		public const int		VK_Y			= 0x59;
		public const int		VK_Z			= 0x5a;
		public const int		VK_F1			= 0x70;
		public const int		VK_F2			= 0x71;
		public const int		VK_F3			= 0x72;
		public const int		VK_F4			= 0x73;
		public const int		VK_F5			= 0x74;
		public const int		VK_F6			= 0x75;
		public const int		VK_F7			= 0x76;
		public const int		VK_F8			= 0x77;
		public const int		VK_F9			= 0x78;
		public const int		VK_F10			= 0x79;
		public const int		VK_F11			= 0x7a;
		public const int		VK_F12			= 0x7b;

		/*-------------------------------------------------------------------------
		 キーボードイベントを発生させる
		---------------------------------------------------------------------------*/
		static public void PostMessage_KEYDOWN(IntPtr handle, int vk, int flag)
		{
			if(handle == (IntPtr)0)	return;
			PostMessage(handle, WM_KEYDOWN, (IntPtr)vk, (IntPtr)flag);
		}
		static public void PostMessage_KEYUP(IntPtr handle, int vk, int flag)
		{
			if(handle == (IntPtr)0)	return;
			PostMessage(handle, WM_KEYUP, (IntPtr)vk, (IntPtr)flag);
		}
	}

	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
    public class gdi32
    {
		[DllImport("gdi32.dll", EntryPoint = "CreateDCA")]
		public static extern IntPtr CreateDC(string lpDriverName, string lpDeviceName, string lpOutput, string lpInitData);

		[DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

		[DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

		[DllImport("gdi32.dll", EntryPoint = "SelectObject")]
		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		[DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
		public static extern int DeleteDC(IntPtr hDC);

		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		public static extern int DeleteObject(IntPtr hObj);

		[DllImport("gdi32.dll", EntryPoint = "BitBlt")]
		public static extern int BitBlt(IntPtr desthDC, int destX, int destY, int destW, int destH,
										IntPtr srchDC, int srcX, int srcY, int op);

		public const int SRCCOPY		= 0xCC0020;
		public const int BLACKNESS		= 0x42;
	}

	/*-------------------------------------------------------------------------
 
	---------------------------------------------------------------------------*/
    public class kernel32
	{
		[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr LoadLibrary(string lpFileName);
		[DllImport("kernel32", SetLastError = true)]
		public static extern bool FreeLibrary(IntPtr hModule);
		[DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = false)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
	}
}
