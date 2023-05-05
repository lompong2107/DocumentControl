<%@ Page Title="ผู้ดูแล | Document Control" Language="C#" MasterPageFile="~/Admin/Navbar.Master" AutoEventWireup="true" CodeBehind="Publish.aspx.cs" Inherits="DocumentControl.Admin.Publish" Culture="en-US" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .CheckBoxCustom input {
            margin-right: 3px;
        }

        .scroll-custom::-webkit-scrollbar {
            width: 5px;
            height: 5px;
        }

        .scroll-custom::-webkit-scrollbar-track {
            background: #ddd;
        }

        .scroll-custom::-webkit-scrollbar-thumb {
            background: #212529;
        }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {
            // Set Scroll PublishTopic
            var div = document.getElementById("PublishTopicScroll");
            var div_position = document.getElementById("div_position");
            var position = parseInt('<%=Request.Form["div_position"] %>');
            if (isNaN(position)) {
                position = 0;
            }
            div.scrollTop = position;
            div.onscroll = function () {
                div_position.value = div.scrollTop;
                console.log(div.scrollTop)
            };
        })
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%-- เก็บค่า --%>
    <asp:HiddenField ID="HFNodeSelected" runat="server" />
    <%-- ค่า Scroll ของ PublishTopic --%>
    <input type="hidden" id="div_position" name="div_position" />

    <div class="container pt-3 pb-2">
        <div class="text-center">
            <h5 class="fw-bold">เอกสารแจกจ่าย เผยแพร่</h5>
        </div>

        <div class="row gx-3">
            <div class="col-5">
                <div class="p-3 border shadow-sm rounded bg-white h-100">
                    <div class="d-flex flex-column" style="height: calc(100vh - 140px);">
                        <div>
                            <asp:CheckBox ID="CBExpandAll" runat="server" Text="แสดง/ซ่อน หัวข้อทั้งหมด" OnCheckedChanged="CBExpandAll_CheckedChanged" AutoPostBack="true" Checked="true" CssClass="CheckBoxCustom text-black" />
                        </div>
                        <div id="PublishTopicScroll" class="overflow-auto scroll-custom">
                            <asp:TreeView ID="TreeViewPublishTopic" runat="server" NodeWrap="true" ShowLines="true" OnSelectedNodeChanged="TreeViewPublishTopic_SelectedNodeChanged" ExpandDepth="1">
                                <LevelStyles>
                                    <asp:TreeNodeStyle Font-Bold="true" />
                                    <asp:TreeNodeStyle Font-Bold="true" />
                                </LevelStyles>
                                <SelectedNodeStyle ForeColor="Black" />
                            </asp:TreeView>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-7">
                <div class="p-3 border shadow-sm rounded bg-white">
                    <asp:Panel ID="PanelDoc" runat="server" Visible="false" CssClass="d-flex flex-column" Style="height: calc(100vh - 140px);">
                        <div class="d-flex flex-wrap mb-3">
                            <div class="me-2">
                                <asp:LinkButton ID="LinkBtnPublishTopic" runat="server" Height="30" Font-Size="Larger" OnClick="LinkBtnPublishTopic_Click"></asp:LinkButton>
                            </div>
                            <div>
                                <asp:ImageButton ID="BtnReturn" runat="server" ImageUrl="~/Image/return.png" Height="25" ToolTip="เปลี่ยนสถานะ" OnClick="BtnReturn_Click" OnClientClick="return alertConfirm(this, 'อัปเดตสถานะ!', 'ต้องการคืนค่าหรือไม่?', 'question', 'ยืนยัน!', 'ยกเลิก');" />
                            </div>
                        </div>
                        <div class="overflow-auto scroll-custom">
                            <asp:GridView ID="GVPublishDoc" runat="server" CssClass="w-100 table table-hover border-0" AutoGenerateColumns="false" OnRowCommand="GVPublishDoc_RowCommand" OnRowDataBound="GVPublishDoc_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="ชื่อเอกสาร">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="HLPublishDocName" runat="server" NavigateUrl='<%# "PublishShowPDF.aspx?PublishDocFileID=" + Eval("PublishDocFileID") %>' Target="_blank" CssClass="link-primary"><%# Eval("FileName") %></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="คำสั่ง">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="BtnReturn" runat="server" Height="25" CommandName="BtnReturn" CommandArgument='<%# Eval("PublishDocID") %>' ImageUrl="~/Image/return.png" CssClass="align-middle shadow-hover" ToolTip="ย้อนกลับ" OnClientClick="return alertConfirm(this, 'อัปเดตสถานะ!', 'ต้องการคืนค่าหรือไม่?', 'question', 'ยืนยัน!', 'ยกเลิก');" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    ไม่พบข้อมูล
                                </EmptyDataTemplate>
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                            </asp:GridView>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="PanelSelectedPublishTopicEmpty" runat="server">
                        <p class="text-center mb-0 text-secondary">ยังไม่เลือกหัวข้อ</p>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
