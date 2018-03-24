const express = require('express');
const bodyParser = require('body-parser');
const http = require('http');
const socket = require('socket.io');
const { RoomMonitor } = require('../monitor');
const Settings = require('../settings');
const DoormanMasterClient = require('../services/DoormanMasterClient');
const routes = {
    sensor: require('./routes/sensor')
};

Settings.load()
    .then(setup)
    .then(start);

function setupClient(ctx) {
    const {settings} = ctx;

    const client = new DoormanMasterClient(settings.masterURL);

    return {
        ...ctx,
        client
    };
}

function setupRoomMonitor(ctx) {
    const {client, settings} = ctx;

    return client.getRoom(settings.roomID)
        .then(room => {
            const monitor = new RoomMonitor(room);

            monitor.state$.subscribe((state) => {
                client.postRoomOccupancySnapshot(settings.roomID, state.count);
            });

            return {
                ...ctx,
                monitor                
            };
        });
}

function setup(settings) {
    return Promise.resolve({ settings })
        .then(setupClient)
        .then(setupRoomMonitor);
}

function start(ctx) {
    const { client, settings, monitor } = ctx;

    const app = express();
    const server = http.Server(app);
    const io = socket(server);

    ctx = {
        ...ctx,
        io
    };

    // setup socket.io
    // 
    io.on('connection', (socket) => { 
        socket.on('ready', function(){
            socket.emit('state', ctx.monitor.getState());
        });
    });

    monitor.state$.subscribe((state) => {
        io.emit('state', state);
    });

    // setup express app
    //  
    app.use(bodyParser.json());
    app.use('/sensor', routes.sensor(ctx));

    // start the server already!
    //
    server.listen(settings.port, () => {
        console.log(`doorman-room-monitor listening on port ${settings.port}.`);
    });
}
