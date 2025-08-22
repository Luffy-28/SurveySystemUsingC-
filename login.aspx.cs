using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace YourNamespace
{
    public partial class Login : Page
    {
        /// <summary>
        /// Handles the page load event.
        /// Clears any previous error messages when the page is loaded.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        /// <summary>
        /// Handles the Register button click event.
        /// Redirects the user to the Register page for new user registration.
        /// </summary>
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            // Redirect the user to the Register page and include a query parameter
            // to indicate the redirection was initiated from the Login page.
            Response.Redirect("Register.aspx?fromLogin=true");
        }

        /// <summary>
        /// Handles the Continue as Anonymous button click event.
        /// Sets up the session for an anonymous user and redirects them to the survey.
        /// </summary>
        protected void btnContinueAnonymous_Click(object sender, EventArgs e)
        {
            // Clear any existing RespondentID to indicate this user is anonymous.
            Session["RespondentID"] = null;

            // Mark the session as anonymous.
            Session["IsAnonymous"] = true;

            // Redirect anonymous users to the survey page.
            Response.Redirect("Survey.aspx");
        }

        /// <summary>
        /// Handles the Login button click event.
        /// Redirects the user to the Staff Search page for staff functionality.
        /// </summary>
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Redirect staff to the Staff Search page
            Response.Redirect("StaffLogin.aspx");
        }
    }
}
