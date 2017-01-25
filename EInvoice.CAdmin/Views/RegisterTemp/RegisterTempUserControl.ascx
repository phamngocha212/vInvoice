<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RegisterTempModels>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<script src="/Content/js/jquery.PrintArea.js"></script>
<% 
    int action = ViewData["action"] == null ? 0 : (int)ViewData["action"];
    string previewContent = ViewData["previewContent"] == null ? "" : (string)ViewData["previewContent"];        
%>
<%=Html.Hidden("actionName")%>
<%=Html.Hidden("ac", action)%>
<%=Html.Hidden("pc", previewContent)%>
<%=Html.Hidden("tempId", Model.tempId) %>
<%=Html.Hidden("id", Model.RegisTemp.Id) %>

<div class="box-body form-horizontal">
    <div class="form-group">
        <label class="col-sm-3 control-label">Đơn vị phát hành</label>
        <div class="col-sm-9">
            <%=Html.TextBox("ComName",Model.CurrentCom.Name, new { @class = "form-control", @readonly = "readonly", @disabled = "disabled" })%>
        </div>
    </div>
    <div class="form-group">
        <label class="col-sm-3 control-label">Mã số thuế</label>
        <div class="col-sm-9">
            <%=Html.TextBox("ComTaxCode",Model.CurrentCom.TaxCode, new {@class = "form-control", @readonly = "readonly", @disabled = "disabled" })%>
        </div>
    </div>
    <div class="form-group">
        <label class="col-sm-3 control-label">Địa chỉ</label>
        <div class="col-sm-9">
            <%=Html.TextBox("ComAddress",Model.CurrentCom.Address, new { @class = "form-control", @readonly = "readonly", @disabled = "disabled" })%>
        </div>
    </div>
    <div class="form-group">
        <label class="col-sm-3 control-label">Tên mẫu đăng ký(<span style="color: #f00">*</span>)</label>
        <div class="col-sm-9">
            <%: Html.TextBox("Name", Model.RegisTemp.Name, new { @class="form-control", @required = "required", maxlength = "50"})%>
        </div>
    </div>
    <div class="form-group">
        <label class="col-sm-3 control-label">Tên hóa đơn(<span style="color: #f00">*</span>)</label>
        <div class="col-sm-9">
            <%: Html.TextBox("NameInvoice", Model.RegisTemp.NameInvoice, new { @class="form-control", @required = "required", maxlength = "150"})%>
        </div>
    </div>
    <div class="form-group">
        <label class="col-sm-3 control-label">Mẫu số(<span style="color: #f00">*</span>)</label>
        <div class="col-sm-9">
            <%: Html.TextBox("InvPattern", Model.RegisTemp.InvPattern, new { @style="width:70px", @required = "required"})%>   
                    STT Mẫu:<%: Html.TextBox("PatternOrder", Model.RegisTemp.PatternOrder.ToString("000"), new {@style="width:43px;", @maxlength = "3", @required = "required"})%>
        </div>
    </div>
    <div class="form-group">
        <label class="col-sm-3 control-label">Logo công ty</label>
        <div class="col-sm-9">
            <input type="file" name="logoImg" id="_logoAttackment" accept=".png,.jpg,.gif,.icon" style="display: none; visibility: hidden;" />
            <input type="text" name="logoFile" value="<%=Model.logoFile %>" id="_logoFile" class="form-control" style="border: none; border-bottom: 1px dashed #ccc;" readonly="readonly" placeholder="Nhấn chuột vào đây để đính kèm logo công ty (png, jpg, gif, icon)..." />
        </div>
    </div>
    <div class="form-group">
        <label class="col-sm-3 control-label">Ảnh nền hóa đơn</label>
        <div class="col-sm-9">
            <input type="file" name="bgrImg" id="_imgAttackment" accept=".png,.jpg,.gif" style="display: none; visibility: hidden;" />
            <input type="text" name="imgFile" id="_imgFile" value="<%=Model.imgFile%>" class="form-control" style="border: none; border-bottom: 1px dashed #ccc;" readonly="readonly" placeholder="Nhấn chuột vào đây để đính kèm ảnh nền hóa đơn (png, jpg, gif)..." />
        </div>
    </div>
    <div class="form-group">
        <label class="col-sm-3 control-label">Xem mẫu hóa đơn</label>
        <div class="col-sm-9">
            <a href="#" onclick="previewTemp()"><i class="fa fa-eye"></i>Xem mẫu</a>
        </div>
    </div>
