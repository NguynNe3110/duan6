using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace WF_Restaurant_Manager
{
    internal class ConnectDB
    {
        private static string connection = "Data Source=DESKTOP-U6PP8D6;Initial Catalog=CSDL_QLNH;Integrated Security=True";
        //trả về một đối tượng SqlConnection
        //Khi một phần khác của ứng dụng cần kết nối với cơ sở dữ liệu, họ chỉ cần gọi:
        ///SqlConnection connection = ConnectDB.GetSqlConnection();

        public static SqlConnection GetSqlConnection()
        {
            return new SqlConnection(connection);
        }
    }
}
