using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Mail;

namespace QUANLYDIEMDANHSV
{
    class XetDieuKien
    {
        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public bool IsPhoneNumber(string number)
        {
            return Regex.Match(number, @"^(\0[0-9]{9})$").Success;
        }
        public bool IsCMND(string cmnd)
        {
            return Regex.Match(cmnd, @"^([0-9]{10,12})$").Success;
        }
    }
}
