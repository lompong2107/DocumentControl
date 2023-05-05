using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl
{
    public partial class Login : System.Web.UI.Page
    {
        string sql;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // เช็คว่ามีการส่ง UserIDมาหรือไม่
                if (Request.QueryString["UserID"] != null)
                {
                    string UserID = Request.QueryString["UserID"];
                    sql = "SELECT UserID, (FIrstNameTh + ' ' + LastNameTh) As Name, DepartmentID FROM F2_Users WHERE UserID = " + UserID;
                    string Name = query.SelectAt(1, sql);
                    string DepartmentID = query.SelectAt(2, sql);

                    Session["UserID"] = UserID;
                    Session["Name"] = Name;
                    Session["DepartmentID"] = DepartmentID;

                    if (Session["LastPage"] != null)
                    {
                        Response.Redirect(Session["LastPage"].ToString());
                    }
                    else
                    {
                        Response.Redirect("Default.aspx");
                    }
                }
            }

            // เช็คว่ามีการ Login หรือยัง
            if (Session["UserID"] != null && Session["LastPage"] != null)
            {
                Response.Redirect(Session["LastPage"].ToString());
            }
        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = TxtUser.Text.Replace("'", "");
            string password = TxtPassword.Text.Replace("'", "");
            if (username.Length > 0 && password.Length > 0)
            {
                sql = "SELECT UserID, (FIrstNameTh + ' ' + LastNameTh) As Name, DepartmentID FROM F2_Users WHERE Username = '" + username + "' AND Password = '" + password + "' AND Status = 1";
                if (query.CheckRow(sql))
                {
                    string UserID = query.SelectAt(0, sql);
                    string Name = query.SelectAt(1, sql);
                    string DepartmentID = query.SelectAt(2, sql);

                    Session["UserID"] = UserID;
                    Session["Name"] = Name;
                    Session["DepartmentID"] = DepartmentID;

                    if (Session["LastPage"] != null)
                    {
                        Response.Redirect(Session["LastPage"].ToString());
                    }
                    else
                    {
                        Response.Redirect("Default.aspx");
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง', 'error');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('ล้มเหลว!', 'กรุณากรอกข้อมูลให้ครบ', 'warning');", true);
            }
        }
    }
}