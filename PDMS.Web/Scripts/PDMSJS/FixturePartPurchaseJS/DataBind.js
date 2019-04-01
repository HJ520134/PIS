//根据主键获取信息
var getByUIDUrl = '../FixturePart/QueryPurchaseByUID';

$('body').on("click", ".js-grid-edit", function () {
    $('#js_edit_modal').find(".modal-title").html('编辑');
    var uid = $(this).attr('data-nid');
    //document.getElementById("id_deliveryPeriod_date").style.display = 'none'
    //document.getElementById("id_deliveryperiod_name").style.display = 'none'
    //document.getElementById("id_delivery_name").style.display = 'none'
    //document.getElementById("id_delivery_date").style.display = 'none'
    GetCommonSet(uid);
    //加载治具图号下拉框信息
    RefreshFixtureNoByEdit();
    //SetControlEdit();

    $('#js_edit_modal').modal('show');
});

$('body').on("click", ".js-grid-view", function () {
    $('#js_edit_modal').find(".modal-title").html('查看');
    var uid = $(this).attr('data-nid');
    //document.getElementById("id_deliveryPeriod_date").style.display = ''
    //document.getElementById("id_deliveryperiod_name").style.display = ''
    //document.getElementById("id_delivery_name").style.display = ''
    //document.getElementById("id_delivery_date").style.display = ''
    GetCommonSet(uid);
    //SetControlReadonly();

    //$('#js_edit_modal').find("[type=button]").hide();
    //$('#js_s_input_plant_edit').selectpicker('refresh');
    //$('#js_s_input_plant_edit').show();

    $('#js_edit_modal').modal('show');
});

$('body').on("click", ".js-grid-submit", function () {
    $('#js_edit_modal').find(".modal-title").html('交货');
    var uid = $(this).attr('data-nid');
    GetCommonSet(uid);
    SetStatus();
    $('#js_edit_modal').modal('show');
});

$('body').on("click", ".js-grid-submitDate", function () {
    $('#js_edit_modal').find(".modal-title").html('交期');
    var uid = $(this).attr('data-nid');
    GetCommonSet(uid);
    SetStatus();

    //预计交货日期
    var receive_complationDateTime1 = $("#js_Purchase_datatable_detail_sub input[name=Receive_Date]");
    //预计交货量
    var receiveQty1 = $("#js_Purchase_datatable_detail_sub input[name=Forcast_Receive_Qty]");
    receive_complationDateTime1.attr('readonly', 'readonly');
    receiveQty1.attr('readonly', 'readonly');
    $('#js_edit_modal').modal('show');
});


