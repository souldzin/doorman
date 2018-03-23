import RealTimeDashboard from './modules/dashboards/RealTimeDashboard';
import HistoricDashboard from './modules/dashboards/HistoricDashboard';
import setupSelectRoomModal from './modules/select-room/select-room-modal';

function start() {
    setupSelectRoomModal();
}

global.Doorman = {};
global.Doorman.UI = {
    start,
    RealTimeDashboard,
    HistoricDashboard
};
