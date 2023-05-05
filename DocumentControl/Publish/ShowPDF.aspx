<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="~/Publish/ShowPDF.aspx.cs" Inherits="DocumentControl.Publish.ShowPDF" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery-3.6.0.js"></script>
    <script type="text/javascript" src="../Scripts/sweetalert2.js"></script>

    <link href="../Content/bootstrap.css" rel="stylesheet" />
    <script src="../Scripts/bootstrap.bundle.js"></script>

    <%-- Alert Custom --%>
    <script type="text/javascript" src="../Scripts/alert.js"></script>
</head>
<body oncontextmenu="return false" onkeydown="if ((arguments[0] || window.event).ctrlKey) return false" class="overflow-hidden">
    <form id="form1" runat="server"></form>
</body>
</html>
