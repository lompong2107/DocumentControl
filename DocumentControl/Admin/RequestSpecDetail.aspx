<%@ Page Title="ผู้ดูแล | Document Control" Language="C#" MasterPageFile="~/Admin/Navbar.Master" AutoEventWireup="true" CodeBehind="RequestSpecDetail.aspx.cs" Inherits="DocumentControl.Admin.RequestSpecDetail" %>

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
    <div class="container pt-3 pb-2">
        <div class="border rounded p-3 bg-white text-black">
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
                    <div>
                        <span class="me-1">สถานะ</span>
                        <asp:Label ID="LbRequestSpecStatus" runat="server" CssClass="text-warning"></asp:Label>
                    </div>
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
                        <asp:GridView ID="GVFiles" runat="server"
                            AutoGenerateColumns="false"
                            DataSourceID="SqlDataSourceSpecDoc" OnRowCommand="GVFiles_RowCommand"
                            CssClass="table table-sm table-striped table-hover table-bordered">
                            <Columns>
                                <asp:TemplateField HeaderText="ไฟล์">
                                    <ItemTemplate>
                                        <a href='<%# ResolveClientUrl("~/DocumentRequest/RequestSpec/ShowPDF.aspx?RequestSpecDocID=" + Eval("RequestSpecDocID")) %>' target="_blank"><%# Eval("FileName").ToString() + "" + Eval("FileExtension") %></a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="สถานะ">
                                    <ItemTemplate>
                                        <div class='rounded p-1 <%# (Eval("RequestSpecDocStatusID").ToString() == "1" ? "bg-warning" : "bg-success") %>'>
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
                            </Columns>
                            <HeaderStyle HorizontalAlign="Center" CssClass="table-primary" />
                            <EmptyDataRowStyle HorizontalAlign="Center" />
                        </asp:GridView>
                    </div>
                </div>

                <hr />

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
            </div>
        </div>

        <div class="my-3">
            <p class="text-secondary">#ยังไม่รู้ว่าจะเอาไว้แก้ไขอะไร</p>
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
