function findPosX(obj) {
    /***
    Original script by Peter-Paul Koch, http://www.quirksmode.org
    ***/
    var curleft = 0;
    if (obj.offsetParent) {
        while (obj.offsetParent) {
            curleft += obj.offsetLeft
            obj = obj.offsetParent;
        }
    }
    else if (obj.x)
        curleft += obj.x;
    return curleft;
}

function findPosY(obj) {
    /***
    Original script by Peter-Paul Koch, http://www.quirksmode.org
    ***/
    var curtop = 0;
    if (obj.offsetParent) {
        while (obj.offsetParent) {
            curtop += obj.offsetTop
            obj = obj.offsetParent;
        }
    }
    else if (obj.y)
        curtop += obj.y;
    return curtop;
}


function SetPossition(divTag, objRelative) {
    divTag.style.left = findPosX(objRelative) + 'px';
    divTag.style.top = findPosY(objRelative)  + objRelative.offsetHeight + 'px';    
}


// Read a page's GET URL variables and return them as an associative array.
// using var second = getUrlVars()["name2"];
function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

//uisng jquery.blockUI.js
function inform(_Message, _BKcolor, _width, _top, _left, _right, _color) {
    if (!_BKcolor) _BKcolor = '#000';
    if (!_top) _top = '10px';
    if (!_left) _left = '';
    if (!_right) _right = "10px";
    if (!_color) _color = '#fff';
    $.blockUI({
        message: _Message,
        fadeIn: 700,
        fadeOut: 700,
        timeout: 10000,
        showOverlay: false,
        centerY: false,
        css: {
            width: _width,
            top: _top,
            left: _left,
            right: _right,
            border: 'none',
            padding: '5px',
            backgroundColor: _BKcolor,
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .6,
            color: _color
        }
    });
}

function FanxiMessage(message, error) {
    var _BKcolor = error ? "#B51104":"#004488";
    if (!message) message = error ? "Action Fail." : "Action complete.";
    inform(message, _BKcolor, "150px", "50px", "", "50px", null);
}

function BlockWaiting(message, background, overlayBackGround, Textcolor) {
    if (!message) message = 'Please Wait...';
    if (!background) background = '#000';
    if (!Textcolor) Textcolor = '#fff';
    if (!overlayBackGround) overlayBackGround = '#transparent';
    
    $.blockUI({
        message: "Please Wait...",
        css: {
            backgroundColor: '#000',
            color: '#fff'
        },
        overlayCSS: {
            backgroundColor: 'transparent'
        }
    });
}

function Confirm(_title, _mess, _type) {
    return window.confirm(_mess);
}



//message
function displayMessages() {
    $("#messagewrapper").slideDown(1000).fadeOut(10000);

    $(document).click(function () {
        $("#messagewrapper").empty();
    });
}


function processJsonMessage(messageData) {
    var messageTypes = ['Message', 'Error', 'Exception'];
    $.each(messageTypes, function (index, messageType) {
        var messageValue = eval('messageData.' + messageType);
        if (messageValue != "" && messageValue != undefined) {
            $("#messagewrapper").append('<div class="' + messageType.toLowerCase() + 'box"><img src="/Content/Images/cross.gif" class="close_message" style="float:right;cursor:pointer" alt="Close" />' + messageValue + '</div>');
        }
    });
    displayMessages();
}

function movePartialMessages() {
    // When partial views are rendered, the PartialMessagesFilter might have added some messages to the partial view output. This method 
    // moves these messages to the main messages area.
    if ($('.partialmessagewrapper').children().length > 0 && $('#messagewrapper').length > 0) {
        $('#messagewrapper').empty();
        $('#messagewrapper').append($('.partialmessagewrapper').children());
        displayMessages();
    }
}