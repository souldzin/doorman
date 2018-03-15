$(document).ready(function () {
    var _ho = new historicOps();
    var firstTitle = 'Daily Average Occupancy';
    _ho.getHistoricData(firstTitle); //First load
    $(document).on('click', '#filter', function () {
        _ho.getHistoricData(firstTitle);
    });
    $(document).on('click', '.historic-chart-btn', function () {
        var title = $(this).attr('data-title');
        _ho.getHistoricData(title);
    });
});

function historicOps() {
    
    this.getHistoricData = function (title) {
        $.ajax({
            url: '../../API/GetHistoricModel',
            data: {
                StartDate: '12/01/2018',
                EndDate: '',
                ChartType:'1'
            },
            type: 'post',
            success: function (resp) {
                $('.historic-data-box').html(resp.page);
                chart(resp.data, resp.categories, title)
            },
            cache: false,
            failure: function (err) {
                console.log(err);
            }
        });
    };
    
    var chart = function (data, categories, title) {
        new Highcharts.Chart({
            chart: {
                renderTo: 'historic-chart',
                type: 'spline',
            },
            title: {
                text: title
            },
            xAxis: {
                categories: categories
            },
            yAxis: {
                minPadding: 0.2,
                maxPadding: 0.2,
                title: {
                    text: 'Occupancy'
                }
            },
            series: [{
                name: 'Occupancy',
                data: data
            }]
        });
    }
}