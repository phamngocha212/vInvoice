<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EInvoice.CAdmin.Models.Captcha>" %>

<div class="form-group">
    <div class="col-sm-12">
    <img src='/Captcha/Show' alt="" />
  
          </div>
</div>
<div class="form-group">
    <div class="col-sm-12">
        <div class="input-group">
            <div class="input-group-addon">
                <i class="fa fa-key"></i>
            </div>
                <%=Html.TextBoxFor(m => m.captch, new { @class = "form-control", placeholder="Mã xác thực"})%>   
        </div>
           
    </div>
</div>
  