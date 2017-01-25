<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CustomerModel>" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<%Html.EnableClientValidation(); %>
<link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
<script type="text/javascript" src="/Content/js/Share.js"></script>
<%=Html.Hidden("id",Model.tmpCustomer.id) %>
<%=Html.Hidden("ComID", Model.tmpCustomer.ComID)%>
<%int CusType = -1; %>
<div class="col-xs-12">
    <div class="box box-danger">
        <div class="box-header with-border">
            <h4 class="box-title"><i class="fa fa-user"></i>THÔNG TIN KHÁCH HÀNG</h4>
        </div>
        <div class="box-body">
            <fieldset>                
                <ol>
                    <%if (Model.tmpCustomer.id > 0)
                      { 
                    %>
                    <li>
                        <label for="Code">
                           <%=Resources.Einvoice.Cus_LblCusCode %>
                        </label>
                        <%=Html.Label(Model.tmpCustomer.Code) %>            
                    </li>
                    <%}
                      else
                      { %>
                    <li>
                        <label for="Code">
                            <%=Resources.Einvoice.Cus_LblCusCode %> <span style="color: red">(*)</span></label>
                        <%=Html.TextBox("Code", Model.tmpCustomer.Code, new { style = "width:250px", @class = "required textandnum", @maxlength="40", title = Resources.Einvoice.Cus_ReqCode })%>
                    </li>
                    <%} %>
                    <li>
                        <label for="TaxCode">
                          Mã số thuế <span id="code" style="color: red; display: none">(*)</span></label>
                        <%=Html.TextBox("TaxCode", Model.tmpCustomer.TaxCode, new { style = "width:250px", title = Resources.Einvoice.Cus_ReqTaxCode, @maxlength="14" })%>
                        <a href="#TaxCode" onclick="check()">Kiểm tra</a></li>
                    <li>
                        <label for="Name">
                           <%=Resources.Einvoice.Cus_LblCusName %> <span style="color: red">(*)</span></label>
                        <%=Html.TextBox("Name", Model.tmpCustomer.Name, new { style = "width:250px", @class = "required",@maxlength="180", title =Resources.Einvoice.Cus_ReqName })%>
                    </li>
                    <%if (Model.tmpCustomer.id > 0)
                      {%>
                    <li>
                        <label for="AccountName">
                            <%=Resources.Einvoice.Cus_LblAccountExist%> <span style="color: red">(*)</span></label>
                        <%=Html.Label(Model.tmpCustomer.AccountName) %>            
                    </li>
                    <%}
                      else
                      {%>
                    <li>
                        <label for="AccountName">
                            <%=Resources.Einvoice.Cus_LblAccountExist%> <span style="color: red">(*)</span></label>
                        <%=Html.TextBox("AccountName", Model.tmpCustomer.AccountName, new {style = "width:250px",@class = "required textandnum",@maxlength="50", title = "Nhập tài khoản cho khách hàng" })%>
                    </li>
                    <%} %>
                    <li>
                        <label for="Address">
                            <%=Resources.Einvoice.Cus_LblAddress %> <span style="color: red">(*)</span>
                        </label>
                        <%=Html.TextBox("Address", Model.tmpCustomer.Address, new { style = "width:500px", @class = "required", @maxlength="290", title = Resources.Einvoice.Cus_ReqAddress })%>
                    </li>
                    <%if (Model.tmpCustomer.DeliverMethod == -1)
                      {%>
                    <li>
                        <label for="DeliverMethod">6. Chọn cách phân phối hóa đơn</label>
                        Gửi hóa đơn qua Mail<input class="deliverMail" name="DeliverMethod" id="Checkbox" type="checkbox" value="0">,
            Gửi hóa đơn qua SMS<input class="DeliverSMS" name="DeliverMethod" id="Checkbox0" type="checkbox" value="1" onclick="checkSMS()" />
                    </li>
                    <%}
                      else if (Model.tmpCustomer.DeliverMethod == 0)
                      {%>
                    <li>
                        <label for="DeliverMethod">Chọn cách phân phối hóa đơn</label>
                        Gửi hóa đơn qua Mail<input class="deliverMail" name="DeliverMethod" id="Checkbox1" type="checkbox" checked="checked" value="0">,
            Gửi hóa đơn qua SMS<input class="DeliverSMS" name="DeliverMethod" id="Checkbox2" type="checkbox" value="1" onclick="checkSMS()" />
                    </li>
                    <%}
                      else if (Model.tmpCustomer.DeliverMethod == 1)
                      {%>
                    <li>
                        <label for="DeliverMethod">Chọn cách phân phối hóa đơn</label>
                        Gửi hóa đơn qua Mail<input class="deliverMail" name="DeliverMethod" id="Checkbox3" type="checkbox" value="0">,
            Gửi hóa đơn qua SMS<input class="DeliverSMS" name="DeliverMethod" id="Checkbox4" type="checkbox" checked="checked" value="1" onclick="checkSMS()" />
                    </li>
                    <%}
                      else if (Model.tmpCustomer.DeliverMethod == 2)
                      {%>
                    <li>
                        <label for="DeliverMethod">6. Chọn cách phân phối hóa đơn</label>
                        Gửi hóa đơn qua Mail<input class="deliverMail" name="DeliverMethod" id="Checkbox5" type="checkbox" checked="checked" value="0">,
            Gửi hóa đơn qua SMS<input class="DeliverSMS" name="DeliverMethod" id="Checkbox6" type="checkbox" checked="checked" value="1" onclick="checkSMS()" />
                    </li>
                    <%}%>
                    <li>
                        <label for="Phone">
                            <%=Resources.Einvoice.Cus_LblPhone %><span id="sdt" style="color: red; display: none">(*)</span>
                        </label>
                        <%=Html.TextBox("Phone", Model.tmpCustomer.Phone, new { style = "width:250px", @class="digits", title ="Nhập số điện thoại!", maxlength="30" ,onkeypress = "return keypress(event);"})%>
                    </li>
                    <li>
                        <label for="Fax">
                            <%=Resources.Einvoice.Cus_lblFax %></label>
                        <%=Html.TextBox("Fax", Model.tmpCustomer.Fax, new { style = "width:250px", @class="digits", maxlength="30" , onkeypress = "return keypress(event);"})%>
                    </li>
                    <li>
                        <label for="Email">
                            <%=Resources.Einvoice.Cus_LblEmail %> <span style="color: red">(*)</span>
                        </label>
                        <%=Html.TextBox("Email", Model.tmpCustomer.Email, new { style = "width:250px", @class = "required email",@maxlength="50", title = Resources.Einvoice.Cus_ReqEmail })%>
                    </li>
                    <li>
                        <label for="RepresentPerson">
                            <%=Resources.Einvoice.Cus_LblRepPerson %>
                        </label>
                        <%=Html.TextBox("RepresentPerson", Model.tmpCustomer.RepresentPerson, new { style = "width:250px", maxlength="140"  })%>
                    </li>
                    <li>
                        <label for="ContactPerson">
                            <%=Resources.Einvoice.Cus_LblContactPerson %>
                        </label>
                        <%=Html.TextBox("ContactPerson", Model.tmpCustomer.ContactPerson, new {@maxlength="140", style = "width:250px" })%>
                    </li>
                    <li>
                        <label for="BankNumber">
                            <%=Resources.Einvoice.Cus_LblBankNumber %>
                        </label>
                        <%=Html.TextBox("BankNumber", Model.tmpCustomer.BankNumber, new { @maxlength="40", @class="digits",style = "width:250px", onkeypress = "return keypress(event);" })%>
                    </li>
                    <li>
                        <label for="BankAccountName">
                            <%=Resources.Einvoice.Cus_LblBankAccont %></label>
                        <%=Html.TextBox("BankAccountName", Model.tmpCustomer.BankAccountName, new {@maxlength="180", style = "width:250px" })%>
                    </li>
                    <li>
                        <label for="BankName">
                            <%=Resources.Einvoice.Cus_LblBankName %></label>
                        <%=Html.TextBox("BankName", Model.tmpCustomer.BankName, new { @maxlength="140",style = "width:250px" })%>
                    </li>
                    <li>
                        <label for="Descriptions">
                            <%=Resources.Einvoice.Cus_LblDescription %></label>
                        <%=Html.TextArea("Descriptions", Model.tmpCustomer.Descriptions,4,40, new { style = "width:300px; height:100px;", maxlength="280"  })%>
                    </li>
                    <li>
                        <label for="CusType">
                            <%=Resources.Einvoice.Cus_LblType%>
                        </label>
                        <%=Html.DropDownList("CusType", new[]
              {
                  new SelectListItem{Text=Resources.Einvoice.Cus_lblNotAccountingCus , Value="0", Selected= (Model.tmpCustomer.CusType == 0)},
                  new SelectListItem{Text=Resources.Einvoice.Cus_lblAccountingCus , Value="1", Selected=  (Model.tmpCustomer.CusType == 1)}
              }, new { style = "width:300px", title =Resources.Einvoice.Cus_ReqInvStatus , onchange = "return cc()", @class = "required" })%>
                    </li>
                    <li id="serial">
                        <label for="SerialCert">
                            <%=Resources.Einvoice.Cus_SerialCert %> <span style="color: red">(*)</span>
                        </label>
                        <%=Html.TextBox("SerialCert", Model.SerialCert, new { style = "width:250px",@maxlength="60", @class="required",title=Resources.Einvoice.Cus_ReqCert })%>                        
                    </li>
          <%if (Model.tmpCustomer.id > 0)
          {%>
        <li>
            <label></label>
            <%=Html.ActionLink("THAY ĐỔI MẬT KHẨU", "ChangePasswordCustomer", "Account", new {username=Model.tmpCustomer.AccountName},null)%>  
        </li>
        <%}%>
                </ol>
            </fieldset>
        </div>
    </div>
