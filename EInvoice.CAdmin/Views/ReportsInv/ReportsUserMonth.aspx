<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ReportsModel>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Tình hình sử dụng hóa đơn
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <script src="/Content/js/jquery.validate.min.js" type="text/javascript"></script>      
    <form method="get" action="/ReportsInv/ReportsUserMonthPrint" id="ReportPrint">
        <div style="width: 500px; margin: 20px auto; border: 1px solid #ccc; border-radius: 5px; padding-top: 10px">
            <div class="box-header with-border">
                <h4 class="box-title">
                    <i class="fa fa-paper-plane"></i>THÁNG LẬP BÁO CÁO
                </h4>
            </div>
            <table style="margin: 10px auto">
                <tr>
                    <td>
                        Tháng báo cáo:
                        <%=Html.DropDownList("month", Model.lstMonth, new { @style = "width:80px ", @class = "required", title = "Chọn tháng báo cáo."}) %>
                    </td>
                    <td>
                        <%=Resources.Einvoice.ReUse_Year%>:
                        <%=Html.DropDownList("year", Model.lstYear, new { @style = "width:80px ", @class = "required", title = "Chọn năm báo cáo."}) %>
                    </td>
                </tr>
            </table>
            <div class="box-footer">
                <button class="btn-sm btn btn-primary element-center" type="submit"><i class="fa fa-check"></i><%=Resources.Einvoice.BtnReport%></button>
            </div>
        </div>
    </form>
    <script type="text/javascript">
        $(document).ready(function () {
            $('form:first').validate();
        });
    </script>
</asp:Content>
