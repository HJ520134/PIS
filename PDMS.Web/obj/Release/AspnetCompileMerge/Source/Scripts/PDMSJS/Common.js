
//查询界面厂区是OP类型变化
$('#js_s_input_plant').change(function () {
    RefreshOPTypesQuery();
    RefreshFunPlantQuery();
    RefreshProcessQuery();
    //refreshProductionLineQuery();
    
    //refreshWorkStationQuery();
    //refreshWorkshopQuery();
    //refreshMachineQuery();
    //refreshVendorQuery();
});

$('#js_s_input_plant_edit').change(function () {
    RefreshOPTypesQueryByEdit();
    RefreshFunPlantByEdit();
    RefreshFixtureNoByEdit();
});



$('#js_s_input_group').change(function () {
    RefreshFunPlantQuery();
    RefreshProcessQuery();
    RefreshWorkStationQuery();
    RefreshWorkshopQuery();
    RefreshProductionLineQuery();
    RefreshRepairQuery();
    RefreshFixtureNoQuery();
    RefreshVendorQuery();
});

$('#js_s_input_group_edit').change(function () {
    RefreshFunPlantByEdit();
    RefreshFixtureNoByEdit();
});



//功能厂change(查询)
$('#js_s_input_FunPlant').change(function () {
    RefreshProcessQuery();
    RefreshWorkStationQuery();
    RefreshProductionLineQuery();
    RefreshWorkshopQuery();
    RefreshRepairQuery();
    RefreshFixtureNoQuery();
    RefreshVendorQuery();
});


$('#js_s_input_FunPlant_edit').change(function () {
    RefreshFixtureNoByEdit();
});



//刷新OP类型(查询)
function RefreshOPTypesQuery() {
    //设置OP
    $('#js_s_input_group').empty();
    $('#js_s_input_group').html('<option></option>');
    var url = '../Fixture/GetCurrentOPType';
    $.post(url, { plant_OrganizationUID: $('#js_s_input_plant').val() }, function (data) {
        for (var i = 0; i < data.length; i++) {
            $('#js_s_input_group').append('<option value="' + data[i].Organization_UID + '">' + data[i].OP_TYPES + '</option>');
        }
    });
    $('#js_s_input_group').selectpicker('refresh');

    RefreshProcessQuery();
    RefreshWorkStationQuery();
    RefreshProductionLineQuery();
    RefreshWorkshopQuery();
    RefreshRepairQuery();
    RefreshFixtureNoQuery();
    RefreshVendorQuery();
}


//刷新功能厂(查询)
function RefreshFunPlantQuery() {
    $('#js_s_input_FunPlant').empty();
    $('#js_s_input_FunPlant').html('<option></option>');
    var opTypeUid = $('#js_s_input_group').val();
    if (opTypeUid != "" && opTypeUid != undefined) {
        var url = '../Fixture/GetFunPlantByOPTypes';
        $.post(url, { Optype: opTypeUid }, function (data) {
            for (var i = 0; i < data.length; i++) {
                $('#js_s_input_FunPlant').append('<option value="' + data[i].FunPlant_OrganizationUID + '">' + data[i].FunPlant + '</option>');
            }
        });
    }
    $('#js_s_input_FunPlant').selectpicker('refresh');
}

//刷新制程(查询)
function RefreshProcessQuery() {
    if ($('#js_select_Process_Info_UID_query').length > 0) {
        $('#js_select_Process_Info_UID_query').empty();
        $('#js_select_Process_Info_UID_query').html('<option></option>');
        //设置制程
        var optypeUid = GetOptypeUID();
        var funcPlantUid = GetFuncPlantUID();

        var url = '../Fixture/GetProcess_InfoList';
        $.post(url, { Plant_Organization_UID: $('#js_s_input_plant option:selected').val(), BG_Organization_UID: optypeUid, FunPlant_Organization_UID: funcPlantUid }, function (data) {
            for (var i = 0; i < data.length; i++) {
                $('#js_select_Process_Info_UID_query').append('<option value="' + data[i].Process_Info_UID + '">' + data[i].Process_ID + "_" +data[i].Process_Name + '</option>');
            }
        });
        $('#js_select_Process_Info_UID_query').selectpicker('refresh');
    }
}


