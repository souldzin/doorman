import * as Highcharts from 'highcharts';
import DoormanMasterClient from '../services/DoormanMasterClient';

require('highcharts/modules/series-label')(Highcharts);

// selectors constants
// --------------------
const CLS_OCCUPANCY_COUNT = '.occupancy-count';
const CLS_OCCUPANCY_TIME = '.occupancy-time';
const CLS_OCCUPANCY_CHART = '.occupancy-chart';

class RealTimeDashboard {
    constructor($el, client) {
        this.$el = $el;
        this._client = client;
    }

    onLoad() {
        this._chart = this._createChart();

        this._generate();
        
        this._interval = setInterval(() => {
            this._generate();
        }, 3000);
    }

    _createChart() {
        const element = this.$el.find(CLS_OCCUPANCY_CHART);

        return new Highcharts.Chart({
            chart: {
                renderTo: element[0],
                defaultSeriesType: 'spline',
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

    _generate() {
        const nextSnapshot = {
            count: Math.floor(Math.random() * 10 + 10),
            timestamp: new Date()
        };

        this._updateWithSnapshot(nextSnapshot);
    }

    _updateWithSnapshot(snapshot) {
        this._updateCount(snapshot.count);
        this._updateTime(snapshot.timestamp);
        this._updateChart(snapshot);
    }

    _updateCount(count) {
        const element = this.$el.find(CLS_OCCUPANCY_COUNT);

        element.text(count);
    }

    _updateTime(time) {
        const element = this.$el.find(CLS_OCCUPANCY_TIME);

        element.text(time.toLocaleString());
    }

    _updateChart({ count, timestamp }) {
        const time = timestamp.getTime();

        const series = this._chart.series[0];
        const isShift = series.data.length > 20; // shift if the series is longer than 20

        series.addPoint([time, count], true, isShift);
    }
}

RealTimeDashboard.start = function start($el) {
    const client = new DoormanMasterClient();
    const dashboard = new RealTimeDashboard($el, client);

    dashboard.onLoad();

    return dashboard;
}

export default RealTimeDashboard;