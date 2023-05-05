using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest.KaizenReport
{
    public partial class KaizenEdit : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        LineNotify LineNotify = new LineNotify();
        StringSpecialClass StringSpecial = new StringSpecialClass();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["KaizenID"] != null && Session["UserID"] != null)
                {
                    string KaizenID = Request.QueryString["KaizenID"];
                    HFKaizenID.Value = KaizenID;

                    // Load Data
                    LoadUserRequest(KaizenID);
                    LoadDDListDepartment();
                    LoadKaizenDetail(KaizenID);
                }
            }
        }


        // --------------- Function
        // ข้อมูลผู้สร้าง
        private void LoadUserRequest(string KaizenID)
        {
            sql = $@"SELECT (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) As Name
                FROM DC_Kaizen
                INNER JOIN F2_Users ON DC_Kaizen.UserID = F2_Users.UserID
                WHERE KaizenID = {KaizenID}";
            TxtUserRequest.Text = query.SelectAt(0, sql);
        }
        // รายการหน่วยงาน
        private void LoadDDListDepartment()
        {
            sql = $@"SELECT DepartmentID, DepartmentName FROM F2_Department WHERE Showstatus = 1 ORDER BY DepartmentName";
            DDListDepartment.DataSource = query.SelectTable(sql);
            DDListDepartment.DataBind();
        }
        // รายการผู้ตรวจสอบ
        private void LoadDDListAcceptLeader(string KaizenID)
        {
            string DepartmentID = DDListDepartment.SelectedValue;
            sql = $@"SELECT F2_Users.UserID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) As Name 
            FROM F2_UserOrganization 
            LEFT JOIN F2_Users ON F2_UserOrganization.UserID = F2_Users.UserID 
            WHERE F2_UserOrganization.DepartmentID = {DepartmentID} AND F2_Users.Status = 1";
            DDListAcceptLeader.DataSource = query.SelectTable(sql);
            DDListAcceptLeader.DataBind();
        }
        // รายการผู้อนุมัติ
        private void LoadDDListApprove()
        {
            string DepartmentID = DDListDepartment.SelectedValue;
            sql = $@"SELECT F2_Users.UserID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) As Name 
            FROM F2_UserOrganization 
            LEFT JOIN F2_Users ON F2_UserOrganization.UserID = F2_Users.UserID
            WHERE F2_UserOrganization.DepartmentID = {DepartmentID} AND F2_Users.PositionsID IN (6,7,9,11) AND F2_Users.Status = 1";
            DDListApprove.DataSource = query.SelectTable(sql);
            DDListApprove.DataBind();
        }
        // โหลดข้อมูล Kaizen Report
        private void LoadKaizenDetail(string KaizenID)
        {
            sql = $@"SELECT DC_Kaizen.KaizenID, DC_Kaizen.KaizenTopic, DC_Kaizen.DateCreate, F2_Department.DepartmentID
            , DC_LeaderAccept.UserID AS LeaderUserID, DC_Approve.UserID AS UserID
            FROM DC_Kaizen
            LEFT JOIN DC_LeaderAccept ON DC_Kaizen.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
            LEFT JOIN DC_Approve ON DC_Kaizen.ApproveID = DC_Approve.ApproveID
            LEFT JOIN F2_Users ON DC_Kaizen.UserID = F2_Users.UserID
            LEFT JOIN F2_Department ON DC_Kaizen.DepartmentID = F2_Department.DepartmentID
            WHERE DC_Kaizen.KaizenID = {KaizenID}";
            string Topic = query.SelectAt(1, sql);
            string DateCreate = query.SelectAt(2, sql);
            string DepartmentID = query.SelectAt(3, sql);
            string LeaderUserID = query.SelectAt(4, sql);
            string ApproveUserID = query.SelectAt(5, sql);

            TxtTopic.Text = Topic;
            LbDateCreate.Text = DateTime.Parse(DateCreate).ToString("dd/MM/yyyy");
            DDListDepartment.SelectedValue = DepartmentID;
            LoadDDListAcceptLeader(KaizenID);
            LoadDDListApprove();
            DDListAcceptLeader.SelectedValue = LeaderUserID;
            DDListApprove.SelectedValue = ApproveUserID;

            // ไฟล์ล่าสุด
            sql = $@"SELECT KaizenDocID, Filepath FROM DC_KaizenDoc WHERE KaizenID = {KaizenID} AND Status = 1 ORDER BY DateCreate DESC";
            DataTable dt = query.SelectTable(sql);
            string KaizenDocID = query.SelectAt(0, sql);
            string FilePath = query.SelectAt(1, sql);
            HLOpenFile.NavigateUrl = $"~/DocumentRequest/KaizenReport/ShowPDF.aspx?KaizenDocID={KaizenDocID}";
            BtnDownload.CommandArgument = FilePath;
            if (dt.Rows.Count > 1)
            {
                BtnHistory.Visible = true;
                BtnHistory.CommandArgument = KaizenID;
            }
            else
            {
                BtnHistory.Visible = false;
            }
        }


        // --------------- Button
        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string KaizenID = Request.QueryString["KaizenID"];
                // ดึงข้อมูลเดิม
                sql = $@"SELECT UserID, LeaderAcceptID, ApproveID FROM DC_Kaizen WHERE KaizenID = {KaizenID}";
                string UserIDCreate = query.SelectAt(0, sql);   // คนสร้างใบ Kaizen
                string LeaderAcceptID = query.SelectAt(1, sql);
                string ApproveID = query.SelectAt(2, sql);

                // ข้อมูลใหม่
                string Topic = TxtTopic.Text;   // เรื่อง
                string UserID = Session["UserID"].ToString();   // คนอัปเดตใบ Kaizen
                string DepartmentID = DDListDepartment.SelectedValue;   // หน่วยงาน
                string AcceptLeader = DDListAcceptLeader.SelectedValue;
                string Approve = DDListApprove.SelectedValue;

                // ---------- Start ตรวจสอบการกรอกข้อมูล ----------
                // เรื่อง
                if (string.IsNullOrEmpty(TxtTopic.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณาระบุเรื่อง.', 'warning');", true);
                    return;
                }
                // ---------- END ตรวจสอบการกรอกข้อมูล ----------

                // อัปเดตหัวหน้าแผนกตรวจสอบและรับทราบ
                sql = $"UPDATE DC_LeaderAccept SET UserID = {AcceptLeader}, AcceptDate = NULL, AcceptStatus = 2 WHERE LeaderAcceptID = {LeaderAcceptID}";
                query.Excute(sql);
                // อัปเดตผู้จัดการโรงงานอนุมัติ
                sql = $"UPDATE DC_Approve SET UserID = {Approve}, ApproveDate = NULL, ApproveStatus = 2 WHERE ApproveID = {ApproveID}";
                query.Excute(sql);

                // อัปเดตใบ Kaizen
                sql = $@"UPDATE DC_Kaizen SET UserIDUpdate = {UserID}, DateUpdate = GETDATE(), KaizenTopic = '{Topic}', Remark = NULL, KaizenStatusID = 1 WHERE KaizenID = {KaizenID}";
                query.Excute(sql);


                // เช็คว่าได้ใส่ไฟล์ไหม
                if (FileUploadFile.HasFile)
                {
                    // ตรวจสอบว่ามีโฟลเดอร์แล้วหรือยัง ถ้ายังก็สร้างเลย
                    string FilePath = $"\\\\192.168.0.100\\PDoc\\KaizenReport\\{UserIDCreate}\\{KaizenID}\\{DateTime.Now.ToString("yyyy-MM-dd HHmmss")}";
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }

                    string FileName = Path.GetFileNameWithoutExtension(FileUploadFile.FileName);
                    string Extension = Path.GetExtension(FileUploadFile.FileName);
                    string FullFileName = StringSpecial.StringSpecial(FileName) + "" + Extension;
                    string FullFilePath = Path.Combine(FilePath, FullFileName);
                    FileUploadFile.SaveAs(FullFilePath);
                    sql = @"INSERT INTO DC_KaizenDoc (KaizenID, FileExtension, FilePath) VALUES (" + KaizenID + ", '" + Extension.Replace(".", "") + "', '" + FullFilePath + "')";
                    query.Excute(sql);
                }

                // แจ้งเตือน
                // ตรวจสอบก่อนว่าผู้ตรวจสอบตรวจสอบหรือยัง ถ้าตรวจสอบแล้วให้แจ้งเตือนใหม่ ถ้ายังไม่ตรวจสอบ ไม่ต้องส่งแจ้งเตือนไปใหม่
                sql = $"SELECT AcceptStatus FROM DC_LeaderAccept WHERE LeaderAcceptID = {LeaderAcceptID}";
                int LeaderAcceptStatus = int.Parse(query.SelectAt(0, sql));
                if (LeaderAcceptStatus == 1)
                {
                    string Name = DDListAcceptLeader.SelectedItem.Text;
                    //LineNotify.Notify("มีรายการอนุมัติ\nถึงคุณ: " + Name + "\nรายละเอียด: http://110.77.148.173/DocumentControl/DocumentRequest/Approve.aspx");
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotificationAndRedirect('สำเร็จ', 'บันทึกข้อมูลสำเร็จ.', 'success', '{ResolveClientUrl("~/DocumentRequest/KaizenReport/KaizenHistory.aspx")}');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
        protected void BtnDownload_Click(object sender, EventArgs e)
        {
            string FilePath = BtnDownload.CommandArgument;
            Response.ContentType = ContentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(FilePath));
            Response.WriteFile(FilePath);
            Response.End();
        }

        // เปิดประวัติไฟล์
        protected void BtnHistory_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "New_Window", "window.open('KaizenHistoryFile.aspx?KaizenID=" + BtnHistory.CommandArgument + "', '_blank', 'location=yes,height=570,width=600,scrollbars=yes,status=yes');", true);
        }
    }
}