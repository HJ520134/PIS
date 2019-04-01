$('#js_btn_add').click(function () {
    $('#js_edit_modal').find(".modal-title").html('新增');
    RefreshOPTypesQueryByEdit();

    $('#js_s_Is_Complated_edit').val('N');
    $('#js_input_Del_Flag_edit').val('N');

    subTable = $('#js_Purchase_datatable_detail').DataTable({
        columns: GetDetailColumn(),
        ordering: false,
        data: [],
        destroy: true
    });

    //加载治具图号下拉框信息
    RefreshFixtureNoByEdit();
    BindVendorInfo();
    SetStatus();
    $('#js_edit_modal').modal('show');
});

$('#js_s_Fixture_No_edit').change(function () {
    var uid = $('#js_s_Fixture_No_edit').val();
    $('#js_s_input_part_id').empty();
    $('#js_s_input_part_id').html('<option></option>');

    if (uid.length > 0) {
        var url = '../FixturePart/GetFixturePartByMUID';
        $.post(url, { UID: uid }, function (data) {
            var obj = jQuery.parseJSON(data);
            for (var i = 0; i < obj.length; i++) {
                $('#js_s_input_part_id').append('<option value="' + obj[i].Fixture_Part_UID + '_' + obj[i].Part_ID + '_' + obj[i].Part_Name + '_' + obj[i].Part_Spec + '">' + obj[i].Part_ID + '_' + obj[i].Part_Name + '</option>');
            }
            $('#js_s_input_part_id').selectpicker('refresh');

        });
    }
});

$('#js_btn_newpart').click(function () {
    if ($('#js_s_input_group_edit').val() == '') {
        PDMS.Utility.MessageBox.info("请输入OP类型");
        return false;
    }

    var partInfo = $('#js_s_input_part_id').val();
    if (partInfo == "" || partInfo == null || partInfo == undefined) {
        PDMS.Utility.MessageBox.info('请选择配件料号进行添加');
        return false;
    }

    var isValid = true;
    var errorMessage = "";
    var Fixture_Part_Order_M_UID = $('#hidFixture_Part_Order_M_UID').val();
    //验证已有的数据是否填写完整
    FIXTUREPART_ONEDATA().each(function (item) {
        if (item.Price == "" || item.Price == undefined || item.Price == 0) {
            errorMessage += '请完成采购单配件明细区第 #' + item.index + "行[采购单价];  ";
            isValid = false;
        }
        if (item.Qty == "" || item.Qty == undefined || item.Qty == 0) {
            errorMessage += '请完成采购单配件明细区第 #' + item.index + "行[采购数量];  ";
            isValid = false;
        }
        if (item.Forcast_Complation_Date == "" || item.Forcast_Complation_Date == undefined) {
            errorMessage += '请完成采购单配件明细区第 #' + item.index + "行[预计完成日期];  ";
            isValid = false;
        }

    });

    if (!isValid) {
        if (errorMessage != "") {
            PDMS.Utility.MessageBox.info(errorMessage);
        }
        return false;
    }

    var valueStr = partInfo.split('_');

    //检查添加的配件是否已经在临时表存在
    var hasItem = FIXTUREPART_ONEDATA({ Fixture_Part_UID: parseInt(valueStr[0]) }).first();
    if (hasItem) {
        PDMS.Utility.MessageBox.info('新添加的配件已经在明细表里存在');
        return false;
    }


    if (partInfo != null && partInfo.length > 0) {
        var index = 1;
        //如果不是第一笔数据则取最大值加1
        if (FIXTUREPART_ONEDATA().last()) {
            index = FIXTUREPART_ONEDATA().last().index + 1;
        }

        var paramItem = {};
        paramItem["index"] = index;
        paramItem["Fixture_Part_Order_D_UID"] = 0;
        paramItem["Fixture_Part_Order_M_UID"] = parseInt(Fixture_Part_Order_M_UID);
        paramItem["Fixture_Part_UID"] = parseInt(valueStr[0]);
        paramItem["Part_ID"] = valueStr[1];
        paramItem["Part_Name"] = valueStr[2];
        paramItem["Part_Spec"] = valueStr[3];
        paramItem["Price"] = '';
        paramItem["Qty"] = '';
        paramItem["Actual_Receive_Qty"] = 0;
        paramItem["Forcast_Complation_Date"] = '';
        paramItem["Del_Flag"] = false;


        paramItem["Vendor_Info_UID"] = 0;
        paramItem["Vendor_ID"] = 0;
        paramItem["Vendor_Name"] = 0;
        paramItem["SumActualQty"] = 0;

        //获取供应商下拉列表数据绑定----------start
        paramItem["VendorInfoList"] = GetVenderInfo();


        FIXTUREPART_ONEDATA.insert({
            index: index, Fixture_Part_Order_D_UID: paramItem.Fixture_Part_Order_D_UID, Fixture_Part_Order_M_UID: paramItem.Fixture_Part_Order_M_UID, Fixture_Part_UID: paramItem.Fixture_Part_UID, Part_ID: paramItem.Part_ID,
            Part_Name: paramItem.Part_Name,
            Part_Spec: paramItem.Part_Spec, Vendor_Info_UID: paramItem.Vendor_Info_UID, Forcast_Complation_Date: paramItem.Forcast_Complation_Date, Price: paramItem.Price, Qty: paramItem.Qty, SumActualQty: paramItem.SumActualQty
        });



        //获取供应商下拉列表数据绑定----------end

        //给明细表动态添加行
        var row = subTable.row.add(paramItem).draw();
        //渲染datetimepicker 效果
        //var complationDateTime = $("#js_Purchase_datatable_detail input[name=Forcast_Complation_Date]");
        //complationDateTime.datetimepicker({ format: 'yyyy-mm-dd', minView: 2, autoclose: true });

    }

});

