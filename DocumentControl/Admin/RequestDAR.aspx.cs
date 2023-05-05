using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Admin
{
    public partial class RequestDAR : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        // --------------- GridView
        protected void GVRequestDAR_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GVRequestDAR, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["style"] = "cursor:pointer";

                string StatusID = DataBinder.Eval(e.Row.DataItem, "RequestDARStatusID").ToString();
                Panel PanelStatus = e.Row.FindControl("PanelStatus") as Panel;
                if (StatusID == "0")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-secondary";
                }
                else if (StatusID == "4")
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
            GridViewRow SelectedRow = GVRequestDAR.SelectedRow;
            string RequestDARID = (SelectedRow.Cells[0].FindControl("HFRequestDARID") as HiddenField).Value;
            Response.Redirect("RequestDARDetail.aspx?RequestDARID=" + RequestDARID);
        }
    }
}