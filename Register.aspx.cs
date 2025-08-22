using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using WebApplication10;


namespace YourNamespace
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["postSurvey"] == "true" && Session["RespondentID"] != null)
                {
                    int respondentID = Convert.ToInt32(Session["RespondentID"]);
                    Console.WriteLine($"Post-survey registration mode. RespondentID: {respondentID}");
                }
                else if (Request.QueryString["postSurvey"] != "true")
                {
                    Console.WriteLine("Pre-survey registration mode.");
                }
                else if (Session["RespondentID"] == null)
                {
                    Response.Redirect("ThankYou.aspx");
                }
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            // Validate required fields except for Email
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || !System.Text.RegularExpressions.Regex.IsMatch(txtFirstName.Text, "^[A-Za-z]+$"))
            {
                lblError.Text = "First name is required and should only contain letters.";
                lblError.Visible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text) || !System.Text.RegularExpressions.Regex.IsMatch(txtLastName.Text, "^[A-Za-z]+$"))
            {
                lblError.Text = "Last name is required and should only contain letters.";
                lblError.Visible = true;
                return;
            }

            if (!DateTime.TryParse(txtDateOfBirth.Text, out var dateOfBirth))
            {
                lblError.Text = "Please enter a valid date of birth.";
                lblError.Visible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text) || !System.Text.RegularExpressions.Regex.IsMatch(txtPhoneNumber.Text, @"^\d{10}$"))
            {
                lblError.Text = "Phone number is required and should be 10 digits.";
                lblError.Visible = true;
                return;
            }

            string email = txtEmail.Text.Trim();
            if (!string.IsNullOrWhiteSpace(email) && !System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                lblError.Text = "Please enter a valid email address.";
                lblError.Visible = true;
                return;
            }

            lblError.Visible = false; // Hide error if all good

            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                int respondentID;

                if (Request.QueryString["postSurvey"] == "true")
                {
                    if (Session["RespondentID"] == null)
                    {
                        lblError.Text = "Session expired. Please log in again.";
                        lblError.Visible = true;
                        Response.Redirect("login.aspx");
                        return;
                    }

                    respondentID = (int)Session["RespondentID"];
                    string updateQuery = @"UPDATE Respondents 
                                           SET FirstName = @FirstName, 
                                               LastName = @LastName, 
                                               Email = @Email, 
                                               DateOfBirth =@DateOfBirth, 
                                               PhoneNumber = @PhoneNumber, 
                                               IsAnonymous = 0
                                           WHERE RespondentID = @RespondentID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text);
                        cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                        cmd.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrEmpty(txtPhoneNumber.Text) ? (object)DBNull.Value : txtPhoneNumber.Text);
                        cmd.Parameters.AddWithValue("@RespondentID", respondentID);
                        cmd.ExecuteNonQuery();
                    }

                    SaveAnswers(conn, respondentID);
                    Response.Redirect("login.aspx");
                }
                else
                {
                    string insertQuery = @"INSERT INTO Respondents 
                                           (FirstName, LastName, Email, DateOfBirth, PhoneNumber, IsAnonymous, CreatedAt) 
                                           VALUES 
                                           (@FirstName, @LastName, @Email, @DateOfBirth, @PhoneNumber, 0, GETDATE());
                                           SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text);
                        cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                        cmd.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrEmpty(txtPhoneNumber.Text) ? (object)DBNull.Value : txtPhoneNumber.Text);

                        respondentID = Convert.ToInt32(cmd.ExecuteScalar());
                        Session["RespondentID"] = respondentID;
                        Session["IsAnonymous"] = false;
                    }

                    Response.Redirect("Survey.aspx");
                }
            }
        }

        private void SaveAnswers(SqlConnection conn, int respondentID)
        {
            var answers = Session["Answers"] as Dictionary<int, string>;
            if (answers == null)
                return;

            foreach (var answer in answers)
            {
                string query = @"INSERT INTO Answers (RespondentID, QuestionID, AnswerText, CreatedAt) 
                                 VALUES (@RespondentID, @QuestionID, @AnswerText, GETDATE());";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RespondentID", respondentID);
                    cmd.Parameters.AddWithValue("@QuestionID", answer.Key);
                    cmd.Parameters.AddWithValue("@AnswerText", answer.Value);
                    cmd.ExecuteNonQuery();
                }
            }

            foreach (var answer in answers)
            {
                int questionID = answer.Key;
                string selectedValue = answer.Value;

                RespondentAttribute attr = GetAttributeAndOption(questionID, selectedValue, ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString);

                if (attr != null)
                {
                    string attributeQuery = @"INSERT INTO RespondentAttributes (RespondentID, AttributeID, OptionID, CreatedAt) 
                                              VALUES (@RespondentID, @AttributeID, @OptionID, GETDATE());";

                    using (SqlCommand cmd = new SqlCommand(attributeQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@RespondentID", respondentID);
                        cmd.Parameters.AddWithValue("@AttributeID", attr.AttributeID);
                        cmd.Parameters.AddWithValue("@OptionID", attr.OptionID);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"No attribute mapping found for QuestionID={questionID}, SelectedValue={selectedValue}");
                }
            }

            Session.Remove("Answers");
        }

        private RespondentAttribute GetAttributeAndOption(int questionID, string selectedValue, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT a.AttributeID, ao.OptionID, ao.OptionName
                    FROM Questions q
                    JOIN AttributeOptions ao ON ao.AttributeID = q.AttributeID
                    JOIN Attributes a ON a.AttributeID = ao.AttributeID
                    WHERE q.QuestionID = @QuestionID AND ao.OptionName = @OptionName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@QuestionID", questionID);
                    cmd.Parameters.AddWithValue("@OptionName", selectedValue);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new RespondentAttribute
                            {
                                AttributeID = reader.GetInt32(0),
                                OptionID = reader.GetInt32(1),
                                OptionName = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
