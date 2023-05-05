<%@ Page Title="รายงานการแจ้ง | Docuemnt Control" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="RequestSpecAll.aspx.cs" Inherits="DocumentControl.DocumentRequest.RequestSpec.RequestSpecAll" EnableEventValidation="false" %>

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
    <%-- เก็บค่า --%>
    <asp:HiddenField ID="HFRequestSpecID" runat="server" />
    <%-- ค่า RequestSpecID --%>
    <div class="container pt-3 pb-2">
        <div class="border rounded bg-white p-3">
            <div class="text-center">
                <h5 class="fw-bold">รายงานการแจ้งการดำเนินการเอกสารวิศวกรรม<br />
                    (Specification & Drawing)</h5>
            </div>
            <div class="row m-0 mb-2">
                <div class="col-6 p-0 align-self-end">
                    <asp:CheckBox ID="CBShowAll" runat="server" Text="แสดงรายการทั้งหมด" OnCheckedChanged="CBShowAll_CheckedChanged" AutoPostBack="true" CssClass="CheckBoxCustom" />
                </div>
                <div class="col-6 p-0 text-end">
                    <span class="align-bottom">แถว : </span>
                    <asp:DropDownList ID="DDListPaging" runat="server" Width="80" CssClass="form-select form-select-sm d-inline-block" AutoPostBack="true" OnSelectedIndexChanged="DDListPaging_SelectedIndexChanged">
                        <asp:ListItem Value="10" Text="10"></asp:ListItem>
                        <asp:ListItem Value="20" Text="20" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="50" Text="50"></asp:ListItem>
                        <asp:ListItem Value="100" Text="100"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <asp:GridView ID="GVRequestSpec" runat="server" CssClass="table table-borderless border-0 table-hover" AutoGenerateColumns="false"
                DataSourceID="SqlDataSourceRequestSpec" DataKeyNames="RequestSpecID"
                AllowPaging="true" AllowSorting="true" 
                EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true" OnRowDataBound="GVRequestSpec_RowDataBound" OnSelectedIndexChanged="GVRequestSpec_SelectedIndexChanged">
                <Columns>
                    <asp:TemplateField HeaderText="อ้างอิง ECR NO" SortExpression="RequestSpecID">
                        <ItemTemplate>
                            <%# int.Parse(Eval("RequestSpecID").ToString()).ToString("D3") %>
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
    <%-- Request Spec --%>
    <asp:SqlDataSource ID="SqlDataSourceRequestSpec" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT DC_RequestSpec.RequestSpecID, DC_RequestSpecDocType.DocTypeName, DC_RequestSpecOperation.OperationName, DC_RequestSpec.OperationOther, DC_RequestSpec.DateRequest, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH) AS Name, DC_RequestSpecStatus.RequestSpecStatusDetail, DC_RequestSpec.RequestSpecStatusID, DC_RequestSpec.RemarkLeader, DC_RequestSpec.RemarkApprove, DC_RequestSpec.RemarkCancel
        FROM DC_RequestSpec
        LEFT JOIN DC_RequestSpecDocType ON DC_RequestSpec.RequestSpecDocTypeID = DC_RequestSpecDocType.RequestSpecDocTypeID
        LEFT JOIN DC_RequestSpecOperation ON DC_RequestSpec.RequestSpecOperationID = DC_RequestSpecOperation.RequestSpecOperationID
        LEFT JOIN DC_RequestSpecStatus ON DC_RequestSpec.RequestSpecStatusID = DC_RequestSpecStatus.RequestSpecStatusID
        LEFT JOIN F2_Users ON DC_RequestSpec.UserID = F2_Users.UserID"></asp:SqlDataSource>
</asp:Content>
