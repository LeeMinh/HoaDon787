
function InputOnlyNumeric(event) {
    var maxLenght = 9;
    
    if (event.length >= maxLenght) {
        event.preventDefault();
        return;
    }
    // Allow special chars + arrows 
    if ( event.keyCode == 8 || event.keyCode == 9
       || event.keyCode == 13 || event.keyCode == 188 || event.keyCode == 190
        || (event.keyCode == 65 && event.ctrlKey === true)
        || (event.keyCode >= 35 && event.keyCode <= 39)) {
        return;
    } else {
    // If it's not a number stop the keypress
    if ( event.shiftKey || (event.keyCode < 48 || event.keyCode > 57)) {
        event.preventDefault();
    }
    }
}
$(document).ready(function () {
    $('.money').keydown(function (event) {
        InputOnlyNumeric(event);
    });
    //var w = screen.width * 3 / 4;
    //var h = screen.height * 3 / 4;
    //setTimeout(function () { $(".modal-content").css("style", "width: " + w + "; height: " + h + "; margin-left: -" + w / 2); }, 500);
    //$(".modal-content").width(w);
    //$(".modal-content").height(h);

});
$(".modal-wide").on("show.bs.modal", function () {
    var height = $(screen).height() * 2 / 3;
    alert(height);
    $(this).find(".modal-body").css("max-height", height);
});
function formatNumberENUS(number) {
    var number = number.toFixed(2) + '';
    var x = number.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}
function formatNumberVIVN(number) {
    var number = number.toFixed(2) + '';
    var x = number.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? (parseInt(x[1]) > 0 ? ',' + x[1] : '') : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + '.' + '$2');
    }
    return x1 + x2;
}
function formatNumber(number, culture) {
    if (isNaN(culture) || culture == '' || culture == 'vi-VN') {
        return formatNumberVIVN(number);
    }
    else {
        return formatNumberENUS(number);
    }
}

function jBeep(soundFile) {

    var soundElem, bodyElem, isHTML5;
    isHTML5 = true;
    try {
        if (typeof document.createElement("audio").play == "undefined") isHTML5 = false;
    }
    catch (ex) {
        isHTML5 = false;
    }

    bodyElem = document.getElementsByTagName("body")[0];
    if (!bodyElem) bodyElem = document.getElementsByTagName("html")[0];

    soundElem = document.getElementById("jBeep");
    if (soundElem) bodyElem.removeChild(soundElem);

    if (isHTML5) {

        soundElem = document.createElement("audio");
        soundElem.setAttribute("id", "jBeep");
        soundElem.setAttribute("src", soundFile);
        soundElem.play();

    }
    else if (navigator.userAgent.toLowerCase().indexOf("msie") > -1) {

        soundElem = document.createElement("bgsound");
        soundElem.setAttribute("id", "jBeep");
        soundElem.setAttribute("loop", 1);
        soundElem.setAttribute("src", soundFile);

        bodyElem.appendChild(soundElem);

    }
    else {

        var paramElem;

        soundElem = document.createElement("object");
        soundElem.setAttribute("id", "jBeep");
        soundElem.setAttribute("type", "audio/wav");
        soundElem.setAttribute("style", "display:none;");
        soundElem.setAttribute("data", soundFile);

        paramElem = document.createElement("param");
        paramElem.setAttribute("name", "autostart");
        paramElem.setAttribute("value", "false");

        soundElem.appendChild(paramElem);
        bodyElem.appendChild(soundElem);

        try {
            soundElem.Play();
        }
        catch (ex) {
            soundElem.object.Play();
        }

    }

}

String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};
String.prototype.startsWith = function (suffix) {
    return this.indexOf(suffix) == 0;
};
function pathCombine(dir1, dir2) {
    if (dir1.endsWith("/") && dir2.startsWith("/")) {
        return dir1 + dir2.substring(1);
    }
    else if (dir1.endsWith("/") && !dir2.startsWith("/") || !dir1.endsWith("/") && dir2.startsWith("/")) {
        return dir1 + dir2;
    }
    else if (!dir1.endsWith("/") && !dir2.startsWith("/")) {
        return dir1 + '/' + dir2;
    }
}
//String.prototype.format = function () {
//    var formatted = this;
//    for (var i = 0; i < arguments.length; i++) {
//        var regexp = new RegExp('\\{' + i + '\\}', 'gi');
//        formatted = formatted.replace(regexp, arguments[i]);
//    }
//    return formatted;
//};

String.format = function () {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i + 1]);
    }
    return s;
}


///////////////////////////////////
function InputOnlyNumeric(event) {
    // Allow special chars + arrows 
    if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9
        || event.keyCode == 27 || event.keyCode == 13 || event.keyCode == 188 || event.keyCode == 190
        || (event.keyCode == 65 && event.ctrlKey === true)
        || (event.keyCode >= 35 && event.keyCode <= 39)) {
        return;
    } else {
        // If it's not a number stop the keypress
        if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
            event.preventDefault();
        }
    }
}

