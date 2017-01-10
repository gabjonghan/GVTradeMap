/*-------------------------------------------------------------------------

 Direct3D
 윈도우관리
 
---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Collections.Generic;
using System.Windows.Forms;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace directx {
	/*-------------------------------------------------------------------------
	 윈도우관리
	---------------------------------------------------------------------------*/
	public class d3d_windows {
		// 윈도우내判定
		public enum hit_check {
			title,			  // フレームの상部
			title_button,	   // タイトル내のボタン
			client,			 // クライアント領域
			inside,			 // 상記以외の윈도우내
			outside			 // 윈도우외
		};

		/*-------------------------------------------------------------------------
		 윈도우
		 継承してクライアント領域を그리기すること
		---------------------------------------------------------------------------*/
		public class window {
			private const int SMALL_HEADER_WIDTH = 48;

			public enum mode {
				normal,		 // 통상
				small,		  // 最소化
			};

			private d3d_windows m_ctrl;
			private d3d_device m_device;

			private mode m_window_mode;
			private bool m_is_draw_header;		  // ヘッダを그리기するときtrue
													// ヘッダを그리지않음ときは最소化できない

			private Vector2 m_pos;					  // 윈도우위치
			private Vector2 m_size;					 // 윈도우사이즈
			private Vector2 m_client_pos;			   // クライアント領域시작위치
			private Vector2 m_client_size;			  // クライアント領域사이즈

			private Vector2 m_screen_size;			  // 화면사이즈
														// クライアント領域그리기デリゲートが참조する
			private float m_z;

			private int m_back_color;			   // 背景색
			private int m_title_color;			  // タイトル색
			private int m_frame_color;			  // フレーム색

			private string m_title;				 // タイトル

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public d3d_device device { get { return m_device; } }
			public Vector2 pos {
				get { return m_pos; }
				set {
					m_pos = value;
					update_pos();
				}
			}
			public Vector2 size {
				get {
					if (m_window_mode == mode.small) return small_size;
					else return normal_size;
				}
				set {
					m_size = value;
					update_size();
				}
			}
			public Vector2 normal_size { get { return m_size; } }
			public Vector2 small_size { get { return new Vector2(SMALL_HEADER_WIDTH, 10); } }
			public int back_color {
				get { return m_back_color; }
				set { m_back_color = value; }
			}
			public int title_color {
				get { return m_title_color; }
				set { m_title_color = value; }
			}
			public int frame_color {
				get { return m_frame_color; }
				set { m_frame_color = value; }
			}

			public Vector2 client_pos { get { return m_pos + m_client_pos; } }
			public Vector2 client_size {
				get { return m_client_size; }
				set { m_client_size = value; update_client_size(); }
			}
			public float z { get { return m_z; } }
			public mode window_mode {
				get { return m_window_mode; }
				set { m_window_mode = value; }
			}
			public Vector2 screen_size { get { return m_screen_size; } }
			public bool is_draw_header {
				get { return m_is_draw_header; }
				set { m_is_draw_header = value; }
			}

			public string title {
				get { return m_title; }
				set { m_title = value; }
			}

			internal d3d_windows ctrl { set { m_ctrl = value; } }

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			private void update_pos() {
				if (m_pos.X < 0) m_pos.X = 0;
				if (m_pos.X >= m_device.client_size.X - 10) m_pos.X = m_device.client_size.X - 10;
				if (m_pos.Y < 0) m_pos.Y = 0;
				if (m_pos.Y >= m_device.client_size.Y - 10) m_pos.Y = m_device.client_size.Y - 10;
			}
			private void update_size() {
				if (m_size.X < 30) m_size.X = 10;
				if (m_size.Y < 10 + 4 * 2) m_size.Y = 10 + 4 * 2;

				if (is_draw_header) {
					m_client_pos = new Vector2(4, 10 + 4);
					m_client_size = new Vector2(m_size.X - 4 * 2, m_size.Y - (10 + 4 * 2));
				} else {
					// ヘッダ없음
					m_client_pos = new Vector2(4, 4);
					m_client_size = new Vector2(m_size.X - 4 * 2, m_size.Y - (4 * 2));
				}
			}
			private void update_client_size() {
				m_size.X = m_client_size.X + 4 * 2;
				m_size.Y = m_client_size.Y + 4 * 2;
				if (is_draw_header) m_size.Y += 10;
				update_size();
			}

			/*-------------------------------------------------------------------------
			 
			---------------------------------------------------------------------------*/
			public window(d3d_device device, Vector2 pos, Vector2 size, float z) {
				m_device = device;
				m_pos = pos;
				m_size = size;
				m_z = z;
				title = "タイトル";

				m_back_color = Color.FromArgb(180, 170, 170, 170).ToArgb();
				title_color = Color.SkyBlue.ToArgb();
				m_frame_color = Color.Black.ToArgb();

				m_window_mode = mode.normal;
				m_screen_size = new Vector2(0, 0);
				is_draw_header = true;

				update_pos();
				update_size();
			}

			/*-------------------------------------------------------------------------
			 マウス押し始め
			---------------------------------------------------------------------------*/
			public bool OnMouseDown(Point pos, MouseButtons button) {
				switch (HitTest(pos)) {
					case hit_check.inside:
					case hit_check.title:
					case hit_check.title_button:
						return true;
					case hit_check.client:
						// クライアント領域
						OnMouseDownClient(pos, button);
						return true;
				}
				return false;
			}

			/*-------------------------------------------------------------------------
			 マウスクリック
			 押して離したとき
			 ボタン계は기본的に離したときに動作する
			---------------------------------------------------------------------------*/
			public bool OnMouseClik(Point pos, MouseButtons button) {
				switch (HitTest(pos)) {
					case hit_check.inside:
					case hit_check.title:
						return true;
					case hit_check.title_button:
						if ((button & MouseButtons.Left) != 0) {
							// 左クリック時のみ
							ToggleWindowMode();
						}
						return true;
					case hit_check.client:
						// クライアント領域
						OnMouseClikClient(pos, button);
						return true;
				}
				return false;
			}

			/*-------------------------------------------------------------------------
			 マウスダブルクリック
			---------------------------------------------------------------------------*/
			public bool OnMouseDClik(Point pos, MouseButtons button) {
				switch (HitTest(pos)) {
					case hit_check.inside:
						return true;
					case hit_check.title:
					case hit_check.title_button:
						if ((button & MouseButtons.Left) != 0) {
							// 左ダブルクリック時のみ
							ToggleWindowMode();
						}
						return true;
					case hit_check.client:
						// クライアント領域
						OnMouseDClikClient(pos, button);
						return true;
				}
				return false;
			}

			/*-------------------------------------------------------------------------
			 マウスホイール
			---------------------------------------------------------------------------*/
			public bool OnMouseWheel(Point pos, int delta) {
				switch (HitTest(pos)) {
					case hit_check.inside:
					case hit_check.title:
					case hit_check.title_button:
						return true;
					case hit_check.client:
						// クライアント領域
						OnMouseWheelClient(pos, delta);
						return true;
				}
				return false;
			}

			/*-------------------------------------------------------------------------
			 ツールチップ용문자열
			---------------------------------------------------------------------------*/
			public string OnToolTipString(Point pos) {
				switch (HitTest(pos)) {
					case hit_check.title:
						if (window_mode == mode.normal) return title + "\nダブルクリックで最소化";
						else return title + "\nダブルクリックで元の사이즈に戻す";
					case hit_check.title_button:
						if (window_mode == mode.normal) return "クリックで最소化";
						else return "クリックで元の사이즈に戻す";
					case hit_check.client:
						// クライアント領域
						return OnToolTipStringClient(pos);
				}
				return null;
			}

			/*-------------------------------------------------------------------------
			 更新
			---------------------------------------------------------------------------*/
			public void Update() {
				//				m_screen_size	= new Vector2(m_device.device.Viewport.Width, m_device.device.Viewport.Height);
				m_screen_size = m_device.client_size;
				OnUpdateClient();
			}

			/*-------------------------------------------------------------------------
			 그리기
			---------------------------------------------------------------------------*/
			public void Draw() {
				if (m_window_mode == mode.small) {
					// 最소化
					int color = m_title_color;
					color &= 0x00ffffff;
					color |= (64 << 24);
					m_device.DrawFillRect(new Vector3(m_pos.X, m_pos.Y, m_z), new Vector2(SMALL_HEADER_WIDTH, 10), color);
					m_device.DrawLineRect(new Vector3(m_pos.X, m_pos.Y, m_z), new Vector2(SMALL_HEADER_WIDTH, 10), m_frame_color);

					// 통상化ボタン
					m_device.DrawLineRect(new Vector3(m_pos.X + SMALL_HEADER_WIDTH - 10, m_pos.Y, m_z), new Vector2(10, 10), Color.Black.ToArgb());
					m_device.DrawLineRect(new Vector3(m_pos.X + SMALL_HEADER_WIDTH - 7, m_pos.Y + 3, m_z), new Vector2(4, 4), Color.FromArgb(50, 50, 50).ToArgb());
				} else {
					// 통상時
					m_device.DrawFillRect(new Vector3(m_pos.X, m_pos.Y, m_z), m_size, m_back_color);
					if (is_draw_header) {
						m_device.DrawFillRect(new Vector3(m_pos.X, m_pos.Y, m_z), new Vector2(m_size.X, 10), m_title_color);
						m_device.DrawLineRect(new Vector3(m_pos.X, m_pos.Y, m_z), new Vector2(m_size.X, 10), m_frame_color);
					}
					m_device.DrawLineRect(new Vector3(m_pos.X, m_pos.Y, m_z), m_size, m_frame_color);

					// 最소化ボタン
					m_device.DrawFillRect(new Vector3(m_pos.X + m_size.X - 10, m_pos.Y, m_z), new Vector2(10, 10), Color.LightGray.ToArgb());
					m_device.DrawLineRect(new Vector3(m_pos.X + m_size.X - 10, m_pos.Y, m_z), new Vector2(10, 10), Color.Black.ToArgb());
					m_device.DrawLine(new Vector3(m_pos.X + m_size.X - 7, m_pos.Y + 7, m_z), new Vector2(m_pos.X + m_size.X - 2, m_pos.Y + 7), Color.FromArgb(50, 50, 50).ToArgb());

					if (m_client_size.Y >= 0) {
						// 그리기범위をクライアント領域に限定する
						//						Viewport	view	= SetViewport(	(int)client_pos.X,
						//															(int)client_pos.Y,
						//															(int)client_size.X +1,
						//															(int)client_size.Y +1);
						// 그리기
						OnDrawClient();
						// 元に戻す
						//						m_device.device.Viewport	= view;
					}
				}
			}

			/*-------------------------------------------------------------------------
			 ビュー포트を설정する
			 변경前のビュー포트を返す
			---------------------------------------------------------------------------*/
			/*			public Viewport SetViewport(int x, int y, int width, int height)
						{
							Viewport	view	= GetViewport();
							Viewport	w_view	= view;
							w_view.X		= x;
							w_view.Y		= y;
							w_view.Width	= width;
							w_view.Height	= height;
							m_device.device.Viewport	= w_view;
							return view;
						}
						public Viewport SetViewport(Viewport new_view)
						{
							Viewport	view	= GetViewport();
							m_device.device.Viewport	= new_view;
							return view;
						}
			*/
			/*-------------------------------------------------------------------------
			 ボタン계の현재용背景그리기
			 マウスを乗せたときに枠を出したいとき용
			---------------------------------------------------------------------------*/
			public void DrawCurrentButtonBack(Vector3 pos, Vector2 size) {
				// 枠付きで그리기
				int color = title_color;
				color &= 0x00ffffff;
				color |= (128 << 24);
				device.DrawFillRect(pos, size, color);
				device.DrawLineRect(pos, size, Color.FromArgb(255, 0, 0, 255).ToArgb());
			}

			/*-------------------------------------------------------------------------
			 ボタン계の현재용背景그리기
			 マウスを乗せたときに出したいとき용
			 枠없음
			---------------------------------------------------------------------------*/
			public void DrawCurrentButtonBack_WithoutFrame(Vector3 pos, Vector2 size) {
				// 枠付きで그리기
				int color = title_color;
				color &= 0x00ffffff;
				color |= (128 << 24);
				device.DrawFillRect(pos, size, color);
			}

			/*-------------------------------------------------------------------------
			 ビュー포트を得る
			---------------------------------------------------------------------------*/
			/*			public Viewport GetViewport()
						{
							return m_device.device.Viewport;
						}
			*/
			/*-------------------------------------------------------------------------
			 ヒットテスト
			---------------------------------------------------------------------------*/
			public hit_check HitTest(Point pos) {
				return HitTest(new Vector2(pos.X, pos.Y));
			}
			public hit_check HitTest(Vector2 pos) {
				if (m_window_mode == mode.small) {
					if (pos.X < m_pos.X) return hit_check.outside;
					//					if(pos.X >= m_pos.X + m_size.X)				return hit_check.outside;
					if (pos.X >= m_pos.X + SMALL_HEADER_WIDTH) return hit_check.outside;
					if (pos.Y < m_pos.Y) return hit_check.outside;
					if (pos.Y >= m_pos.Y + 10) return hit_check.outside;

					if (pos.X < m_pos.X + SMALL_HEADER_WIDTH - 10) return hit_check.title;
					else return hit_check.title_button;
				} else {
					if (pos.X < m_pos.X) return hit_check.outside;
					if (pos.X >= m_pos.X + m_size.X) return hit_check.outside;
					if (pos.Y < m_pos.Y) return hit_check.outside;
					if (pos.Y >= m_pos.Y + m_size.Y) return hit_check.outside;

					// 少なくとも윈도우내
					if (is_draw_header) {
						if (pos.Y < m_pos.Y + 10) {
							if (pos.X < m_pos.X + m_size.X - 10) return hit_check.title;
							else return hit_check.title_button;
						}
					}

					if (pos.X < client_pos.X) return hit_check.inside;
					if (pos.X >= client_pos.X + client_size.X) return hit_check.inside;
					if (pos.Y < client_pos.Y) return hit_check.inside;
					if (pos.Y >= client_pos.Y + client_size.Y) return hit_check.inside;

					// 残りはクライアント領域
					return hit_check.client;
				}
			}

			/*-------------------------------------------------------------------------
			 윈도우모드切替
			---------------------------------------------------------------------------*/
			public void ToggleWindowMode() {
				if (m_window_mode == mode.small) m_window_mode = mode.normal;
				else m_window_mode = mode.small;
			}

			/*-------------------------------------------------------------------------
			 윈도우검색
			 윈도우タイトルで검색
			---------------------------------------------------------------------------*/
			public d3d_windows.window FindWindow(string title) {
				if (m_ctrl == null) return null;
				return m_ctrl.FindWindow(title);
			}

			/*-------------------------------------------------------------------------

			 継承先でのオーバーライド용
			 
			---------------------------------------------------------------------------*/

			/*-------------------------------------------------------------------------
			 クライアント領域の更新
			---------------------------------------------------------------------------*/
			virtual protected void OnUpdateClient() {
			}
			/*-------------------------------------------------------------------------
			 クライアント領域の그리기
			---------------------------------------------------------------------------*/
			virtual protected void OnDrawClient() {
			}
			/*-------------------------------------------------------------------------
			 マウス押し始め
			---------------------------------------------------------------------------*/
			virtual protected void OnMouseDownClient(Point pos, MouseButtons button) {
			}
			/*-------------------------------------------------------------------------
			 マウスクリック
			---------------------------------------------------------------------------*/
			virtual protected void OnMouseClikClient(Point pos, MouseButtons button) {
			}
			/*-------------------------------------------------------------------------
			 マウスダブルリック
			---------------------------------------------------------------------------*/
			virtual protected void OnMouseDClikClient(Point pos, MouseButtons button) {
			}
			/*-------------------------------------------------------------------------
			 マウスホイール
			---------------------------------------------------------------------------*/
			virtual protected void OnMouseWheelClient(Point pos, int delta) {
			}
			/*-------------------------------------------------------------------------
			 クライアント領域のツールチップの取得
			---------------------------------------------------------------------------*/
			virtual protected string OnToolTipStringClient(Point pos) {
				return null;
			}
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		private d3d_device m_device;
		private List<window> m_windows;		 // 윈도우

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public List<window> window_list { get { return m_windows; } }

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public d3d_windows(d3d_device device) {
			m_device = device;
			m_windows = new List<window>();
		}

		/*-------------------------------------------------------------------------
		 更新
		---------------------------------------------------------------------------*/
		public void Update() {
			foreach (window w in m_windows) {
				w.Update();
			}
		}

		/*-------------------------------------------------------------------------
		 그리기
		---------------------------------------------------------------------------*/
		public void Draw() {
			foreach (window w in m_windows) {
				w.Draw();
			}
		}

		/*-------------------------------------------------------------------------
		 추가
		---------------------------------------------------------------------------*/
		public window Add(window _window) {
			_window.ctrl = this;
			m_windows.Add(_window);
			return _window;
		}

		/*-------------------------------------------------------------------------
		 삭제
		---------------------------------------------------------------------------*/
		public void Remove(window _window) {
			try {
				m_windows.Remove(_window);
			} catch {
			}
		}

		/*-------------------------------------------------------------------------
		 ヒットチェックのみ
		---------------------------------------------------------------------------*/
		public bool HitTest(Vector2 pos) {
			foreach (window w in m_windows) {
				if (w.HitTest(pos) != hit_check.outside) {
					return true;
				}
			}
			return false;
		}
		public bool HitTest(Point pos) {
			foreach (window w in m_windows) {
				if (w.HitTest(pos) != hit_check.outside) {
					return true;
				}
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 ダブルクリック時
		 범위외のときfalseを返す
		---------------------------------------------------------------------------*/
		public bool OnMouseDoubleClick(Point pos, MouseButtons button) {
			foreach (window w in m_windows) {
				if (w.OnMouseDClik(pos, button)) return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 マウス押し始め
		 범위외のときfalseを返す
		---------------------------------------------------------------------------*/
		public bool OnMouseDown(Point pos, MouseButtons button) {
			foreach (window w in m_windows) {
				if (w.OnMouseDown(pos, button)) return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 クリック時
		 범위외のときfalseを返す
		---------------------------------------------------------------------------*/
		public bool OnMouseClick(Point pos, MouseButtons button) {
			foreach (window w in m_windows) {
				if (w.OnMouseClik(pos, button)) return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 マウスホイール時
		 범위외のときfalseを返す
		---------------------------------------------------------------------------*/
		public bool OnMouseWheel(Point pos, int delta) {
			foreach (window w in m_windows) {
				if (w.OnMouseWheel(pos, delta)) return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 ツールチップ용の문자열を返す
		 ツールチップがないときnullを返す
		---------------------------------------------------------------------------*/
		public string GetToolTipString(Point pos) {
			foreach (window w in m_windows) {
				string str = w.OnToolTipString(pos);
				if (str != null) return str;
			}
			return null;
		}

		/*-------------------------------------------------------------------------
		 윈도우を검색する
		 タイトルで검색
		---------------------------------------------------------------------------*/
		public d3d_windows.window FindWindow(string title) {
			foreach (window w in m_windows) {
				if (w.title == title) return w;
			}
			return null;
		}
	}
}
