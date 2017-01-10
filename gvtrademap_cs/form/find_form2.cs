/*-------------------------------------------------------------------------

 전체검색
 モードレスダイアログとして使용すること

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
		private const int				LIST_MAX = 2000;		// 목록に표시する최대
	
		private gvt_lib					m_lib;					// よく使う기능をまとめたもの
		private GvoDatabase				m_db;					// DB
		private spot					m_spot;					// 장소목록
		private item_window				m_item_window;			// 아이템윈도우
		private Point					m_gpos;

		private ListViewItemSorter		m_sorter;				// ソート
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

			// 각ページの初期化
			init_page1();
			init_page2();
			init_page3();
		}

		/*-------------------------------------------------------------------------
		 검색ページの初期化
		---------------------------------------------------------------------------*/
		private void init_page1()
		{
			// フィルタ
			comboBox2.SelectedIndex		= (int)m_lib.setting.find_filter;
			comboBox3.SelectedIndex		= (int)m_lib.setting.find_filter2;
			comboBox4.SelectedIndex		= (int)m_lib.setting.find_filter3;
	
			// ヘッダのコラムを추가しておく
			// コラムの사이즈は覚えておく必要がある
			listView1.Columns.Add("찾은이름",	180);
			listView1.Columns.Add("종류",	80);
			listView1.Columns.Add("장소",	120);
	
			// 검색문자열を업데이트
			update_find_strings();
			// 검색결과を업데이트
			update_find_result(null);
			// 장소ボタンの상태업데이트
			update_spot_button_status();
		}

		/*-------------------------------------------------------------------------
		 장소표시목록ページの初期化
		---------------------------------------------------------------------------*/
		private void init_page2()
		{
			// ヘッダのコラムを추가しておく
			// コラムの사이즈は覚えておく必要がある
			listView2.Columns.Add("장소",	120);
			listView2.Columns.Add("종류",	150);
			listView2.Columns.Add("",	100);

			// 장소목록の업데이트
			update_spot_list();
		}
		
		/*-------------------------------------------------------------------------
		 문화권ページの初期化
		---------------------------------------------------------------------------*/
		private void init_page3()
		{
			// ヘッダのコラムを추가しておく
			// コラムの사이즈は覚えておく必要がある
			listView3.Columns.Add("이름",	180);
			listView3.Columns.Add("종류",	80);
			listView3.Columns.Add(" ",	120);

			// 목록の업데이트
			update_cultural_sphere_list();

			// ボタンの업데이트
			update_cultural_sphere_button();
		}

		/*-------------------------------------------------------------------------
		 검색ボタンを클릭
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
	
			// 空白チェック
			if(find_string == "")	return;

			m_now_find		= true;

			// 履歴に추가
			m_lib.setting.find_strings.Add(find_string);

			// 좌표검색判定
			if(do_centering_gpos(find_string)){
				// 검색결과をクリア
				listView1.Items.Clear();		// 아이템내용をクリア
												// ヘッダコラムはクリアされない

				// センタリングリクエスト
				m_lib.setting.centering_gpos	= m_gpos;
				m_lib.setting.req_centering_gpos.Request();
			}else{
				this.Cursor	= Cursors.WaitCursor;
				// 검색
				List<GvoDatabase.Find>	results	= m_db.FindAll(find_string);

				// 검색문자열を업데이트
				update_find_strings();
				// 검색결과を업데이트
				update_find_result(results);
				// 장소ボタンの상태업데이트
				update_spot_button_status();
				this.Cursor	= Cursors.Default;
			}

			m_now_find		= false;
		}
	
		/*-------------------------------------------------------------------------
		 검색履歴を업데이트
		---------------------------------------------------------------------------*/
		private void update_find_strings()
		{
			// 검색ボックス
			comboBox1.DropDownHeight		= 200;
			// 履歴をクリア
			comboBox1.Items.Clear();
			if(m_lib.setting.find_strings.Count > 0){
				foreach(string s in m_lib.setting.find_strings){
					comboBox1.Items.Add(s);
				}
				// 一番상を선택
				comboBox1.SelectedIndex		= 0;
			}
		}

		/*-------------------------------------------------------------------------
		 검색결과を업데이트
		---------------------------------------------------------------------------*/
		private void update_find_result(List<GvoDatabase.Find> results)
		{
			listView1.BeginUpdate();
			listView1.Items.Clear();		// 아이템내용をクリア
											// ヘッダコラムはクリアされない

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
				MessageBox.Show(this, LIST_MAX.ToString() + "건이상は스킵されました。", "검색결과が多すぎます");
			}
		}

		/*-------------------------------------------------------------------------
		 추가する
		---------------------------------------------------------------------------*/
		private void add_item(ListView listview, GvoDatabase.Find i)
		{
			// フィルタ
			if(i.Type != GvoDatabase.Find.FindType.CulturalSphere){
				// 문화권以외のとき
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
		 좌표の入力をチェックする
		 좌표が入力されていればセンタリングする
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

				// 중心にする좌표
				m_gpos			= gpos;
				return true;
			}catch{
				return false;
			}
		}

		/*-------------------------------------------------------------------------
		 선택されている아이템のTAGを得る
		---------------------------------------------------------------------------*/
		private GvoDatabase.Find get_selected_item_tag()
		{
			return get_selected_item_tag(listView1);
		}

		/*-------------------------------------------------------------------------
		 선택されている아이템のTAGを得る
		---------------------------------------------------------------------------*/
		private GvoDatabase.Find get_selected_item_tag(ListView view)
		{
			if(view.SelectedItems.Count <= 0)		return null;
			ListViewItem	item	= view.SelectedItems[0];
			if(item.Tag == null)					return null;
			return (GvoDatabase.Find)item.Tag;
		}
	
		/*-------------------------------------------------------------------------
		 閉じられようとしている
		---------------------------------------------------------------------------*/
		private void find_form2_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(e.CloseReason == CloseReason.UserClosing){
				// このフォームの x ボタンが押されたときのみ취소する
				// Alt + F4 でもここを通る
				e.Cancel	= true;
				this.Hide();	// 비표시にする
			}
		}

		/*-------------------------------------------------------------------------
		 ダブル클릭された
		---------------------------------------------------------------------------*/
		private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			button3.PerformClick();
		}

		/*-------------------------------------------------------------------------
		 장소ボタンの상태업데이트
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
		 선택상태が업데이트された
		---------------------------------------------------------------------------*/
		private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			update_spot_button_status();
		}
	
		/*-------------------------------------------------------------------------
		 アクティブにされた
		 フォーカスを검색ボックスに移す
		---------------------------------------------------------------------------*/
		private void find_form2_Activated(object sender, EventArgs e)
		{
			ActiveControl	= comboBox1;
		}
	
		/*-------------------------------------------------------------------------
		 선택한아이템を장소표시ボタンが押された
		---------------------------------------------------------------------------*/
		private void button3_Click(object sender, EventArgs e)
		{
			do_spot(get_selected_item_tag());
		}

		/*-------------------------------------------------------------------------
		 장소표시する
		---------------------------------------------------------------------------*/
		private void do_spot(GvoDatabase.Find item)
		{
			if(item == null)		return;

			// 장소
			m_lib.setting.req_spot_item.Request(item);
		}
	
		/*-------------------------------------------------------------------------
		 フィルタ변경
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
		 목록내で우클릭された
		---------------------------------------------------------------------------*/
		private void listView1_MouseClick(object sender, MouseEventArgs e)
		{
			if((e.Button & MouseButtons.Right) == 0)	return;		// 우클릭のみ

			GvoDatabase.Find	tag		= get_selected_item_tag();
			if(tag == null)								return;

			// 표시위치조정
			Point		pos	= new Point(e.X, e.Y);

			ItemDatabaseCustom.Data			db		= tag.Database;
			if(db == null){
				// 아이템DBと一致しない
				open_recipe_wiki0_ToolStripMenuItem.Enabled		= false;
				open_recipe_wiki1_ToolStripMenuItem.Enabled		= false;
				copy_all_to_clipboardToolStripMenuItem.Enabled	= false;
			}else{
				copy_all_to_clipboardToolStripMenuItem.Enabled	= true;
				if(db.IsSkill || db.IsReport){
					// 스킬か보고
					open_recipe_wiki0_ToolStripMenuItem.Enabled	= false;
					open_recipe_wiki1_ToolStripMenuItem.Enabled	= false;
				}else{
					if(db.IsRecipe){
						// 레시피
						open_recipe_wiki0_ToolStripMenuItem.Enabled	= true;
						open_recipe_wiki1_ToolStripMenuItem.Enabled	= false;
					}else{
						// 레시피以외
						open_recipe_wiki0_ToolStripMenuItem.Enabled	= false;
						open_recipe_wiki1_ToolStripMenuItem.Enabled	= true;
					}
				}
			}
			contextMenuStrip1.Show(listView1, pos);
		}

		/*-------------------------------------------------------------------------
		 레시피정보wikiを開く
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
		 레시피정보wikiを開く
		 작성가능かどうか검색
		---------------------------------------------------------------------------*/
		private void open_recipe_wiki1_ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GvoDatabase.Find	tag		= get_selected_item_tag();
			if(tag == null)										return;
			if(tag.Database == null)							return;
			tag.Database.OpenRecipeWiki1();
		}

		/*-------------------------------------------------------------------------
		 명칭をクリップボードにコピーする
		---------------------------------------------------------------------------*/
		private void copy_name_to_clipboardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GvoDatabase.Find	tag		= get_selected_item_tag();
			if(tag == null)										return;
			Clipboard.SetText(tag.NameString);
		}

		/*-------------------------------------------------------------------------
		 디테일をクリップボードにコピーする
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
		 ヘッダを클릭された
		---------------------------------------------------------------------------*/
		private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// ソートする
			m_sorter.Sort(listView1, e.Column);
		}

		/*-------------------------------------------------------------------------
		 검색モードにする
		---------------------------------------------------------------------------*/
		public void SetFindMode()
		{
			// 검색をアクティブにする
			tabControl1.SelectedIndex	= 0;
		}

		/*-------------------------------------------------------------------------
		 장소결과の업데이트
		 장소목록をアクティブにする
		---------------------------------------------------------------------------*/
		public void UpdateSpotList()
		{
			if(m_spot.is_spot){
				// 장소목록をアクティブにする
				tabControl1.SelectedIndex	= 2;
			}else{
				// 검색をアクティブにする
				SetFindMode();
			}
	
			// 목록の업데이트
			update_spot_list();
		}

		/*-------------------------------------------------------------------------
		 장소결과の업데이트
		---------------------------------------------------------------------------*/
		private void update_spot_list()
		{
			listView2.BeginUpdate();
			listView2.Items.Clear();

			List<spot.spot_once>	list	= m_spot.list;

			label1.Text					= String.Format("{0}건", list.Count);
			label3.Text					= m_spot.spot_type_str;
			listView2.Columns[2].Text	= m_spot.spot_type_column_str;
			
			// 목록の추가
			foreach(spot.spot_once s in list){
				ListViewItem	item	= new ListViewItem(s.info.Name, 0);
				item.Tag				= s;
				item.SubItems.Add(s.name);
				item.SubItems.Add(s.ex);

				listView2.Items.Add(item);
			}
			listView2.EndUpdate();

			// ボタンの유효무효
			button2.Enabled			= (list.Count <= 0)? false: true;

			// 선택を설정する
			update_select_info();
		}

		/*-------------------------------------------------------------------------
		 선택されているinfoを선택する
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

					// 선택상태にし、見える위치にスクロールさせる
					i.Selected	= true;
					i.EnsureVisible();
					i.Focused	= true;
					return;
				}
			}

			// 선택중のinfoが含まれない場合は1番目を선택상태にする	
			// 문화권の장소時
			if(listView2.Items.Count > 0){
				listView2.Items[0].Selected		= true;
			}
		}

		/*-------------------------------------------------------------------------
		 장소결과の선택が업데이트された
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
		 장소결과のコラムが클릭された
		 ソートする
		---------------------------------------------------------------------------*/
		private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// ソートする
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
		 ESCが押されたときの닫기動作
		---------------------------------------------------------------------------*/
		private void button4_Click(object sender, EventArgs e)
		{
			this.Hide();		// 표시を消す
		}


		/*-------------------------------------------------------------------------
		 문화권を장소표시するボタンを업데이트する
		---------------------------------------------------------------------------*/
		private void update_cultural_sphere_button()
		{
			button5.Enabled	= (listView3.SelectedIndices.Count <= 0)? false: true;
		}

		/*-------------------------------------------------------------------------
		 문화권목록の업데이트
		 途중で업데이트されないため、初期化時のみ
		---------------------------------------------------------------------------*/
		private void update_cultural_sphere_list()
		{
			List<GvoDatabase.Find>	results	= m_db.GetCulturalSphereList();

			listView3.BeginUpdate();
			listView3.Items.Clear();

			// 목록の추가
			foreach(GvoDatabase.Find i in results){
				add_item(listView3, i);
			}
			listView3.EndUpdate();
		}

		/*-------------------------------------------------------------------------
		 문화권を장소표시する
		---------------------------------------------------------------------------*/
		private void button5_Click(object sender, EventArgs e)
		{
			do_spot(get_selected_item_tag(listView3));
		}

		/*-------------------------------------------------------------------------
		 문화권の선택업데이트
		---------------------------------------------------------------------------*/
		private void listView3_SelectedIndexChanged(object sender, EventArgs e)
		{
			// ボタンの업데이트
			update_cultural_sphere_button();
		}

		/*-------------------------------------------------------------------------
		 カラム클릭
		 ソートする
		---------------------------------------------------------------------------*/
		private void listView3_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// ソートする
			m_sorter.Sort(listView3, e.Column);
		}

		/*-------------------------------------------------------------------------
		 문화권목록がダブル클릭された
		---------------------------------------------------------------------------*/
		private void listView3_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			button5.PerformClick();
		}
	}
}
