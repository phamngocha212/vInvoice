<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DecisionModels>" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%Html.EnableClientValidation(); %>
<link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css">
<link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
<script type="text/javascript" src="/Content/js/Share.js"></script>
<form id="MainForm" name="MainForm" method="post" action="" class="form-horizontal">
    <%=Html.Hidden("id", Model.id) %>
    <%=Html.Hidden("ComID", Model.ComID)%>
    <%=Html.Hidden("ComAddress", Model.ComAddress)%>
    <%=Html.Hidden("DecDatasource",Model.DecDatasource)%>
    <%=Html.Hidden("ComName",Model.ComName)%>
    <div class="col-xs-12">
        <div class="box box-danger">
            <div class="box-header with-border">
                <h4 class="box-title"><i class="fa fa-file-text"></i>QUYẾT ĐỊNH PHÁT HÀNH HÓA ĐƠN ĐIỆN TỬ</h4>
            </div>
            <div class="box-body">
                <fieldset>                    
                    <ol>
                        <li>
                            <label for="ComName">
                                <%=Resources.Einvoice.Dec_LblComName %>
                            </label>
                            <%=Html.TextBox("ParentCompany", Model.ParentCompany, new { style = "width:400px",maxlength="1000",  title = "Tên đơn vị chủ quản." })%>
                        </li>
                        <li>
                            <label for="TaxCode">
                                <%=Resources.Einvoice.Dec_LblTaxCode %>
                            </label>
                            <%=Html.TextBox("TaxCode", Model.TaxCode, new { style = "width:150px", @readonly = "true",maxlength="16", @class = "required", title = "Mã số thuế đơn vị chủ quản." })%>
                        </li>
                        <li>
                            <label for="DecisionNo">
                                <%=Resources.Einvoice.Dec_LblNumberDecisions %> <span style="color: red">(*)</span></label>
                            <%=Html.TextBox("DecisionNo", Model.DecisionNo, new { style = "width:150px",maxlength="10", @class = "required", title = "Số quyết định." })%>
                        </li>
                        <li>
                            <label for="Director">
                                <%=Resources.Einvoice.Dec_LblDirector %><span style="color: red">(*)</span></label>
                            <%=Html.TextBox("Director", Model.Director, new { style = "width:200px",maxlength="200",@class = "required", title = "Giám đốc" })%>
                        </li>
                        <li>
                            <label for="Requester"><b><%=Resources.Einvoice.Dec_LblRequester%></b><span style="color: red">(*)</span></label>
                        </li>
                        <%if (Model.id > 0)
                          {%>
                        <li>
                            <%=Html.TextArea("Requester",Model.Requester, new { style = "width:100%", @class = "editor" ,maxlength="500"})%>   
                        </li>
                        <%}
                          else
                          {%>
                        <li>
                            <%=Html.TextArea("Requester", "-Căn cứ Thông tư số 32/2011/TT-BTC ngày 14/3/2011 của Bộ Tài chính hướng dẫn về khởi tạo, phát hành và sử dụng hoá đơn điện tử bán hàng hóa, cung ứng dịch vụ." + Model.Requester, new { style = "width:100%", @class = "editor"})%>
                        </li>
                        <%} %>
                        <li>
                            <label for="Description">
                                <b><%=Resources.Einvoice.Dec_LblArticle1 %> :</b>Hệ thống thiết bị:</label>
                        </li>
                        <li>
                            <%=Html.TextArea("SystemName", Model.SystemName, new { style = "width:100%", @class = "editor",maxlength="500" })%>
                        </li>
                        <li>
                            <label for="SoftApplication">
                                <%=Resources.Einvoice.Dec_LblSoftwareApp %> :</label></li>
                        <li>
                            <%=Html.TextArea("SoftApplication", Model.SoftApplication, new { maxlength="480", @style="width:100%"})%>                                
                        </li>

                        <li>
                            <label for="TechDepartment"><%=Resources.Einvoice.Dec_LblTechDepartment %>  </label>
                        </li>
                        <li>
                            <%=Html.TextArea("TechDepartment", Model.TechDepartment, new { maxlength="500", @style="width:100%", @class="editor"})%>                
                        </li>
                        <li>
                            <label><b><%=Resources.Einvoice.Dec_LblArticle2 %> :</b><%=Resources.Einvoice.Dec_LblListInvType %></label>
                        </li>
                        <li>
                            <div style="padding: 10px 5px 5px 5px">
                                <b><%=Resources.Einvoice.Dec_LblChoosePattern %><span style="color: red">(*)</span></b>
                                <%=Html.TextBox("Article2", "", new  {@class="required",title="Phải chọn điều 2",@style="display:none" })%>
                                <table id="grid" style="width: 100%; min-width: 800px; margin-top: 7px" class="grid">
                                    <thead>
                                        <tr>
                                            <th width="50px">
                                                <%=Resources.Einvoice.Dec_Pattern%>
                                            </th>
                                            <th width="200px">
                                                <%=Resources.Einvoice.Dec_LblListInvType %>
                                            </th>
                                            <th width="450px">
                                                <%=Resources.Einvoice.Dec_LblPurpose %>
                                            </th>
                                            <th width="30px">
                                                <%=Resources.Einvoice.LblEdit %>
                                            </th>
                                            <th width="30px">
                                                <%=Resources.Einvoice.LblDelete %>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbody13">
                                    </tbody>
                                </table>
                                <button type="button" id="Create" class="btn btn-primary element-right" style="margin-top: 5px;"><i class="fa fa-plus"></i>Tạo mới</button>
                            </div>
                        </li>
                        <li>
                            <label for="Workflow">
                                <b><%=Resources.Einvoice.Dec_LblArticle3%>:</b> <%=Resources.Einvoice.Dec_Workflow%></label>
                        </li>
                        <li>
                            <%=Html.TextArea("Workflow", Model.Workflow, new { @style="width:100%", @class="editor"})%>
                        </li>
                        <li>
                            <label for="Responsibility">
                                <b><%=Resources.Einvoice.Dec_LblArticle4%>:</b><%=Resources.Einvoice.Dec_LblResponsibility%></label>
                        </li>
                        <li>
                            <%=Html.TextArea("Responsibility", Model.Responsibility, new { @style="width:100%", @class="editor"})%>
                        </li>
                        <li>
                            <label for="EffectiveDate">
                                <b><%=Resources.Einvoice.Dec_LblArticle5%>:</b><%=Resources.Einvoice.Dec_LblEffectDate%>:
                            </label>                            
                        </li>
                        <li>
                            <%=Html.TextArea("EffectiveDate", Model.EffectiveDate, new { @style="width:100%", @class="editor"})%>
                        </li>
                        <li>
                            <label>Nơi nhận:</label>
                        </li>
                        <li>
                            <%=Html.TextArea("Destination", Model.Destination, new { maxlength="300", @style="width:100%", @class="editor"})%>
                        </li>
                        <li>
                            <label for="city"><%=Resources.Einvoice.Dec_LblCity%></label>
                            <%=Html.TextBox("City", Model.City, new {maxlength="50", style="width: 200px; margin-left: -40px"  })%>
                        </li>
                    </ol>
                </fieldset>
            </div>
        </div>
    </div>
