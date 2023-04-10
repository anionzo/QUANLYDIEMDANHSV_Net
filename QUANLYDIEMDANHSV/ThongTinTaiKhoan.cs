using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace QUANLYDIEMDANHSV
{
    public partial class ThongTinTaiKhoan : Form
    {
        GiangVien gv;
        ConnectSqlServer db;

        public ThongTinTaiKhoan(ConnectSqlServer connect, GiangVien gv)
        {
            InitializeComponent();
            this.db = connect;
            this.gv = gv;

            // Đổ dữ liệu vào comboBox giới tính
            comboBoxGT.Items.Add("Nam");
            comboBoxGT.Items.Add("Nữ");

            // Đổ dữ liệu giảng viên vào các textBox
            txtMaGV.Text = gv.MaGV;
            txtHoTenGV.Text = gv.HoTenGV;
            comboBoxGT.SelectedIndex = gv.GioiTinh == "Nam" ? 0 : 1;
            txtCMND.Text = gv.CMND;
            dTP_NgaySinh.Value = gv.NgaySinh;
            txtNoiSinhGV.Text = gv.NoiSinh;

            txtBoMon.Text = (string)db.ExecuteScalar("select TenBoMon from BoMon where MaBoMon = {0}", gv.MaBoMon);
        }

        private void btnCapNhatTK_Click(object sender, EventArgs e)
        {
            // Gán lại dữ liệu cho biến gv
            gv.HoTenGV = txtHoTenGV.Text;
            gv.GioiTinh = (string)comboBoxGT.SelectedItem;
            gv.CMND = txtCMND.Text;
            gv.NgaySinh = dTP_NgaySinh.Value;
            gv.NoiSinh = txtNoiSinhGV.Text;

            // Cập nhật dữ liệu giảng viên trong SQL Server
            string query = "update GiangVien set HoTenGV = {0}, GioiTinh = {1}, CMND = {2}, NgaySinh = {3}, NoiSinh = {4} where MaGV = {5}";
            db.ExecuteNonQuery(query, gv.HoTenGV, gv.GioiTinh, gv.CMND,
                gv.NgaySinh.ToString("yyyy/MM/dd"), gv.NoiSinh, gv.MaGV);

            MessageBox.Show("Cập nhật giảng viên thành công !", "Thông báo", MessageBoxButtons.OK);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
