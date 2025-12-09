using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;

public class myDAL
{
    private string connString;

    public myDAL()
    {
        connString = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ConnectionString;
    }

    public SqlConnection GetConnection()
    {
        return new SqlConnection(connString);
    }

    // General Execute Non Query (Insert, Update, Delete)
    public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
    {
        using (SqlConnection conn = GetConnection())
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }

    // General Execute Query (Select)
    public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
    {
        using (SqlConnection conn = GetConnection())
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
    }

    // Validate User for Login
    public DataTable ValidateUser(string email, string password)
    {
        string query = "SELECT UserID, FullName, UserRole FROM Users WHERE Email = @Email AND PasswordHash = @Password AND Status = 'Active'";
        SqlParameter[] parameters = {
            new SqlParameter("@Email", email),
            new SqlParameter("@Password", password)
        };
        return ExecuteQuery(query, parameters);
    }

    // Get All Menu Items
    public DataTable GetMenuItems()
    {
        string query = "SELECT * FROM MenuItems WHERE Status = 'Available'";
        return ExecuteQuery(query);
    }

    // Add Menu Item (Admin)
    public int AddMenuItem(string name, string category, decimal price, int stock, int lowLimit, string description)
    {
        // Check for duplicate item name
        if (CheckDuplicateItemName(name))
        {
            throw new Exception("An item with the name '" + name + "' already exists. Please use a different name.");
        }

        string query = "INSERT INTO MenuItems (ItemName, Category, Price, StockQuantity, LowStockLimit, Description) VALUES (@Name, @Category, @Price, @Stock, @LowLimit, @Desc)";
        SqlParameter[] parameters = {
            new SqlParameter("@Name", name),
            new SqlParameter("@Category", category),
            new SqlParameter("@Price", price),
            new SqlParameter("@Stock", stock),
            new SqlParameter("@LowLimit", lowLimit),
            new SqlParameter("@Desc", description)
        };
        return ExecuteNonQuery(query, parameters);
    }

