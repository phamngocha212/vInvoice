<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<EInvoice.CAdmin.Models.AdjustInvoiceModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<%@ Import Namespace="FX.Utils.MvcPaging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Danh sách hóa đơn thay thế
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
            <%using (Html.BeginForm("ReplaceInvIndex", "AdJust", FormMethod.Post, new { @name = "searchForm" }))
              {%>
            <%Html.RenderPartial("IndexShareElement", Model);%>
            <%}%>
       
    <div class="pager">
        <div class="page-a">
        <%=Html.Pager(Model.PageListAdjustSearch.PageSize, Model.PageListAdjustSearch.PageIndex + 1, Model.PageListAdjustSearch.TotalItemCount, new
            {
                action = "ReplaceInvIndex",
                controller = "Adjust",
                Pattern = Model.pattern,
                Serial = Model.Serial,
                nameCus = Model.nameCus,
                code = Model.code,
                CodeTax = Model.CodeTax,
                FromDate=Model.FromDate,
                ToDate=Model.ToDate,
                InvNo=Model.InvNo,
            })%>
            </div>
    </div>
</asp:Content>
