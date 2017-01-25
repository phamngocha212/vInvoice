<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CreateInvCancelModel>" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
<link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
<script type="text/javascript" src="/Content/js/Share.js"></script>
<script src="/Content/js/FXUtils.js" type="text/javascript"></script>
<%
    string date = DateTime.Now.ToString("dd/MM/yyyy");
%>

<div class="box">
    <div class="box-header with-border">
        <i class="fa fa-file-text"></i><b><%=Resources.Einvoice.ReCancel_LblNoticeCancel%></b>
    </div>
    <form id="f1" action="" method="post" onsubmit="sub()">
        <%=Html.Hidden("Id",Model.CancelTemp.id) %>
        <%=Html.Hidden("LstCancelDetail", Model.LstCancelDetail)%>
        <fieldset>
            <ol>
                <li>
                    <label for="Name">
                        <%=Resources.Einvoice.ReCancel_lblComName%>:</label>
                    <%=Html.TextBox("Name", Model.CancelTemp.ComName, new { style = "width:350px", @readonly = "readonly", @disabled = "disabled" })%>
                </li>
                <li>
                    <label for="TaxCode">
                        <%=Resources.Einvoice.ReCancel_lblTaxCode%>:</label>
                    <%=Html.TextBox("TaxCode", Model.CancelTemp.ComTaxCode, new { style = "width:100px", @readonly = "readonly", @disabled = "disabled" })%>
                </li>
                <li>
                    <label for="Address">
                        <%=Resources.Einvoice.ReCancel_lblAddress%>:</label>
                    <%=Html.TextBox("Address", Model.CancelTemp.ComAddress, new { style = "width:350px", @readonly = "readonly", @disabled = "disabled" })%>
                </li>
                <li>
                    <label for="Method">
                        <%=Resources.Einvoice.ReCancel_LblMethodCancel%>: <span style="color: red">(*)</span>
                    </label>
                    <%=Html.TextArea("Method", Model.CancelTemp.Method, new { style = "width:350px", @class="required",@title="Nhập phương pháp hủy", maxlength="300" })%>
                </li>
                <li>
                    <label for="Hour">
                        <%=Resources.Einvoice.ReCancel_LblCancelTime%>:</label>
                    <%=Html.TextBox("Hour", Model.CancelTemp.Hour, new { style = "width:28px", @class="required",@title="Nhập giờ hủy", maxlength="2"})%>
                    <%=Resources.Einvoice.ReCancel_Hour%>
                    <%=Html.TextBox("Minute", Model.CancelTemp.Minute, new { style = "width:28px", @class="required",@title="Nhập phút hủy", maxlength="2"})%>
                    <%=Resources.Einvoice.ReCancel_Minute%> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <%=Resources.Einvoice.ReCancel_lblCancelDate%>
                    <%=Html.TextBox("CancelDate", Model.CancelTemp.CancelDate, new { style = "width:86px", @class = "datepicker", @placeholder="__/__/____",  @readonly = "readonly", @onchange = "checkDate()" })%>
                </li>
                <li>
                    <label for="PublishDate">
                        <%=Resources.Einvoice.ReCancel_PublishDate%>:</label>
                    <%=Html.TextBox("PublishDate", Model.CancelTemp.PublishDate, new { style = "width:86px", @class = "datepicker", @placeholder="__/__/____",  @readonly = "readonly", @onchange = "checkPublishDate()" })%>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <%=Resources.Einvoice.ReCancel_LblPreparedPerson%>: <span style="color: red">(*)</span>
                    <%=Html.TextBox("ComPrepared", Model.CancelTemp.ComPrepared, new { style = "width:195px", @class = "required",@title="Nhập người lập biểu", maxlength="200"})%>
                </li>
                <li>
                    <label for="ComRepresent">
                        <%=Resources.Einvoice.ReCancel_LblRepresentPerson%>: <span style="color: red">(*)</span></label>
                    <%=Html.TextBox("ComRepresent", Model.CancelTemp.ComRepresent, new { style = "width:300px", @class = "required",@title="Nhập người đại diện theo pháp luật", maxlength="100" })%>
                </li>
                <li>
                    <label for="">
                        <%=Resources.Einvoice.ReCancel_lblListInvCancel%>: <span style="color: red">(*)</span></label>
                    <div style="float: right">
                        <button type="button" class="btn btn-sm btn-primary" onclick="resetCate()" id="openner"><i class="fa fa-plus"></i>Tạo mới</button>
                    </div>
                    <br />
                    <br />
                    <%=Html.TextBox("f", "", new { @class = "required", title = "Phải nhập danh sách hóa đơn hủy !",style="display:none" }) %>
                    <table class="grid" style="width: 100%; min-width: 800px" id="grid">
                        <thead>
                            <tr>
                                <th>
                                    <%=Resources.Einvoice.ReCancel_LblInvType%> 
                                </th>
                                <th>
                                    <%=Resources.Einvoice.ReCancel_LblPattern%> 
                                </th>
                                <th>
                                    <%=Resources.Einvoice.ReCancel_LblSerial%> 
                                </th>
                                <th>
                                    <%=Resources.Einvoice.ReCancel_LblFromNo%> 
                                </th>
                                <th>
                                    <%=Resources.Einvoice.ReCancel_LblToNo%> 
                                </th>
                                <th>
                                    <%=Resources.Einvoice.ReCancel_LblAmount%> 
                                </th>
                                <th>
                                    <%=Resources.Einvoice.LblEdit%> 
                                </th>
                                <th>
                                    <%=Resources.Einvoice.LblDelete%> 
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </li>
            </ol>
        </fieldset>
    </form>
