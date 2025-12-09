using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AdminDashboard : System.Web.UI.Page
{
    myDAL dal = new myDAL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserRole"] == null || !Session["UserRole"].ToString().Equals("Administrator", StringComparison.OrdinalIgnoreCase))
        {
            Response.Redirect("Login.aspx");
        }

        if (!IsPostBack)
        {
            LoadMenu();
            LoadLowStock();
            LoadNotifications();
        }
    }

    private void LoadMenu()
    {
        gvMenu.DataSource = dal.GetMenuItems();
        gvMenu.DataBind();
    }

    private void LoadLowStock()
    {
        gvLowStock.DataSource = dal.GetLowStockItems();
        gvLowStock.DataBind();
    }

    private void LoadNotifications()
    {
        // Only show notifications if there are actual low stock items
        DataTable lowStockItems = dal.GetLowStockItems();
        
        if (lowStockItems.Rows.Count > 0)
        {
            int userId = (int)Session["UserID"];
            DataTable notifications = dal.GetUnreadNotifications(userId);
            
            if (notifications.Rows.Count > 0)
            {
                rptNotifications.DataSource = notifications;
                rptNotifications.DataBind();
                pnlNotifications.Visible = true;
            }
            else
            {
                pnlNotifications.Visible = false;
            }
        }
        else
        {
            // No low stock items, hide notifications panel
            pnlNotifications.Visible = false;
        }
    }

    protected void btnAddItem_Click(object sender, EventArgs e)
    {
        try
        {
            string name = txtName.Text;
            string category = ddlCategory.SelectedValue;
            decimal price = decimal.Parse(txtPrice.Text);
            int stock = int.Parse(txtStock.Text);
            int lowLimit = int.Parse(txtLowLimit.Text);
            string desc = txtDesc.Text;

            dal.AddMenuItem(name, category, price, stock, lowLimit, desc);
            lblMsg.Text = "Item added successfully!";
            LoadMenu();
            LoadLowStock();
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
            
            lblMsg.Text = "Item updated successfully!";
            lblMsg.ForeColor = System.Drawing.Color.Green;
        }
        catch (Exception ex)
        {
            lblMsg.Text = "Error: " + ex.Message;
            lblMsg.ForeColor = System.Drawing.Color.Red;
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
            
            lblMsg.Text = "Item deleted successfully!";
            lblMsg.ForeColor = System.Drawing.Color.Green;
        }
        catch (Exception ex)
        {
            lblMsg.Text = "Error: " + ex.Message;
            lblMsg.ForeColor = System.Drawing.Color.Red;
        }
    }
}
