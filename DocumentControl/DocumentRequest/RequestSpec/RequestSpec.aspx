<%@ Page Title="แบบแจ้งการดำเนินการเอกสารวิศวกรรม | Document Control" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="RequestSpec.aspx.cs" Inherits="DocumentControl.DocumentRequest.RequestSpec.RequestSpec" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .CheckBoxCustom input {
            margin-right: 3px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container pt-3 pb-2">
        <div class="border rounded p-3 bg-white">
            <div class="text-center">
                <h5 class="fw-bold">แบบฟอร์มแจ้งการดำเนินการเอกสารวิศวกรรม<br />
                    (Specification & Drawing)</h5>
            </div>

            <p class="m-0 text-secondary">รายละเอียดเอกสาร</p>
            <div class="py-2 px-3">
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">เอกสารทางวิศวกรรมที่ต้องการแจ้งการดำเนินการ<span class="text-danger">*</span></label>
                    </div>
                    <div class="col-lg-4 col-12 border-start">
                        <asp:DropDownList ID="DDListDocType" runat="server" DataSourceID="SqlDataSourceDocType" DataValueField="RequestSpecDocTypeID" DataTextField="DocTypeName" CssClass="form-select form-select-sm"></asp:DropDownList>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">ประเภทของการแจ้งการดำเนินการเอกสาร<span class="text-danger">*</span></label>
                    </div>
                    <div class="col-lg-4 col-12 border-start">
                        <asp:DropDownList ID="DDListOperation" runat="server" DataSourceID="SqlDataSourceOperation" DataValueField="RequestSpecOperationID" DataTextField="OperationName" CssClass="form-select form-select-sm" OnSelectedIndexChanged="DDListOperation_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    </div>
                    <div class="col-lg-4 col-12">
                        <asp:TextBox ID="TxtOperationOther" runat="server" CssClass="form-control form-control-sm" Enabled="false" placeholder="ระบุ การดำเนินการอื่นๆ"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorOperationOther" runat="server"
                            ControlToValidate="TxtOperationOther"
                            ValidationGroup="SaveForm"
                            ErrorMessage="กรุณาระบุ การดำเนินการอื่นๆ."
                            SetFocusOnError="true"
                            ForeColor="#ff1717"
                            Display="Dynamic"
                            Enabled="false"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">ประเภทการเปลี่ยนแปลง<span class="text-danger">*</span></label>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:RadioButtonList ID="RBListTypeOfChange" runat="server" RepeatDirection="Horizontal" CssClass="CheckBoxCustom">
                            <asp:ListItem Value="1" Text="ภายนอก"></asp:ListItem>
                            <asp:ListItem Value="2" Text="ภายใน" Selected="True"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">Project Name</label>
                    </div>
                    <div class="col-lg-6 col-12 border-start">
                        <asp:DropDownList ID="DDListProject" runat="server" DataSourceID="SqlDataSourceProject" DataValueField="ProjectID" DataTextField="ProjectName" CssClass="form-select form-select-sm" AutoPostBack="true" OnDataBound="DDListProject_DataBound">
                            <%--<asp:ListItem Value="0" Text="-- เลือก Project --" Selected="True" disabled="disabled"></asp:ListItem>--%>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">Part Name</label>
                    </div>
                    <div class="col-lg-6 col-12 border-start">
                        <asp:DropDownList ID="DDListPart" runat="server" DataSourceID="SqlDataSourcePart" DataValueField="PartID" DataTextField="PartName" CssClass="form-select form-select-sm" AutoPostBack="true" OnDataBound="DDListPart_DataBound">
                            <%--<asp:ListItem Value="0" Text="-- เลือก Part --" Selected="True" disabled="disabled"></asp:ListItem>--%>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">Part NO.<span class="text-danger">*</span></label>
                    </div>
                    <div class="col-lg-6 col-12 border-start">
                        <asp:DropDownList ID="DDListFG" runat="server" DataSourceID="SqlDataSourceFG" DataValueField="FGID" DataTextField="FGName" CssClass="form-select form-select-sm" AutoPostBack="true" OnDataBound="DDListFG_DataBound">
                            <%--<asp:ListItem Value="0" Text="-- เลือก FG --" Selected="True" disabled="disabled"></asp:ListItem>--%>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorFG" runat="server"
                            ControlToValidate="DDListFG"
                            ValidationGroup="SaveForm"
                            ErrorMessage="กรุณาเลือก Part NO."
                            SetFocusOnError="true"
                            ForeColor="#ff1717"
                            Display="Dynamic"
                            InitialValue="0"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">รายละเอียดการแก้ไข/เพิ่มเติม<span class="text-danger">*</span></label>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:TextBox ID="TxtDetailRequest" runat="server" CssClass="form-control" TextMode="MultiLine" placeholder="รายละเอียดการแก้ไข/เพิ่มเติม"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorDetailRequest" runat="server"
                            ControlToValidate="TxtDetailRequest"
                            ValidationGroup="SaveForm"
                            ErrorMessage="กรุณาป้อนข้อมูล."
                            SetFocusOnError="true"
                            ForeColor="#ff1717"
                            Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="row gy-1 mb-3">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">เหตุผลที่ต้องแจ้งดำเนินการเอกสาร<span class="text-danger">*</span></label>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <asp:TextBox ID="TxtReasonRequest" runat="server" CssClass="form-control" TextMode="MultiLine" placeholder="เหตุผลที่ต้องแจ้งดำเนินการเอกสาร"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorReasonRequest" runat="server"
                            ControlToValidate="TxtReasonRequest"
                            ValidationGroup="SaveForm"
                            ErrorMessage="กรุณาป้อนข้อมูล."
                            SetFocusOnError="true"
                            ForeColor="#ff1717"
                            Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="row gy-1">
                    <div class="col-lg-4 col-12">
                        <label class="fw-bold">เอกสารแนบ<span class="text-danger">*</span></label>
                    </div>
                    <div class="col-lg-8 col-12 border-start">
                        <div class="input-group">
                            <asp:FileUpload ID="FileUploadFile" runat="server" CssClass="form-control form-control-sm" AllowMultiple="true" />
                            <asp:Button ID="BtnAddFile" runat="server" Text="เพิ่มใส่รายการไฟล์" CssClass="btn btn-sm btn-info" OnClick="BtnAddFile_Click" ValidationGroup="SelectedFile" />
                        </div>
                        <div>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorFileUpload" runat="server"
                                ControlToValidate="FileUploadFile"
                                ValidationGroup="SelectedFile"
                                ErrorMessage="กรุณาเลือกไฟล์."
                                SetFocusOnError="true"
                                ForeColor="#ff1717"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                        <div class="row mt-1">
                            <div class="col-lg-6 col-12">
                                <span class="text-secondary">รายการไฟล์</span>
                                <asp:LinkButton ID="LinkBtnDelFile" runat="server" Text="ลบรายการไฟล์" CssClass="link-danger" OnClick="LinkBtnDelFile_Click" CausesValidation="false" Visible="false"></asp:LinkButton>
                                <asp:ListBox ID="ListBoxFiles" runat="server" CssClass="form-control form-control-sm" OnSelectedIndexChanged="ListBoxFiles_SelectedIndexChanged" AutoPostBack="true"></asp:ListBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorFile" runat="server"
                                    ControlToValidate="ListBoxFiles"
                                    ValidationGroup="SaveForm"
                                    ErrorMessage="กรุณาแนบไฟล์."
                                    SetFocusOnError="true"
                                    ForeColor="#ff1717"
                                    Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>
                        </div>
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
                            <asp:TextBox ID="TxtUserRequest" runat="server" CssClass="form-control form-control-sm" Style="width: 300px;" Enabled="false" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row gy-1 mb-3">
                        <div class="col-lg-4 col-12">
                            <span class="fw-bold">ผู้อนุมัติ</span>
                        </div>
                        <div class="col-lg-8 col-12 border-start">
                            <asp:DropDownList ID="DDListAcceptLeader" runat="server" CssClass="form-select form-select-sm" Style="width: 300px;" DataValueField="UserID" DataTextField="Name">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <p class="m-0 text-secondary">NPD ตรวจสอบ</p>
                <div class="py-2 px-3">
                    <div class="row gy-1 mb-3">
                        <div class="col-lg-4 col-12">
                            <span class="fw-bold">วิศวกร</span>
                        </div>
                        <div class="col-lg-8 col-12 border-start">
                            <asp:DropDownList ID="DDListAcceptNPD" runat="server" CssClass="form-select form-select-sm" Style="width: 300px;" DataSourceID="SqlDataSourceUserNPD" DataValueField="UserID" DataTextField="Name">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row gy-1 mb-3">
                        <div class="col-lg-4 col-12">
                            <span class="fw-bold">ผู้อนุมัติ</span>
                        </div>
                        <div class="col-lg-8 col-12 border-start">
                            <asp:DropDownList ID="DDListApprove" runat="server" CssClass="form-select form-select-sm" Style="width: 300px;" DataSourceID="SqlDataSourceUserNPDSectionHead" DataValueField="UserID" DataTextField="Name">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="text-center">
                    <asp:Button ID="BtnSave" runat="server" Text="บันทึกและส่งแจ้งเตือน" CssClass="btn btn-primary" OnClick="BtnSave_Click" ValidationGroup="SaveForm" />
                </div>
            </div>
        </div>
    </div>

    <%-- SqlDataSource --%>
    <%-- เอกสารทางวิศวกรรมที่ต้องการแจ้งการดำเนินการ --%>
    <asp:SqlDataSource ID="SqlDataSourceDocType" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT RequestSpecDocTypeID, DocTypeName FROM DC_RequestSpecDocType WHERE Status = 1"></asp:SqlDataSource>
    <%-- ประเภทของการแจ้งการดำเนินการเอกสาร --%>
    <asp:SqlDataSource ID="SqlDataSourceOperation" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT RequestSpecOperationID, OperationName FROM DC_RequestSpecOperation WHERE Status = 1"></asp:SqlDataSource>
    <%-- Project --%>
    <asp:SqlDataSource ID="SqlDataSourceProject" runat="server" ConnectionString="<%$ ConnectionStrings:TCTFactoryConnectionString %>"
        SelectCommand="SELECT ProjectID, ProjectName FROM Project WHERE Status = 1 ORDER BY ProjectName"></asp:SqlDataSource>
    <%-- Part --%>
    <asp:SqlDataSource ID="SqlDataSourcePart" runat="server" ConnectionString="<%$ ConnectionStrings:TCTFactoryConnectionString %>"
        SelectCommand="SELECT PartID, PartName FROM Part WHERE Status = 1 AND ProjectID = @ProjectID ORDER BY PartName">
        <SelectParameters>
            <asp:ControlParameter ControlID="DDListProject" Name="ProjectID" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <%-- FG --%>
    <asp:SqlDataSource ID="SqlDataSourceFG" runat="server" ConnectionString="<%$ ConnectionStrings:TCTFactoryConnectionString %>"
        SelectCommand="SELECT FGID, FGName FROM FG WHERE Status = 1 AND PartID = @PartID ORDER BY FGName">
        <SelectParameters>
            <asp:ControlParameter ControlID="DDListPart" Name="PartID" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <%-- User In NPD วิศวกร --%>
    <asp:SqlDataSource ID="SqlDataSourceUserNPD" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT F2_Users.UserID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH + ' ' + ISNULL(DC_UserApprove.Remark, '')) AS Name 
        FROM DC_UserApprove 
        LEFT JOIN F2_Users ON DC_UserApprove.UserID = F2_Users.UserID WHERE DC_UserApprove.StatusPermission = 3 AND F2_Users.Status = 1"></asp:SqlDataSource>
    <%-- User In NPD --%>
    <%-- User In NPD Section Head (ผู้อนุมัติ) --%>
    <asp:SqlDataSource ID="SqlDataSourceUserNPDSectionHead" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT F2_Users.UserID, (F2_Users.FirstNameTH + ' ' + F2_Users.LastNameTH + ' ' + ISNULL(DC_UserApprove.Remark, '')) AS Name 
        FROM DC_UserApprove 
        LEFT JOIN F2_Users ON DC_UserApprove.UserID = F2_Users.UserID WHERE DC_UserApprove.StatusPermission = 4 AND F2_Users.Status = 1"></asp:SqlDataSource>
</asp:Content>
