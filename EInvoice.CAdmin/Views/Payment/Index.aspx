<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PaymentModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<%@ Import Namespace="EInvoice.Core" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thanh toán hóa đơn
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script src="/Content/js/BrowserDetectShare.js"></script>
    <script src="/Content/js/main.js"></script>
    <script type="text/javascript">
        function getPattern() {
            if ($('#Pattern').val() != "")
                $('#hdPattern').val($('#Pattern').val());
        }
        var arr = new Array();
        function YesOrNo() {
            var radios = $('#count').val();
            var j = 0;
            for (var i = 0; i < radios; i++) {

                if (radios[i].checked == true) {
                    arr[j] = radios[i].value;
                    j++;
                }
            }
        }
        
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
    </script>      
    <form id="Searchform" method="post" class="form-horizontal">
    <div class="row">
        <div class="col-xs-8 col-xs-offset-2">
            <div class="box box-primary">
                <%
                    SelectList opattern = Model.PatternList;
                    SelectList oSerial = Model.SerialList;
                    int PaymentStatus = Model.PaymentStatus;    
                %>          
                    <div class="box-body">
                        <div class="row">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.Pay_LblPattern%></label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Pattern", opattern, new { @name = "Pattern", @class="form-control" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4">Trạng thái</label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("PaymentStatus", new[]
              {
                  new SelectListItem{Text = "--Trạng thái--", Value = "-1", Selected=(PaymentStatus==-1)},
                  new SelectListItem{Text=Resources.Einvoice.Pay_TxtNoPayStatus, Value="0", Selected=(PaymentStatus == 0)},
                  new SelectListItem{Text=Resources.Einvoice.Pay_TxtPayedStatus, Value="1", Selected=(PaymentStatus == 1)},
              },  new { onchange = "sel()", title = "Chọn tình trạng thanh toán!", @class="searchText form-control" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.Pay_LblFromDate%></label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon"><i class="fa fa-calendar"></i></div>
                                            <%=Html.TextBox("FromDate", Model.FromDate == null ? "" : Model.FromDate.ToString(), new { style = "margin:0px 5px 0px 0px", @placeholder="__/__/____",  @class = "datepicker  form-control" })%>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.MInv_LblCusName%></label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("nameCus", Model.nameCus, new { @Style = "", @class="searchText form-control", maxlength="200" })%>
                                    </div>
                                  
                                </div>

                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.Pay_LblSerial%></label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Serial", oSerial, "--Ký hiệu--", new { @name = "Serial", @Style = "", @class="form-control", title = "Chọn ký hiệu" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.ReSigCus_LblInvNo%></label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("InvNo", "", new { @class="form-control", MaxLength = "7", onkeypress = "return keypress(event);" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.Pay_LblToDate%></label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon"><i class="fa fa-calendar"></i></div>
                                            <%=Html.TextBox("ToDate", Model.ToDate == null ? "" : Model.ToDate.ToString(), new { style = "margin: 0 5px 0 0px", @placeholder="__/__/____",  @class = "datepicker form-control"})%>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.MInv_LblCusCode%></label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("code", Model.code, new {@class="form-control searchText", maxlength="20"  })%>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                    <div class="box-footer">
                        <button class="btn-sm btn btn-primary element-center" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button>
                    </div>
              
            </div>
        </div>
    </div>  
    </form>
    <div class="box box-danger">
        <form id="danhsachHD" method="post" action="/Payment/PaymentInvoice" onsubmit="getPattern();">
            <div class="box-header">
                <h4 class="box-title">
                    <i class="fa fa-paperclip"></i>
                    Danh sách thanh toán
                </h4>
            </div>
            <div class="box-body no-padding table-responsive">

                <table class="table table-bordered table-hover">
                    <thead>
                        <tr>
                            <th style="width: 20px">
                                <%=Resources.Einvoice.lblNo%>
                            </th>
                            <th style="width: 100px">
                                <%=Resources.Einvoice.Pay_LblPattern%>
                            </th>
                            <th style="width: 80px">
                                <%=Resources.Einvoice.Pay_LblSerial%>
                            </th>
                            <th style="width: 80px">
                                <%=Resources.Einvoice.Inv_LblNo%>
                            </th>
                            <th>
                                <%=Resources.Einvoice.Pay_LblCusName%>
                            </th>
                            <th style="width: 120px">Ngày hóa đơn
                            </th>
                            <th style="width: 100px">Thanh toán
                            </th>
                            <th style="width: 100px">
                                <%=Resources.Einvoice.LblDetail%>
                            </th>
                            <th style="width: 20px">
                                <%: Html.CheckBox("ckbAll") %>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <%    
                            IList<IInvoice> lstInv = Model.PageListINV.ToList();
                            int i = Model.PageListINV.PageIndex * Model.PageListINV.PageSize + 1;
                            List<string> lst = new List<string> { "10", "20", "30", "40", "50" };
                            SelectList pages = new SelectList(lst, Model.PageListINV.PageSize);
                            foreach (IInvoice item in lstInv)
                            { 
                        %>
                        <tr>
                            <td style="text-align: right">
                                <%=i%>
                            </td>
                            <td style="text-align: center">
                                <%=Html.Encode(item.Pattern)%>
                            </td>
                            <td style="text-align: center">
                                <%=Html.Encode(item.Serial)%>
                            </td>
                            <%if (item.Status == InvoiceStatus.NewInv)
                              { %>
                            <td style="text-align: center"></td>
                            <%}
                              else
                              { %>
                            <td style="text-align: right">
                                <%=item.No.ToString("0000000")%>
                            </td>
                            <%} %>
                            <td>
                                <%=Html.Encode(item.CusName)%>
                            </td>
                            <td style="text-align: center">
                                <%= String.Format("{0:dd/MM/yyyy}", item.ArisingDate)%>
                            </td>
                            <td style="text-align: center">
                                <img src="<%=item.PaymentStatus == Payment.Paid ? "/Content/Images/valid.png" : "/Content/Images/cross.gif" %>" title="<%=item.PaymentStatus == Payment.Paid ? "Đã thanh toán" :"Chưa thanh toán"%>" />
                            </td>
                            <td style="text-align: center">
                                <a href="#"><i class="fa fa-eye" onclick="viewInv('<%=item.id%>','<%=Html.Encode(item.Pattern)%>')" title="xem chi tiết"></i></a>
                            </td>
                            <%if (item.PaymentStatus == Payment.Unpaid)
                              { %>
                            <td style="text-align: center">
                                <input name="cbid" type="checkbox" value="<%=item.id%>" />
                            </td>
                            <%}
                              else
                              { %>
                            <td style="text-align: center">
                                <i class="fa fa-check"></i>                                
                            </td>
                            <%} %>
                        </tr>
                        <% i++;
                            }%>
                    </tbody>
                    <%=Html.Hidden("hdPattern") %>
                </table>
            </div>
            <div class="box-footer">
                <div class="pager">
                    <div class="page-select">
                        <%=Html.DropDownList("Pagesize", pages, new { onchange = "ChangePage()",@style="width:45px" })%>
                    </div>
                    <div class="page-a">
                        <%=Html.Pager(Model.PageListINV.PageSize, Model.PageListINV.PageIndex + 1, Model.PageListINV.TotalItemCount, new
            {
                action = "Index",
                controller = "Payment",
                Pattern =Model.Pattern,
                Serial = Model.Serial,
                PaymentStatus = Model.PaymentStatus,
                FromDate = Model.FromDate,
                ToDate = Model.ToDate,
                InvNo = Model.InvNo,
                Pagesize = Model.PageListINV.PageSize          
            })%>
                    </div>
                </div>
                <button class="btn btn-sm btn-primary element-right" type="button" onclick="Publish_click();"><i class="fa fa-check"></i>Đối soát hóa đơn thanh toán</button>
            </div>            
        </form>
    </div>
    <script type="text/javascript">
        function ChangePage() {
            //debugger;
            var pattern = $('#Pattern').val();
            var serial = $('#Serial').val();
            var paymentStatus = $('#PaymentStatus').val();
            document.location = "/Payment/Index?Pattern=" + pattern + "&Serial=" + serial + "&PaymentStatus=" + paymentStatus + "&InvNo=" + $("#InvNo").val() + "&FromDate=" + $("#FromDate").val() + "&ToDate=" + $("#ToDate").val() + "&Pagesize=" + document.getElementById("Pagesize").options[document.getElementById("Pagesize").selectedIndex].text;
        }

        $(document).ready(function () {
            // $('form:first').validate();            
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

            $('#ckbAll').change(function () {
                if ($(this).is(':checked')) {
                    $('.checker span').addClass('checked');
                    $('input[name=cbid]').attr('checked', true);
                } else {
                    $('input[name = cbid]').attr('checked', false);
                     $('.checker span').removeClass('checked');                                      
                }
            });

            $('#Pattern').change(function () {
                var jsd = "Pattern=" + $("#Pattern").val();
                $.ajax({
                    type: "POST",
                    url: "/Payment/GetSerialPayments/",
                    data: jsd,
                    success: function (data) {
                        sl = document.getElementById("Serial");
                        while (sl.firstChild) {
                            sl.removeChild(sl.firstChild);
                        }
                        newOpt = new Option("--Ký hiệu--", "");
                        document.getElementById("Serial").options.add(newOpt);
                        newOpt.selected = true;
                        if (data.pu.length > 0) {
                            for (i = 0; i < data.pu.length; i++) {
                                newOption = new Option(data.pu[i]);
                                document.getElementById("Serial").options.add(newOption);
                            }
                        }
                        $('form:first').submit();
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

        function Publish_click() {
            if ($("input:checkbox:checked").length > 0 && $('#pattern').val != "") {
                swal({
                    title: "<%=Resources.Message.Pay_MesConfirm%>",
                    text: "",
                    type: "warning", showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Chấp nhận",
                    cancelButtonText: "Hủy",
                    closeOnConfirm: true
                },
                    function () {
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
                        $('#hdPattern').val($('#Pattern').val());
                        document.forms["danhsachHD"].submit();
                    });

            } else
                sweetAlert("Thông báo", "<%=Resources.Message.Pay_MesSelectInv%>", "error");
        }

        function sel() {
            var PaymentStatus = $('#PaymentStatus').val();
            if ($('#PaymentStatus').val() != "") {
                $("#InvNo").val("");
                $("#InvNo").attr("disabled", "disabled");
            }
            else {
                $("#InvNo").removeAttr("disabled");
            }
        }

        function checkTextAreaMaxLength(textBox, e, length) {
            textBox.value = textBox.value.toUpperCase().replace(/([^0-9A-Z])/g, "");
            var mLen = textBox["MaxLength"];
            if (null == mLen)
                mLen = length;

            var maxLength = parseInt(mLen);
            if (!checkSpecialKeys(e)) {
                if (textBox.value.length > maxLength - 1) {
                    if (window.event)//IE
                        e.returnValue = false;
                    else//Firefox
                        e.preventDefault();
                }
            }
        }        
    </script>
</asp:Content>
