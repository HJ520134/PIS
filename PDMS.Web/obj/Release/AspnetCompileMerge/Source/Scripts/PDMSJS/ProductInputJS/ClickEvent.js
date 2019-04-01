
//获取当前专案所有功能厂

function GetUnacommpolished_Reason(Unacommpolished_ReasonList) {
    $("#s_input_Unacommpolished_Reason").empty();
        if (Unacommpolished_ReasonList != "") {
           
            $.each(Unacommpolished_ReasonList, function (i, item) {
                $("<option></option>")
                    .val(item)
                    .text(item)
                    .appendTo($("#s_input_Unacommpolished_Reason"));
            });
        }
   
}



function ClickEdit(url, flag,uuid, IsRedDisplay ) {
    $("#inputDiv").remove();
    $("#outputDiv").remove();
    $("#d_button_rework").remove();
    $("#d_button_repair").remove();
    //$("#Rework_UID").remove();
    //var tdItem = $(this).parent().next();

    $.blockUI({ message: "<h1>@ViewBag.Pleasewait</h1>"});
    

    $.post(url, { uuid: uuid, flagStr: flag }, function (data) {
        //-----------设置隐藏字段的值 start----------
        $('#Edit_Product_UID').val(uuid);
        $('#FlowChart_Detail_UID').val(data.FlowChart_Detail_UID);
        $('#ReworkFlag').val(data.Rework_Flag);
        //-----------设置隐藏字段的值 end----------
        $.unblockUI();

        GetUnacommpolished_Reason(Unacommpolished_ReasonList);

        $("#s_input_Unacommpolished_Reason").val(data.Unacommpolished_Reason);
        //setTimeout($.unblockUI, 6000);
        $('#js_edit_modal').find('input[name=Location_Flag]').val(data.Location_Flag);
        $('#js_edit_modal').find('input[name=Picking_QTY]').val(data.Picking_QTY);
        $('#js_edit_modal').find('input[name=WH_Picking_QTY]').val(data.WH_Picking_QTY);
        //$('#js_edit_modal').find('input[name=Good_QTY]').val(data.Good_QTY);
        $('#js_edit_modal').find('input[name=Adjust_QTY]').val(data.Adjust_QTY);
        //$('#js_edit_modal').find('input[name=NG_QTY]').val(data.NG_QTY);
        $('#js_edit_modal').find('input[name=WH_QTY]').val(data.WH_QTY);
        $('#js_edit_modal').find('input[name=NullWip_QTY]').val(data.NullWip_QTY);
        //Q报表数据准备
        $('#js_edit_modal').find('input[name=Normal_Good_QTY]').val(data.Normal_Good_QTY);
        $('#js_edit_modal').find('input[name=Abnormal_Good_QTY]').val(data.Abnormal_Good_QTY);
        $('#js_edit_modal').find('input[name=Normal_NG_QTY]').val(data.Normal_NG_QTY);
        $('#js_edit_modal').find('input[name=Abnormal_NG_QTY]').val(data.Abnormal_NG_QTY);
        $('#js_edit_modal').modal('show');

        if (data.Rework_Flag == "Rework") {
            html = '<div class="form-group col-xs-12 col-md-6" id="d_button_rework">' +
            '<label class="col-xs-5 control-label" for="s_button_rework">返修接收数_Rework</label>' +
            '<div class="col-xs-7">' +
            '<input type="button" {0} class="form-control btn btn-block btn-info" id="s_button_rework" value="点击输入明细" name="Rework_Btn">' +
            '</div>' +
            '</div>';
            if (IsRedDisplay == "true") {
                html = html.replace('{0}', "style='background-color: red;'");
            }
            else {
                html = html.replace('{0}', "");
            }
            $("#js_edit_row").append(html);
            
        }
        else if (data.Rework_Flag == "Repair") {
            html = '<div class="form-group col-xs-12 col-md-6" id="d_button_repair">' +
            '<label class="col-xs-5 control-label" for="s_button_repair">返修接收数_Repair</label>' +
            '<div class="col-xs-7">' +
            '<input type="button" {0} class="form-control btn btn-block btn-info" id="s_button_repair" value="点击输入明细" name="Repair_Btn">' +
            '</div>' +
            '</div>';

            if (IsRedDisplay == "true") {
                html = html.replace('{0}', "style='background-color: red;'");
            }
            else {
                html = html.replace('{0}', "");
            }

            $("#js_edit_row").append(html);
        }
    });
};


$('body').on('click', '#s_button_rework', function () {
    var detail_Uid = $('#FlowChart_Detail_UID').val();
    var product_Uid = $('#Edit_Product_UID').val();
    var id = 'js_rework_modal_' + detail_Uid;
    $('#' + id).modal('show');
});

$('body').on('click', '#s_button_repair', function () {
    var detail_Uid = $('#FlowChart_Detail_UID').val();
    var product_Uid = $('#Edit_Product_UID').val();
    var id = 'js_repair_modal_' + detail_Uid;
    $('#' + id).modal('show');
});


