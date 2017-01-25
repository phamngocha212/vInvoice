<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Lỗi dữ liệu
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Lỗi dữ liệu</h2>
    <p>
        <%=ViewBag.Message %>
    </p>
</asp:Content>
