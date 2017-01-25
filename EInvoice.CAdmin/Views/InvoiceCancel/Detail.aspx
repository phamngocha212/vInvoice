<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<InvCancel>" %>

<%@ Import Namespace="EInvoice.Core" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Báo cáo hủy
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/css/reportpub.css" rel="stylesheet" type="text/css" />
    <link href="/Content/css/grid.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        label
        {
            font-size: 14px !important;
        }
        li
        {
            font-size: 14px !important;
        }        
    </style>    
    <script type="text/javascript" src="/Content/js/jquery.min.js"></script>    
    <script src="/Content/js/FXUtils.js" type="text/javascript"></script>
    <form id="cancel" action="/InvoiceCancel/Detail/<%=Model.id %>" method="post">
    <div id="test">
        <div id="reported">
            <center>
                <h5>
                    CỘNG HÒA XÃ HÔI CHỦ NGHĨA VIỆT NAM<br />
                    ĐỘC LẬP - TỰ DO - HẠNH PHÚC</h5>
                <br />
                -------------------------<br />
                <br />
            </center>
            <div style="float: right; width: 150px">
             <%=Resources.Einvoice.ReCancel_NotePatternReport%>
             </div>
            <div>
                <h1 class="centert">
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <%=Resources.Einvoice.ReCancel_lblNoticeResultsInv%></h1>
                <br />
                <br />
                <br />
                <br />
                <ol>
                    <li>
                        <label>
                            1. <%=Resources.Einvoice.ReCancel_lblComName%>:
                        </label>
                        <input name="" value="<%=Html.Encode(Model.ComName) %>" type="text" class="input-border" readonly="readonly"
                        style="width: 620px" />
                        <%--<label style="width:670px;font-weight:normal" class="input-border">
                            <%=Model.ComName %></label>--%>
                    </li>
                    <li>
                        <label>
                            2. <%=Resources.Einvoice.ReCancel_lblTaxCode%>:
                        </label>
                        <input name="" type="text" value="<%=Html.Encode(Model.ComTaxCode) %>" class="input-border" readonly="readonly"
                            style="width: 680px" /></li>
                    <li>
                        <label>
                            3. <%=Resources.Einvoice.ReCancel_lblAddress%> :
                        </label>
                        <input name="" value="<%=Html.Encode(Model.ComAddress) %>" type="text" class="input-border" readonly="readonly" style="width: 699px" />
                     </li>
                    <li>
                        <label>
                            4. <%=Resources.Einvoice.ReCancel_LblMethodCancel%>:
                        </label>
                        <input name="" type="text" value="<%=Html.Encode(Model.Method) %>" class="input-border" readonly="readonly"style="width: 590px" />
                   </li>
                    <li>
                        <label>
                            <%=Resources.Einvoice.ReCancel_Time%>
                            <%=Model.Hour %>
                            <%=Resources.Einvoice.ReCancel_Hour%>
                            <%=Model.Minute %>
                             <%=Resources.Einvoice.ReCancel_Minute%>  <%=Resources.Einvoice.ReCancel_Days%> 
                            <%=Model.CancelDate.Day %>
                            <%=Resources.Einvoice.ReCancel_Months%>
                            <%=Model.CancelDate.Month %>
                            <%=Resources.Einvoice.ReCancel_Years%>
                            <%=Model.CancelDate.Year %>
                            <%=Resources.Einvoice.ReCancel_ComNameAndNoticeCancel%>:
                        </label>
                    </li>
                    <li>
                        <center>
                            <table style="min-width: 800px; width: 92%" class="grid">
                                <thead>
                                    <tr>
                                        <th width="30px">
                                            <%=Resources.Einvoice.ReCancel_LblNo%>
                                        </th>
                                        <th width="200px">
                                            <%=Resources.Einvoice.ReCancel_LblInvType%>
                                        </th>
                                        <th width="100px">
                                            <%=Resources.Einvoice.ReCancel_LblPattern%>
                                        </th>
                                        <th width="100px">
                                            <%=Resources.Einvoice.ReCancel_LblSerial%>
                                        </th>
                                        <th width="80px">
                                            <%=Resources.Einvoice.ReCancel_LblFromNo%>
                                        </th>
                                        <th width="80px">
                                            <%=Resources.Einvoice.ReCancel_LblToNo%>
                                        </th>
                                        <th width="80px">
                                            <%=Resources.Einvoice.ReCancel_LblAmount%>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <%   
                                        IList<InvCancelDetails> lst = Model.InvCancels;
                                        int i = 1;
                                        foreach (InvCancelDetails icd in lst)
                                        {
                                    %>
                                    <tr>
                                        <td style="text-align: center">
                                            <%=i%>
                                        </td>
                                        <td style="text-align: center">
                                            <%=Html.Encode(icd.InvCateName)%>
                                        </td>
                                        <td style="text-align: center">
                                            <%=Html.Encode(icd.InvPattern)%>
                                        </td>
                                        <td style="text-align: center">
                                            <%=Html.Encode(icd.InvSerial)%>
                                        </td>
                                        <td style="text-align: center">
                                            <%=icd.FromNo.ToString("0000000")%>
                                        </td>
                                        <td style="text-align: center">
                                            <%=icd.ToNo.ToString("0000000")%>
                                        </td>
                                        <td style="text-align: center">
                                            <%=icd.Quantity%>
                                        </td>
                                    </tr>
                                    <% i++;
                                    }%>
                                </tbody>
                            </table>
                        </center>
                    </li>
                </ol>
            </div>
            <div class="centert" id="Div1" style="float: left; margin-left: 40px">
                <p>
                    <strong><%=Resources.Einvoice.ReCancel_LblPreparedPerson%></strong></p>
                <i>(<%=Resources.Einvoice.ReCancel_lblSignAndName%>)</i>
            </div>
            <div class="centert" id="agent">
                <p>
                    <%=Resources.Einvoice.ReCancel_Days%>
                    <input name="" value="<%=Model.PublishDate.Day %>" type="text" class="input-border" style="width: 20px" />
                    <%=Resources.Einvoice.ReCancel_Months%>
                    <input name="" value="<%=Model.PublishDate.Month %>" type="text" class="input-border"style="width: 20px" />
                    <%=Resources.Einvoice.ReCancel_Years%>
                    <input name="" value="<%=Model.PublishDate.Year %>" type="text" class="input-border" style="width: 30px" />
                </p>
                <p>	
                    <strong><%=Resources.Einvoice.ReCancel_lblRepPerson%></strong>
                </p>
                <i><%=Resources.Einvoice.ReCancel_lblSignAndStampAndName%></i>
            </div>
        </div>
    </div>
    <div style="padding-top: 0px" class="noprint"> 
        <input type="button" id="print_button" value="<%=Resources.Einvoice.Btn_Print%>" onclick="JavaScript:printex('test');" />
        <input type="button" value="<%=Resources.Einvoice.BtnBack%>" onclick="document.location = '/InvoiceCancel/Index'" />
    </div>
    </form>
    <script type="text/javascript" language="javascript">
        function printex(divID) {
            var printElement = document.getElementById(divID);
            $(printElement).printArea({
                mode: "popup",
                popWd: 900,
                popHt: 600,
                popClose: false
            });
        }
    </script>
</asp:Content>
