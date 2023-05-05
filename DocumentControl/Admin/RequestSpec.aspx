<%@ Page Title="ผู้ดูแล | Document Control" Language="C#" MasterPageFile="~/Admin/Navbar.Master" AutoEventWireup="true" CodeBehind="RequestSpec.aspx.cs" Inherits="DocumentControl.Admin.RequestSpec" EnableEventValidation="false" %>

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

        <asp:GridView ID="GVRequestSpec" runat="server" CssClass="table table-sm table-hover" AutoGenerateColumns="false"
            DataSourceID="SqlDataSourceRequestSpec"
            AllowPaging="true" PageSize="20"
            AllowSorting="true"
            EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true"
            OnRowDataBound="GVRequestSpec_RowDataBound" OnSelectedIndexChanged="GVRequestSpec_SelectedIndexChanged">
            <Columns>
                    <asp:TemplateField HeaderText="อ้างอิง ECR NO" SortExpression="RequestSpecID">
                        <ItemTemplate>
                            <%# int.Parse(Eval("RequestSpecID").ToString()).ToString("D3") %>
                            <asp:HiddenField ID="HFRequestSpecID" runat="server" Value='<%# Eval("RequestSpecID") %>' />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ชนิดของเอกสาร" SortExpression="DocTypeName">
                        <ItemTemplate>
                            <%# Eval("DocTypeName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="การดำเนินการ" SortExpression="OperationName">
                        <ItemTemplate>
                            <%# Eval("OperationName") %> <%# Eval("OperationOther") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วันที่ร้องขอ" SortExpression="DateRequest">
                        <ItemTemplate>
                            <%# DateTime.Parse(Eval("DateRequest").ToString()).ToString("dd/MM/yyyy") %>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ผู้ร้องขอ" SortExpression="Name">
                        <ItemTemplate>
                            <%# Eval("Name") %>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="สถานะ" SortExpression="RequestSpecStatusDetail">
                        <ItemTemplate>
                            <%-- แสดง Remark ตามสถานะ (ยกเลิก, ไม่อนุมัติหัวหน้า, ไม่อนุมัติ NPD) --%>
                            <asp:Panel ID="PanelStatus" runat="server" CssClass="rounded p-1" style="max-width: 400px;">
                                <%# Eval("RequestSpecStatusDetail") + " " + (Eval("RequestSpecStatusID").ToString() == "0" ? Eval("RemarkCancel") : (Eval("RequestSpecStatusID").ToString() == "3" ? Eval("RemarkLeader") : (Eval("RequestSpecStatusID").ToString() == "6" ? "RemarkApprove" : "")))  %>
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
    <%-- Request Spec --%>
    <asp:SqlDataSource ID="SqlDataSourceRequestSpec" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT DC_RequestSpec.RequestSpecID, DC_RequestSpecDocType.DocTypeName, DC_RequestSpecOperation.OperationName, DC_RequestSpec.OperationOther, DC_RequestSpec.DateRequest, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, DC_RequestSpecStatus.RequestSpecStatusDetail, DC_RequestSpec.RequestSpecStatusID, DC_RequestSpec.RemarkLeader, DC_RequestSpec.RemarkApprove, DC_RequestSpec.RemarkCancel
        FROM DC_RequestSpec
        LEFT JOIN DC_RequestSpecDocType ON DC_RequestSpec.RequestSpecDocTypeID = DC_RequestSpecDocType.RequestSpecDocTypeID
        LEFT JOIN DC_RequestSpecOperation ON DC_RequestSpec.RequestSpecOperationID = DC_RequestSpecOperation.RequestSpecOperationID
        LEFT JOIN DC_RequestSpecStatus ON DC_RequestSpec.RequestSpecStatusID = DC_RequestSpecStatus.RequestSpecStatusID
        LEFT JOIN F2_Users ON DC_RequestSpec.UserID = F2_Users.UserID"></asp:SqlDataSource>
</asp:Content>
