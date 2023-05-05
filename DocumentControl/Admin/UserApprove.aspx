<%@ Page Title="สิทธิ์การอนุมัติ | Document Control" Language="C#" MasterPageFile="~/Admin/Navbar.Master" AutoEventWireup="true" CodeBehind="UserApprove.aspx.cs" Inherits="DocumentControl.Admin.UserApprove" Culture="en-US" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .RadioCustom input {
            margin-right: 3px;
        }

        .RadioCustom label {
            margin-right: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container pt-3 pb-2">
        <div class="text-center">
            <h5 class="fw-bold">สิทธิ์การอนุมัติ</h5>
        </div>
        <div class="row row-cols-1 row-cols-lg-2 gy-3">
            <div class="col">
                <div class="rounded-3 bg-white text-dark p-3 h-100">
                    <div class="mb-3 border rounded-2 p-2">
                        <div>
                            <span>สิทธิ์</span>
                        </div>
                        <div>
                            <span class="text-secondary">> Document Action Request</span>
                        </div>
                        <div class="mb-2">
                            <asp:RadioButtonList ID="RBListApproveDAR" runat="server" CssClass="RadioCustom" AutoPostBack="true" RepeatDirection="Horizontal" RepeatLayout="Flow" OnSelectedIndexChanged="RBListApproveDAR_SelectedIndexChanged">
                                <asp:ListItem Value="1" Text="ผู้รับทราบ" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="2" Text="ผู้อนุมัติ"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                        <div>
                            <span class="text-secondary">> Specification & Drawing</span>
                        </div>
                        <div>
                            <asp:RadioButtonList ID="RBListApproveSpec" runat="server" CssClass="RadioCustom" AutoPostBack="true" RepeatDirection="Horizontal" RepeatLayout="Flow" OnSelectedIndexChanged="RBListApproveSpec_SelectedIndexChanged">
                                <asp:ListItem Value="3" Text="วิศวกร"></asp:ListItem>
                                <asp:ListItem Value="4" Text="ผู้อนุมัติ"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="row mx-0 mb-3">
                        <div class="col-12 col-lg-4">
                            <span>แผนก</span>
                        </div>
                        <div class="col-12 col-lg-8">
                            <asp:DropDownList ID="DDListDepartment" runat="server" CssClass="form-select form-select-sm" DataValueField="DepartmentID" DataTextField="DepartmentName" OnSelectedIndexChanged="DDListDepartment_SelectedIndexChanged" AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row mx-0 mb-3">
                        <div class="col-12 col-lg-4">
                            <span>ผู้ใช้</span>
                        </div>
                        <div class="col-12 col-lg-8">
                            <asp:DropDownList ID="DDListUser" runat="server" CssClass="form-select form-select-sm" DataValueField="UserID" DataTextField="Name" OnSelectedIndexChanged="DDListUser_SelectedIndexChanged" AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row mx-0 mb-3">
                        <div class="col-12 col-lg-4">
                            <span>หมายเหตุ</span>
                        </div>
                        <div class="col-12 col-lg-8">
                            <asp:TextBox ID="TxtRemark" runat="server" CssClass="form-control form-control-sm" placeholder="หมายเหตุ"></asp:TextBox>
                        </div>
                    </div>
                    <div class="text-center">
                        <asp:Button ID="BtnAdd" runat="server" Text="เพิ่ม" Width="100" CssClass="btn btn-sm btn-primary" OnClick="BtnAdd_Click" />
                    </div>
                </div>
            </div>
            <div class="col">
                <div class="rounded-3 bg-white text-dark text-center p-3 h-100 d-flex flex-column">
                    <div>
                        <span>รายการสิทธิ์</span>
                        <asp:Label ID="NameApprove" runat="server" CssClass="fw-bold"></asp:Label>
                    </div>
                    <div class="my-1 h-100">
                        <asp:ListBox ID="ListBoxApprove" runat="server" CssClass="form-control h-100" DataValueField="UserID" DataTextField="Name" OnSelectedIndexChanged="ListBoxApprove_SelectedIndexChanged" AutoPostBack="true"></asp:ListBox>
                    </div>
                    <div>
                        <asp:Button ID="BtnDelete" runat="server" Text="ลบ" Width="100" CssClass="btn btn-sm btn-danger" Enabled="false" OnClick="BtnDelete_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
