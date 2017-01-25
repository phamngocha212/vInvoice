<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Staff>" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%Html.EnableClientValidation(); %>
<link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
<link href="/Content/css/redmond/jquery-ui-1.8.12.custom.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
<script type="text/javascript" src="/Content/js/Share.js"></script>
<%=Html.Hidden("ID", Model.ID)%>
<%=Html.Hidden("ComID", Model.ComID)%>
<div class="row">
<div class="col-xs-6 col-xs-offset-3">
    <div class="box box-danger">
        <div class="box-header with-border">
            <h4 class="box-title"><i class="fa fa-user"></i>THÔNG TIN NHÂN VIÊN</h4>
        </div>
        <div class="box-body form-horizontal">
            <div class="form-group">
                <label class="col-sm-3">1. <%=Resources.Einvoice.Staff_LblName%> <span style="color: red">(*)</span></label>
                <div class="col-sm-9">
                      <%=Html.TextBox("FullName", Model.FullName, new { style = "width:250px", @class = "form-control required", title = Resources.Einvoice.Staff_ReqName , maxlength="200"})%>
                </div>
            </div>   <%if (Model.ID > 0)
                      {%>
              <div class="form-group">
                <label class="col-sm-3">2. <%=Resources.Einvoice.Staff_lblAccountInSystem%>  <span style="color: red">(*)</span></label>
                <div class="col-sm-9">
                         <%=Html.Label(Model.AccountName) %>           
                </div>
            </div>  <%}
                      else
                      { %>
             <div class="form-group">
                <label class="col-sm-3">2. <%=Resources.Einvoice.Staff_lblAccountInSystem%>  <span style="color: red">(*)</span></label>
                <div class="col-sm-9">
                      <%=Html.TextBox("AccountName", Model.AccountName, new { style = "width:250px", @class = "required form-control", title = Resources.Einvoice.Staff_ReqAccount, maxlength="50" })%>
                </div>
            </div>   <%} %>
            <div class="form-group">
                <label class="col-sm-3">3. <%=Resources.Einvoice.Staff_LblDivision%></label>
                <div class="col-sm-9">
                      <%=Html.TextBox("Division", Model.Division, new { style = "width:250px", maxlength="200" ,@class="form-control"})%>
                </div>
            </div>

            <div class="form-group">
                <label class="col-sm-3">4. <%=Resources.Einvoice.Staff_LblAddress%><span style="color: red">(*)</label>
                <div class="col-sm-9">
                    <%=Html.TextBox("Address", Model.Address, new { style = "width:250px", maxlength="200", @class = "form-control required", title = Resources.Einvoice.Staff_ReqAddress })%>
                </div>
            </div>
            <div class="form-group">
                            <label class="col-sm-3"> 5. <%=Resources.Einvoice.Staff_LblPhone%></label>
                            <div class="col-sm-9">
                                 <%=Html.TextBox("Mobile", Model.Mobile, new { style = "width:250px", @class="form-control", maxlength="20", onkeypress = "return keypress(event);" })%>
                            </div>
            </div>

             <div class="form-group">
                            <label class="col-sm-3">  6. <%=Resources.Einvoice.Staff_LblEmail%> <span style="color: red">(*)</span></label>
                            <div class="col-sm-9">
                                   <%=Html.TextBox("Email", Model.Email, new { style = "width:250px", maxlength="50", @class = "required email form-control", title = Resources.Einvoice.Staff_ReqCorrectEmail })%>
                            </div>
            </div>

             <div class="form-group">
                            <label class="col-sm-3">7. <%=Resources.Einvoice.Staff_LblDescription%></label>
                            <div class="col-sm-9">
                                     <%=Html.TextArea("Description", Model.Description, new { style = "width: 408px; height: 97px;",@class="form-control", maxlength="300" })%>
                            </div>
            </div>
        </div>
    </div>
</div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('form:first').validate({ onfocusin: false });
        $("#AccountName").keyup(function (event) {
            if ($(this).val().indexOf(' ') >= 0) {
                alert('Tên tài khoản không có dấu cách');
                $(this).val('');
                return false;
            }
            if (!/^\w+$/i.test($(this).val())) {
                alert('Tài khoản không chứa ký tự lạ hoặc ký tự có dấu');
                return false;
            }
        });
    });
    function AutoSucGetAcc() {
        $('#AccountName').autocomplete(
        {
            minLength: 1,
            source: function (request, response) {
                $.ajax(
                {
                    type: "POST",
                    url: '/Staff/SearchByAccountName?searchText=' + $('#AccountName').attr("value"),
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
            },
            focus: function (event, ui) {
                $("#AccountName").val(ui.item.value);
                return false;
            },
            select: function (event, ui) {
                $("#AccountName").val(ui.item.value);
                return false;
            },
        });
    }

    //Chi cho nhap so
    function keypress(e) {
        var keypressed = null;
        if (window.event) {//IE
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
