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
    public partial class DangNhap : Form
    {
        ConnectSqlServer db;

        public DangNhap()
        {
            InitializeComponent();
            db = new ConnectSqlServer(new ConnectString(@".",
                                        "DB_DIEMDANH_SINHVIEN"));
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Bạn có chắc chắn muốn thoát không ?", "Thông báo", MessageBoxButtons.YesNo);

            if(r == DialogResult.Yes)
                this.Close();
        }

        // BNgan@hufi.edu.vn ngan2077
        // DKhanh@hufi.edu.vn khanh4433
        private void btnLogin_Click(object sender, EventArgs e)
        {
            //textBoxUsername.Text = "BNgan@hufi.edu.vn";
            //textBoxPassword.Text = "ngan2077";

            textBoxUsername.Text = "DKhanh@hufi.edu.vn";
            textBoxPassword.Text = "khanh4433";

            // Kiểm tra đã nhập Username chưa
            if (textBoxUsername.Text == string.Empty)
            {
                errorProvider.SetError(textBoxUsername, "Không được phép để trống");
                return;
            }
            // Kiểm tra đã nhập Password chưa
            if (textBoxPassword.Text == string.Empty)
            {
                errorProvider.SetError(textBoxPassword, "Không được phép để trống");
                return;
            }

            // Truy vấn tài khoản
            string query = "select * from TaiKhoan where Username = {0} and Pass = {1}";
            Account acc = db.ExecuteReader(query, textBoxUsername.Text, textBoxPassword.Text).SingleObject<Account>();

            // Kiểm tra tài khoản hợp lệ
            if(acc == null)
            {
                errorProvider.SetError(btnLogin, "Tên đăng nhập hoặc mật khẩu không hợp lệ !");
                return;
            }

            // Truy vấn giảng viên
            query = "select * from GiangVien where MaGV = {0}";
            GiangVien gv = db.ExecuteReader(query, acc.MaGV).SingleObject<GiangVien>();

            string text = string.Format("Đăng nhập thành công : {0} !", gv.HoTenGV);
            MessageBox.Show(text, "Thông báo", MessageBoxButtons.OK);

            Form form;
            // Kiểm tra tài khoản admin
            if (acc.Quyen == "AD")
                form = new QuanLy(db, gv);
            else
                form = new DIEMDANH(db, gv);

            this.Hide();
            form.ShowDialog();
            this.Show();
            
        }
    } 
}
