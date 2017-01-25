<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CusSignIndexModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Trạng thái ký số hóa đơn của khách hàng
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script src="/Content/js/BrowserDetectShare.js"></script>
    <script src="/Content/js/jquery.PrintArea.js"></script>
    <script src="/Content/js/main.js"></script>
    <%
        int SignStatus = -1;
        if (Model.SignStatus != null)
            SignStatus = Convert.ToInt32(Model.SignStatus);
        int pageIndex = ViewData["PageIndex"] == null ? 1 : (int)ViewData["PageIndex"];  
    %>
    <form id="Form1" name="searchForm" method="post" class="form-horizontal" action="/EInvoice/Index">
        <div class="row">
            <div class="col-xs-offset-2 col-xs-8">
                <div class="box box-danger">
                    <div class="box-body">
                        <div class="row">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblPattern%></label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Pattern", Model.lstpattern, "---Mẫu số---", new { @name = "Pattern",@class="form-control"})%>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-4 control-label">Trạng thái ký</label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("SignStatus", new[]
              {
                  new SelectListItem{Text=Resources.Einvoice.ReSigCus_TxtNoSign, Value="0", Selected=(SignStatus == 0)},
                  new SelectListItem{Text=Resources.Einvoice.ReSigCus_TxtSigned, Value="1", Selected=(SignStatus == 1)},
              }, "Chọn", new { style = "width:190px", onchange = "sel()", title = "Chọn tình trạng ký!", @class="searchText"  })%>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblFromTo%></label>
                                    <div class="col-sm-8">

                                        <div class="input-group">
                                            <div class="input-group-addon">
                                                <i class="fa fa-calendar"></i>
                                            </div>

                                            <%=Html.TextBox("FromDate", Model.FromDate, new { style = " margin:0px 5px 0px 0px", @placeholder="__/__/____",  @class = "datepicker  form-control"})%>
                                        </div>
                                    </div>
                                </div>

                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblSerial%></label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Serial", Model.lstserial, "--Ký hiệu--",new { @name = "Serial", @Style = "",@class="form-control", @onchange="document.searchForm.submit();"})%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4 control-label">Số hóa đơn</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("InvNo",Model.InvNo, new { style = "", MaxLength = "7", onkeypress = "return keypress(event);" ,@class="form-control"})%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblToDate%>:</label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon">
                                                <i class="fa fa-calendar"></i>
                                            </div>
                                            <%=Html.TextBox("ToDate", Model.ToDate, new { style = "margin: 0 5px 0 0px", @class = "datepicker form-control", @placeholder="__/__/____"})%>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="box-footer">
                        <button class="btn-sm btn btn-primary element-center" type="submit"><i class="fa fa-search"></i>&nbsp;Tìm kiếm</button>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <div class="box box-danger">
        <div class="box-header">
            <i class="fa fa-paper-plane"></i><b>TÌNH TRẠNG KÝ SỐ HÓA ĐƠN CỦA KHÁCH HÀNG</b>
        </div>

        <div class="box-body no-padding table-responsive">
            <table class="table table-bordered table-hover">
                <thead>
                    <tr>
                        <th width="20px">
                            <%=Resources.Einvoice.lblNo %>
                        </th>
                        <th width="100px">
                            <%=Resources.Einvoice.ReSigCus_LblPattern %>
                        </th>
                        <th width="80px">
                            <%=Resources.Einvoice.ReSigCus_LblSerial %>
                        </th>
                        <th width="50px">
                            <%=Resources.Einvoice.ReSigCus_LblInvNo %>
                        </th>
                        <th width="130px">
                            <%=Resources.Einvoice.ReSigCus_LblCusName %>
                        </th>
                        <th width="60px">
                            <%=Resources.Einvoice.ReSigCus_LblPubDate %>
                        </th>
                        <th width="80px">
                            Trạng thái
                        </th>
                        <th width="60px">
                            <%=Resources.Einvoice.ReSigCus_LblDetail %>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <%    
                    
                        IList<IInvoice> lstInv = Model.PageListCusSign.ToList();
                        int i = Model.PageListCusSign.PageIndex * Model.PageListCusSign.PageSize + 1;
                        List<string> lst = new List<string> { "10", "20", "30", "40", "50" };
                        SelectList pages = new SelectList(lst, Model.defautPagesize);
                        foreach (IInvoice item in lstInv)
                        { 
                    %>
                    <tr>
                        <td align="center">
                            <%=i%>
                        </td>
                        <td align="center">
                            <%=Html.Encode(item.Pattern)%>
                        </td>
                        <td align="center">
                            <%=Html.Encode(item.Serial)%>
                        </td>
                        <%if (item.Status == InvoiceStatus.NewInv)
                          { %>
                        <td align="center"></td>
                        <%}
                          else
                          { %>
                        <td>
                            <%=item.No.ToString("0000000") %>
                        </td>
                        <%} %>
                        <td>
                            <%=Html.Encode(item.CusName)%>
                        </td>
                        <td align="center">
                            <%= String.Format("{0:dd/MM/yyyy}", item.ArisingDate)%>
                        </td>
                        <%if (item.CusSignStatus == cusSignStatus.NoSignStatus || item.CusSignStatus == cusSignStatus.ViewNoSignStatus)
                          {%>
                        <td>Chưa ký</td>
                        <%}
                          else if (item.CusSignStatus == cusSignStatus.SignStatus)
                          {%>
                        <td>Đã ký</td>
                        <%}%>
                        <td style="text-align: center">
                            <a href="#"><span class="fa fa-eye" onclick="viewInv('<%=item.id%>','<%=Html.Encode(item.Pattern)%>');<%= item.Status == InvoiceStatus.CanceledInv || item.Status == InvoiceStatus.ReplacedInv ? "addImg()" : ""%>" title="xem chi tiết"></span></a>
                        </td>
                    </tr>
                    <% i++;
                        }%>
                </tbody>
                <%=Html.Hidden("hdPattern") %>
                <%=Html.Hidden("id","") %>
                <%=Html.Hidden("cusCode") %>
                <%=Html.Hidden("SignStatus")%>
            </table>
            <div class="box-footer clearfix">
                <div class="pager">
                    <div class="page-a">
                        <b>PageSize</b><%=Html.DropDownList("Pagesize", pages, new { onchange = "ChangePage()",@style="width:45px" })%>
                        <%=Html.Pager(Model.PageListCusSign.PageSize, Model.PageListCusSign.PageIndex + 1, Model.PageListCusSign.TotalItemCount, new
            {
                action = "index",
                controller = "CusSignStatus",
                Pattern = Model.Pattern,
                SignStatus = Model.SignStatus,           
                FromDate = Model.FromDate,
                ToDate = Model.ToDate,
                Serial = Model.Serial,
                InvNo = Model.InvNo,
                Pagesize = Model.defautPagesize           
            })%>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript" language="javascript">
        var flagImg = false;
        $(document).ajaxComplete(function () {
            if (flagImg) {
                $('.VATTEMP').prepend('<img id="imgCancel" src="/Content/Images/mark.png" />');
                $('#imgCancel').css({ "position": "absolute", "z-index": "3", "width": "790px", "height": "800px", "margin-top": "100px" });
                flagImg = false;
            }
        });

        function addImg() {
            flagImg = true;
        }
        function getPattern() {
            if ($('#Pattern').val() != "")
                $('#hdPattern').val($('#Pattern').val());
        }
        //phan trang
        function ChangePage() {
            var pattern = $('#Pattern').val();
            var SignStatus = $('#SignStatus').val();
            document.location = "/CusSignStatus/Index?Pattern=" + pattern + "&SignStatus=" + SignStatus + "&Serial=" + $("#Serial").val() + "&InvNo=" + $("#InvNo").val() + "&FromDate=" + $("#FromDate").val() + "&ToDate=" + $("#ToDate").val() + "&Pagesize=" + document.getElementById("Pagesize").options[document.getElementById("Pagesize").selectedIndex].text + "&page=" + "<%:pageIndex %>";
            }
            //dinh dang tieng viet cho datepicker        
            //dinh dang nhap bang tieng viet        
            $(document).ready(function () {
                $(".datepicker").datepicker({
                    format: "dd/mm/yyyy",
                    showOn: "button",
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
                var str = $('#InvNo').val();
                if ($('#InvNo').val() && $('#InvNo').val() > 0) {
                    $('.searchText').attr("disabled", "disabled");
                    $(".datepicker").attr("disabled", "disabled");
                }
                else {
                    $(".searchText").removeAttr("disabled");
                    $(".datepicker").datepicker('enable');
                }

                while (str.length < 7) {
                    str = '0' + str;
                }
                if (str == "0000000")
                    return $('#InvNo').val("");
                else
                    return $('#InvNo').val(str);
            }

            function sel() {
                var SignStatus = $('#SignStatus').val();
                if ($('#SignStatus').val() != "") {
                    $("#InvNo").val("");
                    $("#InvNo").attr("disabled", "disabled");
                }
                else {
                    $("#InvNo").removeAttr("disabled");
                }
            }
            //Chi cho nhap so khi nhap control so hoa don
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

            //hien thi danh sach cac serial khi chon pattern
            function getSerial() {
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
            }
    </script>
</asp:Content>
