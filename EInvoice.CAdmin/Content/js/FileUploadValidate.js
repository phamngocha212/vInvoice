/// <reference path="jquery-1.3.2.js" />
function fileCheck(imgId, divUploadId, fileTypeId) {
    //debugger;
    if (document.getElementById(divUploadId).style.display == 'none') {
        document.getElementById(divUploadId).style.display = 'block'
        document.getElementById(imgId).src = '/Content/Images/no.png';
        document.getElementById(imgId).title = 'Hủy';
        document.getElementById(divUploadId).innerHTML = "<input name='" + fileTypeId + "' type='file' size='37' title='Chọn file XML' class='' id='" + fileTypeId + "' onblur='TestFileType('" + this + "','" + fileTypeId + "')' '/>";
        document.getElementById(fileTypeId).className = 'required';
    }
    else {
        //TestFileType(document.getElementById(fileTypeId), fileTypeId)
        //if (d != 1) {
        document.getElementById(divUploadId).style.display = 'none'
        document.getElementById(imgId).src = '/Content/Images/upload.png';
        document.getElementById(imgId).title = 'Upload File';
        document.getElementById(divUploadId).removeChild(document.getElementById(fileTypeId));
        //}
    }
}

//d = 0;
function TestFileType(fileName, rfileType, fileInputContainer) {
    //debugger;
    if (fileName.value == "") return;
    //if (!fileName) return;
    dots = fileName.value.split(".")
    fileType = dots[dots.length - 1];
    //$("#xmlText").val(fileName);
    if (fileType != rfileType) {
        alert("Không đúng định dạng " + rfileType + "!");
        var parent = document.getElementById(fileInputContainer);
        newFileInput = document.createElement('input');
        parent.removeChild(document.getElementById(rfileType));
        newFileInput.setAttribute('type', 'file');
        newFileInput.setAttribute('id', rfileType);
        newFileInput.setAttribute('size', 37);
        newFileInput.setAttribute('title', 'Chọn file ' + rfileType);
        newFileInput.setAttribute('name', rfileType);
        //newFileInput.setAttribute('class', 'required');
        parent.appendChild(newFileInput);
        newFileInput.addEventListener("change", function () { TestFileType(this, rfileType, fileInputContainer) }, false);
    }
    preview();
}
//        function checkChange(rfileType) {
//            if (d == 1) document.getElementById(rfileType).focus();
//        }