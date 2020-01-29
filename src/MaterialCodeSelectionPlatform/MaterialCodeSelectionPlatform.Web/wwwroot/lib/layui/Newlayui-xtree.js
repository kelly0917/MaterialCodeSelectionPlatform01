//**********************************
//**   本插件依赖 贤心 layui form模块   
//**   由 小巷 制作 QQ：151446298    
//**   版本3.0 时间 2018-01-10 20:48 
//**********************************
//3种选着状态的树
//构造
function layuiXtree(options) {
    var _this = this;
    _this._containerid = options.elem;
    _this._container = document.getElementById(options.elem); //容器
    _this._container.style.minHeight = "100px";
    _this._options = options;
    _this.grantTypeId = "";
    _this.Loading(options);
}

//封装IE8 Class选择
layuiXtree.prototype.getByClassName = function (cn) {
    if (document.getElementsByClassName) return this._container.getElementsByClassName(cn);
    var _xlist = this._container.childNodes;
    var _xtemp = new Array();
    for (var i = 0; i < _xlist.length; i++) {
        var _xchild = _xlist[i];
        var _xclassNames = _xchild.getAttribute('class').split(' ');
        for (var j = 0; j < _xclassNames.length; j++) {
            if (_xclassNames[j] == cn) {
                _xtemp.push(_xchild);
                break;
            }
        }
    }
    return _xtemp;
}

//在一个对象下面找子级
layuiXtree.prototype.getChildByClassName = function (obj, cn) {
    var _xlist = obj.childNodes;
    var _xtemp = new Array();
    for (var i = 0; i < _xlist.length; i++) {
        var _xchild = _xlist[i];
        var _xclassNames = _xchild.getAttribute('class').split(' ');
        for (var j = 0; j < _xclassNames.length; j++) {
            if (_xclassNames[j] == cn) {
                _xtemp.push(_xchild);
                break;
            }
        }
    }
    return _xtemp;
}




//加载特效，且获取数据
layuiXtree.prototype.Loading = function (options) {
    var _this = this;
    _this.xloading = document.createElement("span"); //创建加载对象
    _this.xloading.setAttribute('class', 'layui-icon layui-anim layui-anim-rotate layui-anim-loop');
    _this.xloading.innerHTML = '&#xe63e;';
    _this.xloading.style.fontSize = "50px";
    _this.xloading.style.color = "#009688";
    _this.xloading.style.fontWeight = "bold";
    _this.xloading.style.marginLeft = _this._container.offsetWidth / 2 - 25 + 'px';
    _this.xloading.style.marginTop = _this._container.offsetHeight / 2 - 50 + 'px';
    _this._container.innerHTML = '';
    _this._container.appendChild(_this.xloading); //加载显示
    if (typeof (options.data) == 'object') {
        _this._dataJson = options.data;
        _this.Initial(options);
        return;
    }


    //如果是字符串url，进行异步加载
    var obj = new XMLHttpRequest();
    obj.open('get', options.data, true);
    obj.onreadystatechange = function () {
        if (obj.readyState == 4 && obj.status == 200 || obj.status == 304) { //回调成功
            _this._dataJson = eval('(' + obj.responseText + ')'); //将返回的数据转为json
            _this.Initial(options);
        }
    };
    obj.send();
}

//data验证后的数据初始化
layuiXtree.prototype.Initial = function (o) {
    var _this = this;
    _this._form = o.form; //layui from对象
    _this._domStr = "";  //结构字符串
    _this._isopen = o.isopen != null ? o.isopen : true;
    if (o.color == null) o.color = { open: '#2F4056', close: '#2F4056', end: '#2F4056' };//图标颜色
    _this._iconOpenColor = o.color.open != null ? o.color.open : "#2F4056";
    _this._iconCloseColor = o.color.close != null ? o.color.close : "#2F4056";
    _this._iconEndColor = o.color.end != null ? o.color.end : "#2F4056";
    if (o.icon == null) o.icon = { open: '&#xe625;', close: '&#xe623;', end: '&#xe621;' };//图标样式
    _this._iconOpen = o.icon.open != null ? o.icon.open : '&#xe625;';
    _this._iconClose = o.icon.close != null ? o.icon.close : '&#xe623;';
    _this._iconEnd = o.icon.end != null ? o.icon.end : '&#xe621;';
    _this._click = o.click != null ? o.click : function () { };
    _this._ckall = o.ckall != null ? o.ckall : false;  //全选是否启用
    _this._ckallSuccess = o.ckallback != null ? o.ckallback : function () { };//全选回调
    //_this.CreateCkAll();
    _this.dataBind(_this._dataJson);
    _this._options.finished = o.finished != null ? o.finished : function () { };
    _this.Rendering();
    _this.ckChildFromParent = o.ckChildFromParent != null ? o.ckChildFromParent : true;
    _this.ckParentFromChild = o.ckParentFromChild != null ? o.ckParentFromChild : true;
    _this.isThreeState = typeof (o.isThreeState) == "undefined" ? true : o.isThreeState;//是否三种状态（false为两种）
    _this.readonly = typeof (o.readonly) == "undefined" ? false : o.readonly;//是否只读
    //回调完成
    if (_this._options.finished) {
        _this._options.finished(_this._dataJson);
    }
}



