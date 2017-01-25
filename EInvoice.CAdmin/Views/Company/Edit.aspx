<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<Company>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Cập nhật thông tin đơn vị
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <% using (Html.BeginForm("Update", "Company", FormMethod.Post, new { enctype = "multipart/form-data" }))
       { %>

    <% Html.RenderPartial("CompanyShareElement", Model); %>
    <div class="textc">
        <button class="btn btn-sm btn-primary" type="submit"><i class="fa fa-check"></i>Lưu</button>
        <button class="btn btn-sm" type="button" onclick="Onclick(<%=Model.id%>)">
            <i class="fa fa-backward"></i>Quay lại</button>
    </div>
    <% } %>

    </div>
    
    <script language="javascript" type="text/javascript">
        function Onclick(id) {
            document.location = "/Company/DetailCurrent?id=" + id;
        }
    </script>
</asp:Content>
