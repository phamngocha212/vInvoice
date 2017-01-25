<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<Company>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thông tin đơn vị phát hành hóa đơn
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
    <style>
        .form-group {
            border-bottom: 1px dotted #d2d6de;
            padding: 3px 0 5px 0;
            margin: 3px 0 8px 0 !important;
        }

        .col-sm-8 {
            font-weight: 100 !important;
        }
    </style>
    <div class="row">
        <div class="col-xs-6 col-xs-offset-3">
            <div class="box box-primary">
                <div class="box-header with-border">
                    <h4 class="box-title" style="color: #3c8dbc">
                        <i class="fa fa-info"></i>
                        ĐƠN VỊ PHÁT HÀNH HÓA ĐƠN
                    </h4>
                </div>
                <div class="box-body form-horizontal">
                    <%=Html.Hidden("id",Model.id) %>
                    <%=Html.Hidden("SignatureImage",Html.Encode(Model.SignatureImage))%>
                    <div class="form-group">
                        <label class="col-sm-4">
                            <%=Resources.Einvoice.Com_LblName%> :
                        </label>
                        <label class="col-sm-8">
                            <%=Html.Encode(Model.Name) %>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">
                            Mã số thuế:
                        </label>
                        <label class="col-sm-8">
                            <%=Html.Encode(Model.TaxCode) %>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">
                            <%=Resources.Einvoice.Com_LblAddress %> :
                        </label>
                        <label class="col-sm-8">
                            <%=Html.Encode(Model.Address) %>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">
                            <%=Resources.Einvoice.Com_LblPhone %> :
                        </label>
                        <label class="col-sm-8">
                            <%=Html.Encode(Model.Phone) %>
                        </label>

                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">
                            <%=Resources.Einvoice.Com_LblFax %>:
                        </label>
                        <label class="col-sm-8">
                            <%=Html.Encode(Model.Fax) %>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">
                            <%=Resources.Einvoice.Com_LblEMail %> :
                        </label>
                        <label class="col-sm-8">
                            <%=Html.Encode(Model.Email) %>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">
                            <%=Resources.Einvoice.Com_LblContactPerson %> :
                        </label>
                        <label class="col-sm-8">
                            <%=Html.Encode(Model.ContactPerson) %>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">
                            <%=Resources.Einvoice.Com_lblRepresentPerson %>:
                        </label>
                        <label class="col-sm-8">
                            <%=Html.Encode(Model.RepresentPerson) %>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">
                            <%=Resources.Einvoice.Com_LblBankNumber%>:
                        </label>
                        <label class="col-sm-8">
                            <%=Html.Encode(Model.BankNumber) %>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">
                            Chủ tài khoản :
                        </label>
                        <label class="col-sm-8">
                            <%=Html.Encode(Model.BankAccountName) %>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">
                            <%=Resources.Einvoice.Com_LblBankName %> :
                        </label>
                        <label class="col-sm-8">
                            <%=Html.Encode(Model.BankName) %>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">
                            <%=Resources.Einvoice.Com_LblTaxName %>:
                        </label>
                        <label class="col-sm-8">
                            <%= Html.Encode(ViewData["taxname"])%>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-4">
                            <%=Resources.Einvoice.Com_LblDescription %>:
                        </label>
                        <label class="col-sm-8">
                            <%=Html.Encode(Model.Descriptions)%>
                        </label>
                    </div>

                </div>
                <div class="box-footer">
                    <div class="element-center">
                        <button class="btn btn-sm btn-primary" type="button" onclick="Edit_Onclick(<%=Model.id%>)"><i class="fa fa-edit"></i><%=Resources.Einvoice.BtnEdit %></button>
                        <%if (HttpContext.Current.User.IsInRole("Admin"))
                          { %>
                        <button class="btn btn-sm btn-default" type="button" onclick="gencodeOnclick()">
                            <i class="fa fa-key"></i>Lấy mã kích hoạt</button>
                        <%} %>
                        <button class="btn btn-sm" type="button" onclick="Back_Onclick()">
                            <i class="fa fa-backward"></i>Quay lại</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function Back_Onclick() {
            document.location = "/";
        }
        function Edit_Onclick(id) {
            document.location = "/Company/Edit?id=" + id;
        }
        function gencodeOnclick(id) {
            document.location = "/Company/GenActiveCode";
        }
    </script>

</asp:Content>
