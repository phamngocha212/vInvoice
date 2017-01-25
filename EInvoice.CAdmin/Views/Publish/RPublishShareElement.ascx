<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PublishModel>" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%Html.EnableClientValidation(); %>
<link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
<link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
<style type="text/css">
    .form-group {
        margin-bottom: 8px;
    }

    #hide td {
        border-bottom: 1px solid #cccccc;
        padding-left: 10px;
    }

    .datepicker {
        z-index: 100000 !important;
    }

    .VATTEMP div label.fl-l, div label {
        margin-right: 0 !important;
    }
</style>
<script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
<script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Content/js/Share.js"></script>
<script src="/Content/js/jquery.PrintArea.js"></script>

<form id="MainForm" name="MainForm" method="post" action="" class="form-horizontal">
    <div class="col-xs-12">
        <div class="box box-danger">
            <div class="box-header with-border">
                <h4 class="box-title"><i class="fa fa-file-text"></i>THÔNG BÁO PHÁT HÀNH HÓA ĐƠN ĐIỆN TỬ</h4>
            </div>
            <div class="box-body">
                <%=Html.Hidden("id", Model.mPublish.id) %>
                <%=Html.Hidden("ComID",Model.mPublish.ComID)%>
                <%=Html.Hidden("PubInvoiceList",Model.PubInvoiceList)%>
                <%=Html.Hidden("tempname")%>
                <fieldset>
                    <ol>
                        <li>
                            <label for="ComName">
                                <%=Resources.Einvoice.Pub_lblComNameCreateInv%>:</label>
                            <%=Html.TextBox("ComName", Model.mPublish.ComName, new { style = "width:600px", @readonly = "readonly", @class = "required", title =Resources.Einvoice.Pub_ReqComName  })%>
                        </li>
                        <li>
                            <label for="ComTaxCode">
                                <%=Resources.Einvoice.Pub_LblTaxCode%>:</label>
                            <%=Html.TextBox("ComTaxCode", Model.mPublish.ComTaxCode, new { style = "width:150px", @class = "required", maxlength="15", title = "Mã số thuế của đơn vị!", @readonly = "readonly" })%>
                        </li>
                        <li>
                            <label for="ComAddress">
                                <%=Resources.Einvoice.Pub_LblComAddress%>: <span style="color: red">(*)</span></label>
                            <%=Html.TextBox("ComAddress", Model.mPublish.ComAddress, new { style = "width:600px",@class="required",title=Resources.Einvoice.Pub_ReqComAddress, maxlength="300" })%>
                        </li>
                        <li>
                            <label for="ComPhone">
                                <%=Resources.Einvoice.Pub_LblPhone%>:</label>
                            <%=Html.TextBox("ComPhone", Model.mPublish.ComPhone, new { style = "width:150px", maxlength="20" })%>
                        </li>
                        <li>
                            <label for="RepresentPerson">
                                <%=Resources.Einvoice.Pub_lblRepresentPerson%>: <span style="color: red">(*)</span></label>
                            <%=Html.TextBox("RepresentPerson", Model.mPublish.RepresentPerson, new { style = "width:200px", maxlength="100", @class = "required", title =Resources.Einvoice.Pub_lblRepresentPerson})%>
                        </li>
                        <li>
                            <label for="TaxAuthorityCode">
                                <%=Resources.Einvoice.Pub_LblTaxName%>:</label>
                            <%=Html.DropDownList("TaxAuthorityCode", Model.TaxList,"[Lựa chọn]", new { style = "width:200px" })%>
                        </li>
                        <li>
                            <label for="City">
                                <%=Resources.Einvoice.Pub_LblCity%>: <span style="color: red">(*)</span></label>
                            <%=Html.TextBox("City", Model.mPublish.City, new { style = "width:200px", @class = "required", maxlength="50" , title = "Nơi lập thông báo!" })%>
                        </li>
                        <li>
                            <label for="Delimiter">
                                Dấu phân cách sử dụng:
                            </label>
                            <%=Html.TextArea("Delimiter", Model.mPublish.Delimiter, new { style = "width: 637px; height: 38px; font-family: Arial,Helvetica,sans-serif; font-size: small;", maxlength="250"})%>
                        </li>
                        <li>
                            <label for="ComPhone">
                                <b><%=Resources.Einvoice.Pub_LblListInvType%> <span style="color: red">(*)</span></b></label>
                        </li>
                        <li>
                            <div style="padding: 10px 5px 5px 5px" class="box-body no-padding table-responsive">
                                <button type="button" id="Create" style="float: right; margin: 5px 18px" class="btn btn-sm btn-primary"><i class="fa fa-plus"></i>Tạo mới</button>
                                <%=Html.TextBox("f", "", new  {@class="required",title="Phải chọn danh sách hóa đơn!",style="display:none" })%>
                                <table id="grid" class="table table-bordered table-hover">
                                    <thead>
                                        <tr>
                                            <th width="150px">
                                                <%=Resources.Einvoice.Pub_LblPattern%>
                                            </th>
                                            <th width="150px">
                                                <%=Resources.Einvoice.Pub_LblSerial%>
                                            </th>
                                            <th width="100px">
                                                <%=Resources.Einvoice.Pub_LblCount%>
                                            </th>
                                            <th width="100px">
                                                <%=Resources.Einvoice.Pub_LblFromNo%>
                                            </th>
                                            <th width="100px">
                                                <%=Resources.Einvoice.Pub_LblToNo%>
                                            </th>
                                            <th>
                                                <%=Resources.Einvoice.Pub_lblBeginUsingDate%>
                                            </th>
                                            <th width="30px" align="center">
                                                <%=Resources.Einvoice.LblEdit%>
                                            </th>
                                            <th width="30px" style="border-right-color: #EEE">
                                                <%=Resources.Einvoice.LblDelete%>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbody13">
                                        <%--set noi dung o day bang javascript--%>
                                    </tbody>
                                </table>

                            </div>
                        </li>
                    </ol>
                </fieldset>
            </div>
        </div>
    </div>
