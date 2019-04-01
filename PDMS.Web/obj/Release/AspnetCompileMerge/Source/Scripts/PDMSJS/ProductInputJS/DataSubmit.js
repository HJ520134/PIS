//保存资料按钮
var SaveInfo = function (dataTable, saveUrl) {
    $.blockUI({ message: "<h1>数据提交中，请稍后...</h1>" });
    //var dataTable = ProductData.GetSubDatatable();
    var PPCheckValue = {};
    var PPCheckList = [];
    var IsCheckOK = true;
    var flag = true;

    // 首先检查用户输入的数字是否为 自然数
    function checkRate(input) {
        //if (!flag) {
        //    return;
        //}
        var re = /^[0-9]+[0-9]*]*$/;

        if (!re.test(input)) {
            PDMS.Utility.MessageBox.error("请输入自然数");
            flag = false;
        }
    }

    dataTable
        .rows()
        .every(function (rowIdx, tableLoop, rowLoop) {
            //如果输入的数据不合法则相当于执行continue命令跳出循环，break执行不了
            if (!flag) {
                return false;
            }
            var sub = {};
            var subRework = null;
            var subRepair = null;
            var row = $('#js_plant_datatable>tbody>tr').eq(rowIdx);
            var Process_Seq = $.trim(row.find('input[name=Process_Seq]').val());
            var Process = $.trim(row.find('input[name=Process]').val());
            var Color = $.trim(row.find('input[name=Color]').val());
            var Location_Flag = $.trim(row.find('input[name=Location_Flag]').val());
            // 用户未输入值时候，系统默认为"0"，输入值提交时 需要验证用户输入的值是否为自然数。

            //领料数
            var Picking_QTY = $.trim(row.find('input[name=Picking_QTY]').val()); 
            if (Picking_QTY != "") {
                flag = checkValidRate(Picking_QTY);
                if (!flag) {
                    PDMS.Utility.MessageBox.error("领料数-请输入自然数");
                    return false;
                }
            }
            else {
                Picking_QTY = 0;
            }

            //良品数
            var Normal_Good_QTY = $.trim(row.find('input[name=Normal_Good_QTY]').val());
            if (Normal_Good_QTY != "") {
                flag = checkValidRate(Normal_Good_QTY);
                if (!flag) {
                    PDMS.Utility.MessageBox.error("良品数-请输入自然数");
                    return false;
                }
            }
            else {
                Normal_Good_QTY = 0;
            }

            //NG数
            var Normal_NG_QTY = $.trim(row.find('input[name=Normal_NG_QTY]').val());
            if (Normal_NG_QTY != "") {
                flag = checkValidRate(Normal_NG_QTY);
                if (!flag) {
                    PDMS.Utility.MessageBox.error("NG数-请输入自然数");
                    return false;
                }
            }
            else {
                Normal_NG_QTY = 0;
            }


            //仓库领料数
            var WH_Picking_QTY = $.trim(row.find('input[name=WH_Picking_QTY]').val());
            if (WH_Picking_QTY!= "") {
                flag = checkValidRate(WH_Picking_QTY);
                if (!flag) {
                    PDMS.Utility.MessageBox.error("仓库领料数-请输入自然数");
                    return false;
                }
            }
            else {
                WH_Picking_QTY = 0;
            }


            //未达成原因
            var Unacommpolished_Reason = $.trim(row.find('select[name=Unacommpolished_Reason]').find("option:selected").text());


            //调机试制数
            var Adjust_QTY = $.trim(row.find('input[name=Adjust_QTY]').val());
            if (Adjust_QTY != "") {
                flag = checkValidRate(Adjust_QTY);
                if (!flag) {
                    PDMS.Utility.MessageBox.error("调机试制数-请输入自然数");
                    return false;
                }
            }
            else {
                Adjust_QTY = 0;
            }


            //入库数
            var WH_QTY = $.trim(row.find('input[name=WH_QTY]').val());
            if (WH_QTY != "") {
                flag = checkValidRate(WH_QTY);
                if (!flag) {
                    PDMS.Utility.MessageBox.error("入库数-请输入自然数");
                    return false;
                }
            }
            else {
                WH_QTY = 0;
            }


            //不可用wip数
            var NullWip_QTY = $.trim(row.find('input[name=NullWip_QTY]').val());
            if (NullWip_QTY != "")
            {
                flag = checkValidRate(NullWip_QTY);
                if (!flag) {
                    PDMS.Utility.MessageBox.error("不可用WIP-请输入自然数");
                    return false;
                }
            }else{
                NullWip_QTY = 0;
            }

            //Q报表数据准备

            var Abnormal_Good_QTY = 0;
            if ($.trim(row.find('input[name=Abnormal_Good_QTY]').val()) != "") {
                Abnormal_Good_QTY = $.trim(row.find('input[name=Abnormal_Good_QTY]').val());
                checkRate(Abnormal_Good_QTY);
            }

            var Abnormal_NG_QTY = 0;
            if ($.trim(row.find('input[name=Abnormal_NG_QTY]').val()) != "") {
                Abnormal_NG_QTY = $.trim(row.find('input[name=Abnormal_NG_QTY]').val());
                checkRate(Abnormal_NG_QTY);
            }
            var Place = $.trim(row.find('input[name=Place]').val());
            var Prouct_Plan = $.trim(row.find('input[name=Prouct_Plan]').val());
            var Product_Stage = $.trim(row.find('input[name=Product_Stage]').val());
            var Target_Yield = $.trim(row.find('input[name=Target_Yield]').val());
            ///先直接将 当前的wip数量存为0 然后用 sp计算

            sub["Location_Flag"]=Location_Flag;
            sub["Wip_Qty"] = 0;
            sub["Is_Comfirm"] = false;
            sub["Product_Date"] = $.trim(row.find('input[name=Product_Date]').val());
            sub["Time_Interval"] = $.trim(row.find('input[name=Time_Interval]').val());
            sub["Customer"] = $.trim(row.find('input[name=Customer]').val());
            sub["Project"] = $.trim(row.find('input[name=Project]').val());
            sub["Part_Types"] = $.trim(row.find('input[name=Part_Types]').val());
            sub["FunPlant"] = $.trim(row.find('input[name=FunPlant]').val());
            sub["DRI"] = $.trim(row.find('input[name=DRI]').val());
            sub["FlowChart_Detail_UID"] = $.trim(row.find('input[name=FlowChart_Detail_UID]').val());
            sub["FunPlant_Manager"] = $.trim(row.find('input[name=FunPlant_Manager]').val());
            sub["Product_Phase"] = $.trim(row.find('input[name=Product_Phase]').val());
            sub["Process_Seq"] = $.trim(row.find('input[name=Process_Seq]').val());
            sub["Place"] = Place;
            sub["Process"] = Process;
            sub["FlowChart_Master_UID"] = $.trim(row.find('input[name=FlowChart_Master_UID]').val());
            sub["FlowChart_Version"] = $.trim(row.find('input[name=FlowChart_Version]').val());
            sub["Color"] = Color;
            sub["Prouct_Plan"] = Prouct_Plan;
            sub["Product_Stage"] = Product_Stage;
            sub["Target_Yield"] = Target_Yield;
            //Q报表数据准备
            //sub["Good_QTY"] = Good_QTY;
            sub["Normal_Good_QTY"] = Normal_Good_QTY;
            sub["Abnormal_Good_QTY"] = Abnormal_Good_QTY;
            sub["Normal_NG_QTY"] = Normal_NG_QTY;
            sub["Abnormal_NG_QTY"] = Abnormal_NG_QTY;

            sub["Unacommpolished_Reason"] = Unacommpolished_Reason;
            
            sub["Picking_QTY"] = Picking_QTY;
            sub["WH_Picking_QTY"] = WH_Picking_QTY;
            //sub["NG_QTY"] = NG_QTY;
            sub["WH_QTY"] = WH_QTY;
            sub["NullWip_QTY"] = NullWip_QTY;
            sub["Adjust_QTY"] = Adjust_QTY;
            sub["Creator_UID"] = 90014; // 在后台代码中会自动修改该值为当前修改人的UID，这里赋值只为该值为必填
            sub["Create_Date"] = new Date().toLocaleDateString();
            sub["Modified_UID"] = 90014; // 在后台代码中会自动修改该值为当前修改人的UID，这里赋值只为该值为必填
            sub["Modified_Date"] = new Date().toLocaleDateString();


            var Rework_Flag = $.trim(row.find('input[name=Rework_Flag]').val());
            if (Rework_Flag == 'Rework') {
                sub["IsRepair"] = Rework_Flag
                var detailUID = $.trim(row.find('input[name=FlowChart_Detail_UID]').val());

                subRework = SaveReworkDataByNew(sub);
                sub["ProductReworkInfoVM"] = subRework;
            }
            else if (Rework_Flag == 'Repair') {
                sub["IsRepair"] = Rework_Flag
                var detailUID = $.trim(row.find('input[name=FlowChart_Detail_UID]').val());

                subRepair = SaveRepairDataByNew(sub);
                sub["ProductRepairInfoVM"] = subRepair;
            }
            PPCheckList.push(sub);
        });

    if (flag) {
        PPCheckValue.ProductLists = PPCheckList;
        $.post(saveUrl, { jsonWithProduct: JSON.stringify(PPCheckValue) }, function (data) {
            if (data != '') {
                PDMS.Utility.MessageBox.error(data);
                 $.unblockUI();
            }
            else {
                PDMS.Utility.MessageBox.info("保存成功！");
                location.reload();
        }
        });
    }
    else {
        $.unblockUI();
    }
}
//$('#btn-submit-inner').click('click', function () {

