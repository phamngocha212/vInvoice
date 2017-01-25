<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<IndexAccountModel>" %>

<%@ Import Namespace="IdentityManagement.Domain" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Danh sách người dùng hệ thống
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-xs-12">
            <div class="box box-danger">
                <div class="box-header with-border">
                    <h4 class="box-title"><i class="fa fa-user"></i>Người dùng hệ thống</h4>
                </div>
                <div class="box-body">
                    <div class="row">
                        <div class="col-sm-10">
                          <div class="row">
                            <form id="Searchform" method="post" action="/Account/Index" class="form-inline">
                             <div class="form-group">
                                <label class="col-sm-5" style="margin-right: 10px;">Tài khoản</label>
                               <div class="col-sm-6"> <%=Html.TextBox("username", Model.username, new {@maxlength="200", @Style = "margin-right:10px", @class="form-control" })%></div>
                                 </div>
                                <div class="form-group"> <div class="col-sm-2"><button class="btn btn-primary" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button></div></div>                               
                            </form> 
                            </div>
                        </div>
                      <div class="col-sm-2 " style="text-align:right;">
                            <a href="/Account/Create/" class="btn btn-info "><i class="fa fa-plus"></i>Tạo mới</a>
                        </div>
                    </div>
                    <div class="box-body no-padding table-responsive">
                        <table class="table table-bordered table-hover">
                            <thead>
                                <tr>
                                    <th style="width: 40px">STT
                                    </th>
                                    <th>
                                        <%=Resources.Einvoice.User_LblAccountName %>
                                    </th>
                                    <th>
                                        <%=Resources.Einvoice.User_LblEmail %>
                                    </th>
                                    <th width="100px">
                                        <%=Resources.Einvoice.User_LblActive %>
                                    </th>
                                    <th width="100px">
                                        <%=Resources.Einvoice.User_LblLock %>
                                    </th>
                                    <th width="80px">Ngày tạo
                                    </th>
                                    <th width="40px">
                                        <%=Resources.Einvoice.LblEdit %>
                                    </th>
                                    <th style="width: 40px">Xóa</th>
                                </tr>
                            </thead>
                            <tbody>
                                <%         
                                    IList<user> userlst = Model.PageListUser.ToList();
                                    int i = Model.PageListUser.PageIndex * Model.PageListUser.PageSize + 1;
                                    foreach (user ur in userlst)
                                    {
                                %>
                                <tr>
                                    <td style="text-align: right"><%=i%></td>
                                    <td><%=Html.Encode(ur.username)%></td>
                                    <td><%=Html.Encode(ur.email)%></td>
                                    <td style="text-align: center">
                                        <img src="<%=ur.IsApproved ? "/Content/Images/valid.png" : "/Content/Images/cross.gif" %>" title="<%=ur.IsApproved ? "Sử dụng" :"Khóa"%>" />
                                    </td>
                                    <td style="text-align: center">
                                        <img src="<%=ur.IsLockedOut ? "/Content/Images/valid.png" : "/Content/Images/cross.gif" %>" title="<%=!ur.IsLockedOut ? "Sử dụng" :"Khóa"%>" /></td>
                                    <td style="text-align: center"><%=ur.CreateDate.ToString("dd/MM/yyyy")%></td>
                                    <td style="text-align: center"><a href="/Account/Edit/<%=ur.userid%>" title="<%=Resources.Einvoice.LblEdit %>">
                                        <i class="fa fa-pencil"></i></a></td>
                                    <td style="text-align: center"><a href="#" onclick="OnDelete('<%=ur.userid%>')" title="Delete">
                                        <i class="fa fa-trash"></i></a></td>
                                </tr>
                                <%i++;
                                    }
                                %>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="box-footer">

                    <div class="pager">
                        <div class="page-a">
                            <%=Html.Pager(Model.PageListUser.PageSize, Model.PageListUser.PageIndex + 1, Model.PageListUser.TotalItemCount,new
                {
                   action = "index",
                   controller = "Account",
                   username = Model.username                             
               })%>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>
    <script language="javascript" type="text/javascript">
        function OnDelete(id) {
            alertify.confirm("Xóa người dùng?", function (e) {
                if (e) {
                    document.location = "/Account/delete?id=" + id;
                }
                else return;
            });
        }
    </script>
</asp:Content>
