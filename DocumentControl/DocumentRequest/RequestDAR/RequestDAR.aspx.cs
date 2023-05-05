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
    public partial class RequestDAR : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        LineNotify LineNotify = new LineNotify();
        StringSpecialClass StringSpecial = new StringSpecialClass();
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LastPage"] = "~/DocumentRequest/RequestDAR/RequestDAR.aspx";
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    LoadDDListDetailRequest();
                    LoadUserRequest();
                    LoadDDListAcceptLeader();
                    LoadDDListAcceptNPD();
                    LoadDDListApprove();
                }

                CreateDatatable();
            }
        }


        // --------------- GridView
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
                GridViewRow gvr = (GridViewRow)((ImageButton)e.CommandSource).NamingContainer;
                int RowIndex = gvr.RowIndex;
                DataTable dtDoc = (DataTable)Session["GVRequestDARDoc"];
                DataRow dr = dtDoc.Rows[RowIndex];
                dtDoc.Rows.Remove(dr);
                Session["GVRequestDARDoc"] = dtDoc;
                BindGrid();
            }
        }
        

        // --------------- Function
        private void CreateDatatable()
        {
            DataTable dtDoc = new DataTable();
            dtDoc.Columns.AddRange(new DataColumn[5] {
                    new DataColumn("DocNumber"), new DataColumn("DocName"), new DataColumn("DateEnforce"), new DataColumn("Remark"), new DataColumn("FilePath")
                });
            dtDoc.Rows.Add(string.Empty, string.Empty, DateTime.Now.ToString("dd/MM/yyyy"), string.Empty, string.Empty);
            Session["GVRequestDARDoc"] = dtDoc;
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
        private void LoadUserRequest()
        {
            string UserID = Session["UserID"].ToString();
            sql = $"SELECT (FirstNameTH + ' ' + LastNameTH) As Name FROM F2_Users WHERE UserID = {UserID}";
            TxtUserRequest.Text = query.SelectAt(0, sql);
        }
        private void LoadDDListAcceptLeader()
        {
            string DepartmentID = Session["DepartmentID"].ToString();
            sql = $"SELECT F2_Users.UserID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) As Name FROM F2_UserOrganization LEFT JOIN F2_Users ON F2_UserOrganization.UserID = F2_Users.UserID WHERE F2_UserOrganization.DepartmentID = {DepartmentID} AND F2_Users.Status = 1";

            //sql = "SELECT PoPermissID,PositionID,PermissID,PositionsName,PositionsDetail FROM HR_Permiss_Pos INNER JOIN F2_Positions ON F2_Positions.PositionsID = HR_Permiss_Pos.PositionID WHERE PermissID = '1' And Status = '1';";
            //DataTable dt = query.SelectTable(sql);
            //string FilterStr = string.Empty;
            //foreach (DataRow row in dt.Rows)
            //{
            //    FilterStr += " OR F2_Users.PositionsID = '" + row["PositionID"] + "'";
            //}
            //sql = @"SELECT F2_Users.UserID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTh) As Name 
            //    FROM F2_Users 
            //    INNER JOIN F2_Positions ON F2_Users.PositionsID = F2_Positions.PositionsID 
            //    WHERE F2_Users.UserID IN (SELECT F2_UserOrganization.UserID FROM F2_UserOrganization WHERE DepartmentID = '" + DepartmentID + "' AND F2_Users.Status = '1' AND (F2_Users.PositionsID = '0' " + FilterStr + " )) ORDER BY Name;";

            DDListAcceptLeader.DataSource = query.SelectTable(sql);
            DDListAcceptLeader.DataBind();
        }
        private void LoadDDListAcceptNPD()
        {
            // แผนก NPD
            sql = "SELECT F2_Users.UserID, (FirstNameTH + ' ' + LastNameTH + ' ' + ISNULL(DC_UserApprove.Remark, '')) As Name FROM DC_UserApprove LEFT JOIN F2_Users ON DC_UserApprove.UserID = F2_Users.UserID WHERE DC_UserApprove.StatusPermission = 1 AND F2_Users.Status = 1";
            DDListAcceptNPD.DataSource = query.SelectTable(sql);
            DDListAcceptNPD.DataBind();
        }
        private void LoadDDListApprove()
        {
            // QMR และ EMR
            sql = "SELECT F2_Users.UserID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH + ' ' + ISNULL(DC_UserApprove.Remark, '')) As Name FROM DC_UserApprove LEFT JOIN F2_Users ON DC_UserApprove.UserID = F2_Users.UserID WHERE DC_UserApprove.StatusPermission = 2 AND F2_Users.Status = 1";
            DDListApprove.DataSource = query.SelectTable(sql);
            DDListApprove.DataBind();
        }


        // --------------- Button
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string RequestDARDocTypeID = DDListRequestDARDocType.SelectedValue; // ชนิดของเอกสาร
                string DocTypeOther = TxtDocTypeOther.Text; // ชนิดของเอกสาร อื่นๆ
                string RequestDAROperationID = DDListRequestDAROperation.SelectedValue; // การดำเนินการ
                string OperationOther = TxtOperationOther.Text; // การดำเนินการ อื่นๆ
                string UserID = Session["UserID"].ToString();   // ผู้ร้องขอ
                string AcceptLeader = DDListAcceptLeader.SelectedValue; // หัวหน้าหน่วยงานผู้ร้องขอ
                string Approve = DDListApprove.SelectedValue;   // ผู้อนุมัติ QMR / EMR

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
                // ตรวจสอบการกรอกข้อมูลของเอกสารที่แนบ
                foreach (GridViewRow row in GVRequestDARDoc.Rows)
                {
                    string DocNumber = (row.Cells[0].FindControl("TxtDocNumber") as TextBox).Text;  // เลขที่เอกสาร
                    string DocName = (row.Cells[1].FindControl("TxtDocName") as TextBox).Text;  // ชื่อเอกสาร
                    string DocDate = (row.Cells[2].FindControl("TxtDocDate") as TextBox).Text;  // วันที่บังคับใช้
                    string DocRemark = (row.Cells[3].FindControl("TxtDocRemark") as TextBox).Text;  // เหตุผลการร้องขอ
                    FileUpload FileUpload = row.Cells[4].FindControl("FileUploadFile") as FileUpload;   // ไฟล์แนบ
                    if (string.IsNullOrEmpty(DocNumber) || string.IsNullOrEmpty(DocName) || string.IsNullOrEmpty(DocDate) || string.IsNullOrEmpty(DocRemark) || (FileUpload.HasFile == false))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณากรอกข้อมูลเอกสารให้ครบ.', 'warning');", true);
                        return;
                    }
                }
                // ---------- END ตรวจสอบการกรอกข้อมูล ----------

                // เพิ่มหัวหน้าแผนกตรวจสอบและรับทราบ
                sql = $"INSERT INTO DC_LeaderAccept (UserID, AcceptStatus) VALUES ({AcceptLeader}, 2)";
                query.Excute(sql);
                // ดึงข้อมูลที่เพิ่มล่าสุด
                sql = $"SELECT TOP 1 LeaderAcceptID FROM DC_LeaderAccept WHERE UserID = {AcceptLeader} AND AcceptStatus = 2 ORDER BY LeaderAcceptID DESC";
                string LeaderAccpetID = query.SelectAt(0, sql);

                // เพิ่มผู้จัดการโรงงานอนุมัติ
                sql = $"INSERT INTO DC_Approve (UserID, ApproveStatus) VALUES ({Approve}, 2)";
                query.Excute(sql);
                // ดึงข้อมูลที่เพิ่มล่าสุด
                sql = $"SELECT TOP 1 ApproveID FROM DC_Approve WHERE UserID = {Approve} AND ApproveStatus = 2 ORDER BY ApproveID DESC";
                string ApproveID = query.SelectAt(0, sql);

                // ถ้าเป็นการสร้างใหม่หรือยกเลิก จะเพิ่มการตรวจสอบที่ NPD ด้วย
                string NPDAcceptID = string.Empty;
                if (RequestDAROperationID == "2" || RequestDAROperationID == "3")
                {
                    string AcceptNPD = DDListAcceptNPD.SelectedValue;
                    sql = $"INSERT INTO DC_NPDAccept (UserID, AcceptStatus) VALUES ({AcceptNPD}, 2)";
                    query.Excute(sql);
                    // ดึงข้อมูลที่เพิ่มล่าสุด
                    sql = $"SELECT TOP 1 NPDAcceptID FROM DC_NPDAccept WHERE UserID = {AcceptNPD} AND AcceptStatus = 2 ORDER BY NPDAcceptID DESC";
                    NPDAcceptID = query.SelectAt(0, sql);
                }

                // เพิ่มใบร้องขอ DAR
                sql = $@"INSERT INTO DC_RequestDAR (UserID, DateRequest, RequestDARDocTypeID, DocTypeOther, RequestDAROperationID, OperationOther, LeaderAcceptID, NPDAcceptID, ApproveID, RequestDARStatusID) 
                VALUES ({UserID}, GETDATE(), {RequestDARDocTypeID}, '{DocTypeOther}', {RequestDAROperationID}, '{OperationOther}', {LeaderAccpetID}, '{NPDAcceptID}', {ApproveID}, 2)";
                query.Excute(sql);
                // ดึงข้อมูลที่เพิ่มล่าสุด
                sql = $"SELECT TOP 1 RequestDARID FROM DC_RequestDAR WHERE UserID = {UserID} ORDER BY RequestDARID DESC";
                string RequestDARID = query.SelectAt(0, sql);

                // เพิ่มไฟล์
                // ตรวจสอบว่ามี Folder หลักของการร้องขอนี้ หรือยัง ถ้ายังไม่มีให้สร้าง
                string FilePath = $"\\\\192.168.0.100\\PDoc\\RequestDAR\\{UserID}\\{RequestDARID}";
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }
                int CountFile = 1;  // นับไฟล์ ใช้สร้าง Folder
                string DateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HHmmss");    // วันเดือนปีเวลาใช้สร้าง Folder
                foreach (GridViewRow row in GVRequestDARDoc.Rows)
                {
                    // ตรวจสอบว่ามี Folder ย่อยสำหรับแต่ละเอกสารหริอยัง ถ้ายังไม่มีให้สร้าง
                    string FilePathSub = $"{FilePath}\\{DateTimeNow} {CountFile}";
                    if (!Directory.Exists(FilePathSub))
                    {
                        Directory.CreateDirectory(FilePathSub);
                    }
                    string DocNumber = (row.Cells[0].FindControl("TxtDocNumber") as TextBox).Text;  // เลขที่เอกสาร
                    string DocName = (row.Cells[1].FindControl("TxtDocName") as TextBox).Text.Replace("'", "''");  // ชื่อเอกสาร
                    string DocDate = DateTime.ParseExact((row.Cells[2].FindControl("TxtDocDate") as TextBox).Text, "dd/MM/yyyy", null).ToString("yyyy-MM-dd");  // วันที่บังคับใช้
                    string DocRemark = (row.Cells[3].FindControl("TxtDocRemark") as TextBox).Text.Replace("'", "''");  // เหตุผลการร้องขอ
                    FileUpload FileUpload = row.Cells[4].FindControl("FileUploadFile") as FileUpload;   // ไฟล์แนบ
                    string FileName = Path.GetFileNameWithoutExtension(FileUpload.FileName);    // ชื่อไฟล์
                    string Extension = Path.GetExtension(FileUpload.FileName);  // นามสกุลไฟล์
                    string FullFileName = StringSpecial.StringSpecial(FileName) + Extension;    // ชื่อไฟล์ที่ตัดอักษรพิเศษ กับนามสกุลไฟล์
                    string FullFilePath = Path.Combine(FilePathSub, FullFileName);  // ที่อยู่จัดเก็บไฟล์
                    FullFilePath = FullFilePath.Replace("'", "''"); // ป้องกันการบัคซักหน่อย
                    if (FileName.Length > 0)
                    {
                        FileUpload.SaveAs(FullFilePath);    // บันทึกไฟล์
                    }
                    // เพิ่มข้อมูลไฟล์
                    sql = $@"INSERT INTO DC_RequestDARDoc (RequestDARID, DocNumber, DocName, DateEnforce, Remark, FilePath, RequestDARDocStatusID)
                    VALUES ({RequestDARID}, '{DocNumber}', '{DocName}', '{DocDate}', '{DocRemark}', '{FullFilePath}', 1)";
                    query.Excute(sql);
                    CountFile++;    // เพิ่มค่าไป 1
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'การร้องขอสำเร็จ.', 'success');", true);
                LoadDDListDetailRequest();
                CreateDatatable();
                // แจ้งเตือน
                string Name = DDListAcceptLeader.SelectedItem.Text;
                LineNotify.Notify($"มีรายการอนุมัติ\nถึงคุณ: {Name}\nรายละเอียด: http://110.77.148.173/DocumentControl/DocumentRequest/Approve.aspx");
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
        protected void LinkBtnAddRequestDARDoc_Click(object sender, EventArgs e)
        {
            DataTable dtDoc = (DataTable)Session["GVRequestDARDoc"];
            for (int RowIndex = 0; RowIndex < GVRequestDARDoc.Rows.Count; RowIndex++)
            {
                dtDoc.Rows[RowIndex][0] = (GVRequestDARDoc.Rows[RowIndex].Cells[0].FindControl("TxtDocNumber") as TextBox).Text;
                dtDoc.Rows[RowIndex][1] = (GVRequestDARDoc.Rows[RowIndex].Cells[1].FindControl("TxtDocName") as TextBox).Text;
                dtDoc.Rows[RowIndex][2] = (GVRequestDARDoc.Rows[RowIndex].Cells[2].FindControl("TxtDocDate") as TextBox).Text;
                dtDoc.Rows[RowIndex][3] = (GVRequestDARDoc.Rows[RowIndex].Cells[3].FindControl("TxtDocRemark") as TextBox).Text;
            }
            dtDoc.Rows.Add(string.Empty, string.Empty, DateTime.Now.ToString("dd/MM/yyyy"), string.Empty, string.Empty);
            Session["GVRequestDARDoc"] = dtDoc;
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

       
    }
}