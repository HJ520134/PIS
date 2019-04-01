//当PDMS存在则返回PDMS，如果不存在则返回{}
var PDMS = PDMS || {};
PDMS.Utility = PDMS.Utility || {};
PDMS.Storage = PDMS.Storage || $.initNamespaceStorage('PDMS');

//// PDMS Global Settings
PDMS.Utility.Settings = {
    
    Datatables: {
        dom: 'rt',
        paging: false,
        searching: false,
        bDestroy: true,
        ordering: true,
        autoWidth: false,
        //No initial order
        order: []
    },
    Pages_EN: {

        pageSize: 15,
        pageBtnCount: 11,
        showFirstLastBtn: true,
        firstBtnText: "Home",
        lastBtnText: "Last",
        prevBtnText: "Previous",
        nextBtnText: "Next",
        jumpBtnText: 'Link',
        infoFormat: 'Show {start} To {end} Total {total} ',
        loadFirstPage: true,
        remote: {
            url: null,
            params: null,
            callback: null,
            success: null,
            beforeSend: null,
            complete: null,
            pageIndexName: 'PageNumber',
            pageSizeName: 'PageSize',
            totalName: 'TotalItemCount'
        },
        showInfo: true,
        showJump: true,
        showPageSizes: true,
        pageSizeItems: [15, 25, 35, 50],
        debug: false
    },
    Pages: {

        pageSize: 15,
        pageBtnCount: 11,
        showFirstLastBtn: true,
        firstBtnText: "首页",
        lastBtnText: "最后一页",
        prevBtnText: "上一页",
        nextBtnText: "下一页",
        jumpBtnText: '跳转',
        infoFormat: '显示 {start} 到 {end} 共 {total} ',
        loadFirstPage: true,
        remote: {
            url: null,
            params: null,
            callback: null,
            success: null,
            beforeSend: null,
            complete: null,
            pageIndexName: 'PageNumber',
            pageSizeName: 'PageSize',
            totalName: 'TotalItemCount'
        },
        showInfo: true,
        showJump: true,
        showPageSizes: true,
        pageSizeItems: [15, 25,35, 50],
        debug: false
    },
    PopoverInTableAction: {
        template: ['<div class="action-cnt popover">',
                    '<div class="arrow"></div>',
                        '<div class="popover-content">',
                    '</div>',
                '</div>'].join('')
    },
    Datetimepicker: { format: 'yyyy-mm-dd', minView: 2, autoclose: true },
    Datetimepicker_Month: { format: 'yyyy-mm', todayHighlight: true, autoclose: true, startView: 3, minView: 3, language: 'zh-CN' },
    Datetimepicker_Time: { format: 'yyyy-mm-dd h:i', step: 1, hours12: false, minuteStep: 1, minDate: 0, todayHighlight: true, autoclose: true, language: 'zh-CN' },
    Datetimepicker_Timethree: { format: 'yyyy-mm-dd h:i', step: 1, hours12: false,  minuteStep:30, minDate: 0, todayHighlight: true, autoclose: true, language: 'zh-CN' },
    CriteriaPreviewWidth: 120,
    ValidateMessage: {
        twoDateError: "Date to must great than Date from",
        fourDateError:""
    }
};



//// Hide all Unauthorized Elements
//// call it after page loaded and ajax successed.
PDMS.Utility.HideUnauthorizedElements = function () {

    var pageId = $('#system_page_id');

    if (pageId.length > 0) {

        $.getJSON(pageId.data('url')
            , { pageUrl: pageId.data('id') }
            , function (data) {
                var needHideElements = data.PageElements.split(',');
                $.each(needHideElements, function (index, element) {
                    var target = $(element);
                    if (target.length > 0) {
                        target.hide();
                    }
                    target = null;
                });//end each
                data.type = 'UnauthorizedElements';
            });
    }
};

////// Extend jQuery Ajax for spp custom
//jQuery(function ($) {

//    var _ajax = $.ajax;

//    $.ajax = function (opt) {

//        //back ajax
//        var fn = {
//            error: function (XMLHttpRequest, textStatus, errorThrown) { },
//            success: function (data, textStatus) { }
//        }
//        if (opt.error) {
//            fn.error = opt && opt.error || function (a, b) { };
//        }
//        if (opt.success) {
//            fn.success = opt && opt.success || function (a, b) { };
//        }

//        var _opt = $.extend(opt, {
//            error: function (xhr, textStatus, errorThrown) {
//                //Error参数
//                //jqXHR对象、描述发生错误类型的一个字符串 和 捕获的异常对象。
//                //如果发生了错误，错误信息（第二个参数）除了得到null之外，还可能是"timeout", "error", "abort" ，和 "parsererror"。 
//                //当一个HTTP错误发生时，errorThrown 接收HTTP状态的文本部分，比如： "Not Found" 或者 "Internal Server Error."。
//                //从jQuery 1.5开始, 在error设置可以接受函数组成的数组。每个函数将被依次调用。 
//                //注意：此处理程序在跨域脚本和JSONP形式的请求时不被调用 
//                fn.error(XMLHttpRequest, textStatus, errorThrown);
//            },
//            success: function (data, textStatus) {

//                if (data.error) {

//                    PDMS.Utility.MessageBox.error(data.message);
//                    return;
//                } else {

//                    fn.success(data, textStatus);
//                    if (data.type != 'UnauthorizedElements') {
//                        PDMS.Utility.UnauthorizedElements.Hide();
//                    }
//                }
//            }
//        });

//        _ajax(_opt);
//    };
//});

