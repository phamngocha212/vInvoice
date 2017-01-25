<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EInvoice.Core.Domain.Menu>" %>

<link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>

<%:Html.Hidden("Id",Model.Id) %>
<%:Html.Hidden("ComID", Model.ComID) %>
<div class="col-xs-12">
    <div class="box box-danger">
        <div class="box-header with-border">
            <h4 class="box-title"><i class="fa fa-pencil"></i>THÔNG TIN CHỨC NĂNG</h4>
        </div>
        <div class="box-body">
            <fieldset>
                <ol>
                    <li>
                        <label class="col-sm-3">Menu cha:</label>
                        <%: Html.DropDownList("ParentId", new SelectList(ViewBag.ParentMenus, "Id", "Name", Model.ParentId), "---Menu cha---", new { style = "width:250px" })%>                        
                    </li>
                    <li>
                        <label class="col-sm-3">Tên hiển thị:<span style="color: red">(*)</span></label>
                        <%=Html.TextBox("Name", Model.Name, new {style = "width:250px",@maxlength="200", @class = "required", title ="Nhập tên!" })%>                       
                    </li>
                    <li>
                        <label class="col-sm-3">Mức ưu tiên:<span style="color: red">(*)</span></label>
                        <%=Html.TextBox("Priority", Model.Priority, new {@class = "number"})%>                       
                    </li>
                    <li>
                        <label class="col-sm-3">Hiển thị:</label>
                        <%=Html.CheckBox("IsPub", Model.IsPub)%>                        
                    </li>
                </ol>
            </fieldset>
        </div>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('form:first').validate();
        $('input[type="text"]').keypress(function (event) {
            return validText(event);
        });
    });

    function validText(e) {
        var keypressed = String.fromCharCode(e.which);
        var dbs = new Array("~", "@", "#", "$", "^", "*", ";", "|", "<", ">", "!");
        var sum = dbs.length;
        var i = 0;
        while (i < sum) {
            if (dbs[i] == keypressed)
                return false;
            i++;
        }
        return;
    }
</script>
