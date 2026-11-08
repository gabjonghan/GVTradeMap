/*-------------------------------------------------------------------------

 조선 카운터 수동 시작 대화상자
 한국판 게임은 채팅 로그가 없어 조선 시작과 이자를 자동으로 알 수 없다

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Drawing;
using System.Windows.Forms;

using Utility;

/*-------------------------------------------------------------------------

---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	/*-------------------------------------------------------------------------

	---------------------------------------------------------------------------*/
	public class build_ship_form : Form
	{
		private TextBox			m_name;
		private NumericUpDown	m_days;

		/*-------------------------------------------------------------------------
		 카운터가 쓰는 이름. 일수는 "14일" 처럼 이름에 넣어 기억한다
		---------------------------------------------------------------------------*/
		public string ship_name{
			get{
				string	name	= m_name.Text.Trim();
				if(name == "")	name	= "선박";
				if(m_days.Value > 0)	name	+= String.Format(" {0}일", (int)m_days.Value);
				return name;
			}
		}

		/*-------------------------------------------------------------------------

		---------------------------------------------------------------------------*/
		public build_ship_form()
		{
			Text			= "조선 카운터 시작";
			FormBorderStyle	= FormBorderStyle.FixedDialog;
			MaximizeBox		= false;
			MinimizeBox		= false;
			ShowInTaskbar	= false;
			StartPosition	= FormStartPosition.CenterParent;
			AutoScaleMode	= AutoScaleMode.None;
			float	dpi		= Useful.GetDpiRatio();
			ClientSize		= new Size((int)(280 * dpi), (int)(112 * dpi));

			Label	label1	= new Label();
			label1.Text		= "선박명";
			label1.Bounds	= new Rectangle((int)(12 * dpi), (int)(15 * dpi), (int)(80 * dpi), (int)(20 * dpi));

			m_name			= new TextBox();
			m_name.Bounds	= new Rectangle((int)(96 * dpi), (int)(12 * dpi), (int)(172 * dpi), (int)(22 * dpi));

			Label	label2	= new Label();
			label2.Text		= "건조 일수";
			label2.Bounds	= new Rectangle((int)(12 * dpi), (int)(45 * dpi), (int)(80 * dpi), (int)(20 * dpi));

			m_days			= new NumericUpDown();
			m_days.Minimum	= 0;
			m_days.Maximum	= 999;
			m_days.Bounds	= new Rectangle((int)(96 * dpi), (int)(42 * dpi), (int)(72 * dpi), (int)(22 * dpi));

			Button	ok		= new Button();
			ok.Text			= "시작";
			ok.DialogResult	= DialogResult.OK;
			ok.Bounds		= new Rectangle((int)(112 * dpi), (int)(76 * dpi), (int)(75 * dpi), (int)(25 * dpi));

			Button	cancel	= new Button();
			cancel.Text			= "취소";
			cancel.DialogResult	= DialogResult.Cancel;
			cancel.Bounds		= new Rectangle((int)(193 * dpi), (int)(76 * dpi), (int)(75 * dpi), (int)(25 * dpi));

			Controls.AddRange(new Control[]{ label1, m_name, label2, m_days, ok, cancel });
			AcceptButton	= ok;
			CancelButton	= cancel;

			Useful.SetFontMeiryo(this, def.MEIRYO_POINT);
		}

		/*-------------------------------------------------------------------------
		 건조 중이 아니면 시작하고, 건조 중이면 초기화한다
		---------------------------------------------------------------------------*/
		static public void StartOrResetBuildShip(IWin32Window owner, GvoDatabase db)
		{
			if(db.BuildShipCounter.IsNowBuild){
				if(MessageBox.Show(owner, "조선 카운터를 초기화할까요?", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes){
					db.BuildShipCounter.FinishBuildShip();
				}
				return;
			}
			using(build_ship_form dlg = new build_ship_form()){
				if(dlg.ShowDialog(owner) == DialogResult.OK){
					db.BuildShipCounter.StartBuildShip(dlg.ship_name);
				}
			}
		}

		/*-------------------------------------------------------------------------
		 이자 카운터를 초기화한다
		---------------------------------------------------------------------------*/
		static public void ResetInterest(IWin32Window owner, GvoDatabase db)
		{
			if(MessageBox.Show(owner, "이자 카운터를 초기화할까요?", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes){
				db.InterestDays.ManualReset();
			}
		}
	}
}
