(function ($) {
    $.fn.dropdownlist = function dropdownlist(options) {
        var $that = $(this);
        var settings = $.extend({
            url: null,
            type: 'Get',
            async: true,
            dataType: 'json',
            success: function (data, textStatus, jqXHR) {
                successCallback($that, data, textStatus, jqXHR, settings);
            },
            appendHeader: false,
            jsonData: null,
            value: "Value",
            text: "Text",
            defaultValue: '',
            error: function (error,teststate) {           
                debugger;
            },
            complete:function() {

            }
        }, options);

        if (settings.appendHeader) {
            if (settings.appendHeader == true || settings.appendHeader === "true") {
                $that.html('<option value=""><option/>');
            }
        } else {
            $that.html('');
        }

        if (settings.jsonData) {
            successCallback($that, settings.jsonData, null, null, settings);
        } else {
            $.ajax(settings);
        }
    };

    function successCallback($that, data, textStatus, jqXHR, settings) {
        
        for (var i = 0; i < data.length; i++) {
            var item = data[i];
            $that.append('<option value="' + item.value + '">' + item.text+'<option/>');
        }
        if (settings.defaultValue != '' || settings.defaultValue===0) {
            $that.val(settings.defaultValue);
        }
        layui.form.render('select');


        settings.complete();
    }

    $(document).ready(function () {
        $("select[ajax-url]").each(function () {
            var url = $(this).attr("ajax-url");
            var header = $(this).attr("header");
            if (url) {
                $(this).dropdownlist({ url: url, appendHeader: header });

            }
        });

    });

}(jQuery));