<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<EInvoice.CAdmin.Models.ResetModel>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Lấy lại mật khẩu</title>
    <script src="/Content/js/jquery-1.10.2.min.js"></script>
    <style type="text/css">
        ﻿﻿﻿﻿﻿html, body, div, fieldset, form, label, legend, table, caption, tbody, tfoot, thead, tr, th, td {
            margin: 0;
            padding: 0;
            border: 0;
            outline: 0;
            font-weight: inherit;
            font-style: inherit;
            font-size: 100%;
            font-family: inherit;
            vertical-align: baseline;
        }

        body {
            background-color: white;
        }

        html, body {
            height: 100%;
        }

        img {
            border-width: 0;
            border-color: transparent;
        }

        :focus {
            outline: 0 none;
        }

        body {
            font-size: 80%;
            line-height: 1.4em;
            font-family: Helvetica, arial, clean, sans-serif;
        }

        a {
            color: #4690D6;
        }

            a:hover, a.selected {
                color: #555;
                text-decoration: underline;
            }

        label {
            font-weight: bold;
            color: #333;
            font-size: 110%;
        }

        input, textarea {
            border: 1px solid #eee;
            color: #666;
            padding: 8px;
            width: 100%;
            -webkit-border-radius: 3px;
            -moz-border-radius: 3px;
            border-radius: 3px;
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
        }

            input[type=text]:focus, input[type=password]:focus, textarea:focus {
                border: solid 1px #CCC;
                background: #EEE;
                color: #333;
            }

        .elgg-form-login {
            width: 400px;
            padding: 32px;
            margin: auto;
            background: #FFF;
            border: 1px solid #EEE;
            border-radius: 6px;
        }

            .elgg-form-login .elgg-input-text, .elgg-form-login .elgg-input-password {
                margin: 8px 0;
                padding: 16px;
                font-size: 12px;
                border: 1px solid #DDD;
            }

            .elgg-form-login .elgg-menu-general {
                float: right;
                margin-top: 4px;
            }

                .elgg-form-login .elgg-menu-general li {
                    margin: 8px;
                      display: inline-block;
                }


        .elgg-button {
            font-size: 12px;
            font-weight: bold;
            text-align: left;
            -webkit-border-radius: 2px;
            -moz-border-radius: 2px;
            border-radius: 2px;
            min-width: 125px;
            width: auto;
            padding: 8px 12px;
            cursor: pointer;
            outline: none;
            -webkit-box-shadow: 0;
            -moz-box-shadow: 0;
            box-shadow: 0;
        }

        .elgg-button-submit {
            min-width: 0;
            background: #EEE;
            background: linear-gradient(#FCFCFC, #EEEEEE);
            border: 1px solid #CCC;
            color: #333;
            text-decoration: none;
            text-shadow: 0;
            cursor: pointer;
        }

            .elgg-button-submit:hover {
                text-decoration: none;
                background: #EEE;
            }

            .elgg-button-submit.elgg-state-disabled {
                background: #999;
                border-color: #999;
                cursor: default;
            }

        .hero, .elgg-page-default {
            min-width: 998px;
            height: auto;
            min-height: 100%;
        }

            .hero > .topbar {
                background: #F8F8F8;
                background: rgba(255,255,255, 0.9);
                position: fixed;
                top: 0;
                min-width: 998px;
                width: 100%;
                height: auto;
                z-index: 8000;
                box-shadow: 0 0 5px #DDD;
                -moz-box-shadow: 0 0 5px #DDD;
                -webkit-box-shadow: 0 0 5px #DDD;
            }

                .hero > .topbar > .inner {
                    padding: 8px;
                    margin: auto;
                    width: 90%;
                }

                    .hero > .topbar > .inner > .center {
                        width: 30%;
                    }


                .hero > .topbar .logo {
                    margin: auto;
                    padding: 0 8px 12px;
                    display: block;
                    position: relative;
                    width: auto;
                }

                    .hero > .topbar .logo img {
                        height: 50px;
                        width: auto;
                    }

        .elgg-page-header {
            position: relative;
        }

            .elgg-page-header > .elgg-inner {
                position: relative;
            }

        .hero > .body, .elgg-page-body {
            position: relative;
            width: 100%;
            height: 100%;
            min-height: 100%;
            margin-top: 74px;
            padding-bottom: 112px;
            background: #FFF;
        }


        body {
            background: #FFF;
        }

        .minds-body-header {
            width: 100%;
            height: auto;
            background: transparent;
            padding: 25px 0;
            margin-bottom: 10px;
            display: inline-block;
        }

            .minds-body-header > .inner {
                width: 90%;
                margin: 0 auto;
            }

                .minds-body-header > .inner > .elgg-head {
                    min-width: 998px;
                }


        .center {
            text-align: center;
        }

        @media only screen and (min-device-width : 768px) and (max-device-width : 1024px) and (-webkit-min-device-pixel-ratio: 1) {
            .hero, .elgg-page-default {
                min-width: 740px;
            }

                .hero > .body, .elgg-page-body {
                    max-width: 768px;
                }

                .hero > .topbar {
                    min-width: 740px;
                }
        }

        @media all and (min-width : 0px) and (max-width : 726px) {
            .hero, .elgg-page-default {
                min-width: 320px;
            }

                .hero > .topbar {
                    min-width: 320px;
                }

                    .hero > .topbar > .inner {
                        padding: 0;
                        width: 100%;
                    }

                .hero > .body, .elgg-page-body {
                    margin-top: 48px;
                    padding-bottom: 0;
                }

                    .hero > .body > .inner, .elgg-page-default .elgg-page-body > .elgg-inner {
                        width: 100%;
                    }

                .hero > .topbar .logo {
                    height: 22px;
                    padding: 8px;
                }
        }
        .validation-summary-errors {
            color:#f60e0e;            
        }
    </style>
</head>
<body>
    <div class="hero elgg-page elgg-page-default">
        <div class="topbar">
            <div class="inner">
                <div class="center">
                    <div class="logo">
                        <a href="/">
                            <img src="/Content/images/logo.png" class="minds-com">
                        </a>
                    </div>
                </div>
            </div>
        </div>
        <div class="body elgg-page-body">
            <div class="minds-body-header">
                <div class="inner">
                </div>
            </div>
            <div class="elgg-layout elgg-layout-one-column clearfix">
                <form method="post" action="/Account/Reset" class="elgg-form elgg-form-login">                                        
                    <div id="lblErrorMessage" class="validation-summary-errors"><b><%=Model.lblErrorMessage %></b></div>
                    <fieldset>
                        <input type="text" value="" name="username" maxlength="50" class="elgg-input-text elgg-autofocus" placeholder="tài khoản đăng nhập" autocomplete="off">                        
                        <input type="submit" value="Tìm kiếm" class="elgg-button elgg-button-submit">
                    </fieldset>
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
            $('input[type=text]:first').focus();
            $("input[type='text']").blur(function () {
                checkTextMaxLength(this);
            });            
            $('#captch').val('');
        });
    </script>
</body>
</html>
