using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF_Restaurant_Manager
{
    class TaiKhoan
    {
        private string username;
        private string password;
        private string displayname;
        private string email;

        public TaiKhoan()
        {
        }

        public TaiKhoan(string username, string password, string displayname, string email)
        {
            this.username = username;
            this.password = password;
            this.displayname = displayname;
            this.email = email;
        }

        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string Displayname { get => displayname; set => displayname = value; }
        public string Email { get => email; set => email = value; }
    }
}
