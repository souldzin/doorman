import BaseDashboard from './BaseDashboard';
import * as Highcharts from 'highcharts';

require('highcharts/modules/series-label')(Highcharts);

class RealTimeDashboard extends BaseDashboard {
    /**
     * This is an alias for `new RealTimeDashboard`
     * @param {k} args 
     */
    static start(...args) {
        return new RealTimeDashboard(...args);
    }

    constructor($el) {
        super($el);
    }

    onLoad() {
        console.log("[realtime] Starting dashboard...");
        
        this._chart = this._createChart();

        this._generate();
        
        this._interval = setInterval(() => {
            this._generate();
        }, 3000);
    }

    _getOccupancyCountElement() {
        return this.$el.find('.occupancy-count');
    }

    _getOccupancyTimeElement() {
        return this.$el.find('.occupancy-time');
    }

    _getOccupancyChartElement() {
        return this.$el.find('.occupancy-chart');
    }

    _createChart() {
        const element = this._getOccupancyChartElement()[0];

        return new Highcharts.Chart({
            chart: {
                renderTo: element,
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
        this._getOccupancyCountElement().text(count);
    }

    _updateTime(time) {
        this._getOccupancyTimeElement().text(time.toLocaleString());
    }

    _updateChart({ count, timestamp }) {
        const time = timestamp.getTime();

        const series = this._chart.series[0];
        const isShift = series.data.length > 20; // shift if the series is longer than 20

        series.addPoint([time, count], true, isShift);
    }
}

export default RealTimeDashboard;