using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class StaffDashboard : System.Web.UI.Page
{
    myDAL dal = new myDAL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserRole"] == null || !Session["UserRole"].ToString().Equals("Staff", StringComparison.OrdinalIgnoreCase))
        {
            Response.Redirect("Login.aspx");
        }

        if (!IsPostBack)
        {
            LoadOrders();
            LoadLowStock();
            LoadMenu();
        }
    }

    private void LoadOrders()
    {
        gvOrders.DataSource = dal.GetOrders();
        gvOrders.DataBind();
    }

    private void LoadLowStock()
    {
        gvLowStock.DataSource = dal.GetLowStockItems();
        gvLowStock.DataBind();
    }

    protected void gvOrders_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int orderId = Convert.ToInt32(e.CommandArgument);
            string status = "";

            if (e.CommandName == "ConfirmOrder")
            {
                status = "Confirmed";
            }
            else if (e.CommandName == "CompleteOrder")
            {
                status = "Completed";
            }
            else if (e.CommandName == "CancelOrder")
            {
                status = "Cancelled";
            }

            if (!string.IsNullOrEmpty(status))
            {
                dal.UpdateOrderStatus(orderId, status);
                
                // Send notification to customer
                int customerId = dal.GetCustomerIdByOrderId(orderId);
                if (customerId > 0)
                {
                    string notificationMsg = "Your order #" + orderId + " has been " + status + ".";
                    dal.CreateNotification(customerId, notificationMsg);
                }

                lblMsg.Text = "Order " + status + " successfully.";
                lblMsg.ForeColor = System.Drawing.Color.Green;
                LoadOrders();
                
                // Refresh low stock if confirmed (since stock was deducted)
                if (status == "Confirmed")
                {
                    LoadLowStock();
                }
            }
        }
        catch (Exception ex)
        {
            lblMsg.Text = "Error: " + ex.Message;
            lblMsg.ForeColor = System.Drawing.Color.Red;
        }
    }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Response.Redirect("Login.aspx");
    }

    private void LoadMenu()
    {
        gvMenu.DataSource = dal.GetMenuItemsWithStock();
        gvMenu.DataBind();
    }

    protected void gvMenu_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvMenu.EditIndex = e.NewEditIndex;
        LoadMenu();
    }

    protected void gvMenu_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvMenu.EditIndex = -1;
        LoadMenu();
    }

    protected void gvMenu_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            GridViewRow row = gvMenu.Rows[e.RowIndex];
            
            int itemId = Convert.ToInt32(gvMenu.DataKeys[e.RowIndex].Value);
            string name = ((TextBox)row.Cells[1].Controls[0]).Text;
            string category = ((TextBox)row.Cells[2].Controls[0]).Text;
            decimal price = decimal.Parse(((TextBox)row.Cells[3].Controls[0]).Text);
            int stock = int.Parse(((TextBox)row.Cells[4].Controls[0]).Text);
            int lowLimit = int.Parse(((TextBox)row.Cells[5].Controls[0]).Text);
            string description = ((TextBox)row.Cells[6].Controls[0]).Text;

            dal.UpdateMenuItem(itemId, name, category, price, stock, lowLimit, description);
            
            gvMenu.EditIndex = -1;
            LoadMenu();
            LoadLowStock();
            
            lblMenuMsg.Text = "Item updated successfully!";
            lblMenuMsg.ForeColor = System.Drawing.Color.Green;
        }
        catch (Exception ex)
        {
            lblMenuMsg.Text = "Error: " + ex.Message;
            lblMenuMsg.ForeColor = System.Drawing.Color.Red;
        }
    }

    protected void gvMenu_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            int itemId = Convert.ToInt32(gvMenu.DataKeys[e.RowIndex].Value);
            dal.DeleteMenuItem(itemId);
            
            LoadMenu();
            LoadLowStock();
            
            lblMenuMsg.Text = "Item deleted successfully!";
            lblMenuMsg.ForeColor = System.Drawing.Color.Green;
        }
        catch (Exception ex)
        {
            lblMenuMsg.Text = "Error: " + ex.Message;
            lblMenuMsg.ForeColor = System.Drawing.Color.Red;
        }
    }
}
