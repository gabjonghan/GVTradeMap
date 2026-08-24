/*-------------------------------------------------------------------------

 定수定義

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using Utility;
using System.IO;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvo_base
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	static public class gvo_def
	{
		// 대항해시대Online 의 윈도우 검색용
		public const string		GVO_CLASS_NAME				= "Greate Voyages Online Game MainFrame";
		public const string		GVO_WINDOW_NAME				= "대항해시대 온라인";

		// 유저 데이터 경로
		public const string		GVO_USERDATA_PATH			= @"KOEI\GV Online\";
		// 로그 경로
		public const string		GVO_LOG_PATH				= GVO_USERDATA_PATH + @"Log\Chat\";
		// 메일 경로
		public const string		GVO_MAIL_PATH				= GVO_USERDATA_PATH + @"Mail\";
		// 스크린샷 경로
		public const string		GVO_SCREENSHOT_PATH			= GVO_USERDATA_PATH + @"ScreenShot\";

		// URL
		public const string		URL2						= @"http://www.umiol.com/db/recipe.php?cmd=recsrc&submit=%B8%A1%BA%F7&recsrckey=";
		public const string		URL3						= @"http://www.umiol.com/db/recipe.php?cmd=prosrc&submit=%B8%A1%BA%F7&prosrckey=";

		/*-------------------------------------------------------------------------
		 대항해시대Online 로그의 전체 경로를 얻는다
		---------------------------------------------------------------------------*/
		static public string GetGvoLogPath()
		{
			return Path.Combine(Useful.GetMyDocumentPath(), GVO_LOG_PATH);
		}

		/*-------------------------------------------------------------------------
		 대항해시대Online 메일의 전체 경로를 얻는다
		---------------------------------------------------------------------------*/
		static public string GetGvoMailPath()
		{
			return Path.Combine(Useful.GetMyDocumentPath(), GVO_MAIL_PATH);
		}

		/*-------------------------------------------------------------------------
		 대항해시대Online 메일의 전체 경로를 얻는다
		---------------------------------------------------------------------------*/
		static public string GetGvoScreenShotPath()
		{
			return Path.Combine(Useful.GetMyDocumentPath(), GVO_SCREENSHOT_PATH);
		}
	}
}
