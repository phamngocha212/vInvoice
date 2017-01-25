<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ConvertionModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Chuyển đổi hóa đơn
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script src="/Content/js/BrowserDetectShare.js"></script>
    <script src="/Content/js/jquery.PrintArea.js"></script>
    <script src="/Content/js/main.js"></script>
    <div id="convertmess" style="display: none"></div>

    <div class="row">
        <div class="col-xs-8 col-xs-offset-2">
            <form id="Searchform" method="post" class="form-horizontal"
                action="/InvConvertion/index">
                <div class="box box-primary">
                    <div class="box-header with-border">
                        <h4 class="box-title">
                            <i class="fa fa-refresh"></i>Chuyển đổi hóa đơn
                        </h4>
                    </div>
                    <div class="box-body">
                        <div class="row">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.Conv_LblPattern%>:</label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Pattern", Model.PatternList, new { @name = "Pattern", @class="form-control" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4">Trạng thái:</label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Converted", new[]
              {
                  new SelectListItem{Text="--Trạng thái chuyển đổi--", Value="0", Selected=(Model.Converted == 0)},
                  new SelectListItem{Text=Resources.Einvoice.ConSr_lblNotYetConvert, Value="-1", Selected=(Model.Converted == -1)},
                  new SelectListItem{Text=Resources.Einvoice.ConSr_lblConvertedInv, Value="1", Selected=(Model.Converted == 1)},
              }, new {onchange = "sel()", title = "Chọn tình trạng chuyển đổi!", @class="searchText form-control" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.Conv_LblFromDate%>:</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("FromDate", Model.FromDate == null ? "" : Model.FromDate.ToString(), new {@class = "form-control datepicker vietnameseDate", @placeholder="__/__/____"})%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4">Mã khách hàng:</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("cuscode", Model.cuscode, new {@class="textandnum searchText form-control", maxlength="50" })%>
                                    </div>
                                </div>

                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.Conv_LblSerial%>:</label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Serial", Model.SerialList, "--Ký hiệu--", new { @name = "Serial", @class="form-control", title = "Chọn ký hiệu" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.Conv_LblInvNo%>: </label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("InvNo", "", new { @class="onlynum form-control", maxlength = "7", onkeypress = "return keypress(event);"})%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4"><%=Resources.Einvoice.Conv_LblToDate%>:</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("ToDate", Model.ToDate == null ? "" : Model.ToDate.ToString(), new {@class = "form-control datepicker vietnameseDate", @placeholder="__/__/____" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4">Tên khách hàng:</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("cusName", Model.cusName, new {  maxlength="200", @class="searchText form-control" })%>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="box-footer">
                        <button class="btn-sm btn btn-primary element-center" type="submit"><i class="fa fa-search"></i>&nbsp; &nbsp;Tìm kiếm</button>
                    </div>
                </div>
            </form>
        </div>
    </div>

    <div class="row">
        <div class="col-xs-12">

            <div class="box box-danger">
                <form id="danhsachHD" method="post" action="/InvConvertion/Index">

                    <div class="box-body table-responsive">


                        <table class="table table-bordered table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 20px">STT
                                    </th>
                                    <th width="120px">
                                        <%=Resources.Einvoice.Conv_LblPattern%>
                                    </th>
                                    <th width="120px">
                                        <%=Resources.Einvoice.Conv_LblSerial%>
                                    </th>
                                    <th width="80px">Số
                                    </th>
                                    <th>
                                        <%=Resources.Einvoice.Conv_LblCusName%>
                                    </th>
                                    <th width="120px">
                                        <%=Resources.Einvoice.Conv_LblPubDate%>
                                    </th>
                                    <th width="120px">Trạng thái
                                    </th>
                                    <th width="20px" style="border-right-color: #EEE">
                                        <%=Resources.Einvoice.Conv_LblChoose%>
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
                                    <td></td>
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
                                        <img src="<%=item.Converted ? "/Content/Images/valid.png" : "/Content/Images/cross.gif" %>" title="<%=item.Converted? "Đã chuyển đổi" :"Chưa chuyển đổi"%>" />
                                    </td>
                                    <td style="text-align: center">
                                        <input name="cbid" type="radio" value="<%=item.id%>" onclick="CheckConverted(<%=item.Converted?1:0 %>)" />
                                    </td>
                                </tr>
                                <% i++;
                                    }%>
                            </tbody>
                            <%=Html.Hidden("hdPattern") %>
                            <%=Html.Hidden("id","") %>    <%--k can thiet--%>
                            <%=Html.Hidden("cusCode") %>  <%--ko can thiet--%>
                        </table>
                        <br />
                        <div class="pager">
                            <input type="hidden" id="con" name="con" />
                            <div class="page-select">
                                <%=Html.DropDownList("Pagesize", pages, new { onchange = "ChangePage()",@style="width:45px" })%>
                            </div>
                            <div class="page-a">
                                <%=Html.Pager(Model.PageListINV.PageSize, Model.PageListINV.PageIndex + 1, Model.PageListINV.TotalItemCount, new
                                {
                                    action = "index",
                                    controller = "InvConvertion",
                                    Pattern = Model.Pattern,
                                    Converted = Model.Converted,                
                                    FromDate = Model.FromDate,
                                    ToDate = Model.ToDate,
                                    Serial = Model.Serial,
                                    InvNo = Model.InvNo,
                                    Pagesize = Model.PageListINV.PageSize               
                                })%>
                            </div>
                        </div>
                        <div class="element-right">                            
                            <button id="sourceBtn" class="btn btn-sm btn-primary" type="button" onclick="Convert(0)"><i class="fa fa-refresh"></i><%=Resources.Einvoice.ConSr_btnToProveOrigin%></button>
                            <button id="Button1" class="btn btn-sm btn-danger" type="button" onclick="Convert(1)"><i class="fa fa-save"></i><%=Resources.Einvoice.ConSt_BtnOrigins%></button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(function ($) {
            $(".vietnameseDate").mask("99/99/9999");
        });
        function CheckConverted(converted) {
            if (converted == 1)
                document.getElementById("sourceBtn").disabled = true;
            else
                document.getElementById("sourceBtn").disabled = false;
        }
        function Convert(choice) {            
            id = $("input:radio[name='cbid']:checked").val();
            if (!id || id == ""){
                 
                sweetAlert("Thông báo", "Chưa chọn hóa đơn cần chuyển đổi.", "error");
                return false;
            }

            if(choice == 0 && !confirm("Hóa đơn này chỉ được chuyển đổi chứng minh nguồn gốc duy nhất một lần.\n- Bạn có chắc chắn chuyển đổi không?")){
                return false;
            }

            str = "";
            if (choice == 0)
                str = "ConvertForVerify";
            else str = "ConvertForStore";                        
            ajxCall4Convert(id,$('#Pattern').val(),str)            
        }                

        function ChangePage() {
            var pattern = $('#Pattern').val();
            var convertStatus = $('#Converted').val();
            document.location = "/InvConvertion/index?Pattern=" + pattern + "&Converted=" + convertStatus + "&Serial=" + $("#Serial").val() + "&InvNo=" + $("#InvNo").val() + "&FromDate=" + $("#FromDate").val() + "&ToDate=" + $("#ToDate").val() + "&Pagesize=" + document.getElementById("Pagesize").options[document.getElementById("Pagesize").selectedIndex].text;
        }               

        $(document).ready(function () {            
            $(".datepicker").datepicker({
                showOn: "button",
                format: "dd/mm/yyyy",
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
        
        function sel() {
            var Converted = $('#Converted').val();
            if ($('#Converted').val() && parseInt($('#Converted').val()) != 0) {
                $("#InvNo").val("");
                $("#InvNo").attr("disabled", "disabled");
            }
            else {
                $("#InvNo").removeAttr("disabled");
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
</asp:Content>
