using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System;
using System.IO;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace YourNamespace
{
    public partial class StaffSearch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStates();
                LoadRoomTypes();
                LoadInsuranceOptions();
                LoadRespondents();
                LoadAges();
                LoadGender();
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }



        private void LoadAges()
        {
            string cs = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = "SELECT DISTINCT OptionName FROM AttributeOptions WHERE AttributeID = 8 ORDER BY OptionName";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlAgeRange.DataSource = dt;
                    ddlAgeRange.DataTextField = "OptionName";
                    ddlAgeRange.DataValueField = "OptionName";
                    ddlAgeRange.DataBind();
                }
            }
            ddlAgeRange.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Age Range", ""));
        }

        private void LoadGender()
        {
            string cs = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = "SELECT DISTINCT OptionName FROM AttributeOptions WHERE AttributeID = 7 ORDER BY OptionName";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlGender.DataSource = dt;
                    ddlGender.DataTextField = "OptionName";
                    ddlGender.DataValueField = "OptionName";
                    ddlGender.DataBind();
                }
            }
            ddlGender.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Gender", ""));
        }


        private void LoadStates()
        {
            string cs = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = "SELECT DISTINCT OptionName FROM AttributeOptions WHERE AttributeID = 9 ORDER BY OptionName";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlState.DataSource = dt;
                    ddlState.DataTextField = "OptionName";
                    ddlState.DataValueField = "OptionName";
                    ddlState.DataBind();
                }
            }
            ddlState.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select State", ""));
        }

        private void LoadRoomTypes()
        {
            string cs = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = "SELECT DISTINCT OptionName FROM AttributeOptions WHERE AttributeID = 2 ORDER BY OptionName";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlRoomType.DataSource = dt;
                    ddlRoomType.DataTextField = "OptionName";
                    ddlRoomType.DataValueField = "OptionName";
                    ddlRoomType.DataBind();
                }
            }
            ddlRoomType.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Room Type", ""));
        }

        private void LoadInsuranceOptions()
        {
            string cs = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                string query = "SELECT DISTINCT OptionName FROM AttributeOptions WHERE AttributeID = 5 ORDER BY OptionName";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlInsurance.DataSource = dt;
                    ddlInsurance.DataTextField = "OptionName";
                    ddlInsurance.DataValueField = "OptionName";
                    ddlInsurance.DataBind();
                }
            }
            ddlInsurance.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Insurance", ""));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadRespondents();
        }

        private void LoadRespondents()
        {
            string cs = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                string query = @"
                                SELECT DISTINCT 
                                r.FirstName, 
                                r.LastName, 
                                r.Email AS EmailAddress,

                                (SELECT TOP 1 AnswerText FROM Answers WHERE RespondentID = r.RespondentID AND QuestionID = 3) AS RespondentState,
                                (SELECT TOP 1 AnswerText FROM Answers WHERE RespondentID = r.RespondentID AND QuestionID = 1) AS Gender,
                                (SELECT TOP 1 AnswerText FROM Answers WHERE RespondentID = r.RespondentID AND QuestionID = 2) AS AgeRange,

   
                                (SELECT TOP 1 AnswerText FROM Answers WHERE RespondentID = r.RespondentID AND QuestionID = 4) AS TypeOfService,
                                (SELECT TOP 1 AnswerText FROM Answers WHERE RespondentID = r.RespondentID AND QuestionID = 5) AS RoomType,
                                (SELECT TOP 1 AnswerText FROM Answers WHERE RespondentID = r.RespondentID AND QuestionID = 7) AS InRoomServices,
                                (SELECT TOP 1 AnswerText FROM Answers WHERE RespondentID = r.RespondentID AND QuestionID = 37) AS WifiOptions,
                                (SELECT TOP 1 AnswerText FROM Answers WHERE RespondentID = r.RespondentID AND QuestionID = 9) AS Insurance,
                                (SELECT TOP 1 AnswerText FROM Answers WHERE RespondentID = r.RespondentID AND QuestionID = 10) AS DischargePlans,                                          
                                (SELECT TOP 1 MacAddress FROM Sessions WHERE RespondentID = r.RespondentID) AS MacAddress,
                                (SELECT TOP 1 SessionDate FROM Sessions WHERE RespondentID = r.RespondentID) AS SessionDate

                                FROM Respondents r 
                                WHERE 1=1";



                SqlCommand cmd = new SqlCommand(query, conn);
                AddFilters(cmd);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    RespondentsGridView.DataSource = dt;
                    RespondentsGridView.DataBind();
                }
            }
        }

        private void AddFilters(SqlCommand cmd)
        {
            if (!string.IsNullOrEmpty(txtFirstName.Text))
            {
                cmd.CommandText += " AND r.FirstName LIKE @FirstName";
                cmd.Parameters.AddWithValue("@FirstName", "%" + txtFirstName.Text + "%");
            }
            if (!string.IsNullOrEmpty(txtLastName.Text))
            {
                cmd.CommandText += " AND r.LastName LIKE @LastName";
                cmd.Parameters.AddWithValue("@LastName", "%" + txtLastName.Text + "%");
            }
            if (!string.IsNullOrEmpty(txtEmail.Text))
            {
                cmd.CommandText += " AND r.Email LIKE @Email";
                cmd.Parameters.AddWithValue("@Email", "%" + txtEmail.Text + "%");
            }
            if (!string.IsNullOrEmpty(ddlState.SelectedValue))
            {
                cmd.CommandText += @" AND EXISTS (
            SELECT 1 FROM Answers a 
            WHERE a.RespondentID = r.RespondentID AND a.QuestionID = 3 AND a.AnswerText = @State)";
                cmd.Parameters.AddWithValue("@State", ddlState.SelectedValue);
            }
            if (!string.IsNullOrEmpty(ddlGender.SelectedValue))
            {
                cmd.CommandText += @" AND EXISTS (
            SELECT 1 FROM Answers a 
            WHERE a.RespondentID = r.RespondentID AND a.QuestionID = 1 AND a.AnswerText = @Gender)";
                cmd.Parameters.AddWithValue("@Gender", ddlGender.SelectedValue);
            }
            if (!string.IsNullOrEmpty(ddlAgeRange.SelectedValue))
            {
                cmd.CommandText += @" AND EXISTS (
            SELECT 1 FROM Answers a 
            WHERE a.RespondentID = r.RespondentID AND a.QuestionID = 2 AND a.AnswerText = @AgeRange)";
                cmd.Parameters.AddWithValue("@AgeRange", ddlAgeRange.SelectedValue);
            }
            if (!string.IsNullOrEmpty(ddlRoomType.SelectedValue))
            {
                cmd.CommandText += @" AND EXISTS (
            SELECT 1 FROM Answers a 
            WHERE a.RespondentID = r.RespondentID AND a.QuestionID = 5 AND a.AnswerText LIKE '%' + @RoomType + '%')";
                cmd.Parameters.AddWithValue("@RoomType", ddlRoomType.SelectedValue);
            }
            if (!string.IsNullOrEmpty(ddlInsurance.SelectedValue))
            {
                cmd.CommandText += @" AND EXISTS (
            SELECT 1 FROM Answers a 
            WHERE a.RespondentID = r.RespondentID AND a.QuestionID = 9 AND a.AnswerText LIKE '%' + @Insurance + '%')";
                cmd.Parameters.AddWithValue("@Insurance", ddlInsurance.SelectedValue);
            }
        }




        // ---------------- Export to Excel ----------------
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            // Disable paging if needed
            RespondentsGridView.AllowPaging = false;
            LoadRespondents(); // Rebind data without paging

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Respondents.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    RespondentsGridView.RenderControl(hw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Required for exporting GridView
        }

        // ---------------- Export to PDF ----------------
        protected void btnExportPdf_Click(object sender, EventArgs e)
        {
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Respondents.pdf");
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

            // Create PDF document using iTextSharp
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Create PDF table with number of columns equal to GridView columns
            PdfPTable pdfTable = new PdfPTable(RespondentsGridView.Columns.Count);
            pdfTable.WidthPercentage = 100;

            // Add header cells from GridView
            foreach (DataControlField column in RespondentsGridView.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                cell.BackgroundColor = new BaseColor(240, 240, 240);
                pdfTable.AddCell(cell);
            }

            // Add data rows
            foreach (GridViewRow row in RespondentsGridView.Rows)
            {
                foreach (TableCell cell in row.Cells)
                {
                    pdfTable.AddCell(cell.Text.Trim());
                }
            }

            pdfDoc.Add(pdfTable);
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
        }
    }
}


