const WEEKDAYS = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

class DoormanMasterClient {

    getRoom(roomId) {
        // ... return AJAX request to server
    }

    getRecentTrendData(roomId) {
        // ...
    }

    connectToWebSocket(roomId) {
        // ...
    }

    getHistoricStats(roomId, startDate, endDate) {
        return Promise.resolve({
            average: Math.round(Math.random() * 30 + 5),
            stdev: Math.round(Math.random() *10 + 4),
            max: Math.round(Math.random() * 20 + 30),
            maxDate: new Date(startDate.getTime() + Math.random() * (endDate.getTime() - startDate.getTime())),
            peakWeekday: WEEKDAYS[Math.floor(Math.random() * WEEKDAYS.length)],
            peakTime: `${Math.floor(Math.random()*24)}:${Math.floor(Math.random()*60)}`
        });
    }
}

export default DoormanMasterClient;