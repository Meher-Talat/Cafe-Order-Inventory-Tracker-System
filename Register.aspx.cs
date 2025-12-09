using System;
using System.Web;
using System.Web.UI;

public partial class Register : System.Web.UI.Page
{
    myDAL dal = new myDAL();

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnRegister_Click(object sender, EventArgs e)
    {
        try
        {
            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string role = ddlRole.SelectedValue;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblMessage.Text = "Please fill in all required fields.";
                return;
            }

            dal.RegisterUser(name, email, password, phone, role);
            lblMessage.Text = "Registration successful! Redirecting...";
            lblMessage.ForeColor = System.Drawing.Color.Green;
            
            // Redirect after 2 seconds
            Response.AddHeader("REFRESH", "2;URL=Login.aspx");
        }
        catch (Exception ex)
        {
            lblMessage.Text = "Error: " + ex.Message;
            lblMessage.ForeColor = System.Drawing.Color.Red;
        }
    }
}
