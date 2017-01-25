<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<LogSystemModels>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Ghi vết hệ thống
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>

    <div class="row">
        <div class="col-xs-12">
            <div class="box box-danger">
                <div class="box-header with-border">
                    <h4 class="box-title"><i class="fa fa-file-text"></i>Ghi vết hệ thống</h4>
                </div>
                <div class="box-body">
                    <div class="row">
                        <div class="col-xs-12">
                            <form id="Searchform" method="post" action="/LogManager/Index" class="form-inline">

                                <label>Ngày cập nhật</label>
                                <div class="input-group">
                                    <div class="input-group-addon">
                                        <i class="fa fa-calendar"></i>
                                    </div>
                                    <%=Html.TextBox("DateModify", Model.DateModify, new {@class = "datepicker form-control ",@placeholder="__/__/____" })%>
                                </div>
                                <label>Tìm kiếm</label>
                                <%=Html.TextBox("Keysearch", Model.Keysearch, new {@class="form-control", style = "width:150px; margin:0px 5px 0px 0px"})%>
                                <button class="btn-sm btn btn-primary" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button>
                            </form>
                        </div>

                    </div>
                    <div class="row">
                        <div class="col-xs-12 table-responsive">

                            <table class="table table-bordered table-hover">
                                <thead>
                                    <tr>
                                        <th width="40px">STT
                                        </th>
                                        <th>Tên
                                        </th>
                                        <th width="120px">Cập nhật cuối
                                        </th>
                                        <th width="100px">Dung lượng
                                        </th>
                                        <th width="100px">Chi tiết
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%    
                                        List<FileInfo> cuslst = Model.LogsInfo.ToList();
                                        int i = 1;
                                        foreach (var cus in cuslst)
                                        {
                                            if (cus.Name == ".CurrentLog") { continue; }
                                    %>
                                    <tr>
                                        <td style="text-align: right"><%=i%></td>
                                        <td><%=Html.Encode(cus.Name)%></td>
                                        <td style="text-align: center"><%=cus.LastWriteTime.ToString("dd/MM/yyyy")%></td>
                                        <td style="text-align: center"><%=cus.Length%></td>
                                        <td style="text-align: center"><a href="#" onclick="d('<%=cus.Name %>')">
                                            <i class="fa fa-download"></i></a></td>
                                    </tr>
                                    <% i++;
                                        }%>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>    
    <script type="text/javascript">

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
        function d(name) {
            window.location = "/LogManager/Download?name=" + name;
        }
    </script>
</asp:Content>
