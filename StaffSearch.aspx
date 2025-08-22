<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StaffSearch.aspx.cs" Inherits="YourNamespace.StaffSearch" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Staff Search - Health Services</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #e0f7fa;
            margin: 0;
            padding: 0;
        }

        .container {
            width: 95%;
            max-width: 1200px;
            margin: 0 auto 30px auto;
            background-color: #fff;
            padding: 25px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.15);
        }

        h2 {
            text-align: center;
            margin-bottom: 30px;
            color: #00796b;
        }

        .search-table {
            width: 100%;
            margin-bottom: 20px;
        }

        .search-table td {
            padding: 10px;
            vertical-align: top;
        }

        input[type="text"], select {
            width: 100%;
            padding: 8px 10px;
            font-size: 14px;
            border: 1px solid #bbb;
            border-radius: 4px;
            box-sizing: border-box;
        }

        .search-button {
            text-align: center;
            margin-top: 10px;
        }

        .search-button input,
        .export-buttons input {
            background-color: #388e3c;
            color: white;
            border: none;
            padding: 10px 25px;
            border-radius: 5px;
            cursor: pointer;
            font-size: 15px;
            margin: 5px;
        }

        .scroll-container {
            overflow-x: auto;
        }

        .search-button input:hover,
        .export-buttons input:hover {
            background-color: #2e7d32;
        }

        .results-table {
            width: 100%;
            border-collapse: collapse;
        }

        .results-table th,
        .results-table td {
            padding: 10px;
            border: 1px solid #ddd;
            text-align: left;
        }

        .results-table th {
            background-color: #f7f7f7;
            color: #333;
        }

        .export-buttons {
            text-align: center;
            margin-top: 20px;
        }

        footer {
            font-size: 0.8em;
            color: #555;
            margin-top: 20px;
            border-top: 2px solid #ccc;
            padding-top: 10px;
            border-bottom-left-radius: 8px;
            border-bottom-right-radius: 8px;
            text-align: center;
        }

        @media (max-width: 768px) {
            .search-table td {
                display: block;
                width: 100%;
                margin-bottom: 15px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Back Button -->
        <div class="back-button">
        </div>

        <div class="container">
            <asp:Button ID="btnBack" runat="server" Text="← Back to Dashboard" OnClick="btnBack_Click" />
            <h2>Staff - Respondent Search (Health Services)</h2>

            <table class="search-table">
                <tr>
                    <td><asp:TextBox ID="txtFirstName" runat="server" placeholder="First Name" /></td>
                    <td><asp:TextBox ID="txtLastName" runat="server" placeholder="Last Name" /></td>
                    <td><asp:TextBox ID="txtEmail" runat="server" placeholder="Email Address" /></td>
                    <td>
                        <asp:DropDownList ID="ddlState" runat="server">
                            <asp:ListItem Text="Select State" Value="OptionName" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:DropDownList ID="ddlGender" runat="server">
                            <asp:ListItem Text="Select Gender" Value="" />
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlAgeRange" runat="server">
                            <asp:ListItem Text="Select Age Range" Value="" />
                           
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlRoomType" runat="server">
                            <asp:ListItem Text="Select Room Type" Value="" />
                           
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlInsurance" runat="server">
                            <asp:ListItem Text="Select Insurance" Value="" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" class="search-button">
                        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />

                    </td>
                </tr>
            </table>

            <!-- GridView -->
            <div class="scroll-container">
                <asp:GridView ID="RespondentsGridView" runat="server" AutoGenerateColumns="False" CssClass="results-table">
                    <Columns>
                        <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                        <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                        <asp:BoundField DataField="EmailAddress" HeaderText="Email" />
                        <asp:BoundField DataField="RespondentState" HeaderText="State" />
                        <asp:BoundField DataField="TypeOfService" HeaderText="Type of Service" />
                        <asp:BoundField DataField="WifiOptions" HeaderText="WiFi Options" />
                        <asp:BoundField DataField="DischargePlans" HeaderText="Discharge Plans" />
                        <asp:BoundField DataField="Gender" HeaderText="Gender" />
                        <asp:BoundField DataField="AgeRange" HeaderText="Age Range" />
                        <asp:BoundField DataField="RoomType" HeaderText="Room Type" />
                        <asp:BoundField DataField="Insurance" HeaderText="Private Insurance" />
                        <asp:BoundField DataField="MacAddress" HeaderText="MacAddress" />
                        <asp:BoundField DataField="SessionDate" HeaderText="Session Date" />
                    </Columns>
                </asp:GridView>
            </div>

            <!-- Export Buttons -->
            <div class="export-buttons">
                <asp:Button ID="btnExportExcel" runat="server" Text="Export to Excel" onClick="btnExportExcel_Click"/>
                <asp:Button ID="btnExportPdf" runat="server" Text="Export to PDF" OnClick="btnExportPdf_Click"/>
            </div>

            <!-- Footer -->
            <footer>
                <p>&copy; Copyright Reserved By AVI Research</p>
                <p>Contact Us: email - info@aitresearch.com.au</p>
            </footer>
        </div>
    </form>
</body>
</html>
