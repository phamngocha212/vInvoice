<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<LaunchModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Phát hành lô hóa đơn
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <div class="row">
        <div class="col-xs-8 col-xs-offset-2">
            <div class="box box-primary">
                <div class="box-header with-border">
                    <h4 class="box-title text-center"><i class="fa fa-table"></i><%=Resources.Einvoice.PLInv_lblTitle%></h4>
                </div>
                <form class="form-horizontal">
                    <div class="box-body">
                        <div class="alert alert-danger text-center">
                            <%=Resources.Einvoice.PLInv_NotePleaseWaitToPublish%>
                        </div>
                        <div class="row">
                            <div class="col-xs-4 col-xs-offset-2">
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.PLInv_LblPattern%>:<span style="color: red">(*)</span></label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Pattern",Model.Listpattern, "--Mẫu số--", new {onchange = "GetDataSerial()", @class = "required form-control", title = "Mẫu hóa đơn" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4">Từ Ngày:<span style="color: red">(*)</label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon"><i class="fa fa-calendar"></i></div>
                                            <%=Html.TextBox("FromDate",Model.FromDate, new { @style = " z-index:1000;", @placeholder="__/__/____",  @class = "datepicker  form-control"})%>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xs-4">
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.PLInv_LblSerial%>:<span style="color: red">(*)</span></label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Serial",Model.Listserial, "--Ký hiệu--", new { @name = "Serial", @class="form-control"})%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4">Đến ngày:<span style="color: red">(*)</span></label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon"><i class="fa fa-calendar"></i></div>
                                            <%=Html.TextBox("ToDate",Model.ToDate, new { @style = " z-index:1000;", @placeholder="__/__/____",  @class = "datepicker  form-control"})%>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="box-footer">
                        <button class="btn btn-success element-center" type="button" onclick="LaunchInvoice();"><i class="fa fa-send"></i>PHÁT HÀNH LÔ HÓA ĐƠN</button>
                    </div>
                </form>
            </div>
        </div>
    </div>

    <script language="javascript" type="text/javascript">

        //phát hành theo lô hoas đơn
        function LaunchInvoice() {
            var pattern = $("#Pattern").val();
            var serial = $("#Serial").val();
            var fromDate = $("#FromDate").val();
            var toDate = $("#ToDate").val();
            if (pattern == "" || serial == "" || fromDate == "" || toDate == "") {
                alertify.alert("<%=Resources.Message.MInv_MesReqInformation%>");
                return false;
            }
            $.blockUI({
                css: {
                    border: 'none',
                    padding: '15px',
                    backgroundColor: '#eee',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: .5,
                    color: '#fff'
                },
                message: '<h1>Xin vui lòng đợi.</h1>', fadeIn: 0,
                fadeOut: 10, timeout: 240000
            });
            var jsondata = "FromDate=" + fromDate + "&ToDate=" + toDate + "&pattern=" + pattern + "&serial=" + serial;
            $.ajax({
                type: "POST",
                url: "/EInvoice/LaunchCollectInvoice/",
                data: jsondata,
                success: function (data) {
                    if (data == "True") {
                        $.unblockUI();
                        FanxiMessage("<%=Resources.Message.PInv_Msuccess%>");
                        document.location = '/EInvoice/Index';
                    }
                    else {
                        $.unblockUI();
                        location.reload();
                    }
                }
            });
        }

        //lấy ra danh sách serial khi chọn pattern
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
                        newOpt = new Option("--Ký hiệu--", "");
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
        function htmlEncode(value) {
            return $('<div/>').text(value).html();
        }
        function htmlDecode(value) {
            return $('<div/>').html(value).text();
        }        

        $(document).ready(function () {
            $(".datepicker").datepicker({
                showOn: "button",
                format: "dd/mm/yyyy",                
                changeMonth: true,
                changeYear: true,
                autoclose: true
            });
            $('form:first').validate();
        });

    </script>
</asp:Content>
