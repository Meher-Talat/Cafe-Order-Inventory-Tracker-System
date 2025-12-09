<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AdminDashboard.aspx.cs" Inherits="AdminDashboard" %>

    <!DOCTYPE html>
    <html xmlns="http://www.w3.org/1999/xhtml">

    <head runat="server">
        <title>Admin Dashboard</title>
        <style>
            body {
                font-family: Arial, sans-serif;
                padding: 20px;
            }

            .header {
                display: flex;
                justify-content: space-between;
                align-items: center;
                border-bottom: 2px solid #333;
                padding-bottom: 10px;
                margin-bottom: 20px;
            }

            .section {
                margin-bottom: 30px;
                border: 1px solid #ccc;
                padding: 15px;
                border-radius: 5px;
            }

            .section h3 {
                margin-top: 0;
            }

            .form-grid {
                display: grid;
                grid-template-columns: 1fr 1fr;
                gap: 10px;
                max-width: 600px;
            }

            .form-grid label {
                align-self: center;
            }

            .btn {
                padding: 8px 15px;
                background-color: #28a745;
                color: white;
                border: none;
                cursor: pointer;
            }

            table {
                width: 100%;
                border-collapse: collapse;
                margin-top: 10px;
            }

            table,
            th,
            td {
                border: 1px solid #ddd;
            }

            th,
            td {
                padding: 8px;
                text-align: left;
            }

            th {
                background-color: #f2f2f2;
            }

            .alert {
                color: red;
                font-weight: bold;
            }

            .notification-item {
                padding: 8px;
                border-bottom: 1px solid #ddd;
            }
        </style>
    </head>

    <body>
        <form id="form1" runat="server">
            <div class="header">
                <h1>Admin Dashboard</h1>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" OnClick="btnLogout_Click" />
            </div>

            <asp:Panel ID="pnlNotifications" runat="server" CssClass="section"
                style="background-color: #fff3cd; border-color: #ffc107;">
                <asp:Repeater ID="rptNotifications" runat="server">
                    <ItemTemplate>
                        <div class="notification-item">
                            <strong>
                                <%# Eval("Message") %>
                            </strong>
                            <br />
                            <small style="color: #666;">
                                <%# Eval("CreatedAt", "{0:MMM dd, yyyy hh:mm tt}" ) %>
                            </small>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:Panel>

            <div class="section">
                <h3>Add New Menu Item</h3>
                <div class="form-grid">
                    <label>Item Name:</label>
                    <asp:TextBox ID="txtName" runat="server"></asp:TextBox>

                    <label>Category:</label>
                    <asp:DropDownList ID="ddlCategory" runat="server">
                        <asp:ListItem>Beverage</asp:ListItem>
                        <asp:ListItem>Food</asp:ListItem>
                        <asp:ListItem>Dessert</asp:ListItem>
                    </asp:DropDownList>

                    <label>Price:</label>
                    <asp:TextBox ID="txtPrice" runat="server"></asp:TextBox>

                    <label>Stock Quantity:</label>
                    <asp:TextBox ID="txtStock" runat="server"></asp:TextBox>

                    <label>Low Stock Limit:</label>
                    <asp:TextBox ID="txtLowLimit" runat="server" Text="10"></asp:TextBox>

                    <label>Description:</label>
                    <asp:TextBox ID="txtDesc" runat="server" TextMode="MultiLine"></asp:TextBox>

                    <label></label>
                    <asp:Button ID="btnAddItem" runat="server" Text="Add Item" CssClass="btn"
                        OnClick="btnAddItem_Click" />
                </div>
                <asp:Label ID="lblMsg" runat="server" ForeColor="Green"></asp:Label>
            </div>

            <div class="section">
                <h3>Low Stock Alerts</h3>
                <asp:GridView ID="gvLowStock" runat="server" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="ItemName" HeaderText="Item Name" />
                        <asp:BoundField DataField="StockQuantity" HeaderText="Current Stock" />
                        <asp:BoundField DataField="LowStockLimit" HeaderText="Limit" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class="alert">Low Stock!</span>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <div class="section">
                <h3>All Menu Items</h3>
                <asp:GridView ID="gvMenu" runat="server" AutoGenerateColumns="False" DataKeyNames="ItemID"
                    OnRowEditing="gvMenu_RowEditing" OnRowDeleting="gvMenu_RowDeleting"
                    OnRowUpdating="gvMenu_RowUpdating" OnRowCancelingEdit="gvMenu_RowCancelingEdit">
                    <Columns>
                        <asp:BoundField DataField="ItemID" HeaderText="ID" ReadOnly="True" />
                        <asp:BoundField DataField="ItemName" HeaderText="Item Name" />
                        <asp:BoundField DataField="Category" HeaderText="Category" />
                        <asp:BoundField DataField="Price" HeaderText="Price" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="StockQuantity" HeaderText="Stock" />
                        <asp:BoundField DataField="LowStockLimit" HeaderText="Low Limit" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:BoundField DataField="Status" HeaderText="Status" ReadOnly="True" />
                        <asp:CommandField ShowEditButton="True" ButtonType="Button" />
                        <asp:CommandField ShowDeleteButton="True" ButtonType="Button" />
                    </Columns>
                </asp:GridView>
            </div>
        </form>
    </body>

    </html>