

//客户下拉框改变事件
$("#js_s_input_customer").change(function ff() {
    ClearDropDownListValue('js_s_input_customer')
    GetProject();
});

//专案下拉框改变事件
$("#js_s_input_project").change(function ff() {

    ClearDropDownListValue('js_s_input_project')
    GetProductPhase();
    GetSelctOP();
    GetInterval();
    getProjectFuncplant();
    GetMasterUID();
});

//生产阶段下拉框改变事件
$("#js_s_input_Product_Phase").change(function ff() {
    ClearDropDownListValue('js_s_input_Product_Phase')
    GetPartTypes();
});

//部件类型下拉框改变事件
$("#js_s_input_part_types").change(function ff() {
    ClearDropDownListValue('js_s_input_part_types')
    GetColor();
    getProjectFuncplant();
    GetMasterUID();
});

function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
    if (arr = document.cookie.match(reg))
        return unescape(arr[2]);
    else
        return null;
}

//获取当前专案masterUID

function GetMasterUID() {
   
    var ProjectName = $("#js_s_input_project").val();
    var Part_Types = $("#js_s_input_part_types").val();
    var Product_Phase = $("#js_s_input_Product_Phase").val();
    var opType = $("#js_s_input_OP").val()
    var url = '../EventReportManager/GetSelctMasterUID';
    if (ProjectName != '' && Part_Types != '' && Product_Phase != '' && opType != '') {
        $.ajax({
            url: url,
            async: false,
            data: { "ProjectName": ProjectName, "Part_Types": Part_Types, "Product_Phase": Product_Phase, "opType": opType },
            success: function (data) {
                if (data != "") {
                    $("#js_s_input_MasterUID").val(data);
                }
            }
        });
    }

}

//获取当前专案所有功能厂

function getProjectFuncplant()
{
    $("#js_s_input_Funplant").empty();
    //var url = '../EventReportManager/GetCustomerSource';
    var url = '../EventReportManager/SelectPart';
    //获取功能的名字
    var ProjectName = $("#js_s_input_project").val();
    var Part_Types = $("#js_s_input_part_types").val();
    var Product_Phase = $("#js_s_input_Product_Phase").val();
    var opType = $("#js_s_input_OP").val()
    selectProjects = ProjectName + '_' + Part_Types + '_' + Product_Phase;
    $.post(url, { "selectProjects": selectProjects, "opType": opType }, function (data) {
        if (data != "") {
            $("<option></option>")
                  .val("ALL")
                  .text("ALL")
                  .appendTo($("#js_s_input_Funplant"));

            $.each(data, function (i, item) {
                $("<option></option>")
                    .val(item)
                    .text(item)
                    .appendTo($("#js_s_input_Funplant"));
            });
        }
    });
}

//设置日报的日期改变后，查找（日版本不是周版本或月版本）当天有没有2个版本，如果没有则显示最高版本
$("#js_s_input_date").change(function ff() {
    GetDayVersion();
});