$(document).ready(function ($) {
    // tmcuong add - Only input number
    // with input type = number
    $('input[type=number]').keydown(function (event) {
        InputOnlyNumeric(event);
    });
});


// Clone object
var clone = function (o) {
    var n = {};
    for (i in o)
        n[i] = (typeof o[i] == 'object') ? arguments.callee(o[i]) : o[i];
    return n;
};

function NavigatePaging(currentPage, pageCount) {
    var iFrom = currentPage - 4;
    if (iFrom <= 0) iFrom = 1;
    var sPaging = '<div class="row"><div class="twelve columns paging-box">';
    if (currentPage == 1)
        sPaging += '<a class="page-panel first-panel" onclick="return false;">&lt;&lt;&nbsp;</a> <a class="page-panel" onclick="return false;">&lt;&nbsp;</a>';
    else
        sPaging += '<a class="page-panel first-panel" href="javascript:void(0);" onclick="gotoPage(1);">&lt;&lt;&nbsp;</a> <a class="page-panel" href="javascript:void(0);" onclick="gotoPage(' + (currentPage - 1).toString() + ');">&lt;&nbsp;</a>';
    for (var i = iFrom; i <= iFrom + 8; i++) {
        if (i <= pageCount) {
            // Nếu i không phải trang đầu tiên thì thêm ... ở đầu
            if (i == iFrom && i > 1) {
                sPaging += '<a class="page-panel" ><span>...</span>    </a>'
            }
            var href = (i == currentPage) ? "" : "href='javascript:void(0);'";
            var clsCSS = (i == currentPage) ? "page-panel current" : "page-panel";
            var funcJs = (i == currentPage) ? "return false;" : "gotoPage(" + i + ");";
            sPaging += '<a id="page_' + i + '" class="' + clsCSS + '" ' + href + ' onclick="' + funcJs + '"><span>' + i.toString() + '</span></a>';
            if (i == iFrom + 8 && iFrom + 8 < pageCount) {
                sPaging += ' <a class="page-panel" ><span>...</span>    </a>';
            }
        }
    }
    if (currentPage < pageCount) {
        sPaging += '<a class="page-panel" href="javascript:void(0);" onclick="gotoPage(' + (currentPage + 1).toString() + ');">&nbsp;&gt;</a>';
        sPaging += '<a class="page-panel last-panel" href="javascript:void(0);" onclick="gotoPage(' + pageCount + ');">&nbsp;&gt;&gt;</a>';
    }
    else {
        sPaging += '<a class="page-panel" onclick="return false;">&nbsp;&gt;</a>';
        sPaging += '<a class="page-panel last-panel" onclick="return false;">&nbsp;&gt;&gt;</a>';
    }
    sPaging += '</div></div>';
    return sPaging;
}

/*
 Hiển thị thông báo lỗi của control validate summary
*/
function showValidateSumary(arrValidateSummaryClientID, dialogWidth) {
    if (arrValidateSummaryClientID) {
        var sMessage = '';
        for (var i = 0; i < arrValidateSummaryClientID.length; i++) {
            sMessage += $('#' + arrValidateSummaryClientID[i]).html();
        }
        ShowDialog_Alert(sMessage, _contDialogIconType.Warning, dialogWidth);
    } else {
        ShowDialog_Alert("arrValidateSummaryClientID: " + arrValidateSummaryClientID + " không phù hợp!", _contDialogIconType.Error);
    }
}

/*
 Sinh width cho dialog
*/
function getDialogWidth(dialogWidth) {
    var iWidth = _contDialogFormat.Width;
    if (dialogWidth && dialogWidth > 0) {
        iWidth = dialogWidth;
    }
    return iWidth;
}

/*
Kiểm tra object
*/
function checkUndefined(obj) {
    return typeof (obj) == 'undefined';
}

/*
Gọi thực thi 1 function truyền vào
*/
function callClientFunction(_function) {
    if (!checkUndefined(_function)) {
        var func = _function;
        func();
    }
}

/*
Gọi ajax xuống server
*/
function callServerFunction(commandName) {
    if (!checkUndefined(commandName) && !commandName.isNullOrEmpty() && !commandName.isLastTrimEmpty()) {
        if (!checkUndefined(callAjaxRequest)) {
            callAjaxRequest(commandName);
        }
    }
}

function cancelEvent(args) {
    if (args && !checkUndefined(args)) {
        args.set_cancel(true);
    }
}

/*
tính position của dialog so với hiển thị của form
*/
function getTopOffset() {
    return (parent.document.documentElement.clientHeight) / 2 + parent.window.pageYOffset - 200;
}

