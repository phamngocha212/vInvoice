<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<ChangePasswordModel>" %>

<%@ Import Namespace="IdentityManagement.Domain" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>

<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
    <% using (Html.BeginForm("UpdatePasswordCustomer", "Account", FormMethod.Post, new { @class = "form-horizontal" }))
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
                                <label class="col-sm-6 control-label"><%=Resources.Einvoice.User_NewPass%><span style="color: red">(*)</span></label>
                                <div class="col-sm-6">
                                    <%= Html.Password("newPassword", "", new { @class = "required form-control" })%>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="col-sm-6 control-label">Nhập lại<span style="color: red">(*)</span></label>
                                <div class="col-sm-6">
                                    <%= Html.Password("confirmPassword", "", new { @class = "required form-control", Title = "Nhập lại mật khẩu" })%>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="box-footer text-center">
                    <button class="btn btn-sm btn-primary" type="submit"><i class="fa fa-check"></i><%= Resources.Einvoice.BtnSave%></button>
                    <button class="btn btn-sm" type="reset"><i class="fa fa-refresh"></i>Làm lại</button>
                </div>
            </div>
        </div>
    </div>
    <% }%>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#newPassword").val("");
            $("#confirmPassword").val("");
            $('form:first').validate({
                rules: {
                    newPassword: {
                        required: true,
                        minlength: 8
                    },
                    confirmPassword: {
                        required: true,
                        equalTo: "#newPassword"
                    }
                },
                messages: {
                    newPassword: {
                        required: "<%=Resources.Message.User_MesReqNewPass %> ",
                        minlength: $.format("<%=Resources.Message.User_MesReqMinLengthPass%>  ")
                    },
                    confirmPassword: {
                        required: "<%=Resources.Message.User_MesReqConfirmNewPass%> ",
                        equalTo: "<%=Resources.Message.User_MesErrConfirmPas%> "
                    }
                }
            });

            $("form").submit(function () {
                if (!$('form').valid()) {
                    $("#newPassword").val("");
                    $("#confirmPassword").val("");
                }
            });
        });
    </script>
</asp:Content>
