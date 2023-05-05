using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Admin
{
    public partial class Default : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                if (!Page.IsPostBack)
                {
                    // โหลดข้อมูล
                    LoadCountFile();
                }
            }
        }


        // --------------- Function
        // นับไฟล์ในเอกสารแจกจ่าย
        private void LoadCountFile()
        {
            sql = "SELECT COUNT(PublishDocFileID) FROM DC_PublishDocFile";
            int CountPublishDocFile = int.Parse(query.SelectAt(0, sql));
            sql = "SELECT COUNT(PublishTopicFileID) FROM DC_PublishTopicFile";
            int CountPublishTopicFile = int.Parse(query.SelectAt(0, sql));
            int CountAllFile = CountPublishDocFile + CountPublishTopicFile;
            LbCountFile.Text = CountAllFile.ToString();
        }
    }
}