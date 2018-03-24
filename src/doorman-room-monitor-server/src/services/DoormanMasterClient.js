const axios = require('axios');

class DoormanMasterClient {
    constructor(baseURL) {
        this._axios = axios.create({
            baseURL: baseURL
        });
    }

    postRoom(clientId, clientSecret, roomName) {
        return this._axios
            .request(`/room`, {
                method: "POST",
                params: {
                    name: roomName
                },
                data: {
                    "client_id": clientId,
                    "client_secret": clientSecret
                }
            })
            .then(x => x.data);
    }

    postRoomOccupancySnapshot(roomId, count) {
        return this._axios
            .request(`/room/${roomId}/snapshot`, {
                method: "POST",
                data: {
                    occupancyCount: count
                }
            })
            .then(x => x.data);
    }

    getRoom(roomId) {
        return this._axios
            .request(`/room/${roomId}`, {
                method: "GET",
            })
            .then(x => x.data);
    }
}

module.exports = DoormanMasterClient;