//明细表选择事件触发第三级数据绑定
$('#js_Purchase_datatable_detail').on('click', 'tr', function () {
    var $sender = $(this);
    if (!$sender.hasClass('selected')) {
        $('#js_Purchase_datatable_detail tr.selected').removeClass('selected');
        $sender.addClass('selected');

        //第三级数据绑定
        var tr = $(this).closest('tr');
        var row = subTable.row(tr).data();
        if (row != undefined) { //row = undefined选择了表头或表尾
            var oneItem = FIXTUREPART_ONEDATA({ index: row.index }).first();
            if (oneItem) { //oneItem = false说明没有数据
                BindThreeData(oneItem);
            }

        }
    }
});

//主表删除事件（主表单笔删除同时删除子表多笔记录）
$('#js_btn_deletepart').on('click', function () {
    var checkedItems = $('#js_Purchase_datatable_detail .js-checkbox-item:checked');
    if (checkedItems.length == 0) {
        PDMS.Utility.MessageBox.info("请选择需要删除的行。");
        return false;
    }
    else {
        checkedItems.each(function (index, checkbox) {
            var tr = checkbox.closest('tr');
            var td = $(tr).find("td")[1];
            var index = parseInt(td.textContent);

            FIXTUREPART_TWODATA({ mIndex: index }).remove();
            FIXTUREPART_ONEDATA({ index: index }).remove();

            //subTable.row(tr).remove().draw();
        });

        //重新排列序号
        ReSortList();

        var params = [];
        FIXTUREPART_ONEDATA().each(function (dto) {
            var paramItem = dto;
            paramItem["VendorInfoList"] = GetVenderInfo();
            params.push(paramItem);
        });
        //重新绑定明细表不能用Table.row(tr).remove().draw()这种方法，index不会重新绑定
        subTable = $('#js_Purchase_datatable_detail').DataTable({
            columns: GetDetailColumn(),
            ordering: false,
            data: params,
            destroy: true
        });


        //渲染datetimepicker 效果
        var complationDateTime = $("#js_Purchase_datatable_detail input[name=Forcast_Complation_Date]");
        complationDateTime.datetimepicker({ format: 'yyyy-mm-dd', minView: 2, autoclose: true });


        //传一个空值给三级表绑定
        BindThreeData(FIXTUREPART_ONEDATA({ index: -1 }));
    }
});