//报表类型改变时候
$("#js_s_select_type").change(function () {
    if ($("#js_s_select_type").val() == "daily") //选择的类型为日报表
    {
        $('.dropdown-menu').find('li[name="onlydaily"]').removeAttr('hidden');
        $("#day_version").show();
        $("#week_label_th").hide();
        $("#day_label_th").show();
        $("#Funplant_select").show();
        $("#IsColour_select").show();
        
        $("#day_time_select").show();
        if ($("#js_ppcheckdata_mounth_wrapper").length > 0) {
            $("#js_ppcheckdata_mounth_wrapper").hide();
        }
        if ($("#js_ppcheckdata_week_wrapper").length > 0) {
            $("#js_ppcheckdata_week_wrapper").hide();
        }
        if ($('#js_ppcheckdata_period_wrapper').length > 0)
            $('#js_ppcheckdata_period_wrapper').hide();
        $("#js_ppcheckdata_datatable").show();
        $("#js_ppcheckdata_mounth").hide();
        $("#js_ppcheckdata_week").hide();
        $('#js_ppcheckdata_period').hide();
        $("#day_Select").show();

        $("#week_Select_start").hide();
        $("#week_Select_end").hide();
        $("#week_version").hide();
        $("#week_version_date").hide();
        $("#month_version_date").hide();
        $("#month_Select").hide();
        $("#month_label").hide();
        $("#month_version").hide();
        $("#time_Select_Start").hide();
        $("#time_Select_End").hide();
        $("#time_version").hide();
        GetDayVersion();
    }
    if ($("#js_s_select_type").val() == "weekly") //选择的类型为周报表
    {
        $('.dropdown-menu').find('li[name="onlydaily"]').removeAttr('hidden');
        $("#Funplant_select").hide();
        $("#IsColour_select").hide();
        
        if ($("#js_ppcheckdata_mounth_wrapper").length > 0) {
            $("#js_ppcheckdata_mounth_wrapper").hide();
        }
        if ($("#js_ppcheckdata_datatable_wrapper").length > 0) {
            $("#js_ppcheckdata_datatable_wrapper").hide();
        }
        if ($('#js_ppcheckdata_period_wrapper').length > 0)
            $('#js_ppcheckdata_period_wrapper').hide();
        $("#week_label_th").show();
        $("#day_label_th").hide();
        $("#js_ppcheckdata_week").show();
        $("#js_ppcheckdata_datatable").hide();
        $("#js_ppcheckdata_mounth").hide();
        $('#js_ppcheckdata_period').hide();
        $("#day_time_select").hide();
        $("#day_Select").hide();
        $("#week_Select_start").show();
        $("#week_Select_end").show();
        $("#week_version").show();
        $("#week_version_date").hide();
        $("#month_version_date").hide();
        $("#month_Select").hide();
        $("#month_label").hide();
        $("#month_version").hide();
        $("#time_Select_Start").hide();
        $("#time_Select_End").hide();
        $("#time_version").hide();
        $("#day_version").hide();

        GetWeekVersion();

        //var myDate = new Date();
        //myDate = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate();
        //$("#js_s_input_weekdate_start").val(myDate);
        //GetIntervel(myDate);
        //GetWeekVersion("js_s_input_week_verion", myDate);

    }
    if ($("#js_s_select_type").val() == "monthly") //选择的类型为月报表
    {
        $('.dropdown-menu').find('li[name="onlydaily"]').removeAttr('hidden');
        $("#Funplant_select").hide();
        $("#IsColour_select").hide();
        
        if ($("#js_ppcheckdata_week_wrapper").length > 0) {
            $("#js_ppcheckdata_week_wrapper").hide();
        }
        if ($("#js_ppcheckdata_datatable_wrapper").length > 0) {
            $("#js_ppcheckdata_datatable_wrapper").hide();
        }
        if ($('#js_ppcheckdata_period_wrapper').length > 0)
            $('#js_ppcheckdata_period_wrapper').hide();
        $("#week_label_th").show();
        $("#day_label_th").hide();
        $("#js_ppcheckdata_week").hide();
        $("#js_ppcheckdata_datatable").hide();
        $("#js_ppcheckdata_mounth").show();
        $('#js_ppcheckdata_period').hide();
        $("#day_time_select").hide();
        $("#day_Select").hide();
        $("#week_Select_start").hide();
        $("#week_Select_end").hide();
        $("#week_version").hide();
        $("#week_version_date").hide();
        $("#month_version_date").hide();
        $("#month_Select").show();
        $("#month_label").show();
        $("#month_version").show();
        $("#time_Select_Start").hide();
        $("#time_Select_End").hide();
        $("#time_version").hide();
        $("#day_version").hide();
        GetMonthVersion();


        //var myDate = new Date();
        //var startDay = getCurrentMonthFirst(myDate);
        //var endDay = getCurrentMonthLast(myDate);
        //var currentDay = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate();
        //var myDateStart = startDay.getFullYear() + "-" + (startDay.getMonth() + 1) + "-" + startDay.getDate();
        //var myDateEnd = endDay.getFullYear() + "-" + (endDay.getMonth() + 1) + "-" + endDay.getDate();

        //$('#Interval_Date_Start').val(myDateStart);
        //$('#Interval_Date_End').val(myDateEnd);
        //$('#js_s_begin_date_month').val(currentDay);
        //$("#js_s_input_mounth_interval").val("从： " + myDateStart + "  到：" + myDateEnd);
        //GetMonthVersion("js_s_input_month_verion", myDateStart, myDateEnd);
        //$("#interval_week_date").text($("#js_s_input_mounth_interval").val());
    }
    if ($("#js_s_select_type").val() == "time") //选择的类型为时间段报表
    {
        $('.dropdown-menu').find('li[name="onlydaily"]').attr('hidden','hidden');
        $("#Funplant_select").hide();
         $("#IsColour_select").hide();
        
        if ($("#js_ppcheckdata_week_wrapper").length > 0) {
            $("#js_ppcheckdata_week_wrapper").hide();
        }
        if ($("#js_ppcheckdata_datatable_wrapper").length > 0) {
            $("#js_ppcheckdata_datatable_wrapper").hide();
        }
        if ($("#js_ppcheckdata_mounth_wrapper").length > 0) {
            $("#js_ppcheckdata_mounth_wrapper").hide();
        }
        $("#week_label_th").show();
        $("#day_label_th").hide();

        $("#js_ppcheckdata_week").hide();
        $("#js_ppcheckdata_datatable").hide();
        $("#js_ppcheckdata_mounth").hide();
        $('#js_ppcheckdata_period').show();
        $("#day_time_select").hide();
        $("#day_Select").hide();
        $("#week_Select_start").hide();
        $("#week_Select_end").hide();
        $("#week_version").hide();
        $("#week_version_date").hide();
        $("#month_version_date").hide();
        $("#month_Select").hide();
        $("#month_label").hide();
        $("#month_version").hide();
        $("#time_Select_Start").show();
        $("#time_Select_End").show();
        $("#time_version").show();
        $("#day_version").hide();
        GetTimeSpanVersion();
    }
});


