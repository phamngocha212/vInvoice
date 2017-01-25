<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HISVATInvoice>" %>
<%@ Import Namespace="EInvoice.Core" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="EInvoice.HISExtends.Domain" %>
<script type="text/javascript" src="/Content/js/Share.js"></script>
<script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
<style type="text/css">
    label.error {
        padding: 0 4px;
        position: absolute;
        background-color: #fff;
        line-height: 22px !important;
        font-weight: normal !important;
        border: none;
        border-bottom: 1px dashed #f00;
    }

    .ui-autocomplete {
        border-radius: 0;
        border: 1px dashed #666;
    }

    .ui-corner-all {
        border-radius: 0;
    }

    .ui-menu .ui-menu-item a {
        font-size: .85em;
        line-height: 1.4em;
        padding: 2px .4em;
    }

    .ui-state-hover, .ui-widget-content .ui-state-hover, .ui-widget-header .ui-state-hover, .ui-state-focus, .ui-widget-content .ui-state-focus, .ui-widget-header .ui-state-focus {
        color: #000;
        background: #ccc;
        border: 1px solid #666;
    }

    select {
        min-height: 22px;
    }

    select, textarea, input[type=text] {
        resize: none;
        padding: 0px 5px;
        border: none;
        border-bottom: 1px dashed #ccc;
        height: 22px;
    }

        select.error, textarea.error, input[type=text].error {
            border: none;
            border-bottom: 1px dashed #f00;
        }

        select:focus, textarea:focus, input[type=text]:focus {
            outline: none;
            border: none;
            border-bottom: 1px dashed #000;
        }

    input[type=file] {
        display: none;
        visibility: hidden;
    }

    input[readonly=readonly] {
        cursor: pointer;
    }

    textarea {
        min-height: 55px;
        background-color: #fee;
    }

    .textr {
        text-align: right;
    }

    .textc {
        text-align: center;
    }

    .widget-header {
        margin-top: 3px;
        text-align: center;
        padding-bottom: 7px;
    }

    .frm-header {
        margin: 0;
        padding: 0;
        overflow: hidden;
    }

    div.left {
        width: 75%;
        float: left;
    }

    div.right {
        overflow: hidden;
        padding-left: 10px;
    }

    div.w80 {
        min-width: 80px;
    }

    div.w90 {
        min-width: 90px;
    }

    div.w100 {
        min-width: 100px;
    }

    div.w120 {
        min-width: 120px;
    }

    div.w160 {
        min-width: 160px;
    }

    div.w180 {
        min-width: 180px;
    }

    div.w50p {
        width: 50%;
    }

    div.w60p {
        width: 60%;
    }

    div.w70p {
        width: 70%;
    }

    div.w40p {
        width: 40%;
    }

    div.w30p {
        width: 30%;
    }

    div.push-60 {
        padding-left: 60%;
    }

    div.push-70 {
        padding-left: 70%;
    }

    div.line {
        padding: 3px 0;
        overflow: hidden;
    }

        div.line.null {
            min-height: 22px;
            color: transparent;
            content: 'einvoice';
        }

        div.line.last {
            padding-bottom: 30px;
        }

        div.line > div.label {
            float: left;
            text-align: left;
        }

    div.right > div.line > div.label {
        width: 110px;
    }

    div.line label {
        font-weight: bold;
        line-height: 22px;
    }

    div.line > div.control span.mst {
        float: left;
        padding: 0px 6px !important;
        border: 1px solid #ccc !important;
    }

    div.line > div.control span.ml3 {
        margin-left: 3px;
    }

    div.line > div.control span.nbl:not(:first-child) {
        border-left: none !important;
    }

    div.line > div.label label:after {
        content: ':';
    }

    div.line > div.imp label::after {
        content: ' (*):';
        color: #f00;
    }

    div.line > div.control {
        overflow: hidden;
        padding-left: 7px;
    }

        div.line > div.control input, div.line > div.control select, div.line > div.control textarea {
            width: 100%;
            -moz-box-sizing: border-box;
            -webkit-box-sizing: border-box;
            box-sizing: border-box;
        }

    .pdt100 {
        padding-top: 100px;
    }
