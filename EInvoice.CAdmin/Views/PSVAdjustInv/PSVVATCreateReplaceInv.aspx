﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IInvoice>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thay thế hóa đơn
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
    <%using (Html.BeginForm("CreateReplaceInv", "PSVAdJustInv", FormMethod.Post, new { enctype = "multipart/form-data" }))
      {%>
    <%Html.RenderPartial("PSVVATShareElement", Model);%>
    <div class="textc">        
        <button class="btn btn-sm btn-primary" type="submit"><i class="fa fa-check"></i><%=Resources.Einvoice.BtnEdit%></button>        
        <button class="btn btn-sm" type="button"onclick="document.location = '/PSVAdJustInv/SearchReplaceInv'">
            <i class="fa fa-backward"></i>Quay lại</button>
    </div>
    <%}%>
</asp:Content>