PDMS.Utility.Tools = (function () {
    var tools = {};
    function _transferDate(assigntime) {

        if (!assigntime)
            return null;
        else {
            var reg = new RegExp('-', 'g');
            assigntime = assigntime + "";
            assigntime = assigntime.replace(reg, '/');//正则替换
            assigntime = new Date(parseInt(Date.parse(assigntime), 10));
            return assigntime;
        }
    }
    /// <summary>
    /// 判断html element是否含有特定class 
    /// </summary>
    tools.HasClass = function (el, elClassName) {
        return el.classList ? el.classList.contains(elClassName) : el.className.indexOf(elClassName) > -1;
    };
    /// <summary>
    /// 日期转换
    /// </summary>
    tools.TransferDate = function (assigntime) {

        return this._transferDate(assigntime);
    };
    /// <summary>
    /// 格式化字符串
    /// </summary>
    /// <param name="source">like c#, use {0}/{1}.. as placeholders</param>
    /// <param name="params">single value or arrary to replace prev placeholders</param>
    /// <returns>formated string</returns>
    tools.FormatString = function (source, params) {

        if (arguments.length === 1) {
            return 'params error';
        }
        if (arguments.length > 2 && params.constructor !== Array) {
            params = $.makeArray(arguments).slice(1);
        }
        if (params.constructor !== Array) {
            params = [params];
        }
        $.each(params, function (i, n) {
            source = source.replace(new RegExp("\\{" + i + "\\}", "g"), function () {
                return n;
            });
        });
        return source;
    };
    /// <summary>
    /// 比较传入时间的大小
    /// </summary>
    /// <param name="StartDate">开始时间</param>
    /// <param name="EndDate">结束时间</param>
    /// <returns>true/false</returns>
    tools.DateCompare = function (StartDate, EndDate) {

        StartDate = _transferDate(StartDate);
        EndDate = _transferDate(EndDate);
        if (StartDate == null || EndDate == null) {
            return true
        } else {
            return StartDate <= EndDate
        }
    };
    /// <summary>
    /// 后台验证两段时间是否拥有包含关系
    /// </summary>
    /// <param name="SubStart">待验证开始时间</param>
    /// <param name="SubEnd">待验证结束时间</param>
    /// <param name="HeadStart">验证标准开始时间</param>
    /// <param name="HeadEnd">验证标准结束时间</param>
    /// <returns>Pass：验证通过，NoNeedVerify：没有验证的必要，Other：验证不通过</returns>
    tools.DateCompareInterval = function (SubStart, SubEnd, HeadStart, HeadEnd) {
        SubStart = _transferDate(SubStart);
        SubEnd = _transferDate(SubEnd);
        HeadStart = _transferDate(HeadStart);
        HeadEnd = _transferDate(HeadEnd);
        if (SubStart == null && SubEnd == null)
            return "NoNeedVerify_SubStartAndEndDateNull";
        else if (SubStart != null && SubEnd != null && SubStart > SubEnd)
            return "NoNeedVerify_SubEndLowerThanSubStartDate";
        else if (HeadStart != null && HeadEnd != null && HeadStart > HeadEnd)
            return "NoNeedVerify_HeadEndLowerThanHeadStartDate";
        else {
            if (SubStart == null && SubEnd != null && HeadStart == null && HeadEnd != null && SubEnd > HeadEnd)
                return "SubEndLowerThanHeadEndDate";
            else if (SubStart == null && SubEnd != null && HeadStart != null && HeadEnd == null)
                return "SubStartIsNullHeadStartNotNull";
            else if (SubStart == null && SubEnd != null && HeadStart != null && HeadEnd != null)
                return "SubStartIsNullHeadStartNotNull";
            else if (SubStart != null && SubEnd == null && HeadStart == null && HeadEnd != null)
                return "SubEndIsNullHeadEndNotNull";
            else if (SubStart != null && SubEnd == null && HeadStart != null && HeadEnd != null)
                return "SubEndIsNullHeadEndNotNull";
            else if (SubStart != null && SubEnd == null && HeadStart != null && HeadEnd == null && SubStart < HeadStart)
                return "SubStartLowerThanHeadStartDate";
            else if (SubStart != null && SubEnd != null && HeadStart == null && HeadEnd != null && SubEnd > HeadEnd)
                return "SubEndLowerThanHeadEndDate";
            else if (SubStart != null && SubEnd != null && HeadStart != null && HeadEnd == null && SubStart < HeadStart)
                return "SubStartLowerThanHeadStartDate";
            else if (SubStart != null && SubEnd != null && HeadStart != null && HeadEnd != null && SubStart < HeadStart)
                return "SubStartLowerThanHeadStartDate";
            else if (SubStart != null && SubEnd != null && HeadStart != null && HeadEnd != null && SubEnd > HeadEnd)
                return "SubEndLowerThanHeadEndDate";
            else
                return "Pass";
        }
    };
    /// <summary>
    /// 获取当前登录用户的Account UID
    /// </summary>
    tools.GetLoginUserAccountID = function() {

        var accountId = 0;
        if (PDMS.Storage.sessionStorage.isSet('CurrentUserAccountID') && !PDMS.Storage.sessionStorage.isEmpty('CurrentUserAccountID')) {
            accountId = PDMS.Storage.sessionStorage.get('CurrentUserAccountID');
        }

        if (accountId === 0 || accountId === "undefined") {
            accountId = Cookies.get('CURRENT_ACCOUNTUID');
            PDMS.Storage.sessionStorage.set('CurrentUserAccountID', accountId);
        }

        return accountId;
    };
    return tools;
})();

