<%@ Page Title="สิทธิ์การใช้งาน | Document Control" Language="C#" MasterPageFile="~/Admin/Navbar.Master" AutoEventWireup="true" CodeBehind="Permission.aspx.cs" Inherits="DocumentControl.Admin.Permission" Culture="en-US" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .RadioCustom input {
            margin-right: 3px;
        }

        .RadioCustom label {
            margin-right: 10px;
        }

        /* Change Color Class accordion */
        .accordion-button:not(.collapsed) {
            color: #e40c0c;
            background-color: #ffb8b8;
        }

        .accordion-button::after {
            background-image: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path d="M207.029 381.476L12.686 187.132c-9.373-9.373-9.373-24.569 0-33.941l22.667-22.667c9.357-9.357 24.522-9.375 33.901-.04L224 284.505l154.745-154.021c9.379-9.335 24.544-9.317 33.901.04l22.667 22.667c9.373 9.373 9.373 24.569 0 33.941L240.971 381.476c-9.373 9.372-24.569 9.372-33.942 0z"/></svg>');
        }

        .accordion-button:not(.collapsed)::after {
            background-image: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path fill="red" d="M207.029 381.476L12.686 187.132c-9.373-9.373-9.373-24.569 0-33.941l22.667-22.667c9.357-9.357 24.522-9.375 33.901-.04L224 284.505l154.745-154.021c9.379-9.335 24.544-9.317 33.901.04l22.667 22.667c9.373 9.373 9.373 24.569 0 33.941L240.971 381.476c-9.373 9.372-24.569 9.372-33.942 0z"/></svg>');
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container pt-3 pb-2">
        <div class="text-center">
            <h5 class="fw-bold">สิทธิ์การใช้งาน</h5>
        </div>

        <div class="row row-cols-1 row-cols-lg-2 gy-3">
            <div class="col">
                <div class="rounded-3 bg-white text-dark p-3 h-100">
                    <div class="mb-3 border rounded-2 p-2">
                        <div>
                            <span>สิทธิ์</span>
                        </div>
                        <div>
                            <span class="text-secondary">> เอกสารแจกจ่าย เผยแพร่</span>
                        </div>
                        <div class="mb-2">
                            <asp:RadioButtonList ID="RBListPermissionPublish" runat="server" CssClass="RadioCustom" AutoPostBack="true" RepeatDirection="Horizontal" RepeatLayout="Flow" DataValueField="PermissionID" DataTextField="PermissionDetail" OnSelectedIndexChanged="RBListPermissionPublish_SelectedIndexChanged"></asp:RadioButtonList>
                        </div>
                        <div>
                            <span class="text-secondary">> Document Action Request</span>
                        </div>
                        <div>
                            <asp:RadioButtonList ID="RBListPermissionDAR" runat="server" CssClass="RadioCustom" AutoPostBack="true" RepeatDirection="Horizontal" RepeatLayout="Flow" DataValueField="PermissionID" DataTextField="PermissionDetail" OnSelectedIndexChanged="RBListPermissionDAR_SelectedIndexChanged"></asp:RadioButtonList>
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
                    <div class="text-center">
                        <asp:Button ID="BtnAdd" runat="server" Text="เพิ่ม" Width="100" CssClass="btn btn-sm btn-primary" OnClick="BtnAdd_Click" />
                    </div>
                </div>
            </div>
            <div class="col">
                <div class="rounded-3 bg-white text-dark text-center p-3 h-100 d-flex flex-column">
                    <div>
                        <span>ระดับสิทธิ์</span>
                        <asp:Label ID="NameApprove" runat="server" CssClass="fw-bold"></asp:Label>
                    </div>
                    <div class="my-1 h-100">
                        <asp:ListBox ID="ListBoxPermissionUser" runat="server" CssClass="form-control h-100" DataValueField="UserID" DataTextField="Name" OnSelectedIndexChanged="ListBoxPermissionUser_SelectedIndexChanged" AutoPostBack="true"></asp:ListBox>
                    </div>
                    <div>
                        <asp:Button ID="BtnDelete" runat="server" Text="ลบ" Width="100" CssClass="btn btn-sm btn-danger" Enabled="false" OnClick="BtnDelete_Click" />
                    </div>
                </div>
            </div>
        </div>

        <div class="mt-3">
            <p class="mb-0">รายละเอียดสิทธิ์การใช้งาน</p>
            <div class="accordion" id="accordionDetailPermission">
                <div class="accordion-item">
                    <h2 class="accordion-header" id="headingOne">
                        <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#collapseOne" aria-expanded="true" aria-controls="collapseOne">
                            เอกสารแจกจ่าย เผยแพร่
                        </button>
                    </h2>
                    <div id="collapseOne" class="accordion-collapse collapse show" aria-labelledby="headingOne" data-bs-parent="#accordionDetailPermission">
                        <div class="accordion-body text-dark">
                            <p class="m-0 fw-bold">* Full Control</p>
                            <p class="m-0 ms-3">- หน้า เอกสารเผยแพร่ สามารถเพิ่ม, ลบ, แก้ไข, อัปโหลดข้อมูล ได้ทั้งหมด</p>
                            <p class="m-0 fw-bold">* Ownership</p>
                            <p class="m-0 ms-3">- หน้า เอกสารเผยแพร่ สามารถเพิ่ม, ลบ, แก้ไข, อัปโหลดข้อมูล ได้เฉพาะของตัวเอง</p>
                        </div>
                    </div>
                </div>
                <div class="accordion-item">
                    <h2 class="accordion-header" id="headingTwo">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapseTwo" aria-expanded="false" aria-controls="collapseTwo">
                            Docoment Action Request
                        </button>
                    </h2>
                    <div id="collapseTwo" class="accordion-collapse collapse" aria-labelledby="headingTwo" data-bs-parent="#accordionDetailPermission">
                        <div class="accordion-body text-dark">
                            <p class="m-0 fw-bold">* Document Control</p>
                            <p class="m-0 ms-3">- หน้า รายงานสถานะการดำเนินการ สามารถอัปเดตสถานะเอกสารได้ (เป็นผู้แจกจ่ายเอกสารจากการร้องขอ DAR)</p>
                            <p class="m-0 fw-bold">* Edit Document Control</p>
                            <p class="m-0 ms-3">- หน้า รายงานการร้องขอ DAR (Log Book DAR) สามารถแก้ไขเอกสารร้องขอ DAR ของคนอื่นได้ทั้งหมด</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
