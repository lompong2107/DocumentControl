using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest
{
    public partial class Navbar : System.Web.UI.MasterPage
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["UserID"] = 2305;
            //Session["Name"] = "Lompong Dev";
            //Session["DepartmentID"] = 1;

            if (Session["UserID"] != null)
            {
                if (!Page.IsPostBack)
                {
                    string Name = Session["Name"].ToString();
                    LbName.Text = Name;
                }

                if (CheckPermission())
                {
                    // เช็คสิทธิ์รายการ LogBook
                    HLLogBook.Visible = true;
                    sql = $@"SELECT COUNT(RequestDARID) FROM DC_RequestDAR WHERE RequestDARStatusID = 5 GROUP BY DC_RequestDAR.RequestDARID";
                    int CountRequestDARDocPublish = int.Parse(query.SelectAt(0, sql).ToString());
                    if (CountRequestDARDocPublish > 0)
                    {
                        HLLogBook.Text = $"<span style=\"margin-right: 2px;\">รายงานสถานะการดำเนินการ</span><span class=\"rounded-circle text-center align-top text-white d-inline-block\" style=\"background-color: red; width: 20px; height: 20px; font-size: small;\">{CountRequestDARDocPublish}</span>";
                    }
                }
            }
            if (Session["UserID"] != null)
            {
                CheckNotification();
                if (Session["DepartmentID"].ToString() == "1")
                {
                    HLSetting.Visible = true;
                }
            }
            else
            {
                if (IsPostBack)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotificationAndRedirect('Session หมดเวลาแล้ว!', 'กรุณาเข้าสู่ระบบ.', 'info', '{ResolveClientUrl("~/Login.aspx")}');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotificationAndRedirect('ล้มเหลว!', 'กรุณาเข้าสู่ระบบ.', 'info', '{ResolveClientUrl("~/Login.aspx")}');", true);
                }
            }
        }


        // --------------- Button
        protected void LinkBtnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Login.aspx");
        }


        // --------------- LoadListView
        private void LoadLVNotificationRequestDAR()
        {
            string UserID = Session["UserID"].ToString();
            string DepartmentID = Session["DepartmentID"].ToString();
            sql = $@"SELECT DC_RequestDAR.RequestDARID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, REPLACE(DC_RequestDARDocType.DocTypeName, 'อื่นๆ (Other)', DC_RequestDAR.DocTypeOther) AS DocTypeName, REPLACE(DC_RequestDAROperation.OperationName, 'อื่นๆ', DC_RequestDAR.OperationOther) AS OperationName, DC_RequestDAR.RequestDARStatusID, DC_RequestDARStatus.RequestDARStatusDetail
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
                OR (DC_Approve.UserID = {UserID} AND DC_Approve.ApproveStatus = 2 AND DC_RequestDAR.RequestDARStatusID = 3 )
                OR (DC_RequestDARPublishAccept.DepartmentID = {DepartmentID} AND DC_RequestDARPublishAccept.AcceptStatus = 2 AND DC_RequestDAR.RequestDARStatusID = 6)
                OR (DC_RequestDAR.UserID = {UserID} AND DC_RequestDAR.RequestDARStatusID = 4))
                AND DC_RequestDAR.RequestDARStatusID != 0";
            LVNotificationRequestDAR.DataSource = query.SelectTable(sql);
            LVNotificationRequestDAR.DataBind();
        }
        private void LoadLVNotificationRequestSpec()
        {
            string UserID = Session["UserID"].ToString();
            sql = $@"SELECT DC_RequestSpec.RequestSpecID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, DC_RequestSpecDocType.DocTypeName, REPLACE(DC_RequestSpecOperation.OperationName, 'อื่น ๆ', DC_RequestSpec.OperationOther) AS OperationName, DC_RequestSpec.RequestSpecStatusID, DC_RequestSpecStatus.RequestSpecStatusDetail
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
            LVNotificationRequestSpec.DataSource = query.SelectTable(sql);
            LVNotificationRequestSpec.DataBind();
        }
        private void LoadLVNotificationKaizen()
        {
            string UserID = Session["UserID"].ToString();
            string DepartmentID = Session["DepartmentID"].ToString();
            sql = $@"SELECT DC_Kaizen.KaizenID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, DC_Kaizen.KaizenTopic, DC_Kaizen.KaizenStatusID, DC_KaizenStatus.KaizenStatusDetail
                FROM DC_Kaizen
                LEFT JOIN DC_KaizenStatus ON DC_Kaizen.KaizenStatusID = DC_KaizenStatus.KaizenStatusID
                LEFT JOIN DC_LeaderAccept ON DC_Kaizen.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN DC_Approve ON DC_Kaizen.ApproveID = DC_Approve.ApproveID
                LEFT JOIN F2_Users ON DC_Kaizen.UserID = F2_Users.UserID
                WHERE ((DC_LeaderAccept.UserID = {UserID} AND DC_LeaderAccept.AcceptStatus = 2 AND DC_Kaizen.KaizenStatusID = 1) 
                OR (DC_Approve.UserID = {UserID} AND DC_Approve.ApproveStatus = 2 AND DC_Kaizen.KaizenStatusID = 2)
                OR (DC_Kaizen.UserID = {UserID} AND DC_Kaizen.KaizenStatusID = 3))
                AND DC_Kaizen.KaizenStatusID != 0";
            LVNotificationKaizen.DataSource = query.SelectTable(sql);
            LVNotificationKaizen.DataBind();
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

            // ตรวจสอบว่ามีแจ้งเตือนบ้างไหม ระบบไหนก็ได้
            if (CountRequestDAR > 0 || CountKaizenReport > 0 || CountRequestSpec > 0)
            {
                PanelEmptyNotification.Visible = false;
                LbCountNotification.Visible = true;
                LbCountNotification.Text = (CountRequestDAR + CountKaizenReport + CountRequestSpec).ToString();
            }
            else
            {
                PanelEmptyNotification.Visible = true;
                LbCountNotification.Visible = false;
            }
            LoadLVNotificationRequestDAR();
            LoadLVNotificationRequestSpec();
            LoadLVNotificationKaizen();
            UpdatePanel.Update();
        }

        // ตรวจสอบสิทธิ์ Document Control ของระบบ DAR
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