<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html>

<head runat="server">

    <%--<script src="http://code.jquery.com/jquery-latest.js"></script>--%>
    <script type="text/javascript" src="/Content/js/jquery.min.js"></script>
    <script>

     
    </script>
    <title>ajxPage</title>
</head>
<body>
    <div>
    </div>
</body>
</html>
<script language="javascript" type="text/javascript">
    var _app = navigator.appName;
    if (_app == 'Microsoft Internet Explorer') {
        document.write(
              '<applet CODE="VNPTApplet" CODEBASE="." ARCHIVE="/Content/jar/VNPT_HDDT_Tool.jar" id="VNPTApplet" NAME="VNPTApplet" WIDTH="0" HEIGHT="0">',
              '	<param name="archive" value="/Content/jar/VNPT_HDDT_Tool.jar">',
              '	<param name="code" value="Applet/VNPT_CA_Applet">',
              '	<param name="mayscript" value="yes">',
              '	<param name="scriptable" value="true">',
              '	<param name="name" value="VNPT_CA_Applet">',
              '	If you see this text then Java is disabled in your browser. Please download the Sun Java Plugin from "www.sun.com".',
              '</applet>'
          );
    }
    else {
        document.write(
              '<object type="application/x-java-applet;version=1.4.1" name="VNPTApplet" id="VNPTApplet" height="0" width="0">	',
              '<param name="archive" value="/Content/jar/VNPT_HDDT_Tool.jar">',
              '<param name="code" value="Applet/VNPT_CA_Applet">',
              '<param name="mayscript" value="yes">',
              '<param name="scriptable" value="true">',
              '<param name="name" value="CA_Applet">',
              '	If you see this text then Java is disabled in your browser. Please download the Sun Java Plugin from "www.sun.com".',
              '</object>'
          );
    }
</script>
<%
    string sr = (string)ViewData["sr"];
    string pt = (string)ViewData["pt"];
    //IList<string> data = (IList<string>)ViewData["data"];
%>
<script language="javascript" type="text/javascript">

    function htmlEncode(value) {
        return $('<div/>').text(value).html();
    }
    function htmlDecode(value) {
        return $('<div/>').html(value).text();
    }
    $(document).ready(function () {
        var app1 = document.getElementById("VNPTApplet");
        var serial = "<%=sr%>";
        var pattern = "<%=pt%>";
        var jsondata = {};
        $.ajax({
            type: "POST",
            url: "/Adjust/seq/",
            data: jsondata,
            success: function (data) {
                if (data != null) {
                    var a = new Array(data.length);
                    var d = [];
                    for (var i = 0; i < data.length; i++) {
                        a[i] = app1.signSerial(data[i].split("@")[0], serial);
                        //debugger;
                        if (a[i] == null) {
                            alert(<%=Resources.Message.AdjReInv_MesSignUnsuccess %>);
                            document.location = "/EInvoice/Index";

                            return;
                        }
                    }
                    //alert(signedData);}
                    //debugger;
                    verifyClient(a, data, pattern);
                }
                else {
                    alert("err!");
                    //$.unblockUI();
                }
            }
        });

    });

    function verifyClient(signedData, data, pattern) {
        //debugger;
        //var str = $(xdata);
        d = [];
        for (i = 0; i < data.length; i++)
            d.push(htmlEncode(data[i].split("@")[1]));
        var jsondata = { listData: JSON.stringify(d), listValue: JSON.stringify(signedData), pattern: pattern };
        $.ajax({
            type: "POST",
            url: "/Adjust/verifySignature/",
            data: jsondata,
            success: function (data) {
                //if (data == "") {
                //$.unblockUI();
                document.location = "/EInvoice/Index";
            },
            error: function (xhr, error) {
                //$.unblockUI();
                //debugger;
                alert(xhr.responseText);
            }
        });
    }
</script>
