//-------------------------------------------------------------------------
// 키 할당
//-------------------------------------------------------------------------
using System;
using System.Windows.Forms;

//-------------------------------------------------------------------------
namespace Utility.KeyAssign
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// 키 할당 폼,
	/// 하나의 키 할당을 편집한다.
	/// </summary>
	public partial class KeyAssignForm : Form
	{
		private KeyAssignList.Assign		m_assign;
		private Keys						m_new_assign;

		//-------------------------------------------------------------------------
		/// <summary>
		/// 새로 할당된 키를 얻는다
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
		/// <param name="assign">편집할 키 할당</param>
		public KeyAssignForm(KeyAssignList.Assign assign)
		{
			InitializeComponent();

			m_assign			= assign;
			label4.Text			= assign.Name;
			label3.Text			= assign.Group;

			label1.Text			= assign.KeysString;

			// 아무것도 없는 상태로 OK 를 누르면 할당 없음이 된다
			m_new_assign		= Keys.None;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 키가 눌렸다
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
//			Console.WriteLine(e.KeyData.ToString());
//			Console.WriteLine(((int)e.KeyData).ToString());
	
			// BEEP 음을 억제한다
			// 입력이 텍스트 박스에 반영되지 않도록 한다
			e.SuppressKeyPress	= true;
	
			// 입력된 키를 반영시킨다
			textBox1.Text		= m_assign.GetKeysString(e.KeyData);

			// 할당 가능하면 값을 기억해 둔다
			m_new_assign		= (m_assign.CanAssignKeys(e.KeyData))
									? e.KeyData
									: Keys.None;
		}

		//-------------------------------------------------------------------------
		/// <summary>
		/// form 이 활성화되었다
		/// 텍스트 박스로 포커스를 옮긴다
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void key_assign_form_Shown(object sender, EventArgs e)
		{
			textBox1.Focus();
		}
	}
}
