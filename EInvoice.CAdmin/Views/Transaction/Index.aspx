<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<TransactionIndexModels>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Danh sách giao dịch
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <script src="/Content/js/BrowserDetectShare.js"></script>
    <div class="box box-primary">
        <div class="box-header with-border">
            <h4 class="box-title"><i class="fa fa-file-o"></i>&nbsp;&nbsp;<%=ViewData["Title"] %></h4>
            <%=Html.Hidden("TypeTran", ViewData["TypeTran"]) %>
        </div>

        <%if (Model.PagedListTransaction != null)
          { %>
        <div class="box-body">
            <div class="row">
                <div class="col-xs-11">
                    <form id="Searchform" method="post" class="form-inline" action="/Transaction/Index?TypeTran=<%=ViewData["TypeTran"]%>">
                        <%=Html.Hidden("TypeTran", ViewData["TypeTran"]) %>
                        <label>Mã giao dịch: </label>
                        <div class="input-group">
                            <div class="input-group-addon">
                                <i class="fa fa-lock"></i>
                            </div>
                            <input id="code" name="code" type="text" class="form-control" style="width: 200px; margin-right: 10px" value="<%=Html.Encode(Model.code)%>" maxlength="64" />
                        </div>
                        <label>Trạng thái:</label>
                        <div class="input-group">
                            <%=Html.DropDownList("status", new[]
                                {
                                    new SelectListItem{Text="Tất cả", Value=TranSactionStatus.Null.ToString(), Selected= (Model.status == TranSactionStatus.Null)},
                                    new SelectListItem{Text="Mới upload", Value=TranSactionStatus.NewUpload.ToString(), Selected= (Model.status == TranSactionStatus.NewUpload)},
                                    new SelectListItem{Text="Đang xử lý", Value=TranSactionStatus.Processing.ToString(),Selected= (Model.status == TranSactionStatus.Processing)},
                                    new SelectListItem{Text="Thành công", Value=TranSactionStatus.Completed.ToString(),Selected= (Model.status == TranSactionStatus.Completed)},
                                    new SelectListItem{Text="Lỗi phát hành", Value=TranSactionStatus.Failed.ToString(),Selected= (Model.status == TranSactionStatus.Failed)},
                                    new SelectListItem{Text="Lỗi file upload", Value=TranSactionStatus.NotComplete.ToString(),Selected= (Model.status == TranSactionStatus.NotComplete)}
                                }, new { style = "width:200px; margin-right:15px", title = "Cần chọn!" ,@class="form-control"})%>
                        </div>
                        <button class="btn-sm btn btn-primary" type="submit"><i class="fa fa-search"></i>&nbsp;&nbsp;Tìm kiếm</button>
                    </form>
                </div>
                <div class="col-sm-1">
                    <a href="/Transaction/Upload?TypeTran=<%=ViewData["TypeTran"] %>" class="btn btn-info btn-sm element-right"><i class="fa fa-plus"></i>&nbsp;&nbsp;Tạo mới</a>
                </div>
            </div>

            <div class="row">
                <div class="col-xs-12 table-responsive">

                    <table class="table table-bordered table-hover">
                        <thead>
                            <tr>
                                <th width="20px">STT
                                </th>
                                <th width="250px">Mã giao dịch
                                </th>
                                <th width="120px">Mẫu số
                                </th>
                                <th width="85px">Ký hiệu
                                </th>
                                <th width="120px">Tài khoản</th>
                                <th width="150px">Ngày giao dịch</th>
                                <th width="120px">Trạng thái</th>
                                <th style="width: 80px;">Kết quả</th>
                                <th style="width: 80px;">File lỗi
                                </th>
                                <th style="width: 80px;">
                                    <%=Resources.Einvoice.LblDetail%>
                                </th>
                                <th style="width: 20px;">
                                    <%=Resources.Einvoice.LblDelete%>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <%                    
          List<Transaction> cuslst = Model.PagedListTransaction.ToList();
          int i = Model.PagedListTransaction.PageIndex * Model.PagedListTransaction.PageSize + 1;

          foreach (var cus in cuslst)
          {
                            %>
                            <tr>
                                <td style="text-align: right"><%=i%></td>
                                <td style="text-align: center"><%=cus.id%></td>
                                <td style="text-align: center"><%=Html.Encode(cus.InvPattern)%></td>
                                <td style="text-align: center"><%=Html.Encode(cus.InvSerial)%></td>
                                <td style="text-align: center"><%=Html.Encode(cus.AccountName)%></td>
                                <td style="text-align: center"><%=cus.CreateDate.ToString("dd/MM/yyyy")%></td>
                                <td style="text-align: center"><%=EInvoice.Core.Utils.GetTransactionStatus(cus.Status)%></td>
                                <td style="text-align: center">
                                    <%if (cus.Status != TranSactionStatus.NewUpload && cus.Status != TranSactionStatus.Processing && cus.Status != TranSactionStatus.NotComplete && cus.Status != TranSactionStatus.Failed)
                                      {%>
                                    <a class="dtrans" href="/Transaction/Completed/<%=cus.id%>" title="Download">
                                        <i class="fa fa-download"></i></a>
                                    <%}
                                      else
                                      { %>
                                    <i class="fa fa-lock"></i>
                                    <%} %>
                                </td>

                                <td style="text-align: center">
                                    <%if (cus.Status == TranSactionStatus.NotComplete || cus.Status == TranSactionStatus.Failed)
                                      {%>
                                    <a class="dftrans" href="/Transaction/Download/<%=cus.id%>" title="Download">
                                        <i class="fa fa-download"></i></a>
                                    <%}
                                      else
                                      { %>
                                    <i class="fa fa-lock"></i>
                                    <%} %>
                                </td>

                                <td style="text-align: center"><a href="/Transaction/Details?id=<%=cus.id%>&TypeTran=<%=ViewData["TypeTran"] %>" title="Edit">
                                    <i class="fa fa-eye"></i></a></td>
                                <%if (cus.Status != TranSactionStatus.Processing)
                                  {%>
                                <td style="text-align: center"><a href="#" onclick="OnDelete('<%=cus.id%>')" title="Xóa">
                                    <i class="fa fa-trash"></i></a></td>
                                <%}
                                  else
                                  { %>
                                <td style="text-align: center"><a href="#" title="Khóa">
                                    <i class="fa fa-lock"></i></a></td>
                                <%} %>
                            </tr>
                            <% i++;
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
                    <%=Html.Pager(Model.PagedListTransaction.PageSize, Model.PagedListTransaction.PageIndex + 1, Model.PagedListTransaction.TotalItemCount, new
            {
                action = "index",
                controller = "Transaction",
                keyword = Model.keyword,
                status = Model.status,
                TypeTran=ViewData["TypeTran"]
            })%>
                </div>
            </div>
        </div>
        <%} %>
    </div>
    <script language="javascript" type="text/javascript">
        function OnDelete(id) {
            var typetran = $("#TypeTran").val();
            alertify.confirm("Xoá giao dịch?", function (e) {
                if (e) {
                    document.location = "/Transaction/delete?id=" + id + '&TypeTran=' + typetran;
                }
                else return;
            });
        }

        function OnCompare(id) {
            if (!confirm("Chấp nhận đối soát?"))
                return;
            document.location = "/Transaction/compare?id=" + id;
        }

        $(document).ready(function () {
            // Download ket qua upload thanh cong
            $('.dtrans').click(function () {
                if (confirm('Bạn có muốn tải dưới định dạng dbf không?')) {
                    window.location = $(this).attr('href') + "?downloadtype=dbf";
                } else {
                    window.location = $(this).attr('href') + "?downloadtype=xml";
                }
                return false;
            });
            // Download ket qua upload loi
            $('.dftrans').click(function () {
                window.location = $(this).attr('href') + "?downloadtype=xml";
                return false;
            });
        });
    </script>

</asp:Content>
