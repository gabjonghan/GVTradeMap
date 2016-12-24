/*-------------------------------------------------------------------------

 ヒットテスト

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Drawing;
using System.Collections;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace Utility {
	/*-------------------------------------------------------------------------
	 1つの矩形とのテスト
	 矩形1つの場合はそのまま使ってよい
	 複수の場合は hittest_list を使うと扱いが楽
	---------------------------------------------------------------------------*/
	public class hittest {
		private Rectangle m_rect;   // 判定矩形
		private Point m_position;   // オフセット
		private float m_scale;	  // 스케일
		private bool m_enable;	  // 유효時true
		private float m_angle;	  // 각도
		private int m_priority;	  // 우선순위

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public Rectangle rect {
			get { return m_rect; }
			set { m_rect = value; }
		}
		public Point position {
			get { return m_position; }
			set { m_position = value; }
		}
		public float scale {
			get { return m_scale; }
			set { m_scale = value; }
		}
		public bool enable {
			get { return m_enable; }
			set { m_enable = value; }
		}
		public float angle {
			get { return m_angle; }
			set { m_angle = value; }
		}
		public int priority {
			get { return m_priority; }
			set { m_priority = value; }
		}

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public hittest() {
			m_rect = new Rectangle(0, 0, 0, 0);
			m_position = new Point(0, 0);
			m_scale = 1;
			m_enable = true;
		}
		public hittest(Rectangle rect) {
			m_rect = rect;
			m_position = new Point(0, 0);
			m_scale = 1;
			m_enable = true;
		}
		public hittest(Rectangle rect, Point position) {
			m_rect = rect;
			m_position = position;
			m_scale = 1;
			m_enable = true;
		}
		public hittest(Rectangle rect, Point position, float scale) {
			m_rect = rect;
			m_position = position;
			m_scale = scale;
			m_enable = true;
		}
		public hittest(Rectangle rect, Point position, float scale, float angle) {
			m_rect = rect;
			m_position = position;
			m_scale = scale;
			m_enable = true;
			m_angle = angle;
		}

		/*-------------------------------------------------------------------------
		 hittestとの比較
		---------------------------------------------------------------------------*/
		public bool HitTest(hittest hit) {
			Rectangle rect = hit.CalcRect();
			return HitTest(rect);
		}

		/*-------------------------------------------------------------------------
		 矩形との比較
		---------------------------------------------------------------------------*/
		public bool HitTest(Rectangle rect) {
			if (!enable) return false;

			Rectangle my_rect = CalcRect();
			if (my_rect.X >= rect.X + rect.Width) return false;
			if (my_rect.Y >= rect.Y + rect.Height) return false;
			if (my_rect.X + my_rect.Width < rect.X) return false;
			if (my_rect.Y + my_rect.Height < rect.Y) return false;
			return true;
		}

		/*-------------------------------------------------------------------------
		 点との比較
		---------------------------------------------------------------------------*/
		public bool HitTest(Point pos) {
			if (!enable) return false;

			Rectangle my_rect = CalcRect();
			if (pos.X < my_rect.X) return false;
			if (pos.Y < my_rect.Y) return false;
			if (pos.X >= my_rect.X + my_rect.Width) return false;
			if (pos.Y >= my_rect.Y + my_rect.Height) return false;
			return true;
		}

		/*-------------------------------------------------------------------------
		 hittestとの比較
		 合格するとコールバックが呼ばれる
		---------------------------------------------------------------------------*/
		public bool HitTest(hittest hit, int type) {
			if (HitTest(hit)) {
				OnHit(hit, type);
				return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 矩形との比較
		 合格するとコールバックが呼ばれる
		---------------------------------------------------------------------------*/
		public bool HitTest(Rectangle rect, int type) {
			if (HitTest(rect)) {
				OnHit(rect, type);
				return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 点との比較
		 合格するとコールバックが呼ばれる
		---------------------------------------------------------------------------*/
		public bool HitTest(Point pos, int type) {
			if (HitTest(pos)) {
				OnHit(pos, type);
				return true;
			}
			return false;
		}

		/*-------------------------------------------------------------------------
		 스케일등を考慮した矩形を得る
		 사이즈등は整수に丸められる
		---------------------------------------------------------------------------*/
		public Rectangle CalcRect() {
			float x = m_rect.X + (m_scale * m_position.X);
			float y = m_rect.Y + (m_scale * m_position.Y);
			float size_x = m_scale * m_rect.Width;
			float size_y = m_scale * m_rect.Height;
			return new Rectangle((int)x, (int)y, (int)size_x, (int)size_y);
		}

		/*-------------------------------------------------------------------------

		 継承先でのオーバーライド용
		 
		---------------------------------------------------------------------------*/

		/*-------------------------------------------------------------------------
		 ヒットテストに合格したときのコールバック
		 type はHitTest()に渡された値がそのまま渡される
		---------------------------------------------------------------------------*/
		virtual protected void OnHit(hittest hit, int type) {
		}
		virtual protected void OnHit(Rectangle rect, int type) {
		}
		virtual protected void OnHit(Point pos, int type) {
		}
	}

	/*-------------------------------------------------------------------------
	 복수의 사각형 겹침을 테스트
	---------------------------------------------------------------------------*/
	public class hittest_list : IEnumerable<hittest> {
		private List<hittest> m_list;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public int Count { get { return m_list.Count; } }
		public hittest this[int index] { get { return m_list[index]; } }

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public hittest_list() {
			m_list = new List<hittest>();
		}

		/*-------------------------------------------------------------------------
		 추가
		---------------------------------------------------------------------------*/
		public hittest Add(hittest _hittest) {
			m_list.Add(_hittest);
			return _hittest;
		}

		/*-------------------------------------------------------------------------
		 삭제
		---------------------------------------------------------------------------*/
		public void Remove(hittest _hittest) {
			m_list.Remove(_hittest);
		}

		/*-------------------------------------------------------------------------
		 점과의 비교
		---------------------------------------------------------------------------*/
		public int HitTest_Index(Point pos) {
			int index = 0;
			foreach (hittest h in m_list) {
				if (h.HitTest(pos)) return index;
				index++;
			}
			return -1;
		}
		public hittest HitTest(Point pos) {
			foreach (hittest h in m_list) {
				if (h.HitTest(pos)) return h;
			}
			return null;
		}

		/*-------------------------------------------------------------------------
		 점과의 비교
		 합격하면 콜백을 호출
		---------------------------------------------------------------------------*/
		public int HitTest_Index(Point pos, int type) {
			int index = 0;
			foreach (hittest h in m_list) {
				if (h.HitTest(pos, type)) return index;
				index++;
			}
			return -1;
		}
		public hittest HitTest(Point pos, int type) {
			foreach (hittest h in m_list) {
				if (h.HitTest(pos, type)) return h;
			}
			return null;
		}

		/*-------------------------------------------------------------------------
		 열挙
		---------------------------------------------------------------------------*/
		public IEnumerator<hittest> GetEnumerator() {
			foreach (hittest h in m_list) {
				yield return h;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		/*-------------------------------------------------------------------------
		 全て삭제
		---------------------------------------------------------------------------*/
		public void Clear() {
			m_list.Clear();
		}
	}
}
