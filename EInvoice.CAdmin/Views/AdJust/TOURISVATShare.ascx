<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TourisVATInvoice>" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="EInvoice.TourisExtends.Domain" %>
<script type="text/javascript" src="/Content/js/Share.js"></script>
<script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
<script src="/Content/js/hwcrypto.js"></script>
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
        width: 70px;
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

    table.products {
        width: 100%;
        border-collapse: collapse;
    }

        table.products td, table.products th {
            font-size: 0.95em;
            border: 1px solid #666;
            padding: 3px 7px 2px 7px;
        }

        table.products td {
            padding: 0;
            margin: 0;
            line-height: 20px;
        }

        table.products th {
            text-align: center;
            background-color: #ccc;
            color: #000;
        }

            table.products th.np {
                padding: 0;
            }

        table.products input[type=text] {
            width: 100%;
            -moz-box-sizing: border-box;
            -webkit-box-sizing: border-box;
            box-sizing: border-box;
            border: 1px solid transparent;
        }

        table.products th input[type=text] {
            border: 1px dashed #ccc;
        }

        table.products input[type=text]:focus {
            border: 1px dashed #666;
        }

        table.products td.center {
            text-align: center;
        }

        table.products input[type=checkbox] {
            min-height: 0px;
        }

        table.products tr.alt td {
            color: #000;
            background-color: #EAF2D3;
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

<%: Html.Hidden("id", 0) %>
<%: Html.Hidden("ComID", Model.ComID) %>
<%: Html.Hidden("ComPhone", oCompany.Phone)%>
<%: Html.Hidden("ComBankName", oCompany.BankName)%>
<%: Html.Hidden("ComBankNo", oCompany.BankNumber)%>
<%: Html.Hidden("ComTaxCode", oCompany.TaxCode) %>
<%: Html.Hidden("ComFax", oCompany.Fax) %>
<%: Html.Hidden("ParentName", oCompany.ParentName) %>
<%: Html.Hidden("PubDatasource") %>
<%: Html.Hidden("VatAmount0",Model.VatAmount0) %>
<%: Html.Hidden("VatAmount5",Model.VatAmount5) %>
<%: Html.Hidden("VatAmount10",Model.VatAmount10) %>
<%: Html.Hidden("GrossValue",Model.GrossValue) %>
<%: Html.Hidden("GrossValue0",Model.GrossValue0) %>
<%: Html.Hidden("GrossValue5",Model.GrossValue5) %>
<%: Html.Hidden("GrossValue10",Model.GrossValue10) %>
<%: Html.Hidden("NoInv", Model.No) %>
<%: Html.Hidden("SerialNo", Model.Serial) %>
<%: Html.Hidden("ComPhone", oCompany.Phone)%>
<%: Html.Hidden("CreateBy", HttpContext.Current.User.Identity.Name)%>
<%: Html.Hidden("Pattern", Model.Pattern)%>
<%: Html.Hidden("VATRate", Model.VATRate)%>
<%: Html.Hidden("Total", Model.Total)%>
<%: Html.Hidden("VATAmount", Model.VATAmount)%>
<%: Html.Hidden("NewSerial", ViewData["NewSerial"])%>
<%string newPattern = ViewData["NewPattern"] as string; %>
<div class="box">
    <div class="box-header with-border">
        <i class="fa fa-paper-plane"></i><b><%: Resources.Einvoice.MInv_lblInputDetailInfomationInvoice%></b>
    </div>
    <div class="frm-header">
        <div class="left">
            <div class="line">
                <div class="label w120"><%: Html.LabelFor(m => m.Name, labelText: Resources.Einvoice.MInv_LblNameInv) %></div>
                <div class="control"><%= Html.TextBox("Name", Model.Name, new { @class="required textc", maxlength="150" })%></div>
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
            <div class="line">
                <div class="label w120">
                    <%: Html.LabelFor(m => m.ComName, labelText: Resources.Einvoice.MInv_LblComName) %>
                </div>
                <div class="control">
                    <%: Html.TextBox("ComName", oCompany.Name, new { @readonly = "readonly", @tabindex = -1, maxlength="200" })%>
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
    </div>
    <div class="line">
        <div class="label w120"><%: Html.LabelFor(m => m.ComAddress, labelText: Resources.Einvoice.MInv_LblAddress) %></div>
        <div class="control"><%: Html.TextBox("ComAddress", oCompany.Address, new { maxlength="200", @readonly = "readonly"})%></div>
    </div>
    <div class="line">
        <div class="label w120"><%: Html.LabelFor(m => m.Buyer, labelText: "Họ tên người mua hàng") %></div>
        <div class="control"><%: Html.TextBox("Buyer", Model.Buyer, new { maxlength="200"})%></div>
    </div>
    <div class="left w30p">
        <div class="line">
            <div class="label w80"><%: Html.LabelFor(m => m.CusTaxCode, labelText: Resources.Einvoice.MInv_LblCusTaxCode) %></div>
            <div class="control">
                <%= Html.TextBox("CusTaxCode", Model.CusTaxCode, new { @maxlength = 14})%>
            </div>
        </div>
    </div>
    <div class="right w70p">
        <div class="line">
            <div class="label <%=string.IsNullOrWhiteSpace(Model.Buyer) ? "imp" : null %>" style="width: 130px;"><%: Html.LabelFor(m => m.CusName, labelText: "Tên đơn vị mua") %></div>
            <div class="control">

                <%: Html.TextBoxFor(m => m.CusName, new {@class = string.IsNullOrWhiteSpace(Model.Buyer) ?  "required" : "", maxlength="150" })%>
            </div>
        </div>

    </div>
    <div class="left w70p">
        <div class="line">
            <div class="label"><%: Html.LabelFor(m => m.CusBankNo, labelText:"Số tài khoản") %></div>
            <div class="control"><%: Html.TextBoxFor(m => m.CusBankNo)%></div>
        </div>
    </div>
    <div class="right">
        <div class="line">
            <div class="label w80"><%: Html.LabelFor(m => m.CusBankName, labelText: "Tại") %></div>
            <div class="control">
                <%= Html.TextBoxFor( m=>m.CusBankName, new { @maxlength = 200})%>
            </div>
        </div>
    </div>
    <div class="line">
        <div class="label w120">
            <%: Html.LabelFor(m => m.CusAddress, labelText: Resources.Einvoice.MInv_LblAddress) %>
        </div>
        <div class="control">
            <%: Html.TextBoxFor(m => m.CusAddress, new {maxlength="450"})%>
        </div>
    </div>
    <%= Html.Hidden("CusPhone", Model.CusPhone) %>
    <%= Html.Hidden("CusCode",Model.CusCode)%>
    <div class="line">
        <div class="label w120 imp">
            <%: Html.LabelFor(m => m.PaymentMethod, labelText: "Hình thức thanh toán") %>
        </div>
        <div class="control">
            <%: Html.DropDownListFor(m => m.PaymentMethod, new[]
                        {
                            new SelectListItem{Text=Resources.Einvoice.Minv_txtCashingPay, Value="TM", Selected= (Model.PaymentMethod == "TM")},
                            new SelectListItem{Text=Resources.Einvoice.MInv_txtTransferPay, Value="CK",Selected= (Model.PaymentMethod == "CK")},
                            new SelectListItem{Text=Resources.Einvoice.Minv_txtCreditCardPay, Value="TTD",Selected= (Model.PaymentMethod == "TTD")},                            
                            new SelectListItem{Text=Resources.Einvoice.MInv_Minv_txtCashingPayAndTransferPay,Value="TM/CK",Selected=(Model.PaymentMethod=="TM/CK")},
                            new SelectListItem{Text=Resources.Einvoice.MInv_txtClearingPay,Value="Bù trừ",Selected=(Model.PaymentMethod=="Bù trừ")}
                        }, "-- Chọn hình thức thanh toán --", new { @required = "required" })%>
        </div>
    </div>
    <div class="line">
        <div class="label"><%: Html.Label("FileUpload", "Biên bản hủy đính kèm") %></div>
        <div class="control">
            <input type="file" name="FileUpload" id="_fleAttackment" accept=".doc,.docx,.pdf">
            <input type="text" id="_txtFile" readonly="readonly" placeholder="Nhấn chuột vào đây để đính kèm biên bản hủy (doc, docx, pdf)..." />
        </div>
    </div>
    <div class="line">
        <div class="label w120">
            <%: Html.LabelFor(m => m.Note, labelText: "Ghi chú hóa đơn") %>
        </div>
        <div class="control">
            <%: Html.TextAreaFor(m => m.Note, new { maxlength="500"})%>
        </div>
    </div>
    <div class="line" style="text-align: center;">
        <p>
            (<%if (Model.Type == InvoiceType.ForReplace)
               {%>
            Thay thế  
        <%}
               else
               { %>
            <%=ViewData["typeName"] %>
            <%} %>
        cho hóa đơn điện tử số <b><%=Model.No.ToString("0000000") %></b>, mẫu số <b><%=Model.Pattern %></b>, ký hiệu <b><%=Model.Serial %></b>)
        </p>
    </div>
    <div class="line">
        <table class="products">
            <thead>
                <tr>
                    <th style="width: 20px">Xóa</th>
                    <th style="width: 25px">STT</th>
                    <th class="np">Tên hàng hóa, dịch vụ</th>
                    <th style="width: 90px">ĐVT (Unit)</th>
                    <th style="width: 70px">Số lượng (Quantity)</th>
                    <th style="width: 100px">Đơn giá (Price)</th>
                    <th style="width: 100px">Thành tiền (Total) VNĐ</th>
                    <th style="width: 137px">Thuế suất GTGT(%)</th>
                    <th style="width: 100px">Thuế GTGT</th>
                    <th style="width: 100px">Cộng</th>
                    <th style="width: 50px">Không tính tiền</th>
                </tr>
            </thead>
            <tbody>
                <%
                    int index = 1;
                    if (Model.Type != InvoiceType.ForAdjustInfo)
                    {
                        foreach (var product in Model.ProductList)
                        {  %>
                <tr class="prodata">
                    <td class="delr" style="text-align: center"><i class="fa fa-trash"></i></td>
                    <td class="center"><%: index %></td>
                    <td>
                        <input type="text" value="<%= Html.Encode(product.Name) %>" class="name" maxlength="200" />
                    </td>
                    <td>
                        <input type="text" value="<%= Html.Encode(product.Unit) %>" class="unit" maxlength="50" />
                    </td>
                    <td>
                        <input type="text" value="<%if (product.Quantity == 0)
                                                    { %><%}
                                                    else
                                                    { %><%= Html.Encode(product.Quantity) %><%} %>"
                            class="quantity textr _number onlynum" maxlength="18" />
                    </td>

                    <td>
                        <input type="text" value="<%= Html.Encode(product.Price) %>" class="price textr _number" maxlength="18" />
                    </td>
                    <td>
                        <input type="text" value="<%= Html.Encode(product.Total) %>" class="total textr _number" maxlength="18" />
                    </td>
                    <td class="vatselect">
                        <div class="control">

                            <%--Làm kiểu thủ công nghiệp--%>
                            <select class="required vatrate">
                                <option value="0" <%if (product.VATRate == 0)
                                                    {%>
                                    selected <%} %>>0%</option>
                                <option value="5" <%if (product.VATRate == 5)
                                                    {%>
                                    selected <%} %>>5%</option>
                                <option value="10" <%if (product.VATRate == 10)
                                                     {%>
                                    selected <%} %>>10%</option>
                                <option value="-1" <%if (product.VATRate == -1)
                                                     {%>
                                    selected <%} %>>Không thuế GTGT</option>

                            </select>
                        </div>
                    </td>
                    <td>
                        <input type="text" value="<%= Html.Encode(product.VATAmount) %>" class="vatamount textr _number" maxlength="18" />
                    </td>
                    <td>
                        <input type="text" value="<%= Html.Encode(product.Amount) %>" class="amount textr _number" maxlength="18" />
                    </td>
                    <td class="center">
                        <input type="checkbox" value="false" class="issum" <%: product.IsSum == 1 ? "checked=checked" : "" %> />
                    </td>
                </tr>
                <%
                                                     index++;
                        }
                    }
                    else
                    {
                        index = 1;
                %>
                <tr>
                    <td class="delr" style="text-align: center"><i class="fa fa-trash"></i></td>
                    <td class="center"><%: index %></td>
                    <td>
                        <input type="text" class="name" maxlength="200" />
                    </td>
                    <td>
                        <input type="text" class="unit" maxlength="50" readonly="true" disabled />
                    </td>
                    <td>
                        <input type="text" readonly="true" disabled class="quantity textr _number onlynum" maxlength="18" />
                    </td>

                    <td>
                        <input type="text" readonly="true" disabled class="price textr _number" maxlength="18" />
                    </td>
                    <td>
                        <input type="text" readonly="true" disabled class="total textr _number" maxlength="18" />
                    </td>
                    <td class="vatselect">
                        <div class="control">

                            <%--Làm kiểu thủ công nghiệp--%>
                            <select class="required vatrate" disabled>
                                <option value="0">0%</option>
                                <option value="5">5%</option>
                                <option value="10">10%</option>
                                <option value="-1">Không thuế GTGT</option>
                            </select>
                        </div>
                    </td>
                    <td>
                        <input type="text" class="vatamount textr _number" readonly="true" disabled />
                    </td>
                    <td>
                        <input type="text" readonly="true" disabled class="amount textr _number" maxlength="18" />
                    </td>
                    <td class="center">
                        <input type="checkbox" value="false" class="issum" />
                    </td>
                </tr>
                <%} %>
            </tbody>
            <tfoot>
                <tr>
                    <td colspan="3" style="padding: 5px 0 0 10px;"><%: Html.LabelFor(m => m.Amount, labelText: Resources.Einvoice.MInv_Amount) %></td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td colspan="3" style="padding-right: 10px;"><%: Html.TextBox("Amount", Model.Amount, new {@style="width:100%", @readonly = "readonly", @class="textr _number", @tabindex = -1  }) %></td>
                </tr>
                <tr>
                    <td colspan="11">
                        <div class="line last">
                            <div class="label">
                                <%: Html.LabelFor(m => m.AmountInWords, Resources.Einvoice.MInv_LblAmountInWords) %>
                            </div>
                            <div class="control">
                                <%: Html.TextBox("AmountInWords", EInvoice.Core.NumberToLeter.DocTienBangChu((long)Model.Amount), new { @readonly = "readonly", @tabindex = -1 })%>
                            </div>
                        </div>
                    </td>
                </tr>
            </tfoot>
        </table>
    </div>

</div>
<script type="text/javascript">

    function validateForm(){
        var valid = false;  
        //Kiểm tra dữ liệu sản phẩm.
        $('table.products tbody tr').each(function (i, row) {
            valid = true;
            if (!$(row).find('input[type=text].amount').val() && $(row).find('input[type=text].name').val()) {
                sweetAlert("Lỗi!", "Danh sách sản phẩm chưa có hoặc thông tin sản phẩm bị thiếu!", "error");
                return false;
            }
        });
        if(!valid) { 
            sweetAlert("Lỗi!", "Danh sách sản phẩm chưa có!", "error");
            return false;
        }
        if ($('#errorTaxCode').length > 0) {
            $('#errorTaxCode').show();
            $('#CusTaxCode').addClass('error');
            sweetAlert("Lỗi!", "Mã số thuế của khách hàng không đúng!", "error");
            return false;
        }
        
        if(!$('#Buyer').val() && !$('#CusName').val()){
            sweetAlert("Lỗi!", "Chưa nhập thông tin khách hàng", "error");
            return false;
        }

        if($("#PaymentMethod").val().length<=0)
        {   
            sweetAlert("Lỗi!", "Chưa chọn hình thức thanh toán", "error");
            return false;
        }

        if (!$('#Total').val() || $('#Total').val() == '0') {
            sweetAlert("Lỗi!", "Hóa đơn có giá trị âm [Tổng tiền hóa đơn], chưa thể phát sinh giao dịch.", "error");
            return false;
        }

        //Giá tiền ÂM
        if ($('#VATAmount').val().toString().indexOf('-') >= 0 || $('#Total').val().toString().indexOf('-') >= 0 || $('#Amount').val().toString().indexOf('-') >= 0) {
            swal({
                title: "Cảnh báo!",
                text: "Hóa đơn có giá trị [Cộng tiền dịch vụ] hoặc [Tiền thuế] hoặc [Tổng cộng tiền thanh toán] âm , bạn có muốn tiếp tục ?",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Tiếp tục",
                cancelButtonText: "Quay lại",
                closeOnConfirm: false,
                closeOnCancel: false
            }, function (isConfirm) {
                if (isConfirm) { return true;}
                else { return false; }
            });
        }
        
        return true;
    }

    function wrapProductData(){
        var _products = [];
        var check = 0;
        $("#VatAmount0").val("0");
        $("#VatAmount5").val("0");
        $("#VatAmount10").val("0");
        $("#GrossValue").val("0");
        $("#GrossValue0").val("0");
        $("#GrossValue5").val("0");
        $("#GrossValue10").val("0");

        $("input[type=text]._number").each(function (i, item) {
            var _val = $(this).val();
            $(this).val(_val.replaceAll(",", "").replaceAll(".", ","))
        });        
        var _VATAmount5 = 0;
        var _VATAmount10 = 0;
        var _VATAmount0 = 0;
        var _GrossValue5 = 0;
        var _GrossValue10 = 0;
        var _GrossValue0 = 0;
        var _GrossValue = 0;
        $("table.products tbody tr").each(function (i, row) {                
            var _product = {};
            var _pCode = $(row).find("input[type=text].code");
            var _pName = $(row).find("input[type=text].name");
            var _pUnit = $(row).find("input[type=text].unit");
            var _pQuantity = $(row).find("input[type=text].quantity");
            var _pPrice = $(row).find("input[type=text].price");
            var _pTotal = $(row).find("input[type=text].total");
            var _pVATRate = $(row).find("select.vatrate");
            var _pVATAmount = $(row).find("input[type=text].vatamount");
            var _pAmount = $(row).find("input[type=text].amount");
            var _pIsSum = $(row).find("input[type=checkbox]:first");

            var _quantity = 1;
            if (_pName.val()) {
                _quantity = 1;
                if (_pQuantity.val().length > 0) {
                    _quantity = (_pQuantity.val().replaceAll(",", "") || 1) || 1;
                } else {
                    _quantity = 1;
                }
                var _total = _pTotal.val().replaceAll(",", "");

                var pTotalamount = parseFloat(_total|| 0) || 0;
                if (!_pIsSum.is(":checked")) {
                    if (_pVATRate.val() > 0) {
                        var _vatamount = parseFloat(_pVATAmount.val().replaceAll(",", "") || 0) || 0;

                        if (_pVATRate.val() == 10) {
                            _VATAmount10 = _VATAmount10 + _vatamount;
                            _GrossValue10 = _GrossValue10 + pTotalamount;
                        }
                        if (_pVATRate.val() == 5) {
                            _VATAmount5 = _VATAmount5 + _vatamount;
                            _GrossValue5 = _GrossValue5 + pTotalamount;
                        }
                    } else {
                        _GrossValue = _GrossValue + pTotalamount;                            
                    }
                }

                _product.ProdType = 1;
                _product.Name = _pName.val() || "";
                _product.Code= _pCode.val() || "";
                _product.Unit = _pUnit.val() || "";
                if (_pQuantity.val().length > 0) {
                    _quantity = (_pQuantity.val().replaceAll(",", "") || 0) || 0;
                } else {
                    _quantity = 0;
                }
                _product.Quantity = _quantity;                    
                _product.Price = parseFloat(_pPrice.val() || 0) || 0;
                _product.Total = pTotalamount;
                _product.VATRate = _pVATRate.val();
                _product.VATAmount = parseFloat(_pVATAmount.val().replaceAll(",", "") || 0) || 0;
                _product.Amount = _pAmount.val() || 0;
                _product.IsSum = _pIsSum.is(":checked");

                if (_product.Amount < 0) {
                    _product.Amount = _product.Amount * (-1)
                }
                if (_product.Amount != "" && _product.Name == "") {
                    check = 0;
                    _products.length = 0;
                    return false
                }
                if ((_product.Amount.toString().length > 18) || (_product.Amount.toString().indexOf("e") >= 0)) {
                    check = 1;
                    _products.length = 0;
                    return false
                }
                _products.push(_product);
            }
        });
        if (_products.length == 0) {
            if (check == 0) {
                sweetAlert("Lỗi!", "Danh sách sản phẩm chưa có hoặc thông tin sản phẩm bị thiếu!", "error");
            } else {
                sweetAlert("Lỗi!", "Danh sách sản phẩm có chứa sản phẩm vượt quá giá trị cho phép của hóa đơn", "error");
            }
            return undefined;
        }

        $('#GrossValue').val(_GrossValue);
        $('#GrossValue5').val(_GrossValue5);
        $('#GrossValue0').val(_GrossValue0);
        $('#GrossValue10').val(_GrossValue10);
        $('#VatAmount5').val(_VATAmount5);
        $('#VatAmount10').val(_VATAmount10);
        $('#VatAmount0').val(_VATAmount0);
        $("input[type='text']").each(function (i) {
            checkTextMaxLength(this);
        });
         
        return _products;
    }

    function wrapInvoiceData(){
        var invData = {};
        invData.Pattern = $('#NewPattern').val();        
        invData.SerialNo = $('#NewSerial').val();
        invData.Buyer = $("#Buyer").val();
        invData.CusName = $("#CusName").val();
        invData.CusCode = $("#CusCode").val();
        invData.CusPhone = $("#CusPhone").val();
        invData.CusBankNo = $('#CusBankNo').val();
        invData.CusBankName = $('#CusBankName').val();
        invData.CusAddress = $("#CusAddress").val();
        invData.CusTaxCode = $("#CusTaxCode").val();
        invData.PaymentMethod = $("#PaymentMethod").val();        
        invData.Total = parseFloat($('#Total').val().replaceAll(',', '') || 0);
        invData.VATRate = $("#VATRate").val();
        invData.VATAmount = parseFloat($('#VATAmount').val().replaceAll(',', '') || 0);
        invData.Amount = parseFloat($('#Amount').val().replaceAll(',', '') || 0);
        invData.Total = parseFloat($('#Total').val().replaceAll(',', '') || 0);
        invData.VatAmount0 = $("#VatAmount0").val();
        invData.VatAmount5 = $("#VatAmount5").val();
        invData.VatAmount10 = $("#VatAmount10").val();
        invData.GrossValue = $("#GrossValue").val();
        invData.GrossValue0 = $("#GrossValue0").val();
        invData.GrossValue5 = $("#GrossValue5").val();
        invData.GrossValue10 = $("#GrossValue10").val();
        invData.CreateBy = $("#CreateBy").val();        
        invData.Note = $("#Note").val();
        invData.AmountInWords = $("#AmountInWords").val();
        var products = wrapProductData();
        if(products === undefined || products.length === 0){
            sweetAlert("Lỗi!", "Danh sách sản phẩm chưa có hoặc thông tin sản phẩm bị thiếu!", "error");
            return undefined;
        }
        invData.Products = products;
        return JSON.stringify(invData);
    }

    function publishAdjustInvByPlugIn() {
        if (validateForm() == true) {
            console.log("Valid Done");
            $.blockUI({
                css: {
                    border: 'none',
                    padding: '15px',
                    backgroundColor: '#eee',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: .5,
                    color: '#fff'
                },
                message: '<h1>Xin vui lòng đợi.</h1>', fadeIn: 0,
                fadeOut: 10, timeout: 240000
            });

            window.hwcrypto.selectCertSerial({
                lang: "en"
            }).then(function (response) {
                var certificate = response.value.cert;
                var invData = wrapInvoiceData();
                if(invData === undefined){
                    sweetAlert("Lỗi!", "Danh sách hóa đơn chưa có hoặc thông tin hóa đơn bị thiếu!", "error");
                    return;
                }
                var jsonData = { 
                    invData : invData,
                    NewPattern: $("#NewPattern").val(),
                    NewSerial: $("#NewSerial").val(),
                    OriNo: $('#NoInv').val(),
                    OriPattern: $("#Pattern").val(), 
                    OriSerial:$('#SerialNo').val(),       
                    type: <%=(int)Model.Type%>,
                    CertBase64String: certificate
                };
                console.log(jsonData);
                $.ajax({
                    type: "POST",
                    url: "/AjaxData/LaunchAdjustByPlugin/",
                    data: jsonData,
                    success: function (data) {
                        console.log(data);
                        if (data != "") {

                            if (data === "ERROR:2") {
                                sweetAlert("Thông báo", "Chứng thư số chưa được đăng ký, liên hệ để được hỗ trợ.", "error");
                                $.unblockUI();
                                return;
                            }
                            if (data === "ERROR:3") {
                                sweetAlert("Thông báo", "Chứng thư số hết hạn, liên hệ để được hỗ trợ.", "error");
                                $.unblockUI();
                                return;
                            }
                            if (data === "ERROR:4") {
                                sweetAlert("Thông báo", "Lỗi hệ thống, liên hệ để được hỗ trợ.", "error");
                                $.unblockUI();
                                return;
                            }
                            var hashData = data.hashdata;                                                    
                            window.hwcrypto.signHashData(certificate, {
                                type: "xmlwithcert",
                                hex: hashData.Value
                            }, {
                                lang: "en"
                            }).then(function (response) {

                                // Nhận kết quả từ hàm ký                                    
                                var signedData = response.value.signature;
                                if (signedData == null) {
                                    sweetAlert("Thông báo", "Chưa ký được hóa đơn, vui lòng thực hiện lại.", "error");
                                    $.unblockUI();
                                    return;
                                }
                                wrapInvoices(hashData.Key, signedData, $('#Pattern').val(), $('#SerialNo').val(), $('#NoInv').val(),"/AjaxData/WrapAdjustInvoices/",'/AdJust/adjustinvindex?pattern=' + $('#Pattern').val());                                
                            }, function (err) {
                                //Nhận lỗi trả về từ hàm ký (nếu có); bắt buộc phải lọc trường hợp user_cancelled
                                sweetAlert("Thông báo", err.message + " " + "(" + err.result + ")", "error");
                                $.unblockUI();
                            });


                        } else {
                            sweetAlert("Thông báo", "Chưa ký được hóa đơn, vui lòng thực hiện lại.", "error");
                            $.unblockUI();
                            return;
                        } 
                    }
                });
            }, function (err) {                
                $.unblockUI();
            }); 
        } else {
            sweetAlert("Thông báo", "Dữ liệu hóa đơn không hợp lệ, vui lòng kiểm tra lại!", "error");
            console.log("Valid Failed");
        }
    }
   
    function wrapInvoices(key, signed, pattern, serial, no, url, backLink) {        
        var jsondata = { key: key, signed: signed, pattern: pattern, serial: serial, no: no };
        $.ajax({
            type: "POST",
            url: url,
            data: jsondata,
            success: function (data) {
                if (data === "OK") {               
                    swal({ title: "Thông báo", text: "Điều chỉnh hóa đơn thành công", type: "success" },
                        function () { 
                            document.location = backLink; 
                        }
                    );
                }
                else
                    sweetAlert("Thông báo", data, "error");
            }
        });
    }
    
    function publishReplaceInvByPlugIn() {
        if (validateForm() == true) {
            $.blockUI({
                css: {
                    border: 'none',
                    padding: '15px',
                    backgroundColor: '#eee',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: .5,
                    color: '#fff'
                },
                message: '<h1>Xin vui lòng đợi.</h1>', fadeIn: 0,
                fadeOut: 10, timeout: 240000
            });
            window.hwcrypto.selectCertSerial({
                lang: "en"
            }).then(function (response) {
                var certificate = response.value.cert;
                var jsonData = { 
                    invData : wrapInvoiceData(),
                    NewPattern: $("#NewPattern").val(),
                    NewSerial: $("#NewSerial").val(),
                    OriNo: $("#NoInv").val(),
                    OriPattern: $("#Pattern").val(), 
                    OriSerial:$('#SerialNo').val(), 
                    OriNo: $('#NoInv').val(),
                    CertBase64String: certificate
                };
                console.log(jsonData);
                $.ajax({
                    type: "POST",
                    url: "/AjaxData/LaunchReplaceByPlugin",
                    data: jsonData,
                    success: function (data) {
                        console.log(data);
                        if (data != "") {

                            if (data === "ERROR:2") {
                                sweetAlert("Thông báo", "Chứng thư số chưa được đăng ký, liên hệ để được hỗ trợ.", "error");
                                $.unblockUI();
                                return;
                            }
                            if (data === "ERROR:3") {
                                sweetAlert("Thông báo", "Chứng thư số hết hạn, liên hệ để được hỗ trợ.", "error");
                                $.unblockUI();
                                return;
                            }

                            var hashData = data.hashdata;                            
                            window.hwcrypto.signHashData(certificate, {
                                type: "xmlwithcert",
                                hex: hashData.Value
                            }, {
                                lang: "en"
                            }).then(function (response) {

                                // Nhận kết quả từ hàm ký                                    
                                var signedData = response.value.signature;
                                if (signedData == null) {
                                    sweetAlert("Thông báo", "Chưa ký được hóa đơn, vui lòng thực hiện lại.", "error");
                                    $.unblockUI();
                                    return;
                                }
                                wrapInvoices(hashData.Key, signedData, $('#Pattern').val(), $('#SerialNo').val(), $('#NoInv').val(), "/AjaxData/WrapReplaceInvoices/", '/AdJust/replaceinvindex?pattern=' + $('#Pattern').val());

                                $.unblockUI();
                            }, function (err) {
                                //Nhận lỗi trả về từ hàm ký (nếu có); bắt buộc phải lọc trường hợp user_cancelled
                                sweetAlert("Thông báo", err.message + " " + "(" + err.result + ")", "error");
                                $.unblockUI();
                            });
                        } else {
                            sweetAlert("Thông báo", "Chưa ký được hóa đơn, vui lòng thực hiện lại.", "error");
                            $.unblockUI();
                            return;
                        } 

                    }
                });
            }, function (err) {
                // Nhận lỗi trả về từ hàm lấy chứng thư (nếu có); bắt buộc phải lọc trường hợp user_cancelled
                sweetAlert("Thông báo", err.message + " " + "(" + err.result + ")", "error");
                $.unblockUI();
            });
        } else {
            sweetAlert("Thông báo", "Dữ liệu hóa đơn không hợp lệ, vui lòng kiểm tra lại!", "error");
            console.log("Valid Failed");
        }
    }  

    function countTotalPrice() {
        var amount = 0.0;
        var totalamount = 0;
        var vatamount = 0;
        $("table.products tbody tr").each(function (i, row) {
            //Tính tổng tiền của tất cả các sản phẩm -> Chưa tính thuế;
            var _pName = $(row).find("input[type=text].name");
            var _isSum = $(row).find("input[type=checkbox]");
            var _pTotal = $(row).find("input[type=text].total");

            if (_pName.val()) {
                if (!_isSum.is(":checked")) {                    
                    var _total = _pTotal.val();                    
                    //Tính tổng tiền của tất cả các sản phẩm -> Chưa tính thuế;
                    totalamount = totalamount + parseFloat(_total.replaceAll(",", "") || 0) || 0;

                    //Tính tổng tiền của tất cả các sản phẩm -> Gồm cả thuế;
                    var totalvalue = parseFloat($(this).find('input.amount').val().replaceAll(",", "") || 0) || 0;
                    if (totalvalue > 0) {
                        amount = amount + totalvalue;
                    }
                }
            }
        });        
        $('#Total').val(totalamount);
        vatamount = amount - totalamount;        
        $('#VATAmount').val(vatamount);
        $("#Amount").val(amount.format(0, 3));
        $("#AmountInWords").val(amount.ReadNumber());
    }
    
    var rowid = <%=index%>;    
    function addNewRow(){
        var check = false;
        //Để người dùng nhập hết các ô sản phẩm thì mới add thêm dòng mới
        $("table.products tbody .name").each(function (id, item) {
            if (id < rowid -1 ) {
                if ($(this).val() == "") {
                    check = true;
                    return
                }
            }
        });
        if (check) {
            return false;
        }  
        
        $('.products tbody tr').prop('onclick', null).off('click');
        $(".products tbody").append('<tr class="prodata" >'
                   +' <td class="delr" style="text-align: center"><i class="fa fa-trash"></i></td>'
                   +'<td class="center">'+rowid+'</td>'                   
                   +' <td><input type="text" class="name" maxlength="200" onfocus="addNewRow()"/></td>'
                    +'<td><input type="text" class="unit" maxlength="50" /></td>'
                    +'<td><input type="text" class="quantity textr _number" maxlength="18" /></td>'
                    +'<td><input type="text" class="price textr _number" maxlength="18" /></td>'
                    +'<td><input type="text" class="total textr _number" maxlength="18" /></td>'
                    +'<td class="vatselect"></td>'
                    +'<td><input type="text" class="vatamount textr _number" maxlength="18" /></td>'
                    +'<td><input type="text" class="amount textr _number" maxlength="18" /></td>'
                    +'<td class="center"><input type="checkbox" class="issum" /></td></tr>');
        //    return false;
        rowid++;
        bindEvents2Table();
    }
    $(document).ready(function () {
        
        $(".products tbody tr:last").find(".name").attr("onfocus", "addNewRow()");
        $("input[type=text]:first").focus();
        $("#CusAddress").keyup(function () {
            checkTextMaxLength(this)
        });        
        cutbyMaxlength('ComAddress', 290);
        $("#CusTaxCode").ForceNumericOnly();
        $("input._number").each(function (i, item) {
            var _val = $(this).val();
            if (_val) {
                if (_val.indexOf(".") > -1) {
                    $(this).val(parseFloat(_val).format(2, 3))
                } else {
                    $(this).val(parseInt(_val).format(0, 3))
                }
            }
        }).ForceNumericOnly().focus(function () {
            var _val = $(this).val();
            $(this).val(_val.replaceAll(",", ""))
        }).focusout(function () {
            var _val = $(this).val();
            if (_val) {
                //var _fr = _val.indexOf(".") > -1 ? parseFloat(_val).format(2, 3) : parseInt(_val).format(0, 3);
                //$(this).val(_fr || "").trigger("change")
            }

        });
        $("#CusTaxCode").change(function () {
            var _this = $(this);
            var dt = _this.val();
            if (dt.indexOf("-") > 0)
                dt = dt.replace("-", "");
            $("#errorTaxCode").remove();
            if (_this.val()) {
                $.ajax({
                    data: {
                        mst: dt
                    },
                    success: function (data) {
                        if (!data) {
                            _this.addClass("error").parent(".control").append("<label id='errorTaxCode' for='CusTaxCode' generated='true' class='error'>Mã số t" +
                                "huế không hợp lệ!</label>")
                        } else {
                            _this.removeClass("error")
                        }
                    },
                    type: "POST",
                    url: "/Company/checkMST/"
                })
            } else {
                _this.removeClass("error")
            }
        });
        $("#CusName").autocomplete({
            focus: function (event, ui) { },
            minLength: 1,
            select: function (event, ui) {
                $("#CusTaxCode").val(ui.item.TaxCode);
                $("#CusAddress").val(ui.item.Address);
                $("#CusPhone").val(ui.item.Phone);
                $("#CusCode").val(ui.item.Code);
                $("#CusBankNo").val(ui.item.CusBankNo);
                $("#CusBankName").val(ui.item.CusBankName)
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
                                Code: item.Code,
                                CusBankNo: item.CusBankNo,
                                CusBankName: item.CusBankName
                            }
                        }))
                    },
                    type: "POST",
                    url: "/Customer/SeachByName"
                })
            }
        }).change(function () {
            if (!$(this).val()) {
                $("#CusTaxCode").val("");
                $("#CusAddress").val("");
                $("#CusPhone").val("");
                $("#CusCode").val("");
                $("#CusBankNo").val("");
                $("#CusBankName").val("")
            }
        });
        $("#CusTaxCode").autocomplete({
            focus: function (event, ui) { },
            minLength: 1,
            select: function (event, ui) {

                $("#CusName").val(ui.item.Name);
                $("#CusAddress").val(ui.item.Address);
                $("#CusPhone").val(ui.item.Phone);
                $("#CusCode").val(ui.item.Code);
                $("#CusBankNo").val(ui.item.CusBankNo);
                $("#CusBankName").val(ui.item.CusBankName);
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
                                value: item.TaxCode,
                                Name: item.cusname,
                                Address: item.Address,
                                Phone: item.Phone,
                                Code: item.Code,
                                CusBankNo: item.CusBankNo,
                                CusBankName: item.CusBankName
                            }
                        }))
                    },
                    type: "POST",
                    url: "/Customer/SeachByCusTaxCode"
                })
            }
        }).change(function () {
            if (!$(this).val()) {
                $("#CusName").val("");
                $("#CusAddress").val("");
                $("#CusPhone").val("");
                $("#CusCode").val("");
                $("#CusBankNo").val("");
                $("#CusBankName").val("");
            }
        });
        //HUY thêm

        $("#CusCode").autocomplete({
            focus: function (event, ui) { },
            minLength: 1,
            select: function (event, ui) {

                $("#CusName").val(ui.item.Name);
                $("#CusAddress").val(ui.item.Address);
                $("#CusPhone").val(ui.item.Phone);
                $("#CusTaxCode").val(ui.item.TaxCode);
                $("#CusBankNo").val(ui.item.CusBankNo);
                $("#CusBankName").val(ui.item.CusBankName);
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
                                value: item.Code,
                                Name: item.cusname,
                                Address: item.Address,
                                Phone: item.Phone,
                                Code: item.Code,
                                CusBankNo: item.CusBankNo,
                                TaxCode: item.TaxCode,
                                CusBankName: item.CusBankName
                            }
                        }))
                    },
                    type: "POST",
                    url: "/Customer/SeachByCusCode"
                })
            }
        }).change(function () {
            if (!$(this).val()) {
                $("#CusName").val("");
                $("#CusAddress").val("");
                $("#CusPhone").val("");
                $("#CusTaxCode").val("");
                $("#CusBankNo").val("");
                $("#CusBankName").val("");
            }
        });

        
        bindEvents2Table();
    });
    function submitForm(){        
        var _valid = $('form:first').validate();
        if (!_valid) {            
            return false;
        }                   
            
        var products = wrapProductData();
        if(products.length === 0 || products === undefined){
            sweetAlert("Lỗi!", 'Dữ liệu hóa đơn không hợp lệ, xin vui lòng thử lại!', "error");
            e.preventDefault();
            return false;
        } 
        if ($('#errorTaxCode').length > 0) {
            $('#errorTaxCode').show();
            $('#CusTaxCode').addClass('error');

            swal({
                title: "Cảnh báo!",
                text: "Mã số thuế của khách hàng không đúng.\n- Bạn có muốn tiếp tục lưu hóa đơn không?",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Tiếp tục",
                cancelButtonText: "Quay lại",
                closeOnConfirm: true,
                closeOnCancel: true
            }, function (isConfirm) {
                if (isConfirm) { 
                    //if($('form')[0].checkValidity()){
                    //    sweetAlert("Lỗi!", 'Dữ liệu hóa đơn không hợp lệ, xin vui lòng kiểm tra lại!', "error");

                    //} else {
                    $('#PubDatasource').val(JSON.stringify(products));
                    $('form').submit();
                    //}
                }
                else {
                    e.preventDefault();
                    return false;
                }
            });
            e.preventDefault();
            return false;
        } else {
            $('#PubDatasource').val(JSON.stringify(products));                
        }        

    }
    function bindEventForRate() {
        $(".vatrate").change(function () {            
            var _vatrate = parseInt($(this).val()) || 0;
            var quantity = $(this).parents("tr").find("input.quantity");
            var _quantity = parseFloat(quantity.val().replaceAll(",", "") || 1) || 1; 
            var price = $(this).parents("tr").find("input.price");
            var _price = parseFloat(price.val().replaceAll(",", "") || 0) || 0; 
            var total = $(this).parents("tr").find("input.total");
            var _total = parseFloat(total.val().replaceAll(",", "") || 0) || 0; 
            var vatamount = $(this).parents("tr").find("input.vatamount");
            var _vatamount = parseFloat(vatamount.val().replaceAll(",", "") || 0) || 0;
            var amount = $(this).parents("tr").find("input.amount ");
            var _amount = parseFloat(amount.val().replaceAll(",", "") || 0) || 0;
            if (_price > 0 && _quantity > 0) {    
                _total = _price * _quantity;
                total.val(_total.format(0, 3));
                //Tổng tiền của sản phẩm (chưa - thuế)                                               
                _vatamount = 0;
                if (_vatrate > 0) {
                    _vatamount = parseFloat((_total * _vatrate)/ 100 );
                }
                vatamount.val(_vatamount.format(0, 3));                
                // Tính tổng tiền của sản phẩm sau thuế 
                _amount = _total + _vatamount;                
                amount.val(_amount.format(0, 3));
                
            } else {                   
                if(_vatamount > 0 && _total === 0){
                    vatamount.val(_vatamount.format(0, 3));
                    amount.val(_vatamount.format(0, 3));
                }                                     
            }
            countTotalPrice();
        });
    }
    function bindEvents2Table() {
        $("table.products tbody input").unbind().removeData();
        $("table.products tbody input._number").each(function (i, item) {
            var _val = $(this).val();
            if (_val) {
                _val = _val.replaceAll(",", "");
                if (_val.indexOf(".") > -1) {
                    $(this).val(parseFloat(_val).format(2, 3))
                } else {
                    $(this).val(parseInt(_val).format(0, 3))
                }
            }
        }).ForceNumericOnly().focus(function () {
            var _val = $(this).val();
            $(this).val(_val.replaceAll(",", ""))
        }).focusout(function () {
            var _val = $(this).val();
            if (_val) {
                var _fr = _val.indexOf(".") > -1 ? parseFloat(_val).format(2, 3) : parseInt(_val).format(0, 3);
                $(this).val(_fr || "").trigger("change")
            }
        });
        $("table.products tbody tr").each(function (i, row) {
            var _del = $(row).find("td:first .fa-trash");
            $(_del).click(function () {
                if ($("table.products tbody tr").length > 2) {
                    swal({
                        title: "",
                        text: "Bạn có chắc chắn muốn xóa dòng sản phẩm này?",
                        type: "warning",
                        showCancelButton: true,
                        confirmButtonColor: "#DD6B55",
                        confirmButtonText: "Xóa",
                        cancelButtonText: "Hủy",
                        closeOnConfirm: true
                    }, 
                    function () {
                        rowid--;
                        $(row).remove();
                        $("table.products tbody tr").each(function (ii, rr) {
                            $(rr).children("td:nth-child(2)").text(ii + 1)
                        });
                        $("table.products tbody input.amount:first").trigger("change")
                    });
                }
            })
        });
        $("table.products tbody .name").change(function () {
            $(this).parents("tr:first").find("input.amount").trigger("change")
        });
        $("table.products tbody .quantity").change(function () {
            $(this).parents("tr:first").find(".amount").trigger("change")
        });
        $("table.products tbody .price").change(function () {
            $(this).parents("tr:first").find(".vatamount").trigger("change")
        });
        $("table.products tbody .total").change(function () {
            $(this).parents("tr:first").find(".amount").trigger("change")
        });
        $("table.products tbody input[type=checkbox]").change(function () {
            $(this).parents("tr:first").find(".vatamount").trigger("change")
        });
        
        bindEventForRate();
        
        $("table.products tbody .amount").change(function () {
            var _pName = $(this).parents("tr:first").find("input.name");
            if(!_pName.val()){
                $(this).val(0);
                $(this).parents("tr:first").find("input.amount").val(0);
                return;
            }
            var _vatamountVal = parseFloat(($(this).parents("tr:first").find("input.vatamount").val().replaceAll(",", "") ||0)|| 0);
            if(_vatamountVal == 0)
                $(this).parents("tr:first").find(".vatamount").trigger("change");            
            var _amountVal = parseFloat($(this).val().replaceAll(",", "") ||0)|| 0;                            
            if (_amountVal > 0) {                  
                $(this).parents("tr:first").find("input.total").val(_amountVal.format(0,3)); 
                $(this).parents("tr:first").find("input.vatamount").val(0);
            }            
                                    
            var _amountInv = 0;
            $("table.products tbody .amount").each(function (id, item) {
                var _val = $(item).val();
                var _isSum = $(item).parents("tr:first").find("input[type=checkbox]");
                if (_val && !_isSum.is(":checked")) {
                    _amountInv += parseFloat(_val.replaceAll(",", "") || 0) || 0;
                }
            });            
                       
            $('#Amount').val(_amountInv.format(0,3));             
            countTotalPrice();
        });

        $("table.products tbody .vatamount").change(function () {  
            var _pName = $(this).parents("tr:first").find("input.name");
            if(!_pName.val()){
                $(this).val(0);
                $(this).parents("tr:first").find("input.amount").val(0);
                return;
            }
            var _amountVal = parseFloat(($(this).parents("tr:first").find("input.amount").val().replaceAll(",", "") ||0)|| 0); 
            var _totalVal = parseFloat($(this).parents("tr:first").find("input.total").val().replaceAll(",", "") ||0) || 0;  
            var _vatamountVal = parseFloat($(this).val().replaceAll(",", "") ||0)|| 0; 
            if (_vatamountVal > 0) {  
                if(_totalVal > 0){                                                           
                    $(this).parents("tr:first").find(".vatrate").trigger("change");
                }    
                else{
                    $(this).parents("tr:first").find("input.amount").val(_vatamountVal.format(0,3));
                }
            }
            else{
                var _vatamount = Math.round(parseFloat(_amountVal - _totalVal));                                                          
                $(this).parents("tr:first").find("input.vatamount").val(_vatamount.format(0,3));
            }
            
            var _totalamount = 0;
            var _totalvatamount = 0;
            var _amountInv = 0;
            $("table.products tbody .amount").each(function (id, item) {
                var _val = $(item).val();
                var _isSum = $(item).parents("tr:first").find("input[type=checkbox]");
                if (_val && !_isSum.is(":checked")) {
                    _amountInv += parseFloat(_val.replaceAll(",", "") || 0) || 0;
                }
            });
            $("table.products tbody .vatamount").each(function (id, item) {
                var _val = $(item).val();
                var _isSum = $(item).parents("tr:first").find("input[type=checkbox]");
                if (_val && !_isSum.is(":checked")) {
                    _totalvatamount += parseFloat(_val.replaceAll(",", "") || 0) || 0;
                }
            });
                       
            $('#Amount').val(_amountInv.format(0,3));             
            countTotalPrice();
        });

        
        $("table.products tbody input.name").data("autocomplete_on", false).bind("autocompleteopen", function (event, ui) {
            $(this).data("autocomplete_on", true)
        }).bind("autocompleteclose", function (event, ui) {
            $(this).data("autocomplete_on", false)
        }).autocomplete({
            minLength: 1,
            select: function (event, ui) {
                $(this).parents("tr:first").find("input.code").val(ui.item.Code);
                $(this).parents("tr:first").find("input.unit").val(ui.item.Unit);
                $(this).parents("tr:first").find("input.price").val(ui.item.Price).trigger("focusout");
                if (event.which != 9 || event.keyCode != 9) {
                    $(this).parents("tr:first").find("input.quantity").focus()
                } else {
                    $(this).parents("tr:first").find("input.unit").focus()
                }
                $(this).val(ui.item.Name).trigger("change");
                return false
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
                                label: item.Name,
                                Name: item.Name,
                                Unit: item.Unit,
                                Price: item.Price,
                                Code: item.Code
                            }
                        }))
                    },
                    type: "POST",
                    url: "/Product/SeachByName"
                })
            }
        });
        $("table.products tbody input").bind("keydown.nav", function (e) {
            var _post = 0;
            if (!$(this).is(":checkbox")) {
                _post = this.selectionStart
            }
            var _start = 0,
                _end = $(this).val().length;
            var _cindex = $(this).parents("td:first").index();
            var _rindex = $(this).parents("tr:first").index();
            if (e.which == 13 || e.keyCode == 13) {
                return false
            }
            if (!$(this).data("autocomplete_on")) {
                switch (e.which || e.keyCode) {
                    case 37:
                        if ($(this).is(":checkbox") || !$(this).val() || (!$(this).is(":checkbox") && _post <= _start)) {
                            var _leftrow = $("table.products tbody").children("tr:nth-child(" + (_rindex + 1) + ")");
                            var _leftcol = $(_leftrow).children("td:nth-child(" + (_cindex) + ")");
                            var _cellfocus = _leftcol.children("input:first").focus();
                            if (_cellfocus.length > 0) {
                                $(this).parents("tr:first").find(".amount").trigger("change")
                                //$(this).trigger("change").trigger("blur")
                            }
                            return false
                        }
                        break;
                    case 38:
                        var _uprow = $("table.products tbody").children("tr:nth-child(" + (_rindex) + ")");
                        var _upcol = $(_uprow).children("td:nth-child(" + (_cindex + 1) + ")");
                        var _cellfocus = _upcol.children("input:first").focus();
                        if (_cellfocus.length > 0) {
                            $(this).parents("tr:first").find(".vatamount").trigger("change")
                            //$(this).trigger("change").trigger("blur")
                        }
                        return false;
                        break;
                    case 39:
                        if ($(this).is(":checkbox") || !$(this).val() || (!$(this).is(":checkbox") && _post >= _end)) {
                            var _rightrow = $("table.products tbody").children("tr:nth-child(" + (_rindex + 1) + ")");
                            var _rightcol = $(_rightrow).children("td:nth-child(" + (_cindex + 2) + ")");
                            var _cellfocus = _rightcol.children("input:first").focus();
                            if (_cellfocus.length > 0) {
                                $(this).parents("tr:first").find(".vatamount").trigger("change")
                                //$(this).trigger("change").trigger("blur")
                            }
                            return false
                        }
                        break;
                    case 40:
                        var _downrow = $("table.products tbody").children("tr:nth-child(" + (_rindex + 2) + ")");
                        var _downcol = $(_downrow).children("td:nth-child(" + (_cindex + 1) + ")");
                        var _cellfocus = _downcol.children("input:first").focus();
                        if (_cellfocus.length > 0) {
                            $(this).parents("tr:first").find(".vatamount").trigger("change")
                            //$(this).trigger("change").trigger("blur")
                        }
                        return false;
                        break
                }
            }
        });      

        $(".name").change(function () {
            var vatselect = $(this).parents('tr').find('.vatselect');
            if ($(this).val().length) {
                if (vatselect.html().length <= 0) {
                    vatselect.html('<select class="required vatrate"><option value="0">0%</option><option value="5">5%</option><option value="10">10%</option><option value="-1"> Không thuế GTGT</option></select>');
                    bindEventForRate();
                }
            } else {
                vatselect.html('');
            }
        });
    };
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
