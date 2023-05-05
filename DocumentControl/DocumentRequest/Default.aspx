<%@ Page Title="หน้าหลัก | Document Control" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DocumentControl.DocumentRequest.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .custom-ul li::marker {
            font-family: "Font Awesome 5 Free";
            content: "\f105";
            font-size: 1.5em;
            color: #0d6efd;
        }

        .custom-ul li {
            padding-inline-start: 1ch;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container pt-3 pb-2">

        <div class="border rounded bg-white p-3">
            <div class="border-bottom text-center">
                <h5 class="fw-bold">หน้าหลัก</h5>
            </div>

            <%-- เมนู --%>
            <div class="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-3 p-3">
                <div class="col">
                    <div class="shadow-sm p-3 card h-100">
                        <ul class="list-unstyled">
                            <li>
                                <h5 class="fw-bold">คำร้องขอแก้ไขเอกสาร (DAR)</h5>
                            </li>
                            <li>
                                <ul class="custom-ul">
                                    <li>
                                        <a href="~/DocumentRequest/RequestDAR/RequestDARAll.aspx" runat="server" class="link-primary text-decoration-none">
                                            <span>รายงานการร้องขอ</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a href="~/DocumentRequest/RequestDAR/RequestDAR.aspx" runat="server" class="link-primary text-decoration-none">
                                            <span>ใบคำร้องขอแก้ไขเอกสาร</span>
                                        </a>
                                    </li>
                                    <li id="LiLogBook" runat="server" visible="false">
                                        <a href="~/DocumentRequest/RequestDAR/LogBook.aspx" runat="server" class="link-primary text-decoration-none">
                                            <span>รายงานสถานะการดำเนินการ</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a href="~/DocumentRequest/RequestDAR/RequestDARHistory.aspx" runat="server" class="link-primary text-decoration-none">
                                            <span>ประวัติการร้องขอ</span>
                                        </a>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </div>
                </div>

                <div class="col">
                    <div class="shadow-sm p-3 card h-100">
                        <ul class="list-unstyled">
                            <li>
                                <h5 class="fw-bold">Specification & Drawing</h5>
                            </li>
                            <li>
                                <ul class="custom-ul">
                                    <li>
                                        <a href="~/DocumentRequest/RequestSpec/RequestSpecAll.aspx" runat="server" class="link-primary text-decoration-none">
                                            <span>รายงานการร้องขอ</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a href="~/DocumentRequest/RequestSpec/RequestSpec.aspx" runat="server" class="link-primary text-decoration-none">
                                            <span>แบบฟอร์ม</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a href="~/DocumentRequest/RequestSpec/RequestSpecHistory.aspx" runat="server" class="link-primary text-decoration-none">
                                            <span>ประวัติการร้องขอ</span>
                                        </a>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </div>
                </div>

                <div class="col">
                    <div class="shadow-sm p-3 card h-100">
                        <ul class="list-unstyled">
                            <li>
                                <h5 class="fw-bold">Kaizen</h5>
                            </li>
                            <li>
                                <ul class="custom-ul">
                                    <li>
                                        <a href="~/DocumentRequest/KaizenReport/KaizenReport.aspx" runat="server" class="link-primary text-decoration-none">
                                            <span>รายงาน Kaizen Report</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a href="~/DocumentRequest/KaizenReport/Kaizen.aspx" runat="server" class="link-primary text-decoration-none">
                                            <span>ใบ Kaizen Report</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a href="~/DocumentRequest/KaizenReport/KaizenHistory.aspx" runat="server" class="link-primary text-decoration-none">
                                            <span>ประวัติการสร้าง</span>
                                        </a>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </div>
                </div>

            </div>
        </div>

    </div>
</asp:Content>
