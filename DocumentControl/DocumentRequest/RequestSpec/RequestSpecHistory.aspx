<%@ Page Title="ประวัติการสร้างแบบฟอร์ม | Document Control" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="RequestSpecHistory.aspx.cs" Inherits="DocumentControl.DocumentRequest.RequestSpec.RequestSpecHistory" EnableEventValidation="false" %>

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
    <asp:HiddenField ID="HFRequestSpecID" runat="server" />
    <%-- ค่า RequestSpecID --%>
    <div class="container pt-3 pb-2">
        <div class="border rounded bg-white p-3">
            <div class="text-center">
                <h5 class="fw-bold">ประวัติการสร้างแบบฟอร์มแจ้งการดำเนินการเอกสารวิศวกรรม<br />
                    (Specification & Drawing)</h5>
            </div>

            <div class="border rounded p-2 bg-white">
                <div class="d-flex justify-content-end mb-2">
                    <div class="d-flex">
                        <div class="d-flex me-2 align-items-end">
                            <span class="text-nowrap me-1">สถานะ : </span>
                            <asp:DropDownList ID="DDListStatus" runat="server" DataValueField="RequestSpecStatusID" DataTextField="RequestSpecStatusDetail" CssClass="form-select form-select-sm" OnSelectedIndexChanged="DDListStatus_SelectedIndexChanged" OnDataBound="DDListStatus_DataBound" AutoPostBack="true"></asp:DropDownList>
                        </div>
                        <div class="d-flex align-items-end">
                            <span class="text-nowrap me-1">แถว : </span>
                            <asp:DropDownList ID="DDListPagingRequestSpec" runat="server" Width="80" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="DDListPagingRequestSpec_SelectedIndexChanged">
                                <asp:ListItem Value="10" Text="10"></asp:ListItem>
                                <asp:ListItem Value="20" Text="20" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="50" Text="50"></asp:ListItem>
                                <asp:ListItem Value="100" Text="100"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <%-- สถานะ --%>
                <div class="d-flex justify-content-end mb-1">
                    <div class="badge rounded-pill text-dark me-1">
                        <span>รายการทั้งหมด</span>
                        <asp:Label ID="LbRequestAll" runat="server" Text="0" CssClass="ms-1"></asp:Label>
                    </div>
                    <div class="badge rounded-pill bg-success text-dark me-1">
                        <span>เสร็จสมบูรณ์</span>
                        <asp:Label ID="LbRequestCompleted" runat="server" Text="0" CssClass="ms-1"></asp:Label>
                    </div>
                    <div class=" badge rounded-pill bg-info text-dark me-1">
                        <span>แจกจ่ายแล้ว</span>
                        <asp:Label ID="LbRequestPublish" runat="server" Text="0" CssClass="ms-1"></asp:Label>
                    </div>
                    <div class="badge rounded-pill bg-warning text-dark me-1">
                        <span>รออนุมัติ,รอดำเนินการตรวจสอบ/แก้ไข</span>
                        <asp:Label ID="LbRequestApprove" runat="server" Text="0" CssClass="ms-1"></asp:Label>
                    </div>
                    <div class="badge rounded-pill bg-danger text-dark me-1">
                        <span>ไม่อนุมัติ</span>
                        <asp:Label ID="LbRequestDisApprove" runat="server" Text="0" CssClass="ms-1"></asp:Label>
                    </div>
                </div>

                <asp:GridView ID="GVRequestSpec" runat="server" AutoGenerateColumns="false"
                    CssClass="table table-borderless border-0 table-hover"
                    DataSourceID="SqlDataSourceRequestSpec" DataKeyNames="RequestSpecID"
                    AllowPaging="true" AllowSorting="true" 
                    EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true" 
                    OnRowDataBound="GVRequestSpec_RowDataBound" OnRowCommand="GVRequestSpec_RowCommand" OnSelectedIndexChanged="GVRequestSpec_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField HeaderText="อ้างอิง ECR NO">
                            <ItemTemplate>
                                <%# int.Parse(Eval("RequestSpecID").ToString()).ToString("D3") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ชนิดของเอกสาร">
                            <ItemTemplate>
                                <%# Eval("DocTypeName") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="การดำเนินการ">
                            <ItemTemplate>
                                <%# Eval("OperationName") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="วันที่ร้องขอ">
                            <ItemTemplate>
                                <%# DateTime.Parse(Eval("DateRequest").ToString()).ToString("dd/MM/yyyy") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ผู้อนุมัติ HOD">
                            <ItemTemplate>
                                <span><%# Eval("LeaderName") %></span>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="วิศวกร">
                            <ItemTemplate>
                                <span><%# Eval("NPDName") %></span>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ผู้อนุมัติ NPD">
                            <ItemTemplate>
                                <span><%# Eval("ApproveName") %></span>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="สถานะ">
                            <ItemTemplate>
                                <%-- แสดง Remark ตามสถานะ (ยกเลิก, ไม่อนุมัติหัวหน้า, ไม่อนุมัติ NPD) --%>
                                <asp:Panel ID="PanelStatus" runat="server" CssClass="rounded p-1" Style="max-width: 400px;">
                                    <%# Eval("RequestSpecStatusDetail") + " " + (Eval("RequestSpecStatusDetail").ToString() == "0" ? Eval("RemarkCancel") : (Eval("RequestSpecStatusDetail").ToString() == "3" ? Eval("ReamrkLeader") : (Eval("RequestSpecStatusDetail").ToString() == "6" ? "RemarkApprove" : "")))  %>
                                </asp:Panel>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="จัดการ">
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageBtnEdit" runat="server" CssClass="shadow-hover" CommandName="BtnEdit" CommandArgument='<%# Eval("RequestSpecID") %>' Height="30" ImageUrl="~/Image/edit.png" ToolTip="แก้ไข" />
                                <asp:ImageButton ID="ImageBtnDelete" runat="server" CssClass="shadow-hover" CommandName="BtnDelete" CommandArgument='<%# Eval("RequestSpecID") %>' Height="30" ImageUrl="~/Image/delete.png" OnClientClick="return alertConfirm(this, 'ยกเลิกคำร้องขอ!', 'ต้องการยกเลิกคำร้องขอ?', 'question', 'ตกลง', 'กลับ');" ToolTip="ยกเลิกคำร้องขอ" />
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
    <%-- Request Spec --%>
    <asp:SqlDataSource ID="SqlDataSourceRequestSpec" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT DC_RequestSpec.RequestSpecID, DC_RequestSpecDocType.DocTypeName, DC_RequestSpecOperation.OperationName, DC_RequestSpec.DateRequest
                , (LeaderUser.FirstNameTH + ' ' + LeaderUser.LastNameTH) AS LeaderName, (NPDUser.FirstNameTH + ' ' + NPDUser.LastNameTH) AS NPDName, (ApproveUser.FirstNameTH + ' ' + ApproveUser.LastNameTH) AS ApproveName, DC_RequestSpecStatus.RequestSpecStatusDetail, DC_RequestSpec.RequestSpecStatusID, DC_RequestSpec.RemarkLeader, DC_RequestSpec.RemarkApprove, DC_RequestSpec.RemarkCancel
                FROM DC_RequestSpec
                LEFT JOIN DC_RequestSpecDocType ON DC_RequestSpec.RequestSpecDocTypeID = DC_RequestSpecDocType.RequestSpecDocTypeID
                LEFT JOIN DC_RequestSpecOperation ON DC_RequestSpec.RequestSpecOperationID = DC_RequestSpecOperation.RequestSpecOperationID
                LEFT JOIN DC_RequestSpecStatus ON DC_RequestSpec.RequestSpecStatusID = DC_RequestSpecStatus.RequestSpecStatusID
                LEFT JOIN DC_LeaderAccept ON DC_RequestSpec.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
                LEFT JOIN F2_Users AS LeaderUser ON DC_LeaderAccept.UserID = LeaderUser.UserID
                LEFT JOIN DC_NPDAccept ON DC_RequestSpec.NPDAcceptID = DC_NPDAccept.NPDAcceptID
                LEFT JOIN F2_Users AS NPDUser ON DC_NPDAccept.UserID = NPDUser.UserID
                LEFT JOIN DC_Approve ON DC_RequestSpec.ApproveID = DC_Approve.ApproveID
                LEFT JOIN F2_Users AS ApproveUser ON DC_Approve.UserID = ApproveUser.UserID
                WHERE DC_RequestSpec.UserID = @UserID AND DC_RequestSpec.RequestSpecStatusID LIKE @Status">
        <SelectParameters>
            <asp:SessionParameter Name="UserID" SessionField="UserID" ConvertEmptyStringToNull="true" />
            <asp:ControlParameter Name="Status" ControlID="DDListStatus" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>
