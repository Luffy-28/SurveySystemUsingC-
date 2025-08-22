<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddQuestion.aspx.cs" Inherits="YourNamespace.AddQuestion" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add New Question</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #e0f7fa;
            margin: 0;
            padding: 0;
        }

        form {
            width: 90%;
            max-width: 1200px;
            margin: 30px auto;
            padding: 20px;
            background-color: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.15);
        }

        h2 {
            text-align: center;
            color: #00796b;
        }

        table {
            width: 100%;
            margin-top: 20px;
        }

        table td {
            padding: 10px;
            vertical-align: top;
        }

        input[type="text"], select, textarea {
            width: 100%;
            padding: 8px;
            font-size: 14px;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-sizing: border-box;
        }

        .btn {
            background-color: #388e3c;
            color: white;
            border: none;
            padding: 10px 20px;
            font-size: 14px;
            border-radius: 5px;
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

        .btn:hover {
            background-color: #2e7d32;
        }

        .gridview-container {
            margin-top: 40px;
        }

        .results-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 10px;
        }

        .results-table th, .results-table td {
            padding: 10px;
            border: 1px solid #ddd;
            text-align: left;
        }

        .results-table th {
            background-color: #f7f7f7;
            color: #333;
        }

        .results-table .btn {
            background-color: #e74c3c;
        }

        .results-table .btn:hover {
            background-color: #c0392b;
        }

        footer {
            font-size: 0.8em;
            color: #555;
            margin-top: 30px;
            border-top: 2px solid #ccc;
            padding-top: 10px;
            border-bottom-left-radius: 8px;
            border-bottom-right-radius: 8px;
            text-align: center;
        }

        @media (max-width: 768px) {
            form {
                width: 95%;
            }

            table td {
                display: block;
                width: 100%;
                margin-bottom: 10px;
            }
        }
    </style>
</head>
<body>
   
    <form id="form1" runat="server">
     
        <div>

            <h2>Add New Question</h2>
            <table>
                <tr>
                    <td>Question Text:</td>
                    <td><asp:TextBox ID="txtQuestionText" runat="server" placeholder="Enter Question Text" required></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Question Type:</td>
                    <td>
                        <asp:DropDownList ID="ddlQuestionType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlQuestionType_SelectedIndexChanged">
                            <asp:ListItem Text="Select Type" Value=""></asp:ListItem>
                            <asp:ListItem Text="Single-choice" Value="Single-choice"></asp:ListItem>
                            <asp:ListItem Text="Multi-choice" Value="Multi-choice"></asp:ListItem>
                            <asp:ListItem Text="Dropdown" Value="Dropdown"></asp:ListItem>
                            <asp:ListItem Text="Text" Value="Text"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>Parent Question ID:</td>
                    <td><asp:TextBox ID="txtParentQuestionID" runat="server" placeholder="Parent Question ID (if any)"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Condition:</td>
                    <td><asp:TextBox ID="txtCondition" runat="server" placeholder="Condition (if any)"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Attribute ID:</td>
                    <td><asp:TextBox ID="txtAttributeName" runat="server" placeholder="Attribute ID (optional)"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Sequence Order:</td>
                    <td><asp:TextBox ID="txtSequenceOrder" runat="server" placeholder="Sequence Order" required></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Validation Rule:</td>
                    <td><asp:TextBox ID="txtValidationRule" runat="server" placeholder="Validation Rule (optional)"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Validation Message:</td>
                    <td><asp:TextBox ID="txtValidationMsg" runat="server" placeholder="Validation Message (optional)"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Choices (for Multi-choice or Dropdown):</td>
                    <td>
                        <div id="choicesDiv" runat="server" style="display: none;">
                            <asp:TextBox ID="txtChoices" runat="server" placeholder="Enter choices (comma separated)"></asp:TextBox>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="btnAddQuestion" runat="server" Text="Add Question" CssClass="btn" OnClick="btnAddQuestion_Click" />
                    </td>
                </tr>
            </table>
        </div>

        <div class="gridview-container">
            <h2>Existing Questions</h2>
            <asp:GridView ID="GridViewQuestions" runat="server"
                AutoGenerateColumns="False"
                EmptyDataText="No questions found."
                DataKeyNames="QuestionID"
                OnRowEditing="GridViewQuestions_RowEditing"
                OnRowUpdating="GridViewQuestions_RowUpdating"
                OnRowCancelingEdit="GridViewQuestions_RowCancelingEdit"
                OnRowDeleting="GridViewQuestions_RowDeleting"
                CssClass="results-table">
                <Columns>
                    <asp:BoundField DataField="QuestionID" HeaderText="Question ID" ReadOnly="True" />
                    <asp:BoundField DataField="QuestionText" HeaderText="Question Text" />
                    <asp:BoundField DataField="QuestionType" HeaderText="Question Type" />
                    <asp:BoundField DataField="ParentQuestionID" HeaderText="Parent Question ID" />
                    <asp:BoundField DataField="SequenceOrder" HeaderText="Sequence Order" />
                    <asp:BoundField DataField="ValidationRule" HeaderText="Validation Rule" />
                    <asp:BoundField DataField="ValidationMessage" HeaderText="Validation Message" />

                    <asp:CommandField ShowEditButton="True" ShowCancelButton="True" />

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button ID="btnDelete" runat="server" Text="Delete"
                                CommandName="Delete"
                                CommandArgument='<%# Eval("QuestionID") %>'
                                CssClass="btn"
                                OnClientClick="return confirm('Are you sure you want to delete this question?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <footer>
            <p>&copy; Copyright Reserved By AVI Research</p>
            <p>Contact Us: email - info@aitresearch.com.au</p>
        </footer>
    </form>
</body>
</html>
