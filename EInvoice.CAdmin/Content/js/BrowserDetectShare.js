/// <reference path="jquery-1.3.2.js" />
/// file này dành cho việc download plugin và hiển thị hóa đơn ajax
function create(htmlStr) {
    var frag = document.createDocumentFragment(),
        temp = document.createElement('div');
    temp.innerHTML = htmlStr;
    while (temp.firstChild) {
        frag.appendChild(temp.firstChild);
    }
    return frag;
}

$(function () {    
    fragment = create('<div id="ViewInvoice" class="modal modal-default">' +
        '<div class="modal-dialog">' +
            '<div class="modal-content">' +
                '<div class="modal-body">' +
                    '<div id="container"></div>' +
                    '<div id="inbt"></div>' +
                '</div>' +
                '<div class="modal-footer" style="position:relative;z-index:1000000" id="invoice-footer"></div>' +
            '</div>' +
        '</div>' +
      '</div>');
    document.body.insertBefore(fragment, document.body.childNodes[0]);    
})

function viewInv(idInv, pattern) {
    $('#4plugin').remove();
    document.getElementById("inbt").innerHTML = "";
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
    var jsondata = { idInvoice: idInv, pattern: pattern };
    $.ajax({
        type: "POST",
        url: "/Share/ajxPreview",
        data: jsondata,
        success: function (data) {
   
            $("#container").html(data.invData + "<div class='pagination'></div>");
            $("#invoice-footer").html("<button type='button' class='btn btn-default pull-left btn-sm' data-dismiss='modal'><i class='fa fa-close'></i>Close</button>"+
                "<button type='button' class='btn btn-primary pull-right btn-sm' onclick='printInvoice()'><i class='fa fa-print'></i>In hóa đơn</button>");
            $('#ViewInvoice').modal('show');
            $('.VATTEMP .invtable .prds').ProductNumberPagination({ number: 10 });
              $.unblockUI();
        },
        error: function () {
            $.unblockUI();
        }
    });
}

function ajxCall(idInvoice, pattern) {
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
    var jsondata = { idInvoice: idInvoice, pattern: pattern };
    $.ajax({
        type: "POST",
        url: "/Share/ajxPreview",
        data: jsondata,
        success: function (data) {            
            $("#container").html(data.invData + "<div class='pagination'></div>");
            document.getElementById("invoice-footer").innerHTML =
                            "<button type='button' class='btn btn-default pull-left btn-sm' data-dismiss='modal'><i class='fa fa-close'></i>Close</button>";

            $('#ViewInvoice').modal('show');
            $('.VATTEMP .invtable .prds').ProductNumberPagination({ number: 10 });
            $.unblockUI();
        },
        error: function () {
            $.unblockUI();
        }
    });
}

function ajxCall4Convert(idInvoice, pattern, str) {
    var jsondata = { id: idInvoice, patt: pattern };
    $.ajax({
        type: "POST",
        url: "/InvConvertion/" + str,
        data: jsondata,
        success: function (data) {
            //document.getElementById("ViewInvoice").innerHTML = data;
            if (data == "nochange" || data == "nosuccess") {
                alertify.alert("Hóa đơn không được chuyển đổi.");
                ChangePage();
            }
            else {
                //phan trang
                //$("#container").html(data + "<div class='pagination'><span id='prev' class='prevnext'><</span><span id='next' class='prevnext'>><input type='button' value='Chuyển đổi' id='convertPagination'/></span></div>");
                $("#container").html(data + "<div class='pagination'></div>");
                $("#invoice-footer").html(" <button class='btn btn-sm btn-primary' type='button' id='convertPagination' onclick='print4Convert()' style='z-index:100000;position:relative;bottom:0;'><i class='fa fa-print'></i> Chuyển đổi</button><button type='button' class='btn btn-default pull-left btn-sm' data-dismiss='modal'><i class='fa fa-close'></i>Close</button>");
                var fragment = create('<div id="ds" style="display:none"></div>')
                document.body.insertBefore(fragment, document.body.childNodes[0]);
                $('#ViewInvoice').modal('show');
                //phan trang
                $('.VATTEMP .invtable .prds').ProductNumberPagination({ number: 10 });
                $('#ViewInvoice').on('hidden.bs.modal', function () {
                    ChangePage();               
                });
            }
        }

    });
}

function printInvoice() {
   $("#inbt").css("display", "none");
    var printElement = document.getElementById("container");
    $(printElement).printArea({
        mode: "iframe",
        popWd: 1000,
        popHt: 900,
        popClose: false
    });
 $("#inbt").css("display", "block");
    return (false);
}

