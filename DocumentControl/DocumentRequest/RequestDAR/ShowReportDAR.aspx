<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowReportDAR.aspx.cs" Inherits="DocumentControl.DocumentRequest.RequestDAR.ShowReportDAR" Culture="en-US" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<!DOCTYPE html>

<html>
<head runat="server">
    <title></title>
    <%--<link href="../Content/bootstrap.css" rel="stylesheet" />
    <script src="../Scripts/bootstrap.bundle.js"></script>
    <script src="../Scripts/jquery-3.6.0.js"></script>--%>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager runat="server"></asp:ScriptManager>

            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" 
                InternalBorderColor="204, 204, 204" 
                InternalBorderStyle="Solid" 
                InternalBorderWidth="1px" 
                ToolBarItemBorderStyle="Solid" 
                ToolBarItemBorderWidth="1px" 
                ToolBarItemPressedBorderColor="51, 102, 153" 
                ToolBarItemPressedBorderStyle="Solid" 
                ToolBarItemPressedBorderWidth="1px"
                ToolBarItemPressedHoverBackColor="153, 187, 226"
                BackColor=""
                HighlightBackgroundColor="" 
                LinkActiveColor="" 
                LinkActiveHoverColor="" 
                LinkDisabledColor="" 
                PrimaryButtonBackgroundColor=""
                PrimaryButtonForegroundColor="" 
                PrimaryButtonHoverBackgroundColor="" 
                PrimaryButtonHoverForegroundColor="" 
                SecondaryButtonBackgroundColor="" 
                SecondaryButtonForegroundColor=""
                SecondaryButtonHoverBackgroundColor="" 
                SecondaryButtonHoverForegroundColor=""
                SplitterBackColor="" 
                ToolbarDividerColor=""
                ToolbarForegroundColor=""
                ToolbarForegroundDisabledColor="" 
                ToolbarHoverBackgroundColor="" 
                ToolbarHoverForegroundColor="" 
                ToolBarItemBorderColor="" 
                ToolBarItemHoverBackColor="">
            </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