var GetCommonSet = function (uid) {
    $.post(getByUIDUrl, { Fixture_Part_Order_M_UID: uid }, function (data) {
        $('#hidFixture_Part_Order_M_UID').val(uid);
        var obj = jQuery.parseJSON(data);

        RefreshOPTypesQueryByEdit(obj.Plant_Organization_UID);
        RefreshFunPlantByEdit(obj.BG_Organization_UID);
        //下拉框选择厂区
        $('#js_s_input_plant_edit').selectpicker('refresh');
        $('#js_s_input_plant_edit').selectpicker('val', obj.Plant_Organization_UID);
        //下拉框选择OP
        $('#js_s_input_group_edit').selectpicker('refresh');
        $('#js_s_input_group_edit').selectpicker('val', obj.BG_Organization_UID);
        //下拉框选择功能厂
        $('#js_s_input_FunPlant_edit').selectpicker('refresh');
        $('#js_s_input_FunPlant_edit').selectpicker('val', obj.FunPlant_Organization_UID);

        if (data.Is_Complated) {
            $('#js_s_Is_Complated_edit').val('Y');
        }
        else {
            $('#js_s_Is_Complated_edit').val('N');
        }
        $('#js_input_Order_ID_edit').val(obj.Order_ID);
        $('#js_s_input_Order_Date_edit').val(obj.Order_Date);

        if (obj.Del_Flag) {
            $('#js_input_Del_Flag_edit').val('Y');
        }
        else {
            $('#js_input_Del_Flag_edit').val('N');
        }
        $('#js_input_Remarks_edit').val(obj.Remarks);

        //获取供应商的信息用来绑定动态供应商下拉框
        BindVendorInfo();

        //绑定明细表
        subTable = $('#js_Purchase_datatable_detail').DataTable({
            columns: GetDetailColumn(),
            ordering: false,
            data: obj.FixturePartOrderDList,
            destroy: true
        });

        //渲染datetimepicker 效果
        //var complationDateTime = $("#js_Purchase_datatable_detail input[name=Forcast_Complation_Date]");
        //complationDateTime.datetimepicker({ format: 'yyyy-mm-dd', minView: 2, autoclose: true });

        //如果明细表有数据默认选中第1条第0行是表头
        var firstTr = $('#js_Purchase_datatable_detail tr[role=row]:eq(1)');
        if (firstTr != undefined) {
            firstTr.trigger('click');
        }

        //先把第三张表插入临时表
        $.each(obj.FixturePartOrderScheduleList, function (i, dto) {
            var index = i + 1;
            FIXTUREPART_TWODATA.insert({
                dIndex: index, mIndex: index, Fixture_Part_Order_Schedule_UID: dto.Fixture_Part_Order_Schedule_UID, Fixture_Part_Order_D_UID: dto.Fixture_Part_Order_D_UID, Receive_Date: dto.Receive_Date, Forcast_Receive_Qty: dto.Forcast_Receive_Qty, Actual_Receive_Qty: dto.Actual_Receive_Qty, Is_Complated: dto.Is_Complated, Del_Flag: dto.Del_Flag, DeliveryPeriod_Date: dto.DeliveryPeriod_Date, DeliveryPeriod_Name: dto.DeliveryPeriod_Name, Delivery_Date: dto.Delivery_Date, Delivery_Name: dto.Delivery_Name
            });

        });

        //存入第二张临时表同时更新第三张临时表的index
        $.each(obj.FixturePartOrderDList, function (i, dto) {
            var index = i + 1;
            //将数据插入插件中
            FIXTUREPART_ONEDATA.insert({ index: index, Fixture_Part_Order_D_UID: dto.Fixture_Part_Order_D_UID, Fixture_Part_Order_M_UID: dto.Fixture_Part_Order_M_UID, Fixture_Part_UID: dto.Fixture_Part_UID, Vendor_Info_UID: dto.Vendor_Info_UID, Part_ID: dto.Part_ID, Part_Name: dto.Part_Name, Part_Spec: dto.Part_Spec, Vendor_Name: dto.Vendor_Name, Forcast_Complation_Date: dto.Forcast_Complation_Date, Price: dto.Price, Qty: dto.Qty, SumActualQty: dto.SumActualQty });

            var threeDatas = FIXTUREPART_TWODATA({ Fixture_Part_Order_D_UID: dto.Fixture_Part_Order_D_UID });
            threeDatas.each(function (threeDto, aa) {
                //更新dindex和mindex
                FIXTUREPART_TWODATA({ Fixture_Part_Order_Schedule_UID: threeDto.Fixture_Part_Order_Schedule_UID }).update({ dIndex: aa + 1, mIndex: index });
            });

            //FIXTUREPARTORDERDLIST.push(dto);
            //$.each(obj.FixturePartOrderScheduleList, function (mm, d_dto) {
            //    var dIndex = mm + 1;
            //    FIXTUREPART_TWODATA.insert({
            //            dIndex:dIndex, mIndex: index, Fixture_Part_Order_Schedule_UID: d_dto.Fixture_Part_Order_Schedule_UID, Fixture_Part_Order_D_UID: d_dto.Fixture_Part_Order_D_UID, Receive_Date: d_dto.Receive_Date, Forcast_Receive_Qty: d_dto.Forcast_Receive_Qty, Actual_Receive_Qty: d_dto.Actual_Receive_Qty, Is_Complated: d_dto.Is_Complated, Del_Flag: d_dto.Del_Flag
            //    });


            //});
        });


        //绑定第三级子表
        var oneItem = FIXTUREPART_ONEDATA({ index: 1 }).first();
        BindThreeData(oneItem);

        ////渲染datetimepicker 效果
        //var subcomplationDateTime = $("#js_Purchase_datatable_detail_sub input[name=Receive_Date]");
        //subcomplationDateTime.datetimepicker({ format: 'yyyy-mm-dd', minView: 2, autoclose: true
        //});



    });
}

//查看状态设置所有控件只读
//var SetControlReadonly = function () {
//    //$("select").prop("disabled", true);
//    $('.setAttr').attr('readonly', 'readonly');
//    //$('#js_edit_modal').find('.btn-primary').hide();
//    $('.selectpicker').selectpicker('refresh');
//    //$('input').attr('readonly', 'readonly');


