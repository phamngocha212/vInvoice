<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PaymentTransactionIndexModels>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thanh toán hóa đơn
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>       
    <script type="text/javascript" src="/Content/js/Share.js"></script>    
    <h4 style="text-align: center">
        Quản lý đánh dấu gạch nợ
    </h4>
    <hr />
    <%if (Model.PagedListTransaction != null)
      { %>
    <!--End div widget-header1-->
    <div style="border: 1px solid #CCC; width: 700px; height: 140px; border-radius: 5px; margin: 10px auto">
        <form id="Searchform" method="post" action="/Payment/PaymentTransactionIndex">
            <table width="100%" border="0" cellspacing="4" cellpadding="0" style="height: 85px; text-align: left; margin-left: 10px; line-height: 30px;">
                <tr>
                    <th style="width: 130px; text-align: left">
                        <span>Mã giao dịch:</span>
                    </th>
                    <td>
                        <input id="Text2" name="key" type="text" style="width: 200px; margin-right: 10px"
                            value="<%=Html.Encode(Model.key)%>" />
                    </td>
                    <th style="width: 130px; text-align: center">
                        <span>Trạng thái:</span>
                    </th>
                    <td>
                        <%=Html.DropDownList("status", new[]
                        {
                            new SelectListItem{Text="Tất cả", Value=TranSactionStatus.Null.ToString(), Selected= (Model.status == PaymentTransactionStatus.Null)},
                            new SelectListItem{Text="Mới upload", Value=TranSactionStatus.NewUpload.ToString(), Selected= (Model.status == PaymentTransactionStatus.NewUpload)},
                            new SelectListItem{Text="Đang xử lý", Value=TranSactionStatus.Processing.ToString(),Selected= (Model.status == PaymentTransactionStatus.Processing)},
                            new SelectListItem{Text="Thành công", Value=TranSactionStatus.Completed.ToString(),Selected= (Model.status == PaymentTransactionStatus.Completed)},
                            new SelectListItem{Text="Lỗi gạch nợ", Value=TranSactionStatus.Failed.ToString(),Selected= (Model.status == PaymentTransactionStatus.Failed)},
                            new SelectListItem{Text="Không thành công hết", Value=TranSactionStatus.NotComplete.ToString(),Selected= (Model.status == PaymentTransactionStatus.NotComplete)}
                        }, new { style = "width:200px; margin-right:15px", title = "Cần chọn!" })%>
                    </td>
                </tr>
                <tr>
                    <th style="width: 100px; text-align: left">
                        <span>Từ ngày:</span>
                    </th>
                    <td style="width: 158px">
                        <%=Html.TextBox("FromDate", Model.FromDate, new { style = "width:90px; margin:0px 5px 0px 0px", @class = "datepicker vietnameseDate", @placeholder="__/__/____"})%>
                    </td>
                    <th style="width: 116px; text-align: center">
                        <span>Đến ngày:</span>
                    </th>
                    <td>
                        <%=Html.TextBox("ToDate", Model.ToDate, new { style = "width:90px;margin: 0 5px 0 0px", @class = "datepicker vietnameseDate", @placeholder="__/__/____" })%>
                    </td>
                </tr>
                <tr>
                    <th>
                        <span>Người tạo:</span>
                    </th>
                    <td>
                        <%=Html.TextBox("accountName", Model.accountName, new { maxlength="200", style="width: 200px;"})%>
                    </td>
                </tr>
            </table>
            <div style="float: right; margin-right: 44px; margin-top: 15px">
                <button class="btn-sm" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button>
            </div>
        </form>
    </div>
    <div style="padding: 0px 0px 0px 5px; margin-right: 10px; float: right; margin-bottom: 10px">
        <a href="/Payment/Upload/" class="btn btn-info btn-sm"><i class="fa fa-plus"></i>Tạo mới</a>
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
                <th style="border-right-color: #EEE; width: 60px;">Kết quả
                </th>
                <th style="border-right-color: #EEE; width: 60px;">File lỗi
                </th>
                <th style="border-right-color: #EEE; width: 60px;">
                    <%=Resources.Einvoice.LblDetail%>
                </th>
                <th style="border-right-color: #EEE; width: 20px;">
                    <%=Resources.Einvoice.LblDelete%>
                </th>
            </tr>
        </thead>
        <tbody>
            <%                    
          List<PaymentTransaction> paymentTranLst = Model.PagedListTransaction.ToList();
          int i = Model.PagedListTransaction.PageIndex * Model.PagedListTransaction.PageSize + 1;
          foreach (var cus in paymentTranLst)
          {
            %>
            <tr>
                <td align="center">
                    <%=i%>
                </td>
                <td align="center">
                    <%=cus.id%>
                </td>
                <td align="center">
                    <%=Html.Encode(cus.AccountName)%>
                </td>
                <td align="center">
                    <%=cus.CreateDate.ToString("dd/MM/yyyy")%>
                </td>
                <td align="center">
                    <%=EInvoice.Core.Utils.GetPaymentTransactionStatus(cus.Status)%>
                </td>
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
                <td style="text-align: center">
                    <a href="/Payment/Details/<%=cus.id%>" title="Edit">
                        <img alt="" src="/Content/Images/detail1.png" /></a>
                </td>
                <%if (cus.Status != PaymentTransactionStatus.Processing)
                  {%>
                <td style="text-align: center">
                    <a href="#" onclick="OnDelete('<%=cus.id%>')" title="Delete">
                        <img alt="" src="/Content/Images/no.png" /></a>
                </td>
                <%}
                  else
                  { %>
                <td style="text-align: center">
                    <a href="#" title="Delete">
                        <img alt="" src="/Content/Images/lock.png" /></a>
                </td>
                <%} %>
            </tr>
            <% i++;
}
            %>
        </tbody>
    </table>
    <div class="pager">
        <%=Html.Pager(Model.PagedListTransaction.PageSize, Model.PagedListTransaction.PageIndex + 1, Model.PagedListTransaction.TotalItemCount, new
            {
                action = "PaymentTransactionIndex",
                controller = "Payment",
                keyword = Model.key,
                status = Model.status,
                FromDate=Model.FromDate,
                ToDate= Model.ToDate,
                accountName=Model.accountName
            })%>
    </div>
    <%} %>
    <script language="javascript" type="text/javascript">
        function OnDelete(id) {
            if (!confirm("Xoá giao dịch?"))
                return;
            document.location = "/Payment/delete?id=" + id;
        }        
                
        $(document).ready(function () {            
            $(".datepicker").datepicker({
                showOn: "button",                
                buttonImageOnly: true,
                changeMonth: true,
                changeYear: true,
                autoclose: true
            });
        });
    </script>
</asp:Content>