//从表删除事件
$('#js_btn_deleteSub').on('click', function () {
    var $masterTr = $('#js_Purchase_datatable_detail tr.selected');
    var masterRow = subTable.row($masterTr).data();
    if (masterRow == undefined) {
        PDMS.Utility.MessageBox.info("请选中采购单配件明细区的某一行进行删除。");
        return;
    }

    var checkedItems = $('#js_Purchase_datatable_detail_sub .js-checkbox-item:checked');
    if (checkedItems.length == 0) {
        PDMS.Utility.MessageBox.info("请选择需要删除的行。");
        return;
    }
    else {

        checkedItems.each(function (index, checkbox) {

            var tr = checkbox.closest('tr');
            var td = $(tr).find("td")[1];
            var index = parseInt(td.textContent);

            FIXTUREPART_TWODATA({ dIndex: index, mIndex: masterRow.index }).remove();
            //var threeTable = GetThreeTable();
            //threeTable.row(tr).remove().draw();
        });

        //重新排列序号
        ReSortTwoList();

        var params = [];
        FIXTUREPART_TWODATA({ mIndex: masterRow.index }).each(function (dto) {
            var paramItem = dto;
            params.push(paramItem);
        });
        //重新绑定明细表不能用Table.row(tr).remove().draw()这种方法，index不会重新绑定
        subTwoTable = $('#js_Purchase_datatable_detail_sub').DataTable({
            columns: GetSubColumn(),
            ordering: false,
            data: params,
            destroy: true
        });


        ////渲染datetimepicker 效果
        //var subcomplationDateTime = $("#js_Purchase_datatable_detail_sub input[name=Receive_Date]");
        //subcomplationDateTime.datetimepicker({
        //    format: 'yyyy-mm-dd', minView: 2, autoclose: true
        //});

    }

    //预计交货日期
    var receive_Del_complationDateTime = $("#js_Purchase_datatable_detail_sub input[name=Receive_Date]");
    //预计交货量
    var receive_Del_Qty = $("#js_Purchase_datatable_detail_sub input[name=Forcast_Receive_Qty]");
    //实际交货量
    var actual_Del_ReceiveQty = $("#js_Purchase_datatable_detail_sub input[name=Actual_Receive_Qty]");
    receive_Del_complationDateTime.attr('readonly', 'readonly');
    receive_Del_Qty.attr('readonly', 'readonly');
    actual_Del_ReceiveQty.attr('readonly', 'readonly');
});



//新增交货明细按钮
$('#js_btn_newSub').click(function () {
    //判断主表有没有被选中
    var index = GetSelectedMasterIndex();
    if (index == -1) {
        PDMS.Utility.MessageBox.info('请选择采购单配件明细区的某一行进行添加');
        return false;
    }
    else {
        var isValid = true;
        var errorMessage = "";
        //验证已有的数据是否填写完整
        FIXTUREPART_TWODATA().each(function (item) {
            if (item.Receive_Date == "" || item.Receive_Date == null || item.Receive_Date == undefined) {
                errorMessage += '请完成采购单交货明细区第 #' + item.dIndex + "行[预计交货日期];  ";
                isValid = false;
            }
            if (item.Forcast_Receive_Qty == "" || item.Forcast_Receive_Qty == null || item.Forcast_Receive_Qty == undefined) {
                errorMessage += '请完成采购单交货明细区第 #' + item.dIndex + "行[预计交货量];  ";
                isValid = false;
            }
        });

        if (!isValid) {
            if (errorMessage != "") {
                PDMS.Utility.MessageBox.info(errorMessage);
            }
            return false;
        }

        var masterItem = FIXTUREPART_ONEDATA({ index: index }).first();
        var dIndex = 1;

        var twoSelectRows = FIXTUREPART_TWODATA({ mIndex: masterItem.index });

        if (twoSelectRows.last()) {
            dIndex = twoSelectRows.last().dIndex + 1;
        }
        var paramItem = {
        };
        paramItem["dIndex"] = dIndex;
        paramItem["mIndex"] = masterItem.index;
        paramItem["Fixture_Part_Order_Schedule_UID"] = '';
        paramItem["Fixture_Part_Order_D_UID"] = masterItem.Fixture_Part_Order_D_UID;
        paramItem["Receive_Date"] = '';
        paramItem["Forcast_Receive_Qty"] = '';
        paramItem["Actual_Receive_Qty"] = '';
        paramItem["Is_Complated"] = false;
        paramItem["Del_Flag"] = false;

        //给明细表动态添加行
        var threeTable = GetThreeTable();
        threeTable.row.add(paramItem).draw();

        //渲染datetimepicker 效果
        var complationDateTime = $("#js_Purchase_datatable_detail_sub input[name=Receive_Date]");
        complationDateTime.datetimepicker({ format: 'yyyy-mm-dd', minView: 2, autoclose: true });

        //var lastItem = FIXTUREPART_TWODATA().last();
        FIXTUREPART_TWODATA.insert({
            dIndex: dIndex, mIndex: masterItem.index, Fixture_Part_Order_Schedule_UID: '', Fixture_Part_Order_D_UID: paramItem.Fixture_Part_Order_D_UID, Receive_Date: paramItem.Receive_Date, Forcast_Receive_Qty: paramItem.Forcast_Receive_Qty, Actual_Receive_Qty: paramItem.Actual_Receive_Qty, Is_Complated: paramItem.Is_Complated, Del_Flag: paramItem.Del_Flag
        });

        SetStatus();
        //预计交货日期
        //var receive_complationDateTime2 = $("#js_Purchase_datatable_detail_sub input[name=Receive_Date]");
        //receive_complationDateTime2.last();
        ////预计交货量
        //var receiveQty2 = $("#js_Purchase_datatable_detail_sub input[name=Forcast_Receive_Qty]");
        //receive_complationDateTime2.datetimepicker({ format: 'yyyy-mm-dd', minView: 2, autoclose: true });
        //receive_complationDateTime2.removeAttr('readonly');
        //receiveQty2.removeAttr('readonly');

        //$("#js_Purchase_datatable_detail_sub tr").each(function () {
        //    $(this).find("td:first").css({ color: "red", fontWeight: "bold" });
        //});

        $("#js_Purchase_datatable_detail_sub tr:last").find("input[name=Receive_Date]").removeAttr('readonly');
        $("#js_Purchase_datatable_detail_sub tr:last").find("input[name=Forcast_Receive_Qty]").removeAttr('readonly');

        ////预计交货日期
        //var receive_complationDateTime = $("#js_Purchase_datatable_detail_sub input[name=Receive_Date]");
        ////预计交货量
        //var receiveQty = $("#js_Purchase_datatable_detail_sub input[name=Forcast_Receive_Qty]");

        //receive_complationDateTime.removeClass("disabled");
        //receiveQty.removeClass("disabled");
    }
});