//}

//编辑状态设置所有控件状态
//var SetControlEdit = function () {
//    //$("select").prop("disabled", false);
//    $('.setAttr').removeAttr('readonly');
//    $('.selectpicker').selectpicker('refresh');
//    $('#js_edit_modal').find('.btn-primary').show();

//}

var GetDetailColumn = function () {
    var subcolumns = [
    {
        data: null, //不绑定数据列，需要添加此行
        createdCell: function (td, cellData, rowData, row, col) {
            $(td).html('<input type="checkbox" name="cktrans" class="js-checkbox-item" value="' + rowData.Fixture_Part_Order_D_UID + '">')
                .addClass('table-col-checkbox');
        },
        className: "text-center"
    }, {
        data: null,
        render: function (data, type, row, rowIndex) {
            var index = rowIndex.row + 1;
            return index;
        },
    }, {
        data: "Part_ID",
        className: "min-col-xs"
    }, {
        data: "Part_Name",
        className: "min-col-xs"
    }, {
        data: "Part_Spec",
        className: "min-col-xs"
    }, {
        data: "Vendor_Name",
        render: function (data, type, row, rowIndex) {
            var html = '<select type="text" name="Vendor_Name" class="form-control input-sm js-select-vendor" onchange="ddlVendorChange(this)">';
            var optionHtml = "";
            var title = $('#js_edit_modal').find(".modal-title").html();
            var list = [];
            if (title == '新增') {
                list = GetVenderInfo();
                var aa = 5;
            }
            else {
                if (row.VendorInfoList != null && row.VendorInfoList.length > 0) {
                    list = row.VendorInfoList;
                }
            }
            if (list.length > 0) {
                optionHtml += '<option></option>';
                //for (var i = 0; i < row.VendorInfoList.length; i++) {
                //    if (row.Vendor_Info_UID == row.VendorInfoList[i].Vendor_Info_UID) {
                //        optionHtml += '<option value="' + row.VendorInfoList[i].Vendor_Info_UID + '" selected="selected">' + row.VendorInfoList[i].Vendor_ID + '_' + row.VendorInfoList[i].Vendor_Name + '</option>';

                //    }
                //    else {
                //        optionHtml += '<option value="' + row.VendorInfoList[i].Vendor_Info_UID + '">' + row.VendorInfoList[i].Vendor_ID + '_' + row.VendorInfoList[i].Vendor_Name + '</option>';
                //    }
                //}
                for (var i = 0; i < list.length; i++) {
                    if (row.Vendor_Info_UID == list[i].Vendor_Info_UID) {
                        optionHtml += '<option value="' + list[i].Vendor_Info_UID + '" selected="selected">' + list[i].Vendor_ID + '_' + list[i].Vendor_Name + '</option>';

                    }
                    else {
                        optionHtml += '<option value="' + list[i].Vendor_Info_UID + '">' + list[i].Vendor_ID + '_' + list[i].Vendor_Name + '</option>';
                    }
                }
            }

            html += optionHtml + '</select>';
            return html;
        },
        className: "min-col-xs"
    }, {
        data: "Price",
        render: function (data, type, row, rowIndex) {
            var html = "";
            html = '<input type="number" name="Price" class="form-control input-sm js-select-vendor" style="text-align:right" value="' + row.Price + '" onchange="textPriceChange(this)"/>';
            return html;
        },
        className: "min-col-xs text-right"
    }, {
        data: "Qty",
        render: function (data, type, row, rowIndex) {
            var html = "";
            html = '<input type="number" name="Qty" class="form-control input-sm js-select-vendor" style="text-align:right" value="' + row.Qty + '" onchange="textQtyChange(this)"/>';
            return html;

        },
        className: "min-col-xs text-right"
    }, {
        data: "SumActualQty",
        className: "min-col-xs text-right"
    }, {
        data: "Forcast_Complation_Date",
        render: function (data, type, row, rowIndex) {
            var html = "";
            html = '<input type="text" name="Forcast_Complation_Date" class="form-control input-sm js-select-vendor date" value="' + row.Forcast_Complation_Date + '" onchange="textForcastDateChange(this)"/>';
            return html;
        },
        className: "min-col-xs"
    }
    //{
    //    data: "Del_Flag",
    //    render: function (data, type, row, rowIndex) {
    //        if (row.Del_Flag) {
    //            return 'Y';
    //        }
    //        else {
    //            return 'N';
    //        }
    //    },
    //    className: "min-col-xs"
    //}
    ];
    return subcolumns;
}

