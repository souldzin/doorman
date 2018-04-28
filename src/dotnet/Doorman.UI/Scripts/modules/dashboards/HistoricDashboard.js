import * as Highcharts from 'highcharts';
import DoormanMasterClient from '../services/DoormanMasterClient';
import statsTemplate from './HistoricDashboardStats.mustache';
import headerTemplate from './HistoricDashboardHeader.mustache';
import URL from 'url-parse';
import * as moment from 'moment';

require('highcharts/modules/series-label')(Highcharts);

function formatDate(dt) {
    return dt.toISOString().replace(/T/, ' ').replace(/\..+/, ' ');
}

// selectors constants
// --------------------
const CLS_OCCUPANCY_FILTER_START = '.occupancy-filter-start';
const CLS_OCCUPANCY_FILTER_END = '.occupancy-filter-end';
const CLS_OCCUPANCY_FILTER_BUTTON = '.occupancy-filter-btn';
const CLS_OCCUPANCY_STATS = '.occupancy-stats-view';
const CLS_OCCUPANCY_HEADER = '.occupancy-header-view';
const CLS_OCCUPANCY_CHART = '.occupancy-chart';

// util functions
// ---------------------
function toStringObjectValues(obj) {
    for(var key in obj) {
        const value = obj[key];
        const strValue = value.toLocaleString
            ? value.toLocaleString()
            : value.toString();
        
        obj[key] = strValue;
    }

    return obj;
}

function getDateValue(element) {
    const val = element.val();
    const time = Date.parse(val);
    return Number.isNaN(time) ? null : new Date(time);
}

// HistoricDashboard
// --------------------
class HistoricDashboard {
    constructor($element, roomID, client) {
        this.$el = $element;
        this._roomID = roomID;
        this._client = client;

        this._onLoad();
    }

    _onLoad() {
        this._chart = this._createChart();

        this._client.fetchRoom(this._roomID)
            .catch((e) => {
                alert(`Failed to lookup room ${this._roomID}`);
                console.log(e);
                throw e;
            })
            .then((room) => {
                const filterBtn = this.$el.find(CLS_OCCUPANCY_FILTER_BUTTON);

                filterBtn.on('click', () => {
                    this._onFilter();
                });

                this._renderHeaderView(room);
            });
    }

    _onFilter() {
        const start = this._getFilterStart();
        const end = this._getFilterEnd();

        if(!start || !end) {
            alert("Please enter a start and end date");
            return;
        }

        this._client.fetchHistoricStats(this._roomID, start, end)
            .then((x) => {
                return toStringObjectValues(x);
            })
            .then((x) => {
                this._renderStatsView(x)
            });

        this._client.fetchHistoricTrendData(this._roomID, start, end)
            .then((x) => {
                this._renderChartView(x.points || []);
            });
    }

    _renderHeaderView(room) {
        const html = headerTemplate(room || {});

        const element = this.$el.find(CLS_OCCUPANCY_HEADER);

        element.html(html);
    }

    _renderStatsView(stats) {
        stats = {
            ...stats,
            maxDate: moment(stats.maxDate).format('YYYY-MM-DD HH:mm')
        };

        const html = statsTemplate(stats || {});

        const element = this.$el.find(CLS_OCCUPANCY_STATS);

        element.html(html);
    }

    _renderChartView(data) {
        this._updateChartPoints(data);
    }

    _getFilterStart() {
        const element = this.$el.find(CLS_OCCUPANCY_FILTER_START);

        return getDateValue(element);
    }

    _getFilterEnd() {
        const element = this.$el.find(CLS_OCCUPANCY_FILTER_END);

        return getDateValue(element);
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

    _createChart() {
        const element = this.$el.find(CLS_OCCUPANCY_CHART);
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
}

HistoricDashboard.start = function start($el) {
    const client = new DoormanMasterClient();
    const roomID = new URL(location.href, true).query.roomID;
    console.log(`Room id in HistoricDashboard - ${roomID}`);
    const dashboard = new HistoricDashboard($el, roomID, client);

    return dashboard;
}

export default HistoricDashboard;
