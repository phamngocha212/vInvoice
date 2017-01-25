<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<Staff>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Quản lý nhân viên
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%using (Html.BeginForm("Create", "Staff", FormMethod.Post))
      {%>
    <%Html.RenderPartial("staffShareElement", Model);%>
    <div class="textc">
        <button class="btn btn-sm btn-primary" type="submit"><i class="fa fa-check"></i>&nbsp;Tạo mới</button>
        <button class="btn btn-sm" type="button" onclick="document.location = '/Staff/index'">
            <i class="fa fa-backward"></i>&nbsp;Quay lại</button>
    </div>
    <%}%>
</asp:Content>

