<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<CusIndexModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Danh sách khách hàng
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">        
    <div class="row">
        <div class="col-xs-12">
            <div class="box box-danger">
                <div class="box-header with-border">
                    <h4 class="box-title"><i class="fa fa-user"></i>Danh sách khách hàng</h4>
                </div>
                <div class="box-body">
                    <div class="row" style="margin-bottom: 15px;">
                        <div class="col-sm-10">
                            <div class="row">
                                <form id="Searchform" method="post" action="/Customer/Index" class="form-inline">
                                    <div class="form-group">
                                        <label class="col-sm-5">Tên khách hàng</label>
                                        <div class="col-sm-6">
                                            <input id="name" name="name" type="text" value="<%=Html.Encode(Model.name)%>" maxlength="200" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-sm-5"><%=Resources.Einvoice.Cus_LblCusCode %></label>
                                        <div class="col-sm-6">
                                            <input id="code" name="code" class="textandnum form-control" style="max-width:150px" type="text" value="<%=Html.Encode(Model.code)%>" maxlength="20" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="col-sm-12">
                                            <button class="btn btn-primary" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button>
                                        </div>
                                    </div>
                                </form>
                            </div>
                        </div>
                        <div class="col-sm-2 pull-right">
                            <a href="/Customer/Create/" class="btn btn-info element-right"><i class="fa fa-plus"></i>Tạo mới</a>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 table-responsive">

                            <table class="table table-bordered table-hover">
                                <thead>
                                    <tr>
                                        <th width="20px">STT
                                        </th>
                                        <th width="150px">
                                            <%=Resources.Einvoice.Cus_LblCusName %>
                                        </th>
                                        <th width="85px">Mã KH
                                        </th>
                                        <th width="250px">Địa chỉ
                                        </th>
                                        <th width="120px">Số điện thoại
                                        </th>
                                        <th width="180px">Email
                                        </th>
                                        <th width="120px">
                                            <%=Resources.Einvoice.Cus_LblAccountName%>
                                        </th>
                                        <th width="20px">
                                            <%=Resources.Einvoice.LblEdit%>
                                        </th>
                                        <th style="border-right-color: #EEE; width: 20px;">
                                            <%=Resources.Einvoice.LblDelete%>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%    
                                        List<Customer> cuslst = Model.PagedListCUS.ToList();
                                        int i = Model.PagedListCUS.PageIndex * Model.PagedListCUS.PageSize + 1;
                                        foreach (Customer cus in cuslst)
                                        {
                                    %>
                                    <tr>
                                        <td style="text-align: right"><%=i%></td>
                                        <td><%=Html.Encode(cus.Name)%></td>
                                        <td><%=Html.Encode(cus.Code)%></td>
                                        <td><%=Html.Encode(cus.Address)%></td>
                                        <td><%=Html.Encode(cus.Phone)%></td>
                                        <td><%=Html.Encode(cus.Email)%></td>
                                        <td style="text-align: left"><%=Html.Encode(cus.AccountName)%></td>
                                        <td style="text-align: center"><a href="/Customer/Edit/<%=cus.id%>" title="Edit">
                                            <i class="fa fa-pencil"></i></a></td>
                                        <td style="text-align: center"><a href="#" onclick="OnDelete('<%=cus.id%>')" title="Delete">
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
                            <%=Html.Pager(Model.PagedListCUS.PageSize, Model.PagedListCUS.PageIndex + 1, Model.PagedListCUS.TotalItemCount, new
                                {
                                    action = "index",
                                    controller = "Customer",
                                    name = Model.name,
                                    code = Model.code
                                })%>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>
    <script language="javascript" type="text/javascript">
        function OnDelete(id) {
            alertify.confirm("<%=Resources.Message.Cus_MesConfirmDel%>", function (e) {
                if (e) {
                    document.location = "/Customer/delete?id=" + id;
                }
                else return;
            });
        }
    </script>
</asp:Content>
