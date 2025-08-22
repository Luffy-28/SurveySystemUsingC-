using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace YourNamespace
{
    public partial class AddQuestion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadQuestions();
            }
        }


        protected void ddlQuestionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlQuestionType.SelectedValue == "Single-choice" || ddlQuestionType.SelectedValue == "Multi-choice" || ddlQuestionType.SelectedValue == "Dropdown")
                choicesDiv.Style["display"] = "block";
            else
                choicesDiv.Style["display"] = "none";
        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            string questionText = txtQuestionText.Text;
            string questionType = ddlQuestionType.SelectedValue;
            string validationRule = txtValidationRule.Text;
            string validationMsg = txtValidationMsg.Text;
            int sequenceOrder = int.Parse(txtSequenceOrder.Text);
            int? parentQuestionID = string.IsNullOrEmpty(txtParentQuestionID.Text) ? (int?)null : int.Parse(txtParentQuestionID.Text);
            string condition = txtCondition.Text;
            string attributeName = txtAttributeName.Text;
            string choices = txtChoices.Text;

            if (CheckForSequenceConflict(sequenceOrder))
                ShiftSequenceNumbers(sequenceOrder);

            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                int attributeID = 0;
                if (!string.IsNullOrEmpty(attributeName))
                {
                    string insertAttributeQuery = "INSERT INTO Attributes (AttributeName) VALUES (@AttributeName); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand cmd = new SqlCommand(insertAttributeQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@AttributeName", attributeName);
                        attributeID = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }

                string insertQuestionQuery = @"
                    INSERT INTO Questions (QuestionText, QuestionType, ParentQuestionID, Condition, AttributeID, SequenceOrder, ValidationRule, ValidationMessage)
                    VALUES (@QuestionText, @QuestionType, @ParentQuestionID, @Condition, @AttributeID, @SequenceOrder, @ValidationRule, @ValidationMessage);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(insertQuestionQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@QuestionText", questionText);
                    cmd.Parameters.AddWithValue("@QuestionType", questionType);
                    cmd.Parameters.AddWithValue("@ParentQuestionID", parentQuestionID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Condition", string.IsNullOrEmpty(condition) ? (object)DBNull.Value : condition);
                    cmd.Parameters.AddWithValue("@AttributeID", attributeID == 0 ? (object)DBNull.Value : attributeID);
                    cmd.Parameters.AddWithValue("@SequenceOrder", sequenceOrder);
                    cmd.Parameters.AddWithValue("@ValidationRule", string.IsNullOrEmpty(validationRule) ? (object)DBNull.Value : validationRule);
                    cmd.Parameters.AddWithValue("@ValidationMessage", string.IsNullOrEmpty(validationMsg) ? (object)DBNull.Value : validationMsg);

                    int questionID = Convert.ToInt32(cmd.ExecuteScalar());

                    if ((questionType == "Single-choice" || questionType == "Multi-choice" || questionType == "Dropdown") && !string.IsNullOrEmpty(choices))
                    {
                        string[] choiceArray = choices.Split(',');
                        foreach (string choice in choiceArray)
                        {
                            string insertChoiceQuery = "INSERT INTO AttributeOptions (AttributeID, OptionName) VALUES (@AttributeID, @OptionName)";
                            using (SqlCommand choiceCmd = new SqlCommand(insertChoiceQuery, conn))
                            {
                                choiceCmd.Parameters.AddWithValue("@AttributeID", attributeID);
                                choiceCmd.Parameters.AddWithValue("@OptionName", choice.Trim());
                                choiceCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }

            LoadQuestions();
        }

        private void LoadQuestions()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Questions ORDER BY SequenceOrder";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    GridViewQuestions.DataSource = dt;
                    GridViewQuestions.DataBind();
                }
            }
        }

        private bool CheckForSequenceConflict(int sequenceOrder)
        {
            string cs = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Questions WHERE SequenceOrder = @SequenceOrder";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SequenceOrder", sequenceOrder);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        private void ShiftSequenceNumbers(int sequenceOrder, bool isDelete = false)
        {
            string cs = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();
                string query = isDelete
                    ? "UPDATE Questions SET SequenceOrder = SequenceOrder - 1 WHERE SequenceOrder > @SequenceOrder"
                    : "UPDATE Questions SET SequenceOrder = SequenceOrder + 1 WHERE SequenceOrder >= @SequenceOrder";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SequenceOrder", sequenceOrder);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void GridViewQuestions_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int questionID = Convert.ToInt32(GridViewQuestions.DataKeys[e.RowIndex].Value);
            int sequenceOrder = GetSequenceOrderByQuestionID(questionID);

            string cs = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();
                string query = "DELETE FROM Questions WHERE QuestionID = @QuestionID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@QuestionID", questionID);
                    cmd.ExecuteNonQuery();
                }
            }

            ShiftSequenceNumbers(sequenceOrder, isDelete: true);
            LoadQuestions();
        }

        private int GetSequenceOrderByQuestionID(int questionID)
        {
            string cs = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();
                string query = "SELECT SequenceOrder FROM Questions WHERE QuestionID = @QuestionID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@QuestionID", questionID);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        protected void GridViewQuestions_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridViewQuestions.EditIndex = e.NewEditIndex;
            LoadQuestions();
        }

        protected void GridViewQuestions_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridViewQuestions.EditIndex = -1;
            LoadQuestions();
        }

        protected void GridViewQuestions_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int questionID = Convert.ToInt32(GridViewQuestions.DataKeys[e.RowIndex].Value);
            GridViewRow row = GridViewQuestions.Rows[e.RowIndex];

            string questionText = ((TextBox)row.Cells[1].Controls[0]).Text.Trim();
            string questionType = ((TextBox)row.Cells[2].Controls[0]).Text.Trim();
            string parentIDText = ((TextBox)row.Cells[3].Controls[0]).Text.Trim();
            string seqText = ((TextBox)row.Cells[4].Controls[0]).Text.Trim();
            string rule = ((TextBox)row.Cells[5].Controls[0]).Text.Trim();
            string msg = ((TextBox)row.Cells[6].Controls[0]).Text.Trim();

            int? parentID = string.IsNullOrEmpty(parentIDText) ? (int?)null : int.Parse(parentIDText);
            int sequenceOrder = int.Parse(seqText);

            string cs = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();
                string updateQuery = @"
                    UPDATE Questions
                    SET QuestionText = @Text, QuestionType = @Type, ParentQuestionID = @ParentID,
                        SequenceOrder = @Seq, ValidationRule = @Rule, ValidationMessage = @Msg
                    WHERE QuestionID = @ID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Text", questionText);
                    cmd.Parameters.AddWithValue("@Type", questionType);
                    cmd.Parameters.AddWithValue("@ParentID", parentID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Seq", sequenceOrder);
                    cmd.Parameters.AddWithValue("@Rule", string.IsNullOrEmpty(rule) ? (object)DBNull.Value : rule);
                    cmd.Parameters.AddWithValue("@Msg", string.IsNullOrEmpty(msg) ? (object)DBNull.Value : msg);
                    cmd.Parameters.AddWithValue("@ID", questionID);
                    cmd.ExecuteNonQuery();
                }
            }

            GridViewQuestions.EditIndex = -1;
            LoadQuestions();
        }




    }
}
