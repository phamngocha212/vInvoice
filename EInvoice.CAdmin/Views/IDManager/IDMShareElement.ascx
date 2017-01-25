<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EInvoice.CAdmin.Models.RoleModel>" %>
<%@ Import Namespace="IdentityManagement.Domain" %>
<link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
<style type="text/css">
    fieldset {
        margin: 20px 0 10px 300px !important;
        width: 400px;
    }
</style>
<script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
<script type="text/javascript">
    // chon tat ca cac checkbox
    function selectAll() {
        // xác dinh xem trang thai checkbox
        if ($('input[name=all]').is(':checked')) {
            // set selected toi tat ca cac checkbox ten permissions
            $('input[name=permissions]').attr('checked', true);
        }
        else {
            $('input[name = permissions]').attr('checked', false);
        }
    }

</script>
<%=Html.Hidden("roleid", Model.Id)%>
<div>    
    <h4 style="margin-bottom: 10px; text-align: center">QUYỀN TRUY CẬP HỆ THỐNG</h4><hr />    

    <div id="Role_top">
        <div class="widget-header"></div>
        <label for="name">
            Tên: <span style="color: red">(*)</span></label>
        <%if (Model.Id > 0)
      {%>
        <label><%=Html.Encode(Model.name) %></label>
        <%}else{ %>
        <%=Html.TextBox("name", Model.name, new { style = "width:200px", @class = "required", title = "Nhập tên mô tả!", maxlength="50"})%>
        <%} %>
    </div>
    <!--End div Role_Top-->
    <p style="margin-top: 15px; font-weight: bold;">Chọn các permission cho role: <span style="color: red">(*)</span> </p>
    <div id="box-permission">
        <label>
            <input style="min-height: 0px;" id="all" name="all" value="" type="checkbox" onclick="selectAll();" />
            Chọn hết
        </label>
        <div class="clear"></div>

        <%
            List<permission> lPermissions = new List<permission>();
            lPermissions = (List<permission>)ViewData["Permissions"];

            foreach (var item in lPermissions)
            {
                string lable = item.Description;
                if (string.IsNullOrWhiteSpace(lable)) lable = item.name;
        %>
        <label>
            <input style="min-height: 0px;" type="checkbox" name="permissions" value="<%=Html.Encode(item.name) %>" <%=FX.Utils.Web.UI.GetChecked(Model.Permissions.Contains(item))%> />
            <%=Html.Encode(lable)%>
        </label>

        <%}%>
    </div>
    <!--End div box-permission-->
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('form:first').validate();
        $("form").submit(function () {            
            if (checkValid($('#name').val()) < 1 || $('#name').val().indexOf(" ") >= 0) {
                $('#name').val('');
                alertify.alert("Quyền truy cập không được chứa ký tự đặc biệt hoặc khoảng cách.");
                return false;
            }
        });
    });

    function checkValid(text) {
        var dbs = new Array("~", "@", "#", "$", "^", "*", "|", "<", ">", "!", "'", ";");
        var sum = dbs.length;
        var i = 0;
        while (i < sum) {
            for (var k = 0 ; k < text.length; k++) {
                if (dbs[i] == text[k])
                    return 0;
            }
            i++;
        }
        return 1;
    }

</script>