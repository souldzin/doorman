class DoormanMasterClientFake {
    constructor() {
        this._rooms = {};
    }

    postRoom(clientId, clientSecret, roomName) {
        return Promise.resolve({
            roomId: 0,
            roomName: roomName,
            occupancyCount: 0,
            accessToken: ""
        });
    }

    postRoomOccupancySnapshot(roomId, count) {
        this._rooms[roomId] = count;

        return Promise.resolve({});
    }

    getRoom(roomId) {
        return Promise.resolve({
            roomId: roomId,
            occupancyCount: this._rooms[roomId] || 0
        });
    }
}

module.exports = DoormanMasterClientFake;