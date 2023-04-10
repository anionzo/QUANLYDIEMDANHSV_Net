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
    public partial class DIEMDANH : Form
    {
        ConnectSqlServer db;
        GiangVien gv;

        /*
        Tài khoản GV : BNgan@hufi.edu.vn ngan2077
        Tài khoản AD : DKhanh@hufi.edu.vn khanh4433
         */

        public DIEMDANH(ConnectSqlServer database, GiangVien giangvien)
        {
            InitializeComponent();
            this.db = database;
            this.gv = giangvien;

            // Đổ dữ liệu vào BOX Giảng Viên
            string query = "select TenBoMon from BoMon where MaBoMon = {0}";
            textBoxTenGV.Text = this.gv.HoTenGV;
            textBoxBomon.Text = (string)db.ExecuteScalar(query, this.gv.MaBoMon);

            // Đổ dữ liệu vào ComboBox Học kỳ
            query = "select MaHK, N'Học kỳ ' + convert(varchar, HK) + N' - năm học ' + convert(varchar, NamHoc) from HocKy";
            fishcbHK.Fill(db.ExecuteReader(query).GetAll());
        }

        // Chọn học kì trên ComboBox -> load dữ liệu các lớp học của học kì đó
        private void fishcbHK_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query = "select MaLopMH from MoLopMonHoc where MaHK = {0} and MaGV = {1}";
            var keys = db.ExecuteReader(query, fishcbHK.SelectedK, gv.MaGV).FirstfColumn();

            query = "select * from dbo.Query_MoLopHocPhan({1}, {0})";
            var values = db.ExecuteReader(query, fishcbHK.SelectedK, gv.MaGV).GetAll();

            fishListViewLH.Fill(values);
            fishListViewLH.FillKeys(keys);
        }

        private void fishListViewLH_SelectedIndexChanged(object sender, EventArgs e)
        {
            string MaLopMH = (string)fishListViewLH.SelectedK;

            // Đổ dữ liệu Mở lớp môn học vào các textBox
            textBoxMaLH.Text = MaLopMH;
            textBoxTenMH.Text = fishListViewLH.SelectedV.SubItems[0].Text;

            string query = "select HoanThanh from MoLopMonHoc where MaLopMH = {0}";
            textBoxTinhTrang.Text = (string)db.ExecuteScalar(query, MaLopMH);

            // Đổ dữ liệu các buổi học của lớp đó vào comboBox
            query = "select distinct TuanThu from DiemDanh where MaLopMH = {0}";
            var keys = db.ExecuteReader(query, MaLopMH).FirstfColumn();
            var values = (from key in keys select key.ToString()).ToList();
            fishComboBuoiHoc.Fill<object>(keys, values);

            // Đổ dữ liệu sinh viên vào ListView Sinh Viên
            query = "select MSSV from DangKyMonHoc where MaLopMH = {0}";
            var keys_sv = db.ExecuteReader(query, MaLopMH).FirstfColumn();

            query = "select SINHVIEN.MSSV, HoTen, GioiTinh from DangKyMonHoc, SINHVIEN where DangKyMonHoc.MSSV = SINHVIEN.MSSV and DangKyMonHoc.MaLopMH = {0}";
            var values_sv = db.ExecuteReader(query, MaLopMH).GetAll();

            fishListViewSV.Fill(values_sv);
            fishListViewSV.FillKeys(keys_sv);
        }

        private void fishListViewSV_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Đẩy dữ liệu sinh viên được chọn vào textBox
            string MSSV = fishListViewSV.SelectedV.SubItems[0].Text;
            textBoxMSSV.Text = MSSV;
            textBoxHoTenSV.Text = fishListViewSV.SelectedV.SubItems[1].Text;

            // Đẩy thông tin điểm danh vào checkBox
            // Nếu chưa điểm danh và chưa có phép -> sinh viên không đi học
            // Hoặc là sinh viên đi học hoặc là sinh viên vắng có phép
            // Điểm danh được tích -> sinh viên có đi học
            // Có phép được tích -> sinh viên vắng có phép
            string MaLopMH = textBoxMaLH.Text;
            int buoithu = (int)fishComboBuoiHoc.SelectedK;

            string query = "select TinhTrangDiHoc, TinhTrangPhep from DiemDanh where MSSV = {0} and MaLopMH = {1} and TuanThu = {2}";
            var values = db.ExecuteReader(query, MSSV, MaLopMH, buoithu).FirstRow();

            checkBoxCoMat.Checked = ((Byte)values[0]).ToString() == "1";
            checkBoxCoPhep.Checked = ((Byte)values[1]).ToString() == "1";
        }

        // checkBox đi học được chọn
        private void checkBoxCoMat_CheckedChanged(object sender, EventArgs e)
        {
            if (fishListViewSV.SelectedIndex < 0)
                return;

            string MSSV = fishListViewSV.SelectedV.SubItems[0].Text;
            string MaLopMH = textBoxMaLH.Text;
            int buoithu = (int)fishComboBuoiHoc.SelectedK;

            // Sinh viên có đi học
            if (checkBoxCoMat.Checked)
                if (checkBoxCoPhep.Checked) // Nếu có phép đang được tích -> xóa tích
                    checkBoxCoPhep.Checked = false;

            int value = checkBoxCoMat.Checked ? 1 : 0;
            string update = "update DiemDanh set TinhTrangDiHoc = {3} where MSSV = {0} and MaLopMH = {1} and TuanThu = {2}";
            db.ExecuteNonQuery(update, MSSV, MaLopMH, buoithu, value);
        }

        // checkBox có phép được chọn
        private void checkBoxCoPhep_CheckedChanged(object sender, EventArgs e)
        {
            if (fishListViewSV.SelectedIndex < 0)
                return;

            string MSSV = fishListViewSV.SelectedV.SubItems[0].Text;
            string MaLopMH = textBoxMaLH.Text;
            int buoithu = (int)fishComboBuoiHoc.SelectedK;

            // Sinh viên có đi học
            if (checkBoxCoPhep.Checked)
                if (checkBoxCoMat.Checked) // Nếu có phép đang được tích -> xóa tích
                    checkBoxCoMat.Checked = false;

            int value = checkBoxCoPhep.Checked ? 1 : 0;
            string update = "update DiemDanh set TinhTrangPhep = {3} where MSSV = {0} and MaLopMH = {1} and TuanThu = {2}";
            db.ExecuteNonQuery(update, MSSV, MaLopMH, buoithu, value);
        }

        private void btnTTTK_Click(object sender, EventArgs e)
        {
            this.Hide();
            ThongTinTaiKhoan tttk = new ThongTinTaiKhoan(db, gv);
            tttk.ShowDialog();
            this.Show();
            
        }

        private void btnBaoCao_Click(object sender, EventArgs e)
        {
            if(fishListViewLH.SelectedIndex < 0)
            {
                MessageBox.Show("Chưa chọn lớp học !", "Thông báo", MessageBoxButtons.OK);
                return;
            }

            this.Visible = false;
            BaoCaoDiemDanh form = new BaoCaoDiemDanh(textBoxMaLH.Text, fishComboBuoiHoc.Items.Count);
            form.ShowDialog();
            this.Visible = true;
        }
    }
}
