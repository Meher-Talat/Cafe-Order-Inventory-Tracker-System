using System;
using System.Data;
using System.Web;
using System.Web.UI;

public partial class Login : System.Web.UI.Page
{
    myDAL dal = new myDAL();

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string email = txtEmail.Text.Trim();
        string password = txtPassword.Text.Trim();

        DataTable dt = dal.ValidateUser(email, password);

        if (dt.Rows.Count > 0)
        {
            string role = dt.Rows[0]["UserRole"].ToString();
            Session["UserID"] = dt.Rows[0]["UserID"];
            Session["UserRole"] = role;
            Session["FullName"] = dt.Rows[0]["FullName"];

            if (role.Equals("Administrator", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("AdminDashboard.aspx");
            }
            else if (role.Equals("Customer", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("CustomerDashboard.aspx");
            }
            else if (role.Equals("Staff", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("StaffDashboard.aspx");
            }
        }
        else
        {
            lblMessage.Text = "Invalid Email or Password.";
        }
    }
}
