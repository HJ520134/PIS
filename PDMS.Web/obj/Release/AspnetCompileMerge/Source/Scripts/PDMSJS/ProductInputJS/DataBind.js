//公用方法--------创建“输入接收的返工数”界面


function CreateRepairPage(rowData, selectDate, selectTime) {
    var dynamicModel = $('#js_repair_modal').html();
    var dynamicDiv = '<div class="modal fade" id="{0}" role="dialog">{1}</div>';
    var dynamicId = 'js_repair_modal_' + rowData.FlowChart_Detail_UID;
    dynamicDiv = dynamicDiv.replace('{0}', dynamicId);
    dynamicDiv = dynamicDiv.replace('{1}', dynamicModel);
    
    var $model = $(dynamicDiv);
    var detailHtml = '<input type="hidden" id="js_repair_detail_uid" value=' + rowData.FlowChart_Detail_UID; + '>';
    var productHtml = '<input type="hidden" id="js_repair_product_uid" value=' + rowData.Product_UID + '>';
    $model.find('#js_repair_row').append(detailHtml + productHtml);

    rowData.NewInfo_RepairList.forEach(function (a) {
        if (a.ProductRepairInfoVM == null || a.ProductRepairInfoVM.length == 0) {
            for (var i = 0; i <= 1; i++) {
                var controlHTML = '<div class="form-group col-xs-12 col-md-6 SetReworkInputDisplay"> ';
                controlHTML += '<label class="col-xs-8 control-label">从' + a.JoinName + '{0}</label>';
                controlHTML += '<div class="col-xs-4">';
                controlHTML += '<input type="text"';
                //controlHTML += ' data-Product_UID=' + item.Product_UID;
                controlHTML += ' data-FlowChart_Detail_UID=' + rowData.FlowChart_Detail_UID;
                controlHTML += ' data-Opposite_Detail_UID=' + a.ReworkDetailUID;
                controlHTML += ' data-Rework_Type="{1}"';
                controlHTML += ' class="form-control input-xs rework_text" value="0">';
                controlHTML += '</div></div>';

                if (i == 0) {
                    controlHTML = controlHTML.replace('{0}', '入');
                    controlHTML = controlHTML.replace('{1}', 'Input');
                    $model.find('#js_repair_row').append(controlHTML);
                }
                else {
                    controlHTML = controlHTML.replace('{0}', '出');
                    controlHTML = controlHTML.replace('{1}', 'Output');
                    $model.find('#js_repair_row').append(controlHTML);
                }
            }
        }
        else { //编辑
            for (var i = 0; i < a.ProductRepairInfoVM.length; i++) {
                var item = a.ProductRepairInfoVM[i];
                var controlHTML = '<div class="form-group col-xs-12 col-md-6 SetReworkInputDisplay"> ';
                controlHTML += '<label class="col-xs-8 control-label"  for="' + item.Rework_UID + '">从' + a.JoinName + '{0}</label>';
                controlHTML += '<div class="col-xs-4">';
                controlHTML += '<input type="text" {1} ';
                controlHTML += ' data-Product_UID=' + item.Product_UID;
                controlHTML += ' data-FlowChart_Detail_UID=' + item.FlowChart_Detail_UID;
                controlHTML += ' data-Opposite_Detail_UID=' + item.Opposite_Detail_UID;
                controlHTML += ' data-Rework_Type=' + item.Rework_Type;
                controlHTML += ' class="form-control input-xs rework_text" value="' + item.Opposite_QTY + '"  id="' + item.Rework_UID + '" name="' + item.Rework_UID + '">';
                controlHTML += '</div></div>';

                if (a.ReworkDetailUID == item.Opposite_Detail_UID && item.Rework_Type == 'Input') {
                    controlHTML = controlHTML.replace('{0}', '入');
                    if (item.Is_Match == 1) {
                        controlHTML = controlHTML.replace('{1}', "");
                    }
                    else {
                        controlHTML = controlHTML.replace('{1}', "style='background-color: red;'");
                    }

                    $model.find('#js_repair_row').append(controlHTML);


                }
                else if (a.ReworkDetailUID == item.Opposite_Detail_UID && item.Rework_Type == 'Output') {
                    controlHTML = controlHTML.replace('{0}', '出');
                    if (item.Is_Match == 1) {
                        controlHTML = controlHTML.replace('{1}', "");
                    }
                    else {
                        controlHTML = controlHTML.replace('{1}', "style='background-color: red;'");
                    }

                    $model.find('#js_repair_row').append(controlHTML);

                }

            }
        }

    });

    $("body").append($model.get(0));
}


//function createReworkPage(detail_Uid, product_Uid, Is_Match, selectDate, selectTime) {
//    $("#js_repair_row").empty();
//    var detailHtml = '<input type="hidden" id="js_repair_detail_uid" value=' + detail_Uid + '>';
//    var productHtml = '<input type="hidden" id="js_repair_product_uid" value=' + product_Uid + '>';
//    $("#js_repair_row").append(detailHtml + productHtml);


//    //开始动态创建Repair输入框
//    var param = {};
//    param["Detail_UID"] = detail_Uid;
//    param["Product_UID"] = product_Uid;
//    param["selectDate"] = selectDate;
//    param["selectTime"] = selectTime;

//    $.post(url, param, function (data) {
//        if (data != "") {
//            var result = '';

//            $.each(data, function (i, item) {
//                var temp = item.split('_');
//                var process = temp[0];
//                var color = temp[1];
//                var detail_Uid = temp[2];
//                var rework_Uid = temp[3];
//                var rework_Qty = temp[4];
//                var is_Match = temp[5];
//                var rework_type = temp[6];
//                var controlHTML = '<div class="form-group col-xs-12 col-md-6 SetRepairInputDisplay" {1}> <label class="col-xs-5 control-label"  for="' + rework_Uid + '">' + process + color + '-{0}：</label>' +
//    '<div class="col-xs-7"> <input type="text" {2} class="form-control input-xs rework_text" value="' + rework_Qty + '"  id="' + rework_Uid + '_' + detail_Uid + '" name="{3}">' + 
//    '</div></div>';
                
//                if (rework_type == "") { //新增
//                    controlHTML = controlHTML.replace('{1}', '');
//                    controlHTML = controlHTML.replace('{2}', '');
//                    for (var i = 0; i <= 1; i++) {
//                        if (i == 0) {
//                            result = controlHTML.replace('{0}', '入');
//                            result = result.replace('{3}', process + '_入');
//                        }
//                        else {
//                            result = controlHTML.replace('{0}', '出');
//                            result = result.replace('{3}', process + '_出');
//                        }
                        
//                        $("#js_repair_row").append(result);
//                    }
//                }
//                else if (rework_type == "Input") { //编辑
//                    result = controlHTML.replace('{0}', '入');
//                    result = result.replace('{3}', process + '_入');
//                    result = result.replace('{1}', '');
//                    if (is_Match == '1') {
//                        result = result.replace('{2}', "");
//                    }
//                    else {
//                        result = result.replace('{2}', "style='background-color: red;'");
//                    }

//                    $("#js_repair_row").append(result);
//                }
//                else if (rework_type == "Output") {
//                    result = controlHTML.replace('{0}', '出');
//                    result = result.replace('{3}', process + '_出');
//                    result = result.replace('{1}', '');
//                    if (is_Match == '1') {
//                        result = result.replace('{2}', "");
//                    }
//                    else {
//                        result = result.replace('{2}', "style='background-color: red;'");
//                    }
                    
//                    $("#js_repair_row").append(result);
//                }
//            });
//        }
//    });
//    //$('#js_repair_modal').modal('show');
//};