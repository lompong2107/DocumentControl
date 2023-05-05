<%@ Page Title="รายงาน Kaizen | Document Control" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="KaizenReport.aspx.cs" Inherits="DocumentControl.DocumentRequest.KaizenReport.KaizenReport" EnableEventValidation="false" %>

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
                <h5 class="fw-bold">รายงาน Kaizen Report</h5>
            </div>

            <div class="border rounded p-2 bg-white">
                <div class="d-flex justify-content-end mb-2">
                    <div class="d-flex">
                        <div class="d-flex me-2 align-items-end">
                            <span class="text-nowrap me-1">ไตรมาส</span>
                            <asp:DropDownList ID="DDListQuarter" runat="server" CssClass="form-select form-select-sm">
                                <asp:ListItem Value="1" Text="Q1"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Q2"></asp:ListItem>
                                <asp:ListItem Value="3" Text="Q3"></asp:ListItem>
                                <asp:ListItem Value="4" Text="Q4"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="d-flex me-2 align-items-end">
                            <asp:TextBox ID="TxtYear" runat="server" CssClass="form-control form-control-sm yearpicker"></asp:TextBox>
                            <asp:Button ID="BtnShow" runat="server" Text="แสดง" CssClass="btn btn-sm btn-primary ms-1" OnClick="BtnShow_Click" />
                        </div>
                        <div class="d-flex me-2 align-items-end">
                            <span class="text-nowrap me-1">หน่วยงาน : </span>
                            <asp:DropDownList ID="DDListDepartment" runat="server" DataValueField="DepartmentID" DataTextField="DepartmentName" CssClass="form-select form-select-sm" OnSelectedIndexChanged="DDListDepartment_SelectedIndexChanged" OnDataBound="DDListDepartment_DataBound" AutoPostBack="true"></asp:DropDownList>
                        </div>
                        <div class="d-flex me-2 align-items-end">
                            <span class="text-nowrap me-1">สถานะ : </span>
                            <asp:DropDownList ID="DDListStatus" runat="server" DataValueField="KaizenStatusID" DataTextField="KaizenStatusDetail" CssClass="form-select form-select-sm" OnSelectedIndexChanged="DDListStatus_SelectedIndexChanged" OnDataBound="DDListStatus_DataBound" AutoPostBack="true"></asp:DropDownList>
                        </div>
                        <div class="d-flex align-items-end">
                            <span class="text-nowrap me-1">แถว :</span>
                            <asp:DropDownList ID="DDListPagingKaizen" runat="server" Width="80" CssClass="form-select form-select-sm d-inline-block" AutoPostBack="true" OnSelectedIndexChanged="DDListPagingKaizen_SelectedIndexChanged">
                                <asp:ListItem Value="10" Text="10"></asp:ListItem>
                                <asp:ListItem Value="20" Text="20" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="50" Text="50"></asp:ListItem>
                                <asp:ListItem Value="100" Text="100"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="d-flex justify-content-end align-items-end mb-1">
                    <span class="me-1 text-nowrap">ค้นหา:</span>
                    <%-- ค้นหา --%>
                    <asp:TextBox ID="TxtSearch" runat="server" CssClass="form-control form-control-sm" Width="500" placeholder="ค้นหา ชื่อเรื่อง, หน่วยงาน"></asp:TextBox>
                </div>

                <div>
                    <asp:UpdatePanel ID="UpdatePanelGridView" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <%-- ค้นหา --%>
                            <asp:Button ID="btnInvisibleSearch" runat="server" Style="display: none" OnClick="btnInvisibleSearch_Click" />
                            <asp:GridView ID="GVKaizen" runat="server" CssClass="table table-borderless border-0 table-hover" AutoGenerateColumns="false"
                                DataSourceID="SqlDatasourceKaizen" DataKeyNames="KaizenID"
                                AllowPaging="true" AllowSorting="true"
                                EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true"
                                OnRowDataBound="GVKaizen_RowDataBound" OnSelectedIndexChanged="GVKaizen_SelectedIndexChanged">
                                <Columns>
                                    <asp:TemplateField HeaderText="ชื่อเรื่อง" SortExpression="KaizenTopic">
                                        <ItemTemplate>
                                            <%# Eval("KaizenTopic") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="หน่วยงาน" SortExpression="DepartmentName">
                                        <ItemTemplate>
                                            <%# Eval("DepartmentName") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="วันที่สร้าง" SortExpression="DateCreate">
                                        <ItemTemplate>
                                            <%# DateTime.Parse(Eval("DateCreate").ToString()).ToString("dd/MM/yyyy") %>
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
                                </Columns>
                                <HeaderStyle CssClass="border-bottom" HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="Black" />
                                <RowStyle BackColor="White" CssClass="border-bottom border-light" VerticalAlign="Middle" />
                                <PagerStyle HorizontalAlign="Center" BackColor="White" />
                                <EmptyDataRowStyle HorizontalAlign="Center" BackColor="White" />
                                <SortedAscendingHeaderStyle CssClass="sortasc" />
                                <SortedDescendingHeaderStyle CssClass="sortdesc" />
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnInvisibleSearch" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            $(".yearpicker").yearpicker({

            });

            /* ค้นหา */
            $("#<%= TxtSearch.ClientID %>").keyup(function () {
                $("#<%= btnInvisibleSearch.ClientID %>").click();
            })
        })
    </script>

    <%-- SqlDataSource --%>
    <%-- Kaizen --%>
    <asp:SqlDataSource ID="SqlDataSourceKaizen" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT DC_Kaizen.KaizenID, DC_Kaizen.KaizenTopic, DC_Kaizen.DateCreate, F2_Department.DepartmentName, DC_KaizenStatus.KaizenStatusDetail, DC_Kaizen.KaizenStatusID, DC_Kaizen.Remark
        FROM DC_Kaizen
        LEFT JOIN DC_KaizenStatus ON DC_Kaizen.KaizenStatusID = DC_KaizenStatus.KaizenStatusID
        LEFT JOIN F2_Department ON DC_Kaizen.DepartmentID = F2_Department.DepartmentID 
        WHERE DC_Kaizen.KaizenStatusID LIKE @Status AND DC_Kaizen.KaizenStatusID != 0 AND DC_Kaizen.DepartmentID LIKE @DepartmentID 
        AND YEAR(DC_Kaizen.DateCreate) = @Year 
        AND MONTH(DC_Kaizen.DateCreate) BETWEEN CASE WHEN @Quarter = '1' THEN '1' WHEN @Quarter = '2' THEN '4' WHEN @Quarter = '3' THEN '7' ELSE '10' END AND CASE WHEN @Quarter = '1' THEN '3' WHEN @Quarter = '2' THEN '6' WHEN @Quarter = '3' THEN '9' ELSE '12' END
        AND ((DC_Kaizen.KaizenTopic LIKE CASE WHEN @Search = '%' THEN '%' ELSE '%' + @Search + '%' END) OR (F2_Department.DepartmentName LIKE CASE WHEN @Search = '%' THEN '%' ELSE '%' + @Search + '%' END))">
        <SelectParameters>
            <asp:ControlParameter Name="Status" ControlID="DDListStatus" PropertyName="SelectedValue" />
            <asp:ControlParameter Name="DepartmentID" ControlID="DDListDepartment" PropertyName="SelectedValue" />
            <asp:ControlParameter Name="Quarter" ControlID="DDListQuarter" PropertyName="SelectedValue" />
            <asp:ControlParameter Name="Year" ControlID="TxtYear" PropertyName="Text" />
            <asp:ControlParameter Name="Search" ControlID="TxtSearch" DefaultValue="%" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>
