<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<MailsIndexModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Danh sách mail
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>

    <div class="row">
        <div class="col-xs-8 col-xs-offset-2">
            <div class="box box-primary">
                <form id="Searchform" method="post" class="form-horizontal" action="/SendMail/index">
                    <div class="box-body">
                        <div class="row">
                            <div class="col-xs-6">
                                <div class="form-group">
                                    <label class="col-sm-4">Chủ đề:</label>
                                    <div class="col-sm-8"><%=Html.TextBox("Subject", "", new { @class="form-control", @maxlength="200" })%></div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4">Từ ngày:</label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon">
                                                <i class="fa fa-calendar"></i>
                                            </div>
                                            <%=Html.TextBox("FromSendedDate", Model.FromSendedDate == null ? "" : Model.FromSendedDate.ToString(), new {  @placeholder="__/__/____",  @class = "datepicker form-control"})%>
                                        </div>

                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4">Mail người nhận:</label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon">
                                                <i class="fa fa-envelope"></i>
                                            </div>
                                            <%=Html.TextBox("EmailTo", "", new { @class="form-control", @maxlength="200" })%>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xs-6">
                                <div class="form-group">
                                    <label class="col-sm-4">Trạng thái:</label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Status", new[]
                                  {
                                      new SelectListItem{Text="--Tất cả--", Value="-1", Selected=(Model.Status == -1)},
                                      new SelectListItem{Text="Chưa gửi", Value="0", Selected=(Model.Status == 0)},
                                      new SelectListItem{Text="Đã gửi", Value="1", Selected=(Model.Status == 1)},
                                      new SelectListItem{Text="Gửi lỗi", Value="2", Selected=(Model.Status == 2)},
                                      new SelectListItem{Text="Gửi lại", Value="3", Selected=(Model.Status == 3)},
                                  }, new {  @class="form-control", title = "Chọn tình trạng chuyển đổi!" })%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4">Đến ngày:</label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon">
                                                <i class="fa fa-calendar"></i>
                                            </div>
                                            <%=Html.TextBox("ToSendedDate", Model.ToSendedDate == null ? "" : Model.ToSendedDate.ToString(), new {@placeholder="__/__/____", @class = "datepicker  form-control" })%>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="box-footer">
                         <button class="btn-sm btn btn-primary element-center" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button>
                </div>
                </form>
            </div>
        </div>
    </div>
    <form id="danhsachHD" method="post" action="/SendMail/SelectSendAgain">
        <div class="row">
            <div class="col-xs-12">
                <div class="box box-danger">
                    <div class="box-header with-border">
                        <h4 class="box-title"><i class="fa fa-envelope"></i>Danh sách mail</h4>
                    </div>
                    <div class="box-body table-responsive">
                        <table class="table table-bordered table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 40px">STT</th>
                                    <th>Mail người nhận</th>
                                    <th>Chủ đề</th>
                                    <th>Ngày gửi</th>
                                    <th>Trạng thái</th>
                                    <th>Gửi lại</th>
                                    <th style="width: 40px">Xóa</th>
                                    <th style="width: 40px">Chọn</th>
                                </tr>
                            </thead>
                            <tbody>
                                <%
                                    List<SendMail> lstMail = Model.PageListMail.ToList();
                                    int i = Model.PageListMail.PageIndex * Model.PageListMail.PageSize + 1;
                                    foreach (var item in lstMail)
                                    {
                                %>
                                <tr>
                                    <td style="text-align: right"><%=i %></td>
                                    <td><%=Html.Encode(item.Email) %></td>
                                    <td><%=Html.Encode(item.Subject) %></td>
                                    <td><%=item.SendedDate ==Enumerations.MinDate? "" : String.Format("{0:dd/MM/yyyy}", item.SendedDate)%></td>
                                    <%if (item.Status == 1 && !string.IsNullOrEmpty(item.Note))
                                      {%>
                                    <td><%=item.Note %></td>
                                    <%}
                                      else if (item.Status == 1 && string.IsNullOrEmpty(item.Note))
                                      {%>
                                    <td style="text-align: center">Đã gửi</td>
                                    <%}
                                      else if (item.Status == 2)
                                      {%>
                                    <td style="text-align: center">Gửi lỗi</td>
                                    <%}
                                      else if (item.Status == 3)
                                      {%>
                                    <td style="text-align: center">Chờ gửi lại</td>
                                    <%}
                                      else if (item.Status == 0)
                                      {%>
                                    <td style="text-align: center">Chưa gửi</td>
                                    <%} %>
                                    <td style="text-align: center"><a href="/SendMail/Edit/<%=item.id%>" title="Edit">
                                        <i class="fa fa-send"></i></a></td>
                                    <td style="text-align: center"><a href="#" onclick="OnDelete('<%=item.id %>')" title="Delete">
                                        <i class="fa fa-trash"></i></a></td>
                                    <td style="text-align: center">
                                        <input name="cbid" type="checkbox" value="<%=item.id%>" /></td>
                                </tr>
                                <%i++;
                    }%>
                            </tbody>
                        </table>
                    </div>
                    <div class="box-footer">
                        <div class="pager">
                            <div class="page-a">
                                <%=Html.Pager(Model.PageListMail.PageSize, Model.PageListMail.PageIndex + 1, Model.PageListMail.TotalItemCount, new
            {
                action = "Index",
                controller = "SendMail",
                Subject = Model.Subject,
                FromSendedDate = Model.FromSendedDate,
                ToSendedDate = Model.ToSendedDate,
                Status = Model.Status,
                EmailTo = Model.EmailTo,
            })%>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-xs-12">
                <div class="element-right">
                    <%if (Model.PageListMail.Count > 0)
                      { %>
                    <button class="btn btn-sm btn-success" type="submit" style="margin-right: 27px"><span class="fa fa-refresh"></span>Gửi lại mail</button>
                    <button class="btn btn-sm btn-warning" type="button" style="margin-left: 0px; width: 100px;" onclick="document.location ='/SendMail/DownloadExcelMail?Subject='+'<%=Model.Subject %>'+'&Status='+'<%=Model.Status%>'+'&FromSendedDate='+'<%=Model.FromSendedDate%>'+'&ToSendedDate='+'<%=Model.ToSendedDate %>'+'&EmailTo='+'<%=Model.EmailTo %>'">
                        <span class="fa fa-download"></span>Tải file Excel</button>
                    <%}%>
                </div>
            </div>
        </div>



    </form>
    <script type="text/javascript">
        function OnDelete(id) {
            alertify.confirm("Bạn có xóa mail này không?", function (e) {
                if (e) {
                    document.location = "/SendMail/delete?id=" + id;
                }
                else return;
            });
        }
        
        //dịnh dạng lại datepicker theo tieng viet        
       
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
