import URL from 'url-parse';
import { getMasterURL } from "../environment";
import axios from "axios";

const WEEKDAYS = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

class DoormanMasterClient {
    constructor(baseURL) {
        baseURL = baseURL || getMasterURL();

        console.log(`base url is ${baseURL}`); 
        
        this._axios = axios.create({
            baseURL: baseURL
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
}

export default DoormanMasterClient;