<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplayDoc.aspx.cs" Inherits="DocumentControl.Publish.DisplayDoc" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>แสดงรายการไฟล์ | Document Control</title>
    <!-- Required meta tags -->
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <link rel="icon" href="../Image/docs.png" />
    <link href="../Content/bootstrap.min.css" rel="stylesheet" />
    <script src="../Scripts/bootstrap.bundle.min.js"></script>
    <script src="../Scripts/jquery-3.6.0.js"></script>
    <script type="text/javascript" src="../Scripts/sweetalert2.js"></script>
    <%-- Alert Custom --%>
    <script type="text/javascript" src="../Scripts/alert.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%= TxtSearch.ClientID %>").keyup(function () {
                $("#<%= btnInvisibleSearch.ClientID %>").click();
            })
        })
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
        <div class="container py-3">
            <dvi class="d-flex justify-content-center">
                <asp:Label ID="LbTopic" runat="server" CssClass="h3"></asp:Label>
            </dvi>
            <div class="mb-2">
                <asp:TextBox ID="TxtSearch" runat="server" CssClass="form-control" placeholder="ค้นหา..."></asp:TextBox>
            </div>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div>
                        <%-- ค้นหา --%>
                        <asp:Button ID="btnInvisibleSearch" runat="server" Style="display: none" OnClick="btnInvisibleSearch_Click" />
                        <%-- ตาราง --%>
                        <asp:GridView ID="GVPublishDoc" runat="server" CssClass="table table-striped table-borderless border-0" AutoGenerateColumns="false">
                            <Columns>
                                <asp:TemplateField HeaderText="ชื่อเอกสาร">
                                    <ItemTemplate>
                                        <%--<asp:LinkButton ID="LinkBtnTopic" runat="server" CommandName='LinkBtnTopic' CommandArgument='<%# Eval("PublishDocFileID") %>' Text='<%# Eval("FileName") %>'></asp:LinkButton>--%>
                                        <a href='ShowPDF.aspx?PublishDocFileID=<%# Eval("PublishDocFileID") %>' target="_blank" class="h5 link-primary"><%# Eval("FileName") %></a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Revision">
                                    <ItemTemplate>
                                        <span><%# Eval("Revision") %></span>
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
                            </Columns>
                            <EmptyDataTemplate>
                                ไม่พบข้อมูล
                            </EmptyDataTemplate>
                            <HeaderStyle CssClass="border-bottom" VerticalAlign="Middle" />
                            <RowStyle CssClass="border-bottom border-light" VerticalAlign="Middle" />
                            <EmptyDataRowStyle HorizontalAlign="Center" />
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>
