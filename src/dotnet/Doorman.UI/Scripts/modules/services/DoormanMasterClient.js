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
        return (this._axios
            .get(`/room/${roomId}/recentTrend`, {
                params: {
                    seconds: 3600 // 1 hour
                }
            })
            .then((x) => x.data)
        )        
    }

    fetchHistoricTrendData(roomId, startDate, endDate) {
        return (this._axios
            .get(`/room/${roomId}/historicTrend`, {
                params: {
                    start: startDate,
                    end: endDate
                }
            })
            .then((x) => x.data)
        );
    }

    fetchHistoricStats(roomId, startDate, endDate) {
        return (this._axios
            .get(`/room/${roomId}/stats`, {
                params: {
                    start: startDate,
                    end: endDate
                }
            })
            .then((x) => x.data)
        );
    }

    connectToWebSocket(roomId, callback) {
        var url = this._getWebSocketURL();
        var logger = new signalR.ConsoleLogger(signalR.LogLevel.Information);
        var doormanHub = new signalR.HttpConnection(url);
        var doormanConnection = new signalR.HubConnection(doormanHub, logger);

        doormanConnection.on(roomId, callback);

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