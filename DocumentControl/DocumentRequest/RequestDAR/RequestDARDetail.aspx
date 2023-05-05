<%@ Page Title="รายละเอียดใบ DAR | Document Control" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="~/DocumentRequest/RequestDAR/RequestDARDetail.aspx.cs" Inherits="DocumentControl.DocumentRequest.RequestDAR.RequestDARDetail" Culture="en-US" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .bg-warning {
            background-color: #FCF6BD !important;
        }

        .bg-success {
            background-color: #D0F4DE !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%-- เก็บค่า --%>
    <asp:HiddenField ID="HFRequestDARID" runat="server" />
    <%-- ค่า RequestDARID --%>
    <div class="container pt-3 pb-2">
        <div class="border rounded p-3 bg-white">
            <div class="row row-cols-1 row-cols-md-3">
                <div></div>
                <div class="text-center">
                    <h5 class="fw-bold">ใบคำร้องขอดำเนินการแก้ไขเอกสารและข้อมูล (DAR)</h5>
                </div>
                <div class="text-end">
                    <div>
                        <span>วันที่รับใบ DAR</span>
                        <asp:Label ID="LbDateRequest" runat="server"></asp:Label>
                    </div>
                    <div>
                        <span>DAR No.</span>
                        <asp:Label ID="LbRequestDARID" runat="server"></asp:Label>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanelStatus" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <span class="me-1">สถานะ</span>
                            <asp:Label ID="LbRequestDARStatus" runat="server" CssClass="text-warning"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Panel ID="PanelEdit" runat="server" Visible="false">
                        <a href='~/DocumentRequest/RequestDAR/RequestDAREdit.aspx?RequestDARID=<%=Request.QueryString["RequestDARID"] %>' runat="server" class="link-warning">แก้ไข</a>
                    </asp:Panel>
                </div>
            </div>

            <p class="m-0 text-secondary">รายละเอียดคำร้องขอ</p>
            <div class="py-2 px-3">
                <div class="row gy-1 mb-3">
                    <div class="col-md-4 col-lg-3 col-12">
                        <span class="fw-bold">ชนิดของเอกสาร</span>
                    </div>
                    <div class="col-md-8 col-lg-9 col-12 border-start">
                        <asp:Label ID="LbDocType" runat="server"></asp:Label>
                        <asp:Label ID="LbDocTypeOther" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-md-4 col-lg-3 col-12">
                        <span class="fw-bold">การดำเนินการ</span>
                    </div>
                    <div class="col-md-8 col-lg-9 col-12 border-start">
                        <asp:Label ID="LbOperation" runat="server"></asp:Label>
                        <asp:Label ID="LbOperationOther" runat="server"></asp:Label>
                    </div>
                </div>
                <div>
                    <div>
                        <span class="fw-bold">รายการแก้ไข / เพิ่มเติม</span>
                    </div>
                    <div>
                        <asp:GridView ID="GVRequestDARDoc" runat="server" AutoGenerateColumns="false"
                            CssClass="table table-borderless border-0 mb-0" 
                            DataSourceID="SqlDataSourceRequestDARDoc"
                            EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true" 
                            OnRowCommand="GVRequestDARDoc_RowCommand" OnRowDataBound="GVRequestDARDoc_RowDataBound">
                            <Columns>
                                <asp:TemplateField HeaderText="เลขที่เอกสาร">
                                    <ItemTemplate>
                                        <%# Eval("DocNumber") %>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ชื่อเอกสาร">
                                    <ItemTemplate>
                                        <%# Eval("DocName") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="วันที่บังคับใช้">
                                    <ItemTemplate>
                                        <%# DateTime.Parse(Eval("DateEnforce").ToString()).ToString("dd/MM/yyyy") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="เหตุผลการร้องขอ">
                                    <ItemTemplate>
                                        <%# Eval("Remark") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="เอกสารแนบ">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImageBtnDownload" runat="server" CommandName="BtnDownload" CommandArgument='<%# Eval("FilePath") %>' Height="30" ImageUrl="~/Image/download.png" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="สถานะ">
                                    <ItemTemplate>
                                        <asp:Panel ID="PanelStatus" runat="server" CssClass="rounded p-1">
                                            <%# Eval("DocStatusDetail") %>
                                        </asp:Panel>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="อัปเดตสถานะ">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImageBtnUpdateStatus" runat="server" CssClass="shadow-hover" CommandName="BtnUpdateStatus" CommandArgument='<%# Eval("RequestDARDocID") %>' ImageUrl="~/Image/check.png" Height="30" OnClientClick="return alertConfirm(this, 'ยืนยัน!', 'ต้องการอัปเดตสถานะ?', 'question', 'อัปเดต!', 'ยกเลิก');" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="table-light border-bottom" HorizontalAlign="Center" VerticalAlign="Middle" />
                            <RowStyle CssClass="border-bottom border-light" VerticalAlign="Middle" />
                            <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        </asp:GridView>
                    </div>
                </div>
            </div>

            <hr />

            <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <p class="m-0 text-secondary">ส่วนผู้อนุมัติ</p>
                    <div class="py-2 px-3">
                        <div class="row gy-1 mb-3">
                            <div class="col-md-4 col-lg-3 col-12">
                                <span class="fw-bold">ผู้ร้องขอ</span>
                            </div>
                            <div class="col-md-8 col-lg-9 col-12 border-start">
                                <asp:Label ID="LbUserRequest" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="row gy-1 mb-3">
                            <div class="col-md-4 col-lg-3 col-12">
                                <span class="fw-bold">ผู้ตรวจสอบ</span>
                            </div>
                            <div class="col-md-8 col-lg-9 col-12 border-start">
                                <asp:Label ID="LbLeaderName" runat="server"></asp:Label>
                                <asp:Label ID="LbLeaderStatus" runat="server"></asp:Label>
                            </div>
                        </div>
                        <asp:Panel ID="PanelAcceptNPD" runat="server" CssClass="row gy-1 mb-3" Visible="false">
                            <div class="col-md-4 col-lg-3 col-12">
                                <div class="row gx-1">
                                    <div class="col-md-12 col-auto">
                                        <span class="fw-bold">รับทราบ</span>
                                    </div>
                                    <div class="col-md-12 col-auto">
                                        <span class="text-secondary">(กรณีที่ต้องReview Control Plan)</span>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-8 col-lg-9 col-12 border-start">
                                <asp:Label ID="LbNPDName" runat="server"></asp:Label>
                                <asp:Label ID="LbNPDStatus" runat="server"></asp:Label>
                            </div>
                        </asp:Panel>
                        <div class="row gy-1 mb-3">
                            <div class="col-md-4 col-lg-3 col-12">
                                <span class="fw-bold">ผู้อนุมัติ</span>
                            </div>
                            <div class="col-md-8 col-lg-9 col-12 border-start">
                                <asp:Label ID="LbApproveName" runat="server"></asp:Label>
                                <asp:Label ID="LbApproveStatus" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <%-- ผู้รับทราบเอกสารแจกจ่ายแล้ว --%>
                    <asp:Panel ID="PanelListPublishAccept" runat="server" Visible="false">
                        <p class="m-0 text-secondary">ส่วนผู้อนุมัติเอกสารแจกจ่าย</p>
                        <div class="py-2 px-3">
                            <asp:ListView ID="LVAcceptPublish" runat="server" OnItemDataBound="LVAcceptPublish_ItemDataBound">
                                <ItemTemplate>
                                    <div class="row gy-1 mb-3">
                                        <div class="col-md-4 col-lg-3 col-12">
                                            <span class="fw-bold">แผนก</span>
                                        </div>
                                        <div class="col-md-8 col-lg-9 col-12 border-start">
                                            <asp:Label ID="LbPublishName" runat="server" Text='<%# Eval("DepartmentName") %>'></asp:Label>
                                            <asp:Label ID="LbPublishStatus" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>
                    </asp:Panel>

                    <%-- รับทราบและอนุมัติ --%>
                    <asp:Panel ID="PanelLeaderAccept" runat="server" CssClass="text-center" Visible="false">
                        <asp:Button ID="BtnLeaderAccept" runat="server" Text="รับทราบ" CssClass="btn btn-success" Width="100" OnClick="BtnLeaderAccept_Click" OnClientClick="return alertConfirm(this, 'รับทราบ', 'ยืนยันการรับทราบ?', 'question', 'ยืนยัน', 'ปิด');" />
                    </asp:Panel>
                    <asp:Panel ID="PanelNPDAccept" runat="server" CssClass="text-center" Visible="false">
                        <asp:Button ID="BtnNPDAccept" runat="server" Text="รับทราบ" CssClass="btn btn-success" Width="100" OnClick="BtnNPDAccept_Click" OnClientClick="return alertConfirm(this, 'รับทราบ', 'ยืนยันการรับทราบ?', 'question', 'ยืนยัน', 'ปิด');" />
                    </asp:Panel>
                    <asp:Panel ID="PanelApprove" runat="server" CssClass="text-center" Visible="false">
                        <asp:Button ID="BtnApprove" runat="server" Text="อนุมัติ" CssClass="btn btn-success me-4" Width="100" OnClick="BtnApprove_Click" OnClientClick="return alertConfirm(this, 'รับทราบ', 'ยืนยันการรับทราบ?', 'question', 'ยืนยัน', 'ปิด');" />
                        <!-- Button trigger modal -->
                        <button type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#DisApproveModal">
                            ไม่อนุมัติ
                        </button>
                    </asp:Panel>

                    <%-- รับทราบแจกจ่ายเอกสาร --%>
                    <asp:Panel ID="PanelPublichAccept" runat="server" CssClass="text-center" Visible="false">
                        <asp:Button ID="BtnPublishAccept" runat="server" Text="รับทราบ" CssClass="btn btn-success" Width="100" OnClick="BtnPublishAccept_Click" OnClientClick="return alertConfirm(this, 'รับทราบ', 'ยืนยันการรับทราบ?', 'question', 'ยืนยัน', 'ปิด');" />
                    </asp:Panel>

                    <asp:Panel ID="PanelSendNotification" runat="server" Visible="false">
                        <%-- แจ้งเตือนถึงแผนกไหนบ้าง --%>
                        <p class="m-0 text-secondary">ส่วนการแจ้งเตือนเอกสารแจกจ่าย</p>
                        <div class="row gy-1 mb-4">
                            <div class="col-md-4 col-lg-3 col-12">
                                <span class="fw-bold">ผู้รับทราบการแจกจ่าย</span>
                            </div>
                            <div class="col-md-8 col-lg-9 col-12">
                                <asp:ListBox ID="ListBoxDepartmentNotification" runat="server" DataValueField="DepartmentID" DataTextField="DepartmentName" SelectionMode="Multiple" CssClass="form-select ListBoxSelect2" data-placeholder="เลือกแผนก"></asp:ListBox>
                                <asp:HiddenField ID="HFDepartmentIDSelectedNotification" runat="server" />
                            </div>
                        </div>
                        <div class="text-center">
                            <asp:Button ID="BtnSendNotification" runat="server" Text="ส่งแจ้งตือน แจกจ่ายเอกสารแล้ว" CssClass="btn btn-success" OnClick="BtnSendNotification_Click" OnClientClick="return alertConfirm(this, 'แจกจ่ายเอกสาร', 'ยืนยันการแจกจ่ายเอกสารแล้ว?', 'question', 'ยืนยัน', 'ปิด');" />
                        </div>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="DisApproveModal" tabindex="-1" aria-labelledby="DisApproveModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="DisApproveModalLabel">เหตุผล</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:TextBox ID="TxtRemark" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorRemarkDisApprove" runat="server"
                            ControlToValidate="TxtRemark"
                            ValidationGroup="CheckRemarkDisApprove"
                            ErrorMessage="กรุณาป้อนเหตุผล."
                            SetFocusOnError="true"
                            ForeColor="#ff1717"
                            Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <asp:Button ID="BtnDisApprove" runat="server" Text="ไม่อนุมัติ" CssClass="btn btn-danger" ValidationGroup="CheckRemarkDisApprove" OnClick="BtnDisApprove_Click" OnClientClick="return alertConfirm(this, 'ไม่อนุมัติ', 'ยืนยันการไม่อนุมัติ?', 'question', 'ยืนยัน', 'ปิด');" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            $(".ListBoxSelect2").select2();

            $("#<%=ListBoxDepartmentNotification.ClientID%>").change(function () {
                $("#<%=HFDepartmentIDSelectedNotification.ClientID%>").val($(this).val())
            }).change()
        })
    </script>

    <asp:SqlDataSource ID="SqlDataSourceRequestDARDoc" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT DC_RequestDARDoc.RequestDARDocID, DC_RequestDARDoc.DocNumber, DC_RequestDARDoc.DocName, DC_RequestDARDoc.DateEnforce, DC_RequestDARDoc.Remark, DC_RequestDARDoc.FilePath, DC_RequestDARDoc.RequestDARDocStatusID, DC_RequestDARDocStatus.DocStatusDetail
        FROM DC_RequestDARDoc 
        LEFT JOIN DC_RequestDARDocStatus ON DC_RequestDARDoc.RequestDARDocStatusID = DC_RequestDARDocStatus.RequestDARDocStatusID
        WHERE DC_RequestDARDoc.RequestDARID = @RequestDARID">
        <SelectParameters>
            <asp:QueryStringParameter Name="RequestDARID" QueryStringField="RequestDARID" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>
