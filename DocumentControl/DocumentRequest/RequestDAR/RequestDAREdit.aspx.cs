using DocumentControl.Admin;
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
    public partial class RequestDAREdit : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        LineNotify LineNotify = new LineNotify();
        StringSpecialClass StringSpecial = new StringSpecialClass();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["RequestDARID"] != null && Session["UserID"] != null)
                {
                    string RequestDARID = Request.QueryString["RequestDARID"];
                    HFRequestDARID.Value = RequestDARID;

                    // Load Data
                    LoadDDListDetailRequest();
                    LoadUserRequest(RequestDARID);
                    LoadDDListAcceptLeader(RequestDARID);
                    LoadDDListAcceptNPD();
                    LoadDDListApprove();
                    CreateDatatable();
                    LoadRequestDARDetail(RequestDARID);
                    DDListRequestDARDocType_SelectedIndexChanged(null, null);
                    DDListRequestDAROperation_SelectedIndexChanged(null, null);
                }
            }
        }


        // --------------- Function
        private void CreateDatatable()
        {
            DataTable dtRequestDARDoc = new DataTable();
            dtRequestDARDoc.Columns.AddRange(new DataColumn[6] {
                    new DataColumn("DocNumber"), new DataColumn("DocName"), new DataColumn("DateEnforce"), new DataColumn("Remark"), new DataColumn("FilePath"), new DataColumn("RequestDARDocID")
                });
            Session["GVRequestDARDoc"] = dtRequestDARDoc;
            BindGrid();
        }
        private void LoadDDListDetailRequest()
        {
            sql = "SELECT RequestDARDocTypeID, DocTypeName FROM DC_RequestDARDocType WHERE Status = 1";
            DDListRequestDARDocType.DataSource = query.SelectTable(sql);
            DDListRequestDARDocType.DataBind();

            sql = "SELECT RequestDAROperationID, OperationName FROM DC_RequestDAROperation WHERE Status = 1";
            DDListRequestDAROperation.DataSource = query.SelectTable(sql);
            DDListRequestDAROperation.DataBind();
        }
        private void LoadUserRequest(string RequestDARID)
        {
            sql = $@"SELECT (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) As Name
                FROM DC_RequestDAR
                INNER JOIN F2_Users ON DC_RequestDAR.UserID = F2_Users.UserID
                WHERE RequestDARID = {RequestDARID}";
            TxtUserRequest.Text = query.SelectAt(0, sql);
        }
        private void LoadDDListAcceptLeader(string RequestDARID)
        {
            //sql = "SELECT PoPermissID, PositionID, PermissID, PositionsName, PositionsDetail FROM HR_Permiss_Pos INNER JOIN F2_Positions ON F2_Positions.PositionsID = HR_Permiss_Pos.PositionID WHERE PermissID = '1' And Status = '1';";
            //DataTable dt = query.SelectTable(sql);
            //string FilterStr = string.Empty;
            //foreach (DataRow row in dt.Rows)
            //{
            //    FilterStr += $" OR F2_Users.PositionsID = '{row["PositionID"]}'";
            //}
            //sql = $@"SELECT F2_Users.UserID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTh) As Name 
            //    FROM F2_Users 
            //    INNER JOIN F2_Positions ON F2_Users.PositionsID = F2_Positions.PositionsID 
            //    WHERE F2_Users.UserID IN (SELECT F2_UserOrganization.UserID FROM F2_UserOrganization WHERE DepartmentID = '{DepartmentID}' AND F2_Users.Status = '1' AND (F2_Users.PositionsID = '0' {FilterStr})) ORDER BY Name;";

            string DepartmentID = Session["DepartmentID"].ToString();
            sql = $"SELECT F2_Users.UserID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) As Name FROM F2_UserOrganization LEFT JOIN F2_Users ON F2_UserOrganization.UserID = F2_Users.UserID WHERE F2_UserOrganization.DepartmentID = {DepartmentID} AND F2_Users.Status = 1";
            DDListAcceptLeader.DataSource = query.SelectTable(sql);
            DDListAcceptLeader.DataBind();
        }
        private void LoadDDListAcceptNPD()
        {
            // แผนก NPD
            //sql = "SELECT UserID, (FirstNameTH + ' ' + LastNameTH) As Name FROM F2_Users WHERE UserID IN (50, 441) AND Status = 1";
            sql = "SELECT F2_Users.UserID, (FirstNameTH + ' ' + LastNameTH + ' ' + ISNULL(DC_UserApprove.Remark, '')) As Name FROM DC_UserApprove LEFT JOIN F2_Users ON DC_UserApprove.UserID = F2_Users.UserID WHERE DC_UserApprove.StatusPermission = 1 AND DC_UserApprove.Status = 1";
            DDListAcceptNPD.DataSource = query.SelectTable(sql);
            DDListAcceptNPD.DataBind();
        }
        private void LoadDDListApprove()
        {
            sql = "SELECT F2_Users.UserID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH + ' ' + ISNULL(DC_UserApprove.Remark, '')) As Name FROM DC_UserApprove LEFT JOIN F2_Users ON DC_UserApprove.UserID = F2_Users.UserID WHERE DC_UserApprove.StatusPermission = 2 AND DC_UserApprove.Status = 1";
            DDListApprove.DataSource = query.SelectTable(sql);
            DDListApprove.DataBind();
        }
        private void LoadRequestDARDetail(string RequestDARID)
        {
            sql = $@"SELECT DC_RequestDAR.RequestDARID, DC_RequestDAR.DateRequest, DC_RequestDARDocType.RequestDARDocTypeID, DC_RequestDAR.DocTypeOther, DC_RequestDAROperation.RequestDAROperationID, DC_RequestDAR.OperationOther, DC_LeaderAccept.UserID As LeaderUserID, DC_NPDAccept.UserID, DC_Approve.UserID
                FROM DC_RequestDAR
                LEFT JOIN DC_RequestDAROperation ON DC_RequestDAR.RequestDAROperationID = DC_RequestDAROperation.RequestDAROperationID
                LEFT JOIN DC_RequestDARDocType ON DC_RequestDAR.RequestDARDocTypeID = DC_RequestDARDocType.RequestDARDocTypeID
                LEFT JOIN DC_LeaderAccept ON DC_RequestDAR.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN DC_NPDAccept ON DC_RequestDAR.NPDAcceptID = DC_NPDAccept.NPDAcceptID
                LEFT JOIN DC_Approve ON DC_RequestDAR.ApproveID = DC_Approve.ApproveID
                LEFT JOIN F2_Users ON DC_RequestDAR.UserID = F2_Users.UserID
                WHERE DC_RequestDAR.RequestDARID = {RequestDARID}";
            string DateRequest = query.SelectAt(1, sql);
            string RequestDARDocTypeID = query.SelectAt(2, sql);
            string DocTypeOther = query.SelectAt(3, sql);
            string RequestDAROperationID = query.SelectAt(4, sql);
            string OperationOther = query.SelectAt(5, sql);
            string LeaderUserID = query.SelectAt(6, sql);
            string NPDUserID = query.SelectAt(7, sql);
            string ApproveUserID = query.SelectAt(8, sql);

            LbDateRequest.Text = DateTime.Parse(DateRequest).ToString("dd/MM/yyyy");
            LbRequestDARID.Text = int.Parse(RequestDARID).ToString("D3");
            DDListRequestDARDocType.SelectedValue = RequestDARDocTypeID;
            TxtDocTypeOther.Text = DocTypeOther;
            DDListRequestDAROperation.SelectedValue = RequestDAROperationID;
            TxtOperationOther.Text = OperationOther;
            DDListAcceptLeader.SelectedValue = LeaderUserID;
            DDListAcceptNPD.SelectedValue = NPDUserID;
            DDListApprove.SelectedValue = ApproveUserID;

            sql = $@"SELECT DocNumber, DocName, CONVERT(nvarchar, DateEnforce, 103) AS DateEnforce, Remark, FilePath, RequestDARDocID
                FROM DC_RequestDARDoc WHERE RequestDARID = {RequestDARID} AND RequestDARDocStatusID != 0";
            Session["GVRequestDARDoc"] = query.SelectTable(sql);
            BindGrid();
        }


        // --------------- DropDownList
        protected void DDListRequestDARDocType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DDListRequestDARDocType.SelectedItem.Value == "9")
            {
                TxtDocTypeOther.Enabled = true;
                RequiredFieldValidatorDocTypeOther.Enabled = true;
            }
            else
            {
                TxtDocTypeOther.Enabled = false;
                RequiredFieldValidatorDocTypeOther.Enabled = false;
            }
        }
        protected void DDListRequestDAROperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DDListRequestDAROperation.SelectedItem.Text == "จัดทำขึ้นใหม่" || DDListRequestDAROperation.SelectedItem.Text == "ยกเลิกการใช้")
            {
                PanelAcceptNPD.Visible = true;
            }
            else
            {
                PanelAcceptNPD.Visible = false;
            }

            if (DDListRequestDAROperation.SelectedItem.Text == "อื่นๆ")
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


        // --------------- Button
        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string RequestDARID = Request.QueryString["RequestDARID"];
                sql = $@"SELECT UserID, LeaderAcceptID, NPDAcceptID, ApproveID
                    FROM DC_RequestDAR 
                    WHERE RequestDARID = {RequestDARID}";
                string UserIDCreate = query.SelectAt(0, sql);   // คนสร้างใบร้องขอ
                string LeaderAcceptID = query.SelectAt(1, sql);
                string NPDAcceptID = query.SelectAt(2, sql);
                string ApproveID = query.SelectAt(3, sql);

                string RequestDARDocTypeID = DDListRequestDARDocType.SelectedValue;
                string DocTypeOther = TxtDocTypeOther.Text;
                string RequestDAROperationID = DDListRequestDAROperation.SelectedValue;
                string OperationOther = TxtOperationOther.Text;
                string UserID = Session["UserID"].ToString();   // คนอัปเดตใบร้องขอ
                string AcceptLeader = DDListAcceptLeader.SelectedValue;
                string Approve = DDListApprove.SelectedValue;

                // ---------- Start ตรวจสอบการกรอกข้อมูล ----------
                // ชนิดของเอกสาร
                if (DDListRequestDARDocType.SelectedItem.Value == "9" && string.IsNullOrEmpty(DocTypeOther))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณาระบุชนิดเอกสาร.', 'warning');", true);
                    return;
                }
                // การดำเนินการ
                if (DDListRequestDAROperation.SelectedItem.Text == "อื่นๆ" && string.IsNullOrEmpty(OperationOther))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณาระบุการดำเนินการ.', 'warning');", true);
                    return;
                }
                // รายการเอกสาร
                if (GVRequestDARDoc.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'ไม่มีรายการเอกสาร.', 'warning');", true);
                    return;
                }
                foreach (GridViewRow row in GVRequestDARDoc.Rows)
                {
                    string DocNumber = (row.Cells[0].FindControl("TxtDocNumber") as TextBox).Text;
                    string DocName = (row.Cells[1].FindControl("TxtDocName") as TextBox).Text;
                    string DocDate = (row.Cells[2].FindControl("TxtDocDate") as TextBox).Text;
                    string DocRemark = (row.Cells[3].FindControl("TxtDocRemark") as TextBox).Text;
                    FileUpload FileUpload = row.Cells[4].FindControl("FileUploadFile") as FileUpload;
                    ImageButton ImageBtnDownload = row.Cells[4].FindControl("ImageBtnDownload") as ImageButton;
                    if (string.IsNullOrEmpty(DocNumber) || string.IsNullOrEmpty(DocName) || string.IsNullOrEmpty(DocDate) || string.IsNullOrEmpty(DocRemark) || (FileUpload.HasFile == false && ImageBtnDownload.Visible == false))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณากรอกข้อมูลเอกสารให้ครบ.', 'warning');", true);
                        return;
                    }
                }
                // ---------- END ตรวจสอบการกรอกข้อมูล ----------

                // เก็บสถานะตรวจสอบล่าสุดก่อน ไว้เช็คแจ้งเตือน
                sql = $"SELECT AcceptStatus FROM DC_LeaderAccept WHERE LeaderAcceptID = {LeaderAcceptID}";
                int LeaderAcceptStatus = int.Parse(query.SelectAt(0, sql));
                // อัปเดตหัวหน้าแผนกตรวจสอบและรับทราบ
                sql = $"UPDATE DC_LeaderAccept SET UserID = {AcceptLeader}, AcceptDate = NULL, AcceptStatus = 2 WHERE LeaderAcceptID = {LeaderAcceptID}";
                query.Excute(sql);
                // อัปเดตผู้จัดการโรงงานอนุมัติ
                sql = $"UPDATE DC_Approve SET UserID = {Approve}, ApproveDate = NULL, ApproveStatus = 2 WHERE ApproveID = {ApproveID}";
                query.Excute(sql);
                // ถ้าเป็นการสร้างใหม่หรือยกเลิก จะเพิ่มการตรวจสอบที่ NPD ด้วย
                if (RequestDAROperationID == "2" || RequestDAROperationID == "3")
                {
                    string AcceptNPD = DDListAcceptNPD.SelectedValue;
                    // เช็คว่าเคยมี NPD หรือยัง ถ้ายังให้เพิ่มใหม่
                    if (NPDAcceptID == "0")
                    {
                        sql = $"INSERT INTO DC_NPDAccept (UserID, AcceptStatus) VALUES ({AcceptNPD}, 2)";
                        query.Excute(sql);
                        // ดึงข้อมูลที่เพิ่มล่าสุด
                        sql = $"SELECT TOP 1 NPDAcceptID FROM DC_NPDAccept WHERE UserID = {AcceptNPD} AND AcceptStatus = 2 ORDER BY NPDAcceptID DESC";
                        NPDAcceptID = query.SelectAt(0, sql);
                    }
                    else
                    {
                        // ถ้ามี NPD แล้ว ให้อัปเดต
                        sql = $"UPDATE DC_NPDAccept SET UserID = {AcceptNPD}, AcceptDate = NULL, AcceptStatus = 2 WHERE NPDAcceptID = {NPDAcceptID}";
                        query.Excute(sql);
                    }
                }
                else
                {
                    // ถ้าไม่เป็นการสร้างใหม่หรือยกเลิก และถ้ามี NPD อยู่ให้ลบอันเก่าออกก่อน
                    if (NPDAcceptID != "0")
                    {
                        sql = $"DELETE DC_NPDAccept WHERE NPDAcceptID = {NPDAcceptID}";
                        query.Excute(sql);
                    }
                }

                // อัปเดตใบร้องขอ DAR
                sql = $@"UPDATE DC_RequestDAR SET UserIDUpdate = {UserID}, DateUpdateRequest = GETDATE(), RemarkCancel = NULL, RequestDARDocTypeID = {RequestDARDocTypeID}, DocTypeOther = '{DocTypeOther}', RequestDAROperationID = {RequestDAROperationID}, OperationOther = '{OperationOther}', NPDAcceptID = '{NPDAcceptID}', Remark = NULL, RequestDARStatusID = 2 WHERE RequestDARID = {RequestDARID}";
                query.Excute(sql);

                // เพิ่มไฟล์
                string FilePath = $"\\\\192.168.0.100\\PDoc\\RequestDAR\\{UserIDCreate}\\{RequestDARID}";
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                int CountFile = 1;
                string DateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HHmmss");
                foreach (GridViewRow row in GVRequestDARDoc.Rows)
                {
                    string FilePathSub = $"{FilePath}\\{DateTimeNow} {CountFile}";
                    if (!Directory.Exists(FilePathSub))
                    {
                        Directory.CreateDirectory(FilePathSub);
                    }
                    string RequestDARDocID = (row.Cells[0].FindControl("HFRequestDARDocID") as HiddenField).Value;
                    string DocNumber = (row.Cells[0].FindControl("TxtDocNumber") as TextBox).Text;
                    string DocName = (row.Cells[1].FindControl("TxtDocName") as TextBox).Text.Replace("'", "''");
                    string DocDate = DateTime.ParseExact((row.Cells[2].FindControl("TxtDocDate") as TextBox).Text, "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                    string DocRemark = (row.Cells[3].FindControl("TxtDocRemark") as TextBox).Text.Replace("'", "''");
                    FileUpload FileUpload = row.Cells[4].FindControl("FileUploadFile") as FileUpload;

                    string FileName = Path.GetFileNameWithoutExtension(FileUpload.FileName);
                    string Extension = Path.GetExtension(FileUpload.FileName);
                    string FullFileName = StringSpecial.StringSpecial(FileName) + Extension;
                    string FullFilePath = Path.Combine(FilePathSub, FullFileName);
                    sql = $"SELECT RequestDARDocID, FilePath FROM DC_RequestDARDoc WHERE RequestDARDocID = '{RequestDARDocID}'";
                    string FilePathOld = query.SelectAt(1, sql);
                    FullFilePath = FullFilePath.Replace("'", "''");
                    // เช็คว่ามีเอกสารนี้หรือยัง ถ้ามีแล้วให้อัปเดต
                    if (query.CheckRow(sql))
                    {
                        if (FileUpload.HasFile)
                        {
                            File.Delete(FilePathOld);   // ลบไฟล์เก่าออก
                            FileUpload.SaveAs(FullFilePath);    // บันทึกไฟล์ใหม่
                            sql = $"UPDATE DC_RequestDARDoc SET DocNumber = '{DocNumber}', DocName = '{DocName}', DateEnforce = '{DocDate}', Remark = '" + DocRemark + "', FilePath = '" + FullFilePath + "', RequestDARDocStatusID = 1, PublishDate = NULL WHERE RequestDARDocID = " + RequestDARDocID;
                            query.Excute(sql);
                        }
                        else
                        {
                            sql = $"UPDATE DC_RequestDARDoc SET DocNumber = '{DocNumber}', DocName = '{DocName}', DateEnforce = '{DocDate}', Remark = '" + DocRemark + "', RequestDARDocStatusID = 1, PublishDate = NULL WHERE RequestDARDocID = " + RequestDARDocID;
                            query.Excute(sql);
                        }
                    }
                    // ถ้ายังไม่มีให้เพิ่มใหม่
                    else
                    {
                        if (FileUpload.HasFile)
                        {
                            FileUpload.SaveAs(FullFilePath);    // บันทึกไฟล์ใหม่
                        }
                        sql = $@"INSERT INTO DC_RequestDARDoc (RequestDARID, DocNumber, DocName, DateEnforce, Remark, FilePath, RequestDARDocStatusID)
                            VALUES ({RequestDARID}, '{DocNumber}', '{DocName}', '{DocDate}', '{DocRemark}', '{FullFilePath}', 1)";
                        query.Excute(sql);
                    }
                    CountFile++;
                }
                // แจ้งเตือน
                // ตรวจสอบก่อนว่าผู้ตรวจสอบตรวจสอบหรือยัง ถ้าตรวจสอบแล้วให้แจ้งเตือนใหม่ ถ้ายังไม่ตรวจสอบ ไม่ต้องส่งแจ้งเตือนไปใหม่
                if (LeaderAcceptStatus == 1)
                {
                    string Name = DDListAcceptLeader.SelectedItem.Text;
                    LineNotify.Notify("มีรายการอนุมัติ\nถึงคุณ: " + Name + "\nรายละเอียด: http://110.77.148.173/DocumentControl/DocumentRequest/Approve.aspx");
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotificationAndRedirect('สำเร็จ', 'บันทึกข้อมูลสำเร็จ.', 'success', '{ResolveClientUrl("~/DocumentRequest/RequestDAR/RequestDARHistory.aspx")}');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }


        // ---------------- GridView
        private void BindGrid()
        {
            GVRequestDARDoc.DataSource = (DataTable)Session["GVRequestDARDoc"];
            GVRequestDARDoc.DataBind();
        }
        protected void GVRequestDARDoc_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string Btn = e.CommandName;
            if (Btn == "DeleteRow")
            {
                // ตรวจสอบว่าไฟล์ที่ลบออกเป็นที่มีอยู่ในระบบฐานข้อมูลหรือไม่ ถ้าไม่มีค่าจะเป็น 0 หรือว่างนี่แหละ
                string RequestDARDocID = e.CommandArgument.ToString();
                if (RequestDARDocID != "0" && !string.IsNullOrEmpty(RequestDARDocID))
                {
                    sql = "UPDATE DC_RequestDARDoc SET RequestDARDocStatusID = 0 WHERE RequestDARDocID = " + RequestDARDocID;
                    query.Excute(sql);
                    GridViewRow gvr = (GridViewRow)((ImageButton)e.CommandSource).NamingContainer;
                    int RowIndex = gvr.RowIndex;
                    DataTable dtRequestDARDoc = (DataTable)Session["GVRequestDARDoc"];
                    DataRow dr = dtRequestDARDoc.Rows[RowIndex];
                    dtRequestDARDoc.Rows.Remove(dr);
                    Session["GVRequestDARDoc"] = dtRequestDARDoc;
                    BindGrid();
                }
                else
                {
                    GridViewRow gvr = (GridViewRow)((ImageButton)e.CommandSource).NamingContainer;
                    int RowIndex = gvr.RowIndex;
                    DataTable dtRequestDARDoc = (DataTable)Session["GVRequestDARDoc"];
                    DataRow dr = dtRequestDARDoc.Rows[RowIndex];
                    dtRequestDARDoc.Rows.Remove(dr);
                    Session["GVRequestDARDoc"] = dtRequestDARDoc;
                    BindGrid();
                }
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
        protected void LinkBtnAddRequestDARDoc_Click(object sender, EventArgs e)
        {
            DataTable dtDoc = (DataTable)Session["GVRequestDARDoc"];
            dtDoc.Columns["DateEnforce"].ReadOnly = false;
            for (int RowIndex = 0; RowIndex < GVRequestDARDoc.Rows.Count; RowIndex++)
            {
                // ตรวจสอบว่าไฟล์ที่ลบออกเป็นที่มีอยู่ในระบบฐานข้อมูลหรือไม่ ถ้าไม่มีค่าจะเป็น 0 หรือว่างนี่แหละ
                string RequestDARDocID = (GVRequestDARDoc.Rows[RowIndex].Cells[0].FindControl("HFRequestDARDocID") as HiddenField).Value;
                if (RequestDARDocID == "0")
                {
                    dtDoc.Rows[RowIndex][0] = (GVRequestDARDoc.Rows[RowIndex].Cells[0].FindControl("TxtDocNumber") as TextBox).Text;
                    dtDoc.Rows[RowIndex][1] = (GVRequestDARDoc.Rows[RowIndex].Cells[1].FindControl("TxtDocName") as TextBox).Text;
                    dtDoc.Rows[RowIndex][2] = (GVRequestDARDoc.Rows[RowIndex].Cells[2].FindControl("TxtDocDate") as TextBox).Text;
                    dtDoc.Rows[RowIndex][3] = (GVRequestDARDoc.Rows[RowIndex].Cells[3].FindControl("TxtDocRemark") as TextBox).Text;
                }
            }
            dtDoc.Rows.Add(string.Empty, string.Empty, DateTime.Now.ToString("dd/MM/yyyy"), string.Empty, string.Empty, 0);
            Session["GVRequestDARDoc"] = dtDoc;
            BindGrid();
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
            }
        }
    }
}