/*
Di chuyển dialog ra giữa màn hình đang thao tác
*/
function getDialogPosition(isCheckChange) {
    if (isCheckChange && isCheckChange == true)
        return "center"
    else
        return ["top", getTopOffset()]; //"center",
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/*
Định nghĩa lại hàm trim do lỗi trên IE
*/
String.prototype.trim = function () {
    return this.replace(/(?:(?:^|\n)\s+|\s+(?:$|\n))/g, '').replace(/\s+/g, ' ');
};

/*
Định nghĩa lại hàm string format
*/
String.prototype.format = function () {
    var formatted = this;
    for (var i = 0; i < arguments.length; i++) {
        var regexp = new RegExp('\\{' + i + '\\}', 'gi');
        formatted = formatted.replace(regexp, arguments[i]);
    }
    return formatted;
};

/*
Kiểm tra string empty
*/
String.prototype.isEmpty = function () {
    return this.length == 0;
};

/*
Kiểm tra string null Or empty
*/
String.prototype.isNullOrEmpty = function () {
    return !this;
};

/*
Kiểm tra string empty sau khi trim 
*/
String.prototype.isLastTrimEmpty = function () {
    return this.trim().length == 0;
};

var amount = '';
var tax = '';

function NumericDotOnly(inputObj, e) {
    var isAmount = inputObj.id == "amountVal";
    var e = (!e) ? window.event : e;
    var key = e.keyCode;
    if ((key < 48 || key > 57) && key != 8 && key != 110 && key != 190 || ((key == 110 || key == 190) && ((isAmount) ? amount : tax).indexOf('.') != -1)) {
        inputObj.value = (isAmount) ? amount : tax;
    }

    if (isAmount) {
        amount = inputObj.value;
    }
    else {
        tax = inputObj.value;
    }
}

function formatCurrency(inputObj) {
    if (inputObj.id == "amountVal") {
        amount = convertToFloat(inputObj.value);
    }
    else {
        tax = convertToFloat(inputObj.value);
    }
    inputObj.value = (inputObj.value != '') ? addCommas(convertToFloat(inputObj.value)) : '';
}

function convertToFloat(num) {
    return (num != '') ? parseFloat(num.replace(/,/g, "")).toFixed(2) : num;
}

function addCommas(num) {
    var numParts = num.split('.');
    var numArr = numParts[0].split('').reverse();
    var newArr = [];
    var count = -1;
    for (var i = 0; i < numArr.length; i++) {
        if (i % 3 == 0) {
            newArr[++count] = ',';
        }
        newArr[++count] = numArr[i];
    }
    return newArr.reverse().join('').replace(/((.+?)(,?))$/, '$2') + '.' + numParts[1];
}

function focusCampo(id) {
    var inputField = document.getElementById(id);
    if (inputField != null && inputField.value.length != 0) {
        if (inputField.createTextRange) {
            var fieldRange = inputField.createTextRange();
            fieldRange.moveStart('character', inputField.value.length);
            fieldRange.collapse();
            fieldRange.select();
        } else if (inputField.selectionStart || inputField.selectionStart == '0') {
            var elemLen = inputField.value.length;
            inputField.selectionStart = elemLen;
            inputField.selectionEnd = elemLen;
            inputField.focus();
        }
    } else {
        inputField.focus();
    }
}

function isDate(txtDate) {
    var currVal = txtDate;
    if (currVal == '')
        return false;

    //Declare Regex  
    var rxDatePattern = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/;
    var dtArray = currVal.match(rxDatePattern); // is format OK?

    if (dtArray == null)
        return false;

    //Checks for mm/dd/yyyy format.
    dtMonth = dtArray[3];
    dtDay = dtArray[1];
    dtYear = dtArray[5];

    if (dtMonth < 1 || dtMonth > 12)
        return false;
    else if (dtDay < 1 || dtDay > 31)
        return false;
    else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
        return false;
    else if (dtMonth == 2) {
        var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
        if (dtDay > 29 || (dtDay == 29 && !isleap))
            return false;
    }
    return true;
}

function today() {
    var fullDate = new Date();
    var currentDate = parserDate(fullDate);
    return currentDate;
}

function parserDate(fullDate) {
    var twoDigitMonth = ((fullDate.getMonth().length + 1) === 1) ? (fullDate.getMonth() + 1) : '0' + (fullDate.getMonth() + 1);
    var date = twoDigitMonth + "/" + fullDate.getDate() + "/" + fullDate.getFullYear();
    return date;
}

function string2Date(strDate) {
    var arr = strDate.split("/");
    var date = new Date(arr[2], arr[1] - 1, arr[0]);
    return parserDate(date);
}

function formatMoney(totalValue) {
    if (totalValue == 0) return 0;
    var s = totalValue.toString().split("").reverse().reduce(function (acc, num, i, orig) {
        return num + (i && !(i % 3) ? "." : "") + acc;
    }, "");
    while (s.indexOf("-.") > -1) {
        s = s.replace("-.", "-");
    }
    return s;
}

function replaceMoney(strMoney) {
    if (strMoney == 0)
        return "0";
    else
        return strMoney.replace(/\./g, '');
}
function isValidEmail(email) {
    var pattern = new RegExp(/^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$/);
    return pattern.test(email);
}

function popupwindow(url, title, w, h) {
    var left = (screen.width / 2) - (w / 2);
    var top = (screen.height / 2) - (h / 2);
    return window.open(url, title, 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
}

