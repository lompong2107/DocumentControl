<%@ Page Title="ประวัติการสร้างใบ Kaizen | Document Control" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="KaizenHistory.aspx.cs" Inherits="DocumentControl.DocumentRequest.KaizenReport.KaizenHistory" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .bg-warning {
            background-color: #FCF6BD !important;
        }

        .bg-success {
            background-color: #D0F4DE !important;
        }

        .bg-danger {
            background-color: #F898A4 !important;
        }

        .bg-info {
            background-color: #C0E4F6 !important;
        }

        .sortasc a:after {
            margin-left: 10px;
            display: inline-block;
            content: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" height="15"><path d="M207.029 381.476L12.686 187.132c-9.373-9.373-9.373-24.569 0-33.941l22.667-22.667c9.357-9.357 24.522-9.375 33.901-.04L224 284.505l154.745-154.021c9.379-9.335 24.544-9.317 33.901.04l22.667 22.667c9.373 9.373 9.373 24.569 0 33.941L240.971 381.476c-9.373 9.372-24.569 9.372-33.942 0z"/></svg>');
            transform: rotate(180deg);
        }

        .sortdesc a:after {
            margin-left: 10px;
            display: inline-block;
            content: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" height="15"><path d="M207.029 381.476L12.686 187.132c-9.373-9.373-9.373-24.569 0-33.941l22.667-22.667c9.357-9.357 24.522-9.375 33.901-.04L224 284.505l154.745-154.021c9.379-9.335 24.544-9.317 33.901.04l22.667 22.667c9.373 9.373 9.373 24.569 0 33.941L240.971 381.476c-9.373 9.372-24.569 9.372-33.942 0z"/></svg>');
        }

        .table a {
            text-decoration: none;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container pt-3 pb-2">
        <div class="border rounded bg-white p-3">
            <div class="text-center">
                <h5 class="fw-bold">ประวัติการสร้างใบ Kaizen</h5>
            </div>

            <div class="border rounded p-2 bg-white">
                <div class="d-flex justify-content-end mb-2">
                    <div class="d-flex">
                        <div class="d-flex me-2 align-items-end">
                            <span class="text-nowrap me-1">สถานะ : </span>
                            <asp:DropDownList ID="DDListStatus" runat="server" DataValueField="KaizenStatusID" DataTextField="KaizenStatusDetail" CssClass="form-select form-select-sm" OnSelectedIndexChanged="DDListStatus_SelectedIndexChanged" OnDataBound="DDListStatus_DataBound" AutoPostBack="true"></asp:DropDownList>
                        </div>
                        <div class="d-flex align-items-end">
                            <span class="text-nowrap me-1">แถว :</span>
                            <asp:DropDownList ID="DDListPagingKaizen" runat="server" Width="80" CssClass="form-select form-select-sm d-inline-block" AutoPostBack="true" OnSelectedIndexChanged="DDListPagingKaizen_SelectedIndexChanged">
                                <asp:ListItem Value="10" Text="10" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="20" Text="20"></asp:ListItem>
                                <asp:ListItem Value="50" Text="50"></asp:ListItem>
                                <asp:ListItem Value="100" Text="100"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <%-- สถานะ --%>
                <div class="d-flex justify-content-end mb-1">
                    <div class="badge rounded-pill text-dark me-1">
                        <span>รายการทั้งหมด</span>
                        <asp:Label ID="LbKaizenAll" runat="server" Text="0" CssClass="ms-1"></asp:Label>
                    </div>
                    <div class="badge rounded-pill bg-success text-dark me-1">
                        <span>อนุมัติแล้ว</span>
                        <asp:Label ID="LbKaizenCompleted" runat="server" Text="0" CssClass="ms-1"></asp:Label>
                    </div>
                    <div class="badge rounded-pill bg-warning text-dark me-1">
                        <span>รอตรวจสอบ,รออนุมัติ</span>
                        <asp:Label ID="LbKaizenApprove" runat="server" Text="0" CssClass="ms-1"></asp:Label>
                    </div>
                    <div class="badge rounded-pill bg-danger text-dark me-1">
                        <span>ไม่อนุมัติ</span>
                        <asp:Label ID="LbKaizenDisApprove" runat="server" Text="0" CssClass="ms-1"></asp:Label>
                    </div>
                </div>

                <asp:GridView ID="GVKaizen" runat="server" CssClass="table table-borderless border-0 table-hover" AutoGenerateColumns="false" 
                    DataSourceID="SqlDataSourceKaizen" DataKeyNames="KaizenID"
                    AllowPaging="true" AllowSorting="true" 
                    EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true"
                    OnRowDataBound="GVKaizen_RowDataBound" OnRowCommand="GVKaizen_RowCommand" OnSelectedIndexChanged="GVKaizen_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField HeaderText="#">
                            <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ชื่อเรื่อง" SortExpression="KaizenTopic">
                            <ItemTemplate>
                                <%# Eval("KaizenTopic") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="วันที่สร้าง" SortExpression="DateCreate">
                            <ItemTemplate>
                                <%# DateTime.Parse(Eval("DateCreate").ToString()).ToString("dd/MM/yyyy") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="หน่วยงาน" SortExpression="DepartmentName">
                            <ItemTemplate>
                                <%# Eval("DepartmentName") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ผู้ตรวจสอบ">
                            <ItemTemplate>
                                <%# Eval("LeaderName") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ผู้อนุมัติ">
                            <ItemTemplate>
                                <%# Eval("ApproveName") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="สถานะ" SortExpression="KaizenStatusDetail">
                            <ItemTemplate>
                                <asp:Panel ID="PanelStatus" runat="server" CssClass="rounded p-1">
                                    <%# Eval("KaizenStatusDetail") + " " + Eval("Remark")  %>
                                </asp:Panel>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="จัดการ">
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageBtnEdit" runat="server" CssClass="shadow-hover" CommandName="BtnEdit" CommandArgument='<%# Eval("KaizenID") %>' Height="30" ImageUrl="~/Image/edit.png" ToolTip="แก้ไข" />
                                <asp:ImageButton ID="ImageBtnDelete" runat="server" CssClass="shadow-hover" CommandName="BtnDelete" CommandArgument='<%# Eval("KaizenID") %>' Height="30" ImageUrl="~/Image/delete.png" OnClientClick="return alertConfirm(this, 'ลบ!', 'ต้องการลบใบ Kaizen?', 'question', 'ลบ!', 'ยกเลิก');" ToolTip="ลบ" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="border-bottom" HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="Black" />
                    <RowStyle BackColor="White" CssClass="border-bottom border-light" VerticalAlign="Middle" />
                    <PagerStyle HorizontalAlign="Center" BackColor="White" />
                    <EmptyDataRowStyle HorizontalAlign="Center" BackColor="White" />
                    <SortedAscendingHeaderStyle CssClass="sortasc" />
                    <SortedDescendingHeaderStyle CssClass="sortdesc" />
                </asp:GridView>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            $(".ListBoxSelect2").select2();
        })
    </script>

     <%-- SqlDataSource --%>
    <%-- Kaizen --%>
    <asp:SqlDataSource ID="SqlDataSourceKaizen" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT DC_Kaizen.KaizenID, DC_Kaizen.KaizenTopic, DC_Kaizen.DateCreate
        , (LeaderUser.FirstNameTH + ' ' + LeaderUser.LastNameTH) AS LeaderName, (ApproveUser.FirstNameTH + ' ' + ApproveUser.LastNameTH) AS ApproveName, F2_Department.DepartmentName
        ,  DC_KaizenStatus.KaizenStatusDetail, DC_Kaizen.KaizenStatusID, DC_Kaizen.Remark
        FROM DC_Kaizen
        LEFT JOIN DC_KaizenStatus ON DC_Kaizen.KaizenStatusID = DC_KaizenStatus.KaizenStatusID
        LEFT JOIN DC_LeaderAccept ON DC_Kaizen.LeaderAcceptID = DC_LeaderAccept.LeaderAcceptID
        LEFT JOIN F2_Users AS LeaderUser ON DC_LeaderAccept.UserID = LeaderUser.UserID
        LEFT JOIN DC_Approve ON DC_Kaizen.ApproveID = DC_Approve.ApproveID
        LEFT JOIN F2_Users AS ApproveUser ON DC_Approve.UserID = ApproveUser.UserID
        LEFT JOIN F2_Department ON DC_Kaizen.DepartmentID = F2_Department.DepartmentID 
        WHERE DC_Kaizen.UserID = @UserID AND DC_Kaizen.KaizenStatusID LIKE @Status">
        <SelectParameters>
            <asp:SessionParameter Name="UserID" SessionField="UserID" ConvertEmptyStringToNull="true" />
            <asp:ControlParameter Name="Status" ControlID="DDListStatus" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>
