using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Admin
{
    public partial class Kaizen : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        // --------------- Gridview
        protected void GVKaizen_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GVKaizen, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["style"] = "cursor:pointer";

                string StatusID = DataBinder.Eval(e.Row.DataItem, "KaizenStatusID").ToString();
                Panel PanelStatus = e.Row.FindControl("PanelStatus") as Panel;
                if (StatusID == "3")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-danger";
                }
                else if (StatusID == "4")
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-success";
                }
                else
                {
                    PanelStatus.CssClass = PanelStatus.CssClass + " bg-warning";
                }
            }
        }
        protected void GVKaizen_SelectedIndexChanged(object sender, EventArgs e)
        {
            string KaizenID = GVKaizen.DataKeys[GVKaizen.SelectedIndex].Values[0].ToString();
            Response.Redirect("KaizenDetail.aspx?KaizenID=" + KaizenID);
        }
    }
}