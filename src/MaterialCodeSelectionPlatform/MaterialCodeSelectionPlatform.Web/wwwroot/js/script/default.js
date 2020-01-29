function closeiframe(msg) {
    layer.closeAll('iframe');
    if (msg) {
        layer.msg(msg);
    }
}
//可将表单序列化为Json对象
$.fn.serializeJson = function () {
    var jsonObject = {};
    var array = this.serializeArray();
    for (var i = 0; i < array.length; i++) {
        var obj = array[i];
        jsonObject[obj.name] = trim(obj.value);
    }
    return jsonObject;
}

function trim(val) {
    if (val) {
        return val.replace(/(^\s*)|(\s*$)/g, "");
    } else {
        return val;
    }
}

function dealResult(result) {
    if (result.Success) {
        return true;
    } else {
        layer.msg(result.Message);
        return false;
    }
}

function convertJsonDateTime(jsonDate) {//json日期格式转换为正常格式
    try {
        var date = new Date(parseInt(jsonDate.replace("/Date(", "").replace(")/", ""), 10));
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var seconds = date.getSeconds();
        var milliseconds = date.getMilliseconds();

        return date.getFullYear() + "-" + month + "-" + day + " " + (hours < 10 ? '0' + hours : hours) + ":" + (minutes < 10 ? '0' + minutes : minutes) + ":" + (seconds < 10 ? '0' + seconds : seconds);



    } catch (ex) {
        return "";
    }
}

function converDateTimetoStr(date) {
    try {
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var seconds = date.getSeconds();
        var milliseconds = date.getMilliseconds();

        return date.getFullYear() + "-" + month + "-" + day;



    } catch (ex) {
        return "";
    }
}

//Json时间格式转换
function convertJsonData(jsonDate) {//json日期格式转换为正常格式
    try {
        var date = new Date(parseInt(jsonDate.replace("/Date(", "").replace(")/", ""), 10));
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        return date.getFullYear() + "-" + month + "-" + day;
    } catch (ex) {
        return "";
    }
}

//组装查询列表条件
function getSearchCondition(form) {
    var items = $("#" + form).find("[name]");
    var result = "?";
    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        var $item = $(items[i]);
        var name = $item.attr("name");
        var tagName = items[i].tagName;
        var value = '';
        switch (tagName) {
            case "INPUT":
                var type = $item.attr("type");
                switch (type) {
                    case "text":
                    case "hidden":
                        value = $item.val();
                        break;
                    case "checkbox":
                        if (item.checked) {
                            value = true;
                        } else {
                            value = false;
                        }
                        break;
                }
                break;
            case "SELECT":
                value = $item.val();
                break;
            default:
                break;
        }

        if (i == items.length - 1) {
            result += name + "=" + encodeURIComponent(value);
        } else {
            result += name + "=" + encodeURIComponent(value) + "&";
        }
    }
    return result == "?" ? "" : result;
}
var cookie = {  
    set: function (name, value, time) {
        var _this = this
        var strsec = _this.getsec(time)
        var exp = new Date();
        exp.setTime(exp.getTime() + strsec * 1);
        document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
    },
    get: function (name) {
        var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
        if (arr = document.cookie.match(reg))
            return unescape(arr[2]);
        else
            return null;
    },
    //s20是代表20秒 
    //h是指小时，如12小时则是：h12 
    //d是天数，30天则：d30 
    getsec: function (str) {
        var str1 = str.substring(1, str.length) * 1;
        var str2 = str.substring(0, 1);
        if (str2 == "s") {
            return str1 * 1000;
        }
        else if (str2 == "h") {
            return str1 * 60 * 60 * 1000;
        }
        else if (str2 == "d") {
            return str1 * 24 * 60 * 60 * 1000;
        }
    }
}

var HtmlUtil = {
    /*用浏览器内部转换器实现html转码*/
    htmlEncode: function (html) {
        var temp = document.createElement("div");
        //2.然后将要转换的字符串设置为这个元素的innerText(ie支持)或者textContent(火狐，google支持)
        (temp.textContent != undefined) ? (temp.textContent = html) : (temp.innerText = html);
        var output = temp.innerHTML;
        temp = null;
        return output;
    },
    htmlDecode: function (text) {
        var temp = document.createElement("div");
        //2.然后将要转换的字符串设置为这个元素的innerHTML(ie，火狐，google都支持)
        temp.innerHTML = text;
        var output = temp.innerText || temp.textContent;
        temp = null;
        return output;
    },
    /*3.用正则表达式实现html转码*/
    htmlEncodeByRegExp: function (str) {
        var s = "";
        if (str.length == 0) return "";
        s = str.replace(/&/g, "&");
        s = s.replace(/</g, "<");
        s = s.replace(/>/g, ">");
        s = s.replace(/ /g, " ");
        s = s.replace(/\'/g, "'");
        s = s.replace(/\"/g, "\"");
        return s;
    },
    /*4.用正则表达式实现html解码*/
    htmlDecodeByRegExp: function (str) {
        var s = "";
        if (str.length == 0) return "";
        s = str.replace(/&/g, "&");
        s = s.replace(/</g, "<");
        s = s.replace(/>/g, ">");
        s = s.replace(/ /g, " ");
        s = s.replace(/'/g, "\'");
        s = s.replace(/"/g, "\"");
        return s;
    }
};