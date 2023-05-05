using DocumentControl.DocumentRequest.RequestDAR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Admin
{
    public partial class RequestDARDocType : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        // --------------- GridView
        protected void GVRequestDARDocType_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton BtnStatus = e.Row.FindControl("BtnStatus") as LinkButton;
                string Status = DataBinder.Eval(e.Row.DataItem, "Status").ToString();
                if (Status == "0")
                {
                    BtnStatus.Text = "ไม่ใช้งาน";
                    BtnStatus.CssClass = "text-danger";
                }
                else
                {
                    BtnStatus.Text = "ใช้งาน";
                    BtnStatus.CssClass = "text-success";
                }
            }
        }
        protected void GVRequestDARDocType_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string Btn = e.CommandName.ToString();
            string RequestDARDocTypeID = e.CommandArgument.ToString();
            if (Btn == "BtnStatus")
            {
                // สลับสถานะ
                sql = $"SELECT Status FROM DC_RequestDARDocType WHERE RequestDARDocTypeID = {RequestDARDocTypeID}";
                int Status = int.Parse(query.SelectAt(0, sql));
                if (Status == 1)
                {
                    Status = 0;
                }
                else
                {
                    Status = 1;
                }
                sql = $"UPDATE DC_RequestDARDocType SET Status = {Status} WHERE RequestDARDocTypeID = {RequestDARDocTypeID}";
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'เปลี่ยนสถานะสำเร็จ', 'success');", true);
                    GVRequestDARDocType.DataBind();
                }
            }
            else if (Btn == "BtnEdit")
            {
                HFRequestDARDocTypeID.Value = RequestDARDocTypeID;
                sql = $"SELECT DocTypeName FROM DC_RequestDARDocType WHERE RequestDARDocTypeID = {RequestDARDocTypeID}";
                TxtDocTypeNameEdit.Text = query.SelectAt(0, sql);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "OpenModalEdit()", true);
            }
            else if (Btn == "BtnDelete")
            {
                // เช็คว่ามีการใช้งานในประวัติการจองหรือไม่
                sql = $"SELECT RequestDARID FROM DC_RequestDAR WHERE RequestDARDocTypeID = {RequestDARDocTypeID}";
                if (!query.CheckRow(sql))   // ถ้ามี ห้ามลบ
                {
                    sql = $"DELETE DC_RequestDARDocType WHERE RequestDARDocTypeID = {RequestDARDocTypeID}";
                    if (query.Excute(sql))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'ลบข้อมูลสำเร็จ', 'success');", true);
                        GVRequestDARDocType.DataBind();
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'ไม่สามารถลบได้ มีการนำข้อมูลไปใช้!', 'warning');", true);
                }
            }
        }


        // --------------- Button
        protected void LinkBtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string DocTypeName = TxtDocTypeName.Text;
                sql = $"INSERT INTO DC_RequestDARDocType (DocTypeName) VALUES ('{DocTypeName}')";
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', 'success');", true);
                    TxtDocTypeName.Text = null;
                    GVRequestDARDocType.DataBind();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
        // ปุ่มอัปเดตข้อมูล
        protected void LinkBtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string RequestDARDocTypeID = HFRequestDARDocTypeID.Value;
                string DocTypeName = TxtDocTypeNameEdit.Text;
                sql = $"UPDATE DC_RequestDARDocType SET DocTypeName = '{DocTypeName}' WHERE RequestDARDocTypeID = {RequestDARDocTypeID}";
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', 'success');", true);
                    GVRequestDARDocType.DataBind();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
    }
}