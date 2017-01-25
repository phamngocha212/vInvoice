<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<string>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Mã kích hoạt phần mềm quản lý, phát hành hóa đơn điện tử
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />    
    <div class="row">
        <div class="col-xs-6 col-xs-offset-3">
            <div class="box box-primary">
                <div class="box-header with-border">
                    <h4 class="box-title" style="color:#3c8dbc">
                        <i class="fa fa-info"></i>
                        MÃ KÍCH HOẠT PHẦN MỀM
                    </h4>
                </div>
                <div class="box-body form-horizontal">                    
                    <div class="form-group">
                        <label class="col-sm-4" style="width:127px">
                            Mã kích hoạt :
                        </label>
                        <label class="col-sm-8">
                            <%=Html.Encode(ViewBag.ActiveCode) %>
                        </label>
                    </div>                    
                </div>
                <div class="box-footer">
                    <div class="element-center">                        
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
    </script>

</asp:Content>
