using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentControl.DocumentRequest.RequestDAR
{
    public partial class ShowReportDAR : System.Web.UI.Page
    {
        string sql = string.Empty;
        QuerySQL query = new QuerySQL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string RequestDARID = Request.QueryString["RequestDARID"];

                sql = @"SELECT DC_RequestDAR.RequestDARID, DC_RequestDAR.DateRequest, DC_RequestDARDocType.DocTypeName, DC_RequestDAR.DocTypeOther, DC_RequestDAROperation.OperationName, DC_RequestDAR.OperationOther
                , DC_LeaderAccept.UserID AS LeaderUserID, DC_LeaderAccept.AcceptStatus AS LeaderAcceptStatus, DC_NPDAccept.UserID AS NPDUserID, DC_NPDAccept.AcceptStatus AS NPDAcceptStatus,  DC_Approve.UserID AS ApproveUserID, DC_Approve.ApproveStatus, DC_RequestDAR.RequestDARStatusID
                , (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, (LeaderUser.FirstNameTH + ' ' + LeaderUser.LastNameTH) AS LeaderName, (NPDUser.FirstNameTH + ' ' + NPDUser.LastNameTH) AS NPDName, (ApproveUser.FirstNameTH + ' ' + ApproveUser.LastNameTH) AS ApproveName
                , DC_RequestDAR.Remark
                FROM DC_RequestDAR
                LEFT JOIN DC_RequestDAROperation ON DC_RequestDAR.RequestDAROperationID = DC_RequestDAROperation.RequestDAROperationID
                LEFT JOIN DC_RequestDARDocType ON DC_RequestDAR.RequestDARDocTypeID = DC_RequestDARDocType.RequestDARDocTypeID
                LEFT JOIN F2_Users ON DC_RequestDAR.UserID = F2_Users.UserID
                LEFT JOIN DC_LeaderAccept ON DC_RequestDAR.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN F2_Users AS LeaderUser ON DC_LeaderAccept.UserID = LeaderUser.UserID
                LEFT JOIN DC_NPDAccept ON DC_RequestDAR.NPDAcceptID = DC_NPDAccept.NPDAcceptID
                LEFT JOIN F2_Users AS NPDUser ON DC_NPDAccept.UserID = NPDUser.UserID
                LEFT JOIN DC_Approve ON DC_RequestDAR.ApproveID = DC_Approve.ApproveID
                LEFT JOIN F2_Users AS ApproveUser ON DC_Approve.UserID = ApproveUser.UserID
                WHERE DC_RequestDAR.RequestDARID = " + RequestDARID;
                string DateRequest = DateTime.Parse(query.SelectAt(1, sql)).ToString("dd/MM/yyyy");
                string DocType = query.SelectAt(2, sql) + query.SelectAt(3, sql);
                string Operation = query.SelectAt(4, sql) + query.SelectAt(5, sql);
                string RequestName = query.SelectAt(13, sql);
                string LeaderName = query.SelectAt(14, sql);
                string NPDName = query.SelectAt(15, sql);
                string ApproveName = query.SelectAt(16, sql);
                string Remark = query.SelectAt(17, sql);
                sql = "SELECT RequestDARDocID, DocNumber, DocName, DateEnforce, Remark FROM DC_RequestDARDoc WHERE RequestDARID = " + RequestDARID;
                ReportViewer1.LocalReport.ReportPath = "~DocumentRequest/ReportDAR.rdlc";
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("RequestDARDoc", query.SelectTable(sql)));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("RequestDARID", RequestDARID));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("DateRequest", DateRequest));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("DocType", DocType));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("Operation", Operation));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("UserRequest", RequestName));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("UserLeader", LeaderName));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("UserNPD", NPDName));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("UserApprove", ApproveName));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("UserDateRequest", DateRequest));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("UserDateLeader", DateRequest));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("UserDateNPD", DateRequest));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("UserDateApprove", DateRequest));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("Remark", Remark));
                ReportViewer1.LocalReport.Refresh();
            }
        }
    }
}