    // Place Order (Customer)
    public int PlaceOrder(int customerId, decimal totalAmount, DataTable orderItems)
    {
        using (SqlConnection conn = GetConnection())
        {
            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                // 1. Validate Stock Availability for all items
                foreach (DataRow row in orderItems.Rows)
                {
                    int itemId = (int)row["ItemID"];
                    int requestedQty = (int)row["Quantity"];

                    string checkStockQuery = "SELECT ItemName, StockQuantity FROM MenuItems WHERE ItemID = @ItemID";
                    SqlCommand cmdCheck = new SqlCommand(checkStockQuery, conn, transaction);
                    cmdCheck.Parameters.AddWithValue("@ItemID", itemId);
                    
                    SqlDataReader reader = cmdCheck.ExecuteReader();
                    if (reader.Read())
                    {
                        string itemName = reader["ItemName"].ToString();
                        int availableStock = Convert.ToInt32(reader["StockQuantity"]);
                        reader.Close();

                        if (availableStock < requestedQty)
                        {
                            throw new Exception(string.Format("Insufficient stock for {0}. Available: {1}, Requested: {2}", 
                                itemName, availableStock, requestedQty));
                        }
                    }
                    else
                    {
                        reader.Close();
                        throw new Exception("Item not found");
                    }
                }

                // 2. Insert into Orders Table
                string orderQuery = "INSERT INTO Orders (CustomerID, TotalAmount, Status, PaymentMethod) OUTPUT INSERTED.OrderID VALUES (@CustID, @Total, 'Pending', 'Cash');";
                SqlCommand cmdOrder = new SqlCommand(orderQuery, conn, transaction);
                cmdOrder.Parameters.AddWithValue("@CustID", customerId);
                cmdOrder.Parameters.AddWithValue("@Total", totalAmount);
                
                int orderId = (int)cmdOrder.ExecuteScalar();

                // 3. Insert into OrderDetails Table and Deduct Stock
                foreach (DataRow row in orderItems.Rows)
                {
                    int itemId = (int)row["ItemID"];
                    int qty = (int)row["Quantity"];
                    decimal price = (decimal)row["Price"];

                    // Insert order detail
                    string detailQuery = "INSERT INTO OrderDetails (OrderID, ItemID, Quantity, UnitPrice) VALUES (@OrderID, @ItemID, @Qty, @Price)";
                    SqlCommand cmdDetail = new SqlCommand(detailQuery, conn, transaction);
                    cmdDetail.Parameters.AddWithValue("@OrderID", orderId);
                    cmdDetail.Parameters.AddWithValue("@ItemID", itemId);
                    cmdDetail.Parameters.AddWithValue("@Qty", qty);
                    cmdDetail.Parameters.AddWithValue("@Price", price);
                    cmdDetail.ExecuteNonQuery();

                    // Deduct stock immediately
                    string updateStockQuery = "UPDATE MenuItems SET StockQuantity = StockQuantity - @Qty WHERE ItemID = @ItemID";
                    SqlCommand cmdUpdateStock = new SqlCommand(updateStockQuery, conn, transaction);
                    cmdUpdateStock.Parameters.AddWithValue("@Qty", qty);
                    cmdUpdateStock.Parameters.AddWithValue("@ItemID", itemId);
                    cmdUpdateStock.ExecuteNonQuery();

                    // Check if stock is now low and create notification
                    string checkLowStockQuery = "SELECT ItemName, StockQuantity, LowStockLimit FROM MenuItems WHERE ItemID = @ItemID AND StockQuantity <= LowStockLimit";
                    SqlCommand cmdCheckLow = new SqlCommand(checkLowStockQuery, conn, transaction);
                    cmdCheckLow.Parameters.AddWithValue("@ItemID", itemId);
                    
                    SqlDataReader lowStockReader = cmdCheckLow.ExecuteReader();
                    if (lowStockReader.Read())
                    {
                        string itemName = lowStockReader["ItemName"].ToString();
                        int currentStock = Convert.ToInt32(lowStockReader["StockQuantity"]);
                        lowStockReader.Close();

                        // Create notification for admins (will be done after commit)
                        // Store info to create notification after transaction
                        transaction.Commit();
                        
                        // Create low stock notification outside transaction
                        CreateLowStockNotification(itemName, currentStock);
                        
                        return orderId;
                    }
                    lowStockReader.Close();
                }

                transaction.Commit();
                return orderId;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    
    // Get Low Stock Items (Admin/Staff)
    public DataTable GetLowStockItems()
    {
        string query = "SELECT * FROM MenuItems WHERE StockQuantity <= LowStockLimit";
        return ExecuteQuery(query);
    }

    // Get All Orders (Staff)
    public DataTable GetOrders()
    {
        string query = @"
            SELECT o.OrderID, u.FullName AS CustomerName, o.OrderDate, o.TotalAmount, o.Status 
            FROM Orders o 
            JOIN Users u ON o.CustomerID = u.UserID 
            ORDER BY o.OrderDate DESC";
        return ExecuteQuery(query);
    }

    // Update Order Status (Staff)
    public void UpdateOrderStatus(int orderId, string status)
    {
        using (SqlConnection conn = GetConnection())
        {
            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                string updateQuery = "UPDATE Orders SET Status = @Status WHERE OrderID = @OrderID";
                if (status == "Confirmed")
                {
                    updateQuery = "UPDATE Orders SET Status = @Status, ConfirmedAt = GETDATE() WHERE OrderID = @OrderID";
                }

                SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.ExecuteNonQuery();

                // Deduct Stock if Confirmed
                if (status == "Confirmed")
                {
                    DeductStock(orderId, conn, transaction);
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    // Helper to Deduct Stock
    private void DeductStock(int orderId, SqlConnection conn, SqlTransaction transaction)
    {
        // Get items in the order
        string getItemsQuery = "SELECT ItemID, Quantity FROM OrderDetails WHERE OrderID = @OrderID";
        SqlCommand cmdGet = new SqlCommand(getItemsQuery, conn, transaction);
        cmdGet.Parameters.AddWithValue("@OrderID", orderId);

        DataTable dtItems = new DataTable();
        using (SqlDataAdapter da = new SqlDataAdapter(cmdGet))
        {
            da.Fill(dtItems);
        }

        // Update stock for each item
        foreach (DataRow row in dtItems.Rows)
        {
            int itemId = (int)row["ItemID"];
            int qty = (int)row["Quantity"];

            string updateStockQuery = "UPDATE MenuItems SET StockQuantity = StockQuantity - @Qty WHERE ItemID = @ItemID";
            SqlCommand cmdUpdate = new SqlCommand(updateStockQuery, conn, transaction);
            cmdUpdate.Parameters.AddWithValue("@Qty", qty);
            cmdUpdate.Parameters.AddWithValue("@ItemID", itemId);
            cmdUpdate.ExecuteNonQuery();
        }
    }

    // Register New User
    public int RegisterUser(string name, string email, string password, string phone, string role)
    {
        string query = "INSERT INTO Users (FullName, Email, PasswordHash, PhoneNumber, UserRole) VALUES (@Name, @Email, @Password, @Phone, @Role)";
        SqlParameter[] parameters = {
            new SqlParameter("@Name", name),
            new SqlParameter("@Email", email),
            new SqlParameter("@Password", password),
            new SqlParameter("@Phone", phone),
            new SqlParameter("@Role", role)
        };
        return ExecuteNonQuery(query, parameters);
    }

    // Get Menu Items with Stock Information
    public DataTable GetMenuItemsWithStock()
    {
        string query = "SELECT ItemID, ItemName, Category, Price, StockQuantity, LowStockLimit, Description, Status FROM MenuItems WHERE Status = 'Available'";
        return ExecuteQuery(query);
    }

    // Check if item has sufficient stock
    public bool CheckStockAvailability(int itemId, int quantity)
    {
        string query = "SELECT StockQuantity FROM MenuItems WHERE ItemID = @ItemID";
        SqlParameter[] parameters = {
            new SqlParameter("@ItemID", itemId)
        };
        DataTable dt = ExecuteQuery(query, parameters);
        
        if (dt.Rows.Count > 0)
        {
            int availableStock = Convert.ToInt32(dt.Rows[0]["StockQuantity"]);
            return availableStock >= quantity;
        }
        return false;
    }

    // Create Notification
    public void CreateNotification(int userId, string message)
    {
        string query = "INSERT INTO Notifications (UserID, Message, CreatedAt, IsRead) VALUES (@UserID, @Message, GETDATE(), 0)";
        SqlParameter[] parameters = {
            new SqlParameter("@UserID", userId),
            new SqlParameter("@Message", message)
        };
        ExecuteNonQuery(query, parameters);
    }

    // Create Low Stock Notification for All Admins
    public void CreateLowStockNotification(string itemName, int currentStock)
    {
        // Get all admin users
        string getAdminsQuery = "SELECT UserID FROM Users WHERE UserRole = 'Administrator' AND Status = 'Active'";
        DataTable admins = ExecuteQuery(getAdminsQuery);

        string message = string.Format("Low Stock Alert: {0} has only {1} units remaining!", itemName, currentStock);

        foreach (DataRow admin in admins.Rows)
        {
            int adminId = Convert.ToInt32(admin["UserID"]);
            CreateNotification(adminId, message);
        }
    }

    // Get Unread Notifications for User
    public DataTable GetUnreadNotifications(int userId)
    {
        string query = "SELECT NotificationID, Message, CreatedAt FROM Notifications WHERE UserID = @UserID AND IsRead = 0 ORDER BY CreatedAt DESC";
        SqlParameter[] parameters = {
            new SqlParameter("@UserID", userId)
        };
        return ExecuteQuery(query, parameters);
    }

    // Mark Notification as Read
    public void MarkNotificationAsRead(int notificationId)
    {
        string query = "UPDATE Notifications SET IsRead = 1 WHERE NotificationID = @NotificationID";
        SqlParameter[] parameters = {
            new SqlParameter("@NotificationID", notificationId)
        };
        ExecuteNonQuery(query, parameters);
    }

    // Check for Duplicate Item Name
    public bool CheckDuplicateItemName(string itemName, int? excludeItemId = null)
    {
        string query = "SELECT COUNT(*) FROM MenuItems WHERE ItemName = @ItemName AND Status = 'Available'";
        
        if (excludeItemId.HasValue)
        {
            query += " AND ItemID != @ExcludeItemId";
        }

        using (SqlConnection conn = GetConnection())
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ItemName", itemName);
                if (excludeItemId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@ExcludeItemId", excludeItemId.Value);
                }
                
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
    }

    // Update Menu Item
    public int UpdateMenuItem(int itemId, string name, string category, decimal price, int stock, int lowLimit, string description)
    {
        // Check for duplicate item name (excluding current item)
        if (CheckDuplicateItemName(name, itemId))
        {
            throw new Exception("An item with the name '" + name + "' already exists. Please use a different name.");
        }

        string query = @"UPDATE MenuItems 
                        SET ItemName = @Name, 
                            Category = @Category, 
                            Price = @Price, 
                            StockQuantity = @Stock, 
                            LowStockLimit = @LowLimit, 
                            Description = @Desc 
                        WHERE ItemID = @ItemID";
        
        SqlParameter[] parameters = {
            new SqlParameter("@ItemID", itemId),
            new SqlParameter("@Name", name),
            new SqlParameter("@Category", category),
            new SqlParameter("@Price", price),
            new SqlParameter("@Stock", stock),
            new SqlParameter("@LowLimit", lowLimit),
            new SqlParameter("@Desc", description)
        };
        return ExecuteNonQuery(query, parameters);
    }

    // Delete Menu Item (Soft Delete)
    public int DeleteMenuItem(int itemId)
    {
        string query = "UPDATE MenuItems SET Status = 'Inactive' WHERE ItemID = @ItemID";
        SqlParameter[] parameters = {
            new SqlParameter("@ItemID", itemId)
        };
        return ExecuteNonQuery(query, parameters);
    }

    // Get Menu Item by ID
    public DataTable GetMenuItemById(int itemId)
    {
        string query = "SELECT * FROM MenuItems WHERE ItemID = @ItemID";
        SqlParameter[] parameters = {
            new SqlParameter("@ItemID", itemId)
        };
        return ExecuteQuery(query, parameters);
    }
    // Get Customer ID by Order ID
    public int GetCustomerIdByOrderId(int orderId)
    {
        string query = "SELECT CustomerID FROM Orders WHERE OrderID = @OrderID";
        SqlParameter[] parameters = {
            new SqlParameter("@OrderID", orderId)
        };
        DataTable dt = ExecuteQuery(query, parameters);
        if (dt.Rows.Count > 0)
        {
            return Convert.ToInt32(dt.Rows[0]["CustomerID"]);
        }
        return 0;
    }
}
