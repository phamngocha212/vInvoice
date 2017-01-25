<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<EInvoiceIndexModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<%@ Import Namespace="EInvoice.Core" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Hủy hóa đơn
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script src="/Content/js/BrowserDetectShare.js" type="text/javascript"></script>
    <script src="/Content/js/main.js" type="text/javascript"></script>
    <div class="row">
        <div class="col-xs-8 col-xs-offset-2">
            <div class="box box-primary">
                <div class="box-header  with-border">
                    <h4 class="box-title">
                        <i class="fa fa-file-o"></i>QUẢN LÝ HỦY HÓA ĐƠN <small>(KHÔNG THAY THẾ, SỬA ĐỔI)</small>
                    </h4>

                </div>
                <%
                    SelectList opattern = new SelectList((IList<string>)Model.lstpattern);
                    SelectList oSerial = new SelectList((IList<string>)Model.lstserial);
                %>
                <form id="Searchform" method="post" class='form-horizontal' action="/InvoiceCancel/CancelInvNotReIndex">
                    <div class="box-body">
                        <div class="row">
                            <div class="col-xs-6">
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.MInv_LblPattern%>:</label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Pattern", opattern, new { @name = "Pattern", @class="form-control" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.MInv_LblStatus%>:</label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Status",new[] 
              {
                  new SelectListItem{Text="Chọn", Value="-1", Selected= (Model.Status== -1)},
                  new SelectListItem{Text=Resources.Einvoice.MInv_TextPubInvStatus, Value="1", Selected=  (Model.Status == 1)},
                  new SelectListItem{Text=Resources.Einvoice.MInv_TextPubInvTaxStatus, Value="2", Selected=  (Model.Status == 2)},
                  new SelectListItem{Text=Resources.Einvoice.MInv_TextPubInvReplate, Value="3", Selected=  (Model.Status == 3)},
                  new SelectListItem{Text=Resources.Einvoice.MInv_TextPubInvAdjust, Value="4", Selected=  (Model.Status == 4)},
                    new SelectListItem{Text="Hóa đơn đã xóa", Value="5", Selected=  (Model.Status == 5)},
              }, new { onchange="sel()" , @class = "searchText form-control"})%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.MInv_LblFromTo%>:</label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon"><i class="fa fa-calendar"></i></div>
                                            <%=Html.TextBox("FromDate", Model.FromDate, new { style = "margin:0px 5px 0px 0px",@placeholder="__/__/____", @class = "datepicker  form-control" })%>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.MInv_LblCusName%></label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("nameCus", Model.nameCus, new {maxlength="200", @class="searchText form-control" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.MInv_LblCusCode%>:</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("code", Model.code, new {maxlength="50", @class="searchText form-control" })%>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xs-6">
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.MInv_LblSerial%></label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Serial", oSerial,new { @name = "Serial", @class="form-control"})%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4">Số hóa đơn:</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("InvNo",Model.InvNo, new { @class="form-control", MaxLength = "7", onkeypress = "return keypress(event);" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.MInv_LblToDate%>:</label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon"><i class="fa fa-calendar"></i></div>
                                            <%=Html.TextBox("ToDate", Model.ToDate, new { style = "margin: 0 5px 0 0px", @placeholder="__/__/____", @class = "datepicker form-control" })%>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.MInv_LblTaxCode%> </label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("CodeTax", Model.CodeTax, new { maxlength="15", @class="searchText form-control" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4">Kiểu hóa đơn:</label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("typeInvoice", new[]
                                {
                                    new SelectListItem{Text="Select All", Value="-1", Selected=(Model.typeInvoice == -1)},
                                    new SelectListItem{Text="Hóa đơn thông thường", Value="0", Selected=(Model.typeInvoice == 0)},
                                    new SelectListItem{Text="Hóa đơn thay thế", Value="1", Selected=(Model.typeInvoice == 1)},
                                    new SelectListItem{Text="Hóa đơn điều chỉnh tăng", Value="2", Selected=(Model.typeInvoice ==2)},
                                    new SelectListItem{Text="Hóa đơn điều chỉnh giảm", Value="3", Selected=(Model.typeInvoice == 3)},
                                    new SelectListItem{Text="Hóa đơn điều chỉnh thông tin", Value="4", Selected=(Model.typeInvoice == 4)},
                                },  new { @class = "required searchText form-control", title = "Chọn kiểu hóa đơn!" })%>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="box-footer">
                        <button class="btn-sm btn btn-primary element-center" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button>

                    </div>
                </form>
            </div>

        </div>
    </div>
    <div class="row">
        <div class="col-xs-12">
            <div class="box box-danger">
                <div class="box-body table-responsive">

                    <form id="danhsachHD" method="post" action="/InvoiceCancel/CancelInvApprove">
                        <table class="table table-bordered table-hover">
                            <thead>
                                <tr>
                                    <th width="20px">STT
                                    </th>
                                    <th width="100px"><%=Resources.Einvoice.MInv_LblPattern%>
                                    </th>
                                    <th width="80px"><%=Resources.Einvoice.MInv_LblSerial%>
                                    </th>
                                    <th width="50px">Số
                                    </th>
                                    <th><%=Resources.Einvoice.MInv_LblCusName%>
                                    </th>
                                    <th width="130px">Mã khách hàng</th>
                                    <th width="100px">Người PH
                                    </th>
                                    <th width="80px">Ngày HD
                                    </th>
                                    <th width="110px">Trạng thái
                                    </th>
                                    <th width="80px"><%=Resources.Einvoice.LblDetail%>
                                    </th>
                                    <th width="20px" style="border-right-color: #EEE">Chọn</th>
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
                                        <% string a = "" + item.No.ToString();
                                           while (a.Length < 7)
                                           {
                                               a = '0' + a;
                                           }%>
                                        <%=a %>
                                    </td>
                                    <%} %>
                                    <td>
                                        <%=Html.Encode(item.CusName)%>
                                    </td>
                                    <td><%=item.CusCode==null?"": Html.Encode(item.CusCode) %></td>
                                    <td>
                                        <%=Html.Encode(item.PublishBy) %>
                                    </td>
                                    <td align="center">
                                        <%=item.Status==InvoiceStatus.NewInv? "": String.Format("{0:dd/MM/yyyy}", item.PublishDate)%>
                                    </td>
                                    <%if (item.Status == InvoiceStatus.ReplacedInv || item.Status == InvoiceStatus.CanceledInv)
                                      {%>
                                    <td align="center">Đã hủy hóa đơn  
                                    </td>
                                    <%}
                                      else
                                      { %>
                                    <td align="center">Hóa đơn phát hành
                                    </td>
                                    <%} %>
                                    <%if (item.Status == InvoiceStatus.NewInv)
                                      {%>
                                    <td align="center">
                                        <a href="#" onclick="ajxCall('<%=item.id%>','<%=item.Pattern%>')"><i class="fa fa-eye"></i></a>

                                    </td>
                                    <%}
                                      else
                                      {%>
                                    <td align="center">
                                        <a href="#" onclick="ajxCall('<%=item.id%>','<%=item.Pattern%>')"><i class="fa fa-eye"></i></a>


                                    </td>
                                    <%} %>
                                    <td align="center" id="test">
                                        <input name="cbeinv" id="cbeinv" type="checkbox" <%=(item.Status!= InvoiceStatus.NewInv&&item.Status!=InvoiceStatus.ReplacedInv&&item.Status!=InvoiceStatus.CanceledInv?"":"disabled='disabled'")%>
                                            value="<%=item.id%>" />
                                    </td>
                                </tr>
                                <% i++;
                                    }%>
                            </tbody>
                            <%=Html.Hidden("hdPattern") %>
                            <%=Html.Hidden("Serial")%>
                        </table>
                        <div class="pager">
                            <div class="page-select">
                                <%=Html.DropDownList("Pagesize", pages, new { onchange = "ChangePage()",@style="width:45px" })%>
                            </div>
                            <div class="page-a">
                                <%=Html.Pager(Model.PageListINV.PageSize, Model.PageListINV.PageIndex + 1, Model.PageListINV.TotalItemCount, new
            {
                action = "CancelInvNotReIndex",
                controller = "InvoiceCancel",
                nameCus= Model.nameCus,
                CodeTax=Model.CodeTax,
                code=Model.code,
                Pattern = Model.Pattern,
                Status = Model.Status,                
                FromDate = Model.FromDate,
                ToDate = Model.ToDate,
                Serial = Model.Serial,
                InvNo = Model.InvNo,
                typeInvoice=Model.typeInvoice,
                Pagesize = Model.PageListINV.PageSize               
            })%>
                            </div>
                        </div>
                    </form>
                    <button class="btn btn-sm btn-danger element-right" type="button" onclick="CancelInv_click();"><i class="fa fa-close"></i>Hủy hóa đơn</button>
                </div>
            </div>
        </div>
    </div>

    <script language="javascript" type="text/javascript">
        //chon id cua hóa đơn để phát hành
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
        //Thêm vào phân trang
        function ChangePage() {
            var pattern = $('#Pattern').val();
            var status = $('#Status').val();
            var TypeInvoice = $('#typeInvoice').val();
            var serial = $('#Serial').val();
            var code = $("#code").val();
            var CodeTax = $("CodeTax").val();
            document.location = "/InvoiceCancel/CancelInvNotReIndex?Pattern=" + pattern + "&Status=" + status + "&Serial=" + serial + "&InvNo=" + $("#InvNo").val() + "&FromDate=" + $("#FromDate").val() + "&ToDate=" + $("#ToDate").val() + "&code=" + code + "&nameCus=" + $("#nameCus").val() + "&typeInvoice=" + TypeInvoice + "&Pagesize=" + document.getElementById("Pagesize").options[document.getElementById("Pagesize").selectedIndex].text;
        }
        $(document).ready(function () {
            $('input[type=text]:first').focus();
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
                    url: "/EInvoice/GetSerial/",
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
        //Phát hành hóa đơn
        function CancelInv_click() {
            var names = [];
            $('#test input:checked').each(function () {
                names.push(this.value);
            });

            if ($("input:checkbox:checked").length > 0 && $('#Pattern').val() != "") {
                alertify.confirm("Bạn có muốn hủy hóa đơn không?", function (e) {
                    if (e) {
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
                        $('#Serial').val($('#Serial').val());
                        document.forms["danhsachHD"].submit();
                    }
                    else return;
                });
            }
            else {
                alertify.alert("<%=Resources.Message.MInv_MesChooseInvToPublish%>");
            }
        }
        function htmlEncode(value) {
            return $('<div/>').text(value).html();
        }
        function htmlDecode(value) {
            return $('<div/>').html(value).text();
        }

        //truong hop khi chon trang thai hoa don
        function sel() {
            var status = $('#Status').val();
            if (status != "-1") {
                $("#InvNo").val("");
                $("#InvNo").attr("disabled", "disabled");
            }
            else {
                $("#InvNo").removeAttr("disabled");
            }
        }
    </script>
</asp:Content>