//生产结构
layuiXtree.prototype.dataBind = function (d) {
    var _this = this;
    if (d.length > 0) {
        for (i in d) {
            var xtree_isend = '';
            var xtree_ischecked = '';
            var xtree_isdisabled = d[i].disabled ? ' disabled="disabled" ' : '';
            _this._domStr += '<div class="layui-xtree-item" parentId="' + d[i].parentId + '" id="' + d[i].value + '">';
            if (d[i].data.length > 0 || d[i].isNotEnd) {
                _this._domStr += '<i class="layui-icon layui-xtree-icon" data-xtree="' + (_this._isopen ? '1' : '0') + '">' + (_this._isopen ? _this._iconOpen : _this._iconClose) + '</i>';
                xtree_ischecked = d[i].checked ? ' checked ' : '';
            }
            else {
                _this._domStr += '<i class="layui-icon layui-xtree-icon-null">' + _this._iconEnd + '</i>';
                xtree_isend = 'data-xend="1"';
                xtree_ischecked = d[i].checked ? ' checked ' : '';
                xtree_isdisabled = d[i].disabled ? ' disabled="disabled" ' : '';
            }

            //_this._domStr += '<input type="checkbox" class="layui-xtree-checkbox" ' + xtree_isend + xtree_ischecked + xtree_isdisabled + ' value="' + d[i].value + '" title="' + d[i].title + '" lay-skin="primary" lay-filter="xtreeck' + _this._containerid + '">';
            var GrantTypeClass = "";
            var GrantType = d[i].grantType;
            if (GrantType == 0) {
                GrantTypeClass = "checkbox-add" //选中图标样式

            }
            else if (GrantType == 1) {
                GrantTypeClass = "checkbox-cut" //X图标
            }
            else {
                GrantTypeClass = "checkbox-inherit"; //不选图标样式
            }

            _this._domStr += '<label class="Clslable"><i class="icon icon-checkbox  ' + GrantTypeClass + ' layui-icon layui-xtree-icon-null" value=' + d[i].value + '  title=' + d[i].title + '></i> <span style="cursor:pointer">' + d[i].title + '</span></label>';
            _this.dataBind(d[i].data);
            _this._domStr += '</div>';

        }


    }

}

