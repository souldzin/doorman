const fs = require('fs');
const path = require('path');
const DoormanMasterClient = require('./services/DoormanMasterClient');
const DoormanMasterClientFake = require('./services/DoormanMasterClientFake');
const { args, ARG_MASTER } = require('./settings-args');

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

function updateConfigFile(ctx, configSettings) {
    // are we running without a master?
    if(!ctx.settings[ARG_MASTER]) {
        return;
    }
    // is the conifg already up to date?
    if(ctx.settings.roomID === configSettings.roomID) {
        return;
    }

    const newConfigSettings = {
        ...configSettings,
        roomID: ctx.settings.roomID
    };

    return saveConfigFile(newConfigSettings);
}

function getClient({ settings }) {
    return !settings[ARG_MASTER]
        ? new DoormanMasterClientFake()
        : new DoormanMasterClient(settings.masterURL);
}


function getRoomId({ settings, client }) {
    if(settings.roomID) {
        return settings.roomID;
    } else if(!settings[ARG_MASTER]) {
        return 0;
    } 

    console.log(`"roomID" not found in settings. Fetching from server.`);

    const name = settings.roomName || `New Room (${new Date()})`;

    return client
        .postRoom(settings.clientId, settings.clientSecret, name)
        .then(room => room.roomID);
}

function load() {
    const configSettings = readConfigFile();
    const settings = Object.assign({}, configSettings, args);
    const ctx = {
        settings
    };

    return Promise.resolve(ctx)
        .then((ctx) => Promise.resolve(getClient(ctx))
            .then(client => ({ 
                ...ctx,
                client
            }))
        )
        .then((ctx) => Promise.resolve(getRoomId(ctx))
            .then(roomID => ({
                ...ctx,
                settings: {
                    ...ctx.settings,
                    roomID
                }
            }))
        )
        .then((ctx) => Promise.resolve(updateConfigFile(ctx, configSettings))
            .then(() => ctx)
        );
}

module.exports = {
    load
};