using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Resources;

namespace DocumentControl.DocumentRequest.RequestSpec
{
    public partial class RequestSpec : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        LineNotify LineNotify = new LineNotify();
        StringSpecialClass StringSpecial = new StringSpecialClass();
        List<HttpPostedFile> ListFiles = new List<HttpPostedFile>();
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LastPage"] = "~/DocumentRequest/RequestSpec/RequestSpec.aspx";
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    LoadUserRequest();
                    LoadDDListAcceptLeader();
                    Session["ListFiles"] = null;    // ใช้เก็บไฟล์ชั่วคราว
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
        public void SetEmptyData()
        {
            DDListDocType.DataBind();
            DDListOperation.DataBind();
            RBListTypeOfChange.SelectedIndex = 0;
            DDListProject.DataBind();
            DDListPart.DataBind();
            DDListFG.DataBind();
            TxtDetailRequest.Text = string.Empty;
            TxtReasonRequest.Text = string.Empty;
            Session["ListFiles"] = null;    // ใช้เก็บไฟล์ชั่วคราว
            ListBoxFiles.Items.Clear();
        }


        // --------------- Button
        // ปุ่ม เพิ่มไฟล์ลงใน List
        protected void BtnAddFile_Click(object sender, EventArgs e)
        {
            // ตรวจสอบว่ามีไฟล์ใน Session หรือไม่ ถ้ามีก็เอาใส List
            if (Session["ListFiles"] != null)
            {
                ListFiles = (List<HttpPostedFile>)Session["ListFiles"];
            }
            // เพิ่มไฟล์ใหม่ลงใน List
            foreach (HttpPostedFile postedFile in FileUploadFile.PostedFiles)
            {
                ListFiles.Add(postedFile);
            }
            // เก็บไฟล์ใน List ลง Session เพื่อไม่ให้มันหาย
            Session["ListFiles"] = ListFiles;
            ListBoxFiles.Items.Clear(); // ล้างช่อง ListBox
            // แสดงชื่อไฟล์ที่อยู่ใน Session
            foreach (HttpPostedFile values in (List<HttpPostedFile>)Session["ListFiles"])
            {
                ListBoxFiles.Items.Add(values.FileName);
            }
            // ถ้ามีไฟล์ให้ปิดการตรวจสอบค่า
            if (((List<HttpPostedFile>)Session["ListFiles"]).Count > 0)
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
                ListFiles = (List<HttpPostedFile>)Session["ListFiles"];
                ListFiles.RemoveAt(ListBoxFiles.SelectedIndex);
                Session["ListFiles"] = ListFiles;
                ListBoxFiles.Items.Clear();
                // แสดงชื่อไฟล์ที่อยู่ใน Session
                foreach (HttpPostedFile values in (List<HttpPostedFile>)Session["ListFiles"])
                {
                    ListBoxFiles.Items.Add(values.FileName);
                }
                // ถ้าไม่มีไฟล์ให้เปิดการตรวจสอบค่า
                if (((List<HttpPostedFile>)Session["ListFiles"]).Count == 0)
                {
                    RequiredFieldValidatorFile.Enabled = true;
                }
            }
            LinkBtnDelFile.Visible = false;
        }


        // --------------- ListBox
        protected void ListBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ListBoxFiles.SelectedItem.Text.ToString()))
            {
                LinkBtnDelFile.Visible = false;
            }
            else
            {
                LinkBtnDelFile.Visible = true;
            }
        }


        // --------------- Button
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string DocTypeID = DDListDocType.SelectedValue; // เอกสารทางวิศวกรรมที่ต้องการแจ้งการดำเนินการ
                string OperationID = DDListOperation.SelectedValue; // ประเภทของการแจ้งการดำเนินการเอกสาร
                string OperationOther = TxtOperationOther.Text; // ประเภทของการแจ้งการดำเนินการเอกสาร อื่นๆ
                string TypeOfChangeID = RBListTypeOfChange.SelectedValue;   // ประเภทการเปลี่ยนแปลง
                string FGID = DDListFG.SelectedValue;   // Part NO.
                string DetailRequest = TxtDetailRequest.Text;   // รายละเอียดการแก้ไข/เพิ่มเติม
                string ReasonRequst = TxtReasonRequest.Text;    // เหตุผลที่ต้องแจ้งดำเนินการเอกสาร
                ListFiles = (List<HttpPostedFile>)Session["ListFiles"]; // เอกสารแนบ
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
                if (string.IsNullOrEmpty(ReasonRequst))
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

                // เพิ่มหัวหน้าแผนกตรวจสอบและรับทราบ
                sql = $"INSERT INTO DC_LeaderAccept (UserID, AcceptStatus) VALUES ({AcceptLeader}, 2)";
                query.Excute(sql);
                // ดึงข้อมูลที่เพิ่มล่าสุด
                sql = $"SELECT TOP 1 LeaderAcceptID FROM DC_LeaderAccept WHERE UserID = {AcceptLeader} AND AcceptStatus = 2 ORDER BY LeaderAcceptID DESC";
                string LeaderAccpetID = query.SelectAt(0, sql);

                // เพิ่ม NPD วิศวกร
                sql = $"INSERT INTO DC_NPDAccept (UserID, AcceptStatus) VALUES ({AcceptNPD}, 2)";
                query.Excute(sql);
                // ดึงข้อมูลที่เพิ่มล่าสุด
                sql = $"SELECT TOP 1 NPDAcceptID FROM DC_NPDAccept WHERE UserID = {AcceptNPD} AND AcceptStatus = 2 ORDER BY NPDAcceptID DESC";
                string NPDAcceptID = query.SelectAt(0, sql);

                // เพิ่ม NPD ผู้อนุมัติ
                sql = $"INSERT INTO DC_Approve (UserID, ApproveStatus) VALUES ({Approve}, 2)";
                query.Excute(sql);
                // ดึงข้อมูลที่เพิ่มล่าสุด
                sql = $"SELECT TOP 1 ApproveID FROM DC_Approve WHERE UserID = {Approve} AND ApproveStatus = 2 ORDER BY ApproveID DESC";
                string ApproveID = query.SelectAt(0, sql);

                // เพิ่มแบบฟอร์มแจ้งการดำเนินการเอกสารวิศวกรรม (Specification & Drawing)
                sql = $@"INSERT INTO DC_RequestSpec (UserID, RequestSpecDocTypeID, RequestSpecOperationID, OperationOther, FGID, DetailRequest, ReasonRequest, RequestSpecTypeOfChangeID, LeaderAcceptID, NPDAcceptID, ApproveID, RequestSpecStatusID) 
                VALUES ({UserID}, {DocTypeID}, {OperationID}, '{OperationOther}', {FGID}, '{DetailRequest}', '{ReasonRequst}', {TypeOfChangeID}, {LeaderAccpetID}, {NPDAcceptID}, {ApproveID}, 2)";
                query.Excute(sql);
                // ดึงข้อมูลที่เพิ่มล่าสุด
                sql = $@"SELECT TOP 1 RequestSpecID FROM DC_RequestSpec WHERE UserID = {UserID} ORDER BY RequestSpecID DESC";
                string RequestSpecID = query.SelectAt(0, sql);

                // เพิ่มไฟล์
                // ตรวจสอบว่ามี Folder หลักของการร้องขอนี้ หรือยัง ถ้ายังไม่มีให้สร้าง
                string FilePath = $"\\\\192.168.0.100\\PDoc\\RequestSpec\\{UserID}\\{RequestSpecID}";
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                int CountFile = 1;  // นับไฟล์ ใช้สร้าง Folder
                string DateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HHmmss");    // วันเดือนปีเวลาใช้สร้าง Folder
                foreach (HttpPostedFile postedFile in ListFiles)
                {
                    // ตรวจสอบว่ามี Folder ย่อยสำหรับแต่ละเอกสารหริอยัง ถ้ายังไม่มีให้สร้าง
                    string FilePathSub = $"{FilePath}\\{DateTimeNow} {CountFile}";
                    if (!Directory.Exists(FilePathSub))
                    {
                        Directory.CreateDirectory(FilePathSub);
                    }
                    string FileName = Path.GetFileNameWithoutExtension(postedFile.FileName); // ชื่อไฟล์
                    string Extension = Path.GetExtension(postedFile.FileName);  // นามสกุลไฟล์
                    string FullFileName = StringSpecial.StringSpecial(FileName) + Extension;    // ชื่อไฟล์ที่ตัดอักษรพิเศษ กับนามสกุลไฟล์
                    string FullFilePath = Path.Combine(FilePathSub, FullFileName);  // ที่อยู่จัดเก็บไฟล์
                    FullFilePath = FullFilePath.Replace("'", "''"); // ป้องกันการบัคซักหน่อย
                    postedFile.SaveAs(FullFilePath);    // บันทึกไฟล์
                    // เพิ่มข้อมูลไฟล์
                    sql = $@"INSERT INTO DC_RequestSpecDoc (RequestSpecID, FileName, FileExtension, FilePath, RequestSpecDocStatusID)
                    VALUES ({RequestSpecID}, '{FileName}', '{Extension}', '{FullFilePath}', 1)";
                    query.Excute(sql);
                    CountFile++;    // เพิ่มค่าไป 1
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'การร้องขอสำเร็จ.', 'success');", true);
                SetEmptyData();
                // แจ้งเตือน
                string Name = DDListAcceptLeader.SelectedItem.Text;
                LineNotify.Notify($"มีรายการอนุมัติ\nถึงคุณ: {Name}\nรายละเอียด: http://110.77.148.173/DocumentControl/DocumentRequest/Approve.aspx");
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
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