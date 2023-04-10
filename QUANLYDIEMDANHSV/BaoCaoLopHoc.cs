using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QUANLYDIEMDANHSV
{
    public partial class BaoCaoLopHoc : Form
    {
        public BaoCaoLopHoc()
        {
            InitializeComponent();
        }

        private void BaoCaoLopHoc_Load(object sender, EventArgs e)
        {
            CrystalReport rpt = new CrystalReport();
            crystalReportViewer1.ReportSource = rpt;
            crystalReportViewer1.Refresh();
            crystalReportViewer1.DisplayToolbar = false;
            crystalReportViewer1.DisplayStatusBar = false;
        }
    }
}
