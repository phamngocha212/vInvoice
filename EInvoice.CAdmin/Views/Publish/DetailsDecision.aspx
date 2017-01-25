<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<Decision>" %>
<%@ Import Namespace="EInvoice.Core.Domain" %>
<%@ Import Namespace="EInvoice.CAdmin.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Quyết định phát hành hóa đơn
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
                   
    <script src="/Content/js/FXUtils.js" type="text/javascript"></script>
    <script src="/Content/js/jquery.PrintArea.js" type="text/javascript"></script>
    <style type="text/css">         
        ol li label {
        max-width:520px !important; 
        }
        .word_wrap
        {
            white-space: pre-wrap; /* css-3 */
            white-space: -moz-pre-wrap; /* Mozilla, since 1999 */
            white-space: -pre-wrap; /* Opera 4-6 */
            white-space: -o-pre-wrap; /* Opera 7 */
            word-wrap: break-word; /* Internet Explorer 5.5+ */  
            vertical-align: top;      
            font-weight:100; 
        }
        ol{
            list-style-type:none !important;            
        }
</style>
<% IList<Pupor> lst = (IList<Pupor>)ViewData["Data"]; %>
<form id ="detailDecision" method="post">
<%=Html.Hidden("id",Model.id) %>
<div id="test">
<div id="decision">	    
    <style>ol
{
    margin-left:10px;
}
#decision-bottom .fl-r{
	float:right;
	width:33%;
	
	}
#decision-bottom .fl-l{
	float:left;
	width:65%;
	margin:0;
	}
#decision{
	height: 900px;
    width: 730px;
	color:#333;
	padding-right:10px;	
	margin:10px auto;
	font-size:14px;
	font-family:"Times New Roman", Times, serif;
	}
#decision li{
	margin-bottom:10px;
    list-style:none;
	}
	
	.centert{
	text-align:center;
	}
		
#decision  h2, h3, h4, h5, h6 {
	font-style: normal;
	font-weight: normal;
	line-height: 1.2em; /* Big heading look nicer with smaller line-height */
	margin: .7em 0 .5em;
}

