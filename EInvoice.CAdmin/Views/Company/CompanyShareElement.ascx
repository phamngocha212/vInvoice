<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Company>" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%Html.EnableClientValidation(); %>
<link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
<script type="text/javascript" src="/Content/js/Share.js"></script>

<%=Html.Hidden("id", Model.id) %>
<%=Html.Hidden("flag","0") %>
<div class="col-xs-12">
    <div class="box box-danger">
        <div class="box-header with-border">
            <h4 class="box-title"><i class="fa fa-building"></i>SỬA THÔNG TIN ĐƠN VỊ</h4>
        </div>
        <div class="box-body">
            <fieldset>
                <ol>
                    <li>
                        <label for="Name">
                            1. <%=Resources.Einvoice.Com_LblName%> <span style="color: red">(*)</span></label>
                        <label style="margin-left: 0px"><%=Html.Encode(Model.Name)%></label>
                    </li>
                    <li>
                        <label for="TaxCode">
                            2. Mã số thuế <span style="color: red">(*)</span></label>
                        <label style="margin-left: 0px"><%=Html.Encode(Model.TaxCode)%></label>
                    </li>
                    <li>
                        <label for="Address">
                            3. <%=Resources.Einvoice.Com_LblAddress%> <span style="color: red">(*)</span></label>
                        <%=Html.TextBox("Address", Model.Address, new { style = "width:70%", @maxlength="290", @class = "required", title = Resources.Einvoice.Com_ReqAddress })%>
                    </li>
                    <li>
                        <label for="Phone">
                            4. <%=Resources.Einvoice.Com_LblPhone%>
                        </label>
                        <%=Html.TextBox("Phone", Model.Phone, new { style = "width:500px", @maxlength="20" })%>
                    </li>
                    <li>
                        <label for="Fax">
                            5. <%=Resources.Einvoice.Com_LblFax%>
                        </label>
                        <%=Html.TextBox("Fax", Model.Fax, new { style = "width:500px", @maxlength="20" })%>
                    </li>
                    <li>
                        <label for="Email">
                            6. <%=Resources.Einvoice.Com_LblEMail%> <span style="color: red">(*)</span></label>
                        <%=Html.TextBox("Email", Model.Email, new { style = "width:500px", @class = "required email", @maxlength="40", title = Resources.Einvoice.Com_ReqEmail })%>
                    </li>
                    <li>
                        <label for="ContactPerson">
                            7. <%=Resources.Einvoice.Com_LblContactPerson%>
                        </label>
                        <%=Html.TextBox("ContactPerson", Model.ContactPerson, new { style = "width:500px", @maxlength="100" })%>
                    </li>
                    <li>
                        <label for="RepresentPerson">
                            8. <%=Resources.Einvoice.Com_lblRepresentPerson%> <span style="color: red">(*)</span></label>
                        <%=Html.TextBox("RepresentPerson", Model.RepresentPerson, new { style = "width:500px", @maxlength="100", @class = "required", title = Resources.Einvoice.Com_ReqRepPerson })%>
                    </li>
                    <li>
                        <label for="BankNumber">
                            9. <%=Resources.Einvoice.Com_LblBankNumber%>
                        </label>
                        <%=Html.TextBox("BankNumber", Model.BankNumber, new { style = "width:500px", @maxlength="40" })%>
                    </li>
                    <li>
                        <label for="BankAccountName">
                            10. <%=Resources.Einvoice.Com_LblBankAccount%>
                        </label>
                        <%=Html.TextBox("BankAccountName", Model.BankAccountName, new { style = "width:70%", @maxlength="190" })%>
                    </li>
                    <li>
                        <label for="BankName">
                            11. <%=Resources.Einvoice.Com_LblBankName%>
                        </label>
                        <%=Html.TextBox("BankName", Model.BankName, new { style = "width:70%", @maxlength="140" })%>
                    </li>
                    <% IList<TaxAuthority> lsttax = (IList<TaxAuthority>)ViewData["tax"];
                       SelectList selectax = new SelectList(lsttax, "Code", "Name");
                    %>
                    <li>
                        <label for="TaxAuthorityCode">
                            12. <%=Resources.Einvoice.Com_LblTaxName%>
                        </label>
                        <%=Html.DropDownList("TaxAuthorityCode", selectax, "--Cơ quan thuế--", new { @Style = "width:500px" })%>
                    </li>
                    <li>
                        <label for="Descriptions">
                            13. <%=Resources.Einvoice.Com_LblDescription%>
                        </label>
                        <%=Html.TextArea("Descriptions", Model.Descriptions, new { Style = "width:70%", maxlength="280" })%>
                    </li>
                </ol>
            </fieldset>
        </div>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('form:first').validate({ onfocusin: false });
    });

    //function validateFileExtension(fld) {
    //    var photo = document.getElementById("SignatureImage");
    //    var file = photo.files[0];
    //    var file_lenth = file.size;
    //    if (file_lenth >= 512000) {
    //        alert(Resources.Message.Com_MesErrFileSize);
    //        fld.form.reset();
    //        fld.focus();
    //        return false;
    //    }
    //    if (!/(\.png|\.jpg|\.jpeg)$/i.test(fld.value)) {
    //        alert(Resources.Message.Com_MessErrofiletail);
    //        fld.form.reset();
    //        fld.focus();
    //        return false;
    //    }

    //    var reader = new FileReader();
    //    reader.onloadend = function () {
    //        $('#ImageNew').attr('src', reader.result);
    //    }
    //    reader.readAsDataURL(file);
    //    return true;
    //}
</script>

