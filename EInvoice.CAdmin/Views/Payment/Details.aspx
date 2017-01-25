<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PaymentTransaction>" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thanh toán hóa đơn
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
    <link href="/Content/css/redmond/jquery-ui-1.8.12.custom.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
    <script type="text/javascript" src="/Content/js/Share.js"></script>
    <style type="text/css">
        fieldset {
            margin: 20px 50px 10px 50px !important;
        }

            fieldset label {
                float: left;
                margin-right: 0px;
                min-width: 289px;
            }

            fieldset li {
                width: 108%;
            }
    </style>

    <fieldset>
        <legend>Thông tin giao dịch </legend>
        <ol>
            <li>
                <label for="Phone">
                    1. Công ty</label>
                <%=Html.Encode(Model.Company.Name)%>
            </li>
            <li>
                <label for="Code">
                    2.  Mã giao dịch:
                </label>
                <%=Html.Encode(Model.id)%>
            </li>
            <li>
                <label for="AccountName">
                    5. Trạng thái:
                </label>
                <%=EInvoice.Core.Utils.GetPaymentTransactionStatus(Model.Status)%>
            </li>
            
            <li>
                <label for="Address">
                    6. Thông báo từ hệ thống:
                </label>
            </li>
            <li>
                <%if (!string.IsNullOrEmpty(Model.Messages))
                  {%>
                <%=Html.TextArea("Messages", Model.Messages, new { @style = "width:800px;height:200px" })%>
                <%} %>
            </li>
            <li>
                <a href="/Payment/PaymentTransactionIndex">
                    <img src="/Content/Images/Back-icon.png" /></a>
            </li>
        </ol>
    </fieldset>
</asp:Content>
