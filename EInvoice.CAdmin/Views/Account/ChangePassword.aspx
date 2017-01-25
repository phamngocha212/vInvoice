<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<ChangePasswordModel>" %>

<%@ Import Namespace="IdentityManagement.Domain" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Thay đổi mật khẩu
</asp:Content>

<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
    <% using (Html.BeginForm("ChangePassword", "Account", FormMethod.Post, new { @class = "form-horizontal" }))
       { %>
    <%=Html.Hidden("username", Model.username)%>
    <div class="row">
        <div class="col-xs-offset-2 col-xs-8">
            <div class="box box-danger">
                <div class="box-header with-border">
                    <h4 class="box-title"><i class="fa fa-lock"></i>THAY ĐỔI MẬT KHẨU</h4>
                </div>
                <div class="box-body">
                    <div class="row">
                        <div class="col-xs-10">
                            <div class="form-group">
                                <label class="col-sm-4"><%=Resources.Einvoice.User_LblOldPass%><span style="color: red">(*)</span></label>
                                <div class="col-sm-8">
                                    <%= Html.Password("oldPassword", Model.OldPassword, new { @class = "form-control required", Title = Resources.Einvoice.User_ReqPassWord })%>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-4"><%=Resources.Einvoice.User_NewPass%><span style="color: red">(*)</span></label>
                                <div class="col-sm-8">
                                    <%= Html.Password("NewPassword", Model.NewPassword, new {  @class = "form-control required" })%>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="col-sm-4"><%=Resources.Einvoice.User_RetypeNewPass%><span style="color: red">(*)</span></label>
                                <div class="col-sm-8">
                                    <%= Html.Password("ConfirmPassword", Model.ConfirmPassword, new {  @class = " form-control required", Title = "Nhập lại mật khẩu" })%>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="box-footer text-center">
                    <button class="btn btn-primary" type="submit"><i class="fa fa-save"></i>&nbsp;<%= Resources.Einvoice.BtnSave%></button>
                    <button class="btn" type="reset"><i class="fa fa-refresh"></i>&nbsp;Làm lại</button>
                </div>
                <p style="margin: 15px 5px 0px 0px; font-size: 13.5px">Chú ý: Mật khẩu mạnh phải lớn hơn 8 ký tự, có chứa chữ hoa, chữ thường, số và ký tự đặc biệt.</p>
            </div>
        </div>
    </div>
    <% }%>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#oldPassword').val("");
            $("#NewPassword").val("");
            $("#ConfirmPassword").val("");
            $('form:first').validate({
                rules: {
                    oldPassword: {
                        required: true
                    },
                    NewPassword: {
                        required: true,
                        minlength: 8
                    },
                    ConfirmPassword: {
                        required: true,
                        equalTo: "#NewPassword"
                    }
                },
                messages: {
                    oldPassword: {
                        required: "<%=Resources.Message.User_MesReqOldPass%>"
                    },
                    NewPassword: {
                        required: "<%=Resources.Message.User_MesReqNewPass %> ",
                        minlength: $.format("<%=Resources.Message.User_MesReqMinLengthPass%>  ")
                    },
                    ConfirmPassword: {
                        required: "<%=Resources.Message.User_MesReqConfirmNewPass%> ",
                        equalTo: "<%=Resources.Message.User_MesErrConfirmPas%> "
                    }
                }
            });

            $("form").submit(function () {
                if (!$('form').valid()) {
                    $('#oldPassword').val("");
                    $("#NewPassword").val("");
                    $("#ConfirmPassword").val("");
                    return false;
                }
                if (checkStrengthPass($('#NewPassword').val()) < 4) {
                    $('#oldPassword').val("");
                    $("#ConfirmPassword").val("");
                    alertify.alert("Thông báo mật khẩu không đủ mạnh!\n\nChú ý: Mật khẩu mạnh phải lớn hơn 8 ký tự, có chứa chữ hoa, chữ thường, số và ký tự đặc biệt.");
                    return false;
                }
            });
        });
        function checkStrengthPass(password) {
            var strength = 0;
            if (password.length < 8) {
                return 0;
            }
            if (password.length > 7) {
                if (password.match(/(?=.*[A-Z])/) != null && password.match(/(?=.*[A-Z])/))
                    strength += 1;
                if (password.match(/([a-z])/) != null && password.match(/([a-z])/))
                    strength += 1;

                if (password.match(/([0-9])/) != null && password.match(/([0-9])/))
                    strength += 1;

                if (password.match(/([!,%,&,@,#,$,^,*,?,_,~])/) != null && password.match(/([!,%,&,@,#,$,^,*,?,_,~])/))
                    strength += 1;

                return strength;
            }
        }
    </script>
</asp:Content>
