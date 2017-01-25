<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IInvoice>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Tạo, phát hành hóa đơn
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%using (Html.BeginForm("Create", "EInvoice", FormMethod.Post))
      {%>
    <%Html.RenderPartial("PSVVATShare", Model);%>
    <div class="textc">        
        <button class="btn btn-sm btn-success" type="submit" id="submit" ><i class="fa fa-check"></i>Tạo mới</button>       
        <button class="btn btn-sm" type="button" onclick="document.location = '/PSVInvoice/Index'"><i class="fa fa-backward"></i>Quay lại</button>
    </div>
    <%}%>      
</asp:Content>
