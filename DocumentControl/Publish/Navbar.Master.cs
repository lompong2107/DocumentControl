using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Publish
{
    public partial class Navbar : System.Web.UI.MasterPage
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["UserID"] = 2305;
            //Session["Name"] = "Lompong Dev";
            //Session["DepartmentID"] = 1;

            if (Session["UserID"] != null)
            {
                if (!Page.IsPostBack)
                {
                    string UserID = Session["UserID"].ToString();
                    // ตรวจสอบว่ามีสิทธิ์ 1 Full Control, 2 = Ownership หรือไม่
                    sql = $@"SELECT DC_PermissionUser.PermissionUserID, DC_Permission.PermissionDetail 
                    FROM DC_PermissionUser 
                    LEFT JOIN DC_Permission ON DC_PermissionUser.PermissionID = DC_Permission.PermissionID
                    WHERE DC_PermissionUser.UserID = {UserID} AND DC_PermissionUser.PermissionID IN (1, 2)";
                    if (query.CheckRow(sql))
                    {
                        string PermissionDetail = query.SelectAt(1, sql);
                        LbPermissionDetail.Text = PermissionDetail;
                    }

                    string Name = Session["Name"].ToString();
                    LbName.Text = Name;
                }
                if (Session["DepartmentID"].ToString() == "1")
                {
                    HLSetting.Visible = true;
                }
            }
            else
            {
                if (IsPostBack)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotificationAndRedirect('Session หมดเวลาแล้ว!', 'กรุณาเข้าสู่ระบบ.', 'info', '../Login.aspx');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotificationAndRedirect('ล้มเหลว!', 'กรุณาเข้าสู่ระบบ.', 'info', '../Login.aspx');", true);
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