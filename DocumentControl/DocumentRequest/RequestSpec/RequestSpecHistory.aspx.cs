using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest.RequestSpec
{
    public partial class RequestSpecHistory : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LastPage"] = "~/DocumentRequest/RequestSpec/RequestSpecHistory.aspx";
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    LoadCountStatus();
                    LoadDDListStatus();
                    GVRequestSpec.Sort("RequestSpecID", SortDirection.Descending);
                    DDListPagingRequestSpec_SelectedIndexChanged(null, null);
                }
            }
        }


        // --------------- Function
        private void LoadDDListStatus()
        {
            sql = "SELECT RequestSpecStatusID, RequestSpecStatusDetail FROM DC_RequestSpecStatus;";
            DDListStatus.DataSource = query.SelectTable(sql);
            DDListStatus.DataBind();
        }
        private void LoadCountStatus()
        {
            string UserID = Session["UserID"].ToString();
            // รายการทั้งหมด
            sql = $"SELECT COUNT(RequestSpecID) FROM DC_RequestSpec WHERE UserID = {UserID} AND RequestSpecStatusID != 0";
            LbRequestAll.Text = query.SelectAt(0, sql);
            // รายการที่สำเร็จแล้ว
            sql = $"SELECT COUNT(RequestSpecID) FROM DC_RequestSpec WHERE UserID = {UserID} AND RequestSpecStatusID = 8";
            LbRequestCompleted.Text = query.SelectAt(0, sql);
            // รายการที่แจกจ่ายแล้ว
            sql = $"SELECT COUNT(RequestSpecID) FROM DC_RequestSpec WHERE UserID = {UserID} AND RequestSpecStatusID = 7";
            LbRequestPublish.Text = query.SelectAt(0, sql);
            // รายการที่/รออนุมัติ/รอดำเนินการตรวจสอบ/แก้ไข (NPD)
            sql = $"SELECT COUNT(RequestSpecID) FROM DC_RequestSpec WHERE UserID = {UserID} AND (RequestSpecStatusID = 2 OR RequestSpecStatusID = 4 OR RequestSpecStatusID = 5)";
            LbRequestApprove.Text = query.SelectAt(0, sql);
            // รายการที่ไม่อนุมัติ
            sql = $"SELECT COUNT(RequestSpecID) FROM DC_RequestSpec WHERE UserID = {UserID} AND (RequestSpecStatusID = 3 OR RequestSpecStatusID = 6)";
            LbRequestDisApprove.Text = query.SelectAt(0, sql);
        }


        // --------------- GridView
        // Request Spec
        protected void GVRequestSpec_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int cellIndex = 0; cellIndex < e.Row.Cells.Count - 1; cellIndex++)
                {
                    e.Row.Cells[cellIndex].Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GVRequestSpec, "Select$" + e.Row.RowIndex);
                    e.Row.Cells[cellIndex].Attributes["style"] = "cursor:pointer";
                }
                string StatusID = DataBinder.Eval(e.Row.DataItem, "RequestSpecStatusID").ToString();
                Panel PanelStatus = e.Row.FindControl("PanelStatus") as Panel;
                ImageButton ImageBtnEdit = e.Row.FindControl("ImageBtnEdit") as ImageButton;
                ImageButton ImageBtnDelete = e.Row.FindControl("ImageBtnDelete") as ImageButton;
                if (StatusID == "0")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-secondary";
                    ImageBtnDelete.Visible = false;
                }
                else if (StatusID == "3" || StatusID == "6")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-danger";
                }
                else if (StatusID == "7")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-info";
                    ImageBtnEdit.Visible = false;
                    ImageBtnDelete.Visible = false;
                }
                else if (StatusID == "8")
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
        protected void GVRequestSpec_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string Btn = e.CommandName;
            string RequestSpecID = e.CommandArgument.ToString();
            if (Btn == "BtnEdit")
            {
                Response.Redirect("~/DocumentRequest/RequestSpec/RequestSpecEdit.aspx?RequestSpecID=" + RequestSpecID);
            }
            else if (Btn == "BtnDelete")
            {
                HFRequestSpecID.Value = RequestSpecID;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "OpenModalCancel()", true);
            }
        }
        protected void GVRequestSpec_SelectedIndexChanged(object sender, EventArgs e)
        {
            string RequestSpecID = GVRequestSpec.DataKeys[GVRequestSpec.SelectedIndex].Values[0].ToString();
            Response.Redirect("~/DocumentRequest/RequestSpec/RequestSpecDetail.aspx?RequestSpecID=" + RequestSpecID);
        }


        // DropDownList
        // สถานะ Request Spec
        protected void DDListStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            GVRequestSpec.DataBind();
        }
        protected void DDListStatus_DataBound(object sender, EventArgs e)
        {
            DDListStatus.Items.Insert(0, new ListItem("แสดงทั้งหมด", "%"));
            DDListStatus.SelectedIndex = 0;
        }
        // จำนวนแถวที่แสดงใน GridView
        protected void DDListPagingRequestSpec_SelectedIndexChanged(object sender, EventArgs e)
        {
            GVRequestSpec.PageSize = int.Parse(DDListPagingRequestSpec.SelectedValue);
            GVRequestSpec.DataBind();
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
                string RequestSpecID = HFRequestSpecID.Value;
                sql = $"UPDATE DC_RequestSpec SET RequestSpecStatusID = 0 AND UserIDUpdate = {UserID} AND DateUpdateRequest = GETDATE() AND RemarkCancel = '{RemarkCancel}' WHERE RequestSpecID = {RequestSpecID}";
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'ยกเลิกคำร้องขอสำเร็จ.', 'success');", true);
                    GVRequestSpec.DataBind();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
    }
}