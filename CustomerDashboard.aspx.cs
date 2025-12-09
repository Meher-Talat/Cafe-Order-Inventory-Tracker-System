using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CustomerDashboard : System.Web.UI.Page
{
    myDAL dal = new myDAL();
    DataTable dtCart;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserRole"] == null || !Session["UserRole"].ToString().Equals("Customer", StringComparison.OrdinalIgnoreCase))
        {
            Response.Redirect("Login.aspx");
        }

        if (!IsPostBack)
        {
            LoadMenu();
            InitializeCart();
            LoadNotifications();
        }
    }

    private void LoadNotifications()
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

    private void LoadMenu()
    {
        rptMenu.DataSource = dal.GetMenuItemsWithStock();
        rptMenu.DataBind();
    }

    private void InitializeCart()
    {
        if (Session["Cart"] == null)
        {
            dtCart = new DataTable();
            dtCart.Columns.Add("ItemID", typeof(int));
            dtCart.Columns.Add("ItemName", typeof(string));
            dtCart.Columns.Add("Price", typeof(decimal));
            dtCart.Columns.Add("Quantity", typeof(int));
            dtCart.Columns.Add("Total", typeof(decimal), "Price * Quantity");
            Session["Cart"] = dtCart;
        }
        else
        {
            dtCart = (DataTable)Session["Cart"];
        }
        BindCart();
    }

    private void BindCart()
    {
        gvCart.DataSource = dtCart;
        gvCart.DataBind();

        object sum = dtCart.Compute("Sum(Total)", "");
        lblTotal.Text = sum == DBNull.Value ? "0.00" : sum.ToString();
    }

    protected void rptMenu_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "AddToCart")
        {
            string[] args = e.CommandArgument.ToString().Split(',');
            int itemId = int.Parse(args[0]);
            string itemName = args[1];
            decimal price = decimal.Parse(args[2]);
            int availableStock = int.Parse(args[3]);

            dtCart = (DataTable)Session["Cart"];
            DataRow[] existingRows = dtCart.Select("ItemID = " + itemId);

            int currentQtyInCart = 0;
            if (existingRows.Length > 0)
            {
                currentQtyInCart = (int)existingRows[0]["Quantity"];
            }

            // Check if adding one more would exceed available stock
            if (currentQtyInCart + 1 > availableStock)
            {
                lblOrderMsg.Text = string.Format("Cannot add more {0}. Only {1} available in stock!", itemName, availableStock);
                lblOrderMsg.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (existingRows.Length > 0)
            {
                existingRows[0]["Quantity"] = currentQtyInCart + 1;
            }
            else
            {
                dtCart.Rows.Add(itemId, itemName, price, 1);
            }

            Session["Cart"] = dtCart;
            BindCart();
            
            lblOrderMsg.Text = itemName + " added to cart!";
            lblOrderMsg.ForeColor = System.Drawing.Color.Green;
        }
    }

    protected void btnPlaceOrder_Click(object sender, EventArgs e)
    {
        dtCart = (DataTable)Session["Cart"];
        if (dtCart.Rows.Count > 0)
        {
            int userId = (int)Session["UserID"];
            decimal total = decimal.Parse(lblTotal.Text);

            try
            {
                int orderId = dal.PlaceOrder(userId, total, dtCart);
                lblOrderMsg.Text = "Order Placed Successfully! Order ID: " + orderId;
                lblOrderMsg.ForeColor = System.Drawing.Color.Green;
                
                // Clear Cart
                Session["Cart"] = null;
                InitializeCart();
            }
            catch (Exception ex)
            {
                lblOrderMsg.Text = "Error placing order: " + ex.Message;
                lblOrderMsg.ForeColor = System.Drawing.Color.Red;
            }
        }
        else
        {
            lblOrderMsg.Text = "Cart is empty!";
            lblOrderMsg.ForeColor = System.Drawing.Color.Red;
        }
    }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Response.Redirect("Login.aspx");
    }
}