//获取当前选中的治具维修的index
function GetSelectedMasterIndex() {
    var selectedRow = $('#js_Purchase_datatable_detail tr.selected');
    if (selectedRow.length == 0) {
        return -1;
    } else {
        var indexTd = selectedRow.find("td")[1];
        if (indexTd == undefined) { // 选中表头不让添加
            return -1;
        }
        else {
            return parseInt(indexTd.textContent);
        }

        //var indexStr = indexTd.textContent;
        //if (indexStr != "") {
        //    return parseInt(indexStr);
        //} else {
        //    return -1;
        //}
    }
}




//配件明细表中的供应商数据修改时，同步更新临时表数据
var ddlVendorChange = function (sender) {
    var tr = $(sender).closest('tr');
    var td = $(tr).find("td")[1];
    var index = parseInt(td.textContent);
    var uid = parseInt($(sender).val());
    FIXTUREPART_ONEDATA({ index: index }).update({ Vendor_Info_UID: uid });

}

//配件明细表中的价格数据修改时，同步更新临时表数据
var textPriceChange = function (sender) {
    var tr = $(sender).closest('tr');
    var td = $(tr).find("td")[1];
    var index = parseInt(td.textContent);
    var price = parseFloat($(sender).val());
    FIXTUREPART_ONEDATA({ index: index }).update({ Price: price });

}

//配件明细表中的数量数据修改时，同步更新临时表数据
var textQtyChange = function (sender) {
    var tr = $(sender).closest('tr');
    var td = $(tr).find("td")[1];
    var index = parseInt(td.textContent);
    var num = parseFloat($(sender).val());
    FIXTUREPART_ONEDATA({ index: index }).update({ Qty: num });
}

//配件明细表中的预计完成日期数据修改时，同步更新临时表数据
var textForcastDateChange = function (sender) {
    var tr = $(sender).closest('tr');
    var td = $(tr).find("td")[1];
    var index = parseInt(td.textContent);
    var date = $(sender).val();
    FIXTUREPART_ONEDATA({ index: index }).update({ Forcast_Complation_Date: date });

}

