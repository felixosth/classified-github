using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform;

namespace InSupport.ProblemSolver
{
    public partial class ProblemSolver2 : Form
    {
        //AcroPDFLib.
        List<string> pdfList = new List<string>();

        int selectedIndex = 99;
        public ProblemSolver2()
        {
            InitializeComponent();

            ClientControl.Instance.RegisterUIControlForAutoTheming(splitContainer1.Panel1);
        }

        private void ProblemSolver2_Load(object sender, EventArgs e)
        {
            pdfList = Directory.GetFiles(Application.StartupPath + @"\MIPPlugins\InSupport\PDFs\").ToList();
            foreach(string file in pdfList)
            {
                listBox1.Items.Add(Path.GetFileNameWithoutExtension(file));
            }
            listBox1.SelectedIndex = 0;

            HideStuff();

        }

        void HideStuff()
        {
            axAcroPDF1.setShowScrollbars(false);
            axAcroPDF1.setShowToolbar(false);
            axAcroPDF1.setPageMode("none");

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedIndex == listBox1.SelectedIndex)
                return;


            HideStuff();

            selectedIndex = listBox1.SelectedIndex;
            axAcroPDF1.LoadFile(pdfList[listBox1.SelectedIndex]);
            axAcroPDF1.setZoom(100);

            HideStuff();
        }
    }
}
