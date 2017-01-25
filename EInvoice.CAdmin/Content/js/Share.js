/// <reference path="jquery-1.3.2.js" />
function checkMST(obj, btnID) {
    var jsondata = "mst=" + obj;
    $.ajax({
        type: "POST",
        url: "/Company/checkMST/",
        data: jsondata,
        success: function (data) {
            if (!data) {
                alert("MST chưa đúng !");
            }
            else {
                if (btnID != "")
                    document.forms[0].submit();
                else
                    alert("MST hợp lệ !");
            }
        }
    });
};

function cutbyMaxlength(id, maxLength) {    
    var utf8codeUnits = 0;
    var cChar = 0;
    var str = "";
    var v = $('#' + id).val();
    while (utf8codeUnits < maxLength) {
        if (maxLength > v.length)
            break;
        var c = v.charCodeAt(cChar);
        str += v[cChar];
        cChar++;
        if (c < 128) {
            utf8codeUnits++;
        }
        else if ((c > 127) && (c < 2048)) {
            utf8codeUnits = utf8codeUnits + 2;
        }
        else {
            utf8codeUnits = utf8codeUnits + 3;
        }
    }
    if (cChar > 0)
        $('#' + id).val(str);
}

function nextFocus(controlID) {
    lst = document.forms[0].elements;
    for (i = 0; i < lst.length; i++) {
        if (lst[i].id == controlID) {
            document.getElementById(controlID).style.display = "block";
            $("#" + controlID).focus();
            setTimeout(document.getElementById(controlID), 0);
            if (i != lst.length - 1)
                lst[i + 1].focus();
            else lst[i - 1].focus();
            document.getElementById(controlID).style.display = "none";
        }
    }
}