</form>
<%  
    string date = DateTime.Now.ToString("dd/MM/yyyy");
    int count = Model.RegTempList.Count();   
%>
<div class="modal" id="myModal">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <%=Html.Hidden("count", count)%>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title">Đăng ký loại hóa đơn phát hành</h4>
            </div>
            <div class="modal-body form-horizontal">
                <div class="form-group">
                    <label class="col-sm-4"><%=Resources.Einvoice.Dec_Pattern%>: <span style="color: red">(*)</span></label>
                    <div class="col-sm-8"><%=Html.DropDownList("InvPattern", Model.RegTempList, "--Mẫu số--", new { onchange = "get()", @class = "form-control required", title = "Chọn mẫu!" })%></div>
                </div>
                <div class="form-group">
                    <label class="col-sm-4"><%=Resources.Einvoice.Dec_InvCateName%>:</label>
                    <div class="col-sm-8"><%=Html.TextBox("InvCateName", "" ,new { @class="form-control", @readonly="readonly", maxlength="200" })%></div>
                </div>
                <div class="form-group">
                    <label class="col-sm-4"><%=Resources.Einvoice.Dec_LblPurpose%>: <span style="color: red">(*)</span></label>
                    <div class="col-sm-8"><%=Html.TextArea("Mucdich", "", new {maxlength="300", @class = "required form-control", title = "Mục đích!" })%></div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default pull-left" data-dismiss="modal"><i class="fa fa-backward"></i><%=Resources.Einvoice.BtnBack%></button>
                <button type="button" class="btn btn-primary" id="Save" onclick="btUpdate_click()"><i class="fa fa-save"></i><%=Resources.Einvoice.BtnSave%></button>
            </div>
        </div>
        <!-- /.modal-content -->
    </div>
    <!-- /.modal-dialog -->
