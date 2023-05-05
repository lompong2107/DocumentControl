using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    string Name = Session["Name"].ToString();
                    LbName.Text = Name;
                    if (Session["DepartmentID"].ToString() == "1")
                    {
                        HLSetting.Visible = true;
                    }
                }
                else
                {
                    if (Request.QueryString["UserID"] != null)
                    {
                        string UserID = Request.QueryString["UserID"];
                        Response.Redirect("~/Login.aspx?UserID=" + UserID);
                    }
                    else
                    {
                        Response.Redirect("~/Login.aspx");
                    }
                }
            }
        }


        // --------------- Button
        protected void LinkBtnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Login.aspx");
        }
    }
}