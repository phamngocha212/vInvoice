<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<EInvoice.CAdmin.Models.ActiveModels>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Kích hoạt tài khoản hệ thống</title>

    <link rel="stylesheet" href="/Content/bootstrap/css/bootstrap.min.css">

    <!-- Font Awesome -->
    <link rel="stylesheet" href="/Content/css/font-awesome.min.css">
    <!-- Theme style -->
    <link rel="stylesheet" href="/Content/dist/css/AdminLTE.min.css">
    <link rel="stylesheet" href="/Content/dist/css/skins/_all-skins.min.css">
    <script src="/Content/js/jquery.min.js"></script>
    <script src="/Content/bootstrap/js/bootstrap.min.js"></script>
    <style>
        #main {
            width: 420px;
            clear: left;
            margin: 100px auto;
        }

        #main-nav {
            width: 100%;
            padding: 12px 10% 15px;
            border-bottom: 1px solid #ccc;
        }

        .box.box-primary {
            box-shadow: 0 1px 1px #3c8dbc;
        }
    </style>
</head>
<body>

    <nav id="main-nav">
        <a>
            <img src="/Content/images/logo.png" class="minds-com">
        </a>
    </nav>
    <div class="row" id="">
        <div id="main">
            <div class="box box-primary">
                <form id="main-form" method="post" action="/Account/active" class="form-horizontal">
                    <div class="box-body">
                        <div class="form-group">
                            <%=Html.Hidden("username", Model.username) %>
                            <%=Html.Hidden("code", Model.code) %>
                            <div class="col-sm-12">
                                <div id="lblErrorMessage"><%=Model.ErrMessages %></div>
                            </div>

                        </div>                        
                        <div class="form-group">
                            <div class="col-sm-12">
                                <div class="input-group">
                                    <div class="input-group-addon">
                                        <i class="fa fa-lock"></i>
                                    </div>
                                    <input type="password" value="" id="password" name="password" maxlength="64" class="elgg-input-password form-control" placeholder="mật khẩu đăng nhập" autocomplete="off">
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-sm-12">
                                <div class="input-group">
                                    <div class="input-group-addon">
                                        <i class="fa fa-lock"></i>
                                    </div>
                                    <input type="password" value="" id="comfirmpassword" maxlength="64" name="comfirmpassword" placeholder="xác thực mật khẩu" class="elgg-input-password form-control" autocomplete="off">
                                </div>
                            </div>
                        </div>
                        <%Html.RenderPartial("Captcha", new EInvoice.CAdmin.Models.Captcha()); %>
                    </div>
                    <div class="box-footer">
                        <input type="submit" value="Kích hoạt" class="btn btn-sm btn-primary">
                        <p style="color: #3cacf6; line-height: 1.5"><b>Lưu ý: </b>Mật khẩu phải có ít nhất 8 ký tự, có chữ HOA, thường, số và ký tự đặc biệt.</p>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function checkTextMaxLength(textBox) {
            var mLen = textBox.getAttribute("maxlength");
            var maxLength = parseInt(mLen);

            if (textBox.value.length > maxLength - 1) {
                textBox.value = textBox.value.substring(0, maxLength);
            }
        }
        $(document).ready(function () {
            var error = $('#lblErrorMessage').text();
            if (error.length > 0) {
                $('#lblErrorMessage').addClass('alert alert-danger');
            }

            $('input[type=password]:first').focus();
            $("input[type='password']").blur(function () {
                checkTextMaxLength(this);
            });
            $('#captch').val('');
            $("form").submit(function () {
                if (!$('#captch').val() || !$('#password').val() || !$('#comfirmpassword').val()) {
                    $('#lblErrorMessage').addClass('alert alert-danger');
                    $('#lblErrorMessage').text("Vui lòng nhập đầy đủ thông tin");
                    return false;
                }
                if ($('#password').val() != $('#comfirmpassword').val()) {
                    $('#lblErrorMessage').addClass('alert alert-danger');
                    $('#lblErrorMessage').text("Mật khẩu xác thực không đúng.");
                    return false;
                }
                if (checkStrengthPass($('#password').val()) < 3) {
                    $('#lblErrorMessage').addClass('alert alert-danger');
                    $('#password').val("");
                    $("#confirmpassword").val("");
                    $('#lblErrorMessage').text("Mật khẩu không đủ mạnh.");
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
</body>
</html>
