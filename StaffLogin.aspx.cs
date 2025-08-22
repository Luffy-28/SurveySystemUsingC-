using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace YourNamespace
{
    public partial class StaffLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Clear any previous error messages on page load
            lblError.Text = string.Empty;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Validate login credentials
            if (ValidateLogin(username, password))
            {
                // Redirect to the Staff Search page after successful login
                Response.Redirect("Dashboard.aspx");
            }
            else
            {
                // Display an error message if login fails
                lblError.Text = "Invalid username or password. Please try again.";
            }
        }

        private bool ValidateLogin(string username, string password)
        {
            bool isValid = false;

            // Retrieve connection string from Web.config
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // SQL query to retrieve the hashed password for the given username
                string query = "SELECT PasswordHash FROM Staff WHERE Username = @Username";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Use parameters to prevent SQL injection
                    cmd.Parameters.AddWithValue("@Username", username);

                    conn.Open();
                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        // Retrieve the hashed password from the database
                        string storedHashedPassword = result.ToString();

                        // Compare the entered password with the stored hash
                        if (VerifyPassword(password, storedHashedPassword))
                        {
                            isValid = true;
                        }
                    }
                }
            }

            return isValid;
        }

        private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            // Hash the entered password and compare it with the stored hash
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert entered password to bytes
                byte[] passwordBytes = Encoding.UTF8.GetBytes(enteredPassword);

                // Compute the hash of the entered password
                byte[] hashedBytes = sha256.ComputeHash(passwordBytes);

                // Convert the hash to a string
                string hashedEnteredPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                // Compare the entered password hash with the stored hash
                return hashedEnteredPassword == storedHashedPassword;
            }
        }
    }
}
