<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<DecIndexModels>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<%@ Import Namespace="EInvoice.Core" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Quyết định phát hành
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <div class="row">
        <div class="col-xs-12">
            <div class="box box-primary">
                <div class="box-header with-border">
                    <h4 class="box-title"><i class="fa fa-file-text"></i>&nbsp;&nbsp;<%=Resources.Einvoice.Dec_LblDecisionTitle %></h4>
                </div>
                <div class="box-body">
                    <div class="row">
                        <div class="col-sm-10">
                              <% int dstatus = -1; %>
                            <form id="Searchform" name="Searchform" method="post" action="/Publish/ListDecision" class="form-inline">
                          <div class="row">
                              <div class="form-group">
                                      <label class="col-sm-5">Trạng thái:</label>
                                <div class="col-sm-6">
                                                <%=Html.DropDownList("status", new[]
                                  {
                                      new SelectListItem{Text=Resources.Einvoice.Dec_TxtNewDecision,Value="0",Selected= (dstatus == 0)},
                                      new SelectListItem{Text=Resources.Einvoice.Dec_TxtSendDecision,Value="1",Selected= (dstatus == 1)},
                                      new SelectListItem{Text=Resources.Einvoice.Dec_TxtAcceptDecision,Value="2",Selected= (dstatus == 2)},
                                  }, "--Tất cả--", new {@class="form-control", onchange="document.Searchform.submit()" })%>
                                </div>
                              </div>
                           </div> </form>
                        </div>
                        <div class="col-xs-2">
                            <a href="/Publish/CreateDecision/" class="btn btn-info"><span class="fa fa-plus"></span>&nbsp;&nbsp;Tạo mới</a>
                        </div>
                    </div>                  
                    <div class="row">
                        <div class="col-xs-12 table-responsive">
                            <table class="table table-bordered table-hover">
                                <thead>
                                    <tr>
                                        <th style="width: 40px">STT 
                                        </th>
                                        <th>Tên tổ chức
                                        </th>
                                        <th>
                                            <%=Resources.Einvoice.Dec_LblAddress%> 
                                        </th>
                                        <th style="width: 60px">Số QĐ
                                        </th>
                                        <th style="width: 150px">Giám đốc
                                        </th>
                                        <th style="width: 120px">Trạng thái
                                        </th>
                                        <th style="width: 100px">
                                            <%=Resources.Einvoice.LblDetail%> 
                                        </th>
                                        <th style="width: 40px">
                                            <%=Resources.Einvoice.LblEdit%> 
                                        </th>
                                        <th style="width: 40px">
                                            <%=Resources.Einvoice.LblDelete%> 
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%    
                                        IList<Decision> declst = Model.PageListDEC.ToList();
                                        int i = Model.PageListDEC.PageIndex * Model.PageListDEC.PageSize + 1;
                                        foreach (Decision dec in declst)
                                        {
                                    %>
                                    <tr>
                                        <td style="text-align: right"><%=i%></td>
                                        <td><%=dec.ComName%></td>
                                        <td><%=dec.ComAddress%></td>
                                        <td style="text-align: center"><%=Html.Encode(dec.DecisionNo)%></td>
                                        <td style="text-align: center"><%=Html.Encode(dec.Director)%></td>
                                        <td style="text-align: center"><%=Utils.GetNameStatusDec(dec.Status)%></td>
                                        <td style="text-align: center"><a href="/Publish/DetailsDecision/<%=dec.id%>" title="Details">
                                            <i class="fa fa-eye"></i></a></td>
                                        <%if (dec.Status == 0)
                                          {%>
                                        <td style="text-align: center"><a href="/Publish/EditDecision/<%=dec.id%>" title="Edit">
                                            <i class="fa fa-pencil"></i></a></td>
                                        <td style="text-align: center"><a href="#" onclick="OnDelete('<%=dec.id%>')" title="Delete">
                                            <i class="fa fa-trash"></i></a></td>
                                        <%}
                                          else if (dec.Status == 1)
                                          {%>
                                        <td style="text-align: center"><a href="/Publish/EditDecision/<%=dec.id%>" title="Edit">
                                            <i class="fa fa-pencil"></i></a></td>
                                        <td style="text-align: center">
                                            <i class="fa fa-lock"></i></td>
                                        <%}
                                          else if (dec.Status == 2)
                                          {
                                        %>
                                        <td style="text-align: center">
                                            <i class="fa fa-lock"></i></td>
                                        <td style="text-align: center">
                                            <i class="fa fa-lock"></i></td>
                                        <%} %>
                                    </tr>
                                    <% i++;
                }%>
                                </tbody>
                            </table>
                            <div class="pager">
                                <div class="page-a">
                                    <%=Html.Pager(Model.PageListDEC.PageSize, Model.PageListDEC.PageIndex + 1, Model.PageListDEC.TotalItemCount, new
                                                    {
                                                        action = "listdecision",
                                                        controller = "Publish",
                                                        status = Model.status
                                                    })%>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <script language="javascript" type="text/javascript">
        function OnDelete(id) {
            alertify.confirm("<%=Resources.Message.Dec_MesConfirmDel%>", function (e) {
                if (e) {
                    document.location = "/Publish/deletedecision?id=" + id;
                }
                else return;
            });
        }
        jQuery(function ($) {
            $.datepicker.regional['vi'] = {
                closeText: 'Đóng',
                prevText: '&#x3c;Trước',
                nextText: 'Tiếp&#x3e;',
                currentText: 'Hôm nay',
                monthNames: ['Tháng Một', 'Tháng Hai', 'Tháng Ba', 'Tháng Tư', 'Tháng Năm', 'Tháng Sáu',
		'Tháng Bảy', 'Tháng Tám', 'Tháng Chín', 'Tháng Mười', 'Tháng Mười Một', 'Tháng Mười Hai'],
                monthNamesShort: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
		'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
                dayNames: ['Chủ Nhật', 'Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy'],
                dayNamesShort: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
                dayNamesMin: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
                weekHeader: 'Tu',
                dateFormat: 'dd/mm/yy',
                firstDay: 0,
                isRTL: false,
                showMonthAfterYear: false,
                yearSuffix: ''
            };
        });
        $(document).ready(function () {
            $.datepicker.setDefaults($.datepicker.regional['vi']);
            $(".date").datepicker({
                showOn: "button",
                buttonImage: "/content/images/calendar.gif",
                buttonImageOnly: true
            });
        });
    </script>
</asp:Content>
