<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<PublishModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thông báo phát hành
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <%Html.RenderPartial("RPublishShareElement", Model); %>
        <div class="text-center">
            <button class="btn btn-sm btn-primary" type="button" onclick="Save_Click()"><i class="fa fa-check"></i>Lưu</button>
            <button class="btn btn-sm btn-default" type="button" onclick="document.location = '/Publish/Index'">
                <i class="fa fa-backward"></i>Quay lại</button>
        </div>
    </div>
    <script language="javascript" type="text/javascript">
        function Save_Click() {
            $("#MainForm").attr("action", "/Publish/UpdatePublish");
            $("#MainForm").submit();
        }
    </script>
</asp:Content>
