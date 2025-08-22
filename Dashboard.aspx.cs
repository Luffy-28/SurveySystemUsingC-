using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication10
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddQuestion.aspx"); // update if your URL differs
        }

        protected void btnStaffSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("StaffSearch.aspx");
        }
    }
}