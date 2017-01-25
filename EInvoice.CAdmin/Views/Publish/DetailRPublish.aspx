<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<Publish>" %>

<%@ Import Namespace="EInvoice.Core" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Thông báo phát hành hóa đơn điện tử
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <form id="detailpub" class="detailpub" method="post">
        <%=Html.Hidden("PublishDate",Model.PublishDate) %>
        <%=Html.Hidden("id",Model.id) %>
        <div id="print">
          <style>  
        #reported {
            height: 700px;
            width: 912px;
            color: #333;
            padding-right: 10px;
            margin: 20px auto;
            font-family: "Times New Roman", Times, serif;
            font-size: 14px !important;
            overflow:hidden;
        }

            #reported h1 {
                /*font-size:1.4em;*/
                font-size: 18px !important;
                font-weight: bold;
            }

            #reported .input-border {
                border: 0;
                border-bottom: 1px dotted #ccc;
                font-size: 15px !important;
            }

            #reported label {
                font-weight: bold;
            }

            #reported table {
                border-collapse: collapse;
                border: 1px solid #d7d7d7;
                margin-bottom: 10px;
            }

            #reported th, #reported td {
                padding: 5px;
                vertical-align: middle;
                border-right: 1px solid #d7d7d7;
                border-bottom: 1px solid #d7d7d7;
            }

            #reported th {
                text-align: center;
            }

        #agent {
            float: right;
            text-align: center;
            margin-right: 20px;
            font-size:16px;
        }

            #agent p {
                margin: 0;
            }
        /* THONG BAO */
        #reported li {
            list-style: none;
            position:relative;
            margin-bottom: 3px;
            font-size:18px;
           
        }

        .centert {
            text-align: center;
        }


        #reported h2, h3, h4, h5, h6 {
            font-style: normal;
            font-weight: normal;
            line-height: 1.2em; /* Big heading look nicer with smaller line-height */
            margin: .7em 0 .5em;
        }

        #reported h2 {
            font-size: 16px;
        }

        #reported h3 {
            font-size: 16px;
        }

        #reported h4 {
            /*font-size: 1.2em;*/
            font-size: 16px !important;
            font-weight: bold;
        }

        #reported h5 {
            /*font-size: 1em;*/
            font-size: 16px !important;
            font-weight: bold;
        }

        #reported h6 {
            /*font-size: 1em;*/
            font-size: 16px !important;
            font-style: italic;
        }
        #reported b{
    border-bottom:1px dashed #000;
    padding-bottom:3px;
    line-height:2;

        }
      #reported b:after {
        content: "";
        position: absolute;
         
        margin-left: 0;
        bottom: 4px;
        border-bottom: 1px dashed #000;
        width: 100%;
    } 
    </style>
   
             <div id="reported">
                <center><h5>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM<br /><br />
                    Độc lập - Tự do - Hạnh phúc<br /><br />
                    ---------------------------</h5>
                    </center>
                <br />
                <h1 class="centert">THÔNG BÁO PHÁT HÀNH HÓA ĐƠN ĐIỆN TỬ</h1>
                <br />
                <ol>
                    <li>
                        <label>1. Tên tổ chức khởi tạo hóa đơn: </label>
                        <b><%=Html.Encode(Model.ComName) %></b>
                         
                    <li>
                        <label>2. Mã số thuế:  </label>
                        <b><%=Html.Encode(Model.ComTaxCode) %></b>
                        
                    <li>
                        <label>3. Địa chỉ trụ sở chính: </label>
                        <b><%=Html.Encode(Model.ComAddress) %></b>
                        
                    <li>
                        <label>4. Điện thoại:  </label>
                        <b><%=Html.Encode(Model.ComPhone) %></b>
                         
                    <li>
                        <label>5. Các loại hóa đơn phát hành</label></li>
                    <li>
                        <table style="min-width: 800px; width: 100%" class="grid">
                            <thead>
                                <tr>
                                    <th width="30px">STT
                                    </th>
                                    <th width="200px">Tên loại hóa đơn
                                    </th>
                                    <th width="100px">Mẫu số
                                    </th>
                                    <th width="100px">Kí hiệu
                                    </th>
                                    <th width="80px">Số lượng
                                    </th>
                                    <th width="80px">Từ số
                                    </th>
                                    <th width="80px">Đến số
                                    </th>
                                    <th style="width: 120px; border-right: none;">Ngày bắt đầu sử dụng
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <%   
                                    IList<PublishInvoice> lstpub = (IList<PublishInvoice>)ViewData["lstpubinv"];
                                    int i = 1;
                                    foreach (PublishInvoice pubinv in lstpub)
                                    {
                                %>
                                <tr>
                                    <td style="text-align: center; border-bottom: none"><%=i%></td>
                                    <td style="text-align: center; border-bottom: none"><%=pubinv.InvCateName%></td>
                                    <td style="text-align: center; border-bottom: none"><%=pubinv.InvPattern%></td>
                                    <td style="text-align: center; border-bottom: none"><%=pubinv.InvSerial%></td>
                                    <td style="text-align: center; border-bottom: none"><%=pubinv.Quantity%></td>
                                    <td style="text-align: center; border-bottom: none"><%=pubinv.FromNo.ToString("0000000")%></td>
                                    <td style="text-align: center; border-bottom: none"><%=pubinv.ToNo.ToString("0000000")%></td>
                                    <td style="text-align: center; border-right: none; border-bottom: none"><%=pubinv.StartDate.ToString("dd/MM/yyyy")%></td>
                                </tr>
                                <% i++;
                                    }%>
                            </tbody>
                        </table>
                    </li>
                    <li>
                        <label>6. Tên cơ quan thuế tiếp nhận thông báo: </label>
                        <b><%=Html.Encode(Model.TaxAuthorityName) %></b>
                      
                    <%if (Model.Delimiter != null && !string.IsNullOrEmpty(Model.Delimiter.Trim()))
                      {%>
                    <li><strong><i><%=Html.Encode(Model.Delimiter) %></i></strong></li>
                    <%} %>
                </ol>
                <div class="centert" id="agent">
                    <p>
                        <%=Html.Encode(Model.City) %>, 
                 Ngày
                        <input name="" value="<%=Html.Encode(Model.CreateDate.Day) %>" type="text" class="input-border" style="width: 20px" />
                        tháng
                        <input name="" value="<%=Html.Encode(Model.CreateDate.Month) %>" type="text" class="input-border" style="width: 20px" />
                        năm
                        <input name="" value="<%=Html.Encode(Model.CreateDate.Year) %>" type="text" class="input-border" style="width: 30px" />
                    </p>
                    <p><strong>NGƯỜI ĐẠI DIỆN THEO PHÁP LUẬT</strong></p>
                    <i>Ký, đóng dấu và ghi rõ họ tên</i>
                </div>
            </div> 

        </div>
        <div style="margin: 10px auto; text-align: center">
            <button class="btn  btn-success" type="button" onclick="JavaScript: printex('print');"><i class="fa fa-print"></i>In thông báo</button>
            <%if (Model.Status == PublishStatus.Newpub)
              {%>
            <button class="btn  btn-primary" type="button" onclick="Publish_click(0);"><i class="fa fa-send"></i><%=Resources.Einvoice.Pub_BtnSend %></button>
            <%}
              else if (Model.Status == PublishStatus.Waiting)
              { %>
            <button class="btn  btn-success" type="button" onclick="Publish_click(1);"><i class="fa fa-check"></i><%=Resources.Einvoice.Pub_BtnAccept %></button>
            <%} %>
            <button class="btn  btn-default" type="button" onclick="document.location = '/Publish/Index'">
                <i class="fa fa-backward"></i>Quay lại</button>
        </div>
    </form>
     <script src="/Content/js/FXUtils.js" type="text/javascript"></script>  
    <script src="/Content/js/jquery.PrintArea.js" type="text/javascript"></script>  
    <script type="text/javascript" language="javascript">

        function Publish_click(i) {
            var mess = "<%=Resources.Message.Pub_MesConfirmSend%>";
            if (i == 1)
                mess = "<%=Resources.Message.Pub_MesConfirmAccept %>";
            alertify.confirm(mess, function (e) {
                if (e) {
                    $("#detailpub").attr("action", "/Publish/SelectedPublish");
                    $("#detailpub").submit();
                }
                else return;
            });
        }

        function Huy_Click() {
            $('#pubdate').dialog("close");
        }

        function printex(divID) {  
           //  $("#reported b").css({'padding-bottom' : '0px'});
            var printElement = document.getElementById(divID);
            $(printElement).printArea({
                mode: "iframe",
                popWd: 1000,
                popHt: 700,
                popClose: false
            });
            //$("#reported b").css({ 'padding-bottom': '7px' });
        }
    </script>

</asp:Content>
