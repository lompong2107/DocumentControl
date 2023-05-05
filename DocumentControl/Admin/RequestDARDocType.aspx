<%@ Page Title="ผู้ดูแล | Document Control" Language="C#" MasterPageFile="~/Admin/Navbar.Master" AutoEventWireup="true" CodeBehind="RequestDARDocType.aspx.cs" Inherits="DocumentControl.Admin.RequestDARDocType" Culture="en-US" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
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

    <script type="text/javascript">
        function OpenModalEdit() {
            new bootstrap.Modal($("#openModalEdit")).show();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container pt-3 pb-2">
        <div class="row mx-0">
            <div class="col-2"></div>
            <div class="col-8 text-center">
                <div class="text-center">
                    <h5 class="fw-bold">ชนิดของเอกสาร</h5>
                </div>
            </div>
            <div class="text-end col-2">
                <!-- Button trigger modal -->
                <button type="button" class="btn btn-sm btn-light" data-bs-toggle="modal" data-bs-target="#openModalAdd">
                    <img src="~/Image/add.png" height="15" runat="server" /><span class="ms-1 align-middle">เพิ่มข้อมูล</span>
                </button>
            </div>
        </div>

        <%-- แสดงข้อมูล --%>
        <div class="my-2">
            <asp:GridView ID="GVRequestDARDocType" runat="server" CssClass="table table-sm table-hover" AutoGenerateColumns="false"
                DataSourceID="SqlDataSourceRequestDARDocType"
                AllowPaging="true" PageSize="20"
                AllowSorting="true"
                EmptyDataText="ไม่พบข้อมูล" ShowHeaderWhenEmpty="true"
                OnRowDataBound="GVRequestDARDocType_RowDataBound" OnRowCommand="GVRequestDARDocType_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="#" HeaderStyle-Width="50">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="ชนิดของเอกสาร" DataField="DocTypeName" ItemStyle-HorizontalAlign="Left" SortExpression="DocTypeName" />
                    <asp:TemplateField HeaderText="สถานะ" HeaderStyle-Width="120" SortExpression="Status">
                        <ItemTemplate>
                            <asp:LinkButton ID="BtnStatus" runat="server" CommandName="BtnStatus" CommandArgument='<%# Eval("RequestDARDocTypeID") %>' OnClientClick="return alertConfirm(this, 'เปลี่ยนสถานะ!', 'ต้องการเปลี่ยนสถานะหรือไม่?', 'question', 'ตกลง!', 'ยกเลิก');"></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="คำสั่ง" HeaderStyle-Width="100">
                        <ItemTemplate>
                            <asp:ImageButton ID="BtnEdit" CssClass="shadow-hover" runat="server" CommandName="BtnEdit" CommandArgument='<%# Eval("RequestDARDocTypeID") %>' ImageUrl="~/Image/edit.png" Height="30" ToolTip="แก้ไข" />
                            <asp:ImageButton ID="BtnDelete" CssClass="shadow-hover" runat="server" CommandName="BtnDelete" CommandArgument='<%# Eval("RequestDARDocTypeID") %>' ImageUrl="~/Image/delete.png" Height="30" ToolTip="ลบ" OnClientClick="return alertConfirm(this, 'ลบข้อมูล!', 'ต้องการลบหรือไม่?', 'question', 'ลบ!', 'ยกเลิก');" />
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

        <!-- Modal เพิ่มข้อมูล -->
        <div class="modal fade" id="openModalAdd" tabindex="-1" aria-labelledby="ModalLabelAdd" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header text-black">
                        <h5 class="modal-title" id="LabelAdd">เพิ่มรายการใหม่</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body text-black">
                        <div class="mb-3">
                            <label for='<%= TxtDocTypeName.ClientID %>' class="form-label">ชื่อชนิดของเอกสาร</label>
                            <asp:TextBox ID="TxtDocTypeName" runat="server" CssClass="form-control form-control-sm"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorDocTypeName" runat="server"
                                ControlToValidate="TxtDocTypeName"
                                ValidationGroup="DocTypeName"
                                ErrorMessage="กรุณาป้อนข้อมูล."
                                SetFocusOnError="true"
                                ForeColor="#ff1717"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                        <div class="text-center">
                            <asp:LinkButton ID="LinkBtnSave" runat="server" CssClass="btn btn-sm btn-primary" Width="100" OnClick="LinkBtnSave_Click" ValidationGroup="DocTypeName">
                            <img src="~/Image/save.png" height="15" runat="server" /><span class="ms-1 align-middle">บันทึก</span>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal แก้ไขข้อมูล -->
        <div class="modal fade" id="openModalEdit" tabindex="-1" aria-labelledby="ModalLabelEdit" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header text-black">
                        <h5 class="modal-title" id="ModalLabelEdit">แก้ไขข้อมูล</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body text-black">
                        <div class="mb-3">
                            <label for='<%= TxtDocTypeNameEdit.ClientID %>' class="form-label">ชื่อชนิดของเอกสาร</label>
                            <asp:TextBox ID="TxtDocTypeNameEdit" runat="server" CssClass="form-control form-control-sm"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorDocTypeNameEdit" runat="server"
                                ControlToValidate="TxtDocTypeNameEdit"
                                ValidationGroup="DocTypeNameEdit"
                                ErrorMessage="กรุณาป้อนข้อมูล."
                                SetFocusOnError="true"
                                ForeColor="#ff1717"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                        <div class="text-center">
                            <asp:HiddenField ID="HFRequestDARDocTypeID" runat="server" />
                            <asp:LinkButton ID="LinkBtnUpdate" runat="server" CssClass="btn btn-sm btn-primary" OnClick="LinkBtnUpdate_Click" ValidationGroup="DocTypeNameEdit">
                            <img src="~/Image/save.png" runat="server" height="15" /><span class="ms-1 align-middle">บันทึก</span>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%-- SqlDataSource --%>
    <%-- ชนิดของเอกสาร --%>
    <asp:SqlDataSource ID="SqlDataSourceRequestDARDocType" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT RequestDARDocTypeID, DocTypeName, Status FROM DC_RequestDARDocType ORDER BY Status DESC, DocTypeName ASC"></asp:SqlDataSource>
</asp:Content>
