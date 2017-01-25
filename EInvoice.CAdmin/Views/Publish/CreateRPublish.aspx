<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<PublishModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Tạo thông báo phát hành
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <%Html.RenderPartial("RPublishShareElement", Model); %>
        <div class="text-center">
            <button class="btn btn-sm btn-primary" type="button" onclick="Save_Click()"><i class="fa fa-check"></i>Tạo mới</button>
            <button class="btn btn-sm btn-default" type="button" onclick="document.location = '/Publish/Index'">
                <i class="fa fa-backward"></i>Quay lại</button>
        </div>
    </div>
    <script type="text/javascript">
        function Save_Click() {
            $("#MainForm").attr("action", "/Publish/CreateRPublish");
            $("#MainForm").submit();
        }
        $("#Delimiter").val("Đăng ký sử dụng dấu phân cách là dấu chấm(.) sau chữ số hàng nghìn, triệu, tỷ, nghìn tỷ, triệu tỷ, tỷ tỷ và sử dụng dấu phẩy(,) sau chữ số hàng đơn vị để ghi chữ số sau chữ số hàng đơn vị");
    </script>
</asp:Content>
