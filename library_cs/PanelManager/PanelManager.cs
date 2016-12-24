/*-------------------------------------------------------------------------

 TabControlのtab비표시版

 コードは以下のTipsから持ってきています
 .NET Framework2.0용に多少조정している程度

 Mick Doherty's .net Tips and Tricks
 http://dotnetrix.co.uk/default.htm

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel.Design;

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
namespace Controls
{
	/*-------------------------------------------------------------------------
	 TabControl
	 without tab
	---------------------------------------------------------------------------*/
	[DefaultProperty("SelectedPanel")]
	[DefaultEvent("SelectedIndexChanged")]
	[Designer(typeof(Design.PanelManagerDesigner))]
	public class PanelManager : System.Windows.Forms.Control
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PanelManager()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		private Controls.ManagedPanel m_SelectedPanel;
		
		public event EventHandler SelectedIndexChanged;

		//ManagedPanels
		[Editor(typeof(Editors.ManagedPanelCollectionEditor), typeof(UITypeEditor))]
		public ControlCollection ManagedPanels
		{
			get{return base.Controls;}
		}
	

		//SelectedPanel
		[TypeConverter(typeof(TypeConverters.SelectedPanelConverter))]
		public Controls.ManagedPanel SelectedPanel
		{
			get{return m_SelectedPanel;}
			set
			{
				if (m_SelectedPanel == value) return;
				m_SelectedPanel = value;
				OnSelectedPanelChanged(EventArgs.Empty);
			}
		}
			 

		//SelectedIndex
		[Browsable(false)]
		public int SelectedIndex
		{
			get{return this.ManagedPanels.IndexOf((ManagedPanel)this.SelectedPanel);}
			set
			{
				if (value == -1)
					this.SelectedPanel = null;
				else
					this.SelectedPanel = (ManagedPanel)this.ManagedPanels[value];
			}
		}


		//DefaultSize
		protected override Size DefaultSize
		{
			get
			{
				return new Size(200,100);
			}
		}


		private ManagedPanel oldSelection = null;

		protected void OnSelectedPanelChanged(EventArgs e)
		{
			if (oldSelection != null)
				oldSelection.Visible = false;

			if (m_SelectedPanel != null)
				((Controls.ManagedPanel)m_SelectedPanel).Visible = true;

			bool tabChanged = false;
			if (m_SelectedPanel == null)
				tabChanged = (oldSelection != null);
			else
				tabChanged = (!m_SelectedPanel.Equals(oldSelection));

			if (tabChanged && this.Created)
				if (SelectedIndexChanged != null)
					SelectedIndexChanged(this, EventArgs.Empty);

			oldSelection = (Controls.ManagedPanel)m_SelectedPanel;

		}


		protected override void OnControlAdded(ControlEventArgs e)
		{
			if ((e.Control is Controls.ManagedPanel) == false)
				throw new ArgumentException("Only Mangel.Controls.ManagedPanels can be added to a Mangel.Controls.PanelManger.");
			
			if (this.SelectedPanel != null)
				((Controls.ManagedPanel)this.SelectedPanel).Visible = false;

			this.SelectedPanel = (Controls.ManagedPanel)e.Control;
			e.Control.Visible = true;
			base.OnControlAdded (e);
		}


		protected override void OnControlRemoved(ControlEventArgs e)
		{
			if (e.Control is Controls.ManagedPanel)
			{
				if (this.ManagedPanels.Count > 0)
					this.SelectedIndex = 0;
				else
					this.SelectedPanel = null;
			}
			base.OnControlRemoved (e);
		}
	}

	/*-------------------------------------------------------------------------
	 TabPage
	---------------------------------------------------------------------------*/
	[Designer(typeof(Design.ManagedPanelDesigner))]
	[ToolboxItem(false)]
	public class ManagedPanel: System.Windows.Forms.ScrollableControl
	{
		
		public ManagedPanel()
		{
			base.Dock = DockStyle.Fill;
			SetStyle(ControlStyles.ResizeRedraw, true);
		}


		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(typeof(DockStyle), "Fill")]
		public override DockStyle Dock
		{
			get
			{
				return base.Dock;
			}
			set
			{
				base.Dock = DockStyle.Fill;
			}
		}


		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(typeof(AnchorStyles), "None")]
		public override AnchorStyles Anchor
		{
			get
			{
				return AnchorStyles.None;
			}
			set
			{
				base.Anchor = AnchorStyles.None;
			}
		}


		protected override void OnLocationChanged(EventArgs e)
		{
			base.OnLocationChanged (e);
			base.Location = Point.Empty;
		}


		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged (e);
			if (this.Parent == null)
				this.Size = Size.Empty;
			else
				this.Size = this.Parent.ClientSize;
		}


		protected override void OnParentChanged(EventArgs e)
		{
			if ((this.Parent is Controls.PanelManager)==false &&  this.Parent != null)
				throw new ArgumentException("Managed Panels may only be added to a Panel Manager.");
			base.OnParentChanged (e);
		}
	}
}

