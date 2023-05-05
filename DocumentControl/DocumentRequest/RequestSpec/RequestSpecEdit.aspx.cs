using DocumentControl.Admin;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest.RequestSpec
{
    public partial class RequestSpecEdit : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        LineNotify LineNotify = new LineNotify();
        StringSpecialClass StringSpecial = new StringSpecialClass();
        List<DataListFiles> ListFiles = new List<DataListFiles>();
        public class DataListFiles
        {
            public string Value { get; set; }
            public string FileName { get; set; }
            public HttpPostedFile File { get; set; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["RequestSpecID"] != null && Session["UserID"] != null)
                {
                    string RequestSpecID = Request.QueryString["RequestSpecID"];
                    HFRequestSpecID.Value = RequestSpecID;

                    // Load Data
                    Session["ListFiles"] = null;    // ใช้เก็บไฟล์ชั่วคราว
                    LoadUserRequest();
                    LoadDDListAcceptLeader();
                    LoadRequestSpecDetail(RequestSpecID);
                    DDListOperation_SelectedIndexChanged(null, null);
                }
            }
        }


        // --------------- Function
        private void LoadUserRequest()
        {
            // ชื่อผู้ร้องขอ
            string UserID = Session["UserID"].ToString();
            sql = $"SELECT (FirstNameTH + ' ' + LastNameTH) As Name FROM F2_Users WHERE UserID = {UserID}";
            TxtUserRequest.Text = query.SelectAt(0, sql);
        }
        private void LoadDDListAcceptLeader()
        {
            // โหลดข้อมูล ผู้อนุมัติ หน่วยงานต้นสังกัด
            string DepartmentID = Session["DepartmentID"].ToString();
            sql = $"SELECT F2_Users.UserID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) As Name FROM F2_UserOrganization LEFT JOIN F2_Users ON F2_UserOrganization.UserID = F2_Users.UserID WHERE F2_UserOrganization.DepartmentID = {DepartmentID} AND F2_Users.Status = 1";
            DDListAcceptLeader.DataSource = query.SelectTable(sql);
            DDListAcceptLeader.DataBind();
        }
        private void LoadRequestSpecDetail(string RequestSpecID)
        {
            try
            {
                sql = $@"SELECT DC_RequestSpec.RequestSpecID, DC_RequestSpec.DateRequest, DC_RequestSpec.RequestSpecDocTypeID, DC_RequestSpec.RequestSpecOperationID, DC_RequestSpec.OperationOther, DC_RequestSpec.RequestSpecTypeOfChangeID, Project.ProjectID, Part.PartID, FG.FGID, DC_RequestSpec.DetailRequest, DC_RequestSpec.ReasonRequest
                , DC_LeaderAccept.UserID AS LeaderUserID, DC_NPDAccept.UserID AS NPDUserID, DC_Approve.UserID AS ApproveUserID
                FROM DC_RequestSpec
                LEFT JOIN [TCTFactory].[dbo].[FG] AS FG ON DC_RequestSpec.FGID = FG.FGID
                LEFT JOIN [TCTFactory].[dbo].[Part] AS Part ON FG.PartID = Part.PartID
                LEFT JOIN [TCTFactory].[dbo].[Project] AS Project ON Part.ProjectID = Project.ProjectID
                LEFT JOIN DC_LeaderAccept ON DC_RequestSpec.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN DC_NPDAccept ON DC_RequestSpec.NPDAcceptID = DC_NPDAccept.NPDAcceptID
                LEFT JOIN DC_Approve ON DC_RequestSpec.ApproveID = DC_Approve.ApproveID
                WHERE DC_RequestSpec.RequestSpecID = " + RequestSpecID;
                string DateRequest = query.SelectAt(1, sql);
                string DocTypeID = query.SelectAt(2, sql);
                string OperationID = query.SelectAt(3, sql);
                string OperationOther = query.SelectAt(4, sql);
                string TypeOfChangeID = query.SelectAt(5, sql);
                string ProjectID = query.SelectAt(6, sql);
                string PartID = query.SelectAt(7, sql);
                string FGID = query.SelectAt(8, sql);
                string DetailRequest = query.SelectAt(9, sql);
                string ReasonRequest = query.SelectAt(10, sql);
                // เช็คสถานะ
                string LeaderUserID = query.SelectAt(11, sql);
                string NPDUserID = query.SelectAt(12, sql);
                string ApproveUserID = query.SelectAt(13, sql);


                // Set Value
                LbDateRequest.Text = DateTime.Parse(DateRequest).ToString("dd/MM/yyyy");
                LbRequestSpecID.Text = int.Parse(RequestSpecID).ToString("D3");
                DDListDocType.DataBind();
                DDListDocType.SelectedValue = DocTypeID;
                DDListOperation.DataBind();
                DDListOperation.SelectedValue = OperationID;
                TxtOperationOther.Text = OperationOther;
                RBListTypeOfChange.SelectedValue = TypeOfChangeID;
                DDListProject.DataBind();
                DDListProject.SelectedValue = ProjectID;
                DDListPart.DataBind();
                DDListPart.SelectedValue = PartID;
                DDListFG.DataBind();
                DDListFG.SelectedValue = FGID;
                TxtDetailRequest.Text = DetailRequest;
                TxtReasonRequest.Text = ReasonRequest;
                DDListAcceptLeader.SelectedValue = LeaderUserID;
                DDListAcceptNPD.DataBind();
                DDListAcceptNPD.SelectedValue = NPDUserID;
                DDListApprove.DataBind();
                DDListApprove.SelectedValue = ApproveUserID;

                // Load Doc
                sql = $@"SELECT RequestSpecDocID, RequestSpecID, FileName, FileExtension, FilePath, DC_RequestSpecDoc.RequestSpecDocStatusID, DC_RequestSpecDocStatus.DocStatusDetail
                FROM DC_RequestSpecDoc
                INNER JOIN DC_RequestSpecDocStatus ON DC_RequestSpecDoc.RequestSpecDocStatusID = DC_RequestSpecDocStatus.RequestSpecDocStatusID
                WHERE RequestSpecID = {RequestSpecID}";
                DataTable dataTableDoc = query.SelectTable(sql);
                foreach (DataRow dataRowDoc in dataTableDoc.Rows)
                {
                    ListFiles.Add(new DataListFiles { Value = dataRowDoc["RequestSpecDocID"].ToString(), FileName = dataRowDoc["FileName"].ToString() + dataRowDoc["FileExtension"].ToString() });
                }
                // เก็บไฟล์ใน List ลง Session เพื่อไม่ให้มันหาย
                Session["ListFiles"] = ListFiles;
                ListBoxFiles.Items.Clear(); // ล้างช่อง ListBox
                // แสดงชื่อไฟล์ที่อยู่ใน Session
                foreach (DataListFiles dataListFiles in (List<DataListFiles>)Session["ListFiles"])
                {
                    if (dataListFiles.Value == "0")
                    {
                        dataListFiles.FileName = dataListFiles.FileName + " (ไฟล์ใหม่)";
                    }
                    ListBoxFiles.Items.Add(new ListItem(dataListFiles.FileName, dataListFiles.Value));
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }


        // --------------- Button
        // ปุ่ม เพิ่มไฟล์ลงใน List
        protected void BtnAddFile_Click(object sender, EventArgs e)
        {
            // ตรวจสอบว่ามีไฟล์ใน Session หรือไม่ ถ้ามีก็เอาใส่ List
            if (Session["ListFiles"] != null)
            {
                ListFiles = (List<DataListFiles>)Session["ListFiles"];
            }
            // เพิ่มไฟล์ใหม่ลงใน List
            foreach (HttpPostedFile postedFile in FileUploadFile.PostedFiles)
            {
                ListFiles.Add(new DataListFiles { Value = "0", FileName = postedFile.FileName, File = postedFile });
            }
            // เก็บไฟล์ใน List ลง Session เพื่อไม่ให้มันหาย
            Session["ListFiles"] = ListFiles;
            ListBoxFiles.Items.Clear(); // ล้างช่อง ListBox
            // แสดงชื่อไฟล์ที่อยู่ใน Session
            foreach (DataListFiles dataListFiles in (List<DataListFiles>)Session["ListFiles"])
            {
                if (dataListFiles.Value == "0")
                {
                    dataListFiles.FileName = dataListFiles.FileName + " (ไฟล์ใหม่)";
                }
                ListBoxFiles.Items.Add(new ListItem(dataListFiles.FileName, dataListFiles.Value));
            }
            // ถ้ามีไฟล์ให้ปิดการตรวจสอบค่า
            if (((List<DataListFiles>)Session["ListFiles"]).Count > 0)
            {
                RequiredFieldValidatorFile.Enabled = false;
            }
        }

        // ปุ่มลบไฟล์ออกจาก List
        protected void LinkBtnDelFile_Click(object sender, EventArgs e)
        {
            // ตรวจสอบว่าเลือก
            if (ListBoxFiles.SelectedIndex != -1)
            {
                // ลบรายการที่เลือกออกจาก List
                ListFiles = (List<DataListFiles>)Session["ListFiles"];
                // ตรวจสอบว่าไฟล์ที่ลบออกเป็นไฟล์ที่มีอยู่ในระบบฐานข้อมูลหรือไม่ ถ้าไม่มีค่าจะเป็น 0
                if (ListFiles[ListBoxFiles.SelectedIndex].Value != "0")
                {
                    string RequestSpecDocID = ListFiles[ListBoxFiles.SelectedIndex].Value;
                    sql = $"UPDATE DC_RequestSpecDoc SET RequestSpecDocStatusID = 0 WHERE RequestSpecDocID = {RequestSpecDocID}";
                    query.Excute(sql);
                }
                ListFiles.RemoveAt(ListBoxFiles.SelectedIndex);
                Session["ListFiles"] = ListFiles;
                ListBoxFiles.Items.Clear();
                // แสดงชื่อไฟล์ที่อยู่ใน Session
                foreach (DataListFiles dataListFiles in (List<DataListFiles>)Session["ListFiles"])
                {
                    ListBoxFiles.Items.Add(new ListItem(dataListFiles.FileName, dataListFiles.Value));
                }
                // ถ้าไม่มีไฟล์ให้เปิดการตรวจสอบค่า
                if (((List<DataListFiles>)Session["ListFiles"]).Count == 0)
                {
                    RequiredFieldValidatorFile.Enabled = true;
                }
            }
            LinkBtnOpenFile.Visible = false;
            LinkBtnDelFile.Visible = false;
        }

        // ปุ่มเปิดไฟล์
        protected void LinkBtnOpenFile_Click(object sender, EventArgs e)
        {
            // ตรวจสอบว่าเลือก
            if (ListBoxFiles.SelectedIndex != -1)
            {
                // ลบรายการที่เลือกออกจาก List
                ListFiles = (List<DataListFiles>)Session["ListFiles"];
                // ตรวจสอบว่าไฟล์ที่เปิดเป็นไฟล์ที่มีอยู่ในระบบฐานข้อมูลหรือไม่ ถ้าไม่มีค่าจะเป็น 0
                if (ListFiles[ListBoxFiles.SelectedIndex].Value != "0")
                {
                    string RequestSpecDocID = ListFiles[ListBoxFiles.SelectedIndex].Value;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "New_Tab", "window.open('ShowPDF.aspx?RequestSpecDocID=" + RequestSpecDocID + "', '_blank');", true);
                }
            }
        }

        // ปุ่มอัปเดตแบบฟอร์ม
        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string RequestSpecID = Request.QueryString["RequestSpecID"];
                sql = $@"SELECT UserID, LeaderAcceptID, NPDAcceptID, ApproveID
                    FROM DC_RequestSpec 
                    WHERE RequestSpecID = {RequestSpecID}";
                string UserIDCreate = query.SelectAt(0, sql);   // คนสร้างใบร้องขอ
                string LeaderAcceptID = query.SelectAt(1, sql);
                string NPDAcceptID = query.SelectAt(2, sql);
                string ApproveID = query.SelectAt(3, sql);

                string DocTypeID = DDListDocType.SelectedValue; // เอกสารทางวิศวกรรมที่ต้องการแจ้งการดำเนินการ
                string OperationID = DDListOperation.SelectedValue; // ประเภทของการแจ้งการดำเนินการเอกสาร
                string OperationOther = TxtOperationOther.Text; // ประเภทของการแจ้งการดำเนินการเอกสาร อื่นๆ
                string TypeOfChangeID = RBListTypeOfChange.SelectedValue;   // ประเภทการเปลี่ยนแปลง
                string FGID = DDListFG.SelectedValue;   // Part NO.
                string DetailRequest = TxtDetailRequest.Text;   // รายละเอียดการแก้ไข/เพิ่มเติม
                string ReasonRequest = TxtReasonRequest.Text;    // เหตุผลที่ต้องแจ้งดำเนินการเอกสาร
                ListFiles = (List<DataListFiles>)Session["ListFiles"]; // เอกสารแนบ
                string UserID = Session["UserID"].ToString();   // ผู้ร้องขอ
                string AcceptLeader = DDListAcceptLeader.SelectedValue; // ผู้อนุมัติ หัวหน้าหน่วยงานผู้ร้องขอ
                string AcceptNPD = DDListAcceptNPD.SelectedValue;   // วิศวกร NPD
                string Approve = DDListApprove.SelectedValue;   // หัวหน้าหน่วยงาน NPD

                // ---------- Start ตรวจสอบการกรอกข้อมูล ----------
                // เลือกการดำเนินการ อื่นๆ แล้วไม่กรอกข้อมูลช่องอื่นๆ หรือไม่
                if (DDListOperation.SelectedValue == "5" && string.IsNullOrEmpty(OperationOther))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณาระบุการดำเนินการ.', 'warning');", true);
                    return;
                }
                // เลือก Part NO. หรือไม่
                if (DDListFG.SelectedValue == "0")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณาเลือก Part NO.', 'warning');", true);
                    return;
                }
                // กรอกรายละเอียดการแก้ไข/เพิ่มเติม หรือไม่
                if (string.IsNullOrEmpty(DetailRequest))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณาระบุรายละเอียดการแก้ไข.', 'warning');", true);
                    return;
                }
                // กรอก เหตุผลที่ต้องแจ้งดำเนินการเอกสาร หรือไม่
                if (string.IsNullOrEmpty(ReasonRequest))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณาระบุเหตุผลที่ต้องแจ้งดำเนินการเอกสาร.', 'warning');", true);
                    return;
                }
                // มีเอกสารแนบหรือไม่
                if (ListFiles.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณาแนบเอกสาร.', 'warning');", true);
                    return;
                }
                // ---------- END ตรวจสอบการกรอกข้อมูล ----------

                // เก็บสถานะตรวจสอบล่าสุดก่อน ไว้เช็คแจ้งเตือน
                sql = $"SELECT AcceptStatus FROM DC_LeaderAccept WHERE LeaderAcceptID = {LeaderAcceptID}";
                int LeaderAcceptStatus = int.Parse(query.SelectAt(0, sql));
                // อัปเดตหัวหน้าหน่วยงานผู้ร้องขอ อนุมัติ
                sql = $"UPDATE DC_LeaderAccept SET UserID = {AcceptLeader}, AcceptDate = NULL, AcceptStatus = 2 WHERE LeaderAcceptID = {LeaderAcceptID}";
                query.Excute(sql);

                // อัปเดต NPD วิศวกร
                sql = $"UPDATE DC_NPDAccept SET UserID = {AcceptNPD}, AcceptDate = NULL, AcceptStatus = 2 WHERE NPDAcceptID = {NPDAcceptID}";
                query.Excute(sql);

                // อัปเดต NPD อนุมัติ
                sql = $"UPDATE DC_Approve SET UserID = {Approve}, ApproveDate = NULL, ApproveStatus = 2 WHERE ApproveID = {ApproveID}";
                query.Excute(sql);

                // อัปเดตแบบฟอร์มแจ้งการดำเนินการเอกสารวิศวกรรม (Specification & Drawing)
                sql = $@"UPDATE DC_RequestSpec SET UserIDUpdate = {UserID}, DateUpdateRequest = GETDATE(), RemarkCancel = NULL, RequestSpecDocTypeID = {DocTypeID}, RequestSpecOperationID = {OperationID}, OperationOther = '{OperationOther}', FGID = {FGID}, DetailRequest = '{DetailRequest}', ReasonRequest = '{ReasonRequest}', RequestSpecTypeOfChangeID = {TypeOfChangeID}, RemarkLeader = NULL, RemarkApprove = NULL, RequestSpecStatusID = 2 WHERE RequestSpecID = {RequestSpecID}";
                query.Excute(sql);

                // เพิ่มไฟล์
                // ตรวจสอบว่ามี Folder หลักของการร้องขอนี้ หรือยัง ถ้ายังไม่มีให้สร้าง
                string FilePath = $"\\\\192.168.0.100\\PDoc\\RequestSpec\\{UserIDCreate}\\{RequestSpecID}";
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                int CountFile = 1;  // นับไฟล์ ใช้สร้าง Folder
                string DateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HHmmss");    // วันเดือนปีเวลาใช้สร้าง Folder
                foreach (DataListFiles dataListFiles in ListFiles)
                {
                    // ตรวจสอบว่ามี Folder ย่อยสำหรับแต่ละเอกสารหริอยัง ถ้ายังไม่มีให้สร้าง
                    string FilePathSub = $"{FilePath}\\{DateTimeNow} {CountFile}";
                    if (!Directory.Exists(FilePathSub))
                    {
                        Directory.CreateDirectory(FilePathSub);
                    }
                    string RequestSpecDocID = dataListFiles.Value;
                    // ตรวจสอบว่ามีไฟล์ในฐานข้อมูลหรือยัง ถ้ายังไม่มีค่าจะเป็น 0
                    if (RequestSpecDocID == "0")
                    {
                        string FileName = Path.GetFileNameWithoutExtension(dataListFiles.File.FileName); // ชื่อไฟล์
                        string Extension = Path.GetExtension(dataListFiles.File.FileName);  // นามสกุลไฟล์
                        string FullFileName = StringSpecial.StringSpecial(FileName) + Extension;    // ชื่อไฟล์ที่ตัดอักษรพิเศษ กับนามสกุลไฟล์
                        string FullFilePath = Path.Combine(FilePathSub, FullFileName);  // ที่อยู่จัดเก็บไฟล์
                        FullFilePath = FullFilePath.Replace("'", "''"); // ป้องกันการบัคซักหน่อย
                        dataListFiles.File.SaveAs(FullFilePath);    // บันทึกไฟล์
                        // เพิ่มข้อมูลไฟล์
                        sql = $@"INSERT INTO DC_RequestSpecDoc (RequestSpecID, FileName, FileExtension, FilePath, RequestSpecDocStatusID)
                        VALUES ({RequestSpecID}, '{FileName}', '{Extension}', '{FullFilePath}', 1)";
                        query.Excute(sql);
                        CountFile++;    // เพิ่มค่าไป 1
                    }
                }
                // แจ้งเตือน
                // ตรวจสอบก่อนว่าผู้ตรวจสอบตรวจสอบหรือยัง ถ้าตรวจสอบแล้วให้แจ้งเตือนใหม่ ถ้ายังไม่ตรวจสอบ ไม่ต้องส่งแจ้งเตือนไปใหม่
                if (LeaderAcceptStatus == 1)
                {
                    string Name = DDListAcceptLeader.SelectedItem.Text;
                    LineNotify.Notify($"มีรายการอนุมัติ\nถึงคุณ: {Name}\nรายละเอียด: http://110.77.148.173/DocumentControl/DocumentRequest/Approve.aspx");
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotificationAndRedirect('สำเร็จ', 'บันทึกข้อมูลสำเร็จ.', 'success', '~/DocumentRequest/RequestSpec/RequestSpecHistory.aspx');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // ปุ่มย้อนกลับ
        protected void LinkBtnBack_Click(object sender, EventArgs e)
        {
            if (Session["LastPage"] != null)
            {
                Response.Redirect(Session["LastPage"].ToString());
            }
            else
            {
                Response.Redirect("~/DocumentRequest/RequestSpec/RequestSpecHistory.aspx");
            }
        }


        // --------------- ListBox
        protected void ListBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ListBoxFiles.SelectedItem.Text.ToString()))
            {
                LinkBtnOpenFile.Visible = false;
                LinkBtnDelFile.Visible = false;
            }
            else
            {
                // ลบรายการที่เลือกออกจาก List
                ListFiles = (List<DataListFiles>)Session["ListFiles"];
                // ตรวจสอบว่าไฟล์ที่เปิดเป็นไฟล์ที่มีอยู่ในระบบฐานข้อมูลหรือไม่ ถ้าไม่มีค่าจะเป็น 0
                if (ListFiles[ListBoxFiles.SelectedIndex].Value != "0")
                    LinkBtnOpenFile.Visible = true;
                else
                    LinkBtnOpenFile.Visible = false;
                LinkBtnDelFile.Visible = true;
            }
        }


        // --------------- DropdownList
        protected void DDListOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DDListOperation.SelectedValue == "5")
            {
                TxtOperationOther.Enabled = true;
                RequiredFieldValidatorOperationOther.Enabled = true;
            }
            else
            {
                TxtOperationOther.Enabled = false;
                RequiredFieldValidatorOperationOther.Enabled = false;
            }
        }
        protected void DDListProject_DataBound(object sender, EventArgs e)
        {
            DDListProject.Items.Insert(0, new ListItem("-- เลือก Project --", "0"));
            DDListProject.SelectedIndex = 0;
        }
        protected void DDListPart_DataBound(object sender, EventArgs e)
        {
            DDListPart.Items.Insert(0, new ListItem("-- เลือก Part --", "0"));
            DDListPart.SelectedIndex = 0;
        }
        protected void DDListFG_DataBound(object sender, EventArgs e)
        {
            DDListFG.Items.Insert(0, new ListItem("-- เลือก FG --", "0"));
            DDListFG.SelectedIndex = 0;
        }
    }
}