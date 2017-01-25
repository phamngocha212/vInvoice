<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<StaffModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Danh sách nhân viên
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <div class="row">
        <div class="col-xs-12">
            <div class="box box-danger">
                <div class="box-header with-border">
                    <h4 class="box-title"><i class="fa fa-pencil"></i>Danh sách nhân viên</h4>
                </div>
                <div class="box-body">
                    <div class="row" style="margin-bottom: 15px;">
                        <div class="col-sm-10">
                            <form id="Searchform" method="post" action="/Staff/Index" class="form-inline">
                                <div class="row">
                                    <div class="form-group">
                                        <label class="col-sm-5"><%=Resources.Einvoice.Staff_LblName%>:</label>
                                        <div class="col-sm-6">
                                            <input id="fullname" maxlength="200" name="fullname" type="text" value="<%=Html.Encode(Model.fullname) %>" class="form-control" />
                                        </div>
                                    </div>                                    
                                    <div class="form-group">
                                        <label class="col-sm-5">Tài khoản:</label>
                                        <div class="col-sm-6">
                                            <input id="account" name="account" type="text" maxlength="50" value="<%=Html.Encode(Model.account) %>" class="form-control" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="col-sm-12">
                                            <button class="btn btn-primary" type="submit"><i class="fa fa-search"></i>Tìm kiếm</button>
                                        </div>
                                    </div>
                                </div>
                            </form>
                        </div>
                        <div class="col-sm-2" style="text-align: left">
                            <a href="/Staff/Create/" class="btn btn-info element-right"><i class="fa fa-plus"></i>Tạo mới</a>
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
                                            <%=Resources.Einvoice.Staff_LblName%>
                                        </th>
                                        <th width="100px">
                                            <%=Resources.Einvoice.Staff_LblAccountName%>
                                        </th>
                                        <th width="150px">
                                            <%=Resources.Einvoice.Staff_LblDivision%>
                                        </th>
                                        <th width="300px">Địa chỉ
                                        </th>
                                        <th width="100px">
                                            <%=Resources.Einvoice.Staff_LblPhone%> 
                                        </th>
                                        <th width="100px">
                                            <%=Resources.Einvoice.Staff_LblEmail%>
                                        </th>
                                        <th width="30px">
                                            <%=Resources.Einvoice.LblEdit%>
                                        </th>
                                        <th style="border-right-color: #EEE; width: 20px;">
                                            <%=Resources.Einvoice.LblDelete%>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%    
                                        List<Staff> stalst = Model.PageListStaff.ToList();
                                        int i = Model.PageListStaff.PageIndex * Model.PageListStaff.PageSize + 1;
                                        foreach (Staff item in stalst)
                                        {
                                    %>
                                    <tr>
                                        <td style="text-align: right"><%=i%></td>
                                        <td><%=Html.Encode(item.FullName)%></td>
                                        <td style="text-align: left"><%=Html.Encode(item.AccountName)%></td>
                                        <td><%=Html.Encode(item.Division)%></td>
                                        <td><%=Html.Encode(item.Address)%></td>
                                        <td><%=Html.Encode(item.Mobile)%></td>
                                        <td title="<%=Html.Encode(item.Email)%>"><%=Html.Encode(item.Email)%></td>
                                        <td style="text-align: center"><a href="/Staff/Edit/<%=item.ID%>" title="Edit">
                                            <i class="fa fa-pencil"></i></a></td>
                                        <td style="text-align: center"><a href="#" onclick="OnDelete('<%=item.ID %>')" title="Delete">
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
                            <%=Html.Pager(Model.PageListStaff.PageSize, Model.PageListStaff.PageIndex + 1, Model.PageListStaff.TotalItemCount, new
                                {
                                    action = "Index",
                                    controller = "Staff",
                                    fullname = Model.fullname,
                                    division = Model.division
                                })%>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script language="javascript" type="text/javascript">
        function OnDelete(id) {
            alertify.confirm("<%=Resources.Message.Staff_MessDeleteStaff%>", function (e) {
                if (e) {
                    document.location = "/Staff/delete?id=" + id;
                }
                else return;
            });
        }
    </script>
</asp:Content>
