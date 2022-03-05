using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DockApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //改变主题
            VS2015LightTheme vs2015 = new VS2015LightTheme();
            dockPanel1.Theme = vs2015;

            this.IsMdiContainer = true; 
             Form2 f2 = new Form2();
            f2.Show(dockPanel1, DockState.DockLeft);
            Form3 f3 = new Form3();
            f3.Show(dockPanel1, DockState.DockRight);
            Form4 f4 = new Form4();
            f4.Show(dockPanel1, DockState.Document);
            Form5 f5 = new Form5();
            f5.Show(dockPanel1, DockState.Document);
        }
    }
}