</div>

<div id="ViewInvoice" class="modal modal-default">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-body">
                <div id="container"></div>
                <div id="inbt"></div>
            </div>
            <div class="modal-footer" style="position: relative; z-index: 1000000" id="invoice-footer"></div>
        </div>
    </div>
</div>

<script type="text/javascript">
    function previewTemp() {
        $("#actionName").val("preview");
        document.forms[0].submit()
    }
    $(document).ready(function () {
        $("input[type=text]:first").focus();
        $("#_logoFile").click(function () {
            $("#_logoAttackment").trigger("click")
        }).keypress(function (e) {
            if (e.keyCode == 13 || e.which == 13) {
                $(this).trigger("click");
                return false
            }
        });
        $("#_imgFile").click(function () {
            $("#_imgAttackment").trigger("click")
        }).keypress(function (e) {
            if (e.keyCode == 13 || e.which == 13) {
                $(this).trigger("click");
                return false
            }
        });
        $("#_logoAttackment").change(function () {
            var _file = $(this)[0].files[0];
            if (_file) {
                var _extl = [
                    "png", "jpg", "gif", "icon"
                ];
                var _ext = _file.name.substring(_file.name.lastIndexOf(".") + 1);
                if (_extl.indexOf(_ext) > -1) {
                    $("#_logoFile").val(_file.name)
                } else {
                    $(this).val("");
                    alert("Xin lỗi, định dạng file đính kèm không được chấp nhận.")
                }
            } else {
                $("#_logoFile").val("")
            }
        });
        $("#_imgAttackment").change(function () {
            var _file = $(this)[0].files[0];
            if (_file) {
                var _extl = [
                    "png", "jpg", "gif"
                ];
                var _ext = _file.name.substring(_file.name.lastIndexOf(".") + 1);
                if (_extl.indexOf(_ext) > -1) {
                    $("#_imgFile").val(_file.name)
                } else {
                    $(this).val("");
                    alert("Xin lỗi, định dạng file đính kèm không được chấp nhận.")
                }
            } else {
                $("#_imgFile").val("")
            }
        });
        $("#actionName").val("sub");
        if ($("#pc").val() != "") {
            $("#container").html($("#pc").val() + "<div class='pagination'></div>");
            $("#invoice-footer").html("<button type='button' class='btn btn-default pull-left btn-sm' data-dismiss='modal'><i class='fa fa-close'></i>Close</button><button class='btn btn-sm btn-success' type='button' onclick=\"printInvoice();\"><i class='fa fa-print'></i> In mẫu</button>");
            $('#ViewInvoice').modal('show');
        }
        $("form:first").submit(function () {
            $("input[type='text']").each(function (i) {
                checkTextMaxLength(this);
            });
            if ($('#Name').val().indexOf(" ") >= 0 || $('#InvPattern').val().indexOf(" ") >= 0) {
                alert("Mẫu số và tên mẫu đăng ký không được chứa khoảng cách.");
                return false;
            }
        });
    });
    function printInvoice() {
        $("#inbt").css("display", "none");
        var printElement = document.getElementById("container");
        $(printElement).printArea({
            mode: "iframe",
            popWd: 1000,
            popHt: 900,
            popClose: false
        });
        $("#inbt").css("display", "block");
        return (false);
    }
</script>
