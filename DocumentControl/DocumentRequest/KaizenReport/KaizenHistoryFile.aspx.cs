using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest.KaizenReport
{
    public partial class KaizenHistoryFile : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["KaizenID"]))
                {
                    string KaizenID = Request.QueryString["KaizenID"];
                    LoadKaizenDoc(KaizenID);
                }
            }
        }

        private void LoadKaizenDoc(string KaizenID)
        {
            sql = $@"SELECT KaizenDocID, DateCreate, FilePath FROM DC_KaizenDoc WHERE KaizenID = {KaizenID} AND Status = 1 ORDER BY DateCreate DESC";
            GVKaizenDoc.DataSource = query.SelectTable(sql);
            GVKaizenDoc.DataBind();
            if (GVKaizenDoc.Rows.Count > 1)
            {
                GVKaizenDoc.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        protected void GVKaizenDoc_RowCommand(object sender, GridViewCommandEventArgs e)
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
    }
}