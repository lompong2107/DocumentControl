using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest
{
    public partial class Approve : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LastPage"] = "~/DocumentRequest/Approve.aspx";
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    BindGrid();
                    CheckNotification();
                }
            }
        }


        // --------------- GridView
        private void BindGrid()
        {
            string UserID = Session["UserID"].ToString();
            string DepartmentID = Session["DepartmentID"].ToString();
            // Request DAR
            sql = $@"SELECT DC_RequestDAR.RequestDARID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, REPLACE(DC_RequestDARDocType.DocTypeName, 'อื่นๆ (Other)', DC_RequestDAR.DocTypeOther) AS DocTypeName, REPLACE(DC_RequestDAROperation.OperationName, 'อื่นๆ', DC_RequestDAR.OperationOther) AS OperationName, DC_RequestDAR.DateRequest,  DC_RequestDARStatus.RequestDARStatusDetail, DC_RequestDAR.RequestDARStatusID, DC_RequestDAR.Remark
                FROM DC_RequestDAR
                LEFT JOIN DC_RequestDARStatus ON DC_RequestDAR.RequestDARStatusID = DC_RequestDARStatus.RequestDARStatusID
                LEFT JOIN DC_LeaderAccept ON DC_RequestDAR.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN DC_NPDAccept ON DC_RequestDAR.NPDAcceptID = DC_NPDAccept.NPDAcceptID
                LEFT JOIN DC_Approve ON DC_RequestDAR.ApproveID = DC_Approve.ApproveID
                LEFT JOIN DC_RequestDARDocType ON DC_RequestDAR.RequestDARDocTypeID = DC_RequestDARDocType.RequestDARDocTypeID
                LEFT JOIN DC_RequestDAROperation ON DC_RequestDAR.RequestDAROperationID = DC_RequestDAROperation.RequestDAROperationID
                LEFT JOIN DC_RequestDARPublishAccept ON DC_RequestDAR.RequestDARID = DC_RequestDARPublishAccept.RequestDARID
                LEFT JOIN F2_Users ON DC_RequestDAR.UserID = F2_Users.UserID
                WHERE ((DC_LeaderAccept.UserID = {UserID} AND DC_LeaderAccept.AcceptStatus = 2 AND DC_RequestDAR.RequestDARStatusID = 2) 
                OR (DC_RequestDAR.NPDAcceptID != 0 AND DC_RequestDAR.RequestDARStatusID = 2 AND DC_LeaderAccept.AcceptStatus = 1 AND DC_NPDAccept.UserID = {UserID} AND DC_NPDAccept.AcceptStatus = 2)
                OR (DC_Approve.UserID = {UserID} AND DC_Approve.ApproveStatus = 2 AND DC_RequestDAR.RequestDARStatusID = 3)
                OR (DC_RequestDARPublishAccept.DepartmentID = {DepartmentID} AND DC_RequestDARPublishAccept.AcceptStatus = 2 AND DC_RequestDAR.RequestDARStatusID = 6)
                OR (DC_RequestDAR.UserID = {UserID} AND DC_RequestDAR.RequestDARStatusID = 4))
                AND DC_RequestDAR.RequestDARStatusID != 0";
            GVRequestDAR.DataSource = query.SelectTable(sql);
            GVRequestDAR.DataBind();
            GVRequestDAR.HeaderRow.TableSection = TableRowSection.TableHeader;

            // Request Spec
            sql = $@"SELECT DC_RequestSpec.RequestSpecID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, DC_RequestSpecDocType.DocTypeName, REPLACE(DC_RequestSpecOperation.OperationName, 'อื่น ๆ', DC_RequestSpec.OperationOther) AS OperationName, DC_RequestSpec.DateRequest,  DC_RequestSpecStatus.RequestSpecStatusDetail, DC_RequestSpec.RequestSpecStatusID, DC_RequestSpec.RemarkLeader, DC_RequestSpec.RemarkApprove, DC_RequestSpec.RemarkCancel
                FROM DC_RequestSpec
                LEFT JOIN DC_RequestSpecStatus ON DC_RequestSpec.RequestSpecStatusID = DC_RequestSpecStatus.RequestSpecStatusID
                LEFT JOIN DC_LeaderAccept ON DC_RequestSpec.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN DC_NPDAccept ON DC_RequestSpec.NPDAcceptID = DC_NPDAccept.NPDAcceptID
                LEFT JOIN DC_Approve ON DC_RequestSpec.ApproveID = DC_Approve.ApproveID
                LEFT JOIN DC_RequestSpecDocType ON DC_RequestSpec.RequestSpecDocTypeID = DC_RequestSpecDocType.RequestSpecDocTypeID
                LEFT JOIN DC_RequestSpecOperation ON DC_RequestSpec.RequestSpecOperationID = DC_RequestSpecOperation.RequestSpecOperationID
                LEFT JOIN DC_RequestSpecPublishAccept ON DC_RequestSpec.RequestSpecID = DC_RequestSpecPublishAccept.RequestSpecID
                LEFT JOIN F2_Users ON DC_RequestSpec.UserID = F2_Users.UserID
                WHERE ((DC_LeaderAccept.UserID = {UserID} AND DC_LeaderAccept.AcceptStatus = 2 AND DC_RequestSpec.RequestSpecStatusID = 2) 
                OR (DC_NPDAccept.UserID = {UserID} AND DC_NPDAccept.AcceptStatus = 2 AND DC_RequestSpec.RequestSpecStatusID = 4)
                OR (DC_Approve.UserID = {UserID} AND DC_Approve.ApproveStatus = 2 AND DC_RequestSpec.RequestSpecStatusID = 5)
                OR (DC_RequestSpecPublishAccept.UserID = {UserID} AND DC_RequestSpecPublishAccept.AcceptStatus = 2 AND DC_RequestSpec.RequestSpecStatusID = 7)
                OR (DC_RequestSpec.UserID = {UserID} AND (DC_RequestSpec.RequestSpecStatusID = 3 OR DC_RequestSpec.RequestSpecStatusID = 6)))
                AND DC_RequestSpec.RequestSpecStatusID != 0";
            GVRequestSpec.DataSource = query.SelectTable(sql);
            GVRequestSpec.DataBind();
            GVRequestSpec.HeaderRow.TableSection = TableRowSection.TableHeader;

            // Kaizen Report
            sql = $@"SELECT DC_Kaizen.KaizenID, DC_Kaizen.KaizenTopic, DC_Kaizen.DateCreate, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, F2_Department.DepartmentName, DC_KaizenStatus.KaizenStatusDetail, DC_Kaizen.KaizenStatusID, DC_Kaizen.Remark
                FROM DC_Kaizen
                LEFT JOIN DC_KaizenStatus ON DC_Kaizen.KaizenStatusID = DC_KaizenStatus.KaizenStatusID
                LEFT JOIN DC_LeaderAccept ON DC_Kaizen.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN DC_Approve ON DC_Kaizen.ApproveID = DC_Approve.ApproveID
                LEFT JOIN F2_Users ON DC_Kaizen.UserID = F2_Users.UserID
                LEFT JOIN F2_Department ON DC_Kaizen.DepartmentID = F2_Department.DepartmentID
                WHERE ((DC_LeaderAccept.UserID = {UserID} AND DC_LeaderAccept.AcceptStatus = 2 AND DC_Kaizen.KaizenStatusID = 1) 
                OR (DC_Approve.UserID = {UserID} AND DC_Approve.ApproveStatus = 2 AND DC_Kaizen.KaizenStatusID = 2)
                OR (DC_Kaizen.UserID = {UserID} AND DC_Kaizen.KaizenStatusID = 3))
                AND DC_Kaizen.KaizenStatusID != 0";
            GVKaizen.DataSource = query.SelectTable(sql);
            GVKaizen.DataBind();
            GVKaizen.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        // Request DAR
        protected void GVRequestDAR_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GVRequestDAR, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["style"] = "cursor:pointer";

                string StatusID = DataBinder.Eval(e.Row.DataItem, "RequestDARStatusID").ToString();
                Panel PanelStatus = e.Row.FindControl("PanelStatus") as Panel;
                if (StatusID == "4")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-danger";
                }
                else if (StatusID == "6")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-info";
                }
                else
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-warning";
                }
            }
        }
        protected void GVRequestDAR_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow SelectedRow = GVRequestDAR.SelectedRow;
            string RequestDARID = (SelectedRow.Cells[0].FindControl("HFRequestDARID") as HiddenField).Value;
            Response.Redirect("~/DocumentRequest/RequestDAR/RequestDARDetail.aspx?RequestDARID=" + RequestDARID);
        }

        // Request Spec
        protected void GVRequestSpec_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GVRequestSpec, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["style"] = "cursor:pointer";

                string StatusID = DataBinder.Eval(e.Row.DataItem, "RequestSpecStatusID").ToString();
                Panel PanelStatus = e.Row.FindControl("PanelStatus") as Panel;
                if (StatusID == "3" || StatusID == "6")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-danger";
                }
                else if (StatusID == "7")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-info";
                }
                else
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-warning";
                }
            }
        }
        protected void GVRequestSpec_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow SelectedRow = GVRequestSpec.SelectedRow;
            string RequestSpecID = (SelectedRow.Cells[0].FindControl("HFRequestSpecID") as HiddenField).Value;
            Response.Redirect("~/DocumentRequest/RequestSpec/RequestSpecDetail.aspx?RequestSpecID=" + RequestSpecID);
        }

        // Kaizen Report
        protected void GVKaizen_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GVKaizen, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["style"] = "cursor:pointer";

                string StatusID = DataBinder.Eval(e.Row.DataItem, "KaizenStatusID").ToString();
                Panel PanelStatus = e.Row.FindControl("PanelStatus") as Panel;
                if (StatusID == "3")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-danger";
                }
                else
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-warning";
                }
            }
        }
        protected void GVKaizen_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow SelectedRow = GVKaizen.SelectedRow;
            string KaizenID = (SelectedRow.Cells[0].FindControl("HFKaizenID") as HiddenField).Value;
            Response.Redirect("~/DocumentRequest/KaizenReport/KaizenDetail.aspx?KaizenID=" + KaizenID);
        }

        // --------------- Function
        public void CheckNotification()
        {
            string UserID = Session["UserID"].ToString();
            string DepartmentID = Session["DepartmentID"].ToString();
            // การแจ้งเตือน Request DAR
            sql = $@"SELECT COUNT(DC_RequestDAR.RequestDARID)
                FROM DC_RequestDAR
                LEFT JOIN DC_LeaderAccept ON DC_RequestDAR.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN DC_NPDAccept ON DC_RequestDAR.NPDAcceptID = DC_NPDAccept.NPDAcceptID
                LEFT JOIN DC_Approve ON DC_RequestDAR.ApproveID = DC_Approve.ApproveID
                LEFT JOIN DC_RequestDARPublishAccept ON DC_RequestDAR.RequestDARID = DC_RequestDARPublishAccept.RequestDARID
                WHERE ((DC_LeaderAccept.UserID = {UserID} AND DC_LeaderAccept.AcceptStatus = 2 AND DC_RequestDAR.RequestDARStatusID = 2) 
                OR (DC_RequestDAR.NPDAcceptID != 0 AND DC_RequestDAR.RequestDARStatusID = 2 AND DC_LeaderAccept.AcceptStatus = 1 AND DC_NPDAccept.UserID = {UserID} AND DC_NPDAccept.AcceptStatus = 2)
                OR (DC_Approve.UserID = {UserID} AND DC_Approve.ApproveStatus = 2 AND DC_RequestDAR.RequestDARStatusID = 3)
                OR (DC_RequestDARPublishAccept.DepartmentID = {DepartmentID} AND DC_RequestDARPublishAccept.AcceptStatus = 2 AND DC_RequestDAR.RequestDARStatusID = 6)
                OR (DC_RequestDAR.UserID = {UserID} AND DC_RequestDAR.RequestDARStatusID = 4)) 
                AND DC_RequestDAR.RequestDARStatusID != 0";
            int CountRequestDAR = int.Parse(query.SelectAt(0, sql).ToString());

            // การแจ้งเตือน Request Spec
            sql = $@"SELECT COUNT(DC_RequestSpec.RequestSpecID)
                FROM DC_RequestSpec
                LEFT JOIN DC_LeaderAccept ON DC_RequestSpec.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN DC_NPDAccept ON DC_RequestSpec.NPDAcceptID = DC_NPDAccept.NPDAcceptID
                LEFT JOIN DC_Approve ON DC_RequestSpec.ApproveID = DC_Approve.ApproveID
                LEFT JOIN DC_RequestSpecPublishAccept ON DC_RequestSpec.RequestSpecID = DC_RequestSpecPublishAccept.RequestSpecID
                WHERE ((DC_LeaderAccept.UserID = {UserID} AND DC_LeaderAccept.AcceptStatus = 2 AND DC_RequestSpec.RequestSpecStatusID = 2) 
                OR (DC_NPDAccept.UserID = {UserID} AND DC_NPDAccept.AcceptStatus = 2 AND DC_RequestSpec.RequestSpecStatusID = 4)
                OR (DC_Approve.UserID = {UserID} AND DC_Approve.ApproveStatus = 2 AND DC_RequestSpec.RequestSpecStatusID = 5)
                OR (DC_RequestSpecPublishAccept.UserID = {UserID} AND DC_RequestSpecPublishAccept.AcceptStatus = 2 AND DC_RequestSpec.RequestSpecStatusID = 7)
                OR (DC_RequestSpec.UserID = {UserID} AND (DC_RequestSpec.RequestSpecStatusID = 3 OR DC_RequestSpec.RequestSpecStatusID = 6)))
                AND DC_RequestSpec.RequestSpecStatusID != 0";
            int CountRequestSpec = int.Parse(query.SelectAt(0, sql).ToString());

            // การแจ้งเตือน Kaizen Report
            sql = $@"SELECT COUNT(DC_Kaizen.KaizenID)
                FROM DC_Kaizen
                LEFT JOIN DC_LeaderAccept ON DC_Kaizen.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN DC_Approve ON DC_Kaizen.ApproveID = DC_Approve.ApproveID
                WHERE ((DC_LeaderAccept.UserID = {UserID} AND DC_LeaderAccept.AcceptStatus = 2 AND DC_Kaizen.KaizenStatusID = 1)
                OR (DC_Approve.UserID = {UserID} AND DC_Approve.ApproveStatus = 2 AND DC_Kaizen.KaizenStatusID = 2)
                OR (DC_Kaizen.UserID = {UserID} AND DC_Kaizen.KaizenStatusID = 3))
                AND DC_Kaizen.KaizenStatusID != 0";
            int CountKaizenReport = int.Parse(query.SelectAt(0, sql).ToString());
            if (CountRequestDAR > 0 || CountKaizenReport > 0 || CountRequestSpec > 0)
            {
                PanelEmptyNotification.Visible = false;
                // Request DAR
                if (CountRequestDAR > 0)
                {
                    PanelRequestDAR.Visible = true;
                    LbCountApproveRequestDAR.Visible = true;
                    LbCountApproveRequestDAR.Text = CountRequestDAR.ToString();
                }
                else
                {
                    PanelRequestDAR.Visible = false;
                    LbCountApproveRequestDAR.Visible = false;
                }

                // Request Spec
                if (CountRequestSpec > 0)
                {
                    PanelRequestSpec.Visible = true;
                    LbCountApproveRequestSpec.Visible = true;
                    LbCountApproveRequestSpec.Text = CountRequestSpec.ToString();
                }
                else
                {
                    PanelRequestSpec.Visible = false;
                    LbCountApproveRequestSpec.Visible = false;
                }

                // Kaizen Report
                if (CountKaizenReport > 0)
                {
                    PanelKaizen.Visible = true;
                    LbCountApproveKaizen.Visible = true;
                    LbCountApproveKaizen.Text = CountKaizenReport.ToString();
                }
                else
                {
                    PanelKaizen.Visible = false;
                    LbCountApproveKaizen.Visible = false;
                }
                LbCountApproveAll.Visible = true;
                LbCountApproveAll.Text = (CountRequestDAR + CountKaizenReport + CountRequestSpec).ToString();
            }
            else
            {
                PanelEmptyNotification.Visible = true;
                LbCountApproveAll.Visible = false;
                LbCountApproveRequestDAR.Visible = false;
                LbCountApproveRequestSpec.Visible = false;
                LbCountApproveKaizen.Visible = false;
                PanelRequestDAR.Visible = false;
                PanelRequestSpec.Visible = false;
                PanelKaizen.Visible = false;
            }
        }
    }
}