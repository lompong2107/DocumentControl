using DocumentControl.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Publish
{
    public partial class PublishHistoryFile : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["PublishTopicID"]))
                {
                    string PublishTopicID = Request.QueryString["PublishTopicID"];
                    LoadPublishTopicFile(PublishTopicID);
                }
                if (!string.IsNullOrEmpty(Request.QueryString["PublishDocID"]))
                {
                    string PublishDocID = Request.QueryString["PublishDocID"];
                    LoadPublishDocFile(PublishDocID);
                }
            }
        }

        private void LoadPublishTopicFile(string PublishTopicID)
        {
            sql = $@"SELECT DC_PublishTopic.TopicName, DC_PublishTopicFile.PublishTopicFileID, DC_PublishTopicFile.FilePath, DC_PublishTopicFile.PublishDate, DC_PublishTopicFile.FileExtension, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name
            FROM DC_PublishTopic 
            LEFT JOIN DC_PublishTopicFile ON DC_PublishTopic.PublishTopicID = DC_PublishTopicFile.PublishTopicID 
            LEFT JOIN F2_Users ON DC_PublishTopicFile.UserID = F2_Users.UserID
            WHERE DC_PublishTopic.PublishTopicID = {PublishTopicID} AND DC_PublishTopicFile.Status = 1 
            ORDER BY DC_PublishTopicFile.PublishDate DESC";
            GVPublishTopicFile.DataSource = query.SelectTable(sql);
            GVPublishTopicFile.DataBind();
            if (GVPublishTopicFile.Rows.Count > 1)
            {
                GVPublishTopicFile.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        protected void GVPublishTopicFile_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string Btn = e.CommandName;
            string Value = e.CommandArgument.ToString();
            if (Btn == "BtnDownload")
            {
                string[] SplitFilePath = Value.Split('\\');
                string FileName = SplitFilePath.Last();

                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName);
                Response.ContentType = "application/pdf";
                Response.WriteFile(Value);
                Response.End();
            }
        }

        private void LoadPublishDocFile(string PublishDocID)
        {
            sql = $@"SELECT DC_PublishDocFile.PublishDocFileID, DC_PublishDoc.FileName, DC_PublishDocFile.FilePath, DC_PublishDocFile.PublishDate, DC_PublishDocFile.Revision, DC_PublishDocFile.FileExtension, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name
            FROM DC_PublishDoc 
            LEFT JOIN DC_PublishDocFile ON DC_PublishDoc.PublishDocID = DC_PublishDocFile.PublishDocID 
            LEFT JOIN F2_Users ON DC_PublishDocFile.UserID = F2_Users.UserID
            WHERE DC_PublishDoc.PublishDocID = {PublishDocID} AND DC_PublishDocFile.Status = 1 
            ORDER BY DC_PublishDocFile.PublishDate DESC";
            GVPublishDocFile.DataSource = query.SelectTable(sql);
            GVPublishDocFile.DataBind();
            if (GVPublishDocFile.Rows.Count > 1)
            {
                GVPublishDocFile.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        protected void GVPublishDocFile_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string Btn = e.CommandName;
            string Value = e.CommandArgument.ToString();

            if (Btn == "LinkBtnPublishDoc")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "New_Window", "window.open('ShowPDF.aspx?PublishDocFileID=" + Value + "', null, 'height=500,width=800,status=yes,toolbar=yes,menubar=yes,location=no');", true);
            }
            else if (Btn == "BtnDownload")
            {
                string[] SplitFilePath = Value.Split('\\');
                string FileName = SplitFilePath.Last();

                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName);
                Response.ContentType = "application/pdf";
                Response.WriteFile(Value);
                Response.End();
            }
        }
    }
}