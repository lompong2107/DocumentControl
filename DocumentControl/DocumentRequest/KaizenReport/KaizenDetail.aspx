<%@ Page Title="รายละเอียดใบ Kaizen" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="KaizenDetail.aspx.cs" Inherits="DocumentControl.DocumentRequest.KaizenReport.KaizenDetail" %>

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
    <asp:HiddenField ID="HFKaizenID" runat="server" />
    <%-- ค่า KaizenID --%>
    <div class="container pt-3 pb-2">
        <div class="border rounded p-3 bg-white">
            <div class="row row-cols-1 row-cols-md-3">
                <div></div>
                <div class="text-center">
                    <h5 class="fw-bold">Kaizen Report</h5>
                </div>
                <div class="text-end">
                    <div>
                        <span>วันที่</span>
                        <asp:Label ID="LbDateCreate" runat="server"></asp:Label>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanelStatus" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <span class="me-1">สถานะ</span>
                            <asp:Label ID="LbKaizenStatus" runat="server" CssClass="text-warning"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Panel ID="PanelEdit" runat="server" Visible="false">
                        <a href='<%= ResolveClientUrl("~/DocumentRequest/KaizenReport/KaizenEdit.aspx?KaizenID=" + Request.QueryString["KaizenID"]) %>' class="link-warning">แก้ไข</a>
                    </asp:Panel>
                </div>
            </div>

            <p class="m-0 text-secondary">รายละเอียด</p>
            <div class="py-2 px-3">
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <span class="fw-bold">ชื่อเรื่อง</span>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:Label ID="LbTopic" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <span class="fw-bold">ไฟล์แนบ</span>
                    </div>
                    <div class="col-lg-8 col-12 d-flex border-start">
                        <asp:HyperLink ID="HLOpenFile" runat="server" Text="เปิดไฟล์" CssClass="btn btn-sm btn-primary me-2" Target="_blank" Width="100"></asp:HyperLink>
                        <asp:Button ID="BtnDownload" runat="server" Text="ดาวน์โหลด" CssClass="btn btn-sm btn-secondary me-2" Width="100" OnClick="BtnDownload_Click" />
                        <asp:Button ID="BtnHistory" runat="server" Text="ประวัติ" CssClass="btn btn-sm btn-secondary" Width="100" OnClick="BtnHistory_Click" />
                    </div>
                </div>
            </div>

            <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <p class="m-0 text-secondary">ส่วนผู้อนุมัติ</p>
                    <div class="py-2 px-3">
                        <div class="row gy-1 mb-3">
                            <div class="col-lg-4 col-12">
                                <span class="fw-bold">ผู้สร้าง</span>
                            </div>
                            <div class="col-lg-8 col-12 border-start">
                                <asp:Label ID="LbUserRequest" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="row gy-1 mb-3">
                            <div class="col-lg-4 col-12">
                                <span class="fw-bold">หน่วยงาน</span>
                            </div>
                            <div class="col-lg-8 col-12 border-start">
                                <asp:Label ID="LbDepartment" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="row gy-1 mb-3">
                            <div class="col-lg-4 col-12">
                                <span class="fw-bold">ผู้ตรวจสอบ</span>
                            </div>
                            <div class="col-lg-8 col-12 border-start">
                                <asp:Label ID="LbLeaderName" runat="server"></asp:Label>
                                <asp:Label ID="LbLeaderStatus" runat="server"></asp:Label>
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

                    <%-- รับทราบและอนุมัติ --%>
                    <asp:Panel ID="PanelLeaderAccept" runat="server" CssClass="text-center" Visible="false">
                        <asp:Button ID="BtnLeaderAccept" runat="server" Text="รับทราบ" CssClass="btn btn-success" Width="100" OnClick="BtnLeaderAccept_Click" />
                    </asp:Panel>
                    <asp:Panel ID="PanelApprove" runat="server" CssClass="text-center" Visible="false">
                        <asp:Button ID="BtnApprove" runat="server" Text="อนุมัติ" CssClass="btn btn-success me-4" Width="100" OnClick="BtnApprove_Click" />
                        <!-- Button trigger modal -->
                        <button type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#DisApproveModal">
                            ไม่อนุมัติ
                        </button>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
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
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <asp:Button ID="BtnDisApprove" runat="server" Text="ไม่อนุมัติ" CssClass="btn btn-danger" OnClick="BtnDisApprove_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
