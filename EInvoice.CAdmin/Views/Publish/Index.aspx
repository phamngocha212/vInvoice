<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<PubIndexModels>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<%@ Import Namespace="EInvoice.Core" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thông báo phát hành 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <div class="row">
        <div class="col-xs-12">
            <div class="box box-primary">
                <div class="box-header with-border">
                    <h4 class="box-title"><i class="fa fa-file-text"></i><%=Resources.Einvoice.Pub_LblTitle%></h4>
                </div>
                <div class="box-body">
                    <div class="row">
                        <div class="col-xs-11">
                            <% int pstatus = -1; %>
                            <form method="post" action="/Publish/Index" style="" class="form-inline">
                                <label><%=Resources.Einvoice.Pub_FromDate%>:</label>
                                <div class="input-group">
                                    <div class="input-group-addon"><i class="fa fa-calendar"></i></div>                                    
                                    <input value="<%=Html.Encode(Model.fromdate) %>"  name="fromdate" class="datepicker w85 form-control" placeholder="__/__/____" />
                                </div>
                                <label><%=Resources.Einvoice.Pub_lblToDate%>:</label>
                                <div class="input-group">
                                    <div class="input-group-addon">
                                        <i class="fa fa-calendar"></i>
                                    </div>
                                    <input value="<%=Html.Encode(Model.todate) %>" name="todate" class="datepicker w85 form-control" placeholder="__/__/____"/>                                    
                                </div>
                                <label><%=Resources.Einvoice.Pub_LblStatus%>:</label>

                                <%=Html.DropDownList("status",new[]
                             {
                                 new SelectListItem{Text=Resources.Einvoice.Pub_TxtNewPub, Value="0", Selected= (pstatus == 0)},
                                 new SelectListItem{Text=Resources.Einvoice.Pub_TxtSendedPub, Value="1", Selected= (pstatus == 1)},
                                 new SelectListItem{Text=Resources.Einvoice.Pub_txtAcceptedPub, Value="2", Selected= (pstatus == 2)},
                             }, "--Trạng thái--", new { style = "width:230px", @class="form-control" } 
                             ) %>

                                <button class="btn-sm btn btn-primary" type="submit"><i class="fa fa-search"></i>&nbsp;&nbsp;Tìm kiếm</button>
                            </form>
                        </div>
                        <div class="col-xs-1">
                            <a href="/Publish/CreateRPublish/" class="btn btn-info btn-sm element-right"><span class="fa fa-plus"></span>&nbsp;&nbsp;Tạo mới</a>
                        </div>
                    </div>                    
                    <div class="row">
                        <div class="col-xs-12 table-responsive">
                            <table class="table table-bordered table-hover">
                                <thead>
                                    <tr>
                                        <th style="width: 40px">STT
                                        </th>
                                        <th>
                                            <%=Resources.Einvoice.Pub_LblComName %>
                                        </th>
                                        <th>
                                            <%=Resources.Einvoice.Pub_LblTaxName %>
                                        </th>
                                        <th style="width: 100px">Ngày lập
                                        </th>
                                        <th style="width: 120px"><%=Resources.Einvoice.Pub_LblStatus %></th>
                                        <th style="width: 100px">
                                            <%=Resources.Einvoice.LblDetail %>
                                        </th>
                                        <th style="width: 40px">
                                            <%=Resources.Einvoice.LblEdit %>
                                        </th>
                                        <th style="width: 40px">
                                            <%=Resources.Einvoice.LblDelete %>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%   
                                        IList<Publish> lstpub = Model.PageListPUB.ToList();
                                        int i = Model.PageListPUB.PageIndex * Model.PageListPUB.PageSize + 1;
                                        foreach (Publish pub in lstpub)
                                        {
                                    %>
                                    <tr>
                                        <td style="text-align: right"><%=i%></td>
                                        <td><%=Html.Encode(pub.ComName)%></td>
                                        <td><%=Html.Encode(pub.TaxAuthorityName)%></td>
                                        <td style="text-align: center"><%=pub.CreateDate.ToString("dd'/'MM'/'yyyy")%></td>
                                        <td style="text-align: center"><%=Utils.GetNamePublishStatus(pub.Status)%></td>
                                        <td style="text-align: center"><a href="/Publish/DetailRPublish/<%=pub.id%>" title="Details"><i class="fa fa-eye"></i></a></td>
                                        <%if (pub.Status == PublishStatus.Newpub)
                                          {%>
                                        <td style="text-align: center"><a href="/Publish/EditRPublish/<%=pub.id%>" title="Edit"><i class="fa fa-pencil"></i></a></td>
                                        <td style="text-align: center"><a href="#" onclick="OnDelete('<%=pub.id%>')" title="Delete"><i class="fa fa-trash"></i></a></td>
                                        <%}
                                          else if (pub.Status == PublishStatus.Waiting)
                                          {%>
                                        <td style="text-align: center"><a href="/Publish/EditRPublish/<%=pub.id%>" title="Edit"><i class="fa fa-pencil"></i></a></td>
                                        <td style="text-align: center"><i class="fa fa-lock"></i></td>
                                        <%}
                                          else if (pub.Status == PublishStatus.InUse)
                                          { %>
                                        <td style="text-align: center"><i class="fa fa-lock"></i></td>
                                        <td style="text-align: center"><i class="fa fa-lock"></i></td>
                                        <%} %>
                                    </tr>
                                    <% i++;
                                        }%>
                                </tbody>
                            </table>
                            <div class="pager">
                                <div class="page-a">
                                    <%=Html.Pager(Model.PageListPUB.PageSize, Model.PageListPUB.PageIndex + 1, Model.PageListPUB.TotalItemCount,new{
                                                        action = "index",
                                                        controller = "Publish",
                                                        fromdate = Model.fromdate,
                                                        todate = Model.todate,
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
        //xóa thông báo phát hành
        function OnDelete(id) {
            alertify.confirm("<%=Resources.Message.Pub_MesConfirmDel%>", function (e) {
                if (e) {
                    document.location = "/Publish/DeleteRPublish?id=" + id;
                }
                else return;
            });
        }
 
       
        //Định dạng hình ảnh hiển thi giao diện chọn datepicker
        $(document).ready(function () {                        
            $(".datepicker").datepicker({
                showOn: "button",
                format: 'dd/mm/yyyy',
                buttonImageOnly: true,
                changeMonth: true,
                changeYear: true,
                autoclose: true
            });
        });
    </script>
</asp:Content>
