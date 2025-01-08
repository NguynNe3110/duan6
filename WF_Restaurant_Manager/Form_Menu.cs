using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WF_Restaurant_Manager
{
    public partial class Form_Menu : Form
    {
        Modify modify = new Modify();
        Table table = new Table();
        public int currentTableID = -1; // ID của bàn đang được chọn
        public decimal totalAmount = 0; // Tổng tiền tạm thời
        public Button selectedTableButton;
        SqlCommand SqlCommand;
        Dictionary<int, Button> tableButtons = new Dictionary<int, Button>();

        public Form_Menu()
        {
            InitializeComponent();
        }
        private Form currentFormChild;
        private void OpenChildForm(Form ChildForm)
        {
            if (currentFormChild != null)
            {
                currentFormChild.Close();
            }
            currentFormChild = ChildForm;
            ChildForm.TopLevel = false;
            ChildForm.FormBorderStyle = FormBorderStyle.None;
            ChildForm.Dock = DockStyle.Fill;
            panelBody.Controls.Add(ChildForm);
            panelBody.Tag = ChildForm;
            ChildForm.BringToFront();
            ChildForm.Show();
        }
        private void mn_Home_Click(object sender, EventArgs e)
        {
            if (currentFormChild != null)
            {
                currentFormChild.Close();
            }
            btn_Themmon.Enabled = true;
            btn_Xoamon.Enabled = true;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (currentFormChild != null)
            {
                currentFormChild.Close();
            }
            btn_Themmon.Enabled = true;
            btn_Xoamon.Enabled = true;
        }
        private void mns1_ThongtinTK_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Form_AccountI_nfo());
            btn_Themmon.Enabled = false;
            btn_Xoamon.Enabled = false;
        }
        private void mns1_Dangky_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Form_Dangky());
            btn_Themmon.Enabled = false;
            btn_Xoamon.Enabled = false;
        }
        private void mns1_Dangxuat_Click(object sender, EventArgs e)
        {
            //thực hiện đăng xuất ra ngoài màn hình form đăng nhập
            Form1 form = new Form1();
            this.Hide();
            form.Show();
            //this.Close();
        }
        private void mn_Maytinh_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Form_Maytinh());
            btn_Themmon.Enabled = false;
            btn_Xoamon.Enabled = false;
        }
        private void mn_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void Form_Menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn muốn thoát chương trình ?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
        }
        //thong tin món được lấy ra dựa vào TableID.
        private void LoadOrderDetails(int tableID)
        {
            // Xóa dữ liệu cũ trong DataGridView
            dtgvHome.Rows.Clear();

            // Kết nối cơ sở dữ liệu
            string query = @"
        SELECT 
            f.NameFood AS DishName, 
            od.Quantity, 
            f.Price
        FROM 
            [Table] t
            JOIN [Order] o ON t.IDTable = o.IDTable
            JOIN OrderDetail od ON o.IDOrder = od.IDOrder
            JOIN Food f ON od.IDFood = f.IDFood
        WHERE 
            t.IDTable = @TableID";

            using (SqlConnection connection = ConnectDB.GetSqlConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableID", tableID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string dishName = reader["DishName"].ToString();
                            int quantity = int.Parse(reader["Quantity"].ToString());
                            decimal price = decimal.Parse(reader["Price"].ToString());

                            // Thêm vào DataGridView
                            dtgvHome.Rows.Add(dishName, quantity, price);
                        }
                    }
                }
            }
        }


        //csdl -> combobox loại , món.
        public void cboLoai()
        {
            cbo_TenLoai.DataSource = modify.getTable("select * from Category");
            cbo_TenLoai.DisplayMember = "CategoryName";
            cbo_TenLoai.ValueMember = "IDCategory";

            cbo_TenLoai.SelectedIndexChanged += (sender, e) =>
            {
                // Lấy categoryID từ cbo_TenLoai
                int categoryID = (int)cbo_TenLoai.SelectedValue;

                // Gán dữ liệu cho cbo_TenMon
                cbo_TenMon.DataSource = modify.getTenMon(categoryID);
                cbo_TenMon.DisplayMember = "NameFood";
                cbo_TenMon.ValueMember = "IDFood";
            };

            // Khởi tạo giá trị mặc định cho cbo_TenMon
            if (cbo_TenLoai.SelectedValue != null)
            {
                int categoryID = (int)cbo_TenLoai.SelectedValue;
                cbo_TenMon.DataSource = modify.getTenMon(categoryID);
                cbo_TenMon.DisplayMember = "NameFood";
                cbo_TenMon.ValueMember = "IDFood";
            }

        }

        //form load
        private void Form_Menu_Load(object sender, EventArgs e)
        {
            cboLoai();
        }

        //Nếu muốn cập nhật danh sách bàn
        private void LoadTables()
        {
            var tables = table.GetAllTables(); // Hàm lấy danh sách tất cả các bàn từ cơ sở dữ liệu

            foreach (var table in tables)
            {
                Button tableButton = new Button
                {
                    Text = table.NameTable,
                    Tag = table.IDTable, // Lưu ID bàn trong thuộc tính Tag

                    BackColor = table.Status == 1 ? Color.LightGreen : Color.LightGray // Màu sắc theo trạng thái bàn
                };

                tableButton.Click += Table_Click; // Gắn sự kiện click
                flowLayoutPanel1.Controls.Add(tableButton);
            }
        }

        //event click table.
        private void Table_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            currentTableID = (int)clickedButton.Tag;

            // Đổi màu bàn để đánh dấu
            ResetTableColors();
            clickedButton.BackColor = Color.LightGreen;

            // Hiển thị món đã gọi lên DataGridView
            LoadOrderDetails(currentTableID);
        }

        // Hàm reset màu các bàn
        private void ResetTableColors()
        {
            foreach (Button button in flowLayoutPanel1.Controls)
            {
                if (button.Tag != null)
                {
                    button.BackColor = Color.LightGray; // Màu mặc định
                }
            }
        }


        //Hàm thêm món ăn vào OrderDetail:
        private void AddFoodToOrder(int orderID, int foodID, int quantity)
        {
            string query = @"
        IF EXISTS (SELECT 1 FROM OrderDetail WHERE IDOrder = @orderID AND IDFood = @foodID)
        BEGIN
            UPDATE OrderDetail
            SET Quantity = Quantity + @quantity
            WHERE IDOrder = @orderID AND IDFood = @foodID
        END
        ELSE
        BEGIN
            INSERT INTO OrderDetail (IDOrder, IDFood, Quantity)
            VALUES (@orderID, @foodID, @quantity)
        END";
            using (SqlConnection connection = ConnectDB.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@orderID", orderID);
                command.Parameters.AddWithValue("@foodID", foodID);
                command.Parameters.AddWithValue("@quantity", quantity);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        //Hàm lấy OrderID của bàn:
        private int GetOrderIDByTable(int tableID)
        {
            string query = @"
        SELECT IDOrder 
        FROM [Order] 
        WHERE IDTable = @tableID 
          AND IDOrder IN (SELECT IDOrder FROM [Order] WHERE IDTable = @tableID AND IDOrder IS NOT NULL)";

            using (SqlConnection connection = ConnectDB.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@tableID", tableID);

                connection.Open();
                var result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }



        //Hàm tạo Order mới:
        private int CreateNewOrder(int tableID)
        {
            string query = @"
        INSERT INTO [Order] (IDTable) 
        OUTPUT INSERTED.IDOrder 
        VALUES (@tableID)";

            using (SqlConnection connection = ConnectDB.GetSqlConnection())
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@tableID", tableID);

                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }


        private void btnThemMon_Click(object sender, EventArgs e)
        {
            //if (selectedTableButton == null)
            //{
            //    MessageBox.Show("Không tìm thấy bàn đã chọn!" +MessageBox.Show(""+currentTableID));
            //    return;
            //}

            // Lấy thông tin bàn
            int tableID = 1;

            // Lấy ID Order hiện tại của bàn
            int orderID = GetOrderIDByTable(tableID);

            // Nếu không có Order, tạo mới
            if (orderID == -1)
            {
                orderID = CreateNewOrder(tableID);
            }

            // Lấy thông tin món ăn
            int foodID = (int)cbo_TenMon.SelectedValue;
            int quantity = (int)nmr_SoLuong.Value;

            // Lấy giá tiền món ăn từ cơ sở dữ liệu và chuyển sang decimal
            //int priceInt =
            decimal price = GetPriceByFoodID(foodID); // Chuyển từ int sang decimal

            // Tính thành tiền
            decimal totalAmount = price * quantity;

            // Thêm món vào OrderDetail
            AddFoodToOrder(orderID, foodID, quantity);

            // Cập nhật DataGridView
            var orderDetails = table.GetOrderDetailsByTable(tableID);
            dtgvHome.DataSource = orderDetails;

            // Tính tổng tiền
            decimal grandTotal = 0;
            foreach (DataGridViewRow row in dtgvHome.Rows)
            {
                if (row.Cells["Thành tiền"].Value != null)
                {
                    grandTotal += Convert.ToDecimal(row.Cells["Thành tiền"].Value);
                }
            }
            txb_Tongtien.Text = grandTotal.ToString("C");

            UpdateTableStatus();
        }

        private decimal GetPriceByFoodID(int foodID)
        {
            string query = "SELECT Price FROM Food WHERE IDFood = @foodID";

            using (SqlConnection connection = ConnectDB.GetSqlConnection())
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@foodID", foodID);

                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        return reader.GetDecimal(0); // Trả về giá từ cột đầu tiên
                    }
                    else
                    {
                        return 0; // Trả về giá 0 nếu không tìm thấy
                    }
                }
                catch (Exception ex)
                {
                    // Log exception or handle error
                    Console.WriteLine(ex.Message);
                }
            }
            return 0; // Trả về 0 nếu gặp lỗi hoặc không tìm thấy
        }
        private void UpdateTableStatus()
        {
            var tables = table.GetAllTables();
            foreach (var table in tables)
            {
                if (tableButtons.ContainsKey(table.IDTable))
                {
                    Button tableButton = tableButtons[table.IDTable];
                    tableButton.BackColor = table.Status == 1 ? Color.LightGreen : Color.LightGray;
                }
            }
        }

        private void btn_ban1_Click(object sender, EventArgs e)
        {

        }
    }
}
