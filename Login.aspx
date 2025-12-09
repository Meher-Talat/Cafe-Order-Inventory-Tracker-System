<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

    <!DOCTYPE html>
    <html xmlns="http://www.w3.org/1999/xhtml">

    <head runat="server">
        <title>Cafe System Login</title>
        <style>
            body {
                font-family: Arial, sans-serif;
                display: flex;
                justify-content: center;
                align-items: center;
                height: 100vh;
                background-color: #f0f2f5;
            }

            .login-container {
                background: white;
                padding: 30px;
                border-radius: 8px;
                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                width: 300px;
            }

            .login-container h2 {
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

            .form-group input {
                width: 100%;
                padding: 8px;
                box-sizing: border-box;
            }

            .btn-login {
                width: 100%;
                padding: 10px;
                background-color: #007bff;
                color: white;
                border: none;
                border-radius: 4px;
                cursor: pointer;
            }

            .btn-login:hover {
                background-color: #0056b3;
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
            <div class="login-container">
                <h2>Login</h2>
                <div class="form-group">
                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" placeholder="Enter email"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Password</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Enter password">
                    </asp:TextBox>
                </div>
                <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn-login" OnClick="btnLogin_Click" />
                <div class="error-msg">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </div>
                <div style="text-align: center; margin-top: 10px;">
                    <a href="Register.aspx">Create an Account</a>
                </div>
            </div>
        </form>
    </body>

    </html>