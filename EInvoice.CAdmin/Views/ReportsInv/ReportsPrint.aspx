<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ReportsPrintModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    BÁO CÁO TÌNH HÌNH SỬ DỤNG HÓA ĐƠN
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">  
    <script src="/Content/js/jquery.PrintArea.js"></script>
    <%=Html.Hidden("year",ViewData[" year"]) %>
    <%=Html.Hidden("quarter",ViewData["quarter"]) %>
    <div>
        <div id="print"><%: Html.Raw(Model.Html.Replace("&lt;br/&gt;","<br />"))  %></div>
        <%-- <div id = "print1"> htmlText </div>--%>
    </div>
    <div class="text-center">
        <button class="btn btn-sm btn-primary" type="button" onclick="JavaScript: printex('print');"><i class="fa fa-print"></i> In báo cáo</button>                              
        <button class="btn btn-sm btn-primary" type="button" style="margin-left: 0px; width: 100px;" onclick="DownloadreportXls();">
            <i class="fa fa-download"></i>Xuất file XLS</button>  
          <button class="btn btn-sm btn-primary" type="button" style="margin-left: 0px; width: 100px;" onclick="Download_xml();">
            <i class="fa fa-download"></i>Xuất file XML</button>
        <button class="btn btn-sm" type="button"onclick="document.location = '/ReportsInv/Reports'">
            <i class="fa fa-backward"></i>Quay lại</button>       
    </div>
    <script language="javascript" type="text/javascript">
        function printex(divID) {
            var printElement = document.getElementById(divID);
            $(printElement).printArea({
                mode: "iframe",
                popWd: 1000,
                popHt: 900,
                popClose: false
            });
        }
        function DownloadreportXls() {
            var year = $("#year").val();
            var Quarter = $("#quarter").val();
            document.location = "/ReportsInv/DownloadXls?year=" + year + "&Quarter=" + Quarter + "&statusReport=" + 0;
        }
        function Download_xml() {
            var year = $("#year").val();
            var Quarter = $("#quarter").val();
            document.location = "/ReportsInv/DownloadXml?year=" + year + "&Quarter=" + Quarter + "&statusReport=" + 0;
        }
    </script>
</asp:Content>
