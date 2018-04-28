import * as Highcharts from 'highcharts';
import DoormanMasterClient from '../services/DoormanMasterClient';
import template from './RealTimeDashboard.mustache';
import URL from 'url-parse';

require('highcharts/modules/series-label')(Highcharts);

// selectors constants
// --------------------
const CLS_DASHBOARD_BODY = '.dashboard-body';
const CLS_DASHBOARD_CHART = '.dashboard-chart';

class RealTimeDashboard {
    constructor($element, roomID, client) {
        this.$el = $element;
        this._roomID = roomID;
        this._client = client;

        this._onLoad();
    }

    _onLoad() {
        this._client.fetchRoom(this._roomID)
            .catch((e) => {
                alert(`Failed to lookup room ${this._roomID}`);
                console.log(e);
                throw e;
            })
            .then((room) => {
                this._chart = this._createChart();
                this._onRoomUpdate(room);

                this._ws = this._client.connectToWebSocket(this._roomID, (room) => this._onRoomUpdate(room));
            });
    }

    _onRoomUpdate(room) {
        this._client.fetchRecentTrendData(room.roomID)
            .then(({ points }) => {
                this._updateBody(room);
                this._updateChartPoints(points);
            });
    }

    _createChart() {
        const element = this.$el.find(CLS_DASHBOARD_CHART);
        const offset = new Date().getTimezoneOffset();

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
                minRange: 20000,
            },
            time: {
                getTimezoneOffset: () => offset
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
        this._updateChart(snapshot);
        this._updateBody(snapshot);
    }

    _renderBody(room) {
        return template({
            count: room.occupancyCount,
            timestamp: room.lastSnapshotAt
                ? new Date(room.lastSnapshotAt).toLocaleString()
                : "",
            room: room
        });
    }

    _updateBody(room) {
        const html = this._renderBody(room);

        this.$el.find(CLS_DASHBOARD_BODY).html(html);
    }

    _updateChartPoints(points) {
        const data = points
            .map(x => [
                new Date(x.timestamp).getTime(),
                x.occupancyCount
            ]);

        const series = this._chart.series[0];
        
        series.setData(data);
    }

    _updateChart({ count, timestamp }) {
        const time = timestamp.getTime();

        const series = this._chart.series[0];
        const isShift = series.data.length > 20; // shift if the series is longer than 20

        series.addPoint([time, count], true, isShift);
    }
}

RealTimeDashboard.start = function start($element) {
    const client = new DoormanMasterClient();
    const roomID = new URL(location.href, true).query.roomID;
    console.log(`Room id in RealTimeDashboard - ${roomID}`);
    const dashboard = new RealTimeDashboard($element, roomID, client);

    return dashboard;
}

export default RealTimeDashboard;