/// <reference path="jquery-1.3.2.js" />
/// <reference path="../ui/ui.core.js" />
/// <reference path="../ui/ui.dialog.js" />
/// <reference path="../ui/ui.draggable.js" />
/// <reference path="../ui/ui.resizable.js" />
/// <reference path="FanxiAjax.js" />

//include
//////////////////////////////////////////////////////////////////////////
var widthTemp;
var heightTemp;

function NewWindow(path, _width, _height) {
    $("<div>").append($("<iframe src='" + path + "' style='height:100%; width:100%'>")).dialog({
        bgiframe: false,
        width: _width,
        height: _height,
        modal: true,
        title: '',
        close: onClose
    });
}

function NewWindowAjax(path, params, _width, _height) {
    if (!params) params = new Array();
    widthTemp = _width;
    heightTemp = _height;
    ajaxcallplain(path, params, writecontent, "GET", null, null);
}

function writecontent(ret) {
    try {
        $("<div>").html(ret).dialog({
            bgiframe: false,
            width: widthTemp,
            height: heightTemp,
            modal: true,
            title: '',
            close: onClose
        });
    }
    catch (ex)
 { window.location.reload(); }
}

function onClose() {
    document.body.removeChild(document.body.lastChild);
}




