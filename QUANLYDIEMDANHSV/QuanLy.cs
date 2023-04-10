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
    public partial class QuanLy : Form
    {
        GiangVien gv;
        ConnectSqlServer db;

        public QuanLy(ConnectSqlServer connect, GiangVien giangvien)
        {
            InitializeComponent();
            this.db = connect;
            this.gv = giangvien;
        }

        private void tabControl_QuanLy_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch (tabControl_QuanLy.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    LoadThanhPhan TP = new LoadThanhPhan();
                    TP.loadGioiTinh(cboGioiTinh_GV);
                    TP.loadBoMon(cboBoMonGV);
                    loadDGV_GiangVien();
                    break;
                case 2:
                    LoadThanhPhan TPMH = new LoadThanhPhan();
                    TPMH.loadCboGV(cbo_MaGV_Lop);
                    TPMH.loadCboGV(cbo_TenMonHoc);
                    TPMH.LoadMonHoc(CboMaMH_Lop);
                    TPMH.loadCBoHoanThanh(cboHoanThanh_Lop);
                    loadDGV_Lop();
                    break;
                case 3:
                    break;

            }
        }
        private void QuanLy_Load(object sender, EventArgs e)
        {
            this.Load_SinhVien();
        }


        #region Lai Thiết Đồng
        void Load_SinhVien()
        {
            // Đổ dữ liệu vào ListView sinh viên
            string query = "select MSSV, HoTen, ChuyenNganh, CMND, TrangThai, GioiTinh from SINHVIEN";
            var values = db.ExecuteReader(query).GetAll();
            var keys = (from key in values select key[0]).ToList();

            fishListViewSV.FillKeys(keys);
            fishListViewSV.Fill(values);

            ThanhPhan.loadGioiTinh(cboGioiTinh);

            
        }

        private void fishListViewSV_SelectedIndexChanged(object sender, EventArgs e)
        {
            string MSSV = (string)fishListViewSV.SelectedK;

            // Đọc dữ liệu sinh viên vừa chọn từ SQL Server
            string query = "select * from SINHVIEN where MSSV = {0}";
            SinhVien sv = db.ExecuteReader(query, MSSV).SingleObject<SinhVien>();

            // Đổ dữ liệu sinh viên vào các textBox
            txtMSSV_SV.Text = sv.MSSV;
            txtHoTen_SV.Text = sv.HoTen;
            dTP_NgaySinh_SV.Value = sv.NgaySinh;
            txtNoiSinh_SV.Text = sv.NoiSinh;
            cboGioiTinh.SelectedIndex = sv.GioiTinh == "Nam" ? 0 : 1;
            txtTrangThai_SV.Text = sv.TrangThai;
            txtChuyenNganh_SV.Text = sv.ChuyenNganh;
            txtBacDaotao_SV.Text = sv.BacDaoTao;
            txt_CMND_SV.Text = sv.CMND;
        }

        // Lấy dữ liệu sinh viên từ các textBox
        SinhVien GetInfoSV()
        {
            SinhVien sv = new SinhVien();

            // Đổ dữ liệu sinh viên vào các textBox
            sv.MSSV = txtMSSV_SV.Text;
            sv.HoTen = txtHoTen_SV.Text;
            sv.NgaySinh = dTP_NgaySinh_SV.Value;
            sv.NoiSinh = txtNoiSinh_SV.Text;
            sv.GioiTinh = (string)cboGioiTinh.SelectedItem;
            sv.TrangThai = txtTrangThai_SV.Text;
            sv.ChuyenNganh = txtChuyenNganh_SV.Text;
            sv.BacDaoTao = txtBacDaotao_SV.Text;
            sv.CMND = txt_CMND_SV.Text;
            return sv;
        }

        private void btn_Them_SV_Click(object sender, EventArgs e)
        {
            SinhVien sv = this.GetInfoSV();

            // Kiểm tra trùng mã số sinh viên
            var keys = fishListViewSV.Keys;
            var list_mssv = (from mssv in keys select mssv.ToString()).ToList();

            if (list_mssv.Contains(sv.MSSV))
            {
                MessageBox.Show("Mã số sinh viên đã tồn tại !", "Thông báo", MessageBoxButtons.OK);
                return;
            }

            // Insert sinh viên vào SQL Server
            //(MSSV, TrangThai, BacDaoTao, HoTen, ChuyenNganh, NgaySinh, NoiSinh, CMND, GioiTinh)
            string query = "insert into SINHVIEN values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})";
            db.ExecuteNonQuery(query, sv.MSSV, sv.TrangThai, sv.BacDaoTao, sv.HoTen, sv.ChuyenNganh,
                sv.NgaySinh.ToString("yyyy/MM/dd"), sv.NoiSinh, sv.CMND, sv.GioiTinh);

            // Thêm sinh viên vào ListView
            string[] val = { sv.MSSV, sv.HoTen, sv.ChuyenNganh, sv.CMND, sv.TrangThai, sv.GioiTinh };
            fishListViewSV.AddValue(sv.MSSV, val);

            MessageBox.Show("Thêm sinh viên thành công !", "Thông báo", MessageBoxButtons.OK);
        }

        private void btnSua_SV_Click(object sender, EventArgs e)
        {
            SinhVien sv = this.GetInfoSV();

            var keys = (from key in fishListViewSV.Keys select key.ToString()).ToList();
            int index = keys.IndexOf(sv.MSSV);

            if (index == -1)
            {
                MessageBox.Show(string.Format("Không tồn tại mã sinh viên {0} !", sv.MSSV), "Thông báo", MessageBoxButtons.OK);
                return;
            }

            // Update sinh viên vào SQL Server
            //(MSSV, TrangThai, BacDaoTao, HoTen, ChuyenNganh, NgaySinh, NoiSinh, CMND, GioiTinh)
            string query = "update SINHVIEN set TrangThai = {0}, BacDaoTao = {1}, HoTen = {2}, ChuyenNganh = {3}, NgaySinh = {4}, NoiSinh = {5}, CMND = {6}, GioiTinh = {7} where MSSV = {8}";
            db.ExecuteNonQuery(query, sv.TrangThai, sv.BacDaoTao, sv.HoTen, sv.ChuyenNganh,
                sv.NgaySinh.ToString("yyyy/MM/dd"), sv.NoiSinh, sv.CMND, sv.GioiTinh, sv.MSSV);

            // Sửa sinh viên trong ListView
            string[] val = { sv.HoTen, sv.ChuyenNganh, sv.CMND, sv.TrangThai, sv.GioiTinh };
            var row = fishListViewSV.Items[index];

            foreach (int i in Fish.range(val.Length))
                row.SubItems[i + 1].Text = val[i];

            MessageBox.Show("Sửa sinh viên thành công !", "Thông báo", MessageBoxButtons.OK);
        }

        private void btn_XoaSV_Click(object sender, EventArgs e)
        {
            // Chưa sinh viên nào được chọn
            if (fishListViewSV.SelectedIndex == -1)
            {
                MessageBox.Show("Chưa chọn sinh viên !", "Thông báo", MessageBoxButtons.OK);
                return;
            }

            string MSSV = (string)fishListViewSV.SelectedK;
            var r = MessageBox.Show(string.Format("Xóa sinh viên {0} !", MSSV), "Thông báo", MessageBoxButtons.YesNo);

            if (r == DialogResult.No)
                return;

            // Xóa sinh viên trên ListView
            fishListViewSV.RemoveSelectedRow();

            // Xóa sinh viên vừa chọn trong SQL Server
            string query = "delete SINHVIEN where MSSV = {0}";
            db.ExecuteNonQuery(query, MSSV);

            MessageBox.Show("Xóa sinh viên thành công !", "Thông báo", MessageBoxButtons.OK);
        }

        #endregion

        //-----------------------
        DataTable ds = new DataTable();
        DataSet ds_GV = new DataSet();
        LoadThanhPhan ThanhPhan = new LoadThanhPhan();
        //clean databingding


        #region Mai Trung Tiến - Quản Lý Giảng Viên

        private bool btnThem_GV_Nhan = false;
        private void KhoaCacNut()
        {
            btnLuu_GV.Enabled = false;
            txtMaGV.Enabled = false;

        }
        private void DGW_QLGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                btnThem_GV_Nhan = false;
                btnLuu_GV.Enabled = false;
            }
            KhoaCacNut();
            DongCacNut();
        }

        public void DongCacNut()
        {
            txtCapBac_GV.Enabled = false;
            txtCMND_GV.Enabled = false;
            txtHoTen_GV.Enabled = false;
            txtNoiSinh_GV.Enabled = false;
            cboGioiTinh_GV.Enabled = false;
            dPK_NgaySinh_GV.Enabled = false;
            txtMaGV.Enabled = false;
            cboBoMonGV.Enabled = false;
        }
        public void MoCacNut()
        {
            txtCapBac_GV.Enabled = true;
            txtCMND_GV.Enabled = true;
            txtHoTen_GV.Enabled = true;
            txtNoiSinh_GV.Enabled = true;
            cboGioiTinh_GV.Enabled = true;
            dPK_NgaySinh_GV.Enabled = true;
            txtMaGV.Enabled = true;
            cboBoMonGV.Enabled = true;

        }
        public void LoadBingDinhGV(DataTable data)
        {
            txtCapBac_GV.DataBindings.Clear();
            txtCMND_GV.DataBindings.Clear();
            txtHoTen_GV.DataBindings.Clear();
            txtNoiSinh_GV.DataBindings.Clear();
            cboGioiTinh_GV.DataBindings.Clear();
            dPK_NgaySinh_GV.DataBindings.Clear();
            txtMaGV.DataBindings.Clear();
            cboBoMonGV.DataBindings.Clear();

            txtMaGV.DataBindings.Add("Text", data, "MaGV", true, DataSourceUpdateMode.Never);
            txtCapBac_GV.DataBindings.Add("Text", data, "BacCap", true, DataSourceUpdateMode.Never);
            txtHoTen_GV.DataBindings.Add("Text", data, "HoTenGV", true, DataSourceUpdateMode.Never);
            txtCMND_GV.DataBindings.Add("Text", data, "CMND", true, DataSourceUpdateMode.Never);
            txtNoiSinh_GV.DataBindings.Add("Text", data, "NoiSinh", true, DataSourceUpdateMode.Never);
            cboGioiTinh_GV.DataBindings.Add("Text", data, "GioiTinh", true, DataSourceUpdateMode.Never);
            dPK_NgaySinh_GV.DataBindings.Add("Text", data, "NgaySinh", true, DataSourceUpdateMode.Never);
            cboBoMonGV.DataBindings.Add("Text", data, "TenBoMon", true, DataSourceUpdateMode.Never);
        }
        //-----------------------
        public void LoadDGV_GV()
        {
            DGW_QLGV.Columns.Clear();
            DGW_QLGV.Columns.Add("MaGV", "Mã GV");
            DGW_QLGV.Columns[0].DataPropertyName = "MaGV";
            DGW_QLGV.Columns.Add("HoTenGV", "Họ Tên");
            DGW_QLGV.Columns[1].DataPropertyName = "HoTenGV";
            DGW_QLGV.Columns.Add("GioiTinh", "Giới Tính");
            DGW_QLGV.Columns[2].DataPropertyName = "GioiTinh";
            DGW_QLGV.Columns.Add("BacCap", "Bậc Cấp");
            DGW_QLGV.Columns[3].DataPropertyName = "BacCap";
            DGW_QLGV.Columns.Add("NgaySinh", "Ngày Sinh");
            DGW_QLGV.Columns[4].DataPropertyName = "NgaySinh";
            DGW_QLGV.Columns.Add("TenBoMon", "Bộ Môn");
            DGW_QLGV.Columns[5].DataPropertyName = "TenBoMon";
            DGW_QLGV.Columns.Add("CMND", "CMND");
            DGW_QLGV.Columns[6].DataPropertyName = "CMND";
        }
        //-----------------------
        public void loadDGV_GiangVien()
        {
            KhoaCacNut();
            DongCacNut();
            LoadDGV_GV();

            //DataSet dataset = new DataSet();
            DataTable data = new DataTable();
            data = ThanhPhan.LoadGiangVien();
            DGW_QLGV.DataSource = data;
            // thêm data binding
            LoadBingDinhGV(data);

        }

        private void btnThem_GV_Click(object sender, EventArgs e)
        {
            btnThem_GV_Nhan = true;
            btnLuu_GV.Enabled = true;
            MoCacNut();
            txtCapBac_GV.Clear();
            txtCMND_GV.Clear();
            txtHoTen_GV.Clear();
            txtNoiSinh_GV.Clear();
            txtMaGV.Clear();
        }
        private void btnSua_GV_Click(object sender, EventArgs e)
        {
            btnThem_GV_Nhan = false;
            btnLuu_GV.Enabled = true;
            MoCacNut();
            txtMaGV.Enabled = false;
        }

        private void btnXoa_GV_Click(object sender, EventArgs e)
        {
            string tb = "";
            int XetGV = ThanhPhan.demGiangVien(txtMaGV.Text);
            if (XetGV == 0)
            {
                MessageBox.Show("Không có giảng viên xóa !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            DialogResult r = MessageBox.Show("Bạn có chắc muốn xóa giảng viên: " + txtHoTen_GV.Text + " ?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
            if (r == DialogResult.Yes)
            {
                tb = ThanhPhan.XoaGV(txtMaGV.Text);
            }
            if (tb == "Xóa giảng viên thành công!")
            {
                loadDGV_GiangVien();
            }
            MessageBox.Show(tb, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Question);

        }

        private void btnLuu_GV_Click(object sender, EventArgs e)
        {
            XetDieuKien dk = new XetDieuKien();
            if (string.IsNullOrEmpty(txtMaGV.Text))
            {
                MessageBox.Show("Chưa nhập Mã GV!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            if (string.IsNullOrEmpty(txtHoTen_GV.Text))
            {
                MessageBox.Show("Chưa họ tên GV!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            if (string.IsNullOrEmpty(txtCapBac_GV.Text))
            {
                MessageBox.Show("Chưa nhập cấp bặc GV!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            if (string.IsNullOrEmpty(txtNoiSinh_GV.Text))
            {
                MessageBox.Show("Chưa nhập nơi sinh GV!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            if (string.IsNullOrEmpty(txtCMND_GV.Text))
            {
                MessageBox.Show("Chưa nhập CMND GV!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            bool kq = dk.IsCMND(txtCMND_GV.Text);
            if (kq == false)
            {
                MessageBox.Show("CMND chưa đúng định dạng!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            DateTimePicker dt = new DateTimePicker();
            if ((dt.Value.Year - dPK_NgaySinh_GV.Value.Year) <= 18)
            {
                MessageBox.Show("Chưa đủ tuổi phải trên 18 !!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (btnThem_GV_Nhan == true)
            {
                DialogResult r = MessageBox.Show("Bạn có chắc muốn thêm giảng viên: " + txtHoTen_GV.Text + " ?", "Câu Hỏi", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                if (r == DialogResult.Yes)
                {
                    string tb = ThanhPhan.InsertGV(txtMaGV, cboGioiTinh_GV, txtHoTen_GV, txtCapBac_GV, dTP_NgaySinh_SV, txtNoiSinh_GV, txtCMND_GV, cboBoMonGV);
                    MessageBox.Show(tb, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    loadDGV_GiangVien();
                }
            }
            else
            {
                DialogResult r = MessageBox.Show("Bạn có chắc muốn sửa giảng viên: " + txtHoTen_GV.Text + " ?", "Câu Hỏi", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                if (r == DialogResult.Yes)
                {
                    string tb = ThanhPhan.UpdateGV(txtMaGV.Text, cboGioiTinh_GV, txtHoTen_GV, txtCapBac_GV, dTP_NgaySinh_SV, txtNoiSinh_GV, txtCMND_GV, cboBoMonGV);
                    MessageBox.Show(tb, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    loadDGV_GiangVien();
                }
            }

        }

        private void btnTimKiemGV_Click(object sender, EventArgs e)
        {
            LoadDGV_GV();
            DataTable data = new DataTable();
            data = ThanhPhan.TimGV(txtTimKiem_GV);

            DGW_QLGV.DataSource = data;
            // thêm data binding
            LoadBingDinhGV(data);

        }

        private void button1_Click(object sender, EventArgs e)
        { //    // Load lại Bang gv
            loadDGV_GiangVien();

        }

        //----------- GL lớp học

        DataTable ds_MoLop = new DataTable();
        DataSet ds_ML = new DataSet();
        public void DongCacNutMH()
        {
            txtMaLopMH_Lop.Enabled = false;
            txtHocKi_Lop.Enabled = false;
            cboHoanThanh_Lop.Enabled = false;
            txtSoLuongSV_Lop.Enabled = false;
            CboMaMH_Lop.Enabled = false;
            dTPBatDau_Lop.Enabled = false;
            dTP_KetThuc_Lop.Enabled = false;
            cbo_MaGV_Lop.Enabled = false;
        }
        public void MoCacNutMH()
        {
            txtMaLopMH_Lop.Enabled = true;
            txtHocKi_Lop.Enabled = true;
            cboHoanThanh_Lop.Enabled = true;
            txtSoLuongSV_Lop.Enabled = true;
            CboMaMH_Lop.Enabled = true;
            dTPBatDau_Lop.Enabled = true;
            dTP_KetThuc_Lop.Enabled = true;
            cbo_MaGV_Lop.Enabled = true;
        }
        public void LoadBingDinhMH(DataTable data)
        {
            txtMaLopMH_Lop.DataBindings.Clear();
            txtHocKi_Lop.DataBindings.Clear();
            cboHoanThanh_Lop.DataBindings.Clear();
            txtSoLuongSV_Lop.DataBindings.Clear();
            CboMaMH_Lop.DataBindings.Clear();
            dTPBatDau_Lop.DataBindings.Clear();
            dTP_KetThuc_Lop.DataBindings.Clear();
            cbo_MaGV_Lop.DataBindings.Clear();

            txtMaLopMH_Lop.DataBindings.Add("Text", data, "MaLopMH", true, DataSourceUpdateMode.Never);
            txtHocKi_Lop.DataBindings.Add("Text", data, "MaHK", true, DataSourceUpdateMode.Never);
            cboHoanThanh_Lop.DataBindings.Add("Text", data, "HoanThanh", true, DataSourceUpdateMode.Never);
            txtSoLuongSV_Lop.DataBindings.Add("Text", data, "SoLuongSV", true, DataSourceUpdateMode.Never);
            CboMaMH_Lop.DataBindings.Add("Text", data, "TenMH", true, DataSourceUpdateMode.Never);
            dTPBatDau_Lop.DataBindings.Add("Text", data, "NgayBatDau", true, DataSourceUpdateMode.Never);
            dTP_KetThuc_Lop.DataBindings.Add("Text", data, "NgayKetThuc", true, DataSourceUpdateMode.Never);
            cbo_MaGV_Lop.DataBindings.Add("Text", data, "HoTenGV", true, DataSourceUpdateMode.Never);
        }


        public void LoadDGV_MH()
        {
            dataGridViewLop.Columns.Clear();
            dataGridViewLop.Columns.Add("MaLopMH", "Mã Lớp");
            dataGridViewLop.Columns[0].DataPropertyName = "MaLopMH";
            dataGridViewLop.Columns.Add("MaHK", "Mã Học Kì");
            dataGridViewLop.Columns[1].DataPropertyName = "MaHK";
            dataGridViewLop.Columns.Add("HoanThanh", "Hoàn Thành");
            dataGridViewLop.Columns[2].DataPropertyName = "HoanThanh";
            dataGridViewLop.Columns.Add("NgayBatDau", "Ngày Bắt Đầu");
            dataGridViewLop.Columns[3].DataPropertyName = "NgayBatDau";
            dataGridViewLop.Columns.Add("NgayKetThuc", "Ngày Kết Thúc");
            dataGridViewLop.Columns[4].DataPropertyName = "NgayKetThuc";
            dataGridViewLop.Columns.Add("SoLuongSV", "Sĩ Số");
            dataGridViewLop.Columns[5].DataPropertyName = "SoLuongSV";
            dataGridViewLop.Columns.Add("HoTenGV", "Họ Tên GV");
            dataGridViewLop.Columns[6].DataPropertyName = "HoTenGV";
            dataGridViewLop.Columns.Add("TenMH", "Tên MH");
            dataGridViewLop.Columns[7].DataPropertyName = "TenMH";
        }
        private bool btnThem_MH_Nhan = false;
        private void KhoaCacNutMH()
        {
            btn_Luu_LOP.Enabled = false;
            txtMaLopMH_Lop.Enabled = false;


        }
        public void loadDGV_Lop()
        {
            KhoaCacNutMH();
            DongCacNutMH();
            //----
            LoadDGV_MH();
            //----

            //DataSet dataset = new DataSet();
            DataTable data = new DataTable();
            data = ThanhPhan.LoadMoLopHP();
            dataGridViewLop.DataSource = data;
            // thêm data binding
            LoadBingDinhMH(data);
        }

        private void dataGridViewLop_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                btnThem_MH_Nhan = false;
                btn_Luu_LOP.Enabled = false;
            }
            KhoaCacNutMH();
            DongCacNutMH();
        }

        private void btnThem_Lop_Click(object sender, EventArgs e)
        {
            btnThem_MH_Nhan = true;
            btn_Luu_LOP.Enabled = true;
            MoCacNutMH();
            txtMaLopMH_Lop.Clear();
            txtHocKi_Lop.Clear();
            txtSoLuongSV_Lop.Clear();
        }

        private void btnSua_Lop_Click(object sender, EventArgs e)
        {
            btnThem_MH_Nhan = false;
            btn_Luu_LOP.Enabled = true;
            MoCacNutMH();
            txtMaLopMH_Lop.Enabled = false;

        }

        private void btn_Xoa_Lop_Click(object sender, EventArgs e)
        {
            string tb = "";
            try
            {
                DialogResult r = MessageBox.Show("Bạn có chắc muốn xóa giảng viên: " + txtHoTen_GV.Text + " ?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                if (r == DialogResult.Yes)
                {
                    tb = ThanhPhan.DeleteMoLop(txtMaLopMH_Lop);
                }
                if (tb == "Xóa lớp thành công!")
                {
                    loadDGV_Lop();
                }
                MessageBox.Show(tb, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            catch
            {
                MessageBox.Show("Lỗi không thể thực hiện", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }

        private void btn_Luu_LOP_Click(object sender, EventArgs e)
        {
            XetDieuKien dk = new XetDieuKien();

            if (string.IsNullOrEmpty(txtMaLopMH_Lop.Text))
            {
                MessageBox.Show("Chưa nhập Mã Lớp!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }


            if (string.IsNullOrEmpty(txtHocKi_Lop.Text))
            {
                MessageBox.Show("Chưa nhập học kì!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }


            if (string.IsNullOrEmpty(txtSoLuongSV_Lop.Text))
            {
                MessageBox.Show("Chưa nhập số lượng sinh viên được đăng ký!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            if (btnThem_MH_Nhan == true)
            {
                DialogResult r = MessageBox.Show("Bạn có chắc muốn thêm lớp ?: " + txtMaLopMH_Lop.Text + " ?", "Câu Hỏi", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                if (r == DialogResult.Yes)
                {
                    try
                    {
                        string tb = ThanhPhan.InsertMoLop(txtMaLopMH_Lop, txtHocKi_Lop, dTPBatDau_Lop, dTP_KetThuc_Lop, cboHoanThanh_Lop, txtSoLuongSV_Lop, CboMaMH_Lop, cbo_MaGV_Lop);
                        MessageBox.Show(tb, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        loadDGV_Lop();
                    }
                    catch
                    {
                        MessageBox.Show("Không thể thêm", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        loadDGV_Lop();
                    }
                }
            }
            else
            {
                DialogResult r = MessageBox.Show("Bạn có chắc muốn sửa lớp : " + txtMaLopMH_Lop.Text + " ?", "Câu Hỏi", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                if (r == DialogResult.Yes)
                {
                    try
                    {

                        string tb = ThanhPhan.UpdateMoLop(txtMaLopMH_Lop, txtHocKi_Lop, dTPBatDau_Lop, dTP_KetThuc_Lop, cboHoanThanh_Lop, txtSoLuongSV_Lop, CboMaMH_Lop, cbo_MaGV_Lop);
                        MessageBox.Show(tb, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        loadDGV_Lop();
                    }
                    catch
                    {
                        MessageBox.Show("Không thể sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        loadDGV_Lop();
                    }
                }
            }
        }

        private void BtnMoLop_LoadLai_Click(object sender, EventArgs e)
        {
            loadDGV_Lop();
        }

        private void inBaoCao_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.Hide();
            BaoCaoLopHoc form = new BaoCaoLopHoc();
            form.ShowDialog();
            this.Visible = true;
            this.Show();

        }

        private void btn_tim_Click(object sender, EventArgs e)
        {
            KhoaCacNutMH();
            DongCacNutMH();
            MoCacNutMH();
            //----
            LoadDGV_MH();
            //----
            //DataSet dataset = new DataSet();
            DataTable data = new DataTable();
            //data = ThanhPhan.LoadMoLopTheoMaGV(cbo_TenMonHoc);
            data = ThanhPhan.LoadMoLopTheoTenGV(textLopTimKiem);
            dataGridViewLop.DataSource = data;
            // thêm data binding
            LoadBingDinhMH(data);
        }
        #endregion

    }
}