</style>
<% 
    IInvoice inv = (IInvoice)ViewData["Data"];
    Company oCompany = (Company)ViewData["company"];
    List<string> taxCode = new List<string>();
    string temp = oCompany.TaxCode + "                                             ";
    for (int i = 0; i < 14; i++)
    {
        taxCode.Add(temp[i].ToString());
    }
%>

<%: Html.Hidden("id",0) %>
<%: Html.Hidden("ComID", Model.ComID) %>
<%: Html.Hidden("ComPhone", oCompany.Phone)%>
<%: Html.Hidden("ComBankName", oCompany.BankName)%>
<%: Html.Hidden("ComBankNo", oCompany.BankNumber)%>
<%: Html.Hidden("ComTaxCode", oCompany.TaxCode) %>
<%: Html.Hidden("ComFax", oCompany.Fax) %>
<%: Html.Hidden("PubDatasource") %>
<%: Html.Hidden("VatAmount0") %>
<%: Html.Hidden("VatAmount5") %>
<%: Html.Hidden("VatAmount10") %>
<%: Html.Hidden("GrossValue") %>
<%: Html.Hidden("GrossValue0") %>
<%: Html.Hidden("GrossValue5") %>
<%: Html.Hidden("GrossValue10") %>
<%: Html.Hidden("NoInv", Model.No) %>
<%: Html.Hidden("SerialNo", Model.Serial) %>
<%: Html.Hidden("ComPhone", oCompany.Phone)%>
<%: Html.Hidden("CreateBy", HttpContext.Current.User.Identity.Name)%>
<%: Html.Hidden("Pattern", Model.Pattern)%>
<%: Html.Hidden("NewSerial", ViewData["NewSerial"])%>
<%= Html.Hidden("Name", Model.Name.ToUpper())%>
<%string newPattern = ViewData["NewPattern"] as string; %>
<div class="box">
    <div class="box-header with-border">
        <i class="fa fa-paper-plane"></i><b><%: Resources.Einvoice.MInv_lblInputDetailInfomationInvoice%></b>
    </div>

    <div class="frm-header">
        <div class="left">
            <div class="line">
                <div class="label w120">
                    <%: Html.LabelFor(m => m.ComName, labelText: Resources.Einvoice.MInv_LblComName) %>
                </div>
                <div class="control">
                    <%: Html.TextBox("ComName", oCompany.Name, new { @readonly = "readonly", @tabindex = -1, maxlength="200" })%>
                </div>
            </div>
            <div class="line">
                <div class="label w120"><%: Html.LabelFor(m => m.ComTaxCode, Resources.Einvoice.MInv_LblComTaxcode) %></div>
                <div class="control">
                    <% int _taxindex = 0;
                       int[] space = new[] { 2, 10, 13 };
                       foreach (var tax in taxCode)
                       {
                    %>
                    <span class="mst <%: Html.Raw( space.Contains(_taxindex) ? "ml3" : "nbl"  ) %>"><%:Html.Raw(String.IsNullOrEmpty(tax.Trim()) ? "&nbsp" : tax) %></span>
                    <%_taxindex++;
                       } %>
                </div>
            </div>

        </div>
        <div class="line">
            <div class="label w90">
                <%: Html.LabelFor(m => m.Pattern, labelText: Resources.Einvoice.MInv_LblPattern) %>
            </div>
            <div class="control">
                <%: Html.TextBox("NewPattern", newPattern, new { @readonly = "readonly", @tabindex = -1 })%>
            </div>
        </div>
        <div class="line">
            <div class="label w90"><%: Html.LabelFor(m => m.MaBA, labelText: "Số phiếu thu") %></div>
            <div class="control"><%= Html.TextBox("SoPT", Model.SoPT)%></div>
        </div>
    </div>

    <div class="line">
        <div class="label w120"><%: Html.LabelFor(m => m.ComAddress, labelText: Resources.Einvoice.MInv_LblAddress) %></div>
        <div class="control"><%: Html.TextBox("ComAddress", oCompany.Address, new {maxlength="290",@readonly="readonly"})%></div>
    </div>

    <div class="line">
        <div class="label w120 imp"><%: Html.LabelFor(m => m.CusName, labelText: "Họ tên người mua, Tên đơn vị") %></div>
        <div class="control"><%: Html.TextBoxFor(m => m.CusName, new { maxlength="150", @required = "required" })%></div>
    </div>

    <div class="line">
        <div class="label w120">
            <%: Html.LabelFor(m => m.CusAddress, labelText: Resources.Einvoice.MInv_LblAddress) %>
        </div>
        <div class="control">
            <%: Html.TextBox("CusAddress", Model.CusAddress, new { maxlength = "290" })%>
        </div>
    </div>

    <div class="line">
        <div class="label w80">
            <%: Html.LabelFor(m => m.CusCode,  labelText: "Mã bệnh nhân") %>
        </div>
        <div class="control">
            <%: Html.TextBoxFor(m => m.CusCode, new { @class="textandnum", maxlength="100"})%>
        </div>
    </div>

    <div class="left">
        <div class="line">
            <div class="label w180 imp">
                <%: Html.LabelFor(m => m.Total, labelText: "Thành tiền không chịu thuế GTGT") %>
            </div>
            <div class="control">
                <%if (Model.Type == InvoiceType.ForAdjustInfo)
                  {%>
                <%: Html.TextBox("Total", 0, new { @class="textr _number", @readonly = "readonly"}) %>
                <%}
                  else
                  { %>
                <%: Html.TextBox("Total", Model.Total, new { @class="textr _number", @required = "required"}) %>
                <%} %>
            </div>
        </div>
    </div>
    <div class="right">
        <div class="line">
            <div class="label">
                <label>Thuế GTGT</label>
            </div>
            <div class="control">
                <%: Html.DropDownList("VATRate",new[]
                        {
                            new SelectListItem{Text="0% ", Value="0", Selected= (Model.GrossValue0>0)},
                            new SelectListItem{Text="5%", Value="5",Selected= (Model.GrossValue5>0)},
                            new SelectListItem{Text="10%", Value="10",Selected= (Model.GrossValue10>0)},
                            new SelectListItem{Text=Resources.Einvoice.MInv_TxtNotVATRate, Value="-1",Selected= (Model.GrossValue >0)}
                        }, new {@class = "required ", title = "Chọn thuế GTGT"})%>
            </div>
        </div>
    </div>
    <div class="line w120">
        <div class="label">
            <%: Html.LabelFor(m => m.Amount, labelText: Resources.Einvoice.MInv_Amount) %>
        </div>
        <div class="control">
            <%: Html.Hidden("VATAmount", Model.Type != InvoiceType.ForAdjustInfo ?  Model.VATAmount: 0)%>
            <%: Html.TextBox("Amount", Model.Type != InvoiceType.ForAdjustInfo ? Model.Amount : 0, new { @readonly = "readonly", @class="_number", @tabindex = -1  }) %>
        </div>
    </div>
    <div class="line w120">
        <div class="label">
            <%: Html.LabelFor(m => m.AmountInWords, Resources.Einvoice.MInv_LblAmountInWords) %>
        </div>
        <div class="control">
            <%: Html.TextBox("AmountInWords", Model.Type != InvoiceType.ForAdjustInfo ? Model.AmountInWords: "Không đồng", new { @readonly = "readonly", @tabindex = -1 })%>
        </div>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $("input[type=text]:first").focus();
        $("#CusAddress").keyup(function () {
            checkTextMaxLength(this)
        });

        cutbyMaxlength('ComAddress', 290);

        $("#CusName").autocomplete({
            focus: function (event, ui) { },
            minLength: 1,
            select: function (event, ui) {
                $("#CusTaxCode").val(ui.item.TaxCode);
                $("#CusAddress").val(ui.item.Address);
                $("#CusPhone").val(ui.item.Phone);
                $("#CusCode").val(ui.item.Code)
            },
            source: function (request, response) {
                $.ajax({
                    data: {
                        searchText: request.term
                    },
                    dataType: "JSON",
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                id: item.cusid,
                                value: item.Name,
                                TaxCode: item.TaxCode,
                                Address: item.Address,
                                Phone: item.Phone,
                                Code: item.Code
                            }
                        }))
                    },
                    type: "POST",
                    url: "/Customer/SeachByName"
                })
            }
        }).change(function () {
            if (!$(this).val()) {
                $("#CusAddress").val("");
            }
        });
        $("#Total").change(function () {
            $("#VATRate").trigger("change");

        });
        $("#VATRate").change(function () {
            var _vatrate = parseInt($(this).val());

            if ($("#Total").val()) {
                var _totalamount = parseFloat($("#Total").val().replaceAll(",", "") || 0) || 0;
                var _vatamount = parseFloat(_totalamount / 100 * _vatrate);
                var _amount = 0;
                if (_vatrate > 0) {
                    $("#VATAmount").val(_vatamount.format(0, 3));
                    _amount = parseFloat(_totalamount + _vatamount);

                } else {
                    $("#VATAmount").val("0");
                    _amount = _totalamount;
                }

                if (_amount < 0) {
                    _amount = 0;
                }

                $("#Amount").val(_amount).attr("value", _amount.format(0, 3));
                $("#AmountInWords").val(_amount.ReadNumber())
            } else {
                $("#Amount").val(0).attr("value", 0);
                $("#AmountInWords").val("")
            }
        });

        $("button[type=submit]").click(function () {
            var check = 0;
            $("#VatAmount0").val("0");
            $("#VatAmount5").val("0");
            $("#VatAmount10").val("0");
            $("#GrossValue").val("0");
            $("#GrossValue0").val("0");
            $("#GrossValue5").val("0");
            $("#GrossValue10").val("0");

            var _valid = $("form:first").valid();
            if (!_valid) {
                $('input:text[required]').parent().show();
                return false;
            }

            var _totalamount = parseFloat($("#Total").val().replaceAll(",", "") || 0) || 0;
            var _vatamount = parseFloat($("#VATAmount").val().replaceAll(",", "") || 0) || 0;
            var _vatrate = parseInt($("#VATRate").val());
            if (_vatrate > 0) {
                if (_vatrate == 10) {
                    $("#VatAmount10").val(_vatamount);
                    $("#GrossValue10").val(_totalamount)
                }
                if (_vatrate == 5) {
                    $("#VatAmount5").val(_vatamount);
                    $("#GrossValue5").val(_totalamount)
                }
            } else {
                if (_vatrate == -1) {
                    $("#GrossValue").val(_totalamount)
                } else {
                    $("#VatAmount0").val("0");
                    $("#GrossValue0").val(_totalamount)
                }
            }

            if ($("#Total").val().indexOf("-") >= 0) {
                sweetAlert("Lỗi", "Hóa đơn có giá trị âm [Tổng tiền hóa đơn], chưa thể phát sinh giao dịch.", "error");

                return false;
            }
            $("input[type=text]._number").each(function (i, item) {
                var _val = $(this).val();
                $(this).val(_val.replaceAll(",", "").replaceAll(".", ","))
            });

            var _products = [];
            var _product = {};
            _product.ProdType = 1;
            _product.Code = "";
            _product.Name = "Thu tiền khám chữa bệnh";
            _product.Unit = "";
            _product.Quantity = 0;
            _product.Price = 0;
            _product.Amount = $("#Amount").val();
            _products.push(_product);

            if (!$("#Total").val() || $("#Total").val() == "0") {
                swal({
                    title: "Cảnh báo!",
                    text: "Hóa đơn chưa có giá trị [Tổng tiền hóa đơn]. Bạn có muốn tiếp tục lưu hóa đơn không?",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Tiếp tục",
                    cancelButtonText: "Quay lại",
                    closeOnConfirm: false,
                    closeOnCancel: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        return true;
                    } else {
                        return false;
                    }
                });
            }
            $("#PubDatasource").val(JSON.stringify(_products));
        });
    });

    new function ($) {
        $.fn.getCursorPosition = function () {
            var pos = 0;
            var el = $(this).get(0);
            if (document.selection) {
                el.focus();
                var Sel = document.selection.createRange();
                var SelLength = document.selection.createRange().text.length;
                Sel.moveStart("character", -el.value.length);
                pos = Sel.text.length - SelLength
            } else if (el.selectionStart || el.selectionStart == "0")
                pos = el.selectionStart;
            return pos
        }
    }(jQuery);
</script>
