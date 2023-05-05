using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Admin
{
    public partial class Publish : System.Web.UI.Page
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
                    LoadMainPublishTopic();
                }
            }
        }

        // --------------- TreeView
        // โหลดหัวข้อหลัก

        protected void LoadMainPublishTopic()
        {
            TreeViewPublishTopic.Nodes.Clear();
            sql = "SELECT PublishTopicID, SubPublishTopicID, TopicName, Status FROM DC_PublishTopic ORDER BY TopicName";
            DataTable dt = query.SelectTable(sql);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            ds.Relations.Add("ChildRows", ds.Tables[0].Columns["PublishTopicID"], ds.Tables[0].Columns["SubPublishTopicID"], false);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string MainPublishTopicID = row["PublishTopicID"].ToString();
                string SubPublishTopicID = row["SubPublishTopicID"].ToString();
                string MainTopicName = row["TopicName"].ToString();
                string Status = row["Status"].ToString();
                if (SubPublishTopicID == "0")
                {
                    TreeNode tn = new TreeNode();
                    // ตรวจสอบว่าหัวข้อถูกลบหรือไม่
                    if (Status == "0")
                    {
                        tn.Text = "<span class='text-danger'>" + MainTopicName + "</span>";
                    }
                    else
                    {
                        tn.Text = MainTopicName;
                    }
                    tn.Value = MainPublishTopicID;
                    tn.SelectAction = TreeNodeSelectAction.SelectExpand;
                    string PublishTopicIDSelected = HFNodeSelected.Value;
                    if (PublishTopicIDSelected == MainPublishTopicID)
                    {
                        tn.Select();
                    }
                    // Call Recursive function
                    GetChildRows(row, tn);
                    TreeViewPublishTopic.Nodes.Add(tn);
                }
            }
        }
        // โหลดหัวข้อย่อย
        private void GetChildRows(DataRow dataRow, TreeNode treeNode)
        {
            DataRow[] childRows = dataRow.GetChildRows("ChildRows");
            foreach (DataRow childRow in childRows)
            {
                TreeNode childTreeNode = new TreeNode();
                if (childRow["Status"].ToString() == "0")
                {
                    childTreeNode.Text = "<span class='text-danger'>" + childRow["TopicName"].ToString() + "</span>";
                }
                else
                {
                    childTreeNode.Text = childRow["TopicName"].ToString();
                }
                childTreeNode.Value = childRow["PublishTopicID"].ToString();
                childTreeNode.SelectAction = TreeNodeSelectAction.SelectExpand;
                string PublishTopicIDSelected = HFNodeSelected.Value;
                if (PublishTopicIDSelected == childRow["PublishTopicID"].ToString())
                {
                    childTreeNode.Select();
                }
                treeNode.ChildNodes.Add(childTreeNode);
                if (childRow.GetChildRows("ChildRows").Length > 0)
                {
                    GetChildRows(childRow, childTreeNode);
                }
            }
        }

        // Method เมื่อกดเลือกหัวข้อ
        protected void TreeViewPublishTopic_SelectedNodeChanged(object sender, EventArgs e)
        {
            string PublishTopicID = TreeViewPublishTopic.SelectedValue;
            // ตรวจสอบว่ามีการเลือกหัวข้อหรือไม่
            if (TreeViewPublishTopic.SelectedNode.Selected)
            {
                PanelDoc.Visible = true;
                PanelSelectedPublishTopicEmpty.Visible = false;
                LinkBtnPublishTopic.Text = TreeViewPublishTopic.SelectedNode.Text;    // Set ชื่อหัวข้อ
                // ตรวจสอบว่ามีไฟล์ในหัวข้อนี้หรือไม่
                sql = "SELECT PublishTopicFileID FROM DC_PublishTopicFile WHERE PublishTopicID = " + PublishTopicID + " ORDER BY PublishDate DESC";
                DataTable dt = query.SelectTable(sql);
                if (dt.Rows.Count > 0)
                {
                    LinkBtnPublishTopic.Enabled = true;
                    LinkBtnPublishTopic.CssClass = "";
                    LinkBtnPublishTopic.CommandArgument = dt.Rows[0]["PublishTopicFileID"].ToString();
                }
                else
                {
                    LinkBtnPublishTopic.Enabled = false;
                    LinkBtnPublishTopic.CssClass = "text-decoration-none text-black";
                }
                // ตรวจสอบว่าหัวข้อนี้ถูกลบหรือไม่
                sql = "SELECT Status FROM DC_PublishTopic WHERE PublishTopicID = " + PublishTopicID;
                string Status = query.SelectAt(0, sql);
                if (Status == "0")
                {
                    BtnReturn.Visible = true;
                    BtnReturn.CommandArgument = PublishTopicID;
                }
                else
                {
                    BtnReturn.Visible = false;
                }
                // โหลดข้อมูลเอกสารในหัวข้อ
                LoadPublishDoc(TreeViewPublishTopic.SelectedValue);

                // เก็บค่าหัวข้อ
                HFNodeSelected.Value = PublishTopicID;
            }
        }


        // --------------- GridView
        // โหลดข้อมูลเอกสาร
        private void LoadPublishDoc(string PublishTopicID)
        {
            sql = @"SELECT DC_PublishDoc.PublishDocID, DC_PublishDoc.FileName, DC_PublishDocFile.PublishDocFileID, DC_PublishDocFile.FilePath, DC_PublishDocFile.PublishDate, DC_PublishDoc.Status
                FROM DC_PublishDoc 
                LEFT JOIN DC_PublishDocFile ON DC_PublishDoc.PublishDocID = DC_PublishDocFile.PublishDocID 
                AND DC_PublishDocFile.PublishDocFileID IN (SELECT MAX(PublishDocFileID) FROM DC_PublishDocFile GROUP BY PublishDocID)
                WHERE DC_PublishDoc.PublishTopicID = " + PublishTopicID + " ORDER BY DC_PublishDoc.FileName";
            GVPublishDoc.DataSource = query.SelectTable(sql);
            GVPublishDoc.DataBind();
        }
        // Method การกดปุ่มใน GridView
        protected void GVPublishDoc_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string Btn = e.CommandName;
            string PublishDocID = e.CommandArgument.ToString();
            // ปุ่มอัปเดตสถานะเป็น 1
            if (Btn == "BtnReturn")
            {
                try
                {
                    sql = "UPDATE DC_PublishDoc SET Status = 1 WHERE PublishDocID = " + PublishDocID;
                    if (query.Excute(sql))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'อัปเดตข้อมูลสำเร็จ', 'success');", true);
                        string PublishTopicID = HFNodeSelected.Value;
                        LoadPublishDoc(PublishTopicID);
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
                }
            }
        }
        // Method ในระหว่างการโหลดแถว GridView
        protected void GVPublishDoc_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // ตรวจสอบว่าไฟล์ถูกลบหรือไม่
                string Status = DataBinder.Eval(e.Row.DataItem, "Status").ToString();
                ImageButton BtnReturn = e.Row.FindControl("BtnReturn") as ImageButton;
                if (Status == "0")
                {
                    BtnReturn.Visible = true;
                }
                else
                {
                    BtnReturn.Visible = false;
                }
            }
        }


        // --------------- Button
        // เปิดไฟล์ที่หัวข้อ
        protected void LinkBtnPublishTopic_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "New_Tab", "window.open('PublishShowPDF.aspx?PublishTopicFileID=" + LinkBtnPublishTopic.CommandArgument + "', '_blank');", true);
        }
        protected void BtnReturn_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string PublishTopicID = BtnReturn.CommandArgument;
                sql = "UPDATE DC_PublishTopic SET Status = 1 WHERE PublishTopicID = " + PublishTopicID;
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'อัปเดตข้อมูลสำเร็จ', 'success');", true);
                    // โหลดข้อมูลใหม่
                    LoadMainPublishTopic();
                    TreeViewPublishTopic_SelectedNodeChanged(null, null);
                    // Expand Parent ของ Node ที่เลือก
                    ExpandToRoot(TreeViewPublishTopic.SelectedNode.Parent);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // ทำการ Expand Parent ของ Node ที่เลือก
        private void ExpandToRoot(TreeNode node)
        {
            if (node != null)
            {
                node.Expand();
                if (node.Parent != null)
                {
                    ExpandToRoot(node.Parent);
                }
            }
        }

        // แสดงหัวข้อ ซ่อนหัวข้อ ทั้งหมด
        protected void CBExpandAll_CheckedChanged(object sender, EventArgs e)
        {
            if (CBExpandAll.Checked)
            {
                TreeViewPublishTopic.ExpandAll();
            }
            else
            {
                TreeViewPublishTopic.CollapseAll();
            }
        }
    }
}