//周报日期选择变化时候
$("#js_s_input_weekdate_start").change(function () {
    GetWeekVersion();
});


// 月报日期变化时候执行
$("#js_s_begin_date_month").change(function () {
    GetMonthVersion();
});


//月报版本选择变化时候
$("#js_s_input_month_verion").change(function () {
    var customer = $("#js_s_input_customer").val();
    var ProjectName = $("#js_s_input_project").val();
    var ProductPhaseName = $("#js_s_input_Product_Phase").val();
    var PartTypesName = $("#js_s_input_part_types").val();
    var Version = $("#js_s_input_month_verion").val();
    var url = '../EventReportManager/GetVersionBeginEndDate';

    var myDate;
    myDate = new Date($("#js_s_begin_date_month").val());

    var startDay = getCurrentMonthFirst(myDate);
    var endDay = getCurrentMonthLast(myDate);
    var myDateStart = startDay.getFullYear() + "-" + (startDay.getMonth() + 1) + "-" + startDay.getDate();
    var myDateEnd = endDay.getFullYear() + "-" + (endDay.getMonth() + 1) + "-" + endDay.getDate();

    $.post(url, { "CustomerName": customer, "ProjectName": ProjectName, "ProductPhaseName": ProductPhaseName, "PartTypesName": PartTypesName, "Version": Version, "startDay": myDateStart, "endDay": myDateEnd }, function (data) {
        $("#interval_week_date").text(data.Interval);
    });
});

/// 时间段版本发生更改时
//$("#js_s_input_month_verion").change(function () {
//    $("#js_s_verion_interval option:selected").text($("#js_s_input_month_verion option:selected").text());
//});



