import BaseDashboard from './BaseDashboard';
import * as DoormanMasterClient from '../common/DoormanMasterClient';

function renderStatsView(stats) {
    return (
`<div>
    <span class="data-label">Occupancy Average:</span>
    <span class="data-general">${stats.average || ""}</span>
</div>
<div>
    <span class="data-label">Occupancy StdDev:</span>
    <span class="data-general">${stats.stdev || ""}</span>
</div>
<div>
    <span class="data-label">Max Occupancy:</span>
    <span class="data-general">${stats.max || ""}</span>
</div>
<div>
    <span class="data-label">Max Occupancy Date:</span>
    <span class="data-general">${stats.maxDate || ""}</span>
</div>
<div>
    <span class="data-label">Peak Weekday:</span>
    <span class="data-general">${stats.peakWeekday || ""}</span>
</div>
<div>
    <span class="data-label">Peak Time:</span>
    <span class="data-general">${stats.peakTime || ""}</span>
</div>`
    );
}

class HistoricDashboard extends BaseDashboard {
    /**
     * This is an alias for `new HistoricDashboard`
     * @param {k} args 
     */
    static start(...args) {
        debugger;
        return new HistoricDashboard(...args);
    }

    constructor($el) {
        super($el);
    }

    onLoad() {
        const filterBtn = this._getFilterButtonElement();

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

        DoormanMasterClient.getHistoricStats(0, start, end)
            .then((x) => {
                for(var key in x) {
                    x[key] = x[key].toLocaleString
                        ? x[key].toLocaleString()
                        : x[key].toString();
                }

                return x;
            })
            .then((x) => {
                this._renderStatsView(x)
            });
    }

    _renderStatsView(stats) {
        const element = this._getStatsViewElement();

        console.log(element);

        element.html(renderStatsView(stats || {}));
    }

    _getFilterStart() {
        const startVal = this._getFilterStartElement().val();
        const startTime = Date.parse(startVal);
        return Number.isNaN(startTime) ? null : new Date(startTime);
    }

    _getFilterEnd() {
        const val = this._getFilterEndElement().val();
        const time = Date.parse(val);
        return Number.isNaN(time) ? null : new Date(time);
    }

    _getFilterStartElement() {
        return this.$el.find('.occupancy-filter-start');
    }

    _getFilterEndElement() {
        return this.$el.find('.occupancy-filter-end');
    }

    _getFilterButtonElement() {
        return this.$el.find('.occupancy-filter-btn');
    }

    _getOccupancyChartElement() {
        return this.$el.find('.occupancy-chart');        
    }

    _getStatsViewElement() {
        return this.$el.find('.occupancy-stats-view');
    }

    _createChart(categories, data) {
        const element = this._getOccupancyChartElement()[0];

        return new Highcharts.Chart({
            chart: {
                renderTo: element,
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

HistoricDashboard.start = function(...args) {
    return new HistoricDashboard(...args);
}

export default HistoricDashboard;
