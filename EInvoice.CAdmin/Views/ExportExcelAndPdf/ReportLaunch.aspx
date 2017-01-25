<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<EInvoice.CAdmin.Models.ReportsLaunchModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Bảng kê hóa đơn hàng tháng
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="/Content/js/Share.js"></script>
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <form method="get" class="form-horizontal" action="/ExportExcelAndPdf/ReportLaunchPrint">
        <div class="row">
            <div class="col-xs-offset-2 col-xs-8">
                <div class="box box-danger">
                    <div class="box-header with-border">
                        <h4 class="box-title">
                            <i class="fa fa-paper-plane"></i>BẢNG KÊ TẠO LẬP VÀ PHÁT HÀNH HÓA ĐƠN
                        </h4>
                    </div>
                    <div class="box-body">
                        <div class="row">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblPattern%></label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Pattern", Model.lstPattern, new { @name = "Pattern", @Style = "" ,@class="form-control"})%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4 control-label">Số hóa đơn</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("InvNo", "", new { style = "",@class="form-control", MaxLength = "7", onkeypress = "return keypress(event);" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4 control-label">Người tạo</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("CreateBy",Model.CreateBy, new { maxlength="100", style = " margin:0px 5px 0px 0px",onchange="sel()", @class="searchText form-control" })%>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblSerial%></label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Serial", Model.lstSerial, "--Ký hiệu--",new { @name = "Serial", @Style = "",@class="form-control"})%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4 control-label">Ngày phát hành</label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon">
                                                <i class="fa fa-calendar"></i>
                                            </div>
                                            <%=Html.TextBox("PublishDate", Model.PublishDate, new { style = "margin: 0 5px 0 0px", @class = "datepicker form-control" })%>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4 control-label">Người phát hành</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("PublishBy", Model.PublishBy, new {maxlength="100", style = " margin: 0 5px 0 0px", onchange = "sel()", @class="form-control searchText" })%>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="box-footer">
                        <button class="btn-sm btn btn-primary element-center" type="button" onclick="test()"><i class="fa fa-search"></i>&nbsp;<%=Resources.Einvoice.BtnReportQuick%></button>
                    </div>
                </div>
            </div>
        </div>
    </form>
    <script type="text/javascript">       
        function test() {
            var FromDate = $("#FromDate").val();
            var ToDate = $("#ToDate").val();
            if (FromDate != "" && ToDate != "") {
                document.forms[0].submit()
            }
            if (FromDate != "" || ToDate != "") {
                if (FromDate != "" && ToDate == "") {
                    alert("<%=Resources.Message.ReUse_ReqTime%>");

                }
                if (FromDate == "" && ToDate != "") {
                    alert("<%=Resources.Message.ReUse_ReqTime%>");
                }
                return false;
            }
            else {
                document.forms[0].submit()
            }
        }
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

            $('#Pattern').change(function () {
                var jsd = "Pattern=" + $("#Pattern").val();
                $.ajax({
                    type: "POST",
                    url: "/CusSignStatus/GetSerial/",
                    data: jsd,
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
                        }
                    }
                });
            });
        });

        function checkInvNo() {
            if ($('#InvNo').val() && $('#InvNo').val() > 0) {
                $('.searchText').attr("disabled", "disabled");
                $(".datepicker").attr("disabled", "disabled");
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
        function sel() {
            var publishDate = $('#PublishDate').val();
            var CreateBy = $('#CreateBy').val();
            var PublishBy = $('#PublishBy').val();
            if (publishDate != "" || CreateBy != "" || PublishBy != "") {
                $("#InvNo").val("");
                $("#InvNo").attr("disabled", "disabled");
            }
            else {
                $("#InvNo").removeAttr("disabled");
            }
        }
    </script>
</asp:Content>
