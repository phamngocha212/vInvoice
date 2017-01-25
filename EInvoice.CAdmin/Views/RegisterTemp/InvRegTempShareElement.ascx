<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RegisterTempChoiseModels>" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
<form method="post" action="/Registertemp/ChooseTemp">
    <div class="col-xs-12">
        <div class="box box-danger">
            <div class="box-header with-border">
                <h4 class="box-title"><i class="fa fa-file-text"></i>ĐĂNG KÝ MẪU HÓA ĐƠN</h4>
            </div>
            <div class="box-body form-horizontal">
                <div class="form-group">
                    <label class="col-sm-2 control-label">Tên đơn vị</label>
                    <div class="col-sm-9">
                        <%=Html.TextBox("Name",Model.CurrentCom.Name, new { @class = "form-control", @readonly = "readonly", @disabled = "disabled" })%>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-2 control-label">Mã số thuế</label>
                    <div class="col-sm-9">
                        <%=Html.TextBox("TaxCode",Model.CurrentCom.TaxCode, new {@class = "form-control", @readonly = "readonly", @disabled = "disabled" })%>
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-2 control-label">Địa chỉ</label>
                    <div class="col-sm-9">
                        <%=Html.TextBox("Address",Model.CurrentCom.Address, new { @class = "form-control", @readonly = "readonly", @disabled = "disabled" })%>
                    </div>
                </div>

                <div class="form-group">
                    <label class="col-sm-2 control-label">Loại hóa đơn(<span style="color:#f00">*</span>)</label>
                    <div class="col-sm-9">
                        <%=Html.DropDownList("cateId",new SelectList(Model.InvCategories,"id","Name"),"--Loại hóa đơn--",new {@class = "required form-control", title = "Chọn loại hóa đơn."  })%>
                    </div>
                </div>

                <div class="form-group">
                    <label class="col-sm-2 control-label">Mẫu hóa đơn(<span style="color:#f00">*</span>)</label>
                    <div class="col-sm-9">
                        <select name="InvTemps" id="InvTemps" class="form-control">
                            <option>--Chọn mẫu hóa đơn--</option>
                        </select>
                    </div>
                </div>
            </div>
            <div class="box box-footer" style="display: none;text-align:center" id="btnRegister">
                <button class="btn btn-sm btn-primary" type="button" onclick="registerTemp()"><i class="fa fa-check"></i>Đăng ký</button>
                <button class="btn btn-sm" type="button" onclick="viewTemp()">
                    <i class="fa fa-eye"></i>Xem mẫu</button>
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
    </div>
</form>
<script type="text/javascript">
    function registerTemp() {
        document.location = '/Registertemp/create?tempid=' + $('#InvTemps').val();
    }

    function viewTemp() {
        var jsondata = "tempid=" + $('#InvTemps').val();
        $.ajax({
            type: "POST",
            url: "/AjaxData/previewTemplate/",
            data: jsondata,
            success: function (data) {
                $("#container").html(data + "<div class='pagination'></div>");
                $("#invoice-footer").html(" <button type='button' class='btn btn-default pull-left btn-sm' data-dismiss='modal'><i class='fa fa-close'></i>Close</button><button type='button' class='btn btn-sm btn-primary'><a href='/Registertemp/create?tempid=" + $('#InvTemps').val() + "' style='color:white'><i class='fa fa-check'></i>Đăng ký</a></button>");
                $('#ViewInvoice').modal('show');
            }
        });
    }

    $(document).ready(function () {
        $('#cateId').change(function () {
            $('#btnRegister').css('display', 'none');
            var jsd = "invCateId=" + $("#cateId").val();
            $.ajax({
                type: "POST",
                url: "/AjaxData/getTempsbyInvCateId/",
                data: jsd,
                success: function (data) {
                    sl = document.getElementById("InvTemps");
                    while (sl.firstChild) {
                        sl.removeChild(sl.firstChild);
                    }
                    newOpt = new Option("--Chọn mẫu hóa đơn--", "");
                    document.getElementById("InvTemps").options.add(newOpt);
                    newOpt.selected = true;
                    if (data.listTemps.length > 0) {
                        for (i = 0; i < data.listTemps.length; i++) {
                            newOption = new Option(data.listTemps[i].TemplateName, data.listTemps[i].Id);
                            document.getElementById("InvTemps").options.add(newOption);
                        }
                    }
                }
            });
        });

        $('#InvTemps').change(function () {
            if ($('#InvTemps').val()) {
                $('#btnRegister').css('display', 'block');
            }
        });
    });
</script>
