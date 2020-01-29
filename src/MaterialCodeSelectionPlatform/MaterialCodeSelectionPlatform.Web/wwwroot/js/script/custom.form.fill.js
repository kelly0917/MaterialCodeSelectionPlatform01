(function ($) {
    $.fn.fillData = function (data, reander) {
        var $form = $(this);
        for (var i in data) {
            var $item = $form.find('[name="' + i + '"]');
            if ($item.length == 0)
                continue;
            var tagName = $item[0].tagName;
            var value = data[i];
            if ($item.hasClass("date-time")) {
                value = convertJsonDateTime(value);
            } else if ($item.hasClass("date")) {
                value = convertJsonData(value);
            }

            switch (tagName) {
                case "TD":
                    $item.text(value == null ? "" : value);
                    reanderCallBack($item, value, i, data, tagName);
                    break;
                case "TEXTAREA":
                    value = HtmlUtil.htmlDecode(value);
                    $item.val(value);
                    reanderCallBack($item, value, i, data, tagName);
                    break;
                case "INPUT":
                    try {   
                        var type = $item.attr("type");
                        switch (type) {
                            case "text":
                                value = HtmlUtil.htmlDecode(value);

                                //日期类型，要转换
                                if ($item.attr("onfocus").indexOf("yyyy-MM-dd HH:mm:ss") > -1) {
                                    if (value.substring(0, 5) == "/Date") {
                                        value = convertJsonDateTime(value);
                                    }
                                }
                                else if ($item.attr("onfocus").indexOf("yyyy-MM-dd") > -1) {
                                    //日期类型，要转换
                                    if (value.substring(0, 5) == "/Date") {
                                        value = convertJsonData(value);
                                    }
                                }
                                
                                break;
                            case "checkbox":
                                if (value == true) {
                                    //$item = $form.find('[name="' + i + '"][value="' + value + '"]');
                                    $item.attr("checked", "checked");
                                    $item.val("true");
                                    layui.form.render('checkbox');
                                } else {
                                    $item.remove("checked");
                                }
                            case "radio":
                                if (value == true) {
                                    //单选框自动选中 根据name和数据库value 匹配页面的radio dom
                                    $item = $form.find('[name="' + i + '"][value="' + value + '"]');
                                    $item.attr("checked", "checked");
                                    layui.form.render('radio');
                                } else {
                                    $item.remove("checked");
                                }   
                                break;
                            default:
                        }



                    } catch (e) {

                    }

                    $item.val(value);
                    reanderCallBack($item, value, i, data, tagName);
                    break;
                case "SELECT":
                    $item.val(value);
                    layui.form.render('select');
                    reanderCallBack($item, value, i, data, tagName);
                    break;
                case "LABEL":
                    $item.text(value);
                    reanderCallBack($item, value, i, data, tagName);
                    break;
                case "SPAN":
                    $item.text(value);
                    reanderCallBack($item, value, i, data, tagName);
                    break;
                default:
                    $item.val(value);
                    reanderCallBack($item, value, i, data, tagName);
                    break;
            }
        }
    };

    function reanderCallBack($item, value, i, data, tagName) {
        if (typeof (reander) == "function") {
            reander.apply($form, $item, value, i, data);
        }
    }

    $(function () {
    });

}(jQuery));