//刷新工站(查询)
function RefreshWorkStationQuery() {
    //检查画面上是否有这个下拉框
    if ($('#js_s_input_WorkStation_UID').length > 0) {
        $('#js_s_input_WorkStation_UID').empty();
        $('#js_s_input_WorkStation_UID').html('<option></option>');

        var optypeUid = GetOptypeUID();
        var funcPlantUid = GetFuncPlantUID();
        //设置工站
        var url = '../Fixture/GetWorkstationList';
        $.post(url, { Plant_Organization_UID: $('#js_s_input_plant option:selected').val(), BG_Organization_UID: optypeUid, FunPlant_Organization_UID: funcPlantUid }, function (data) {
            for (var i = 0; i < data.length; i++) {
                $('#js_s_input_WorkStation_UID').append('<option value="' + data[i].WorkStation_UID + '">' + data[i].WorkStation_ID + "_" + data[i].WorkStation_Name + '</option>');
            }
        });

        $('#js_s_input_WorkStation_UID').selectpicker('refresh');
    }
}

//刷新产线(查询)
function RefreshProductionLineQuery() {
    //检查画面上是否有这个下拉框
    if ($('#js_s_input_Line_Name').length > 0) {
        $('#js_s_input_Line_Name').empty();
        $('#js_s_input_Line_Name').html('<option></option>');
        var optypeUid = GetOptypeUID();
        var funcPlantUid = GetFuncPlantUID();
        var url = '../Fixture/GetProductionLineList';
        $.post(url, { Plant_Organization_UID: $('#js_s_input_plant option:selected').val(), BG_Organization_UID: optypeUid, FunPlant_Organization_UID: funcPlantUid }, function (data) {
                for (var i = 0; i < data.length; i++) {
                    $('#js_s_input_Line_Name').append('<option value="' + data[i].Production_Line_UID + '">' + data[i].Line_ID + "_" + data[i].Line_Name + '</option>');
                }
        });
        $('#js_s_input_Line_Name').selectpicker('refresh');
    }
}


//刷新生产地点(查询)
function RefreshWorkshopQuery() {
    //检查画面上是否有这个下拉框
    if ($('#js_s_input_Workshop_UID').length > 0) {
        $('#js_s_input_Workshop_UID').empty();
        $('#js_s_input_Workshop_UID').html('<option></option>');
        var optypeUid = GetOptypeUID();
        var funcPlantUid = GetFuncPlantUID();
        var url = '../Fixture/GetWorkshopList';
        $.post(url, { Plant_Organization_UID: $('#js_s_input_plant option:selected').val(), BG_Organization_UID: optypeUid, FunPlant_Organization_UID: funcPlantUid }, function (data) {
            for (var i = 0; i < data.length; i++) {
                $('#js_s_input_Workshop_UID').append('<option value="' + data[i].Workshop_UID + '">' + data[i].Workshop_ID + "_" + data[i].Workshop_Name + '</option>');
            }
            $('#js_s_input_Workshop_UID').selectpicker('refresh');
        });
    }
}


//刷新维修间
function RefreshRepairQuery() {
    //检查画面上是否有这个下拉框
    if ($('#js_s_input_Location_UID').length > 0) {
        $('#js_s_input_Location_UID').empty();
        $('#js_s_input_Location_UID').html('<option></option>');
        var optypeUid = GetOptypeUID();
        var funcPlantUid = GetFuncPlantUID();
        var url = '../Fixture/GetRepairLoactionList';
        $.post(url, { Plant_Organization_UID: $('#js_s_input_plant option:selected').val(), BG_Organization_UID: optypeUid, FunPlant_Organization_UID: funcPlantUid }, function (data) {
            for (var i = 0; i < data.length; i++) {
                $('#js_s_input_Location_UID').append('<option value="' + data[i].Repair_Location_UID + '">' + data[i].Repair_Location_ID + "_" + data[i].Repair_Location_Name + '</option>');
            }
            $('#js_s_input_Location_UID').selectpicker('refresh');
        });
    }
}

//刷新治具编号下拉框
function RefreshFixtureNoQuery() {
    //检查画面上是否有这个下拉框
    if ($('#js_select_Fixture_NO').length > 0) {
        $('#js_select_Fixture_NO').empty();
        $('#js_select_Fixture_NO').html('<option></option>');
        var optypeUid = GetOptypeUID();
        var funcPlantUid = GetFuncPlantUID();
        var url = '../Fixture/GetFixtureNoList';
        $.post(url, { Plant_Organization_UID: $('#js_s_input_plant option:selected').val(), BG_Organization_UID: optypeUid, FunPlant_Organization_UID: funcPlantUid }, function (data) {
            for (var i = 0; i < data.length; i++) {
                $('#js_select_Fixture_NO').append('<option value="' + data[i] + '">' + data[i] + '</option>');
            }
            $('#js_select_Fixture_NO').selectpicker('refresh');
        });
    }

}


