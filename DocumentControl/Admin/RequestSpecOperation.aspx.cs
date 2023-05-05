using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Admin
{
    public partial class RequestSpecOperation : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        // --------------- GridView
        protected void GVRequestSpecOperation_RowDataBound(object sender, GridViewRowEventArgs e)
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
        protected void GVRequestSpecOperation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string Btn = e.CommandName.ToString();
            string RequestSpecOperationID = e.CommandArgument.ToString();
            if (Btn == "BtnStatus")
            {
                // สลับสถานะ
                sql = "SELECT Status FROM DC_RequestSpecOperation WHERE RequestSpecOperationID = " + RequestSpecOperationID;
                int Status = int.Parse(query.SelectAt(0, sql));
                if (Status == 1)
                {
                    Status = 0;
                }
                else
                {
                    Status = 1;
                }
                sql = "UPDATE DC_RequestSpecOperation SET Status = " + Status + " WHERE RequestSpecOperationID = " + RequestSpecOperationID;
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'เปลี่ยนสถานะสำเร็จ', 'success');", true);
                    GVRequestSpecOperation.DataBind();
                }
            }
            else if (Btn == "BtnEdit")
            {
                HFRequestSpecOperationID.Value = RequestSpecOperationID;
                sql = "SELECT OperationName FROM DC_RequestSpecOperation WHERE RequestSpecOperationID = " + RequestSpecOperationID;
                TxtOperationNameEdit.Text = query.SelectAt(0, sql);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "OpenModalEdit()", true);
            }
            else if (Btn == "BtnDelete")
            {
                // เช็คว่ามีการใช้งานในประวัติการจองหรือไม่
                sql = "SELECT RequestSpecID FROM DC_RequestSpec WHERE RequestSpecOperationID = " + RequestSpecOperationID;
                if (!query.CheckRow(sql))   // ถ้ามี ห้ามลบ
                {
                    sql = "DELETE DC_RequestSpecOperation WHERE RequestSpecOperationID = " + RequestSpecOperationID;
                    if (query.Excute(sql))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'ลบข้อมูลสำเร็จ', 'success');", true);
                        GVRequestSpecOperation.DataBind();
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
                string OperationName = TxtOperationName.Text;
                sql = "INSERT INTO DC_RequestSpecOperation (OperationName) VALUES ('" + OperationName + "')";
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', 'success');", true);
                    TxtOperationName.Text = null;
                    GVRequestSpecOperation.DataBind();
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
                string RequestSpecOperationID = HFRequestSpecOperationID.Value;
                string OperationName = TxtOperationNameEdit.Text;
                sql = "UPDATE DC_RequestSpecOperation SET OperationName = '" + OperationName + "' WHERE RequestSpecOperationID = " + RequestSpecOperationID;
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'บันทึกข้อมูลสำเร็จ', 'success');", true);
                    GVRequestSpecOperation.DataBind();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
    }
}