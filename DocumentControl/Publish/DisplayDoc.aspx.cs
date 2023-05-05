using DocumentControl.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Publish
{
    public partial class DisplayDoc : System.Web.UI.Page
    {
        QuerySQL query = new QuerySQL();
        string sql = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["PublishTopicID"] != null) {
                    string PublishTopicID = Request.QueryString["PublishTopicID"].ToString();
                    sql = $"SELECT TopicName FROM DC_PublishTopic WHERE PublishTopicID = {PublishTopicID}";
                    LbTopic.Text = query.SelectAt(0, sql);
                    LoadPublishDoc(PublishTopicID);
                }
            }
        }

        // --------------- GridView
        // โหลดข้อมูลเอกสาร
        private void LoadPublishDoc(string PublishTopicID)
        {
            string Search = string.Empty;
            if (TxtSearch.Text.Length > 0)
            {
                Search = $@" AND DC_PublishDoc.FileName like '%{TxtSearch.Text}%'";
            }
            sql = $@"SELECT DC_PublishDoc.PublishDocID, DC_PublishDoc.FileName, DC_PublishDocFile.PublishDocFileID, DC_PublishDocFile.FilePath, DC_PublishDocFile.PublishDate, DC_PublishDocFile.Revision, DC_PublishDocFile.FileExtension
                FROM DC_PublishDoc 
                LEFT JOIN DC_PublishDocFile ON DC_PublishDoc.PublishDocID = DC_PublishDocFile.PublishDocID 
                AND DC_PublishDocFile.PublishDocFileID IN (SELECT MAX(PublishDocFileID) FROM DC_PublishDocFile GROUP BY PublishDocID)
                WHERE DC_PublishDoc.PublishTopicID = {PublishTopicID} AND DC_PublishDoc.Status = 1 {Search} ORDER BY DC_PublishDoc.FileName";
            GVPublishDoc.DataSource = query.SelectTable(sql);
            GVPublishDoc.DataBind();
            if (GVPublishDoc.Rows.Count > 1)
            {
                GVPublishDoc.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        // ค้นหา
        protected void btnInvisibleSearch_Click(object sender, EventArgs e)
        {
            string PublishTopicID = Request.QueryString["PublishTopicID"].ToString();
            LoadPublishDoc(PublishTopicID);
        }
    }
}