</form>

<div class="modal" id="myModal">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title">Đăng ký mẫu hóa đơn phát hành</h4>
                <%=Html.Hidden("count", Model.RegTempList.Count())%>
            </div>
            <div class="modal-body">
                <form id="DialogForm" name="DialogForm" action="" class="form-horizontal">
                    <div class="form-group">
                        <label class="col-sm-4"><%=Resources.Einvoice.Pub_LblPattern%>: <span style="color: red">(*)</span></label>
                        <div class="col-sm-8">
                            <%=Html.DropDownList("RegisterName", Model.RegTempList, "--Chọn mẫu--", new { style = "width:150px", onchange = "get()", @class = "required", title = "Chọn mẫu!" })%>
                            <a href="#" onclick="ajxCall()" style="display: none" title="Preview" id="pre" name="pre">
                                <img src="/Content/Images/preview.gif" alt="" />
                            </a>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4"><%=Resources.Einvoice.Pub_LblSerial%>: <span style="color: red">(*)</span>:</label>
                        <div class="col-sm-8">
                            <%=Html.TextBox("InvSerialPrefix", "", new { name = "InvSerialPrefix", onchange = "get()", style = "width:30px", @class = "required", minlength = "2", maxlength = "2", title = "Nhập 2 ký tự ", onkeypress = "return char(event);" })%>/
                    <%=Html.TextBox("InvSerialSuffix", "", new { name = "InvSerialSuffix", onchange = "get()", style = "width:30px", @class = "required", minlength = "2", maxlength = "2", title = "Nhập 2 ký tự", onkeypress = "return keypress(event);" })%>E
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4"><%=Resources.Einvoice.Pub_LblCount%>: <span style="color: red">(*)</span></label>
                        <div class="col-sm-8"><%=Html.TextBox("Quantity", "", new { style = "width:100px", onchange = "Sum();", maxlength = "7", onkeypress = "return keypress(event);", @class = "required onlynum", title = "Nhập liệu!" })%></div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4"><%=Resources.Einvoice.Pub_LblFromNo%>:</label>
                        <div class="col-sm-8">

                            <%=Html.TextBox("FromNo", "", new { @class="onlynum" ,style = "width:100px;color:red;background:#F5F5F5" , onkeypress = "return keypress(event);", @readonly = "readonly", maxlength="30" })%>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4"><%=Resources.Einvoice.Pub_LblToNo%>:</label>
                        <div class="col-sm-8">
                            <%=Html.TextBox("ToNo", "", new { @class="onlynum" ,style = "width:100px;color:red", onkeypress = "return keypress(event);", onchange = "quan();", maxlength="30" })%>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4"><%=Resources.Einvoice.Pub_lblBeginUsingDate%>:</label>
                        <div class="col-sm-8">
                            <div class="input-group">
                                <div class="input-group-addon"><i class="fa fa-calendar"></i></div>
                                <%=Html.TextBox("StartDate", "", new { style = "width:150px", title = "Nhập liệu!", @placeholder="__/__/____",  @class = "datepicker  form-control", maxlength="30" })%>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">Tên công ty:</label>
                        <div class="col-sm-8">
                            <%=Html.TextBox("ComNameM", "", new { style = "width:290px", maxlength="100"})%>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">Mã số thuế:</label>
                        <div class="col-sm-8">
                            <%=Html.TextBox("ComTaxCodeM", "", new { style = "width:290px", maxlength="20", @readonly="readonly"})%>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">Địa chỉ:</label>
                        <div class="col-sm-8">
                            <%=Html.TextBox("ComAddressM","", new { style = "width:290px", maxlength="250"})%>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">Fax:</label>
                        <div class="col-sm-8">
                            <%=Html.TextBox("ComFaxM", "", new { style = "width:290px", maxlength="20"})%>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">Điện thoại:</label>
                        <div class="col-sm-8">
                            <%=Html.TextBox("ComPhoneM", "", new {style = "width:290px", maxlength="20"})%>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">Số tài khoản:</label>
                        <div class="col-sm-8">
                            <%=Html.TextBox("ComBankNumberM", "", new {style = "width:290px", maxlength="20"})%>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">Điện thoại:</label>
                        <div class="col-sm-8">
                            <%=Html.TextBox("ComPhoneM", "", new {style = "width:290px", maxlength="20"})%>
                        </div>
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button class="Save btn btn-success btn-sm" id="Save" type="button" onclick="btUpdate_click()"><i class="fa fa-pencil"></i>Lưu dữ liệu</button>

                <button class="Save btn btn-primary btn-sm" id="SaveAndCreate" type="button" onclick="btSave_Create()"><i class="fa fa-save"></i><%=Resources.Einvoice.BtnSaveAndCreate%></button>

                <button type="button" class="btn btn-sm btn-default pull-left" onclick="btCancel_click()" data-dismiss="modal"><i class="fa fa-backward"></i><%=Resources.Einvoice.BtnBack%></button>
            </div>
        </div>
        <!-- /.modal-content -->
    </div>
    <!-- /.modal-dialog -->
