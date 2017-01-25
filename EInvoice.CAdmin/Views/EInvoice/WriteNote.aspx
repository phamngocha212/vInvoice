<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<WriteNoteModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Ghi chú hóa đơn
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
    <script type="text/javascript" src="/Content/js/Share.js"></script>
    <%using (Html.BeginForm("WriteNote", "EInvoice", FormMethod.Post))
      {%>
    <%=Html.Hidden("id",Model.id) %>
    <%=Html.Hidden("pattern", Model.pattern)%>
    <%=Html.Hidden("TypeView", Model.TypeView)%>
    <div class="row">
        <div class="box box-danger">
            <div class="box-header with-border">
                <h4 class="box-title"><i class="fa fa-paste"></i>THÊM THÔNG TIN GHI CHÚ</h4>
            </div>

            <div class="box-body">
                <fieldset>
                    <ol>
                        <li>Ghi chú hóa đơn<span style="color: red">(*)</span>
                        </li>
                        <li>
                            <%=Html.TextArea("Note", Model.Note, new {@style="height:120px;" ,@class = "required form-control", title="Thêm vào ghi chú!"})%>
                        </li>
                    </ol>
                </fieldset>
            </div>
            <div class="text-center">
                <%if (Model.TypeView == "0")
                  {%>
                <button class="btn btn-default" onclick="window.history.back();"><i class="fa fa-backward"></i><%=Resources.Einvoice.BtnBack%></button>
                <%}
                  else
                  {%>
                <button class="btn btn-primary" type="submit"><i class="fa fa-save"></i>Lưu</button>
                <%} %>
            </div>
        </div>
    </div>
    <%}%>
    <script type="text/javascript">
        $(document).ready(function () {
            $('form:first').validate();
        });
    </script>
</asp:Content>
