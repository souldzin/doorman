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
            .then((room) => {
                this._room = room;
                this._chart = this._createChart();
                this._updateBody(room);
            });
    }

    _createChart() {
        const element = this.$el.find(CLS_DASHBOARD_CHART);

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
                minRange: 20000
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

    _renderBody(snapshot) {
        return template({
            count: snapshot.count,
            timestamp: snapshot.timestamp.toLocaleString(),
            room: "EMSE Room (ID: 12)"
        });
    }

    _updateBody(snapshot) {
        const html = this._renderBody(snapshot);

        this.$el.find(CLS_DASHBOARD_BODY).html(html);
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
    const roomID = new URL(location.href).query.roomID;
    const dashboard = new RealTimeDashboard($element, roomID, client);

    return dashboard;
}

export default RealTimeDashboard;