</div>
<!-- /.modal -->
<div id="src" class="modal modal-default">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-body">
                <div id="invData"></div>
            </div>
            <div class="modal-footer" id="invoice-footer"></div>
        </div>
    </div>
</div>
<script type="text/javascript">      
    $(document).ready(function (){
        $(".datepicker").datepicker({
            format: 'dd/mm/yyyy',
            showOn: "button",                        
            changeMonth: true,
            changeYear: true,
            startDate: '+0d',
            autoclose: true
        });
    });
    var arrRegis = new Array();
    var c = $("#count").val();    
    function get() {
        var jsondata = "Id=" + $("#RegisterName").val() + "&InvSerialPrefix=" + $("#InvSerialPrefix").val() + "&InvSerialSuffix=" + $("#InvSerialSuffix").val();
        if ($("#RegisterName").val() == "" || $("#RegisterName").val() == "Select one") {
            document.getElementById("pre").style.display = "none";
            return
        }
        $.ajax({
            data   : jsondata,            
            type   : "POST",
            url    : "/Publish/ajx/"
        }).done(function(data){
            $("#FromNo").val(pad(data.ic + 1, 7));
            Sum();
            $("#tempname").val(data.tempname)
        }).fail(function(){ alertify.alert("Có lỗi xảy ra, vui lòng thực hiện lại."); });
        regId   = $("#RegisterName").val();
        regname = $("#RegisterName :selected").text();
        document.getElementById("pre").style.display = "inline"
    }
    function ajxCall() {
        viewTemp();
    }
    function viewTemp() {
        var jsondata = {            
            serialNo     : $("#InvSerialPrefix").val() + "/" + $("#InvSerialSuffix").val() + "E",
            tempid       : $("#RegisterName").val()
        };
        var promise = $.ajax({
            data   : jsondata,            
            type   : "POST",
            url    : "/Publish/AjxPreviewPubInv/"
        });
        promise.done(function(data){
            document.getElementById("invData").innerHTML = data;
            document.getElementById("invoice-footer").innerHTML =
                        "<button class='btn btn-sm btn-success' type='button' onclick=\"printInvoice();\"><i class='fa fa-print'></i> In mẫu</button>";
            $('#src').modal('show');
        }).fail(function() { alertify.alert("Có lỗi xảy ra, vui lòng thực hiện lại."); })       
    }
    function pad(number, length) {
        var str = "" + number;
        while (str.length < length) {
            str = "0" + str
        }
        return str
    }
    function keypress(e) {
        var keypressed = null;
        if (window.event) {
            keypressed = window.event.keyCode;
        } else {
            keypressed = e.which;
        }
        if (keypressed < 48 || keypressed > 57) {
            if (keypressed == 8 || keypressed == 127) {
                return
            }
            return false
        }
    }
    function char(e) {
        var char = null;
        if (window.event) {
            char = window.event.keyCode;
        } else {
            char = e.which;
        }
        if (char < 65 || char > 122) {
            if (char == 8 || char == 127) {
                return
            }
            return false
        }
    }
    var currentRow = null;
    $("#Create").click(function () {
        var date = $("#StartDate").val();
        $("#RegisterName").val("");
        currentRow = null;
        if (arrRegis.length >= parseInt(c, 10)) 
            alert("<%=Resources.Message.AdjReInv_MesNoRTemp%>");
        else {
            var objSelect = document.getElementById("RegisterName");
            for (var i = 0; i < arrRegis.length; i++) 
                for (var j = 0; j < objSelect.options.length; j++) {
                    if (objSelect.options[j].text == arrRegis[i]) 
                        objSelect.remove(j);
                }
            bindDataToForm(currentRow);
            document.getElementById("SaveAndCreate").style.display = "inline";        
            $('#myModal').modal('show');
        }
    });
    var PubDatasource = [];
    var regId = 0;
    var regname = null;
    function AddRegis(regId, regisname) {
        var objSelect = document.getElementById("RegisterName");
        for (var i = 0; i < objSelect.options.length; i++) {
            if (objSelect.options[i].text == regisname) {
                objSelect.remove(i)
            }
        }
        var optn = document.createElement("option");
        optn.text  = regisname;
        optn.value = regId;
        objSelect.add(optn)
    }
    function AddValue(regId, regisname) {
        var objSelect = document.getElementById("RegisterName");
        for (var i = 0; i < arrRegis.length; i++) 
            for (var j = 0; j < objSelect.options.length; j++) {
                if (objSelect.options[j].text == arrRegis[i]) 
                    objSelect.remove(j);
            }
        var optn = document.createElement("option");
        optn.text  = regisname;
        optn.value = regId;
        objSelect.add(optn)
    }
    function DelSelected() {
        var objSelect = document.getElementById("RegisterName");
        for (var i = 0; i < objSelect.options.length; i++) {
            if (objSelect.options[i].value == regId) {
                objSelect.remove(i)
            }
        }
    }
    function PubViewDatasource() {
        $("#f").val("ok");
        $("#grid tbody tr").remove();
        if (PubDatasource == null || PubDatasource == undefined) 
            return;
        for (var i = 0; i < PubDatasource.length; i++) {
            var jsonRow = PubDatasource[i];
            addRow("grid", jsonRow.RegisterName, jsonRow.RegisterID, jsonRow.InvSerialPrefix, jsonRow.InvSerialSuffix, jsonRow.Quantity, jsonRow.FromNo, jsonRow.ToNo, jsonRow.StartDate);
            arrRegis[i] = jsonRow.RegisterName;
        }
        $("#grid tbody tr:odd").addClass("oddtr");
        $("#grid tbody tr").mouseover(function () {
            $(this).addClass("trover")
        }).mouseout(function () {
            $(this).removeClass("trover")
        });
        $("#grid tbody tr td span.Delete").click(function () {
            if(!confirm("Bạn có muốn xóa dòng này không?")) return;
            var RegisterName = $(this).parent().parent().find("td").eq(0).html();
            var RegId = $(this).parent().parent().find("td").eq(1).html();
            if (RegisterName != null && RegisterName != undefined) {
                for (var i = 0; i < PubDatasource.length; i++) {
                    if (PubDatasource[i].RegisterName == RegisterName) {
                        PubDatasource.splice(i, 1);
                        arrRegis.splice(i, 1)
                    }
                }
                $(this).parent().parent().remove();
                AddRegis(RegId, RegisterName)
            }
            if (PubDatasource.length == 0) {
                $("#f").val("");
                nextFocus("f")
            }                                      
        });
        $("#grid tbody tr td span.Edit").click(function () {
            document.getElementById("pre").style.display = "inline";
            if (PubDatasource == null || PubDatasource == undefined) 
                return [];
            var RegisterName = $(this).parent().parent().find("td").eq(0).html();
            var RegId = $(this).parent().parent().find("td").eq(1).html();
            var RetSearch = $.grep(PubDatasource, function (item, index) {
                return item.RegisterName == RegisterName
            });
            if (RetSearch.length > 0) 
                currentRow = RetSearch[0];
            else 
                currentRow = null;
            AddValue(RegId, RegisterName);
            bindDataToForm(currentRow);
            document.getElementById("SaveAndCreate").style.display = "none";
            $("#myModal").modal("show");
        })
    }
    function addRow(GridID, nameregis, regisId, kihieutt, kihieuht, soluong, tuso, denso, ngaybd) {
        kihieuht += "E";
        var newrow = "<tr>";
        newrow += "<td style='text-align:center'>" + nameregis + "</td>";
        newrow += "<td style='visibility:hidden;display:none'>" + regisId + "</td>";
        newrow += "<td style='text-align:center'>" + kihieutt.toUpperCase() + "/" + kihieuht + "</td>";
        newrow += "<td style='text-align:right'>" + soluong + "</td>";
        newrow += "<td style='text-align:right'>" + pad(tuso, 7) + "</td>";
        newrow += "<td style='text-align:right'>" + pad(denso, 7) + "</td>";
        newrow += "<td style='text-align:center'>" + ngaybd + "</td>";
        newrow += "<td align='center'><span class='glyphicon glyphicon-pencil Edit'></span></td>";
        newrow += "<td align='center'><span class='glyphicon glyphicon-trash Delete'></td>";
        newrow += "</tr>";
        $("#" + GridID + " tbody").append(newrow)
    }
    function btSave_Create() {
        if (!$("#Quantity").val() || $("#InvSerialPrefix").val().length != 2 || $("#InvSerialSuffix").val().length != 2) {
            $("#DialogForm").valid();
        } else if (!$("#StartDate").val()) {
            alert("<%=Resources.Message.Dec_MesErrSelectDate%>");
                $("#DialogFrom").valid()
            } else {                
                if (currentRow == null) {
                    currentRow = {
                        FromNo         : $("#FromNo").val(),
                        id             : 0,
                        InvSerialPrefix: $("#InvSerialPrefix").val().toUpperCase(),
                        InvSerialSuffix: $("#InvSerialSuffix").val(),
                        PubTemp        : [
                            {
                                ComAddress   : $("#ComAddressM").val(),
                                ComBankNumber: $("#ComBankNumberM").val(),
                                ComFax       : $("#ComFaxM").val(),
                                ComName      : $("#ComNameM").val(),
                                ComPhone     : $("#ComPhoneM").val(),
                                ComTaxCode   : $("#ComTaxCodeM").val(),
                                id           : 0,
                                PublishInvID : 0
                            }
                        ],
                        Quantity       : $("#Quantity").val(),
                        RegisterID     : $("#RegisterName").val(),
                        RegisterName   : $("#RegisterName :selected").text(),
                        StartDate      : $("#StartDate").val(),
                        ToNo           : $("#ToNo").val()
                    };
                    PubDatasource.push(currentRow)
                } else {
                    currentRow.RegisterID               = $("#RegisterName").val();
                    currentRow.RegisterName             = $("#RegisterName :selected").text();
                    currentRow.InvSerialPrefix          = $("#InvSerialPrefix").val().toUpperCase();
                    currentRow.InvSerialSuffix          = $("#InvSerialSuffix").val();
                    currentRow.Quantity                 = $("#Quantity").val();
                    currentRow.FromNo                   = $("#FromNo").val();
                    currentRow.ToNo                     = $("#ToNo").val();
                    currentRow.StartDate                = $("#StartDate").val();
                    currentRow.PubTemp[0].ComName       = $("#ComNameM").val();
                    currentRow.PubTemp[0].ComAddress    = $("#ComAddressM").val();
                    currentRow.PubTemp[0].ComPhone      = $("#ComPhoneM").val();
                    currentRow.PubTemp[0].ComFax        = $("#ComFaxM").val();
                    currentRow.PubTemp[0].ComBankNumber = $("#ComBankNumberM").val();
                    currentRow.PubTemp[0].ComTaxCode    = $("#ComTaxCodeM").val()
                }
                PubViewDatasource();
                DelSelected()
            }
        $("#RegisterName").val("");
        $("#InvSerialPrefix").val("");
        $("#InvSerialSuffix").val("");
        $("#Quantity").val("");
        $("#FromNo").val("");
        $("#ToNo").val("");
        $("#StartDate").val("");
        currentRow = null;
        $("#f").val("ok");
        nextFocus("f");
        //$('#myModal').modal('hide');
    }
    function btUpdate_click() {
        if (!$("#Quantity").val() || $("#InvSerialPrefix").val().length != 2 || $("#InvSerialSuffix").val().length != 2 || $("#RegisterName").val() == "") {
            $("#DialogForm").valid()
        } else if (!$("#StartDate").val()) {
            alert("<%=Resources.Message.Dec_MesErrSelectDate%>");
            $("#DialogFrom").valid()
        } else {            
            if (currentRow == null) {
                currentRow = {
                    FromNo         : $("#FromNo").val(),
                    id             : 0,
                    InvSerialPrefix: $("#InvSerialPrefix").val().toUpperCase(),
                    InvSerialSuffix: $("#InvSerialSuffix").val(),
                    PubTemp        : [
                        {
                            ComAddress   : $("#ComAddressM").val(),
                            ComBankNumber: $("#ComBankNumberM").val(),
                            ComFax       : $("#ComFaxM").val(),
                            ComName      : $("#ComNameM").val(),
                            ComPhone     : $("#ComPhoneM").val(),
                            ComTaxCode   : $("#ComTaxCodeM").val(),
                            id           : 0,
                            PublishInvID : 0
                        }
                    ],
                    Quantity       : $("#Quantity").val(),
                    RegisterID     : $("#RegisterName").val(),
                    RegisterName   : $("#RegisterName :selected").text(),
                    StartDate      : $("#StartDate").val(),
                    ToNo           : $("#ToNo").val()
                };
                PubDatasource.push(currentRow)
            } else {
                currentRow.RegisterID               = $("#RegisterName").val();
                currentRow.RegisterName             = $("#RegisterName :selected").text();
                currentRow.InvSerialPrefix          = $("#InvSerialPrefix").val().toUpperCase();
                currentRow.InvSerialSuffix          = $("#InvSerialSuffix").val();
                currentRow.Quantity                 = $("#Quantity").val();
                currentRow.FromNo                   = $("#FromNo").val();
                currentRow.ToNo                     = $("#ToNo").val();
                currentRow.StartDate                = $("#StartDate").val();
                currentRow.PubTemp[0].ComName       = $("#ComNameM").val();
                currentRow.PubTemp[0].ComAddress    = $("#ComAddressM").val();
                currentRow.PubTemp[0].ComPhone      = $("#ComPhoneM").val();
                currentRow.PubTemp[0].ComFax        = $("#ComFaxM").val();
                currentRow.PubTemp[0].ComBankNumber = $("#ComBankNumberM").val();
                currentRow.PubTemp[0].ComTaxCode    = $("#ComTaxCodeM").val()
            }
            PubViewDatasource();
            $('#myModal').modal('hide');
            DelSelected();
        }
    $("#f").val("ok");
    nextFocus("f")
}
function btCancel_click() {
    $("#dialog").dialog("close");
    nextFocus("f")
}
function bindDataToForm(JsonObject) {
    if (JsonObject == null || JsonObject == undefined) {
        $("#InvSerialPrefix").val("");
        $("#InvSerialSuffix").val("");
        $("#Quantity").val("");
        $("#FromNo").val("");
        $("#ToNo").val("");
        $("#StartDate").val("");
        $("#ComNameM").val($("#ComName").val());
        $("#ComAddressM").val($("#ComAddress").val());
        $("#ComPhoneM").val($("#ComPhone").val());
        $("#ComFaxM").val($("#ComFax").val());
        $("#ComTaxCodeM").val($("#ComTaxCode").val());
        $("#ComBankNumberM").val($("#ComBankNumber").val())
    } else {
        $("#RegisterName").val(JsonObject.RegisterID);
        $("#InvSerialPrefix").val(JsonObject.InvSerialPrefix);
        $("#InvSerialSuffix").val(JsonObject.InvSerialSuffix);
        $("#Quantity").val(JsonObject.Quantity);
        $("#FromNo").val(pad(JsonObject.FromNo, 7));
        $("#ToNo").val(pad(JsonObject.ToNo, 7));
        $("#StartDate").val(JsonObject.StartDate);
        $("#ComNameM").val(JsonObject.PubTemp[0].ComName);
        $("#ComAddressM").val(JsonObject.PubTemp[0].ComAddress);
        $("#ComPhoneM").val(JsonObject.PubTemp[0].ComPhone);
        $("#ComFaxM").val(JsonObject.PubTemp[0].ComFax);
        $("#ComTaxCodeM").val(JsonObject.PubTemp[0].ComTaxCode);
        $("#ComBankNumberM").val(JsonObject.PubTemp[0].ComBankNumber)
    }
}
PubDatasource = <%=Model.PubInvoiceList %>;
function Sum() {
    var Quantity,
        FromNo,
        sum = new Number();
    var c = new String();
    c        = $("#FromNo").val();
    FromNo   = parseInt(c, 10);
    c        = $("#Quantity").val();
    Quantity = parseInt(c, 10);
    if (Quantity == 0) {
        alert("Nhập giá trị lớn hơn 0!");
        $("#Quantity").val("");
        $("#ToNo").val("")
    }
    sum = Quantity + FromNo - 1;
    if (sum.toString().length > 7) {
        $("#Quantity").val("");
        $("#ToNo").val("")
    } else {
        if (FromNo && Quantity) {
            $("#ToNo").val(pad(sum, 7));
            $("#FromNo").val(pad($("#FromNo").val(), 7))
        }
    }
}
function quan() {
    var ToNo, FromNo, h = new Number();
    var c = new String();
    c      = $("#ToNo").val();
    ToNo   = parseInt(c, 10);
    c      = $("#FromNo").val();
    FromNo = parseInt(c, 10);
    h      = ToNo - FromNo + 1;
    if (ToNo && FromNo) {
        $("#Quantity").val(h);
        $("#ToNo").val(pad($("#ToNo").val(), 7))
    }
    if ($("#ToNo").val() < $("#FromNo").val()) {
        alert("<%=Resources.Einvoice.Pub_ReqFromNumLessToNum %>");
        $("#Quantity").val("");
        $("#ToNo").val("")
    }
}
function printInvoice() {    
    var printElement = document.getElementById("invData");
    $(printElement).printArea({
        mode    : "iframe",
        popClose: false,
        popHt   : 900,
        popWd   : 1000
    });    
    return (false)
}
 
