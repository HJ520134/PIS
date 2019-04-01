
//#region ------ 查询条件的五级联动
$("#js_s_input_Plant").change(function ff() {
    GetOPType();
});

$("#js_s_input_OPType").change(function ff() {
    GetProject();
});

$("#js_s_input_project").change(function ff() {
    GetPartType();
});

$("#js_s_input_Part_Type").change(function ff() {
    QueryColor();
});
$("#js_s_input_FunPlant").change(function ff() {
    QueryColor();
});
$("#js_s_input_date").change(function ff() {
    QueryColor();
});

$("#js_s_input_MaterialType").change(function ff() {
    QueryColor();
});



function QueryPlant() {
    var url = '../Quality/QueryPlant';
    $.post(url, function (data) {
        if (data != "") {
            $("#js_s_input_Plant").empty();
            $.each(data, function (i, item) {
                $("<option></option>")
                    .val(item.Organization_UID)
                    .text(item.Name)
                    .appendTo($("#js_s_input_Plant"));
            });
            GetOPType();
        }
    });
}

function GetOPType() {
    var Plant_OrganizationUID = $("#js_s_input_Plant").val();
    var url = '../Quality/QueryOPType';
    $.post(url, { Plant_OrganizationUID: Plant_OrganizationUID }, function (data) {
        if (data != "") {
            $("#js_s_input_OPType").empty();
            $.each(data, function (i, item) {
                $("<option></option>")
                    .val(item.Organization_UID)
                    .text(item.Name)
                    .appendTo($("#js_s_input_OPType"));
            });
            GetProject();
        }
    });
}

function GetProject() {
    var OPType_Organization_UID = $("#js_s_input_OPType").val();
    var url = '../Quality/GetQAProject';
    $.post(url, { OPType_Organization_UID: OPType_Organization_UID }, function (data) {
        if (data != "") {
            var mType = data.MaterielType;
            var project = data.Project;

            $("#js_s_input_project").empty();
            $.each(project, function (i, item) {
                $("<option></option>")
                    .val(item.Project_UID)
                    .text(item.ProjectName)
                    .appendTo($("#js_s_input_project"));
            });

            $("#js_s_input_MaterialType").empty();
            $.each(mType, function (i, item) {
                $("<option></option>")
                    .val(item)
                    .text(item)
                    .appendTo($("#js_s_input_MaterialType"));
            });
            GetPartType();
        }
    });
}

function GetPartType() {
    $("#js_s_input_Part_Type").empty();
    var Project_UID = $("#js_s_input_project").val();
    var url = '../Quality/GetPartType';
    $.post(url, { Project_UID: Project_UID }, function (data) {
        if (data != "") {
            $.each(data, function (i, item) {
                $("<option></option>")
                    .val(item.FlowChart_Master_UID)
                    .text(item.Part_Type)
                    .appendTo($("#js_s_input_Part_Type"));
            });
        }
        QueryColor();
    });

}

function QueryColor() {
    $("#js_s_input_color").empty();
    var fun_plant = $("#js_s_input_FunPlant").val();
    var Flowchart_Master_UID = $("#js_s_input_Part_Type").find("option:selected").val();
    var productDate = $("#js_s_input_date").val();
    var MaterialType = $("#js_s_input_MaterialType").val();
    if (fun_plant == null || fun_plant == "") {
        fun_plant = 'ALL';
    }
    if (productDate == null || productDate == "") {
        return;
    }
    if (Flowchart_Master_UID == null || Flowchart_Master_UID == "") {
        return;
    }
    var url = '../Quality/QueryRecordColor';
    $.post(url, { Flowchart_Master_UID: Flowchart_Master_UID, FunPlant: fun_plant, ProductDate: productDate, MaterialType: MaterialType }, function (data) {
        if (data != "") {
            $.each(data, function (i, item) {
                $("<option></option>")
                    .val(item)
                    .text(item)
                    .appendTo($("#js_s_input_color"));
            });
        }
        GetProcess();
    });

}

function GetInterval() {
    $("#js_s_input_interval_time").empty();
    var CurrentOp = $("#js_s_input_OPType").text();
    var url = '../EventReportManager/GetIntervalTime'
    if (CurrentOp != '') {
        $.ajax({
            url: url,
            async: false,
            data: { "OP": CurrentOp, "PageName": "ProductReport" },
            success: function (data) {
                if (data != "") {
                    $.each(data, function (i, item) {
                        $('<option></option>').val(item["Enum_Value"]).text(item["Enum_Value"]).appendTo($("#js_s_input_interval_time"));
                    });
                } else {
                    PDMS.Utility.MessageBox.error(data);
                }
            }
        });
    }
}

function GetProcess() {
    var Product_Date = $("#js_s_input_date").val();
    if (Product_Date == "") {
        return;
    }
    var Flowchart_Master_UID = $('#js_s_input_Part_Type').val();

    $("#js_s_input_process").empty();
    var FunPlant = $('#js_s_input_FunPlant').val();
    var url = '../Quality/GetProcessSource';
    var Color = $("#js_s_input_color").val();
    $.post(url, { Flowchart_Master_UID: Flowchart_Master_UID, FunPlant: FunPlant, Product_Date: Product_Date, Color: Color }, function (data) {
        if (data != "") {
            $.each(data, function (i, item) {
                $("<option></option>")
                    .val(item.FlowChart_Detail_UID)
                    .text(item.ProcessName)
                    .appendTo($("#js_s_input_process"));
            });
        }
    });
}

function GetProcessBasicInfo() {
    var process_Seq = $("#js_s_input_process").val();
    var FlowChart_Master_UID = $("#js_s_input_Part_Type").val();
    var url = '../Quality/QueryConditions';
    $.post(url, { "FlowChart_Master_UID": FlowChart_Master_UID }, function (data) {
        if (data != "") {
            var place = data.Place;
            var mtype = data.MaterialType;
            if (place != "") {
                $("#js_s_input_place").empty();
                $.each(place, function (i, item) {
                    $("<option></option>")
                        .val(item)
                        .text(item)
                        .appendTo($("#js_s_input_place"));
                });
            }

        }
    });
}

//#endregion
