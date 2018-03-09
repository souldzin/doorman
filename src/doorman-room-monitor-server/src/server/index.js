const express = require('express');
const bodyParser = require('body-parser');
const http = require('http');
const socket = require('socket.io');
const { RoomMonitor } = require('../monitor');
const routes = {
    sensor: require('./routes/sensor')
};

main();

function main() {
    const port = Number(process.argv[2] || "9000")
    const app = express();
    const server = http.Server(app);
    const io = socket(server);
    const monitor = new RoomMonitor();

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

    // setup context 
    //   - this is used to inject dependencies to services
    const ctx = {
        monitor,
        io
    };

    // setup express app
    //  
    app.use(bodyParser.json());
    app.use('/sensor', routes.sensor(ctx));

    // start the server already!
    //
    server.listen(port, () => {
        console.log(`doorman-room-monitor listening on port ${port}.`);
    });
}
