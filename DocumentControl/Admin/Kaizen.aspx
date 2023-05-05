<%@ Page Title="ผู้ดูแล | Document Control" Language="C#" MasterPageFile="~/Admin/Navbar.Master" AutoEventWireup="true" CodeBehind="Kaizen.aspx.cs" Inherits="DocumentControl.Admin.Kaizen" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .bg-warning {
            background-color: #FCF6BD !important;
        }

        .bg-success {
            background-color: #D0F4DE !important;
        }

        .bg-danger {
            background-color: #F898A4 !important;
        }

        .bg-info {
            background-color: #C0E4F6 !important;
        }

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
            <h5 class="fw-bold">คำร้องขอ Document Action Request</h5>
        </div>

        <asp:GridView ID="GVKaizen" runat="server" CssClass="table table-sm table-hover" AutoGenerateColumns="false"
            DataSourceID="SqlDataSourceKaizen" DataKeyNames="KaizenID"
            AllowPaging="true" PageSize="20"
            AllowSorting="true"
            EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true"
            OnRowDataBound="GVKaizen_RowDataBound" OnSelectedIndexChanged="GVKaizen_SelectedIndexChanged">
            <Columns>
                <asp:TemplateField HeaderText="ชื่อเรื่อง" SortExpression="KaizenTopic">
                    <ItemTemplate>
                        <%# Eval("KaizenTopic") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="หน่วยงาน" SortExpression="DepartmentName">
                    <ItemTemplate>
                        <%# Eval("DepartmentName") %>
                        <asp:HiddenField ID="HFKaizenID" runat="server" Value='<%# Eval("KaizenID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="วันที่สร้าง" SortExpression="DateCreate">
                    <ItemTemplate>
                        <%# DateTime.Parse(Eval("DateCreate").ToString()).ToString("dd/MM/yyyy") %>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="สถานะ" SortExpression="KaizenStatusDetail">
                    <ItemTemplate>
                        <asp:Panel ID="PanelStatus" runat="server" CssClass="rounded p-1">
                            <%# Eval("KaizenStatusDetail") + " " + Eval("Remark")  %>
                        </asp:Panel>
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

    <%-- SqlDataSource --%>
    <%-- Kaizen Report --%>
    <asp:SqlDataSource ID="SqlDataSourceKaizen" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT DC_Kaizen.KaizenID, DC_Kaizen.KaizenTopic, DC_Kaizen.DateCreate, F2_Department.DepartmentName, DC_KaizenStatus.KaizenStatusDetail, DC_Kaizen.KaizenStatusID, DC_Kaizen.Remark
        FROM DC_Kaizen
        LEFT JOIN DC_KaizenStatus ON DC_Kaizen.KaizenStatusID = DC_KaizenStatus.KaizenStatusID
        LEFT JOIN F2_Department ON DC_Kaizen.DepartmentID = F2_Department.DepartmentID"></asp:SqlDataSource>
</asp:Content>
