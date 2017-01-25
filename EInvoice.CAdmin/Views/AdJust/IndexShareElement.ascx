<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EInvoice.CAdmin.Models.AdjustInvoiceModel>" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>

<script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
<script src="/Content/js/BrowserDetectShare.js"></script>
<script src="/Content/js/jquery.PrintArea.js"></script>
<script src="/Content/js/main.js"></script>
<style>
    .form-group {
        display: block;
        overflow: hidden;
        margin-bottom: 10px;
    }
</style>
<%    
    SelectList opattern = Model.lstPattern;
    SelectList oSerial = Model.lstSerial;
%>
<div class="row">
    <div class="col-xs-offset-2 col-xs-8">
        <div class="box box-primary">

            <div class="box-body">
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label class="col-sm-4 control-label">
                                <%=Resources.Einvoice.MInv_LblPattern%>:
                            </label>
                            <div class="col-sm-8">
                                <%=Html.DropDownList("Pattern", opattern, new { @name = "Pattern", onchange = "getSerial()", @class="form-control" })%>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-sm-4">
                                <%=Resources.Einvoice.MInv_LblFromTo%>:
                            </label>
                            <div class="col-sm-8">
                                <div class="input-group">
                                    <div class="input-group-addon"><i class="fa fa-calendar"></i></div>
                                    <%=Html.TextBox("FromDate", Model.FromDate, new {@placeholder="__/__/____", @class = "form-control datepicker" })%>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-4">
                                Tên khách hàng
                            </label>
                            <div class="col-sm-8">
                                <%=Html.TextBox("nameCus", Model.nameCus, new {maxlength="200", @class="form-control searchText" })%>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-sm-4">
                                <%=Resources.Einvoice.MInv_LblTaxCode%>
                            </label>
                            <div class="col-sm-8">
                                <%=Html.TextBox("CodeTax", Model.CodeTax, new { maxlength="20", @class="form-control searchText" })%>
                            </div>
                        </div>


                    </div>
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label class="col-sm-4">
                                <%=Resources.Einvoice.MInv_LblSerial%>
                            </label>
                            <div class="col-sm-8">
                                <%=Html.DropDownList("Serial", oSerial,new { @name = "Serial",@class="form-control", @onchange="document.searchForm.submit();"})%>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-4">
                                <%=Resources.Einvoice.MInv_LblToDate%>:
                            </label>
                            <div class="col-sm-8">
                                <div class="input-group">
                                    <div class="input-group-addon"><i class="fa fa-calendar"></i></div>
                                    <%=Html.TextBox("ToDate", Model.ToDate, new {@placeholder="__/__/____", @class = "form-control datepicker" })%>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-4">
                                Số hóa đơn:
                            </label>
                            <div class="col-sm-8">
                                <%=Html.TextBox("InvNo", Model.InvNo, new { @class="form-control", maxlength = "7", onkeypress = "return keypress(event);" })%>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="col-sm-4 cont">
                                <%=Resources.Einvoice.MInv_LblCusCode%>:
                            </label>
                            <div class="col-sm-8">
                                <%=Html.TextBox("code", Model.code, new {maxlength="20", @class="searchText form-control" })%>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
            <div class="box-footer">
                <div class="col-xs-12">
                    <button class="btn-sm btn btn-primary element-center" type="submit"><i class="fa fa-search"></i>&nbsp;&nbsp;Tìm kiếm</button>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="row">
    <div class="col-xs-12">
        <div class="box box-danger">
            <div class="box-header with-border">
                <div class="box-title">
                    <i class="fa fa-file-text-o"></i>&nbsp;&nbsp;Danh sách hóa đơn thay thế
                </div>
            </div>

            <div class="box-body no-padding table-responsive">
                <table class="table table-bordered table-hover">
                    <thead>
                        <tr>
                            <th rowspan="2">
                                <%=Resources.Einvoice.lblNo%>
                            </th>
                            <th colspan="5" style="color: Red">
                                <%=ViewData["Title1"]%>
                            </th>
                            <th colspan="4" style="color: Blue">
                                <%=ViewData["Title2"]%>
                            </th>
                            <th rowspan="2">Văn bản
                            </th>
                        </tr>
                        <tr>
                            <th>
                                <%=Resources.Einvoice.AdjReInv_LblPattern %>
                            </th>
                            <th>
                                <%=Resources.Einvoice.AdjReInv_LblSerial%>
                            </th>
                            <th>
                                <%=Resources.Einvoice.Inv_LblNo %>
                            </th>
                            <th>
                                <%=Resources.Einvoice.AdjReInv_LblDetail%>
                            </th>
                            <th>Ghi chú
                            </th>
                            <th>
                                <%=Resources.Einvoice.AdjReInv_LblPattern %>
                            </th>
                            <th>
                                <%=Resources.Einvoice.AdjReInv_LblSerial%>
                            </th>
                            <th>
                                <%=Resources.Einvoice.Inv_LblNo %>
                            </th>
                            <th>
                                <%=Resources.Einvoice.AdjReInv_LblDetail%>
                            </th>
                            <th>Ghi chú
                            </th>
                        </tr>
                    </thead>

                    <tbody>
                        <%
                            int i = Model.PageListAdjustSearch.PageIndex * Model.PageListAdjustSearch.PageSize;
                            foreach (var item in Model.PageListAdjustSearch)
                            {
                                i++;
                        %>
                        <tr>
                            <td style="text-align: right">
                                <%=i %>
                            </td>
                            <td style="text-align: center">
                                <%=Html.Encode(item.OldPattern)%>
                            </td>
                            <td style="text-align: center">
                                <%=Html.Encode(item.OldSerial)%>
                            </td>
                            <td style="text-align: right">
                                <%=item.OldNo.ToString("0000000")%>
                            </td>
                            <td style="text-align: center">
                                <a href="#"><i class="fa fa-eye" onclick="viewInv('<%=item.InvId%>','<%=Html.Encode(item.OldPattern)%>')" title="xem chi tiết"></i></a>
                            </td>
                            <td style="text-align: center">
                                <a href="/EInvoice/WriteNote?id=<%=Html.Encode(item.InvId) %>&Pattern=<%=Html.Encode(item.OldPattern) %>&TypeView=<%=Html.Encode(ViewData["TypeView"])%>"
                                    title="WriteNote">
                                    <img alt="" src="/Content/Images/Note.png" /></a>
                                <%-- <%=string.IsNullOrEmpty(objInvHasReplace.Note)?"":objInvHasReplace.Note%>--%>
                            </td>
                            <td style="text-align: center">
                                <%=Html.Encode(item.NewPattern)%>
                            </td>
                            <td style="text-align: center">
                                <%=Html.Encode(item.NewSerial)%>
                            </td>
                            <td style="text-align: right">
                                <%=item.NewNo.ToString("0000000")%>
                            </td>
                            <td style="border-right: 1px solid #ccc; text-align: center">
                                <a href="#"><i class="fa fa-eye" onclick="viewInv('<%=item.AdjustInvId%>','<%=Html.Encode(item.NewPattern)%>')" title="xem chi tiết"></i></a>
                            </td>
                            <td style="text-align: center">
                                <a href="/EInvoice/WriteNote?id=<%=Html.Encode(item.AdjustInvId)%>&Pattern=<%=Html.Encode(item.NewPattern)%>&TypeView=<%=Html.Encode(ViewData["TypeView"])%>"
                                    title="WriteNote">
                                    <img alt="" src="/Content/Images/Note.png" /></a>
                                <%--<%=string.IsNullOrEmpty(objInvReplace.Note)?"":objInvReplace.Note%>--%>
                            </td>
                            <%
                                if (string.IsNullOrEmpty(item.Attachefile))
                                {%>
                            <td style="text-align: center"><i class="fa fa-lock"></i>
                            </td>
                            <%}
                                else
                                { %>
                            <td style="border-right: 1px solid #ccc; text-align: center">
                                <a href="/AdJust/DownloadRecordsCancel?id=<%=item.id %>" title="Download">
                                    <i class="fa fa-download"></i></a>
                            </td>
                            <%} %>
                        </tr>
                        <%}%>
                    </tbody>

                </table>
            </div>
        </div>
    </div>