namespace Design
{
	public class PanelManagerDesigner:System.Windows.Forms.Design.ParentControlDesigner
	{
		private DesignerVerbCollection m_verbs = new DesignerVerbCollection();
		private IDesignerHost m_DesignerHost;
		private ISelectionService m_SelectionService;
		
		private Controls.PanelManager HostControl
		{
			get{return (Controls.PanelManager)this.Control;}
		}


		public PanelManagerDesigner():base()
		{
			DesignerVerb verb1 = new DesignerVerb("ページの추가", new EventHandler(OnAddPanel));
			DesignerVerb verb2 = new DesignerVerb("ページの삭제", new EventHandler(OnRemovePanel));
			m_verbs.AddRange(new DesignerVerb[] {verb1, verb2});
		}


		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			// Don't want DrawGrid Dots.
		}


		public override DesignerVerbCollection Verbs
		{
			get
			{
				if (m_verbs.Count == 2)
					m_verbs[1].Enabled = HostControl.ManagedPanels.Count > 0;
				return m_verbs;
			}
		}


		public IDesignerHost DesignerHost
		{
			get
			{
				if (m_DesignerHost == null)
					m_DesignerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
				return m_DesignerHost;
			}
		}


		public ISelectionService SelectionService
		{
			get
			{
				if (m_SelectionService == null)
					m_SelectionService = (ISelectionService)GetService(typeof(ISelectionService));
				
				return m_SelectionService;
			}
		}
		

		private void OnAddPanel(Object sender, EventArgs e)
		{
			Control.ControlCollection oldManagedPanels = HostControl.Controls;
			
			RaiseComponentChanging(TypeDescriptor.GetProperties(HostControl)["ManagedPanels"]);
			
			Controls.ManagedPanel P = (Controls.ManagedPanel)DesignerHost.CreateComponent(typeof(Controls.ManagedPanel));
			P.Text = P.Name;
			HostControl.ManagedPanels.Add(P);
			
			RaiseComponentChanged(TypeDescriptor.GetProperties(HostControl)["ManagedPanels"], oldManagedPanels, HostControl.ManagedPanels);
			HostControl.SelectedPanel = P;
			
			SetVerbs();
		}


		private void OnRemovePanel(Object sender, EventArgs e)
		{
			Control.ControlCollection oldManagedPanels = HostControl.Controls;
			
			if (HostControl.SelectedIndex < 0) return;
			
			RaiseComponentChanging(TypeDescriptor.GetProperties(HostControl)["TabPages"]);
			
			DesignerHost.DestroyComponent((Controls.ManagedPanel)HostControl.ManagedPanels[HostControl.SelectedIndex]);

			RaiseComponentChanged(TypeDescriptor.GetProperties(HostControl)["ManagedPanels"], oldManagedPanels, HostControl.ManagedPanels);
			
			SelectionService.SetSelectedComponents(new IComponent[] {HostControl}, SelectionTypes.Auto);
			
			SetVerbs();
		}


		private void SetVerbs()
		{
			Verbs[1].Enabled = HostControl.ManagedPanels.Count == 1;
		}


		protected override void PostFilterProperties(IDictionary properties)
		{
			properties.Remove("AutoScroll");
			properties.Remove("AutoScrollMargin");
			properties.Remove("AutoScrollMinSize");
			properties.Remove("Text");
			base.PostFilterProperties (properties);
		}

