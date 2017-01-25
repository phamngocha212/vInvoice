<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<Transaction>" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Chi tiết giao dịch
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />        
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
                <label for="Code">
                    1.  Mã giao dịch:
                </label>
                <%=Html.Encode(Model.id)%>
            </li>
            <li>
                <label for="TaxCode">
                    2. Mẫu số:
                </label>
                <%=Html.Encode(Model.InvPattern)%>  
            </li>
            <li>
                <label for="Name">
                    3. Ký hiệu:
                </label>
                <%=Html.Encode(Model.InvSerial)%>  
            </li>
            <li>
                <label for="AccountName">
                    4. Trạng thái:
                </label>
                <%=EInvoice.Core.Utils.GetTransactionStatus(Model.Status)%>
            </li>
            
            <li>
                <label for="Address">
                    5. Thông báo từ hệ thống:
                </label>
            </li>
            <li>
                <%if (!string.IsNullOrEmpty(Model.Messages))
                  {%>
                <%=Html.TextArea("Messages", Model.Messages, new { @style = "width:800px;height:200px" })%>
                <%} %>
            </li>
            <li>
                <a href="/PsvTransaction/index?TypeTran=<%=ViewData["TypeTran"] %>">
                    <button class="btn btn-sm" type="button"><i class="fa fa-backward"></i>Quay lại</button></a>                 
            </li>
        </ol>
    </fieldset>
</asp:Content>
