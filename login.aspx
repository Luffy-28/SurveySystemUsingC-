<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="YourNamespace.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login - AITR</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #e0f7fa;
            margin: 0;
            padding: 0;
        }

        .container {
            width: 400px;
            margin: 80px auto;
            background-color: #fff;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.15);
            text-align: center;
        }

        h2 {
            color: #00796b;
            margin-bottom: 25px;
        }

        .btn {
            width: 100%;
            padding: 10px 0;
            font-size: 16px;
            border: none;
            border-radius: 5px;
            margin: 8px 0;
            cursor: pointer;
        }

        .btn-primary {
            background-color: #388e3c;
            color: white;
        }

        .btn-secondary {
            background-color: #cccccc;
            color: #555;
        }

        .or-text {
            margin: 10px 0;
            font-weight: bold;
            color: #555;
        }

        footer {
            font-size: 0.8em;
            color: #555;
            margin-top: 20px;
            border-top: 2px solid #ccc;
            padding-top: 10px;
            border-bottom-left-radius: 8px;
            border-bottom-right-radius: 8px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Welcome to the Survey Portal</h2>

            <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn btn-primary" OnClick="btnRegister_Click" />
            <div class="or-text">OR</div>
            <asp:Button ID="btnContinueAnonymous" runat="server" Text="Continue as Anonymous" CssClass="btn btn-secondary" OnClick="btnContinueAnonymous_Click" />
            <div class="or-text">OR</div>
            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary" OnClick="btnLogin_Click" />

            <footer>
                <p>&copy; Copyright Reserved By AIT Research</p>
                <p>Contact Us: email - info@aitresearch.com.au</p>
            </footer>
        </div>
    </form>
</body>
</html>