</div>
<!-- /.modal -->
<script type="text/javascript">
    function checkDate() {
        var d1 = Date.parseExact($("#EffectiveDate").val(), "d/M/yyyy");
        var d2 = new Date.parseExact("<%=date%>", "d/M/yyyy");
        if (d1 < d2) {
            alert("<%=Resources.Message.Dec_MesEffectiveDateBigToday %>");
            var now = new Date().toString("dd/MM/yyyy");
            $("#EffectiveDate").val(now);
            return false
        }
        return true
    }
    var arrPattern = new Array();
    var c = "";
    var currentRow = null;
    $("#Create").click(function () {
        $("#InvPattern").val("");
        currentRow = null;
        if (arrPattern.length >= parseInt(c, 10)) 
            alertify.alert("Cần đăng ký mẫu hóa đơn!");
        else {
            var objSelect = document.getElementById("InvPattern");
            for (var i = 0; i < arrPattern.length; i++) 
                for (var j = 0; j < objSelect.options.length; j++) {
                    if (objSelect.options[j].value == arrPattern[i]) 
                        objSelect.remove(j);
                }
            bindDataToForm(currentRow);        
            $('#myModal').modal('show');
        }
    });
    var DecDatasource = [];
    function DecViewDatasource() {
        $("#dialog").dialog({
            autoOpen: false,
            height: 230,
            modal: true,
            width: 680
        });
        $("#grid tbody tr").remove();
        if (DecDatasource == null || DecDatasource == undefined) 
            return;
        for (var i = 0; i < DecDatasource.length; i++) {
            var jsonRow = DecDatasource[i];
            addRow("grid", jsonRow.InvPattern, jsonRow.InvCateName, jsonRow.Mucdich);
            arrPattern[i] = jsonRow.InvPattern
        }
        $("#grid tbody tr:odd").addClass("oddtr");
        $("#grid tbody tr").mouseover(function () {
            $(this).addClass("trover")
        }).mouseout(function () {
            $(this).removeClass("trover")
        });
        $("#grid tbody tr td .Delete").click(function () {
            if (confirm("Bạn có chắc chắn muốn xóa dòng này không?")) {
                var InvPattern = $(this).parent().parent().find("td").eq(0).html();
                if (InvPattern != null && InvPattern != undefined) {
                    for (var i = 0; i < DecDatasource.length; i++) {
                        if (DecDatasource[i].InvPattern == InvPattern) {
                            DecDatasource.splice(i, 1);
                            arrPattern.splice(i, 1)
                        }
                    }
                    $(this).parent().parent().remove()
                }
                AddPattern(InvPattern)
            }
            if (DecDatasource.length == 0) {
                $("#Article2").val("");
                nextFocus("Article2")
            }
        });
        $("#grid tbody tr td .Edit").click(function () {            
            if (DecDatasource == null || DecDatasource == undefined) 
                return [];
            var InvPattern = $(this).parent().parent().find("td").eq(0).html();
            var RetSearch = $.grep(DecDatasource, function (item, index) {
                return item.InvPattern == InvPattern
            });
            if (RetSearch.length > 0) 
                currentRow = RetSearch[0];
            else 
                currentRow = null;
            AddValue(InvPattern);
            bindDataToForm(currentRow);        
            $('#myModal').modal('show');
        })
    }
    function bindDataToForm(JsonObject) {
        if (JsonObject == null || JsonObject == undefined) {
            $("#InvCateName").val("");
            $("#Mucdich").val("")
        } else {
            $("#InvPattern").val(JsonObject.InvPattern);
            $("#InvCateName").val(JsonObject.InvCateName);
            $("#Mucdich").val(JsonObject.Mucdich)
        }
    }
    function addRow(GridID, nameregis, invname, mucdich) {
        var newrow = "<tr>";
        newrow += "<td>" + nameregis + "</td>";
        newrow += "<td>" + htmlEntities(invname) + "</td>";
        newrow += "<td>" + htmlEntities(mucdich) + "</td>";
        newrow += "<td style='text-align:center'><span class='glyphicon glyphicon-pencil Edit'></span></td>";
        newrow += "<td style='text-align:center'><span class='glyphicon glyphicon-trash Delete'></td>";
        newrow += "</tr>";
        $("#" + GridID + " tbody").append(newrow)
    }
    function htmlEntities(str) {
        return String(str).replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;")
    }

    function resetPattern() {
        currentRow = null;
        $("#InvPattern").val("")
    }
    function btUpdate_click() {
        if (!$("#Mucdich").val() || !$("#InvPattern").val()) {
            $("#DialogForm").valid()
        } else {
            if (sc()) {
                DecViewDatasource();
                $('#myModal').modal('hide');
                DelSelected()
            }
        }
        $("#Article2").val("ok");    
        nextFocus("Article2");
    }
    function sc() {
        if (currentRow == null) {
            currentRow = {
                InvCateName: $("#InvCateName").val(),
                InvPattern: $("#InvPattern").val(),
                Mucdich: $("#Mucdich").val()
            };
            DecDatasource.push(currentRow)
        } else {
            currentRow.InvPattern = $("#InvPattern").val();
            currentRow.InvCateName = $("#InvCateName").val();
            currentRow.Mucdich = $("#Mucdich").val()
        }
        return true
    }
    DecDatasource = <%=Model.DecDatasource%>;
    function get() {
        var jsd = "pattern=" + $("#InvPattern").val();
        if ($("#InvPattern").val() == "") 
            return;
        $.ajax({
            data: jsd,
            success: function (data) {
                $("#InvCateName").val(data.invcate)
            },
            type: "POST",
            url: "/Publish/getInv/"
        });
        pattern = $("#InvPattern").val()
    }
    function AddPattern(Pattern) {
        var objSelect = document.getElementById("InvPattern");
        for (var i = 0; i < objSelect.options.length; i++) {
            if (objSelect.options[i].value == Pattern) {
                objSelect.remove(i);
                break
            }
        }
        var optn = document.createElement("option");
        optn.text = Pattern;
        optn.value = Pattern;
        objSelect.add(optn)
    }
    function AddValue(Pattern) {
        var objSelect = document.getElementById("InvPattern");
        for (var i = 0; i < arrPattern.length; i++) 
            for (var j = 0; j < objSelect.options.length; j++) {
                if (objSelect.options[j].value == arrPattern[i]) 
                    objSelect.remove(j);
            }
        var optn = document.createElement("option");
        optn.text = Pattern;
        optn.value = Pattern;
        objSelect.add(optn)
    }
    function DelSelected() {
        var objSelect = document.getElementById("InvPattern");
        for (var i = 0; i < objSelect.options.length; i++) {
            if (objSelect.options[i].value == pattern) {
                objSelect.remove(i);
                break
            }
        }
    }
    var pattern = null;
    function btCancel_click() {
        $("#dialog").dialog("close");
        nextFocus("Article2")
    }
    
    $(document).ready(function () {
        $("form:first").validate();        
        $(".datepicker").datepicker({
            format: "dd/mm/yyyy",
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            showOn: "button",
            autoclose: true
        });
        $("#dialog").dialog({
            autoOpen: false,
            height: 280,
            modal: true,
            width: 680
        });
        DecViewDatasource();
        c = $("#count").val()
    });
    $("#MainForm").submit(function () {
        if (DecDatasource.toString() == "") {
            $("#Article2").val("");
            return false
        } else {
            $("#Article2").val("ok");
            $("#DecDatasource").val(JSON.stringify(DecDatasource))
        }
    });
</script>
