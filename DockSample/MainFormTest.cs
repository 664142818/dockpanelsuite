using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DockSample
{
    public partial class MainFormTest : Form
    {
        private bool m_bSaveLayout = true;
        private DeserializeDockContent m_deserializeDockContent;
        private DummySolutionExplorer m_solutionExplorer;
        private DummyPropertyWindow m_propertyWindow;
        private DummyToolbox m_toolbox;
        private DummyOutputWindow m_outputWindow;
        private DummyTaskList m_taskList;
        private bool _showSplash;
        private SplashScreen _splashScreen;

        public MainFormTest()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;
        }

        private void menuItemNew_Click(object sender, EventArgs e)
        {
            DummyDoc dummyDoc = CreateNewDocument();
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                dummyDoc.MdiParent = this;
                dummyDoc.Show();
            }
            else
                dummyDoc.Show(dockPanel);
        }
        private DummyDoc CreateNewDocument()
        {
            DummyDoc dummyDoc = new DummyDoc();

            int count = 1;
            string text = $"Document{count}";
            while (FindDocument(text) != null)
            {
                count++;
                text = $"Document{count}";
            }

            dummyDoc.Text = text;
            return dummyDoc;
        }

        private IDockContent FindDocument(string text)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                    if (form.Text == text)
                        return form as IDockContent;

                return null;
            }
            else
            {
                foreach (IDockContent content in dockPanel.Documents)
                    if (content.DockHandler.TabText == text)
                        return content;

                return null;
            }
        }

        private void MainFormTest_Load(object sender, EventArgs e)
        {
            SetSchema(null, null);
        }

       

        private void CloseAllContents()
        {
            // we don't want to create another instance of tool window, set DockPanel to null
            //m_solutionExplorer.DockPanel = null;
            //m_propertyWindow.DockPanel = null;
            //m_toolbox.DockPanel = null;
            //m_outputWindow.DockPanel = null;
            //m_taskList.DockPanel = null;

            // Close all other document windows
            CloseAllDocuments();

            // IMPORTANT: dispose all float windows.
            foreach (var window in dockPanel.FloatWindows.ToList())
                window.Dispose();

            System.Diagnostics.Debug.Assert(dockPanel.Panes.Count == 0);
            System.Diagnostics.Debug.Assert(dockPanel.Contents.Count == 0);
            System.Diagnostics.Debug.Assert(dockPanel.FloatWindows.Count == 0);
        }

        private readonly ToolStripRenderer _toolStripProfessionalRenderer = new ToolStripProfessionalRenderer();

        private void SetSchema(object sender, System.EventArgs e)
        {
            // Persist settings when rebuilding UI
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.temp.config");

            dockPanel.SaveAsXml(configFile);
            CloseAllContents();

            WeifenLuo.WinFormsUI.Docking.VS2005Theme vS2005Theme1 = new VS2005Theme();
            this.dockPanel.Theme = vS2005Theme1;
            this.EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2005, vS2005Theme1);

            menuItemSchemaVS2005.Checked = (sender == menuItemSchemaVS2005);
            menuItemSchemaVS2003.Checked = (sender == menuItemSchemaVS2003);
            menuItemSchemaVS2012Light.Checked = (sender == menuItemSchemaVS2012Light);
            menuItemSchemaVS2012Blue.Checked = (sender == menuItemSchemaVS2012Blue);
            menuItemSchemaVS2012Dark.Checked = (sender == menuItemSchemaVS2012Dark);
            menuItemSchemaVS2013Light.Checked = (sender == menuItemSchemaVS2013Light);
            menuItemSchemaVS2013Blue.Checked = (sender == menuItemSchemaVS2013Blue);
            menuItemSchemaVS2013Dark.Checked = (sender == menuItemSchemaVS2013Dark);
            menuItemSchemaVS2015Light.Checked = (sender == menuItemSchemaVS2015Light);
            menuItemSchemaVS2015Blue.Checked = (sender == menuItemSchemaVS2015Blue);
            menuItemSchemaVS2015Dark.Checked = (sender == menuItemSchemaVS2015Dark);
            if (dockPanel.Theme.ColorPalette != null)
            {
                //statusBar.BackColor = dockPanel.Theme.ColorPalette.MainWindowStatusBarDefault.Background;
            }

            //if (File.Exists(configFile))
            //    dockPanel.LoadFromXml(configFile, m_deserializeDockContent);
        }

        private void EnableVSRenderer(VisualStudioToolStripExtender.VsVersion version, ThemeBase theme)
        {
            vsToolStripExtender1.SetStyle(mainMenu, version, theme);
            //vsToolStripExtender1.SetStyle(toolBar, version, theme);
            //vsToolStripExtender1.SetStyle(statusBar, version, theme);
        }

        private void SetDocumentStyle(object sender, System.EventArgs e)
        {
            DocumentStyle oldStyle = dockPanel.DocumentStyle;
            DocumentStyle newStyle;
            if (sender == menuItemDockingMdi)
                newStyle = DocumentStyle.DockingMdi;
            else if (sender == menuItemDockingWindow)
                newStyle = DocumentStyle.DockingWindow;
            else if (sender == menuItemDockingSdi)
                newStyle = DocumentStyle.DockingSdi;
            else
                newStyle = DocumentStyle.SystemMdi;

            if (oldStyle == newStyle)
                return;

            if (oldStyle == DocumentStyle.SystemMdi || newStyle == DocumentStyle.SystemMdi)
                CloseAllDocuments();

            dockPanel.DocumentStyle = newStyle;
            menuItemDockingMdi.Checked = (newStyle == DocumentStyle.DockingMdi);
            menuItemDockingWindow.Checked = (newStyle == DocumentStyle.DockingWindow);
            menuItemDockingSdi.Checked = (newStyle == DocumentStyle.DockingSdi);
            menuItemSystemMdi.Checked = (newStyle == DocumentStyle.SystemMdi);
            menuItemLayoutByCode.Enabled = (newStyle != DocumentStyle.SystemMdi);
            menuItemLayoutByXml.Enabled = (newStyle != DocumentStyle.SystemMdi);
            //toolBarButtonLayoutByCode.Enabled = (newStyle != DocumentStyle.SystemMdi);
            //toolBarButtonLayoutByXml.Enabled = (newStyle != DocumentStyle.SystemMdi);
        }

        private void CloseAllDocuments()
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                    form.Close();
            }
            else
            {
                foreach (IDockContent document in dockPanel.DocumentsToArray())
                {
                    // IMPORANT: dispose all panes.
                    document.DockHandler.DockPanel = null;
                    document.DockHandler.Close();
                }
            }
        }

        private void menuItemSolutionExplorer_Click(object sender, EventArgs e)
        {
            DummySolutionExplorer m_solutionExplorer = new DummySolutionExplorer();
            m_solutionExplorer.RightToLeftLayout = RightToLeftLayout;
            m_solutionExplorer.Show(dockPanel);
        }

        private void SetSplashScreen()
        {

            _showSplash = true;
            _splashScreen = new SplashScreen();

            ResizeSplash();
            _splashScreen.Visible = true;
            _splashScreen.TopMost = true;

            Timer _timer = new Timer();
            _timer.Tick += (sender, e) =>
            {
                _splashScreen.Visible = false;
                _timer.Enabled = false;
                _showSplash = false;
            };
            _timer.Interval = 4000;
            _timer.Enabled = true;
        }

        private void ResizeSplash()
        {
            if (_showSplash)
            {

                var centerXMain = (this.Location.X + this.Width) / 2.0;
                var LocationXSplash = Math.Max(0, centerXMain - (_splashScreen.Width / 2.0));

                var centerYMain = (this.Location.Y + this.Height) / 2.0;
                var LocationYSplash = Math.Max(0, centerYMain - (_splashScreen.Height / 2.0));

                _splashScreen.Location = new Point((int)Math.Round(LocationXSplash), (int)Math.Round(LocationYSplash));
            }
        }
    }
}
