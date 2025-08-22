using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using WebApplication10;



namespace YourNamespace
{
    public partial class ThankYou : Page
    {
        /// <summary>
        /// Handles the Page Load event.
        /// Checks if a RespondentID exists in the session. If not, redirects to the login page.
        /// Logs the outcome for debugging purposes.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // If RespondentID is missing, log the event and redirect to the login page
                if (Session["RespondentID"] == null || Session["Answers"] == null)
                {
                    Console.WriteLine($"{DateTime.Now}: ThankYou Page_Load - RespondentID missing. Redirecting to login.");
                    Response.Redirect("login.aspx");
                }
                else
                {
                    // Log the RespondentID found in the session
                    Console.WriteLine($"{DateTime.Now}: ThankYou Page_Load - RespondentID found: {Session["RespondentID"]}");
                }
            }
        }

        /// <summary>
        /// Handles the Anonymous button click event.
        /// Updates the respondent's status to anonymous in the database and redirects to the login page.
        /// Logs all actions and errors for debugging purposes.
        /// </summary>
        protected void btnAnonymous_Click(object sender, EventArgs e)
        {
            if (SaveAnswersToDatabase(true)) // Save answers as anonymous
            {
                ClearSurveySession(); // Clear session data
                Response.Redirect("login.aspx"); // Redirect to login
            }
        }

        /// <summary>
        /// Handles the Register button click event.
        /// Redirects the user to the Register page with a query string indicating post-survey registration.
        /// Logs the event and any errors for debugging purposes.
        /// </summary>
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine($"{DateTime.Now}: Register button clicked.");

                // Check if RespondentID exists in the session
                if (Session["RespondentID"] == null)
                {
                    Console.WriteLine($"{DateTime.Now}: RespondentID missing in session. Redirecting to ThankYou.");
                    Response.Redirect("ThankYou.aspx");
                }

                // Retain the RespondentID for post-survey registration
                int respondentID = Convert.ToInt32(Session["RespondentID"]);
                Session["IsAnonymous"] = false; // Ensure the user is not anonymous anymore

                // Redirect to the Register page with a postSurvey flag
                Response.Redirect($"Register.aspx?postSurvey=true&respondentID={respondentID}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now}: Error in btnRegister_Click: {ex.Message}");
            }
        }

        private bool SaveAnswersToDatabase(bool isAnonymous)
        {
            try
            {
                if (Session["RespondentID"] == null || Session["Answers"] == null)
                {
                    throw new InvalidOperationException("Session data is missing.");
                }

                int respondentID = Convert.ToInt32(Session["RespondentID"]);
                var answers = (Dictionary<int, string>)Session["Answers"];
                string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if the user is pre-registered
                    bool isPreRegistered = !isAnonymous;

                    // Save each answer in the database
                    foreach (var answer in answers)
                    {
                        string query = @"INSERT INTO Answers (RespondentID, QuestionID, AnswerText, CreatedAt)
                                         VALUES (@RespondentID, @QuestionID, @AnswerText, GETDATE())";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@RespondentID", respondentID);
                            cmd.Parameters.AddWithValue("@QuestionID", answer.Key);
                            cmd.Parameters.AddWithValue("@AnswerText", answer.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // Save respondent attributes for each answer
                    SaveAttributesFromAnswers(respondentID, answers, connectionString);
                    // Save session details after saving answers
                   


                    // Update Respondent as Anonymous if needed
                    if (isAnonymous)
                    {
                        string updateQuery = "UPDATE Respondents SET IsAnonymous = 1 WHERE RespondentID = @RespondentID";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@RespondentID", respondentID);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving answers: {ex.Message}");
                return false;
            }
        }

        private void SaveAttributesFromAnswers(int respondentID, Dictionary<int, string> answers, string connectionString)
        {
            foreach (var answer in answers)
            {
                int questionID = answer.Key;
                string selectedValue = answer.Value;

                // Get AttributeID and OptionID for the question
                RespondentAttribute attr = GetAttributeAndOption(questionID, selectedValue, connectionString);

                if (attr != null)
                {
                    // Save the attribute to the RespondentAttributes table
                    SaveToRespondentAttributes(attr.AttributeID, attr.OptionID, respondentID, connectionString);
                }

            }
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
        private void SaveToRespondentAttributes(int attributeID, int optionID, int respondentID, string connectionString)
        {
            System.Diagnostics.Debug.WriteLine($"Saving RespondentAttribute for RespondentID={respondentID}, AttributeID={attributeID}, OptionID={optionID}");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
        INSERT INTO RespondentAttributes (RespondentID, AttributeID, OptionID, CreatedAt)
        VALUES (@RespondentID, @AttributeID, @OptionID, GETDATE());";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RespondentID", respondentID);
                    cmd.Parameters.AddWithValue("@AttributeID", attributeID);
                    cmd.Parameters.AddWithValue("@OptionID", optionID);
                    cmd.ExecuteNonQuery();
                }
            }
        }



        private void ClearSurveySession()
        {
            Session.Remove("RespondentID");
            Session.Remove("Answers");
        }
    }
}

//code abort

//already registred user's session is not loaded in database,
