using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest.KaizenReport
{
    public partial class Kaizen : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        LineNotify LineNotify = new LineNotify();
        StringSpecialClass StringSpecial = new StringSpecialClass();
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LastPage"] = "~/DocumentRequest/KaizenReport/Kaizen.aspx";
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    LoadUserRequest();
                    LoadDDListAcceptLeader();
                    LoadDDListApprove();
                }
            }
        }
        private void LoadUserRequest()
        {
            string UserID = Session["UserID"].ToString();
            sql = @"SELECT (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) As Name, F2_Department.DepartmentName FROM F2_Users
            INNER JOIN F2_Department ON F2_Users.DepartmentID = F2_Department.DepartmentID
            WHERE F2_Users.UserID = " + UserID;
            TxtUserRequest.Text = query.SelectAt(0, sql);
            TxtDepartmentRequest.Text = query.SelectAt(1, sql);
        }
        private void LoadDDListAcceptLeader()
        {
            string DepartmentID = Session["DepartmentID"].ToString();
            sql = "SELECT F2_Users.UserID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) As Name FROM F2_UserOrganization LEFT JOIN F2_Users ON F2_UserOrganization.UserID = F2_Users.UserID WHERE F2_UserOrganization.DepartmentID = " + DepartmentID + " AND F2_Users.Status = 1 ORDER BY F2_Users.FirstNameTH";
            DDListAcceptLeader.DataSource = query.SelectTable(sql);
            DDListAcceptLeader.DataBind();
        }

        private void LoadDDListApprove()
        {
            string DepartmentID = Session["DepartmentID"].ToString();
            sql = "SELECT F2_Users.UserID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) As Name FROM F2_UserOrganization LEFT JOIN F2_Users ON F2_UserOrganization.UserID = F2_Users.UserID WHERE F2_UserOrganization.DepartmentID = " + DepartmentID + " AND F2_Users.Status = 1 ORDER BY F2_Users.FirstNameTH";
            DDListApprove.DataSource = query.SelectTable(sql);
            DDListApprove.DataBind();
        }

        // --------------- Button
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string Topic = TxtTopic.Text.Replace("'", "''");
                string UserID = Session["UserID"].ToString();
                string DepartmentID = Session["DepartmentID"].ToString();
                string AcceptLeader = DDListAcceptLeader.SelectedValue;
                string Approve = DDListApprove.SelectedValue;

                // ---------- Start ตรวจสอบการกรอกข้อมูล ----------
                // เรื่อง
                if (string.IsNullOrEmpty(TxtTopic.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณาระบุเรื่อง.', 'warning');", true);
                    return;
                }
                // แนบไฟล์
                if (!FileUploadFile.HasFile)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณาแนบไฟล์.', 'warning');", true);
                    return;
                }
                // ---------- END ตรวจสอบการกรอกข้อมูล ----------

                // เพิ่มหัวหน้าแผนกตรวจสอบและรับทราบ
                sql = "INSERT INTO DC_LeaderAccept (UserID, AcceptStatus) VALUES (" + AcceptLeader + ", 2)";
                query.Excute(sql);
                // ดึงข้อมูลที่เพิ่มล่าสุด
                sql = "SELECT TOP 1 LeaderAcceptID FROM DC_LeaderAccept WHERE UserID = " + AcceptLeader + " AND AcceptStatus = 2 ORDER BY LeaderAcceptID DESC";
                string LeaderAccpetID = query.SelectAt(0, sql);
                // เพิ่มผู้จัดการโรงงานอนุมัติ
                sql = "INSERT INTO DC_Approve (UserID, ApproveStatus) VALUES (" + Approve + ", 2)";
                query.Excute(sql);
                // ดึงข้อมูลที่เพิ่มล่าสุด
                sql = "SELECT TOP 1 ApproveID FROM DC_Approve WHERE UserID = " + Approve + " AND ApproveStatus = 2 ORDER BY ApproveID DESC";
                string ApproveID = query.SelectAt(0, sql);
                // เพิ่มใบ Kaizen
                sql = $@"INSERT INTO DC_Kaizen (DepartmentID ,UserID, KaizenTopic, LeaderAcceptID, ApproveID, KaizenStatusID)
                VALUES ({DepartmentID}, {UserID}, '{Topic}', {LeaderAccpetID}, {ApproveID}, 1)";
                query.Excute(sql);
                // ดึงข้อมูลที่เพิ่มล่าสุด
                sql = "SELECT TOP 1 KaizenID FROM DC_Kaizen WHERE UserID = " + UserID + " ORDER BY KaizenID DESC";
                string KaizenID = query.SelectAt(0, sql);
                string FilePath = "\\\\192.168.0.100\\PDoc\\KaizenReport\\" + UserID + "\\" + KaizenID + "\\" + DateTime.Now.ToString("yyyy-MM-dd HHmmss");

                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }

                // เพิ่มเอกสาร
                string FileName = Path.GetFileNameWithoutExtension(FileUploadFile.FileName);
                string Extension = Path.GetExtension(FileUploadFile.FileName);
                string FullFileName = StringSpecial.StringSpecial(FileName) + "" + Extension;
                string FullFilePath = Path.Combine(FilePath, FullFileName);
                FullFilePath = FullFilePath.Replace("'", "''");
                if (FileName.Length > 0)
                {
                    FileUploadFile.SaveAs(FullFilePath);
                }
                sql = @"INSERT INTO DC_KaizenDoc (KaizenID, FileExtension, FilePath) VALUES (" + KaizenID + ", '" + Extension.Replace(".", "") + "', '" + FullFilePath + "')";
                query.Excute(sql);

                // Set Empty Data
                SetEmptyData();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'การบันทึกสำเร็จ.', 'success');", true);
                // แจ้งเตือน
                string Name = DDListAcceptLeader.SelectedItem.Text;
                //LineNotify.Notify("มีรายการอนุมัติ\nถึงคุณ: " + Name + "\nรายละเอียด: http://110.77.148.173/DocumentControl/DocumentRequest/Approve.aspx");
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // Function
        public void SetEmptyData()
        {
            TxtTopic.Text = string.Empty;
        }
    }
}