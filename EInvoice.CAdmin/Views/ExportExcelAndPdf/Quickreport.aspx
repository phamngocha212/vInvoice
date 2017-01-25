<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<EInvoice.CAdmin.Models.ReportsDetailModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Bảng kê chi tiết hóa đơn
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="/Content/js/Share.js"></script>
    <script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <%
        if (Model.Status == 0) Model.Status = -1;
    %>
    <form method="get" class="form-horizontal" action="/ExportExcelAndPdf/QuickreportPrince">
        <div class="row">
            <div class="col-xs-offset-2 col-xs-8">
                <div class="box box-danger">
                    <div class="box-header with-border">
                        <h4 class="box-title">
                            <i class="fa fa-paper-plane"></i>THỐNG KÊ CHI TIẾT HÓA ĐƠN
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
                                    <label class="col-sm-4 control-label">Trạng thái</label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Status",new[] 
                                          {
                                              new SelectListItem{Text="--Trạng thái--", Value="-1", Selected= (Model.Status== -1)},
                                              new SelectListItem{Text=Resources.Einvoice.MInv_TextCreateStatus, Value="0", Selected= (Model.Status== 0)},
                                              new SelectListItem{Text=Resources.Einvoice.MInv_TextPubInvStatus, Value="1", Selected=  (Model.Status == 1)},
                                              new SelectListItem{Text=Resources.Einvoice.MInv_TextPubInvTaxStatus, Value="2", Selected=  (Model.Status == 2)},
                                              new SelectListItem{Text=Resources.Einvoice.MInv_TextPubInvReplate, Value="3", Selected=  (Model.Status == 3)},
                                              new SelectListItem{Text=Resources.Einvoice.MInv_TextPubInvAdjust, Value="4", Selected=  (Model.Status == 4)},
                                              new SelectListItem{Text="Hóa đơn hủy", Value="5", Selected=  (Model.Status == 5)},
                                          }, new { style = "", onchange="sel()", @class="searchText form-control"})%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblToDate%></label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon">
                                                <i class="fa fa-calendar"></i>
                                            </div>
                                            <%=Html.TextBox("ToDate", Model.ToDate, new { style = "margin: 0 5px 0 0px", @class = "datepicker vietnameseDate form-control" })%>
                                        </div>

                                    </div>
                                </div>                                
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblSerial%></label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Serial", Model.lstSerial, "--Ký hiệu--",new { @name = "Serial", @class="form-control"})%>
                                    </div>
                                </div>
                               
                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblFromTo%></label>
                                    <div class="col-sm-8">

                                        <div class="input-group">
                                            <div class="input-group-addon">
                                                <i class="fa fa-calendar"></i>
                                            </div>

                                            <%=Html.TextBox("FromDate", Model.FromDate, new { style = " margin:0px 5px 0px 0px", @class = "datepicker vietnameseDate form-control"})%>
                                        </div>
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

        //Chi cho nhap so
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

    </script>
</asp:Content>