//时间段中开始日期发生变化
$('#Interval_Date_Start').change(function () {
    var startDate = $("#Interval_Date_Start").val();
    var endDate = $("#Interval_Date_End").val();
    if (startDate != '' && endDate != '') {
        validTime(startDate, endDate,1);
        GetTimeSpanVersion();
    }
});

// 时间段中结束日期发生变化
$("#Interval_Date_End").change(function () {
    var startDate = $("#Interval_Date_Start").val();
    var endDate = $("#Interval_Date_End").val();
    if (startDate != '' && endDate != '') {
        validTime(startDate, endDate,2);
        GetTimeSpanVersion();
    }
});



//时间段中版本选择发生变化
$("#js_s_verion_interval").change(function () {
    var customer = $("#js_s_input_customer").val();
    var ProjectName = $("#js_s_input_project").val();
    var ProductPhaseName = $("#js_s_input_Product_Phase").val();
    var PartTypesName = $("#js_s_input_part_types").val();
    var Version = $("#js_s_verion_interval").val();
    var startDate = $("#Interval_Date_Start").val();
    var endDate = $("#Interval_Date_End").val();
    var url = '../EventReportManager/GetVersionBeginEndDate';

    $.post(url, { "CustomerName": customer, "ProjectName": ProjectName, "ProductPhaseName": ProductPhaseName, "PartTypesName": PartTypesName, "Version": Version, "startDay": startDate, "endDay": endDate }, function (data) {
        //$("#js_s_input_month_verion_date").val(data.Interval);
        $("#interval_week_date").text(data.Interval);
        //$("#Interval_Date_Start").val(formatDate(new Date(data.VersionBeginDate)));
        //$("#Interval_Date_End").val(formatDate(new Date(data.VersionEndDate)));
    });
});




//获取下拉框客户
function GetCustomer() {
    $("#js_s_input_customer").empty();
    //var url = '../EventReportManager/GetCustomerSource';
    var url = '../EventReportManager/GetCustomerSource';
    $.post(url, function (data) {
        if (data != "") {
            $.each(data, function (i, item) {
                $("<option></option>")
                    .val(item)
                    .text(item)
                    .appendTo($("#js_s_input_customer"));
            });
            GetProject();
        }
    });
}

//获取下拉框专案
function GetProject() {
    var projectCount = 0;
    $("#js_s_input_project").empty();
    //$("#js_s_input_project").append("<option value=''></option>");
    var temp = $("#js_s_input_customer").val();
    var url = '../EventReportManager/GetProjectSource';
    var CookieProject = getCookie('ProductReportProject');
    $.ajax({
        url: url,
        async: false,
        data:{ "CustomerName": temp },
        success: function(data) {
            if (data != "") {
                projectCount = data.length;
                $.each(data, function (i, item) {
                    $("<option></option>")
                        .val(item)
                        .text(item)
                        .appendTo($("#js_s_input_project"));
                    if(item==CookieProject)
                    {
                        $("#js_s_input_project").val(CookieProject);
                    }

                });
             
            }
        }
    });
    //如果Project列表不为0，则继续加载下级联动
    if (projectCount > 0) {
       ////先判断是否有cookie
       
       
        GetProductPhase();
        GetSelctOP();
        GetInterval();
        getProjectFuncplant();
        GetMasterUID();
    }
}


//获取下拉框生产阶段
function GetProductPhase() {
    var phaseCount = 0;
    $("#js_s_input_Product_Phase").empty();
    //$("#js_s_input_Product_Phase").append("<option value=''></option>");

    var tempCustomer = $("#js_s_input_customer").val();
    var tempProject = $("#js_s_input_project").val();
    var url = '../EventReportManager/GetProductPhaseSource';
    if (tempCustomer != '' && tempProject != '') {
        $.ajax({
            url: url,
            async: false,
            data: { "CustomerName": tempCustomer, "ProjectName": tempProject },
            success: function (data) {
                if (data != "") {
                    phaseCount = data.length;
                    $.each(data, function (i, item) {
                        $("<option></option>")
                            .val(item)
                            .text(item)
                            .appendTo($("#js_s_input_Product_Phase"));
                    });
                }
            }
        });
    }
    //ajax同步获取下级联动并加载
    if (phaseCount > 0) {
        GetPartTypes();
    }
}