//渲染呈现
layuiXtree.prototype.Rendering = function () {
    var _this = this; //当前树对象
    _this._container.innerHTML = _this._domStr;
    _this._domStr = "";
    $(".Clslable").children().each(function () {
        $(this).bind('click', function () {
            //debugger;
            var data = this; //当前dom
            if ($(data).hasClass('icon icon-checkbox')) { //如果为选择框
                data = this;
            }
            else {
                data = $(data).prev();//替换i标签为当前对象 如果是span 获取上一个兄弟对象
            }

            if (_this.readonly == false) {//是否只读
                if ($(data).hasClass('checkbox-add') && _this.grantTypeId != $(data).attr("value")) {
                    _this.grantTypeId = $(data).attr("value");      //如果第一次进来存入ID
                }
                else if ($(data).hasClass('checkbox-add') && _this.grantTypeId == $(data).attr("value")) {  //已存入id就不是第一次进来
                    if (_this.isThreeState) {//三种状态
                        $(data).removeClass("checkbox-add")
                        $(data).addClass("checkbox-cut")
                        _this.grantTypeId = $(data).attr("value");
                    } else {//两种状态
                        $(data).removeClass("checkbox-add")
                        $(data).addClass("checkbox-inherit")
                        _this.grantTypeId = $(data).attr("value");
                    }
                }
                else if ($(data).hasClass('checkbox-inherit')) {
                    $(data).removeClass("checkbox-inherit")
                    $(data).addClass("checkbox-add")
                    _this.grantTypeId = $(data).attr("value");
                }
                else {
                    $(data).removeClass("checkbox-cut")
                    $(data).addClass("checkbox-inherit")
                    _this.grantTypeId = $(data).attr("value");
                }
            }

            //选中后方法回调
            _this._click(data);
        })
    })




    var xtree_items = _this.getByClassName('layui-xtree-item');
    var xtree_icons = _this.getByClassName('layui-xtree-icon');
    var xtree_nullicons = _this.getByClassName('layui-xtree-icon-null');

    for (var i = 0; i < xtree_items.length; i++) {
        if (xtree_items[i].parentNode == _this._container)
            xtree_items[i].style.margin = '5px 0 0 10px';
        else {
            xtree_items[i].style.margin = '5px 0 0 45px';
            if (!_this._isopen) xtree_items[i].style.display = 'none';
        }
    }

    for (var i = 0; i < xtree_icons.length; i++) {
        xtree_icons[i].style.position = "relative";
        xtree_icons[i].style.top = "3px";
        xtree_icons[i].style.margin = "0 5px 0 0";
        xtree_icons[i].style.fontSize = "18px";
        xtree_icons[i].style.color = _this._isopen ? _this._iconOpenColor : _this._iconCloseColor;
        xtree_icons[i].style.cursor = "pointer";

        xtree_icons[i].onclick = function () {
            var xtree_chi = this.parentNode.childNodes;
            if (this.getAttribute('data-xtree') == 1) {
                for (var j = 0; j < xtree_chi.length; j++) {
                    if (xtree_chi[j].getAttribute('class') == 'layui-xtree-item')
                        xtree_chi[j].style.display = 'none';
                }
                this.setAttribute('data-xtree', '0')
                this.innerHTML = _this._iconClose;
                this.style.color = _this._iconCloseColor;
            } else {
                for (var j = 0; j < xtree_chi.length; j++) {
                    if (xtree_chi[j].getAttribute('class') == 'layui-xtree-item')
                        xtree_chi[j].style.display = 'block';
                }
                this.setAttribute('data-xtree', '1')
                this.innerHTML = _this._iconOpen;
                this.style.color = _this._iconOpenColor;
            }
        }
    }

    for (var i = 0; i < xtree_nullicons.length; i++) {
        xtree_nullicons[i].style.position = "relative";
        xtree_nullicons[i].style.top = "3px";
        xtree_nullicons[i].style.margin = "0 5px 0 0";
        xtree_nullicons[i].style.fontSize = "18px";
        xtree_nullicons[i].style.color = _this._iconEndColor;
    }


    var _xtree_disableds = _this.getByClassName('layui-disabled');
    for (var i = 0; i < _xtree_disableds.length; i++) {
        _xtree_disableds[i].getElementsByTagName('span')[0].style.color = "#B5B5B5";
    }

}

//更新渲染
layuiXtree.prototype.render = function () {
    var _this = this;
    _this.Loading(_this._options);
}




//获取全部选中的末级checkbox对象
layuiXtree.prototype.GetChecked = function () {
    var _this = this;
    var arr = new Array();
    var arrIndex = 0;
    var cks = _this.getByClassName('layui-xtree-checkbox');
    for (var i = 0; i < cks.length; i++) {
        if (cks[i].checked && cks[i].getAttribute('data-xend') == '1') {
            arr[arrIndex] = cks[i]; arrIndex++;
        }
    }
    return arr;
}

//获取全部的原始checkbox对象
layuiXtree.prototype.GetAllCheckBox = function () {
    var _this = this;
    var arr = new Array();
    var arrIndex = 0;
    var cks = _this.getByClassName('layui-xtree-checkbox');
    for (var i = 0; i < cks.length; i++) {
        arr[arrIndex] = cks[i]; arrIndex++;
    }
    return arr;
}

//根据值来获取其父级的checkbox原dom对象
layuiXtree.prototype.GetParent = function (a) {
    var _this = this;
    var cks = _this.getByClassName('layui-xtree-checkbox');
    for (var i = 0; i < cks.length; i++) {
        if (cks[i].value == a) {
            if (cks[i].parentNode.parentNode.getAttribute('id') == _this._container.getAttribute('id')) return null;
            return _this.getChildByClassName(cks[i].parentNode.parentNode, 'layui-xtree-checkbox')[0];
        }
    }
    return null;
}