//});

function SaveReworkDataByNew(item) {
    var ProductReworkInfoVM = [];
    var id = 'js_rework_modal_' + item.FlowChart_Detail_UID;
    $mid = $('#' + id);
    $mid.find(".rework_text").each(function () {
        var sub = {};
        sub["FlowChart_Detail_UID"] = this.getAttribute("data-FlowChart_Detail_UID");
        sub["Opposite_Detail_UID"] = this.getAttribute("data-Opposite_Detail_UID");
        sub["Opposite_QTY"] = this.value;
        sub["Rework_Type"] = this.getAttribute("data-Rework_Type");
        sub["Rework_Flag"] = "Rework";


        ProductReworkInfoVM.push(sub);

    });
    return ProductReworkInfoVM;
}

function SaveRepairDataByNew(item) {
    var ProductRepairInfoVM = [];
    var id = 'js_repair_modal_' + item.FlowChart_Detail_UID;
    $mid = $('#' + id);
    $mid.find(".rework_text").each(function () {
        var sub = {};
        sub["FlowChart_Detail_UID"] = this.getAttribute("data-FlowChart_Detail_UID");
        sub["Opposite_Detail_UID"] = this.getAttribute("data-Opposite_Detail_UID");
        sub["Opposite_QTY"] = this.value;
        sub["Rework_Type"] = this.getAttribute("data-Rework_Type");
        sub["Rework_Flag"] = "Repair";


        ProductRepairInfoVM.push(sub);

    });
    return ProductRepairInfoVM;
}

