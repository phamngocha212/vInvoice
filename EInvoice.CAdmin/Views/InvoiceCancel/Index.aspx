<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InvCancelModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Báo cáo hủy
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <% 
        string creater = Model.creater;
    %>

    <div class="row">
        <div class="col-xs-12">
            <div class="box box-danger">
                <div class="box-header with-border">
                    <h4 class="box-title"><i class="fa fa-file-text"></i>Quản lý hủy hóa đơn</h4>
                </div>
                <div class="box-body">
                    <div class="row">
                        <div class="col-xs-11">
                            <form id="Searchform" method="post" action="/InvoiceCancel/Index" class="form-inline">

                                <label style="margin-right: 10px;"><%=Resources.Einvoice.ReCancel_LblPreparedPerson %></label>
                                <div class="input-group">
                                    <div class="input-group-addon"><i class="fa fa-user"></i></div>
                                    <input type="text" id="Text1" name="creater" value="<%=Html.Encode(Model.creater)%>" maxlength="100" class="form-control" />
                                </div>
                                <label class="control-label"><%=Resources.Einvoice.ReCancel_lblFormDate%></label>
                                <div class="input-group ">
                                    <div class="input-group-addon">
                                        <i class="fa fa-calendar"></i>
                                    </div>
                                    <%=Html.TextBox("dateFrom", Model.dateFrom, new {@placeholder="__/__/____",  @class = "form-control datepicker " })%>
                                </div>
                                <label class="control-label"><%=Resources.Einvoice.ReCancel_ToDate%></label>
                                <div class="input-group ">
                                    <div class="input-group-addon">
                                        <i class="fa fa-calendar"></i>
                                    </div>
                                    <%=Html.TextBox("dateTo", Model.dateTo, new {@placeholder="__/__/____", @class = "datepicker form-control "})%>
                                </div>
                                <button class="btn-sm btn btn-primary" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button>

                            </form>
                        </div>
                        <div class="col-xs-1">
                            <a href="/InvoiceCancel/Create/" class="btn btn-success btn-sm element-right"><i class="fa fa-plus"></i>&nbsp;&nbsp;Tạo mới</a>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 table-responsive">
                            <table class="table table-hover table-bordered">
                                <thead>
                                    <tr>
                                        <th style="width: 40px">STT 
                                        </th>
                                        <th>
                                            <%=Resources.Einvoice.ReCancel_LblMethodCancel%> 
                                        </th>
                                        <th style="width: 100px">
                                            <%=Resources.Einvoice.ReCancel_Hour%> 
                                        </th>
                                        <th style="width: 150px">
                                            <%=Resources.Einvoice.ReCancel_Days%> 
                                        </th>
                                        <th style="width: 200px">
                                            <%=Resources.Einvoice.ReCancel_LblPreparedPerson%> 
                                        </th>
                                        <th style="width: 100px">
                                            <%=Resources.Einvoice.LblDetail%> 
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%
                                        List<InvCancel> lstInvCan = Model.PageListIC.ToList();
                                        int i = Model.PageListIC.PageIndex * Model.PageListIC.PageSize + 1;
                                        foreach (InvCancel ic in lstInvCan)
                                        {
                                    %>
                                    <tr>
                                        <td style="text-align: center">
                                            <%=i %>
                                        </td>
                                        <td style="text-align: center">
                                            <%=Html.Encode(ic.Method) %>
                                        </td>
                                        <td style="text-align: center">
                                            <%=ic.Hour %>
                                            <%=Resources.Einvoice.ReCancel_Hour%>
                                            <%=ic.Minute %>
                                            <%=Resources.Einvoice.ReCancel_Minute%> 
                                        </td>
                                        <td style="text-align: center">
                                            <%=ic.CancelDate.ToString("dd/MM/yyyy")%>
                                        </td>
                                        <td style="text-align: center">
                                            <%=Html.Encode(ic.ComPrepared) %>
                                        </td>
                                        <td style="text-align: center">
                                            <a href="/InvoiceCancel/Detail/<%=ic.id%>" title="Details">
                                                <i class="fa fa-eye"></i></a>
                                        </td>
                                    </tr>
                                    <%
                                            i++;
                                        } 
                                    %>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <div class="box-footer">
                    <div class="pager">
                        <div class="page-a">
                            <%=Html.Pager(Model.PageListIC.PageSize, Model.PageListIC.PageIndex + 1, Model.PageListIC.TotalItemCount, new
                            {
                                action = "index",
                                controller = "InvoiceCancel",

                            })%>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script language="javascript" type="text/javascript">
        function OnDelete(id) {
            alertify.confirm("Are you sure?", function (e) {
                if (e) {
                    document.location = "/InvoiceCancel/delete?id=" + id;
                }
                else return;
            });
        }

        $(document).ready(function () {
            // $('form:first').validate();            
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
