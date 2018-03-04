$(document).ready(function () {
    new realtimeChart();
});

function realtimeChart() {
    
    var getChartPoint = function () {
        $.ajax({
            url: 'API/GetRealTime',
            type: 'get',
            success: function (resp) {
                $('.occupancy-size').html(resp.val);
                var point = parseInt(resp.val);
                
                var series = chart.series[0];
                var shift = series.data.length > 20; // shift if the series is longer than 20

                // add the point
                var d = new Date();
                var x = d.getTime();
                var y = point;
                chart.series[0].addPoint([x, y], true, shift);

                $('.occupancy-time').html(d.toLocaleString());

                // call it again after 3 seconds
                setTimeout(getChartPoint, 3000);
            },
            cache: false,
            failure: function (err) {
                console.log(err);
            }
        });
    };
    
    var chart = new Highcharts.Chart({
        chart: {
            renderTo: 'realtime-chart',
            defaultSeriesType: 'spline',
            events: {
                load: getChartPoint
            }
        },
        title: {
            text: 'Realtime Occupancy'
        },
        xAxis: {
            type: 'datetime',
            tickPixelInterval: 150,
            maxZoom: 20 * 1000
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
            data: []
        }]
    });
}
