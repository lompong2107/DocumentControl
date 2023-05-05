using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Admin
{
    public partial class PublishShowPDF : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            string FilePath = string.Empty;
            string FileName = string.Empty;
            string FileExtension = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["PublishTopicFileID"]))
            {
                string PublishTopicFileID = Request.QueryString["PublishTopicFileID"];
                sql = "SELECT FilePath, FileExtension FROM DC_PublishTopicFile WHERE PublishTopicFileID = " + PublishTopicFileID;
                FilePath = query.SelectAt(0, sql);
                FileExtension = query.SelectAt(1, sql);
                string[] SplitFilePath = FilePath.Split('\\');
                FileName = SplitFilePath.Last();
            }

            if (!string.IsNullOrEmpty(Request.QueryString["PublishDocFileID"]))
            {
                string PublishDocFileID = Request.QueryString["PublishDocFileID"];
                sql = "SELECT FilePath, FileExtension FROM DC_PublishDocFile WHERE PublishDocFileID = " + PublishDocFileID;
                FilePath = query.SelectAt(0, sql);
                FileExtension = query.SelectAt(1, sql);
                string[] SplitFilePath = FilePath.Split('\\');
                FileName = SplitFilePath.Last();
            }
            if (FileExtension.ToLower() == "png" || FileExtension.ToLower() == "jpg")
            {
                byte[] imgBytes = File.ReadAllBytes(FilePath);
                if (imgBytes.Length > 0)
                {
                    Response.Clear();
                    Response.ContentType = "image/" + FileExtension;
                    Response.BinaryWrite(imgBytes);
                    Response.End();
                }
            }
            else if (FileExtension.ToLower() == "pdf")
            {
                // #toolbar=0&navpanes=0
                byte[] pdfBytes = File.ReadAllBytes(FilePath);
                Response.Clear();
                Response.AddHeader("Content-Disposition", "inline; filename=FilePDF");
                Response.ContentType = "application/pdf;";
                //Response.WriteFile(FilePath);
                Response.BinaryWrite(pdfBytes);
                Response.End();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('นามสกุลไฟล์ไม่ถูกต้อง! (" + FileExtension + ")', '(สามารถเปิดได้เฉพาะ pdf, jpg, png)', 'error');", true);
            }
        }
    }
}