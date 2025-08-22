<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="YourNamespace.Register" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Register - AITR</title>
    <style>
        /* Unified Styling */
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
            width: 500px;
            border: 1px solid #aaa;
            border-radius: 8px;
            box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.15);
            overflow: hidden;
            display: flex;
            flex-direction: column;
            background-color: #fff;
        }

        .title-section {
            text-align: center;
            padding: 10px;
            font-size: 1.8em;
            font-weight: bold;
            color: #00796b; /* Teal */
            border-bottom: 1px solid #ddd;
        }

        .content {
            display: flex;
            flex-direction: row;
            padding: 20px;
        }

        .left-section {
            flex: 1;
            background-color: #e0f2f1;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }

        .left-section .register-title {
            font-size: 2em;
            font-weight: bold;
            color: #004d40;
            text-align: center;
        }

        .right-section {
            flex: 1;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }

        .right-section .form-field {
            width: 100%;
            margin: 8px 0;
        }

        .right-section .form-field input {
            width: 100%;
            padding: 8px;
            font-size: 1em;
            border: 1px solid #bbb;
            border-radius: 5px;
            box-sizing: border-box;
        }

        .register-button {
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

        .validation-error {
            color: #d32f2f; /* Red for errors */
            font-size: 0.9em;
            margin-top: -8px;
            margin-bottom: 8px;
            display: none;
        }

        footer {
            font-size: 0.8em;
            color: #555;
            margin-top: 10px;
            border-top: 2px solid #ccc;
            padding-top: 10px;
            border-bottom-left-radius: 8px;
            border-bottom-right-radius: 8px;
            text-align: center;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server" onsubmit="validateForm(event)">
        <div class="container">
            <!-- Title Section -->
            <div class="title-section">Registration Page</div>

            <!-- Main Content Area -->
            <div class="content">
                <div class="left-section">
                    <div class="register-title">Register</div>
                </div>

                <div class="right-section">
                    <div class="form-field">
                        <asp:TextBox ID="txtFirstName" runat="server" Placeholder="First Name" CssClass="form-control"></asp:TextBox>
                        <div id="firstNameError" class="validation-error">First name should contain only letters.</div>
                    </div>
                    <div class="form-field">
                        <asp:TextBox ID="txtLastName" runat="server" Placeholder="Last Name"></asp:TextBox>
                        <div id="lastNameError" class="validation-error">Last name should contain only letters.</div>
                    </div>
                    <div class="form-field">
                        <asp:TextBox ID="txtDateOfBirth" runat="server" Placeholder="Date of Birth" TextMode="Date"></asp:TextBox>
                        <div id="dobError" class="validation-error">Date of birth is required.</div>
                    </div>
                    <div class="form-field">
                        <asp:TextBox ID="txtEmail" runat="server" Placeholder="Email (Optional)" TextMode="Email"></asp:TextBox>
                    </div>
                    <div class="form-field">
                        <asp:TextBox ID="txtPhoneNumber" runat="server" Placeholder="Phone Number" TextMode="Phone"></asp:TextBox>
                    </div>
                    <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false" EnableViewState="false"></asp:Label>
                    <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="register-button" OnClick="btnRegister_Click" />
                </div>
            </div>

            <!-- Footer -->
            <footer>
                <p>Copyright Reserved By AIT Research</p>
                <p>Contact Us: email - info@aitresearch.com.au</p>
            </footer>
        </div>
    </form>
</body>
</html>
