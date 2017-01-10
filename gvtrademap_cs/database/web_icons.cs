/*-------------------------------------------------------------------------

 @web icon

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 define
---------------------------------------------------------------------------*/
//#define	DRAW_POPUPS_BOUNDINGBOX

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using directx;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs {
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class WebIcons {
		// 바운딩 박스사이즈
		private const int BB_ONCE_SIZE = 400;
		private const int BB_OUTSIDESCREEEN_OFFSET = 16;

		// 破棄する距離の2乗
		private const float REMOVE_LENGTH_SQ = 34 * 34;

		private enum icon_index {
			// 풍향
			wind_0,
			wind_1,
			wind_2,
			wind_3,
			wind_4,
			wind_5,
			wind_6,
			wind_7,

			// 재해
			memo_0,
			memo_1,
			memo_2,
			memo_3,
			memo_4,
		};

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public class Data {
			private Point m_pos;					// 위치
			private int m_icon_index;		   // 아이콘번호
			private string m_memo;				  // 메모(미사용??)

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public Point Position { get { return m_pos; } }
			public int IconIndex { get { return m_icon_index; } }
			public string Memo { get { return m_memo; } }

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public Data(Point pos, int icon_index, string memo) {
				m_pos = pos;
				m_icon_index = icon_index;
				m_memo = memo;
			}
		}

		/*-------------------------------------------------------------------------
		 바운딩 박스付き
		---------------------------------------------------------------------------*/
		public class DataListBB : D3dBB2d {
			private List<Data> m_list;

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public List<Data> List { get { return m_list; } }

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public DataListBB() {
				m_list = new List<Data>();
				// オフセットを지정しておく
				base.OffsetLT = new Vector2(-BB_OUTSIDESCREEEN_OFFSET, -BB_OUTSIDESCREEEN_OFFSET);
				base.OffsetRB = new Vector2(BB_OUTSIDESCREEEN_OFFSET, BB_OUTSIDESCREEEN_OFFSET);
			}

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public bool Add(Data p) {
				Vector2 size = base.IfUpdate(transform.ToVector2(p.Position)).Size;
				if (size.X > BB_ONCE_SIZE) return false;
				if (size.Y > BB_ONCE_SIZE) return false;

				// 추가
				m_list.Add(p);
				base.Update(transform.ToVector2(p.Position));
				return true;		// 추가した
			}
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private gvt_lib m_lib;

		private List<Data> m_list;
		private List<DataListBB> m_draw_list;

		private bool m_optimize;		// 그리기 최적화할 경우 true
										// 같은아이콘에 거리가 가까운 경우 하나로 모아서 그릴 경우도 true
		private DrawSettingWebIcons m_draw_flags;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public WebIcons(gvt_lib lib) {
			m_lib = lib;

			m_list = new List<Data>();
			m_draw_list = new List<DataListBB>();
			m_draw_flags = 0;
		}

		/*-------------------------------------------------------------------------
		 데이터を로드
		---------------------------------------------------------------------------*/
		public bool Load(string fname) {
			if (!File.Exists(fname)) return false;	  // 파일을 찾을 수 없습니다

			string line = "";
			try {
				using (StreamReader sr = new StreamReader(
					fname, Encoding.GetEncoding("UTF-8"))) {

					while ((line = sr.ReadLine()) != null) {
						if (line == "") continue;

						string[] split = line.Split(new char[] { ',' });
						if (split.Length < 3) continue;

						try {
							// 4792 크기의 맵 기준에 맞춰져 있기 때문에, / 4792 * 16384 (측량 최대) / 4 (배율 조정한 현재 지도의 크기) 으로 값을 조정해야 한다.
							int x = Convert.ToInt32(split[0]);
							int y = Convert.ToInt32(split[1]);
							x = (int)(x / 4792.0 * 16384.0 / 4.0);
							y = (int)(y / 4792.0 * 16384.0 / 4.0);
							int index = Convert.ToInt32(split[2]);
							string memo = "";
							if (split.Length < 4) memo = split[3];
							m_list.Add(new Data(new Point(x, y), index, memo));
						} catch {
						}
					}
				}
			} catch {
				// 읽기실패
				return false;
			}

			// 그리기용に仕분ける
			create_draw_list();
			return true;
		}

		/*-------------------------------------------------------------------------
		 업데이트
		 최적화현황が変わった場合は그리기목록を作りなおす
		---------------------------------------------------------------------------*/
		public void Update() {
			// 설정변경のチェック
			if ((m_optimize == m_lib.setting.remove_near_web_icons)
				&& (m_draw_flags == m_lib.setting.draw_setting_web_icons)) {
				// 설정변경없음
				return;
			}

			// 作り直す
			create_draw_list();
		}

		/*-------------------------------------------------------------------------
		 그리기용に仕분ける
		---------------------------------------------------------------------------*/
		private void create_draw_list() {
			m_draw_list.Clear();

			// 그리기플래그
			m_draw_flags = m_lib.setting.draw_setting_web_icons;

			// 목록を作る
			List<Data> free_list = create_list();

			// 적당히 거리가 가까우면 그룹을 만듬
			while (free_list.Count > 0) {
				m_draw_list.Add(create_bb(ref free_list));
			}
		}

		/*-------------------------------------------------------------------------
		 목록を作る
		 설정によっては同じ아이콘で距離が近い場合추가されない
		---------------------------------------------------------------------------*/
		private List<Data> create_list() {
			List<Data> free_list = new List<Data>();

			m_optimize = m_lib.setting.remove_near_web_icons;
			if (!m_optimize) {
				// 전부を無条건に含む
				foreach (Data i in m_list) free_list.Add(i);
			} else {
				foreach (Data i in m_list) {
					if (is_add_list(i, free_list)) {
						free_list.Add(i);
					}
				}
			}
			return free_list;
		}

		/*-------------------------------------------------------------------------
		 추가する必要があるか조사
		 同じ종류の아이콘で距離が近い場合falseを返す
		---------------------------------------------------------------------------*/
		private bool is_add_list(Data wi, List<Data> list) {
			foreach (Data i in list) {
				// 아이콘が同じ
				if (wi.IconIndex != i.IconIndex) continue;
				// 距離が近い
				Vector2 p1 = transform.ToVector2(wi.Position);
				Vector2 p2 = transform.ToVector2(i.Position);
				if ((p1 - p2).LengthSq() > REMOVE_LENGTH_SQ) continue;

				// 近い위치にあるので추가しない
				return false;
			}
			// 추가する
			return true;
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private DataListBB create_bb(ref List<Data> free_list) {
			List<Data> old = free_list;
			free_list = new List<Data>();

			DataListBB bb = new DataListBB();
			foreach (Data i in old) {
				// 그리기플래그がないものは추가しない
				if (!is_draw(i.IconIndex)) continue;

				if (!bb.Add(i)) {
					free_list.Add(i);
				}
			}
			return bb;
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		public void Draw() {
			if (!m_lib.setting.draw_web_icons) return;

			m_lib.loop_image.EnumDrawCallBack(new LoopXImage.DrawHandler(draw_proc), 32f);
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		private void draw_proc(Vector2 offset, LoopXImage image) {
			float size = image.ImageScale;
			if (size < 0.5) size = 0.5f;
			else if (size > 1) size = 1;
			Vector2 scale = new Vector2(size, size);

			D3dBB2d.CullingRect rect = new D3dBB2d.CullingRect(image.Device.client_size);
			m_lib.device.sprites.BeginDrawSprites(m_lib.icons.texture, offset, image.ImageScale, scale);
			foreach (DataListBB i in m_draw_list) {
				// 바운딩 박스で화면외かどうか조사
				if (i.IsCulling(offset, image.ImageScale, rect)) {
					continue;
				}
#if DRAW_POPUPS_BOUNDINGBOX
				d3d_bb2d.Draw(i.bb, image.device, 0.5f, offset, image.scale, Color.Blue.ToArgb());
#endif
				foreach (Data p in i.List) {
					int index = p.IconIndex;
					if (index < 0) index = 0;
					if (index > (int)icon_index.memo_4) index = (int)icon_index.memo_4;

					Vector3 pos = new Vector3(p.Position.X, p.Position.Y, 0.8f);
					m_lib.device.sprites.AddDrawSpritesNC(pos, m_lib.icons.GetIcon(icons.icon_index.web_icon_0 + index));
				}
			}
			m_lib.device.sprites.EndDrawSprites();
		}

		/*-------------------------------------------------------------------------
		 표시항목チェック
		---------------------------------------------------------------------------*/
		private bool is_draw(int index) {
			// 그리기플래그
			DrawSettingWebIcons flag = m_lib.setting.draw_setting_web_icons;

			switch ((icon_index)index) {
				case icon_index.wind_0:
				case icon_index.wind_1:
				case icon_index.wind_2:
				case icon_index.wind_3:
				case icon_index.wind_4:
				case icon_index.wind_5:
				case icon_index.wind_6:
				case icon_index.wind_7:
					if ((flag & DrawSettingWebIcons.wind) == 0) return false;
					break;
				case icon_index.memo_0:
					if ((flag & DrawSettingWebIcons.accident_0) == 0) return false;
					break;
				case icon_index.memo_1:
					if ((flag & DrawSettingWebIcons.accident_1) == 0) return false;
					break;
				case icon_index.memo_2:
					if ((flag & DrawSettingWebIcons.accident_2) == 0) return false;
					break;
				case icon_index.memo_3:
					if ((flag & DrawSettingWebIcons.accident_3) == 0) return false;
					break;
				case icon_index.memo_4:
					if ((flag & DrawSettingWebIcons.accident_4) == 0) return false;
					break;
			}
			return true;
		}
	}
}