PDMS.Utility.Datatables = (function () {

    var customOptions = {},
        seqIndex = -1,
        nosortIndexs = [],
        $targetTable = null,
        $targetTableThead = null;

    var _init = function () {

        if (window.matchMedia("(max-width: 1200px)").matches) {

            if ($targetTable.find('tbody>tr').eq(0).find('.td-col-detail').length == 0) {
                $targetTable.find('thead tr').prepend('<th>&nbsp;</th>');
                $targetTable.find('tfoot tr').prepend('<th>&nbsp;</th>');
                //添加colums
                customOptions.columns.unshift({
                    data: null,
                    orderable: false,
                    defaultContent: ' ',
                    className: "text-center text-success td-col-detail",
                    createdCell: function (td, cellData, rowData, row, col) {
                        $(td).html('<i class="fa fa-plus-circle td-detail-icon"></i>');
                    }
                });
            }

        };

        seqIndex = _getSeqIndex();
        nosortIndexs = _getNosortIndexs();
        $targetTableThead = _getTargetTableThead();
    };

    var _getTargetTableThead = function () {

        return $targetTableThead = $targetTableThead || $targetTable.find('thead tr')
    };

    var _getSeqIndex = function () {

        if (seqIndex === -1) {
            var targetCol = _getTargetTableThead().find('.table-col-seq');
            if (targetCol.length > 0) {
                seqIndex = targetCol.index();
            };
        }

        return seqIndex;
    };

    var _getNosortIndexs = function () {

        var $targetTableTheadCols = $targetTableThead.find('th');

        $.each($targetTableTheadCols, function (idx, val) {

            var flag = PDMS.Utility.Tools.HasClass(val, 'nosort');

            if (flag) {
                nosortIndexs.push(idx);
            };
        });

        return nosortIndexs;
    };

    var _setOptions = function () {

        if (customOptions.columns) {

            if (nosortIndexs.length > 0) {
                //set nosort column
                $.each(nosortIndexs, function (index, el) {
                    var targetCol = customOptions.columns[el];
                    targetCol["orderable"] = false;
                    targetCol["data"] = null;
                });
            };


        };

        return customOptions;
    };

    var _resize = function (t) {
        //hide cols
        var cols = customOptions.columns,
            indexArrary = [],
            visibleColClassNames = [],
            invisibleColNames = [],
            that = this;

        var setColVisiblity = function (visibleColClassNames, invisibleColNames) {

            $.each(cols, function (index, el) {

                for (var i = 0; i < visibleColClassNames.length; i++) {

                    var className = visibleColClassNames[i];
                    if (el.className && el.className.indexOf(className) > -1) {

                        t.columns('.' + className).visible(true);
                    };
                };

                for (var i = 0; i < invisibleColNames.length; i++) {

                    var className = invisibleColNames[i];
                    if (el.className && el.className.indexOf(className) > -1) {

                        t.columns('.' + className).visible(false);
                    };
                };
            });

            return indexArrary;
        };

        if (window.matchMedia("(max-width:768px)").matches) {

            visibleColClassNames = ['min-col-xs', 'td-col-detail'];
            invisibleColNames = ['min-col-lg', 'min-col-md', 'min-col-sm'];

        } else if (window.matchMedia("(max-width:992px)").matches) {

            visibleColClassNames = ['min-col-sm', 'min-col-xs', 'td-col-detail'];
            invisibleColNames = ['min-col-lg', 'min-col-md'];

        } else if (window.matchMedia("(max-width: 1200px)").matches) {

            visibleColClassNames = ['min-col-md', 'min-col-sm', 'min-col-xs', 'td-col-detail'];
            invisibleColNames = ['min-col-lg'];

        } else {

            visibleColClassNames = ['min-col-md', 'min-col-sm', 'min-col-xs', 'min-col-lg'];
            //invisibleColNames = ['td-col-detail'];
        }

        return setColVisiblity(visibleColClassNames, invisibleColNames);
    };

    return {

        SetDatetable: function (datatableConfig) {

            var that = this,
                tableId = datatableConfig.tableId,
                options = datatableConfig.tableOptions;

            //remove # if tableid start with #
            if (tableId.indexOf('#') == 0) {
                tableId = tableId.substring(1);
            }

            $targetTable = $('#' + tableId);
            customOptions = options;

            _init();

            var finalOptions = _setOptions();
            var finalTable = $targetTable.DataTable(finalOptions);

            //set seq
            if (seqIndex > -1) {

                finalTable
					.on('order.dt', function () {

					    finalTable.column(seqIndex, { order: 'applied' })
			        	.nodes()
			        	.each(function (cell, i) {
			        	    cell.innerHTML = i + 1;
			        	});
					})
			        .draw();
            };

            _resize(finalTable);

            $(document).on('click', '.td-detail-icon', function () {
                //get row
                var rowIdx = $(this).closest('tr').index();
                var rowCells = finalTable.cells(rowIdx, '')[0];
                var rowDataHtml = '';
                var modalId = tableId + '_modal';

                //build content of modal body
                for (var i = seqIndex + 1; i < rowCells.length; i++) {

                    var title = finalTable.columns(i).header();
                    var cellData = finalTable.cells(rowIdx, i).data();

                    rowDataHtml += '<div class="form-group">' +
                                    '<label class="col-sm-4 control-label">' + $(title).html() + '</label>' +
                                    '<div class="col-sm-8">' +
                                      '<p class="form-control-static">' + cellData[0] + '</p>' +
                                    '</div>' +
                                  '</div>';
                };

                if ($('#' + modalId).length === 0) {

                    var modalHtml = '<div class="modal fade" id="' + modalId + '" tabindex="-1" role="dialog">' +
                                      '<div class="modal-dialog" role="document">' +
                                        '<div class="modal-content">' +
                                          '<div class="modal-header">' +
                                            '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                                            '<h4 class="modal-title" id="myModalLabel">Row Details</h4>' +
                                          '</div>' +
                                          '<div class="modal-body">' +
                                            '<div class="form-horizontal">' + rowDataHtml + '</div>' +
                                          '</div>' +
                                          '<div class="modal-footer">' +
                                            '<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>' +
                                          '</div>' +
                                        '</div>' +
                                      '</div>' +
                                    '</div>';

                    $('body').append(modalHtml);

                } else {

                    $('#' + modalId + " .form-horizontal").empty().append(rowDataHtml);
                }

                $('#' + modalId).modal('show');
            });

            //$(window).on('resize.dtr', finalTable.settings()[0].oApi._fnThrottle(function () {

            //    PDMS.Utility.Datatables.ResizeDatatable(finalTable);
            //}));

            ////Destroy event handler
            //finalTable.on('destroy.dtr', function () {
            //    $(window).off('resize.dtr');
            //});

            return finalTable;
        },
        ResizeDatatable: function (t) {
            _resize(t);
        }
    }
})();

