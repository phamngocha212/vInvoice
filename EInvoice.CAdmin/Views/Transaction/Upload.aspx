<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<UploadModel>" %>

<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Upload dữ liệu
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <style type="text/css">
        input[type=file] {
            display: none;
            visibility: hidden;
        }

        input[type=text] {
            resize: none;
            padding: 0px 5px;
            border: none;
            border-bottom: 1px dashed #ccc;
            height: 22px;
            background-color: #fee !important;
        }

        label.error {
            color:#f00;
        }
    </style>
    <div class="wrap-preload" style="display: none">
        <div class="showbox">
            <div class="loader">
                <svg class="circular" viewBox="25 25 50 50">
                    <circle class="path" cx="50" cy="50" r="20" fill="none" stroke-width="2" stroke-miterlimit="10" />
                </svg>
            </div>
            <p>Uploading ... </p>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-8 col-xs-offset-2">
            <div class="box box-primary">
                <div class="box-header with-border">
                    <h4 class="box-title text-center"><i class="fa fa-upload"></i>
                        <%=Model.TypeLabel%>
                    </h4>
                </div>


                <%if (Model.TypeTrans == 0 || Model.TypeTrans == 3)
                  { %>
                <%Html.RenderPartial("UploadInvoiceData", Model); %>
                <%}
                  if (Model.TypeTrans == 1)
                  {%>
                <%Html.RenderPartial("UploadCustomerData", Model); %>
                <%} if (Model.TypeTrans == 2)
                  {%>
                <%Html.RenderPartial("UploadInvoiceCancelData", Model); %>
                <%}%>
            </div>
        </div>
    </div>
    <script type="text/javascript">

        function htmlEncode(value) {
            return $('<div/>').text(value).html();
        }
        function htmlDecode(value) {
            return $('<div/>').html(value).text();
        }
        function SaveUpload() {
            if (document.getElementById("TypeTrans").value == 0) {
                var str = $('#Month').val();
                while (str.length < 2) {
                    str = '0' + str;
                }
                $('#DateNow').val($('#Year').val() + str);
            }
            if ($('#adminForm').valid()) {
                $("#uploadButton").hide();
                $(".wrap-preload").css("display", "block");
                $(".box").addClass("blur");
            }
        }

        $(document).ready(function () {
            $('form:first').validate({ onfocusin: false });

            $(".datepicker").datepicker({
                showOn: "button",
                format: "dd/mm/yyyy",
                showButtonPanel: true,
                buttonImageOnly: true,
                changeMonth: true,
                changeYear: true
            });

            $('#_txtFile').click(function () {
                $('#FilePath').trigger('click');
            }).keypress(function (e) {
                if (e.keyCode == 13 || e.which == 13) {
                    $(this).trigger('click');
                    return false;
                }
            });

            $('#FilePath').change(function () {
                var _file = $(this)[0].files[0];
                if (_file) {
                    var _extl = ['zip', 'xls', 'xlsx'];
                    var _ext = _file.name.substring(_file.name.lastIndexOf('.') + 1);
                    if (_extl.indexOf(_ext) > -1) {
                        $('#_txtFile').val(_file.name);
                    }
                    else {
                        $(this).val('');
                        sweetAlert("Lỗi!", 'Định dạng file đính kèm không được chấp nhận.', "error");
                    }
                    if (this.files[0].size > 104857600) {
                        alertify.alert("Dung lượng file phải nhỏ hơn 100MB!");
                        $('#FilePath').val('');
                    }
                } else {
                    $('#_txtFile').val('');
                }
            });
        });
    </script>
</asp:Content>
