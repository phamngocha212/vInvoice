<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<user>" %>

<%@ Import Namespace="IdentityManagement.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thêm tài khoản có quyền [ServiceRole]
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% using (Html.BeginForm("CreateNewServiceRole", "Account", FormMethod.Post))
       { %>
    <% Html.RenderPartial("RoleServiceUseControl", Model); %>    
    <div class="textc">
        <button class="btn btn-sm btn-primary" type="submit"><i class="fa fa-check"></i>Tạo mới</button>
        <button class="btn btn-sm" type="button" onclick="document.location = '/Account/ServiceRoleIndex'">
            <i class="fa fa-backward"></i>Quay lại</button>
    </div>
    <% } %>

</asp:Content>
