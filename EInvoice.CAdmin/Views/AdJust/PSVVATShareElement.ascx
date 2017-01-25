<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PSVVATInvoice>" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="EInvoice.PSVExtends.Domain" %>
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
        <div class="label w120 imp"><%: Html.LabelFor(m => m.Buyer, labelText: "Họ tên người mua hàng") %></div>
        <div class="control"><%: Html.TextBox("Buyer", Model.Buyer, new {  @required = "required", maxlength="200"})%></div>
    </div>

    <div class="line w120p">
        <div class="line">
            <div class="label"><%: Html.LabelFor(m => m.CusName, labelText: Resources.Einvoice.MInv_LblCusName + "(Customer’s name)") %></div>
            <div class="control"><%: Html.TextBoxFor(m => m.CusName, new { maxlength="200" })%></div>
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
    <div class="line">
        <div class="label w120">
            <%: Html.LabelFor(m => m.ShipAddress, labelText: "Địa chỉ giao hàng") %>
        </div>
        <div class="control">
            <%: Html.TextBox("ShipAddress", Model.ShipAddress, new { maxlength = "290" })%>
        </div>
    </div>

    <div class="left w50p">
        <div class="line">
            <div class="label w120">
                <%: Html.LabelFor(m => m.CusTaxCode, labelText: Resources.Einvoice.MInv_LblCusTaxCode) %>
            </div>
            <div class="control">
                <%= Html.TextBox("CusTaxCode", Model.CusTaxCode, new { @maxlength = 14})%>
            </div>
        </div>
    </div>
    <div class="right w50p">
        <div class="line">
            <div class="label w80">
                <%: Html.LabelFor(m => m.CusCode,  labelText: "Mã khách hàng") %>
            </div>
            <div class="control">
                <%: Html.TextBoxFor(m => m.CusCode, new { @class="textandnum", maxlength="100"})%>
            </div>
        </div>
    </div>

    <div class="left w50p">
        <div class="line">
            <div class="label w120">
                <%: Html.LabelFor(m => m.CusBankNo, labelText: "Số tài khoản") %>
            </div>
            <div class="control">
                <%= Html.TextBox("CusBankNo", Model.CusBankNo, new { @maxlength = 30, @class = "number" }) %>
            </div>
        </div>
    </div>
    <div class="right w50p">
        <div class="line">
            <div class="label w80">
                <%: Html.LabelFor(m => m.CusBankName,  labelText: "Tại") %>
            </div>
            <div class="control">
                <%= Html.TextBoxFor(m => m.CusBankName, new { maxlength="50"})%>
            </div>
        </div>
    </div>

    <div class="line">
        <div class="label w120 imp">
            <%: Html.LabelFor(m => m.PaymentMethod, labelText: Resources.Einvoice.MInv_LblPayMethod) %>
        </div>
        <div class="control">
            <%: Html.DropDownListFor(m => m.PaymentMethod, new[]
                        {
                            new SelectListItem{Text=Resources.Einvoice.Minv_txtCashingPay, Value="TM", Selected= (Model.PaymentMethod == "TM")},
                            new SelectListItem{Text=Resources.Einvoice.MInv_txtTransferPay, Value="CK",Selected= (Model.PaymentMethod == "CK")},
                            new SelectListItem{Text=Resources.Einvoice.Minv_txtCreditCardPay, Value="TTD",Selected= (Model.PaymentMethod == "TTD")},
                            new SelectListItem{Text="Hình thức HDDT", Value="HDDT",Selected= (Model.PaymentMethod == "HDDT")},
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
                    <th style="width: 100px;">Mã hàng </th>
                    <th class="np">Tên hàng hóa, dịch vụ</th>
                    <th style="width: 90px">ĐVT (Unit)</th>
                    <th style="width: 70px">Số lượng (Quantity)</th>
                    <th style="width: 90px">Đơn giá (Price)</th>
                    <th style="width: 100px">Thành tiền (Amount) VNĐ</th>
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
                <tr>
                    <td class="delr" style="text-align: center"><i class="fa fa-trash"></i></td>
                    <td class="center"><%: index %></td>
                    <td>
                        <input type="text" value="<%= Html.Encode(product.Code) %>" class="code" maxlength="50" />
                    </td>
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
                        <input type="text" value="<%if (product.Price == 0)
                                                    {%><%}
                                                    else
                                                    { %><%= Html.Encode(product.Price) %><%} %>"
                            class="price textr _number" maxlength="18" />
                    </td>
                    <td>
                        <input type="text" value="<%= Html.Encode(product.Amount) %>"
                            class="amount textr _number" maxlength="18" />
                    </td>
                    <td class="center">
                        <input type="checkbox" value="false" class="issum" />
                    </td>
                </tr>
                <%
                                                    index++;
                        }
                    }
                    else
                    {
                        index = 0;
                %>
                <tr>
                    <td class="delr" style="text-align: center"><i class="fa fa-trash"></i></td>
                    <td class="center"><%: index %></td>
                    <td>
                        <input type="text" class="code" maxlength="50" />
                    </td>
                    <td>
                        <input type="text" class="name" maxlength="200" />
                    </td>
                    <td>
                        <input type="text" readonly="readonly" disabled class="unit" maxlength="50" />
                    </td>
                    <td>
                        <input type="text" readonly="readonly" disabled class="quantity textr _number onlynum" maxlength="18" />
                    </td>
                    <td>
                        <input type="text" readonly="readonly" disabled class="price textr _number" maxlength="18" />
                    </td>
                    <td>
                        <input type="text" readonly="readonly" disabled class="amount textr _number" maxlength="18" />
                    </td>
                    <td class="center">
                        <input type="checkbox" value="false" class="issum" />
                    </td>
                </tr>
                <%} %>
            </tbody>
        </table>
    </div>
    <div class="left w60p pdt100">
        <div class="line null"></div>
        <div class="line">
            <div class="label push-60">
                <%: Html.LabelFor(m => m.VATRate, labelText: Resources.Einvoice.MInv_LblVATRate) %>
            </div>
            <div class="control">
                <%: Html.DropDownList("VATRate",new[]
                        {
                            new SelectListItem{Text="0% ", Value="0", Selected= (Model.GrossValue0>0)},
                            new SelectListItem{Text="5%", Value="5",Selected= (Model.GrossValue5>0)},
                            new SelectListItem{Text="10%", Value="10",Selected= (Model.GrossValue10>0)},
                            new SelectListItem{Text=Resources.Einvoice.MInv_TxtNotVATRate, Value="-1",Selected= (Model.GrossValue >0)}
                        }, new {@class = "required", title = "Cần chọn!"})%>
            </div>
        </div>
        <div class="line null"></div>
    </div>
    <div class="right">
        <div class="line">
            <div class="label w180">
                <%: Html.LabelFor(m => m.Otherfees, labelText: "Phụ phí khác") %>
            </div>
            <div class="control">
                <%: Html.TextBox("Otherfees", Model.Otherfees, new {@class="textr _number", @tabindex = -1 }) %>
            </div>
        </div>

        <div class="line">
            <div class="label w180">
                <%: Html.LabelFor(m => m.Total, labelText: Resources.Einvoice.MInv_lblTotalService) %>
            </div>
            <div class="control">
                <%: Html.TextBoxFor(m => m.Total, new { @readonly = "readonly", @class="textr _number", @tabindex = -1 }) %>
            </div>
        </div>
        <div class="line">
            <div class="label w180">
                <%: Html.LabelFor(m => m.VATAmount, labelText: Resources.Einvoice.MInv_LblVATAmount) %>
            </div>
            <div class="control">
                <%: Html.TextBoxFor(m => m.VATAmount, new { @readonly = "readonly", @class="textr _number", @tabindex = -1  })%>
            </div>
        </div>
        <div class="line">
            <div class="label w180">
                <%: Html.LabelFor(m => m.Amount, labelText: Resources.Einvoice.MInv_Amount) %>
            </div>
            <div class="control">
                <%: Html.TextBoxFor(m => m.Amount, new { @class="textr _number", @tabindex = -1  }) %>
            </div>
        </div>
    </div>

    <div class="line">
        <div class="label w120">
            <%: Html.LabelFor(m => m.AmountInWords, Resources.Einvoice.MInv_LblAmountInWords) %>
        </div>
        <div class="control">
            <%--@readonly = "readonly"--%>
            <%: Html.TextBoxFor(m => m.AmountInWords, new {@maxlength="200" })%>
        </div>
    </div>
