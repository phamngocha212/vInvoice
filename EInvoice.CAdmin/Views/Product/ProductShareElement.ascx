<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Products>" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
<%: Html.HiddenFor(m => m.Id) %>
<%: Html.HiddenFor(m => m.ComID) %>

<div class="col-xs-6 col-xs-offset-3">
    <div class="box box-danger">
        <div class="box-header with-border">
            <h4 class="box-title"><i class="fa fa-television"></i>THÔNG TIN SẢN PHẨM</h4>
        </div>
        <div class="box-body form-horizontal">
            <div class="form-group">
                <label class="col-sm-3">
                    <%: Html.LabelFor(m => m.Code, Resources.Einvoice.Prod_LblCode ) %>
                </label>
                <div class="col-sm-9">
                    <% if (Model.Id > 0)
                       { %>
                    <label><%=Html.Encode(Model.Code) %></label>
                    <% }
                       else
                       { %>
                    <%: Html.TextBoxFor(m => m.Code, new { @class= "required textandnum form-control",@maxlength="20",@placeholder="Nhập không quá 20 ký tự" }) %>
                    <%} %>
                </div>
            </div>
            <div class="form-group">
                <label class="col-sm-3">
                    <%: Html.LabelFor(m => m.NameProduct, Resources.Einvoice.Prod_LblName) %>
                </label>
                <div class="col-sm-9">

                    <%: Html.TextBoxFor(m => m.NameProduct, new { @class="required form-control ", @maxlength="200",@placeholder="Nhập không quá 200 ký tự" }) %>
                </div>
            </div>
            <div class="form-group">
                <label class="col-sm-3">
                    <%: Html.LabelFor(m => m.Price, Resources.Einvoice.Prod_LblCost) %>
                </label>
                <div class="col-sm-9">

                    <%: Html.TextBoxFor(m => m.Price, new {  @class="form-control",@maxlength="21",@placeholder="Nhập không quá 21 ký tự"   }) %>
                </div>
            </div>
            <div class="form-group">
                <label class="col-sm-3">
                    <%: Html.LabelFor(m => m.Unit, Resources.Einvoice.Prod_LblUnit) %>
                </label>
                <div class="col-sm-9">


                    <%: Html.TextBoxFor(m => m.Unit, new {@class="form-control",@maxlength="50" }) %>
                </div>
            </div>

            <div class="form-group">
                <label class="col-sm-3">
                    <%: Html.LabelFor(m => m.Description, Resources.Einvoice.Prod_LblDescription) %>
                </label>
                <div class="col-sm-9">

                    <%: Html.TextAreaFor(m => m.Description, new { cols="10", rows="10",@class="form-control", maxlength="190" }) %>
                </div>
            </div>

        </div>
    </div>
</div>

<script type="text/javascript">
    $(document).ready(function () {
        $('form:first')
            .submit(function () {
                var _price = $('#Price').val();
                $('#Price').val(String(_price).replaceAll(',', ''));
            }).validate();
        $('#Price').keydown(function (e) {
            $(this).val($(this).val().toUpperCase().replace(/([^0-9A-Z])/g, ""));
            var mLen = $(this)["maxlength"];
            if (null == mLen)
                mLen = length;

            var maxLength = parseInt(mLen);
            if (!checkSpecialKeys(e)) {
                if ($(this).val().length > maxLength - 1) {
                    if (window.event)//IE
                        e.returnValue = false;
                    else//Firefox
                        e.preventDefault();
                }
            }
        });
        $('#Price').ForceNumericOnly()
            .focus(function () {
                var _val = $(this).val();
                $(this).val(_val.replaceAll(',', ''));
            })
            .focusout(function () {
                var _val = $(this).val();
                if (_val) {
                    var _fr = _val.indexOf('.') > -1 ? parseFloat(_val).format(2, 3) : parseInt(_val).format(0, 3);
                    $(this).val(_fr || '').trigger('change');
                }
            }).trigger('focusout');
    });

    /// Chỉ cho nhập số vào textbox
    jQuery.fn.ForceNumericOnly = function () {
        return this.each(function () {
            $(this).keydown(function (e) {
                var key = e.charCode || e.keyCode || 0;
                var comma = $(this).hasClass('comma');
                /// One . key press
                if (comma && comma == true && $(this).val().indexOf('.') > -1 && key == 110) {
                    return false;
                }
                // allow backspace, tab, delete, enter, arrows, numbers and keypad numbers ONLY
                // home, end, period, and numpad decimal
                return (
                    key == 8 ||
                    key == 9 ||
                    key == 13 ||
                    key == 46 ||
                    (key == 110 ? (comma && comma == true) ? true : false : false) ||
                    key == 190 ||
                    (key >= 35 && key <= 40) ||
                    (key >= 48 && key <= 57) ||
                    (key >= 96 && key <= 105) ||
                    (key >= 112 && key <= 123));
            });
        });
    };

    /// Format số tiền
    Number.prototype.format = function (n, x) {
        var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\.' : '$') + ')';
        return this.toFixed(Math.max(0, ~~n)).replace(new RegExp(re, 'g'), '$&,');
    };

    String.prototype.replaceAll = function (strTarget, strSubString) {
        var _text = this;
        var intIndexOfMatch = _text.indexOf(strTarget);

        while (intIndexOfMatch != -1) {
            _text = _text.replace(strTarget, strSubString)
            intIndexOfMatch = _text.indexOf(strTarget);
        }
        return (_text);
    }
</script>

