using DocumentControl.DocumentRequest.RequestDAR;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Admin
{
    public partial class RequestDARDetail : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
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
        private void LoadGVRequestDARDoc(string RequestDARID)
        {
            try
            {
                sql = @"SELECT DC_RequestDARDoc.RequestDARDocID, DC_RequestDARDoc.DocNumber, DC_RequestDARDoc.DocName, DC_RequestDARDoc.DateEnforce, DC_RequestDARDoc.Remark, DC_RequestDARDoc.FilePath, DC_RequestDARDoc.RequestDARDocStatusID, DC_RequestDARDocStatus.DocStatusDetail
                FROM DC_RequestDARDoc 
                LEFT JOIN DC_RequestDARDocStatus ON DC_RequestDARDoc.RequestDARDocStatusID = DC_RequestDARDocStatus.RequestDARDocStatusID
                WHERE DC_RequestDARDoc.RequestDARID = " + RequestDARID;
                GVRequestDARDoc.DataSource = query.SelectTable(sql);
                GVRequestDARDoc.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
        protected void GVRequestDARDoc_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string Btn = e.CommandName;
            if (Btn == "BtnDownload")
            {
                string FilePath = e.CommandArgument.ToString();
                Response.ContentType = ContentType;
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(FilePath));
                Response.WriteFile(FilePath);
                Response.End();
            }
        }
        protected void GVRequestDARDoc_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
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
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-warning";
                }
                else if (StatusID == "2")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-success";
                }
            }
        }


        // --------------- Function
        private void LoadRequestDARDetail(string RequestDARID)
        {
            try
            {
                sql = $@"SELECT DC_RequestDAR.RequestDARID, DC_RequestDAR.DateRequest, DC_RequestDARDocType.DocTypeName, DC_RequestDAR.DocTypeOther, DC_RequestDAROperation.OperationName, DC_RequestDAR.OperationOther
                , DC_LeaderAccept.AcceptStatus AS LeaderAcceptStatus, DC_NPDAccept.AcceptStatus AS NPDAcceptStatus, DC_Approve.ApproveStatus, DC_RequestDAR.RequestDARStatusID, DC_RequestDARStatus.RequestDARStatusDetail
                , (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, (LeaderUser.FirstNameTH + ' ' + LeaderUser.LastNameTH) AS LeaderName, (NPDUser.FirstNameTH + ' ' + NPDUser.LastNameTH) AS NPDName, (ApproveUser.FirstNameTH + ' ' + ApproveUser.LastNameTH) AS ApproveName
                , DC_RequestDAR.Remark
                FROM DC_RequestDAR
                LEFT JOIN DC_RequestDARStatus ON DC_RequestDAR.RequestDARStatusID =  DC_RequestDARStatus.RequestDARStatusID
                LEFT JOIN DC_RequestDAROperation ON DC_RequestDAR.RequestDAROperationID = DC_RequestDAROperation.RequestDAROperationID
                LEFT JOIN DC_RequestDARDocType ON DC_RequestDAR.RequestDARDocTypeID = DC_RequestDARDocType.RequestDARDocTypeID
                LEFT JOIN F2_Users ON DC_RequestDAR.UserID = F2_Users.UserID
                LEFT JOIN DC_LeaderAccept ON DC_RequestDAR.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN F2_Users LeaderUser ON DC_LeaderAccept.UserID = LeaderUser.UserID
                LEFT JOIN DC_NPDAccept ON DC_RequestDAR.NPDAcceptID = DC_NPDAccept.NPDAcceptID
                LEFT JOIN F2_Users NPDUser ON DC_NPDAccept.UserID = NPDUser.UserID
                LEFT JOIN DC_Approve ON DC_RequestDAR.ApproveID = DC_Approve.ApproveID
                LEFT JOIN F2_Users ApproveUser ON DC_Approve.UserID = ApproveUser.UserID
                WHERE DC_RequestDAR.RequestDARID = " + RequestDARID;
                string DateRequest = query.SelectAt(1, sql);
                string DocTypeName = query.SelectAt(2, sql);
                string DocTypeOther = query.SelectAt(3, sql);
                string OperationName = query.SelectAt(4, sql);
                string OperationOther = query.SelectAt(5, sql);
                // เช็คสถานะ
                string LeaderAcceptStatus = query.SelectAt(6, sql);
                string NPDAcceptStatus = query.SelectAt(7, sql);
                string ApproveStatus = query.SelectAt(8, sql);
                string RequestDARStatusID = query.SelectAt(9, sql);
                string RequestDARStatusDetail = query.SelectAt(10, sql);
                // ชื่อ
                string Name = query.SelectAt(11, sql);
                string LeaderName = query.SelectAt(12, sql);
                string NPDName = query.SelectAt(13, sql);
                string ApproveName = query.SelectAt(14, sql);
                // เหตุผลการไม่อนุมัติ
                string Remark = query.SelectAt(15, sql);

                LbDateRequest.Text = DateTime.Parse(DateRequest).ToString("dd/MM/yyyy");
                LbRequestDARID.Text = int.Parse(RequestDARID).ToString("D3");
                LbRequestDARStatus.Text = RequestDARStatusDetail;
                LbDocType.Text = DocTypeName;
                LbDocTypeOther.Text = DocTypeOther;
                LbOperation.Text = OperationName;
                LbOperationOther.Text = OperationOther;
                LbUserRequest.Text = Name;
                LbLeaderName.Text = LeaderName;
                LbNPDName.Text = NPDName;
                LbApproveName.Text = ApproveName;

                if (OperationName == "จัดทำขึ้นใหม่" || OperationName == "ยกเลิกการใช้")
                {
                    PanelAcceptNPD.Visible = true;
                }

                LoadGVRequestDARDoc(RequestDARID);

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

                LoadLVAcceptPublish();

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
                }
                // การร้องขอ แจกจ่ายแล้ว
                if (RequestDARStatusID == "6")
                {
                    LbRequestDARStatus.CssClass = "text-info";
                }
                // การร้องขอ เสร็จแล้ว
                if (RequestDARStatusID == "7")
                {
                    LbRequestDARStatus.CssClass = "text-success";
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }

        // โหลดข้อมูลสถานะผู้รับทราบเอกสารแจกจ่าย
        private void LoadLVAcceptPublish()
        {
            try
            {
                string RequestDARID = HFRequestDARID.Value;
                sql = $@"SELECT  F2_Department.DepartmentName, DC_RequestDARPublishAccept.AcceptStatus, DC_RequestDARPublishAccept.UserID
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
    }
}