PDMS.Utility.Pages = (function () {
    return {
        Set: function (config) {
            var datatable;
            var pageId = config.pageId;
            if (pageId.indexOf('#') < 0) {
                pageId = '#' + pageId;
            }
            $(config.pageId).page({
                remote: {
                    url: config.remoteUrl,
                    params: config.searchParams,
                    success: function (data, pageIndex) {

                        config.tableOptions.aaData = data.Items;
                        config.tableOptions.destroy = true;

                        datatable = PDMS.Utility.Datatables.SetDatetable(config);
                        var table = config.tableId;
                        $(table).find('.js-checkbox-all').prop('checked', false);
                    }
                }//end remote
            });//end page

        },//end SetPages

        CustomSet: function (config) {
            var datatable;
            var pageId = config.pageId;
            if (pageId.indexOf('#') < 0) {
                pageId = '#' + pageId;
            }
            var pageSize = config.pageSize;
            var pageNumber = config.pageNumber;
            $(config.pageId).page({
                pageNumber:pageNumber,
                pageSize: 15,
                pageBtnCount: 11,
                prevBtnText: "上一页",
                nextBtnText: "下一页",
                showInfo: true,
                showJump: true,
                showPageSizes: true,
                pageSizeItems: [15, 25,35, 50],
                debug: false,
                remote: {
                    url: config.remoteUrl,
                    params: config.searchParams,
                    success: function (data, pageIndex) {

                        config.tableOptions.aaData = data.Items;
                        config.tableOptions.destroy = true;

                        datatable = PDMS.Utility.Datatables.SetDatetable(config);
                    }
                }//end remote
            });//end page

        }
    }
})();

PDMS.Utility.ReturnDataTable = (function () {
    return {
        Set: function (config, chk_value) {
            var datatable;
            var pageId = config.pageId;
            if (pageId.indexOf('#') < 0) {
                pageId = '#' + pageId;
            }

            $(config.pageId).page({
                remote: {
                    url: config.remoteUrl,
                    params: config.searchParams,
                    success: function (data, pageIndex) {


                        config.tableOptions.aaData = data.Items;
                        config.tableOptions.destroy = true;
                        debugger;
                        datatable = PDMS.Utility.Datatables.SetDatetable(config);
                        //第一次加载的时候隐藏列，第二次加载隐藏列的时候会有bug
                        for (var i = 0; i < chk_value.length; i++) {
                            var tableColumn = datatable.column(chk_value[i]);
                            tableColumn.visible(!tableColumn.visible());
                        }
                    }
                }//end remote
            });//end page
            return datatable;
        }//end SetPages
    }
})();

PDMS.Utility.GetCurrentLanguageId = function () {
    var languageId = $("#hidCurrentLanguageIdFlag").val();
    return languageId;
}

