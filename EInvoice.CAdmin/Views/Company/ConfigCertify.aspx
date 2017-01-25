<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    [Cấu hình định danh]
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        label {
            min-width: 165px !important;
        }

        .textc {
            text-align: center;
        }

        input[type=text] {
            outline: none;
        }

        input[type=button],
        input[type=submit] {
            padding: 3px 7px;
            min-width: 73px;
            text-align: center;
            border: 1px solid #808080;
        }
        ul.help {
            margin-top: 30px;
        }
            ul.help li {
                line-height: 1.5em;
            }
    </style>
    <% using (Html.BeginForm("ConfigCertify", "Company", FormMethod.Post, new { enctype = "multipart/form-data" }))
       {
           Html.EnableClientValidation(); %>
    <div style="text-align: center; font-size: large; font-weight: bold;">CẤU HÌNH CHUỖI ĐỊNH DANH MẬT KHẨU VỚI TỔNG CỤC THUẾ</div>
    <fieldset>
        <ol>
            <li>
                <label for="VAN_SYSTEM_CODE">Mã doanh nghiệp:</label>
                <%: Html.TextBox("VAN_SYSTEM_CODE", null, new { style="min-width: 60px; padding: 0px 5px;" })%>
            </li>
            <li>
                <label for="VAN_TAX_OFFICE_CODE">Mã cơ quan thuế:</label>
                <%: Html.TextBox("VAN_TAX_OFFICE_CODE", null, new { style="min-width: 60px; padding: 0px 5px;" })%>
            </li>
            <li>
                <label for="VAN_AUT_CODE">Chuỗi định danh mật khẩu:</label>
                <%: Html.TextBox("VAN_AUT_CODE",  null, new { style="min-width: 600px; padding: 0px 5px;" })%>
            </li>
        </ol>
    </fieldset>
    <div class="textc">
        <input type="submit" value="<%: Resources.Einvoice.BtnEdit %>" />
        <input type="button" value="<%: Resources.Einvoice.BtnBack %>" onclick="javascript: history.go(-1);" />
    </div>
    <% } %>
    <div>
        <ul class="help">
            <li>Mã doanh nghiệp: Có cấu trúc LHD_{Mã_doanh_nghiệp_tự_đặt: tố đa 10 ký tự}. VD: LHD_VDC</li>
            <li>Mã cơ quan thuế: Là mã của cơ quan thuế nơi doanh nghiệp khai báo thuế. Liên hệ cơ quan thuế (chi cục) để được hướng dẫn.</li>
            <li>Chuỗi định danh mật khẩu: Là chuỗi định danh mà hệ thống xác thực của TCT (tổng cục thuế) dành cho doanh nghiệp sau khi doanh nghiệp đăng ký xác thực với TCT.</li>
        </ul>
    </div>
</asp:Content>
