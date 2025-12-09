<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomerDashboard.aspx.cs" Inherits="CustomerDashboard" %>

    <!DOCTYPE html>
    <html xmlns="http://www.w3.org/1999/xhtml">

    <head runat="server">
        <title>Customer Dashboard</title>
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
                margin-bottom: 20px;
            }

            .menu-grid {
                display: grid;
                grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
                gap: 20px;
            }

            .menu-item {
                border: 1px solid #ddd;
                padding: 15px;
                border-radius: 8px;
                text-align: center;
            }

            .menu-item h4 {
                margin: 10px 0;
            }

            .price {
                font-weight: bold;
                color: #28a745;
            }

            .stock-status {
                font-size: 12px;
                font-weight: bold;
                margin: 5px 0;
            }

            .in-stock {
                color: #28a745;
            }

            .low-stock {
                color: #ff9800;
            }

            .out-of-stock {
                color: #dc3545;
            }

            .btn-add {
                background-color: #007bff;
                color: white;
                border: none;
                padding: 5px 10px;
                cursor: pointer;
                margin-top: 10px;
            }

            .btn-add:disabled {
                background-color: #ccc;
                cursor: not-allowed;
            }

            .cart-section {
                margin-top: 40px;
                border-top: 2px solid #333;
                padding-top: 20px;
            }

            table {
                width: 100%;
                border-collapse: collapse;
            }

            th,
            td {
                border: 1px solid #ddd;
                padding: 8px;
                text-align: left;
            }
        </style>
    </head>

    <body>
        <form id="form1" runat="server">
            <div class="header">
                <h1>Customer Dashboard</h1>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" OnClick="btnLogout_Click" />
            </div>

            <asp:Panel ID="pnlNotifications" runat="server" Visible="false" CssClass="section">
                <h3>Notifications</h3>
                <asp:Repeater ID="rptNotifications" runat="server">
                    <ItemTemplate>
                        <div
                            style="background-color: #e8f4f8; padding: 10px; margin-bottom: 5px; border-left: 4px solid #007bff;">
                            <%# Eval("Message") %>
                                <span style="float:right; font-size: 12px; color: #666;">
                                    <%# Eval("CreatedAt") %>
                                </span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:Panel>

            <h2>Menu</h2>
            <asp:Repeater ID="rptMenu" runat="server" OnItemCommand="rptMenu_ItemCommand">
                <HeaderTemplate>
                    <div class="menu-grid">
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="menu-item">
                        <h4>
                            <%# Eval("ItemName") %>
                        </h4>
                        <p>
                            <%# Eval("Description") %>
                        </p>
                        <p class="price">$<%# Eval("Price") %>
                        </p>

                        <%# Convert.ToInt32(Eval("StockQuantity"))==0
                            ? "<p class='stock-status out-of-stock'>OUT OF STOCK</p>" :
                            Convert.ToInt32(Eval("StockQuantity")) <=Convert.ToInt32(Eval("LowStockLimit"))
                            ? "<p class='stock-status low-stock'>Low Stock: " + Eval("StockQuantity") + " left</p>"
                            : "<p class='stock-status in-stock'>In Stock: " + Eval("StockQuantity") + "</p>" %>

                            <asp:Button ID="btnAdd" runat="server" Text="Add to Order" CommandName="AddToCart"
                                CommandArgument='<%# Eval("ItemID") + "," + Eval("ItemName") + "," + Eval("Price") + "," + Eval("StockQuantity") %>'
                                CssClass="btn-add" Enabled='<%# Convert.ToInt32(Eval("StockQuantity")) > 0 %>' />
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

            <div class="cart-section">
                <h2>Your Order</h2>
                <asp:GridView ID="gvCart" runat="server" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="ItemName" HeaderText="Item" />
                        <asp:BoundField DataField="Price" HeaderText="Price" />
                        <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                        <asp:BoundField DataField="Total" HeaderText="Total" />
                    </Columns>
                </asp:GridView>
                <br />
                <h3>Total Amount: $<asp:Label ID="lblTotal" runat="server" Text="0.00"></asp:Label>
                </h3>
                <asp:Button ID="btnPlaceOrder" runat="server" Text="Place Order" OnClick="btnPlaceOrder_Click"
                    BackColor="#28a745" ForeColor="White" Height="40px" Width="150px" />
                <br />
                <asp:Label ID="lblOrderMsg" runat="server"></asp:Label>
            </div>
        </form>
    </body>

    </html>