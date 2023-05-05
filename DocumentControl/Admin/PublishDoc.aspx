<%@ Page Title="ผู้ดูแล | Document Control" Language="C#" MasterPageFile="~/Admin/Navbar.Master" AutoEventWireup="true" CodeBehind="PublishDoc.aspx.cs" Inherits="DocumentControl.Admin.PublishDoc" Culture="en-US" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .sortasc a:after {
            margin-left: 10px;
            display: inline-block;
            content: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" height="15"><path d="M207.029 381.476L12.686 187.132c-9.373-9.373-9.373-24.569 0-33.941l22.667-22.667c9.357-9.357 24.522-9.375 33.901-.04L224 284.505l154.745-154.021c9.379-9.335 24.544-9.317 33.901.04l22.667 22.667c9.373 9.373 9.373 24.569 0 33.941L240.971 381.476c-9.373 9.372-24.569 9.372-33.942 0z"/></svg>');
            transform: rotate(180deg);
        }

        .sortdesc a:after {
            margin-left: 10px;
            display: inline-block;
            content: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" height="15"><path d="M207.029 381.476L12.686 187.132c-9.373-9.373-9.373-24.569 0-33.941l22.667-22.667c9.357-9.357 24.522-9.375 33.901-.04L224 284.505l154.745-154.021c9.379-9.335 24.544-9.317 33.901.04l22.667 22.667c9.373 9.373 9.373 24.569 0 33.941L240.971 381.476c-9.373 9.372-24.569 9.372-33.942 0z"/></svg>');
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container pt-3 pb-2">
        <div class="text-center">
            <span class="h5">ไฟล์เอกสารแจกจ่าย เผยแพร่</span>
        </div>

        <div class="mb-3 rounded p-2 bg-white">
            <p class="fw-bold text-black">เอกสารที่หัวข้อ</p>
            <asp:GridView ID="GVPublishTopicFile" runat="server" CssClass="table table-hover table-sm border-0" AutoGenerateColumns="false"
                DataSourceID="SqlDataSourcePublishTopicFile"
                AllowPaging="true" PageIndex="10"
                AllowSorting="true">
                <Columns>
                    <asp:TemplateField HeaderText="ชื่อเอกสาร" SortExpression="TopicName">
                        <ItemTemplate>
                            <%# Eval("TopicName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วันที่อัปโหลด" SortExpression="PublishDate">
                        <ItemTemplate>
                            <%# DateTime.Parse(Eval("PublishDate").ToString()).ToString("dd/MM/yyyy HH:mm:ss") %>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="table-danger" HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="Black" />
                <RowStyle BackColor="White" />
                <PagerStyle HorizontalAlign="Center" BackColor="White" />
                <EmptyDataRowStyle HorizontalAlign="Center" BackColor="White" />
                <SortedAscendingHeaderStyle CssClass="sortasc" />
                <SortedDescendingHeaderStyle CssClass="sortdesc" />
            </asp:GridView>
        </div>

        <div class="rounded bg-white p-2">
            <p class="fw-bold text-black">เอกสารในหัวข้อ</p>
            <asp:GridView ID="GVPublishDocFile" runat="server" CssClass="table table-hover table-sm border-0" AutoGenerateColumns="false"
                DataSourceID="SqlDataSourcePublishDocFile"
                AllowPaging="true" PageIndex="10"
                AllowSorting="true">
                <Columns>
                    <asp:TemplateField HeaderText="ชื่อเอกสาร" SortExpression="FileName">
                        <ItemTemplate>
                            <%# Eval("FileName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วันที่อัปโหลด" SortExpression="PublishDate">
                        <ItemTemplate>
                            <%# DateTime.Parse(Eval("PublishDate").ToString()).ToString("dd/MM/yyyy HH:mm:ss") %>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="table-danger" HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="Black" />
                <RowStyle BackColor="White" />
                <PagerStyle HorizontalAlign="Center" BackColor="White" />
                <EmptyDataRowStyle HorizontalAlign="Center" BackColor="White" />
                <SortedAscendingHeaderStyle CssClass="sortasc" />
                <SortedDescendingHeaderStyle CssClass="sortdesc" />
            </asp:GridView>
        </div>
    </div>

    <%-- SqlDataSource --%>
    <%-- เอกสารที่หัวข้อ --%>
    <asp:SqlDataSource ID="SqlDataSourcePublishTopicFile" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT DC_PublishTopic.TopicName, DC_PublishTopicFile.PublishTopicFileID, DC_PublishTopicFile.PublishDate
        FROM DC_PublishTopicFile LEFT JOIN DC_PublishTopic ON DC_PublishTopicFile.PublishTopicID = DC_PublishTopic.PublishTopicID"></asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourcePublishDocFile" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT DC_PublishDocFile.PublishDocFileID, DC_PublishDoc.FileName, DC_PublishDocFile.PublishDate
                FROM DC_PublishDocFile LEFT JOIN DC_PublishDoc ON DC_PublishDocFile.PublishDocID = DC_PublishDoc.PublishDocID"></asp:SqlDataSource>
</asp:Content>
