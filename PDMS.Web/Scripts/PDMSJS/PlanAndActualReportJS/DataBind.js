var SearchEquip = function (url) {
    PDMS.Utility.Criteria.Build();
    $('#js_rpt_equip_tr').empty();
    $('#js_equip_datatable').DataTable().clear();

    var selectOptype = $('#js_s_input_group').val();
    var selectProject = $('#js_s_input_project').val();
    var selectPartsType = $('#js_s_input_parttypes').val();
    if (selectOptype == '0') {
        PDMS.Utility.MessageBox.info('设备对比必须要选中专案，Business Group不能为ALL');
        return false;
    }
    if (selectProject == '0') {
        PDMS.Utility.MessageBox.info('设备对比必须要选中专案，专案不能为ALL');
        return false;
    }
    if (selectPartsType == '0') {
        PDMS.Utility.MessageBox.info('设备对比必须要选中专案，部件不能为ALL');
        return false;
    }
    if (new Date($('#js_s_input_modified_from').val()) > new Date($('#js_s_input_modified_to').val())) {
        PDMS.Utility.MessageBox.error('开始日期不能大于结束日期');
        return false;
    }  
    var _getParams = function () {
        return $('#js_form_query').serialize().replace(/\+/g, " ");
    };
    $('#js_rpt_equip_tr').append("<th>制程序号</th>");
    $('#js_rpt_equip_tr').append("<th>制程</th>");
    $('#js_rpt_equip_tr').append("<th>设备名称</th>");
    var columns = [
        {
            data: "Process_Seq",
            className: "min-col-xs text-right"
        },
        {
            data: "Process",
            className: "min-col-xs"
        }, {
            data: "Equipment_Name",
            className: "min-col-xs"
        }];

    var params = {};
    params['PlantUID'] = $('#js_s_input_plant').val();
    params['OpTypeUID'] = $('#js_s_input_group').val();
    params['ProjectUID'] = $('#js_s_input_project').val();
    params['PartTypeUID'] = $('#js_s_input_parttypes').val();
    params['StartDate'] = $('#js_s_input_modified_from').val();
    params['EndDate'] = $('#js_s_input_modified_to').val();
    $.blockUI({ message: "<h1>查询中，请稍后...</h1>" });
    $.post(url, params, function (data) {
        var dynamicName = data.columnList;
        $.each(dynamicName, function (i, item) {
            $('#js_rpt_equip_tr').append("<th>" + item + "</th>");
            var obj = {};
            obj['data'] = item;
            obj['className'] = "min-col-xs text-right";
            columns.push(obj);
        });
        var dt = $('#js_equip_datatable').DataTable({
            destroy: true,
            scrollX: true,
            scrollY: 500,
            data: data.data,
            columns: columns
        });
        new $.fn.dataTable.FixedColumns(dt, { "iLeftColumns": 3 });
        $('#js_rpt_equip_tr th').removeClass('text-right')
        $.unblockUI();
    });
}