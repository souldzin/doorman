import * as Highcharts from 'highcharts';
import DoormanMasterClient from '../services/DoormanMasterClient';
import template from './HistoricDashboard.mustache';

require('highcharts/modules/series-label')(Highcharts);

// selectors constants
// --------------------
const CLS_OCCUPANCY_FILTER_START = '.occupancy-filter-start';
const CLS_OCCUPANCY_FILTER_END = '.occupancy-filter-end';
const CLS_OCCUPANCY_FILTER_BUTTON = '.occupancy-filter-btn';
const CLS_OCCUPANCY_STATS = '.occupancy-stats-view';
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
    constructor($el, client) {
        this.$el = $el;
        this._client = client;
    }

    onLoad() {
        const filterBtn = this.$el.find(CLS_OCCUPANCY_FILTER_BUTTON);

        this._renderStatsView(null);

        filterBtn.on('click', () => {
            this._onFilter();
        });
    }

    _onFilter() {
        const start = this._getFilterStart();
        const end = this._getFilterEnd();

        if(!start || !end) {
            alert("Please enter a start and end date");
            return;
        }

        this._client.getHistoricStats(0, start, end)
            .then((x) => {
                return toStringObjectValues(x);
            })
            .then((x) => {
                this._renderStatsView(x)
            });
    }

    _renderStatsView(stats) {
        const html = template(stats || {});

        const element = this.$el.find(CLS_OCCUPANCY_STATS);

        element.html(html);
    }

    _getFilterStart() {
        const element = this.$el.find(CLS_OCCUPANCY_FILTER_START);

        return getDateValue(element);
    }

    _getFilterEnd() {
        const element = this.$el.find(CLS_OCCUPANCY_FILTER_END);

        return getDateValue(element);
    }

    _createChart(categories, data) {
        const element = this.$el.find(CLS_OCCUPANCY_CHART);

        return new Highcharts.Chart({
            chart: {
                renderTo: element[0],
                type: 'spline',
            },
            title: {
                text: 'Historic Occupancy Trend'
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

HistoricDashboard.start = function start($el) {
    const client = new DoormanMasterClient();
    const dashboard = new HistoricDashboard($el, client);

    dashboard.onLoad();

    return dashboard;
}

export default HistoricDashboard;