var GetSubColumn = function (isEdit) {
    var subcolumns = [
    {
        data: null, //不绑定数据列，需要添加此行
        createdCell: function (td, cellData, rowData, row, col) {
            $(td).html('<input type="checkbox" name="cktrans" class="js-checkbox-item" value="' + rowData.Fixture_Part_Order_Schedule_UID + '">')
                .addClass('table-col-checkbox');
        },
        className: "text-center"
    }, {
        data: null,
        render: function (data, type, row, rowIndex) {
            var index = rowIndex.row + 1;
            return index;
        },
    }, {
        data: "Receive_Date",
        render: function (data, type, row, rowIndex) {
            var html = "";
            html = '<input type="text"  name="Receive_Date" readonly="readonly" class="form-control input-sm js-select-vendor date" value="' + row.Receive_Date + '" onchange="textReceiveChange(this)"/>';
            return html;
        },
        className: "min-col-xs"
    }, {
        data: "Forcast_Receive_Qty",
        render: function (data, type, row, rowIndex) {
            var html = "";
            html = '<input onkeyup="onkeyupkl(this)" onafterpaste="onafterpastekl(this)" type="number"  name="Forcast_Receive_Qty" readonly="readonly" class="form-control input-sm js-select-vendor" style="text-align:right"  value="' + row.Forcast_Receive_Qty + '" onchange="textForcastReceiveQtyChange(this)"/>';
            return html;
        },
        className: "min-col-xs"
    }, {
        data: "Actual_Receive_Qty",
        render: function (data, type, row, rowIndex) {
            var html = "";
            if ((row.Delivery_Name != null || row.Delivery_Name != "") && (row.Actual_Receive_Qty == null)) {
                html = '<input onkeyup="onkeyupkl(this)" onafterpaste="onafterpastekl(this)" type="number" name="Actual_Receive_Qty"  class="form-control input-sm js-select-vendor" style="text-align:right" value="' + row.Actual_Receive_Qty + '" onchange="textActualReceiveQtyChange(this)"/>';
            } else {
                html = '<input onkeyup="onkeyupkl(this)" onafterpaste="onafterpastekl(this)" type="number" name="Actual_Receive_Qty" readonly="readonly" class="form-control input-sm js-select-vendor" style="text-align:right" value="' + row.Actual_Receive_Qty + '" onchange="textActualReceiveQtyChange(this)"/>';
            }
            return html;
        },
        className: "min-col-xs"
    },
    //{
    //    data: "Is_Complated",
    //    render: function (data, type, row, rowIndex) {
    //        var html = '<select type="text" name="SelectComplated" class="form-control input-sm js-select-vendor" onchange="ddlDelChange(this)">';
    //        var optionHtml = "";
    //        optionHtml += '<option value=0 {0}>N</option>';
    //        optionHtml += '<option value=1 {1}>Y</option>';
    //        if (row.Is_Complated) {
    //            optionHtml = optionHtml.replace('{0}', '');
    //            optionHtml = optionHtml.replace('{1}', 'selected="selected"');
    //        }
    //        else {
    //            optionHtml = optionHtml.replace('{0}', 'selected="selected"');
    //            optionHtml = optionHtml.replace('{1}', '');
    //        }
    //        html += optionHtml;
    //        return html;
    //    },
    //    className: "min-col-xs"
    //},

       {
           data: "DeliveryPeriod_Name",
           render: function (data, type, row, rowIndex) {
               var html = "";
               html = '<input type="text"  readonly="true" name="DeliveryPeriod_Name"  onfocus=this.blur() class="form-control input-sm js-select-vendor" style="text-align:left" value="' + row.DeliveryPeriod_Name + '" onchange="textDelivery_Date(this)"/>';
               if (row.DeliveryPeriod_Name == null || row.DeliveryPeriod_Name == "") {
                   return "";
               } else {
                   return html;
               }
           },
           className: "min-col-xs"
       },
      {
          data: "DeliveryPeriod_Date",
          render: function (data, type, row, rowIndex) {
              var html = "";
              var DeliveryPeriod_Date = "";
              if (row.DeliveryPeriod_Date == null || row.DeliveryPeriod_Date == "") {
                  DeliveryPeriod_Date = row.DeliveryPeriod_Date;
              } else {
                  DeliveryPeriod_Date = new Date(row.DeliveryPeriod_Date).Format("yyyy-MM-dd hh:mm")
              };

              html = '<input type="text" name="DeliveryPeriod_Name" readonly="true" class="form-control input-sm js-select-vendor" style="text-align:left"  value="' + DeliveryPeriod_Date + '" onchange="textDelivery_Date(this)"/>';
              if (row.DeliveryPeriod_Date == null || row.DeliveryPeriod_Date == "") {
                  return "";
              } else {
                  return html;
              }
          },
          className: "min-col-xs"
      },
     {
         data: "Delivery_Name",
         render: function (data, type, row, rowIndex) {
             var html = "";
             html = '<input type="text"  readonly="true" onfocus=this.blur() name="Delivery_Name" class="form-control input-sm js-select-vendor" style="text-align:left" value="' + row.Delivery_Name + '" onchange="textDelivery_Name(this)"/>';
             if (row.Delivery_Name == null || row.Delivery_Name == '') {
                 return "";
             } else {
                 return html;
             }
         },
         className: "min-col-xs"
     },
    {
        data: "Delivery_Date",
        render: function (data, type, row, rowIndex) {
            var html = "";
            var Delivery_Date = "";
            if (row.Delivery_Date == null || row.Delivery_Date == "") {
                Delivery_Date = row.Delivery_Date;
            } else {
                Delivery_Date = new Date(row.Delivery_Date).Format("yyyy-MM-dd hh:mm")
            };

            html = '<input type="text"  readonly="true"  name="Delivery_Date" class="form-control input-sm js-select-vendor" style="text-align:left" value="' + Delivery_Date + '" onchange="textDelivery_Date(this)"/>';
            if (row.Delivery_Date == null || row.Delivery_Date == "") {
                return "";
            } else {
                return html;
            }
        },
        className: "min-col-xs"
    }
    //{
    //    data: "Del_Flag",
    //    render: function (data, type, row, rowIndex) {
    //        var html = '<select type="text" name="SelectDel" class="form-control input-sm js-select-vendor" onchange="ddlDelChange(this)">';
    //        var optionHtml = "";
    //        optionHtml += '<option value=0 {0}>N</option>';
    //        optionHtml += '<option value=1 {0}>Y</option>';
    //        optionHtml += '</select>';
    //        if (row.Del_Flag) {
    //            optionHtml.replace('{0}', '');
    //            optionHtml.replace('{1}', 'selected="selected"');
    //        }
    //        else {
    //            optionHtml.replace('{0}', 'selected="selected"');
    //            optionHtml.replace('{1}', '');
    //        }
    //        html += optionHtml;
    //        return html;
    //    },
    //    className: "min-col-xs"
        //}
    ];
    return subcolumns;
}

