<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<RegisterTempModels>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Chỉnh sửa mẫu hóa đơn
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="box box-danger">
        <div class="box-header with-border">
            <h4 class="box-title"><i class="fa fa-file-text"></i>CHỈNH SỬA MẪU HÓA ĐƠN</h4>
        </div>
        <% using (Html.BeginForm("Edit", "RegisterTemp", FormMethod.Post, new { enctype = "multipart/form-data" }))
           { %>
        <% Html.RenderPartial("RegisterTempUserControl", Model); %>
        <div class="box box-footer" style="text-align: center">
            <button class="btn btn-sm btn-primary" type="submit"><i class="fa fa-check"></i>Lưu mẫu</button>
            <button class="btn btn-sm" type="button" onclick="document.location='/RegisterTemp/Index'">
                <i class="fa fa-backward"></i>Quay lại</button>
        </div>

        <% } %>
    </div>
</asp:Content>