//交货明细表预计交货日期修改时，同步更新临时表数据
var textReceiveChange = function (sender) {
    var $masterTr = $('#js_Purchase_datatable_detail tr.selected');
    var masterRow = subTable.row($masterTr).data();

    var tr = $(sender).closest('tr');
    var td = $(tr).find("td")[1];
    var index = parseInt(td.textContent);
    var date = $(sender).val();
    FIXTUREPART_TWODATA({ dIndex: index, mIndex: masterRow.index }).update({ Receive_Date: date });
}

//交货明细表预计交货量修改时，同步更新临时表数据
var textForcastReceiveQtyChange = function (sender) {
    var $masterTr = $('#js_Purchase_datatable_detail tr.selected');
    var masterRow = subTable.row($masterTr).data();

    var tr = $(sender).closest('tr');
    var td = $(tr).find("td")[1];
    var index = parseInt(td.textContent);
    var qty = $(sender).val();
    FIXTUREPART_TWODATA({ dIndex: index, mIndex: masterRow.index }).update({ Forcast_Receive_Qty: qty });

}

var onkeyupkl = function (t)
{
    //sender.value = sender.value.replace(/\D/g, '');
    var num = t.value;
    var re = /^\d*$/;
    if (!re.test(num)) {
        isNaN(parseInt(num)) ? t.value = 0 : t.value = parseInt(num);
    }
}

