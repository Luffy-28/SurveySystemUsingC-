<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="WebApplication10.Dashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Staff Dashboard</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #e0f7fa;
            margin: 0;
            padding: 0;
        }

        .container {
            width: 90%;
            max-width: 800px;
            margin: 80px auto;
            background-color: #fff;
            border-radius: 10px;
            padding: 40px 30px;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.15);
            text-align: center;
        }

        h2 {
            color: #00796b;
            margin-bottom: 40px;
        }

        .button-section {
            display: flex;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 30px;
        }

        .action-card {
            flex: 1;
            min-width: 280px;
            height: 180px;
            background-color: #388e3c;
            color: white;
            font-size: 1.4em;
            font-weight: bold;
            display: flex;
            justify-content: center;
            align-items: center;
            border-radius: 10px;
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

        .action-card:hover {
            background-color: #2e7d32;
        }

        footer {
            font-size: 0.85em;
            color: #555;
            text-align: center;
            margin-top: 40px;
            padding-top: 10px;
            border-top: 2px solid #ccc;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Welcome, Staff Member</h2>

            <div class="button-section">
                <div class="action-card" onclick="location.href='AddQuestion.aspx';">
                    Add Question
                </div>
                <div class="action-card" onclick="location.href='StaffSearch.aspx';">
                    Staff Search
                </div>
            </div>

            <footer>
                <p>&copy; Copyright Reserved By AVI Research</p>
                <p>Contact Us: email - info@aitresearch.com.au</p>
            </footer>
        </div>
    </form>
</body>
</html>