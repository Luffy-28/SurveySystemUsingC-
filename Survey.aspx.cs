using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using WebApplication10;
using System;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace YourNamespace
{
    public partial class Survey : System.Web.UI.Page
    {

        private string NormalizeId(string input) => Regex.Replace(input.Trim(), "[^a-zA-Z0-9_]", "_");
        private List<Question> questions; // List of all questions loaded from the database
        private int currentQuestionIndex; // Index of the current question being displayed

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["IsAnonymous"] == null)
                    Session["IsAnonymous"] = true;

                if (Session["RespondentID"] == null && (bool)Session["IsAnonymous"])
                    GenerateAnonymousRespondentID();

                LoadQuestions();
                currentQuestionIndex = 0;
                Session["CurrentQuestionIndex"] = currentQuestionIndex;
                Session["Questions"] = questions;
                Session["Answers"] = new Dictionary<int, string>();
                DisplayQuestion();
            }
            else
            {
                questions = (List<Question>)Session["Questions"];
                currentQuestionIndex = (int)Session["CurrentQuestionIndex"];
                DisplayQuestion();
            }
        }

        private int GetNextQuestionIndex(Dictionary<int, string> answers)
        {
            var skippedQuestions = new Queue<Question>();

            for (int i = currentQuestionIndex + 1; i < questions.Count; i++)
            {
                Question nextQuestion = questions[i];

                if (nextQuestion.ParentQuestionID == null || string.IsNullOrEmpty(nextQuestion.Condition))
                {
                    return i;
                }

                if (answers.TryGetValue(nextQuestion.ParentQuestionID.Value, out var parentAnswer))
                {
                    if (EvaluateCondition(nextQuestion.Condition, parentAnswer))
                    {
                        return i;
                    }
                }

                skippedQuestions.Enqueue(nextQuestion);
            }

            while (skippedQuestions.Count > 0)
            {
                var next = skippedQuestions.Dequeue();
                if (IsQuestionVisible(next))
                {
                    return questions.FindIndex(q => q.QuestionID == next.QuestionID);
                }
            }

            return -1;
        }


        /// <summary>
        /// Load questions and their metadata from the database.
        /// </summary>
        private void LoadQuestions()
        {
            questions = new List<Question>();
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT QuestionID, QuestionText, QuestionType,ParentQuestionID, Condition, SequenceOrder, ValidationRule, ValidationMessage 
                                 FROM Questions ORDER BY SequenceOrder";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        questions.Add(new Question
                        {
                            QuestionID = reader.GetInt32(0),
                            QuestionText = reader.GetString(1),
                            QuestionType = reader.GetString(2),
                            ParentQuestionID = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3),
                            Condition = reader.IsDBNull(4) ? null : reader.GetString(4),
                            SequenceOrder = reader.GetInt32(5),
                            ValidationRule = reader.IsDBNull(6) ? null : reader.GetString(6),
                            ValidationMsg = reader.IsDBNull(7) ? null : reader.GetString(7)
                        });
                    }
                }
            }
            Session["Questions"] = questions;
        }

        /// <summary>
        /// Display the current question dynamically based on its type and properties.
        /// </summary>
        private void DisplayQuestion()
        {
            questions = (List<Question>)Session["Questions"];
            if (currentQuestionIndex < 0 || currentQuestionIndex >= questions.Count)
                return;

            Question currentQuestion = questions[currentQuestionIndex];
            lblSurveyTitle.Text = "Respondent Survey";
            lblQuestion.Text = currentQuestion.QuestionText;
            QuestionPlaceholder.Controls.Clear();

            // Fetch Choices for the Current Question
            List<string> choices = GetChoices(currentQuestion.QuestionID);

            // Render input controls based on question type
            if (currentQuestion.QuestionType == "Single-choice" || currentQuestion.QuestionType == "Dropdown")
            {
                DropDownList ddl = new DropDownList();
                ddl.ID = "ddlAnswer";
                // Load choices for other dropdown or single-choice questions
                LoadChoices(currentQuestion.QuestionID, ddl);


                QuestionPlaceholder.Controls.Add(ddl);
            }
            else if (currentQuestion.QuestionType == "Multi-choice")
            {

                foreach (var choice in GetChoices(currentQuestion.QuestionID))
                {
                    CheckBox cb = new CheckBox
                    {
                        Text = choice,
                        ID = "cb_" + NormalizeId(choice)
                    };
                    QuestionPlaceholder.Controls.Add(cb);
                    QuestionPlaceholder.Controls.Add(new Literal { Text = "<br/>" });
                }
            }
            else if (currentQuestion.QuestionType == "Text")
            {
                TextBox txt = new TextBox
                {
                    ID = "txtAnswer",
                    CssClass = "form-control"
                };
                QuestionPlaceholder.Controls.Add(txt);
            }
            // Handle dynamic button visibility
            UpdateButtonVisibility();


        }
        private void UpdateButtonVisibility()
        {
            var answers = (Dictionary<int, string>)Session["Answers"];
            int nextIndex = GetNextQuestionIndex(answers);

            btnNext.Visible = true; // Always show Next
                                    // btnSubmit.Visible = false; // Never show Submit
        }

        /// <summary>
        /// Load choices dynamically for a given question ID.
        /// </summary>
        private void LoadChoices(int questionID, DropDownList ddl)
        {
            foreach (var choice in GetChoices(questionID))
            {
                ddl.Items.Add(new ListItem(choice));
            }
        }

        /// <summary>
        /// Retrieve choices from the database for a given question.
        /// </summary>
        private List<string> GetChoices(int questionID)
        {
            List<string> choices = new List<string>();
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Fetch Question Details to Determine Source of Choices
                string questionQuery = "SELECT AttributeID FROM Questions WHERE QuestionID = @QuestionID";
                using (SqlCommand questionCmd = new SqlCommand(questionQuery, conn))
                {
                    questionCmd.Parameters.AddWithValue("@QuestionID", questionID);
                    object attributeIdObj = questionCmd.ExecuteScalar();

                    if (attributeIdObj != null && attributeIdObj != DBNull.Value)
                    {
                        // If AttributeID exists, fetch choices from AttributeOptions table
                        int attributeID = Convert.ToInt32(attributeIdObj);
                        string optionsQuery = "SELECT OptionName FROM AttributeOptions WHERE AttributeID = @AttributeID ORDER BY OptionID";
                        using (SqlCommand optionsCmd = new SqlCommand(optionsQuery, conn))
                        {
                            optionsCmd.Parameters.AddWithValue("@AttributeID", attributeID);
                            using (SqlDataReader reader = optionsCmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    choices.Add(reader.GetString(0));
                                }
                            }
                        }
                    }
                    else
                    {
                        // Fallback to Choices column in Questions table for other questions
                        string choicesQuery = "SELECT Choices FROM Questions WHERE QuestionID = @QuestionID";
                        using (SqlCommand choicesCmd = new SqlCommand(choicesQuery, conn))
                        {
                            choicesCmd.Parameters.AddWithValue("@QuestionID", questionID);
                            object choicesObj = choicesCmd.ExecuteScalar();
                            if (choicesObj != null && choicesObj != DBNull.Value)
                            {
                                choices = choicesObj.ToString().Split(',').Select(choice => choice.Trim()).ToList();
                            }
                        }
                    }
                }
            }
            return choices;
        }


        /// <summary>
        /// Handle the "Next" button click and navigate to the next question.
        /// </summary>
        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (!ValidateQuestion())
                return;

            SaveAnswer(); // Save the current answer

            var answers = (Dictionary<int, string>)Session["Answers"];
            int nextIndex = GetNextQuestionIndex(answers);

            if (nextIndex != -1)
            {
                // Move to the next question
                currentQuestionIndex = nextIndex;
                Session["CurrentQuestionIndex"] = currentQuestionIndex;
                DisplayQuestion();
            }
            else
            {
                // End of the survey
                if (Session["RespondentID"] != null && !(bool)Session["IsAnonymous"]) // Pre-registration
                {
                    try
                    {
                        string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            int respondentID = (int)Session["RespondentID"];
                            SaveAllAnswers(conn, respondentID); // Save answers and attributes
                        }

                        SaveSessionDetails(); // Save session details

                        // Show survey completion message
                        string script = @"alert('Survey completed successfully! Redirecting to login...'); 
                                  window.location.href = 'login.aspx';";
                        ClientScript.RegisterStartupScript(this.GetType(), "SurveyComplete", script, true);
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors during saving
                        string script = $"alert('Error saving survey data: {ex.Message}');";
                        ClientScript.RegisterStartupScript(this.GetType(), "SaveError", script, true);
                    }
                }
                else
                {
                    // Anonymous user or post-survey registration
                    System.Diagnostics.Debug.WriteLine("Anonymous or Post-Registered Flow.");
                    SaveSessionDetails();
                    Response.Redirect("ThankYou.aspx");
                }
            }
        }


        /// <summary>
        /// Handle the "Previous" button click and navigate to the previous question.
        /// </summary>
        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentQuestionIndex > 0)
            {

                currentQuestionIndex--;

                // Update existing answer if navigating back
                SaveOrUpdateAnswer();

                // Navigate back to the previous visible question
                while (currentQuestionIndex >= 0 && !IsQuestionVisible(questions[currentQuestionIndex]))
                {
                    currentQuestionIndex--;
                }

                Session["CurrentQuestionIndex"] = currentQuestionIndex;
                DisplayQuestion();
            }
        }


        /// <summary>
        /// Check if a question should be displayed based on conditions.
        /// </summary>
        private bool IsQuestionVisible(Question question)
        {
            if (question.ParentQuestionID == null || string.IsNullOrEmpty(question.Condition))
                return true;

            var answers = (Dictionary<int, string>)Session["Answers"];
            if (!answers.TryGetValue(question.ParentQuestionID.Value, out var answer))
                return false;


            // Evaluate the condition for sub-questions
            bool result = EvaluateCondition(question.Condition, answer);

            // Debugging logs for parent question visibility
            System.Diagnostics.Debug.WriteLine($"Question: {question.QuestionText}");
            System.Diagnostics.Debug.WriteLine($"Parent Condition: {question.Condition}");
            System.Diagnostics.Debug.WriteLine($"Parent Answer: {answer}");
            System.Diagnostics.Debug.WriteLine($"Parent Question Visibility: {result}");

            return result;
        }

        /// <summary>
        /// Evaluate a question's condition based on previous answers.
        /// </summary>
        private bool EvaluateCondition(string condition, string answer)
        {
            if (string.IsNullOrEmpty(condition))
                return false;

            if (condition.Contains("IN"))
            {
                // Extract valid options from the condition string
                var validOptions = condition
                    .Split('(')[1]
                    .TrimEnd(')')
                    .Replace("'", "")
                    .Split(',')
                    .Select(o => o.Trim());

                // Split the provided answer into selected options
                var selectedOptions = answer
                    .Split(',')
                    .Select(o => o.Trim());

                // Ensure at least one selected option matches any valid option
                bool isValid = selectedOptions.Any(selected =>
                    validOptions.Any(valid => valid.Equals(selected, StringComparison.OrdinalIgnoreCase)));

                // Debugging log for the validation process
                System.Diagnostics.Debug.WriteLine($"Condition: {condition}");
                System.Diagnostics.Debug.WriteLine($"Answer: {answer}");
                System.Diagnostics.Debug.WriteLine($"Valid Options: {string.Join(", ", validOptions)}");
                System.Diagnostics.Debug.WriteLine($"Selected Options: {string.Join(", ", selectedOptions)}");
                System.Diagnostics.Debug.WriteLine($"Condition Validation Result: {isValid}");

                return isValid;
            }

            if (condition.Contains("IS NOT NULL"))
            {
                // Ensure the answer is not null or empty.
                bool isValid = !string.IsNullOrEmpty(answer);

                return isValid;
            }



            return false;
        }
        /// <summary>
        /// Validate the current question input dynamically based on database conditions.
        /// </summary>
        /// <summary>
        /// Validate the current question input dynamically based on database conditions.
        /// </summary>
        private bool ValidateQuestion()
        {
            if (questions == null)
            {
                DisplayErrorMessage("Questions are not loaded. Please reload the page.");
                return false;
            }

            Question currentQuestion = questions[currentQuestionIndex];
            string errorMessage = string.Empty;

            // Fetch validation rules and messages from the current question
            string validationRule = currentQuestion.ValidationRule;
            string validationMessage = currentQuestion.ValidationMsg;

            // Fetch available choices dynamically (from Choices column or AttributeOptions table)
            List<string> availableChoices = GetChoices(currentQuestion.QuestionID);

            // Validate Single-choice or Dropdown questions
            if (currentQuestion.QuestionType == "Single-choice" || currentQuestion.QuestionType == "Dropdown")
            {
                DropDownList ddl = (DropDownList)QuestionPlaceholder.FindControl("ddlAnswer");
                if (ddl == null || ddl.SelectedItem == null || string.IsNullOrEmpty(ddl.SelectedItem.Text))
                {
                    errorMessage = validationMessage ?? $"{currentQuestion.QuestionText} is mandatory. Please provide a valid response.";
                }
                else if (availableChoices.Count > 0 && !availableChoices.Contains(ddl.SelectedItem.Text))
                {
                    // Ensure the selected value matches an available choice
                    errorMessage = $"{currentQuestion.QuestionText} contains an invalid choice. Please select a valid option.";
                }
            }

            // Validate Multi-choice questions
            else if (currentQuestion.QuestionType == "Multi-choice")
            {
                int selectedCount = 0;

                foreach (string choice in availableChoices)
                {
                    CheckBox cb = (CheckBox)QuestionPlaceholder.FindControl("cb_" + choice.Replace(" ", "_"));
                    if (cb != null && cb.Checked)
                    {
                        selectedCount++;
                    }
                }

                // Parse and apply validation rules
                if (!string.IsNullOrEmpty(validationRule))
                {
                    if (validationRule.Contains("Min:"))
                    {
                        int minSelection = ExtractConditionValue(validationRule, "Min:");
                        if (selectedCount < minSelection)
                        {
                            errorMessage = validationMessage ?? $"{currentQuestion.QuestionText} requires at least {minSelection} selections.";
                        }
                    }

                    if (validationRule.Contains("Max:"))
                    {
                        int maxSelection = ExtractConditionValue(validationRule, "Max:");
                        if (selectedCount > maxSelection)
                        {
                            errorMessage = validationMessage ?? $"{currentQuestion.QuestionText} allows a maximum of {maxSelection} selections.";
                        }
                    }
                }
            }

            // Validate Text questions
            else if (currentQuestion.QuestionType == "Text")
            {
                TextBox txt = (TextBox)QuestionPlaceholder.FindControl("txtAnswer");

                if (txt == null || string.IsNullOrWhiteSpace(txt.Text))
                {
                    errorMessage = validationMessage ?? $"{currentQuestion.QuestionText} is mandatory. Please provide a valid response.";
                }
                else if (!string.IsNullOrEmpty(validationRule))
                {
                    // Apply regex-based validation for text inputs
                    if (validationRule.StartsWith("Regex:"))
                    {
                        string pattern = validationRule.Replace("Regex:", string.Empty);
                        if (!System.Text.RegularExpressions.Regex.IsMatch(txt.Text, pattern))
                        {
                            errorMessage = validationMessage ?? $"{currentQuestion.QuestionText} has an invalid format.";
                        }
                    }
                }
            }

            // Display error if validation fails
            if (!string.IsNullOrEmpty(errorMessage))
            {
                DisplayErrorMessage(errorMessage);
                return false;
            }

            // Clear any previous errors
            DisplayErrorMessage(string.Empty);
            return true;
        }
        private int ExtractConditionValue(string validationRule, string key)
        {
            try
            {
                int startIndex = validationRule.IndexOf(key) + key.Length;
                int endIndex = validationRule.IndexOf(';', startIndex);
                if (endIndex == -1) endIndex = validationRule.Length;

                return int.Parse(validationRule.Substring(startIndex, endIndex - startIndex));
            }
            catch
            {
                return 0; // Return 0 if parsing fails
            }
        }

        /// <summary>
        /// Get the answer for a given question from session or database.
        /// </summary>
        private string GetAnswer(int questionID)
        {
            Dictionary<int, string> answers = (Dictionary<int, string>)Session["Answers"];
            return answers.ContainsKey(questionID) ? answers[questionID] : null;
        }

        /// <summary>
        /// Save or update the answer for the current question to prevent duplicates.
        /// </summary>
        private void SaveOrUpdateAnswer()
        {
            questions = (List<Question>)Session["Questions"];
            Question currentQuestion = questions[currentQuestionIndex];
            Dictionary<int, string> answers = (Dictionary<int, string>)Session["Answers"];
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            // Check if the answer already exists
            string existingAnswer = GetAnswer(currentQuestion.QuestionID);

            if (existingAnswer != null)
            {
                // Update existing answer in the database
                UpdateAnswerInDatabase(currentQuestion.QuestionID, existingAnswer, connectionString);
            }
            else
            {
                // Save new answer
                SaveAnswer();
            }
        }

        /// <summary>
        /// Update an existing answer in the database.
        /// </summary>
        private void UpdateAnswerInDatabase(int questionID, string answer, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"UPDATE Answers 
                         SET AnswerText = @AnswerText, CreatedAt = GETDATE() 
                         WHERE RespondentID = @RespondentID AND QuestionID = @QuestionID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AnswerText", answer);
                    cmd.Parameters.AddWithValue("@RespondentID", (int)Session["RespondentID"]);
                    cmd.Parameters.AddWithValue("@QuestionID", questionID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Ensure GenerateAnonymousRespondentID avoids duplicates.
        /// </summary>
        private void GenerateAnonymousRespondentID()
        {// Check if a RespondentID already exists in the session
            if (Session["RespondentID"] != null && (int)Session["RespondentID"] > 0)
            {
                System.Diagnostics.Debug.WriteLine($"RespondentID already exists in session: {Session["RespondentID"]}");
                return; // Use the existing RespondentID
            }
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Generate a unique email for anonymous respondents
                string placeholderEmail = $"anonymous_{Guid.NewGuid()}@example.com";

                int respondentID = -1;

                string checkQuery = "SELECT RespondentID FROM Respondents WHERE Email = @Email";

                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Email", placeholderEmail);
                    object result = checkCmd.ExecuteScalar();
                    if (result != null)
                    {
                        respondentID = Convert.ToInt32(result);
                        System.Diagnostics.Debug.WriteLine($"Existing RespondentID found: {respondentID}");
                    }
                }
                // If no existing respondent, insert a new one
                if (respondentID == -1)
                {
                    string insertQuery = @"INSERT INTO Respondents (FirstName, LastName, Email, IsAnonymous, CreatedAt) 
                                   VALUES ('Anonymous', 'Anonymous', @Email, 1, GETDATE());
                                   SELECT SCOPE_IDENTITY();";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@Email", placeholderEmail);
                        respondentID = Convert.ToInt32(insertCmd.ExecuteScalar());
                        System.Diagnostics.Debug.WriteLine($"New Anonymous RespondentID created: {respondentID}");
                    }
                }

                Session["RespondentID"] = respondentID;
                System.Diagnostics.Debug.WriteLine($"Anonymous RespondentID set to: {respondentID}");
            }
        }

        /// <summary>
        /// Display an error message dynamically on the page.
        /// </summary>
        private void DisplayErrorMessage(string message)
        {
            Label lblValidationError = (Label)QuestionPlaceholder.FindControl("lblValidationError");

            // If the label does not exist, create it dynamically
            if (lblValidationError == null)
            {
                lblValidationError = new Label
                {
                    ID = "lblValidationError",
                    CssClass = "validation-error",
                    ForeColor = System.Drawing.Color.Red
                };
                QuestionPlaceholder.Controls.AddAt(0, lblValidationError); // Add error label after the question label
            }

            // Set the message
            lblValidationError.Text = message;
            // Debugging logs
            System.Diagnostics.Debug.WriteLine($"Validation Error: {message}");


        }

        /// <summary>
        /// Save the answer for the current question to the session.
        /// </summary>
        /// <summary>
        /// Save the answer for the current question to the session and database.
        /// </summary>
        private void SaveAnswer()
        {
            // Retrieve the current question and the respondent's answers
            questions = (List<Question>)Session["Questions"];
            Question currentQuestion = questions[currentQuestionIndex];
            Dictionary<int, string> answers = (Dictionary<int, string>)Session["Answers"];

            // Ensure Session["Answers"] is initialized
            if (answers == null)
            {
                answers = new Dictionary<int, string>();
                Session["Answers"] = answers;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            // Save the answer based on the question type
            if (currentQuestion.QuestionType == "Single-choice" || currentQuestion.QuestionType == "Dropdown")
            {
                DropDownList ddl = (DropDownList)QuestionPlaceholder.FindControl("ddlAnswer");
                if (ddl != null)
                {
                    answers[currentQuestion.QuestionID] = ddl.SelectedValue; // Save selected value

                    // Save to RespondentAttributes if AttributeID exists
                    SaveRespondentAttribute(currentQuestion.QuestionID, ddl.SelectedValue, connectionString);
                }
            }
            else if (currentQuestion.QuestionType == "Multi-choice")
            {
                List<string> selectedChoices = new List<string>();
                foreach (string choice in GetChoices(currentQuestion.QuestionID))
                {
                    CheckBox cb = (CheckBox)QuestionPlaceholder.FindControl("cb_" + NormalizeId(choice));
                    if (cb != null && cb.Checked)
                    {
                        selectedChoices.Add(choice);
                    }
                }
                answers[currentQuestion.QuestionID] = string.Join(",", selectedChoices); // Save as comma-separated values

                // Save to RespondentAttributes if AttributeID exists
                foreach (var choice in selectedChoices)
                {
                    SaveRespondentAttribute(currentQuestion.QuestionID, choice, connectionString);
                }
            }
            else if (currentQuestion.QuestionType == "Text")
            {
                TextBox txt = (TextBox)QuestionPlaceholder.FindControl("txtAnswer");
                if (txt != null)
                {
                    answers[currentQuestion.QuestionID] = txt.Text; // Save text input
                }
            }

            // Store the updated answers back in session
            Session["Answers"] = answers;
        }

        /// <summary>
        /// Save respondent attributes to the session for anonymous users or directly to the database for registered users.
        /// </summary>
        private void SaveRespondentAttribute(int questionID, string selectedValue, string connectionString)
        {
            // Handle anonymous respondents (store in session)
            if (Session["IsAnonymous"] != null && (bool)Session["IsAnonymous"])
            {
                var sessionAttributes = Session["Attributes"] as Dictionary<int, List<RespondentAttribute>>
                                        ?? new Dictionary<int, List<RespondentAttribute>>();
                Session["Attributes"] = sessionAttributes;

                if (!sessionAttributes.ContainsKey(questionID))
                {
                    sessionAttributes[questionID] = new List<RespondentAttribute>();
                }

                // Add if not already present
                if (!sessionAttributes[questionID].Any(attr => attr.OptionName == selectedValue))
                {
                    var attr = GetAttributeAndOption(questionID, selectedValue, connectionString);
                    if (attr != null)
                    {
                        sessionAttributes[questionID].Add(attr);
                        System.Diagnostics.Debug.WriteLine($"Saved attribute to session: QuestionID={questionID}, OptionName={selectedValue}");
                    }
                }
            }
            else // Registered user - save to database
            {
                var attr = GetAttributeAndOption(questionID, selectedValue, connectionString);
                if (attr != null && Session["RespondentID"] != null)
                {
                    int respondentID = (int)Session["RespondentID"];
                    SaveToRespondentAttributes(attr.AttributeID, attr.OptionID, respondentID, connectionString);
                }
            }
        }


        /// <summary>
        /// Retrieve AttributeID and OptionID from the database for a given question and selected value.
        /// </summary>
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

        /// <summary>
        /// Save respondent attributes to the database.
        /// </summary>
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

        /// <summary>
        /// Save answers for registered users in the database.
        /// </summary>
        private void SaveAllAnswers(SqlConnection conn, int respondentID)
        {
            // Retrieve answers from the session
            var answers = Session["Answers"] as Dictionary<int, string>;
            if (answers == null)
                return;

            // Save each answer to the Answers table
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

            // Save respondent attributes for each individual choice
            foreach (var answer in answers)
            {
                int questionID = answer.Key;
                string selectedValue = answer.Value;

                // Split values in case of multiple selections (multi-choice)
                var selectedValues = selectedValue.Split(',')
                                                  .Select(v => v.Trim())
                                                  .Where(v => !string.IsNullOrEmpty(v));

                foreach (var value in selectedValues)
                {
                    RespondentAttribute attr = GetAttributeAndOption(questionID, value, ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString);

                    if (attr != null)
                    {
                        SaveToRespondentAttributes(attr.AttributeID, attr.OptionID, respondentID, ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString);
                    }

                }
            }

            // Clear answers from session
            Session.Remove("Answers");
        }



        private string GetMacAddress()
        {
            string macAddress = string.Empty;
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up &&
                    nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    macAddress = nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            // Format it as 00:14:22:01:23:45
            if (!string.IsNullOrEmpty(macAddress))
            {
                macAddress = string.Join(":", Enumerable.Range(0, macAddress.Length / 2)
                                .Select(i => macAddress.Substring(i * 2, 2)));
            }

            return macAddress;
        }



        /// <summary>
        /// Records the session.
        /// </summary>

        private void SaveSessionDetails()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Retrieve respondent ID and IP address
                int respondentID = (int)Session["RespondentID"];
                string MacAddress = GetMacAddress();// Fetch client IP address

                TimeZoneInfo sydneyZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
                DateTime sydneyTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, sydneyZone);


                // Insert session details into the database
                string query = @"INSERT INTO Sessions (RespondentID, SessionDate, MacAddress, CreatedAt)
                         VALUES (@RespondentID, @SessionDate, @MacAddress, @CreatedAt)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RespondentID", respondentID);
                    cmd.Parameters.AddWithValue("@SessionDate", sydneyTime);
                    cmd.Parameters.AddWithValue("@MacAddress", MacAddress);
                    cmd.Parameters.AddWithValue("@CreatedAt", sydneyTime);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}