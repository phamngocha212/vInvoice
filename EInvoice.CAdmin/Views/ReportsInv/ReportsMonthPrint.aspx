<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<EInvoice.CAdmin.Models.ReportsPrintModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thống kê hóa đơn theo tháng
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/reportInv.css" rel="stylesheet" type="text/css" /> 
    <script src="/Content/js/jquery.PrintArea.js"></script>   
    <%=Html.Hidden("Month",ViewData["Month"] ) %>
    <%=Html.Hidden("Year",ViewData["Year"]) %>
    <div>
        <div id="print"> 
            <style>
                #contentarea
                {
                    margin: 6px;
                }

                element.style
                {
                    width: 100%;
                }
                table.grid
                {	
                    color: #333333;
                    border-collapse: collapse;
                }
                table.grid th
                {
                    color: #333333;
                    background-color: #eee;
                    font-weight: bold;
                    border-right: solid 1px #fff;
                    border-bottom: solid 1px #ccc;
                    padding: 4px;
                    text-align: center;
                }

                table.grid td
                {
                    border-bottom: 1px solid #ccc;
                    padding: 6px 4px;

                }
                table.grid tr:hover
                {
                    background-color: #eef;
                }
                /**/
                .report-select{
                    width:500px;
                    margin:30px auto;
                    text-align:center;
                    font-weight:bold;
                }
                .report-select input, .report-select select{
                    margin:0 5px;
                }
                .report-select .report-btn{
                    margin-top:15px;
                }
                /**/
                .report-used-vat{
                    margin:35px auto;
                    padding:20px 0;
                    width:1524px;
                }
                .report-used-header{
                    margin-bottom:10px;
                }
                .report-used-header h3, .report-used-header p{
                    margin:0;
                    text-align:center;
                    font-family:"Times New Roman", Times, serif;
                }
                .report-used-header h3{
                    font-size:1.2em;
                    text-transform:uppercase;
                }
                .report-used-header ol{
                    margin:50px 0 0 -26px;
                    list-style:decimal;
                }
                .report-used-header label{
                    font-weight:bold;
                    margin-right:5px;
                    background:#fff;
                    float:left;
                    padding-right:5px;
                }
                .report-used-header li{
                    list-style:none;
                    padding:0;
                    height:12px;
                    padding-top:10px;
                    clear:both;
                    margin-bottom:10px;
                }
                .report-used-header .filtext{
                    float:left;
                }
                .report-used-list{
                    margin-bottom:15px;
                    border-bottom:1px solid #333;
                    border-left:1px solid #333;
                    margin-top:5px;
                }
                .report-used-list thead td{
                    text-align:center;
                    font-style:italic;
                }
                .report-used-list th, .report-used-list td{
                    padding:5px;
                    border-top:1px solid #333;
                    border-right:1px solid #333;
                }
                .report-used-list th{
                    text-align:center;
                }
                .report-used-bottom{
                    text-align:center;
                    margin:20px 0 50px 0;
                }

                /**/
                .header-l{
                    width:80%;
                    float:left;
                }
                .header-r{
                    width:20%;
                    float:right;
                }
                .header-r i{
                    color:#008000;
                }
                .national{
                    text-align:center;
                    margin-bottom:30px;
                    font-size:120%;
                    font-family:"Times New Roman", Times, serif;
                }
                .national h4{
                    text-transform:uppercase;
                    margin:0;
                    font-size:1em;
                    font-family:"Times New Roman", Times, serif;}
                .national p{
                    font-size:1.2em;
                }
                table.grid tr.selected td
                {
                    background-color: #ffe;
                    font-weight: bold;
                }

                table.grid td a
                {
                    margin-right: 5px;
                }

                table.grid td.center
                {
                    text-align: center;
                }

                table.grid td.right
                {
                    text-align: right;
                }
               
            </style>
            <%=Model.Html  %>
        </div>
    </div>
    <div class="text-center">
        <button class="btn btn-sm" type="button"onclick="document.location = '/ReportsInv/ReportsMonth'">
            <i class="fa fa-backward"></i>Quay lại</button>
        <button class="btn btn-sm btn-primary" type="button" onclick="JavaScript: printex('print');"><i class="fa fa-print"></i>In báo cáo</button>                              
        <button class="btn btn-sm btn-primary" type="button" style="margin-left: 0px;" onclick="Download_Excel()">
            <i class="fa fa-download"></i> Xuất file XLS</button> 
        <button class="btn btn-sm btn-primary" type="button" style="margin-left: 0px;" onclick="Download_xml()">
            <i class="fa fa-download"></i> Xuất file XML</button>
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
        function Download_Excel() {
            var Month = $("#Month").val();
            var Year = $("#Year").val();
            document.location = "/ReportsInv/DownloadReportMonth_Excel?month=" + Month + "&year=" + Year;
        }
        function Download_xml() {
            var Month = $("#Month").val();
            var Year = $("#Year").val();
            document.location = "/ReportsInv/DownloadReportMonth_xml?month=" + Month + "&year=" + Year;
        }
    </script>

</asp:Content>
