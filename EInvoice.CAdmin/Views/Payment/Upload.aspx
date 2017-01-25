<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thanh toán hóa đơn
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/SharedFormElements.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="/Content/js/jquery.validate.min.js"></script>
    <script src="/Content/js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <style type="text/css">
        fieldset
        {
            margin: 20px 20px 10px 20px !important;
        }
        fieldset label
        {
            float: left;
            margin-right: 0px;
            max-width: 150px;
        }
        fieldset li
        {
            margin-left: 10px;
        }
    </style>
    <h4 style="text-align:center">
            Gạch nợ theo lô
        </h4>
    <div style="padding: 10px;">        
        <div style="text-align:center">
            <p style="background-color: Silver; color: Black; padding: 20px; font-size: larger">
                Chọn file gạch nợ .xls Upload lên hệ thống, dung lượng file không vượt quá 3MB!<br />
                Sử dụng báo cáo chi tiết thu theo ngày trên CT QLNVT: "1.15-HDDT-Gửi cập nhật đồng bộ"
            </p>
            <form method="post" action="/payment/upload" id="adminForm" name="adminForm" enctype="multipart/form-data">
            <fieldset>
                <ol>
                    <li>
                        <label>
                            Chọn file:<span style="color: red">(*)</span>
                        </label>
                        <input type="file" name="FilePath" id="FilePath" style="float: left" class="required"/>
                    </li>
                </ol>
            </fieldset>
            <div class="clear">
            </div>
            <p>
                <input id="CLick" type="submit" name="phathanh" class="required"
                    value="UPLOAD FILE DỮ LIỆU" style="margin-left: 50px; margin-top: 30px; width: 200px;
                    height: 30px" onclick="checkSize();"/></p>
            </form>
        </div>
    </div>
    <script type="text/javascript">
        function checkSize() {
            $.blockUI({
                css: {
                    border: 'none',
                    padding: '15px',
                    backgroundColor: '#eee',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: .5,
                    color: '#fff'
                },
                message: '<h1>Xin vui lòng đợi.</h1>', fadeIn: 0,
                fadeOut: 10, timeout: 240000
            });
        }      
    </script>
</asp:Content>
