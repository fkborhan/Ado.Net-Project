using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonthlyProject_ADO.Net_
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }
        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
           

        }

        private void Form4_Load(object sender, EventArgs e)
        {
            CrystalReport3 cr2 = new CrystalReport3();
            cr2.SetParameterValue("id", Form1.GetComBo);
            cr2.SetParameterValue("studentid", Form1.GetComBo, cr2.Subreports[0].Name.ToString());
            crystalReportViewer1.ReportSource = cr2;
            crystalReportViewer1.Refresh();
        }
    }
}
