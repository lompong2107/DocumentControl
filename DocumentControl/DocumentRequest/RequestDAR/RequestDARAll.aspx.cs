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
    public partial class RequestDARAll : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LastPage"] = "~/DocumentRequest/RequestDAR/RequestDARAll.aspx";
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    GVRequestDAR.Sort("RequestDARID", SortDirection.Descending);
                    CBShowAll_CheckedChanged(null, null);
                    DDListPaging_SelectedIndexChanged(null, null);

                    // ตรวจสอบสิทธิ์การแก้ไขคำร้องขอแก้ไขเอกสาร DAR ของคนอื่น
                    string UserID = Session["UserID"].ToString();
                    sql = $"SELECT UserID FROM DC_PermissionUser WHERE PermissionID = 4 AND UserID = {UserID}";
                    if (query.CheckRow(sql))
                    {
                        GVRequestDAR.Columns[6].Visible = true;
                    }
                }
            }
            else
            {
                if (ViewState["FilterGVRequestDAR"] != null)
                {
                    SqlDataSourceRequestDAR.FilterExpression = ViewState["FilterGVRequestDAR"].ToString();
                }
            }
        }


        // --------------- GridView
        protected void GVRequestDAR_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string BtnName = e.CommandName;
            string RequestDARID = e.CommandArgument.ToString();
            if (BtnName == "BtnEdit")
            {
                Response.Redirect("RequestDAREditAll.aspx?RequestDARID=" + RequestDARID);
            }
            else if (BtnName == "BtnDelete")
            {
                HFRequestDARID.Value = RequestDARID;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "OpenModalCancel()", true);
            }
        }
        protected void GVRequestDAR_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int cellIndex = 0; cellIndex < e.Row.Cells.Count - 2; cellIndex++)
                {
                    e.Row.Cells[cellIndex].Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GVRequestDAR, "Select$" + e.Row.RowIndex);
                    e.Row.Cells[cellIndex].Attributes["style"] = "cursor:pointer";
                }
                string StatusID = DataBinder.Eval(e.Row.DataItem, "RequestDARStatusID").ToString();
                Panel PanelStatus = e.Row.FindControl("PanelStatus") as Panel;
                ImageButton ImageBtnEdit = e.Row.FindControl("ImageBtnEdit") as ImageButton;
                ImageButton ImageBtnDelete = e.Row.FindControl("ImageBtnDelete") as ImageButton;
                if (StatusID == "0")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-secondary";
                    ImageBtnDelete.Visible = false;
                }
                else if (StatusID == "4")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-danger";
                }
                else if (StatusID == "6")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-info";
                    ImageBtnEdit.Visible = false;
                    ImageBtnDelete.Visible = false;
                }
                else if (StatusID == "7")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-success";
                    ImageBtnEdit.Visible = false;
                    ImageBtnDelete.Visible = false;
                }
                else
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-warning";
                }
            }
        }
        protected void GVRequestDAR_SelectedIndexChanged(object sender, EventArgs e)
        {
            string RequestDARID = GVRequestDAR.DataKeys[GVRequestDAR.SelectedIndex].Values[0].ToString();
            Response.Redirect("RequestDARDetail.aspx?RequestDARID=" + RequestDARID);
        }


        // CheckBox
        // แสดงรายการทั้งหมด
        protected void CBShowAll_CheckedChanged(object sender, EventArgs e)
        {
            if (!CBShowAll.Checked)
            {
                // ไม่แสดงสถานะ ยกเลิกคำร้องขอ/ไม่อนุมัติ/เสร็จสมบูรณ์
                SqlDataSourceRequestDAR.FilterExpression = "RequestDARStatusID <> 7 AND RequestDARStatusID <> 4 AND RequestDARStatusID <> 0";
            }
            else
            {
                SqlDataSourceRequestDAR.FilterExpression = null;
            }
            ViewState.Add("FilterGVRequestDAR", SqlDataSourceRequestDAR.FilterExpression);
        }


        // DropDownList
        // จำนวนแถวที่แสดงใน GridView
        protected void DDListPaging_SelectedIndexChanged(object sender, EventArgs e)
        {
            GVRequestDAR.PageSize = int.Parse(DDListPaging.SelectedValue);
            GVRequestDAR.DataBind();
        }


        // Button
        // ปุ่มยกเลิกคำร้องขอ
        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                string RemarkCancel = TxtRemarkCancel.Text;
                if (string.IsNullOrEmpty(RemarkCancel))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณาระบุเหตุผล.', 'warning');", true);
                    return;
                }
                string UserID = Session["UserID"].ToString();
                string RequestDARID = HFRequestDARID.Value;
                sql = $"UPDATE DC_RequestDAR SET RequestDARStatusID = 0, UserIDUpdate = {UserID}, DateUpdateRequest = GETDATE(), RemarkCancel = '{RemarkCancel}' WHERE RequestDARID = {RequestDARID}";
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'ยกเลิกคำร้องขอสำเร็จ.', 'success');", true);
                    GVRequestDAR.DataBind();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
    }
}