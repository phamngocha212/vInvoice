<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<Certificate>" %>

<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
    <div class="row">
        <div class="col-md-6 col-md-offset-3 col-sm-12">
            <div class="box box-primary">
                <div class="box-header with-border">
                    <h4 class="box-title">
                        <i class="fa fa-key"></i><%=Resources.Einvoice.Key_LblKeyStoreInfo %>
                    </h4>
                </div>
                <div class="box-body form-horizontal">
                    <%if (!string.IsNullOrEmpty(Model.Cert))
                      { %>
                    <div class="form-group">
                        <label class="col-sm-5">
                            1. <%=Resources.Einvoice.Cert_lblOwnCA %> :
                        </label>
                        <label class="col-sm-7">
                            <%=Html.Label(Html.Encode(Model.OwnCA)) %>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-5">
                            2. <%=Resources.Einvoice.Cert_LblSerialCert %>:
                        </label>
                        <label class="col-sm-7">
                            <%=Html.Label(Html.Encode(Model.SerialCert))%>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-5">
                            3. <%=Resources.Einvoice.Cert_LblOrgName %>:
                        </label>
                        <label class="col-sm-7">
                            <%=Html.Label(Html.Encode(Model.OrganizationCA)) %>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-5">
                            4. <%=Resources.Einvoice.Cert_ValidFrom%>:
                        </label>
                        <label class="col-sm-7">
                            <%=Html.Label(Model.ValidFrom.ToString()) %>
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-5">
                            5. <%=Resources.Einvoice.Cert_LblExpiryDate%>:
                        </label>
                        <label class="col-sm-7">
                            <%=Html.Label(Model.ValidTo.ToString()) %>
                        </label>
                    </div>
                    <%}
                      else
                      {             
                    %>
                    <div class="form-group">
                        <label class="col-sm-5">
                            1.loại keystore:
                        </label>
                        <label class="col-sm-7">
                            HSM
                        </label>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-5">
                            2. serialNumber:
                        </label>
                        <label class="col-sm-7">
                            <%=Html.Encode(Model.SerialCert) %>
                        </label>
                    </div>
                    <%} %>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
