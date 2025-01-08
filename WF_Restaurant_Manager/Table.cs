using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF_Restaurant_Manager
{
    internal class Table
    {

        public int IDTable { get; set; }
        public string NameTable { get; set; }
        public int Status { get; set; } // 0: Bàn trống, 1: Đang phục vụ

        public List<Table> GetAllTables()
        {
            List<Table> tables = new List<Table>();
            string query = "SELECT IDTable, NameTable, Status FROM [Table]";

            using (SqlConnection connection = ConnectDB.GetSqlConnection()) // Kết nối cơ sở dữ liệu
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Table table = new Table
                        {
                            IDTable = reader.GetInt32(0), // Lấy giá trị từ cột IDTable
                            NameTable = reader.GetString(1), // Lấy giá trị từ cột NameTable
                            Status = reader.GetInt32(2) // Lấy giá trị từ cột Status
                        };

                        tables.Add(table);
                    }
                }
            }

            return tables;
        }

        public DataTable GetOrderDetailsByTable(int tableID)
        {
            string query = @"SELECT f.Name AS [Tên món], od.Quantity AS [Số lượng], 
                        f.Price AS [Giá tiền], (od.Quantity * f.Price) AS [Thành tiền]
                     FROM OrderDetail od
                     JOIN Food f ON od.IDFood = f.IDFood
                     WHERE od.IDOrder = (SELECT IDOrder FROM [Order] WHERE IDTable = @tableID AND Status = 0)";
            using (SqlConnection connection = ConnectDB.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@tableID", tableID);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
        }

        public Table GetTableInfo(int tableID)
        {
            string query = "SELECT * FROM [Table] WHERE IDTable = @tableID";
            using (SqlConnection connection = ConnectDB.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@tableID", tableID);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Table
                        {
                            IDTable = (int)reader["IDTable"],
                            NameTable = reader["NameTable"].ToString(),
                            Status = (int)reader["Status"]
                        };
                    }
                }
            }
            return null;
        }

        //private void LoadOrderDetails(int tableID)
        //{
        //    // Xóa dữ liệu cũ trong DataGridView
        //    dataGridViewOrderDetails.Rows.Clear();

        //    // Kết nối cơ sở dữ liệu
        //    string connectionString = "your_connection_string_here"; // Chuỗi kết nối đến database
        //    string query = "SELECT DishName, Quantity, Price FROM Orders WHERE TableID = @TableID";

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@TableID", tableID);
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    string dishName = reader["DishName"].ToString();
        //                    int quantity = int.Parse(reader["Quantity"].ToString());
        //                    decimal price = decimal.Parse(reader["Price"].ToString());

        //                    // Thêm vào DataGridView
        //                    dataGridViewOrderDetails.Rows.Add(dishName, quantity, price);
        //                }
        //            }
        //        }
        //    }
        //}



    }
}
