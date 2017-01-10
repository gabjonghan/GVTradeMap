//-------------------------------------------------------------------------
// 키アサイン
//-------------------------------------------------------------------------
using System;
using System.Windows.Forms;

//-------------------------------------------------------------------------
namespace Utility.KeyAssign
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// 키アサインフォーム, 
	/// 1つの키アサインを編集する. 
	/// </summary>
	public partial class KeyAssignForm : Form
	{
		private KeyAssignList.Assign		m_assign;
		private Keys						m_new_assign;

		//-------------------------------------------------------------------------
		/// <summary>
		/// 新しくアサインされた키を得る
		/// </summary>
		public Keys NewAssignKey{
			get{
				return m_new_assign;
			}
		}
	
		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축
		/// </summary>
		/// <param name="assign">編集する키アサイン</param>
		public KeyAssignForm(KeyAssignList.Assign assign)
		{
			InitializeComponent();

			m_assign			= assign;
			label4.Text			= assign.Name;
			label3.Text			= assign.Group;

			label1.Text			= assign.KeysString;

			// なにも없음でOKを押すと할당없음となる
			m_new_assign		= Keys.None;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 키が押された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
//			Console.WriteLine(e.KeyData.ToString());
//			Console.WriteLine(((int)e.KeyData).ToString());
	
			// BEEP音を抑制する
			// 入力がテキストボックスに反映されないようにする
			e.SuppressKeyPress	= true;
	
			// 入力された키を反映させる
			textBox1.Text		= m_assign.GetKeysString(e.KeyData);

			// 할당가능なら値を覚えておく
			m_new_assign		= (m_assign.CanAssignKeys(e.KeyData))
									? e.KeyData
									: Keys.None;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// formがアクティブになった
		/// テキストボックスにフォーカスを移す
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void key_assign_form_Shown(object sender, EventArgs e)
		{
			textBox1.Focus();
		}
	}
}
