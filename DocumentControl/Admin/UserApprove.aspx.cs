using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.Admin
{
    public partial class UserApprove : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDepartment();
                LoadUser();
                RBListApproveDAR_SelectedIndexChanged(null, null);
            }
        }


        // --------------- Function
        // โหลดข้อมูลแผนก
        private void LoadDepartment()
        {
            sql = "SELECT DepartmentID, DepartmentName FROM F2_Department WHERE Showstatus = 1 ORDER BY DepartmentName";
            DDListDepartment.DataSource = query.SelectTable(sql);
            DDListDepartment.DataBind();
            DDListDepartment.SelectedIndex = 0;
        }
        // โหลดข้อมูลชื่อผู้ใช้
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
        // โหลดข้อมูลชื่อผู้ช้ที่อยู่ในสิทธิ์ที่เลือก
        private void LoadApprove()
        {
            string ApproveStatus = RBListApproveDAR.SelectedValue + RBListApproveSpec.SelectedValue;
            sql = @"SELECT DC_UserApprove.UserID ,(F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH + ' ' + ISNULL(DC_UserApprove.Remark, '')) AS Name
                FROM DC_UserApprove 
                LEFT JOIN F2_Users ON DC_UserApprove.UserID = F2_Users.UserID
                WHERE DC_UserApprove.StatusPermission = " + ApproveStatus;
            ListBoxApprove.DataSource = query.SelectTable(sql);
            ListBoxApprove.DataBind();
            ListBoxApprove_SelectedIndexChanged(null, null);
        }


        // --------------- DropDownList
        // เลือกแผนก
        protected void DDListDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadUser();
            ListBoxApprove_SelectedIndexChanged(null, null);
        }
        // เลือกสิทธิ์ DAR
        protected void RBListApproveDAR_SelectedIndexChanged(object sender, EventArgs e)
        {
            RBListApproveSpec.ClearSelection();
            NameApprove.Text = " " + RBListApproveDAR.SelectedItem.Text;
            LoadApprove();
            DDListUser_SelectedIndexChanged(null, null);
        }
        // เลือกสิทธิ์ Spec
        protected void RBListApproveSpec_SelectedIndexChanged(object sender, EventArgs e)
        {
            RBListApproveDAR.ClearSelection();
            NameApprove.Text = " " + RBListApproveSpec.SelectedItem.Text;
            LoadApprove();
            DDListUser_SelectedIndexChanged(null, null);
        }
        // เลือกผู้ใช้
        protected void DDListUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadApprove();
            ListBoxApprove_SelectedIndexChanged(null, null);
            string UserIDSelected = DDListUser.SelectedValue;
            if (ListBoxApprove.Items.FindByValue(UserIDSelected) != null)
            {
                BtnAdd.Enabled = false;
            }
            else
            {
                BtnAdd.Enabled = true;
            }
        }


        // --------------- Button
        // เพิ่มผู้ใช้
        protected void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string UserID = DDListUser.SelectedValue;
                string Status = RBListApproveDAR.SelectedValue + RBListApproveSpec.SelectedValue;
                string Remark = TxtRemark.Text;
                sql = "INSERT INTO DC_UserApprove (UserID, StatusPermission, Remark) VALUES (" + UserID + ", " + Status + ", '" + Remark + "')";
                if (query.Excute(sql))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alertNotification('สำเร็จ!', 'เพิ่มสิทธิ์การอนุมัติสำเร็จ', 'success');", true);
                    DDListUser_SelectedIndexChanged(null, null);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", $"alertNotification(\"ล้มเหลว!\", `{ex.Message}`, \"error\");", true);
            }
        }
        // ลบผู้ใช้
        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string UserID = ListBoxApprove.SelectedValue;
                string Status = RBListApproveDAR.SelectedValue + RBListApproveSpec.SelectedValue;
                sql = "DELETE DC_UserApprove WHERE UserID = " + UserID + " AND StatusPermission = " + Status;
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


        // --------------- ListBox
        // เลือกรายการผู้อนุมัติ
        protected void ListBoxApprove_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ListBoxApprove.SelectedValue))
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