<%@ Page Title="ผู้ดูแล | Document Control" Language="C#" MasterPageFile="~/Admin/Navbar.Master" AutoEventWireup="true" CodeBehind="RequestDARDetail.aspx.cs" Inherits="DocumentControl.Admin.RequestDARDetail" Culture="en-US" %>

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
        <div class="border rounded p-3 bg-white text-black">
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
                        <asp:GridView ID="GVRequestDARDoc" runat="server" CssClass="table" EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true" AutoGenerateColumns="false" OnRowCommand="GVRequestDARDoc_RowCommand" OnRowDataBound="GVRequestDARDoc_RowDataBound">
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
                            </Columns>
                            <HeaderStyle HorizontalAlign="Center" />
                            <EmptyDataRowStyle HorizontalAlign="Center" />
                        </asp:GridView>
                    </div>
                </div>
            </div>

            <hr />

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
        </div>

        <div class="my-3">
            <p class="text-secondary">#ยังไม่รู้ว่าจะเอาไว้แก้ไขอะไร</p>
        </div>
    </div>
</asp:Content>
