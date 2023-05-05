using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest.RequestDAR
{
    public partial class RequestDARHistory : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LastPage"] = "~/DocumentRequest/RequestDAR/RequestDARHistory.aspx";
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    LoadCountStatus();
                    LoadDDListStatus();
                    GVRequestDAR.Sort("RequestDARID", SortDirection.Descending);
                    DDListPagingRequestDAR_SelectedIndexChanged(null, null);
                }
            }
        }


        // --------------- Function
        private void LoadDDListStatus()
        {
            sql = "SELECT RequestDARStatusID, RequestDARStatusDetail FROM DC_RequestDARStatus;";
            DDListRequestDARStatus.DataSource = query.SelectTable(sql);
            DDListRequestDARStatus.DataBind();
        }
        private void LoadCountStatus()
        {
            string UserID = Session["UserID"].ToString();
            // รายการทั้งหมด
            sql = $"SELECT COUNT(RequestDARID) FROM DC_RequestDAR WHERE UserID = {UserID} AND RequestDARStatusID != 0";
            LbRequestAll.Text = query.SelectAt(0, sql);
            // รายการที่สำเร็จแล้ว
            sql = $"SELECT COUNT(RequestDARID) FROM DC_RequestDAR WHERE UserID = {UserID} AND RequestDARStatusID = 7";
            LbRequestCompleted.Text = query.SelectAt(0, sql);
            // รายการที่แจกจ่ายแล้ว
            sql = $"SELECT COUNT(RequestDARID) FROM DC_RequestDAR WHERE UserID = {UserID} AND RequestDARStatusID = 6";
            LbRequestPublish.Text = query.SelectAt(0, sql);
            // รายการที่รอตรวจสอบ/รออนุมัติ/รอดำเนินการแก้ไข (DC)
            sql = $"SELECT COUNT(RequestDARID) FROM DC_RequestDAR WHERE UserID = {UserID} AND (RequestDARStatusID = 2 OR RequestDARStatusID = 3 OR RequestDARStatusID = 6)";
            LbRequestApprove.Text = query.SelectAt(0, sql);
            // รายการที่ไม่อนุมัติ
            sql = $"SELECT COUNT(RequestDARID) FROM DC_RequestDAR WHERE UserID = {UserID} AND RequestDARStatusID = 4";
            LbRequestDisApprove.Text = query.SelectAt(0, sql);
        }


        // --------------- GridView
        // Request DAR
        protected void GVRequestDAR_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int cellIndex = 0; cellIndex < e.Row.Cells.Count - 1; cellIndex++)
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
                    ImageBtnEdit.Visible = false;
                    ImageBtnDelete.Visible = false;
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-success";
                }
                else
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-warning";
                }
            }
        }
        protected void GVRequestDAR_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string Btn = e.CommandName;
            string RequestDARID = e.CommandArgument.ToString();
            if (Btn == "BtnEdit")
            {
                Response.Redirect("~/DocumentRequest/RequestDAR/RequestDAREdit.aspx?RequestDARID=" + RequestDARID);
            }
            else if (Btn == "BtnDelete")
            {
                HFRequestDARID.Value = RequestDARID;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "OpenModalCancel()", true);
            }
        }
        protected void GVRequestDAR_SelectedIndexChanged(object sender, EventArgs e)
        {
            string RequestDARID = GVRequestDAR.DataKeys[GVRequestDAR.SelectedIndex].Values[0].ToString();
            Response.Redirect("~/DocumentRequest/RequestDAR/RequestDARDetail.aspx?RequestDARID=" + RequestDARID);
        }


        // DropDownList
        // สถานะ RequestDAR
        protected void DDListRequestDARStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            GVRequestDAR.DataBind();
        }
        protected void DDListRequestDARStatus_DataBound(object sender, EventArgs e)
        {
            DDListRequestDARStatus.Items.Insert(0, new ListItem("แสดงทั้งหมด", "%"));
            DDListRequestDARStatus.SelectedIndex = 0;
        }
        // จำนวนแถวที่แสดงใน GridView
        protected void DDListPagingRequestDAR_SelectedIndexChanged(object sender, EventArgs e)
        {
            GVRequestDAR.PageSize = int.Parse(DDListPagingRequestDAR.SelectedValue);
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
                sql = $"UPDATE DC_RequestDAR SET RequestDARStatusID = 0 AND UserIDUpdate = {UserID} AND DateUpdateRequest = GETDATE() AND RemarkCancel = '{RemarkCancel}' WHERE RequestDARID = {RequestDARID}";
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