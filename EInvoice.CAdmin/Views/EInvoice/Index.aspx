<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<EInvoiceIndexModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<%@ Import Namespace="EInvoice.Core" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Danh sách hóa đơn
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script src="/Content/js/BrowserDetectShare.js"></script>
    <script src="/Content/js/jquery.PrintArea.js"></script>
    <script src="/Content/js/main.js"></script>
    <script src="/Content/js/hwcrypto.js"></script>

    <%            
        SelectList oSerial = new SelectList((IList<string>)Model.lstserial);
    %>
    <form id="Searchform" name="searchForm" method="post" class="form-horizontal" action="/EInvoice/Index">
        <div class="row">
            <div class="col-xs-offset-2 col-xs-8">
                <div class="box box-danger">
                    <div class="box-body">
                        <div class="row">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblPattern%></label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Pattern", new SelectList(Model.lstpattern, Model.Pattern), new { @name = "Pattern",@class="form-control"})%>
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

                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblCusName%></label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("nameCus", Model.nameCus, new { @Style = "", @maxlength="200" , @class="searchText form-control"})%>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-4 control-label">Mã khách hàng</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("code", Model.code, new { style = "", @maxlength="30", @class="searchText form-control" })%>
                                    </div>
                                </div>


                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblSerial%></label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Serial", oSerial, "--Ký hiệu--",new { @name = "Serial", @Style = "",@class="form-control", @onchange="document.searchForm.submit();"})%>
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
                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblTaxCode%> </label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("CodeTax", Model.CodeTax, new { @Style = "", @maxlength="16", @class="searchText form-control" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4 control-label">Kiểu hóa đơn:</label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("typeInvoice", new[]
                                {
                                    new SelectListItem{Text="--Tất cả--", Value="-1", Selected=(Model.typeInvoice == -1)},
                                    new SelectListItem{Text="Hóa đơn thông thường", Value="0", Selected=(Model.typeInvoice == 0)},
                                    new SelectListItem{Text="Hóa đơn thay thế", Value="1", Selected=(Model.typeInvoice == 1)},
                                    new SelectListItem{Text="Hóa đơn điều chỉnh tăng", Value="2", Selected=(Model.typeInvoice ==2)},
                                    new SelectListItem{Text="Hóa đơn điều chỉnh giảm", Value="3", Selected=(Model.typeInvoice == 3)},
                                    new SelectListItem{Text="Hóa đơn điều chỉnh thông tin", Value="4", Selected=(Model.typeInvoice == 4)},
                                },  new { style = "", @class = "required searchText form-control", title = "Chọn kiểu hóa đơn!"})%>
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
            <i class="fa fa-paper-plane"></i><b>DANH SÁCH HÓA ĐƠN</b>
            <a id="crtInvoice" class="btn btn-primary btn-sm"><i class="fa fa-plus"></i>Tạo mới</a>
        </div>
        <form id="danhsachHD" method="post" action="/EInvoice/LaunchChoice">
            <div class="box-body no-padding table-responsive">
                <table class="table table-bordered table-hover">
                    <thead>
                        <tr>
                            <th style="width: 20px">STT
                            </th>
                            <th style="width: 80px"><%=Resources.Einvoice.MInv_LblPattern%>
                            </th>
                            <th style="width: 70px"><%=Resources.Einvoice.MInv_LblSerial%>
                            </th>
                            <th style="width: 50px">Số
                            </th>
                            <th><%=Resources.Einvoice.MInv_LblCusName%>
                            </th>
                            <th style="width: 80px">Mã KH</th>
                            <th style="width: 100px">KH xem HĐ</th>
                            <th style="width: 90px">Ngày HĐ</th>
                            <th style="width: 100px">Trạng thái</th>
                            <th style="width: 100px">Người PH</th>
                            <th style="width: 100px">Thanh toán</th>
                            <th style="width: 40px">Xem
                            </th>
                            <th style="width: 20px"><%=Resources.Einvoice.LblEdit%>
                            </th>
                            <th style="width: 20px"><%=Resources.Einvoice.LblDelete%>
                            </th>
                            <th style="width: 60px; text-align: center">

                                <%: Html.CheckBox("ckbAll", new { style="vertical-align:middle;"})%>
                            </th>
                            <th style="width: 70px">Ghi chú</th>
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
                                <%=item.No.ToString("0000000") %>
                            </td>
                            <%} %>
                            <td>
                                <% string cusName = !string.IsNullOrWhiteSpace(item.CusName) ? item.CusName : item.Buyer; %>
                                <%=Html.Encode(cusName)%>
                            </td>
                            <td><%=item.CusCode == null ? "" :Html.Encode(item.CusCode) %></td>
                            <td style="text-align: center">
                                <img src="<%=item.CusSignStatus == cusSignStatus.NocusSignStatus || item.CusSignStatus == cusSignStatus.NoSignStatus ? "/Content/Images/cross.gif" : "/Content/Images/valid.png" %>" />
                            </td>
                            <td style="text-align: center">
                                <%=item.ArisingDate!=EInvoice.Core.Domain.Enumerations.MinDate? String.Format("{0:dd/MM/yyyy}", item.ArisingDate) : ""%>
                            </td>
                            <%if (item.Status == InvoiceStatus.CanceledInv || item.Status == InvoiceStatus.ReplacedInv)
                              { %>
                            <td style="text-align: center">Đã hủy
                            </td>
                            <%}
                              else
                              {%>
                            <td style="text-align: center">
                                <%=Utils.GetNameInvoiceStatus(item.Status)%>
                            </td>
                            <%} %>
                            <td>
                                <%=Html.Encode(item.PublishBy) %>
                            </td>
                            <td style="text-align: center">
                                <img src="<%=item.PaymentStatus == Payment.Paid ? "/Content/Images/valid.png" : "/Content/Images/cross.gif" %>" title="<%=item.PaymentStatus == Payment.Paid ? "Đã thanh toán" :"Chưa thanh toán"%>" />
                            </td>
                            <td style="text-align: center">
                                <a href="#"><span class="fa fa-eye" onclick="viewInv('<%=item.id%>','<%=Html.Encode(item.Pattern)%>');<%= item.Status == InvoiceStatus.CanceledInv || item.Status == InvoiceStatus.ReplacedInv ? "addImg()" : ""%>" title="xem chi tiết"></span></a>
                            </td>
                            <%if (item.Status == InvoiceStatus.NewInv)
                              {%>
                            <td style="text-align: center">
                                <a href="/EInvoice/Edit/<%=item.id%>?Pattern=<%=Html.Encode(item.Pattern) %>" title="Edit">
                                    <i class="fa fa-pencil"></i></a>
                            </td>
                            <td style="text-align: center">
                                <a href="#" onclick="OnDelete('<%=Html.Encode(item.Pattern) %>','<%=item.id%>')" title="Delete">
                                    <i class="fa fa-trash"></i></a>
                            </td>
                            <%}
                              else
                              {%>

                            <td style="text-align: center">
                                <i class="fa fa-lock"></i>
                            </td>
                            <td style="text-align: center">
                                <i class="fa fa-lock"></i>
                            </td>
                            <%} %>
                            <td style="text-align: center">
                                <%=(item.Status == InvoiceStatus.NewInv? "<input name='cbid' type='checkbox'  value='" + item.id +"'/>":"<i class='fa fa-check'></i>")%>                                    
                            </td>
                            <td style="text-align: center"><a href="/EInvoice/WriteNote/<%=item.id%>?Pattern=<%=Html.Encode(item.Pattern)%>&TypeView=<%="0"%>" title="WriteNote">
                                <img alt="" src="/Content/Images/Note.png" /></a></td>
                        </tr>
                        <% i++;
                            }%>
                    </tbody>
                    <%=Html.Hidden("hdPattern") %>
                    <%=Html.Hidden("Serial")%>
                </table>
            </div>
            <div class="box-footer clearfix">
                <div class="pager">
                    <div class="page-select">
                        <%=Html.DropDownList("Pagesize", pages, new { onchange = "ChangePage()",@style="width:45px;" })%>
                    </div>
                    <div class="page-a">
                        <%=Html.Pager(Model.PageListINV.PageSize, Model.PageListINV.PageIndex + 1, Model.PageListINV.TotalItemCount, new
            {
                action = "index",
                controller = "EInvoice",
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
                <%if (Model.SignPlugin > 0)
                  {%>
                <button class="btn btn-sm btn-success ele-center" type="button" onclick="publishPlugIn()" style="float: right;"><i class="fa fa-check"></i>Phát hành hóa đơn</button>
                <%}
                  else
                  {%>
                <button class="btn btn-sm btn-success ele-center" type="button" onclick="Publish_click()" style="float: right;"><i class="fa fa-check"></i>Phát hành hóa đơn</button>
                <%} %>
            </div>
        </form>
    </div>

    <script type="text/javascript">
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
            document.location = "/EInvoice?Pattern=" + pattern + "&Status=" + status + "&Serial=" + serial + "&InvNo=" + $("#InvNo").val() + "&FromDate=" + $("#FromDate").val() + "&ToDate=" + $("#ToDate").val() + "&code=" + code + "&nameCus=" + $("#nameCus").val() + "&typeInvoice=" + TypeInvoice + "&Pagesize=" + document.getElementById("Pagesize").options[document.getElementById("Pagesize").selectedIndex].text;
        }
        //xóa hóa đơn chưa phát hành
        function OnDelete(Pattern, id) {
            swal({
                title: "Bạn muốn xóa dữ liệu này ?",
                text: "Dữ liệu sẽ ko thể khôi phục sau khi xóa",
                type: "warning", showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Xóa",
                cancelButtonText: "Hủy",
                closeOnConfirm: true
            }, function () {
                document.location = "/EInvoice/delete?Pattern=" + Pattern + "&id=" + id;
                //swal("Deleted!", "Your imaginary file has been deleted.", "success");
            });
        }
        //dịnh dạng lại datepicker theo tieng viet                
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
                        document.searchForm.submit();
                    }
                });
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
                    $('input[name=cbid]').not(':disabled').prop('checked', true);
                } else {
                    $('input[name=cbid]').not(':disabled').prop('checked', false);
                }
            });
            $('#crtInvoice').click(function () {
                var pattern = $("#Pattern").val();
                document.location = '/EInvoice/create?Pattern=' + pattern;
            });
            $(function () {
                $('#4plugin').remove();
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

        function publishPlugIn() {
            var cbid = "";
            $('input[name=cbid]:checked').each(function () {
                cbid += this.value + "|";
            });
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
            if ($("input:checkbox:checked").length > 0 && $('#Pattern').val() != "") {
                window.hwcrypto.selectCertSerial({
                    lang: "en"
                }).then(function (response) {
                    var certificate = response.value.cert;
                    var jsondata = { cbid: cbid, Pattern: $('#Pattern').val(), Serial: $('#Serial').val(), certificate: certificate };
                    $.ajax({
                        type: "POST",
                        url: "/AjaxData/LaunchChoiceByPlugin/",
                        data: jsondata,
                        success: function (data) {                            
                            if (data != "") {
                                if (data === "ERROR:1" || data === "ERROR:2") {
                                    sweetAlert("Thông báo", "Chứng thư số chưa được đăng ký, liên hệ để được hỗ trợ.", "error");
                                    $.unblockUI();
                                    return;
                                }
                                if (data === "ERROR:3") {
                                    sweetAlert("Thông báo", "Chứng thư số hết hạn, liên hệ để được hỗ trợ.", "error");                                    
                                    $.unblockUI();
                                    return;
                                }
                                if (data === "ERROR:4") {
                                    sweetAlert("Thông báo", "Có lỗi trong quá trình xử lý, vui lòng thực hiện lại.", "error");
                                    $.unblockUI();
                                    return;
                                }
                                var signData = {
                                    "pattern": $('#Pattern').val(),
                                    "serial": $('#Serial').val(),
                                    "keys": []                                    
                                };
                                var hashData = [];
                                var keys = "";
                                for (var i = 0; i < data.hashdata.length; i++) {
                                    hashData.push(data.hashdata[i].Hash);
                                    signData.keys.push(data.hashdata[i].Key);
                                }
                                window.hwcrypto.signHashData(certificate, {
                                    type: "xmlwithcert",
                                    hex: hashData
                                }, {
                                    lang: "en"
                                }).then(function (response) {
                                    // Nhận kết quả từ hàm ký                                    
                                    var signedData = response.value.signature;
                                    if (signedData == null) {
                                        sweetAlert("Thông báo", "Chưa ký được hóa đơn, vui lòng thực hiện lại.", "error");
                                        return;
                                    }
                                    signData.signeds = signedData;                                    
                                    launchInvoices(signData);
                                    $.unblockUI();
                                }, function (err) {
                                    //Nhận lỗi trả về từ hàm ký (nếu có); bắt buộc phải lọc trường hợp user_cancelled
                                    sweetAlert("Thông báo", err.message + " " + "(" + err.result + ")", "error");
                                    $.unblockUI();
                                });
                            }
                            else {
                                sweetAlert("Thông báo", "Chứng thư chọn khác chứng thư đăng kí.", "error");
                                $.unblockUI();
                            }
                        },
                        error: function () {
                            sweetAlert("Thông báo", "Có lỗi trong quá trình xử lý, vui lòng thực hiện lại.", "error");
                            $.unblockUI();
                        }
                    });
                }, function (err) {
                    // Nhận lỗi trả về từ hàm lấy chứng thư (nếu có); bắt buộc phải lọc trường hợp user_cancelled
                    sweetAlert("Thông báo", err.message + " " + "(" + err.result + ")", "error");
                    $.unblockUI();
                });
            }
            else {
                sweetAlert("Thông báo", "<%=Resources.Message.MInv_MesChooseInvToPublish%>", "error");
                $.unblockUI();
            }
        }

        function launchInvoices(signData) {
            var invoicesjson = { signData: JSON.stringify(signData) };
            $.ajax({
                type: "POST",
                url: "/AjaxData/wrapandlaunch/",
                data: invoicesjson,
                dataType: "json",
                success: function (data) {
                    console.log(data);
                    if (data === "OK") {

                        swal({ title: "Thông báo", text: "Phát hành thành công: " + signData['keys'].length + " hóa đơn.", type: "success" },
                            function () { document.location = '/EInvoice/Index?Pattern=' + $('#Pattern').val(); }
                        );
                    } else {
                        sweetAlert("Thông báo", data, "error");
                    }
                }, error: function () {
                    sweetAlert("Thông báo", "Có lỗi trong quá trình xử lý, vui lòng thực hiện lại.", "error");
                }
            });
        }

        //Phát hành hóa đơn
        function Publish_click() {
            var names = [];
            $('input:checked').each(function () {
                names.push(this.value);
            });

            if ($("input:checkbox:checked").length > 0 && $('#Pattern').val() != "") {
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
            } else {
                sweetAlert("Thông báo", "<%=Resources.Message.MInv_MesChooseInvToPublish%>", "error");

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
