using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Admin
{
    public partial class Navbar : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //Session["UserID"] = 2305;
                //Session["Name"] = "Lompong Dev";
                //Session["DepartmentID"] = 1;

                if (Session["UserID"] != null)
                {
                    string Name = Session["Name"].ToString();
                    LbName.Text = Name;
                    if (Session["DepartmentID"].ToString() != "1")
                    {
                        Response.Redirect("~/Login.aspx");
                    }
                }
                else
                {
                    Response.Redirect("~/Login.aspx");
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