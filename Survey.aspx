<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Survey.aspx.cs" Inherits="YourNamespace.Survey" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Survey</title>
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
            margin: 40px auto;
            background-color: #fff;
            border-radius: 8px;
            padding: 25px;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.15);
        }

        .title-section {
            text-align: center;
            font-size: 1.8em;
            font-weight: bold;
            color: #00796b;
            margin-bottom: 20px;
        }

        .question-section .question {
            font-size: 1.2em;
            font-weight: bold;
            margin-bottom: 20px;
            color: #004d40;
        }

        .choices {
            margin-bottom: 15px;
            background-color: #e0f2f1;
            padding: 12px;
            border-radius: 6px;
        }

        .choices input[type="text"],
        .choices select,
        .choices textarea {
            width: 100%;
            max-width: 600px;
            padding: 8px;
            font-size: 1em;
            border: 1px solid #bbb;
            border-radius: 5px;
            box-sizing: border-box;
            margin-bottom: 10px;
        }

        .validation-error {
            color: #d32f2f;
            font-size: 0.9em;
            margin-top: 5px;
        }

        .navigation-buttons {
            display: flex;
            justify-content: space-between;
            margin-top: 25px;
        }

        .navigation-buttons .btn {
            padding: 10px 20px;
            font-size: 1em;
            border-radius: 5px;
            border: none;
            cursor: pointer;
        }

        .btn-primary {
            background-color: #388e3c;
            color: white;
        }

        .btn-secondary {
            background-color: #9e9e9e;
            color: white;
        }

        .btn:disabled {
            background-color: #ccc;
            cursor: not-allowed;
        }

        footer {
            font-size: 0.85em;
            color: #555;
            text-align: center;
            margin-top: 30px;
            padding-top: 15px;
            border-top: 2px solid #ccc;
            border-bottom-left-radius: 8px;
            border-bottom-right-radius: 8px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- Survey Title -->
            <div class="title-section">
                <asp:Label ID="lblSurveyTitle" runat="server" Text="Online Survey"></asp:Label>
            </div>

            <!-- Question Section -->
            <div class="question-section">
                <asp:Label ID="lblQuestion" runat="server" CssClass="question"></asp:Label>
                <div class="choices">
                    <asp:PlaceHolder ID="QuestionPlaceholder" runat="server"></asp:PlaceHolder>
                </div>
                <asp:Label ID="lblValidationError" runat="server" CssClass="validation-error" />
            </div>

            <!-- Navigation Buttons -->
            <div class="navigation-buttons">
                <asp:Button ID="btnPrevious" runat="server" Text="Previous" CssClass="btn btn-secondary" OnClick="btnPrevious_Click" />
                <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="btn btn-primary" OnClick="btnNext_Click" />
            </div>
        </div>

        <!-- Footer -->
        <footer>
            <p>&copy; Copyright Reserved By AVI Research</p>
            <p>Contact Us: email - info@aitresearch.com.au</p>
        </footer>
    </form>
</body>
</html>