Date.prototype.Format = function (fmt) { //author: meizz
    var o = {
        "M+": this.getMonth() + 1, //月份
        "d+": this.getDate(), //日
        "h+": this.getHours(), //小时
        "m+": this.getMinutes(), //分
        "s+": this.getSeconds(), //秒
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度
        "S": this.getMilliseconds() //毫秒
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}
//绑定第三级表的数据
var BindThreeData = function (oneItem) {
    var twoList = FIXTUREPART_TWODATA({ mIndex: oneItem.index });
    var paramList = [];

    twoList.each(function (dto) {
        var paramItem = {};
        paramItem["dIndex"] = '';
        paramItem["mIndex"] = '';
        paramItem["Fixture_Part_Order_Schedule_UID"] = dto.Fixture_Part_Order_Schedule_UID;
        paramItem["Fixture_Part_Order_D_UID"] = dto.Fixture_Part_Order_D_UID;
        paramItem["Receive_Date"] = dto.Receive_Date;
        paramItem["Forcast_Receive_Qty"] = dto.Forcast_Receive_Qty;
        paramItem["Actual_Receive_Qty"] = dto.Actual_Receive_Qty;
        paramItem["Is_Complated"] = dto.Is_Complated;
        paramItem["Del_Flag"] = dto.Del_Flag;
        paramItem["DeliveryPeriod_Date"] = dto.DeliveryPeriod_Date;
        paramItem["DeliveryPeriod_Name"] = dto.DeliveryPeriod_Name;
        paramItem["Delivery_Date"] = dto.Delivery_Date;
        paramItem["Delivery_Name"] = dto.Delivery_Name;
        paramList.push(paramItem);
    });

    var isEdit = true;
    var title = $('#js_edit_modal').find(".modal-title").html();
    if (title == '交货') {
        isEdit = false;
    }
    subTwoTable = $('#js_Purchase_datatable_detail_sub').DataTable({
        columns: GetSubColumn(isEdit),
        ordering: false,
        data: paramList,
        destroy: true
    });

    SetStatus();
    //渲染datetimepicker 效果
    //var complationDateTime = $("#js_Purchase_datatable_detail_sub input[name=Receive_Date]");
    //complationDateTime.datetimepicker({ format: 'yyyy-mm-dd', minView: 2, autoclose: true });


}

var GetThreeTable = function () {
    if (subTwoTable != null) {
        return subTwoTable;
    }
};

//绑定供应商的数据
var BindVendorInfo = function () {
    var url = '../Fixture/GetVendorInfoList';
    $.post(url, { Plant_Organization_UID: $('#js_s_input_plant option:selected').val(), BG_Organization_UID: 0, FunPlant_Organization_UID: 0 }, function (data) {
        $.each(data, function (i, dto) {
            var index = i + 1;
            VENDORINFOLIST_DATA.insert({ index: index, Plant_Organization_UID: dto.Plant_Organization_UID, BG_Organization_UID: dto.BG_Organization_UID, Vendor_Info_UID: dto.Vendor_Info_UID, Vendor_ID: dto.Vendor_ID, Vendor_Name: dto.Vendor_Name, Is_Enable: dto.Is_Enable });
        });

    });
}

//获取按厂区，OP分组的供应商列表
var GetVenderInfo = function () {
    var plantUID = parseInt($('#js_s_input_plant option:selected').val());
    var bgUID = $('#js_s_input_group_edit').val();
    if (bgUID.length == 0) {
        bgUID = null;
    }
    else {
        bgUID = parseInt(bgUID);
    }
    var params = []
    VENDORINFOLIST_DATA({ Plant_Organization_UID: plantUID, BG_Organization_UID: bgUID }).each(function (dto) {
        params.push(dto);
    });
    return params;
}

var ReSortList = function () {
    FIXTUREPART_ONEDATA().each(function (dto, i) {
        var index = i + 1;
        //更新三级表外键索引
        FIXTUREPART_TWODATA({ mIndex: dto.index }).update({ mIndex: index });
        //更新二级表索引
        FIXTUREPART_ONEDATA({ index: dto.index }).update({ index: index });
    });
}

var ReSortTwoList = function () {
    FIXTUREPART_TWODATA().each(function (dto, i) {
        var index = i + 1;
        FIXTUREPART_TWODATA({ dIndex: dto.dIndex }).update({ dIndex: index });
    });
}

var SetStatus = function () {
    var title = $('#js_edit_modal').find(".modal-title").html();

    //供应商
    var vendor = $("#js_Purchase_datatable_detail select");
    //采购单价采购数量
    var purchase = $("#js_Purchase_datatable_detail input");
    //预计完成日期
    var forcast_complationDateTime = $("#js_Purchase_datatable_detail input[name=Forcast_Complation_Date]");
    //预计交货日期
    var receive_complationDateTime = $("#js_Purchase_datatable_detail_sub input[name=Receive_Date]");
    //预计交货量
    var receiveQty = $("#js_Purchase_datatable_detail_sub input[name=Forcast_Receive_Qty]");
    //实际交货量
    var actualReceiveQty = $("#js_Purchase_datatable_detail_sub input[name=Actual_Receive_Qty]");
    //是否完成
    var isComplation = $("#js_Purchase_datatable_detail_sub select[name=SelectComplated]");
    //删除状态
    var deleteStatus = $("#js_Purchase_datatable_detail_sub select[name=SelectDel]");
    deleteStatus.attr('disabled', 'true');

    if (title == '交货') {
        $('#js_input_Order_ID_edit').attr('readonly', 'readonly');
        $('#js_s_input_plant_edit').attr('disabled', 'true');
        $('#js_s_input_group_edit').attr('disabled', 'true');
        $('#js_s_input_FunPlant_edit').attr('disabled', 'true');
        $('#js_s_input_plant_edit').selectpicker('refresh');
        $('#js_s_input_group_edit').selectpicker('refresh');
        $('#js_s_input_FunPlant_edit').selectpicker('refresh');

        $("button[data-id='js_s_input_plant_edit']").removeClass("disabled");
        $("button[data-id='js_s_input_group_edit']").removeClass("disabled");
        $("button[data-id='js_s_input_FunPlant_edit']").removeClass("disabled");

        $('#js_s_input_Order_Date_edit').attr('disabled', 'true');
        $('#js_input_Remarks_edit').attr('readonly', 'readonly');

        $('#js_s_Fixture_No_edit').attr('disabled', 'true');
        $('#js_s_input_part_id').attr('disabled', 'true');
        $('#js_s_Fixture_No_edit').selectpicker('refresh');
        $('#js_s_input_part_id').selectpicker('refresh');
        $("button[data-id='js_s_Fixture_No_edit']").removeClass("disabled");
        $("button[data-id='js_s_input_part_id']").removeClass("disabled");

        $('#hiddiv').show();

        $('#js_btn_newpart').hide();
        $('#js_btn_deletepart').hide();
        $('#js_btn_newSub').hide();
        $('#js_btn_deleteSub').hide();

        $('#js_s_Fixture_No_edit').attr('disabled', 'true');
        $('#js_s_input_part_id').attr('disabled', 'true');
        vendor.attr('disabled', 'true');
        purchase.attr('readonly', 'readonly');
        forcast_complationDateTime.attr('readonly', 'readonly');
        receive_complationDateTime.attr('readonly', 'readonly');
        receiveQty.attr('readonly', 'readonly');
        //actualReceiveQty.removeAttr('readonly');
        isComplation.removeAttr('disabled');
        $('#js_btn_submitEdit').hide();
    }
    else if (title == '编辑') {
        $('#js_input_Order_ID_edit').attr('readonly', 'readonly');
        $('#js_s_input_plant_edit').removeAttr('disabled');
        $('#js_s_input_group_edit').removeAttr('disabled');
        $('#js_s_input_FunPlant_edit').removeAttr('disabled');
        $('#js_s_input_plant_edit').selectpicker('refresh');
        $('#js_s_input_group_edit').selectpicker('refresh');
        $('#js_s_input_FunPlant_edit').selectpicker('refresh');

        $('#js_s_input_Order_Date_edit').removeAttr('disabled');
        $('#js_input_Remarks_edit').removeAttr('readonly');

        $('#js_s_Fixture_No_edit').removeAttr('disabled');
        $('#js_s_input_part_id').removeAttr('disabled');
        $('#js_s_Fixture_No_edit').selectpicker('refresh');
        $('#js_s_input_part_id').selectpicker('refresh');
        $('#hiddiv').hide();
        vendor.removeAttr('disabled');
        purchase.removeAttr('readonly');
        forcast_complationDateTime.datetimepicker({ format: 'yyyy-mm-dd', minView: 2, autoclose: true });
        receive_complationDateTime.datetimepicker({ format: 'yyyy-mm-dd', minView: 2, autoclose: true });
        //actualReceiveQty.attr('readonly', 'readonly');
        isComplation.attr('disabled', 'true');
        $('#js_btn_newSub').hide();
        $('#js_btn_deleteSub').hide();
        $('#js_edit_modal').find('.btn-primary').show();
        $('#js_btn_submitEdit').show();
    }
    else if (title == '查看') {
        $('#js_input_Order_ID_edit').attr('readonly', 'readonly');
        $('#js_s_input_plant_edit').attr('disabled', 'true');
        $('#js_s_input_group_edit').attr('disabled', 'true');
        $('#js_s_input_FunPlant_edit').attr('disabled', 'true');
        $('#js_s_input_plant_edit').selectpicker('refresh');
        $('#js_s_input_group_edit').selectpicker('refresh');
        $('#js_s_input_FunPlant_edit').selectpicker('refresh');

        $("button[data-id='js_s_input_plant_edit']").removeClass("disabled");
        $("button[data-id='js_s_input_group_edit']").removeClass("disabled");
        $("button[data-id='js_s_input_FunPlant_edit']").removeClass("disabled");

        $('#js_s_input_Order_Date_edit').attr('disabled', 'true');
        $('#js_input_Remarks_edit').attr('readonly', 'readonly');

        $('#js_s_Fixture_No_edit').attr('disabled', 'true');
        $('#js_s_input_part_id').attr('disabled', 'true');
        $("button[data-id='js_s_Fixture_No_edit']").removeClass("disabled");
        $("button[data-id='js_s_input_part_id']").removeClass("disabled");


        vendor.attr('disabled', 'true');
        purchase.attr('readonly', 'readonly');
        receive_complationDateTime.attr('readonly', 'readonly');
        receiveQty.attr('readonly', 'readonly');
        actualReceiveQty.attr('readonly', 'readonly');
        isComplation.attr('disabled', 'true');
        $('#hiddiv').show();
        $('#js_edit_modal').find('.btn-primary').hide();
        $('#js_btn_submitEdit').hide();
    }
    else if (title == '新增') {
        $('#hiddiv').hide();
        $('#js_s_input_plant_edit').removeAttr('disabled');
        $('#js_s_input_group_edit').removeAttr('disabled');
        $('#js_s_input_FunPlant_edit').removeAttr('disabled');
        $('#js_s_input_plant_edit').selectpicker('refresh');
        $('#js_s_input_group_edit').selectpicker('refresh');
        $('#js_s_input_FunPlant_edit').selectpicker('refresh');

        $('#js_s_input_Order_Date_edit').removeAttr('disabled');
        $('#js_input_Remarks_edit').removeAttr('readonly');

        //
        $('#js_input_Order_ID_edit').removeAttr('readonly');

        $('#js_s_Fixture_No_edit').removeAttr('disabled');
        $('#js_s_input_part_id').removeAttr('disabled');
        $('#js_s_Fixture_No_edit').selectpicker('refresh');
        $('#js_s_input_part_id').selectpicker('refresh');

        forcast_complationDateTime.datetimepicker({ format: 'yyyy-mm-dd', minView: 2, autoclose: true });
        actualReceiveQty.attr('readonly', 'readonly');
        isComplation.attr('disabled', 'true');
        $('#js_btn_newSub').hide();
        $('#js_btn_deleteSub').hide();
        $('#js_edit_modal').find('.btn-primary').show();
        $('#js_btn_submitEdit').show();
    }
    else if (title == '交期') {
        $('#js_input_Order_ID_edit').attr('readonly', 'readonly');
        $('#js_s_input_plant_edit').attr('disabled', 'true');
        $('#js_s_input_group_edit').attr('disabled', 'true');
        $('#js_s_input_FunPlant_edit').attr('disabled', 'true');
        $('#js_s_input_plant_edit').selectpicker('refresh');
        $('#js_s_input_group_edit').selectpicker('refresh');
        $('#js_s_input_FunPlant_edit').selectpicker('refresh');

        $("button[data-id='js_s_input_plant_edit']").removeClass("disabled");
        $("button[data-id='js_s_input_group_edit']").removeClass("disabled");
        $("button[data-id='js_s_input_FunPlant_edit']").removeClass("disabled");

        $('#js_s_input_Order_Date_edit').attr('disabled', 'true');
        $('#js_input_Remarks_edit').attr('readonly', 'readonly');

        $('#js_s_Fixture_No_edit').attr('disabled', 'true');
        $('#js_s_input_part_id').attr('disabled', 'true');
        $('#js_s_Fixture_No_edit').selectpicker('refresh');
        $('#js_s_input_part_id').selectpicker('refresh');
        $("button[data-id='js_s_Fixture_No_edit']").removeClass("disabled");
        $("button[data-id='js_s_input_part_id']").removeClass("disabled");

        $('#js_btn_newpart').hide();
        $('#js_btn_deletepart').hide();
        $('#hiddiv').show();
        $('#js_btn_newSub').show();
        $('#js_btn_deleteSub').show();
        $('#js_s_Fixture_No_edit').attr('disabled', 'true');
        $('#js_s_input_part_id').attr('disabled', 'true');
        vendor.attr('disabled', 'true');
        purchase.attr('readonly', 'readonly');
        actualReceiveQty.attr('readonly', 'readonly');
        isComplation.attr('disabled', 'true');
        //receive_complationDateTime.attr('readonly', 'readonly');
        //receiveQty.attr('readonly', 'readonly');
        $('#js_btn_submitEdit').hide();
    }
}