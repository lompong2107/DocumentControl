using DocumentControl.Admin;
using DocumentControl.DocumentRequest.RequestDAR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest.KaizenReport
{
    public partial class KaizenReport : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LastPage"] = "~/DocumentRequest/KaizenReport/KaizenReport.aspx";
            if (!Page.IsPostBack)
            {

                DateTime Today = DateTime.Today;
                // ไตรมาส
                int Month = Today.Month;
                if (Month >= 1 && Month <= 3)
                {
                    DDListQuarter.SelectedValue = "1";
                }
                else if (Month >= 4 && Month <= 6)
                {
                    DDListQuarter.SelectedValue = "2";
                }
                else if (Month >= 7 && Month <= 9)
                {
                    DDListQuarter.SelectedValue = "3";
                }
                else if (Month >= 10 && Month <= 12)
                {
                    DDListQuarter.SelectedValue = "4";
                }

                if (Session["UserID"] != null)
                {
                    TxtYear.Text = Today.Year.ToString();
                    LoadDDListDepartment();
                    LoadDDListStatus();
                    GVKaizen.Sort("DepartmentName", SortDirection.Descending);                    
                    DDListPagingKaizen_SelectedIndexChanged(null, null);
                }
            }
        }


        // --------------- Function
        private void LoadDDListDepartment()
        {
            sql = "SELECT DepartmentID, DepartmentName FROM F2_Department WHERE Showstatus = 1";
            DDListDepartment.DataSource = query.SelectTable(sql);
            DDListDepartment.DataBind();
        }
        private void LoadDDListStatus()
        {
            sql = "SELECT KaizenStatusID, KaizenStatusDetail FROM DC_KaizenStatus WHERE KaizenStatusID != 0;";
            DDListStatus.DataSource = query.SelectTable(sql);
            DDListStatus.DataBind();
        }


        // --------------- GridView
        protected void GVKaizen_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;
            }

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


        // DropDownList
        // สถานะ Kaizen
        protected void DDListStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            GVKaizen.DataBind();
        }
        protected void DDListStatus_DataBound(object sender, EventArgs e)
        {
            DDListStatus.Items.Insert(0, new ListItem("แสดงทั้งหมด", "%"));
            DDListStatus.SelectedIndex = 0;
        }
        // จำนวนแถวที่แสดงใน GridView
        protected void DDListPagingKaizen_SelectedIndexChanged(object sender, EventArgs e)
        {
            GVKaizen.PageSize = int.Parse(DDListPagingKaizen.SelectedValue);
            GVKaizen.DataBind();
        }
        // หน่วยงาน
        protected void DDListDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            GVKaizen.DataBind();
        }

        protected void DDListDepartment_DataBound(object sender, EventArgs e)
        {
            DDListDepartment.Items.Insert(0, new ListItem("แสดงทั้งหมด", "%"));
            DDListDepartment.SelectedIndex = 0;
        }


        // Button
        // ปุ่มแสดง
        protected void BtnShow_Click(object sender, EventArgs e)
        {
            GVKaizen.DataBind();
        }
        protected void btnInvisibleSearch_Click(object sender, EventArgs e)
        {
            GVKaizen.DataBind();
        }
    }
}