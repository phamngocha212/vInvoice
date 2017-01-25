<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DisPaymentTransactionIndexModels>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thanh toán hóa đơn
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <div class="widget-header1">
        Quản lý hủy đánh dấu gạch nợ
    </div>
    <!--End div widget-header1-->
    <div style="padding: 10px 5px 10px 5px; float: left; vertical-align: bottom">
        <form id="Searchform" method="post" action="/Payment/PaymentTransactionIndex">
            <b>Mã giao dịch: </b>
            <input id="key" name="key" type="text" style="width: 200px; margin-left: 10px; margin-right: 10px" value="<%=Html.Encode(Model.key)%>" />
            <b style="margin-right:10px">Trạng thái: </b>
            <%=Html.DropDownList("status", new[]
                        {
                            new SelectListItem{Text="Tất cả", Value=TranSactionStatus.Null.ToString(), Selected= (Model.status == PaymentTransactionStatus.Null)},
                            new SelectListItem{Text="Mới upload", Value=TranSactionStatus.NewUpload.ToString(), Selected= (Model.status == PaymentTransactionStatus.NewUpload)},
                            new SelectListItem{Text="Đang xử lý", Value=TranSactionStatus.Processing.ToString(),Selected= (Model.status == PaymentTransactionStatus.Processing)},
                            new SelectListItem{Text="Thành công", Value=TranSactionStatus.Completed.ToString(),Selected= (Model.status == PaymentTransactionStatus.Completed)},
                            new SelectListItem{Text="Lỗi gạch nợ", Value=TranSactionStatus.Failed.ToString(),Selected= (Model.status == PaymentTransactionStatus.Failed)},
                            new SelectListItem{Text="Lỗi file upload", Value=TranSactionStatus.NotComplete.ToString(),Selected= (Model.status == PaymentTransactionStatus.NotComplete)}
                        }, new { style = "width:200px; margin-right:15px", title = "Cần chọn!" })%>
            <button class="btn-sm" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button>
        </form>
    </div>
    <div style="padding: 10px 0px 10px 5px; float: right">
        <a href="/Payment/UploadDisPayment/" title="Create">
            <img alt="" src="/Content/Images/add.jpg" /></a>
    </div>
    <div class="clear">
    </div>

    <table style="width: 100%; min-width: 800px" class="grid">
        <thead>
            <tr>
                <th width="20px">STT
                </th>
                <th width="250px">Mã giao dịch
                </th>
                <th width="150px">Tài khoản
                </th>
                <th width="150px">Bắt đầu giao dịch
                </th>
                <th width="100px">Trạng thái
                </th>
                <th style="border-right-color: #EEE; width: 40px;">Kết quả</th>
                <th style="border-right-color: #EEE; width: 40px;">File lỗi
                </th>
                <th style="border-right-color: #EEE; width: 20px;">
                    <%=Resources.Einvoice.LblDetail%>
                </th>
                <th style="border-right-color: #EEE; width: 20px;">
                    <%=Resources.Einvoice.LblDelete%>
                </th>
            </tr>
        </thead>
        <tbody>

            <%if (Model.PagedListTransaction != null)
      { %>

            <%                    
          List<PaymentTransaction> paymentTranLst = Model.PagedListTransaction.ToList();
          int i = Model.PagedListTransaction.PageIndex * Model.PagedListTransaction.PageSize + 1;
          foreach (var cus in paymentTranLst)
          {
            %>
            <tr>
                <td align="center"><%=i%></td>
                <td align="center"><%=cus.id%></td>
                <td align="center"><%=Html.Encode(cus.AccountName)%></td>
                <td align="center"><%=cus.CreateDate.ToString("dd/MM/yyyy")%></td>
                <td align="center"><%=EInvoice.Core.Utils.GetPaymentTransactionStatus(cus.Status)%></td>
                <td align="center">
                    <%if (cus.CompleteResult != null)
                      {%>
                    <a href="/Payment/Completed/<%=cus.id%>" title="Download">
                        <img alt="" src="/Content/Images/Download-icon.png" /></a>
                    <%}
                      else
                      { %>
                    <img alt="" src="/Content/Images/lock.png" />
                    <%} %>
                </td>
                <td style="text-align: center">
                    <%if (cus.FailResult != null)
                      {%>
                    <a href="/Payment/Download/<%=cus.id%>" title="Download">
                        <img alt="" src="/Content/Images/Download-icon.png" /></a>
                    <%}
                      else
                      { %>
                    <img alt="" src="/Content/Images/lock.png" />
                    <%} %>
                </td>
                <td style="text-align: center"><a href="/Payment/Details/<%=cus.id%>" title="Edit">
                    <img alt="" src="/Content/Images/detail1.png" /></a></td>
                <%if (cus.Status != PaymentTransactionStatus.Processing)
                  {%>
                <td style="text-align: center"><a href="#" onclick="OnDelete('<%=cus.id%>')" title="Delete">
                    <img alt="" src="/Content/Images/no.png" /></a></td>
                <%}
                  else
                  { %>
                <td style="text-align: center"><a href="#" title="Delete">
                    <img alt="" src="/Content/Images/lock.png" /></a></td>
                <%} %>
            </tr>
            <% i++;
          }
            %>


            <%} %>

        </tbody>
    </table>
    <div class="pager">

        <%if (Model.PagedListTransaction != null)
      { %>


        <%=Html.Pager(Model.PagedListTransaction.PageSize, Model.PagedListTransaction.PageIndex + 1, Model.PagedListTransaction.TotalItemCount, new
            {
                action = "PaymentTransactionIndex",
                controller = "Payment",
                keyword = Model.key,
                status = Model.status
            })%>

         <%} %>

    </div>
    <script language="javascript" type="text/javascript">
        function OnDelete(id) {            
            alertify.confirm("Xoá giao dịch?", function (e) {
                if (e) {
                    document.location = "/Payment/delete?id=" + id;
                }
                else return;
            });
        }
    </script>

</asp:Content>
