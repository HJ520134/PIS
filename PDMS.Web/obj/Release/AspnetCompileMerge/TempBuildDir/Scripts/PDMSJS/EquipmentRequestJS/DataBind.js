
function CreateTab() {
    $("#myTab li").remove();
    var QueryMode = $('#js_s_input_querytype').val();
    var projectValue = $('#js_s_input_project').val();
    if (QueryMode == 1) { //Lock预估查询按月查询
        var myDate = new Date();
        var currentYear = myDate.getFullYear();
        var currentMonth = myDate.getMonth() + 1;
        var $holdDate = $('#js_s_input_date').val();
        var selectDate = new Date($holdDate.replace("-", "/").replace("-", "/"))
        var selectDateYear = selectDate.getFullYear();
        var selectDateMonth = selectDate.getMonth() + 1;
        var isFirst = true;
        var setMonth;
        for (i = selectDateMonth + 1; i <= selectDateMonth + 3; i++) {
            setMonth = i;
            if (i == 13) {
                setMonth = selectDateYear + 1 + "-01";
                $("#myTab").append('<li class="ppchecktab" ><a href="#" data-toggle="tab">' + setMonth + '</a></li>');
            }
            else if (i == 14) {
                setMonth = selectDateYear + 1 + "-02";
                $("#myTab").append('<li class="ppchecktab" ><a href="#" data-toggle="tab">' + setMonth + '</a></li>');
            }
            else if (i == 15) {
                setMonth = selectDateYear + 1 + "-03";
                $("#myTab").append('<li class="ppchecktab" ><a href="#" data-toggle="tab">' + setMonth + '</a></li>');
            }
            else {
                if (setMonth.toString().length < 2) {
                    setMonth = '0' + i;
                }
                $("#myTab").append('<li class="ppchecktab" ><a href="#" data-toggle="tab">' + selectDateYear + '-' + setMonth + '</a></li>');
            }
        }
    }
    else { //四周生产计划，按周查询
        for (i = 1; i <= 4; i++) {
            $("#myTab").append('<li class="ppchecktab" ><a href="#" data-toggle="tab">第' + i + '周</a></li>');
        }
    }
}
var SearchByPlant = function (url) {
    $('#js_rpt_tr').empty();
    $('#js_rpt_tr_foot').empty();
    $('#js_rpt_datatable').DataTable().clear();
    $('#js_rpt_tr').append("<th>@ViewBag.Functionfactory</th>");
    $('#js_rpt_tr_foot').append("<th>@ViewBag.Functionfactory</th>");
    $('#js_rpt_tr').append("<th>设备</th>");
    $('#js_rpt_tr_foot').append("<th>设备</th>");
    var columns = [{
            data: "FunPlant",
            className: "min-col-xs"
        }, {
            data: "Equipment_Name",
            className: "min-col-xs"
        }
    ];
    var params = {};
    params['PlantUID'] = $('#js_s_input_plant').val();
    params['OpTypeUID'] = $('#js_s_input_group').val();
    params['ProjectUID'] = $('#js_s_input_project').val();
    params['PartTypeUID'] = $('#js_s_input_parttypes').val();
    params['DateFrom'] = $('#js_s_input_date').val();
    params['QueryMode'] = $('#js_s_input_querytype').val();
    params['hidTab'] = $('#hidTab').val();
    $.post(url, params, function (data) {
        var isRequest = 0;
        var j = 0;
        var m = 0
        var dynamicName = data.columnList;
        for (var k = 0; k < dynamicName.length; k++) {
            if (dynamicName[k] == "Product_Date") {
                j = k;
            }
        }
        $.each(dynamicName, function (i, item) {
            if (item == "Product_Date") {
                isRequest = 1;
                return true; //相当于continue;
            }
            if (isRequest == 0) {
                if (m != j - 1) { //求总列
                    $('#js_rpt_tr').append("<th>" + item + "现有</th>");
                    $('#js_rpt_tr_foot').append("<th>" + item + "现有</th>");
                    m++;
                }
                else {
                    $('#js_rpt_tr').append("<th>主设备汇总</th>");
                    $('#js_rpt_tr_foot').append("<th>主设备汇总</th>");
                    m = 0;
                }
            }
            else {
                if (m < j - 1) {
                    $('#js_rpt_tr').append("<th>" + dynamicName[m] + "需求</th>");
                    $('#js_rpt_tr_foot').append("<th>" + dynamicName[m] + "需求</th>");
                    m++;
                }
                else if (m == j - 1) {
                    $('#js_rpt_tr').append("<th>需求汇总</th>");
                    $('#js_rpt_tr_foot').append("<th>需求汇总</th>");
                    m++;
                }
                else {
                    $('#js_rpt_tr').append("<th>" + item + "</th>");
                    $('#js_rpt_tr_foot').append("<th>" + item + "</th>");
                }
            }
            var obj = {};
            obj['data'] = item;
            obj['className'] = "min-col-xs text-right";
            columns.push(obj);
        });
        var dt = $('#js_rpt_datatable').DataTable({
            destroy: true,
            scrollX: true,
            scrollY: 450,
            data: data.data,
            columns: columns
        });
        $("table th").removeClass('text-right');
        //冻结区域
        new $.fn.dataTable.FixedColumns(dt, { "iLeftColumns": 2 });
    });
}
var SearchALLOP = function (url) {
    var columnsTwo = [
                    {
                        data: null,
                        createdCell: function (td, cellData, rowData, row, col) {
                            var hidOptype = '<input type="hidden" id="hidOptype" value=' + rowData.Organization_UID + '>';
                            var hidFunc = '<input type="hidden" id="hidFunc" value=' + rowData.System_FunPlant_UID + '>';
                            var hidFunPlant = '<input type="hidden" id="hidFunPlant" value="' + rowData.FunPlant + '">';
                            var hidEquip = '<input type="hidden" id="hidEquip" value="' + rowData.Equipment_Name + '">';
                            var buttonEdit = '<button type="button" id="btnProject" class="btn btn-default btn-sm">统计单个专案</button>';
                            var result = '<button type="button" class="btn btn-default" rel="action-popover">' +
                                '<i class="fa fa-reorder"></i>' +
                                '</button>' +
                                '<div class="hidden popover-content">';
                            result = hidOptype + hidFunc + hidFunPlant + hidEquip + buttonEdit;
                            $(td).html(result);
                        },
                        className: "text-center js-grid-edit-optype"
                    },
                    {
                        data: null,
                        className: "min-col-xs text-center"
                    },
                        {
                            data: "FunPlant",
                            className: "min-col-xs"
                        }, {
                            data: "Equipment_Name",
                            className: "min-col-xs"

                        }, {
                            data: "MP_CurrentQty",
                            className: "min-col-xs text-right"

                        }, {
                            data: "NPI_CurrentQty",
                            className: "min-col-xs text-right"

                        }, {
                            data: null,
                            className: "min-col-xs text-right",
                            createdCell: function (td, cellData, rowData, row, col) {
                                var result = rowData.MP_CurrentQty + rowData.NPI_CurrentQty;
                                $(td).html(result);
                            }
                        }, {
                            data: "Request_MPANDNPI",
                            className: "min-col-xs text-right"

                        }, {
                            data: null,
                            className: "min-col-xs text-right",
                            createdCell: function (td, cellData, rowData, row, col) {
                                var result = rowData.MP_CurrentQty + rowData.NPI_CurrentQty - rowData.Request_MPANDNPI;
                                $(td).html(result);
                            }
                        }
    ];
    var dt2 = $('#js_rpt_op_datatable').DataTable({
        "bDestroy": true,
        scrollX: true,
        scrollY: 420,
        "ajax": {
            "url": url,
            "data": function (data) {
                return $('#js_form_query').serialize();
            }
        },
        "columns": columnsTwo,
        columnDefs: [{
            orderable: false,
            targets: [0, 1],
        }]
    });
    dt2.on('order.dt search.dt', function () {
        dt2.column(1, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
    $("table th").removeClass('text-right');
}
