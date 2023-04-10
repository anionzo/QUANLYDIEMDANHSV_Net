using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUANLYDIEMDANHSV
{
    public class Account
    {
        public string Username { get; set; }
        public string Pass { get; set; }
        public string Quyen { get; set; }
        public string MaGV { get; set; }
    }

    public class GiangVien
    {
        public string MaGV { get; set; }
        public string GioiTinh { get; set; }
        public string HoTenGV { get; set; }
        public string BacCap { get; set; }
        public DateTime NgaySinh { get; set; }
        public string NoiSinh { get; set; }
        public string CMND { get; set; }
        public string MaBoMon { get; set; }
    }

    public class BoMon
    {
        public string MaBoMon { get; set; }
        public string TenBoMon { get; set; }
        public string MaNganh { get; set; }
    }

    public class SinhVien
    {
        public string MSSV { get; set; }
        public string TrangThai { get; set; }
        public string BacDaoTao { get; set; }
        public string HoTen { get; set; }
        public string ChuyenNganh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string NoiSinh { get; set; }
        public string CMND { get; set; }
        public string GioiTinh { get; set; }
    }
}
