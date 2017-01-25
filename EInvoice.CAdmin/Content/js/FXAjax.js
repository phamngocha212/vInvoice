/// <reference path="jquery-1.3.2.js" />

function getBrowserID() { 
    var browser=navigator.appName;
    var b_version=navigator.appVersion;
    var version=parseFloat(b_version);
    switch(browser)
    {
        case "Microsoft Internet Explorer" :
           return 1;
           break;
        case "Netscape" :
           return 2;
           break;

       default:
           return -1;
    }
}


function ajaxcall(url, params, onsusscessFull, method, onFailure, onAborted) // "Name='afds'"
{
    var urlStr = url;
    var _param = "";
    for (var i = 0; i < params.length; i++) {
        _param += params[i];
        if (i < params.length - 1) _param += "&";
    }
    if (method.toUpperCase()== "GET") urlStr += "?" + _param ;
    //inint ajax object
    var xmlHttp=null;
   if(getBrowserID() == 1)  // neu la IE
   {
        try{xmlHttp=new ActiveXObject("Msxml2.XMLHTTP");}
        catch (e){xmlHttp=new ActiveXObject("Microsoft.XMLHTTP");}
   }
   else xmlHttp = new XMLHttpRequest();  // Firefox, Opera 8.0+, Safari
    //end init    
    if (xmlHttp==null)
    {
        alert ("Browser does not support HTTP Request");
        return;
    }
    xmlHttp.onreadystatechange = function() {
        if (xmlHttp.readyState == 4) {

            if ((xmlHttp.status == 200) && onsusscessFull) {
                try {
                        var ret = JSON.parse(xmlHttp.responseText); 
                }
                catch (ex) {
                    onFailure(xmlHttp.responseText);
                }
                onsusscessFull(ret);
            }
            else if (xmlHttp.status != 200 && onFailure) onFailure(xmlHttp.responseText);
        }
    }

     //xmlHttp.abort() 
     xmlHttp.open(method, urlStr, true);
     xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
     xmlHttp.send(_param);
 }


 function ajaxcallplain(url, params, onsusscessFull, method, onFailure, onAborted, UpdateTargetId) // "Name='afds'"
 {
     var urlStr = url;
     var _param = "";
     for (var i = 0; i < params.length; i++) {
         _param += params[i];
         if (i < params.length - 1) _param += "&";
     }
     if (method.toUpperCase() == "GET") urlStr += "?" + _param;
     //inint ajax object
     var xmlHttp = null;
     if (getBrowserID() == 1)  // neu la IE
     {
         try { xmlHttp = new ActiveXObject("Msxml2.XMLHTTP"); }
         catch (e) { xmlHttp = new ActiveXObject("Microsoft.XMLHTTP"); }
     }
     else xmlHttp = new XMLHttpRequest();  // Firefox, Opera 8.0+, Safari
     //end init    
     if (xmlHttp == null) {
         alert("Browser does not support HTTP Request");
         return;
     }
     xmlHttp.onreadystatechange = function() {
         if (xmlHttp.readyState == 4) {

             if ((xmlHttp.status == 200) && onsusscessFull) {
                 try {
                     var ret = xmlHttp.responseText;
                 }
                 catch (ex) {
                     onFailure(xmlHttp.responseText);
                 }
                 if (UpdateTargetId) $('#' + UpdateTargetId).html(ret);
                 onsusscessFull(ret);
                
             }
             else if (xmlHttp.status != 200 && onFailure) onFailure(xmlHttp.responseText);
         }
     }

     //xmlHttp.abort() 
     xmlHttp.open(method, urlStr, true);
     xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
     xmlHttp.send(_param);
 }

 function encodeMyHtml(value) {
     return value.replace(/&/g, "%26");
 } 
