<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<RegisterTempChoiseModels>" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Chọn mẫu hóa đơn
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% using (Html.BeginForm("ChooseTemp", "RegisterTemp", FormMethod.Get))
       { %>
    <% Html.RenderPartial("InvRegTempShareElement", Model); %>        
    <% } %> 
</asp:Content>
