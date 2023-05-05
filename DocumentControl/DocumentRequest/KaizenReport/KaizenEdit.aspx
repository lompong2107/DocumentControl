<%@ Page Title="แก้ไขใบ Kaizen | Document Control" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="KaizenEdit.aspx.cs" Inherits="DocumentControl.DocumentRequest.KaizenReport.KaizenEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
                </div>
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
                        <span class="fw-bold">แนบไฟล์</span>
                    </div>
                    <div class="col-lg-8 col-12 border-start d-flex align-items-center">
                        <asp:FileUpload ID="FileUploadFile" runat="server" CssClass="form-control form-control-sm" />
                        <asp:HyperLink ID="HLOpenFile" runat="server" Text="เปิดไฟล์" CssClass="btn btn-sm btn-primary ms-3 me-1" Target="_blank" Width="100"></asp:HyperLink>
                        <asp:Button ID="BtnDownload" runat="server" Text="ดาวน์โหลด" CssClass="btn btn-sm btn-secondary me-1" Width="100" OnClick="BtnDownload_Click" />
                        <asp:Button ID="BtnHistory" runat="server" Text="ประวัติ" CssClass="btn btn-sm btn-secondary" Width="100" OnClick="BtnHistory_Click" />
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
                        <asp:DropDownList ID="DDListDepartment" runat="server" CssClass="form-select form-select-sm" Style="width: 300px;" DataValueField="DepartmentID" DataTextField="DepartmentName">
                        </asp:DropDownList>
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
            <div class="d-flex justify-content-between">
                <asp:HyperLink ID="HLBack" runat="server" CssClass="btn btn-outline-secondary" Text="กลับ" NavigateUrl="~/DocumentRequest/KaizenReport/KaizenHistory.aspx" Width="100"></asp:HyperLink>
                <asp:Button ID="BtnUpdate" runat="server" Text="บันทึก" CssClass="btn btn-success" OnClick="BtnUpdate_Click" Width="100" />
            </div>
        </div>
    </div>
</asp:Content>