function GetSelctOP() {
    var phaseCount = 0;
   
    var tempCustomer = $("#js_s_input_customer").val();
    var tempProject = $("#js_s_input_project").val();
    var url = '../EventReportManager/GetSelctOP';
    if (tempCustomer != '' && tempProject != '') {
        $.ajax({
            url: url,
            async: false,
            data: { "CustomerName": tempCustomer, "ProjectName": tempProject },
            success: function (data) {
                if (data != "") {
                  $("#js_s_input_OP").val(data);
                }
            }
        });
    }
  
}

function GetInterval() {
    $("#js_s_input_interval_time").empty();

    var CurrentOp = $("#js_s_input_OP").val();
    var url = '../EventReportManager/GetIntervalTime';
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
//下拉框初始化获取部件类型
function GetPartTypes() {
    var partTypesCount = 0;
    $("#js_s_input_part_types").empty();
    //$("#js_s_input_part_types").append("<option value=''></option>");
    var tempCustomer = $("#js_s_input_customer").val();
    var tempProject = $("#js_s_input_project").val();
    var tempPhaseName = $("#js_s_input_Product_Phase").val();
    var url = '../EventReportManager/GetPartTypesSource';
    if (tempCustomer != '' && tempProject != '' && tempPhaseName != '') {
        $.ajax({
            url: url,
            async: false,
            data: { "CustomerName": tempCustomer, "ProjectName": tempProject, "ProductPhaseName": tempPhaseName },
            success: function (data) {
                if (data != "") {
                    partTypesCount = data.length;
                    $.each(data, function (i, item) {
                        $("<option></option>")
                            .val(item)
                            .text(item)
                            .appendTo($("#js_s_input_part_types"));
                    });
                }
            }
        });

    }
    //ajax同步获取下级联动并加载
    if (partTypesCount > 0) {
        GetColor();
        getProjectFuncplant();
        GetMasterUID();
    }
}


//获取下拉框颜色
function GetColor() {
    var colorCount = 0;
    $("#js_s_input_color").empty();
    var tempCustomer = $("#js_s_input_customer").val();
    var tempProject = $("#js_s_input_project").val();
    var tempPhaseName = $("#js_s_input_Product_Phase").val();
    var tempPartTypes = $("#js_s_input_part_types").val();
    var url = '../EventReportManager/GetColorSource';
    if (tempCustomer != '' && tempProject != '' && tempPhaseName != '' && tempPartTypes != '') {
        $.ajax({
            url: url,
            async: false,
            data: { "CustomerName": tempCustomer, "ProjectName": tempProject, "ProductPhaseName": tempPhaseName, "PartTypesName": tempPartTypes },
            success: function (data) {
                if (data != "") {
                    colorCount = data.length;
                    $.each(data, function (i, item) {
                        $("<option></option>")
                            .val(item)
                            .text(item)
                            .appendTo($("#js_s_input_color"));
                    });
                }
            }
        });
    }
    if (colorCount > 0) {
        //根据选择的查询类型分别加载不同的下拉框
        if ($("#js_s_select_type").val() == "daily") {
            //如果天的日期字段不为空，则自动加载版本信息
            if ($('#js_s_input_date').val() != '') {
                GetDayVersion();
            }
        }
        else if ($("#js_s_select_type").val() == "weekly") {
            //如果周的日期字段不为空，则自动加载版本信息
            if ($("#js_s_input_weekdate_start").val() != '') {
                GetWeekVersion();
            }
        }
        else if($("#js_s_select_type").val() == "monthly"){
            //如果月的日期字段不为空，则自动加载版本信息
            if ($("#js_s_begin_date_month").val() != '') {
                GetMonthVersion();
            }
        }
        else if ($("#js_s_select_type").val() == "time") {
            GetTimeSpanVersion();
        }
    }
}

function GetDayVersion() {
    $("#input_day_verion").empty();
    $("#input_day_verion").val('');
    var tempCustomer = $("#js_s_input_customer").val();
    var tempProject = $("#js_s_input_project").val();
    //if (tempProject.length > 15) {
    //    tempProject = tempProject.substr(0, 15);
    //}
    var tempPhaseName = $("#js_s_input_Product_Phase").val();
    var tempPartTypes = $("#js_s_input_part_types").val();
    var day = $("#js_s_input_date").val();
    if (tempCustomer != '' && tempProject != '' && tempPhaseName != '' && tempPartTypes != '' && day != '') {
        var url = '../EventReportManager/GetDayVersion';
        //查询出当天的版本
        $.get(url, { "CustomerName": tempCustomer, "ProjectName": tempProject, "ProductPhaseName": tempPhaseName, "PartTypesName": tempPartTypes, "Day": day }, function (data) {
            if (data != "") {
                $.each(data, function (i, item) {
                    $("<option></option>")
                        .val(item)
                        .text(item)
                        .appendTo($("#input_day_verion"));
                });
                
            }
        });
    }
}

function GetWeekVersion() {
    var myDate;
    //查看周所在日期是否有指定输入日期，如果有则以输入的日期为标准查版本，如果没有则以当天的日期为当前周查版本
    if ($("#js_s_input_weekdate_start").val() == '') {
        myDate = new Date();
        myDate = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate();
        $("#js_s_input_weekdate_start").val(myDate);
    }
    else {
        myDate = $("#js_s_input_weekdate_start").val();
    }

    
    //获取时间段
    GetIntervel(myDate);
    //GetweekVersion("js_s_input_week_verion", myDate);
    $("#interval_week_date").text($("#js_s_input_week_interval").val());

    $("#js_s_input_week_verion").empty();
    $("#js_s_input_week_verion").val('');
    var tempCustomer = $("#js_s_input_customer").val();
    var tempProject = $("#js_s_input_project").val();
    //if (tempProject.length > 15) {
    //    tempProject = tempProject.substr(0, 15);
    //}
    var tempPhaseName = $("#js_s_input_Product_Phase").val();
    var tempPartTypes = $("#js_s_input_part_types").val();
    var startDay;
    var endDay;
    var url = '../EventReportManager/GetWeekVersionSource';
    $.post(url, { "CustomerName": tempCustomer, "ProjectName": tempProject, "ProductPhaseName": tempPhaseName, "PartTypesName": tempPartTypes, "myDate": myDate }, function (data) {
        if (data != "") {
            $.each(data, function (i, item) {

                $("<option></option>")
                    .val(item)
                    .text(item)
                    .appendTo($("#js_s_input_week_verion"));
            });
        }
    });
}


///获取月版本号
function GetMonthVersion() {
    $("#js_s_input_month_verion").empty();
    $("#js_s_input_month_verion").val('');
    var tempCustomer = $("#js_s_input_customer").val();
    var tempProject = $("#js_s_input_project").val();
    //if (tempProject.length > 15) {
    //    tempProject = tempProject.substr(0, 15);
    //}
    var tempPhaseName = $("#js_s_input_Product_Phase").val();
    var tempPartTypes = $("#js_s_input_part_types").val();

    var url = '../EventReportManager/GetVersionSource';

    //查看月所在日期是否有指定输入日期，如果有则以输入的日期为标准查版本，如果没有则以当天的日期为当前月查版本
    var myDate;

    
    if ($("#js_s_begin_date_month").val() == '') {
        //输入的日期为空则默认为当天日期
        myDate = new Date();
    }
    else {
        //以输入的日期为标准查版本
        myDate = new Date($("#js_s_begin_date_month").val());
    }

    var startDay = getCurrentMonthFirst(myDate);
    var endDay = getCurrentMonthLast(myDate);
    var currentDay = myDate.getFullYear() + "-" + (myDate.getMonth() + 1) + "-" + myDate.getDate();
    var myDateStart = startDay.getFullYear() + "-" + (startDay.getMonth() + 1) + "-" + startDay.getDate();
    var myDateEnd = endDay.getFullYear() + "-" + (endDay.getMonth() + 1) + "-" + endDay.getDate();

    //$('#Interval_Date_Start').val(myDateStart);
    //$('#Interval_Date_End').val(myDateEnd);
    $('#js_s_begin_date_month').val(currentDay);
    $('#hidMonth_Date_End').val(myDateEnd);
    $("#js_s_input_mounth_interval").val("从： " + myDateStart + "  到：" + myDateEnd);
    //GetMonthVersion("js_s_input_month_verion", myDateStart, myDateEnd);
    $("#interval_week_date").text($("#js_s_input_mounth_interval").val());



    $.post(url, { "CustomerName": tempCustomer, "ProjectName": tempProject, "ProductPhaseName": tempPhaseName, "PartTypesName": tempPartTypes, "beginTime": myDateStart, "endTime": myDateEnd }, function (data) {
        if (data != "") {
            $.each(data, function (i, item) {

                $("<option></option>")
                    .val(item)
                    .text(item)
                    .appendTo($("#js_s_input_month_verion"));
            });


            var Version = $("#js_s_input_month_verion").val();
            var urlTwo = '../EventReportManager/GetVersionBeginEndDate';
            $.post(urlTwo, { "CustomerName": tempCustomer, "ProjectName": tempProject, "ProductPhaseName": tempPhaseName, "PartTypesName": tempPartTypes, "Version": Version, "startDay": myDateStart, "endDay": myDateEnd }, function (data) {
                //查询生产到最后一天的日期加载
                $("#interval_week_date").text(data.Interval);

            });
        }
    });
}

///获取时间段版本号
function GetTimeSpanVersion() {
    $("#js_s_verion_interval").empty();
    $("#js_s_verion_interval").val('');

    var tempCustomer = $("#js_s_input_customer").val();
    var tempProject = $("#js_s_input_project").val();
    //if (tempProject.length > 15) {
    //    tempProject = tempProject.substr(0, 15);
    //}
    var tempPhaseName = $("#js_s_input_Product_Phase").val();
    var tempPartTypes = $("#js_s_input_part_types").val();
    var startDay = $('#Interval_Date_Start').val();
    var endDay = $('#Interval_Date_End').val();
    if (startDay != '' && endDay != '') {
        var url = '../EventReportManager/GetVersionSource';
        $.post(url, { "CustomerName": tempCustomer, "ProjectName": tempProject, "ProductPhaseName": tempPhaseName, "PartTypesName": tempPartTypes, "beginTime": startDay, "endTime": endDay }, function (data) {
            if (data != "") {
                $.each(data, function (i, item) {

                    $("<option></option>")
                        .val(item)
                        .text(item)
                        .appendTo($("#js_s_verion_interval"));
                });

                var Version = $("#js_s_verion_interval").val();
                var urlTwo = '../EventReportManager/GetVersionBeginEndDate';
                $.post(urlTwo, { "CustomerName": tempCustomer, "ProjectName": tempProject, "ProductPhaseName": tempPhaseName, "PartTypesName": tempPartTypes, "Version": Version, "startDay": startDay, "endDay": endDay }, function (data) {
                    $("#interval_week_date").text(data.Interval);
                });
            }
        });
    }
}





function GetIntervel(myDate) {
    var url = '../EventReportManager/GetDateTime';
    //$.post(url, { "mydate": myDate }, function (data) {
    //    $("#js_s_input_week_interval").val(data);
    //});
    $.ajax({
        url: url,
        async: false,
        data: { "mydate": myDate },
        success: function (data) {
            if (data != "") {
                $("#js_s_input_week_interval").val(data);
            }
        }
    });
};

//比较日期大小
function validTime(startTime, endTime, param) {
    var arr1 = startTime.split("-");
    var arr2 = endTime.split("-");
    var date1 = new Date(parseInt(arr1[0]), parseInt(arr1[1]) - 1, parseInt(arr1[2]), 0, 0, 0);
    var date2 = new Date(parseInt(arr2[0]), parseInt(arr2[1]) - 1, parseInt(arr2[2]), 0, 0, 0);
    if (date1.getTime() > date2.getTime()) {
        alert('结束日期不能小于开始日期', this);
        if (param == 1) {
            $("#Interval_Date_Start").val("");
        }
        else {
            $("#Interval_Date_End").val("");
        }
        return false;
    } else {
        return true;
    }
    return false;
}

/* 获取当前月的第一天*/
function getCurrentMonthFirst(date) {

    date.setDate(1);
    return date;
}

/* 获取当前月的最后一天*/
function getCurrentMonthLast(date) {

    var currentMonth = date.getMonth();
    var nextMonth = ++currentMonth;
    var nextMonthFirstDay = new Date(date.getFullYear(), nextMonth, 1);
    var oneDay = 1000 * 60 * 60 * 24;
    return new Date(nextMonthFirstDay - oneDay);
}

//清空联动下拉框的值
function ClearDropDownListValue(value) {
    switch (value) {
        case 'js_s_input_customer':
            //清空project的值
            $("#js_s_input_project").empty();
            $("#js_s_input_project").val('');
            //清空生产阶段的值
            $("#js_s_input_Product_Phase").empty();
            $("#js_s_input_Product_Phase").val('');
            //清空部件类型的值
            $("#js_s_input_part_types").empty();
            $("#js_s_input_part_types").val('');
            //清空颜色的值
            $("#js_s_input_color").empty();
            $("#js_s_input_color").val('');
            //清空日期的值
            //$('#js_s_input_date').val('');
            //清空版本的值
            $('#input_day_verion').empty();
            $('#input_day_verion').val('');
            $('#js_s_input_week_verion').empty();
            $('#js_s_input_week_verion').val();
            $('#js_s_input_month_verion').empty();
            $('#js_s_input_month_verion').val();
            break;
        case 'js_s_input_project':
            //清空生产阶段的值
            $("#js_s_input_Product_Phase").empty();
            $("#js_s_input_Product_Phase").val('');
            //清空部件类型的值
            $("#js_s_input_part_types").empty();
            $("#js_s_input_part_types").val('');
            //清空颜色的值
            $("#js_s_input_color").empty();
            $("#js_s_input_color").val('');
            //清空日期的值
            //$('#js_s_input_date').val('');
            //清空版本的值
            $('#input_day_verion').empty();
            $('#input_day_verion').val('');
            $('#js_s_input_week_verion').empty();
            $('#js_s_input_week_verion').val();
            $('#js_s_input_month_verion').empty();
            $('#js_s_input_month_verion').val();
            break;
        case 'js_s_input_Product_Phase':
            //清空部件类型的值
            $("#js_s_input_part_types").empty();
            $("#js_s_input_part_types").val('');
            //清空颜色的值
            $("#js_s_input_color").empty();
            $("#js_s_input_color").val('');
            //清空日期的值
            //$('#js_s_input_date').val('');
            //清空版本的值
            $('#input_day_verion').empty();
            $('#input_day_verion').val('');
            $('#js_s_input_week_verion').empty();
            $('#js_s_input_week_verion').val();
            $('#js_s_input_month_verion').empty();
            $('#js_s_input_month_verion').val();
            break;
        case 'js_s_input_part_types':
            //清空颜色的值
            $("#js_s_input_color").empty();
            $("#js_s_input_color").val('');
            //清空日期的值
            //$('#js_s_input_date').val('');
            //清空版本的值
            $('#input_day_verion').empty();
            $('#input_day_verion').val('');
            $('#js_s_input_week_verion').empty();
            $('#js_s_input_week_verion').val();
            $('#js_s_input_month_verion').empty();
            $('#js_s_input_month_verion').val();
            break;
    }
}