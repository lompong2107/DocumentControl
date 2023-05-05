using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest.RequestSpec
{
    public partial class RequestSpecDetail : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        LineNotify LineNotify = new LineNotify();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["RequestSpecID"] != null)
            {
                string RequestSpecID = Request.QueryString["RequestSpecID"];
                Session["LastPage"] = $"~/DocumentRequest/RequestSpec/RequestSpecDetail.aspx?RequestSpecID={RequestSpecID}";
            }
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["RequestSpecID"] != null && Session["UserID"] != null)
                {
                    string RequestSpecID = Request.QueryString["RequestSpecID"];
                    HFRequestSpecID.Value = RequestSpecID;
                    LoadRequestSpecDetail(RequestSpecID);
                }
            }
        }


        // --------------- GridView
        protected void GVFiles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string Btn = e.CommandName;
                if (Btn == "BtnOpen")
                {
                    string RequestSpecDocID = e.CommandArgument.ToString();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "New_Tab", "window.open('ShowPDF.aspx?RequestSpecDocID=" + RequestSpecDocID + "', '_blank');", true);
                }
                else if (Btn == "BtnDownload")
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
                    string RequestSpecDocID = e.CommandArgument.ToString();
                    // อัปเดตเอกสารว่าแจกจ่ายแล้ว
                    sql = $"UPDATE DC_RequestSpecDoc SET RequestSpecDocStatusID = 2, UserIDUpdate = {UserID}, PublishDate = GETDATE() WHERE RequestSpecDocID = " + RequestSpecDocID;
                    if (query.Excute(sql))
                    {
                        string RequestSpecID = HFRequestSpecID.Value;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'อัปเดตสถานะแจกจ่ายสำเร็จ.', 'success');", true);
                        LoadRequestSpecDetail(RequestSpecID);
                        GVFiles.DataBind();
                        UpdatePanel.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
        protected void GVFiles_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string RequestSpecID = Request.QueryString["RequestSpecID"].ToString();
                    sql = $"SELECT RequestSpecStatusID FROM DC_RequestSpec WHERE RequestSpecID = {RequestSpecID}";
                    string RequestSpecStatusID = query.SelectAt(0, sql);
                    string RequestSpecDocStatusID = DataBinder.Eval(e.Row.DataItem, "RequestSpecDocStatusID").ToString();
                    if (RequestSpecStatusID == "4" && RequestSpecDocStatusID == "1")
                    {
                        // สถานะแจกจ่ายแล้ว
                        ImageButton ImageBtnUpdateStatus = e.Row.FindControl("ImageBtnUpdateStatus") as ImageButton;
                        ImageBtnUpdateStatus.Visible = true;
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
                string RequestSpecID = HFRequestSpecID.Value;
                sql = @"SELECT LeaderAcceptID FROM DC_RequestSpec WHERE RequestSpecID = " + RequestSpecID;
                string LeaderAcceptID = query.SelectAt(0, sql);
                // อัปเดตสถานะหัวหน้าหน่วยงานอนุมัติแล้ว
                sql = "UPDATE DC_LeaderAccept SET AcceptDate = GETDATE(), AcceptStatus = 1 WHERE LeaderAcceptID = " + LeaderAcceptID;
                query.Excute(sql);
                // อัปเดตสถานะใบ Spec เป็นรอดำเนินการตรวจสอบ/แก้ไข (NPD)
                sql = "UPDATE DC_RequestSpec SET RequestSpecStatusID = 4 WHERE RequestSpecID = " + RequestSpecID;
                query.Excute(sql);
                // แจ้งเตือน
                string Name = LbNPDName.Text;
                LineNotify.Notify("มีรายการร้องขอ\nถึงคุณ: " + Name + "\nรายละเอียด: http://110.77.148.173/DocumentControl/DocumentRequest/Approve.aspx");
                LoadRequestSpecDetail(RequestSpecID);
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
        protected void BtnDisAccept_Click(object sender, EventArgs e)
        {
            try
            {
                string Remark = TxtRemarkDisAccept.Text;
                if (string.IsNullOrEmpty(Remark))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณาระบุเหตุผล.', 'warning'); new bootstrap.Modal($(\"#DisAcceptModal\")).show();", true);
                    return;
                }
                string RequestSpecID = HFRequestSpecID.Value;
                sql = @"SELECT LeaderAcceptID FROM DC_RequestSpec WHERE RequestSpecID = " + RequestSpecID;
                string LeaderAcceptID = query.SelectAt(0, sql);
                // อัปเดตสถานะหัวหน้าหน่วยงานผู้ร้องขอ เป็น ไม่อนุมัติ
                sql = "UPDATE DC_LeaderAccept SET AcceptDate = GETDATE(), AcceptStatus = 3 WHERE LeaderAcceptID = " + LeaderAcceptID;
                query.Excute(sql);
                // อัปเดตสถานะใบ Spec เป็น ไม่อนุมัติ
                sql = "UPDATE DC_RequestSpec SET RequestSpecStatusID = 3, RemarkLeader = '" + Remark + "' WHERE RequestSpecID = " + RequestSpecID;
                query.Excute(sql);
                LoadRequestSpecDetail(RequestSpecID);
                UpdatePanelStatus.Update();
                UpdatePanel.Update();
                Navbar navbar = (Navbar)Page.Master;
                navbar.CheckNotification();
                // แจ้งเตือน
                sql = $@"SELECT (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name
                FROM DC_RequestSpec 
                INNER JOIN F2_Users ON DC_RequestSpec.UserID = F2_Users.UserID 
                INNER JOIN F2_Department ON F2_Users.DepartmentID = F2_Department.DepartmentID 
                WHERE RequestSpecID = {RequestSpecID}";
                string Name = query.SelectAt(0, sql);
                LineNotify.Notify("ถูกปฏิเสธการอนุมัติ\nอ้างอิง ECR NO: " + RequestSpecID + "\nถึงคุณ: " + Name + "\nรายละเอียด: http://110.77.148.173/DocumentControl/DocumentRequest/RequestSpec/RequestSpecHistory.aspx");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'ปฏิเสธแล้ว.', 'success');", true);
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
                string RequestSpecID = HFRequestSpecID.Value;
                sql = @"SELECT NPDAcceptID FROM DC_RequestSpec WHERE RequestSpecID = " + RequestSpecID;
                string NPDAcceptID = query.SelectAt(0, sql);
                // อัปเดตสถานะวิศวกรแจกจ่ายเอกสารแล้ว
                sql = "UPDATE DC_NPDAccept SET AcceptDate = GETDATE(), AcceptStatus = 1 WHERE NPDAcceptID = " + NPDAcceptID;
                query.Excute(sql);
                // อัปเดตสถานะใบ Spec เป็น รออนุมัติ (HOD NPD) ***ส่งให้อนุมัติก่อน***
                sql = "UPDATE DC_RequestSpec SET RequestSpecStatusID = 5 WHERE RequestSpecID = " + RequestSpecID;
                query.Excute(sql);
                LoadRequestSpecDetail(RequestSpecID);
                UpdatePanelStatus.Update();
                UpdatePanel.Update();
                Navbar navbar = (Navbar)Page.Master;
                navbar.CheckNotification();
                // แจ้งเตือน
                string Name = LbApproveName.Text;
                LineNotify.Notify("มีรายการอนุมัติ\nถึงคุณ: " + Name + "\nรายละเอียด:http://110.77.148.173/DocumentControl/DocumentRequest/Approve.aspx");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'แจกจ่ายเอกสารแล้ว.', 'success');", true);
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
                string RequestSpecID = HFRequestSpecID.Value;
                sql = @"SELECT ApproveID FROM DC_RequestSpec WHERE RequestSpecID = " + RequestSpecID;
                string ApproveID = query.SelectAt(0, sql);
                // อัปเดตสถานะหัวหน้า NPD อนุมัติแล้ว
                sql = "UPDATE DC_Approve SET ApproveDate = GETDATE(), ApproveStatus = 1 WHERE ApproveID = " + ApproveID;
                query.Excute(sql);
                // อัปเดตสถานะใบ Spec เป็น แจกจ่ายแล้ว หลังจากหัวหน้า NPD อนุมัติ
                sql = "UPDATE DC_RequestSpec SET RequestSpecStatusID = 7 WHERE RequestSpecID = " + RequestSpecID;
                query.Excute(sql);
                // เพิ่มผู้รับทราบ คือหน่วยหน้าหน่วยงานผู้ร้องขอ
                sql = $@"SELECT DC_LeaderAccept.UserID 
                FROM DC_RequestSpec
                INNER JOIN DC_LeaderAccept ON DC_RequestSpec.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                WHERE DC_RequestSpec.RequestSpecID = {RequestSpecID}";
                string UserIDPublishAccept = query.SelectAt(0, sql);
                sql = $@"INSERT INTO DC_RequestSpecPublishAccept (RequestSpecID, UserID) VALUES ({RequestSpecID}, {UserIDPublishAccept})";
                query.Excute(sql);
                LoadRequestSpecDetail(RequestSpecID);
                UpdatePanelStatus.Update();
                UpdatePanel.Update();
                Navbar navbar = (Navbar)Page.Master;
                navbar.CheckNotification();
                // แจ้งเตือน
                string Name = LbLeaderName.Text;
                LineNotify.NotifyDocumentControl($"แจกจ่ายเอกสารแล้ว\nอ้างอิง ECR NO: {RequestSpecID}\nถึงคุณ: {Name}\nรายละเอียด: http://110.77.148.173/DocumentControl/DocumentRequest/RequestSpec/RequestSpecDetail.aspx?RequestSpecID={RequestSpecID}");
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
                string Remark = TxtRemarkDisApprove.Text;
                if (string.IsNullOrEmpty(Remark))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณาระบุเหตุผล.', 'warning'); new bootstrap.Modal($(\"#DisApproveModal\")).show();", true);
                    return;
                }
                string RequestSpecID = HFRequestSpecID.Value;
                sql = @"SELECT ApproveID FROM DC_RequestSpec WHERE RequestSpecID = " + RequestSpecID;
                string ApproveID = query.SelectAt(0, sql);
                // อัปเดตสถานะหัวหน้าแผนก NPD ไม่อนุมัติ
                sql = "UPDATE DC_Approve SET ApproveDate = GETDATE(), ApproveStatus = 3 WHERE ApproveID = " + ApproveID;
                query.Excute(sql);
                // อัปเดตสถานะใบ Spec เป็น ไม่อนุมัติ
                sql = "UPDATE DC_RequestSpec SET RequestSpecStatusID = 6, Remark = '" + Remark + "' WHERE RequestSpecID = " + RequestSpecID;
                query.Excute(sql);
                LoadRequestSpecDetail(RequestSpecID);
                UpdatePanelStatus.Update();
                UpdatePanel.Update();
                Navbar navbar = (Navbar)Page.Master;
                navbar.CheckNotification();
                // แจ้งเตือน
                sql = $@"SELECT (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name
                FROM DC_RequestSpec 
                INNER JOIN F2_Users ON DC_RequestSpec.UserID = F2_Users.UserID 
                INNER JOIN F2_Department ON F2_Users.DepartmentID = F2_Department.DepartmentID 
                WHERE RequestSpecID = {RequestSpecID}";
                string Name = query.SelectAt(0, sql);
                LineNotify.Notify("ถูกปฏิเสธการอนุมัติ\nอ้างอิง ECR NO: " + RequestSpecID + "\nถึงคุณ: " + Name + "\nรายละเอียด: http://110.77.148.173/DocumentControl/DocumentRequest/RequestSpec/RequestSpecHistory.aspx");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'ปฏิเสธแล้ว.', 'success');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
        // รับทราบแจกจ่ายเอกสาร
        protected void BtnPublishAccept_Click(object sender, EventArgs e)
        {
            try
            {
                string RequestSpecID = HFRequestSpecID.Value;
                string UserID = Session["UserID"].ToString();
                // อัปเดตสถานะผู้รับทราบเอกสารแจกจ่าย เป็น รับทราบแล้ว
                sql = $@"UPDATE DC_RequestSpecPublishAccept SET AcceptDate = GETDATE(), AcceptStatus = 1 WHERE RequestSpecID = {RequestSpecID} AND UserID = {UserID}";
                if (query.Excute(sql))
                {
                    // อัปเดตสถานะใบ Spec เป็น เสร็จสมบูรณ์
                    sql = "UPDATE DC_RequestSpec SET RequestSpecStatusID = 8 WHERE RequestSpecID = " + RequestSpecID;
                    query.Excute(sql);
                    LoadRequestSpecDetail(RequestSpecID);
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


        // --------------- Function โหลดข้อมูลเอกสาร Spec
        private void LoadRequestSpecDetail(string RequestSpecID)
        {
            try
            {
                sql = $@"SELECT DC_RequestSpec.RequestSpecID, DC_RequestSpec.DateRequest, DC_RequestSpecDocType.DocTypeName, DC_RequestSpecOperation.OperationName, DC_RequestSpec.OperationOther, DC_RequestSpecTypeOfChange.TypeOfChangeDetail, Project.ProjectName, Part.PartName, FG.FGName, DC_RequestSpec.DetailRequest, DC_RequestSpec.ReasonRequest
                , DC_LeaderAccept.UserID AS LeaderUserID, DC_LeaderAccept.AcceptStatus AS LeaderAcceptStatus, DC_NPDAccept.UserID AS NPDUserID, DC_NPDAccept.AcceptStatus AS NPDAcceptStatus,  DC_Approve.UserID AS ApproveUserID, DC_Approve.ApproveStatus, DC_RequestSpec.RequestSpecStatusID, DC_RequestSpecStatus.RequestSpecStatusDetail
                , (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, (LeaderUser.FirstNameTH + ' ' + LeaderUser.LastNameTH) AS LeaderName, (NPDUser.FirstNameTH + ' ' + NPDUser.LastNameTH) AS NPDName, (ApproveUser.FirstNameTH + ' ' + ApproveUser.LastNameTH) AS ApproveName, (PublishUser.FirstNameTH + ' ' + PublishUser.LastNameTH) AS PublishName
                , DC_RequestSpec.RemarkLeader, DC_RequestSpec.RemarkApprove, DC_RequestSpec.RemarkCancel, DC_RequestSpecPublishAccept.AcceptStatus AS PublishAcceptStatus, DC_RequestSpecPublishAccept.UserID AS PublishUserID
                FROM DC_RequestSpec
                LEFT JOIN DC_RequestSpecStatus ON DC_RequestSpec.RequestSpecStatusID =  DC_RequestSpecStatus.RequestSpecStatusID
                LEFT JOIN DC_RequestSpecDocType ON DC_RequestSpec.RequestSpecDocTypeID = DC_RequestSpecDocType.RequestSpecDocTypeID
                LEFT JOIN DC_RequestSpecOperation ON DC_RequestSpec.RequestSpecOperationID = DC_RequestSpecOperation.RequestSpecOperationID
                LEFT JOIN DC_RequestSpecTypeOfChange ON DC_RequestSpec.RequestSpecTypeOfChangeID = DC_RequestSpecTypeOfChange.RequestSpecTypeOfChangeID
                LEFT JOIN [TCTFactory].[dbo].[FG] AS FG ON DC_RequestSpec.FGID = FG.FGID
                LEFT JOIN [TCTFactory].[dbo].[Part] AS Part ON FG.PartID = Part.PartID
                LEFT JOIN [TCTFactory].[dbo].[Project] AS Project ON Part.ProjectID = Project.ProjectID
                LEFT JOIN F2_Users ON DC_RequestSpec.UserID = F2_Users.UserID
                LEFT JOIN DC_LeaderAccept ON DC_RequestSpec.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN F2_Users AS LeaderUser ON DC_LeaderAccept.UserID = LeaderUser.UserID
                LEFT JOIN DC_NPDAccept ON DC_RequestSpec.NPDAcceptID = DC_NPDAccept.NPDAcceptID
                LEFT JOIN F2_Users AS NPDUser ON DC_NPDAccept.UserID = NPDUser.UserID
                LEFT JOIN DC_Approve ON DC_RequestSpec.ApproveID = DC_Approve.ApproveID
                LEFT JOIN F2_Users AS ApproveUser ON DC_Approve.UserID = ApproveUser.UserID
                LEFT JOIN DC_RequestSpecPublishAccept ON DC_RequestSpec.RequestSpecID = DC_RequestSpecPublishAccept.RequestSpecID AND DC_RequestSpecPublishAccept.UserID = LeaderUser.UserID
                LEFT JOIN F2_Users AS PublishUser ON DC_RequestSpecPublishAccept.UserID = PublishUser.UserID
                WHERE DC_RequestSpec.RequestSpecID = " + RequestSpecID;
                string DateRequest = query.SelectAt(1, sql);
                string DocTypeName = query.SelectAt(2, sql);
                string OperationName = query.SelectAt(3, sql);
                string OperationOther = query.SelectAt(4, sql);
                string TypeOfChange = query.SelectAt(5, sql);
                string ProjectName = query.SelectAt(6, sql);
                string PartName = query.SelectAt(7, sql);
                string FGName = query.SelectAt(8, sql);
                string DetailRequest = query.SelectAt(9, sql);
                string ReasonRequest = query.SelectAt(10, sql);
                // เช็คสถานะ
                string UserID = Session["UserID"].ToString();
                string LeaderUserID = query.SelectAt(11, sql);
                string LeaderAcceptStatus = query.SelectAt(12, sql);
                string NPDUserID = query.SelectAt(13, sql);
                string NPDAcceptStatus = query.SelectAt(14, sql);
                string ApproveUserID = query.SelectAt(15, sql);
                string ApproveStatus = query.SelectAt(16, sql);
                string RequestSpecStatusID = query.SelectAt(17, sql);
                string RequestSpecStatusDetail = query.SelectAt(18, sql);
                // ชื่อ
                string NameRequest = query.SelectAt(19, sql);
                string NameLeader = query.SelectAt(20, sql);
                string NameNPD = query.SelectAt(21, sql);
                string NameApprove = query.SelectAt(22, sql);
                string NamePublish = query.SelectAt(23, sql);
                // เหตุผลการไม่อนุมัติ
                string RemarkAccept = query.SelectAt(24, sql);
                string RemarkApprove = query.SelectAt(25, sql);
                // เหตุผลการยกเลิก
                string RemarkCancel = query.SelectAt(26, sql);
                // เช็คสถานะการรับทราบเอกสารแจกจ่ายของแต่ละแผนก
                string PublishAcceptStatus = query.SelectAt(27, sql);
                string PublishUserID = query.SelectAt(28, sql);

                // Set Value
                LbDateRequest.Text = DateTime.Parse(DateRequest).ToString("dd/MM/yyyy");
                LbRequestSpecID.Text = int.Parse(RequestSpecID).ToString("D3");
                LbRequestSpecStatus.Text = RequestSpecStatusDetail + RemarkCancel;
                LbDocType.Text = DocTypeName;
                LbOperation.Text = OperationName;
                LbOperationOther.Text = OperationOther;
                LbTypeOfChange.Text = TypeOfChange;
                LbProjectName.Text = ProjectName;
                LbPartName.Text = PartName;
                LbFG.Text = FGName;
                LbDetailRequest.Text = DetailRequest;
                LbReasonRequest.Text = ReasonRequest;
                LbUserRequest.Text = NameRequest;
                LbLeaderName.Text = NameLeader;
                LbNPDName.Text = NameNPD;
                LbApproveName.Text = NameApprove;

                // การแสดงสถานะของผู้ตรวจสอบ รับทราบ อนุมัติ
                // หัวหน้าหน่วยงานผู้ร้องขออนุมัติ
                if (LeaderAcceptStatus == "2")
                {
                    LbLeaderStatus.Text = "(รออนุมัติ)";
                    LbLeaderStatus.CssClass = "text-warning";
                }
                else if (LeaderAcceptStatus == "1")
                {
                    LbLeaderStatus.Text = "(อนุมัติแล้ว)";
                    LbLeaderStatus.CssClass = "text-success";
                }
                else if (LeaderAcceptStatus == "3")
                {
                    LbLeaderStatus.Text = "(ไม่อนุมัติ " + RemarkAccept + ")";
                    LbLeaderStatus.CssClass = "text-danger";
                }
                // วิศวกร
                if (NPDAcceptStatus == "2")
                {
                    LbNPDStatus.Text = "(รอดำเนินการตรวจสอบ/แก้ไข)";
                    LbNPDStatus.CssClass = "text-warning";
                }
                else if (NPDAcceptStatus == "1")
                {
                    LbNPDStatus.Text = "(แจกจ่ายแล้ว)";
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
                    LbApproveStatus.Text = "(ไม่อนุมัติ " + RemarkApprove + ")";
                    LbApproveStatus.CssClass = "text-danger";
                }

                // แสดง ซ่อน ปุ่มการรับทราบ และอนุมัติ
                // การร้องขอ รออนุมัติ (HOD) และ รออนุมัติ และเป็นผู้อนุมัติ
                if (RequestSpecStatusID == "2" && LeaderAcceptStatus == "2" && LeaderUserID == UserID)
                {
                    PanelLeaderAccept.Visible = true;
                }
                // ไม่อนุมัติหรืออนุมัติแล้ว(รอดำเนินการตรวจสอบ/แก้ไข (NPD)) และเป็นผู้อนุมัติ
                if ((RequestSpecStatusID == "3" || RequestSpecStatusID == "4") && (LeaderAcceptStatus != "2") && (LeaderUserID == UserID))
                {
                    PanelLeaderAccept.Visible = false;
                }

                // การร้องขอ รอดำเนินการตรวจสอบ/แก้ไข (NPD) และ รอแจกจ่าย และเป็นผู้แจกจ่าย(วิศวกร) และ เอกสารแจกจ่ายหมดแล้วจริงๆ
                if (RequestSpecStatusID == "4" && NPDAcceptStatus == "2" && NPDUserID == UserID && CheckSpecDocPublish(RequestSpecID))
                {
                    PanelNPDAccept.Visible = true;
                }
                // แจกจ่ายแล้ว และเป็นผู้แจกจ่าย(วิศวกร) และ เอกสารแจกจ่ายหมดแล้วจริงๆ
                if (NPDAcceptStatus == "1" && NPDUserID == UserID && !CheckSpecDocPublish(RequestSpecID))
                {
                    PanelNPDAccept.Visible = false;
                }
                // ตรวจสอบว่าเป็น NPD วิศวกร จะเป็นผู้อัปเดตสถานะแจกจ่ายเอกสารได้
                if (NPDUserID == UserID && !CheckSpecDocPublish(RequestSpecID) && RequestSpecStatusID == "4")
                {
                    GVFiles.Columns[3].Visible = true;
                }
                else
                {
                    GVFiles.Columns[3].Visible = false;
                }

                // การร้องขอ รออนุมัติ และ รออนุมัติ และเป็นผู้อนุมัติ
                if (RequestSpecStatusID == "5" && ApproveStatus == "2" && ApproveUserID == UserID)
                {
                    PanelApprove.Visible = true;
                }
                // การร้องขอ ไม่อนุมัติหรืออนุมัติแล้ว(แจกจ่ายแล้ว) และสถานะไม่รออนุมัติ และเป็นผู้อนุมัติ
                if ((RequestSpecStatusID == "6" || RequestSpecStatusID == "7") && (ApproveStatus != "2") && (ApproveUserID == UserID))
                {
                    PanelApprove.Visible = false;
                }

                // การร้องขอ แจกจ่ายแล้ว และหัวหน้าหน่วยงานผู้ร้องขอยังไม่กดรับทราบ และเป็นผู้รับทราบ
                if (RequestSpecStatusID == "7" && PublishAcceptStatus == "2" && PublishUserID == UserID)
                {
                    PanelPublichAccept.Visible = true;
                }
                // การร้องขอ เสร็จแล้ว และหัวหน้าหน่วยงานผู้ร้องขอกดรับทราบแล้ว และเป็นผู้รับทราบ
                else if (RequestSpecStatusID == "8" && PublishAcceptStatus == "1" && PublishUserID == UserID)
                {
                    PanelPublichAccept.Visible = false;
                }

                // สถานะ
                // การร้องขอ ยกเลิก
                if (RequestSpecStatusID == "0")
                {
                    LbRequestSpecStatus.CssClass = "text-secondary";
                }
                // การร้องขอ รออนุมัติ (HOD), รอดำเนินการตรวจสอบ/แก้ไข (NPD), รออนุมัติ (HOD NPD)
                if (RequestSpecStatusID == "2" || RequestSpecStatusID == "4" || RequestSpecStatusID == "5")
                {
                    LbRequestSpecStatus.CssClass = "text-warning";
                }
                // การร้องขอ ไม่อนุมัติ
                if (RequestSpecStatusID == "3" || RequestSpecStatusID == "6")
                {
                    LbRequestSpecStatus.CssClass = "text-danger";
                    // เช็คว่าเป็นผู้ร้องขอ นี้หรือไม่
                    sql = $"SELECT RequestSpecID FROM DC_RequestSpec WHERE UserID = {UserID} AND RequestSpecID = {RequestSpecID}";
                    if (query.CheckRow(sql))
                    {
                        PanelEdit.Visible = true;
                    }
                }
                // การร้องขอ แจกจ่ายแล้ว
                if (RequestSpecStatusID == "7")
                {
                    LbRequestSpecStatus.CssClass = "text-info";
                }
                // การร้องขอ เสร็จแล้ว
                if (RequestSpecStatusID == "8")
                {
                    PanelPublichAccept.Visible = false;
                    LbRequestSpecStatus.CssClass = "text-success";
                }

                // ส่วนผู้รับทราบเอกสารแจกจ่าย
                if (RequestSpecStatusID == "7" || RequestSpecStatusID == "8")
                {
                    PanelListPublishAccept.Visible = true;
                    LbPublishName.Text = NamePublish;
                    if (PublishAcceptStatus == "1")
                    {
                        LbPublishStatus.Text = "(รับทราบแล้ว)";
                        LbPublishStatus.CssClass = "text-success";
                    }
                    else if (PublishAcceptStatus == "2")
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


        // --------------- Function
        // ตรวจสอบสถานะเอกสารแจกจ่าย สำหรับ NPD ที่ต้องแจกจ่าย
        private bool CheckSpecDocPublish(string RequestSpecID)
        {
            // ตรวจสอบว่ามีเอกสารที่ อยู่ระหว่างการดำเนินการแก้ไข
            sql = $@"SELECT RequestSpecDocID FROM DC_RequestSpecDoc WHERE RequestSpecID = {RequestSpecID} AND RequestSpecDocStatusID = 1";
            if (query.CheckRow(sql))
            {
                return false;
            }
            return true;
        }
    }
}