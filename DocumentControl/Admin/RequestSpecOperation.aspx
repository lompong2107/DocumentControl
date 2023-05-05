<%@ Page Title="ผู้ดูแล | Document Control" Language="C#" MasterPageFile="~/Admin/Navbar.Master" AutoEventWireup="true" CodeBehind="RequestSpecOperation.aspx.cs" Inherits="DocumentControl.Admin.RequestSpecOperation" %>

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
                    <h5 class="fw-bold">การดำเนินการ</h5>
                </div>
            </div>
            <div class="text-end col-2">
                <!-- Button trigger modal -->
                <button type="button" class="btn btn-sm btn-light" data-bs-toggle="modal" data-bs-target="#openModalAdd">
                    <img src="~/Image/add.png" runat="server" height="15" /><span class="ms-1 align-middle">เพิ่มข้อมูล</span>
                </button>
            </div>
        </div>

        <%-- แสดงข้อมูล --%>
        <div class="my-2">
            <asp:GridView ID="GVRequestSpecOperation" runat="server" CssClass="table table-sm table-hover" AutoGenerateColumns="false" 
                DataSourceID="SqlDataSourceRequestSpecOperation"
                AllowPaging="true" PageSize="20"
                AllowSorting="true" 
                ShowHeaderWhenEmpty="true" EmptyDataText="ไม่พบข้อมูล"
                OnRowDataBound="GVRequestSpecOperation_RowDataBound" OnRowCommand="GVRequestSpecOperation_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="#" HeaderStyle-Width="50">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="การดำเนินการ" DataField="OperationName" ItemStyle-HorizontalAlign="Left" SortExpression="OperationName" />
                    <asp:TemplateField HeaderText="สถานะ" HeaderStyle-Width="120" SortExpression="Status">
                        <ItemTemplate>
                            <asp:LinkButton ID="BtnStatus" runat="server" CommandName="BtnStatus" CommandArgument='<%# Eval("RequestSpecOperationID") %>' OnClientClick="return alertConfirm(this, 'เปลี่ยนสถานะ!', 'ต้องการเปลี่ยนสถานะหรือไม่?', 'question', 'ตกลง!', 'ยกเลิก');"></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="คำสั่ง" HeaderStyle-Width="100">
                        <ItemTemplate>
                            <asp:ImageButton ID="BtnEdit" runat="server" CssClass="shadow-hover" CommandName="BtnEdit" CommandArgument='<%# Eval("RequestSpecOperationID") %>' ImageUrl="~/Image/edit.png" Height="30" ToolTip="แก้ไข" />
                            <asp:ImageButton ID="BtnDelete" runat="server" CssClass="shadow-hover" CommandName="BtnDelete" CommandArgument='<%# Eval("RequestSpecOperationID") %>' ImageUrl="~/Image/delete.png" Height="30" ToolTip="ลบ" OnClientClick="return alertConfirm(this, 'ลบข้อมูล!', 'ต้องการลบหรือไม่?', 'question', 'ลบ!', 'ยกเลิก');" />
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
                            <label for='<%= TxtOperationName.ClientID %>' class="form-label">ชื่อการดำเนินการ</label>
                            <asp:TextBox ID="TxtOperationName" runat="server" CssClass="form-control form-control-sm"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorOperationName" runat="server"
                                ControlToValidate="TxtOperationName"
                                ValidationGroup="OperationName"
                                ErrorMessage="กรุณาป้อนข้อมูล."
                                SetFocusOnError="true"
                                ForeColor="#ff1717"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                        <div class="text-center">
                            <asp:LinkButton ID="LinkBtnSave" runat="server" CssClass="btn btn-sm btn-primary" Width="100" OnClick="LinkBtnSave_Click" ValidationGroup="OperationName">
                            <img src="~/Image/save.png" runat="server" height="15" /><span class="ms-1 align-middle">บันทึก</span>
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
                            <label for='<%= TxtOperationNameEdit.ClientID %>' class="form-label">ชื่อการดำเนินการ</label>
                            <asp:TextBox ID="TxtOperationNameEdit" runat="server" CssClass="form-control form-control-sm"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorOperationNameEdit" runat="server"
                                ControlToValidate="TxtOperationNameEdit"
                                ValidationGroup="OperationNameEdit"
                                ErrorMessage="กรุณาป้อนข้อมูล."
                                SetFocusOnError="true"
                                ForeColor="#ff1717"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                        <div class="text-center">
                            <asp:HiddenField ID="HFRequestSpecOperationID" runat="server" />
                            <asp:LinkButton ID="LinkBtnUpdate" runat="server" CssClass="btn btn-sm btn-primary" OnClick="LinkBtnUpdate_Click" ValidationGroup="OperationNameEdit">
                            <img src="~/Image/save.png" runat="server" height="15" /><span class="ms-1 align-middle">บันทึก</span>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%-- SqlDataSource --%>
    <%-- การดำเนินการ --%>
    <asp:SqlDataSource ID="SqlDataSourceRequestSpecOperation" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT RequestSpecOperationID, OperationName, Status FROM DC_RequestSpecOperation ORDER BY Status DESC, OperationName ASC"></asp:SqlDataSource>
</asp:Content>
