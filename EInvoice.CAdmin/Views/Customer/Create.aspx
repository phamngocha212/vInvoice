<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<CustomerModel>" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thêm mới khách hàng
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%using (Html.BeginForm("Create", "Customer", FormMethod.Post))
      {%>  
        <%Html.RenderPartial("CtmShareElement", Model);%>
    <div class="text-center">
        <button class="btn btn-sm btn-primary" type="button" onclick="test()"><i class="fa fa-check"></i>Tạo mới</button>        
        <button class="btn btn-sm" type="button"onclick="document.location = '/Customer/index'">
            <i class="fa fa-backward"></i>Quay lại</button> 
        </div>        
    <%}%>
    <script type="text/javascript">
        function test() {
            if ($("form:first").valid()) {
                if ($('#TaxCode').val() != "")
                    checkMST($('#TaxCode').val(), 'save');
                else document.forms[0].submit();
            }
        }
    </script>
</asp:Content>
