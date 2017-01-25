<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<EInvoice.CAdmin.Models.RoleModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Tạo quyền người dùng
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% using (Html.BeginForm("CreateRole","IDManager", FormMethod.Post))
      { %>
      <% Html.RenderPartial("IDMShareElement", Model); %>
    <div class="text-center">
      <button class="btn btn-sm" type="submit"><i class="fa fa-check"></i>Tạo mới</button>        
        <button class="btn btn-sm" type="button" onclick="document.location = '/IDManager/index'">
            <i class="fa fa-backward"></i> Quay lại</button>  
        </div>      
   <% } %>

</asp:Content>