$(document).ready(function () {     
    $("form:first").validate();
    $("#DialogForm").validate({
        messages: {
            StartDate: {
                required: "<%=Resources.Message.Dec_MesErrSelectDate%>"
            }
        },
        rules   : {
            StartDate: {
                required: true
            }
        }
    });
              
    $("#InvName").val($("#InvCateName").val());
    PubViewDatasource();
    $("#InvCateName").val($("#InvName").val());
    $("#InvSerialPrefix").change(function () {
        $(this).val($(this).val().toUpperCase())
    });
 
    $("#MainForm").submit(function () {
        if (PubDatasource.length== 0) {
            $("#f").val("");
            return false
        } else {
            $("#f").val("ok");
            if(!$("#ComAddress").val() || !$('#City').val() || !$('#RepresentPerson').val())                
                return false;
            for (z = 0; z < PubDatasource.length; z++) {
                var j = PubDatasource[z].StartDate.split("/");
                PubDatasource[z].StartDate = j[1] + "/" + j[0] + "/" + j[2];
                if (parseInt(j[1]) > 12) 
                    PubDatasource[z].StartDate = j[0] + "/" + j[1] + "/" + j[2];
            }
            $("#PubInvoiceList").val(JSON.stringify(PubDatasource))
        }
    });   
});
</script>

