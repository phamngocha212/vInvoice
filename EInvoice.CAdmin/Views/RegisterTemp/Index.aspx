<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<IPagedList<RegisterTemp>>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Đăng ký mẫu hóa đơn
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-xs-12">
            <div class="box box-danger">
                <div class="box-header with-border">
                    <h4 class="box-title"><i class="fa fa-info"></i>ĐĂNG KÝ MẪU HÓA ĐƠN</h4>
                </div>
                <div class="box-body">
                    <div class="row">
                        <div class="col-sm-10"></div>
                        <div class="col-sm-2" style="text-align: left;">
                            <a href="/Registertemp/Choosetemp/" class="btn btn-info element-right"><i class="fa fa-plus"></i>Đăng ký</a>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-xs-12">
                            <table class="table table-bordered table-hover">
                                <thead>
                                    <tr>
                                        <th width="30px">STT
                                        </th>
                                        <th width="150px">Mẫu đăng ký
                                        </th>
                                        <th width="200px">Tên mẫu 
                                        </th>
                                        <th width="150px">Tên loại
                                        </th>
                                        <th width="100px">Mẫu số
                                        </th>
                                        <th width="30px">Sửa
                                        </th>
                                        <th width="30px" style="border-right-color: #EEE">Xóa
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%
                                        IList<RegisterTemp> rtList = Model.ToList();
                                        int i = Model.PageIndex * Model.PageSize + 1;
                                        foreach (RegisterTemp rt in rtList)
                                        {
                                    %>
                                    <tr>
                                        <td style="text-align: right">
                                            <%=i%>
                                        </td>
                                        <td style="text-align: center">
                                            <%=Html.Encode(rt.Name)%>
                                        </td>
                                        <td style="text-align: center">
                                            <%=rt.InvoiceTemp !=null ? Html.Encode(rt.InvoiceTemp.TemplateName) : ""%>
                                        </td>
                                        <td style="text-align: center">
                                            <%=rt.InvoiceTemp != null ? Html.Encode(rt.InvoiceTemp.InvCateName) : ""%>
                                        </td>
                                        <td style="text-align: center">
                                            <%=Html.Encode(rt.InvPattern)%>
                                        </td>
                                        <td style="text-align: center">
                                            <a href="/RegisterTemp/Edit?id=<%=rt.Id%>" title="Edit">
                                                <i class="fa fa-pencil"></i></a>
                                        </td>
                                        <td style="text-align: center">
                                            <a href="#" onclick="OnDelete('<%=rt.Id%>')" title="Delete">
                                                <i class="fa fa-trash"></i></a>
                                        </td>
                                    </tr>
                                    <%
                                            i++;
                                        } 
                                    %>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div class="box-footer">
                        <div class="pager">
                            <div class="page-a">
                                <%=Html.Pager(Model.PageSize, Model.PageIndex + 1, Model.TotalItemCount)%>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function OnDelete(id) {
            alertify.confirm("Bạn chắc chắn muốn xóa đăng ký mẫu này?", function (e) {
                if (e) {
                    document.location = "/RegisterTemp/Delete?id=" + id;
                }
                else return;
            });
        }
    </script>
</asp:Content>