PDMS.Utility.MessageBox = {
    info: function (message, callback) {
        var id = PDMS.Utility.GetCurrentLanguageId();
        if ($('#infoModal').length == 0) {
            var modalHtml = '<div class="modal fade" id="infoModal" tabindex="-1" role="dialog">' +
                                '<div class="modal-dialog" role="document">' +
                                    '<div class="modal-content">' +
                                        '<div class="modal-header">' +
                                            '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                                            '<h4 class="modal-title" id="myModalLabel">Replace1</h4>' +
                                        '</div>' +
                                        '<div class="modal-body"></div>' +
                                        '<div class="modal-footer">' +
                                            '<button type="button" class="btn btn-default" data-dismiss="modal">Replace2</button>' +
                                        '</div>' +
                                    '</div>' +
                                '</div>' +
                            '</div>';
            if (id == 1) {
                modalHtml = modalHtml.replace('Replace1', 'Prompt');
                modalHtml = modalHtml.replace('Replace2', 'Close');
            }
            else {
                modalHtml = modalHtml.replace('Replace1', '提示');
                modalHtml = modalHtml.replace('Replace2', '关闭');
            }

            $('body').append(modalHtml);
        }
        var modal = $("#infoModal").modal();
        $("#infoModal .modal-body").text(message);
        $("#infoModal .modal-footer .btn").unbind("click");
        $("#infoModal .modal-footer .btn").bind("click", function () {
            if (callback) {
                callback();
                modal.hide();
            }
        });
        modal.show();
    },
    hideinfo:function () {
        var closeBtn = $('#infoModal .modal-footer .btn');
        if (closeBtn != undefined) {
            closeBtn.click();
        }
    },
    infohtml: function (message, callback) {
        var id = PDMS.Utility.GetCurrentLanguageId();
        if ($('#infohtmlModal').length == 0) {
            var modalHtml = '<div class="modal fade" id="infohtmlModal" tabindex="-1" role="dialog">' +
                                '<div class="modal-dialog" role="document">' +
                                    '<div class="modal-content">' +
                                        '<div class="modal-header">' +
                                            '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                                            '<h4 class="modal-title" id="myModalLabel">Replace1</h4>' +
                                        '</div>' +
                                        '<div class="modal-body"></div>' +
                                        '<div class="modal-footer">' +
                                            '<button type="button" class="btn btn-default" data-dismiss="modal">Replace2</button>' +
                                        '</div>' +
                                    '</div>' +
                                '</div>' +
                            '</div>';
            if (id == 1) {
                modalHtml = modalHtml.replace('Replace1', 'Prompt');
                modalHtml = modalHtml.replace('Replace2', 'Close');
            }
            else {
                modalHtml = modalHtml.replace('Replace1', '提示');
                modalHtml = modalHtml.replace('Replace2', '关闭');
            }

            $('body').append(modalHtml);
        }
        var modal = $("#infohtmlModal").modal();
        $("#infohtmlModal .modal-body").html(message);
        $("#infohtmlModal .modal-footer .btn").unbind("click");
        $("#infohtmlModal .modal-footer .btn").bind("click", function () {
            if (callback) {
                callback();
                modal.hide();
            }
        });
        modal.show();
    },

    hideinfohtml: function () {
        var closeBtn = $('#infohtmlModal .modal-footer .btn');
        if (closeBtn != undefined) {
            closeBtn.click();
        }
    },

    error: function (message, callback) {
        var id = PDMS.Utility.GetCurrentLanguageId();
        if ($('#errorModal').length == 0) {
            var modalHtml = '<div class="modal fade modal-danger" id="errorModal" tabindex="-1" role="dialog">' +
                               '<div class="modal-dialog" role="document">' +
                                   '<div class="modal-content">' +
                                       '<div class="modal-header">' +
                                           '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                                           '<h4 class="modal-title" id="myModalLabel">Replace1</h4>' +
                                       '</div>' +
                                       '<div class="modal-body"></div>' +
                                       '<div class="modal-footer">' +
                                           '<button type="button" class="btn btn-default" data-dismiss="modal">Replace2</button>' +
                                       '</div>' +
                                   '</div>' +
                               '</div>' +
                           '</div>';
            if (id == 1) {
                modalHtml = modalHtml.replace('Replace1', 'Error');
                modalHtml = modalHtml.replace('Replace2', 'Close');
            }
            else {
                modalHtml = modalHtml.replace('Replace1', '错误');
                modalHtml = modalHtml.replace('Replace2', '关闭');
            }
            $('body').append(modalHtml);
        }
        var modal = $("#errorModal").modal();
        $("#errorModal .modal-body").text(message);
        $("#errorModal .modal-footer .btn").unbind("click");
        $("#errorModal .modal-footer .btn").bind("click", function () {
            if (callback) {
                callback();
                modal.hide();
            }
        });
        modal.show();
    },

    hideerror:function myfunction() {
        var closeBtn = $('#errorModal .modal-footer .btn');
        if (closeBtn != undefined) {
            closeBtn.click();
        }
    },

    confirm: function (message, callbackYes, callbackNo) {
        var id = PDMS.Utility.GetCurrentLanguageId();
        if ($('#confirmModal').length == 0) {
            var modalHtml = '<div class="modal fade" id="confirmModal" tabindex="-1" role="dialog">' +
                               '<div class="modal-dialog" role="document">' +
                                   '<div class="modal-content">' +
                                       '<div class="modal-header">' +
                                           '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                                           '<h4 class="modal-title" id="myModalLabel">Replace1</h4>' +
                                       '</div>' +
                                       '<div class="modal-body"></div>' +
                                       '<div class="modal-footer">' +
                                           '<button type="button" class="btn btn-primary">Replace2</button>' +
                                           '<button type="button" class="btn btn-default" data-dismiss="modal">Replace3</button>' +
                                       '</div>' +
                                   '</div>' +
                               '</div>' +
                           '</div>';
            if (id == 1) {
                modalHtml = modalHtml.replace('Replace1', 'Confirm');
                modalHtml = modalHtml.replace('Replace2', 'Yes');
                modalHtml = modalHtml.replace('Replace3', 'No');
            }
            else {
                modalHtml = modalHtml.replace('Replace1', '确认');
                modalHtml = modalHtml.replace('Replace2', '是');
                modalHtml = modalHtml.replace('Replace3', '否');
            }
            $('body').append(modalHtml);
        }
        var modal = $("#confirmModal").modal();
        $("#confirmModal .modal-body").text(message);
        $("#confirmModal .modal-footer .btn-default").unbind("click");
        $("#confirmModal .modal-footer .btn-default").bind("click", function () {
            if (callbackNo) {
                callbackNo();
            }
            $("#confirmModal").modal('hide');
        });
        $("#confirmModal .modal-footer .btn-primary").unbind("click");
        $("#confirmModal .modal-footer .btn-primary").bind("click", function () {
            if (callbackYes) {
                callbackYes();
            }
            $("#confirmModal").modal('hide');
        });
        modal.show();
    },
    confirmhtml: function (message, callbackYes, callbackNo) {
        var id = PDMS.Utility.GetCurrentLanguageId();
        if ($('#confirmhtmlModal').length == 0) {
            var modalHtml = '<div class="modal fade" id="confirmhtmlModal" tabindex="-1" role="dialog">' +
                               '<div class="modal-dialog" role="document">' +
                                   '<div class="modal-content">' +
                                       '<div class="modal-header">' +
                                           '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                                           '<h4 class="modal-title" id="myModalLabel">Replace1</h4>' +
                                       '</div>' +
                                       '<div class="modal-body"></div>' +
                                       '<div class="modal-footer">' +
                                           '<button type="button" class="btn btn-primary">Replace2</button>' +
                                           '<button type="button" class="btn btn-default" data-dismiss="modal">Replace3</button>' +
                                       '</div>' +
                                   '</div>' +
                               '</div>' +
                           '</div>';
            if (id == 1) {
                modalHtml = modalHtml.replace('Replace1', 'Confirm');
                modalHtml = modalHtml.replace('Replace2', 'Yes');
                modalHtml = modalHtml.replace('Replace3', 'No');
            }
            else {
                modalHtml = modalHtml.replace('Replace1', '确认');
                modalHtml = modalHtml.replace('Replace2', '是');
                modalHtml = modalHtml.replace('Replace3', '否');
            }
            $('body').append(modalHtml);
        }
        var modal = $("#confirmhtmlModal").modal();
        $("#confirmhtmlModal .modal-body").html(message);
        $("#confirmhtmlModal .modal-footer .btn-default").unbind("click");
        $("#confirmhtmlModal .modal-footer .btn-default").bind("click", function () {
            if (callbackNo) {
                callbackNo();
            }
            $("#confirmhtmlModal").modal('hide');
        });
        $('#confirmhtmlModal').on('hide.bs.modal', function () {
            if (callbackNo) {
                callbackNo();
            }
        })

        $("#confirmhtmlModal .modal-footer .btn-primary").unbind("click");
        $("#confirmhtmlModal .modal-footer .btn-primary").bind("click", function () {
            if (callbackYes) {
                callbackYes();
            }
            $("#confirmhtmlModal").modal('hide');
        });

        modal.show();
    }
}