</div>
<div style="display: none" id="cer">
    <fieldset>
        <%=Html.Hidden("Cerid", Model.Cerid)%>
        <legend>Thông tin chứng thư ( Đối với khách hàng là doanh nghiệp có chữ ký số ) </legend>
        <ol>
            <li>
                <label>1. Chủ sở hữu chứng thư</label>
                <%=Html.TextBox("OwnCA",Model.OwnCA, new {style = "width:300px", @readonly = "readonly" })%>
            </li>
            <li>
                <label>2. Tên đơn vị cấp chứng thư số</label>
                <%=Html.TextBox("OrganizationCA",Model.OrganizationCA, new {style = "width:300px", @readonly = "readonly" })%>
            </li>
            <li>
                <label>3. Thông tin mã hóa chứng thư</label>
                <%=Html.TextArea("Cer",Model.Cer, new { style = "width:300px; height:50px", @readonly = "readonly" })%>
            </li>
            <li>
                <label>4. Thông tin serialcert </label>
                <%=Html.TextBox("serialcer", Model.serialcer, new { style = "width:300px; height:50px", @readonly = "readonly" })%>
            </li>
            <li>
                <label>5. Ngày bắt đầu</label>
                <%=Html.TextBox("ValidForm", Model.ValidForm, new { style = "width:170px", @readonly = "readonly" })%>
            </li>
            <li>
                <label>6. Ngày hết hạn</label>
                <%=Html.TextBox("ValidTo", Model.ValidTo, new { @readonly="readonly",style="width:170px"})%>
            </li>
        </ol>
    </fieldset>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('#AccountName').on('keypress', function (e) {
            var keypressed = null;
            if (window.event)
                keypressed = window.event.keyCode;
            else
                keypressed = e.which;
            if (keypressed < 48
                    || (keypressed > 57 && keypressed < 65)
                    || (keypressed > 90 && keypressed < 97)
                    || keypressed > 122) {
                if (e.charCode == 0 || e.charCode == 46) {// không phải kí tự thì vẫn ok           
                    return;
                }                
                return false;
            }
        });
        $('form:first').validate();
        $("form:first").submit(function () {
            if (!$('#Code').val() && !$('#Name').val()) {
                $('form:first').valid();
                return false;
            }

            if ($('#AccountName').val().startsWith('.')) {
                $('#AccountName').val('');                
                alertify.alert("Tài khoản khách hàng không được chứa ký tự đặc biệt.");
                return false;
            }
            if ($('#AccountName').val().indexOf(" ") >= 0) {
                $('#AccountName').val('');
                alertify.alert("Tài khoản khách hàng không được chứa khoảng cách.");
                return false;
            }
        });
        
        cc();        
    });
    
    function cc() {
        if ($("#CusType").val() == 0) {
            document.getElementById("TaxCode").className = '';
            document.getElementById("serial").style.display = 'none';
            document.getElementById("SerialCert").className = '';
            // document.getElementById("code").style.display = 'none';
            $('#TaxCode').attr({ title: '' });
            $("#SerialCert").val("");
            $("#Cer").val("");
            $("#OwnCA").val("");
            $("#OrganizationCA").val("");
            $("#ValidForm").val("");
            $("#ValidTo").val("");
        }
        else {
            //document.getElementById("code").style.display = 'block';
            document.getElementById("TaxCode").className = 'required';
            $('#TaxCode').attr({ title: '<%=Resources.Einvoice.Cus_ReqTaxCode %>' });
            document.getElementById("serial").style.display = 'block';
            document.getElementById("SerialCert").className = 'required';
        }
        if ($('input:checkbox:checked.DeliverSMS').val() == "1") {
            document.getElementById("sdt").style.display = 'block';
            document.getElementById("Phone").className = 'required';
        }
        else {
            document.getElementById("sdt").style.display = 'none';
            document.getElementById("Phone").className = "";
        }
    }
    //1.select deliverMethod need:  
    //+ deliverMethod=0:required Email,
    //+ deliverMethod=1:required TelephoneNumber
    function checkSMS() {
        if ($('input:checkbox:checked.DeliverSMS').val() == "1") {
            document.getElementById("sdt").style.display = 'block';
            document.getElementById("Phone").className = 'required';
        }
        else {
            document.getElementById("sdt").style.display = 'none';
            document.getElementById("Phone").className = "";
        }
    }
    function AutoSucGetAcc(accid) {
        $('#' + accid).autocomplete(
        {
            minLength: 1,
            source: function (request, response) {
                $.ajax(
                {
                    type: "POST",
                    url: '/Customer/SearchByAccountName?searchText=' + $('#' + accid).attr("value"),
                    dataType: "json",
                    data: { term: "" },
                    success: function (data) {
                        response($.map(data, function (u) {
                            return {
                                label: u.username,
                                value: u.username
                            }
                        }));
                    }
                });
            }
        });
    }
    function check() {
        if ($("#TaxCode").val() != "")
            checkMST($('#TaxCode').val(), '');
    }       
    //Chỉ cho phép nhập số
    function keypress(e) {
        var keypressed = null;
        if (window.event) { //IE
            keypressed = window.event.keyCode;
            if (keypressed < 48 || keypressed > 57) {
                return false;
            }
        }
        else {
            keypressed = e.which; //NON-IE, Standard
            if (keypressed < 48 || keypressed > 57) {
                if (e.charCode == 0) {// không phải kí tự            
                    return;
                }
                return false;
            }
        }
    }
</script>
