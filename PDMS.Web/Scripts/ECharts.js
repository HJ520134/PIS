

//產製折線圖
function EChart_Line(DivID, TitleDt, ItemDt, xAxisDt, SeriesDt) {
    var myChart = echarts.init(document.getElementById(DivID), "macarons"); //default,macarons,infographic

    option = {
        title: {
            text: TitleDt
        },
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            data: ItemDt
        },
        toolbox: {
            show: true,
            feature: {
                mark: { show: false },
                dataView: { show: false, readOnly: false },
                magicType: { show: true, type: ['line', 'bar'] },
                restore: { show: false },
                saveAsImage: { show: true }
            }
        },
        calculable: true,
        xAxis: [
            {
                type: 'category',
                boundaryGap: false,
                data: xAxisDt
            }
        ],
        yAxis: [
            {
                type: 'value',
                axisLabel: {
                    formatter: '{value} %'
                }
            }
        ],
        series: SeriesDt
    };
    myChart.setOption(option);
}

