<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="Register" %>

    <!DOCTYPE html>
    <html xmlns="http://www.w3.org/1999/xhtml">

    <head runat="server">
        <title>Register</title>
        <style>
            body {
                font-family: Arial, sans-serif;
                display: flex;
                justify-content: center;
                align-items: center;
                height: 100vh;
                background-color: #f0f2f5;
            }

            .register-container {
                background: white;
                padding: 30px;
                border-radius: 8px;
                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                width: 350px;
            }

            .register-container h2 {
                text-align: center;
                margin-bottom: 20px;
            }

            .form-group {
                margin-bottom: 15px;
            }

            .form-group label {
                display: block;
                margin-bottom: 5px;
            }

            .form-group input,
            .form-group select {
                width: 100%;
                padding: 8px;
                box-sizing: border-box;
            }

            .btn-register {
                width: 100%;
                padding: 10px;
                background-color: #28a745;
                color: white;
                border: none;
                border-radius: 4px;
                cursor: pointer;
            }

            .btn-register:hover {
                background-color: #218838;
            }

            .link {
                text-align: center;
                margin-top: 10px;
                display: block;
            }

            .error-msg {
                color: red;
                text-align: center;
                margin-top: 10px;
            }
        </style>
    </head>

    <body>
        <form id="form1" runat="server">
            <div class="register-container">
                <h2>Create Account</h2>
                <div class="form-group">
                    <label>Full Name</label>
                    <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Password</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Phone Number</label>
                    <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Role</label>
                    <asp:DropDownList ID="ddlRole" runat="server">
                        <asp:ListItem>Customer</asp:ListItem>
                        <asp:ListItem>Staff</asp:ListItem>
                        <asp:ListItem>Administrator</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn-register"
                    OnClick="btnRegister_Click" />
                <div class="error-msg">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </div>
                <a href="Login.aspx" class="link">Already have an account? Login</a>
            </div>
        </form>
    </body>

    </html>