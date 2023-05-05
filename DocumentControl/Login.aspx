<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="DocumentControl.Login" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>เข้าสู่ระบบ | Document Control</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <link rel="stylesheet" href="Content/bootstrap.min.css" />

    <script type="text/javascript" src="Scripts/jquery-3.6.0.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.bundle.min.js"></script>
    <script type="text/javascript" src="Scripts/sweetalert2.js"></script>

    <style>
        html, body {
            height: 100vh;
            width: 100%;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        body {
            min-height: 100vh;
        }

        .shadow-hover:hover {
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
    <%-- Alert Custom --%>
    <script type="text/javascript" src="Scripts/alert.js"></script>

    <form id="form1" runat="server">
        <div style="width: 380px;" class="bg-white border rounded-3 p-4 shadow-sm">
            <div class="mb-3 text-center">
                <image src="Image/docs.png" width="50"></image>
                <h2 class="fw-bold">Document Control</h2>
            </div>
            <div class="mb-3">
                <asp:TextBox ID="TxtUser" runat="server" placeholder="ชื่อผู้ใช้" CssClass="form-control form-control-lg"></asp:TextBox>
            </div>
            <div class="mb-3">
                <asp:TextBox ID="TxtPassword" runat="server" placeholder="รหัสผ่าน" CssClass="form-control form-control-lg" TextMode="Password"></asp:TextBox>
            </div>
            <div class="mb-3 text-center">
                <asp:Button ID="BtnLogin" runat="server" Text="เข้าสู่ระบบ" CssClass="btn btn-primary btn-lg" Width="150px" OnClick="BtnLogin_Click" />
            </div>
            <div class="text-center">
                <a id="BtnHowTo" class="text-decoration-none" href="Document/คู่มือโปรแกรม Document Control.pdf" target="_blank" height="25">
                    <div class="shadow-hover">
                        <img src="Image/user-guide.png" height="25" class="align-bottom" />
                        คู่มือ?
                    </div>
                </a>
            </div>
        </div>
    </form>
</body>
</html>
