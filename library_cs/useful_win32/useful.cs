/*-------------------------------------------------------------------------

 汎用的なもの
 staticなクラス

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Web;
using System.Text.RegularExpressions;
using System.Reflection;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace useful
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	static public class useful
	{
		/*-------------------------------------------------------------------------
		 Controlのフォントをメイリオに変更する
		 メイリオがインストールされていない場合はなにもしない
		---------------------------------------------------------------------------*/
		static public bool SetFontMeiryo(Control ctrl, float point)
		{
			Font	font	= new Font("メイリオ", point, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			if(font.Name != "メイリオ"){
				// メイリオフォントが作成できない
				return false;
			}
			ctrl.Font		= font;
			return true;
		}

		/*-------------------------------------------------------------------------
		 Formのフォントをメイリオに変更する
		 Formが持つControl全てに設定される
		 メイリオがインストールされていない場合はなにもしない
		---------------------------------------------------------------------------*/
		static public void SetFontMeiryo(Form form, float point)
		{
//			SetFontMeiryo((Control)form, point);
			foreach(Control ctrl in form.Controls){
				SetFontMeiryo(ctrl, point);
			}
		}

		/*-------------------------------------------------------------------------
		 URL エンコード
		 UTF8
		---------------------------------------------------------------------------*/
		static public string UrlEncodeUTF8(string str)
		{
			return HttpUtility.UrlEncode(str, Encoding.Unicode);
		}

		/*-------------------------------------------------------------------------
		 URL エンコード
		 EUC-JP
		---------------------------------------------------------------------------*/
		static public string UrlEncodeEUCJP(string str)
		{
			return HttpUtility.UrlEncode(str, Encoding.GetEncoding("euc-jp"));
		}
	
		/*-------------------------------------------------------------------------
		 radian -> degree
		---------------------------------------------------------------------------*/
		static public float ToDegree(float radian)
		{
			return radian * (float)(360d/(Math.PI*2));
		}

		/*-------------------------------------------------------------------------
		 degree -> radian
		---------------------------------------------------------------------------*/
		static public float ToRadian(float degree)
		{
			return degree * (float)((Math.PI*2)/360d);
		}

		/*-------------------------------------------------------------------------
		 短い日時文字列を返す
		 秒を含まない
		---------------------------------------------------------------------------*/
		public static string ToShortDateTimeString(DateTime dt)
		{
			if(dt == null)	return "unknown date time";
			string		str	= dt.ToString("yyyy/MM/dd");
			return str + dt.ToString(" HH:mm");
		}

		/*-------------------------------------------------------------------------
		 したらば風の日時文字列を返す
		---------------------------------------------------------------------------*/
		public static string TojbbsDateTimeString(DateTime dt)
		{
			if(dt == null)	return "unknown date time";
			string		str	= dt.ToString("yyyy/MM/dd");
			switch(dt.DayOfWeek){
			case DayOfWeek.Sunday:
				str		+= "(日)";
				break;
			case DayOfWeek.Monday:
				str		+= "(月)";
				break;
			case DayOfWeek.Tuesday:
				str		+= "(火)";
				break;
			case DayOfWeek.Wednesday:
				str		+= "(水)";
				break;
			case DayOfWeek.Thursday:
				str		+= "(木)";
				break;
			case DayOfWeek.Friday:
				str		+= "(金)";
				break;
			case DayOfWeek.Saturday:
				str		+= "(土)";
				break;
			}
			return str + dt.ToString(" HH:mm:ss");
		}

		/*-------------------------------------------------------------------------
		 ToShortDateTimeString()
		 TojbbsDateTimeString()
		 で得られた文字列からDateTimeに変換する
		---------------------------------------------------------------------------*/
		public static DateTime ToDateTime(string datetime)
		{
			try{
				Match	m;
				if((m = match(@"([0-9]+)/([0-9]+)/([0-9]+).* ([0-9]+):([0-9]+):([0-9]+)", datetime)) != null){
					// したらばBBS
					return new DateTime(	Convert.ToInt32(m.Groups[1].Value),
											Convert.ToInt32(m.Groups[2].Value),
											Convert.ToInt32(m.Groups[3].Value),
											Convert.ToInt32(m.Groups[4].Value),
											Convert.ToInt32(m.Groups[5].Value),
											Convert.ToInt32(m.Groups[6].Value) );
				}else if((m = match(@"([0-9]+)/([0-9]+)/([0-9]+) ([0-9]+):([0-9]+)", datetime)) != null){
					// 短い版
					return new DateTime(	Convert.ToInt32(m.Groups[1].Value),
											Convert.ToInt32(m.Groups[2].Value),
											Convert.ToInt32(m.Groups[3].Value),
											Convert.ToInt32(m.Groups[4].Value),
											Convert.ToInt32(m.Groups[5].Value), 0);
				}else{
					return new DateTime();
				}
			}catch{
				return new DateTime();
			}
		}
	
		/*-------------------------------------------------------------------------
		 正規表現で調べる
		 マッチしたときは Match を返す
		 マッチしない場合は null を返す
		---------------------------------------------------------------------------*/
		static public Match match(string regex, string str)
		{
			Match	m	= Regex.Match(str, regex);
			if(!m.Success)	return null;
			return m;
		}

		/*-------------------------------------------------------------------------
		 1,000
		 のような文字列を返す
		---------------------------------------------------------------------------*/
		static public string ToComma3(int num)
		{
			try{
				return String.Format("{0:#,0}", num);
			}catch{
				return "0";
			}
		}

		/*-------------------------------------------------------------------------
		 Assemblyを読み込む
		 MDX1.1がインストールされているかどうかを得ることができる
		 読み込みに失敗したAssemblyNameを返す
		 読み込みに成功した場合はnullを返す
		---------------------------------------------------------------------------*/
		static public bool LoadReferencedAssembly(Assembly ass, out AssemblyName error_ass)
		{
			error_ass	= null;
			if(ass == null)		return false;

			AssemblyName[]	names	= ass.GetReferencedAssemblies();
			foreach(AssemblyName i in names){
				try{
					Assembly	ddd	= Assembly.Load(i);
				}catch{
					// 失敗
					error_ass	= i;
					return false;
				}
			}
			return true;
		}
	}
}