//Rework单笔编辑信息对象转换
function SaveReworkDataByModify(item) {
    var ProductReworkInfoVM = [];
    var id = 'js_rework_modal_' + item.FlowChart_Detail_UID;
    $mid = $('#' + id);
    $mid.find(".rework_text").each(function () {

        var sub = {};
        sub["Rework_UID"] = this.id;
        sub["Product_UID"] = this.getAttribute("data-Product_UID");
        sub["FlowChart_Detail_UID"] = this.getAttribute("data-FlowChart_Detail_UID");
        sub["Opposite_Detail_UID"] = this.getAttribute("data-Opposite_Detail_UID");
        sub["Opposite_QTY"] = this.value;
        sub["Rework_Type"] = this.getAttribute("data-Rework_Type");
        sub["Rework_Flag"] = "Rework";

        ProductReworkInfoVM.push(sub);
    });
    return ProductReworkInfoVM;
}

//Repair单笔编辑信息对象转换
function SaveRepairDataByModify(item) {
    var ProductRepairInfoVM = [];
    var id = 'js_repair_modal_' + item.FlowChart_Detail_UID;
    $mid = $('#' + id);
    $mid.find(".rework_text").each(function () {

        var sub = {};
        sub["Rework_UID"] = this.id;
        sub["Product_UID"] = this.getAttribute("data-Product_UID");
        sub["FlowChart_Detail_UID"] = this.getAttribute("data-FlowChart_Detail_UID");
        sub["Opposite_Detail_UID"] = this.getAttribute("data-Opposite_Detail_UID");
        sub["Opposite_QTY"] = this.value;
        sub["Rework_Type"] = this.getAttribute("data-Rework_Type");
        sub["Rework_Flag"] = "Repair";

        ProductRepairInfoVM.push(sub);
    });
    return ProductRepairInfoVM;
}









