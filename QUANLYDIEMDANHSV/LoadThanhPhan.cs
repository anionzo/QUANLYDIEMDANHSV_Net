using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace QUANLYDIEMDANHSV
{
    class LoadThanhPhan
    {
        string con = @"Data Source=.;Initial Catalog=DB_DIEMDANH_SINHVIEN;Integrated Security=True";

        #region Giang Viên
        public void loadGioiTinh(ComboBox combo)
        {
            combo.Items.Add("Nam");
            combo.Items.Add("Nữ");


        }
        public int demGiangVien(string maGV)
        {
            SqlConnection connetion = new SqlConnection(con);

            string dem = string.Format(" select count(*) from GiangVien where MaGV = '{0}'", maGV);
            SqlCommand cmd = new SqlCommand(dem, connetion);
            connetion.Open();
            int kq = (int)cmd.ExecuteScalar();

            connetion.Close();
            return kq;

        }

        public string XoaGV(string maGV)
        {
            string KQ = "Chưa Nhập Mã Giảng Viên";
            SqlConnection connetion = new SqlConnection(con);

            if (maGV != "")
            {
                int d = demGiangVien(maGV);

                string dem = string.Format(@"declare  @kq NVARCHAR(100) exec pr_deleteGV '{0}', @kq output select @kq as'PQ'", maGV);
                SqlCommand cmd = new SqlCommand(dem, connetion);
                connetion.Open();
                KQ = (string)cmd.ExecuteScalar();

                connetion.Close();
            }
            return KQ;
        }
        public string UpdateGV(string maGV, ComboBox cboGT, TextBox ten, TextBox bac, DateTimePicker ns, TextBox nsinh, TextBox cmnd, ComboBox mabn)
        {
            string KQ = "Chưa Nhập Mã Giảng Viên";
            SqlConnection connetion = new SqlConnection(con);

            if (maGV != "")
            {
                string dem = string.Format(@" declare @KQ NVARCHAR(100) 
                                              SET DATEFORMAT DMY EXEC pr_UpdateGV '{0}',N'{1}',N'{2}',N'{3}','{4}',N'{5}','{6}','{7}', @KQ OUTPUT 
                                                SELECT @KQ", maGV, cboGT.Text, ten.Text, bac.Text, ns.Text, nsinh.Text, cmnd.Text, mabn.SelectedValue);
                SqlCommand cmd = new SqlCommand(dem, connetion);
                connetion.Open();
                KQ = (string)cmd.ExecuteScalar();
                connetion.Close();

            }
            return KQ;
        }
        public string InsertGV(TextBox maGV, ComboBox cboGT, TextBox ten, TextBox bac, DateTimePicker ns, TextBox nsinh, TextBox cmnd, ComboBox mabn)
        {
            string KQ = "Chưa Nhập Mã Giảng Viên";
            SqlConnection connetion = new SqlConnection(con);

            if (maGV.Text != "")
            {
                int d = demGiangVien(maGV.Text);
                KQ = "Đã có giảng viên, nhập lại mã giảng viên";
                if (d == 0)
                {
                    string dem = string.Format(@"declare @KQ nvarchar(200) 
                                                SET DATEFORMAT DMY
                                                EXEC pr_InsertGV '{0}', N'{1}',N'{2}',N'{3}','{4}',N'{5}','{6}','{7}', @KQ output
                                                select @KQ as pn", maGV.Text, cboGT.Text, ten.Text, bac.Text, ns.Text, nsinh.Text, cmnd.Text, mabn.SelectedValue);
                    SqlCommand cmd = new SqlCommand(dem, connetion);
                    connetion.Open();
                    KQ = (string)cmd.ExecuteScalar();
                    connetion.Close();
                }
            }
            return KQ;
        }


        public DataTable TimGV(TextBox tim)
        {
            string KQ = "Chưa Nhập Mã Giảng Viên";

            DataTable data = new DataTable();
            if (tim.Text != "")
            {
                string dem = string.Format(@"SELECT  MaGV, GioiTinh,HoTenGV,BacCap,NgaySinh,NoiSinh,CMND,TenBoMon from GiangVien g, BoMon b where g.MaBoMon = b.MaBoMon and g.HoTenGV LIKE N'%{0}%'", tim.Text);
                SqlDataAdapter daAP = new SqlDataAdapter(dem, con);
                daAP.Fill(data);
            }
            return data;
        }
        public DataTable LoadGiangVien()
        {
            string sqlTable = @"select MaGV, GioiTinh,HoTenGV,BacCap,NgaySinh,NoiSinh,CMND,TenBoMon from GiangVien g, BoMon b where g.MaBoMon = b.MaBoMon";
            DataTable data = new DataTable();
            SqlDataAdapter table_Gv = new SqlDataAdapter(sqlTable, con);
            table_Gv.Fill(data);
            // sử dụng datatable để chuyển đổi được các giá trị
            return data;
        }
        public void loadBoMon(ComboBox combo)
        {
            string sqlNgaySinh = @"select * from BoMon";
            DataSet data = new DataSet();
            SqlDataAdapter table_Gv = new SqlDataAdapter(sqlNgaySinh, con);
            table_Gv.Fill(data, "BoMon");
            // sử dụng datatable để chuyển đổi được các giá trị
            combo.DataSource = data.Tables["BoMon"];
            combo.DisplayMember = "TenBoMon";
            combo.ValueMember = "MaBoMon";
        }
        #endregion


        #region LopHoc
        public void loadCBoHoanThanh(ComboBox combo)
        {
            combo.Items.Add("Hoàn thành");
            combo.Items.Add("Chưa hoàn thành");


        }
        public void loadCboGV(ComboBox combo)
        {

            string sqlNgaySinh = @"select * from GiangVien";
            DataSet data = new DataSet();
            SqlDataAdapter table_Gv = new SqlDataAdapter(sqlNgaySinh, con);
            table_Gv.Fill(data, "GiangVien");
            // sử dụng datatable để chuyển đổi được các giá trị
            combo.DataSource = data.Tables["GiangVien"];
            combo.DisplayMember = "HoTenGV";
            combo.ValueMember = "MaGV";

        }
        public DataTable LoadMoLopHP()
        {
            string sqlTable = @"  SELECT MaLopMH,MaHK,TenMH,NgayBatDau,NgayKetThuc,HoanThanh, SoLuongSV,g.HoTenGV FROM MoLopMonHoc m, GiangVien g, MonHoc mh where m.MaGV = g.MaGV and mh.MaMH = m.MaMH";
            DataTable data = new DataTable();
            SqlDataAdapter table_Gv = new SqlDataAdapter(sqlTable, con);
            table_Gv.Fill(data);
            // sử dụng datatable để chuyển đổi được các giá trị
            return data;
        }

        public DataTable LoadMoLopTheoMaGV(ComboBox maGV)
        {
            string sqlTable = string.Format(@" SELECT MaLopMH,MaHK,TenMH,NgayBatDau,NgayKetThuc,HoanThanh, SoLuongSV,g.HoTenGV FROM MoLopMonHoc m, GiangVien g, MonHoc mh where m.MaGV = g.MaGV and mh.MaMH = m.MaMH and m.MaGV = '{0}'", maGV.SelectedValue);
           

            DataTable data = new DataTable();
            SqlDataAdapter table_Gv = new SqlDataAdapter(sqlTable, con);
            table_Gv.Fill(data);
            // sử dụng datatable để chuyển đổi được các giá trị
            return data;
        }
        public DataTable LoadMoLopTheoTenGV(TextBox TenGV)
        {
            string sqlTable = string.Format(@" SELECT MaLopMH,MaHK,TenMH,NgayBatDau,NgayKetThuc,HoanThanh, SoLuongSV,g.HoTenGV FROM MoLopMonHoc m, GiangVien g, MonHoc mh where m.MaGV = g.MaGV and mh.MaMH = m.MaMH and g.HoTenGV LIKE  N'%{0}%'", TenGV.Text);


            DataTable data = new DataTable();
            SqlDataAdapter table_Gv = new SqlDataAdapter(sqlTable, con);
            table_Gv.Fill(data);
            // sử dụng datatable để chuyển đổi được các giá trị
            return data;
        }
        public void LoadMonHoc(ComboBox combo)
        {
            string sqlNgaySinh = @" select TenMH,MaMH from MonHoc";
            DataSet data = new DataSet();
            SqlDataAdapter table_Gv = new SqlDataAdapter(sqlNgaySinh, con);
            table_Gv.Fill(data, "MonHoc");
            // sử dụng datatable để chuyển đổi được các giá trị
            combo.DataSource = data.Tables["MonHoc"];
            combo.DisplayMember = "TenMH";
            combo.ValueMember = "MaMH";
        }

        public string InsertMoLop(TextBox molop, TextBox maHK, DateTimePicker nbd, DateTimePicker nkt, ComboBox hoant, TextBox slsv, ComboBox mondk, ComboBox gv)
        {
            string KQ = "Chưa Nhập lop";
            SqlConnection connetion = new SqlConnection(con);

            if (molop.Text != "")
            {
                string dem = string.Format(@"DECLARE @KQ nvarchar(300)
                                                SET DATEFORMAT DMY
                                                exec pr_InsertMoLopMH '{0}',{1},N'{2}',N'{3}',N'{4}',{5},'{6}','{7}', @KQ output
                                                select @kq", molop.Text, Convert.ToInt32(maHK.Text), nbd.Text, nkt.Text, hoant.Text, Convert.ToInt32(slsv.Text), mondk.SelectedValue, gv.SelectedValue);
                SqlCommand cmd = new SqlCommand(dem, connetion);
                connetion.Open();
                KQ = (string)cmd.ExecuteScalar();
                connetion.Close();

            }
            return KQ;
        }
        public string DeleteMoLop(TextBox molop)
        {
            string KQ = "Chưa Nhập mã lớp";
            SqlConnection connetion = new SqlConnection(con);

            if (molop.Text != "")
            {
                string dem = string.Format(@"declare @KQ nvarchar(20)
                                            exec pr_DeleteMoLopMH '{0}', @kq output
                                            select @KQ", molop.Text);
                SqlCommand cmd = new SqlCommand(dem, connetion);
                connetion.Open();
                KQ = (string)cmd.ExecuteScalar();
                connetion.Close();

            }
            return KQ;
        }
        public string UpdateMoLop(TextBox molop, TextBox maHK, DateTimePicker nbd, DateTimePicker nkt, ComboBox hoant, TextBox slsv, ComboBox mondk, ComboBox gv)
        {
            string KQ = "Chưa Nhập mã lớp";
            SqlConnection connetion = new SqlConnection(con);

            if (molop.Text != "")
            {
                string dem = string.Format(@"declare @KQ  nvarchar(300)
                                                set dateformat dmy
                                                exec pr_UpdateMoLopMH  '{0}',{1}, '{2}', '{3}',N'{4}', {5},'{6}', '{7}' ,@KQ  output
                                                select @KQ", molop.Text, Convert.ToInt32(maHK.Text), nbd.Text, nkt.Text, hoant.Text, Convert.ToInt32(slsv.Text), mondk.SelectedValue, gv.SelectedValue);
                SqlCommand cmd = new SqlCommand(dem, connetion);
                connetion.Open();
                KQ = (string)cmd.ExecuteScalar();
                connetion.Close();
            }
            return KQ;

        }
        #endregion
    }
}
