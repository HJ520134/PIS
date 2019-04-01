var Class = function () {
    var klass = function () {
        this.init.apply(this, arguments);
    };

    klass.prototype.init = function () { };

    klass.fn = klass.prototype;

    klass.extend = function (obj) {
        var extended = obj.extended;
        for (var i in obj) {
            klass[i] = obj[i];
        };
        if (extended) extended(klass);
    };

    klass.include = function (obj) {
        var included = obj.included;
        for (var i in obj) {
            klass.fn[i] = obj[i];
        };
        klass.self = obj;
        if (included) included(klass);
    };

    return klass;
};

//localURL ajax使用的 area 前段的網址
var _LocalhostURL = "http://" + window.location.host;


// 顯示讀取遮罩
function ShowProgressBar() {
    displayProgress();
    displayMaskFrame();
}
// 隱藏讀取遮罩
function HideProgressBar() {
    var progress = $('#divProgress');
    var maskFrame = $("#divMaskFrame");
    progress.hide();
    maskFrame.hide();
}
// 顯示讀取畫面
function displayProgress() {
    var w = $(document).width();
    var h = $(window).height();
    var progress = $('#divProgress');
    progress.css({ "z-index": 999999, "top": (h / 2) - (progress.height() / 2), "left": (w / 2) - (progress.width() / 2) });
    progress.show();
}
// 顯示遮罩畫面
function displayMaskFrame() {
    var w = $(window).width();
    var h = $(document).height();
    var maskFrame = $("#divMaskFrame");
    maskFrame.css({ "z-index": 999998, "opacity": 0.7, "width": w, "height": h });
    maskFrame.show();
}

///取得QueryString參數
function GetQueryString(id) {
    var href = window.location.href.split('?');

    if (href.length > 1) {

        var params = href[1].split('&');
        var stringArry = [];
        $.each(params, function (i) {
            var id = String(params[i]).split('=')[0];
            var value = String(params[i]).split('=')[1];
            stringArry.push({ Id: id, Value: value });
        });
        var result = $.grep(stringArry, function (e) {
            if (e.Id == id) {
                return e;
            }
        });
        return result[0].Value;
    }
    return null;
}

//圖片放大
function ImgEnlarge(id) {
    // 先取得先關區塊及圖片的寬高
    // 並設定每張圖片的邊距、縮放倍數及動畫速度
    var $block = $('#' + id),
        $li = $block.find('li'),
        $img = $li.find('img'),
        _width = $img.width(),
        _height = $img.height(),
        _margin = 10,
        _ratio = 10,
        _speed = 400;

    // 把每一個 li 橫向排列好
    $li.each(function (i) {
        var $this = $(this),
            _left = i * (_width + _margin);

        // 先把排列後的位置記錄在 .data('position') 中
        $this.css('left', _left).data('position', {
            left: _left,
            top: parseInt($this.css('top'), 10) || 0
        });
    }).hover(function () {	// 當滑鼠移入 $li 時
        var $this = $(this),
            positionData = $this.data('position');

        // 改變 z-index 以免被遮到, 並移動 left 及 top
        // 同時找到 img 縮放寬高為原來的 _ratio 倍
        $this.css('z-index', 1).stop().animate({
            left: positionData.left - (_width * _ratio - _width) / 2,
            top: positionData.top - (_height * _ratio - _height) / 2
        }, _speed).find('img').stop().animate({
            width: _width * _ratio,
            height: _height * _ratio
        }, _speed);
    }, function () {	// 當滑鼠移出 $li 時
        var $this = $(this),
            positionData = $this.data('position');

        // 還原 z-index 並移回原來的 left 及 top
        // 同時找到 img 還原寬高
        $this.css('z-index', 0).stop().animate({
            left: positionData.left,
            top: positionData.top
        }, _speed).find('img').stop().animate({
            width: _width,
            height: _height
        }, _speed);
    });
}

//new Guid
function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
          .toString(16)
          .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
      s4() + '-' + s4() + s4() + s4();
}

