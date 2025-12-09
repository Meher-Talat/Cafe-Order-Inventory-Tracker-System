<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StaffDashboard.aspx.cs" Inherits="StaffDashboard" %>

    <!DOCTYPE html>
    <html xmlns="http://www.w3.org/1999/xhtml">

    <head runat="server">
        <title>Staff Dashboard</title>
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

            .btn {
                padding: 5px 10px;
                color: white;
                border: none;
                cursor: pointer;
                margin-right: 5px;
            }

            .btn-confirm {
                background-color: #007bff;
            }

            .btn-complete {
                background-color: #28a745;
            }

            .btn-cancel {
                background-color: #dc3545;
            }

            .status-pending {
                color: orange;
                font-weight: bold;
            }

            .status-confirmed {
                color: blue;
                font-weight: bold;
            }

            .status-completed {
                color: green;
                font-weight: bold;
            }

            .status-cancelled {
                color: red;
                font-weight: bold;
            }
        </style>
    </head>

    <body>
        <form id="form1" runat="server">
            <div class="header">
                <h1>Staff Dashboard</h1>
                <asp:Button ID="btnLogout" runat="server" Text="Logout" OnClick="btnLogout_Click" />
            </div>

            <div class="section">
                <h3>Manage Orders</h3>
                <asp:Label ID="lblMsg" runat="server"></asp:Label>
                <asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="False"
                    OnRowCommand="gvOrders_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="OrderID" HeaderText="Order ID" />
                        <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                        <asp:BoundField DataField="OrderDate" HeaderText="Date" />
                        <asp:BoundField DataField="TotalAmount" HeaderText="Total" DataFormatString="{0:C}" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='<%# "status-" + Eval("Status").ToString().ToLower() %>'>
                                    <%# Eval("Status") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:Button ID="btnConfirm" runat="server" Text="Confirm" CommandName="ConfirmOrder"
                                    CommandArgument='<%# Eval("OrderID") %>' CssClass="btn btn-confirm"
                                    Visible='<%# Eval("Status").ToString() == "Pending" %>' />
                                <asp:Button ID="btnComplete" runat="server" Text="Complete" CommandName="CompleteOrder"
                                    CommandArgument='<%# Eval("OrderID") %>' CssClass="btn btn-complete"
                                    Visible='<%# Eval("Status").ToString() == "Confirmed" %>' />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CommandName="CancelOrder"
                                    CommandArgument='<%# Eval("OrderID") %>' CssClass="btn btn-cancel"
                                    Visible='<%# Eval("Status").ToString() == "Pending" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
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
                                <span style="color:red; font-weight:bold;">Low Stock!</span>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <div class="section">
                <h3>Manage Menu Items</h3>
                <asp:Label ID="lblMenuMsg" runat="server"></asp:Label>
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