<%@ Page Title="รายการร้องขอทั้งหมด | Document Control" Language="C#" MasterPageFile="~/DocumentRequest/Navbar.Master" AutoEventWireup="true" CodeBehind="~/DocumentRequest/Approve.aspx.cs" Inherits="DocumentControl.DocumentRequest.Approve" Culture="en-US" EnableEventValidation="false" %>

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
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container pt-3 pb-2">
        <div class="border rounded bg-white p-3">
            <div class="text-center">
                <h5 class="fw-bold d-inline-block">รายการอนุมัติทั้งหมด</h5>
                <asp:Label ID="LbCountApproveAll" runat="server" Text="99" BackColor="Red" ForeColor="White" Font-Bold="true" Font-Size="Small" CssClass="rounded-circle text-center align-top" Width="20" Height="20"></asp:Label>
            </div>

            <%-- แสดงไม่มีการแจ้งเตือน --%>
            <asp:Panel ID="PanelEmptyNotification" runat="server" Visible="false">
                <p class="mb-0 text-secondary text-center">ไม่มีการแจ้งเตือน</p>
            </asp:Panel>

            <%-- รายการอนุมัติ Request DAR --%>
            <asp:Panel ID="PanelRequestDAR" runat="server" Visible="false">
                <div class="mb-1">
                    <span class="fw-bold fs-5">คำร้องขอแก้ไขเอกสาร</span>
                    <asp:Label ID="LbCountApproveRequestDAR" runat="server" Text="99" BackColor="Red" ForeColor="White" Font-Bold="true" Font-Size="Small" CssClass="rounded-circle text-center align-top" Width="20" Height="20"></asp:Label>
                </div>
                <asp:GridView ID="GVRequestDAR" runat="server" CssClass="table table-borderless border-0 table-hover" AutoGenerateColumns="false"
                    AllowPaging="true" AllowSorting="true"
                    EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true"
                    OnRowDataBound="GVRequestDAR_RowDataBound" OnSelectedIndexChanged="GVRequestDAR_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField HeaderText="เลขที่ DAR">
                            <ItemTemplate>
                                <%# int.Parse(Eval("RequestDARID").ToString()).ToString("D3") %>
                                <asp:HiddenField ID="HFRequestDARID" runat="server" Value='<%# Eval("RequestDARID") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ผู้ร้องขอ">
                            <ItemTemplate>
                                <%# Eval("Name") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ชนิดของเอกสาร">
                            <ItemTemplate>
                                <%# Eval("DocTypeName") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="การดำเนินการ">
                            <ItemTemplate>
                                <%# Eval("OperationName") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="วันที่ร้องขอ">
                            <ItemTemplate>
                                <%# DateTime.Parse(Eval("DateRequest").ToString()).ToString("dd/MM/yyyy") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="สถานะ">
                            <ItemTemplate>
                                <%-- แสดง Remark ตามสถานะ (ยกเลิก, ไม่อนุมัติ) --%>
                                <asp:Panel ID="PanelStatus" runat="server" CssClass="rounded p-1" style="max-width: 400px;">
                                    <%# Eval("RequestDARStatusDetail") + " " + (Eval("RequestDARStatusID").ToString() == "0" ? Eval("RemarkCancel") : (Eval("RequestDARStatusID").ToString() == "4" ? Eval("Remark") : "")) %>
                                </asp:Panel>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="border-bottom" HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="Black" />
                    <RowStyle BackColor="White" CssClass="border-bottom border-light" VerticalAlign="Middle" />
                    <PagerStyle HorizontalAlign="Center" BackColor="White" />
                    <EmptyDataRowStyle HorizontalAlign="Center" BackColor="White" />
                </asp:GridView>
            </asp:Panel>

            <%-- รายการอนุมัติ Request Specification & Drawing --%>
            <asp:Panel ID="PanelRequestSpec" runat="server" Visible="false" CssClass="mt-3">
                <div class="mb-1">
                    <span class="fw-bold fs-5">คำร้องขอ Specification & Drawing</span>
                    <asp:Label ID="LbCountApproveRequestSpec" runat="server" Text="99" BackColor="Red" ForeColor="White" Font-Bold="true" Font-Size="Small" CssClass="rounded-circle text-center align-top" Width="20" Height="20"></asp:Label>
                </div>
                <asp:GridView ID="GVRequestSpec" runat="server" CssClass="table table-borderless border-0 table-hover" AutoGenerateColumns="false"
                    AllowPaging="true" AllowSorting="true"
                    EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true"
                    OnRowDataBound="GVRequestSpec_RowDataBound" OnSelectedIndexChanged="GVRequestSpec_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField HeaderText="อ้างอิง ECR NO">
                            <ItemTemplate>
                                <%# int.Parse(Eval("RequestSpecID").ToString()).ToString("D3") %>
                                <asp:HiddenField ID="HFRequestSpecID" runat="server" Value='<%# Eval("RequestSpecID") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ผู้ร้องขอ">
                            <ItemTemplate>
                                <%# Eval("Name") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ชนิดของเอกสาร">
                            <ItemTemplate>
                                <%# Eval("DocTypeName") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="การดำเนินการ">
                            <ItemTemplate>
                                <%# Eval("OperationName") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="วันที่ร้องขอ">
                            <ItemTemplate>
                                <%# DateTime.Parse(Eval("DateRequest").ToString()).ToString("dd/MM/yyyy") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="สถานะ">
                            <ItemTemplate>
                                <%-- แสดง Remark ตามสถานะ (ยกเลิก, ไม่อนุมัติหัวหน้า, ไม่อนุมัติ NPD) --%>
                                <asp:Panel ID="PanelStatus" runat="server" CssClass="rounded p-1" Style="max-width: 400px;">
                                    <%# Eval("RequestSpecStatusDetail") + " " + (Eval("RequestSpecStatusID").ToString() == "0" ? Eval("RemarkCancel") : (Eval("RequestSpecStatusID").ToString() == "3" ? Eval("RemarkLeader") : (Eval("RequestSpecStatusID").ToString() == "6" ? "RemarkApprove" : "")))  %>
                                </asp:Panel>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="border-bottom" HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="Black" />
                    <RowStyle BackColor="White" CssClass="border-bottom border-light" VerticalAlign="Middle" />
                    <PagerStyle HorizontalAlign="Center" BackColor="White" />
                    <EmptyDataRowStyle HorizontalAlign="Center" BackColor="White" />
                </asp:GridView>
            </asp:Panel>

            <%-- รายการอนุมัติ Kaizen Report --%>
            <asp:Panel ID="PanelKaizen" runat="server" Visible="false" CssClass="mt-3">
                <div class="mb-1">
                    <span class="fw-bold fs-5">Kaizen Report</span>
                    <asp:Label ID="LbCountApproveKaizen" runat="server" Text="99" BackColor="Red" ForeColor="White" Font-Bold="true" Font-Size="Small" CssClass="rounded-circle text-center align-top" Width="20" Height="20"></asp:Label>
                </div>
                <asp:GridView ID="GVKaizen" runat="server" CssClass="table table-borderless border-0 table-hover" AutoGenerateColumns="false"
                    AllowPaging="true" AllowSorting="true"
                    EmptyDataText="ยังไม่มีข้อมูล" ShowHeaderWhenEmpty="true"
                    OnRowDataBound="GVKaizen_RowDataBound" OnSelectedIndexChanged="GVKaizen_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField HeaderText="ชื่อเรื่อง">
                            <ItemTemplate>
                                <%# Eval("KaizenTopic") %>
                                <asp:HiddenField ID="HFKaizenID" runat="server" Value='<%# Eval("KaizenID") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="หน่วยงาน">
                            <ItemTemplate>
                                <%# Eval("DepartmentName") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ผู้สร้าง">
                            <ItemTemplate>
                                <%# Eval("Name") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="วันที่สร้าง">
                            <ItemTemplate>
                                <%# DateTime.Parse(Eval("DateCreate").ToString()).ToString("dd/MM/yyyy") %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="สถานะ">
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
                </asp:GridView>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