		public override void InitializeNewComponent(IDictionary defaultValues)
		{
			base.InitializeNewComponent(defaultValues);

			HostControl.ManagedPanels.Add((Controls.ManagedPanel)DesignerHost.CreateComponent(typeof(Controls.ManagedPanel)));
			HostControl.ManagedPanels.Add((Controls.ManagedPanel)DesignerHost.CreateComponent(typeof(Controls.ManagedPanel)));
			Controls.PanelManager pm = (Controls.PanelManager)this.Control;
			pm.ManagedPanels[0].Text = pm.ManagedPanels[0].Name;
			pm.ManagedPanels[1].Text = pm.ManagedPanels[1].Name;
			HostControl.SelectedIndex = 0;
		}

/*		public override void OnSetComponentDefaults()
		{
			HostControl.ManagedPanels.Add((Controls.ManagedPanel)DesignerHost.CreateComponent(typeof(Controls.ManagedPanel)));
			HostControl.ManagedPanels.Add((Controls.ManagedPanel)DesignerHost.CreateComponent(typeof(Controls.ManagedPanel)));
			Controls.PanelManager pm = (Controls.PanelManager)this.Control;
			pm.ManagedPanels[0].Text = pm.ManagedPanels[0].Name;
			pm.ManagedPanels[1].Text = pm.ManagedPanels[1].Name;
			HostControl.SelectedIndex = 0;
		}
*/
	}


	public class ManagedPanelDesigner:System.Windows.Forms.Design.ScrollableControlDesigner
	{

		private DesignerVerbCollection m_verbs = new DesignerVerbCollection();
		private ISelectionService m_SelectionService;

		private Controls.ManagedPanel HostControl
		{
			get
			{return (Controls.ManagedPanel)this.Control;}
		}


		public ISelectionService SelectionService
		{
			get
			{
				if (m_SelectionService == null)
					m_SelectionService = (ISelectionService)GetService(typeof(ISelectionService));
				
				return m_SelectionService;
			}
		}


		public ManagedPanelDesigner():base()
		{
			DesignerVerb verb1 = new DesignerVerb("PanelManagerを선택", new EventHandler(OnSelectManager));
			m_verbs.Add(verb1);
		}


		private void OnSelectManager(Object sender, EventArgs e)
		{
			if (this.HostControl.Parent != null)
				this.SelectionService.SetSelectedComponents(new Component[] {this.HostControl.Parent});
		}


		public override System.Windows.Forms.Design.SelectionRules SelectionRules
		{
			get
			{
				return System.Windows.Forms.Design.SelectionRules.Visible;
			}
		}


		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			base.OnPaintAdornments (pe);
			Color penColor;
			if (this.Control.BackColor.GetBrightness() >= 0.5)
				penColor = ControlPaint.Dark(this.Control.BackColor);
			else
				penColor = Color.White;

			Pen dashedPen = new Pen(penColor);
			Rectangle borderRectangle = this.Control.ClientRectangle;
			borderRectangle.Width -= 1;
			borderRectangle.Height -= 1;
			dashedPen.DashStyle = DashStyle.Dash;
			pe.Graphics.DrawRectangle(dashedPen, borderRectangle);
			dashedPen.Dispose();
		}


		public override DesignerVerbCollection Verbs
		{
			get
			{
				return m_verbs;
			}
		}


		protected override void PostFilterProperties(IDictionary properties)
		{
			properties.Remove("Anchor");
			properties.Remove("TabStop");
			properties.Remove("TabIndex");
			base.PostFilterProperties (properties);
		}

		public override void InitializeNewComponent(IDictionary defaultValues)
		{
			base.InitializeNewComponent(defaultValues);

			this.Control.Visible = true;
		}

//		public override void OnSetComponentDefaults()
//		{
//			base.OnSetComponentDefaults ();
//			this.Control.Visible = true;
//		}


	}

}


namespace Editors
{
	public class ManagedPanelCollectionEditor:System.ComponentModel.Design.CollectionEditor
	{
		public ManagedPanelCollectionEditor(Type type):base(type){}

		protected override Type CreateCollectionItemType()
		{
			return typeof(Controls.ManagedPanel);
		}


	}

}


namespace TypeConverters
{
	public class SelectedPanelConverter:ReferenceConverter
	{
		public SelectedPanelConverter():base(typeof(Controls.ManagedPanel)){}

		protected override bool IsValueAllowed(ITypeDescriptorContext context, object value)
		{
			if (context != null)
			{
				Controls.PanelManager pm = (Controls.PanelManager)context.Instance;
				return pm.ManagedPanels.Contains((Controls.ManagedPanel)value);
			}
			return false;

		}

	}
}
