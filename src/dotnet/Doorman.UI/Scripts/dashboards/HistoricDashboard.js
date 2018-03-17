import BaseDashboard from './BaseDashboard';

class HistoricDashboard extends BaseDashboard {
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