PDMS.Utility.Criteria = (function () {

    var _clearCriteria = function () {

        var $moreCriteriaCnt = $('#js_criteria_more_cnt'),
            $previewCriteriaCnt = $('#js_search_keywords');

        if ($moreCriteriaCnt.length > 0) {
            $moreCriteriaCnt.remove();
        }
        $previewCriteriaCnt.html("");
    }

    return {

        Init: function () {

            $(document).on('click', '.btn-search-keywords-more', function () {

                var $previewCriteriaCnt = $('#js_search_keywords').hide();

                var template = '<div class="alert alert-dismissible fade in" id="js_criteria_more_cnt" role="alert" >' +
                                    '<button type="button" class="close js-criteria-more-close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span></button>' +
                                    '<strong>Criteria:</strong> ' + $previewCriteriaCnt.data('criteria') +
                               '</div>';
                $('#js_criteria_alert_cnt').append(template);
            });

            $(document).on('click', '.js-criteria-more-close', function () {
                $('#js_search_keywords').show();
            });
        },

        Build: function () {

            _clearCriteria();

            var $searchform = $('#js_form_query'),
                $previewCriteriaCnt = $('#js_search_keywords'),
                htmlAppend = '<small><strong>Criteria: </strong>',
                criteria = [],
                criteriaStr = '';

            var $labels = $searchform.find('label').not('.radio-inline').not('.checkbox-inline').not('.error');

            $.each($labels, function (index, item) {

                var $label = $(item),
                    labelText = $label.text(),
                    queryText = '',
                    type = 'input',
                    labelData = $label.data(),
                    $labelForCnt = $label.next(),
                    targetId = $label.attr('for');

                if (labelData.type && labelData.type.toLowerCase() !== 'input') {

                    type = labelData.type.toLowerCase();

                    switch (type) {
                        case 'select':
                            queryText = $.trim($('#' + targetId + ' option:selected').text());
                            break;
                        case 'checkbox':
                            queryText = $.map(
                                            $labelForCnt.find('input[name=' + labelData.forName + ']:checked').parent()
                                            , function (val, i) {
                                                return $.trim(val.innerText);
                                            })
                                        .join(',');
                            if ($.trim(queryText).length > 0) { queryText = '[ ' + queryText + ' ]'; }
                            break;
                        case 'radio':
                            queryText = $.trim($labelForCnt.find('input[name=' + labelData.forName + ']:checked').parent().text());
                            break
                        case 'date-interval':
                            {
                                var $dates = $label.parent().find('input.date');
                                joinStr = ' ';
                                queryText = $.map(
                                                $dates
                                                , function (val, i) {
                                                    var dateInput = $(val);
                                                    var dateValue = dateInput.val();
                                                    if (dateValue !== '') {

                                                        return dateInput.prev().text() + ' ' + dateValue;
                                                    }
                                                })
                                            .join(' ');
                            }
                            break;
                        case 'datetime-interval':
                            {
                                var $dates = $label.parent().find('input.date_time');
                                joinStr = ' ';
                                queryText = $.map(
                                                $dates
                                                , function (val, i) {
                                                    var dateInput = $(val);
                                                    var dateValue = dateInput.val();
                                                    if (dateValue !== '') {

                                                        return dateInput.prev().text() + ' ' + dateValue;
                                                    }
                                                })
                                            .join(' ');
                            }
                            break;
                        case 'num-interval':
                            {
                                var $dates = $label.parent().find('input');
                                joinStr = ' ';
                                queryText = $.map(
                                                $dates
                                                , function (val, i) {
                                                    var dateInput = $(val);
                                                    var dateValue = dateInput.val();
                                                    if (dateValue !== '') {

                                                        return dateInput.prev().text() + ' ' + dateValue;
                                                    }
                                                })
                                            .join(' ');
                            }
                            break;
                        case 'date-intervalthree':
                            {
                                var $dates = $label.parent().find('input.date_timethree');
                                joinStr = ' ';
                                queryText = $.map(
                                                $dates
                                                , function (val, i) {
                                                    var dateInput = $(val);
                                                    var dateValue = dateInput.val();
                                                    if (dateValue !== '') {

                                                        return dateInput.prev().text() + ' ' + dateValue;
                                                    }
                                                })
                                            .join(' ');
                            }
                            break;
                        default:
                            PDMS.Utility.MessageBox.info(type + ' type is not be supported');
                            break;
                    };
                } else {
                    //-----------------add by Rock 2017-09-05 用户没有在input标签的id里面定义label标签的for这种情况，排除掉 start----------------------------------
                    if (document.getElementById(targetId) == null) {
                        return true; //相当于continue
                    }
                    //-----------------add by Rock 2017-09-05 end----------------------------------
                    //default input
                    type = document.getElementById(targetId).tagName;
                    
                    if (type!= "INPUT") {
                        switch (type) {
                            case 'SELECT':
                                queryText = $.trim($('#' + targetId + ' option:selected').text());
                                break;
                            case 'CHECKBOX':
                                queryText = $.map(
                                                $labelForCnt.find('input[name=' + labelData.forName + ']:checked').parent()
                                                , function (val, i) {
                                                    return $.trim(val.innerText);
                                                })
                                            .join(',');
                                if ($.trim(queryText).length > 0) { queryText = '[ ' + queryText + ' ]'; }
                                break;
                            case 'RADIO':
                                queryText = $.trim($labelForCnt.find('input[name=' + labelData.forName + ']:checked').parent().text());
                                break
                            case 'date-interval':
                                {
                                    var $dates = $label.parent().find('input.date');
                                    joinStr = ' ';
                                    queryText = $.map(
                                                    $dates
                                                    , function (val, i) {
                                                        var dateInput = $(val);
                                                        var dateValue = dateInput.val();
                                                        if (dateValue !== '') {

                                                            return dateInput.prev().text() + ' ' + dateValue;
                                                        }
                                                    })
                                                .join(' ');
                                }
                                break;
                            case 'datetime-interval':
                                {
                                    var $dates = $label.parent().find('input.date_time');
                                    joinStr = ' ';
                                    queryText = $.map(
                                                    $dates
                                                    , function (val, i) {
                                                        var dateInput = $(val);
                                                        var dateValue = dateInput.val();
                                                        if (dateValue !== '') {

                                                            return dateInput.prev().text() + ' ' + dateValue;
                                                        }
                                                    })
                                                .join(' ');
                                }
                                break;
                            case 'date-intervalthree':
                                {
                                    var $dates = $label.parent().find('input.date_timethree');
                                    joinStr = ' ';
                                    queryText = $.map(
                                                    $dates
                                                    , function (val, i) {
                                                        var dateInput = $(val);
                                                        var dateValue = dateInput.val();
                                                        if (dateValue !== '') {

                                                            return dateInput.prev().text() + ' ' + dateValue;
                                                        }
                                                    })
                                                .join(' ');
                                }
                                break;
                            case 'num-interval':
                                {
                                    var $dates = $label.parent().find('input');
                                    joinStr = ' ';
                                    queryText = $.map(
                                                    $dates
                                                    , function (val, i) {
                                                        var dateInput = $(val);
                                                        var dateValue = dateInput.val();
                                                        if (dateValue !== '') {

                                                            return dateInput.prev().text() + ' ' + dateValue;
                                                        }
                                                    })
                                                .join(' ');
                                }
                                break;
                            default:
                                PDMS.Utility.MessageBox.info(type + ' type is not be supported');
                                break;
                        };
                    }
                    //-----------------add by Rock 2017-06-05 start----------------------------------
                    else if (type == "INPUT") {
                        queryText = $('#' + targetId).val();
                    }
                    //-----------------add by Rock 2017-06-05 end----------------------------------
                    //if (type == "SELECT") {
                    //    queryText = $.trim($('#' + targetId + ' option:selected').text());
                    //} else {
                    //    queryText = $('#' + targetId).val();
                    //}
                    //queryText = $('#' + targetId).val();
                }

                if ($.trim(queryText) !== '' && $.trim(queryText) !== 'Nothing') {
                    criteria.push(labelText + '=' + queryText);
                }
            });

            criteriaStr = criteria.join(' & ');

            if (criteriaStr.length > 0) {

                if (criteriaStr.getStrWidth() > PDMS.Utility.Settings.CriteriaPreviewWidth) {

                    htmlAppend += criteriaStr.substring(0, PDMS.Utility.Settings.CriteriaPreviewWidth)
                    htmlAppend += '</small> ';
                    htmlAppend += '<a class="btn btn-default btn-xs btn-search-keywords-more">' +
                                      ' More <i class="search-more fa fa-caret-down"></i>' +
                                  '</a>';
                    $previewCriteriaCnt.data('criteria', criteriaStr).html(htmlAppend);
                } else {

                    htmlAppend += criteriaStr
                    htmlAppend += '</small>';
                    $previewCriteriaCnt.html(htmlAppend);
                };

                $previewCriteriaCnt.show();
            } else {

                $previewCriteriaCnt.html('');
            }
        },//end build

        Clear: function (appendFunction) {

            _clearCriteria();

            var $searchform = $('#js_form_query');

            $searchform.find('input:text').val('');
            $searchform.find('input[type=checkbox]:checked').prop('checked', false);
            $searchform.find('input[type=radio]:checked').prop('checked', false);
            $searchform.find('select option:eq(0)').prop('selected', true);

            if (appendFunction && jQuery.isFunction(appendFunction)) {
                appendFunction();
            }
        }//end clear
    }
})();

