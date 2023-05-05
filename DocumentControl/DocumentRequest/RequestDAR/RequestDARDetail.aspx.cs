using DocumentControl.DocumentRequest.KaizenReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest.RequestDAR
{
    public partial class RequestDARDetail : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        LineNotify LineNotify = new LineNotify();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["RequestDARID"] != null)
            {
                string RequestDARID = Request.QueryString["RequestDARID"];
                Session["LastPage"] = $"~/DocumentRequest/RequestDAR/RequestDARDetail.aspx?RequestDARID={RequestDARID}";
            }
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["RequestDARID"] != null && Session["UserID"] != null)
                {
                    string RequestDARID = Request.QueryString["RequestDARID"];
                    HFRequestDARID.Value = RequestDARID;
                    LoadRequestDARDetail(RequestDARID);
                }
            }
        }


        // --------------- GridView
        protected void GVRequestDARDoc_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string Btn = e.CommandName;
                string RequestDARDocID = e.CommandArgument.ToString();
                if (Btn == "BtnDownload")
                {
                    string FilePath = e.CommandArgument.ToString();
                    Response.ContentType = ContentType;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(FilePath));
                    Response.WriteFile(FilePath);
                    Response.End();
                }
                else if (Btn == "BtnUpdateStatus")
                {
                    string UserID = Session["UserID"].ToString();
                    // อัปเดตเอกสารว่าแจกจ่ายแล้ว
                    sql = $"UPDATE DC_RequestDARDoc SET RequestDARDocStatusID = 2, UserIDUpdate = {UserID}, PublishDate = GETDATE() WHERE RequestDARDocID = {RequestDARDocID}";
                    if (query.Excute(sql))
                    {
                        string RequestDARID = HFRequestDARID.Value;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'อัปเดตสถานะแจกจ่ายสำเร็จ.', 'success');", true);
                        LoadRequestDARDetail(RequestDARID);
                        UpdatePanel.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
        protected void GVRequestDARDoc_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                // ตรวจสอบว่าการร้องขอเป็นสถานะ อยู่ระหว่างการดำเนินการแก้ไข
                string RequestDARID = HFRequestDARID.Value;
                if (!string.IsNullOrEmpty(RequestDARID))
                {
                    sql = $"SELECT RequestDARStatusID FROM DC_RequestDAR WHERE RequestDARID = {RequestDARID}";
                    string RequestDARStatusID = query.SelectAt(0, sql);
                    // สถานะคำร้องขอ RequestDAR ไม่เท่ากับ รอดำเนินการแก้ไข (DC) และ แจกจ่ายแล้ว และ เสร็จสมบูรณ์
                    if (RequestDARStatusID != "5" && RequestDARStatusID != "6" && RequestDARStatusID != "7")
                    {
                        // ช่องสถานะ
                        e.Row.Cells[5].Visible = false;
                    }
                    // สถานะคำร้องขอ RequestDAR ไม่เท่ากับ รอดำเนินการแก้ไข (DC) หรือ (สถานะคำร้องขอ RequestDAR เท่ากับ รอดำเนินการแก้ไข (DC) และ ไม่มีสิทธิ์อัปเดตสถานะแจกจ่ายเอกสาร)
                    if (RequestDARStatusID != "5" || (RequestDARStatusID == "5" && !CheckPermission()))
                    {
                        // ช่องอัปเดตสถานะแจกจ่ายเอกสาร
                        e.Row.Cells[6].Visible = false;
                    }

                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        // ถ้าไม่มีไฟล์ก็ซ้อนปุ่มดาวน์โหลด
                        string FilePath = DataBinder.Eval(e.Row.DataItem, "FilePath").ToString();
                        ImageButton ImageBtnDownload = e.Row.FindControl("ImageBtnDownload") as ImageButton;
                        if (!File.Exists(FilePath))
                        {
                            ImageBtnDownload.Visible = false;
                        }

                        string StatusID = DataBinder.Eval(e.Row.DataItem, "RequestDARDocStatusID").ToString();
                        Panel PanelStatus = e.Row.FindControl("PanelStatus") as Panel;
                        if (StatusID == "1")
                        {
                            // สถานะรอแจกจ่าย
                            PanelStatus.CssClass = PanelStatus.CssClass + " bg-warning";
                        }
                        else if (StatusID == "2")
                        {
                            // สถานะแจกจ่ายแล้ว
                            ImageButton ImageBtnUpdateStatus = e.Row.FindControl("ImageBtnUpdateStatus") as ImageButton;
                            ImageBtnUpdateStatus.Visible = false;
                            PanelStatus.CssClass = PanelStatus.CssClass + " bg-success";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }


        // --------------- Button
        protected void BtnLeaderAccept_Click(object sender, EventArgs e)
        {
            try
            {
                string RequestDARID = HFRequestDARID.Value;
                sql = $"SELECT LeaderAcceptID, NPDAcceptID FROM DC_RequestDAR WHERE RequestDARID = {RequestDARID}";
                string LeaderAcceptID = query.SelectAt(0, sql);
                string NPDAcceptID = query.SelectAt(1, sql);
                sql = $"UPDATE DC_LeaderAccept SET AcceptDate = GETDATE(), AcceptStatus = 1 WHERE LeaderAcceptID = {LeaderAcceptID}";
                query.Excute(sql);
                if (NPDAcceptID == "0")
                {
                    sql = $"UPDATE DC_RequestDAR SET RequestDARStatusID = 3 WHERE RequestDARID = {RequestDARID}";
                    query.Excute(sql);
                }
                // แจ้งเตือน
                string Name = string.Empty;
                if (LbOperation.Text == "จัดทำขึ้นใหม่" || LbOperation.Text == "ยกเลิกการใช้")
                {
                    Name = LbNPDName.Text;
                }
                else
                {
                    Name = LbApproveName.Text;
                }
                LineNotify.Notify("มีรายการอนุมัติ\nถึงคุณ: " + Name + "\nรายละเอียด: http://110.77.148.173/DocumentControl/DocumentRequest/Approve.aspx");
                LoadRequestDARDetail(RequestDARID);
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
        protected void BtnNPDAccept_Click(object sender, EventArgs e)
        {
            try
            {
                string RequestDARID = HFRequestDARID.Value;
                sql = $"SELECT NPDAcceptID FROM DC_RequestDAR WHERE RequestDARID = {RequestDARID}";
                string NPDAcceptID = query.SelectAt(0, sql);
                sql = $"UPDATE DC_NPDAccept SET AcceptDate = GETDATE(), AcceptStatus = 1 WHERE NPDAcceptID = {NPDAcceptID}";
                query.Excute(sql);
                sql = $"UPDATE DC_RequestDAR SET RequestDARStatusID = 3 WHERE RequestDARID = {RequestDARID}";
                query.Excute(sql);
                LoadRequestDARDetail(RequestDARID);
                UpdatePanelStatus.Update();
                UpdatePanel.Update();
                Navbar navbar = (Navbar)Page.Master;
                navbar.CheckNotification();
                // แจ้งเตือน
                string Name = LbApproveName.Text;
                LineNotify.Notify("มีรายการอนุมัติ\nถึงคุณ: " + Name + "\nรายละเอียด:http://110.77.148.173/DocumentControl/DocumentRequest/Approve.aspx");
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
                string RequestDARID = HFRequestDARID.Value;
                sql = $"SELECT ApproveID FROM DC_RequestDAR WHERE RequestDARID = {RequestDARID}";
                string ApproveID = query.SelectAt(0, sql);
                sql = $"UPDATE DC_Approve SET ApproveDate = GETDATE(), ApproveStatus = 1 WHERE ApproveID = {ApproveID}";
                query.Excute(sql);
                sql = $"UPDATE DC_RequestDAR SET RequestDARStatusID = 5 WHERE RequestDARID = {RequestDARID}";
                query.Excute(sql);
                LoadRequestDARDetail(RequestDARID);
                UpdatePanelStatus.Update();
                UpdatePanel.Update();
                Navbar navbar = (Navbar)Page.Master;
                navbar.CheckNotification();
                // แจ้งเตือน
                LineNotify.Notify("มีรายการที่อนุมัติแล้ว\nรายละเอียด:http://110.77.148.173/DocumentControl/DocumentRequest/RequestDAR/LogBook.aspx");
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
                string RequestDARID = HFRequestDARID.Value;
                sql = $"SELECT ApproveID FROM DC_RequestDAR WHERE RequestDARID = {RequestDARID}";
                string ApproveID = query.SelectAt(0, sql);
                sql = $"UPDATE DC_Approve SET ApproveDate = GETDATE(), ApproveStatus = 3 WHERE ApproveID = {ApproveID}";
                query.Excute(sql);
                sql = $"UPDATE DC_RequestDAR SET RequestDARStatusID = 4, Remark = '{Remark}' WHERE RequestDARID = {RequestDARID}";
                query.Excute(sql);
                LoadRequestDARDetail(RequestDARID);
                UpdatePanelStatus.Update();
                UpdatePanel.Update();
                Navbar navbar = (Navbar)Page.Master;
                navbar.CheckNotification();
                // แจ้งเตือน
                sql = $@"SELECT (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, F2_Department.DepartmentName 
                FROM DC_RequestDAR 
                INNER JOIN F2_Users ON DC_RequestDAR.UserID = F2_Users.UserID 
                INNER JOIN F2_Department ON F2_Users.DepartmentID = F2_Department.DepartmentID 
                WHERE RequestDARID = {RequestDARID}";
                string Name = query.SelectAt(0, sql);
                string DepartmentName = query.SelectAt(1, sql);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'ปฏิเสธแล้ว.', 'success');", true);
                LineNotify.Notify("ถูกปฏิเสธการอนุมัติ\nรหัสเอกสาร: " + RequestDARID + "\nถึงคุณ: " + Name + "\nรายละเอียด: http://110.77.148.173/DocumentControl/DocumentRequest/RequestDAR/RequestDARHistory.aspx");
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
        // รับทราบแจกจ่ายเอกสาร ของแต่ละแผนก
        protected void BtnPublishAccept_Click(object sender, EventArgs e)
        {
            try
            {
                string RequestDARID = HFRequestDARID.Value;
                string UserID = Session["UserID"].ToString();
                string DepartmentID = Session["DepartmentID"].ToString();
                sql = $@"UPDATE DC_RequestDARPublishAccept SET UserID = {UserID}, AcceptDate = GETDATE(), AcceptStatus = 1 WHERE RequestDARID = {RequestDARID} AND DepartmentID = {DepartmentID}";
                if (query.Excute(sql))
                {
                    // เช็คว่ารับทราบการแจกจ่ายเอกสารหมดหรือยัง
                    sql = $"SELECT RequestDARPublishAcceptID FROM DC_RequestDARPublishAccept WHERE RequestDARID = {RequestDARID} AND AcceptStatus = 2";
                    if (!query.CheckRow(sql))   // ถ้าไม่มีสถานะเอกสารรอดำเนินการแล้วให้แสดงการทำงานของการแจ้งเตือนและอัปเดตสถานะ DAR เป็นแจกจ่ายแล้ว
                    {
                        sql = $"UPDATE DC_RequestDAR SET RequestDARStatusID = 7 WHERE RequestDARID = {RequestDARID}";
                        query.Excute(sql);
                    }
                    LoadRequestDARDetail(RequestDARID);
                    UpdatePanelStatus.Update();
                    UpdatePanel.Update();
                    Navbar navbar = (Navbar)Page.Master;
                    navbar.CheckNotification();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'รับทราบแล้ว.', 'success');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }


        // --------------- Function โหลดข้อมูลเอกสาร DAR
        private void LoadRequestDARDetail(string RequestDARID)
        {
            try
            {
                string DepartmentID = Session["DepartmentID"].ToString();
                sql = $@"SELECT DC_RequestDAR.RequestDARID, DC_RequestDAR.DateRequest, DC_RequestDARDocType.DocTypeName, DC_RequestDAR.DocTypeOther, DC_RequestDAROperation.OperationName, DC_RequestDAR.OperationOther
                , DC_LeaderAccept.UserID AS LeaderUserID, DC_LeaderAccept.AcceptStatus AS LeaderAcceptStatus, DC_NPDAccept.UserID AS NPDUserID, DC_NPDAccept.AcceptStatus AS NPDAcceptStatus,  DC_Approve.UserID AS ApproveUserID, DC_Approve.ApproveStatus, DC_RequestDAR.RequestDARStatusID, DC_RequestDARStatus.RequestDARStatusDetail
                , (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, (LeaderUser.FirstNameTH + ' ' + LeaderUser.LastNameTH) AS LeaderName, (NPDUser.FirstNameTH + ' ' + NPDUser.LastNameTH) AS NPDName, (ApproveUser.FirstNameTH + ' ' + ApproveUser.LastNameTH) AS ApproveName
                , DC_RequestDAR.Remark, DC_RequestDARPublishAccept.AcceptStatus AS PublishAcceptStatus
                FROM DC_RequestDAR
                LEFT JOIN DC_RequestDARStatus ON DC_RequestDAR.RequestDARStatusID =  DC_RequestDARStatus.RequestDARStatusID
                LEFT JOIN DC_RequestDAROperation ON DC_RequestDAR.RequestDAROperationID = DC_RequestDAROperation.RequestDAROperationID
                LEFT JOIN DC_RequestDARDocType ON DC_RequestDAR.RequestDARDocTypeID = DC_RequestDARDocType.RequestDARDocTypeID
                LEFT JOIN F2_Users ON DC_RequestDAR.UserID = F2_Users.UserID
                LEFT JOIN DC_LeaderAccept ON DC_RequestDAR.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN F2_Users AS LeaderUser ON DC_LeaderAccept.UserID = LeaderUser.UserID
                LEFT JOIN DC_NPDAccept ON DC_RequestDAR.NPDAcceptID = DC_NPDAccept.NPDAcceptID
                LEFT JOIN F2_Users AS NPDUser ON DC_NPDAccept.UserID = NPDUser.UserID
                LEFT JOIN DC_Approve ON DC_RequestDAR.ApproveID = DC_Approve.ApproveID
                LEFT JOIN F2_Users AS ApproveUser ON DC_Approve.UserID = ApproveUser.UserID
                LEFT JOIN DC_RequestDARPublishAccept ON DC_RequestDAR.RequestDARID = DC_RequestDARPublishAccept.RequestDARID AND DC_RequestDARPublishAccept.DepartmentID = {DepartmentID}
                WHERE DC_RequestDAR.RequestDARID = {RequestDARID}";
                string DateRequest = query.SelectAt(1, sql);
                string DocTypeName = query.SelectAt(2, sql);
                string DocTypeOther = query.SelectAt(3, sql);
                string OperationName = query.SelectAt(4, sql);
                string OperationOther = query.SelectAt(5, sql);
                // เช็คสถานะ
                string UserID = Session["UserID"].ToString();
                string LeaderUserID = query.SelectAt(6, sql);
                string LeaderAcceptStatus = query.SelectAt(7, sql);
                string NPDUserID = query.SelectAt(8, sql);
                string NPDAcceptStatus = query.SelectAt(9, sql);
                string ApproveUserID = query.SelectAt(10, sql);
                string ApproveStatus = query.SelectAt(11, sql);
                string RequestDARStatusID = query.SelectAt(12, sql);
                string RequestDARStatusDetail = query.SelectAt(13, sql);
                // ชื่อ
                string NameRequest = query.SelectAt(14, sql);
                string NameLeader = query.SelectAt(15, sql);
                string NameNPD = query.SelectAt(16, sql);
                string NameApprove = query.SelectAt(17, sql);
                // เหตุผลการไม่อนุมัติ
                string Remark = query.SelectAt(18, sql);
                // เช็คสถานะการรับทราบเอกสารแจกจ่ายของแต่ละแผนก
                string PublishAcceptStatus = query.SelectAt(19, sql);

                // Set Value
                LbDateRequest.Text = DateTime.Parse(DateRequest).ToString("dd/MM/yyyy");
                LbRequestDARID.Text = int.Parse(RequestDARID).ToString("D3");
                LbRequestDARStatus.Text = RequestDARStatusDetail;
                LbDocType.Text = DocTypeName;
                LbDocTypeOther.Text = DocTypeOther;
                LbOperation.Text = OperationName;
                LbOperationOther.Text = OperationOther;
                LbUserRequest.Text = NameRequest;
                LbLeaderName.Text = NameLeader;
                LbNPDName.Text = NameNPD;
                LbApproveName.Text = NameApprove;

                GVRequestDARDoc.DataBind();

                if (OperationName == "จัดทำขึ้นใหม่" || OperationName == "ยกเลิกการใช้")
                {
                    PanelAcceptNPD.Visible = true;
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
                // ผู้รับทราบ
                if (NPDAcceptStatus == "2")
                {
                    LbNPDStatus.Text = "(รอรับทราบ)";
                    LbNPDStatus.CssClass = "text-warning";
                }
                else if (NPDAcceptStatus == "1")
                {
                    LbNPDStatus.Text = "(รับทราบแล้ว)";
                    LbNPDStatus.CssClass = "text-success";
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
                // การร้องขอ รอตรวจสอบ และ รอผู้ตรวจสอบ และเป็นผู้ตรวจสอบ
                if (RequestDARStatusID == "2" && LeaderAcceptStatus == "2" && LeaderUserID == UserID)
                {
                    PanelLeaderAccept.Visible = true;
                }
                // ตรวจสอบแล้ว และเป็นผู้ตรวจสอบ
                if (LeaderAcceptStatus == "1" && LeaderUserID == UserID)
                {
                    PanelLeaderAccept.Visible = false;
                }
                // การร้องขอ รอตรวจสอบ และ รอรับทราบ และเป็นผู้รับทราบ และตรวจสอบแล้ว
                if (RequestDARStatusID == "2" && NPDAcceptStatus == "2" && NPDUserID == UserID && LeaderAcceptStatus == "1")
                {
                    PanelNPDAccept.Visible = true;
                }
                // รับทราบแล้ว และเป็นผู้รับทราบ
                if (NPDAcceptStatus == "1" && NPDUserID == UserID)
                {
                    PanelNPDAccept.Visible = false;
                }
                // การร้องขอ รออนุมัติ และ รออนุมัติ และเป็นผู้อนุมัติ
                if (RequestDARStatusID == "3" && ApproveStatus == "2" && ApproveUserID == UserID)
                {
                    PanelApprove.Visible = true;
                }
                // การร้องขอ ไม่อนุมัติหรืออนุมัติแล้ว และสถานะไม่รออนุมัติ และเป็นผู้อนุมัติ
                if ((RequestDARStatusID == "4" || RequestDARStatusID == "5") && (ApproveStatus != "2") && (ApproveUserID == UserID))
                {
                    PanelApprove.Visible = false;
                }

                CheckPublishDoc();  // ตรวจสอบว่าพร้อมจะแจ้งเตือนแจกจ่ายเอกสารหรือยัง

                // การร้องขอ แจกจ่ายแล้ว และแผนกของผู้ใช้นั้นยังไม่กดรับทราบ และผู้ใช้อยู่ในแผนกที่ต้องกดรับทราบ
                if (RequestDARStatusID == "6" && PublishAcceptStatus == "2")
                {
                    PanelPublichAccept.Visible = true;
                }
                // การร้องขอ แจกจ่ายแล้ว และแผนกของผู้ใช้นั้นกดรับทราบแล้ว และผู้ใช้อยู่ในแผนกที่ต้องกดรับทราบ
                else if (RequestDARStatusID == "6" && PublishAcceptStatus == "1")
                {
                    PanelPublichAccept.Visible = false;
                }

                // การร้องขอ ยกเลิก
                if (RequestDARStatusID == "0")
                {
                    LbRequestDARStatus.CssClass = "text-secondary";
                }
                // การร้องขอ รอตรวจสอบ (HOD/NPD), รออนุมัติ (QMR/EMR), รอดำเนินการแก้ไข (DC)
                if (RequestDARStatusID == "2" || RequestDARStatusID == "3" || RequestDARStatusID == "5")
                {
                    LbRequestDARStatus.CssClass = "text-warning";
                }
                // การร้องขอ ไม่อนุมัติ
                if (RequestDARStatusID == "4")
                {
                    LbRequestDARStatus.CssClass = "text-danger";
                    // เช็คว่าเป็นผู้ร้องขอ นี้หรือไม่
                    sql = $"SELECT RequestDARID FROM DC_RequestDAR WHERE UserID = {UserID} AND RequestDARID = {RequestDARID}";
                    if (query.CheckRow(sql))
                    {
                        PanelEdit.Visible = true;
                    }
                }
                // การร้องขอ แจกจ่ายแล้ว
                if (RequestDARStatusID == "6")
                {
                    LoadLVAcceptPublish();
                    PanelListPublishAccept.Visible = true;
                    LbRequestDARStatus.CssClass = "text-info";
                }
                // การร้องขอ เสร็จแล้ว
                if (RequestDARStatusID == "7")
                {
                    LoadLVAcceptPublish();
                    PanelListPublishAccept.Visible = true;
                    PanelPublichAccept.Visible = false;
                    LbRequestDARStatus.CssClass = "text-success";
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        private void CheckPublishDoc()
        {
            try
            {
                string RequestDARID = HFRequestDARID.Value;
                sql = $@"SELECT RequestDARStatusID FROM DC_RequestDAR WHERE RequestDARID = {RequestDARID}";
                string RequestDARStatusID = query.SelectAt(0, sql);
                // เช็คว่าเผยแพร่เอกสารหมดหรือยัง
                sql = $"SELECT RequestDARDocID FROM DC_RequestDARDoc WHERE RequestDARID = {RequestDARID} AND RequestDARDocStatusID = 1";
                // การร้องขอ สถานะอยู่ระหว่างการดำเนินการแก้ไข และเอกสารแจกจ่ายทั้งหมดแล้ว
                if (RequestDARStatusID == "5" && !query.CheckRow(sql))
                {
                    // ถ้าไม่มีสถานะเอกสารรอดำเนินการแล้วให้แสดงการทำงานของการแจ้งเตือนและอัปเดตสถานะ DAR เป็นแจกจ่ายแล้ว
                    PanelSendNotification.Visible = true;
                }
                else
                {
                    PanelSendNotification.Visible = false;
                }
                LoadListBoxDepartmentNotification();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // -------------------- Load ListBox Department แจ้งเตือนเอกสารแจกจ่าย
        private void LoadListBoxDepartmentNotification()
        {
            try
            {
                sql = "SELECT DepartmentID, DepartmentName FROM F2_Department WHERE Showstatus = 1 ORDER BY DepartmentName";
                ListBoxDepartmentNotification.DataSource = query.SelectTable(sql);
                ListBoxDepartmentNotification.DataBind();
                string RequestDARID = HFRequestDARID.Value;
                sql = $@"SELECT F2_Department.DepartmentID
                FROM F2_Department 
                INNER JOIN F2_Users ON F2_Department.DepartmentID = F2_Users.DepartmentID
                INNER JOIN DC_RequestDAR ON F2_Users.UserID = DC_RequestDAR.UserID
                WHERE DC_RequestDAR.RequestDARID = {RequestDARID}";
                string DepartmentID = query.SelectAt(0, sql);
                ListBoxDepartmentNotification.SelectedValue = DepartmentID;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // ปุ่ม ส่งแจ้งตือน แจกจ่ายเอกสารแล้ว
        protected void BtnSendNotification_Click(object sender, EventArgs e)
        {
            try
            {
                string RequestDARID = HFRequestDARID.Value;
                List<String> ListDepartmentIDAll = HFDepartmentIDSelectedNotification.Value.Split(',').ToList();
                if (ListDepartmentIDAll.Count > 0)
                {
                    sql = $"UPDATE DC_RequestDAR SET RequestDARStatusID = 6 WHERE RequestDARID = {RequestDARID}";
                    if (query.Excute(sql))
                    {
                        // Insert แผนกที่แจ้งเตือน รอมากดรับทราบ
                        foreach (string DepartmentID in ListDepartmentIDAll)
                        {
                            sql = $@"INSERT INTO DC_RequestDARPublishAccept (RequestDARID, DepartmentID) VALUES ({RequestDARID}, {DepartmentID});";
                            query.Excute(sql);
                        }

                        // ดึงแผนกทั้งหมดที่เลือก
                        List<string> selectedItemsList = new List<string>();
                        foreach (ListItem item in ListBoxDepartmentNotification.Items)
                            if (item.Selected)
                                selectedItemsList.Add(item.Text);

                        // แจ้งเตือนกลุ่มแจกจ่ายเอกสารควบคุม
                        string DepartmentName = string.Join(", ", selectedItemsList);
                        LineNotify.NotifyDocumentControl($"แจกจ่ายเอกสารแล้ว\nรหัสเอกสาร: {RequestDARID}\nถึงแผนก: {DepartmentName}\nรายละเอียด: http://110.77.148.173/DocumentControl/DocumentRequest/RequestDAR/RequestDARDetail.aspx?RequestDARID={RequestDARID}");
                        LoadRequestDARDetail(RequestDARID);
                        UpdatePanelStatus.Update();
                        UpdatePanel.Update();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'ส่งแจ้งเตือนสำเร็จ.', 'success');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ข้อมูลไม่ถูกต้อง!', 'กรุณาเลือกแผนกที่ต้องการแจ้งเตือน.', 'warning');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\",`{ex.Message}`, \"error\");", true);
            }
        }

        // โหลดข้อมูลสถานะผู้รับทราบเอกสารแจกจ่าย
        private void LoadLVAcceptPublish()
        {
            try
            {
                string RequestDARID = HFRequestDARID.Value;
                sql = $@"SELECT F2_Department.DepartmentName, DC_RequestDARPublishAccept.AcceptStatus, DC_RequestDARPublishAccept.UserID
                FROM DC_RequestDARPublishAccept 
                INNER JOIN F2_Department ON DC_RequestDARPublishAccept.DepartmentID = F2_Department.DepartmentID
                WHERE RequestDARID = {RequestDARID}";
                LVAcceptPublish.DataSource = query.SelectTable(sql);
                LVAcceptPublish.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        protected void LVAcceptPublish_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListViewItemType.DataItem)
                {
                    string AcceptStatus = DataBinder.Eval(e.Item.DataItem, "AcceptStatus").ToString();
                    Label LbPublishStatus = e.Item.FindControl("LbPublishStatus") as Label;
                    if (AcceptStatus == "1")
                    {
                        string UserID = DataBinder.Eval(e.Item.DataItem, "UserID").ToString();
                        sql = $@"SELECT (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name 
                        FROM F2_Users
                        WHERE UserID = {UserID}";
                        string NameAccept = $"{query.SelectAt(0, sql)} ";
                        LbPublishStatus.Text = $"({NameAccept}รับทราบแล้ว)";
                        LbPublishStatus.CssClass = "text-success";
                    }
                    else if (AcceptStatus == "2")
                    {
                        LbPublishStatus.Text = "(รอรับทราบ)";
                        LbPublishStatus.CssClass = "text-warning";
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // ตรวจสอบสิทธิ์ Document Control
        private bool CheckPermission()
        {
            // PermissionID 3 = สิทธิ์ Document Control รายงานสถานะการดำเนินการ สามารถอัปเดตสถานะเอกสารได้(เป็นผู้แจกจ่ายเอกสารจากการร้องขอ DAR)
            // ตรวจสอบว่าผู้ใช้นี้มีสิทธิ์ใช้งานหน้า รายงานสถานะการดำเนินการ หรือไม่
            sql = $"SELECT PermissionID FROM DC_PermissionUser WHERE UserID = {Session["UserID"]} AND PermissionID = 3";
            DataTable dt = query.SelectTable(sql);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}