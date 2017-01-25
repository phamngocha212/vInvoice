<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IInvoice>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="EInvoice.TourisExtends.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Điều chỉnh hóa đơn
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
    <%using (Html.BeginForm("CreateReplaceInv", "AdJust", FormMethod.Post, new { enctype = "multipart/form-data" }))
      {%>
    <%=Html.Hidden("type",ViewData["type"])%>
    <%Html.RenderPartial("TOURISVATShare", Model);%>
    <div class="textc">
        <%if ((int)ViewData["SignPlugin"] > 0)
          {%>
        <button class="btn btn-sm btn-primary" type="button" onclick="publishReplaceInvByPlugIn()"><i class="fa fa-check"></i><%=Resources.Einvoice.BtnEdit%></button>
        <%}
          else
          {%>
        <button class="btn btn-sm btn-primary" type="submit" onclick="submitForm()"><i class="fa fa-check"></i><%=Resources.Einvoice.BtnEdit%></button>
        <%} %>
        <button class="btn btn-sm" type="button" onclick="document.location = '/AdJust/SearchReplaceInv'">
            <i class="fa fa-backward"></i>Quay lại</button>
    </div>
    <%}%>
</asp:Content>
