<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AdjustModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thay thế hóa đơn
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
       
    </style>
     
    <script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
      <%using (Html.BeginForm("CreateReplaceInv", "AdJust", FormMethod.Get, new { @class = "form-horizontal" }))
          {%>
    <div class="row">
        <div class="col-xs-offset-3 col-xs-6">
            <div class="box box-primary">
                <div class="box-header with-border">
                    <h4 class="box-title">
                        <i class="fa fa-search"></i>&nbsp &nbsp;
                        <%=Resources.Einvoice.ReplaceInv_LblSeachReInv%>
                    </h4>
                </div>
            
            <div class="box-body with-border">
                 <div class="form-group">
                     <label class="col-sm-5 control-label"><%=Resources.Einvoice.ReplaceInv_LblPattern%> (<i>Pattern</i>): <span style="color: red">(*)</span></label>
                     <div class="col-sm-7">
                         <%=Html.DropDownList("pattern", Model.lstpattern, "--Chọn mẫu số--", new {onchange = "getserial()", @class = "required form-control", title = Resources.Einvoice.ReplaceInv_ReqPattern })%>
                     </div>
                 </div>
                <div class="form-group">
                     <label class="col-sm-5 control-label"><%=Resources.Einvoice.ReplaceInv_LblSerial%> (<i>Serial No</i>): <span style="color: red">(*)</span></label>
                     <div class="col-sm-7">
                          <%=Html.DropDownList("serial", Model.lstserial,"--Chọn ký hiệu--", new { name = "serial", @class = "required form-control", title = Resources.Einvoice.ReplaceInv_ReqSerial })%>
                     </div>
                 </div>
                 <div class="form-group">
                     <label class="col-sm-5 control-label"><%=Resources.Einvoice.ReplaceInv_LblInvNo%>(<i>No</i>): <span style="color: red">(*)</span></label>
                     <div class="col-sm-7">
                           <%=Html.TextBox("invNo", Model.invNo, new {  @class = "required form-control", MaxLength = "7", @onchange = "pad(this.value,7)", onkeypress = "return keypress(event);", title = Resources.Einvoice.ReplaceInv_ReqInvNo })%>
                     </div>
                 </div>
            </div>
            <div class="box-footer">
                   <button class="btn btn-sm btn-primary element-center" type="submit"><i class="fa fa-check"></i><%=Resources.Einvoice.ReplaceInv_BtnReplace%></button>
            </div>
        </div>
            </div>
    </div>
      <%} %>
     
    <script type="text/javascript">
        $(document).ready(function () {
            $('form:first').validate();
        });
        // định dạng số hóa đơn
        function pad(number, length) {
            var str = '' + number;
            while (str.length < length) {
                str = '0' + str;
            }
            if (str == "0000000")
                return $("#invNo").val("");
            return $("#invNo").val(str);
        }
        //Định dạng ký tự nhập
        function keypress(e) {
            var keypressed = null;
            if (window.event) {
                keypressed = window.event.keyCode; //IE
            }
            else {
                keypressed = e.which; //NON-IE, Standard
            }
            if (keypressed < 48 || keypressed > 57) {
                if (keypressed == 8 || keypressed == 127) {
                    return;
                }
                return false;
            }
        }
        // lấy ra danh sách serial khi chọn pattern
        function getserial() {
            var jsd = "pattern=" + $('#pattern').val();
            $.ajax({
                type: "POST",
                url: "/AdJust/getserial/",
                data: jsd,
                success: function (data) {
                    sl = document.getElementById("serial");
                    while (sl.firstChild) {
                        sl.removeChild(sl.firstChild);
                    }
                    if (data.q.length > 0) {
                        newOpt = new Option("--Chọn ký hiệu--", "");
                        document.getElementById("serial").options.add(newOpt);
                        newOpt.selected = true;
                        for (i = 0; i < data.q.length; i++) {
                            newOption = new Option(data.q[i]);
                            document.getElementById("serial").options.add(newOption);
                        }
                    }
                }
            });
        }
    </script>
</asp:Content>