</div>
<%
    SelectList sl = Model.lstInvCategory;
%>
<div class="modal " id="myModal">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title">Danh sách hủy hóa đơn điện tử</h4>
            </div>
            <div class="modal-body">
                <form class="DialogForm form-horizontal" id="DialogForm" action="">
                    <div class="form-group">
                        <label class="col-sm-3"><%=Resources.Einvoice.ReCancel_ReqTypeInv%>:</label>
                        <div class="col-sm-9">
                            <%=Html.DropDownList("InvCateName", sl,"--Loại hóa đơn--", new {onchange="ajxGetPattern()",@class="required form-control",@title="Chọn loại hóa đơn !" })%>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-3"><%=Resources.Einvoice.ReCancel_LblPattern%>: </label>
                        <div class="col-sm-9">
                            <select id="InvPattern" onchange="ajxGetSerial()" class="form-control required" title="Chọn mẫu số !">
                            </select>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-3">
                            <%=Resources.Einvoice.ReCancel_LblFromNo%>:</label>
                        <div class="col-sm-3">
                            <%=Html.TextBox("CurrentNo", "", new { @class="form-control", @readonly = "readonly" })%>
                        </div>
                        <label class="col-sm-2"><%=Resources.Einvoice.ReCancel_LblToNo%>:</label>
                        <div class="col-sm-3"><%=Html.TextBox("ToNo", "", new { @class="form-control", @readonly = "readonly" })%></div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-3">
                            <%=Resources.Einvoice.ReCancel_LblAmount%>:
                        </label>
                        <div class="col-sm-3">
                            <%=Html.TextBox("Quantity", "", new {@class="form-control", @readonly = "readonly" })%>
                        </div>

                        <label class="col-sm-2"><%=Resources.Einvoice.ReCancel_LblSerial%>:</label>
                        <div class="col-sm-3"><%=Html.TextBox("InvSerial", "", new { @class="form-control", @readonly = "readonly" })%></div>
                    </div>
                    <div class="form-group">

                        <div id="mess" style="color: red"></div>

                    </div>
                </form>
            </div>
            <div class="modal-footer">

                <button class="Save btn btn-success btn-sm" id="Save" type="button" onclick="save()"><i class="fa fa-pencil"></i>Lưu dữ liệu</button>

                <button class="Save btn btn-primary btn-sm" id="SaveAndCreate" type="button" onclick="saveandcreate()"><i class="fa fa-save"></i><%=Resources.Einvoice.BtnSaveAndCreate%></button>

                <button type="button" class="btn btn-sm btn-default pull-left" onclick="btCancel_click()" data-dismiss="modal"><i class="fa fa-backward"></i><%=Resources.Einvoice.BtnBack%></button>

            </div>
        </div>
        <!-- /.modal-content -->
    </div>
    <!-- /.modal-dialog -->
