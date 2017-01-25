<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<AccountModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Tạo mới người dùng hệ thống
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
    <% using (Html.BeginForm("New", "Account", FormMethod.Post))
       { %>
    <% Html.RenderPartial("AccountShareElement", Model); %>
    <div class="textc">
        <button class="btn btn-sm btn-primary" type="submit"><i class="fa fa-check"></i>Lưu</button>
        <button class="btn btn-sm" type="button" onclick="document.location = '/Account/index'">
            <i class="fa fa-backward"></i>Quay lại</button>
    </div>
    <% } %>
        </div>
</asp:Content>
