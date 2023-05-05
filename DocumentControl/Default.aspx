<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DocumentControl.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>เมนู | Docuemnt Control</title>
    <link rel="icon" href="Image/docs.png" />
    <%-- Bootstrap --%>
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <script src='<%= ResolveClientUrl("~/Scripts/bootstrap.bundle.min.js") %>'></script>
    <%-- CSS Custom --%>
    <link href="~/Content/style.css" rel="stylesheet" />
    <%-- Jquery 3.6.0 --%>
    <script src='<%= ResolveClientUrl("~/Scripts/jquery-3.6.0.js") %>'></script>
    <%-- Font Awesome 5.15.4 --%>
    <!-- our project just needs Font Awesome Solid + Brands -->
    <link href="~/Content/fontawesome.css" rel="stylesheet" />
    <link href="~/Content/brands.css" rel="stylesheet" />
    <link href="~/Content/solid.css" rel="stylesheet" />

    <style>
        .shadow-hover:hover {
            /*box-shadow: 0 .5rem 1rem rgba(0,0,0,.15) !important;*/
            transform: scale(1.05);
            transition: 0.3s;
        }

        @font-face {
            font-family: 'Sarabun';
            src: url('Fonts/Sarabun-Regular.ttf') format('truetype');
        }

        * {
            font-family: 'Sarabun', sans-serif;
        }
    </style>
</head>
<body class="bg-light">
    <form id="form1" runat="server">
        <!-- Content Start -->
        <div class="content w-100 ms-0">
            <!-- Navbar Start -->
            <nav class="navbar navbar-expand navbar-light fixed-top px-3 py-0">
                <div class="navbar-nav align-items-center ms-auto">
                    <div class="nav-item dropdown">
                        <a href="#" class="nav-link dropdown-toggle" data-bs-toggle="dropdown">
                            <asp:Label ID="LbName" runat="server" Text="ชื่อ-สกุล" CssClass="mx-2" Style="font-size: 1.1rem;"></asp:Label>
                        </a>
                        <div class="dropdown-menu dropdown-menu-end bg-light border-0 shadow-sm m-0">
                            <asp:HyperLink ID="HLManual" runat="server" CssClass="dropdown-item" NavigateUrl="~/Document/คู่มือโปรแกรม Document Control.pdf" Target="_blank">คู่มือ?</asp:HyperLink>
                            <asp:HyperLink ID="HLSetting" runat="server" CssClass="dropdown-item" NavigateUrl="~/Admin/Default.aspx" Visible="false">การตั้งค่า</asp:HyperLink>
                            <asp:LinkButton ID="LinkBtnLogout" runat="server" CssClass="dropdown-item" OnClick="LinkBtnLogout_Click" Text="ออกจากระบบ"></asp:LinkButton>
                        </div>
                    </div>
                </div>
            </nav>
            <!-- Navbar End -->

            <div class="d-table vh-100 w-100">
                <div class="d-table-cell align-middle">
                    <div class="text-center mb-3">
                        <image src="Image/docs.png" width="50"></image>
                        <h1 class="fw-bold">Document Control</h1>
                    </div>
                    <div class="row mx-0">
                        <div class="col-6 p-2">
                            <div class="bg-white ms-auto" style="height: 300px; width: 300px;">
                                <a href="DocumentRequest/Default.aspx" class="btn btn-outline-dark shadow-hover" style="height: 300px; width: 300px;">
                                    <div class="d-table h-100">
                                        <div class="d-table-cell align-middle">
                                            <img src="Image/registration-form.png" width="200" />
                                            <h2 class="mb-0 fw-bold">Document Action Request</h2>
                                        </div>
                                    </div>
                                </a>
                            </div>
                        </div>
                        <div class="col-6 p-2">
                            <div class="bg-white me-auto" style="height: 300px; width: 300px;">
                                <a href="Publish/Publish.aspx" class="btn btn-outline-dark shadow-hover" style="height: 300px; width: 300px;">
                                    <div class="d-table h-100">
                                        <div class="d-table-cell align-middle">
                                            <img src="Image/cloud-storage.png" width="200" />
                                            <h2 class="mb-0 fw-bold">เอกสารแจกจ่าย เผยแพร่</h2>
                                        </div>
                                    </div>
                                </a>
                            </div>
                        </div>
                    </div>
                    <div class="text-center">
                        <label class="text-secondary" style="font-size: 14px;">Thai Cubic Technology Co., Ltd V2.00.00</label>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
