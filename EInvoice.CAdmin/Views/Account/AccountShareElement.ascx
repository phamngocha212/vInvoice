<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AccountModel>" %>
<%@ Import Namespace="IdentityManagement.Domain" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%Html.EnableClientValidation(); %>
<link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
<style type="text/css">
    .tt {
        border: 1px solid #ccc;
        height: 120px;
        margin: 10px 0px 10px 0px;
        padding: 10px 0px 10px 10px;
    }

        .tt input[type=checkbox] {
            vertical-align: middle;
        }
</style>
<script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
<%=Html.Hidden("id", Model.UserTmp.userid)%>
<div class="col-xs-12">
    <div class="box box-danger">
        <div class="box-header with-border">
            <h4 class="box-title"><i class="fa fa-user"></i>THÔNG TIN NGƯỜI DÙNG HỆ THỐNG</h4>
        </div>
        <div class="box-body">
            <fieldset>
                <ol>
                    <%if (Model.UserTmp.userid > 0)
                      {%>
                    <li>
                        <label for="fullname">Tên người dùng:<span style="color: red">(*)</span></label>
                        <%=Html.TextBox("fullname", ViewData["fullname"], new { style = "width:200px", @maxlength="50", @class = "required", title = "Nhập tên người dùng!" })%>
                    </li>
                    <li>
                        <label for="username"><%=Resources.Einvoice.User_LblAccountName %><span style="color: red">(*)</span></label>
                        <%=Html.Encode(Model.UserTmp.username) %>            
                    </li>
                    <li>
                        <label for="email"><%=Resources.Einvoice.User_LblEmail%> <span style="color: red">(*)</span></label>
                        <%=Html.Encode(Model.UserTmp.email) %>            
                    </li>
                    <%}
                      else
                      {%>
                    <li>
                        <label for="fullname">Tên người dùng:<span style="color: red">(*)</span></label>
                        <%=Html.TextBox("fullname", null, new { style = "width:200px", @maxlength="50", @class = "required", title = "Nhập tên người dùng!" })%>
                    </li>
                    <li>
                        <label for="username"><%=Resources.Einvoice.User_LblAccountName %><span style="color: red">(*)</span></label>
                        <%=Html.TextBox("username", Model.UserTmp.username, new { style = "width:200px", @maxlength="50", @class = "required", title = Resources.Einvoice.User_ReqAccountName })%>
                    </li>
                    <li>
                        <label for="email"><%=Resources.Einvoice.User_LblEmail%></label>
                        <%=Html.TextBox("email", Model.UserTmp.email, new { style = "width:200px", @maxlength="100"})%>
                    </li>
                    <%}%>
                    <li>
                        <label for="password"><%=Resources.Einvoice.User_LblPassWord %> <span style="color: red">(*)</span></label>
                        <%=Html.Password("password", Model.UserTmp.password, new { style = "width:200px", @class = "required", minlength = "6", @maxlength="64", title = Resources.Einvoice.User_ReqPassWord })%>            
                    </li>
                    <li>
                        <label for="RetypePassword"><%=Resources.Einvoice.User_lblRetypePass %><span style="color: red">(*)</span></label>
                        <%=Html.Password("RetypePassword", Model.RetypePassword, new { style = "width:200px", @class = "required", minlength = "6",@maxlength="64", title = Resources.Einvoice.User_ReqRetypeNewPass })%>          
                    </li>
                    <li>
                        <label for="IsApproved"><%=Resources.Einvoice.User_LblActive %></label>
                        <%=Html.CheckBox("IsApproved", Model.UserTmp.IsApproved)%>
                    </li>
                    <li>
                        <label for="IsLockedOut"><%=Resources.Einvoice.User_LblLock %></label>
                        <%=Html.CheckBox("IsLockedOut", Model.UserTmp.IsLockedOut)%>
                    </li>
                    <li style="border-bottom: none">
                        <div>
                            <b>Quyền truy cập</b>
                            <table style="width: 100%" class="tt">
                                <%
                                    int rowcount = Model.AllRoles.Length / 3;
                                    int odd = Model.AllRoles.Length % 3;
                                    for (int i = 0; i < rowcount; i++)
                                    {
                                %>
                                <tr>
                                    <td>
                                        <input style="margin-left: 10px" type="checkbox" name="UserRoles" value="<%=Html.Encode(Model.AllRoles[i * 3])%>" <%= FX.Utils.Web.UI.GetChecked(Model.UserRoles.Contains(Model.AllRoles[i * 3]))%> />
                                        <span><%=Html.Encode(Model.AllRoles[i * 3])%></span>
                                    </td>
                                    <td>
                                        <input style="margin-left: 10px" type="checkbox" name="UserRoles" value="<%=Html.Encode(Model.AllRoles[i * 3 + 1])%>" <%= FX.Utils.Web.UI.GetChecked(Model.UserRoles.Contains(Model.AllRoles[i * 3+1]))%> />
                                        <span><%=Html.Encode(Model.AllRoles[i * 3 + 1])%></span>
                                    </td>
                                    <td>
                                        <input style="margin-left: 10px" type="checkbox" name="UserRoles" value="<%=Html.Encode(Model.AllRoles[i * 3 + 2])%>" <%= FX.Utils.Web.UI.GetChecked(Model.UserRoles.Contains(Model.AllRoles[i * 3+2]))%> />
                                        <span><%=Html.Encode(Model.AllRoles[i * 3 + 2])%></span>
                                    </td>
                                </tr>
                                <%}
                                    if (odd != 0)
                                    { %>
                                <tr>
                                    <td>
                                        <input style="margin-left: 10px" type="checkbox" name="UserRoles" value="<%=Model.AllRoles[rowcount * 3]%>" <%= FX.Utils.Web.UI.GetChecked(Model.UserRoles.Contains(Model.AllRoles[rowcount * 3]))%>>
                                        <span><%=Html.Encode(Model.AllRoles[rowcount * 3])%></span>
                                    </td>
                                    <td>
                                        <% if (odd > 1)
                                           {%>
                                        <input style="margin-left: 10px" type="checkbox" name="UserRoles" value="<%=Model.AllRoles[rowcount * 3 +1]%>" <%= FX.Utils.Web.UI.GetChecked(Model.UserRoles.Contains(Model.AllRoles[rowcount * 3 + 1]))%>>
                                        <span><%=Html.Encode(Model.AllRoles[rowcount * 3 + 1])%></span>
                                        <%} %>
                                    </td>
                                    <td>
                                        <% if (odd > 2)
                                           {%>
                                        <input style="margin-left: 10px" type="checkbox" name="UserRoles" value="<%=Model.AllRoles[rowcount * 3 +2 ]%>" <%= FX.Utils.Web.UI.GetChecked(Model.UserRoles.Contains(Model.AllRoles[rowcount * 3 + 2]))%>>
                                        <span><%= Html.Encode(Model.AllRoles[rowcount * 3 + 2])%></span>
                                        <%} %>
                                    </td>
                                </tr>
                                <%} %>
                            </table>
                        </div>
                    </li>
                </ol>
            </fieldset>
        </div>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('#username').keypress(function (event) {
            var keypressed = event.keyCode;           
            if (keypressed < 48
                    || (keypressed > 57 && keypressed < 64)
                    || (keypressed > 90 && keypressed < 97)
                    || keypressed > 122) {
                if (event.charCode == 0 || keypressed == 46) {// không phải kí tự thì vẫn ok           
                    return;
                }                                
                return false;
            }
        });
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
            if ($('#username').val().startsWith('.') || $('#username').val().startsWith('@')) {
                $('#username').val('');
                $('#password').val("");
                $("#RetypePassword").val("");
                alertify.alert("Tài khoản người dùng không được chứa ký tự đặc biệt.");
                return false;
            }
            if ($('#username').val().indexOf(" ") >= 0 || $('#password').val().indexOf(" ") >= 0 || $('#RetypePassword').val().indexOf(" ") >= 0) {
                $('#username').val('');
                $('#password').val("");
                $("#RetypePassword").val("");
                alertify.alert("Tài khoản hoặc mật khẩu không được chứa khoảng cách.");
                return false;
            }
        });
    });    
</script>
