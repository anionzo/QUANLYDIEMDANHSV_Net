using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

namespace QUANLYDIEMDANHSV
{
    public partial class BaoCaoDiemDanh : Form
    {
        string MaLopMH;
        int SoBuoi;
        public BaoCaoDiemDanh(string MaLopMH, int SoBuoi)
        {
            InitializeComponent();
            this.MaLopMH = MaLopMH;
            this.SoBuoi = SoBuoi;
        }

        private void BaoCaoDiemDanh_Load(object sender, EventArgs e)
        {
            CrystalReportDiemDanh rpt = new CrystalReportDiemDanh();

            ParameterValues param_MaLopMH = new ParameterValues();
            ParameterDiscreteValue pardis_MaLopMH = new ParameterDiscreteValue();
            pardis_MaLopMH.Value = this.MaLopMH;
            param_MaLopMH.Add(pardis_MaLopMH);


            ParameterValues param_SoBuoi = new ParameterValues();
            ParameterDiscreteValue pardis_SoBuoi = new ParameterDiscreteValue();
            pardis_SoBuoi.Value = this.SoBuoi;
            param_SoBuoi.Add(pardis_SoBuoi);

            rpt.DataDefinition.ParameterFields[0].ApplyCurrentValues(param_MaLopMH);
            rpt.DataDefinition.ParameterFields[1].ApplyCurrentValues(param_SoBuoi);

            crystalReportViewer1.ReportSource = rpt;
            crystalReportViewer1.Refresh();
            crystalReportViewer1.DisplayToolbar = false;
            crystalReportViewer1.DisplayStatusBar = false;
        }
    }
}
