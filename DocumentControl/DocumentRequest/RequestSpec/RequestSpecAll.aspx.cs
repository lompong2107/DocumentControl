using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest.RequestSpec
{
    public partial class RequestSpecAll : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LastPage"] = "~/DocumentRequest/RequestSpec/RequestSpecAll.aspx";
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    GVRequestSpec.Sort("RequestSpecID", SortDirection.Descending);
                    CBShowAll_CheckedChanged(null, null);
                    DDListPaging_SelectedIndexChanged(null, null);
                }
            }
            else
            {
                if (ViewState["FilterGVRequestSpec"] != null)
                {
                    SqlDataSourceRequestSpec.FilterExpression = ViewState["FilterGVRequestSpec"].ToString();
                }
            }
        }

        // --------------- GridView
        protected void GVRequestSpec_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GVRequestSpec, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["style"] = "cursor:pointer";

                string StatusID = DataBinder.Eval(e.Row.DataItem, "RequestSpecStatusID").ToString();
                Panel PanelStatus = e.Row.FindControl("PanelStatus") as Panel;
                if (StatusID == "0")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-secondary";
                }
                else if (StatusID == "3" || StatusID == "6")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-danger";
                }
                else if (StatusID == "7")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-info";
                }
                else if (StatusID == "8")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-success";
                }
                else
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-warning";
                }
            }
        }
        protected void GVRequestSpec_SelectedIndexChanged(object sender, EventArgs e)
        {
            string RequestSpecID = GVRequestSpec.DataKeys[GVRequestSpec.SelectedIndex].Values[0].ToString();
            Response.Redirect("RequestSpecDetail.aspx?RequestSpecID=" + RequestSpecID);
        }


        // CheckBox
        // แสดงรายการทั้งหมด
        protected void CBShowAll_CheckedChanged(object sender, EventArgs e)
        {
            if (!CBShowAll.Checked)
            {
                // ไม่แสดงสถานะ ยกเลิกคำร้องขอ/ไม่อนุมัติ/เสร็จสมบูรณ์
                SqlDataSourceRequestSpec.FilterExpression = "(RequestSpecStatusID <> 8 AND RequestSpecStatusID <> 6 AND RequestSpecStatusID <> 3 AND RequestSpecStatusID <> 0)";
            }
            else
            {
                SqlDataSourceRequestSpec.FilterExpression = null;
            }
            ViewState.Add("FilterGVRequestSpec", SqlDataSourceRequestSpec.FilterExpression);
        }


        // DropDownList
        // จำนวนแถวที่แสดงใน GridView
        protected void DDListPaging_SelectedIndexChanged(object sender, EventArgs e)
        {
            GVRequestSpec.PageSize = int.Parse(DDListPaging.SelectedValue);
            GVRequestSpec.DataBind();
        }
    }
}