/*-------------------------------------------------------------------------

 전체검색
 모드리스 대화상자로 사용할 것

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using Utility;
using Utility.Ctrl;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public partial class find_form2 : Form
	{
		private const int				LIST_MAX = 2000;		// 목록에 표시할 최대 개수
	
		private gvt_lib					m_lib;					// 자주 쓰는 기능을 모아 놓은 것
		private GvoDatabase				m_db;					// DB
		private spot					m_spot;					// 장소목록
		private item_window				m_item_window;			// 아이템윈도우
		private Point					m_gpos;

		private ListViewItemSorter		m_sorter;				// 정렬
		private bool					m_now_find;
	
		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public find_form2(gvt_lib lib, GvoDatabase db, spot _spot, item_window _item_window)
		{
			m_lib				= lib;
			m_db				= db;
			m_spot				= _spot;
			m_item_window		= _item_window;
			m_gpos				= new Point(-1, -1);

			m_sorter			= new ListViewItemSorter();

			InitializeComponent();

			Useful.SetFontMeiryo(this, def.MEIRYO_POINT);

			m_now_find			= false;

			// tooltip
			toolTip1.AutoPopDelay		= 30*1000;		// 30초표시
			toolTip1.BackColor			= Color.LightYellow;

			// 각 페이지 초기화
			init_page1();
			init_page2();
			init_page3();
		}

		/*-------------------------------------------------------------------------
		 검색 페이지 초기화
		---------------------------------------------------------------------------*/
		private void init_page1()
		{
			// 필터
			comboBox2.SelectedIndex		= (int)m_lib.setting.find_filter;
			comboBox3.SelectedIndex		= (int)m_lib.setting.find_filter2;
			comboBox4.SelectedIndex		= (int)m_lib.setting.find_filter3;
	
			// 헤더 컬럼을 추가해 둔다
			// 컬럼 크기는 기억해 둘 필요가 있다
			listView1.Columns.Add("찾은이름",	180);
			listView1.Columns.Add("종류",	80);
			listView1.Columns.Add("장소",	120);
	
			// 검색 문자열을 갱신
			update_find_strings();
			// 검색 결과를 갱신
			update_find_result(null);
			// 장소 버튼의 상태 갱신
			update_spot_button_status();
		}

		/*-------------------------------------------------------------------------
		 장소 표시 목록 페이지 초기화
		---------------------------------------------------------------------------*/
		private void init_page2()
		{
			// 헤더 컬럼을 추가해 둔다
			// 컬럼 크기는 기억해 둘 필요가 있다
			listView2.Columns.Add("장소",	120);
			listView2.Columns.Add("종류",	150);
			listView2.Columns.Add("",	100);

			// 장소 목록 갱신
			update_spot_list();
		}
		
		/*-------------------------------------------------------------------------
		 문화권 페이지 초기화
		---------------------------------------------------------------------------*/
		private void init_page3()
		{
			// 헤더 컬럼을 추가해 둔다
			// 컬럼 크기는 기억해 둘 필요가 있다
			listView3.Columns.Add("이름",	180);
			listView3.Columns.Add("종류",	80);
			listView3.Columns.Add(" ",	120);

			// 목록 갱신
			update_cultural_sphere_list();

			// 버튼 갱신
			update_cultural_sphere_button();
		}

		/*-------------------------------------------------------------------------
		 검색 버튼을 클릭
		---------------------------------------------------------------------------*/
		private void button1_Click(object sender, EventArgs e)
		{
			do_find();
		}

		/*-------------------------------------------------------------------------
		 검색
		---------------------------------------------------------------------------*/
		private void do_find()
		{
			string		find_string	= comboBox1.Text;
	
			// 공백 확인
			if(find_string == "")	return;

			m_now_find		= true;

			// 이력에 추가
			m_lib.setting.find_strings.Add(find_string);

			// 좌표 검색 판정
			if(do_centering_gpos(find_string)){
				// 검색 결과를 초기화
				listView1.Items.Clear();		// 아이템 내용을 초기화
												// 헤더 컬럼은 초기화되지 않는다

				// 센터링 리퀘스트
				m_lib.setting.centering_gpos	= m_gpos;
				m_lib.setting.req_centering_gpos.Request();
			}else{
				this.Cursor	= Cursors.WaitCursor;
				// 검색
				List<GvoDatabase.Find>	results	= m_db.FindAll(find_string);

				// 검색 문자열을 갱신
				update_find_strings();
				// 검색 결과를 갱신
				update_find_result(results);
				// 장소 버튼의 상태 갱신
				update_spot_button_status();
				this.Cursor	= Cursors.Default;
			}

			m_now_find		= false;
		}
	
		/*-------------------------------------------------------------------------
		 검색 이력을 갱신
		---------------------------------------------------------------------------*/
		private void update_find_strings()
		{
			// 검색 박스
			comboBox1.DropDownHeight		= 200;
			// 이력을 초기화
			comboBox1.Items.Clear();
			if(m_lib.setting.find_strings.Count > 0){
				foreach(string s in m_lib.setting.find_strings){
					comboBox1.Items.Add(s);
				}
				// 맨 위를 선택
				comboBox1.SelectedIndex		= 0;
			}
		}

		/*-------------------------------------------------------------------------
		 검색 결과를 갱신
		---------------------------------------------------------------------------*/
		private void update_find_result(List<GvoDatabase.Find> results)
		{
			listView1.BeginUpdate();
			listView1.Items.Clear();		// 아이템 내용을 초기화
											// 헤더 컬럼은 초기화되지 않는다

			bool	is_overflow	= false;
			if(results != null){
				foreach(GvoDatabase.Find i in results){
					add_item(listView1, i);
					if(listView1.Items.Count >= LIST_MAX){
						is_overflow		= true;
						break;
					}
				}
			}
			listView1.EndUpdate();

			if(results != null){
//				label2.Text				= String.Format("{0}건", listView1.Items.Count);
				label2.Text				= String.Format("{0}건", results.Count);
			}else{
				label2.Text				= "0건";
			}

			if(is_overflow){
				MessageBox.Show(this, LIST_MAX.ToString() + "건 이상은 생략되었습니다.", "검색 결과가 너무 많습니다");
			}
		}

		/*-------------------------------------------------------------------------
		 추가한다
		---------------------------------------------------------------------------*/
		private void add_item(ListView listview, GvoDatabase.Find i)
		{
			// 필터
			if(i.Type != GvoDatabase.Find.FindType.CulturalSphere){
				// 문화권 이외일 때
				switch(m_lib.setting.find_filter){
				case _find_filter.world_info:
					if(i.Type == GvoDatabase.Find.FindType.Database)	return;
					break;
				case _find_filter.item_database:
					if(i.Type != GvoDatabase.Find.FindType.Database)	return;
					break;
				case _find_filter.both:
				default:
					break;
				}
			}
			
			ListViewItem	item	= new ListViewItem(i.NameString, 0);
			item.Tag				= i;
			item.ToolTipText		= i.TooltipString;
			item.SubItems.Add(i.TypeString);
			item.SubItems.Add(i.SpotString);

			listview.Items.Add(item);
		}

		/*-------------------------------------------------------------------------
		 좌표 입력을 확인한다
		 좌표가 입력되어 있으면 센터링한다
		---------------------------------------------------------------------------*/
		private bool do_centering_gpos(string str)
		{
			try{
				str	= str.Replace('０', '0');
				str	= str.Replace('１', '1');
				str	= str.Replace('２', '2');
				str	= str.Replace('３', '3');
				str	= str.Replace('４', '4');
				str	= str.Replace('５', '5');
				str	= str.Replace('６', '6');
				str	= str.Replace('７', '7');
				str	= str.Replace('８', '8');
				str	= str.Replace('９', '9');
				str	= str.Replace('、', ',');
				str	= str.Replace('，', ',');
				str	= str.Replace('.', ',');
				str	= str.Replace('。', ',');
				str	= str.Replace('．', ',');

				string[] split	= str.Split(new char[]{','});
				if(split.Length != 2)	return false;

				Point	gpos	= new Point(Convert.ToInt32(split[0]),
											Convert.ToInt32(split[1]));
				if(gpos.X < 0)			return false;
				if(gpos.Y < 0)			return false;

				// 중심으로 삼을 좌표
				m_gpos			= gpos;
				return true;
			}catch{
				return false;
			}
		}

		/*-------------------------------------------------------------------------
		 선택되어 있는 아이템의 TAG 를 얻는다
		---------------------------------------------------------------------------*/
		private GvoDatabase.Find get_selected_item_tag()
		{
			return get_selected_item_tag(listView1);
		}

		/*-------------------------------------------------------------------------
		 선택되어 있는 아이템의 TAG 를 얻는다
		---------------------------------------------------------------------------*/
		private GvoDatabase.Find get_selected_item_tag(ListView view)
		{
			if(view.SelectedItems.Count <= 0)		return null;
			ListViewItem	item	= view.SelectedItems[0];
			if(item.Tag == null)					return null;
			return (GvoDatabase.Find)item.Tag;
		}
	
		/*-------------------------------------------------------------------------
		 닫히려고 하고 있다
		---------------------------------------------------------------------------*/
		private void find_form2_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(e.CloseReason == CloseReason.UserClosing){
				// 이 폼의 x 버튼이 눌렸을 때만 취소한다
				// Alt + F4 로도 여기를 지난다
				e.Cancel	= true;
				this.Hide();	// 비표시로 만든다
			}
		}

		/*-------------------------------------------------------------------------
		 더블 클릭되었다
		---------------------------------------------------------------------------*/
		private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			button3.PerformClick();
		}

		/*-------------------------------------------------------------------------
		 장소 버튼의 상태 갱신
		---------------------------------------------------------------------------*/
		private void update_spot_button_status()
		{
			GvoDatabase.Find	tag		= get_selected_item_tag();
			if(tag == null){
				button3.Enabled		= false;
			}else{
				if(tag.Type == GvoDatabase.Find.FindType.Database){
					button3.Enabled		= false;
				}else{
					button3.Enabled		= true;
				}
			}
		}
	
		/*-------------------------------------------------------------------------
		 선택 상태가 갱신되었다
		---------------------------------------------------------------------------*/
		private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			update_spot_button_status();
		}
	
		/*-------------------------------------------------------------------------
		 활성화되었다
		 포커스를 검색 박스로 옮긴다
		---------------------------------------------------------------------------*/
		private void find_form2_Activated(object sender, EventArgs e)
		{
			ActiveControl	= comboBox1;
		}
	
		/*-------------------------------------------------------------------------
		 선택한 아이템을 장소 표시 버튼이 눌렸다
		---------------------------------------------------------------------------*/
		private void button3_Click(object sender, EventArgs e)
		{
			do_spot(get_selected_item_tag());
		}

		/*-------------------------------------------------------------------------
		 장소 표시한다
		---------------------------------------------------------------------------*/
		private void do_spot(GvoDatabase.Find item)
		{
			if(item == null)		return;

			// 장소
			m_lib.setting.req_spot_item.Request(item);
		}
	
		/*-------------------------------------------------------------------------
		 필터 변경
		---------------------------------------------------------------------------*/
		private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_lib.setting.find_filter	= _find_filter.world_info + comboBox2.SelectedIndex;
			do_find();
		}
		private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_lib.setting.find_filter2	= _find_filter2.name + comboBox3.SelectedIndex;
			do_find();
		}
		private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_lib.setting.find_filter3	= _find_filter3.full_text_search + comboBox4.SelectedIndex;
			do_find();
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(m_now_find)	return;
			do_find();
		}

		/*-------------------------------------------------------------------------
		 목록 안에서 우클릭되었다
		---------------------------------------------------------------------------*/
		private void listView1_MouseClick(object sender, MouseEventArgs e)
		{
			if((e.Button & MouseButtons.Right) == 0)	return;		// 우클릭만

			GvoDatabase.Find	tag		= get_selected_item_tag();
			if(tag == null)								return;

			// 표시위치조정
			Point		pos	= new Point(e.X, e.Y);

			ItemDatabaseCustom.Data			db		= tag.Database;
			if(db == null){
				// 아이템 DB 와 일치하지 않는다
				open_recipe_wiki0_ToolStripMenuItem.Enabled		= false;
				open_recipe_wiki1_ToolStripMenuItem.Enabled		= false;
				copy_all_to_clipboardToolStripMenuItem.Enabled	= false;
			}else{
				copy_all_to_clipboardToolStripMenuItem.Enabled	= true;
				if(db.IsSkill || db.IsReport){
					// 스킬 아니면 보고
					open_recipe_wiki0_ToolStripMenuItem.Enabled	= false;
					open_recipe_wiki1_ToolStripMenuItem.Enabled	= false;
				}else{
					if(db.IsRecipe){
						// 레시피
						open_recipe_wiki0_ToolStripMenuItem.Enabled	= true;
						open_recipe_wiki1_ToolStripMenuItem.Enabled	= false;
					}else{
						// 레시피 이외
						open_recipe_wiki0_ToolStripMenuItem.Enabled	= false;
						open_recipe_wiki1_ToolStripMenuItem.Enabled	= true;
					}
				}
			}
			contextMenuStrip1.Show(listView1, pos);
		}

		/*-------------------------------------------------------------------------
		 레시피 정보 wiki 를 연다
		 레시피검색
		---------------------------------------------------------------------------*/
		private void open_recipe_wiki0_ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GvoDatabase.Find	tag		= get_selected_item_tag();
			if(tag == null)										return;
			if(tag.Database == null)							return;
			tag.Database.OpenRecipeWiki0();
		}

		/*-------------------------------------------------------------------------
		 레시피 정보 wiki 를 연다
		 작성 가능한지 검색
		---------------------------------------------------------------------------*/
		private void open_recipe_wiki1_ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GvoDatabase.Find	tag		= get_selected_item_tag();
			if(tag == null)										return;
			if(tag.Database == null)							return;
			tag.Database.OpenRecipeWiki1();
		}

		/*-------------------------------------------------------------------------
		 명칭을 클립보드에 복사한다
		---------------------------------------------------------------------------*/
		private void copy_name_to_clipboardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GvoDatabase.Find	tag		= get_selected_item_tag();
			if(tag == null)										return;
			Clipboard.SetText(tag.NameString);
		}

		/*-------------------------------------------------------------------------
		 상세를 클립보드에 복사한다
		---------------------------------------------------------------------------*/
		private void copy_all_to_clipboardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GvoDatabase.Find	tag		= get_selected_item_tag();
			if(tag == null)										return;
			if(tag.Data != null){
				Clipboard.SetText(tag.Data.TooltipString);
			}else if(tag.Database != null){
				Clipboard.SetText(tag.Database.GetToolTipString());
			}
		}

		/*-------------------------------------------------------------------------
		 헤더가 클릭되었다
		---------------------------------------------------------------------------*/
		private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// 정렬한다
			m_sorter.Sort(listView1, e.Column);
		}

		/*-------------------------------------------------------------------------
		 검색 모드로 만든다
		---------------------------------------------------------------------------*/
		public void SetFindMode()
		{
			// 검색을 활성화한다
			tabControl1.SelectedIndex	= 0;
		}

		/*-------------------------------------------------------------------------
		 장소 결과 갱신
		 장소 목록을 활성화한다
		---------------------------------------------------------------------------*/
		public void UpdateSpotList()
		{
			if(m_spot.is_spot){
				// 장소 목록을 활성화한다
				tabControl1.SelectedIndex	= 2;
			}else{
				// 검색을 활성화한다
				SetFindMode();
			}
	
			// 목록 갱신
			update_spot_list();
		}

		/*-------------------------------------------------------------------------
		 장소 결과 갱신
		---------------------------------------------------------------------------*/
		private void update_spot_list()
		{
			listView2.BeginUpdate();
			listView2.Items.Clear();

			List<spot.spot_once>	list	= m_spot.list;

			label1.Text					= String.Format("{0}건", list.Count);
			label3.Text					= m_spot.spot_type_str;
			listView2.Columns[2].Text	= m_spot.spot_type_column_str;
			
			// 목록 추가
			foreach(spot.spot_once s in list){
				ListViewItem	item	= new ListViewItem(s.info.Name, 0);
				item.Tag				= s;
				item.SubItems.Add(s.name);
				item.SubItems.Add(s.ex);

				listView2.Items.Add(item);
			}
			listView2.EndUpdate();

			// 버튼의 유효/무효
			button2.Enabled			= (list.Count <= 0)? false: true;

			// 선택을 설정한다
			update_select_info();
		}

		/*-------------------------------------------------------------------------
		 선택되어 있는 info 를 선택한다
		---------------------------------------------------------------------------*/
		private void update_select_info()
		{
			GvoWorldInfo.Info info	= m_item_window.info;

			if(info != null){
				foreach(ListViewItem i in listView2.Items){
					object tag			= i.Tag;
					if(tag == null)		continue;

					spot.spot_once	s	= (spot.spot_once)tag;
					if(s.info.Name != info.Name)	continue;

					// 선택 상태로 만들고, 보이는 위치로 스크롤시킨다
					i.Selected	= true;
					i.EnsureVisible();
					i.Focused	= true;
					return;
				}
			}

			// 선택 중인 info 가 포함되지 않은 경우에는 첫 번째를 선택 상태로 만든다
			// 문화권을 장소 지정할 때
			if(listView2.Items.Count > 0){
				listView2.Items[0].Selected		= true;
			}
		}

		/*-------------------------------------------------------------------------
		 장소 결과의 선택이 갱신되었다
		---------------------------------------------------------------------------*/
		private void listView2_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(listView2.SelectedIndices.Count <= 0)	return;
			if(listView2.SelectedItems[0].Tag == null)	return;
			spot.spot_once	s	= listView2.SelectedItems[0].Tag as spot.spot_once;
			if(s == null)								return;

			m_lib.setting.req_spot_item_changed.Request(s);
		}

		/*-------------------------------------------------------------------------
		 장소 결과의 컬럼이 클릭되었다
		 정렬한다
		---------------------------------------------------------------------------*/
		private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// 정렬한다
			m_sorter.Sort(listView2, e.Column);
		}

		/*-------------------------------------------------------------------------
		 장소표시해제
		---------------------------------------------------------------------------*/
		private void button2_Click(object sender, EventArgs e)
		{
			m_lib.setting.req_spot_item.Request(null);
		}

		/*-------------------------------------------------------------------------
		 ESC 가 눌렸을 때의 닫기 동작
		---------------------------------------------------------------------------*/
		private void button4_Click(object sender, EventArgs e)
		{
			this.Hide();		// 표시를 지운다
		}


		/*-------------------------------------------------------------------------
		 문화권을 장소 표시하는 버튼을 갱신한다
		---------------------------------------------------------------------------*/
		private void update_cultural_sphere_button()
		{
			button5.Enabled	= (listView3.SelectedIndices.Count <= 0)? false: true;
		}

		/*-------------------------------------------------------------------------
		 문화권 목록 갱신
		 도중에 갱신되지 않으므로, 초기화 시에만
		---------------------------------------------------------------------------*/
		private void update_cultural_sphere_list()
		{
			List<GvoDatabase.Find>	results	= m_db.GetCulturalSphereList();

			listView3.BeginUpdate();
			listView3.Items.Clear();

			// 목록 추가
			foreach(GvoDatabase.Find i in results){
				add_item(listView3, i);
			}
			listView3.EndUpdate();
		}

		/*-------------------------------------------------------------------------
		 문화권을 장소 표시한다
		---------------------------------------------------------------------------*/
		private void button5_Click(object sender, EventArgs e)
		{
			do_spot(get_selected_item_tag(listView3));
		}

		/*-------------------------------------------------------------------------
		 문화권 선택 갱신
		---------------------------------------------------------------------------*/
		private void listView3_SelectedIndexChanged(object sender, EventArgs e)
		{
			// 버튼 갱신
			update_cultural_sphere_button();
		}

		/*-------------------------------------------------------------------------
		 컬럼 클릭
		 정렬한다
		---------------------------------------------------------------------------*/
		private void listView3_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// 정렬한다
			m_sorter.Sort(listView3, e.Column);
		}

		/*-------------------------------------------------------------------------
		 문화권 목록이 더블 클릭되었다
		---------------------------------------------------------------------------*/
		private void listView3_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			button5.PerformClick();
		}
	}
}