function SaveRepairData(detailUID,Color) {
    var id = 'js_repair_modal_' + detailUID;
    $mid = $('#' + id);
    var detail_Uid = $mid.find("#js_repair_detail_uid").val();
    var product_Uid = $mid.find("#js_repair_product_uid").val();
    //var result = {};
    var reworkList = [];
    var isEmpty = false;
    $mid.find(".rework_text").each(function () {

        //将颜色不同的Repair输入框隐藏掉
        //var strLabel = $(this).parent().parent().find('.control-label').text();
        //var index = strLabel.indexOf(Color);
        //if (index < 0){
        //    return; //实现continue功能
        //}

        var reworkOper = this.name;
        var reworkQty = this.value;
        if (!checkValidRate(reworkQty)) {
            reworkQty = 0;
        }
        var reworkUid = 0;
        var tempId = this.id.split('_');
        if (tempId[0].length > 0) {
            reworkUid = tempId[0]
        }
        var reworkDetailUid = tempId[1];
        //如果输入框的值都没有，则默认设置为0
        if (reworkQty.length == 0) {
            reworkQty = 0;
            //PDMS.Utility.MessageBox.error(reworkOper + "数值不能为空！");
            //isEmpty = true;
            //return false;
        }
        var sub = {};
        sub["reworkOper"] = reworkOper;
        sub["reworkQty"] = reworkQty;
        sub["Rework_UID"] = reworkUid;
        sub["reworkDetailUid"] = reworkDetailUid;
        sub["detailuid"] = detail_Uid;
        sub["Rework_Flag"] = "Repair";

        var flag = reworkOper.substring(reworkOper.length - 1, reworkOper.length);
        if (flag == '入') {
            sub["Rework_Type"] = "Input";
        }
        if (flag == '出') {
            sub["Rework_Type"] = "Output";
        }

        //sub["projectuid"] = product_Uid;
        reworkList.push(sub);
    });
    //result.ReworkDatas = reworkList;
    return reworkList;
};

