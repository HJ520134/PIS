//厂区下拉框改变事件
$("#js_s_input_plant").change(function ff() {
    ClearDropDownListValue('js_s_input_plant');
    GetOpTypes();
});

//OpType下拉框改变事件
$("#js_s_input_group").change(function ff() {
    ClearDropDownListValue('js_s_input_group');
    GetProject();
});

//Project下拉框改变事件
$("#js_s_input_project").change(function ff() {
    ClearDropDownListValue('js_s_input_project');
    GetPartTypes();
});


//获取OpTypes
var GetOpTypes = function () {
    var opTypeCount = 0;
    $("#js_s_input_group").empty();
    var temp = $("#js_s_input_plant").val();
    var url = '../ProductionPlanningReport/GetOpTypesByPlantName';
    $.ajax({
        url: url,
        async: false,
        data:{ "PlantName": temp },
        success: function(data) {
            if (data != "") {
                opTypeCount = data.length;
                $.each(data, function (i, item) {
                    $("<option></option>")
                        .val(i)
                        .text(item)
                        .appendTo($("#js_s_input_group"));
                });
            }
        }
    });
    //如果Project列表不为0，则继续加载下级联动
    if (opTypeCount > 0) {
        //GetProductPhase();
    }
}

//获取Project专案
var GetProject = function () {
    var projectCount = 0;
    $("#js_s_input_project").empty();
    var temp = $("#js_s_input_group").val();
    var url = '../ProductionPlanningReport/GetProjectByOpType';
    $.ajax({
        url: url,
        async: false,
        data:{ "OpTypeUID": temp },
        success: function(data) {
            if (data != "") {
                projectCount = data.length;
                $.each(data, function (i, item) {
                    $("<option></option>")
                        .val(i)
                        .text(item)
                        .appendTo($("#js_s_input_project"));
                });
                GetPartTypes();
            }
        }
    });
    //如果Project列表不为0，则继续加载下级联动
    //if (projectCount > 0) {
        
    //}
}

//下拉框初始化获取部件类型
function GetPartTypes() {
    var partTypesCount = 0;
    $("#js_s_input_parttypes").empty();
    //$("#js_s_input_part_types").append("<option value=''></option>");
    var tempProject = $("#js_s_input_project").val();
    var url = '../ProductionPlanningReport/GetPartTypesByProject';
    $.ajax({
        url: url,
        async: false,
        data: {"ProjectUID": tempProject},
        success: function (data) {
            if (data != "") {
                partTypesCount = data.length;
                $.each(data, function (i, item) {
                    $("<option></option>")
                        .val(i)
                        .text(item)
                        .appendTo($("#js_s_input_parttypes"));
                });
            }
        }
    });

    //ajax同步获取下级联动并加载
    //if (partTypesCount > 0) {
    //    GetColor();
    //}
}


//清空联动下拉框的值
function ClearDropDownListValue(value) {
    switch (value) {
        case 'js_s_input_plant':
            //清空OpType的值
            $("#js_s_input_group").empty();
            $("#js_s_input_group").val('');
            //清空专案
            $("#js_s_input_project").empty();
            $("#js_s_input_project").val('');
            //清空部件类型的值
            $("#js_s_input_parttypes").empty();
            $("#js_s_input_parttypes").val('');
            break;
        case 'js_s_input_group':
            //清空专案
            $("#js_s_input_project").empty();
            $("#js_s_input_project").val('');
            //清空部件类型的值
            $("#js_s_input_parttypes").empty();
            $("#js_s_input_parttypes").val('');
            break;
        case 'js_s_input_project':
            //清空部件类型的值
            $("#js_s_input_parttypes").empty();
            $("#js_s_input_parttypes").val('');
            break;
        case 'js_s_input_parttypes':
            break;
    }
}


var DisplayTable = function () {
    var selectOptype = $('#js_s_input_group').val();
    var selectProject = $('#js_s_input_project').val();
    if (selectOptype == '0') { 
        $('#js_rpt_datatable').show();
        $('#js_rpt_op_datatable').hide();
    }
    if (selectOptype != '0') {
        $('#js_rpt_op_datatable').show();
        $('#js_rpt_datatable').hide();

    }
}