//#region javastript string extend
String.prototype.endWith = function (s) {
    if (s == null || s == "" || this.length == 0 || s.length > this.length)
        return false;
    if (this.substring(this.length - s.length) == s)
        return true;
    else
        return false;
    return true;
}

String.prototype.startWith = function (s) {
    if (s == null || s == "" || this.length == 0 || s.length > this.length)
        return false;
    if (this.substr(0, s.length) == s)
        return true;
    else
        return false;
    return true;
}

String.prototype.getStrWidth = function () {
    var realLength = 0;
    var len = this.length;
    var charCode = -1;
    for (var i = 0; i < len; i++) {
        charCode = this.charCodeAt(i);
        if (charCode >= 0 && charCode <= 128) {
            realLength += 1;
        } else {
            // 如果是中文则长度加1.5
            realLength += 1.5;
        }
    }
    return realLength;
}
//#endregion

//#region validate Custom Methods
jQuery.validator.addMethod(

    "StartDateGTEnd"
    , function (value, element, params) {

        //var tools = new PDMS.Utility.Tools();
        return PDMS.Utility.Tools.DateCompare($(params).val(), value);
    }
    , 'Date to must great than Date from'
);
//#endregion

var GetRootPath = function(){
    var strFullPath = window.document.location.href;
    var strPath = window.document.location.pathname;
    var pos = strFullPath.indexOf(strPath);
    var prePath = strFullPath.substring(0, pos);
    var postPath = strPath.substring(0, strPath.substr(1).indexOf('/') + 1);
    return (prePath + postPath);
}

