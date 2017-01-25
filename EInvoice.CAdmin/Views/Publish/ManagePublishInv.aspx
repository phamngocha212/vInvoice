<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<PublishInvModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Mẫu hóa đơn đăng ký
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script src="/Content/js/jquery.PrintArea.js"></script>

    <style>        
        .VATTEMP {
            margin-bottom: 0px !important;
        }
        .modal-footer{
            background-color:#fff;
        }
    </style>
    <%
        SelectList opattern = new SelectList((IList<string>)Model.lstpattern);
        SelectList oSerial = new SelectList((IList<string>)Model.lstserial);
    %>
    <form id="Searchform" name="searchForm" method="post" class="form-horizontal" action="/Publish/ManagePublishInv">
        <div class="row">
            <div class="col-xs-offset-2 col-xs-8">
                <div class="box box-primary">

                    <div class="box-body">
                        <div class="row">
                            <div class="col-xs-6">
                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblPattern%></label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Pattern", opattern, new { @name = "Pattern",@class="form-control" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4 control-label">Từ ngày</label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon">
                                                <i class="fa fa-calendar"></i>
                                            </div>
                                            <%=Html.TextBox("FromDate", Model.FromDate, new { style = "", @class = " form-control datepicker",@placeholder="__/__/____"})%>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xs-6">
                                <div class="form-group">
                                    <label class="col-sm-4 control-label"><%=Resources.Einvoice.MInv_LblSerial%></label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Serial",oSerial,"--Ký hiệu--",new { @name = "Serial",@class="form-control", @onchange="document.searchForm.submit();"})%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4 control-label">Đến ngày</label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon">
                                                <i class="fa fa-calendar"></i>
                                            </div>
                                            <%=Html.TextBox("ToDate", Model.ToDate, new { @class = "form-control datepicker",@placeholder="__/__/____"})%>
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
    <div class="clearfix"></div>
    <div class="box box-danger">
        <div class="box-header with-border">
            <h4 class="box-title">
                <i class="fa fa-file-text"></i>Danh sách mẫu đăng ký
            </h4>
        </div>
        <div class="box-body no-padding table-responsive">
            <table class="table table-bordered table-hover">
                <thead>
                    <tr>
                        <th width="30px">STT
                        </th>
                        <th width="50px">Mẫu số
                        </th>
                        <th width="50px">Ký hiệu
                        </th>
                        <th width="70px">Số lượng
                        </th>
                        <th width="70px">Từ số
                        </th>
                        <th width="70px">Đến số
                        </th>
                        <th width="70px">Số hiện tại
                        </th>
                        <th width="70px">Số còn lại
                        </th>
                        <th width="100px">Ngày bắt đầu
                        </th>
                        <th width="100px">Trạng thái</th>
                        <th width="40px">Xem mẫu</th>
                    </tr>
                </thead>
                <tbody>
                    <%   
                        IList<PublishInvoice> lstpubInv = Model.PageListPubInv.ToList();
                        int i = Model.PageListPubInv.PageIndex * Model.PageListPubInv.PageSize + 1;
                        foreach (PublishInvoice pubinv in lstpubInv)
                        {
                    %>
                    <tr>
                        <td style="text-align: right"><%=i%></td>
                        <td><%=Html.Encode(pubinv.InvPattern)%></td>
                        <td><%=Html.Encode(pubinv.InvSerial)%></td>
                        <td style="text-align: right"><%=Html.Encode(pubinv.ToNo-pubinv.FromNo+1)%></td>
                        <td style="text-align: center">
                            <%=pubinv.FromNo.ToString("0000000")%>
                        </td>
                        <td style="text-align: center">
                            <%=pubinv.ToNo.ToString("0000000")%>
                        </td>
                        <td style="text-align: center">
                            <%=pubinv.CurrentNo.ToString("0000000")%>
                        </td>
                        <td style="text-align: right"><%=pubinv.ToNo-pubinv.CurrentNo%></td>
                        <td style="text-align: center"><%=pubinv.StartDate.ToString("dd'/'MM'/'yyyy")%></td>
                        <%if (pubinv.Status == 1)
                          {%>
                        <td style="text-align: center">Chưa sửa dụng</td>
                        <%}
                          else if (pubinv.Status == 2)
                          { %>
                        <td style="text-align: center">Đang sử dụng</td>
                        <%}
                          else if (pubinv.Status == 3 && pubinv.CurrentNo < pubinv.ToNo)
                          { %>
                        <td style="text-align: center">Đã hủy</td>
                        <%}
                          else if (pubinv.CurrentNo == pubinv.ToNo)
                          { %>
                        <td style="text-align: center">Đã sử dụng hết</td>
                        <%} %>
                        <%if (pubinv.PubTemp.Count() > 0)
                          {%>
                        <td style="text-align: center" >
                            <i class="fa fa-eye" onclick="viewTemp('<%=pubinv.RegisterID %>','<%=Html.Encode(pubinv.InvSerial)%>')" ></i></td>
                        <%}
                          else
                          {%>
                        <td></td>
                        <%} %>
                    </tr>
                    <% i++;
                        }%>
                </tbody>
            </table>
        </div>
        <div class="box-footer clearfix">
            <div class="pager  ">
                <div class="page-a">
                    <%=Html.Pager(Model.PageListPubInv.PageSize, Model.PageListPubInv.PageIndex + 1, Model.PageListPubInv.TotalItemCount,new{
                action = "ManagePublishInv",
                controller = "Publish",
                FromDate = Model.FromDate,
                ToDate = Model.ToDate,
                Pattern = Model.Pattern,
                Serial = Model.Serial
            })%>
                </div>
            </div>
        </div>
    </div>
    <div id="src" style="display: none;" class="modal modal-default">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-body">
                    <div id="invData"></div>                    
                </div>
            </div>
            <div class="modal-footer" id="invoice-footer"></div>
        </div>
    </div>
    <script type="text/javascript">
        // Định dang datetime        
        $(function ($) {
            $(".vietnameseDate").mask("99/99/9999");
        });
        
        //xem view mau phat hanh
        function viewTemp(tempid, serialNo) {
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
            var jsondata = "tempid=" + tempid + "&serialNo=" + serialNo;
            $.ajax({
                type: "POST",
                url: "/Publish/AjxPreviewPubInv/",
                data: jsondata,
                success: function (data) {
                    document.getElementById("invData").innerHTML = data;                    
                    document.getElementById("invoice-footer").innerHTML =
                            "<button class='btn btn-sm btn-success' type='button' onclick=\"printInvoice();\"><i class='fa fa-print'></i> In mẫu</button>";
                    $('#src').modal('show');

                    $.unblockUI();
                },
                error: function () {
                    $.unblockUI();
                }
            });
        }
        //in mau
        function printInvoice() {
            $("#inbt").css("display", "none");
            var printElement = document.getElementById("invData");
            $(printElement).printArea({
                mode: "iframe",
                popWd: 1000,
                popHt: 900,
                popClose: false
            });
            $("#inbt").css("display", "block");
            return (false);
        }

        //Định dạng hình ảnh hiển thi giao diện chọn datepicker
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
                        if (data.pu.length > 0) {
                            newOpt = new Option("--Ký hiệu--", "");
                            document.getElementById("Serial").options.add(newOpt);
                            newOpt.selected = true;
                            for (i = 0; i < data.pu.length; i++) {
                                newOption = new Option(data.pu[i]);
                                document.getElementById("Serial").options.add(newOption);
                            }
                        }
                        document.searchForm.submit();
                    }
                });
            });
        });
    </script>
</asp:Content>

