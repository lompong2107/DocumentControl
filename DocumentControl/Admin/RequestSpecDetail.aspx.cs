using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Admin
{
    public partial class RequestSpecDetail : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["RequestSpecID"] != null && Session["UserID"] != null)
                {
                    string RequestSpecID = Request.QueryString["RequestSpecID"];
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
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "New_Tab", "window.open('RequestSpecShowPDF.aspx?RequestSpecDocID=" + RequestSpecDocID + "', '_blank');", true);
                }
                else if (Btn == "BtnDownload")
                {
                    string FilePath = e.CommandArgument.ToString();
                    Response.ContentType = ContentType;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(FilePath));
                    Response.WriteFile(FilePath);
                    Response.End();
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
                , DC_LeaderAccept.AcceptStatus AS LeaderAcceptStatus, DC_NPDAccept.AcceptStatus AS NPDAcceptStatus, DC_Approve.ApproveStatus, DC_RequestSpec.RequestSpecStatusID, DC_RequestSpecStatus.RequestSpecStatusDetail
                , (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, (LeaderUser.FirstNameTH + ' ' + LeaderUser.LastNameTH) AS LeaderName, (NPDUser.FirstNameTH + ' ' + NPDUser.LastNameTH) AS NPDName, (ApproveUser.FirstNameTH + ' ' + ApproveUser.LastNameTH) AS ApproveName, (PublishUser.FirstNameTH + ' ' + PublishUser.LastNameTH) AS PublishName
                , DC_RequestSpec.RemarkLeader, DC_RequestSpec.RemarkApprove, DC_RequestSpec.RemarkCancel, DC_RequestSpecPublishAccept.AcceptStatus AS PublishAcceptStatus
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
                string LeaderAcceptStatus = query.SelectAt(11, sql);
                string NPDAcceptStatus = query.SelectAt(12, sql);
                string ApproveStatus = query.SelectAt(13, sql);
                string RequestSpecStatusID = query.SelectAt(14, sql);
                string RequestSpecStatusDetail = query.SelectAt(15, sql);
                // ชื่อ
                string NameRequest = query.SelectAt(16, sql);
                string NameLeader = query.SelectAt(17, sql);
                string NameNPD = query.SelectAt(18, sql);
                string NameApprove = query.SelectAt(19, sql);
                string NamePublish = query.SelectAt(20, sql);
                // เหตุผลการไม่อนุมัติ
                string RemarkAccept = query.SelectAt(21, sql);
                string RemarkApprove = query.SelectAt(22, sql);
                // เหตุผลการยกเลิก
                string RemarkCancel = query.SelectAt(23, sql);
                // เช็คสถานะการรับทราบเอกสารแจกจ่ายของแต่ละแผนก
                string PublishAcceptStatus = query.SelectAt(24, sql);

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
                }
                // การร้องขอ แจกจ่ายแล้ว
                if (RequestSpecStatusID == "7")
                {
                    LbRequestSpecStatus.CssClass = "text-info";
                }
                // การร้องขอ เสร็จแล้ว
                if (RequestSpecStatusID == "8")
                {
                    LbRequestSpecStatus.CssClass = "text-success";
                }

                // ส่วนผู้รับทราบเอกสารแจกจ่าย
                if (RequestSpecStatusID == "7" || RequestSpecStatusID == "8")
                {
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
    }
}