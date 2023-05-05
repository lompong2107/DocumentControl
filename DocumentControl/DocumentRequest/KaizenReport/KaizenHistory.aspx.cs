using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest.KaizenReport
{
    public partial class KaizenHistory : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LastPage"] = "~/DocumentRequest/KaizenReport/KaizenHistory.aspx";
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    LoadCountStatus();
                    LoadDDListStatus();
                    GVKaizen.Sort("KaizenID", SortDirection.Descending);
                    DDListPagingKaizen_SelectedIndexChanged(null, null);
                }
            }
        }


        // --------------- Function
        private void LoadDDListStatus()
        {
            sql = "SELECT KaizenStatusID, KaizenStatusDetail FROM DC_KaizenStatus;";
            DDListStatus.DataSource = query.SelectTable(sql);
            DDListStatus.DataBind();
        }
        private void LoadCountStatus()
        {
            string UserID = Session["UserID"].ToString();
            // รายการทั้งหมด
            sql = $"SELECT COUNT(KaizenID) FROM DC_Kaizen WHERE UserID = {UserID} AND KaizenStatusID != 0";
            LbKaizenAll.Text = query.SelectAt(0, sql);
            // รายการที่อนุมัติแล้ว
            sql = $"SELECT COUNT(KaizenID) FROM DC_Kaizen WHERE UserID = {UserID} AND KaizenStatusID = 4";
            LbKaizenCompleted.Text = query.SelectAt(0, sql);
            // รายการที่รอตรวจสอบ/รออนุมัติ
            sql = $"SELECT COUNT(KaizenID) FROM DC_Kaizen WHERE UserID = {UserID} AND (KaizenStatusID = 1 OR KaizenStatusID = 2)";
            LbKaizenApprove.Text = query.SelectAt(0, sql);
            // รายการที่ไม่อนุมัติ
            sql = $"SELECT COUNT(KaizenID) FROM DC_Kaizen WHERE UserID = {UserID} AND KaizenStatusID = 3";
            LbKaizenDisApprove.Text = query.SelectAt(0, sql);
        }


        // --------------- GridView
        // Kaizen Report
        protected void GVKaizen_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int cellIndex = 0; cellIndex < e.Row.Cells.Count - 1; cellIndex++)
                {
                    e.Row.Cells[cellIndex].Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GVKaizen, "Select$" + e.Row.RowIndex);
                    e.Row.Cells[cellIndex].Attributes["style"] = "cursor:pointer";
                }
                string StatusID = DataBinder.Eval(e.Row.DataItem, "KaizenStatusID").ToString();
                Panel PanelStatus = e.Row.FindControl("PanelStatus") as Panel;
                ImageButton ImageBtnEdit = e.Row.FindControl("ImageBtnEdit") as ImageButton;
                ImageButton ImageBtnDelete = e.Row.FindControl("ImageBtnDelete") as ImageButton;
                if (StatusID == "0")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-secondary";
                    ImageBtnDelete.Visible = false;
                }
                else if (StatusID == "3")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-danger";
                }
                else if (StatusID == "4")
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
        protected void GVKaizen_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string Btn = e.CommandName;
            string KaizenID = e.CommandArgument.ToString();
            if (Btn == "BtnEdit")
            {
                Response.Redirect("~/DocumentRequest/KaizenReport/KaizenEdit.aspx?KaizenID=" + KaizenID);
            }
            else if (Btn == "BtnDelete")
            {
                sql = "UPDATE DC_Kaizen SET KaizenStatusID = 0 WHERE KaizenID = " + KaizenID;
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'ยกเลิกคำสำเร็จ.', 'success');", true);
                }
                GVKaizen.DataBind();
            }
        }
        protected void GVKaizen_SelectedIndexChanged(object sender, EventArgs e)
        {
            string KaizenID = GVKaizen.DataKeys[GVKaizen.SelectedIndex].Values[0].ToString();
            Response.Redirect("~/DocumentRequest/KaizenReport/KaizenDetail.aspx?KaizenID=" + KaizenID);
        }


        // DropDownList
        // สถานะ Kaizen
        protected void DDListStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            GVKaizen.DataBind();
        }
        protected void DDListStatus_DataBound(object sender, EventArgs e)
        {
            DDListStatus.Items.Insert(0, new ListItem("แสดงทั้งหมด", "%"));
            DDListStatus.SelectedIndex = 0;
        }
        // จำนวนแถวที่แสดงใน GridView
        protected void DDListPagingKaizen_SelectedIndexChanged(object sender, EventArgs e)
        {
            GVKaizen.PageSize = int.Parse(DDListPagingKaizen.SelectedValue);
            GVKaizen.DataBind();
        }
    }
}