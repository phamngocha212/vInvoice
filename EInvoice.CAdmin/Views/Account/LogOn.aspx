<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<EInvoice.CAdmin.Models.LogOnModel>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Đăng nhập hệ thống</title>
    <link rel="Shortcut Icon" href="/Content/favicon.ico" />

    <link rel="stylesheet" href="/Content/bootstrap/css/bootstrap.min.css">

    <!-- Font Awesome -->
    <link rel="stylesheet" href="/Content/css/font-awesome.min.css">    
    <!-- Theme style -->
    <link rel="stylesheet" href="/Content/dist/css/AdminLTE.min.css">        
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
                <form method="post" action="/Account/logon" class=" form-horizontal">
                    <div class="box-body">
                        <div class="form-group">
                            <%=Html.AntiForgeryToken() %>
                            <%=Html.Hidden("ReturnUrl",Model.ReturnUrl ?? "/")%>
                            <input type="hidden" name="IsThread" value="<%=Html.Encode(Model.IsThread)%>" />
                            <div class="col-sm-12">
                                <div id="lblErrorMessage"><%=Model.lblErrorMessage %></div>
                            </div>

                        </div>
                        <div class="form-group">
                            <div class="col-sm-12">
                                <div class="input-group">
                                    <div class="input-group-addon">
                                        <i class="fa fa-user"></i>
                                    </div>
                                    <input type="text" value="" name="username" maxlength="50" class="form-control" placeholder="Tài khoản"/>
                                </div>

                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-sm-12">
                                <div class="input-group">
                                    <div class="input-group-addon">
                                        <i class="fa fa-lock"></i>
                                    </div>
                                    <input type="password" value="" name="password" maxlength="50" class="form-control" placeholder="Mật khẩu" />
                                </div>
                            </div>
                        </div>

                        <%Html.RenderPartial("Captcha", new EInvoice.CAdmin.Models.Captcha()); %>
                    </div>
                    <div class="box-footer">
                        <input type="submit" value="Đăng nhập" class="btn btn-sm btn-primary">
                        <a href="/Account/ResetPassword" class="pull-right">Quên mật khẩu</a>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <script src="/Content/js/jquery.min.js"></script>
    <script src="/Content/bootstrap/js/bootstrap.min.js"></script>
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
            $('input[type=text]:first').focus();
            $("input[type='text']").blur(function () {
                checkTextMaxLength(this);
            });
            $("input[type='password']").blur(function () {
                checkTextMaxLength(this);
            });
            $('#captch').val('');
        });
    </script>
</body>
</html>
