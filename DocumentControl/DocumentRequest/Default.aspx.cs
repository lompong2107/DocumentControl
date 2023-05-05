using DocumentControl.DocumentRequest.RequestDAR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace DocumentControl.DocumentRequest
{
    public partial class Default : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                if (CheckPermission())
                {
                    // เช็คสิทธิ์รายการ LogBook
                    LiLogBook.Visible = true;
                }
            }
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