///下拉式選單繫結
function BindDDL(url, jsonData, ddlObj, textField, valueField, selectedText, IsDisplaydefaultText, defaultText) {
    if (!ddlObj) return null;
    else
        return $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json; charset=utf-8",
            data: jsonData,
            dataType: "json",
            async: false,
            success: function (result) {
                //清空內容
                ddlObj.find('option').remove().end();
                if (IsDisplaydefaultText) {
                    //預設文字
                    ddlObj.append('<option style="display: none;" value="">' + defaultText + '</option>');
                }
                $.each(result, function () {                    
                    var text = $(this)[0][textField];
                    var value = $(this)[0][valueField];
                    ddlObj.append('<option value="' + value + '">' + text + '</option>');
                    //塞值
                    if (text === selectedText)
                        ddlObj.val(value);
                });
            },
        });
}

//轉型成float
function formatFloat(num, pos) {
    var size = Math.pow(10, pos);
    return Math.round(num * size) / size;
}

//限制只能輸入整數
function checkInteger(obj) {
    var code = event.keyCode;
    if (!/^\d+$/.test(obj.val())) {
        obj.val(/^\d+/.exec(obj.val()));
    }
}

//限制只能輸入數字
function checkfloat(e, pnumber) {
    if (!/^\d+[.]?\d*$/.test(pnumber)) {
        $(e).val(/^\d+[.]?\d*/.exec($(e).val()));
    }
    return false;
}

//判斷浮點數
function checkPercent(obj) {
    var code = event.keyCode;

    if (!/^\d+$/.test(obj.val()))
        obj.val(/^\d*\.\d*$/.exec(obj.val()));

    if (parseInt(obj.val()) >= 100) {
        obj.val(100);
    }

    if (parseInt(obj.val()) < 0.0009)
        obj.val(obj.val().charAt(0) + obj.val().charAt(1) + obj.val().charAt(2) + obj.val().charAt(3) + obj.val().charAt(4) + obj.val().charAt(5));

    if (obj.val().length > 6)
        obj.val(obj.val().charAt(0) + obj.val().charAt(1) + obj.val().charAt(2) + obj.val().charAt(3) + obj.val().charAt(4) + obj.val().charAt(5) + obj.val().charAt(6));

    if (parseInt(obj.val()) < 10 && parseInt(obj.val()) >= 1 && obj.val().length > 5)
        obj.val(obj.val().charAt(0) + obj.val().charAt(1) + obj.val().charAt(2) + obj.val().charAt(3) + obj.val().charAt(4) + obj.val().charAt(5));

}
//比對select項目後設定
function setSelector(elementID, text) {
    var isSuccess = false;
    var options = $("#" + elementID + " option");
    $.each(options, function () {
        if ($(this).html() == text) {
            $(this).attr('selected', true);
            isSuccess = true;
        }
    });
    $("#" + elementID).click();
    return isSuccess;
}
//顯示訊息
function ShowMessage(isShow, type, ms) {    
    if (isShow) {
        if (type == 1) {
            $("#divSuccess").show();
            $("#lblSuccess").text(ms);
            $("#divError").hide();
            $("#lblError").val("");
        }
        else {
            $("#divError").show();
            $("#lblError").text(ms);
            $("#divSuccess").hide();
            $("#lblSuccess").val("");
        }
    }
    else {
        $("#divError").hide();
        $("#divSuccess").hide();
        $("#lblError").text("");
        $("#lblSuccess").text("");
    }
}

//建立kendo表格
function CreateKendoGrid(config) {
    if (!config) {
        config = {};
    };

    if (!config.obj)
        return;

    if (!config.scrollable)
        config.scrollable = true;
    if (!config.sortable)
        config.sortable = true;
    if (!config.resizable)
        config.resizable = true;
    if (!config.toolbar)
        config.toolbar = "";
    if (!config.dataBound)
        config.dataBound = "";
    if (!config.detailInit)
        config.detailInit = "";
    var grid = config.obj.kendoGrid({
        scrollable: config.scrollable,
        sortable: config.sortable,
        resizable: config.resizable,
        selectable: "row",
        dataSource: config.dataSource,
        columns: config.columns,
        toolbar: config.toolbar,
        dataBound: config.dataBound,
        detailInit: config.detailInit,
        pageable: (config.pageable ? (config.pageableIsCN ? {
            messages: {
                display: "{0} - {1} Records / Total Records:： {2}",
                empty: config.emptyMessage,
                page: "頁",
                of: "of {0}",
                itemsPerPage: "items per page",
                first: "第一頁",
                previous: "上一頁",
                next: "下一頁",
                last: "最後一頁",
                refresh: "Refresh"
            }
        } : true) : false)
    });
};