</div>
<!-- /.modal -->
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $(".datepicker").datepicker({
            showOn: "button",
            format: "dd/mm/yyyy",
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            autoclose: true
        });
        $('form:first').validate();

        $('#openner').click(function () {
            $('#myModal').modal('show');
            return false;
        });
        $('#closer').click(function () {
            $('#myModal').modal('hide');
            nextFocus("f");
            return false;
        });
    })
    function closeDialog() {
        $('#dialog').dialog('close');
    }
    //var ipattern = "";
    var pubid = 0;
    function ajxGetPattern() {
        resetForm();
        var jsondata = "id=" + $("#InvCateName").val();
        if ($("#InvCateName").val() == "") return;
        $.ajax({
            type: "POST",
            url: "/InvoiceCancel/ajxGetPattern/",
            data: jsondata,
            dataType: "json",
            success: function (data) {
                //debugger;
                //$.unblockUI();
                if (data.length > 0) {

                    newOpt = new Option("--Chọn mẫu số--", "");
                    document.getElementById("InvPattern").options.add(newOpt);
                    newOpt.selected = true;
                    //debugger;
                    if (CancelSource.length != 0) {
                        for (i = 0; i < data.length; i++)
                            for (j = 0; j < CancelSource.length; j++)
                                if (data[i].PubInvId == CancelSource[j].PubID) {
                                    data.splice(i, 1);
                                    i -= 1;
                                    break;
                                }
                    }
                    for (i = 0; i < data.length; i++) {
                        //debugger;
                        newOption = new Option(data[i].InvPattern + ", " + data[i].InvSerial + " (" + (data[i].CurrentNo + 1) + "-" + data[i].ToNo + ")", data[i].PubInvId);
                        document.getElementById("InvPattern").options.add(newOption);
                    }
                    //debugger;
                    if (pubid != 0) {
                        var objSelect = document.getElementById("InvPattern");
                        for (var i = 0; i < objSelect.options.length; i++) {
                            if (objSelect.options[i].value == pubid) {
                                objSelect.options[i].selected = true;
                                break;
                            }
                        }
                        pubid = 0;
                        //ipattern = "";
                    }
                }
            },
            error: function (xhr, error) {
                //debugger;
                //$.unblockUI();
                alert(xhr.responseText);
            }

        });
    }
    function ajxGetSerial() {
        //debugger;
        //var arr = $('#InvPattern :selected').text().split(',');
        //$("#InvPattern").text(arr[0]);
        //$("#InvPattern:selected").val(test);
        var jsondata = "&id=" + $("#InvPattern").val();
        if ($("#InvPattern").val() == "") return;
        $.ajax({
            type: "POST",
            url: "/InvoiceCancel/ajxGetSerial/",
            data: jsondata,
            success: function (data) {
                //debugger;
                //$.unblockUI();
                $("#InvSerial").val(data.InvSerial);
                $("#CurrentNo").val(pad(data.CurrentNo + 1, 7));
                $("#ToNo").val(pad(data.ToNo, 7));
                $("#Quantity").val(data.ToNo - data.CurrentNo);
            },
            error: function (xhr, error) {
                //debugger;
                //$.unblockUI();
                alert(xhr.responseText);
            }
        });
    }
    function pad(number, length) {
        var str = '' + number;
        while (str.length < length) {
            str = '0' + str;
        }
        return str;
    }
    var currentRow = null;
    var PubDatasource = [];
    var cancelRow = null;
    var CancelSource = [];
    function addCancelData(pubID) {
        //debugger;
        cancelRow = { PubID: pubID };
        CancelSource.push(cancelRow);
    }
    function delCancelData(pubID) {
        //debugger;
        //var data = JSLINQ(CancelSource).Where(function (item) { return item.CateID == cateID && item.InvPattern == invPattern && item.PubID == pubID; }).Select(function (item) { return item; });
        for (i = 0; i < CancelSource.length; i++) {
            if (CancelSource[i].PubID == pubID) {
                CancelSource.splice(i, 1);
                break;
            }
        }
        //CancelSource.
    }
    function sc() {
        if ($("#InvCateName").val() == "") {
            //$("#mess").text("Chọn Loại hóa đơn").show().fadeOut(2000);
            return false;
        }
        if ($("#InvPattern").val() == "" || $("#InvPattern").val() == null) {
            //$("#mess").text("Chọn Mẫu số").show().fadeOut(2000);
            return false;
        }
        if (currentRow == null) {
            var lstpattern = $('#InvPattern :selected').text().split(',');
            var pattern = lstpattern[0];
            currentRow = { id: 0, InvCateID: $('#InvCateName').val(), InvCateName: $('#InvCateName :selected').text(), InvSerial: $('#InvSerial').val(), PubID: $('#InvPattern').val(), InvPattern: pattern, Quantity: $('#Quantity').val(), CurrentNo: $('#CurrentNo').val(), ToNo: $('#ToNo').val() };
            PubDatasource.push(currentRow);
            addCancelData($('#InvPattern').val());
        }
        else {
            var lstpattern = $('#InvPattern :selected').text().split(',');
            var pattern = lstpattern[0];
            currentRow.InvCateID = $('#InvCateName').val();
            currentRow.InvCateName = $('#InvCateName :selected').text();
            currentRow.InvSerial = $('#InvSerial').val();
            currentRow.PubID = $('#InvPattern').val();
            currentRow.InvPattern = pattern;
            currentRow.Quantity = $('#Quantity').val();
            currentRow.CurrentNo = $('#CurrentNo').val();
            currentRow.ToNo = $('#ToNo').val();
            addCancelData($('#InvPattern').val());
        }
        return true;
    }
    function save() {
        //debugger;       
        if (sc()) {
            PubViewDatasource();
            $('#myModal').modal('hide');
        }

        $('#DialogForm').valid();
        $("#f").val("ok");
        nextFocus("f");
    }
    function saveandcreate() {
        //debugger;
        if (sc()) {
            if ($('#DialogForm').valid()) {
                PubViewDatasource();
                resetCate();
                ajxGetPattern();
            }
        }
        $("#f").val("ok");
        nextFocus("f");
    }
    function addRow(GridID, catename, mauso, pubID, cateID, kihieu, tuso, denso, soluong) {
        var newrow = '<tr>';
        newrow += '<td>' + catename + '</td>';
        newrow += '<td>' + mauso + '</td>';
        newrow += '<td style="visibility:hidden;display:none">' + pubID + '</td>';
        newrow += '<td style="visibility:hidden;display:none">' + cateID + '</td>';
        newrow += '<td style="text-align:center">' + kihieu + '</td>';
        newrow += '<td style="text-align:right">' + pad(tuso, 7) + '</td>';
        newrow += '<td style="text-align:right">' + pad(denso, 7) + '</td>';
        newrow += '<td style="text-align:right">' + soluong + '</td>';
        newrow += '<td style="text-align:center"><i class="fa fa-pencil Edit"></span></td>';
        newrow += '<td style="text-align:center"><i class="fa fa-trash Delete"></td>';
        newrow += '</tr>';
        $('#' + GridID + ' tbody').append(newrow);
    }
    function PubViewDatasource() {
        var d = $('#dialog');
        d.dialog({
            autoOpen: false,
            height: 270,
            width: 700,
            modal: true
        });
        $('#grid tbody tr').remove();
        if (PubDatasource == null || PubDatasource == undefined) return;
        for (var i = 0; i < PubDatasource.length; i++) {
            var jsonRow = PubDatasource[i];
            addRow("grid", jsonRow.InvCateName, jsonRow.InvPattern, jsonRow.PubID, jsonRow.InvCateID, jsonRow.InvSerial, jsonRow.CurrentNo, jsonRow.ToNo, jsonRow.Quantity);

        }

        //delete row
        $('#grid tbody tr td .Delete').click(function () {
            if (confirm("You are want delete this row?")) {
                //remove datasource
                var InvCateName = $(this).parent().parent().find('td').eq(0).html();
                var PubID = $(this).parent().parent().find('td').eq(2).html();
                if (InvCateName != null && InvCateName != undefined) {
                    for (var i = 0; i < PubDatasource.length; i++) {
                        if (PubDatasource[i].PubID == PubID)
                            PubDatasource.splice(i, 1);
                    }
                    //remove row of table
                    $(this).parent().parent().remove();
                    delCancelData(PubID);
                }
            }
            if (PubDatasource.length == 0) {

                $("#f").val("");
                nextFocus("f");
            }
        });

        //edit row
        $('#grid tbody tr td .Edit').click(function () {
            // get currentRow ;
            //debugger;
            if (PubDatasource == null || PubDatasource == undefined) return [];
            var PubID = $(this).parent().parent().find('td').eq(2).html();
            var RetSearch = $.grep(PubDatasource,
                function (item, index) {
                    return item.PubID == PubID;
                });
            if (RetSearch.length > 0) currentRow = RetSearch[0];
            else currentRow = null;
            //bind CurrentRow vao form
            bindDataToForm(currentRow);
            //        var $dialog = $('#dialog')
            document.getElementById("SaveAndCreate").style.display = 'none';
            $('#myModal').modal('show');
            delCancelData(PubID);
        });
    }
    function bindDataToForm(JsonObject) {
        if (JsonObject == null || JsonObject == undefined) {
            //xoa trang form
            resetForm();
        }
        else {
            // fill du lieu vao form
            $('#InvCateName').val(JsonObject.InvCateID);
            //debugger;
            pubid = JsonObject.PubID;
            //ipattern = JsonObject.InvPattern;
            ajxGetPattern()
            //debugger;
            //$('#InvPattern').val(JsonObject.InvPattern);
            $('#InvSerial').val(JsonObject.InvSerial);
            $('#Quantity').val(JsonObject.Quantity);
            $('#CurrentNo').val(JsonObject.CurrentNo);
            $('#ToNo').val(JsonObject.ToNo);
        }
    }
    function resetForm() {

        $('#InvSerial').val("");
        $('#Quantity').val("");
        $('#CurrentNo').val("");
        $('#ToNo').val("");
        document.getElementById("InvPattern").innerHTML = "";
    }
    function resetCate() {
        currentRow = null;
        $('#InvCateName').val("");
        resetForm();
    }
    function sub() {
        //debugger;
        cr = null;
        CancelDataSource = [];
        for (i = 0; i < PubDatasource.length; i++) {
            cr = { id: 0, InvCateID: PubDatasource[i].InvCateID, InvCateName: PubDatasource[i].InvCateName, InvSerial: PubDatasource[i].InvSerial, InvPattern: PubDatasource[i].InvPattern, Quantity: PubDatasource[i].Quantity, FromNo: PubDatasource[i].CurrentNo, ToNo: PubDatasource[i].ToNo, InvCancellation: null };
            CancelDataSource.push(cr);
        }
        if (CancelDataSource.toString() == "")
            $("#f").val("");
        else {
            $("#f").val("ok");
            $("#LstCancelDetail").val(JSON.stringify(CancelDataSource));
        }
    };
    function checkDate() {
        var d1 = Date.parseExact($("#CancelDate").val(), "d/M/yyyy");
        var d2 = new Date.parseExact("<%=date%>", "d/M/yyyy");
        if (d1 < d2) {
            alert("<%=Resources.Message.Dec_MesEffectiveDateBigToday %>");
            var now = new Date().toString('dd/MM/yyyy');
            $("#CancelDate").val(now);
            return false;
        }
        return true;
    }
    function checkPublishDate() {
        var d1 = Date.parseExact($("#PublishDate").val(), "d/M/yyyy");
        var d2 = new Date.parseExact("<%=date%>", "d/M/yyyy");
        if (d1 < d2) {
            alert("<%=Resources.Message.Dec_MesEffectiveDateBigToday %>");
            var now = new Date().toString('dd/MM/yyyy');
            $("#PublishDate").val(now);
            return false;
        }
        return true;
    }
</script>
