//-------------------------------------------------------------------------
// 범용적인 것들
// static 클래스
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Xml;

//-------------------------------------------------------------------------
namespace Utility
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// 범용적인 것들, static 클래스
	/// </summary>
	static public class Useful
	{
		//-------------------------------------------------------------------------
		/// <summary>
		/// MyDocument 폴더의 경로를 얻는다
		/// </summary>
		/// <returns>MyDocument 폴더의 경로</returns>
		static public string GetMyDocumentPath()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Control 의 폰트를 메이리오로 변경한다.
		/// 메이리오가 설치되어 있지 않은 경우에는 아무것도 하지 않는다
		/// </summary>
		/// <param name="ctrl">대상 컨트롤</param>
		/// <param name="point">폰트 크기</param>
		/// <returns>성공하면 true 를 반환한다</returns>
		static public bool SetFontMeiryo(Control ctrl, float point)
		{
			try
			{
				Font font = new Font("メイリオ", point, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
				if (font.Name != "メイリオ")
				{
					// 메이리오 폰트를 작성할 수 없다
					return false;
				}
				ctrl.Font = font;
			}
			catch
			{
				// 실패
				return false;
			}
			return true;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Form 의 폰트를 메이리오로 변경한다.
		/// Form 이 가진 Control 전부에 설정된다.
		/// 메이리오가 설치되어 있지 않은 경우에는 아무것도 하지 않는다
		/// </summary>
		/// <param name="form">대상 컨트롤</param>
		/// <param name="point">폰트 크기</param>
		static public void SetFontMeiryo(Form form, float point)
		{
			//			SetFontMeiryo((Control)form, point);
			foreach (Control ctrl in form.Controls)
			{
				SetFontMeiryo(ctrl, point);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// URL 인코드, UTF8
		/// </summary>
		/// <param name="str">변환할 문자열</param>
		/// <returns>변환 후의 문자열</returns>
		static public string UrlEncodeUTF8(string str)
		{
			return HttpUtility.UrlEncode(str, Encoding.UTF8);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// URL 인코드, EUC-JP
		/// </summary>
		/// <param name="str">변환할 문자열</param>
		/// <returns>변환 후의 문자열</returns>
		static public string UrlEncodeEUCJP(string str)
		{
			return HttpUtility.UrlEncode(str, Encoding.GetEncoding("euc-jp"));
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// URL 인코드, 시프트 JIS
		/// </summary>
		/// <param name="str">변환할 문자열</param>
		/// <returns>변환 후의 문자열</returns>
		static public string UrlEncodeShiftJis(string str)
		{
			return HttpUtility.UrlEncode(str, Encoding.GetEncoding("UTF-8"));
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 라디안을 도(degree)로 변환한다
		/// </summary>
		/// <param name="radian">라디안</param>
		/// <returns>도(degree)</returns>
		static public float ToDegree(float radian)
		{
			return radian * (float)(360d / (Math.PI * 2));
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 도(degree)를 라디안으로 변환한다
		/// </summary>
		/// <param name="degree">도(degree)</param>
		/// <returns>라디안</returns>
		static public float ToRadian(float degree)
		{
			return degree * (float)((Math.PI * 2) / 360d);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 짧은 일시 문자열을 반환한다.
		/// 초를 포함하지 않는다
		/// </summary>
		/// <param name="dt">변환 대상 일시</param>
		/// <returns>일시 문자열</returns>
		public static string ToShortDateTimeString(DateTime dt)
		{
			return dt.ToString("yyyy/MM/dd") + dt.ToString(" HH:mm");
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 시타라바 스타일의 일시 문자열을 반환한다
		/// </summary>
		/// <param name="dt">변환 대상 일시</param>
		/// <returns>일시 문자열</returns>
		public static string TojbbsDateTimeString(DateTime dt)
		{
			string str = dt.ToString("yyyy/MM/dd", DateTimeFormatInfo.InvariantInfo);
			switch (dt.DayOfWeek)
			{
				case DayOfWeek.Sunday:
					str += "(일)";
					break;
				case DayOfWeek.Monday:
					str += "(월)";
					break;
				case DayOfWeek.Tuesday:
					str += "(화)";
					break;
				case DayOfWeek.Wednesday:
					str += "(수)";
					break;
				case DayOfWeek.Thursday:
					str += "(목)";
					break;
				case DayOfWeek.Friday:
					str += "(금)";
					break;
				case DayOfWeek.Saturday:
					str += "(토)";
					break;
			}
			return str + dt.ToString(" HH:mm:ss");
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 전각 숫자를 반각으로 변환한다.
		/// 숫자 이외의 문자가 포함되어 있어도 된다
		/// </summary>
		/// <param name="str">변환 대상 문자열</param>
		/// <returns>변환 후의 문자열</returns>
		static public string AdjustNumber(string str)
		{
			str = str.Replace('０', '0');
			str = str.Replace('１', '1');
			str = str.Replace('２', '2');
			str = str.Replace('３', '3');
			str = str.Replace('４', '4');
			str = str.Replace('５', '5');
			str = str.Replace('６', '6');
			str = str.Replace('７', '7');
			str = str.Replace('８', '8');
			str = str.Replace('９', '9');
			return str;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// ToShortDateTimeString(), TojbbsDateTimeString()
		/// 로 얻어진 문자열을 DateTime 으로 변환한다.
		/// 변환에 실패한 경우에는 new DateTime() 을 반환한다.
		/// </summary>
		/// <param name="datetime">변환 대상 문자열</param>
		/// <returns>변환 후의 일시</returns>
		public static DateTime ToDateTime(string datetime)
		{
			try
			{
				Match m;
				if ((m = match(@"([0-9]+)/([0-9]+)/([0-9]+).* ([0-9]+):([0-9]+):([0-9]+)", datetime)) != null)
				{
					// 시타라바 BBS
					return new DateTime(Convert.ToInt32(m.Groups[1].Value),
											Convert.ToInt32(m.Groups[2].Value),
											Convert.ToInt32(m.Groups[3].Value),
											Convert.ToInt32(m.Groups[4].Value),
											Convert.ToInt32(m.Groups[5].Value),
											Convert.ToInt32(m.Groups[6].Value), new GregorianCalendar());
				}
				else if ((m = match(@"([0-9]+)/([0-9]+)/([0-9]+) ([0-9]+):([0-9]+)", datetime)) != null)
				{
					// 짧은 버전
					return new DateTime(Convert.ToInt32(m.Groups[1].Value),
											Convert.ToInt32(m.Groups[2].Value),
											Convert.ToInt32(m.Groups[3].Value),
											Convert.ToInt32(m.Groups[4].Value),
											Convert.ToInt32(m.Groups[5].Value), 0, new GregorianCalendar());
				}
				else
				{
					return new DateTime();
				}
			}
			catch
			{
				return new DateTime();
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 정규 표현식으로 조사한다.
		/// 매치했을 때는 Match 를 반환한다.
		/// 매치하지 않는 경우에는 null 을 반환한다.
		/// </summary>
		/// <param name="regex">정규 표현식</param>
		/// <param name="str">대상 문자열</param>
		/// <returns>매치한 것</returns>
		static public Match match(string regex, string str)
		{
			Match m = Regex.Match(str, regex);
			if (!m.Success) return null;
			return m;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 1,000 같은 문자열을 반환한다
		/// </summary>
		/// <param name="num">변환 대상 값</param>
		/// <returns>변환 후의 문자열</returns>
		static public string ToComma3(int num)
		{
			try
			{
				return String.Format("{0:#,0}", num);
			}
			catch
			{
				return "0";
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 1,000 같은 문자열을 반환한다
		/// </summary>
		/// <param name="num">변환 대상 값</param>
		/// <returns>변환 후의 문자열</returns>
		static public string ToComma3(Int64 num)
		{
			try
			{
				return String.Format("{0:#,0}", num);
			}
			catch
			{
				return "0";
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// Assembly 를 로드한다.
		/// MDX1.1 이 설치되어 있는지 여부를 알 수 있다.
		/// 읽기에 실패한 AssemblyName 을 반환한다.
		/// 읽기에 성공한 경우에는 null 을 반환한다.
		/// </summary>
		/// <param name="ass">어셈블리명</param>
		/// <param name="error_ass">읽기에 실패한 어셈블리</param>
		/// <returns>읽기에 성공하면 true</returns>
		static public bool LoadReferencedAssembly(Assembly ass, out AssemblyName error_ass)
		{
			error_ass = null;
			if (ass == null) return false;

			AssemblyName[] names = ass.GetReferencedAssemblies();
			foreach (AssemblyName i in names)
			{
				try
				{
					Assembly ddd = Assembly.Load(i);
				}
				catch
				{
					// 실패
					error_ass = i;
					return false;
				}
			}
			return true;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// OS 명을 얻는다.
		/// Windows XP 같은 문자열이 얻어진다
		/// </summary>
		/// <param name="os">OS</param>
		/// <returns>OS명</returns>
		static public string GetOsName(OperatingSystem os)
		{
			switch (os.Platform)
			{
				case System.PlatformID.Win32Windows:
					if (os.Version.Major >= 4)
					{
						switch (os.Version.Minor)
						{
							case 0: return "Windows 95";
							case 10: return "Windows 98";
							case 90: return "Windows Me";
						}
					}
					break;
				case System.PlatformID.Win32NT:
					switch (os.Version.Major)
					{
						case 3:
							switch (os.Version.Minor)
							{
								case 0: return "Windows NT 3";
								case 1: return "Windows NT 3.1";
								case 5: return "Windows NT 3.5";
								case 51: return "Windows NT 3.51";
							}
							break;
						case 4:
							if (os.Version.Minor == 0)
							{
								return "Windows NT 4.0";
							}
							break;
						case 5:
							switch (os.Version.Minor)
							{
								case 0: return "Windows 2000";
								case 1: return "Windows XP";
								case 2: return "Windows Server 2003";
							}
							break;
						case 6:
							switch (os.Version.Minor)
							{
								case 0: return "Windows Vista or Windows Server 2008";
								case 1: return "Windows 7 or Windows Server 2008 R2";
							}
							break;
					}
					break;
				case System.PlatformID.Win32S: return "Win32s";
				case System.PlatformID.WinCE: return "Windows CE";
				case System.PlatformID.Unix: return "Unix";
				case System.PlatformID.Xbox: return "Xbox 360";
				case System.PlatformID.MacOSX: return "Macintosh";
			}

			// 불명
			return "Unknown OS";
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// CMD 를 실행한다.
		/// STDOUT 의 내용을 반환한다.
		/// </summary>
		/// <param name="Argments">커맨드와 인수</param>
		/// <returns>STDOUT 의 내용</returns>
		public static string ExecCMD(string Argments)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = Environment.GetEnvironmentVariable("ComSpec");
			startInfo.RedirectStandardInput = false;
			startInfo.RedirectStandardOutput = true;
			startInfo.UseShellExecute = false;
			startInfo.CreateNoWindow = true;
			startInfo.Arguments = Argments;
			Process process = Process.Start(startInfo);
			string str = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			return str;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// int 를 Enum 으로 변환한다.
		/// 변환에 실패한 경우 null 을 반환한다.
		/// </summary>
		/// <param name="enumType">Enum 의 타입 typeof(enum)</param>
		/// <param name="value">변환대상</param>
		/// <returns>변환 후의 Enum</returns>
		static public object ToEnum(Type enumType, object value)
		{
			if (value == null) return null;
			if (Enum.IsDefined(enumType, value))
			{
				return Enum.Parse(enumType, value.ToString());
			}
			return null;
		}
		public static object ToEnum(Type enumType, object value, object default_value)
		{
			return Useful.ToEnum(enumType, value) ?? default_value;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// ListView 의 tooltip 을 SubItems 별로 설정한다.
		/// </summary>
		/// <remarks>
		/// <para>ListView 의 tooltip 을 SubItems 별로 설정한다.</para>
		/// <para>MouseMove 안에서 호출할 것.</para>
		/// <para>표시할 tooltip 은 SubItem 의 Tag 를 string 으로 캐스트한다.</para>
		/// <para>ListView.ShowItemToolTips 는 false 여야 한다.</para>
		/// </remarks>
		/// <param name="listview">대상 ListView</param>
		/// <param name="tooltip">대상 ToolTip</param>
		/// <param name="mouse_x">마우스 위치 (listview 의 클라이언트 좌표)</param>
		/// <param name="mouse_y">마우스 위치 (listview 의 클라이언트 좌표)</param>
		static public void UpdateListViewSubItemToolTip(ListView listview, ToolTip tooltip, int mouse_x, int mouse_y)
		{
			if (listview == null) return;
			if (tooltip == null) return;

			ListViewItem item = listview.GetItemAt(mouse_x, mouse_y);
			ListViewHitTestInfo info = listview.HitTest(mouse_x, mouse_y);
			string tooltip_str = "";
			if ((item != null) && (info.SubItem != null))
			{
				tooltip_str = info.SubItem.Tag as string;
				if (String.IsNullOrEmpty(tooltip_str)) tooltip_str = "";
			}
			set_tooltip(tooltip, listview, tooltip_str);
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 툴팁을 설정한다.
		/// 지금 설정된 것과 같으면 아무것도 하지 않는다.
		/// </summary>
		/// <param name="tooltip"></param>
		/// <param name="control"></param>
		/// <param name="str"></param>
		static private void set_tooltip(ToolTip tooltip, Control control, string str)
		{
			if (tooltip.GetToolTip(control) != str)
			{
				// 같은 문자열을 지정하면 깜빡거린다
				tooltip.SetToolTip(control, str);
			}
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 문자열을 Int32 로 변환한다.
		/// 실패한 경우에는 defalt_value 를 반환한다
		/// </summary>
		/// <param name="_from">변환 대상 문자열</param>
		/// <param name="defalte_value">실패 시 반환할 값</param>
		/// <returns>변환 후의 값</returns>
		static public int ToInt32(string _from, int defalte_value)
		{
			int ret;
			if (!Int32.TryParse(_from, out ret)) return defalte_value;
			return ret;
		}

		public static bool ToBool(string _from, bool defalte_value)
		{
			_from = _from.ToLower();
			if (_from == "true" || _from == "1")
				return true;
			if (_from == "false" || _from == "0")
				return false;
			else
				return defalte_value;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 문자열을 Point 로 변환한다.
		/// 실패한 경우에는 defalt_value 를 반환한다
		/// </summary>
		/// <param name="x">변환 대상 문자열 x</param>
		/// <param name="y">변환 대상 문자열 y</param>
		/// <param name="defalte_value">실패 시 반환할 값</param>
		/// <returns>변환 후의 값</returns>
		static public Point ToPoint(string x, string y, Point defalte_value)
		{
			int rx, ry;
			if (!Int32.TryParse(x, out rx)) return defalte_value;
			if (!Int32.TryParse(y, out ry)) return defalte_value;
			return new Point(rx, ry);
		}

		public static void XmlAddAttribute(XmlNode node, string attri_name, string value)
		{
			if (string.IsNullOrEmpty(value))
				return;
			value = value.Trim();
			if (string.IsNullOrEmpty(value))
				return;
			XmlAttribute attribute = node.OwnerDocument.CreateAttribute(attri_name);
			attribute.Value = value;
			node.Attributes.Append(attribute);
		}

		public static XmlNode XmlAddNode(XmlNode p_node, string node_name, string name)
		{
			if (string.IsNullOrEmpty(name))
				return (XmlNode)null;
			name = name.Trim();
			if (string.IsNullOrEmpty(name))
				return (XmlNode)null;
			XmlNode node = Useful.XmlAddNode(p_node, node_name);
			Useful.XmlAddAttribute(node, "name", name);
			return node;
		}

		public static XmlNode XmlAddNode(XmlNode p_node, string node_name)
		{
			XmlNode newChild = (XmlNode)p_node.OwnerDocument.CreateElement(node_name);
			p_node.AppendChild(newChild);
			return newChild;
		}

		public static XmlNode XmlAddPoint(XmlNode p_node, string node_name, Point p)
		{
			XmlNode node = Useful.XmlAddNode(p_node, node_name);
			Useful.XmlAddAttribute(node, "x", p.X.ToString());
			Useful.XmlAddAttribute(node, "y", p.Y.ToString());
			return node;
		}

		public static Point XmlGetPoint(XmlNode p_node, string node_name, Point default_p)
		{
			if (p_node == null)
				return default_p;
			XmlNode xmlNode = (XmlNode)p_node[node_name];
			if (xmlNode == null)
				return default_p;
			XmlAttribute xmlAttribute1 = xmlNode.Attributes["x"];
			XmlAttribute xmlAttribute2 = xmlNode.Attributes["y"];
			if (xmlAttribute1 == null || xmlAttribute2 == null)
				return default_p;
			else
				return Useful.ToPoint(xmlAttribute1.Value, xmlAttribute2.Value, default_p);
		}

		public static string XmlGetAttribute(XmlNode node, string attri_name, string default_value)
		{
			if (node == null)
				throw new ArgumentNullException();
			if (node.Attributes[attri_name] == null)
				return default_value;
			else
				return node.Attributes[attri_name].Value;
		}

		public static void XmlRemoveNodeWhenEmpty(XmlNode p_node, XmlNode node)
		{
			if (node.Attributes.Count > 0 || node.ChildNodes.Count < 0)
				return;
			p_node.RemoveChild(node);
		}

		public static string XmlGetFirstText(XmlNode node)
		{
			if (node == null)
				return (string)null;
			if (!node.HasChildNodes)
				return (string)null;
			foreach (XmlNode xmlNode in node.ChildNodes)
			{
				if (xmlNode is XmlText)
					return xmlNode.Value;
			}
			return (string)null;
		}

		public static string XmlGetElementText(XmlNode parent, string name)
		{
			if (parent == null)
				return "";
			XmlNode node = (XmlNode)parent[name];
			if (node == null)
				return "";
			string firstText = Useful.XmlGetFirstText(node);
			if (!string.IsNullOrEmpty(firstText))
				return firstText;
			else
				return "";
		}

		public static XmlNode[] XmlGetElements(XmlNode parent, string name)
		{
			if (parent == null)
				return (XmlNode[])new XmlElement[0];
			List<XmlNode> list = new List<XmlNode>();
			foreach (XmlNode xmlNode in parent.ChildNodes)
			{
				if (xmlNode.Name == name)
					list.Add(xmlNode);
			}
			return list.ToArray();
		}

		public static XmlNode XmlGetElement(XmlNode parent, string name, string name2)
		{
			if (parent == null)
				return (XmlNode)null;
			foreach (XmlNode node in parent.ChildNodes)
			{
				if (node.Name == name && Useful.XmlGetAttribute(node, "name", "") == name2)
					return node;
			}
			return (XmlNode)null;
		}

		public static XmlDocument XmlCreateXml(string root_name, string version)
		{
			if (string.IsNullOrEmpty(root_name))
				return (XmlDocument)null;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild((XmlNode)xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", (string)null));
			xmlDocument.AppendChild((XmlNode)xmlDocument.CreateElement(root_name));
			if (!string.IsNullOrEmpty(version))
				Useful.XmlAddAttribute((XmlNode)xmlDocument.DocumentElement, "version", version);
			return xmlDocument;
		}

		public static XmlDocument XmlLoadXml(string file_name)
		{
			if (string.IsNullOrEmpty(file_name))
				return (XmlDocument)null;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(file_name);
				if (xmlDocument.DocumentElement == null || xmlDocument.DocumentElement.ChildNodes.Count <= 0)
					return (XmlDocument)null;
				else
					return xmlDocument;
			}
			catch
			{
				return (XmlDocument)null;
			}
		}

		public static Size CalcClientSizeFromControlSize(Form form, Control ctrl, Size new_ctrl_size)
		{
			if (form == null || new_ctrl_size.Width <= 0 || new_ctrl_size.Height <= 0)
				throw new ArgumentException("인수 오류");
			Size size = form.ClientSize - ctrl.Size;
			return new_ctrl_size + size;
		}

		public static void ChangeClientSize_Center(Form form, Size new_client_size)
		{
			Useful.ChangeClientSize_Center(form, new_client_size, false);
		}

		public static void ChangeClientSize_Center(Form form, Size new_client_size, bool is_hide_window_when_update)
		{
			if (form == null || new_client_size.Width <= 0 || new_client_size.Height <= 0)
				throw new ArgumentException("인수 오류");
			if (form.ClientSize == new_client_size)
				return;
			Point positionCenter = Useful.GetPositionCenter(form);
			Size size = form.Size - form.ClientSize + new_client_size;
			Point positionFromCenter = Useful.GetPositionFromCenter(positionCenter, size);
			if (is_hide_window_when_update)
			{
				bool visible = form.Visible;
				form.Visible = false;
				form.SetBounds(positionFromCenter.X, positionFromCenter.Y, size.Width, size.Height);
				form.Visible = visible;
			}
			else
				form.SetBounds(positionFromCenter.X, positionFromCenter.Y, size.Width, size.Height);
		}

		public static Point GetPositionCenter(Form form)
		{
			if (form == null)
				return Point.Empty;
			Point location = form.Location;
			location.X += form.Size.Width / 2;
			location.Y += form.Size.Height / 2;
			if (location.X < 0)
				location.X = 0;
			if (location.Y < 0)
				location.Y = 0;
			return location;
		}

		public static Point GetPositionFromCenter(Form form, Point center_pos)
		{
			if (form == null)
				return Point.Empty;
			else
				return Useful.GetPositionFromCenter(center_pos, form.Size);
		}

		public static Point GetPositionFromCenter(Point center_pos, Size size)
		{
			center_pos.X -= size.Width / 2;
			center_pos.Y -= size.Height / 2;
			if (center_pos.X < 0)
				center_pos.X = 0;
			if (center_pos.Y < 0)
				center_pos.Y = 0;
			return center_pos;
		}

		public static Size GetPrimaryScreenSize()
		{
			Rectangle bounds = Screen.PrimaryScreen.Bounds;
			return new Size(bounds.Width, bounds.Height);
		}


		//-------------------------------------------------------------------------
		/// <summary>
		/// DPI 스케일링 배율을 얻는다
		/// </summary>
		/// <returns>MyDocument 폴더의 경로</returns>
		static public float GetDpiRatio()
		{
			return Graphics.FromHwnd(IntPtr.Zero).DpiX / 96.0f;
		}
	}
}
