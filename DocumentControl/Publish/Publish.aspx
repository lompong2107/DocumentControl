<%@ Page Title="เผยแพร่เอกสาร | Docuemnt Control" Language="C#" MasterPageFile="~/Publish/Navbar.Master" AutoEventWireup="true" CodeBehind="~/Publish/Publish.aspx.cs" Inherits="DocumentControl.Publish.Publish" Culture="en-US" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .CheckBoxCustom input {
            margin-right: 3px;
        }

        .CheckBoxCustom label {
            margin-right: 3px;
        }

        .scroll-custom::-webkit-scrollbar {
            width: 5px;
            height: 5px;
            background-color: transparent;
        }

        .scroll-custom::-webkit-scrollbar-track {
            background: #ddd;
            border-radius: 5px;
        }

        .scroll-custom::-webkit-scrollbar-thumb {
            background: #212529;
            border-radius: 5px;
        }

        .no-transition {
            transition: none !important;
        }

        .select2-container--default .select2-search--inline, .select2-container--default .select2-search--inline .select2-search__field {
            width: 100% !important;
        }
    </style>

    <script type="text/javascript">
        //Cross-browser function to select content
        function SelectText(element) {
            var doc = document;
            if (doc.body.createTextRange) {
                var range = document.body.createTextRange();
                range.moveToElementText(element);
                range.select();
            } else if (window.getSelection) {
                var selection = window.getSelection();
                var range = document.createRange();
                range.selectNodeContents(element);
                selection.removeAllRanges();
                selection.addRange(range);
            }
        }

        function CopyImage() {
            //Make the container Div contenteditable
            $(".copyable").attr("contenteditable", true);
            //Select the image
            SelectText($(".copyable").get(0));
            //Execute copy Command
            //Note: This will ONLY work directly inside a click listenner
            document.execCommand('copy');
            //Unselect the content
            window.getSelection().removeAllRanges();
            //Make the container Div uneditable again
            $(".copyable").removeAttr("contenteditable");
            //Success!!
            alertToast("คัดลอก QR Code แล้ว", "success");
        };

        function DownloadImage() {
            var a = document.createElement("a"); //Create <a>
            a.href = $(".copyable img").attr("src") //Image Base64 Goes here
            a.download = "QRCode.png"; //File name Here
            a.click(); //Downloaded file
            a.remove();
        }

        function CopyToClipboard(link) {
            //navigator.clipboard.writeText(link).then(function () {
            //    alertToast("คัดลอกลิงก์สำเร็จ", "success");
            //})

            // https://stackoverflow.com/questions/46445981/copy-element-text-to-clipboard-aspx
            var $temp = $("<input>");
            $("body").append($temp);
            $temp.val(link).select();
            document.execCommand("copy");
            $temp.remove();
            alertToast("คัดลอกลิงก์สำเร็จ", "success");
        }

        function OpenModalPublishTopicUploadFile() {
            new bootstrap.Modal($("#ModalPublishTopicUploadFile")).show();
        }

        function OpenModalPublishDocUploadFile() {
            new bootstrap.Modal($("#ModalPublishDocUploadFile")).show();
        }

        function OpenModalPublishTopicRename() {
            new bootstrap.Modal($("#ModalPublishTopicRename")).show();
        }

        function OpenModalPublishDocRename() {
            new bootstrap.Modal($("#ModalPublishDocRename")).show();
        }

        function OpenModalPublishTopicQRCode() {
            new bootstrap.Modal($("#ModalPublishTopicQRCode")).show();
        }

        function OpenOffcanvasDetailPublishTopic() {
            new bootstrap.Offcanvas($("#offcanvasDetailPublishTopic")).show();
        }

        function OpenOffcanvasDetailPublishDoc() {
            new bootstrap.Offcanvas($("#offcanvasDetailPublishDoc")).show();
        }

        $(document).ready(function () {
            function SetStart() {
                // Set Scroll PublishTopic
                var div = document.getElementById("PublishTopicScroll");
                var div_position = document.getElementById("div_position");
                var position = parseInt('<%=Request.Form["div_position"] %>');
                //var position = parseInt(document.getElementById("div_position").value);
                if (isNaN(position)) {
                    position = 0;
                }
                div.scrollTop = position;
                div.onscroll = function () {
                    div_position.value = div.scrollTop;
                    console.log(div.scrollTop)
                };

                // Select2
                $(".ListBoxSelect2").select2({
                    width: '100%',
                    placeholder: 'เลือกผู้ใช้ที่มีสิทธิ์เข้าถึงหัวข้อ',
                    allowClear: true
                });

                $("#<%=ListBoxPublishTopicUserAccess.ClientID%>").change(function () {
                    $("#<%=HFUserIDSelectedAccess.ClientID%>").val($(this).val())
                    console.log($("#<%=HFUserIDSelectedAccess.ClientID%>").val())
                }).change()
            }
            SetStart();

            // ดูมาจากลิงก์นี้ ถ้าจะใช้นะ
            // https://stackoverflow.com/questions/30184643/after-post-back-my-jquery-code-not-working
            // Postback ของ UpdatePanel จะได้ทำงานได้
            //var parameter = Sys.WebForms.PageRequestManager.getInstance();
            //parameter.add_endRequest(SetStart);

            var parameter = Sys.WebForms.PageRequestManager.getInstance();
            parameter.add_endRequest((sender, e) => {
                if (sender._postBackSettings.panelsToUpdate != null) {
                    // Select2
                    $(".ListBoxSelect2").select2({
                        width: '100%',
                        placeholder: 'เลือกผู้ใช้ที่มีสิทธิ์เข้าถึงหัวข้อ',
                        allowClear: true
                    });
                    // แก้ไขผู้ใช้ที่มีสิทธิ์เข้าถึงหัวข้อ
                    $("#<%=ListBoxEditPublishTopicUserAccess.ClientID%>").change(function () {
                        $("#<%=HFEditUserIDSelectedAccess.ClientID%>").val($(this).val())
                        console.log($("#<%=HFEditUserIDSelectedAccess.ClientID%>").val())
                    }).change()
                }
            });

            // แสดง/ซ่อน div ฟอร์มเพิ่มหัวข้อ เวลา Postback
            var div_toggleAddPublishTopic = $("#div_toggleAddPublishTopic");
            var stateVisibleAddPublishTopic = parseInt('<%=Request.Form["div_toggleAddPublishTopic"] %>');
            if (isNaN(stateVisibleAddPublishTopic)) {
                stateVisibleAddPublishTopic = 0;
            }
            div_toggleAddPublishTopic.val(stateVisibleAddPublishTopic)
            if (stateVisibleAddPublishTopic == 1) {
                $("#flush-collapseAddPublishTopic").addClass('no-transition').collapse("show");
                $("#flush-collapseAddPublishTopic").removeClass('no-transition')
            }
            $(".btn-toggle-addpublishtopic").click(function () {
                if (div_toggleAddPublishTopic.val() == 0) {
                    $("#div_toggleAddPublishTopic").val(1)
                } else {
                    $("#div_toggleAddPublishTopic").val(0)
                }
            })

            // แสดง/ซ่อน div ฟอร์มเพิ่มเผยแพร่เอกสาร เวลา Postback
            var div_toggleAddPublishDoc = $("#div_toggleAddPublishDoc");
            var stateVisiblePublishDoc = parseInt('<%=Request.Form["div_toggleAddPublishDoc"] %>');
            if (isNaN(stateVisiblePublishDoc)) {
                stateVisiblePublishDoc = 0;
            }
            div_toggleAddPublishDoc.val(stateVisiblePublishDoc)
            if (stateVisiblePublishDoc == 1) {
                $("#flush-collapseAddPublishDoc").addClass('no-transition').collapse("show");
                $("#flush-collapseAddPublishDoc").removeClass('no-transition')
            }
            $(".btn-toggle-addpublishdoc").click(function () {
                if (div_toggleAddPublishDoc.val() == 0) {
                    $("#div_toggleAddPublishDoc").val(1)
                } else {
                    $("#div_toggleAddPublishDoc").val(0)
                }
            })
        })
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%-- เก็บค่า --%>
    <%-- ค่า PublishTopicID --%>
    <asp:HiddenField ID="HFNodeSelected" runat="server" />
    <%-- ค่า Scroll ของช่อง หัวข้อ (PublishTopic) --%>
    <input type="hidden" id="div_position" name="div_position" />
    <%-- ค่า แสดงซ่อน div เพิ่มหัวข้อ 1=แสดง, 0=ซ่อน ไว้ใช้ตอน Postback --%>
    <input type="hidden" id="div_toggleAddPublishTopic" name="div_toggleAddPublishTopic" />
    <%-- ค่า แสดงซ่อน div เพิ่มเผยแพร่เอกสาร 1=แสดง, 0=ซ่อน ไว้ใช้ตอน Postback --%>
    <input type="hidden" id="div_toggleAddPublishDoc" name="div_toggleAddPublishDoc" />
    <div class="container-fluid py-3">
        <div class="row gx-3 mx-0">
            <div class="col-5">
                <div class="p-3 border shadow-sm rounded bg-white h-100">
                    <div class="d-flex flex-column" style="height: calc(100vh - 120px);">
                        <asp:Panel ID="accordionFlushAddPublishTopic" runat="server" class="accordion">
                            <div class="accordion-item">
                                <h2 class="accordion-header" id="flush-headingAddPublishTopic">
                                    <button class="accordion-button collapsed p-1 btn-toggle-addpublishtopic" type="button" data-bs-toggle="collapse" data-bs-target="#flush-collapseAddPublishTopic" aria-expanded="false">
                                        <i class="fas fa-plus"></i>
                                        <span class="ms-1 align-middle">เพิ่มหัวข้อ</span>
                                    </button>
                                </h2>
                                <div id="flush-collapseAddPublishTopic" class="accordion-collapse collapse" data-bs-parent="#<%= accordionFlushAddPublishTopic.ClientID %>">
                                    <div class="accordion-body">
                                        <div>
                                            <asp:CheckBox ID="CBMainPublishTopic" runat="server" Text="หัวข้อหลัก" OnCheckedChanged="CBMainPublishTopic_CheckedChanged" AutoPostBack="true" Checked="true" CssClass="CheckBoxCustom" Enabled="false" />
                                        </div>
                                        <div class="input-group">
                                            <asp:TextBox ID="TxtPublishTopicName" runat="server" CssClass="form-control form-control-sm" placeholder="ชื่อหัวข้อ"></asp:TextBox>
                                            <asp:Button ID="BtnAddPublishTopic" runat="server" Text="เพิ่มหัวข้อ" CssClass="btn btn-sm btn-primary" OnClick="BtnAddPublishTopic_Click" ValidationGroup="CheckAddPublishTopic" />
                                        </div>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorPublishTopicName" runat="server"
                                            ControlToValidate="TxtPublishTopicName"
                                            ValidationGroup="CheckAddPublishTopic"
                                            ErrorMessage="กรุณากรอกชื่อหัวข้อ."
                                            SetFocusOnError="true"
                                            ForeColor="#ff1717"
                                            Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:Panel ID="PanelPublishTopicType" runat="server">
                                            <div class="d-flex">
                                                <div>
                                                    <asp:RadioButtonList ID="RBListPublishTopicType" runat="server" RepeatDirection="Horizontal" CssClass="CheckBoxCustom" OnSelectedIndexChanged="RBListPublishTopicType_SelectedIndexChanged" AutoPostBack="true">
                                                        <asp:ListItem Value="1" Text="ทุกคน" Selected="True"></asp:ListItem>
                                                        <asp:ListItem Value="2" Text="กำหนด"></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </div>
                                            </div>
                                            <asp:Panel ID="PanelUserAccess" runat="server" Visible="false">
                                                <asp:ListBox ID="ListBoxPublishTopicUserAccess" runat="server" DataSourceID="SqlDataSourceUserID" DataValueField="UserID" DataTextField="Name" SelectionMode="Multiple" CssClass="ListBoxSelect2"></asp:ListBox>
                                                <asp:HiddenField ID="HFUserIDSelectedAccess" runat="server" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorPublishTopicUserAccess" runat="server"
                                                    ControlToValidate="ListBoxPublishTopicUserAccess"
                                                    ValidationGroup="CheckAddPublishTopic"
                                                    ErrorMessage="กรุณาเลือกชื่อผู้ใช้."
                                                    SetFocusOnError="true"
                                                    ForeColor="#ff1717"
                                                    Display="Dynamic"
                                                    Enabled="false"></asp:RequiredFieldValidator>
                                            </asp:Panel>
                                        </asp:Panel>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                        <div class="d-flex flex-wrap mt-2 text-secondary " style="font-size: 0.9rem;">
                            <div class="me-2">
                                <span class="align-middle">การเข้าถึง</span>
                            </div>
                            <div class="me-2">
                                <i class="fas fa-globe" style="color: gray;"></i>
                                <span class="align-middle">ทุกคน</span>
                            </div>
                            <div class="me-2">
                                <i class="fas fa-user-lock" style="color: gray;"></i>
                                <span class="align-middle">กำหนด</span>
                            </div>
                            <div>
                                <i class="fas fa-globe" style="color: orange;"></i>
                                <i class="fas fa-user-lock" style="color: orange;"></i>
                                <i class="fas fa-user" style="color: orange;"></i>
                                <span class="align-middle">เป็นเจ้าของหัวข้อ</span>
                            </div>
                        </div>
                        <hr class="my-2" />
                        <div>
                            <asp:CheckBox ID="CBExpandAll" runat="server" Text="แสดง/ซ่อน หัวข้อทั้งหมด" OnCheckedChanged="CBExpandAll_CheckedChanged" AutoPostBack="true" Checked="true" CssClass="CheckBoxCustom" />
                        </div>
                        <div id="PublishTopicScroll" class="overflow-auto scroll-custom">
                            <asp:TreeView ID="TreeViewPublishTopic" runat="server" NodeWrap="true" ShowLines="true" OnSelectedNodeChanged="TreeViewPublishTopic_SelectedNodeChanged" ExpandDepth="1">
                                <NodeStyle ForeColor="Black" />
                                <LevelStyles>
                                    <asp:TreeNodeStyle Font-Bold="true" ForeColor="Blue" />
                                    <asp:TreeNodeStyle Font-Bold="true" ForeColor="Black" />
                                </LevelStyles>
                                <SelectedNodeStyle ForeColor="DeepPink" />
                            </asp:TreeView>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-7">
                <div class="p-3 border shadow-sm rounded bg-white">
                    <asp:Panel ID="PanelPublishDoc" runat="server" Visible="false" CssClass="d-flex flex-column" Style="height: calc(100vh - 120px);">
                        <div class="row mb-3">
                            <div class="col-auto">
                                <div class="d-flex flex-column">
                                    <asp:HyperLink ID="HLPublishTopic" runat="server" CssClass="link-primary" Font-Size="Larger" Target="_blank"></asp:HyperLink>
                                    <asp:Label ID="LbShowFileNameReal" runat="server" CssClass="text-secondary" Style="font-size: 0.9rem;"></asp:Label>
                                </div>
                            </div>
                            <div class="col-auto">
                                <asp:ImageButton ID="BtnDownload" runat="server" ImageUrl="~/Image/download-file.png" Height="30" ToolTip="ดาวน์โหลด" CssClass="me-1" OnClick="BtnDownload_Click" />
                                <asp:ImageButton ID="BtnLink" runat="server" ImageUrl="~/Image/link-file.png" Height="30" ToolTip="คัดลอกลิงก์" CssClass="me-1" />
                                <asp:ImageButton ID="BtnHistory" runat="server" ImageUrl="~/Image/history-file.png" Height="30" ToolTip="ประวัติ" CssClass="me-1" />
                                <asp:ImageButton ID="BtnUploadPublishTopicShow" runat="server" ImageUrl="~/Image/upload-file.png" Height="30" ToolTip="อัปโหลด" CssClass="me-1" OnClick="BtnUploadPublishTopicShow_Click" />
                                <asp:ImageButton ID="BtnEditShow" runat="server" ImageUrl="~/Image/edit-file.png" Height="30" ToolTip="แก้ไขชื่อ" CssClass="me-1" OnClick="BtnEditShow_Click" />
                                <asp:ImageButton ID="BtnQRCode" runat="server" ImageUrl="~/Image/qr-code.png" Height="30" ToolTip="QR Code" CssClass="me-1" OnClick="BtnQRCode_Click" />
                                <asp:LinkButton ID="LinkBtnInfo" runat="server" ToolTip="รายละเอียด" CssClass="me-3 text-info" OnClick="LinkBtnInfo_Click"><i class="fas fa-info-circle align-top" style="font-size: 1.9em;"></i></asp:LinkButton>
                                <asp:ImageButton ID="BtnDelete" runat="server" ImageUrl="~/Image/delete.png" Height="25" ToolTip="ลบ" OnClick="BtnDelete_Click" OnClientClick="return alertConfirm(this, 'ลบข้อมูล!', 'ต้องการลบหรือไม่?', 'question', 'ลบ!', 'ยกเลิก');" />
                            </div>
                        </div>
                        <asp:Panel ID="PanelAddPublishDoc" runat="server" CssClass="mb-3">
                            <div class="accordion" id="accordionFlushPublishDoc">
                                <div class="accordion-item border">
                                    <h2 class="accordion-header" id="flush-headingOne">
                                        <button class="accordion-button collapsed p-1 btn-toggle-addpublishdoc" type="button" data-bs-toggle="collapse" data-bs-target="#flush-collapseAddPublishDoc">
                                            <img src="../Image/docs.png" height="15" />
                                            <span>เผยแพร่เอกสาร</span>
                                        </button>
                                    </h2>
                                    <div id="flush-collapseAddPublishDoc" class="accordion-collapse collapse" data-bs-parent="#accordionFlushPublish">
                                        <div class="accordion-body">
                                            <div class="row mb-2">
                                                <div class="col-lg-2 col-12">
                                                    <span>ชื่อไฟล์</span>
                                                </div>
                                                <div class="col-lg-10 col-12">
                                                    <asp:TextBox ID="TxtPublishDocFileName" runat="server" CssClass="form-control form-control-sm" placeholder="กรอกชื่อไฟล์ที่นี่"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorPublishDocFileName" runat="server"
                                                        ControlToValidate="TxtPublishDocFileName"
                                                        ValidationGroup="CheckAddPublishDoc"
                                                        ErrorMessage="กรุณากรอกชื่อไฟล์."
                                                        SetFocusOnError="true"
                                                        ForeColor="#ff1717"
                                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-lg-2 col-12">
                                                    <span>Revision</span>
                                                </div>
                                                <div class="col-lg-10 col-12">
                                                    <asp:TextBox ID="TxtPublishDocRevision" runat="server" CssClass="form-control form-control-sm" placeholder="XX" Width="100" TextMode="Number"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorRevisionPublishDocRevision" runat="server"
                                                        ControlToValidate="TxtPublishDocRevision"
                                                        ValidationGroup="CheckAddPublishDoc"
                                                        ErrorMessage="กรุณากรอกเลข Revision."
                                                        SetFocusOnError="true"
                                                        ForeColor="#ff1717"
                                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-lg-2 col-12">
                                                    <span>แนบไฟล์</span>
                                                </div>
                                                <div class="col-lg-10 col-12">
                                                    <asp:FileUpload ID="FileUploadPublishDoc" runat="server" CssClass="form-control form-control-sm" />
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorFileUploadPublishDoc" runat="server"
                                                        ControlToValidate="FileUploadPublishDoc"
                                                        ValidationGroup="CheckAddPublishDoc"
                                                        ErrorMessage="กรุณาเลือกไฟล์."
                                                        SetFocusOnError="true"
                                                        ForeColor="#ff1717"
                                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                            <div class="text-center">
                                                <asp:Button ID="BtnAddPublishDoc" runat="server" Text="เผยแพร่" CssClass="btn btn-sm btn-primary" OnClick="BtnAddPublishDoc_Click" Width="100" ValidationGroup="CheckAddPublishDoc" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                        <div class="overflow-auto scroll-custom">
                            <%-- ค่า PublishTopicID --%>
                            <asp:HiddenField ID="HFUploadSelected" runat="server" />
                            <%-- ค่า PublishDocFileID ใช้แก้ไขไฟล์ --%>
                            <asp:HiddenField ID="HFPublishDocFileID" runat="server" />
                            <asp:GridView ID="GVPublishDoc" runat="server" CssClass="table table-borderless table-hover border-0" AutoGenerateColumns="false" OnRowCommand="GVPublishDoc_RowCommand" OnRowDataBound="GVPublishDoc_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="ชื่อเอกสาร">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="HLPublishDocName" runat="server" NavigateUrl='<%# "ShowPDF.aspx?PublishDocFileID=" + Eval("PublishDocFileID") %>' Target="_blank" CssClass="link-primary"><%# Eval("FileName") %></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Revision">
                                        <ItemTemplate>
                                            <span><%# Eval("Revision") %></span>
                                        </ItemTemplate>
                                        <HeaderStyle CssClass="text-center" />
                                        <ItemStyle CssClass="text-center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="นามสกุลไฟล์">
                                        <ItemTemplate>
                                            <span><%# Eval("FileExtension") %></span>
                                        </ItemTemplate>
                                        <HeaderStyle CssClass="text-center" />
                                        <ItemStyle CssClass="text-center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="คำสั่ง">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="BtnDownload" runat="server" CommandName="BtnDownload" CommandArgument='<%# Eval("FilePath") %>' Height="30" ImageUrl="~/Image/download-file.png" CssClass="align-middle" ToolTip="ดาวน์โหลด" />
                                            <asp:ImageButton ID="BtnLink" runat="server" CommandName="BtnLink" CommandArgument='<%# Eval("FilePath") %>' Height="30" ImageUrl="~/Image/link-file.png" CssClass="align-middle" ToolTip="คัดลอกลิงก์" OnClientClick='<%# string.Format("CopyToClipboard(\"{0}\"); return false;", Eval("FilePath").ToString().Replace("\\", "\\\\")) %>' />
                                            <asp:ImageButton ID="BtnHistory" runat="server" CommandName="BtnHistory" CommandArgument='<%# Eval("PublishDocID") %>' Height="30" ImageUrl="~/Image/history-file.png" CssClass="align-middle" ToolTip="ประวัติ" />
                                            <asp:ImageButton ID="BtnUpload" runat="server" CommandName="BtnUpload" CommandArgument='<%# Eval("PublishDocID") %>' Height="30" ImageUrl="~/Image/upload-file.png" CssClass="align-middle" ToolTip="อัปโหลด" />
                                            <asp:ImageButton ID="BtnEditShow" runat="server" CommandName="BtnEdit" CommandArgument='<%# Eval("PublishDocFileID") %>' Height="30" ImageUrl="~/Image/edit-file.png" CssClass="align-middle" ToolTip="แก้ไขชื่อ" />
                                            <asp:LinkButton ID="LinkBtnInfo" runat="server" CommandName="LinkBtnInfo" CommandArgument='<%# Eval("PublishDocID") %>' CssClass="me-3 text-info" ToolTip="รายละเอียด"><i class="fas fa-info-circle align-middle" style="font-size: 1.9em;"></i></asp:LinkButton>
                                            <asp:ImageButton ID="BtnDelete" runat="server" CommandName="BtnDelete" CommandArgument='<%# Eval("PublishDocID") %>' Height="25" ImageUrl="~/Image/delete.png" CssClass="align-middle" ToolTip="ลบ" OnClientClick="return alertConfirm(this, 'ลบข้อมูล!', 'ต้องการลบหรือไม่?', 'question', 'ลบ!', 'ยกเลิก');" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    ไม่พบข้อมูล
                                </EmptyDataTemplate>
                                <HeaderStyle CssClass="border-bottom" VerticalAlign="Middle" />
                                <RowStyle CssClass="border-bottom border-light" VerticalAlign="Middle" />
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


        <%-- Modal Start --%>
        <!-- Modal PublishTopic Rename -->
        <div class="modal fade" id="ModalPublishTopicRename" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">แก้ไขชื่อหัวข้อ</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="input-group">
                            <asp:TextBox ID="TxtPublishTopicRename" runat="server" CssClass="form-control form-control-sm"></asp:TextBox>
                            <asp:Button ID="BtnEdit" runat="server" Text="บันทึก" CssClass="btn btn-sm btn-primary" OnClick="BtnEdit_Click" ValidationGroup="CheckEditPublishTopicName" />
                        </div>
                        <div>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorPublishTopicRename" runat="server"
                                ControlToValidate="TxtPublishTopicRename"
                                ValidationGroup="CheckEditPublishTopicName"
                                ErrorMessage="กรุณากรอกชื่อหัวข้อ."
                                SetFocusOnError="true"
                                ForeColor="#ff1717"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal PublishDocFile Rename -->
        <div class="modal fade" id="ModalPublishDocRename" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">แก้ไขเอกสาร</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="row mb-2">
                            <div class="col-2"><span>ชื่อไฟล์</span></div>
                            <div class="col">
                                <asp:TextBox ID="TxtPublishDocRename" runat="server" CssClass="form-control form-control-sm"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorPublishDocRename" runat="server"
                                    ControlToValidate="TxtPublishDocRename"
                                    ValidationGroup="CheckEditPublishDoc"
                                    ErrorMessage="กรุณากรอกชื่อไฟล์."
                                    SetFocusOnError="true"
                                    ForeColor="#ff1717"
                                    Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="row mb-2">
                            <div class="col-2"><span>Revision</span></div>
                            <div class="col">
                                <asp:TextBox ID="TxtEditPublishDocRevision" runat="server" CssClass="form-control form-control-sm" placeholder="XX" Width="100" TextMode="Number"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorEditPublishDocRevision" runat="server"
                                    ControlToValidate="TxtEditPublishDocRevision"
                                    ValidationGroup="CheckEditPublishDoc"
                                    ErrorMessage="กรุณากรอกเลข Revision."
                                    SetFocusOnError="true"
                                    ForeColor="#ff1717"
                                    Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="text-center">
                            <asp:Button ID="BtnEditPublishDoc" runat="server" Text="บันทึก" CssClass="btn btn-sm btn-primary" Width="200" OnClick="BtnEditPublishDoc_Click" ValidationGroup="CheckEditPublishDoc" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal PublishTopic Upload FIle -->
        <div class="modal fade" id="ModalPublishTopicUploadFile" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <asp:Label ID="LBTitleModal" runat="server"></asp:Label>
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="input-group">
                            <asp:FileUpload ID="FileUploadPublishTopic" runat="server" CssClass="form-control form-control-sm" />
                            <asp:Button ID="BtnUpdatePublishTopic" runat="server" Text="อัปโหลด" CssClass="btn btn-sm btn-primary" OnClick="BtnUpdatePublishTopic_Click" ValidationGroup="CheckUploadFilePublishTopic" />
                        </div>
                        <div>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorFileUploadPublishTopic" runat="server"
                                ControlToValidate="FileUploadPublishTopic"
                                ValidationGroup="CheckUploadFilePublishTopic"
                                ErrorMessage="กรุณาเลือกไฟล์."
                                SetFocusOnError="true"
                                ForeColor="#ff1717"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal PublishDoc Upload File -->
        <div class="modal fade" id="ModalPublishDocUploadFile" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <asp:Label ID="LBFileName" runat="server"></asp:Label>
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div>
                            <asp:FileUpload ID="FileUploadPublishDocNew" runat="server" CssClass="form-control form-control-sm" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorFileUploadPublishDocNew" runat="server"
                                ControlToValidate="FileUploadPublishDocNew"
                                ValidationGroup="CheckUploadPublishDocNew"
                                ErrorMessage="กรุณาเลือกไฟล์."
                                SetFocusOnError="true"
                                ForeColor="#ff1717"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                        <div class="my-2">
                            <label class="form-label">Revision : </label>
                            <asp:TextBox ID="TxtPublishDocRevisionNew" runat="server" CssClass="form-control form-control-sm d-inline" placeholder="XX" Width="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorPublishDocRevisionNew" runat="server"
                                ControlToValidate="TxtPublishDocRevisionNew"
                                ValidationGroup="CheckUploadPublishDocNew"
                                ErrorMessage="กรุณากรอกเลข Revision."
                                SetFocusOnError="true"
                                ForeColor="#ff1717"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                        <div class="text-center">
                            <asp:Button ID="BtnUpdatePublishDoc" runat="server" Text="อัปโหลด" CssClass="btn btn-sm btn-primary" OnClick="BtnUpdatePublishDoc_Click" Width="100" ValidationGroup="CheckUploadPublishDocNew" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal PublishTopic QRCode -->
        <div class="modal fade" id="ModalPublishTopicQRCode" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <asp:Label ID="LBTitleModalQRCode" runat="server"></asp:Label>
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="copyable mx-auto" style="width: 150px; height: 180px;">
                            <asp:PlaceHolder ID="PlaceHolderQRCode" runat="server"></asp:PlaceHolder>
                        </div>
                        <div class="text-center">
                            <asp:Button ID="BtnDownloadQRCode" runat="server" Text="ดาวน์โหลด" Width="150" CssClass="btn btn-info" OnClientClick="DownloadImage(); return false;" />
                            <asp:Button ID="BtnCopy" runat="server" Text="คัดลอก" Width="150" CssClass="btn btn-secondary ms-3" OnClientClick="CopyImage(); return false;" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <%-- Modal End --%>


        <%-- Offcanvas Start --%>
        <%-- รายละเอียดหัวข้อ --%>
        <div class="offcanvas offcanvas-start" tabindex="-1" id="offcanvasDetailPublishTopic">
            <div class="offcanvas-header">
                <h5 class="offcanvas-title">รายละเอียดหัวข้อ</h5>
                <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas"></button>
            </div>
            <div class="offcanvas-body">
                <div class="mb-3 d-flex flex-column">
                    <span class="fw-bold">ชื่อหัวข้อ</span>
                    <asp:Label ID="LbDetailPublishTopicName" runat="server" CssClass="text-secondary"></asp:Label>
                </div>
                <div class="mb-3 d-flex flex-column">
                    <span class="fw-bold">สร้างโดย</span>
                    <asp:Label ID="LbDetailPublishTopicCreatedBy" runat="server" CssClass="text-secondary"></asp:Label>
                </div>
                <div class="mb-3 d-flex flex-column">
                    <span class="fw-bold">วันที่สร้าง</span>
                    <asp:Label ID="LbDetailPublishTopicDateCreate" runat="server" CssClass="text-secondary"></asp:Label>
                </div>
                <asp:Panel ID="PanelAccess" runat="server" CssClass="mb-3 d-flex flex-column">
                    <asp:UpdatePanel ID="UpdatePanelEditPublishTopic" runat="server">
                        <ContentTemplate>
                            <div class="d-flex">
                                <span class="fw-bold">การเข้าถึง</span>
                                <asp:LinkButton ID="LinkBtnEditPublishTopicUserAccess" runat="server" CssClass="text-warning ms-2" Width="100" OnClick="LinkBtnEditPublishTopicUserAccess_Click"><i class="fas fa-edit"></i> แก้ไข</asp:LinkButton>
                            </div>

                            <%-- แสดงประเภทหัวข้อ และผู้ใช้ที่มีสิธิ์เข้าถึง --%>
                            <asp:Panel ID="PanelDisplayAccess" runat="server">
                                <asp:Label ID="LbPublishTopicTypeName" runat="server" CssClass="text-secondary"></asp:Label>
                                <asp:ListView ID="LVPublishTopicUserAccess" runat="server" GroupPlaceholderID="groupPlaceHolder" ItemPlaceholderID="itemPlaceHolder">
                                    <LayoutTemplate>
                                        <div class="d-flex flex-column overflow-auto scroll-custom" style="max-height: 300px;">
                                            <asp:PlaceHolder runat="server" ID="groupPlaceHolder"></asp:PlaceHolder>
                                        </div>
                                    </LayoutTemplate>
                                    <GroupTemplate>
                                        <div class="ps-2">
                                            <asp:PlaceHolder runat="server" ID="itemPlaceHolder"></asp:PlaceHolder>
                                        </div>
                                    </GroupTemplate>
                                    <ItemTemplate>
                                        <span class="text-secondary"><i class="fas fa-user-friends"></i><%# Eval("Name") %></span>
                                    </ItemTemplate>
                                </asp:ListView>
                            </asp:Panel>
                            <%-- แสดงแก้ไขประเภทหัวข้อ และผู้ใช้ที่มีสิธิ์เข้าถึง --%>
                            <asp:Panel ID="PanelEditPublishTopicUserAccess" runat="server" Visible="false" CssClass="border p-2 rounded">
                                <div class="d-flex">
                                    <div>
                                        <asp:RadioButtonList ID="RBListEditPublishTopicType" runat="server" RepeatDirection="Horizontal" CssClass="CheckBoxCustom" OnSelectedIndexChanged="RBListEditPublishTopicType_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Value="1" Text="ทุกคน" Selected="True"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="กำหนด"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                                <asp:Panel ID="PanelEditUserAccess" runat="server" Visible="false">
                                    <asp:ListBox ID="ListBoxEditPublishTopicUserAccess" runat="server" DataSourceID="SqlDataSourceUserID" DataValueField="UserID" DataTextField="Name" SelectionMode="Multiple" CssClass="ListBoxSelect2"></asp:ListBox>
                                    <asp:HiddenField ID="HFEditUserIDSelectedAccess" runat="server" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorEditPublishTopicUserAccess" runat="server"
                                        ControlToValidate="ListBoxEditPublishTopicUserAccess"
                                        ValidationGroup="CheckEditPublishTopic"
                                        ErrorMessage="กรุณาเลือกชื่อผู้ใช้."
                                        SetFocusOnError="true"
                                        ForeColor="#ff1717"
                                        Display="Dynamic"
                                        Enabled="false"></asp:RequiredFieldValidator>
                                </asp:Panel>
                                <div class="d-flex mt-1 justify-content-between">
                                    <asp:Button ID="BtnSaveEditPublishTopic" runat="server" CssClass="btn btn-sm btn-success" Text="บันทึก" Width="100" OnClick="BtnSaveEditPublishTopic_Click" ValidationGroup="CheckEditPublishTopic" />
                                    <asp:Button ID="BtnCancelEditPublishTopic" runat="server" CssClass="btn btn-sm btn-light border" Text="ยกเลิก" OnClick="BtnCancelEditPublishTopic_Click" />
                                </div>
                            </asp:Panel>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="BtnSaveEditPublishTopic" />
                        </Triggers>
                    </asp:UpdatePanel>
                </asp:Panel>
            </div>
        </div>

        <%-- รายละเอียดไฟล์ --%>
        <div class="offcanvas offcanvas-start" tabindex="-1" id="offcanvasDetailPublishDoc">
            <div class="offcanvas-header">
                <h5 class="offcanvas-title">รายละเอียดเอกสาร</h5>
                <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas"></button>
            </div>
            <div class="offcanvas-body">
                <div class="mb-3 d-flex flex-column">
                    <span class="fw-bold">ชื่อเอกสาร</span>
                    <asp:Label ID="LbDetailPublishDocFileName" runat="server" CssClass="text-secondary"></asp:Label>
                </div>
                <div class="mb-3 d-flex flex-column">
                    <span class="fw-bold">สร้างโดย</span>
                    <asp:Label ID="LbDetailPublishDocCreatedBy" runat="server" CssClass="text-secondary"></asp:Label>
                </div>
                <div class="mb-3 d-flex flex-column">
                    <span class="fw-bold">วันที่สร้าง</span>
                    <asp:Label ID="LbDetailPublishDocDateCreate" runat="server" CssClass="text-secondary"></asp:Label>
                </div>
            </div>
        </div>
        <%-- Offcanvas End --%>
    </div>

    <%-- SqlDataSource --%>
    <asp:SqlDataSource ID="SqlDataSourceUserID" runat="server" ConnectionString="<%$ ConnectionStrings:TCTV1ConnectionString %>"
        SelectCommand="SELECT UserID, (FirstNameTH + ' ' + LastNameTH) AS Name FROM F2_Users WHERE Status = 1"></asp:SqlDataSource>
</asp:Content>
