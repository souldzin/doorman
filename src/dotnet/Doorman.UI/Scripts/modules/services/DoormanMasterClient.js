import URL from 'url-parse';
import { getMasterURL } from "../environment";
import axios from "axios";
import * as signalR from '@aspnet/signalr-client';

const WEEKDAYS = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

class DoormanMasterClient {
    constructor(baseURL) {
        this._baseURL = baseURL || getMasterURL();

        this._axios = axios.create({
            baseURL: this._baseURL
        });
    }

    fetchRoom(roomId) {
        return (this._axios
            .get("/room", {
                params: {
                    roomId: roomId
                }
            })
            .then((x) => x.data)
        );
    }

    fetchRecentTrendData(roomId) {
        // ...
    }

    fetchHistoricStats(roomId, startDate, endDate) {
        return Promise.resolve({
            average: Math.round(Math.random() * 30 + 5),
            stdev: Math.round(Math.random() *10 + 4),
            max: Math.round(Math.random() * 20 + 30),
            maxDate: new Date(startDate.getTime() + Math.random() * (endDate.getTime() - startDate.getTime())),
            peakWeekday: WEEKDAYS[Math.floor(Math.random() * WEEKDAYS.length)],
            peakTime: `${Math.floor(Math.random()*24)}:${Math.floor(Math.random()*60)}`
        });
    }

    connectToWebSocket(roomId) {
        var url = this._getWebSocketURL();
        var logger = new signalR.ConsoleLogger(signalR.LogLevel.Information);
        var doormanHub = new signalR.HttpConnection(url);
        var doormanConnection = new signalR.HubConnection(doormanHub, logger);

        doormanConnection.on('Broadcast', (result) => {
            console.log(result);
        });

        return (
            doormanConnection
                .start({ transport: ['webSockets', 'serverSentEvents', 'longPolling'] })
                .catch(err => {
                    console.log(err + 'connection error');
                })
        );
    }

    _getWebSocketURL() {
        const url = new URL(this._baseURL);
        url.pathname = "/doorman";
        url.query = {};
        return url.toString();
    }
}

export default DoormanMasterClient;