<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<IPagedList<Products>>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Danh sách sản phẩm
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <div class="row">
        <div class="col-xs-12">
            <div class="box box-danger">
                <div class="box-header with-border">
                    <h4 class="box-title"><i class="fa fa-pencil"></i>Danh sách sản phẩm, dịch vụ</h4>
                </div>
                <div class="box-body">
                    <div class="row">
                        <div class="col-sm-10">
                            <form id="Searchform" method="post" action="/Product/Index" class="form-inline">
                                <div class="row">
                                    <div class="form-group">
                                        <label class="col-sm-5"><%=Resources.Einvoice.Prod_LblCode %></label>
                                       <div class="col-sm-6">
                                            <input id="code" name="code" type="text" class="textandnum form-control" value="<%=Html.Encode(ViewData["code"]) %>" maxlength="200"  />
                                       </div>
                                    </div>
                                    <div class="form-group">
                                          <label class="col-sm-5"><%=Resources.Einvoice.Prod_LblName %></label>
                                           <div class="col-sm-6">  <input id="name" name="name" type="text" value="<%=Html.Encode(ViewData["name"]) %>" maxlength="200" class="form-control"/></div>
                                    </div>
                                    <div class="form-group">  
                                        <div class="col-sm-12">
                                            
                                         <button class="btn btn-primary" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button>
                                        </div>
                                    </div>
                                </div>
                            </form>
                        </div>
                        <div class="col-sm-2" style="text-align:left;">
                            <a href="/Product/Create/" class="btn btn-info element-right"><i class="fa fa-plus"></i>Tạo mới</a>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 table-responsive">

                            <table class="table table-bordered table-hover">
                                <thead>
                                    <tr>
                                        <th width="40px">STT
                                        </th>
                                        <th width="150px">
                                            <%=Resources.Einvoice.Prod_LblCode %>
                                        </th>
                                        <th>
                                            <%=Resources.Einvoice.Prod_LblName %>
                                        </th>
                                        <th width="150px">
                                            <%=Resources.Einvoice.Prod_LblCost%>
                                        </th>
                                        <th width="150px">
                                            <%=Resources.Einvoice.Prod_LblUnit %>
                                        </th>
                                        <th width="40px">
                                            <%=Resources.Einvoice.LblEdit %> 
                                        </th>
                                        <th width="40px">
                                            <%=Resources.Einvoice.LblDelete %>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%    
                                        List<Products> Prolst = Model.ToList();
                                        int i = Model.PageIndex * Model.PageSize + 1;
                                        foreach (Products pro in Prolst)
                                        {
                                    %>
                                    <tr>
                                        <td style="text-align: right"><%=i%></td>
                                        <td><%=Html.Encode(pro.Code)%></td>
                                        <td><%=Html.Encode(pro.NameProduct)%></td>
                                        <td style="text-align: center"><%=pro.Price%></td>
                                        <td style="text-align: center"><%=Html.Encode(pro.Unit)%></td>
                                        <td style="text-align: center"><a href="/Product/Edit/<%=pro.Id%>" title="Edit">
                                            <i class="fa fa-pencil"></i></a></td>
                                        <td style="text-align: center"><a href="#" onclick="OnDelete('<%=pro.Id%>')" title="Delete">
                                            <i class="fa fa-trash"></i></a></td>
                                    </tr>
                                    <% i++;
                }%>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <div class="box-footer">

                    <div class="pager">
                        <div class="page-a">
                            <%=Html.Pager(Model.PageSize, Model.PageIndex + 1, Model.TotalItemCount, new
                                {
                                    action = "index",
                                    controller = "Product",
                                    name = ViewData["name"],
                                    code=ViewData["code"]
                                })%>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>
    <script language="javascript" type="text/javascript">
        function OnDelete(id) {
            alertify.confirm("<%=Resources.Message.Prod_MesConfirmDel%>", function (e) {
                if (e) {
                    document.location = "/Product/delete?id=" + id;
                }
                else return;
            });
        }
    </script>
</asp:Content>