#decision h2 {
	font-size: 1.75em;
}
#decision h3 {
	font-size: 1.5em;
}
#decision h4 {
	font-size: 1.2em;
	font-weight: bold;
}
#decision h5 {
	font-size: 1em;
	font-weight: bold;
}
#decision h6 {
	font-size: 1em;
	font-style: italic;
}
	
	</style>   
        <div style="float:right;width:44%;text-align:center">
        	<span style="font-size:14px; font-weight:700;">
                CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM<br/>
                <strong>Độc Lập - Tự Do - Hạnh Phúc</strong>
            </span>
            <p>***********</p>
            <p><%=Html.Encode(Model.City) %>, ngày <%=Html.Encode(Model.CreateDate.Day) %> tháng <%=Html.Encode(Model.CreateDate.Month) %> năm <%=Html.Encode(Model.CreateDate.Year) %></p>
        </div>          
        <div style="float:left;width:56%;font-size:14px">
        	<div class="clearfix">
                <label class="word_wrap"><%=Html.Encode(Model.ParentCompany.ToUpper()) %></label>
            </div>
            <div class="clearfix">
                <label class="word_wrap"  style="font-weight:700;"><%=Html.Encode(Model.ComName)%></label>
            </div>
            <label class="word_wrap"  style="font-weight:700;">Số:<%=Html.Encode(Model.DecisionNo)%></label>
        </div>                               
    
    <div class="centert" id="dec-title" style="text-align:center;padding-top:40px">    
      <h5 style="margin-top:96px;text-align:center">QUYẾT ĐỊNH CỦA <label class="word_wrap"  style="font-weight:700;"><%=Html.Encode(Model.ComName.ToUpper()) %></label></h5>
            <i style="font-weight:700;">V/v: Áp dụng hoá đơn điện tử</i>
            <div>-----------------------</div>
    </div>
    <h5 class="centert"><p>GIÁM ĐỐC <label class="word_wrap" style="font-weight:700;"><%=Html.Encode(Model.Director.ToUpper())%></label></p></h5>
   <%-- <p>-Căn cứ Thông tư số 32/2011/TT-BTC ngày 14/3/2011 của Bộ Tài chính hướng dẫn về khởi tạo, phát hành và sử dụng hoá đơn điện tử bán hàng hóa, cung ứng dịch vụ.</p>
    <p>-Căn cứ Quyết định thành lập (hoặc Giấy đăng ký kinh doanh) số:	<%=Model.DecisionNo %></p>
    <p>-Xét đề nghị của Ông/Bà:	<%=Model.Requester%></p>--%>
    <label class="word_wrap"><i><%=Model.Requester%></i></label>
    <h5 class="centert">QUYẾT ĐỊNH</h5>
    <p><span class="step"><b>Điều 1:</b></span> Áp dụng hình thức hóa đơn điện tử trong đơn vị từ ngày <%=ViewData["mmddyy"]%>  trên cơ sở hệ thống thiết bị và các bộ phận kỹ thuật liên quan như sau:</p>     
    <ol>    
        <li><b>1. Hệ thống thiết bị</b><label class="word_wrap"><%=Html.Encode(Model.SystemName)%></label></li>
        <li><b>2. Phần mềm ứng dụng</b><br /><label class="word_wrap" style="margin-left:20px"><%=Html.Encode(Model.SoftApplication)%></label></li>
        <li><b>3. Bộ phận kỹ thuật hoặc tên nhà cung ứng dịch vụ chịu trách nhiệm về mặt kỹ thuật
        hóa đơn điện tử, phần mềm ứng dụng</b></li>
        <li style="margin-left:20px"><label class="word_wrap"><%=Html.Encode(Model.TechDepartment)%></label></li>
    </ol>
    <p><span class="step"><b>Điều 2:</b></span> Mẫu các loại hóa đơn và mục đích sử dụng của mỗi loại hóa đơn</p>
    <ol style="margin-left:20px">
        <%int i = 1;
          foreach (Pupor inv in lst)
          {%>
        <li><%=i %>.<strong> <label class="word_wrap"><%=Html.Encode(inv.InvCateName)%></label></strong></li>
    	<li style="margin-left:10px"><p>- Mẫu số: <label class="word_wrap"><%=Html.Encode(inv.InvPattern)%></label></p></li>
        <li style="margin-left:10px"><p>- Mục đích: <label class="word_wrap"><%=Html.Encode(inv.Mucdich)%></label></p></li>        	
        <%i++;
        } %>              
    </ol>
    <p><span class="step"><b>Điều 3:</b></span> Quy trình khởi tạo, lập, luân chuyển và lưu trữ dữ liệu hoá đơn điện tử trong nội bộ tổ chức.</p>
    <ol style="margin-left:20px"><li><label class="word_wrap"><%=Model.Workflow%></label></li></ol>
    <p><span class="step"><b>Điều 4:</b></span> Trách nhiệm của từng bộ phận trực thuộc liên quan việc khởi tạo, lập, xử lý, luân chuyển và lưu trữ dữ liệu hoá đơn điện tử trong nội bộ tổ chức bao gồm cả trách nhiệm của người được thực hiện chuyển đổi hóa đơn điện tử sang hóa đơn giấy.</p>
    <ol style="margin-left:20px"><li><label class="word_wrap"><%=Model.Responsibility%></label></li></ol>   
   <%-- <p><span class="step"><b>Điều 5:</b></span> Quyết định này có hiệu lực thi hành kể từ ngày  <%=ViewData["mmddyy"]%>.  Lãnh đạo các bộ phận kế toán, bộ phận bán hàng, bộ phận kỹ thuật... chịu trách nhiệm triển khai, thực hiện Quyết định này.</p>--%>
   <p> <span class ="step"><b>Điều 5:</b></span><label class="word_wrap"><%=Model.EffectiveDate%></label></p>
    <br />
    <div id="decision-bottom">
        <div class="fl-l">
            <p><strong>Nơi nhận</strong></p>
           <%-- <p>- Cơ quản Thuế trực tiếp quản lý (Cục, Chi Cục, ...)</p>
            <p>- Như điều 4 ( đề thể hiện )</p>
            <p>- Lãnh đạo đơn vị</p>
            <p>- Lưu</p>--%>
            <label class="word_wrap"><%=Model.Destination%></label>
        </div>
        <div class="fl-r">
        	<p style="margin-left:50px;">GIÁM ĐỐC</p>
            <p><i>( Ký, đóng dấu, ghi rõ họ tên )</i></p>
        </div>
    </div>
</div>
</div>
<div  style="padding-top:0px;margin:100px auto;text-align:center;clear:both">             
    <button class="btn btn-sm  btn-success" type="button" onclick="JavaScript: printex('test');"><i class="fa fa-print"></i> In quyết định</button>
    <%if (Model.Status== 0)
      { %>    
    <button class="btn btn-sm  btn-success" type="button" onclick="Dec_click(0)"><i class="fa fa-send"></i> <%=Resources.Einvoice.Dec_BtnSend%></button>
    <%}else %>
    <%if(Model.Status==1) {%>     
    <button class="btn btn-sm  btn-success" type="button" onclick="Dec_click(1)"><i class="fa fa-check"></i> <%=Resources.Einvoice.Dec_BtnAccept%></button>
    <%} %>    
    <button class="btn btn-sm" type="button" onclick="document.location = '/Publish/ListDecision'">
        <i class="fa fa-backward"></i> Quay lại</button> 
</div>
</form>

<script type="text/javascript">
    function Dec_click(i) {
        var mess = "<%=Resources.Message.Dec_MesConfirmSend%>";
        if (i == 1)
            mess = "<%=Resources.Message.Dec_MesConfirmAccept %>";
        alertify.confirm(mess, function (e) {
            if (e) {
                $("#detailDecision").attr("action", "/Publish/EditStatus");
                $("#detailDecision").submit();
            }
            else return;
        });        
    }    
    function printex(divID) {
        var printElement = document.getElementById(divID);
        $(printElement).printArea({
            mode: "iframe",
            popWd: 980,
            popHt: 620,
            popClose: false
        });
    }
</script>
</asp:Content>
