<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<ReportsModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Bảng kê hóa đơn hàng tháng
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">       
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <div style="width: 600px; margin: 20px auto; border: 1px solid #ccc; border-radius: 5px; padding-top: 10px">       
        <form id="ReportPrint" method="post" action="/ReportsInv/ReportsMonthPrint">
            <div class="box-header with-border">
                <h4 class="box-title">
                    <i class="fa fa-paper-plane"></i>BẢNG KÊ HOÁ ĐƠN, CHỨNG TỪ HÀNG HOÁ, DỊCH VỤ BÁN RA
                </h4>
            </div>
            <table style="margin: 10px auto">
                <tr>
                    <td><b>THÁNG:</b>
                        <%=Html.DropDownList("Month", Model.lstMonth, new { @style = "width:80px "})%>
                    </td>
                    <td><b>NĂM:</b>
                        <%=Html.DropDownList("Year", Model.lstYear,  new { @style = "width:80px "})%>
                    </td>
                </tr>
            </table>
            <div class="box-footer">
                <button class="btn-sm btn btn-primary element-center" type="submit"><span class="fa fa-check"></span> <%=Resources.Einvoice.BtnReport%></button>
            </div>
        </form>
    </div>
</asp:Content>
