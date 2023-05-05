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
    public partial class LogBook : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        LineNotify LineNotify = new LineNotify();
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LastPage"] = "~/DocumentRequest/RequestDAR/LogBook.aspx";
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    if (!CheckPermission())
                    {
                        Response.Redirect("~/Default.aspx");
                    }
                    GVRequestDAR.Sort("RequestDARID", SortDirection.Descending);
                    CBShowAll_CheckedChanged(null, null);
                    DDListPaging_SelectedIndexChanged(null, null);
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
        protected void GVRequestDAR_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GVRequestDAR, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["style"] = "cursor:pointer";

                string StatusID = DataBinder.Eval(e.Row.DataItem, "RequestDARStatusID").ToString();
                Panel PanelStatus = e.Row.FindControl("PanelStatus") as Panel;
                if (StatusID == "4")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-danger";
                }
                else if (StatusID == "6")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-info";
                }
                else if (StatusID == "7")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-success";
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
            Response.Redirect("~/DocumentRequest/RequestDAR/RequestDARDetail.aspx?RequestDARID=" + RequestDARID);
        }


        // CheckBox
        // แสดงรายการทั้งหมด
        protected void CBShowAll_CheckedChanged(object sender, EventArgs e)
        {
            if (CBShowAll.Checked)
            {
                SqlDataSourceRequestDAR.FilterExpression = "(RequestDARStatusID = 5 OR RequestDARStatusID = 6) AND RequestDARDocStatusID <> 0";
            }
            else
            {
                SqlDataSourceRequestDAR.FilterExpression = "RequestDARStatusID = 5 AND RequestDARDocStatusID <> 0 ";
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

        // ตรวจสอบสิทธิ์ Document Control
        private bool CheckPermission()
        {
            // PermissionID 3 = สิทธิ์ Document Control รายงานสถานะการดำเนินการ สามารถอัปเดตสถานะเอกสารได้(เป็นผู้แจกจ่ายเอกสารจากการร้องขอ DAR)
            // ตรวจสอบว่าผู้ใช้นี้มีสิทธิ์ใช้งานหน้า รายงานสถานะการดำเนินการ หรือไม่
            sql = "SELECT PermissionID FROM DC_PermissionUser WHERE UserID = " + Session["UserID"] + " AND PermissionID = 3";
            DataTable dt = query.SelectTable(sql);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}