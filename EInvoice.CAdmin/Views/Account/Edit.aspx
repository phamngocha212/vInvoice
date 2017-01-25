<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<AccountModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Cập nhật thông tin người dùng hệ thống
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <% using (Html.BeginForm("Update", "Account", FormMethod.Post))
           { %>
        <% Html.RenderPartial("AccountShareElement", Model); %>
        <div class="textc">
            <button class="btn btn-sm btn-primary" type="submit"><i class="fa fa-check"></i>Lưu</button>
            <button class="btn btn-sm" type="button" onclick="document.location = '/Account/index'">
                <span class="fa fa-backward"></span>Quay lại</button>
        </div>
        <% } %>
    </div>
</asp:Content>
