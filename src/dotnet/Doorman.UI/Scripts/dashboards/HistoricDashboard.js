import BaseDashboard from './BaseDashboard';

class HistoricDashboard extends BaseDashboard {
    /**
     * This is an alias for `new HistoricDashboard`
     * @param {k} args 
     */
    static start(...args) {
        return new HistoricDashboard(...args);
    }

    constructor($el) {
        super($el);
    }

    onLoad() {
        console.log("[historic] Starting dashboard...");
    }
}

HistoricDashboard.start = function(...args) {
    return new HistoricDashboard(...args);
}

export default HistoricDashboard;