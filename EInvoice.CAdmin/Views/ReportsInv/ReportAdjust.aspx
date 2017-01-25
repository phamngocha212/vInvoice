<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ReportEinv_Adjust>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Báo cáo sửa đổi
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <form id="Form1" method="post" class="form-horizontal" action="/ReportsInv/ReportAdjust">
        <div class="row">
            <div class="col-xs-offset-2 col-xs-8">
                <div class="box box-danger">
                    <div class="box-header with-border">
                        <h4 class="box-title">
                            <i class="fa fa-paper-plane"></i>BÁO CÁO SỬA ĐỔI HÓA ĐƠN
                        </h4>
                    </div>
                    <div class="box-body">
                        <div class="row">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-4 control-label">Từ ngày</label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon">
                                                <i class="fa fa-calendar"></i>
                                            </div>
                                            <%=Html.TextBox("FromDate", Model.FromDate, new {@placeholder="__/__/____", @class = "datepicker form-control"})%>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4 control-label">Người thực hiện</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("username", Model.username, new {  maxlength="200",@class="form-control"})%>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="col-sm-4 control-label">Đến ngày</label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon">
                                                <i class="fa fa-calendar"></i>
                                            </div>
                                            <%=Html.TextBox("ToDate", Model.ToDate, new {  @placeholder="__/__/____", @class = "datepicker form-control"})%>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="box-footer">
                        <button class="btn-sm btn-primary element-center" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <div class="clearfix"></div>
    <div class="box">
        <div class="box-body no-padding">
            <table class="grid">
                <thead>
                    <tr>
                        <th width="50px">STT </th>
                        <th width="150px" style="color: red">Mẫu HD cũ </th>
                        <th width="150px" style="color: red">Ký hiệu HD cũ</th>
                        <th width="100px" style="color: red">Số HD cũ</th>
                        <th width="100px" style="color: red">Ngày ký</th>
                        <th width="100px" style="color: red">Tổng tiền</th>

                        <th width="150px" style="color: blue">Mẫu HD mới</th>
                        <th width="150px" style="color: blue">Ký hiệu hD mới</th>
                        <th width="100px" style="color: blue">Số HD mới</th>
                        <th width="100px" style="color: blue">Ngày ký</th>
                        <th width="100px" style="color: blue">Tổng tiền</th>

                        <th width="100px">Khách hàng</th>
                        <th width="100px">Địa chỉ</th>
                        <th width="100px">Mã khách hàng</th>
                        <th width="100px">Mã số thuế kh</th>

                        <th width="150px">Người thực hiện</th>
                        <th width="150px">Ngày thực hiện</th>
                    </tr>
                </thead>
                <tbody>
                    <%    
                        List<RecordAdjust> lstfile = Model.PageListRecordAdjust.ToList();
                        int i = Model.PageListRecordAdjust.PageIndex * Model.PageListRecordAdjust.PageSize + 1;
                        foreach (RecordAdjust item in lstfile)
                        {
                            if (item == null) continue;
                    %>
                    <tr>
                        <td align="center"><%=i%></td>
                        <td align="center"><%=item.patternOlder%></td>
                        <td align="center"><%=item.serialOlder%></td>
                        <td align="center"><%=Convert.ToDecimal(item.noOlder).ToString("0000000")%></td>
                        <td align="center"><%=item.publishDateOlder.ToString("dd/MM/yyyy")%></td>
                        <td align="center"><%=item.totalMoneyOlder %> </td>

                        <td align="center"><%=item.patternNew%></td>
                        <td align="center"><%=item.serialNew%></td>
                        <td align="center"><%=Convert.ToDecimal(item.noNew).ToString("0000000")%></td>
                        <td align="center"><%=item.publishDateNew.ToString("dd/MM/yyyy")%></td>
                        <td align="center"><%=item.totalMoneyNew%></td>

                        <td><%=Html.Encode(item.cusnameNew)%>                
                        </td>
                        <td><%=Html.Encode(item.addressCusNew)%>                
                        </td>
                        <td><%=Html.Encode(item.cuscodeNew)%>                
                        </td>
                        <td align="center"><%=Html.Encode(item.cusTaxcode)%></td>
                        <td align="center"><%=Html.Encode(item.username)%></td>
                        <td align="center"><%=item.proccessdate%></td>
                    </tr>
                    <% i++;
                    }%>
                </tbody>
            </table>
        </div>
        <div class="box-footer clearfix">
            <div class="pager  ">
                <div class="page-a">
                    <%=Html.Pager(Model.PageListRecordAdjust.PageSize, Model.PageListRecordAdjust.PageIndex + 1, Model.PageListRecordAdjust.TotalItemCount, new
            {
                action = "ReportAdjust",
                controller = "Reports",
                FromDate = Model.FromDate,
                ToDate = Model.ToDate,
                username= Model.username
            })%>
                </div>
            </div>
        </div>
        <div style="float: right; margin-top: 8px">
            <button class="btn btn-sm" type="button" onclick="document.location = '/'">
                <i class="fa fa-backward"></i>Quay lại</button>
            <button class="btn btn-sm btn-primary" type="button" style="margin-left: 0px;" onclick="DownloadExcel()">
                <i class="fa fa-download"></i>Xuất file XLS</button>
        </div>
    </div>
    <script type="text/javascript">

        function DownloadExcel() {
            var FromDate = $("#FromDate").val();
            var ToDate = $("#ToDate").val();
            var username = $("#username").val();
            document.location = "/ReportsInv/DownloadExcel?FromDate=" + FromDate + "&ToDate=" + ToDate + "&username=" + username + "&status=" + "1";
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

        });
    </script>    
</asp:Content>
