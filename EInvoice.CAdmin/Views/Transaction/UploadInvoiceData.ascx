<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<UploadModel>" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<form method="post" action="/Transaction/UploadInvoiceData" id="adminForm" name="adminForm" enctype="multipart/form-data">
    <div class="box-body">
        <div class="alert alert-danger text-center">Chọn file .zip của file .xml hoặc .xls Upload lên hệ thống, dung lượng file không vượt quá 100MB!</div>
        <div class="row">
            <div class="col-xs-12 form-horizontal">
                <%=Html.Hidden("TypeLabel",Model.TypeLabel) %>
                <%=Html.Hidden("TypeTrans",Model.TypeTrans) %>
                <%=Html.Hidden("DateNow","")%>
                <div class="form-group">
                    <label class="col-sm-3  control-label">Kỳ Cước:<span style="color: red">(*)</span> </label>
                    <div class="col-sm-9">
                        <%=Html.DropDownList("Month", new SelectList(Model.Months, Model.Month.ToString("00")))%>
                        <%=Html.DropDownList("Year", new SelectList(Model.Years, Model.Year), new { onchange="setDate($(this).val())"})%>
                    </div>
                </div>

                <div class="form-group">
                    <label class="col-sm-3  control-label">
                        <%=Resources.Einvoice.PLInv_LblPattern%>:<span style="color: red">(*)</span>
                    </label>
                    <div class="col-sm-9">
                        <%=Html.DropDownList("Pattern",Model.Listpattern, "--Chọn mẫu số--", new {  @class="form-control required", onchange = "GetDataSerial()", title = "Nhập thông tin!" })%>
                    </div>
                </div>

                <div class="form-group">
                    <label class="col-sm-3  control-label">
                        <%=Resources.Einvoice.PLInv_LblSerial%>:
                    </label>
                    <div class="col-sm-9">
                        <%=Html.DropDownList("Serial",Model.Listserial, "--Chọn ký hiệu--", new { @name = "Serial",@class="form-control"})%>
                    </div>
                </div>

                <div class="form-group">
                    <label class="col-sm-3  control-label">
                        Chọn file:<span style="color: red">(*)</span>
                    </label>
                    <div class="col-sm-9">
                        <input type="file" name="FilePath" id="FilePath" accept=".zip,.ZIP,.xls,.XLS,.XLSX" class="required form-control">
                        <input type="text" id="_txtFile" readonly="readonly" placeholder="Nhấn chuột vào đây để đính kèm file" class="form-control" />
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

    function setDate(year) {
        var today = new Date();
        var mm = today.getMonth() + 1;
        var yyyy = today.getFullYear();
        if (yyyy > year) {
            var objSelect = document.getElementById("Month");
            while (objSelect.firstChild) {
                objSelect.removeChild(objSelect.firstChild);
            }
            newOpt = new Option(mm, mm);
            document.getElementById("Month").options.add(newOpt);
            newOpt.selected = true;
            for (i = mm + 1; i <= 12; i++) {
                newOption = new Option(i);
                document.getElementById("Month").options.add(newOption);
            }
        }
        else {
            var objSelect = document.getElementById("Month");
            while (objSelect.firstChild) {
                objSelect.removeChild(objSelect.firstChild);
            }
            newOpt = new Option(mm, mm);
            document.getElementById("Month").options.add(newOpt);
            newOpt.selected = true;
            for (i = mm; i >= 1; i--) {
                newOption = new Option(i);
                document.getElementById("Month").options.add(newOption);
            }
        }
    }
</script>
