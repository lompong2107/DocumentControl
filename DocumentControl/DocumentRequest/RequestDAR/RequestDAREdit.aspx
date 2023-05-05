<%@ Page Title="แก้ไขใบร้องขอ | Document Control" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="~/DocumentRequest/RequestDAR/RequestDAREdit.aspx.cs" Inherits="DocumentControl.DocumentRequest.RequestDAR.RequestDAREdit" Culture="en-US" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%-- เก็บค่า --%>
    <asp:HiddenField ID="HFRequestDARID" runat="server" />
    <%-- ค่า RequestDARID --%>
    <div class="container pt-3 pb-2">
        <div class="border rounded p-3 bg-white">
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
                </div>
            </div>

            <p class="m-0 text-secondary">รายละเอียดคำร้องขอ</p>
            <div class="py-2 px-3">
                <div class="row gy-1 mb-3">
                    <div class="col-md-4 col-lg-3 col-12">
                        <span class="fw-bold">ชนิดของเอกสาร<span class="text-danger">*</span></span>
                    </div>
                    <div class="col-md-4 col-12 border-start">
                        <asp:DropDownList ID="DDListRequestDARDocType" runat="server" DataValueField="RequestDARDocTypeID" DataTextField="DocTypeName" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="DDListRequestDARDocType_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div class="col-md-4 col-12">
                        <asp:TextBox ID="TxtDocTypeOther" runat="server" CssClass="form-control form-control-sm" Enabled="false" placeholder="ระบุ ชนิดของเอกสารอื่นๆ"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorDocTypeOther" runat="server"
                            ControlToValidate="TxtDocTypeOther"
                            ErrorMessage="กรุณาระบุ ชนิดของเอกสารอื่นๆ."
                            SetFocusOnError="true"
                            ForeColor="#ff1717"
                            Display="Dynamic"
                            Enabled="false"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-md-4 col-lg-3 col-12">
                        <span class="fw-bold">การดำเนินการ<span class="text-danger">*</span></span>
                    </div>
                    <div class="col-md-4 col-12 border-start">
                        <asp:DropDownList ID="DDListRequestDAROperation" runat="server" DataValueField="RequestDAROperationID" DataTextField="OperationName" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="DDListRequestDAROperation_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div class="col-md-4 col-12">
                        <asp:TextBox ID="TxtOperationOther" runat="server" CssClass="form-control form-control-sm" Enabled="false" placeholder="ระบุ การดำเนินการอื่นๆ"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorOperationOther" runat="server"
                            ControlToValidate="TxtOperationOther"
                            ErrorMessage="กรุณาระบุ การดำเนินการอื่นๆ."
                            SetFocusOnError="true"
                            ForeColor="#ff1717"
                            Display="Dynamic"
                            Enabled="false"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div>
                    <span class="fw-bold">รายการแก้ไข / เพิ่มเติม</span>
                </div>
                <div>
                    <asp:GridView ID="GVRequestDARDoc" runat="server" AutoGenerateColumns="false"
                        CssClass="table table-borderless border-0 mb-0"
                        EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true" OnRowCommand="GVRequestDARDoc_RowCommand" OnRowDataBound="GVRequestDARDoc_RowDataBound">
                        <Columns>
                            <asp:TemplateField HeaderText='เลขที่เอกสาร<span class="text-danger">*</span>'>
                                <ItemTemplate>
                                    <asp:HiddenField ID="HFRequestDARDocID" runat="server" Value='<%# Eval("RequestDARDocID") %>' />
                                    <asp:TextBox ID="TxtDocNumber" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("DocNumber") %>' placeholder="F-XX-XX"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorDocNumber" runat="server"
                                        ControlToValidate="TxtDocNumber"
                                        ErrorMessage="กรุณาระบุ เลขที่เอกสาร."
                                        SetFocusOnError="true"
                                        ForeColor="#ff1717"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText='ชื่อเอกสาร<span class="text-danger">*</span>'>
                                <ItemTemplate>
                                    <asp:TextBox ID="TxtDocName" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("DocName") %>' placeholder="เช่น แบบลงทะเบียนผู้เข้าอบรม"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorDocName" runat="server"
                                        ControlToValidate="TxtDocName"
                                        ErrorMessage="กรุณาระบุ ชื่อเอกสาร."
                                        SetFocusOnError="true"
                                        ForeColor="#ff1717"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText='วันที่บังคับใช้<span class="text-danger">*</span>'>
                                <ItemTemplate>
                                    <asp:TextBox ID="TxtDocDate" runat="server" CssClass="form-control form-control-sm datepicker" Text='<%# DateTime.ParseExact(Eval("DateEnforce").ToString(), "dd/MM/yyyy", null).ToString("dd/MM/yyyy") %>' placeholder="dd/MM/yyyy"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText='เหตุผลการร้องขอ<span class="text-danger">*</span>'>
                                <ItemTemplate>
                                    <asp:TextBox ID="TxtDocRemark" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("Remark") %>' placeholder="เหตุผล"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorDocRemark" runat="server"
                                        ControlToValidate="TxtDocRemark"
                                        ErrorMessage="กรุณาระบุ เหตุผลการร้องขอ."
                                        SetFocusOnError="true"
                                        ForeColor="#ff1717"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="เอกสารแนบ">
                                <ItemTemplate>
                                    <div class="d-flex">
                                        <asp:FileUpload ID="FileUploadFile" runat="server" CssClass="form-control form-control-sm" />
                                        <asp:ImageButton ID="ImageBtnDownload" CssClass="shadow-hover" runat="server" CommandName="BtnDownload" CommandArgument='<%# Eval("FilePath") %>' Height="30" ImageUrl="~/Image/download.png" CausesValidation="false" />
                                    </div>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageBtnDelete" runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("RequestDARDocID") %>' Height="25" ImageUrl="~/Image/cancel.png" CssClass="align-middle shadow-hover" CausesValidation="false" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="table-light border-bottom" HorizontalAlign="Center" VerticalAlign="Middle" />
                        <RowStyle CssClass="border-bottom border-light" />
                        <EmptyDataRowStyle HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
                <div class="text-end">
                    <asp:LinkButton ID="LinkBtnAddRequestDARDoc" runat="server" Text="+ เพิ่มช่อง" OnClick="LinkBtnAddRequestDARDoc_Click" CausesValidation="false"></asp:LinkButton>
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
                        <asp:TextBox ID="TxtUserRequest" runat="server" CssClass="form-control form-control-sm" Style="width: 300px;" Enabled="false" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-md-4 col-lg-3 col-12">
                        <span class="fw-bold">ผู้ตรวจสอบ</span>
                    </div>
                    <div class="col-md-8 col-lg-9 col-12 border-start">
                        <asp:DropDownList ID="DDListAcceptLeader" runat="server" CssClass="form-select form-select-sm" Style="width: 300px;" DataValueField="UserID" DataTextField="Name">
                        </asp:DropDownList>
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
                        <asp:DropDownList ID="DDListAcceptNPD" runat="server" CssClass="form-select form-select-sm" Style="width: 300px;" DataValueField="UserID" DataTextField="Name">
                        </asp:DropDownList>
                    </div>
                </asp:Panel>
                <div class="row gy-1 mb-3">
                    <div class="col-md-4 col-lg-3 col-12">
                        <span class="fw-bold">ผู้อนุมัติ</span>
                    </div>
                    <div class="col-md-8 col-lg-9 col-12 border-start">
                        <asp:DropDownList ID="DDListApprove" runat="server" CssClass="form-select form-select-sm" Style="width: 300px;" DataValueField="UserID" DataTextField="Name">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="d-flex justify-content-between">
                <asp:HyperLink ID="HLBack" runat="server" CssClass="btn btn-outline-secondary" Text="กลับ" NavigateUrl="~/DocumentRequest/RequestDAR/RequestDARHistory.aspx" Width="100"></asp:HyperLink>
                <asp:Button ID="BtnUpdate" runat="server" Text="บันทึก" CssClass="btn btn-success" OnClick="BtnUpdate_Click" Width="100" />
            </div>
        </div>
    </div>

    <script type="text/javascript">
        $(".datepicker").datepicker({
            showOtherMonths: true,
            showButtonPanel: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: "dd/mm/yy"
        });
    </script>
</asp:Content>
