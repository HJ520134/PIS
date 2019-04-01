
//Rework初始化动态页面生成，新增和编辑都是此方法
function CreateReworkPage(rowData, selectDate, selectTime) {
    var dynamicModel = $('#js_rework_modal').html();
    var dynamicDiv = '<div class="modal fade" id="{0}" role="dialog">{1}</div>';
    var dynamicId = 'js_rework_modal_' + rowData.FlowChart_Detail_UID;
    dynamicDiv = dynamicDiv.replace('{0}', dynamicId);
    dynamicDiv = dynamicDiv.replace('{1}', dynamicModel);

    var $model = $(dynamicDiv);
    var detailHtml = '<input type="hidden" id="js_rework_detail_uid" value=' + rowData.FlowChart_Detail_UID + '>';
    var productHtml = '<input type="hidden" id="js_rework_product_uid" value=' + rowData.Product_UID + '>';
    $model.find('#js_rework_row').append(detailHtml + productHtml);

    rowData.NewInfo_ReworkList.forEach(function (a) {
        
        //a.ProductReworkInfoVM.length = 0 新增； >0 编辑
        if (a.ProductReworkInfoVM == null || a.ProductReworkInfoVM.length == 0) { //新增
            for (var i = 0; i <= 1; i++) {
                var controlHTML = '<div class="form-group col-xs-12 col-md-6 SetReworkInputDisplay"> ';
                controlHTML += '<label class="col-xs-8 control-label">从' + a.JoinName + '{0}</label>';
                controlHTML += '<div class="col-xs-4">';
                controlHTML += '<input type="text"';
                controlHTML += ' data-FlowChart_Detail_UID=' + rowData.FlowChart_Detail_UID;
                controlHTML += ' data-Opposite_Detail_UID=' + a.RepairDetailUID;
                controlHTML += ' data-Rework_Type="{1}"';
                controlHTML += ' class="form-control input-xs rework_text" value="0">';
                controlHTML += '</div></div>';

                if (i == 0) {
                    controlHTML = controlHTML.replace('{0}', '入');
                    controlHTML = controlHTML.replace('{1}', 'Input');
                    $model.find('#js_rework_row').append(controlHTML);
                }
                else {
                    controlHTML = controlHTML.replace('{0}', '出');
                    controlHTML = controlHTML.replace('{1}', 'Output');
                    $model.find('#js_rework_row').append(controlHTML);
                }
            }

        }
        else { //编辑
            for (var i = 0; i < a.ProductReworkInfoVM.length; i++) {
                var item = a.ProductReworkInfoVM[i];
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


                if (a.RepairDetailUID == item.Opposite_Detail_UID && item.Rework_Type == 'Input') {
                    controlHTML = controlHTML.replace('{0}', '入');
                    if (item.Is_Match == 1) {
                        controlHTML = controlHTML.replace('{1}', "");
                    }
                    else {
                        controlHTML = controlHTML.replace('{1}', "style='background-color: red;'");
                    }

                    $model.find('#js_rework_row').append(controlHTML);
                }
                else if (a.RepairDetailUID == item.Opposite_Detail_UID && item.Rework_Type == 'Output') {
                    controlHTML = controlHTML.replace('{0}', '出');
                    if (item.Is_Match == 1) {
                        controlHTML = controlHTML.replace('{1}', "");
                    }
                    else {
                        controlHTML = controlHTML.replace('{1}', "style='background-color: red;'");
                    }
                    
                    $model.find('#js_rework_row').append(controlHTML);
                }
            }
        }


    });

    $("body").append($model.get(0));
}


//点击Rework按钮弹出返工数对话框
$('body').on('click', '.rework', function () {
    var tr = $(this).closest('tr');
    var row = table.row(tr).data();
    var detailUID = row.FlowChart_Detail_UID;
    var Product_UID = row.Product_UID;
    var Is_Match = row.Is_Match;

    var id = 'js_rework_modal_' + detailUID;
    $('#' + id).modal('show');

});