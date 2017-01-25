<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<DecisionModels>" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Quyết định phát hành
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%Html.RenderPartial("DecisionShareElement", Model); %>       
    <div class="text-center">
        <br />
          <button class="btn btn-primary btn-sm" type="button" onclick="Save_Click()"><i class="fa fa-save"></i>Tạo mới</button>
          <button class="btn btn-default btn-sm" type="button" onclick="document.location = '/Publish/ListDecision'">
        <i class="fa fa-backward"></i>Quay lại</button>     
    </div>
       
  <script language="javascript" type="text/javascript">
      function Save_Click() {
          $("#MainForm").attr("action", "/Publish/CreateDecision");
          $("#MainForm").submit();
      }
  </script>
</asp:Content>
