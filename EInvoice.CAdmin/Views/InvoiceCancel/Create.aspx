<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CreateInvCancelModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Báo cáo hủy
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.RenderPartial("InvCancelShareElement", Model); %>
    <div class="text-center">
        <br />
        <button class="btn btn-sm btn-primary" type="button" onclick="s()"><i class="fa fa-save"></i> Lưu</button>
        <button class="btn btn-sm " type="button" onclick="document.location = '/InvoiceCancel/Index'">
            <i class="fa fa-backward"></i> Quay lại</button>
    </div>
    <script language="javascript" type="text/javascript">
        function s() {
            $("#f1").attr("action", "/InvoiceCancel/Create");
            $("#f1").submit();
        }
    </script>
</asp:Content>
