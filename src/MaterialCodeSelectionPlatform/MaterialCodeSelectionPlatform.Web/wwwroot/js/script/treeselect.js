//treeSelect组件
layui.define(['layer', 'tree'], function (exports) {
    "use strict";

    var _MOD = 'treeselect',
        layer = layui.layer,
        tree = layui.tree,
        $ = layui.jquery,
        hint = layui.hint(),
        THIS = 'layui-this',
        SHOW = 'layui-show',
        HIDE = 'layui-hide',
        DISABLED = 'layui-disabled',
        dom = $(document),
        win = $(window);
    var TreeSelect = function (options) {
        this.options = options;
        this.v = '1.0.0';
    };
    /**
    * 初始化下拉树选择框
    */
    TreeSelect.prototype.init = function (elem, options) {
        var that = this,
            TIPS = '请选择',
            CLASS = 'layui-form-select',
            TITLE = 'layui-select-title',
            NONE = 'layui-select-none',
            initValue = '',
            thatInput,
            treeblock = 'tree-select-empty',
            treenonle = 'tree-select-none',
            times = new Date().getTime(),
            treeid = "selecttree_" + times,
            hide = function (e, clear) {
                if (!$(e.target).parent().hasClass(TITLE) || clear) {
                    $('.' + CLASS).removeClass(CLASS + 'ed ' + CLASS + 'up');
                    thatInput && initValue && thatInput.val(initValue);
                }
                thatInput = null;
            },           
            filter = function filter(inputName, dom) {
                return $(dom).children('a').children('cite').text().indexOf(inputName) == -1;
            },
            initTree = function (li, inputName) {
                if (inputName == "") {
                    for (var i = 0; i < li.length; i++) {
                        var _item = $(li[i]);
                        var ul = _item.find('ul');// 所有ul 根据ul找他图标按钮自动收缩
                        if (ul.length > 0) {
                            for (var j = 0; j < ul.length; j++) {//不选时自动收缩                          
                                if ($(ul[j]).hasClass('layui-show')) {//如果没条件 树是展开的就收起
                                    $($(ul[j])[0]).prev().prev().click(); 
                                }
                            }
                        }                     

                    }
                }
                else {
                    for (var i = 0; i < li.length; i++) {
                        var _item = $(li[i]);
                        var ul = _item.children('ul');// 是否有子节点
                        if (ul.length > 0) {
                            var cite = ul.children('li').find('cite'); //获取所有下级的文本值
                            var isExists = false;
                            for (var j = 0; j < cite.length; j++) { //下级树存在搜索条件
                                if ($(cite[j]).text().indexOf(inputName) != -1) {
                                    isExists = true;
                                    if (!$($(cite[j]).parents('ul')[0]).hasClass('layui-show')) {
                                        $($(cite[j]).parents('ul')[0]).prev().prev().click();//模拟点击事件展开树
                                    }

                                }
                            }
                            if (!isExists) { //不存在子节点
                                if (filter(inputName, _item)) {// 不匹配搜索条件
                                    _item.addClass('layui-hide');
                                }
                            }
                        }
                        else { //没有子节点
                            if (filter(inputName, _item)) {// 不匹配搜索条件
                                _item.addClass('layui-hide');
                            }
                        }
                    }
                }
             
            },
            events = function (reElem, disabled) {
                var select = $(this),
                    title = reElem.find('.' + TITLE),
                    input = title.find('input'),
                    tree = reElem.find('.layui-tree'),
                    defaultVal = elem.val();
                //
                //$.each(data.options.data,
                //    function(i, o) {
                //        if (o.id === defaultVal) {
                //            input.val(o.name);
                //        }
                //    });

                if (disabled) return;

                //展开下拉
                var showDown = function () {
                    var top = reElem.offset().top + reElem.outerHeight() + 5 - win.scrollTop(),
                        dlHeight = tree.outerHeight();
                    reElem.addClass(CLASS + 'ed');

                    //上下定位识别
                    if (top + dlHeight > win.height() && top >= dlHeight) {
                        reElem.addClass(CLASS + 'up');
                    }
                    var li = $(".tree-select-tips").nextAll();//所有父节点
                    reElem.find('li').removeClass('layui-hide');               
                    initTree(li, "");
                },
                    hideDown = function () {
                        reElem.removeClass(CLASS + 'ed ' + CLASS + 'up');
                        input.blur();
                    };

                //点击标题区域
                title.on('click',
                    function (e) {
                        debugger;
                        reElem.hasClass(CLASS + 'ed')
                            ? (
                                hideDown()
                            )
                            : (
                                hide(e, true),
                                showDown()                                
                            );
                        tree.find('.' + NONE).remove();
                    });

                //点击箭头获取焦点
                title.find('.layui-edge').on('click',
                    function () {
                        input.focus();
                    });
                title.on('input propertychange', function (e) {
                    var input = e.target,
                        inputName = $.trim(input.value);
                    if (inputName == "") elem.val("");//清空value值

                    var li = $(".tree-select-tips").nextAll();//所有父节点
                    reElem.find('li').removeClass('layui-hide');                   
                    initTree(li, inputName); //模糊查询               
                   
                    var shows = li.not('.layui-hide');
                    if (!shows.length) {
                        reElem.find('dd.' + treenonle).addClass(treeblock).text('无匹配项');
                    }
                    else {
                        reElem.find('dd.' + treenonle).removeClass(treeblock);
                    }
                })
                //键盘事件
                input.on('keyup',
                    function (e) {
                        var keyCode = e.keyCode;
                        //Tab键
                        if (keyCode === 9) {
                            showDown();
                        }
                    })
                    .on('keydown',
                        function (e) {
                            var keyCode = e.keyCode;
                            //Tab键
                            if (keyCode === 9) {
                                hideDown();
                            } else if (keyCode === 13) { //回车键
                                e.preventDefault();
                            }
                        });
                //渲染tree
                layui.tree({
                    elem: "#" + treeid,
                    click: function (obj) {

                        elem.val(obj.id).removeClass('layui-form-danger');
                        input.val(obj.name);
                        hideDown(true);
                        return false;
                    },
                    nodes: that.options.data
                });
                //点击树箭头不隐藏
                tree.find(".layui-tree-spread").on('click', function () {
                    return false;
                });
                //关闭下拉
                $(document).off('click', hide).on('click', hide);
            },
            defaultVal = function (reElem, defaultName, defaultValue) { //设置默认值
                var title = reElem.find('.' + TITLE)
                var input = title.find('input');
                elem.val(defaultValue);
                input.val(defaultName);
            }
        var othis = $(elem),
            hasRender = othis.next('.' + CLASS),
            disabled = that.disabled,
            value = othis.value ? othis.value : othis.val(),
            placeholder = othis.attr("placeholder") ? othis.attr("placeholder") : TIPS;
        if (typeof othis.attr('lay-ignore') === 'string') return othis.show();
        //隐藏原控件
        othis.hide();
        //替代元素
        //var reElem = $([
        //    '<div class="layui-unselect ' + CLASS + (disabled ? ' layui-select-disabled' : '') + '">',
        //    '<div class="' +
        //    TITLE +
        //    '"><input type="text" placeholder="' +
        //    placeholder +
        //    '" value="' +
        //    (value ? value : '') +
        //    '" readonly class="layui-input layui-input-small layui-unselect' +
        //    (disabled ? (' ' + DISABLED) : '') +
        //    '">', '<i class="layui-edge"></i></div>', '<ul id="' + treeid + '" class="layui-anim layui-anim-upbit layui-tree"></ul>', '</div>'
        //].join(''));
        //替代元素
        var reElem = $([
            '<div class="layui-unselect ' + CLASS + (disabled ? ' layui-select-disabled' : '') + '">',
            '<div class="' +
            TITLE +
            '"><input type="text" placeholder="' +
            placeholder +
            '" value="' +
            (value ? value : '') +
            '" class="layui-input layui-input-small layui-unselect' +
            (disabled ? (' ' + DISABLED) : '') +
            '">', '<i class="layui-edge"></i></div>', '<ul id="' + treeid + '" class="layui-anim layui-anim-upbit layui-tree"><dd class="tree-select-tips tree-select-none"></dd></ul>', '</div>'
        ].join(''));
        hasRender[0] && hasRender.remove(); //如果已经渲染，则Rerender
        othis.after(reElem);
        events.call(this, reElem, disabled);
        if (options.defaultName && options.defaultValue) { //如果有默认值就赋值默认值
            defaultVal.call(this, reElem, options.defaultName, options.defaultValue);
        }

    };

    /**
     * 判断是否json
     * @param {any} obj
     */
    function isJson(obj) {
        return typeof (obj) == "object" && Object.prototype.toString.call(obj).toLowerCase() == "[object object]" && !obj.length;
    }
    // 导出组件
    //exports(_MOD, new TreeSelect());
    //暴露接口
    exports(_MOD, function (options) {
        var treeSelect = new TreeSelect(options = options || {});
        var elem = $(options.elem);
        if (!elem[0])
            return hint.error('layui.treeSelect 没有找到' + options.elem + '元素');
        if (!options.data)
            return hint.error('layui.treeSelect 缺少参数 data ,data 可直接指定treedata，也可以是treedata数据url，treedata参见layui tree模块');
        if (isJson(options.data))
            treeSelect.init(elem);
        else {
            $.ajax({
                url: options.data,
                dataType: "json",
                type: !options.method ? "POST" : options.method,
                success: function (d) {
                    options.data = d;
                    treeSelect.init(elem, options);
                }
            });
        }
    });
});