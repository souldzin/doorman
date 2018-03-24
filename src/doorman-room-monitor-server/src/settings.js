const fs = require('fs');
const path = require('path');
const DoormanMasterClient = require('./services/DoormanMasterClient');

const VAR_DOORMAN_ROOM_MONITOR_CONFIG = "DOORMAN_ROOM_MONITOR_CONFIG";
const DEFAULT_CONFIG_FILE_PATH = "./config/doorman.config.json";

function getConfigFilePath() {
    return process.env[VAR_DOORMAN_ROOM_MONITOR_CONFIG] || path.resolve(DEFAULT_CONFIG_FILE_PATH);
}

function readConfigFile() {
    const filePath = getConfigFilePath();
    console.log(`Reading settings from file ${filePath}`);

    const txt = fs.readFileSync(filePath);
    const obj = JSON.parse(txt);

    console.log(obj);

    return obj;
}

function saveConfigFile(settings) {
    const filePath = getConfigFilePath();
    console.log(`Saving settings to file ${filePath}`)

    const txt = JSON.stringify(settings, null, 4);
    fs.writeFileSync(filePath, txt);

    return settings;
}

function injectRoomId(settings) {
    if(settings.roomID) {
        return Promise.resolve(settings);
    }

    console.log(`"roomID" not found in settings. Fetching from server.`);

    const client = new DoormanMasterClient(settings.masterURL);
    const name = settings.roomName || `New Room (${new Date()})`;

    return client
        .postRoom(settings.clientId, settings.clientSecret, name)
        .then(room => {
            return saveConfigFile({
                ...settings,
                roomID: room.roomID
            });
        });
}

function injectPort(settings) {
    return Object.assign({}, settings, {
        port: Number(settings.port || process.argv[2] || 9000)
    });
}

function load() {
    const settings = readConfigFile();

    return Promise.resolve(settings)
        .then(injectRoomId)
        .then(injectPort);
}

module.exports = {
    load
};