var GetOptypeUID = function () {
    var optypeUid = $('#js_s_input_group').val();

    if (optypeUid == '' || optypeUid == undefined) {
        optypeUid = 0;
    }
    return optypeUid;
}

var GetFuncPlantUID = function () {
    var funcPlantUid = $('#js_s_input_FunPlant').val();

    if (funcPlantUid == "" || funcPlantUid == undefined) {
        funcPlantUid = 0;
    }
    return funcPlantUid;
}




//刷新OP类型(编辑)
function RefreshOPTypesQueryByEdit(plantValue) {
    //设置OP
    $('#js_s_input_group_edit').empty();
    $('#js_s_input_group_edit').html('<option></option>');
    var plantUid = $('#js_s_input_plant_edit').val();
    if (plantValue != undefined) {
        plantUid = plantValue;
    }
    var url = '../Fixture/GetCurrentOPType';
    $.post(url, { plant_OrganizationUID: plantUid }, function (data) {
        for (var i = 0; i < data.length; i++) {
            $('#js_s_input_group_edit').append('<option value="' + data[i].Organization_UID + '">' + data[i].OP_TYPES + '</option>');
        }
    });
    $('#js_s_input_group_edit').selectpicker('refresh');

}


//刷新功能厂(编辑)
function RefreshFunPlantByEdit(optypeValue) {
    $('#js_s_input_FunPlant_edit').empty();
    $('#js_s_input_FunPlant_edit').html('<option></option>');
    var opTypeUid = $('#js_s_input_group_edit').val();
    if (optypeValue != undefined) {
        opTypeUid = optypeValue;
    }
    if (opTypeUid != "" && opTypeUid != undefined) {
        var url = '../Fixture/GetFunPlantByOPTypes';
        $.post(url, { Optype: opTypeUid }, function (data) {
            for (var i = 0; i < data.length; i++) {
                $('#js_s_input_FunPlant_edit').append('<option value="' + data[i].FunPlant_OrganizationUID + '">' + data[i].FunPlant + '</option>');
            }
        });
    }
    $('#js_s_input_FunPlant_edit').selectpicker('refresh');
}

//刷新治具图号的加载（编辑）
function RefreshFixtureNoByEdit() {
    if ($('#js_s_Fixture_No_edit').length > 0) {
        var plantUID = $('#js_s_input_plant_edit').val();
        var optyeUID = $('#js_s_input_group_edit').val();
        if (optyeUID.length == 0) {
            optyeUID = 0;
        }
        var funcUID = $('#js_s_input_FunPlant_edit').val();
        if (funcUID == null || funcUID.length == 0) {
            funcUID = 0;
        }
        var params = {};
        params["PlantUID"] = plantUID;
        params["Optype"] = optyeUID;
        params["FuncUID"] = funcUID;
        //设置默认为空下拉框
        $('#js_s_Fixture_No_edit').empty();
        $('#js_s_Fixture_No_edit').html('<option></option>');
        var url = '../FixturePart/GetFixturePartByPlantOptypeFunc';
        $.post(url, params, function (data) {
            var obj = jQuery.parseJSON(data);
            for (var i = 0; i < obj.length; i++) {
                $('#js_s_Fixture_No_edit').append('<option value="' + obj[i].Fixture_Part_Setting_M_UID + '">' + obj[i].Fixture_NO + '</option>');
            }
            $('#js_s_Fixture_No_edit').selectpicker('refresh');
        });
    }
}


//刷新供应商
function RefreshVendorQuery() {
    //检查画面上是否有这个下拉框
    if ($('#js_s_input_Vendor_Info_UID').length > 0) {
        $('#js_s_input_Vendor_Info_UID').empty();
        $('#js_s_input_Vendor_Info_UID').html('<option></option>');
        var optypeUid = GetOptypeUID();
        var funcPlantUid = GetFuncPlantUID();
        var url = '../Fixture/GetVendorInfoList';
        $.post(url, { Plant_Organization_UID: $('#js_s_input_plant option:selected').val(), BG_Organization_UID: optypeUid, FunPlant_Organization_UID: funcPlantUid }, function (data) {
            for (var i = 0; i < data.length; i++) {
                $('#js_s_input_Vendor_Info_UID').append('<option value="' + data[i].Vendor_Info_UID + '">' + data[i].Vendor_ID + "_" + data[i].Vendor_Name  + '</option>');
            }
            $('#js_s_input_Vendor_Info_UID').selectpicker('refresh');
        });

    }
    
}