</div>
<script type="text/javascript">
    var rowid = <%=index%>;
    if (rowid < 10) {
        rowid = 10;
    }
    function addNewRow(){
        var check = false;
        //Để người dùng nhập hết các ô sản phẩm thì mới add thêm dòng mới
        $("table.products tbody .name").each(function (id, item) {
            if (id < rowid -1 ) {
                console.log($(this).val());
                if ($(this).val() == "") {
                    check = true;
                    return
                }
            }
        });
        if (check) {
            return false;
        }
        rowid++;
        $('.products tbody tr').prop('onclick', null).off('click');
        $(".products tbody").append(" <tr class='last' >"
                + '<td class="delr" style="text-align: center"><i class="fa fa-trash"></i></td>'
                + '<td class="center">'+rowid+'</td>'
                + '<td><input type="text" class="code" maxlength="50"/></td>'
                + '<td><input type="text" class="name" maxlength="200"  onfocus="addNewRow()"/></td>'
                + '<td>  <input type="text" class="unit" maxlength="50" /></td>'
                + '<td><input type="text" class="quantity textr _number" maxlength="18" /></td>'
                + '<td> <input type="text" class="price textr _number" maxlength="18" /></td>'
                + '<td> <input type="text" class="amount textr _number" maxlength="18" /> </td>'
                + '<td class="center">  <input type="checkbox" class="issum" /></td></tr>');
        //    return false;
        bindEvents2Table();

    }
    $(document).ready(function () {
       
        $(".products tbody tr:last").find(".name").attr("onfocus", "addNewRow()");
        $('input[type=text]:first').focus();
        cutbyMaxlength('ComAddress', 200);
        $('#CusAddress').keyup(function () {
            checkTextMaxLength(this);
        });

        $('#CusTaxCode').ForceNumericOnly();
        $('input._number').each(function (i, item) {
            var _val = $(this).val();
            if (_val) {
                if (_val.indexOf('.') > -1) {
                    $(this).val(parseFloat(_val).format(2, 3));
                } else {
                    $(this).val(parseInt(_val).format(0, 3));
                }
            }
        }).ForceNumericOnly()
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
            });

        /// File đính kèm
        $('#_txtFile').click(function () {
            $('#_fleAttackment').trigger('click');
        }).keypress(function (e) {
            if (e.keyCode == 13 || e.which == 13) {
                $(this).trigger('click');
                return false;
            }
        });

        /// Hiển thị file đính kèm
        $('#_fleAttackment').change(function () {
            var _file = $(this)[0].files[0];
            if (_file) {
                var _extl = ['doc', 'docx', 'pdf'];
                var _ext = _file.name.substring(_file.name.lastIndexOf('.') + 1);
                if (_extl.indexOf(_ext) > -1) {
                    $('#_txtFile').val(_file.name);
                }
                else {
                    $(this).val('');
                    sweetAlert("Lỗi!", 'Định dạng file đính kèm không được chấp nhận.', "error");
                }
            } else {
                $('#_txtFile').val('');
            }
        });

        /// Check Mã số thuế
        $('#CusTaxCode').change(function () {
            var _this = $(this);
            var dt = _this.val();
            if (dt.indexOf("-") > 0)
                dt = dt.replace("-", "");
            
            $('#errorTaxCode').remove();
            if (_this.val()) {
                $.ajax({
                    type: "POST",
                    url: "/Company/checkMST/",
                    data: {
                        mst: dt
                    },
                    success: function (data) {
                        if (!data) {
                            _this.addClass('error').parent('.control').append('<label id="errorTaxCode" for="CusTaxCode" generated="true" class="error">Mã số thuế không hợp lệ!</label>');
                        }
                        else {
                            _this.removeClass('error');
                        }
                    }
                });
            }
            else {
                _this.removeClass('error');
            }
        });
        /// Sự kiện tải thông tin khách hàng
        $('#CusName').autocomplete({
            source: function (request, response) {
                $.ajax(
                {
                    type: "POST",
                    url: '/Customer/SeachByName',
                    dataType: "JSON",
                    data: {
                        searchText: request.term
                    },
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
                        }));
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },
            focus: function (event, ui) {
            },
            minLength: 1,
            select: function (event, ui) {
                $('#CusTaxCode').val(ui.item.TaxCode);
                $('#CusAddress').val(ui.item.Address);
                $('#CusPhone').val(ui.item.Phone);
                $('#CusCode').val(ui.item.Code);
            }
        }).change(function () {
            if (!$(this).val()) {
                $('#CusTaxCode').val('');
                $('#CusAddress').val('');
                $('#CusPhone').val('');
                $('#CusCode').val('');
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
        /// Thay đổi % giá trị thuế
        $('#VATRate').change(function () {            
            var _vatrate = parseInt($(this).val());
            var Otherfees = parseFloat($("#Otherfees").val().replaceAll(",", "") || 0) || 0;            
          
            if ($('#Total').val()) {
                var _totalamount = parseFloat($('#Total').val().replaceAll(',', '') || 0);
                var _taxableamount = 0, _totalableamount = 0;
                $('table.products tbody tr').each(function (i, row) {
                    if (!$(row).find('.issum:first').is(':checked')) {
                        _taxableamount += parseFloat(($(row).find('.amount:first').val() || "0").replaceAll(',', '') || 0);                        
                    }
                });

                var _vatamount = parseFloat(_taxableamount / 100 * _vatrate);
                var _amount = 0;

                if (_vatrate > 0) {
                    $('#VATAmount').val(_vatamount.format(0, 3));
                    _amount = parseFloat(_totalamount + _vatamount + Otherfees);

                }
                else {
                    $('#VATAmount').val('0');
                    _amount = _totalamount + Otherfees;
                }
                if(_amount < 0){
                    _amount = 0;
                }
                /// Tổng tiền bao gồm thuế
                $('#Amount').val(_amount).attr('value', _amount.format(0, 3));
                /// Tổng tiền bao gồm thuế bằng chữ
                $('#AmountInWords').val(_amount.ReadNumber());
            }
            else {
                $('#Amount').val(0).attr('value', 0);
                $('#AmountInWords').val('');
            }
        });
        ///Thay đổi tổng cộng tiền thanh toán
        $('#Amount').change(function () {
            var _amountVal = parseFloat($(this).val().replaceAll(',', '') || 0);
            $('#AmountInWords').val(_amountVal.ReadNumber());
        });
        /// Thay đổi số tiền
        $('#Total').change(function () {
            $('#VATAmount').trigger('change');
            //ABC
        });
        $("#Otherfees").change(function () {
            $("#VATRate").trigger("change");
      
        });         
        $("#Otherfees").focus(function () {   
            if($(this).val() == 0 )
                $(this).val(0);
        });
        /// Thay đổi tiền thuế
        $('#VATAmount').change(function () {
            var _totalamount = parseFloat($('#Total').val().replaceAll(',', '') || 0);
            var _vatamount = parseFloat($('#VATAmount').val().replaceAll(',', '') || 0);
            $('#Amount').val(_totalamount + _vatamount).trigger('focusout');
            $('#AmountInWords').val((_totalamount + _vatamount).ReadNumber());
        });

        bindEvents2Table();

        $('button[type=submit]').click(function () {
            var check = 0;
            $('#VatAmount0').val('0');
            $('#VatAmount5').val('0');
            $('#VatAmount10').val('0');
            $('#GrossValue').val('0');
            $('#GrossValue0').val('0');
            $('#GrossValue5').val('0');
            $('#GrossValue10').val('0');
            var _totalamount = parseFloat($('#Total').val().replaceAll(',', '') || 0);
            var _vatamount = parseFloat($('#VATAmount').val().replaceAll(',', '') || 0);
            var _vatrate = parseInt($('#VATRate').val());
            if (_vatrate > 0) {
                if (_vatrate == 10) {
                    $("#VatAmount10").val(_vatamount);
                    $('#GrossValue10').val(_totalamount);
                }
                if (_vatrate == 5) {
                    $("#VatAmount5").val(_vatamount);
                    $('#GrossValue5').val(_totalamount);
                }
            }
            else {
                if (_vatrate == -1) {
                    $('#GrossValue').val(_totalamount);
                }
                if (_vatrate == 0) {
                    $("#VatAmount0").val('0');
                    $('#GrossValue0').val(_totalamount);
                }
            }
            /// Validate form
            var _valid = $(this).parents('form:first').valid();
            if (!_valid) {
                return false;
            }

            /// Nếu mã số thuế không đúng, không cho submitform
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
                    closeOnConfirm: false,
                    closeOnCancel: false
                }, function (isConfirm) {
                    if (isConfirm) { }
                    else { return false; }
                });
            }

            if (!$('#Total').val() || $('#Total').val() == '0') {
                sweetAlert("Lỗi!", "Hóa đơn có giá trị âm [Tổng tiền hóa đơn], chưa thể phát sinh giao dịch.", "error");

                return false;
            }

            /// Chuyển định dạng số về dạng hệ thống chấp nhận
            $('input[type=text]._number').each(function (i, item) {
                var _val = $(this).val();
                $(this).val(_val.replaceAll(',', '').replaceAll('.', ','));
            });

            var _products = [];

            /// Lấy danh sách sản phẩm
            $('table.products tbody tr').each(function (i, row) {
                var _product = {};

                var _pName = $(row).find('input[type=text].name');
                var _pUnit = $(row).find('input[type=text].unit');
                var _pQuantity = $(row).find('input[type=text].quantity');
                var _pPrice = $(row).find('input[type=text].price');
                var _pAmount = $(row).find('input[type=text].amount');
                var _pIsSum = $(row).find('input[type=checkbox]:first');
                if (_pAmount.val() || _pName.val()) {
                    _product.ProdType = 1;
                    _product.Name = _pName.val() || '';
                    _product.Unit = _pUnit.val() || '';
                    _product.Quantity = _pQuantity.val() || 0;
                    _product.Price = _pPrice.val() || 0;
                    _product.Amount = _pAmount.val() || 0;
                    _product.IsSum = _pIsSum.is(':checked');
                    if (_product.Amount < 0) {
                        _product.Amount = _product.Amount * (-1);
                    }
                    if (_product.Amount != "" && _product.Name == "") {
                        check = 0;
                        _products.length = 0;
                        return false;
                    }
                    if ((_product.Amount.toString().length > 18 || _product.Amount.toString().indexOf('e') >= 0) || (_product.Price > 0 && (_product.Price.toString().length > 18 || _product.Price.toString().indexOf('e') >= 0)) || (_product.Quantity > 0 && (_product.Quantity.toString().length > 18 || _product.Quantity.toString().indexOf('e') >= 0))) {
                        check = 1;
                        _products.length = 0;
                        return false;
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
                return;
            }

            if (!$('#Total').val() || $('#Total').val() == '0') {

                swal({
                    title: "Cảnh báo!",
                    text: "Xin lỗi, hóa đơn chưa có giá trị [Tổng cộng tiền thanh toán].\n- Bạn có muốn tiếp tục lưu hóa đơn không?",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Tiếp tục",
                    cancelButtonText: "Quay lại",
                    closeOnConfirm: false,
                    closeOnCancel: false
                }, function (isConfirm) {
                    if (isConfirm) { }
                    else {
                        return false;
                    }
                });
            }

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
                    if (isConfirm) { }
                    else { return false; }
                });
            }
            //Kiểm tra các giá trị âm
            if ($('#CusName').val() && $('#PaymentMethod').val()) {
                if ($('#VATAmount').val().toString().indexOf('-') >= 0) {
                    var _vatAmount = $('#VATAmount').val() * (-1);
                    $('#VATAmount').val(_vatAmount)
                }
                if ($('#Total').val().toString().indexOf('-') >= 0) {
                    var _total = $('#Total').val() * (-1);
                    $('#Total').val(_total)
                }
                if ($('#Amount').val().toString().indexOf('-') >= 0) {
                    var _amount = $('#Amount').val() * (-1);
                    $('#Amount').val(_amount)
                }
            }
            $("input[type='text']").each(function (i) {
                checkTextMaxLength(this);
            });
            $('#PubDatasource').val(JSON.stringify(_products));
        });
    });

    /// Tải lại toàn bộ các sự kiện cho table
    function bindEvents2Table() {
        $('table.products tbody input')
            .unbind() // gỡ các sự kiện
            .removeData(); // gỡ toàn bộ dữ liệu
        $('table.products tbody input._number')
            .each(function (i, item) {
                var _val = $(this).val();
                if (_val) {
                    _val = _val.replaceAll(',', '');
                    if (_val.indexOf('.') > -1) {
                        $(this).val(parseFloat(_val).format(2, 3));
                    } else {
                        $(this).val(parseInt(_val).format(0, 3));
                    }
                }
            }).ForceNumericOnly()
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
            });

        /// Nạp sự kiện xóa dòng cho các dòng
        $('table.products tbody tr').each(function (i, row) {
            var _del = $(row).children('td:first');
            $(_del).unbind().click(function () {
                if ($('table.products tbody tr').length > 2) {
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
        /// Sự kiện thay đổi Tên
        $('table.products tbody .name').change(function () {
            $(this).parents('tr:first').find('input.amount').trigger('change');
        });
        /// Sự kiện thay đổi số lượng
        $('table.products tbody .quantity').change(function () {
            $(this).parents('tr:first').find('input.amount').trigger('change');
        });

        /// Sự kiện thay đổi giá tiền
        $('table.products tbody .price').change(function () {
            $(this).parents('tr:first').find('input.amount').trigger('change');
        });

        /// Không tính tiền
        $('table.products tbody input[type=checkbox].issum').change(function () {
            if ($(this).is(':checked')) {
                $(this).parents('tr:first').find('input[type=checkbox].tax').removeAttr('checked').attr('disabled', 'disabled');
            } else {
                $(this).parents('tr:first').find('input[type=checkbox].tax').removeAttr('checked').removeAttr('disabled', 'disabled');
            }
            $(this).parents('tr:first').find('input.amount').trigger('change');
        });       

        /// Sự kiện thay đổi thành tiền
        $('table.products tbody .amount').change(function () {            
            //Nếu người dùng ko nhập thì mặc định số lượng là 1 
            // var _quantity = parseFloat($(this).parents("tr:first").find("input.quantity").val().replaceAll(",", "") || 0) || 0;
            var _price = parseInt($(this).parents("tr:first").find("input.price").val().replaceAll(",", "") || 0) || 0;
            var _quantityVal = $(this).parents("tr:first").find("input.quantity").val();
            if (_quantityVal != "") {
                _quantityVal = parseFloat($(this).parents("tr:first").find("input.quantity").val().replaceAll(",", "") || 0) || 0;
            } else {
                _quantityVal = 1;
            }
            var _priceVal = $(this).parents("tr:first").find("input.price").val();
            if ((_quantityVal * _price == 0) && (_priceVal != "")) {
                $(this).val(0)
            } else if (_quantityVal * _price > 0) {
                $(this).val(parseFloat(_quantityVal * _price).format(0, 3))
            }
            var _totalamount = 0;
            var _amountnotax = 0;
            $('table.products tbody .amount').each(function (id, item) {
                var _val = $(item).val();
                /// Kiểm tra sản phẩm có tính tiền hay không?
                var _isSum = $(item).parents("tr:first").find('input[type=checkbox].issum');
                if (_val && !_isSum.is(':checked')) {
                    _totalamount += parseInt(_val.replaceAll(',', '') || 0);
                }                
            });
            /// Tổng tiền chưa thuế
            $('#Total').val(_totalamount.format(0, 3));
            /// Bắt sự kiện thay đổi % giá trị thuế
            $('#VATRate').trigger('change');                        
        });

        /// Tự động điền sản phẩm
        $('table.products tbody input.name')
            .data('autocomplete_on', false)
            .bind('autocompleteopen', function (event, ui) {
                $(this).data('autocomplete_on', true);
            })
            .bind('autocompleteclose', function (event, ui) {
                $(this).data('autocomplete_on', false);
            })
            .autocomplete({
                source: function (request, response) {
                    $.ajax(
                    {
                        type: "POST",
                        url: '/Product/SeachByName',
                        dataType: "JSON",
                        data: {
                            searchText: request.term
                        },
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.Name,
                                    Name: item.Name,
                                    Unit: item.Unit,
                                    Price: item.Price
                                }
                            }));
                        }//,
                        //error: function (XMLHttpRequest, textStatus, errorThrown) {
                        //alert(textStatus);
                        //}
                    });
                },
                minLength: 1,
                select: function (event, ui) {
                    $(this).parents('tr:first').find('input.unit').val(ui.item.Unit);
                    $(this).parents('tr:first').find('input.price').val(ui.item.Price).trigger('focusout');
                    if (event.which != 9 || event.keyCode != 9) {
                        $(this).parents('tr:first').find('input.quantity').focus();
                    } else {
                        $(this).parents('tr:first').find('input.unit').focus();
                    }
                    $(this).val(ui.item.Name).trigger('change');
                    return false;
                }
            });

        /// Thêm điều hướng cho table
        $('table.products tbody input').bind('keydown.nav', function (e) {
            var _post = 0;
            if (!$(this).is(':checkbox')) {
                _post = this.selectionStart;
            }
            var _start = 0, _end = $(this).val().length;

            var _cindex = $(this).parents('td:first').index();
            var _rindex = $(this).parents('tr:first').index();


            if (e.which == 13 || e.keyCode == 13) {
                return false;
            }

            if (!$(this).data('autocomplete_on')) {
                switch (e.which || e.keyCode) {
                    case 37: // left
                        if ($(this).is(':checkbox') || !$(this).val() || (!$(this).is(':checkbox') && _post <= _start)) {
                            var _leftrow = $('table.products tbody').children('tr:nth-child(' + (_rindex + 1) + ')');
                            var _leftcol = $(_leftrow).children('td:nth-child(' + (_cindex) + ')');
                            var _cellfocus = _leftcol.children('input:first').focus();
                            if (_cellfocus.length > 0) {
                                $(this).parents("tr:first").find(".amount").trigger("change")
                                //$(this).trigger('change').trigger('blur');
                            }
                            return false;
                        }
                        break;
                    case 38: // up
                        var _uprow = $('table.products tbody').children('tr:nth-child(' + (_rindex) + ')');
                        var _upcol = $(_uprow).children('td:nth-child(' + (_cindex + 1) + ')');
                        var _cellfocus = _upcol.children('input:first').focus();
                        if (_cellfocus.length > 0) {
                            $(this).parents("tr:first").find(".amount").trigger("change")
                            //$(this).trigger('change').trigger('blur');
                        }
                        return false;
                        break;
                    case 39: // right
                        if ($(this).is(':checkbox') || !$(this).val() || (!$(this).is(':checkbox') && _post >= _end)) {
                            var _rightrow = $('table.products tbody').children('tr:nth-child(' + (_rindex + 1) + ')');
                            var _rightcol = $(_rightrow).children('td:nth-child(' + (_cindex + 2) + ')');
                            var _cellfocus = _rightcol.children('input:first').focus();
                            if (_cellfocus.length > 0) {
                                $(this).parents("tr:first").find(".amount").trigger("change")
                                //$(this).trigger('change').trigger('blur');
                            }
                            return false;
                        }
                        break;
                    case 40: // down
                        var _downrow = $('table.products tbody').children('tr:nth-child(' + (_rindex + 2) + ')');
                        var _downcol = $(_downrow).children('td:nth-child(' + (_cindex + 1) + ')');
                        var _cellfocus = _downcol.children('input:first').focus();
                        if (_cellfocus.length > 0) {
                            $(this).parents("tr:first").find(".amount").trigger("change")
                            //$(this).trigger('change').trigger('blur');
                        }
                        return false;
                        break;
                }
            }
        });

        // Nếu là dòng cuối cùng thì thêm dòng mới
        $('table.products tbody input:text').change(function () {
            var _this = $(this);
            var _rowsc = _this.parents('tbody').children('tr').length;
            if (_this.parents('tr:first').children('td:nth-child(2)').text() == _rowsc) {
                var _addable = false;
                _this.parents('tr:first').find('input:text').each(function (i, item) {
                    if ($(item).val()) {
                        _addable = true;
                    }
                });
                if (_addable) {
                    var _row = $('<tr><td class="delr" style="text-align: center"><i class="fa fa-trash"></i></td><td class="center">' + (_rowsc + 1) + '</td><td><input type="text" class="code" maxlength="50"/></td><td><input type="text" class="name" maxlength=200>' +
                                    '</td><td><input type="text" class="unit" maxlength=50></td><td><input type="text" class="quantity textr _number"></td><td>' +
                                    '<input type="text" class="price textr _number"></td><td><input type="text" class="amount textr _number">' +
                                    '</td><td class="center"><input type="checkbox"></td></tr>');
                    $('table.products tbody').append(_row);
                    /// Bind lại toàn bộ sự kiện
                    bindEvents2Table();
                }
            }
        });
    };
    //xác định vị trí con trỏ chuột
    new function ($) {
        $.fn.getCursorPosition = function () {
            var pos = 0;
            var el = $(this).get(0);
            // IE Support
            if (document.selection) {
                el.focus();
                var Sel = document.selection.createRange();
                var SelLength = document.selection.createRange().text.length;
                Sel.moveStart('character', -el.value.length);
                pos = Sel.text.length - SelLength;
            }
                // Firefox support
            else if (el.selectionStart || el.selectionStart == '0')
                pos = el.selectionStart;
            return pos;
        }
    }(jQuery);
</script>
