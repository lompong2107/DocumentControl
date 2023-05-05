<%@ Page Title="ผู้ดูแล | Document Control" Language="C#" MasterPageFile="~/Admin/Navbar.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DocumentControl.Admin.Default" Culture="en-US" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container pt-3 pb-2">
        <div class="text-center">
            <h5 class="fw-bold">หน้าแรก</h5>
        </div>
        <div class="row gx-3">
            <div class="col">
                <div class="rounded-3 p-3 text-center bg-white text-dark d-flex flex-column" style="height: 120px; width: 200px;">
                    <p class="mb-0 border-bottom">ไฟล์เอกสารแจกจ่าย</p>
                    <div class="d-flex justify-content-center h-100">
                        <asp:Label ID="LbCountFile" runat="server" Font-Bold="true" CssClass="h3 mb-0 align-self-center"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
