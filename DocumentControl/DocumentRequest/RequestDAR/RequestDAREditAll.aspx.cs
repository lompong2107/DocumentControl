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
    public partial class RequestDAREditAll : System.Web.UI.Page
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
                    if (CheckPermissionEdit())
                    {
                        string RequestDARID = Request.QueryString["RequestDARID"];
                        HFRequestDARID.Value = RequestDARID;

                        // Load Data
                        LoadDDListDetailRequest();
                        LoadUserRequest(RequestDARID);
                        LoadDDListAcceptLeader(RequestDARID);
                        LoadDDListAcceptNPD();
                        LoadDDListApprove(RequestDARID);
                        CreateDatatable();
                        LoadRequestDARDetail(RequestDARID);
                        DDListRequestDARDocType_SelectedIndexChanged(null, null);
                        DDListRequestDAROperation_SelectedIndexChanged(null, null);
                    }
                    else
                    {
                        Response.Redirect("~/Default.aspx");
                    }
                }
            }
        }

        // --------------- Function

        private bool CheckPermissionEdit()
        {
            // ตรวจสอบสิทธิ์การแก้ไขคำร้องขอแก้ไขเอกสาร DAR ของคนอื่น
            string UserID = Session["UserID"].ToString();
            sql = $"SELECT UserID FROM DC_PermissionUser WHERE PermissionID = 4 AND UserID = {UserID}";
            if (query.CheckRow(sql))
            {
                return true;
            }
            return false;
        }
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
            sql = $@"SELECT (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) As Name
                FROM DC_RequestDAR
                INNER JOIN DC_LeaderAccept ON DC_RequestDAR.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                INNER JOIN F2_Users ON DC_LeaderAccept.UserID = F2_Users.UserID
                WHERE RequestDARID = {RequestDARID}";
            TxtAcceptLeader.Text = query.SelectAt(0, sql);
        }
        private void LoadDDListAcceptNPD()
        {
            // แผนก NPD
            sql = "SELECT F2_Users.UserID, (FirstNameTH + ' ' + LastNameTH + ' ' + ISNULL(DC_UserApprove.Remark, '')) As Name FROM DC_UserApprove LEFT JOIN F2_Users ON DC_UserApprove.UserID = F2_Users.UserID WHERE DC_UserApprove.StatusPermission = 1 AND DC_UserApprove.Status = 1";
            DDListAcceptNPD.DataSource = query.SelectTable(sql);
            DDListAcceptNPD.DataBind();
        }
        private void LoadDDListApprove(string RequestDARID)
        {
            sql = $@"SELECT (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) As Name
                FROM DC_RequestDAR
                INNER JOIN DC_Approve ON DC_RequestDAR.ApproveID = DC_Approve.ApproveID
                INNER JOIN F2_Users ON DC_Approve.UserID = F2_Users.UserID
                WHERE RequestDARID = {RequestDARID}";
            TxtApprove.Text = query.SelectAt(0, sql);
        }
        private void LoadRequestDARDetail(string RequestDARID)
        {
            sql = $@"SELECT DC_RequestDAR.RequestDARID, DC_RequestDAR.DateRequest, DC_RequestDARDocType.RequestDARDocTypeID, DC_RequestDAR.DocTypeOther, DC_RequestDAROperation.RequestDAROperationID, DC_RequestDAR.OperationOther, DC_NPDAccept.UserID
                FROM DC_RequestDAR
                LEFT JOIN DC_RequestDAROperation ON DC_RequestDAR.RequestDAROperationID = DC_RequestDAROperation.RequestDAROperationID
                LEFT JOIN DC_RequestDARDocType ON DC_RequestDAR.RequestDARDocTypeID = DC_RequestDARDocType.RequestDARDocTypeID
                LEFT JOIN DC_NPDAccept ON DC_RequestDAR.NPDAcceptID = DC_NPDAccept.NPDAcceptID
                LEFT JOIN F2_Users ON DC_RequestDAR.UserID = F2_Users.UserID
                WHERE DC_RequestDAR.RequestDARID = {RequestDARID}";
            string DateRequest = query.SelectAt(1, sql);
            string RequestDARDocTypeID = query.SelectAt(2, sql);
            string DocTypeOther = query.SelectAt(3, sql);
            string RequestDAROperationID = query.SelectAt(4, sql);
            string OperationOther = query.SelectAt(5, sql);
            string NPDUserID = query.SelectAt(6, sql);

            LbDateRequest.Text = DateTime.Parse(DateRequest).ToString("dd/MM/yyyy");
            LbRequestDARID.Text = int.Parse(RequestDARID).ToString("D3");
            DDListRequestDARDocType.SelectedValue = RequestDARDocTypeID;
            TxtDocTypeOther.Text = DocTypeOther;
            DDListRequestDAROperation.SelectedValue = RequestDAROperationID;
            TxtOperationOther.Text = OperationOther;
            DDListAcceptNPD.SelectedValue = NPDUserID;

            sql = $@"SELECT DocNumber, DocName, DateEnforce, Remark, FilePath, RequestDARDocID
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
                sql = $@"SELECT UserID, NPDAcceptID FROM DC_RequestDAR WHERE RequestDARID = {RequestDARID}";
                string UserIDCreate = query.SelectAt(0, sql);   // คนสร้างใบร้องขอ
                string NPDAcceptID = query.SelectAt(1, sql);

                string RequestDARDocTypeID = DDListRequestDARDocType.SelectedValue;
                string DocTypeOther = TxtDocTypeOther.Text;
                string RequestDAROperationID = DDListRequestDAROperation.SelectedValue;
                string OperationOther = TxtOperationOther.Text;
                string UserID = Session["UserID"].ToString();   // คนอัปเดตใบร้องขอ

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
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณากรอกข้อมูลให้ครบ.', 'warning');", true);
                        return;
                    }
                }
                // ---------- END ตรวจสอบการกรอกข้อมูล ----------

                // ถ้าเป็นการสร้างใหม่หรือยกเลิก จะเพิ่มการตรวจสอบที่ NPD ด้วย
                if (RequestDAROperationID == "2" || RequestDAROperationID == "3")
                {
                    string AcceptNPD = DDListAcceptNPD.SelectedValue;
                    // เช็คว่าเคยมีผู้รับทราบหรือยัง ถ้ายังให้เพิ่มใหม่
                    if (NPDAcceptID == "0")
                    {
                        sql = $"INSERT INTO DC_NPDAccept (UserID, AcceptStatus) VALUES ({AcceptNPD}, 2)";
                        query.Excute(sql);  // เพิ่มผู้รับทราบ
                        // ดึงข้อมูลที่เพิ่มล่าสุด
                        sql = $"SELECT TOP 1 NPDAcceptID FROM DC_NPDAccept WHERE UserID = {AcceptNPD} AND AcceptStatus = 2 ORDER BY NPDAcceptID DESC";
                        NPDAcceptID = query.SelectAt(0, sql);   // ดึงรหัสผู้รับทราบล่าสุด
                        // ตรวจสอบสถานะคำร้อง และสถานะการตรวจสอบ
                        sql = $@"SELECT DC_RequestDAR.RequestDARStatusID, DC_RequestDAR.ApproveID, DC_LeaderAccept.AcceptStatus 
                            FROM DC_RequestDAR
                            INNER JOIN DC_LeaderAccept ON DC_RequestDAR.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                            WHERE DC_RequestDAR.RequestDARID = {RequestDARID}";
                        int RequestDARStatusID = int.Parse(query.SelectAt(0, sql));
                        int ApproveID = int.Parse(query.SelectAt(1, sql));
                        int LeaderAcceptStatus = int.Parse(query.SelectAt(2, sql));
                        if ((new[] { 2, 3, 4, 5, 6 }).Contains(RequestDARStatusID))
                        {
                            // อัปเดตผู้จัดการโรงงานอนุมัติ
                            sql = $"UPDATE DC_Approve SET ApproveDate = NULL, ApproveStatus = 2 WHERE ApproveID = {ApproveID}";
                            query.Excute(sql);
                            // อัปเดตสถานะใบร้องขอ DAR
                            sql = $@"UPDATE DC_RequestDAR SET RequestDARStatusID = 2 WHERE RequestDARID = {RequestDARID}";
                            query.Excute(sql);  // อัปเดตสถานะคำร้องขอ
                            // ตรวจสอบว่าผู้ตรวจสอบ ตรวจสอบไปแล้วหรือยัง ถ้าตรวจสอบแล้วต้องส่งแจ้งเตือน ถ้ายังไม่ตรวจสอบไม่ต้องส่งแจ้งเตือน
                            if (LeaderAcceptStatus == 1)
                            {
                                // แจ้งเตือน
                                string Name = DDListAcceptNPD.SelectedItem.Text;
                                LineNotify.Notify("มีรายการอนุมัติ\nถึงคุณ: " + Name + "\nรายละเอียด:http://110.77.148.173/DocumentControl/DocumentRequest/Approve.aspx");
                            }
                        }
                    }
                }
                else
                {
                    // ถ้าไม่เป็นการสร้างใหม่หรือยกเลิก และถ้ามี NPD รับทราบอยู่ให้ลบออก
                    if (NPDAcceptID != "0")
                    {
                        sql = $"DELETE DC_NPDAccept WHERE NPDAcceptID = {NPDAcceptID}";
                        query.Excute(sql);
                    }
                }

                // อัปเดตข้อมูลใบร้องขอ DAR
                sql = $@"UPDATE DC_RequestDAR SET UserIDUpdate = {UserID}, DateUpdateRequest = GETDATE(), RequestDARDocTypeID = {RequestDARDocTypeID}, DocTypeOther = '{DocTypeOther}', RequestDAROperationID = {RequestDAROperationID}, OperationOther = '{OperationOther}', NPDAcceptID = '{NPDAcceptID}', Remark = NULL WHERE RequestDARID = {RequestDARID}";
                query.Excute(sql);

                // บันทึกไฟล์
                string FilePath = $"\\\\192.168.0.100\\PDoc\\RequestDAR\\{UserIDCreate}\\{RequestDARID}";
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                // ข้อมูลเอกสาร
                foreach (GridViewRow row in GVRequestDARDoc.Rows)
                {
                    string RequestDARDocID = (row.Cells[0].FindControl("HFRequestDARDocID") as HiddenField).Value;
                    string DocNumber = (row.Cells[0].FindControl("TxtDocNumber") as TextBox).Text;
                    string DocName = (row.Cells[1].FindControl("TxtDocName") as TextBox).Text;
                    string DocDate = DateTime.Parse((row.Cells[2].FindControl("TxtDocDate") as TextBox).Text).ToString("yyyy-MM-dd");
                    string DocRemark = (row.Cells[3].FindControl("TxtDocRemark") as TextBox).Text;
                    FileUpload FileUpload = row.Cells[4].FindControl("FileUploadFile") as FileUpload;

                    string FileName = Path.GetFileNameWithoutExtension(FileUpload.FileName);
                    string Extension = Path.GetExtension(FileUpload.FileName);
                    string FullFileName = StringSpecial.StringSpecial(FileName) + " " + row.RowIndex + Extension;
                    string FullFilePath = Path.Combine(FilePath, FullFileName);

                    sql = $"SELECT RequestDARDocID, FilePath FROM DC_RequestDARDoc WHERE RequestDARDocID = '{RequestDARDocID}'";
                    string FilePathOld = query.SelectAt(1, sql);
                    DocName = DocName.Replace("'", "''");
                    DocRemark = DocRemark.Replace("'", "''");
                    // เช็คว่ามีเอกสารนี้หรือยัง ถ้ามีแล้วให้อัปเดต
                    if (query.CheckRow(sql))
                    {
                        if (FileUpload.HasFile)
                        {
                            File.Delete(FilePathOld);   // ลบไฟล์เก่า
                            FileUpload.SaveAs(FullFilePath);    // บันทึกไฟล์ใหม่
                            sql = $"UPDATE DC_RequestDARDoc SET DocNumber = '{DocNumber}', DocName = '{DocName}', DateEnforce = '{DocDate}', Remark = '{DocRemark}', FilePath = '{FullFilePath}', RequestDARDocStatusID = 1, PublishDate = NULL WHERE RequestDARDocID = {RequestDARDocID}";
                            query.Excute(sql);  // อัปเดตข้อมูล
                        }
                        else
                        {
                            // อัปเดตข้อมูลเอกสาร
                            sql = $"UPDATE DC_RequestDARDoc SET DocNumber = '{DocNumber}', DocName = '{DocName}', DateEnforce = '{DocDate}', Remark = '{DocRemark}', RequestDARDocStatusID = 1, PublishDate = NULL WHERE RequestDARDocID = {RequestDARDocID}";
                            query.Excute(sql);
                        }
                    }
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotificationAndRedirect('สำเร็จ', 'บันทึกข้อมูลสำเร็จ.', 'success', '/DocumentRequest/RequestDAR/RequestDARALL.aspx');", true);
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
            }
        }
    }
}