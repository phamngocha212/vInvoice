<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<SendMail>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Gửi mail khách hàng
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css">
    <script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
    <%using (Html.BeginForm("SendAgain", "SendMail", FormMethod.Post))
      {%>
    <%=Html.Hidden("id", Model.id)%>
    <fieldset>
        <legend>Thông tin mail</legend>
        <ol>
            <li>
                <label for="Subject">
                    Chủ đề</label>
                <%=Html.TextBox("Subject", Model.Subject, new { style = "width:500px", @class = "required", title = "Nhập chủ đề!", @maxlength="200"  })%>
            </li>
            <li>
                <label for="EmailFrom">
                    Mail Người gửi</label>
                <%=Html.TextBox("EmailFrom", Model.EmailFrom, new { style = "width:200px", @class = "required email", title = "Nhập người gửi", @maxlength="100"  })%>
            </li>
            <li>
                <label for="Email">
                    Mail Người nhận
                </label>
                <%=Html.TextBox("Email", Model.Email, new { style = "width:200px", @class = "required email", title = "Nhập người nhận.", @maxlength="100"  })%>
            </li>
    </fieldset>    
    <button class="btn btn-sm" type="submit"><i class="fa fa-send"></i> Gửi lại</button>
    <button class="btn btn-sm" type="button" onclick="document.location = '/SendMail/Index'">
        <i class="fa fa-backward"></i>Quay lại</button>
    <%}%>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('form:first').validate();
        });
    </script>
</asp:Content>

