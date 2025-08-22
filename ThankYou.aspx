<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ThankYou.aspx.cs" Inherits="YourNamespace.ThankYou" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Thank You</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
            background-color: #e0f7fa;
        }

        .container {
            text-align: center;
            background-color: #fff;
            padding: 30px 40px;
            border-radius: 10px;
            box-shadow: 0px 4px 12px rgba(0, 0, 0, 0.15);
        }

        .title {
            font-size: 2em;
            font-weight: bold;
            color: #00796b;
            margin-bottom: 20px;
        }

        .message {
            font-size: 1.2em;
            color: #444;
            margin-bottom: 30px;
        }

        .buttons {
            display: flex;
            justify-content: center;
            gap: 20px;
            margin-bottom: 20px;
        }

        .btn {
            padding: 10px 20px;
            font-size: 1em;
            border: none;
            border-radius: 5px;
            cursor: pointer;
        }

        .btn-anonymous {
            background-color: #cccccc;
            color: #333;
        }

        .btn-register {
            background-color: #388e3c;
            color: white;
        }

        footer {
            font-size: 0.8em;
            color: #555;
            border-top: 2px solid #ccc;
            padding-top: 10px;
            border-bottom-left-radius: 8px;
            border-bottom-right-radius: 8px;
            margin-top: 20px;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- Thank You Title -->
            <div class="title">Thank you for the Survey!</div>

            <!-- Prompt Message -->
            <div class="message">Please choose how you would like to submit your responses:</div>

            <!-- Buttons -->
            <div class="buttons">
                <asp:Button ID="btnAnonymous" runat="server" Text="Anonymous" CssClass="btn btn-anonymous" OnClick="btnAnonymous_Click" />
                <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn btn-register" OnClick="btnRegister_Click" />
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
