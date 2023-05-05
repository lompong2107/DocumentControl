<%@ Page Title="Log Book | Docuemnt Control" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="~/DocumentRequest/RequestDAR/LogBook.aspx.cs" Inherits="DocumentControl.DocumentRequest.RequestDAR.LogBook" Culture="en-US" EnableEventValidation="false" %>

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

        .CheckBoxCustom input {
            margin-right: 3px;
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

        .table a {
            text-decoration: none;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container pt-3 pb-2">
        <div class="border rounded bg-white p-3">
            <div class="text-center">
                <h5 class="fw-bold">รายงานสถานะการดำเนินการ</h5>
            </div>

            <div class="d-flex justify-content-between align-items-end mb-2">
                <div>
                    <asp:CheckBox ID="CBShowAll" runat="server" Text="แสดงรายการทั้งหมด" OnCheckedChanged="CBShowAll_CheckedChanged" AutoPostBack="true" CssClass="CheckBoxCustom" />
                </div>
                <div class="text-end">
                    <span class="me-1 align-bottom">(สถานะรอดำเนินการแก้ไข (DC)/แจกจ่ายแล้ว)</span>
                    <span class="align-bottom">แถว :</span>
                    <asp:DropDownList ID="DDListPaging" runat="server" Width="80" CssClass="form-select form-select-sm d-inline-block" AutoPostBack="true" OnSelectedIndexChanged="DDListPaging_SelectedIndexChanged">
                        <asp:ListItem Value="10" Text="10"></asp:ListItem>
                        <asp:ListItem Value="20" Text="20" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="50" Text="50"></asp:ListItem>
                        <asp:ListItem Value="100" Text="100"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <asp:GridView ID="GVRequestDAR" runat="server" CssClass="table table-borderless border-0 table-hover" AutoGenerateColumns="false"
                DataSourceID="SqlDataSourceRequestDAR" DataKeyNames="RequestDARID"
                AllowPaging="true" AllowSorting="true"
                EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true" OnRowDataBound="GVRequestDAR_RowDataBound" OnSelectedIndexChanged="GVRequestDAR_SelectedIndexChanged">
                <Columns>
                    <asp:TemplateField HeaderText="เลขที่ DAR" SortExpression="RequestDARID">
                        <ItemTemplate>
                            <%# int.Parse(Eval("RequestDARID").ToString()).ToString("D3") %>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ชนิดของเอกสาร" SortExpression="DocTypeName">
                        <ItemTemplate>
                            <%# Eval("DocTypeName") %> <%# Eval("DocTypeOther") %>
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
                    <asp:TemplateField HeaderText="สถานะ" SortExpression="RequestDARStatusDetail">
                        <ItemTemplate>
                            <asp:Panel ID="PanelStatus" runat="server" CssClass="rounded p-1" Style="max-width: 400px;">
                                <%# Eval("RequestDARStatusDetail") + " " + Eval("Remark")  %>
                            </asp:Panel>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="border-bottom" HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="Black" />
                <RowStyle BackColor="White" CssClass="border-bottom border-light" VerticalAlign="Middle" />
                <PagerStyle HorizontalAlign="Center" BackColor="White" />
                <EmptyDataRowStyle HorizontalAlign="Center" BackColor="White" />
                <SortedAscendingHeaderStyle CssClass="sortasc" />
                <SortedDescendingHeaderStyle CssClass="sortdesc" />
            </asp:GridView>
        </div>
    </div>

    <%-- SqlDataSource --%>
    <%-- Request DAR --%>
    <asp:SqlDataSource ID="SqlDataSourceRequestDAR" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT DC_RequestDAR.RequestDARID, DC_RequestDARDocType.DocTypeName, DC_RequestDAR.DocTypeOther, DC_RequestDAROperation.OperationName, DC_RequestDAR.OperationOther, DC_RequestDAR.DateRequest, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name,  DC_RequestDARStatus.RequestDARStatusDetail, DC_RequestDAR.RequestDARStatusID, DC_RequestDAR.Remark, DC_RequestDARDoc.RequestDARDocStatusID
        FROM DC_RequestDARDoc
        LEFT JOIN DC_RequestDAR ON DC_RequestDARDoc.RequestDARID = DC_RequestDAR.RequestDARID
        LEFT JOIN DC_RequestDARStatus ON DC_RequestDAR.RequestDARStatusID = DC_RequestDARStatus.RequestDARStatusID
        LEFT JOIN DC_RequestDARDocType ON DC_RequestDAR.RequestDARDocTypeID = DC_RequestDARDocType.RequestDARDocTypeID
        LEFT JOIN DC_RequestDAROperation ON DC_RequestDAR.RequestDAROperationID = DC_RequestDAROperation.RequestDAROperationID
        LEFT JOIN F2_Users ON DC_RequestDAR.UserID = F2_Users.UserID
        GROUP BY DC_RequestDAR.RequestDARID, DC_RequestDARDocType.DocTypeName, DC_RequestDAR.DocTypeOther, DC_RequestDAROperation.OperationName, DC_RequestDAR.OperationOther, DC_RequestDAR.DateRequest, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH),  DC_RequestDARStatus.RequestDARStatusDetail, DC_RequestDAR.RequestDARStatusID, DC_RequestDAR.Remark, DC_RequestDARDoc.RequestDARDocStatusID"></asp:SqlDataSource>
</asp:Content>
