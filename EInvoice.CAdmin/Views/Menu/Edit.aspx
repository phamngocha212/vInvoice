<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<EInvoice.Core.Domain.Menu>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Cập nhật chức năng
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% using (Html.BeginForm("Edit", "Menu", FormMethod.Post))
       { %>
    <% Html.RenderPartial("MenuUseControl", Model); %>      
    <div class=" col-xs-12 text-center">
        <button class="btn btn-sm btn-primary" type="submit"><i class="fa fa-check"></i>Lưu</button>
        <button class="btn btn-sm" type="button" onclick="document.location = '/Menu/index'">
            <i class="fa fa-backward"></i>Quay lại</button>
    </div>
    <%}%>

</asp:Content>
