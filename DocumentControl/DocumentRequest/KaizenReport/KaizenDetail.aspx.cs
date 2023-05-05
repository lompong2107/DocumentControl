using DocumentControl.Admin;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest.KaizenReport
{
    public partial class KaizenDetail : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        LineNotify LineNotify = new LineNotify();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["KaizenID"] != null)
            {
                string KaizenID = Request.QueryString["KaizenID"];
                Session["LastPage"] = $"~/DocumentRequest/KaizenReport/KaizenDetail.aspx?KaizenID={KaizenID}";
            }
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["KaizenID"] != null && Session["UserID"] != null)
                {
                    string KaizenID = Request.QueryString["KaizenID"];
                    HFKaizenID.Value = KaizenID;
                    LoadKaizenDetail(KaizenID);
                }
            }
        }


        // --------------- Button
        protected void BtnLeaderAccept_Click(object sender, EventArgs e)
        {
            try
            {
                string KaizenID = HFKaizenID.Value;
                sql = @"SELECT LeaderAcceptID FROM DC_Kaizen WHERE KaizenID = " + KaizenID;
                string LeaderAcceptID = query.SelectAt(0, sql);
                sql = "UPDATE DC_LeaderAccept SET AcceptDate = GETDATE(), AcceptStatus = 1 WHERE LeaderAcceptID = " + LeaderAcceptID;
                query.Excute(sql);
                sql = "UPDATE DC_Kaizen SET KaizenStatusID = 2 WHERE KaizenID = " + KaizenID;
                query.Excute(sql);
                // แจ้งเตือน
                string Name = LbApproveName.Text;
                //LineNotify.Notify("มีรายการอนุมัติ\nถึงคุณ: " + Name + "\nรายละเอียด: http://110.77.148.173/DocumentControl/DocumentRequest/Approve.aspx");
                LoadKaizenDetail(KaizenID);
                UpdatePanelStatus.Update();
                UpdatePanel.Update();
                Navbar navbar = (Navbar)Page.Master;
                navbar.CheckNotification();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'รับทราบแล้ว.', 'success');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
        protected void BtnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                string KaizenID = HFKaizenID.Value;
                sql = @"SELECT ApproveID FROM DC_Kaizen WHERE KaizenID = " + KaizenID;
                string ApproveID = query.SelectAt(0, sql);
                sql = "UPDATE DC_Approve SET ApproveDate = GETDATE(), ApproveStatus = 1 WHERE ApproveID = " + ApproveID;
                query.Excute(sql);
                sql = "UPDATE DC_Kaizen SET KaizenStatusID = 4 WHERE KaizenID = " + KaizenID;
                query.Excute(sql);
                LoadKaizenDetail(KaizenID);
                UpdatePanelStatus.Update();
                UpdatePanel.Update();
                Navbar navbar = (Navbar)Page.Master;
                navbar.CheckNotification();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'อนุมัติแล้ว.', 'success');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
        protected void BtnDisApprove_Click(object sender, EventArgs e)
        {
            try
            {
                string Remark = TxtRemark.Text;
                if (string.IsNullOrEmpty(Remark))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณาระบุเหตุผล.', 'warning');", true);
                    return;
                }
                string KaizenID = HFKaizenID.Value;
                sql = @"SELECT ApproveID FROM DC_Kaizen WHERE KaizenID = " + KaizenID;
                string ApproveID = query.SelectAt(0, sql);
                sql = "UPDATE DC_Approve SET ApproveDate = GETDATE(), ApproveStatus = 3 WHERE ApproveID = " + ApproveID;
                query.Excute(sql);
                sql = "UPDATE DC_Kaizen SET KaizenStatusID = 3, Remark = '" + Remark + "' WHERE KaizenID = " + KaizenID;
                query.Excute(sql);
                LoadKaizenDetail(KaizenID);
                UpdatePanelStatus.Update();
                UpdatePanel.Update();
                Navbar navbar = (Navbar)Page.Master;
                navbar.CheckNotification();
                // แจ้งเตือน
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'ปฏิเสธแล้ว.', 'success');", true);
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


        // --------------- Function โหลดข้อมูลใบ Kaizen Report
        private void LoadKaizenDetail(string KaizenID)
        {
            try
            {
                sql = $@"SELECT DC_Kaizen.KaizenID, DC_Kaizen.KaizenTopic, DC_Kaizen.DateCreate, F2_Department.DepartmentName
                , DC_LeaderAccept.UserID AS LeaderUserID, DC_LeaderAccept.AcceptStatus AS LeaderAcceptStatus, DC_Approve.UserID AS ApproveUserID, DC_Approve.ApproveStatus, DC_Kaizen.KaizenStatusID, DC_KaizenStatus.KaizenStatusDetail
                , (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, (LeaderUser.FirstNameTH + ' ' + LeaderUser.LastNameTH) AS LeaderName, (ApproveUser.FirstNameTH + ' ' + ApproveUser.LastNameTH) AS ApproveName
                , DC_Kaizen.Remark
                FROM DC_Kaizen
                LEFT JOIN DC_KaizenStatus ON DC_Kaizen.KaizenStatusID =  DC_KaizenStatus.KaizenStatusID
                LEFT JOIN F2_Department ON DC_Kaizen.DepartmentID = F2_Department.DepartmentID
                LEFT JOIN F2_Users ON DC_Kaizen.UserID = F2_Users.UserID
                LEFT JOIN DC_LeaderAccept ON DC_Kaizen.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN F2_Users AS LeaderUser ON DC_LeaderAccept.UserID = LeaderUser.UserID
                LEFT JOIN DC_Approve ON DC_Kaizen.ApproveID = DC_Approve.ApproveID
                LEFT JOIN F2_Users AS ApproveUser ON DC_Approve.UserID = ApproveUser.UserID
                WHERE DC_Kaizen.KaizenID = {KaizenID}";
                string Topic = query.SelectAt(1, sql);
                string DateCreate = query.SelectAt(2, sql);
                string DepartmentName = query.SelectAt(3, sql);
                // เช็คสถานะ
                string UserID = Session["UserID"].ToString();
                string LeaderUserID = query.SelectAt(4, sql);
                string LeaderAcceptStatus = query.SelectAt(5, sql);
                string ApproveUserID = query.SelectAt(6, sql);
                string ApproveStatus = query.SelectAt(7, sql);
                string KaizenStatusID = query.SelectAt(8, sql);
                string KaizenStatusDetail = query.SelectAt(9, sql);
                // ชื่อ
                string NameRequest = query.SelectAt(10, sql);
                string NameLeader = query.SelectAt(11, sql);
                string NameApprove = query.SelectAt(12, sql);
                // เหตุผลการไม่อนุมัติ
                string Remark = query.SelectAt(13, sql);

                LbDateCreate.Text = DateTime.Parse(DateCreate).ToString("dd/MM/yyyy");
                LbKaizenStatus.Text = KaizenStatusDetail;
                LbTopic.Text = Topic;
                LbUserRequest.Text = NameRequest;
                LbDepartment.Text = DepartmentName;
                LbLeaderName.Text = NameLeader;
                LbApproveName.Text = NameApprove;

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

                // การแสดงสถานะของผู้ตรวจสอบ รับทราบ อนุมัติ
                // ผู้ตรวจสอบ
                if (LeaderAcceptStatus == "2")
                {
                    LbLeaderStatus.Text = "(รอตรวจสอบ)";
                    LbLeaderStatus.CssClass = "text-warning";
                }
                else if (LeaderAcceptStatus == "1")
                {
                    LbLeaderStatus.Text = "(ตรวจสอบแล้ว)";
                    LbLeaderStatus.CssClass = "text-success";
                }
                // ผู้อนุมัติ
                if (ApproveStatus == "2")
                {
                    LbApproveStatus.Text = "(รออนุมัติ)";
                    LbApproveStatus.CssClass = "text-warning";
                }
                else if (ApproveStatus == "1")
                {
                    LbApproveStatus.Text = "(อนุมัติแล้ว)";
                    LbApproveStatus.CssClass = "text-success";
                }
                else if (ApproveStatus == "3")
                {
                    LbApproveStatus.Text = "(ไม่อนุมัติ " + Remark + ")";
                    LbApproveStatus.CssClass = "text-danger";
                }

                // แสดง ซ่อน ปุ่มการรับทราบ และอนุมัติ
                // สถานะ รอตรวจสอบ และ รอผู้ตรวจสอบ และเป็นผู้ตรวจสอบ
                if (KaizenStatusID == "1" && LeaderAcceptStatus == "2" && LeaderUserID == UserID)
                {
                    PanelLeaderAccept.Visible = true;
                }
                // ตรวจสอบแล้ว และเป็นผู้ตรวจสอบ
                if (LeaderAcceptStatus == "1" && LeaderUserID == UserID)
                {
                    PanelLeaderAccept.Visible = false;
                }
                // สถานะ รออนุมัติ และ รออนุมัติ และเป็นผู้อนุมัติ
                if (KaizenStatusID == "2" && ApproveStatus == "2" && ApproveUserID == UserID)
                {
                    PanelApprove.Visible = true;
                }
                // สถานะ ไม่อนุมัติหรืออนุมัติแล้ว และสถานะไม่รออนุมัติ และเป็นผู้อนุมัติ
                if ((KaizenStatusID == "3" || KaizenStatusID == "4") && (ApproveStatus != "2") && (ApproveUserID == UserID))
                {
                    PanelApprove.Visible = false;
                }

                // สถานะใบ Kaizen
                // สถานะ ยกเลิก
                if (KaizenStatusID == "0")
                {
                    LbKaizenStatus.CssClass = "text-secondary";
                }
                // สถานะ ไม่อนุมัติ
                if (KaizenStatusID == "3")
                {
                    LbKaizenStatus.CssClass = "text-danger";
                    // เช็คว่าเป็นผู้สร้างใบ Kaizen นี้หรือไม่
                    sql = $"SELECT KaizenID FROM DC_Kaizen WHERE UserID = {UserID} AND KaizenID = {KaizenID}";
                    if (query.CheckRow(sql))
                    {
                        PanelEdit.Visible = true;
                    }
                }
                // สถานะ อนุมัติแล้ว
                if (KaizenStatusID == "4")
                {
                    LbKaizenStatus.CssClass = "text-success";
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
    }
}