/*-------------------------------------------------------------------------

 상수 정의

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

		// URL (검색어는 UTF-8 로 URL 인코드해서 끝에 붙인다)
		// 레시피 책 검색 (대항해시대 조이)
		public const string		URL2						= @"https://www.ssjoy.org/dho/?act=&mid=recipeBook&vid=&citySrl%5B%5D=&city%5B%5D=&searchKeyword=";
		// 레시피 검색 (대항해시대 조이)
		public const string		URL3						= @"https://www.ssjoy.org/dho/?act=&mid=recipe&vid=&ingredientSrl%5B%5D=&ingredient%5B%5D=&productSrl%5B%5D=&product%5B%5D=&searchKeyword=";

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
