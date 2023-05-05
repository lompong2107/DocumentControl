<%@ Page Title="รายละเอียดแบบแจ้งการดำเนินการเอกสารวิศวกรรม | Document Control" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="RequestSpecDetail.aspx.cs" Inherits="DocumentControl.DocumentRequest.RequestSpec.RequestSpecDetail" MaintainScrollPositionOnPostback="true" %>

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
    <asp:HiddenField ID="HFRequestSpecID" runat="server" />
    <%-- ค่า RequestSpecID --%>
    <div class="container pt-3 pb-2">
        <div class="border rounded p-3 bg-white">
            <div class="row row-cols-1 row-cols-md-3">
                <div></div>
                <div class="text-center">
                    <h5 class="fw-bold">แบบฟอร์มแจ้งการดำเนินการเอกสารวิศวกรรม<br />
                        (Specification & Drawing)</h5>
                </div>
                <div class="text-end">
                    <div>
                        <span>วันที่</span>
                        <asp:Label ID="LbDateRequest" runat="server"></asp:Label>
                    </div>
                    <div>
                        <span>อ้างอิง ECR NO</span>
                        <asp:Label ID="LbRequestSpecID" runat="server"></asp:Label>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanelStatus" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <span class="me-1">สถานะ</span>
                            <asp:Label ID="LbRequestSpecStatus" runat="server" CssClass="text-warning"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Panel ID="PanelEdit" runat="server" Visible="false">
                        <a href='<%= ResolveClientUrl("~/DocumentRequest/RequestSpec/RequestSpecEdit.aspx?RequestSpecID=" + Request.QueryString["RequestSpecID"]) %>' class="link-warning">แก้ไข</a>
                    </asp:Panel>
                </div>
            </div>

            <p class="m-0 text-secondary">รายละเอียดเอกสาร</p>
            <div class="py-2 px-3">
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">เอกสารทางวิศวกรรมที่ต้องการแจ้งการดำเนินการ</label>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:Label ID="LbDocType" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">ประเภทของการแจ้งการดำเนินการเอกสาร</label>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:Label ID="LbOperation" runat="server"></asp:Label>
                        <asp:Label ID="LbOperationOther" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">ประเภทการเปลี่ยนแปลง</label>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:Label ID="LbTypeOfChange" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">Project Name</label>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:Label ID="LbProjectName" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">Part Name</label>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:Label ID="LbPartName" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">Part NO.</label>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:Label ID="LbFG" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">รายละเอียดการแก้ไข/เพิ่มเติม</label>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:Label ID="LbDetailRequest" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">เหตุผลที่ต้องแจ้งดำเนินการเอกสาร</label>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:Label ID="LbReasonRequest" runat="server"></asp:Label>
                    </div>
                </div>
                <div>
                    <div class="text-center">
                        <label class="fw-bold">เอกสารแนบ</label>
                    </div>
                    <div>
                        <asp:GridView ID="GVFiles" runat="server" AutoGenerateColumns="false"
                            CssClass="table table-borderless border-0 mb-0"
                            DataSourceID="SqlDataSourceSpecDoc"
                            EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true"
                            OnRowCommand="GVFiles_RowCommand" OnRowDataBound="GVFiles_RowDataBound">
                            <Columns>
                                <asp:TemplateField HeaderText="ไฟล์">
                                    <ItemTemplate>
                                        <a href='<%# ResolveClientUrl("~/DocumentRequest/RequestSpec/ShowPDF.aspx?RequestSpecDocID=" + Eval("RequestSpecDocID")) %>' target="_blank"><%# Eval("FileName").ToString() + "" + Eval("FileExtension") %></a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="สถานะ">
                                    <ItemTemplate>
                                        <div class='rounded p-1 <%# (Eval("RequestSpecDocStatusID").ToString() == "1" ? "bg-warning" : "bg-success") %>' style="max-width: 400px;">
                                            <%# Eval("DocStatusDetail") %>
                                        </div>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="คำสั่ง">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" CommandName="BtnOpen" CommandArgument='<%# Eval("RequestSpecDocID") %>' Text="เปิดไฟล์" CssClass="link-primary" />
                                        <asp:LinkButton runat="server" CommandName="BtnDownload" CommandArgument='<%# Eval("FilePath") %>' Text="ดาวน์โหลด" CssClass="link-secondary ms-2" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="อัปเดตสถานะ" Visible="false">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImageBtnUpdateStatus" runat="server" CommandName="BtnUpdateStatus" CommandArgument='<%# Eval("RequestSpecDocID") %>' ImageUrl="~/Image/check.png" Height="30" OnClientClick="return alertConfirm(this, 'ยืนยัน!', 'ต้องการอัปเดตสถานะ?', 'question', 'อัปเดต!', 'ยกเลิก');" Visible="false" />
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

                <hr />

                <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <p class="m-0 text-secondary">ตรวจสอบ (หน่วยงานต้นสังกัด)</p>
                        <div class="py-2 px-3">
                            <div class="row gy-1 mb-3">
                                <div class="col-lg-4 col-12">
                                    <span class="fw-bold">ผู้ร้องขอ</span>
                                </div>
                                <div class="col-lg-8 col-12 border-start">
                                    <asp:Label ID="LbUserRequest" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="row gy-1 mb-3">
                                <div class="col-lg-4 col-12">
                                    <span class="fw-bold">ผู้อนุมัติ</span>
                                </div>
                                <div class="col-lg-8 col-12 border-start">
                                    <asp:Label ID="LbLeaderName" runat="server"></asp:Label>
                                    <asp:Label ID="LbLeaderStatus" runat="server"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <p class="m-0 text-secondary">NPD ตรวจสอบ</p>
                        <div class="py-2 px-3">
                            <div class="row gy-1 mb-3">
                                <div class="col-lg-4 col-12">
                                    <span class="fw-bold">วิศกร</span>
                                </div>
                                <div class="col-lg-8 col-12 border-start">
                                    <asp:Label ID="LbNPDName" runat="server"></asp:Label>
                                    <asp:Label ID="LbNPDStatus" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="row gy-1 mb-3">
                                <div class="col-lg-4 col-12">
                                    <span class="fw-bold">ผู้อนุมัติ</span>
                                </div>
                                <div class="col-lg-8 col-12 border-start">
                                    <asp:Label ID="LbApproveName" runat="server"></asp:Label>
                                    <asp:Label ID="LbApproveStatus" runat="server"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <%-- ผู้รับทราบเอกสารแจกจ่ายแล้ว --%>
                        <asp:Panel ID="PanelListPublishAccept" runat="server" Visible="false">
                            <p class="m-0 text-secondary">ส่วนผู้รับทราบเอกสารแจกจ่าย</p>
                            <div class="py-2 px-3">
                                <div class="row gy-1 mb-3">
                                    <div class="col-lg-4 col-12">
                                        <span class="fw-bold">ผู้รับทราบ</span>
                                    </div>
                                    <div class="col-lg-8 col-12 border-start">
                                        <asp:Label ID="LbPublishName" runat="server"></asp:Label>
                                        <asp:Label ID="LbPublishStatus" runat="server"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>

                        <%-- อนุมัติ HOD --%>
                        <asp:Panel ID="PanelLeaderAccept" runat="server" CssClass="text-center" Visible="false">
                            <asp:Button ID="BtnLeaderAccept" runat="server" Text="อนุมัติ" CssClass="btn btn-success me-4" Width="100" OnClick="BtnLeaderAccept_Click" OnClientClick="return alertConfirm(this, 'อนุมัติ', 'ยืนยันการอนุมัติ?', 'question', 'ยืนยัน', 'ปิด');" />
                            <!-- Button trigger modal -->
                            <button type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#DisAcceptModal">
                                ไม่อนุมัติ
                            </button>
                        </asp:Panel>
                        <%-- วิศวร NPD แจกจ่ายเอกสาร ส่งอนุมัติหัวหน้า NPD --%>
                        <asp:Panel ID="PanelNPDAccept" runat="server" CssClass="text-center" Visible="false">
                            <asp:Button ID="BtnNPDAccept" runat="server" Text="แจกจ่ายเอกสารแล้ว ส่งอนุมัติ" CssClass="btn btn-success" OnClick="BtnNPDAccept_Click" OnClientClick="return alertConfirm(this, 'แจกจ่ายเอกสาร', 'ยืนยันการแจกจ่ายเอกสารแล้ว?', 'question', 'ยืนยัน', 'ปิด');" />
                        </asp:Panel>
                        <asp:Panel ID="PanelApprove" runat="server" CssClass="text-center" Visible="false">
                            <asp:Button ID="BtnApprove" runat="server" Text="อนุมัติ" CssClass="btn btn-success me-4" Width="100" OnClick="BtnApprove_Click" OnClientClick="return alertConfirm(this, 'อนุมัติ', 'ยืนยันการอนุมัติ?', 'question', 'ยืนยัน', 'ปิด');" />
                            <!-- Button trigger modal -->
                            <button type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#DisApproveModal">
                                ไม่อนุมัติ
                            </button>
                        </asp:Panel>

                        <%-- รับทราบแจกจ่ายเอกสาร --%>
                        <asp:Panel ID="PanelPublichAccept" runat="server" CssClass="text-center" Visible="false">
                            <asp:Button ID="BtnPublishAccept" runat="server" Text="รับทราบ" CssClass="btn btn-success" Width="100" OnClick="BtnPublishAccept_Click" OnClientClick="return alertConfirm(this, 'รับทราบ', 'ยืนยันการรับทราบเอกสารแจกจ่าย?', 'question', 'ยืนยัน', 'ปิด');" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>

        <!-- Modal -->
        <%-- หัวหน้าหน่วยงานไม่อนุมัติ --%>
        <div class="modal fade" id="DisAcceptModal" tabindex="-1" aria-labelledby="DisAcceptModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="DisAcceptModalLabel">เหตุผล</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <asp:TextBox ID="TxtRemarkDisAccept" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorRemarkDisAccept" runat="server"
                            ControlToValidate="TxtRemarkDisAccept"
                            ValidationGroup="CheckRemarkDisAccept"
                            ErrorMessage="กรุณาป้อนเหตุผล."
                            SetFocusOnError="true"
                            ForeColor="#ff1717"
                            Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="BtnDisAccept" runat="server" Text="ไม่อนุมัติ" CssClass="btn btn-danger" ValidationGroup="CheckRemarkDisAccept" OnClick="BtnDisAccept_Click" OnClientClick="return alertConfirm(this, 'ไม่อนุมัติ', 'ยืนยันการไม่อนุมัติ?', 'question', 'ยืนยัน', 'ปิด');" />
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">ปิด</button>
                    </div>
                </div>
            </div>
        </div>

        <%-- หัวหน้า NPD ไม่อนุมัติ --%>
        <div class="modal fade" id="DisApproveModal" tabindex="-1" aria-labelledby="DisApproveModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="DisApproveModalLabel">เหตุผล</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <asp:TextBox ID="TxtRemarkDisApprove" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorRemarkDisApprove" runat="server"
                            ControlToValidate="TxtRemarkDisApprove"
                            ValidationGroup="CheckRemarkDisApprove"
                            ErrorMessage="กรุณาป้อนเหตุผล."
                            SetFocusOnError="true"
                            ForeColor="#ff1717"
                            Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="BtnDisApprove" runat="server" Text="ไม่อนุมัติ" CssClass="btn btn-danger" OnClick="BtnDisApprove_Click" ValidationGroup="CheckRemarkDisApprove" OnClientClick="return alertConfirm(this, 'ไม่อนุมัติ', 'ยืนยันการไม่อนุมัติ?', 'question', 'ยืนยัน', 'ปิด');" />
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">ปิด</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%-- SqlDataSource --%>
    <%-- Spec Doc --%>
    <asp:SqlDataSource ID="SqlDataSourceSpecDoc" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT DC_RequestSpecDoc.RequestSpecDocID, DC_RequestSpecDoc.RequestSpecID, DC_RequestSpecDoc.FileName, DC_RequestSpecDoc.FileExtension, DC_RequestSpecDoc.FilePath, DC_RequestSpecDoc.RequestSpecDocStatusID, DC_RequestSpecDocStatus.DocStatusDetail
        FROM DC_RequestSpecDoc
        INNER JOIN DC_RequestSpecDocStatus ON DC_RequestSpecDoc.RequestSpecDocStatusID = DC_RequestSpecDocStatus.RequestSpecDocStatusID
        WHERE DC_RequestSpecDoc.RequestSpecID = @RequestSpecID">
        <SelectParameters>
            <asp:QueryStringParameter Name="RequestSpecID" QueryStringField="RequestSpecID" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>