var onafterpastekl = function (sender) {
    //sender.value = sender.value.replace(/\D/g, '');
}

    //交货明细表实际交货量修改时，同步更新临时表数据
    var textActualReceiveQtyChange = function (sender) {
        var $masterTr = $('#js_Purchase_datatable_detail tr.selected');
        var masterRow = subTable.row($masterTr).data();
        var tr = $(sender).closest('tr');
        var td = $(tr).find("td")[1];
        var index = parseInt(td.textContent);
        var qty = $(sender).val();
        FIXTUREPART_TWODATA({ dIndex: index, mIndex: masterRow.index }).update({ Actual_Receive_Qty: qty });
    }

    //更新交货人
    var textDelivery_Name = function (sender) {
        var $masterTr = $('#js_Purchase_datatable_detail tr.selected');
        var masterRow = subTable.row($masterTr).data();
        var tr = $(sender).closest('tr');
        var td = $(tr).find("td")[1];
        var index = parseInt(td.textContent);
        var Delivery_Name = $(sender).val();
        FIXTUREPART_TWODATA({ dIndex: index, mIndex: masterRow.index }).update({ Delivery_Name: Delivery_Name });
    }

    //更新交货人
    var textDelivery_Date = function (sender) {
        var $masterTr = $('#js_Purchase_datatable_detail tr.selected');
        var masterRow = subTable.row($masterTr).data();
        var tr = $(sender).closest('tr');
        var td = $(tr).find("td")[1];
        var index = parseInt(td.textContent);
        var Delivery_Date = $(sender).val();
        FIXTUREPART_TWODATA({ dIndex: index, mIndex: masterRow.index }).update({ Delivery_Date: Delivery_Date });
    }

    var textDeliveryPeriod_Name = function (sender) {
        var $masterTr = $('#js_Purchase_datatable_detail tr.selected');
        var masterRow = subTable.row($masterTr).data();
        var tr = $(sender).closest('tr');
        var td = $(tr).find("td")[1];
        var index = parseInt(td.textContent);
        var DeliveryPeriod_Name = $(sender).val();
        FIXTUREPART_TWODATA({ dIndex: index, mIndex: masterRow.index }).update({ DeliveryPeriod_Name: DeliveryPeriod_Name });
    }

    var textDeliveryPeriod_Date = function (sender) {
        var $masterTr = $('#js_Purchase_datatable_detail tr.selected');
        var masterRow = subTable.row($masterTr).data();
        var tr = $(sender).closest('tr');
        var td = $(tr).find("td")[1];
        var index = parseInt(td.textContent);
        var DeliveryPeriod_Date = $(sender).val();
        FIXTUREPART_TWODATA({ dIndex: index, mIndex: masterRow.index }).update({ DeliveryPeriod_Date: DeliveryPeriod_Date });
    }


    var ddlDelChange = function (sender) {
        var $masterTr = $('#js_Purchase_datatable_detail tr.selected');
        var masterRow = subTable.row($masterTr).data();

        var tr = $(sender).closest('tr');
        var td = $(tr).find("td")[1];
        var index = parseInt(td.textContent);
        var isComplete = $(sender).val();
        if (isComplete == '1') {
            FIXTUREPART_TWODATA({ dIndex: index, mIndex: masterRow.index }).update({ Is_Complated: true });
        }
        else {
            FIXTUREPART_TWODATA({ dIndex: index, mIndex: masterRow.index }).update({ Is_Complated: false });
        }
    }



    $('#js_btn_saveEdit').click(function () {
        //插入数据前进行数据检查
        var isValid = true;
        var index = 1;

        if ($('#js_s_input_group_edit').val() == '') {
            PDMS.Utility.MessageBox.info("请输入OP类型");
            return false;
        }

        if ($('#js_input_Order_ID_edit').val() == '') {
            PDMS.Utility.MessageBox.info("请输入采购单号");
            return false;
        } else {
            var orderID_Val = $('#js_input_Order_ID_edit').val();
            var orderIDTest = /[^a-z^A-Z^0-9]/;
            if (orderIDTest.test(orderID_Val)) {
                PDMS.Utility.MessageBox.info("采购单号格式输入不正确");
                return false;
            }
        }

        if (FIXTUREPART_ONEDATA().count() == 0) {
            PDMS.Utility.MessageBox.info("请输入采购单配件明细");
            return false;
        }

        //if (FIXTUREPART_TWODATA().count() == 0) {
        //    PDMS.Utility.MessageBox.info("请输入采购单交货明细");
        //    return false;
        //}


        FIXTUREPART_ONEDATA().each(function (dto) {
            var masterQty = dto.Qty;//采购数量
            index = dto.index;
            var detailQty = 0;
            var sumActuaReceiveQty = 0;//实际交货的总数
            if (!isValid) {
                return false;
            }

            if (dto.Vendor_Info_UID == 0) {
                isValid = false;
                PDMS.Utility.MessageBox.info("请输入采购单配件明细区第" + dto.index + "行供应商");
                return false;
            }

            if (dto.Price == '') {
                isValid = false;
                PDMS.Utility.MessageBox.info("请输入采购单配件明细区第" + dto.index + "行采购单价");
                return false;
            }

            if (dto.Qty == '') {
                isValid = false;
                PDMS.Utility.MessageBox.info("请输入采购单配件明细区第" + dto.index + "行采购数量");
                return false;
            }
            var title = $('#js_edit_modal').find(".modal-title").html();
            if (title == "交期") {
                FIXTUREPART_TWODATA({ mIndex: dto.index }).each(function (detailDto) {
                    if (detailDto.Receive_Date == '') {
                        isValid = false;
                        PDMS.Utility.MessageBox.info("请输入采购单交货明细区第" + detailDto.dIndex + "行预计交货日期");
                        return false;
                    }
                    if (detailDto.Forcast_Receive_Qty == '') {
                        isValid = false;
                        PDMS.Utility.MessageBox.info("请输入采购单交货明细区第" + detailDto.dIndex + "行预计交货量");
                        return false;
                    }

                    var Forcast_Receive_Qty = parseInt(detailDto.Forcast_Receive_Qty)
                    if (Forcast_Receive_Qty > masterQty) {
                        isValid = false;
                        PDMS.Utility.MessageBox.info("采购单配件明细区第" + index + "行预计交货数应小于采购数量");
                        return false;
                    }

                    if (Forcast_Receive_Qty == 0) {
                        isValid = false;
                        PDMS.Utility.MessageBox.info("采购单配件明细区第" + index + "行预计交货数不能为0");
                        return false;
                    }

                    //detailQty += parseInt(detailDto.Forcast_Receive_Qty);
                });

                //if (isValid) {
                //    if (masterQty != detailQty) {
                //        isValid = false;
                //        PDMS.Utility.MessageBox.info("采购单配件明细区第" + index + "行采购数量和预计交货量不一致");
                //        return false;
                //    }
                //}
            }

            if (isValid) {
                if (title == '交货') {
                    FIXTUREPART_TWODATA().each(function (detailDto) {
                        if (detailDto.mIndex == dto.index) {
                            if (detailDto.Actual_Receive_Qty != null && detailDto.Actual_Receive_Qty != "") {
                                sumActuaReceiveQty += parseInt(detailDto.Actual_Receive_Qty)
                            }
                        }

                        //实际交货量应该小于预计交货量
                        if (detailDto.Actual_Receive_Qty > detailDto.Forcast_Receive_Qty) {
                            isValid = false;
                            PDMS.Utility.MessageBox.error("采购单交货明细区：第" + detailDto.dIndex + "行实际交货量大于预计交货量");
                            return false;
                        }
                    });
                    if (sumActuaReceiveQty > masterQty) {
                        isValid = false;
                        PDMS.Utility.MessageBox.error("采购单配件明细区：第" + dto.index + "行实际交货量大于采购数量");
                        return false;
                    }
                }
            }
        });

        var title = $('#js_edit_modal').find(".modal-title").html();

        if (isValid) {
            var params = {};
            if ($('#hidFixture_Part_Order_M_UID').val() == '') { //新增
                params["Fixture_Part_Order_M_UID"] = 0;
                params["Plant_Organization_UID"] = $('#js_s_input_plant_edit').val();
                params["BG_Organization_UID"] = $('#js_s_input_group_edit').val();
                params["FunPlant_Organization_UID"] = $('#js_s_input_FunPlant_edit').val();
                params["Order_ID"] = $('#js_input_Order_ID_edit').val();
                params["Is_Complated"] = false;
                params["Del_Flag"] = false;

            }
            else { //编辑
                params["Fixture_Part_Order_M_UID"] = $('#hidFixture_Part_Order_M_UID').val();
            }
            var orderData = $('#js_s_input_Order_Date_edit').val();

            if (orderData == null || orderData == "") {
                PDMS.Utility.MessageBox.error("请填写订单日期")
            }

            params["Order_Date"] = orderData
            params["Remarks"] = $('#js_input_Remarks_edit').val();
            params["Is_SubmitFlag"] = $('#Is_SubmitFlag').val();//订单提交状态
            params["Is_Submit"] = "";

            if (title == '交货') {
                params["Is_Submit"] = "交货"
            }
            if (title == '交期') {
                params["Is_Submit"] = "交期"
            }

            var detailParams = [];
            FIXTUREPART_ONEDATA().each(function (dto) {
                var detailParam = {};
                detailParam["index"] = dto.index;
                detailParam["Fixture_Part_Order_D_UID"] = dto.Fixture_Part_Order_D_UID;
                detailParam["Fixture_Part_Order_M_UID"] = dto.Fixture_Part_Order_M_UID;
                detailParam["Fixture_Part_UID"] = dto.Fixture_Part_UID;
                detailParam["Vendor_Info_UID"] = dto.Vendor_Info_UID;
                detailParam["Price"] = dto.Price;
                detailParam["Qty"] = dto.Qty;
                detailParam["Forcast_Complation_Date"] = dto.Forcast_Complation_Date;

                detailParams.push(detailParam);
            });
            params["FixturePartOrderDList"] = detailParams;

            var subParams = [];
            FIXTUREPART_TWODATA().each(function (dto) {
                var subParam = {};
                subParam["dIndex"] = dto.dIndex;
                subParam["mIndex"] = dto.mIndex;
                subParam["Fixture_Part_Order_Schedule_UID"] = dto.Fixture_Part_Order_Schedule_UID;
                subParam["Fixture_Part_Order_D_UID"] = dto.Fixture_Part_Order_D_UID;
                subParam["Receive_Date"] = dto.Receive_Date;
                subParam["Forcast_Receive_Qty"] = dto.Forcast_Receive_Qty;
                subParam["Actual_Receive_Qty"] = dto.Actual_Receive_Qty;
                subParam["Is_Complated"] = dto.Is_Complated;
                subParam["Del_Flag"] = dto.Del_Flag;
                subParams.push(subParam);
            });
            params["FixturePartOrderScheduleList"] = subParams;

            var url = '../FixturePart/SaveFixturePartByMUID';
            $.post(url, params, function (data) {
                var obj = jQuery.parseJSON(data);
                if (obj.length > 0) {
                    if (obj == "保存成功" || obj == "提交成功") {
                        $('#js_edit_modal').modal('hide');
                        PDMS.Utility.MessageBox.info(obj);
                        $('#btn_search').trigger('click');
                    } else {
                        PDMS.Utility.MessageBox.error(obj);
                    }
                }
                else {
                    $('#js_edit_modal').modal('hide');
                    $('#js_Config_Submit').modal('hide');
                    //window.location.reload();
                    PDMS.Utility.MessageBox.info("保存成功");
                    $('#btn_search').trigger('click');
                }
            });
        }

    });