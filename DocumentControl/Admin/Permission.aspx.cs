using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Admin
{
    public partial class Permission : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPermission();
                LoadDepartment();
                LoadUser();
                RBListPermissionPublish_SelectedIndexChanged(null, null);
            }
        }


        // --------------- Function
        // โหลดรายการสิทธิ์
        private void LoadPermission()
        {
            sql = "SELECT PermissionID, PermissionDetail FROM DC_Permission WHERE Status = 1 AND PermissionID IN (1, 2)";
            RBListPermissionPublish.DataSource = query.SelectTable(sql);
            RBListPermissionPublish.DataBind();
            RBListPermissionPublish.SelectedValue = "1";

            sql = "SELECT PermissionID, PermissionDetail FROM DC_Permission WHERE Status = 1 AND PermissionID IN (3, 4)";
            RBListPermissionDAR.DataSource = query.SelectTable(sql);
            RBListPermissionDAR.DataBind();
        }
        // โหลดแผนก
        private void LoadDepartment()
        {
            sql = "SELECT DepartmentID, DepartmentName FROM F2_Department WHERE Showstatus = 1 ORDER BY DepartmentName";
            DDListDepartment.DataSource = query.SelectTable(sql);
            DDListDepartment.DataBind();
            DDListDepartment.SelectedIndex = 0;
        }
        // โหลดผู้ใช้
        private void LoadUser()
        {
            sql = "SELECT UserID, (FirstNameTH + ' ' + LastNameTH) AS Name FROM F2_Users WHERE Status = 1 AND DepartmentID = " + DDListDepartment.SelectedValue + " ORDER BY FirstNameTH";
            DDListUser.DataSource = query.SelectTable(sql);
            DDListUser.DataBind();
            if (DDListUser.Items.Count > 0)
            {
                DDListUser.SelectedIndex = 0;
            }
            DDListUser_SelectedIndexChanged(null, null);
        }
        // โหลดผู้ใช้ที่มีในสิทธิ์ที่เลือก
        private void LoadPermissionUser()
        {
            string PermissionID = RBListPermissionDAR.SelectedValue + RBListPermissionPublish.SelectedValue;
            sql = @"SELECT DC_PermissionUser.UserID ,(F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name
                FROM DC_PermissionUser 
                LEFT JOIN F2_Users ON DC_PermissionUser.UserID = F2_Users.UserID
                WHERE DC_PermissionUser.PermissionID = " + PermissionID;
            ListBoxPermissionUser.DataSource = query.SelectTable(sql);
            ListBoxPermissionUser.DataBind();
            ListBoxPermissionUser_SelectedIndexChanged(null, null);
        }


        // --------------- DropDownList
        // เลือกแผนก
        protected void DDListDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadUser();
        }
        // เลือกผู้ใช้
        protected void DDListUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPermissionUser();
            ListBoxPermissionUser_SelectedIndexChanged(null, null);
            string UserIDSelected = DDListUser.SelectedValue;
            if (ListBoxPermissionUser.Items.FindByValue(UserIDSelected) != null)
            {
                BtnAdd.Enabled = false;
            }
            else
            {
                BtnAdd.Enabled = true;
            }
        }


        // --------------- Button
        // ปุ่มเพิ่มสิทธิ์
        protected void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string UserID = DDListUser.SelectedValue;
                string PermissionID = RBListPermissionDAR.SelectedValue + RBListPermissionPublish.SelectedValue;
                sql = $"INSERT INTO DC_PermissionUser (PermissionID, UserID) VALUES ({PermissionID},{UserID})";
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertToast('สำเร็จ!', 'success');", true);
                    DDListUser_SelectedIndexChanged(null, null);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
        // ปุ่มลบผู้ใช้ออกจากสิทธิ์
        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string UserID = ListBoxPermissionUser.SelectedValue;
                string PermissionID = RBListPermissionDAR.SelectedValue + RBListPermissionPublish.SelectedValue;
                sql = $"DELETE DC_PermissionUser WHERE UserID = {UserID} AND PermissionID = {PermissionID}";
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'ลบสิทธิ์การใช้งานสำเร็จ', 'success');", true);
                    LoadPermissionUser();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }


        // --------------- RadioButton
        // Request Action Request
        protected void RBListPermissionDAR_SelectedIndexChanged(object sender, EventArgs e)
        {
            RBListPermissionPublish.ClearSelection();
            NameApprove.Text = " " + RBListPermissionDAR.SelectedItem.Text;
            LoadPermissionUser();
            DDListUser_SelectedIndexChanged(null, null);
        }
        // Publish
        protected void RBListPermissionPublish_SelectedIndexChanged(object sender, EventArgs e)
        {
            RBListPermissionDAR.ClearSelection();
            NameApprove.Text = " " + RBListPermissionPublish.SelectedItem.Text;
            LoadPermissionUser();
            DDListUser_SelectedIndexChanged(null, null);
        }


        // -------------- ListBox
        // รายการผู้ใช้ในสิทธิ์
        protected void ListBoxPermissionUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ListBoxPermissionUser.SelectedValue))
            {
                BtnDelete.Enabled = true;
            }
            else
            {
                BtnDelete.Enabled = false;
            }
        }
    }
}