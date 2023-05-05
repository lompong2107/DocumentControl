<%@ Page Title="ประวัติการร้องขอ | Document Control" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="RequestDARHistory.aspx.cs" Inherits="DocumentControl.DocumentRequest.RequestDAR.RequestDARHistory" EnableEventValidation="false" %>

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

        .table a {
            text-decoration: none;
        }
    </style>

    <script type="text/javascript">
        function OpenModalCancel() {
            new bootstrap.Modal($("#ModalCancel")).show();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%-- เก็บค่า --%>
    <asp:HiddenField ID="HFRequestDARID" runat="server" />
    <%-- ค่า RequestDARID --%>
    <div class="container pt-3 pb-2">
        <div class="border rounded bg-white p-3">
            <div class="text-center">
                <h5 class="fw-bold">ประวัติการร้องขอ DAR</h5>
            </div>

            <div class="border rounded p-2 bg-white">
                <div class="d-flex justify-content-end mb-1">
                    <div class="d-flex">
                        <div class="d-flex me-2 align-items-end">
                            <span class="text-nowrap me-1">สถานะ : </span>
                            <asp:DropDownList ID="DDListRequestDARStatus" runat="server" DataValueField="RequestDARStatusID" DataTextField="RequestDARStatusDetail" CssClass="form-select form-select-sm" OnSelectedIndexChanged="DDListRequestDARStatus_SelectedIndexChanged" OnDataBound="DDListRequestDARStatus_DataBound" AutoPostBack="true"></asp:DropDownList>
                        </div>
                        <div class="d-flex align-items-end">
                            <span class="text-nowrap me-1">แถว : </span>
                            <asp:DropDownList ID="DDListPagingRequestDAR" runat="server" Width="80" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="DDListPagingRequestDAR_SelectedIndexChanged">
                                <asp:ListItem Value="10" Text="10"></asp:ListItem>
                                <asp:ListItem Value="20" Text="20" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="50" Text="50"></asp:ListItem>
                                <asp:ListItem Value="100" Text="100"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <%-- สถานะ --%>
                <div class="d-flex justify-content-end mb-2">
                    <div class="badge rounded-pill text-dark me-1">
                        <span>รายการทั้งหมด</span>
                        <asp:Label ID="LbRequestAll" runat="server" Text="0" CssClass="ms-1"></asp:Label>
                    </div>
                    <div class="badge rounded-pill bg-success text-dark me-1">
                        <span>เสร็จสมบูรณ์</span>
                        <asp:Label ID="LbRequestCompleted" runat="server" Text="0" CssClass="ms-1"></asp:Label>
                    </div>
                    <div class="badge rounded-pill bg-info text-dark me-1">
                        <span>แจกจ่ายแล้ว</span>
                        <asp:Label ID="LbRequestPublish" runat="server" Text="0" CssClass="ms-1"></asp:Label>
                    </div>
                    <div class="badge rounded-pill bg-warning text-dark me-1">
                        <span>รอตรวจสอบ,รออนุมัติ,รอดำเนินการแก้ไข</span>
                        <asp:Label ID="LbRequestApprove" runat="server" Text="0" CssClass="ms-1"></asp:Label>
                    </div>
                    <div class="badge rounded-pill bg-danger text-dark me-1">
                        <span>ไม่อนุมัติ</span>
                        <asp:Label ID="LbRequestDisApprove" runat="server" Text="0" CssClass="ms-1"></asp:Label>
                    </div>
                </div>

                <asp:GridView ID="GVRequestDAR" runat="server" CssClass="table table-borderless border-0 table-hover" AutoGenerateColumns="false"
                    DataSourceID="SqlDataSourceRequestDAR" DataKeyNames="RequestDARID"
                    AllowPaging="true" AllowSorting="true"
                    EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true"
                    OnRowDataBound="GVRequestDAR_RowDataBound" OnRowCommand="GVRequestDAR_RowCommand" OnSelectedIndexChanged="GVRequestDAR_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField HeaderText="เลขที่ DAR" SortExpression="RequestDARID">
                            <ItemTemplate>
                                <%# int.Parse(Eval("RequestDARID").ToString()).ToString("D3") %>
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
                                <%# Eval("OperationName") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="วันที่ร้องขอ" SortExpression="DateRequest">
                            <ItemTemplate>
                                <%# DateTime.Parse(Eval("DateRequest").ToString()).ToString("dd/MM/yyyy") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ผู้ตรวจสอบ" SortExpression="LeaderName">
                            <ItemTemplate>
                                <%# Eval("LeaderName") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="รับทราบ" SortExpression="NPDName">
                            <ItemTemplate>
                                <%# (Eval("NPDName").ToString().Length == 0) ? "-" : Eval("NPDName") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ผู้อนุมัติ" SortExpression="ApproveName">
                            <ItemTemplate>
                                <%# Eval("ApproveName") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="สถานะ" SortExpression="RequestDARStatusID">
                            <ItemTemplate>
                                <%-- แสดง Remark ตามสถานะ (ยกเลิก, ไม่อนุมัติ) --%>
                                <asp:Panel ID="PanelStatus" runat="server" CssClass="rounded p-1" Style="max-width: 400px;">
                                    <%# Eval("RequestDARStatusDetail") + " " + (Eval("RequestDARStatusID").ToString() == "0" ? Eval("RemarkCancel") : (Eval("RequestDARStatusID").ToString() == "4" ? Eval("Remark") : "")) %>
                                </asp:Panel>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="จัดการ">
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageBtnEdit" runat="server" CssClass="shadow-hover" CommandName="BtnEdit" CommandArgument='<%# Eval("RequestDARID") %>' Height="30" ImageUrl="~/Image/edit.png" ToolTip="แก้ไข" />
                                <asp:ImageButton ID="ImageBtnDelete" runat="server" CssClass="shadow-hover" CommandName="BtnDelete" CommandArgument='<%# Eval("RequestDARID") %>' Height="30" ImageUrl="~/Image/delete.png" OnClientClick="return alertConfirm(this, 'ยกเลิกคำร้องขอ!', 'ต้องการยกเลิกคำร้องขอ?', 'question', 'ตกลง', 'กลับ');" ToolTip="ยกเลิกคำร้องขอ" />
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

            <!-- Modal -->
            <div class="modal fade" id="ModalCancel" tabindex="-1" aria-labelledby="CancelModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="CancelModalLabel">เหตุผล</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <asp:TextBox ID="TxtRemarkCancel" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorRemarkCancel" runat="server"
                                ControlToValidate="TxtRemarkCancel"
                                ValidationGroup="CancelRequest"
                                ErrorMessage="กรุณาระบุเหตุผล."
                                SetFocusOnError="true"
                                ForeColor="#ff1717"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                            <asp:Button ID="BtnCancel" runat="server" Text="ยกเลิกคำร้องขอ" CssClass="btn btn-danger" OnClick="BtnCancel_Click" ValidationGroup="CancelRequest" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            $(".ListBoxSelect2").select2();
        })
    </script>

    <%-- SqlDataSource --%>
    <%-- Request DAR --%>
    <asp:SqlDataSource ID="SqlDataSourceRequestDAR" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT DC_RequestDAR.RequestDARID, DC_RequestDARDocType.DocTypeName, DC_RequestDAROperation.OperationName, DC_RequestDAR.DateRequest
                , (LeaderUser.FirstNameTH + ' ' + LeaderUser.LastNameTH) AS LeaderName, (NPDUser.FirstNameTH + ' ' + NPDUser.LastNameTH) AS NPDName, (ApproveUser.FirstNameTH + ' ' + ApproveUser.LastNameTH) AS ApproveName, DC_RequestDARStatus.RequestDARStatusDetail, DC_RequestDAR.RequestDARStatusID, DC_RequestDAR.Remark, DC_RequestDAR.RemarkCancel
                FROM DC_RequestDAR
                LEFT JOIN DC_RequestDARDocType ON DC_RequestDAR.RequestDARDocTypeID = DC_RequestDARDocType.RequestDARDocTypeID
                LEFT JOIN DC_RequestDAROperation ON DC_RequestDAR.RequestDAROperationID = DC_RequestDAROperation.RequestDAROperationID
                LEFT JOIN DC_RequestDARStatus ON DC_RequestDAR.RequestDARStatusID = DC_RequestDARStatus.RequestDARStatusID
                LEFT JOIN DC_LeaderAccept ON DC_RequestDAR.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN F2_Users AS LeaderUser ON DC_LeaderAccept.UserID = LeaderUser.UserID
                LEFT JOIN DC_NPDAccept ON DC_RequestDAR.NPDAcceptID = DC_NPDAccept.NPDAcceptID
                LEFT JOIN F2_Users AS NPDUser ON DC_NPDAccept.UserID = NPDUser.UserID
                LEFT JOIN DC_Approve ON DC_RequestDAR.ApproveID = DC_Approve.ApproveID
                LEFT JOIN F2_Users AS ApproveUser ON DC_Approve.UserID = ApproveUser.UserID
                WHERE DC_RequestDAR.UserID = @UserID AND DC_RequestDAR.RequestDARStatusID LIKE @Status">
        <SelectParameters>
            <asp:SessionParameter Name="UserID" SessionField="UserID" ConvertEmptyStringToNull="true" />
            <asp:ControlParameter Name="Status" ControlID="DDListRequestDARStatus" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>
