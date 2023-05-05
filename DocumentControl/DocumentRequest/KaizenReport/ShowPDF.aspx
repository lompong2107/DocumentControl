<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowPDF.aspx.cs" Inherits="DocumentControl.DocumentRequest.KaizenReport.ShowPDF" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title></title>
    <script type="text/javascript" src='<%= ResolveClientUrl("~/Scripts/jquery-3.6.0.js") %>'></script>
    <script type="text/javascript" src='<%= ResolveClientUrl("~/Scripts/sweetalert2.js") %>'></script>

    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <script src='<%= ResolveClientUrl("~/Scripts/bootstrap.bundle.min.js") %>'></script>

    <%-- Alert Custom --%>
    <script type="text/javascript" src='<%= ResolveClientUrl("~/Scripts/alert.js") %>'></script>
</head>
<body oncontextmenu="return false" onkeydown="if ((arguments[0] || window.event).ctrlKey) return false" class="overflow-hidden">
    <form id="form1" runat="server"></form>
</body>
</html>