$(document).ready(function () {

    (function () {

        $.extend($.fn.dataTable.defaults, PDMS.Utility.Settings.Datatables);
        var id = PDMS.Utility.GetCurrentLanguageId();
        var url = GetRootPath();
        url = url + '/Common/GetSelectLanguageId';
        $.ajaxSettings.async = false;
        $.get(url, function (data) {
            if (data == '1') {
                $.fn.page.defaults = PDMS.Utility.Settings.Pages_EN;
            }
            else {
                $.fn.page.defaults = PDMS.Utility.Settings.Pages;
            }
        });

        $.fn.serializeObject = function () {
            var o = {};
            var a = this.serializeArray();
            $.each(a, function () {
                if (o[this.name]) {
                    if (!o[this.name].push) {
                        o[this.name] = [o[this.name]];
                    }
                    o[this.name].push(this.value || '');
                } else {
                    o[this.name] = this.value || '';
                }
            });
            return o;
        };

        var $searchForm = $('#js_form_query');
        if ($searchForm.data('need-validate')) {

            if ($searchForm.find('ul.form-validate-error').length == 0) {

                $searchForm.append('<div class="col-xs-12"><ul class="list-group form-validate-error"></ul></div>');
            }
            var $errorCnt = $searchForm.find($('ul.form-validate-error'));

            $searchForm.validate({
                errorContainer: $errorCnt,
                errorLabelContainer: $errorCnt,
                wrapper: 'li',
                rules: {
                    Modified_Date_End: { StartDateGTEnd: "#js_s_input_modified_from" }
                }
            });
        }

        //checkbox all
        $(document).on('click', '.js-checkbox-all', function () {
            debugger;
            var $self = $(this);
            $self.closest('.dataTables_wrapper').find('.js-checkbox-item,.js-checkbox-item2').prop('checked', $self.prop('checked'));
        });

        //action popover in datatables
        $(document).popover({
            selector: '[rel=action-popover]',
            container: 'body',
            trigger: 'focus',
            content: function () {
                return $(this).parent().find('.popover-content').html();
            },
            template: PDMS.Utility.Settings.PopoverInTableAction.template,
            placement: "bottom",
            html: true
        });

        $('.date').datetimepicker(PDMS.Utility.Settings.Datetimepicker);
        $('.date_time').datetimepicker(PDMS.Utility.Settings.Datetimepicker_Time);
        $('.date-month').datetimepicker(PDMS.Utility.Settings.Datetimepicker_Month);
        $('.date_timethree').datetimepicker(PDMS.Utility.Settings.Datetimepicker_Timethree);
        

        $('.date-month').click(function () {
            $('.datetimepicker-months .table-condensed tbody tr td').find('span:eq(0)').text('1月');
            $('.datetimepicker-months .table-condensed tbody tr td').find('span:eq(1)').text('2月');
            $('.datetimepicker-months .table-condensed tbody tr td').find('span:eq(2)').text('3月');
            $('.datetimepicker-months .table-condensed tbody tr td').find('span:eq(3)').text('4月');
            $('.datetimepicker-months .table-condensed tbody tr td').find('span:eq(4)').text('5月');
            $('.datetimepicker-months .table-condensed tbody tr td').find('span:eq(5)').text('6月');
            $('.datetimepicker-months .table-condensed tbody tr td').find('span:eq(6)').text('7月');
            $('.datetimepicker-months .table-condensed tbody tr td').find('span:eq(7)').text('8月');
            $('.datetimepicker-months .table-condensed tbody tr td').find('span:eq(8)').text('9月');
            $('.datetimepicker-months .table-condensed tbody tr td').find('span:eq(9)').text('10月');
            $('.datetimepicker-months .table-condensed tbody tr td').find('span:eq(10)').text('11月');
            $('.datetimepicker-months .table-condensed tbody tr td').find('span:eq(11)').text('12月');
        })
    })();
});


