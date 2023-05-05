<%@ Page Title="ใบ Kaizen | Document Control" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="Kaizen.aspx.cs" Inherits="DocumentControl.DocumentRequest.KaizenReport.Kaizen" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container pt-3 pb-2">
        <div class="border rounded p-3 bg-white">
            <div class="text-center">
                <h5 class="fw-bold">Kaizen Report</h5>
            </div>
            <p class="m-0 text-secondary">รายละเอียด</p>
            <div class="py-2 px-3">
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <span class="fw-bold">ชื่อเรื่อง<span class="text-danger">*</span></span>
                    </div>
                    <div class="col-lg-6 col-12 border-start">
                        <asp:TextBox ID="TxtTopic" runat="server" CssClass="form-control form-control-sm" placeholder="ระบุ ชื่อเรื่อง"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server"
                            ControlToValidate="TxtTopic"
                            ErrorMessage="กรุณาป้อนชื่อเรื่อง."
                            SetFocusOnError="true"
                            ForeColor="#ff1717"
                            Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <span class="fw-bold">แนบไฟล์<span class="text-danger">*</span></span>
                    </div>
                    <div class="col-lg-6 col-12 border-start">
                        <asp:FileUpload ID="FileUploadFile" runat="server" CssClass="form-control form-control-sm" />
                        <asp:RequiredFieldValidator runat="server"
                            ControlToValidate="FileUploadFile"
                            ErrorMessage="กรุณาแนบไฟล์."
                            SetFocusOnError="true"
                            ForeColor="#ff1717" 
                            Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>
                </div>
            </div>

            <p class="m-0 text-secondary">ส่วนผู้อนุมัติ</p>
            <div class="py-2 px-3">
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <span class="fw-bold">ผู้สร้าง</span>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:TextBox ID="TxtUserRequest" runat="server" CssClass="form-control form-control-sm" Style="width: 300px;" Enabled="false" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <span class="fw-bold">หน่วยงานผู้สร้าง</span>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:TextBox ID="TxtDepartmentRequest" runat="server" CssClass="form-control form-control-sm" Style="width: 300px;" Enabled="false" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <span class="fw-bold">ผู้ตรวจสอบ</span>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:DropDownList ID="DDListAcceptLeader" runat="server" CssClass="form-select form-select-sm" Style="width: 300px;" DataValueField="UserID" DataTextField="Name">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <span class="fw-bold">ผู้อนุมัติ</span>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:DropDownList ID="DDListApprove" runat="server" CssClass="form-select form-select-sm" Style="width: 300px;" DataValueField="UserID" DataTextField="Name">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="text-center">
                <asp:Button ID="BtnSave" runat="server" Text="บันทึกและส่งแจ้งเตือน" CssClass="btn btn-primary" OnClick="BtnSave_Click" />
            </div>
        </div>
    </div>
</asp:Content>
