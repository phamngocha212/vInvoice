<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<MenusModels>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Danh sách chức năng
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />

    <div class="row">
        <div class="col-xs-12">
            <div class="box box-danger">
                <div class="box-header with-border">
                    <h4 class="box-title"><i class="fa fa-pencil"></i>Danh sách chức năng</h4>
                </div>
                <div class="box-body">
                    <div class="row">
                        <div class="col-xs-12">
                            <% int position = 0; %>
                            <form id="Searchform" method="post" action="/Menu/Index" class="form-inline">
                                <b>Loại menu:</b>
                                <%=Html.DropDownList("position", new[]
                                     {
                                         new SelectListItem{Text="Menu dọc", Value="0", Selected= (position == 0)},
                                         new SelectListItem{Text="Menu ngang", Value="1", Selected= (position == 1)}
                                     }, new { style = "width:130px;margin-left: 20px",@class="form-control", onchange="changePosition();"}
                                     )%>
                                <label>Menu cha:</label>
                                <%=Html.DropDownList("parentId", new SelectList(Model.RootMenus,"Id","Name","--Chon menu cha--"),new { style = "width:200px;margin-left: 5px",@class="form-control", onchange="$('form:first').submit();" }) %>
                            </form>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 table-responsive">
                            <table class="table table-bordered table-hover">
                                <thead>
                                    <tr>
                                        <th style="width: 30px">STT
                                        </th>
                                        <th>Tên
                                        </th>                                        
                                        <th style="width: 50px">Vị trí
                                        </th>
                                        <th style="width: 100px">?Sử dụng
                                        </th>
                                        <th style="width: 30px">Sửa
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%int i = Model.PagedListMenus.PageIndex * Model.PagedListMenus.PageSize;
                                      foreach (var it in Model.PagedListMenus.ToList())
                                      {
                                          var rootName = it.Level > 0 ? "---" + it.Item.Name : it.Item.Name;
                                          i++;
                                    %>
                                    <tr>
                                        <td style="text-align: right"><%=i %></td>
                                        <td><%=Html.Encode(rootName)%></td>                                        
                                        <td style="text-align: center"><%=it.Item.Priority%></td>
                                        <td style="text-align: center">
                                            <img src="<%=it.Item.IsPub ? "/Content/Images/valid.png" : "/Content/Images/cross.gif" %>" title="<%=it.Item.IsPub ? "Sử dụng" :"Khóa"%>" /></td>
                                        <td style="text-align: center"><a href="/Menu/Edit/<%=it.Id%>" title="Edit">
                                            <i class="fa fa-pencil"></i></a></td>
                                    </tr>
                                    <%foreach (var child in it.Items)
                                      {                                          
                                          var childName = "";
                                          for (int lv = 0; lv < child.Level; lv++)
                                          {
                                              childName += "---&nbsp;";
                                          }
                                          childName = childName + child.Item.Name;
                                    %>
                                    <tr>
                                        <td style="text-align: right">                                            
                                        </td>
                                        <td><%=childName %></td>                                        
                                        <td style="text-align: center"><%=child.Item.Priority %></td>
                                        <td style="text-align: center">
                                            <img src="<%=child.Item.IsPub ? "/Content/Images/valid.png" : "/Content/Images/cross.gif" %>" /></td>
                                        <td style="text-align: center"><a href="/Menu/Edit/<%=child.Item.Id%>" title="Edit">
                                            <i class="fa fa-pencil"></i></a></td>
                                    </tr>
                                    <%}
                                      }%>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>

            </div>

        </div>
    </div>
    <script type="text/javascript">
        function changePosition() {
            $('#parentId').val(0);
            $('form:first').submit();
        }
    </script>
</asp:Content>
