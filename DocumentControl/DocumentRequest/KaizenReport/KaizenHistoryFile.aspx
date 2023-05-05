<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KaizenHistoryFile.aspx.cs" Inherits="DocumentControl.DocumentRequest.KaizenReport.KaizenHistoryFile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ประวัติไฟล์ | Document Control</title>
    <link rel="icon" href="~/Image/docs.png" />
    <link href="~/Content/bootstrap.css" rel="stylesheet" />
    <script src='<%= ResolveClientUrl("~/Scripts/bootstrap.bundle.js") %>'></script>
    <script src='<%= ResolveClientUrl("~/Scripts/jquery-3.6.0.js") %>'></script>
    <script type="text/javascript" src='<%= ResolveClientUrl("~/Scripts/sweetalert2.js") %>'></script>

    <%-- Alert Custom --%>
    <script type="text/javascript" src='<%= ResolveClientUrl("~/Scripts/alert.js") %>'></script>

    <script type="text/javascript">
        function CopyToClipboard(link) {
            //navigator.clipboard.writeText(link).then(function () {
            //    alertToast("คัดลอกลิงก์สำเร็จ", "success");
            //})

            // https://stackoverflow.com/questions/46445981/copy-element-text-to-clipboard-aspx
            var $temp = $("<input>");
            $("body").append($temp);
            $temp.val(link).select();
            document.execCommand("copy");
            $temp.remove();
            alertToast("คัดลอกลิงก์สำเร็จ", "success");
        }
    </script>

    <style>
        @font-face {
            font-family: 'Sarabun';
            src: url('<%= ResolveClientUrl("~/Fonts/Sarabun-Regular.ttf") %>') format('truetype');
        }

        * {
            font-family: 'Sarabun', sans-serif;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:GridView ID="GVKaizenDoc" runat="server" CssClass="w-100 table table-hover border-0" AutoGenerateColumns="false" OnRowCommand="GVKaizenDoc_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="วันที่อัปโหลด">
                        <ItemTemplate>
                            <a href='<%# ResolveClientUrl("~/DocumentRequest/KaizenReport/ShowPDF.aspx?KaizenDocID=" + Eval("KaizenDocID")) %>' target="_blank"><%# Eval("DateCreate") %></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="คำสั่ง">
                        <ItemTemplate>
                            <asp:ImageButton ID="BtnDownload" runat="server" CommandName="BtnDownload" CommandArgument='<%# Eval("FilePath") %>' Height="30" ImageUrl="~/Image/download-file.png" CssClass="align-middle" ToolTip="ดาวน์โหลด" />
                            <asp:ImageButton ID="BtnLink" runat="server" CommandName="BtnLink" CommandArgument='<%# Eval("FilePath") %>' Height="30" ImageUrl="~/Image/link-file.png" CssClass="align-middle" ToolTip="คัดลอกลิงก์" OnClientClick='<%# string.Format("CopyToClipboard(\"{0}\"); return false;", Eval("FilePath").ToString().Replace("\\", "\\\\")) %>' />                                                     
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
