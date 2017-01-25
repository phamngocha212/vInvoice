<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<CertInvModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Danh sách hóa đơn chứng thực
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>    
    <script src="/Content/js/BrowserDetectShare.js"></script>
    <script src="/Content/js/main.js"></script>
    <div class="widget-header1">
        Danh sách hóa đơn cần chứng thực
    </div>
    <!--End div widget-header1-->
    <div style="padding: 10px 5px 10px 5px; float: left; vertical-align: bottom">
        <form id="Searchform" name="searchForm" method="post" action="/CertifyInv/Index">
            <b><%=Resources.Einvoice.Pattern %> </b>
            <%=Html.DropDownList("Pattern", new SelectList(Model.Patterns,Model.pattern), new { @Style = "Width:110px", onchange="document.searchForm.submit();" })%>
            <b>Trạng thái</b>
            <%=Html.DropDownList("status",new[] 
              {
                  new SelectListItem{Text="--Trạng thái--", Value="-1", Selected= (Model.status== -1)},
                  new SelectListItem{Text="Chưa xác thực", Value="0", Selected= (Model.status== 0)},
                  new SelectListItem{Text="Xác thực thành công", Value="1", Selected=  (Model.status == 1)},
                  new SelectListItem{Text="Xác thực lỗi", Value="2", Selected=  (Model.status == 2)}                  
              }, new { style = "width:150px"})%>
            <b>Loại xác thực</b>
            <%=Html.DropDownList("type",new[] 
              {                  
                  new SelectListItem{Text="Xác thực hóa đơn", Value="0", Selected= (Model.type== 0)},
                  new SelectListItem{Text="Xác thực hủy hóa đơn", Value="1", Selected=  (Model.status == 1)}                                
              }, new { style = "width:160px"})%>
            <button class="btn-sm" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button>
        </form>
    </div>
    <form id="danhsachHD" method="post" action="/CertifyInv/Certifies">
        <table style="width: 100%; min-width: 800px" class="grid">
            <thead>
                <tr>
                    <th width="20px">STT
                    </th>
                    <th>Mẫu số
                    </th>
                    <th>Ký hiệu
                    </th>
                    <th>Số hóa đơn
                    </th>
                    <th style="width:100px">Trạng thái
                    </th>
                    <th style="width:100px">Loại xác thực
                    </th>
                    <th>Chi tiết</th>
                    <th  style="border-right-color: #EEE;width:20px">
                        <%: Html.Label("ckbAll", Resources.Einvoice.Pay_LblChoose) %>
                        <%: Html.CheckBox("ckbAll") %>
                    </th>
                </tr>
            </thead>
            <tbody>
                <%    
                    var cuslst = Model.PagedListInvCert.ToList();
                    int i = Model.PagedListInvCert.PageIndex * Model.PagedListInvCert.PageSize + 1;
                    foreach (var cus in cuslst)
                    {
                %>
                <tr>
                    <td align="center"><%=i%></td>
                    <td><%=Html.Encode(cus.Pattern)%></td>
                    <td><%=Html.Encode(cus.Serial)%></td>
                    <td><%=Html.Encode(cus.No.ToString("0000000"))%></td>
                    <td><%=cus.Status == 0 ? "Chưa xác thực" : cus.Status == 1 ? "Xác thực thành công" : "Xác thực lỗi"%></td>
                    <td><%=cus.Type == 0 ? "Xác thực hóa đơn" : "Xác thực hủy hóa đơn"%></td>
                    <td align="center">
                            <img alt="" src="/Content/Images/detail1.png" onclick="ajxCall('<%=cus.InvId%>','<%=cus.Pattern%>')" />
                    </td>
                    <td align="center" class="test">
                        <input name="cbid" id="cbid" type="checkbox" <%=(cus.Status != 1?"":"disabled='disabled'")%>
                            value="<%=cus.Id%>" /></td>
                </tr>
                <% i++;
                }%>
            </tbody>
        </table>
        <div class="pager">
            <%=Html.Pager(Model.PagedListInvCert.PageSize, Model.PagedListInvCert.PageIndex + 1, Model.PagedListInvCert.TotalItemCount, new
            {
                action = "index",
                controller = "CertifyInv",
                pattern = Model.pattern,
                status = Model.status,
                type = Model.type
            })%>
        </div>

        <div class="clear">
        </div>
        <input style="margin-top: 15px;" id="certifyButton" name="" type="button" class="button" value="Gửi xác thực" />
        <%=Html.Hidden("cpattern", Model.pattern) %>
        <%=Html.Hidden("ctype", Model.type) %>
    </form>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#ckbAll').change(function () {
                if ($(this).is(':checked')) {
                    $('input[name=cbid]').not(':disabled').attr('checked', true);
                } else {
                    $('input[name=cbid]').not(':disabled').attr('checked', false);
                }
            });
            $('#certifyButton').click(function () {
                var ids = [];
                $('.test input:checked').each(function () {
                    ids.push(this.value);
                });

                if ($("input:checkbox:checked").length > 0 && $('#cpattern').val()) {
                    if (!confirm("Xác thực danh sách hóa đơn đã chọn?")) return;
                    else {
                        $('#ctype').val($('#type').val());
                        document.forms["danhsachHD"].submit();
                    }
                }
                else {
                    alert("Chưa chọn hóa đơn cần xác thực");
                }
            });
        });
    </script>
</asp:Content>
