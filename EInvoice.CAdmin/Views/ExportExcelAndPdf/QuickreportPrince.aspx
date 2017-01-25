<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ReportsDetailModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    THỐNG KÊ CHI TIẾT HÓA ĐƠN
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <script src="/Content/js/FXUtils.js" type="text/javascript"></script>
    <script src="/Content/js/jquery.PrintArea.js"></script>
    <%=Html.Hidden("hdPattern") %>
    <div class="row">
        <div class="col-xs-12">
            <div id="Print" class="table-responsive">
                <%=Model.Html  %>
            </div>
            <div class="pager">
                <div class="page-a">
                    <%=Html.Pager(Model.pageSize, Model.pageIndex + 1, Model.totalRecords, new
        {
            action = "QuickreportPrince",
            controller = "ExportExcelAndPdf",
            Pattern = Model.Pattern,
            Status = Model.Status,
            FromDate = Model.FromDate,
            ToDate = Model.ToDate,
            Serial = Model.Serial,
            Pagesize = Model.pageSize              
        })%>
                </div>
            </div>
        </div>
    </div>
    <%         
        string Nameexport = "THỐNG KÊ CHI TIẾT HÓA ĐƠN";
        string pattern = Model.Pattern;
        string serial = Model.Serial;
        int Status = Model.Status;
        string FormDate = Model.FromDate;
        string ToDate = Model.ToDate;
    %>
    <div class="box-footer">
        <button class="btn btn-sm" type="button" onclick="document.location = '/ExportExcelAndPdf/Quickreport?Pattern=<%=ViewData["Pattern"] %>'">
            <i class="fa fa-backward"></i>Quay lại</button>
        <button class="btn btn-sm btn-primary" type="button" onclick="JavaScript: printex('Print');"><i class="fa fa-print"></i>In báo cáo</button>
        <button class="btn btn-sm btn-primary" type="button" style="margin-left: 0px;" onclick="document.location ='/ExportExcelAndPdf/DownloadQuickReportPrint?nameexport='+'<%=Nameexport %>'+'&pattern='+'<%=pattern %>'+'&serial='+'<%=serial%>'+'&Status='+'<%=Status%>'+'&FromDate='+'<%=FormDate %>'+'&ToDate='+'<%=ToDate %>'+'&type='+'xls'">
            <i class="fa fa-download"></i>Xuất file XLS</button>
        <button class="btn btn-sm btn-primary" type="button" style="margin-left: 0px;" onclick="document.location ='/ExportExcelAndPdf/DownloadQuickReportPrint?nameexport='+'<%=Nameexport %>'+'&pattern='+'<%=pattern %>'+'&serial='+'<%=serial%>'+'&Status='+'<%=Status%>'+'&FromDate='+'<%=FormDate %>'+'&ToDate='+'<%=ToDate %>'+'&type='+'pdf'">
            <i class="fa fa-download"></i>Xuất file PDF</button>
    </div>
    <script type="text/javascript">        
        function printex(divID) {
            var printElement = document.getElementById(divID);
            $(printElement).printArea({
                mode: "iframe",
                popWd: 1000,
                popHt: 900,
                popClose: false
            });
        }
        function htmlEncode(value) {
            return $('<div/>').text(value).html();
        }
    </script>

</asp:Content>
