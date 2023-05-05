using DocumentControl.Admin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Media.Animation;
using System.Xml.Linq;

namespace DocumentControl.Publish
{
    public partial class Publish : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        StringSpecialClass StringSpecial = new StringSpecialClass();
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LastPage"] = "~/Publish/Publish.aspx";
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    // โหลดข้อมูล
                    // หากสงสัยการใช้ TreeView ให้ดูที่นี่
                    // - https://csharp-video-tutorials.blogspot.com/2013/11/part-163-dynamically-adding-treenodes.html
                    // - https://youtu.be/AmajKLs54zA
                    LoadMainPublishTopic();
                    // ตรวจสอบว่ามีสิทธิ์ไหม
                    if (!CheckPermissionAll())
                    {
                        accordionFlushAddPublishTopic.Visible = false;
                    }
                }
            }
        }


        // --------------- TreeView
        // โหลดหัวข้อหลัก
        protected void LoadMainPublishTopic()
        {
            try
            {
                TreeViewPublishTopic.Nodes.Clear();
                string UserID = Session["UserID"].ToString();
                // ดึงข้อมูลหัวข้อ
                // เงื่อนไข WHERE คือ
                // หัวข้อสถานะ 1 และ (หัวข้อเป็น Global หรือข้อหัวย่อย หรือ ((หัวข้อเป็น Assign และ (ต้องมีสิทธิ์เข้าถึงหัวข้อนั้น และ ไม่มีสิทธิ์ Full Control) หรือ เป็นคนสร้าง) หรือ (หัวข้อเป็น Assign และ มีสิทธิ์ Full Control))
                // ** ผมต้องการรกรองข้อมูลที่ผู้ใช้นั้นเข้าถึงได้ และตามสิทธิ์ เพื่อที่จะได้ไม่ต้องไปเช็คทีหลังกลัวมันทำงานเยอะแล้วมันทำงานช้า
                sql = $@"
                SELECT PublishTopicID, SubPublishTopicID, TopicName, TopicType, UserID
                FROM DC_PublishTopic
                WHERE Status = 1 AND 
                (
	                TopicType IN (0, 1) OR 
	                (
                        (TopicType = 2 AND 
		                ((PublishTopicID IN (SELECT PublishTopicID FROM DC_PublishTopicUserAccess WHERE UserID = {UserID}) AND (SELECT COUNT(PermissionUserID) FROM DC_PermissionUser WHERE PermissionID = 1 AND UserID = {UserID}) = 0) OR UserID = {UserID})) 
                        OR 
	                    (TopicType = 2 AND 
		                (SELECT COUNT(PermissionUserID) FROM DC_PermissionUser WHERE PermissionID = 1 AND UserID = {UserID}) > 0)
                    )
                )
                ORDER BY TopicName
                ";
                DataTable dt = query.SelectTable(sql);
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                // เชื่อมความสัมพันธ์ระหว่างหัวข้อหลักกับหัวข้อย่อย
                ds.Relations.Add("ChildRows", ds.Tables[0].Columns["PublishTopicID"], ds.Tables[0].Columns["SubPublishTopicID"], false);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string MainPublishTopicID = row["PublishTopicID"].ToString(); // รหัสหัวข้อหลัก
                    string SubPublishTopicID = row["SubPublishTopicID"].ToString();   // รหัสหัวข้อย่อย
                    string MainTopicName = row["TopicName"].ToString();
                    string TopicType = row["TopicType"].ToString(); // 1 = Global, Assign
                    string UserIDCreated = row["UserID"].ToString();    // ผู้ใช้ที่สร้างหัวข้อม
                    // SubPublishTopicID ค่า 0 = เป็นหัวข้อหลัก
                    if (SubPublishTopicID == "0")
                    {
                        // Set Color
                        string Color = string.Empty;
                        if (UserIDCreated == UserID)
                        {
                            // เป็นเจ้าของหัวข้อจะเป็นสีส้ม
                            Color = "style='color: orange;'";
                        }
                        else
                        {
                            Color = "style='color: gray;'";
                        }
                        // Set Image
                        string Image = string.Empty;
                        if (TopicType == "1")
                        {
                            // TopicType = ทุกคนจะเป็นรูปโลก
                            // Image Global
                            Image = $"<i class=\"fas fa-globe\" {Color}></i>";
                        }
                        else
                        {
                            // Image Assign
                            Image = $"<i class=\"fas fa-user-lock\" {Color}></i>";
                        }
                        TreeNode tn = new TreeNode();
                        tn.Value = MainPublishTopicID;
                        tn.Text = $@"<div class='d-flex align-items-center'>{Image}<span class='ms-1'>{MainTopicName}</span></div>";
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
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // Method โหลดหัวข้อย่อย
        private void GetChildRows(DataRow dataRow, TreeNode treeNode)
        {
            try
            {
                DataRow[] childRows = dataRow.GetChildRows("ChildRows");
                foreach (DataRow childRow in childRows)
                {
                    string UserID = Session["UserID"].ToString();
                    string PublishTopicID = childRow["PublishTopicID"].ToString(); // รหัสหัวข้อหลัก
                    string TopicName = childRow["TopicName"].ToString();
                    string UserIDCreated = childRow["UserID"].ToString();    // ผู้ใช้ที่สร้างหัวข้อม

                    // Set Image And Color
                    string Image = string.Empty;
                    if (UserIDCreated == UserID)
                    {
                        // เป็นเจ้าของจะเป็นสีส้ม
                        Image = $"<i class=\"fas fa-user\" style='color: orange;'></i>";
                    }

                    TreeNode childTreeNode = new TreeNode();
                    childTreeNode.Text = $@"<div class='d-flex align-items-center'>{Image}<span class='ms-1'>{TopicName}</span></div>";
                    childTreeNode.Value = PublishTopicID;
                    childTreeNode.SelectAction = TreeNodeSelectAction.SelectExpand;

                    string PublishTopicIDSelected = HFNodeSelected.Value;
                    if (PublishTopicIDSelected == PublishTopicID)
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
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // Method เมื่อกดเลือกหัวข้อ
        protected void TreeViewPublishTopic_SelectedNodeChanged(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    string PublishTopicID = TreeViewPublishTopic.SelectedValue;
                    // ตรวจสอบว่ามีการเลือกหัวข้อหรือไม่
                    if (TreeViewPublishTopic.SelectedNode.Selected)
                    {
                        CBMainPublishTopic.Enabled = true;
                        CBMainPublishTopic.Checked = false;    // Uncheck หัวข้อหลัก
                        RequiredFieldValidatorPublishTopicUserAccess.Enabled = false;
                        PanelPublishTopicType.Visible = false;
                        PanelPublishDoc.Visible = true;    // แสดงข้อมูลฝั่งขวาอ่ะ
                        PanelSelectedPublishTopicEmpty.Visible = false;    // และซ่อนข้อความแสดง ยังไม่เลือกหัวข้อ
                        HLPublishTopic.Text = TreeViewPublishTopic.SelectedNode.Text; // Set ชื่อหัวข้อ
                                                                                      // ตรวจสอบว่ามีไฟล์ในหัวข้อนี้หรือไม่
                        sql = $"SELECT FilePath, PublishTopicFileID, FileExtension FROM DC_PublishTopicFile WHERE PublishTopicID = {PublishTopicID} AND Status = 1 ORDER BY PublishDate DESC";
                        DataTable dt = query.SelectTable(sql);
                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Rows[0]["FileExtension"].ToString().ToLower() != "pdf" && dt.Rows[0]["FileExtension"].ToString().ToLower() != "jpg" && dt.Rows[0]["FileExtension"].ToString().ToLower() != "png")
                            {
                                HLPublishTopic.Enabled = false;
                                HLPublishTopic.CssClass = "link-dark";
                            }
                            else
                            {
                                HLPublishTopic.Enabled = true;
                                HLPublishTopic.CssClass = "";
                                HLPublishTopic.NavigateUrl = $"ShowPDF.aspx?PublishTopicFileID={dt.Rows[0]["PublishTopicFileID"]}";
                            }
                            BtnDownload.Visible = true;
                            BtnLink.Visible = true;
                            string FilePath = dt.Rows[0]["FilePath"].ToString();
                            string[] FilePathSplit = FilePath.Split('\\');
                            string GetFileName = FilePathSplit.Last();
                            //int FindExtensionIndex = GetFileName.LastIndexOf('.');
                            //string GetFileNameWithoutExtension = GetFileName.Remove(FindExtensionIndex, GetFileName.Length - FindExtensionIndex);
                            LbShowFileNameReal.Visible = true;
                            //LbShowFileNameReal.Text = "(" + GetFileNameWithoutExtension + ")";
                            LbShowFileNameReal.Text = "(" + GetFileName + ")";
                            BtnDownload.CommandArgument = FilePath;
                            BtnLink.OnClientClick = string.Format("CopyToClipboard(\"{0}\"); return false;", FilePath.Replace("\\", "\\\\"));
                            if (dt.Rows.Count == 1)
                            {
                                BtnHistory.Visible = false;
                            }
                            else
                            {
                                BtnHistory.Visible = true;
                                BtnHistory.OnClientClick = $"window.open('PublishHistoryFile.aspx?PublishTopicID={PublishTopicID}', '_blank', 'location=yes,height=570,width=600,scrollbars=yes,status=yes');";
                            }
                        }
                        else
                        {
                            HLPublishTopic.Enabled = false;
                            HLPublishTopic.CssClass = "text-decoration-none text-black";
                            BtnDownload.Visible = false;
                            BtnLink.Visible = false;
                            BtnHistory.Visible = false;
                            LbShowFileNameReal.Text = string.Empty;
                            LbShowFileNameReal.Visible = false;
                        }

                        // ตรวจสอบมีสิทธิ์ Full Control ไหม หรือ (ถ้ามีสิทธิ์ Ownership ต้อง เป็นเจ้าของไหม)
                        string UserID = Session["UserID"].ToString();
                        sql = $@"SELECT PublishTopicID, SubPublishTopicID FROM DC_PublishTopic WHERE PublishTopicID = {PublishTopicID} AND UserID = {UserID}";
                        bool CheckUserCreated = query.CheckRow(sql);
                        if (CheckPermissionFullControl() || (CheckPermissionOwnership() && CheckUserCreated))
                        {
                            BtnUploadPublishTopicShow.Visible = true;
                            BtnEditShow.Visible = true;
                            BtnDelete.Visible = true;
                            BtnQRCode.Visible = true;

                        }
                        else
                        {
                            BtnUploadPublishTopicShow.Visible = false;
                            BtnEditShow.Visible = false;
                            BtnDelete.Visible = false;
                            BtnHistory.Visible = false;
                            BtnQRCode.Visible = false;
                        }
                        // ตรวจสอบว่ามีสิทธิ์อะไรไหม
                        if (CheckPermissionAll())
                        {
                            PanelAddPublishDoc.Visible = true;
                        }
                        else
                        {
                            PanelAddPublishDoc.Visible = false;
                        }
                        // โหลดข้อมูลเอกสารในหัวข้อ
                        LoadPublishDoc(TreeViewPublishTopic.SelectedValue);

                        // เก็บค่าหัวข้อ
                        HFNodeSelected.Value = PublishTopicID;
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }


        // --------------- Button
        // เพิ่มหัวข้อใหม่
        protected void BtnAddPublishTopic_Click(object sender, EventArgs e)
        {
            try
            {
                string UserID = Session["UserID"].ToString();
                string PublishTopicID = HFNodeSelected.Value;
                string PublishTopicName = TxtPublishTopicName.Text;
                // ตรวจสอบว่ามีการกรอกชื่อหัวข้อหรือไม่
                if (PublishTopicName.Length == 0 || string.IsNullOrEmpty(PublishTopicName))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณากรอกข้อมูล.', 'warning');", true);
                    return;
                }

                PublishTopicName = PublishTopicName.Replace("'", "''");
                // ตรวจสอบว่ามีการเลือกหัวข้อหรือไม่
                if (PublishTopicID.Length == 0)
                {
                    string PublishTopicType = RBListPublishTopicType.SelectedValue;
                    sql = $"INSERT INTO DC_PublishTopic (TopicName, SubPublishTopicID, UserID, TopicType) VALUES ('{PublishTopicName}', 0, {UserID}, {PublishTopicType})";
                    if (query.Excute(sql))
                    {
                        // ถ้าเลือกเป็นหัวข้อ Assign ให้ทำงาน If นี้
                        if (PublishTopicType == "2")
                        {
                            sql = $@"SELECT TOP 1 PublishTopicID FROM DC_PublishTopic WHERE UserID = {UserID} ORDER BY PublishTopicID DESC";
                            PublishTopicID = query.SelectAt(0, sql);
                            List<String> ListUserIDAll = HFUserIDSelectedAccess.Value.Split(',').ToList();
                            foreach (string UserIDSelected in ListUserIDAll)
                            {
                                sql = $@"INSERT INTO DC_PublishTopicUserAccess (PublishTopicID, UserID) VALUES ({PublishTopicID}, {UserIDSelected})";
                                query.Excute(sql);
                            }
                        }
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'เพิ่มหัวข้อสำเร็จ', 'success');", true);
                    }
                }
                else if (PublishTopicID.Length > 0)
                {
                    sql = $"INSERT INTO DC_PublishTopic (TopicName, SubPublishTopicID, UserID, TopicType) VALUES ('{PublishTopicName}', {PublishTopicID}, {UserID}, 0)";
                    if (query.Excute(sql))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'เพิ่มหัวข้อสำเร็จ', 'success');", true);
                    }
                }
                // Set Default Data
                TxtPublishTopicName.Text = string.Empty;
                RBListPublishTopicType.SelectedValue = "1";
                ListBoxPublishTopicUserAccess.ClearSelection();
                // โหลดข้อมูลหัวข้อ
                LoadMainPublishTopic();
                if (PublishTopicID.Length > 0)
                {
                    // Expand Parent ของ Node ที่เลือก
                    ExpandToRoot(TreeViewPublishTopic.SelectedNode.Parent);
                    // Expand Node ที่เลือกด้วย
                    TreeViewPublishTopic.SelectedNode.Expand();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // ปุ่มดาวน์โหลดไฟล์ที่หัวข้อ
        protected void BtnDownload_Click(object sender, ImageClickEventArgs e)
        {
            string Value = BtnDownload.CommandArgument;
            string[] SplitFilePath = Value.Split('\\');
            string FileName = SplitFilePath.Last();

            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName);
            Response.ContentType = "application/pdf";
            Response.WriteFile(Value);
            Response.End();
        }

        // ปุ่มลบหัวข้อ
        protected void BtnDelete_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string UserID = Session["UserID"].ToString();
                string PublishTopicID = HFNodeSelected.Value;
                sql = $"UPDATE DC_PublishTopic SET UserIDUpdate = {UserID}, DateUpdate = GETDATE(), Status = 0 WHERE PublishTopicID = {PublishTopicID}";
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'ลบหัวข้อสำเร็จ', 'success');", true);
                    LoadMainPublishTopic();
                    PanelPublishDoc.Visible = false;
                    PanelSelectedPublishTopicEmpty.Visible = true;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // ปุ่มเพิ่มไฟล์เอกสารใหม่
        protected void BtnAddPublishDoc_Click(object sender, EventArgs e)
        {
            string PublishTopicID = HFNodeSelected.Value;
            string PublishDocFileName = TxtPublishDocFileName.Text;
            string Revision = TxtPublishDocRevision.Text;
            // ตรวจสอบว่ากรอกชื่อไฟล์หรือไม่
            if (PublishDocFileName.Length > 0 && Revision.Length > 0)
            {
                // ตรวจสอบว่ามีการอัปโหลดไฟล์หรือไม่
                if (FileUploadPublishDoc.HasFile)
                {
                    try
                    {
                        string UserID = Session["UserID"].ToString();
                        string FileName = Path.GetFileNameWithoutExtension(FileUploadPublishDoc.PostedFile.FileName);
                        string Extension = Path.GetExtension(FileUploadPublishDoc.PostedFile.FileName);
                        string FullFileName = FileName + Extension;
                        PublishDocFileName = PublishDocFileName.Replace("'", "''");
                        string FilePath = "\\\\192.168.0.100\\PDoc\\" + UserID + "\\" + DateTime.Now.ToString("yyyy-MM-dd HHmmss");
                        // ตรวจสอบว่ามีโฟลเดอร์นี้หรือไม่
                        if (!Directory.Exists(FilePath))
                        {
                            Directory.CreateDirectory(FilePath);
                        }
                        string FullFilePath = Path.Combine(FilePath, FullFileName);
                        FileUploadPublishDoc.SaveAs(FullFilePath);
                        FullFilePath = FullFilePath.Replace("'", "''");
                        // Insert New Data
                        sql = $"INSERT INTO DC_PublishDoc (UserID, PublishTopicID, FileName) VALUES ({UserID}, {PublishTopicID}, '{PublishDocFileName}')";
                        query.Excute(sql);
                        // Select Last Insert
                        sql = $"SELECT TOP 1 PublishDocID FROM DC_PublishDoc WHERE UserID = {UserID} ORDER BY PublishDocID DESC";
                        string PublicDocID = query.SelectAt(0, sql);
                        sql = $"INSERT INTO DC_PublishDocFile (PublishDocID ,UserID, FileExtension, FilePath, Revision) VALUES ({PublicDocID}, {UserID}, '{Extension.Replace(".", "")}', '{FullFilePath}', '{Revision}')";
                        if (query.Excute(sql))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'เพิ่มไฟล์สำเร็จ', 'success');", true);
                            // เปิดฟอร์มเผยแพร่เอกสารไว้เหมือนเดิม
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Show Accordion", "$(document).ready(function() { $('#accordionFlushPublishDoc .collapse').collapse('show'); })", true);
                            TxtPublishDocFileName.Text = string.Empty;
                            TxtPublishDocRevision.Text = string.Empty;
                            LoadPublishDoc(PublishTopicID);
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ข้อมูลไม่ถูกต้อง!', 'กรุณาเลือกไฟล์.', 'warning');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ข้อมูลไม่ถูกต้อง!', 'กรุณากรอกข้อมูลให้ครบ.', 'warning');", true);
            }
        }

        // เปิด Modal Upload ไฟล์ที่หัวข้อ
        protected void BtnUploadPublishTopicShow_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                sql = $"SELECT TopicName FROM DC_PublishTopic WHERE PublishTopicID = {HFNodeSelected.Value}";
                LBTitleModal.Text = query.SelectAt(0, sql);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "OpenModalPublishTopicUploadFile()", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // เปิด Modal แก้ไขชื่อหัวข้อ
        protected void BtnEditShow_Click(object sender, ImageClickEventArgs e)
        {
            sql = $"SELECT TopicName FROM DC_PublishTopic WHERE PublishTopicID = {HFNodeSelected.Value}";
            TxtPublishTopicRename.Text = query.SelectAt(0, sql);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "OpenModalPublishTopicRename()", true);
        }

        // อัปโหลดไฟล์ที่หัวข้อ
        protected void BtnUpdatePublishTopic_Click(object sender, EventArgs e)
        {
            try
            {
                string PublishTopicID = HFNodeSelected.Value;
                string UserID = Session["UserID"].ToString();
                string FileName = Path.GetFileNameWithoutExtension(FileUploadPublishTopic.PostedFile.FileName);
                string Extension = Path.GetExtension(FileUploadPublishTopic.PostedFile.FileName);
                string FullFileName = FileName + Extension;
                string FilePath = "\\\\192.168.0.100\\PDoc\\" + UserID + "\\" + DateTime.Now.ToString("yyyy-MM-dd HHmmss");
                // ตรวจสอบว่ามีโฟลเดอร์นี้หรือไม่
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                string FullFilePath = Path.Combine(FilePath, FullFileName);
                FileUploadPublishTopic.SaveAs(FullFilePath);
                FullFilePath = FullFilePath.Replace("'", "''");
                // Insert Data
                sql = $"INSERT INTO DC_PublishTopicFile (UserID, PublishTopicID, FileExtension, FilePath) VALUES ({UserID}, {PublishTopicID}, '{Extension.Replace(".", "")}', '{FullFilePath}')";
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'อัปโหลดข้อมูลสำเร็จ', 'success');", true);
                    TreeViewPublishTopic_SelectedNodeChanged(null, null);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // แก้ไขชื่อหัวข้อ
        protected void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                string UserID = Session["UserID"].ToString();
                string PublishTopicID = HFNodeSelected.Value;
                string TopicName = TxtPublishTopicRename.Text;
                sql = $"UPDATE DC_PublishTopic SET UserIDUpdate = {UserID}, DateUpdate = GETDATE(), TopicName = '{TopicName}' WHERE PublishTopicID = " + PublishTopicID;
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', 'success');", true);
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

        // Generate QR Code
        protected void BtnQRCode_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string PublishTopicID = HFNodeSelected.Value;
                sql = $"SELECT TopicName FROM DC_PublishTopic WHERE PublishTopicID = {PublishTopicID}";
                LBTitleModalQRCode.Text = query.SelectAt(0, sql);

                string text = $"http://110.77.148.173/DocumentControl/Publish/DisplayDoc.aspx?PublishTopicID={PublishTopicID}";
                QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator(); // ชื่อซ้ำกับ Project เฉยเลย จึงต้องใส่ QRCoder. ให้มันด้วย
                QRCoder.QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCoder.QRCodeGenerator.ECCLevel.Q);
                QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);
                System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
                imgBarCode.Height = 150;
                imgBarCode.Width = 150;
                Bitmap bitMap = qrCode.GetGraphic(20);
                MemoryStream ms = new MemoryStream();
                bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = ms.ToArray();
                imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                PlaceHolderQRCode.Controls.Add(imgBarCode);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "OpenModalPublishTopicQRCode()", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // ปุ่มแก้ไขเอกสาร
        protected void BtnEditPublishDoc_Click(object sender, EventArgs e)
        {
            try
            {
                string UserID = Session["UserID"].ToString();
                string PublishDocFileID = HFPublishDocFileID.Value;
                sql = $@"SELECT DC_PublishDoc.PublishDocID
                FROM DC_PublishDoc 
                LEFT JOIN DC_PublishDocFile ON DC_PublishDoc.PublishDocID = DC_PublishDocFile.PublishDocID 
                WHERE DC_PublishDocFile.PublishDocFileID = {PublishDocFileID}";
                string PublishDocID = query.SelectAt(0, sql);
                string FileName = TxtPublishDocRename.Text;
                string Revision = TxtEditPublishDocRevision.Text;
                // Update Data
                sql = $"UPDATE DC_PublishDoc SET UserIDUpdate = {UserID}, DateUpdate = GETDATE(), FileName = '{FileName}' WHERE PublishDocID = {PublishDocID}";
                query.Excute(sql);
                sql = $"UPDATE DC_PublishDocFile SET Revision = '{Revision}' WHERE PublishDocFileID = {PublishDocFileID}";
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', 'success');", true);
                    LoadPublishDoc(HFNodeSelected.Value);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // ปุ่มอัปโหลดไฟล์เอกสาร
        protected void BtnUpdatePublishDoc_Click(object sender, EventArgs e)
        {
            try
            {
                string PublishDocID = HFUploadSelected.Value;
                string UserID = Session["UserID"].ToString();
                string Revision = TxtPublishDocRevisionNew.Text;
                string FileName = Path.GetFileNameWithoutExtension(FileUploadPublishDocNew.PostedFile.FileName);
                string Extension = Path.GetExtension(FileUploadPublishDocNew.PostedFile.FileName);
                string FullFileName = FileName + Extension;
                string FilePath = "\\\\192.168.0.100\\PDoc\\" + UserID + "\\" + DateTime.Now.ToString("yyyy-MM-dd HHmmss");
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                string FullFilePath = Path.Combine(FilePath, FullFileName);
                FileUploadPublishDocNew.SaveAs(FullFilePath);
                FullFilePath = FullFilePath.Replace("'", "''");
                // INSERT Data
                sql = $"INSERT INTO DC_PublishDocFile (UserID, PublishDocID, FileExtension, FilePath, Revision) VALUES ({UserID}, {PublishDocID}, '{Extension.Replace(".", "")}', '{FullFilePath}', '{Revision}')";
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'อัปโหลดข้อมูลสำเร็จ', 'success');", true);
                    LoadPublishDoc(HFNodeSelected.Value);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // ปุ่มแสดงรายละเอียดหัวข้อ
        protected void LinkBtnInfo_Click(object sender, EventArgs e)
        {
            try
            {
                LinkBtnEditPublishTopicUserAccess.Visible = true;
                PanelDisplayAccess.Visible = true;
                PanelEditPublishTopicUserAccess.Visible = false;

                string UserID = Session["UserID"].ToString();
                string PublishTopicID = HFNodeSelected.Value;
                sql = $@"SELECT DC_PublishTopic.TopicName, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, DC_PublishTopic.DateCreate, DC_PublishTopic.TopicType, DC_PublishTopic.UserID 
                FROM DC_PublishTopic
                LEFT JOIN F2_Users ON DC_PublishTopic.UserID = F2_Users.UserID
                WHERE DC_PublishTopic.PublishTopicID = {PublishTopicID}";
                string TopicName = query.SelectAt(0, sql);
                string TopicCreatedBy = query.SelectAt(1, sql);
                string TopicDateCreate = query.SelectAt(2, sql);
                string TopicType = query.SelectAt(3, sql);
                string UserIDCreated = query.SelectAt(4, sql);
                LbDetailPublishTopicName.Text = TopicName;
                LbDetailPublishTopicCreatedBy.Text = TopicCreatedBy;
                LbDetailPublishTopicDateCreate.Text = string.IsNullOrEmpty(TopicDateCreate) ? "-" : DateTime.Parse(TopicDateCreate).ToString("dd/MM/yyyy HH:mm:ss");
                if (TopicType == "1")
                {
                    PanelAccess.Visible = true;
                    LbPublishTopicTypeName.Text = "<i class=\"fas fa-globe\" style=\"color: gray;\"></i> ทุกคน";
                    // ต้องไม่มี User แล้วนะ
                    sql = $@"SELECT (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name
                    FROM DC_PublishTopicUserAccess
                    LEFT JOIN F2_Users ON DC_PublishTopicUserAccess.UserID = F2_Users.UserID
                    WHERE DC_PublishTopicUserAccess.PublishTopicID = {PublishTopicID}";
                    DataTable dtPublishTopicUserAccess = query.SelectTable(sql);
                    LVPublishTopicUserAccess.DataSource = dtPublishTopicUserAccess;
                    LVPublishTopicUserAccess.DataBind();
                }
                else if (TopicType == "2")
                {
                    PanelAccess.Visible = true;
                    LbPublishTopicTypeName.Text = "<i class=\"fas fa-user-lock\" style=\"color: gray;\"></i> กำหนด";
                    sql = $@"SELECT (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name
                    FROM DC_PublishTopicUserAccess
                    LEFT JOIN F2_Users ON DC_PublishTopicUserAccess.UserID = F2_Users.UserID
                    WHERE DC_PublishTopicUserAccess.PublishTopicID = {PublishTopicID}";
                    DataTable dtPublishTopicUserAccess = query.SelectTable(sql);
                    LVPublishTopicUserAccess.DataSource = dtPublishTopicUserAccess;
                    LVPublishTopicUserAccess.DataBind();
                }
                else
                {
                    PanelAccess.Visible = false;
                }
                // ตรวจสอบว่าเป็นเจ้าของหัวข้อหรือไม่ ถ้าเป็นจะแก้ไขได้ หรือถ้ามีสิทธิ์ Full Control
                if (UserIDCreated == UserID || CheckPermissionFullControl())
                {
                    LinkBtnEditPublishTopicUserAccess.Visible = true;
                }
                else
                {
                    LinkBtnEditPublishTopicUserAccess.Visible = false;
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenOffcanvas", "OpenOffcanvasDetailPublishTopic();", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // ปุ่มแสดงการแก้ไขผู้ใช้ที่มีสิทธิ์เข้าถึง และ แก้ไขประเภทของหัวข้อ
        protected void LinkBtnEditPublishTopicUserAccess_Click(object sender, EventArgs e)
        {
            try
            {
                LinkBtnEditPublishTopicUserAccess.Visible = false;
                PanelDisplayAccess.Visible = false;
                PanelEditPublishTopicUserAccess.Visible = true;
                string PublishTopicID = HFNodeSelected.Value;
                sql = $"SELECT TopicType FROM DC_PublishTopic WHERE PublishTopicID = {PublishTopicID}";
                string TopicType = query.SelectAt(0, sql);
                RBListEditPublishTopicType.SelectedValue = TopicType;
                RBListEditPublishTopicType_SelectedIndexChanged(null, null);
                if (TopicType == "2")
                {
                    ListBoxEditPublishTopicUserAccess.DataBind();
                    sql = $@"SELECT UserID FROM DC_PublishTopicUserAccess WHERE PublishTopicID = {PublishTopicID}";
                    DataTable dtPublishTopicUserAccess = query.SelectTable(sql);
                    int CountRowTable = dtPublishTopicUserAccess.Rows.Count;
                    string UserIDAccess = string.Empty;
                    if (CountRowTable > 0)
                    {
                        for (int CountRow = 0; CountRow < CountRowTable; CountRow++)
                        {
                            ListBoxEditPublishTopicUserAccess.Items.FindByValue(dtPublishTopicUserAccess.Rows[CountRow]["UserID"].ToString()).Selected = true;
                            UserIDAccess += dtPublishTopicUserAccess.Rows[CountRow]["UserID"].ToString();
                            if (CountRow < CountRowTable - 1)
                            {
                                UserIDAccess += ",";
                            }
                        }
                        HFEditUserIDSelectedAccess.Value = UserIDAccess;
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // ปุ่มบันทึกแก้ไขข้อมูล
        protected void BtnSaveEditPublishTopic_Click(object sender, EventArgs e)
        {
            try
            {
                string UserID = Session["UserID"].ToString();
                string PublishTopicID = HFNodeSelected.Value;
                string TopicType = RBListEditPublishTopicType.SelectedValue;

                // อัปเดตหัวข้อ
                sql = $"UPDATE DC_PublishTopic SET TopicType = {TopicType}, UserIDUpdate = {UserID}, DateUpdate = GETDATE() WHERE PublishTopicID = {PublishTopicID}";
                query.CheckRow(sql);

                if (TopicType == "1")
                {
                    // ถ้าเคยเป็นหัวข้อ 2 และมีผู้ใช้ที่มีสิธิ์เข้าถึงหัวข้อ ให้ลบออกให้หมด
                    sql = $"SELECT UserID FROM DC_PublishTopicUserAccess WHERE PublishTopicID = {PublishTopicID}";
                    if (query.CheckRow(sql))
                    {
                        sql = $"DELETE DC_PublishTopicUserAccess WHERE PublishTopicID = {PublishTopicID}";
                        query.Excute(sql);
                    }
                }
                else if (TopicType == "2")
                {
                    string UserIDSelectedAll = HFEditUserIDSelectedAccess.Value;

                    // ลบผู้ใช้ที่ไม่มีในตัวเลือก
                    sql = $"DELETE DC_PublishTopicUserAccess WHERE PublishTopicID = {PublishTopicID}";
                    query.Excute(sql);

                    // เพิ่มผู้ใช้ที่มีสิทธิ์
                    List<String> ListUserIDAll = UserIDSelectedAll.Split(',').ToList();
                    foreach (string UserIDSelected in ListUserIDAll)
                    {
                        sql = $@"INSERT INTO DC_PublishTopicUserAccess (PublishTopicID, UserID) VALUES ({PublishTopicID}, {UserIDSelected})";
                        query.Excute(sql);
                    }
                }
                // โหลดข้อมูลใหม่
                LoadMainPublishTopic();
                TreeViewPublishTopic_SelectedNodeChanged(null, null);
                // Expand Parent ของ Node ที่เลือก
                ExpandToRoot(TreeViewPublishTopic.SelectedNode.Parent);
                LinkBtnInfo_Click(null, null);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', 'success');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // ปุ่มยกเลิกแก้ไขหัวข้อ
        protected void BtnCancelEditPublishTopic_Click(object sender, EventArgs e)
        {
            LinkBtnEditPublishTopicUserAccess.Visible = true;
            PanelDisplayAccess.Visible = true;
            PanelEditPublishTopicUserAccess.Visible = false;
        }

        // --------------- CheckBox
        // CheckBox ใช้เลือกเพิ่มหัวข้อหลัก
        protected void CBMainPublishTopic_CheckedChanged(object sender, EventArgs e)
        {
            if (CBMainPublishTopic.Checked)
            {
                CBMainPublishTopic.Enabled = false;
                if (TreeViewPublishTopic.SelectedValue.Length > 0)
                {
                    TreeViewPublishTopic.SelectedNode.Selected = false;
                    HFNodeSelected.Value = string.Empty;
                }
                RequiredFieldValidatorPublishTopicUserAccess.Enabled = true;
                PanelPublishTopicType.Visible = true;
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


        // --------------- GridView
        // โหลดข้อมูลเอกสาร
        private void LoadPublishDoc(string PublishTopicID)
        {
            try
            {
                sql = $@"SELECT DC_PublishDoc.PublishDocID, DC_PublishDoc.FileName, DC_PublishDocFile.PublishDocFileID, DC_PublishDocFile.FilePath, DC_PublishDocFile.PublishDate, DC_PublishDocFile.Revision, DC_PublishDocFile.FileExtension, DC_PublishDoc.UserID
                FROM DC_PublishDoc 
                LEFT JOIN DC_PublishDocFile ON DC_PublishDoc.PublishDocID = DC_PublishDocFile.PublishDocID 
                AND DC_PublishDocFile.PublishDocFileID IN (SELECT MAX(PublishDocFileID) FROM DC_PublishDocFile GROUP BY PublishDocID)
                WHERE DC_PublishDoc.PublishTopicID = {PublishTopicID} AND DC_PublishDoc.Status = 1 ORDER BY DC_PublishDoc.FileName";
                GVPublishDoc.DataSource = query.SelectTable(sql);
                GVPublishDoc.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // Method การกดปุ่มใน GridView
        protected void GVPublishDoc_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string Btn = e.CommandName;
                string Value = e.CommandArgument.ToString();
                // ปุ่มดาวน์โหลดเอกสาร
                if (Btn == "BtnDownload")
                {
                    string[] SplitFilePath = Value.Split('\\');
                    string FileName = SplitFilePath.Last();

                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + StringSpecial.StringSpecial(FileName));
                    Response.ContentType = "application/pdf";
                    Response.WriteFile(Value);
                    Response.End();
                }
                // ปุ่มดูประวัติเอกสาร
                else if (Btn == "BtnHistory")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "New_Window", "window.open('PublishHistoryFile.aspx?PublishDocID=" + Value + "', '_blank', 'location=yes,height=570,width=600,scrollbars=yes,status=yes');", true);
                }
                // ปุ่มอัปโหลดเอกสาร
                else if (Btn == "BtnUpload")
                {
                    HFUploadSelected.Value = Value;
                    sql = $"SELECT FileName FROM DC_PublishDoc WHERE PublishDocID = {Value}";
                    LBFileName.Text = query.SelectAt(0, sql);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "OpenModalPublishDocUploadFile()", true);
                }
                // ปุ่มแก้ไขชื่อไฟล์
                else if (Btn == "BtnEdit")
                {
                    HFPublishDocFileID.Value = Value; // เอาไว้เก็บ PublishDocFileID
                    sql = $@"SELECT DC_PublishDoc.FileName, DC_PublishDocFile.Revision
                    FROM DC_PublishDoc 
                    LEFT JOIN DC_PublishDocFile ON DC_PublishDoc.PublishDocID = DC_PublishDocFile.PublishDocID 
                        AND DC_PublishDocFile.PublishDocFileID IN (SELECT MAX(PublishDocFileID) FROM DC_PublishDocFile GROUP BY PublishDocID)
                    WHERE DC_PublishDocFile.PublishDocFileID = {Value}";
                    TxtPublishDocRename.Text = query.SelectAt(0, sql);
                    TxtEditPublishDocRevision.Text = query.SelectAt(1, sql);
                    // ตรวจสอบว่าถ้าไม่มีสิทธิ์ Full Control ไม่ให้เปลี่ยน Revision
                    if (!CheckPermissionFullControl())
                    {
                        TxtEditPublishDocRevision.Enabled = false;
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "OpenModalPublishDocRename()", true);
                }
                // ปุ่มแสดงรายละเอียดไฟล์
                else if (Btn == "LinkBtnInfo")
                {
                    sql = $@"SELECT DC_PublishDoc.FileName, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, DC_PublishDocFile.PublishDate 
                    FROM DC_PublishDoc 
                    LEFT JOIN DC_PublishDocFile ON DC_PublishDoc.PublishDocID = DC_PublishDocFile.PublishDocID 
                        AND DC_PublishDocFile.PublishDocFileID IN (SELECT MAX(PublishDocFileID) FROM DC_PublishDocFile GROUP BY PublishDocID)
                    LEFT JOIN F2_Users ON DC_PublishDoc.UserID = F2_Users.UserID
                    WHERE DC_PublishDoc.PublishDocID = " + Value;
                    LbDetailPublishDocFileName.Text = query.SelectAt(0, sql);
                    LbDetailPublishDocCreatedBy.Text = query.SelectAt(1, sql);
                    LbDetailPublishDocDateCreate.Text = DateTime.Parse(query.SelectAt(2, sql)).ToString("dd/MM/yyyy HH:mm:ss");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenOffcanvas", "OpenOffcanvasDetailPublishDoc();", true);
                }
                // ปุ่มลบเอกสาร
                else if (Btn == "BtnDelete")
                {
                    string UserID = Session["UserID"].ToString();
                    sql = $"UPDATE DC_PublishDoc SET UserIDUpdate = {UserID}, DateUpdate = GETDATE(), Status = 0 WHERE PublishDocID = {Value}";
                    if (query.Excute(sql))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'ลบข้อมูลสำเร็จ', 'success');", true);
                        string PublishTopicID = HFNodeSelected.Value;
                        LoadPublishDoc(PublishTopicID);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
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
                try
                {
                    // เช็คว่ามีมากกว่า 1 ไฟล์ไหม ถ้ามีน้องกว่า 2 ไฟล์ลงไป แสดงว่าไม่มีประวัติไฟล์เลย
                    string PublishDocID = DataBinder.Eval(e.Row.DataItem, "PublishDocID").ToString();
                    sql = $"SELECT PublishDocFileID FROM DC_PublishDocFile WHERE PublishDocID = {PublishDocID}";
                    DataTable dt = query.SelectTable(sql);
                    if (dt.Rows.Count <= 1)
                    {
                        ImageButton BtnHistory = e.Row.FindControl("BtnHistory") as ImageButton;
                        BtnHistory.Visible = false;
                    }

                    // เช็คสิทธิ์การใช้งาน
                    // ตรวจสอบมีสิทธิ์ Full Control ไหม หรือ (ถ้ามีสิทธิ์ Ownership ต้อง เป็นเจ้าของไหม)
                    string UserID = Session["UserID"].ToString();
                    string UserIDCreated = DataBinder.Eval(e.Row.DataItem, "UserID").ToString();
                    if (!CheckPermissionFullControl() && (!CheckPermissionOwnership() || UserIDCreated != UserID))
                    {
                        //ImageButton BtnDownload = e.Row.FindControl("BtnDownload") as ImageButton;
                        //BtnDownload.Visible = false;
                        ImageButton BtnLink = e.Row.FindControl("BtnLink") as ImageButton;
                        BtnLink.Visible = false;
                        ImageButton BtnHistory = e.Row.FindControl("BtnHistory") as ImageButton;
                        BtnHistory.Visible = false;
                        ImageButton BtnUpload = e.Row.FindControl("BtnUpload") as ImageButton;
                        BtnUpload.Visible = false;
                        ImageButton BtnEditShow = e.Row.FindControl("BtnEditShow") as ImageButton;
                        BtnEditShow.Visible = false;
                        ImageButton BtnDelete = e.Row.FindControl("BtnDelete") as ImageButton;
                        BtnDelete.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
                }
            }
        }


        // --------------- Function
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

        // ตรวจสอบสิทธิ์ Document Control
        private bool CheckPermissionAll()
        {
            string UserID = Session["UserID"].ToString();
            // ตรวจสอบว่ามีสิทธิ์ 1 Full Control, 2 = Ownership หรือไม่
            sql = $"SELECT PermissionID FROM DC_PermissionUser WHERE UserID = {UserID} AND PermissionID IN (1, 2)";
            DataTable dt = query.SelectTable(sql);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        private bool CheckPermissionFullControl()
        {
            string UserID = Session["UserID"].ToString();
            // ตรวจสอบว่ามีสิทธิ์ 1 Full Control หรือไม่
            sql = $"SELECT PermissionID FROM DC_PermissionUser WHERE UserID = {UserID} AND PermissionID = 1";
            DataTable dt = query.SelectTable(sql);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        private bool CheckPermissionOwnership()
        {
            string UserID = Session["UserID"].ToString();
            // ตรวจสอบว่ามีสิทธิ์ 2 Ownership หรือไม่
            sql = $"SELECT PermissionID FROM DC_PermissionUser WHERE UserID = {UserID} AND PermissionID = 2";
            DataTable dt = query.SelectTable(sql);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }


        // --------------- RadioButtonList
        // TopicType ใช้ตอนเพิ่มหัวข้อ
        protected void RBListPublishTopicType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RBListPublishTopicType.SelectedValue == "2")
            {
                PanelUserAccess.Visible = true;
                RequiredFieldValidatorPublishTopicUserAccess.Enabled = true;
            }
            else
            {
                PanelUserAccess.Visible = false;
                RequiredFieldValidatorPublishTopicUserAccess.Enabled = false;
            }
        }

        // TopicType ใช้ตอนแก้ไขหัวข้อ
        protected void RBListEditPublishTopicType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RBListEditPublishTopicType.SelectedValue == "2")
            {
                PanelEditUserAccess.Visible = true;
                RequiredFieldValidatorEditPublishTopicUserAccess.Enabled = true;
            }
            else
            {
                PanelEditUserAccess.Visible = false;
                RequiredFieldValidatorEditPublishTopicUserAccess.Enabled = false;
            }
        }
    }
}