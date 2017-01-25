<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Tình hình sử dụng hóa đơn
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <script src="/Content/js/jquery.validate.min.js" type="text/javascript"></script>
    <%
        SelectList cl = Model;
    %>
    <form method="get" action="/ReportsInv/ReportsPrint" id="ReportPrint">
        <div style="width: 500px; margin: 20px auto; border: 1px solid #ccc; border-radius: 5px; padding-top: 10px">
            <div class="box-header with-border">
                <h4 class="box-title">
                    <i class="fa fa-paper-plane"></i>KỲ LẬP BÁO CÁO
                </h4>
            </div>
            <table style="margin: 10px auto">
                <tr>
                    <td>
                        <%=Resources.Einvoice.ReUse_LblQuarter%>:
                                <select name="quarter" class="required" title="Chọn quý!" style="width: 100px">
                                    <option id="Option2" value="1"><%=Resources.Einvoice.ReUse_ReqQuarterOne%></option>
                                    <option id="Option3" value="2"><%=Resources.Einvoice.ReUse_ReqQuarterTwo%></option>
                                    <option id="Option4" value="3"><%=Resources.Einvoice.ReUse_ReqQuarterThree%></option>
                                    <option id="Option5" value="4"><%=Resources.Einvoice.ReUse_ReqQuarterFour%></option>
                                </select>
                    </td>
                    <td>
                        <%=Resources.Einvoice.ReUse_Year%>:<%=Html.DropDownList("year", cl, new { @style = "width:80px ", @class = "required", title = "Chọn năm!" })%>
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
