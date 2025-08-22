<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StaffLogin.aspx.cs" Inherits="YourNamespace.StaffLogin" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Staff Login - AITR</title>
    <style>
        /* Unified CSS Styling */
        body {
            font-family: Arial, sans-serif;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
            background-color: #e0f7fa; /* Light cyan background */
        }

        .container {
            width: 400px;
            text-align: center;
            border: 1px solid #aaa;
            padding: 20px;
            border-radius: 8px;
            background-color: #fff;
            box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.15);
        }

        .title-section {
            font-size: 1.8em;
            font-weight: bold;
            color: #00796b;
            margin-bottom: 20px;
        }

        .form-field {
            width: 100%;
            margin: 10px 0;
        }

        .form-field input {
            width: 100%;
            padding: 8px;
            font-size: 1em;
            border: 1px solid #bbb;
            border-radius: 5px;
            box-sizing: border-box;
        }

        .login-button {
            width: 100%;
            padding: 10px;
            font-size: 1em;
            background-color: #388e3c; /* Green */
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            margin-top: 15px;
        }

        .error-message {
            color: #d32f2f;
            margin-top: 10px;
            font-size: 0.9em;
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="title-section">Staff Login</div>

            <div class="form-field">
                <asp:TextBox ID="txtUsername" runat="server" Placeholder="Username"></asp:TextBox>
            </div>
            <div class="form-field">
                <asp:TextBox ID="txtPassword" runat="server" Placeholder="Password" TextMode="Password"></asp:TextBox>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="login-button" OnClick="btnLogin_Click" />
            <asp:Label ID="lblError" runat="server" CssClass="error-message" Visible="false"></asp:Label>

            <!-- Footer -->
            <footer>
                <p>&copy; Copyright Reserved By AIT Research</p>
                <p>Contact Us: email - info@aitresearch.com.au</p>
            </footer>
        </div>
    </form>
</body>
</html>
