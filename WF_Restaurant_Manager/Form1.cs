using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WF_Restaurant_Manager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ckPass_Click(object sender, EventArgs e)
        {
            if (txbPassWord.PasswordChar == '*')
            {
                txbPassWord.PasswordChar = '\0';
            }
            else
            {
                txbPassWord.PasswordChar = '*';
            }

        }
        Modify modify = new Modify();
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txbUserName.Text;
            string pass = txbPassWord.Text;
            if(username.Trim() == "")// .trim() -> cheeck neu chi co dau cach
            {
                MessageBox.Show("Tên tài khoản không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (pass.Trim() == "")// .trim() -> cheeck neu chi co dau cach
            {
                MessageBox.Show("Vui lòng nhập mật khẩu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string query = " SELECT * FROM Account WHERE UserName = '" + username + "' AND PassWord = '" + pass + "' ;";
                if(modify.taikhoan(query).Count != 0)// dem so kq select dc
                {
                    MessageBox.Show("Đăng nhập thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    Form_Menu form_Menu = new Form_Menu();
                    form_Menu.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Tên tài khoản hoặc mật khẩu không chính xác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void linklbQuenMK_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
            ResetPassW frmReset = new ResetPassW();
            frmReset.Show();
            this.Hide();
        }
    }
}
