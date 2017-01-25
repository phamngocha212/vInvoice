<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<UploadModel>" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<form method="post" action="/Transaction/UploadInvoiceCancelData" id="adminForm" name="adminForm" enctype="multipart/form-data">
    <div class="box-body">
        <div class="alert alert-danger text-center">Chọn file .zip của file .xml hoặc .xls Upload lên hệ thống, dung lượng file không vượt quá 100MB!</div>
        <div class="row">
            <div class="col-xs-12 form-horizontal">
                <%=Html.Hidden("TypeLabel",Model.TypeLabel) %>
                <%=Html.Hidden("TypeTrans",Model.TypeTrans) %>
                <div class="form-group">
                    <label class="col-sm-3 control-label">
                        <%=Resources.Einvoice.PLInv_LblPattern%>:<span style="color: red">(*)</span>
                    </label>
                    <div class="col-sm-9">
                        <%=Html.DropDownList("Pattern",Model.Listpattern, "--Chọn mẫu số--", new {  @class="form-control required", onchange = "GetDataSerial()", title = "Nhập thông tin!" })%>
                    </div>
                </div>

                <div class="form-group">
                    <label class="col-sm-3 control-label">
                        <%=Resources.Einvoice.PLInv_LblSerial%>:
                    </label>
                    <div class="col-sm-9">
                        <%=Html.DropDownList("Serial",Model.Listserial, "--Chọn ký hiệu--", new { @name = "Serial",@class="form-control"})%>
                    </div>
                </div>

                <div class="form-group">
                    <label class="col-sm-3 control-label">
                        Chọn file:<span style="color: red">(*)</span>
                    </label>
                    <div class="col-sm-9">
                        <input type="file" name="FilePath" id="FilePath" accept=".zip,.ZIP,.xls,.XLS,.xlsx,.XLSX" class="required form-control">
                        <input type="text" id="_txtFile" readonly="readonly" placeholder="Nhấn chuột vào đây để đính kèm file" class="form-control" />
                    </div>
                </div>

                <div class="form-group">
                    <label class="col-sm-3 control-label">
                        File quyết định:<span style="color: red">(*)</span>
                    </label>
                    <div class="col-sm-9">
                        <input type="file" name="FileQDPath" id="FileQDPath" accept=".png,.jpg" class="required form-control">
                        <input type="text" id="txtFileQDPath" readonly="readonly" placeholder="Nhấn chuột vào đây để đính kèm file" class="form-control" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-3 control-label">
                        Lí do hủy:<span style="color: red">(*)</span>
                    </label>
                    <div class="col-sm-9">
                        <textarea id="reasonDel" name="reasonDel" class="required form-control"></textarea>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="box-footer">
        <div class="element-right">
            <button class="btn btn-sm btn-primary" id="uploadButton" type="submit" onclick="SaveUpload()" style="margin-left: 10px;" name="phathanh"><i class="fa fa-upload"></i>UPLOAD DỮ LIỆU</button>
            <button class="btn btn-sm btn-default" type="button" onclick="document.location ='/Transaction/Index?TypeTran=<%=Model.TypeTrans%>'">
                <i class="fa fa-backward"></i>QUAY LẠI</button>
        </div>
    </div>
</form>

<script type="text/javascript">
    function GetDataSerial() {
        var jsondata = "opattern=" + $("#Pattern").val()
        $.ajax({
            type: "POST",
            url: "/EInvoice/GetSerialByPatter/",
            data: jsondata,
            success: function (data) {
                sl = document.getElementById("Serial");
                while (sl.firstChild) {
                    sl.removeChild(sl.firstChild);
                }
                if (data.pu.length > 0) {
                    newOpt = new Option("--Chọn ký hiệu--", "");
                    document.getElementById("Serial").options.add(newOpt);
                    newOpt.selected = true;
                    for (i = 0; i < data.pu.length; i++) {
                        newOption = new Option(data.pu[i]);
                        document.getElementById("Serial").options.add(newOption);
                    }
                    var objSelect = document.getElementById("Serial");
                    for (var i = 0; i < objSelect.options.length; i++) {
                        if (objSelect.options[i].value == document.getElementById("Serial").value) {
                            objSelect.options[i].selected = true;
                            break;
                        }
                    }
                }
            }
        });
    }
    $(document).ready(function () {
        $('#txtFileQDPath').click(function () {
            $('#FileQDPath').trigger('click');
        }).keypress(function (e) {
            if (e.keyCode == 13 || e.which == 13) {
                $(this).trigger('click');
                return false;
            }
        });

        //Ckeck file QD upload
        $('#FileQDPath').change(function () {
            var _file = $(this)[0].files[0];
            if (_file) {
                var _extl = ['peg', 'tiff', 'gif', 'png', 'jpeg', 'jpg', 'bmp', 'PEG', 'TIFF', 'GIF', 'PNG', 'JPEG', 'JPG', 'BMP'];
                var _ext = _file.name.substring(_file.name.lastIndexOf('.') + 1);
                if (_extl.indexOf(_ext) > -1) {

                    if (this.files[0].size > 5242880) {
                        alertify.alert("Dung lượng file phải nhỏ hơn 5MB!");
                        $('#FileQDPath').val('');
                    }
                    else
                        $('#txtFileQDPath').val(_file.name);
                }
                else {
                    $(this).val('');
                    alertify.alert('Xin lỗi, định dạng file đính kèm không được chấp nhận.');
                }
            } else {
                $('#txtFileQDPath').val('');
            }

        });
    });
</script>