function EditInfo(url) {
    var Product_UID = $('#Edit_Product_UID').val();
    var color = $('#hidColor').val();

    var PPCheckValue = {};
    var PPCheckList = [];
    var reworkFlag = $('#ReworkFlag').val();
    var flag = true;

    function checkRate(input) {
        if (!flag) {
            return;
        }
        var re = /^[0-9]+[0-9]*]*$/;

        if (!re.test(input)) {
            PDMS.Utility.MessageBox.error("请输入自然数");

            flag = false;

        }
    }

    var submitJson = $('#js_form_data_edit').serializeObject();

    var Picking_QTY = $('#s_input_Picking_QTY').val();
    checkRate(Picking_QTY);

    var WH_Picking_QTY = $('#s_input_WH_Picking_QTY').val();
    checkRate(WH_Picking_QTY);

    var Adjust_QTY = $('#s_input_Adjust_QTY').val();
    checkRate(Adjust_QTY);

    var WH_QTY = $('#s_input_WH_QTY').val();
    checkRate(WH_QTY);

    var NullWip_QTY = $('#s_input_NullWip_QTY').val();
    checkRate(NullWip_QTY);

    var Normal_Good_QTY = $('#s_input_Normal_Good_QTY').val();
    checkRate(Normal_Good_QTY);

    var Abnormal_Good_QTY = $('#s_input_Abnormal_Good_QTY').val();
    checkRate(Abnormal_Good_QTY);

    var Normal_NG_QTY = $('#s_input_Normal_NG_QTY').val();
    checkRate(Normal_NG_QTY);

    var Abnormal_NG_QTY = $('#s_input_Abnormal_NG_QTY').val();
    checkRate(Abnormal_NG_QTY);


    var inputQty = 0;
    var outputQty = 0;
    

    if (reworkFlag == "Rework") {
        //inputQty = $('input[name=Input_Rework_QTY]').val();
        //checkRate(inputQty);
        //outputQty = $('input[name=Output_Rework_QTY]').val();
        //checkRate(outputQty);
    }
    else if (reworkFlag == "Repair") {

    }

    var sub = {};
    var reworkList = [];

    sub["Product_UID"] = $('#Edit_Product_UID').val();
    sub["FlowChart_Detail_UID"]= $('#FlowChart_Detail_UID').val();
    sub["Normal_Good_QTY"] = Normal_Good_QTY;
    sub["Abnormal_Good_QTY"] = Abnormal_Good_QTY;
    sub["Normal_NG_QTY"] = Normal_NG_QTY;
    sub["Abnormal_NG_QTY"] = Abnormal_NG_QTY;
    sub["Picking_QTY"] = Picking_QTY;
    sub["WH_Picking_QTY"] = WH_Picking_QTY;
    sub["WH_QTY"] = WH_QTY;
    sub["NullWip_QTY"] = NullWip_QTY;
    sub["Adjust_QTY"] = Adjust_QTY;
    sub["IsRepair"] = reworkFlag;
    sub["Location_Flag"] = $('#Location_Flag').val();
    sub["Unacommpolished_Reason"] = $('#s_input_Unacommpolished_Reason').find("option:selected").text();
    if (reworkFlag == "Rework") {
        
        //var childSub = {};

        //childSub["Rework_UID"] = $('#inputQty').find('input[name=Rework_UID]').val();
        //childSub["reworkQty"] = inputQty;
        //childSub["Rework_Type"] = "Input";
        //childSub["Rework_Flag"] = "Rework";
        //reworkList.push(childSub);

        //childSub = {};
        //childSub["Rework_UID"] = $('#outputQty').find('input[name=Rework_UID]').val();
        //childSub["reworkQty"] = outputQty;
        //childSub["Rework_Type"] = "Output";
        //childSub["Rework_Flag"] = "Rework";
        //reworkList.push(childSub);

        //sub["ReworkList"] = reworkList;

        sub["ProductReworkInfoVM"] = SaveReworkDataByModify(sub);
    }
    else if (reworkFlag == "Repair") {
        //sub["IsRepair"] = reworkFlag;
        //sub["ReworkList"] = SaveReworkData($('#FlowChart_Detail_UID').val(),color);
        sub["ProductRepairInfoVM"] = SaveRepairDataByModify(sub);
    }

    PPCheckList.push(sub);
    PPCheckValue.ProductLists = PPCheckList;

    if (flag) {
        $.post(url, { jsonWithProduct: JSON.stringify(PPCheckValue) }, function (data) {
            if (data != '') {
                $('#js_edit_modal').modal('hide');
                if (data == "success")
                {
                    PDMS.Utility.MessageBox.info("修改成功");
                    location.reload();
                }

                else {
                    PDMS.Utility.MessageBox.error(data);
                    //setTimeout(location.reload(), 6000);                    
                }

            }
        });
    }

};

function checkValidRate(input) {
    var flag = true;
    var re = /^[0-9]+[0-9]*]*$/;

    if (!re.test(input)) {
        flag = false;
    }
    return flag;
}