//-------------------------------------------------------------------------
// 키 할당
// 설정
//-------------------------------------------------------------------------
using System.Windows.Forms;

//-------------------------------------------------------------------------
namespace Utility.KeyAssign
{
	//-------------------------------------------------------------------------
	/// <summary>
	/// 키 할당 설정 폼
	/// </summary>
	public partial class KeyAssignListForm : Form
	{
		private KeyAssiginSettingHelper		m_helper;

		/// <summary>
		/// OK 버튼이 눌렸을 때의 변경 내용
		/// </summary>
		public KeyAssignList List			{	get{	return m_helper.List;	}}

		//-------------------------------------------------------------------------
		/// <summary>
		/// 구축
		/// </summary>
		/// <param name="assign_list">키 할당 목록</param>
		public KeyAssignListForm(KeyAssignList assign_list)
		{
			InitializeComponent();

			// 컬럼 정렬 유효
			listView1.EnableSort(true);

			// 헬퍼에 맡긴다
			m_helper	= new KeyAssiginSettingHelper(	assign_list,
														this,
														comboBox1,
														listView1,
														button1, button4, button5);
		}
	}
}
