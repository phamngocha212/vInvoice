<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Xác nhận tình trạng kê khai thuế
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script src="/Content/js/BrowserDetectShare.js" type="text/javascript"></script>
    <script src="/Content/js/main.js" type="text/javascript"></script>
    <script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>

    <%
        string declarationOffsetDate = ViewData["declarationOffsetDate"].ToString();
        SelectList listQuarter = new SelectList((List<string>) ViewData["listQuarter"]);
    %>

    <%= Html.Hidden("declarationOffsetDate", declarationOffsetDate)%>  
    <%= Html.Hidden("FirstDeclaration", ViewData["FirstDeclaration"])%>  

    <div class="row">
        <div class="col-xs-8 col-xs-offset-2">
            <div class="box box-primary">
                <div class="box-header  with-border">
                    <h4 class="box-title" style="text-align:center;">
                       <p style="color: red;"><i class="fa fa-file-o"></i>XÁC NHẬN TÌNH TRẠNG KÊ KHAI THUẾ</p> 
                       <p style="width: 60%; margin: 10px auto 0 auto;">Căn cứ vào khai báo của Doanh Nghiệp, hệ thống hóa đơn điện tử sẽ tạo lập hóa đơn điều chỉnh (nếu đã kê khai) thay thế / hủy hóa đơn (nếu chưa kê khai)</p>
                    </h4>
                </div>
                
                <form id="Searchform" method="post" class='form-horizontal' action="/TaxDeclaration/Declare">
                    <%= Html.Hidden("insertDeclarationDate")%>
                    <%= Html.Hidden("Processedby", ViewData["Processedby"])%>
                    <div class="box-body">
                        <div class="row">
                            <div class="col-xs-6 col-xs-offset-3">
                                <div class="form-group">
                                    <label class="col-sm-4">Lần xác nhận cuồi cùng:</label>
                                    <div class="col-sm-8">
                                        <div class="input-group">
                                            <div class="input-group-addon"><i class="fa fa-calendar"></i></div>
                                            <%=Html.TextBox("ToDate", ViewData["Date"], new { style = "margin: 0 5px 0 0px", @placeholder="__/__/____", @readonly = "readonly", @class = "form-control" })%>
                                        </div>                                       
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4">Kỳ xác nhận:</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("LastQuarter", ViewData["Quarter"],new { @name = "LastQuarter", @readonly = "readonly", @class="form-control"})%>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-sm-4">Người xác nhận:</label>
                                    <div class="col-sm-8">
                                        <%=Html.TextBox("UserName", ViewData["userName"], new { maxlength="15", @readonly = "readonly", @class="searchText form-control" })%>
                                    </div>
                                </div>
                               <div class="form-group">
                                    <label class="col-sm-4">Thực hiện kê khai:</label>
                                    <div class="col-sm-8">
                                        <%=Html.DropDownList("Quarter", listQuarter, new { @name = "Quarter", @Style = "",@class="form-control"})%>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="box-footer">
                        <button class="btn-sm btn btn-primary" style="float: right; margin-right: 30px;" type="submit"><i class="fa fa-check-circle"></i>Xác nhận</button>

                    </div>
                </form>
            </div>

        </div>
    </div>

    <script language="javascript" type="text/javascript">       
        $(document).ready(function () {
            $("button[type=submit]").click(function () {
                var declarationOffsetDate = $("#declarationOffsetDate").val();
                var quarter = $("#Quarter").val();
                var lastQuarter = $("#LastQuarter").val();
                var firstDeclaration = $("#FirstDeclaration").val();
                if (declarationOffsetDate == 'true') {
                    sweetAlert("Thông báo", "Chưa đến ngày chốt kê khai hóa đơn!", "error");
                    return false;
                }
                else if (declarationOffsetDate == 'false') {
                    if (quarter != '') {
                        currentMonth = parseInt(quarter.split(' ')[1].split('/')[0]);
                        currentYear = parseInt(quarter.split(' ')[1].split('/')[1]);

                        if (firstDeclaration == 'false') {                      
                            lastMonth = parseInt(lastQuarter.split(' ')[1].split('/')[0]);
                            lastYear = parseInt(lastQuarter.split(' ')[1].split('/')[1]);

                            if ((lastYear < currentYear) && lastMonth == 4) {
                                var today = new Date();
                                var toQuarter = (today.getMonth() + 1) % 3 == 0 ? (today.getMonth() + 1) / 3 : Math.floor((today.getMonth() + 1) / 3) + 1;
                                if (currentMonth == 1 && toQuarter < 2) {
                                    sweetAlert("Thông báo", "Quý " + currentMonth + " chưa kết thúc!", "error");
                                    return false;
                                }
                                else if (currentMonth != 1) {
                                    sweetAlert("Thông báo", "Cần kê khai hóa đơn các quý trước!", "error");
                                    return false;
                                }
                            }
                            else if (lastYear == currentYear) {
                                if ((currentMonth - lastMonth != 1)) {
                                    sweetAlert("Thông báo", "Cần kê khai hóa đơn các quý trước!", "error");
                                    return false;
                                }
                                else if (currentMonth - lastMonth == 1) {
                                    var today = new Date();
                                    var toQuarter = (today.getMonth() + 1) % 3 == 0 ? (today.getMonth() + 1) / 3 : Math.floor((today.getMonth() + 1) / 3) + 1;

                                    if (currentMonth == toQuarter) {
                                        sweetAlert("Thông báo", "Quý " + currentMonth + " chưa kết thúc!", "error");
                                        return false;
                                    }
                                }
                            }                            
                        }
                        else if (firstDeclaration == 'true') {
                            if (currentMonth != 1) {
                                sweetAlert("Thông báo", "Cần kê khai hóa đơn các quý trước!", "error");
                                return false;
                            }
                        }
                                            
                    }
                    else if (quarter == '') {
                        sweetAlert("Thông báo", "Cần chọn quý kê khai khóa đơn!", "error");
                        return false;
                    }
                }               

                $("#insertDeclarationDate").val(quarter);
            });

            //$('button[type="submit"]').prop('disabled', true);

            //$('#Quarter').change(function () {
            //    if ($("#Quarter").val() == '') {
            //        $('button[type="submit"]').prop('disabled', true);
            //    }
            //    else {
            //        $('button[type="submit"]').prop('disabled', false);
            //    }
            //});
        });       
    </script>
</asp:Content>

