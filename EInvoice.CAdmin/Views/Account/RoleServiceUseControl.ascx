<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<user>" %>
<%@ Import Namespace="IdentityManagement.Domain" %>
<link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>

<%=Html.Hidden("userid", Model.userid)%>

<div class="col-xs-12">
    <div class="box box-danger">
        <div class="box-header with-border">
            <h4 class="box-title"><i class="fa fa-user"></i>THÔNG TIN NGƯỜI DÙNG HỆ THỐNG</h4>
        </div>
        <div class="box-body">
            <fieldset>
                <ol>
                    <%if (Model.userid > 0)
                      {%>
                    <li>
                        <label for="username"><%=Resources.Einvoice.User_LblAccountName %><span style="color: red">(*)</span></label>
                        <%=Html.Encode(Model.username) %>            
                    </li>
                    <li>
                        <label for="email"><%=Resources.Einvoice.User_LblEmail%> <span style="color: red">(*)</span></label>
                        <%=Html.Encode(Model.email) %>            
                    </li>
                    <%}
                      else
                      {%>
                    <li>
                        <label for="username"><%=Resources.Einvoice.User_LblAccountName %><span style="color: red">(*)</span></label>
                        <%=Html.TextBox("username", Model.username, new { style = "width:200px", @maxlength="50", @class = "required textandnum", title = Resources.Einvoice.User_ReqAccountName })%>
                    </li>
                    <li>
                        <label for="email"><%=Resources.Einvoice.User_LblEmail%> <span style="color: red">(*)</span></label>
                        <%=Html.TextBox("email", Model.email, new { style = "width:200px", @class = "required email",@maxlength="100", title = Resources.Einvoice.User_MesInvalidEmail })%>
                    </li>
                    <%}%>
                    <li>
                        <label for="password"><%=Resources.Einvoice.User_LblPassWord %> <span style="color: red">(*)</span></label>
                        <%=Html.Password("password", Model.password, new { style = "width:200px", @class = "required", minlength = "6", @maxlength="64", title = Resources.Einvoice.User_ReqPassWord })%>            
                    </li>
                    <li>
                        <label for="RetypePassword"><%=Resources.Einvoice.User_lblRetypePass %><span style="color: red">(*)</span></label>
                        <%=Html.Password("RetypePassword", ViewData["RetypePassword"], new { style = "width:200px", @class = "required", minlength = "6",@maxlength="64", title = Resources.Einvoice.User_ReqRetypeNewPass })%>          
                    </li>
                    <li>
                        <label for="IsApproved"><%=Resources.Einvoice.User_LblActive %></label>
                        <%=Html.CheckBox("IsApproved", Model.IsApproved)%>
                    </li>
                    <li>
                        <label for="IsLockedOut"><%=Resources.Einvoice.User_LblLock %></label>
                        <%=Html.CheckBox("IsLockedOut", Model.IsLockedOut)%>
                    </li>                    
                </ol>
            </fieldset>
        </div>
    </div>
</div>

<script type="text/javascript">
    $(document).ready(function () {
        $('form:first').validate({
            rules: {
                password: {
                    required: true,
                    minlength: 6
                },
                RetypePassword: {
                    required: true,
                    equalTo: "#password"
                }
            },
            messages: {
                password: {
                    required: "<%=Resources.Message.User_MesReqNewPass %> ",
                    minlength: $.format("<%=Resources.Message.User_MesReqMinLengthPass%>")
                },
                RetypePassword: {
                    required: "<%=Resources.Message.User_MesReqConfirmNewPass%> ",
                    equalTo: "<%=Resources.Message.User_MesErrConfirmPas%> "
                }
            }
        });
        if ($('#userid').val() < 1) {
            $('#username').val("");
            $('#password').val("");
            $('#RetypePassword').val('');
        }
        $("form").submit(function () {
            if (!$('form').valid()) {
                $('#username').val("");
                $("#password").val("");
                $("#RetypePassword").val("");
                return false;
            }
            if (checkValid($('#username').val()) < 1) {
                $('#username').val('');
                $('#password').val("");
                $("#RetypePassword").val("");
                alertify.alert("Tài khoản người dùng không được chứa ký tự đặc biệt.");
                return false;
            }
        });
    });

    function checkValid(text) {
        var dbs = new Array("~", "@", "#", "$", "^", "*", "|", "<", ">", "!", "'", ";");
        var sum = dbs.length;
        var i = 0;
        while (i < sum) {
            for (var k = 0 ; k < text.length; k++) {
                if (dbs[i] == text[k])
                    return 0;
            }
            i++;
        }
        return 1;
    }
</script>