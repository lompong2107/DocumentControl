<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="~/Publish/PublishHistoryFile.aspx.cs" Inherits="DocumentControl.Publish.PublishHistoryFile" Culture="en-US" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>ประวัติไฟล์ | Document Control</title>
    <link rel="icon" href="../Image/docs.png" />
    <link href="../Content/bootstrap.css" rel="stylesheet" />
    <script src="../Scripts/bootstrap.bundle.js"></script>
    <script src="../Scripts/jquery-3.6.0.js"></script>
    <script type="text/javascript" src="../Scripts/sweetalert2.js"></script>

    <%-- Alert Custom --%>
    <script type="text/javascript" src="../Scripts/alert.js"></script>

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
            src: url('../Fonts/Sarabun-Regular.ttf') format('truetype');
        }

        * {
            font-family: 'Sarabun', sans-serif;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:GridView ID="GVPublishTopicFile" runat="server" CssClass="w-100 table table-hover border-0" AutoGenerateColumns="false" OnRowCommand="GVPublishTopicFile_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="ชื่อเอกสาร">
                        <ItemTemplate>
                            <div class="d-flex flex-column">
                                <asp:LinkButton ID="LinkBtnPublishTopic" runat="server" OnClientClick='<%# string.Format("window.open(\"ShowPDF.aspx?PublishTopicFileID={0}\", null, \"height=500,width=800,status=yes,toolbar=yes,menubar=yes,location=no\");", Eval("PublishTopicFileID")) %>' Text='<%# Eval("TopicName") %>'></asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="นามสกุลไฟล์">
                        <ItemTemplate>
                            <span><%# Eval("FileExtension") %></span>
                        </ItemTemplate>
                        <HeaderStyle CssClass="text-center" />
                        <ItemStyle CssClass="text-center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="อัปโหลดโดย">
                        <ItemTemplate>
                            <span><%# Eval("Name") %></span>
                        </ItemTemplate>
                        <HeaderStyle CssClass="text-center" />
                        <ItemStyle CssClass="text-center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วันที่อัปโหลด">
                        <ItemTemplate>
                            <%# Eval("PublishDate") %>
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
            <asp:GridView ID="GVPublishDocFile" runat="server" CssClass="w-100 table table-hover border-0" AutoGenerateColumns="false" OnRowCommand="GVPublishDocFile_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="ชื่อเอกสาร">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkBtnPublishDoc" runat="server" CommandName='LinkBtnPublishDoc' CommandArgument='<%# Eval("PublishDocFileID") %>' Text='<%# Eval("FileName") %>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Revision">
                        <ItemTemplate>
                            <%# Eval("Revision") %>
                        </ItemTemplate>
                        <HeaderStyle CssClass="text-center" />
                        <ItemStyle CssClass="text-center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="นามสกุลไฟล์">
                        <ItemTemplate>
                            <span><%# Eval("FileExtension") %></span>
                        </ItemTemplate>
                        <HeaderStyle CssClass="text-center" />
                        <ItemStyle CssClass="text-center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="อัปโหลดโดย">
                        <ItemTemplate>
                            <span><%# Eval("Name") %></span>
                        </ItemTemplate>
                        <HeaderStyle CssClass="text-center" />
                        <ItemStyle CssClass="text-center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วันที่อัปโหลด">
                        <ItemTemplate>
                            <%# Eval("PublishDate") %>
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
