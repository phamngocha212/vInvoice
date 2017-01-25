<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<VNIOModel>" %>

<%@ Import Namespace="EInvoice.VNIOExtends.Models" %>
<asp:Content ID="errorTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%=Model.Title %>
</asp:Content>

<asp:Content ID="errorContent" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm("VNIOReport", "VNIOReport", FormMethod.Post))
        { %>

    <%=Html.Hidden("Code", Model.Code) %>
    <div class="col-md-6 col-md-offset-3">
        <div class="box box-primary">
            <div class="box-header">
                <h4 class="box-heading"><%=Model.Title %></h4>
            </div>
            <div class="box-body">
                <div class="row">
                    <div class="form-group">
                        <label class="col-sm-4">
                            Ngày báo cáo:
                        </label>
                        <div class="col-sm-8">
                            <div class="input-group">
                                <div class="input-group-addon">
                                    <i class="fa fa-calendar"></i>
                                </div>
                                <%=Html.TextBox("ReportDate", Model.ReportDate.ToShortDateString(), new { @class="form-control datepicker"}) %>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="box-footer" style="text-align:center">
                <button type="submit" class="btn btn-primary"><i class="fa fa-save"></i>Báo cáo</button>
            </div>
        </div>
    </div>
    <%} %><script>
              $(document).ready(function () {
                  $(".datepicker").datepicker({
                      showOn: "button",
                      format: "dd/mm/yyyy",
                      buttonImageOnly: true,
                      changeMonth: true,
                      changeYear: true,
                      autoclose: true
                  });
              });
    </script>
</asp:Content>