</div>

<script type="text/javascript">

    $(document).ready(function () {
        $(".datepicker").datepicker({
            showOn: "button",
            format: "dd/mm/yyyy",
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            autoclose: true
        });
        $('#InvNo').change(function () {
            var val = $('#InvNo').val();
            if (parseInt(val, 10).toString() == "NaN") {
                $('#InvNo').val('');
            }
            checkInvNo();
        });
        checkInvNo();
    });

    function checkInvNo() {
        if ($('#InvNo').val() && $('#InvNo').val() > 0) {
            $('.searchText').attr("disabled", "disabled");
            $(".datepicker").attr("disabled", "disabled");
            $(".datepicker").datepicker('disabled');
        }
        else {
            $(".searchText").removeAttr("disabled");
            $(".datepicker").datepicker('enable');
        }
        var str = $('#InvNo').val();
        while (str.length < 7) {
            str = '0' + str;
        }
        if (str == "0000000")
            return $('#InvNo').val("");
        else
            return $('#InvNo').val(str);
    }

    // lấy serial trong khi chọn pattern
    function getSerial() {
        var jsd = "Pattern=" + $("#Pattern").val();
        $.ajax({
            type: "POST",
            url: "/EInvoice/GetSerial/",
            data: jsd,
            success: function (data) {
                sl = document.getElementById("Serial");
                while (sl.firstChild) {
                    sl.removeChild(sl.firstChild);
                }
                if (data.pu.length > 0) {
                    for (i = 0; i < data.pu.length; i++) {
                        newOption = new Option(data.pu[i]);
                        document.getElementById("Serial").options.add(newOption);
                    }
                }
            }
        });
    }
    //Chỉ nhập số
    function keypress(e) {
        var keypressed = null;
        if (window.event) {
            keypressed = window.event.keyCode; //IE
        }
        else {
            keypressed = e.which; //NON-IE, Standard
        }
        if (keypressed < 48 || keypressed > 57) {
            if (keypressed == 8 || keypressed == 127) {
                return;
            }
            return false;
        }
    }
</script>
