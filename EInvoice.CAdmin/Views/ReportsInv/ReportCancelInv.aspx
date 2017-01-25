<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ReportEinv_Cancel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Báo cáo hủy hóa đơn
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>    
    <form id="Searchform" method="post" class="form-horizontal" action="/ReportsInv/ReportCancelInv">
        <div class="row">
            <div class="col-xs-offset-2 col-xs-8">
                <div class="box box-danger">
                    <div class="box-header with-border">
                        <h4 class="box-title">
                            <i class="fa fa-paper-plane"></i>BÁO CÁO HỦY HÓA ĐƠN
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
                                            <%=Html.TextBox("FromDate", Model.FromDate, new { @placeholder="__/__/____", @class = "datepicker form-control"})%>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4 control-label">Người hủy</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("username", Model.username, new { @class="form-control",style = "", maxlength="200"})%>
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
                                            <%=Html.TextBox("ToDate", Model.ToDate, new { @placeholder="__/__/____", @class = "datepicker form-control"})%>
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
            <table class="table table-bordered table-hover">
                <thead>
                    <tr>
                        <th width="30px">STT  </th>
                        <th width="80px">Mẫu số</th>
                        <th width="80px">Ký hiệu</th>
                        <th width="100px">Số hóa đơn</th>
                        <th width="80px">Ngày PH</th>
                        <th width="80px">Tổng tiền</th>

                        <th>Tên khách hàng</th>
                        <th>Địa chỉ</th>
                        <th width="80px">Mã KH</th>
                        <th width="90px">Mã số thuế</th>
                        <th width="120px">Người thực hiện</th>
                        <th width="120px">Ngày thực hiện</th>

                    </tr>
                </thead>
                <tbody>
                    <%    
                        List<RecordInvCancel> lstfile = Model.PageListRecordInvCancel.ToList();
                        int i = Model.PageListRecordInvCancel.PageIndex * Model.PageListRecordInvCancel.PageSize + 1;
                        foreach (RecordInvCancel item in lstfile)
                        {
                    %>
                    <tr>
                        <td style="text-align: right"><%=i%></td>

                        <td style="text-align: center"><%=item.pattern%></td>
                        <td style="text-align: center"><%=item.serial%></td>
                        <td style="text-align: right"><%=item.no%></td>
                        <td style="text-align: center"><%=item.publishDate.ToString("dd/MM/yyyy")%></td>
                        <td style="text-align: right"><%=item.totalAmount%></td>

                        <td>
                            <%=Html.Encode(item.cusName)%>                       
                        </td>
                        <td>
                            <%=Html.Encode(item.addressCus)%>                        
                        </td>
                        <td style="text-align: center">
                            <%=Html.Encode(item.cusCode)%>                        
                        </td>
                        <td style="text-align: center"><%=item.cusTaxCode%></td>
                        <td style="text-align: center"><%=item.Username%></td>
                        <td style="text-align: center"><%=item.DayCancelInv%></td>
                    </tr>
                    <% i++;
                        }%>
                </tbody>
            </table>
        </div>
        <div class="box-footer clearfix">
            <div class="pager  ">
                <div class="page-a">
                    <%=Html.Pager(Model.PageListRecordInvCancel.PageSize, Model.PageListRecordInvCancel.PageIndex + 1, Model.PageListRecordInvCancel.TotalItemCount, new
            {
                action = "ReportCancelInv",
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
            document.location = "/ReportsInv/DownloadExcelInvCancel?FromDate=" + FromDate + "&ToDate=" + ToDate + "&username=" + username;
        }
        // xử lý chọn ngày tháng datetime
        // xử lý chọn ngày tháng datetime                
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
